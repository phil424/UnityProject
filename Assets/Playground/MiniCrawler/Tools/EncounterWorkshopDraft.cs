using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Tools
{
    public sealed class EncounterWorkshopDraft
    {
        public sealed class SpawnEntryDraft
        {
            public ActorDefinition Actor { get; set; }
            public int Count { get; set; }
            public float StartDelay { get; set; }
            public int BatchSize { get; set; }
            public float TimeBetweenSpawns { get; set; }
            public float TimeBetweenBatches { get; set; }

            public SpawnEntryDraft(LevelSpawnGroup.SpawnEntry source)
            {
                Actor = source.Actor;
                Count = source.Count;
                StartDelay = source.StartDelay;
                BatchSize = source.BatchSize;
                TimeBetweenSpawns = source.TimeBetweenSpawns;
                TimeBetweenBatches = source.TimeBetweenBatches;
            }

            public SpawnEntryDraft(EncounterDefinition.SpawnEntryDefinition source)
            {
                Actor = source.Actor;
                Count = source.Count;
                StartDelay = source.StartDelay;
                BatchSize = source.BatchSize;
                TimeBetweenSpawns = source.TimeBetweenSpawns;
                TimeBetweenBatches = source.TimeBetweenBatches;
            }

            public LevelSpawnGroup.SpawnEntry CreateRuntimeEntry()
            {
                return new LevelSpawnGroup.SpawnEntry(
                    Actor,
                    Mathf.Max(1, Count),
                    Mathf.Max(0f, StartDelay),
                    Mathf.Clamp(BatchSize, 1, Mathf.Max(1, Count)),
                    Mathf.Max(0f, TimeBetweenSpawns),
                    Mathf.Max(0f, TimeBetweenBatches)
                );
            }

            public EncounterDefinition.SpawnEntryDefinition CreateDefinition()
            {
                return new EncounterDefinition.SpawnEntryDefinition(
                    Actor,
                    Count,
                    StartDelay,
                    BatchSize,
                    TimeBetweenSpawns,
                    TimeBetweenBatches
                );
            }
        }

        public sealed class SpawnSourceDraft
        {
            public LevelSpawnSource Source { get; }

            public LevelSpawnSource.SpawnShape Shape { get; set; }
            public float Radius { get; set; }
            public Vector2 BoxSize { get; set; }

            public string Name => Source != null ? Source.gameObject.name : "Spawn Source";

            public SpawnSourceDraft(LevelSpawnSource source)
            {
                Source = source;
                Shape = source.AuthoredShape;
                Radius = source.AuthoredRadius;
                BoxSize = source.AuthoredBoxSize;
            }

            public SpawnSourceDraft(
                LevelSpawnSource source,
                EncounterDefinition.SpawnSourceDefinition definition)
            {
                Source = source;
                Shape = definition.Shape;
                Radius = definition.Radius;
                BoxSize = definition.BoxSize;
            }

            public void Apply()
            {
                Source?.SetRuntimeConfigurationOverride(Shape, Radius, BoxSize);
            }

            public void Clear()
            {
                Source?.ClearRuntimeConfigurationOverride();
            }

            public EncounterDefinition.SpawnSourceDefinition CreateDefinition()
            {
                return new EncounterDefinition.SpawnSourceDefinition(
                    Name,
                    Shape,
                    Radius,
                    BoxSize
                );
            }
        }

        public sealed class SpawnGroupDraft
        {
            public LevelSpawnGroup Source { get; }

            public List<SpawnEntryDraft> Entries { get; } = new();
            public List<SpawnSourceDraft> Sources { get; } = new();

            public string Name => Source != null ? Source.gameObject.name : "Spawn Group";

            public SpawnGroupDraft(LevelSpawnGroup source)
            {
                Source = source;

                foreach (LevelSpawnGroup.SpawnEntry entry in source.AuthoredEntries)
                {
                    if (entry != null)
                        Entries.Add(new SpawnEntryDraft(entry));
                }

                foreach (LevelSpawnSource spawnSource in source.SpawnSources)
                {
                    if (spawnSource != null)
                        Sources.Add(new SpawnSourceDraft(spawnSource));
                }
            }

            public SpawnGroupDraft(
                LevelSpawnGroup source,
                EncounterDefinition.SpawnGroupDefinition definition)
            {
                Source = source;

                foreach (EncounterDefinition.SpawnEntryDefinition entry in definition.Entries)
                {
                    if (entry != null)
                        Entries.Add(new SpawnEntryDraft(entry));
                }

                for (int i = 0; i < definition.SpawnSources.Count; i++)
                {
                    Sources.Add(
                        new SpawnSourceDraft(
                            source.SpawnSources[i],
                            definition.SpawnSources[i]
                        )
                    );
                }
            }

            public void Apply()
            {
                List<LevelSpawnGroup.SpawnEntry> runtimeEntries = new();

                foreach (SpawnEntryDraft entry in Entries)
                {
                    if (entry != null)
                        runtimeEntries.Add(entry.CreateRuntimeEntry());
                }

                Source?.SetRuntimeScheduleOverride(runtimeEntries);

                foreach (SpawnSourceDraft source in Sources)
                    source?.Apply();
            }

            public void Clear()
            {
                Source?.ClearRuntimeScheduleOverride();

                foreach (SpawnSourceDraft source in Sources)
                    source?.Clear();
            }

            public EncounterDefinition.SpawnGroupDefinition CreateDefinition()
            {
                List<EncounterDefinition.SpawnEntryDefinition> entries = new();
                List<EncounterDefinition.SpawnSourceDefinition> sources = new();

                foreach (SpawnEntryDraft entry in Entries)
                {
                    if (entry != null)
                        entries.Add(entry.CreateDefinition());
                }

                foreach (SpawnSourceDraft source in Sources)
                {
                    if (source != null)
                        sources.Add(source.CreateDefinition());
                }

                return new EncounterDefinition.SpawnGroupDefinition(
                    Name,
                    entries,
                    sources
                );
            }
        }

        public sealed class PhaseDraft
        {
            public LevelEncounterPhase Source { get; }

            public string DisplayName { get; set; }
            public float AdvanceAfterSeconds { get; set; }
            public bool AdvanceWhenCleared { get; set; }

            public List<SpawnGroupDraft> Groups { get; } = new();

            public PhaseDraft(LevelEncounterPhase source)
            {
                Source = source;
                DisplayName = source.AuthoredDisplayName;
                AdvanceAfterSeconds = source.AuthoredAdvanceAfterSeconds;
                AdvanceWhenCleared = source.AuthoredAdvanceWhenCleared;

                foreach (LevelSpawnGroup group in source.SpawnGroups)
                {
                    if (group != null)
                        Groups.Add(new SpawnGroupDraft(group));
                }
            }

            public PhaseDraft(
                LevelEncounterPhase source,
                EncounterDefinition.PhaseDefinition definition)
            {
                Source = source;
                DisplayName = definition.DisplayName;
                AdvanceAfterSeconds = definition.AdvanceAfterSeconds;
                AdvanceWhenCleared = definition.AdvanceWhenCleared;

                for (int i = 0; i < definition.SpawnGroups.Count; i++)
                {
                    Groups.Add(
                        new SpawnGroupDraft(
                            source.SpawnGroups[i],
                            definition.SpawnGroups[i]
                        )
                    );
                }
            }

            public void Apply()
            {
                Source?.SetRuntimeAuthoringOverride(
                    DisplayName,
                    AdvanceAfterSeconds,
                    AdvanceWhenCleared
                );

                foreach (SpawnGroupDraft group in Groups)
                    group?.Apply();
            }

            public void Clear()
            {
                Source?.ClearRuntimeAuthoringOverride();

                foreach (SpawnGroupDraft group in Groups)
                    group?.Clear();
            }

            public EncounterDefinition.PhaseDefinition CreateDefinition()
            {
                List<EncounterDefinition.SpawnGroupDefinition> groups = new();

                foreach (SpawnGroupDraft group in Groups)
                {
                    if (group != null)
                        groups.Add(group.CreateDefinition());
                }

                return new EncounterDefinition.PhaseDefinition(
                    DisplayName,
                    AdvanceAfterSeconds,
                    AdvanceWhenCleared,
                    groups
                );
            }
        }

        private LevelEncounter sourceEncounter;

        private int revision;
        private int appliedRevision;
        private int savedRevision;

        public string DisplayName { get; set; }
        public string Description { get; set; }

        public List<PhaseDraft> Phases { get; } = new();
        public List<SpawnGroupDraft> UnphasedGroups { get; } = new();

        public LevelEncounter SourceEncounter => sourceEncounter;

        public bool IsInitialized => sourceEncounter != null;
        public bool HasChangesFromSource => revision > 0;
        public bool HasUnappliedChanges => revision != appliedRevision;
        public bool HasUnsavedChanges => revision != savedRevision;

        public void CaptureFrom(LevelEncounter encounter)
        {
            sourceEncounter = encounter;

            Phases.Clear();
            UnphasedGroups.Clear();

            if (encounter == null)
            {
                DisplayName = string.Empty;
                Description = string.Empty;
                ResetRevisions();
                return;
            }

            DisplayName = encounter.AuthoredDisplayName;
            Description = encounter.AuthoredDescription;

            foreach (LevelEncounterPhase phase in encounter.Phases)
            {
                if (phase != null)
                    Phases.Add(new PhaseDraft(phase));
            }

            if (Phases.Count == 0)
            {
                foreach (LevelSpawnGroup group in GetUnphasedGroups(encounter))
                    UnphasedGroups.Add(new SpawnGroupDraft(group));
            }

            ResetRevisions();
        }

        public bool TryCaptureFromDefinition(
            LevelEncounter encounter,
            EncounterDefinition definition,
            out string error)
        {
            if (!EncounterDefinitionRuntimeApplicator.ValidateCompatibility(
                    definition,
                    encounter,
                    out error))
            {
                return false;
            }

            sourceEncounter = encounter;

            Phases.Clear();
            UnphasedGroups.Clear();

            DisplayName = definition.DisplayName;
            Description = definition.Description;

            if (definition.IsPhased)
            {
                for (int i = 0; i < definition.Phases.Count; i++)
                {
                    Phases.Add(
                        new PhaseDraft(
                            encounter.Phases[i],
                            definition.Phases[i]
                        )
                    );
                }
            }
            else
            {
                List<LevelSpawnGroup> groups = GetUnphasedGroups(encounter);

                for (int i = 0; i < definition.UnphasedGroups.Count; i++)
                {
                    UnphasedGroups.Add(
                        new SpawnGroupDraft(
                            groups[i],
                            definition.UnphasedGroups[i]
                        )
                    );
                }
            }

            ResetRevisions();
            return true;
        }

        public void MarkModified()
        {
            revision++;
        }

        public void MarkSaved()
        {
            savedRevision = revision;
        }

        public void ApplyRuntimeOverrides()
        {
            if (sourceEncounter == null)
                return;

            sourceEncounter.SetRuntimeIdentityOverride(DisplayName, Description);

            foreach (PhaseDraft phase in Phases)
                phase?.Apply();

            foreach (SpawnGroupDraft group in UnphasedGroups)
                group?.Apply();

            appliedRevision = revision;
        }

        public void ClearRuntimeOverrides()
        {
            sourceEncounter?.ClearRuntimeIdentityOverride();

            foreach (PhaseDraft phase in Phases)
                phase?.Clear();

            foreach (SpawnGroupDraft group in UnphasedGroups)
                group?.Clear();
        }

        public void WriteToDefinition(EncounterDefinition definition)
        {
            if (definition == null)
                return;

            List<EncounterDefinition.PhaseDefinition> phaseDefinitions = new();
            List<EncounterDefinition.SpawnGroupDefinition> unphasedDefinitions = new();

            foreach (PhaseDraft phase in Phases)
            {
                if (phase != null)
                    phaseDefinitions.Add(phase.CreateDefinition());
            }

            foreach (SpawnGroupDraft group in UnphasedGroups)
            {
                if (group != null)
                    unphasedDefinitions.Add(group.CreateDefinition());
            }

            definition.ReplaceContent(
                DisplayName,
                Description,
                phaseDefinitions,
                unphasedDefinitions
            );
        }

        public void CollectActors(List<ActorDefinition> destination)
        {
            if (destination == null)
                return;

            foreach (PhaseDraft phase in Phases)
            {
                foreach (SpawnGroupDraft group in phase.Groups)
                    CollectGroupActors(group, destination);
            }

            foreach (SpawnGroupDraft group in UnphasedGroups)
                CollectGroupActors(group, destination);
        }

        private void ResetRevisions()
        {
            revision = 0;
            appliedRevision = 0;
            savedRevision = 0;
        }

        private static void CollectGroupActors(
            SpawnGroupDraft group,
            List<ActorDefinition> destination)
        {
            foreach (SpawnEntryDraft entry in group.Entries)
            {
                ActorDefinition actor = entry?.Actor;

                if (actor != null && !destination.Contains(actor))
                    destination.Add(actor);
            }
        }

        private static List<LevelSpawnGroup> GetUnphasedGroups(LevelEncounter encounter)
        {
            List<LevelSpawnGroup> groups = new();

            foreach (LevelSpawnGroup group in encounter.SpawnGroups)
            {
                if (group == null)
                    continue;

                if (group.GetComponentInParent<LevelEncounterPhase>() == null)
                    groups.Add(group);
            }

            groups.Sort(
                (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex())
            );

            return groups;
        }
    }
}