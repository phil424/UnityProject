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

        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public IReadOnlyList<LevelSpawnGroup> SpawnGroups => spawnGroups;
        public bool IsStarted { get; private set; }
        public float ElapsedSeconds => elapsedSeconds;
        public float AdvanceAfterSeconds => advanceAfterSeconds;
        public bool AdvanceWhenCleared => advanceWhenCleared;

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

            if (advanceWhenCleared && IsCleared)
                return true;

            return advanceAfterSeconds > 0f && elapsedSeconds >= advanceAfterSeconds;
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

            spawnGroups = ownedGroups.ToArray();
        }

        private void OnValidate()
        {
            advanceAfterSeconds = Mathf.Max(0f, advanceAfterSeconds);
            RefreshOwnedSpawnGroups();
        }
    }
}