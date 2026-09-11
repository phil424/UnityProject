using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public class LevelEncounter : MonoBehaviour
    {
        public enum EncounterPresentationState
        {
            Unknown,
            Locked,
            Available,
            Active,
            Cleared,
            Expired
        }

        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;

        [Header("Initial State")]
        [SerializeField] private bool knownAtLevelStart = true;
        [SerializeField] private bool availableAtLevelStart = true;

        [Header("On Completed")]
        [SerializeField] private LevelEncounterActions onCompletedActions = new();

        [Header("Prototype Supply")]
        [SerializeField] private bool reusableByPrototypeSupply;

        [Header("Runtime (Debug)")]
        [SerializeField] private bool isKnown;
        [SerializeField] private bool isAvailable;
        [SerializeField] private bool isCompleted;
        [SerializeField] private bool isExpired;
        [SerializeField] private bool isCombatActivated;
        [SerializeField] private bool isStarted;
        [SerializeField] private int currentPhaseIndex = -1;
        [SerializeField] private EncounterPresentationState presentationState = EncounterPresentationState.Unknown;
        [SerializeField] private long availabilitySequence;
        [SerializeField] private long startedSequence;

        private static long nextAvailabilitySequence;
        private static long nextStartedSequence;

        private LevelSpawnGroup[] spawnGroups = Array.Empty<LevelSpawnGroup>();
        private LevelEncounterPhase[] phases = Array.Empty<LevelEncounterPhase>();
        
        private bool hasRuntimeIdentityOverride;
        private string runtimeDisplayName;
        private string runtimeDescription;

        public event Action<LevelEncounter, EncounterPresentationState> StateChanged;
        public event Action<LevelEncounter> Started;
        public event Action<LevelEncounter, LevelEncounterPhase, int> PhaseStarted;
        public event Action<LevelEncounter> Completed;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;

        public string AuthoredDisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string AuthoredDescription => description ?? string.Empty;

        public string DisplayName => hasRuntimeIdentityOverride
            ? ResolveDisplayName(runtimeDisplayName)
            : AuthoredDisplayName;

        public string Description => hasRuntimeIdentityOverride
            ? runtimeDescription ?? string.Empty
            : AuthoredDescription;
        
        public bool HasStarted => isStarted;
        public long StartedSequence => startedSequence;

        public Transform Anchor => transform;
        public Vector3 AnchorPosition => transform.position;

        public IReadOnlyList<LevelSpawnGroup> SpawnGroups => spawnGroups;
        public IReadOnlyList<LevelEncounterPhase> Phases => phases;

        public bool ReusableByPrototypeSupply => reusableByPrototypeSupply;
        public bool IsKnown => isKnown;
        public bool IsAvailable => isAvailable;
        public bool IsCompleted => isCompleted;
        public bool IsExpired => isExpired;
        public bool IsCombatActivated => isCombatActivated;
        public bool IsSelectable => isKnown && isAvailable && !isCompleted && !isExpired;

        public long AvailabilitySequence => availabilitySequence;
        public EncounterPresentationState PresentationState => presentationState;

        public int PhaseCount => phases.Length;
        public int CurrentPhaseIndex => currentPhaseIndex;
        public int CurrentPhaseNumber => currentPhaseIndex >= 0 ? currentPhaseIndex + 1 : 0;

        public LevelEncounterPhase CurrentPhase =>
            currentPhaseIndex >= 0 && currentPhaseIndex < phases.Length
                ? phases[currentPhaseIndex]
                : null;

        public bool HasBegunSpawning => phases.Length > 0 ? currentPhaseIndex >= 0 : HasStartedSpawnGroups;
        public bool HasStartedSpawnGroups => AnySpawnGroup(group => group.IsSpawningStarted);

        public bool HasUnstartedSpawnGroups =>
            AnySpawnGroup(group => group.ConfiguredSpawnCount > 0 && !group.IsSpawningStarted);

        public bool HasInactiveCombatGroups =>
            AnySpawnGroup(group => group.ConfiguredSpawnCount > 0 && !group.IsCombatActive);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeSequences()
        {
            nextAvailabilitySequence = 0;
            nextStartedSequence = 0;
        }

        private void OnEnable()
        {
            RefreshOwnedContent();
            SubscribeToSpawnGroups();
            Health.AnyDied += HandleActorDied;
        }

        private void OnDisable()
        {
            UnsubscribeFromSpawnGroups();
            Health.AnyDied -= HandleActorDied;
        }

        private void Update()
        {
            UpdatePhaseProgression(Time.deltaTime);
        }
        
        public void SetRuntimeIdentityOverride(string newDisplayName, string newDescription)
        {
            hasRuntimeIdentityOverride = true;
            runtimeDisplayName = newDisplayName;
            runtimeDescription = newDescription;
        }

        public void ClearRuntimeIdentityOverride()
        {
            hasRuntimeIdentityOverride = false;
            runtimeDisplayName = null;
            runtimeDescription = null;
        }

        private string ResolveDisplayName(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? name : value;
        }

        public void PrepareForLevel()
        {
            UnsubscribeFromSpawnGroups();
            RefreshOwnedContent();
            SubscribeToSpawnGroups();

            foreach (LevelEncounterPhase phase in phases)
                phase?.PrepareForLevel();

            isKnown = knownAtLevelStart || availableAtLevelStart;
            isAvailable = availableAtLevelStart;
            isCompleted = false;
            isExpired = false;
            isCombatActivated = false;
            isStarted = false;
            startedSequence = 0;
            currentPhaseIndex = -1;
            availabilitySequence = 0;

            if (isAvailable)
                StampAvailability();

            ValidatePhaseStructure();

            if (phases.Length > 0 && phases[0] != null && phases[0].WantsLevelStartSpawning)
                StartPhase(0);

            RefreshState();
        }

        public void ClearForLevel()
        {
            foreach (LevelEncounterPhase phase in phases)
                phase?.PrepareForLevel();

            isKnown = false;
            isAvailable = false;
            isCompleted = false;
            isExpired = false;
            isCombatActivated = false;
            isStarted = false;
            startedSequence = 0;
            currentPhaseIndex = -1;
            availabilitySequence = 0;

            SetPresentationState(EncounterPresentationState.Unknown);
        }

        public bool RearmForPrototypeSupply()
        {
            if (!reusableByPrototypeSupply)
                return false;

            foreach (LevelSpawnGroup group in spawnGroups)
                group?.PrepareForLevel();

            foreach (LevelEncounterPhase phase in phases)
                phase?.PrepareForLevel();

            isKnown = true;
            isAvailable = true;
            isCompleted = false;
            isExpired = false;
            isCombatActivated = false;
            isStarted = false;
            startedSequence = 0;
            currentPhaseIndex = -1;
            availabilitySequence = 0;

            StampAvailability();
            RefreshState();

            return true;
        }

        public bool MakeKnown()
        {
            if (isKnown)
                return false;

            isKnown = true;
            RefreshState();
            return true;
        }

        public bool MakeAvailable()
        {
            if (isAvailable || isCompleted || isExpired)
                return false;

            isKnown = true;
            isAvailable = true;
            StampAvailability();

            RefreshState();
            return true;
        }

        public bool BeginSpawning()
        {
            if (phases.Length > 0)
                return currentPhaseIndex < 0 && StartPhase(0);

            bool changed = false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && group.BeginSpawning())
                    changed = true;
            }

            TryMarkStarted();
            RefreshState();

            return changed;
        }

        public bool ActivateCombat()
        {
            bool changed = !isCombatActivated;
            isCombatActivated = true;

            if (phases.Length > 0)
            {
                foreach (LevelEncounterPhase phase in phases)
                {
                    if (phase != null && phase.IsStarted)
                        phase.ActivateCombat();
                }
            }
            else
            {
                foreach (LevelSpawnGroup group in spawnGroups)
                {
                    if (group != null && group.ActivateCombat())
                        changed = true;
                }
            }

            TryMarkStarted();
            RefreshState();
            return changed;
        }

        public bool Expire()
        {
            if (isCompleted || isExpired)
                return false;

            isExpired = true;
            isAvailable = false;

            RefreshState();
            return true;
        }

        public bool Complete()
        {
            if (isCompleted)
                return false;

            isKnown = true;
            isAvailable = false;
            isCompleted = true;

            RefreshPresentationState();

            Completed?.Invoke(this);
            onCompletedActions?.Execute(this);

            return true;
        }

        [ContextMenu("Debug/Make Known")]
        private void DebugMakeKnown()
        {
            MakeKnown();
        }

        [ContextMenu("Debug/Make Available")]
        private void DebugMakeAvailable()
        {
            MakeAvailable();
        }

        [ContextMenu("Debug/Begin Spawning")]
        private void DebugBeginSpawning()
        {
            BeginSpawning();
        }

        [ContextMenu("Debug/Activate Combat")]
        private void DebugActivateCombat()
        {
            ActivateCombat();
        }

        [ContextMenu("Debug/Expire")]
        private void DebugExpire()
        {
            Expire();
        }

        private bool StartPhase(int phaseIndex)
        {
            if (phaseIndex < 0 || phaseIndex >= phases.Length)
                return false;

            LevelEncounterPhase phase = phases[phaseIndex];

            if (phase == null || !phase.StartPhase(isCombatActivated))
                return false;

            currentPhaseIndex = phaseIndex;

            TryMarkStarted();
            PhaseStarted?.Invoke(this, phase, phaseIndex);

            Debug.Log(
                $"Encounter '{DisplayName}' started phase {phaseIndex + 1}/{phases.Length}: {phase.DisplayName}",
                this
            );

            RefreshState();
            return true;
        }

        private void UpdatePhaseProgression(float deltaTime)
        {
            if (isCompleted ||
                isExpired ||
                !isCombatActivated ||
                currentPhaseIndex < 0 ||
                currentPhaseIndex >= phases.Length - 1)
            {
                return;
            }

            LevelEncounterPhase currentPhase = phases[currentPhaseIndex];

            if (currentPhase == null)
                return;

            currentPhase.Tick(deltaTime);

            if (currentPhase.ShouldAdvance())
                StartPhase(currentPhaseIndex + 1);
        }

        private void RefreshOwnedContent()
        {
            LevelSpawnGroup[] groupCandidates = GetComponentsInChildren<LevelSpawnGroup>(true);
            List<LevelSpawnGroup> ownedGroups = new();

            foreach (LevelSpawnGroup group in groupCandidates)
            {
                if (group != null && group.GetComponentInParent<LevelEncounter>() == this)
                    ownedGroups.Add(group);
            }

            spawnGroups = ownedGroups.ToArray();

            LevelEncounterPhase[] phaseCandidates = GetComponentsInChildren<LevelEncounterPhase>(true);
            List<LevelEncounterPhase> ownedPhases = new();

            foreach (LevelEncounterPhase phase in phaseCandidates)
            {
                if (phase != null && phase.GetComponentInParent<LevelEncounter>() == this)
                    ownedPhases.Add(phase);
            }

            ownedPhases.Sort(
                (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())
            );

            phases = ownedPhases.ToArray();
        }

        private void SubscribeToSpawnGroups()
        {
            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null)
                    continue;

                group.SpawningStarted += HandleSpawningStarted;
                group.SpawningCompleted += HandleGroupStateChanged;
                group.CombatActivated += HandleGroupStateChanged;
            }
        }

        private void UnsubscribeFromSpawnGroups()
        {
            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null)
                    continue;

                group.SpawningStarted -= HandleSpawningStarted;
                group.SpawningCompleted -= HandleGroupStateChanged;
                group.CombatActivated -= HandleGroupStateChanged;
            }
        }

        private void HandleSpawningStarted(LevelSpawnGroup group, int spawnCount)
        {
            if (group != null && group.IsCombatActive)
                isCombatActivated = true;

            TryMarkStarted();
            RefreshState();
        }

        private void HandleGroupStateChanged(LevelSpawnGroup group)
        {
            if (group != null && group.IsCombatActive)
                isCombatActivated = true;

            TryMarkStarted();
            RefreshState();
        }

        private void HandleActorDied(Health health)
        {
            RefreshState();
        }
        
        private bool TryMarkStarted()
        {
            if (isStarted || !isCombatActivated || !HasBegunSpawning)
                return false;

            isStarted = true;
            startedSequence = ++nextStartedSequence;

            Started?.Invoke(this);
            return true;
        }

        private void RefreshState()
        {
            if (!isCompleted && HasCleared())
            {
                Complete();
                return;
            }

            RefreshPresentationState();
        }

        private void RefreshPresentationState()
        {
            SetPresentationState(ResolvePresentationState());
        }

        private EncounterPresentationState ResolvePresentationState()
        {
            if (!isKnown)
                return EncounterPresentationState.Unknown;

            if (isCompleted)
                return EncounterPresentationState.Cleared;

            if (isStarted)
                return EncounterPresentationState.Active;

            if (isExpired)
                return EncounterPresentationState.Expired;

            if (!isAvailable)
                return EncounterPresentationState.Locked;

            return EncounterPresentationState.Available;
        }

        private bool HasCleared()
        {
            bool hasConfiguredGroup = false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null || group.ConfiguredSpawnCount <= 0)
                    continue;

                hasConfiguredGroup = true;

                if (!group.IsComplete)
                    return false;
            }

            return hasConfiguredGroup;
        }

        private bool AnySpawnGroup(Func<LevelSpawnGroup, bool> predicate)
        {
            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && predicate(group))
                    return true;
            }

            return false;
        }

        private void SetPresentationState(EncounterPresentationState newState)
        {
            if (presentationState == newState)
                return;

            presentationState = newState;
            StateChanged?.Invoke(this, presentationState);
        }

        private void StampAvailability()
        {
            availabilitySequence = ++nextAvailabilitySequence;
        }

        private void ValidatePhaseStructure()
        {
            if (phases.Length == 0)
                return;

            foreach (LevelEncounterPhase phase in phases)
            {
                if (phase != null && phase.transform.parent != transform)
                {
                    Debug.LogWarning(
                        $"Encounter phase '{phase.name}' should be a direct child of encounter '{DisplayName}' so sibling order is unambiguous.",
                        phase
                    );
                }
            }

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null || group.ConfiguredSpawnCount <= 0)
                    continue;

                LevelEncounterPhase phase = group.GetComponentInParent<LevelEncounterPhase>();

                if (phase == null || phase.GetComponentInParent<LevelEncounter>() != this)
                {
                    Debug.LogWarning(
                        $"Encounter '{DisplayName}' uses phases but spawn group '{group.name}' is not owned by a phase.",
                        group
                    );
                }
            }
        }

        private void OnValidate()
        {
            if (availableAtLevelStart)
                knownAtLevelStart = true;

            RefreshOwnedContent();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}