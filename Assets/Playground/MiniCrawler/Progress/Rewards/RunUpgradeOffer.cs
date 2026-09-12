using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunUpgradeOffer
    {
        public PartyMemberDefinition Member { get; }
        public RunRewardDefinition Reward { get; }

        public bool IsValid => Member != null && Reward != null && Reward.IsConfigured;

        public RunUpgradeOffer(PartyMemberDefinition member, RunRewardDefinition reward)
        {
            Member = member;
            Reward = reward;
        }
    }
}