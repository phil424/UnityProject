using System.Collections.Generic;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    public static class EncounterRuntimeContentBuilder
    {
        private const string RuntimeRootName =
            "[Runtime Encounter Content]";

        public static bool Rebuild(
            LevelEncounter encounter,
            EncounterDefinition definition,
            out string error)
        {
            error = string.Empty;

            if (encounter == null)
            {
                error = "LevelEncounter is missing.";
                return false;
            }

            if (definition == null)
            {
                error = "EncounterDefinition is missing.";
                return false;
            }

            if (definition.Phases.Count > 0 &&
                definition.UnphasedGroups.Count > 0)
            {
                error =
                    "EncounterDefinition cannot contain both phased " +
                    "and unphased content.";

                return false;
            }

            DetachPreviousRuntimeContent(encounter);

            GameObject rootObject =
                new(RuntimeRootName);

            Transform root = rootObject.transform;

            root.SetParent(encounter.transform, false);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            root.localScale = Vector3.one;

            if (definition.IsPhased)
            {
                BuildPhases(
                    root,
                    definition.Phases
                );
            }
            else
            {
                BuildGroups(
                    root,
                    definition.UnphasedGroups
                );
            }

            encounter.SetRuntimeContentRoot(root);

            return true;
        }

        public static void Clear(LevelEncounter encounter)
        {
            if (encounter == null)
                return;

            Transform oldRoot =
                encounter.RuntimeContentRoot;

            if (oldRoot == null)
                return;

            // Detach first so RefreshOwnedContent cannot rediscover
            // the object while Unity waits until end-of-frame to Destroy.
            oldRoot.SetParent(null);

            encounter.ClearRuntimeContentRoot();

            Object.Destroy(oldRoot.gameObject);
        }

        private static void DetachPreviousRuntimeContent(
            LevelEncounter encounter)
        {
            Transform oldRoot =
                encounter.RuntimeContentRoot;

            if (oldRoot == null)
                return;

            oldRoot.SetParent(null);
            encounter.ClearRuntimeContentRoot();

            Object.Destroy(oldRoot.gameObject);
        }

        private static void BuildPhases(
            Transform root,
            IReadOnlyList<EncounterDefinition.PhaseDefinition> phases)
        {
            for (int phaseIndex = 0;
                 phaseIndex < phases.Count;
                 phaseIndex++)
            {
                EncounterDefinition.PhaseDefinition definition =
                    phases[phaseIndex];

                if (definition == null)
                    continue;

                string phaseName =
                    string.IsNullOrWhiteSpace(definition.DisplayName)
                        ? $"Phase {phaseIndex + 1}"
                        : definition.DisplayName;

                GameObject phaseObject =
                    new($"Phase {phaseIndex + 1} - {phaseName}");

                phaseObject.transform.SetParent(root, false);

                LevelEncounterPhase phase =
                    phaseObject.AddComponent<LevelEncounterPhase>();

                phase.SetRuntimeAuthoringOverride(
                    definition.DisplayName,
                    definition.AdvanceAfterSeconds,
                    definition.AdvanceWhenCleared
                );

                BuildGroups(
                    phaseObject.transform,
                    definition.SpawnGroups
                );
            }
        }

        private static void BuildGroups(
            Transform parent,
            IReadOnlyList<EncounterDefinition.SpawnGroupDefinition> groups)
        {
            for (int groupIndex = 0;
                 groupIndex < groups.Count;
                 groupIndex++)
            {
                EncounterDefinition.SpawnGroupDefinition definition =
                    groups[groupIndex];

                if (definition == null)
                    continue;

                string groupName =
                    string.IsNullOrWhiteSpace(definition.Label)
                        ? $"Spawn Group {groupIndex + 1}"
                        : definition.Label;

                GameObject groupObject =
                    new(groupName);

                groupObject.transform.SetParent(parent, false);

                LevelSpawnGroup group =
                    groupObject.AddComponent<LevelSpawnGroup>();

                // Definition content should not automatically spawn
                // merely because the component's authored default is true.
                group.SetRuntimeStartBehaviorOverride(
                    shouldStartSpawning: false,
                    shouldStartCombatActive: false
                );

                List<LevelSpawnGroup.SpawnEntry> entries = new();

                foreach (EncounterDefinition.SpawnEntryDefinition entry
                         in definition.Entries)
                {
                    if (entry == null)
                        continue;

                    entries.Add(
                        new LevelSpawnGroup.SpawnEntry(
                            entry.Actor,
                            entry.Count,
                            entry.StartDelay,
                            entry.BatchSize,
                            entry.TimeBetweenSpawns,
                            entry.TimeBetweenBatches
                        )
                    );
                }

                group.SetRuntimeScheduleOverride(entries);

                BuildSpawnSources(
                    groupObject.transform,
                    definition.SpawnSources
                );
            }
        }

        private static void BuildSpawnSources(
            Transform parent,
            IReadOnlyList<EncounterDefinition.SpawnSourceDefinition> sources)
        {
            for (int sourceIndex = 0;
                 sourceIndex < sources.Count;
                 sourceIndex++)
            {
                EncounterDefinition.SpawnSourceDefinition definition =
                    sources[sourceIndex];

                if (definition == null)
                    continue;

                string sourceName =
                    string.IsNullOrWhiteSpace(definition.Label)
                        ? $"Spawn Source {sourceIndex + 1}"
                        : definition.Label;

                GameObject sourceObject =
                    new(sourceName);

                sourceObject.transform.SetParent(parent, false);

                LevelSpawnSource source =
                    sourceObject.AddComponent<LevelSpawnSource>();

                source.SetRuntimeConfigurationOverride(
                    definition.Shape,
                    definition.Radius,
                    definition.BoxSize
                );
            }
        }
    }
}