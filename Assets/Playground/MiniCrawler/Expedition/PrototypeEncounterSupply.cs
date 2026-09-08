using System;
using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Expedition
{
    [DefaultExecutionOrder(-425)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StageDirector), typeof(EncounterDirectionController), typeof(QuickEncounterChoices))]
    public sealed class PrototypeEncounterSupply : MonoBehaviour
    {
        private sealed class RuntimeState
        {
            public LevelEncounter Encounter;
            public long ObservedAvailabilitySequence;
            public int Occurrence;

            public bool HasExpiry;
            public WorldTimestamp ExpiresAt;

            public bool WaitingForReuse;
            public WorldTimestamp ReuseAt;

            public RuntimeState(LevelEncounter encounter)
            {
                Encounter = encounter;
            }
        }

        public static PrototypeEncounterSupply Instance { get; private set; }

        [Header("Availability")]
        [SerializeField, Min(1f)] private float encounterLifetimeWorldMinutes = 60f;
        [SerializeField, Min(0f)] private float initialExpiryStaggerWorldMinutes = 15f;

        [Header("Backfill")]
        [SerializeField, Min(0f)] private float reuseDelayWorldMinutes = 20f;
        [SerializeField, Min(3)] private int minimumQuickChoices = 3;

        private readonly List<RuntimeState> states = new();
        private readonly List<RuntimeState> initialAvailable = new();

        private StageDirector stageDirector;
        private EncounterDirectionController directionController;
        private QuickEncounterChoices quickChoices;

        private bool initializedForLevel;

        public event Action SupplyChanged;

        public float EncounterLifetimeWorldMinutes => encounterLifetimeWorldMinutes;
        public float ReuseDelayWorldMinutes => reuseDelayWorldMinutes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            stageDirector = GetComponent<StageDirector>();
            directionController = GetComponent<EncounterDirectionController>();
            quickChoices = GetComponent<QuickEncounterChoices>();
        }

        private void Update()
        {
            if (stageDirector == null || stageDirector.State != StageDirector.LevelState.FightingMinions)
            {
                if (initializedForLevel)
                    ResetRuntime();

                return;
            }

            WorldClockState worldClock = RunProgress.WorldClock;

            if (worldClock == null || !worldClock.IsInitialized)
                return;

            WorldTimestamp currentTime = worldClock.CurrentTime;

            if (!initializedForLevel)
                InitializeForLevel(currentTime);

            bool changed = false;

            changed |= SyncEncounterStates(currentTime);
            changed |= ExpireDueEncounters(currentTime);
            changed |= SyncEncounterStates(currentTime);
            changed |= RearmNaturallyDueEncounters(currentTime);
            changed |= EnsureMinimumSupply(currentTime);

            if (changed)
                SupplyChanged?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool TryGetExpiry(LevelEncounter encounter, out WorldTimestamp expiry)
        {
            RuntimeState state = FindState(encounter);

            if (state == null || !state.HasExpiry)
            {
                expiry = default;
                return false;
            }

            expiry = state.ExpiresAt;
            return true;
        }

        public int GetOccurrence(LevelEncounter encounter)
        {
            RuntimeState state = FindState(encounter);
            return state?.Occurrence ?? 0;
        }

        public bool IsExpiryProtected(LevelEncounter encounter)
        {
            if (encounter == null)
                return false;

            if (directionController != null && directionController.SelectedEncounter == encounter)
                return true;

            return encounter.HasStartedSpawnGroups;
        }

        private void InitializeForLevel(WorldTimestamp currentTime)
        {
            states.Clear();
            initialAvailable.Clear();

            foreach (LevelEncounter encounter in stageDirector.Encounters)
            {
                if (encounter == null)
                    continue;

                RuntimeState state = new(encounter);
                states.Add(state);

                if (encounter.IsSelectable && encounter.AvailabilitySequence > 0)
                    initialAvailable.Add(state);
            }

            initialAvailable.Sort((a, b) =>
                a.Encounter.AvailabilitySequence.CompareTo(b.Encounter.AvailabilitySequence));

            for (int i = 0; i < initialAvailable.Count; i++)
            {
                float stagger = i * initialExpiryStaggerWorldMinutes;
                TrackAvailability(initialAvailable[i], currentTime, stagger);
            }

            initializedForLevel = true;
            SupplyChanged?.Invoke();
        }

        private bool SyncEncounterStates(WorldTimestamp currentTime)
        {
            bool changed = false;

            foreach (RuntimeState state in states)
            {
                LevelEncounter encounter = state.Encounter;

                if (encounter == null)
                    continue;

                if (encounter.IsSelectable &&
                    encounter.AvailabilitySequence > 0 &&
                    encounter.AvailabilitySequence != state.ObservedAvailabilitySequence)
                {
                    TrackAvailability(state, currentTime, 0f);
                    changed = true;
                }

                if (!encounter.IsCompleted && !encounter.IsExpired)
                    continue;

                if (state.HasExpiry)
                {
                    state.HasExpiry = false;
                    changed = true;
                }

                if (!encounter.ReusableByPrototypeSupply || state.WaitingForReuse)
                    continue;

                state.WaitingForReuse = true;
                state.ReuseAt = currentTime.AddMinutes(reuseDelayWorldMinutes);
                changed = true;
            }

            return changed;
        }

        private bool ExpireDueEncounters(WorldTimestamp currentTime)
        {
            bool changed = false;

            foreach (RuntimeState state in states)
            {
                LevelEncounter encounter = state.Encounter;

                if (encounter == null ||
                    !state.HasExpiry ||
                    !encounter.IsSelectable ||
                    currentTime.CompareTo(state.ExpiresAt) < 0)
                {
                    continue;
                }

                if (IsExpiryProtected(encounter))
                    continue;

                if (!encounter.Expire())
                    continue;

                state.HasExpiry = false;

                Debug.Log($"Encounter expired: {encounter.DisplayName}", this);
                changed = true;
            }

            return changed;
        }

        private bool RearmNaturallyDueEncounters(WorldTimestamp currentTime)
        {
            bool changed = false;

            foreach (RuntimeState state in states)
            {
                if (!state.WaitingForReuse || currentTime.CompareTo(state.ReuseAt) < 0)
                    continue;

                if (TryRearm(state, currentTime, forcedBackfill: false))
                    changed = true;
            }

            return changed;
        }

        private bool EnsureMinimumSupply(WorldTimestamp currentTime)
        {
            int requiredSelectable = GetRequiredSelectableCount();
            int selectableCount = CountSelectableEncounters();
            bool changed = false;

            while (selectableCount < requiredSelectable)
            {
                RuntimeState candidate = FindOldestWaitingReusable();

                if (candidate == null)
                    break;

                if (!TryRearm(candidate, currentTime, forcedBackfill: true))
                    break;

                selectableCount++;
                changed = true;
            }

            return changed;
        }

        private int GetRequiredSelectableCount()
        {
            int required = minimumQuickChoices;

            if (quickChoices != null &&
                !quickChoices.IncludeSelectedEncounter &&
                directionController != null &&
                directionController.SelectedEncounter != null &&
                directionController.SelectedEncounter.IsSelectable)
            {
                required++;
            }

            return required;
        }

        private int CountSelectableEncounters()
        {
            int count = 0;

            foreach (LevelEncounter encounter in stageDirector.Encounters)
            {
                if (encounter != null && encounter.IsSelectable)
                    count++;
            }

            return count;
        }

        private RuntimeState FindOldestWaitingReusable()
        {
            RuntimeState best = null;

            foreach (RuntimeState state in states)
            {
                if (!state.WaitingForReuse ||
                    state.Encounter == null ||
                    !state.Encounter.ReusableByPrototypeSupply)
                {
                    continue;
                }

                if (best == null || state.ReuseAt.CompareTo(best.ReuseAt) < 0)
                    best = state;
            }

            return best;
        }

        private bool TryRearm(RuntimeState state, WorldTimestamp currentTime, bool forcedBackfill)
        {
            if (state?.Encounter == null || !state.Encounter.RearmForPrototypeSupply())
                return false;

            state.WaitingForReuse = false;
            state.HasExpiry = false;

            TrackAvailability(state, currentTime, 0f);

            Debug.Log(
                forcedBackfill
                    ? $"Encounter backfilled early to maintain supply: {state.Encounter.DisplayName}"
                    : $"Encounter returned to supply: {state.Encounter.DisplayName}",
                this
            );

            return true;
        }

        private void TrackAvailability(RuntimeState state, WorldTimestamp currentTime, float extraLifetimeWorldMinutes)
        {
            state.ObservedAvailabilitySequence = state.Encounter.AvailabilitySequence;
            state.Occurrence++;
            state.WaitingForReuse = false;

            state.ExpiresAt = currentTime.AddMinutes(
                encounterLifetimeWorldMinutes + Mathf.Max(0f, extraLifetimeWorldMinutes)
            );

            state.HasExpiry = true;
        }

        private RuntimeState FindState(LevelEncounter encounter)
        {
            foreach (RuntimeState state in states)
            {
                if (state.Encounter == encounter)
                    return state;
            }

            return null;
        }

        private void ResetRuntime()
        {
            states.Clear();
            initialAvailable.Clear();
            initializedForLevel = false;

            SupplyChanged?.Invoke();
        }
    }
}