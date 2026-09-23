using MiniCrawler.Encounters;

namespace MiniCrawler.Progress
{
    public static class ExpeditionObjectiveMatching
    {
        public static bool MatchesEncounterCompletion(
            ExpeditionObjectiveState objective,
            LevelEncounter encounter)
        {
            if (objective == null || objective.IsComplete || encounter == null)
                return false;

            return objective.Definition is EncounterCompletionObjectiveDefinition definition &&
                   definition.Matches(encounter);
        }

        public static bool IsEncounterRelevant(LevelEncounter encounter)
        {
            if (!RunProgress.HasActiveRun || encounter == null)
                return false;

            foreach (ExpeditionObjectiveState objective in RunProgress.Objectives)
            {
                if (MatchesEncounterCompletion(objective, encounter))
                    return true;
            }

            return false;
        }
    }
}