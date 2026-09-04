using System;
using System.Collections;
using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniCrawler.Spawning
{
    [DisallowMultipleComponent]
    public class LevelSpawnGroup : MonoBehaviour
    {
        [Serializable]
        public class SpawnEntry
        {
            [SerializeField] private ActorDefinition actor;
            [SerializeField, Min(1)] private int count = 1;

            [Header("Timing")]
            [SerializeField, Min(0f)] private float startDelay;
            [SerializeField, Min(1)] private int batchSize = 1;
            [SerializeField, Min(0f)] private float timeBetweenSpawns;
            [SerializeField, Min(0f)] private float timeBetweenBatches;

            public ActorDefinition Actor => actor;
            public int Count => count;
            public float StartDelay => startDelay;
            public int BatchSize => batchSize;
            public float TimeBetweenSpawns => timeBetweenSpawns;
            public float TimeBetweenBatches => timeBetweenBatches;

            public bool IsConfigured => actor != null && count > 0;

            public void ClampValues()
            {
                count = Mathf.Max(1, count);
                startDelay = Mathf.Max(0f, startDelay);
                batchSize = Mathf.Max(1, batchSize);
                timeBetweenSpawns = Mathf.Max(0f, timeBetweenSpawns);
                timeBetweenBatches = Mathf.Max(0f, timeBetweenBatches);
            }
        }

        [Header("Level Start")]
        [FormerlySerializedAs("startActive")]
        [SerializeField] private bool startSpawning = true;
        [SerializeField] private bool startCombatActive = true;

        [Header("Spawn Sources")]
        [SerializeField] private LevelSpawnSource[] spawnSources;

        [Header("Schedule")]
        [SerializeField] private List<SpawnEntry> entries = new();

        public event Action<LevelSpawnGroup, int> SpawningStarted;
        public event Action<LevelSpawnGroup, ActorDefinition, Pose> SpawnRequested;

        private readonly List<GameObject> spawnedActors = new();
        private int runningEntries;

        public bool StartSpawning => startSpawning;
        public bool StartCombatActive => startCombatActive;

        public bool IsSpawningStarted { get; private set; }
        public bool IsCombatActive { get; private set; }
        public bool IsRunning => runningEntries > 0;

        public int ConfiguredSpawnCount
        {
            get
            {
                if (!HasValidSpawnSource())
                    return 0;

                int total = 0;

                foreach (SpawnEntry entry in entries)
                {
                    if (entry != null && entry.IsConfigured)
                        total += entry.Count;
                }

                return total;
            }
        }

        public void PrepareForLevel()
        {
            StopAllCoroutines();

            spawnedActors.Clear();
            runningEntries = 0;

            IsSpawningStarted = false;
            IsCombatActive = startCombatActive;
        }

        public bool BeginSpawning()
        {
            if (IsSpawningStarted || !HasValidSpawnSource())
                return false;

            int validEntryCount = 0;
            int spawnCount = 0;

            foreach (SpawnEntry entry in entries)
            {
                if (entry == null || !entry.IsConfigured)
                    continue;

                validEntryCount++;
                spawnCount += entry.Count;
            }

            if (validEntryCount <= 0 || spawnCount <= 0)
                return false;

            IsSpawningStarted = true;
            runningEntries = validEntryCount;

            // Report the committed spawn count before any immediate coroutine
            // can issue its first SpawnRequested event.
            SpawningStarted?.Invoke(this, spawnCount);

            foreach (SpawnEntry entry in entries)
            {
                if (entry != null && entry.IsConfigured)
                    StartCoroutine(RunEntry(entry));
            }

            return true;
        }

        public bool ActivateCombat()
        {
            if (IsCombatActive)
                return false;

            IsCombatActive = true;

            foreach (GameObject actor in spawnedActors)
            {
                if (actor == null)
                    continue;

                CombatEngagementState engagement = actor.GetComponent<CombatEngagementState>();

                if (engagement == null)
                    engagement = actor.AddComponent<CombatEngagementState>();

                engagement.SetEngaged(true);
            }

            return true;
        }

        public void RegisterSpawnedActor(GameObject actor)
        {
            if (actor == null)
                return;

            CombatEngagementState engagement = actor.GetComponent<CombatEngagementState>();

            if (engagement == null)
                engagement = actor.AddComponent<CombatEngagementState>();

            engagement.SetEngaged(IsCombatActive);
            spawnedActors.Add(actor);
        }

        public void StopSpawning()
        {
            StopAllCoroutines();
            runningEntries = 0;
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

        private IEnumerator RunEntry(SpawnEntry entry)
        {
            if (entry.StartDelay > 0f)
                yield return new WaitForSeconds(entry.StartDelay);

            int remaining = entry.Count;

            while (remaining > 0)
            {
                int batchCount = Mathf.Min(entry.BatchSize, remaining);

                for (int i = 0; i < batchCount; i++)
                {
                    if (TryGetSpawnPose(out Pose pose))
                        SpawnRequested?.Invoke(this, entry.Actor, pose);

                    remaining--;

                    if (i < batchCount - 1 && entry.TimeBetweenSpawns > 0f)
                        yield return new WaitForSeconds(entry.TimeBetweenSpawns);
                }

                if (remaining > 0 && entry.TimeBetweenBatches > 0f)
                    yield return new WaitForSeconds(entry.TimeBetweenBatches);
            }

            runningEntries = Mathf.Max(0, runningEntries - 1);
        }

        private bool TryGetSpawnPose(out Pose pose)
        {
            if (spawnSources == null || spawnSources.Length == 0)
            {
                pose = default;
                return false;
            }

            int startIndex = UnityEngine.Random.Range(0, spawnSources.Length);

            for (int i = 0; i < spawnSources.Length; i++)
            {
                int index = (startIndex + i) % spawnSources.Length;
                LevelSpawnSource source = spawnSources[index];

                if (source == null)
                    continue;

                pose = source.GetSpawnPose();
                return true;
            }

            pose = default;
            return false;
        }

        private bool HasValidSpawnSource()
        {
            if (spawnSources == null)
                return false;

            foreach (LevelSpawnSource source in spawnSources)
            {
                if (source != null)
                    return true;
            }

            return false;
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        private void OnValidate()
        {
            foreach (SpawnEntry entry in entries)
                entry?.ClampValues();
        }
    }
}