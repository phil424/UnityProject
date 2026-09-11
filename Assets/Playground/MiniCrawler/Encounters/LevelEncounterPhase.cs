using System;
using System.Collections.Generic;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public sealed class LevelEncounterPhase : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private string displayName;

        [Header("Progression")]
        [SerializeField, Min(0f)] private float advanceAfterSeconds;
        [SerializeField] private bool advanceWhenCleared = true;

        private LevelSpawnGroup[] spawnGroups = Array.Empty<LevelSpawnGroup>();
        private float elapsedSeconds;
        
        private bool hasRuntimeAuthoringOverride;
        private string runtimeDisplayName;
        private float runtimeAdvanceAfterSeconds;
        private bool runtimeAdvanceWhenCleared;

        public string AuthoredDisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public float AuthoredAdvanceAfterSeconds => advanceAfterSeconds;
        public bool AuthoredAdvanceWhenCleared => advanceWhenCleared;

        public string DisplayName => hasRuntimeAuthoringOverride
            ? ResolveDisplayName(runtimeDisplayName)
            : AuthoredDisplayName;

        public IReadOnlyList<LevelSpawnGroup> SpawnGroups => spawnGroups;

        public bool IsStarted { get; private set; }
        public float ElapsedSeconds => elapsedSeconds;

        public float AdvanceAfterSeconds =>
            hasRuntimeAuthoringOverride ? runtimeAdvanceAfterSeconds : advanceAfterSeconds;

        public bool AdvanceWhenCleared =>
            hasRuntimeAuthoringOverride ? runtimeAdvanceWhenCleared : advanceWhenCleared;

        public bool IsCleared
        {
            get
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
        }

        public bool WantsLevelStartSpawning
        {
            get
            {
                foreach (LevelSpawnGroup group in spawnGroups)
                {
                    if (group != null && group.ConfiguredSpawnCount > 0 && group.StartSpawning)
                        return true;
                }

                return false;
            }
        }

        private void Awake()
        {
            RefreshOwnedSpawnGroups();
        }
        
        public void SetRuntimeAuthoringOverride(string newDisplayName, float newAdvanceAfterSeconds, bool newAdvanceWhenCleared)
        {
            hasRuntimeAuthoringOverride = true;
            runtimeDisplayName = newDisplayName;
            runtimeAdvanceAfterSeconds = Mathf.Max(0f, newAdvanceAfterSeconds);
            runtimeAdvanceWhenCleared = newAdvanceWhenCleared;
        }

        public void ClearRuntimeAuthoringOverride()
        {
            hasRuntimeAuthoringOverride = false;
            runtimeDisplayName = null;
            runtimeAdvanceAfterSeconds = 0f;
            runtimeAdvanceWhenCleared = false;
        }

        private string ResolveDisplayName(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? name : value;
        }

        public void PrepareForLevel()
        {
            RefreshOwnedSpawnGroups();
            IsStarted = false;
            elapsedSeconds = 0f;
        }

        public bool StartPhase(bool activateCombat)
        {
            if (IsStarted)
                return false;

            RefreshOwnedSpawnGroups();

            IsStarted = true;
            elapsedSeconds = 0f;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group == null || group.ConfiguredSpawnCount <= 0)
                    continue;

                // Combat first so immediate spawns inherit the intended state.
                if (activateCombat)
                    group.ActivateCombat();

                group.BeginSpawning();
            }

            return true;
        }

        public void ActivateCombat()
        {
            if (!IsStarted)
                return;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && group.ConfiguredSpawnCount > 0)
                    group.ActivateCombat();
            }
        }

        public void Tick(float deltaTime)
        {
            if (IsStarted && deltaTime > 0f)
                elapsedSeconds += deltaTime;
        }

        public bool ShouldAdvance()
        {
            if (!IsStarted)
                return false;

            if (AdvanceWhenCleared && IsCleared)
                return true;

            return AdvanceAfterSeconds > 0f && elapsedSeconds >= AdvanceAfterSeconds;
        }

        private void RefreshOwnedSpawnGroups()
        {
            LevelSpawnGroup[] candidates = GetComponentsInChildren<LevelSpawnGroup>(true);
            List<LevelSpawnGroup> ownedGroups = new();

            foreach (LevelSpawnGroup group in candidates)
            {
                if (group != null && group.GetComponentInParent<LevelEncounterPhase>() == this)
                    ownedGroups.Add(group);
            }

            ownedGroups.Sort(
                (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())
            );

            spawnGroups = ownedGroups.ToArray();
        }

        private void OnValidate()
        {
            advanceAfterSeconds = Mathf.Max(0f, advanceAfterSeconds);
            RefreshOwnedSpawnGroups();
        }
    }
}