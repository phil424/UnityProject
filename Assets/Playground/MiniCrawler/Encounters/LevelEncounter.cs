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
        public enum EncounterState
        {
            Dormant,
            Available,
            Active,
            Cleared
        }

        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;

        [Header("Location")]
        [SerializeField] private Transform anchor;

        [Header("Availability")]
        [SerializeField] private bool availableAtLevelStart = true;

        [Header("Content")]
        [SerializeField] private LevelSpawnGroup[] spawnGroups;

        [Header("Runtime (Debug)")]
        [SerializeField] private EncounterState state = EncounterState.Dormant;

        private bool isAvailable;

        public event Action<LevelEncounter, EncounterState> StateChanged;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;

        public Transform Anchor => anchor != null ? anchor : transform;
        public Vector3 AnchorPosition => Anchor.position;

        public IReadOnlyList<LevelSpawnGroup> SpawnGroups =>
            spawnGroups ?? Array.Empty<LevelSpawnGroup>();

        public EncounterState State => state;
        public bool IsAvailable => isAvailable;
        public bool IsSelectable => state == EncounterState.Available || state == EncounterState.Active;

        private void OnEnable()
        {
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
            isAvailable = availableAtLevelStart;
            RefreshState();
        }

        public void ClearForLevel()
        {
            isAvailable = false;
            SetState(EncounterState.Dormant);
        }

        public bool MakeAvailable()
        {
            if (isAvailable || state == EncounterState.Cleared)
                return false;

            isAvailable = true;
            RefreshState();
            return true;
        }

        [ContextMenu("Debug/Make Available")]
        private void DebugMakeAvailable()
        {
            MakeAvailable();
        }

        private void SubscribeToSpawnGroups()
        {
            if (spawnGroups == null)
                return;

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
            if (spawnGroups == null)
                return;

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
            if (HasCleared())
            {
                SetState(EncounterState.Cleared);
                return;
            }

            if (HasActiveCombat())
            {
                SetState(EncounterState.Active);
                return;
            }

            SetState(isAvailable ? EncounterState.Available : EncounterState.Dormant);
        }

        private bool HasActiveCombat()
        {
            if (spawnGroups == null)
                return false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null)
                    continue;

                if (group.IsSpawningStarted && group.IsCombatActive && !group.IsComplete)
                    return true;
            }

            return false;
        }

        private bool HasCleared()
        {
            if (spawnGroups == null)
                return false;

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

        private void SetState(EncounterState newState)
        {
            if (state == newState)
                return;

            state = newState;
            StateChanged?.Invoke(this, state);
        }

        private void OnDrawGizmosSelected()
        {
            Transform resolvedAnchor = anchor != null ? anchor : transform;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(resolvedAnchor.position, 0.5f);

            if (anchor != null)
                Gizmos.DrawLine(transform.position, anchor.position);
        }
    }
}