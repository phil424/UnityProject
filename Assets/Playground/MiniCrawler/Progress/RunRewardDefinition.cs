using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public enum RunUpgradeRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public abstract class RunRewardDefinition : ScriptableObject
    {
        public abstract string Id { get; }

        public abstract string DisplayName { get; }

        public abstract string Description { get; }

        public abstract Sprite Icon { get; }

        public abstract RunUpgradeRarity Rarity { get; }

        public abstract bool IsConfigured { get; }
        
        public virtual bool AllowDuplicatePendingOffers => true;

        public abstract bool CanApply(PartyMemberDefinition member, RunBuild build);

        public abstract bool TryApply(PartyMemberDefinition member, RunBuild build);
    }
}