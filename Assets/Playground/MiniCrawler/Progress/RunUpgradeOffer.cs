using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunUpgradeOffer
    {
        public PartyMemberDefinition Member { get; }

        public RunUpgradeDefinition Upgrade { get; }

        public bool IsValid =>
            Member != null &&
            Upgrade != null &&
            Upgrade.Amount > 0f;

        public RunUpgradeOffer(
            PartyMemberDefinition member,
            RunUpgradeDefinition upgrade
        )
        {
            Member = member;
            Upgrade = upgrade;
        }
    }
}