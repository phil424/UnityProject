using System;

namespace MiniCrawler.Progress
{
    public sealed class ExpeditionObjectiveState
    {
        public ExpeditionObjectiveDefinition Definition { get; }
        public int Progress { get; private set; }

        public int RequiredProgress => Definition != null ? Definition.RequiredProgress : 0;
        public bool IsComplete => Definition != null && Progress >= RequiredProgress;

        public ExpeditionObjectiveState(ExpeditionObjectiveDefinition definition)
        {
            Definition = definition;
        }

        public bool AddProgress(int amount)
        {
            if (Definition == null || amount <= 0 || IsComplete)
                return false;

            int nextProgress = Math.Min(RequiredProgress, Progress + amount);

            if (nextProgress == Progress)
                return false;

            Progress = nextProgress;
            return true;
        }
    }
}