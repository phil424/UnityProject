using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunUpgradeOffer
    {
        public PartyMemberDefinition Member { get; }

        public RunRewardDefinition Reward { get; }

        // Compatibility for existing stat-upgrade
        // callers while the reward architecture
        // becomes more generic.
        public RunUpgradeDefinition Upgrade =>
            Reward as RunUpgradeDefinition;

        public bool IsValid =>
            Member != null &&
            Reward != null &&
            Reward.IsConfigured;

        public RunUpgradeOffer(
            PartyMemberDefinition member,
            RunRewardDefinition reward
        )
        {
            Member =
                member;

            Reward =
                reward;
        }
    }
}