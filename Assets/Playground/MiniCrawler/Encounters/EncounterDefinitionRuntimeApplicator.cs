namespace MiniCrawler.Encounters
{
    public static class EncounterDefinitionRuntimeApplicator
    {
        public static bool TryApply(
            EncounterDefinition definition,
            LevelEncounter encounter,
            out string error)
        {
            if (!ValidateDefinition(
                    definition,
                    encounter,
                    out error))
            {
                return false;
            }

            if (!EncounterRuntimeContentBuilder.Rebuild(
                    encounter,
                    definition,
                    out error))
            {
                return false;
            }

            encounter.SetRuntimeIdentityOverride(
                definition.DisplayName,
                definition.Description
            );

            EncounterRuleRunner.ApplyTo(
                encounter,
                definition.Rules
            );

            encounter.RefreshRuntimeContent();

            return true;
        }

        public static bool ValidateDefinition(
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

            if (definition.Phases.Count > 0 &&
                definition.UnphasedGroups.Count > 0)
            {
                error =
                    $"Encounter Definition '{definition.name}' contains " +
                    "both phased and unphased content.";

                return false;
            }

            if (definition.Phases.Count == 0 &&
                definition.UnphasedGroups.Count == 0)
            {
                error =
                    $"Encounter Definition '{definition.name}' contains " +
                    "no encounter content.";

                return false;
            }

            return true;
        }

        // Compatibility alias so any remaining J6/J7 call sites
        // continue compiling while the old structural limitation disappears.
        public static bool ValidateCompatibility(
            EncounterDefinition definition,
            LevelEncounter encounter,
            out string error)
        {
            return ValidateDefinition(
                definition,
                encounter,
                out error
            );
        }
    }
}