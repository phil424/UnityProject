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

        [Header("Runtime (Debug)")]
        [SerializeField] private bool isKnown;
        [SerializeField] private bool isAvailable;
        [SerializeField] private bool isCompleted;
        [SerializeField] private bool isExpired;
        [SerializeField] private EncounterPresentationState presentationState = EncounterPresentationState.Unknown;

        private LevelSpawnGroup[] spawnGroups = Array.Empty<LevelSpawnGroup>();

        public event Action<LevelEncounter, EncounterPresentationState> StateChanged;
        public event Action<LevelEncounter> Completed;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;

        public Transform Anchor => transform;
        public Vector3 AnchorPosition => transform.position;

        public IReadOnlyList<LevelSpawnGroup> SpawnGroups => spawnGroups;

        public bool IsKnown => isKnown;
        public bool IsAvailable => isAvailable;
        public bool IsCompleted => isCompleted;
        public bool IsExpired => isExpired;

        public bool IsSelectable => isKnown && isAvailable && !isCompleted && !isExpired;

        public EncounterPresentationState PresentationState => presentationState;

        public bool HasUnstartedSpawnGroups
        {
            get
            {
                foreach (LevelSpawnGroup group in spawnGroups)
                {
                    if (group != null && group.ConfiguredSpawnCount > 0 && !group.IsSpawningStarted)
                        return true;
                }

                return false;
            }
        }

        public bool HasInactiveCombatGroups
        {
            get
            {
                foreach (LevelSpawnGroup group in spawnGroups)
                {
                    if (group != null && group.ConfiguredSpawnCount > 0 && !group.IsCombatActive)
                        return true;
                }

                return false;
            }
        }

        private void OnEnable()
        {
            RefreshOwnedSpawnGroups();
            SubscribeToSpawnGroups();
            Health.AnyDied += HandleActorDied;
        }

        private void OnDisable()
        {
            UnsubscribeFromSpawnGroups();
            Health.AnyDied -= HandleActorDied;
        }

        public void PrepareForLevel()
        {
            UnsubscribeFromSpawnGroups();
            RefreshOwnedSpawnGroups();
            SubscribeToSpawnGroups();

            isKnown = knownAtLevelStart || availableAtLevelStart;
            isAvailable = availableAtLevelStart;
            isCompleted = false;
            isExpired = false;

            RefreshState();
        }

        public void ClearForLevel()
        {
            isKnown = false;
            isAvailable = false;
            isCompleted = false;
            isExpired = false;

            SetPresentationState(EncounterPresentationState.Unknown);
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

            RefreshState();
            return true;
        }

        public bool BeginSpawning()
        {
            bool changed = false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && group.BeginSpawning())
                    changed = true;
            }

            RefreshState();
            return changed;
        }

        public bool ActivateCombat()
        {
            bool changed = false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && group.ActivateCombat())
                    changed = true;
            }

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

        private void RefreshOwnedSpawnGroups()
        {
            LevelSpawnGroup[] candidates = GetComponentsInChildren<LevelSpawnGroup>(true);
            List<LevelSpawnGroup> ownedGroups = new();

            foreach (LevelSpawnGroup group in candidates)
            {
                if (group == null)
                    continue;

                LevelEncounter owner = group.GetComponentInParent<LevelEncounter>();

                if (owner == this)
                    ownedGroups.Add(group);
            }

            spawnGroups = ownedGroups.ToArray();
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
            RefreshState();
        }

        private void HandleGroupStateChanged(LevelSpawnGroup group)
        {
            RefreshState();
        }

        private void HandleActorDied(Health health)
        {
            RefreshState();
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

            if (HasActiveCombat())
                return EncounterPresentationState.Active;

            if (isExpired)
                return EncounterPresentationState.Expired;

            if (!isAvailable)
                return EncounterPresentationState.Locked;

            return EncounterPresentationState.Available;
        }

        private bool HasActiveCombat()
        {
            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && group.IsSpawningStarted && group.IsCombatActive && !group.IsComplete)
                    return true;
            }

            return false;
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

        private void SetPresentationState(EncounterPresentationState newState)
        {
            if (presentationState == newState)
                return;

            presentationState = newState;
            StateChanged?.Invoke(this, presentationState);
        }

        private void OnValidate()
        {
            if (availableAtLevelStart)
                knownAtLevelStart = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}