using MiniCrawler.Abilities;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName =
            "New Ability Evolution Reward",
        menuName =
            "Mini Crawler/Run Rewards/" +
            "Ability Evolution"
    )]
    public class AbilityEvolutionRewardDefinition :
        RunRewardDefinition
    {
        [Header("Identity")]
        [SerializeField]
        private string id;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private Sprite icon;

        [Header("Rarity")]
        [SerializeField]
        private RunUpgradeRarity rarity =
            RunUpgradeRarity.Rare;

        [Header("Evolution")]
        [SerializeField]
        private AbilityEvolutionDefinition
            evolution;

        public AbilityEvolutionDefinition
            Evolution =>
                evolution;

        public override string Id =>
            string.IsNullOrWhiteSpace(id)
                ? name
                : id;

        public override string DisplayName
        {
            get
            {
                if (
                    !string.IsNullOrWhiteSpace(
                        displayName
                    )
                )
                {
                    return displayName;
                }

                if (evolution == null)
                    return name;

                return
                    evolution.DisplayName;
            }
        }

        public override string Description
        {
            get
            {
                if (evolution == null)
                    return "Transform an owned ability.";

                if (!string.IsNullOrWhiteSpace(evolution.Description))
                    return evolution.Description;

                AbilityDefinition ability = evolution.TargetAbility;

                return ability != null ? $"Evolve " + $"{ability.DisplayName}." : "Transform an owned ability.";
            }
        }

        public override Sprite Icon => icon != null ? icon : evolution != null && 
                            evolution.Icon != null ? evolution.Icon : evolution != null &&
                            evolution.TargetAbility != null ? evolution.TargetAbility.Icon : null;

        public override RunUpgradeRarity Rarity => rarity;

        public override bool IsConfigured => evolution != null && evolution.IsConfigured;
            
        public override bool AllowDuplicatePendingOffers => false;

        public override bool CanApply(PartyMemberDefinition member, RunBuild build)
        {
            if (member == null || build == null || evolution == null || !evolution.IsConfigured)
            {
                return false;
            }

            return build.CanAcquireAbilityEvolution(evolution);
        }

        public override bool TryApply(PartyMemberDefinition member, RunBuild build)
        {
            if (!CanApply(member, build))
                return false;

            return
                build.TryAcquireAbilityEvolution(evolution);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}