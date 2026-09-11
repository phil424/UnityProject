using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [CreateAssetMenu(
        fileName = "New Encounter Definition",
        menuName = "Mini Crawler/Encounters/Encounter Definition"
    )]
    public sealed class EncounterDefinition : ScriptableObject
    {
        [Serializable]
        public sealed class SpawnEntryDefinition
        {
            [SerializeField] private ActorDefinition actor;
            [SerializeField, Min(1)] private int count = 1;
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

            public SpawnEntryDefinition(
                ActorDefinition actor,
                int count,
                float startDelay,
                int batchSize,
                float timeBetweenSpawns,
                float timeBetweenBatches)
            {
                this.actor = actor;
                this.count = count;
                this.startDelay = startDelay;
                this.batchSize = batchSize;
                this.timeBetweenSpawns = timeBetweenSpawns;
                this.timeBetweenBatches = timeBetweenBatches;

                ClampValues();
            }

            public SpawnEntryDefinition(SpawnEntryDefinition source)
                : this(
                    source?.actor,
                    source?.count ?? 1,
                    source?.startDelay ?? 0f,
                    source?.batchSize ?? 1,
                    source?.timeBetweenSpawns ?? 0f,
                    source?.timeBetweenBatches ?? 0f)
            {
            }

            public void ClampValues()
            {
                count = Mathf.Max(1, count);
                startDelay = Mathf.Max(0f, startDelay);
                batchSize = Mathf.Clamp(batchSize, 1, count);
                timeBetweenSpawns = Mathf.Max(0f, timeBetweenSpawns);
                timeBetweenBatches = Mathf.Max(0f, timeBetweenBatches);
            }
        }

        [Serializable]
        public sealed class SpawnSourceDefinition
        {
            [SerializeField] private string label;
            [SerializeField] private LevelSpawnSource.SpawnShape shape;
            [SerializeField, Min(0f)] private float radius = 1.5f;
            [SerializeField] private Vector2 boxSize = new(3f, 3f);

            public string Label => label ?? string.Empty;
            public LevelSpawnSource.SpawnShape Shape => shape;
            public float Radius => radius;
            public Vector2 BoxSize => boxSize;

            public SpawnSourceDefinition(
                string label,
                LevelSpawnSource.SpawnShape shape,
                float radius,
                Vector2 boxSize)
            {
                this.label = label;
                this.shape = shape;
                this.radius = radius;
                this.boxSize = boxSize;

                ClampValues();
            }

            public SpawnSourceDefinition(SpawnSourceDefinition source)
                : this(
                    source?.label,
                    source?.shape ?? LevelSpawnSource.SpawnShape.Point,
                    source?.radius ?? 0f,
                    source?.boxSize ?? Vector2.zero)
            {
            }

            public void ClampValues()
            {
                radius = Mathf.Max(0f, radius);
                boxSize.x = Mathf.Max(0f, boxSize.x);
                boxSize.y = Mathf.Max(0f, boxSize.y);
            }
        }

        [Serializable]
        public sealed class SpawnGroupDefinition
        {
            [SerializeField] private string label;
            [SerializeField] private List<SpawnEntryDefinition> entries = new();
            [SerializeField] private List<SpawnSourceDefinition> spawnSources = new();

            public string Label => label ?? string.Empty;
            public IReadOnlyList<SpawnEntryDefinition> Entries => entries;
            public IReadOnlyList<SpawnSourceDefinition> SpawnSources => spawnSources;

            public SpawnGroupDefinition(
                string label,
                IReadOnlyList<SpawnEntryDefinition> entries,
                IReadOnlyList<SpawnSourceDefinition> spawnSources)
            {
                this.label = label;

                if (entries != null)
                {
                    foreach (SpawnEntryDefinition entry in entries)
                    {
                        if (entry != null)
                            this.entries.Add(new SpawnEntryDefinition(entry));
                    }
                }

                if (spawnSources != null)
                {
                    foreach (SpawnSourceDefinition source in spawnSources)
                    {
                        if (source != null)
                            this.spawnSources.Add(new SpawnSourceDefinition(source));
                    }
                }
            }

            public SpawnGroupDefinition(SpawnGroupDefinition source)
                : this(source?.label, source?.entries, source?.spawnSources)
            {
            }

            public void ClampValues()
            {
                foreach (SpawnEntryDefinition entry in entries)
                    entry?.ClampValues();

                foreach (SpawnSourceDefinition source in spawnSources)
                    source?.ClampValues();
            }
        }

        [Serializable]
        public sealed class PhaseDefinition
        {
            [SerializeField] private string displayName;
            [SerializeField, Min(0f)] private float advanceAfterSeconds;
            [SerializeField] private bool advanceWhenCleared = true;
            [SerializeField] private List<SpawnGroupDefinition> spawnGroups = new();

            public string DisplayName => displayName ?? string.Empty;
            public float AdvanceAfterSeconds => advanceAfterSeconds;
            public bool AdvanceWhenCleared => advanceWhenCleared;
            public IReadOnlyList<SpawnGroupDefinition> SpawnGroups => spawnGroups;

            public PhaseDefinition(
                string displayName,
                float advanceAfterSeconds,
                bool advanceWhenCleared,
                IReadOnlyList<SpawnGroupDefinition> spawnGroups)
            {
                this.displayName = displayName;
                this.advanceAfterSeconds = advanceAfterSeconds;
                this.advanceWhenCleared = advanceWhenCleared;

                if (spawnGroups != null)
                {
                    foreach (SpawnGroupDefinition group in spawnGroups)
                    {
                        if (group != null)
                            this.spawnGroups.Add(new SpawnGroupDefinition(group));
                    }
                }

                ClampValues();
            }

            public PhaseDefinition(PhaseDefinition source)
                : this(
                    source?.displayName,
                    source?.advanceAfterSeconds ?? 0f,
                    source?.advanceWhenCleared ?? true,
                    source?.spawnGroups)
            {
            }

            public void ClampValues()
            {
                advanceAfterSeconds = Mathf.Max(0f, advanceAfterSeconds);

                foreach (SpawnGroupDefinition group in spawnGroups)
                    group?.ClampValues();
            }
        }

        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;

        [Header("Phased Content")]
        [SerializeField] private List<PhaseDefinition> phases = new();

        [Header("Legacy / Unphased Content")]
        [SerializeField] private List<SpawnGroupDefinition> unphasedGroups = new();

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;

        public IReadOnlyList<PhaseDefinition> Phases => phases;
        public IReadOnlyList<SpawnGroupDefinition> UnphasedGroups => unphasedGroups;

        public bool IsPhased => phases.Count > 0;

        public void EnsureId(string fallback)
        {
            if (string.IsNullOrWhiteSpace(id))
                id = string.IsNullOrWhiteSpace(fallback) ? name : fallback;
        }

        public void ReplaceContent(
            string newDisplayName,
            string newDescription,
            IReadOnlyList<PhaseDefinition> newPhases,
            IReadOnlyList<SpawnGroupDefinition> newUnphasedGroups)
        {
            displayName = newDisplayName;
            description = newDescription;

            phases = new List<PhaseDefinition>();
            unphasedGroups = new List<SpawnGroupDefinition>();

            if (newPhases != null)
            {
                foreach (PhaseDefinition phase in newPhases)
                {
                    if (phase != null)
                        phases.Add(new PhaseDefinition(phase));
                }
            }

            if (newUnphasedGroups != null)
            {
                foreach (SpawnGroupDefinition group in newUnphasedGroups)
                {
                    if (group != null)
                        unphasedGroups.Add(new SpawnGroupDefinition(group));
                }
            }

            ClampValues();
        }

        private void OnValidate()
        {
            EnsureId(name);
            ClampValues();
        }

        private void ClampValues()
        {
            foreach (PhaseDefinition phase in phases)
                phase?.ClampValues();

            foreach (SpawnGroupDefinition group in unphasedGroups)
                group?.ClampValues();
        }
    }
}