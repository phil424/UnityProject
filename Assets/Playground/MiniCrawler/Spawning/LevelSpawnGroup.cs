using System;
using System.Collections;
using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

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

        [Header("Spawn Sources")]
        [SerializeField] private LevelSpawnSource[] spawnSources;

        [Header("Schedule")]
        [SerializeField] private List<SpawnEntry> entries = new();

        public event Action<ActorDefinition, Pose> SpawnRequested;

        private int runningEntries;

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

        public void Begin()
        {
            StopSpawning();

            if (!HasValidSpawnSource())
            {
                Debug.LogWarning($"Spawn group '{name}' has no valid spawn source.");
                return;
            }

            runningEntries = 0;

            foreach (SpawnEntry entry in entries)
            {
                if (entry != null && entry.IsConfigured)
                    runningEntries++;
            }

            foreach (SpawnEntry entry in entries)
            {
                if (entry == null || !entry.IsConfigured)
                    continue;

                StartCoroutine(RunEntry(entry));
            }
        }

        public void StopSpawning()
        {
            StopAllCoroutines();
            runningEntries = 0;
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
                        SpawnRequested?.Invoke(entry.Actor, pose);

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