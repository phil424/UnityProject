using System.Collections.Generic;
using MiniCrawler.Spawning;

namespace MiniCrawler.Encounters
{
    public static class EncounterDefinitionRuntimeApplicator
    {
        public static bool TryApply(
            EncounterDefinition definition,
            LevelEncounter encounter,
            out string error)
        {
            if (!ValidateCompatibility(definition, encounter, out error))
                return false;

            encounter.SetRuntimeIdentityOverride(
                definition.DisplayName,
                definition.Description
            );

            if (definition.IsPhased)
            {
                for (int phaseIndex = 0; phaseIndex < definition.Phases.Count; phaseIndex++)
                {
                    EncounterDefinition.PhaseDefinition phaseDefinition =
                        definition.Phases[phaseIndex];

                    LevelEncounterPhase phase = encounter.Phases[phaseIndex];

                    phase.SetRuntimeAuthoringOverride(
                        phaseDefinition.DisplayName,
                        phaseDefinition.AdvanceAfterSeconds,
                        phaseDefinition.AdvanceWhenCleared
                    );

                    for (int groupIndex = 0;
                         groupIndex < phaseDefinition.SpawnGroups.Count;
                         groupIndex++)
                    {
                        ApplyGroup(
                            phaseDefinition.SpawnGroups[groupIndex],
                            phase.SpawnGroups[groupIndex]
                        );
                    }
                }

                return true;
            }

            List<LevelSpawnGroup> unphasedGroups = GetUnphasedGroups(encounter);

            for (int i = 0; i < definition.UnphasedGroups.Count; i++)
                ApplyGroup(definition.UnphasedGroups[i], unphasedGroups[i]);

            return true;
        }

        public static bool ValidateCompatibility(
            EncounterDefinition definition,
            LevelEncounter encounter,
            out string error)
        {
            error = string.Empty;

            if (definition == null)
            {
                error = "Encounter Definition is missing.";
                return false;
            }

            if (encounter == null)
            {
                error = "LevelEncounter is missing.";
                return false;
            }

            if (definition.Phases.Count > 0 && definition.UnphasedGroups.Count > 0)
            {
                error =
                    $"Encounter Definition '{definition.name}' contains both phased " +
                    "and unphased content. J6 expects one structure or the other.";

                return false;
            }

            if (definition.IsPhased)
            {
                if (encounter.PhaseCount != definition.Phases.Count)
                {
                    error =
                        $"Definition '{definition.name}' has {definition.Phases.Count} phases, " +
                        $"but encounter '{encounter.name}' has {encounter.PhaseCount}.";
                    return false;
                }

                for (int phaseIndex = 0; phaseIndex < definition.Phases.Count; phaseIndex++)
                {
                    EncounterDefinition.PhaseDefinition definitionPhase =
                        definition.Phases[phaseIndex];

                    LevelEncounterPhase runtimePhase = encounter.Phases[phaseIndex];

                    if (runtimePhase.SpawnGroups.Count != definitionPhase.SpawnGroups.Count)
                    {
                        error =
                            $"Phase {phaseIndex + 1} has " +
                            $"{definitionPhase.SpawnGroups.Count} definition groups but " +
                            $"{runtimePhase.SpawnGroups.Count} scene groups.";

                        return false;
                    }

                    for (int groupIndex = 0;
                         groupIndex < definitionPhase.SpawnGroups.Count;
                         groupIndex++)
                    {
                        if (!ValidateGroup(
                                definitionPhase.SpawnGroups[groupIndex],
                                runtimePhase.SpawnGroups[groupIndex],
                                phaseIndex,
                                groupIndex,
                                out error))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (encounter.PhaseCount > 0)
            {
                error =
                    $"Definition '{definition.name}' is unphased but encounter " +
                    $"'{encounter.name}' contains {encounter.PhaseCount} phases.";

                return false;
            }

            List<LevelSpawnGroup> unphasedGroups = GetUnphasedGroups(encounter);

            if (unphasedGroups.Count != definition.UnphasedGroups.Count)
            {
                error =
                    $"Definition '{definition.name}' has {definition.UnphasedGroups.Count} " +
                    $"unphased groups but encounter '{encounter.name}' has " +
                    $"{unphasedGroups.Count}.";

                return false;
            }

            for (int i = 0; i < definition.UnphasedGroups.Count; i++)
            {
                if (!ValidateGroup(
                        definition.UnphasedGroups[i],
                        unphasedGroups[i],
                        -1,
                        i,
                        out error))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateGroup(
            EncounterDefinition.SpawnGroupDefinition definition,
            LevelSpawnGroup group,
            int phaseIndex,
            int groupIndex,
            out string error)
        {
            error = string.Empty;

            if (definition == null || group == null)
            {
                error = $"Encounter group mapping {groupIndex + 1} is incomplete.";
                return false;
            }

            if (group.SpawnSources.Count != definition.SpawnSources.Count)
            {
                string phaseLabel =
                    phaseIndex >= 0 ? $"phase {phaseIndex + 1}, " : string.Empty;

                error =
                    $"{phaseLabel}group {groupIndex + 1} has " +
                    $"{definition.SpawnSources.Count} definition spawn sources but " +
                    $"{group.SpawnSources.Count} scene spawn sources.";

                return false;
            }

            return true;
        }

        private static void ApplyGroup(
            EncounterDefinition.SpawnGroupDefinition definition,
            LevelSpawnGroup group)
        {
            List<LevelSpawnGroup.SpawnEntry> runtimeEntries = new();

            foreach (EncounterDefinition.SpawnEntryDefinition entry in definition.Entries)
            {
                if (entry == null)
                    continue;

                runtimeEntries.Add(
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

            group.SetRuntimeScheduleOverride(runtimeEntries);

            for (int sourceIndex = 0;
                 sourceIndex < definition.SpawnSources.Count;
                 sourceIndex++)
            {
                EncounterDefinition.SpawnSourceDefinition sourceDefinition =
                    definition.SpawnSources[sourceIndex];

                LevelSpawnSource source = group.SpawnSources[sourceIndex];

                source.SetRuntimeConfigurationOverride(
                    sourceDefinition.Shape,
                    sourceDefinition.Radius,
                    sourceDefinition.BoxSize
                );
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