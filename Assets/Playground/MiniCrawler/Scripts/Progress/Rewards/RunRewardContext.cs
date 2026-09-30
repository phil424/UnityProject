namespace MiniCrawler.Progress
{
    public readonly struct RunRewardContext
    {
        public RunRewardVector RewardVector { get; }
        public string SourceLabel { get; }

        public bool HasSourceLabel => !string.IsNullOrWhiteSpace(SourceLabel);

        public RunRewardContext(RunRewardVector rewardVector, string sourceLabel = null)
        {
            RewardVector = rewardVector;
            SourceLabel = string.IsNullOrWhiteSpace(sourceLabel) ? string.Empty : sourceLabel.Trim();
        }
    }

    public static class RunRewardContextPresentation
    {
        public static string GetTitle(RunRewardContext context)
        {
            string rewardLabel = RunRewardVectorPresentation.GetDisplayName(context.RewardVector);
            return context.HasSourceLabel ? $"{context.SourceLabel} • {rewardLabel} REWARD" : $"{rewardLabel} REWARD";
        }
    }
}