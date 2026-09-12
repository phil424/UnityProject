using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Tools
{
    public sealed class EncounterWorkshopDraft
    {
        private EncounterDefinition runtimePreviewDefinition;
        
        private static PhaseDraft CreateDefaultPhase(
            int phaseNumber,
            ActorDefinition actor)
        {
            PhaseDraft phase = new(
                $"Phase {phaseNumber}",
                advanceAfterSeconds: 0f,
                advanceWhenCleared: true
            );

            SpawnGroupDraft group =
                new($"Phase {phaseNumber} Spawn Group");

            group.Entries.Add(
                new SpawnEntryDraft{
                    Actor = actor,
                    Count = 4,
                    StartDelay = 0f,
                    BatchSize = 4,
                    TimeBetweenSpawns = 0f,
                    TimeBetweenBatches = 0f
                }
            );

            group.Sources.Add(
                new SpawnSourceDraft(
                    $"Phase {phaseNumber} Spawn Source",
                    LevelSpawnSource.SpawnShape.Circle,
                    radius: 5f,
                    boxSize: new Vector2(3f, 3f)
                )
            );

            phase.Groups.Add(group);

            return phase;
        }
        
        public void CreateBasic(
            LevelEncounter encounter,
            ActorDefinition actor)
        {
            ClearRuntimeOverrides();

            sourceEncounter = encounter;

            DisplayName = "New Encounter";
            Description = string.Empty;

            Phases.Clear();
            UnphasedGroups.Clear();
            Rules.Clear();

            Phases.Add(
                CreateDefaultPhase(
                    phaseNumber: 1,
                    actor
                )
            );

            ResetRevisions();
            MarkModified();
        }
        
        public void AddPhase(ActorDefinition actor)
        {
            if (Phases.Count == 0 &&
                UnphasedGroups.Count > 0)
            {
                PhaseDraft converted = new(
                    "Phase 1",
                    advanceAfterSeconds: 0f,
                    advanceWhenCleared: true
                );

                foreach (SpawnGroupDraft group in UnphasedGroups)
                    converted.Groups.Add(new SpawnGroupDraft(group));

                Phases.Add(converted);
                UnphasedGroups.Clear();
            }

            if (Phases.Count > 0)
            {
                PhaseDraft previousFinal =
                    Phases[Phases.Count - 1];

                if (previousFinal.AdvanceAfterSeconds <= 0f)
                    previousFinal.AdvanceAfterSeconds = 8f;

                previousFinal.AdvanceWhenCleared = true;
            }

            Phases.Add(
                CreateDefaultPhase(
                    Phases.Count + 1,
                    actor
                )
            );

            MarkModified();
            NormalizeRuleTargets();
        }
        
        public void DuplicatePhase(int phaseIndex)
        {
            if (phaseIndex < 0 ||
                phaseIndex >= Phases.Count)
            {
                return;
            }

            PhaseDraft duplicate =
                new(Phases[phaseIndex]);

            duplicate.DisplayName =
                $"{duplicate.DisplayName} Copy";

            Phases.Insert(
                phaseIndex + 1,
                duplicate
            );

            MarkModified();
            NormalizeRuleTargets();
        }
        
        public bool RemovePhase(int phaseIndex)
        {
            if (Phases.Count <= 1)
                return false;

            if (phaseIndex < 0 ||
                phaseIndex >= Phases.Count)
            {
                return false;
            }

            Phases.RemoveAt(phaseIndex);

            PhaseDraft finalPhase =
                Phases[Phases.Count - 1];

            finalPhase.AdvanceAfterSeconds = 0f;

            MarkModified();
            NormalizeRuleTargets();

            return true;
        }
        
        private void NormalizeRuleTargets()
        {
            int phaseCount =
                Mathf.Max(1, Phases.Count);

            foreach (RuleDraft rule in Rules)
            {
                if (rule == null)
                    continue;

                rule.PhaseIndex = Mathf.Clamp(
                    rule.PhaseIndex,
                    0,
                    phaseCount - 1
                );

                foreach (RuleActionDraft action in rule.Actions)
                {
                    if (action == null)
                        continue;

                    action.PhaseIndex = Mathf.Clamp(
                        action.PhaseIndex,
                        0,
                        phaseCount - 1
                    );

                    if (Phases.Count <= 0)
                        continue;

                    int groupCount = Mathf.Max(
                        1,
                        Phases[action.PhaseIndex].Groups.Count
                    );

                    action.GroupIndex = Mathf.Clamp(
                        action.GroupIndex,
                        0,
                        groupCount - 1
                    );
                }
            }
        }

        public sealed class RuleActionDraft
        {
            public EncounterActionKind Kind { get; set; }
            public int PhaseIndex { get; set; }
            public int GroupIndex { get; set; }
            public string SignalId { get; set; }

            public RuleActionDraft()
            {
                Kind = EncounterActionKind.ActivateEncounterCombat;
            }

            public RuleActionDraft(EncounterActionDefinition source)
            {
                Kind = source.Kind;
                PhaseIndex = source.PhaseIndex;
                GroupIndex = source.GroupIndex;
                SignalId = source.SignalId;
            }

            public EncounterActionDefinition CreateDefinition()
            {
                return new EncounterActionDefinition(
                    Kind,
                    PhaseIndex,
                    GroupIndex,
                    SignalId
                );
            }
        }

        public sealed class RuleDraft
        {
            public string DisplayName { get; set; } = "Encounter Rule";
            public bool Enabled { get; set; } = true;

            public EncounterTriggerKind TriggerKind { get; set; } =
                EncounterTriggerKind.PartyProximity;

            public bool RequireEncounterAvailable { get; set; } = true;
            public float ProximityRadius { get; set; } = 5f;
            public Vector3 LocalOffset { get; set; }

            public float DelaySeconds { get; set; }
            public int PhaseIndex { get; set; }

            public List<RuleActionDraft> Actions { get; } = new();

            public RuleDraft()
            {
            }

            public RuleDraft(EncounterRuleDefinition source)
            {
                DisplayName = source.DisplayName;
                Enabled = source.Enabled;

                EncounterTriggerDefinition trigger = source.Trigger;

                if (trigger != null)
                {
                    TriggerKind = trigger.Kind;
                    RequireEncounterAvailable = trigger.RequireEncounterAvailable;
                    ProximityRadius = trigger.ProximityRadius;
                    LocalOffset = trigger.LocalOffset;
                    DelaySeconds = trigger.DelaySeconds;
                    PhaseIndex = trigger.PhaseIndex;
                }

                foreach (EncounterActionDefinition action in source.Actions)
                {
                    if (action != null)
                        Actions.Add(new RuleActionDraft(action));
                }
            }

            public EncounterRuleDefinition CreateDefinition()
            {
                EncounterTriggerDefinition trigger = new(
                    TriggerKind,
                    RequireEncounterAvailable,
                    ProximityRadius,
                    LocalOffset,
                    DelaySeconds,
                    PhaseIndex
                );

                List<EncounterActionDefinition> actions = new();

                foreach (RuleActionDraft action in Actions)
                {
                    if (action != null)
                        actions.Add(action.CreateDefinition());
                }

                return new EncounterRuleDefinition(
                    DisplayName,
                    Enabled,
                    trigger,
                    actions
                );
            }

            public static RuleDraft CreateApproachStartRule()
            {
                RuleDraft rule = new()
                {
                    DisplayName = "Approach Start",
                    TriggerKind = EncounterTriggerKind.PartyProximity,
                    RequireEncounterAvailable = true,
                    ProximityRadius = 6f
                };

                rule.Actions.Add(
                    new RuleActionDraft
                    {
                        Kind = EncounterActionKind.ActivateEncounterCombat
                    }
                );

                rule.Actions.Add(
                    new RuleActionDraft
                    {
                        Kind = EncounterActionKind.BeginEncounterSpawning
                    }
                );

                return rule;
            }
        }
    
        public sealed class SpawnEntryDraft
        {
            public ActorDefinition Actor { get; set; }
            public int Count { get; set; }
            public float StartDelay { get; set; }
            public int BatchSize { get; set; }
            public float TimeBetweenSpawns { get; set; }
            public float TimeBetweenBatches { get; set; }
            
            public SpawnEntryDraft()
            {
            }
            
            public SpawnEntryDraft(SpawnEntryDraft source)
            {
                Actor = source.Actor;
                Count = source.Count;
                StartDelay = source.StartDelay;
                BatchSize = source.BatchSize;
                TimeBetweenSpawns = source.TimeBetweenSpawns;
                TimeBetweenBatches = source.TimeBetweenBatches;
            }

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
            public LevelSpawnSource Source { get; private set; }

            public LevelSpawnSource.SpawnShape Shape { get; set; }
            public float Radius { get; set; }
            public Vector2 BoxSize { get; set; }

            public string Name => string.IsNullOrWhiteSpace(Label) ? "Spawn Source" : Label;
            public string Label { get; set; } = "Spawn Source";

            public SpawnSourceDraft(LevelSpawnSource source)
            {
                Source = source;
                Shape = source.AuthoredShape;
                Radius = source.AuthoredRadius;
                BoxSize = source.AuthoredBoxSize;
                
                Label = source != null ? source.gameObject.name : "Spawn Source";
            }

            public SpawnSourceDraft(
                LevelSpawnSource source,
                EncounterDefinition.SpawnSourceDefinition definition)
            {
                Source = source;
                Shape = definition.Shape;
                Radius = definition.Radius;
                BoxSize = definition.BoxSize;
                
                Label = string.IsNullOrWhiteSpace(definition.Label) ? "Spawn Source" : definition.Label;
            }
            
            public SpawnSourceDraft(
                string label,
                LevelSpawnSource.SpawnShape shape,
                float radius,
                Vector2 boxSize)
            {
                Label = label;
                Shape = shape;
                Radius = radius;
                BoxSize = boxSize;
            }
            
            public SpawnSourceDraft(SpawnSourceDraft source)
            {
                Label = source.Label;
                Shape = source.Shape;
                Radius = source.Radius;
                BoxSize = source.BoxSize;
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
            public LevelSpawnGroup Source { get; private set; }

            public List<SpawnEntryDraft> Entries { get; } = new();
            public List<SpawnSourceDraft> Sources { get; } = new();

            public string Name => string.IsNullOrWhiteSpace(Label) ? "Spawn Group" : Label;
            
            public string Label { get; set; } = "Spawn Group";
            
            public SpawnGroupDraft(string label)
            {
                Label = label;
            }
            
            public SpawnGroupDraft(SpawnGroupDraft source)
            {
                Label = source.Label;

                foreach (SpawnEntryDraft entry in source.Entries)
                {
                    if (entry != null)
                        Entries.Add(new SpawnEntryDraft(entry));
                }

                foreach (SpawnSourceDraft spawnSource in source.Sources)
                {
                    if (spawnSource != null)
                        Sources.Add(new SpawnSourceDraft(spawnSource));
                }
            }
            
            public SpawnGroupDraft(
                EncounterDefinition.SpawnGroupDefinition definition)
            {
                Label = string.IsNullOrWhiteSpace(definition.Label)
                    ? "Spawn Group"
                    : definition.Label;

                foreach (EncounterDefinition.SpawnEntryDefinition entry
                        in definition.Entries)
                {
                    if (entry != null)
                        Entries.Add(new SpawnEntryDraft(entry));
                }

                foreach (EncounterDefinition.SpawnSourceDefinition source
                        in definition.SpawnSources)
                {
                    if (source == null)
                        continue;

                    Sources.Add(
                        new SpawnSourceDraft(
                            source.Label,
                            source.Shape,
                            source.Radius,
                            source.BoxSize
                        )
                    );
                }
            }

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
                
                Label = source != null ? source.gameObject.name : "Spawn Group";
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
                
                Label = string.IsNullOrWhiteSpace(definition.Label) ? "Spawn Group" : definition.Label;
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
            public LevelEncounterPhase Source { get; private set; }

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
            
            public PhaseDraft(PhaseDraft source)
            {
                DisplayName = source.DisplayName;
                AdvanceAfterSeconds = source.AdvanceAfterSeconds;
                AdvanceWhenCleared = source.AdvanceWhenCleared;

                foreach (SpawnGroupDraft group in source.Groups)
                {
                    if (group != null)
                        Groups.Add(new SpawnGroupDraft(group));
                }
            }
            
            public PhaseDraft(
                string displayName,
                float advanceAfterSeconds,
                bool advanceWhenCleared)
            {
                DisplayName = displayName;
                AdvanceAfterSeconds = advanceAfterSeconds;
                AdvanceWhenCleared = advanceWhenCleared;
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
        public List<RuleDraft> Rules { get; } = new();

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
            Rules.Clear();

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
            if (!EncounterDefinitionRuntimeApplicator.ValidateDefinition(
                    definition,
                    encounter,
                    out error))
            {
                return false;
            }

            sourceEncounter = encounter;

            Phases.Clear();
            UnphasedGroups.Clear();
            Rules.Clear();

            DisplayName = definition.DisplayName;
            Description = definition.Description;

            if (definition.IsPhased)
            {
                foreach (EncounterDefinition.PhaseDefinition phaseDefinition
                        in definition.Phases)
                {
                    PhaseDraft phase = new(
                        phaseDefinition.DisplayName,
                        phaseDefinition.AdvanceAfterSeconds,
                        phaseDefinition.AdvanceWhenCleared
                    );

                    foreach (EncounterDefinition.SpawnGroupDefinition groupDefinition
                            in phaseDefinition.SpawnGroups)
                    {
                        phase.Groups.Add(
                            new SpawnGroupDraft(groupDefinition)
                        );
                    }

                    Phases.Add(phase);
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
            
            foreach (EncounterRuleDefinition rule in definition.Rules)
            {
                if (rule != null)
                    Rules.Add(new RuleDraft(rule));
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

            if (runtimePreviewDefinition != null)
            {
                UnityEngine.Object.Destroy(
                    runtimePreviewDefinition
                );
            }

            runtimePreviewDefinition =
                ScriptableObject.CreateInstance<EncounterDefinition>();

            runtimePreviewDefinition.name =
                "Workshop Runtime Preview";

            WriteToDefinition(runtimePreviewDefinition);

            runtimePreviewDefinition.EnsureId(
                "workshop-runtime-preview"
            );

            if (!EncounterDefinitionRuntimeApplicator.TryApply(
                    runtimePreviewDefinition,
                    sourceEncounter,
                    out string error))
            {
                Debug.LogError(
                    $"Could not apply Workshop Draft: {error}",
                    sourceEncounter
                );

                return;
            }

            appliedRevision = revision;
        }

        public void ClearRuntimeOverrides()
        {
            if (sourceEncounter != null)
            {
                EncounterRuleRunner.ClearFrom(
                    sourceEncounter
                );

                EncounterRuntimeContentBuilder.Clear(
                    sourceEncounter
                );

                sourceEncounter.ClearRuntimeIdentityOverride();
            }

            if (runtimePreviewDefinition != null)
            {
                UnityEngine.Object.Destroy(
                    runtimePreviewDefinition
                );

                runtimePreviewDefinition = null;
            }
        }

        public void WriteToDefinition(EncounterDefinition definition)
        {
            if (definition == null)
                return;

            List<EncounterDefinition.PhaseDefinition> phaseDefinitions = new();
            List<EncounterDefinition.SpawnGroupDefinition> unphasedDefinitions = new();
            List<EncounterRuleDefinition> ruleDefinitions = new();

            foreach (RuleDraft rule in Rules)
            {
                if (rule != null)
                    ruleDefinitions.Add(rule.CreateDefinition());
            }

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
                unphasedDefinitions,
                ruleDefinitions
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