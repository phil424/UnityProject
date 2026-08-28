using MiniCrawler.Abilities;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Ability Unlock Reward",
        menuName =
            "Mini Crawler/Run Rewards/Ability Unlock"
    )]
    public class AbilityUnlockRewardDefinition :
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
            RunUpgradeRarity.Common;

        [Header("Ability")]
        [SerializeField]
        private AbilityDefinition ability;

        [Header("Eligibility")]
        [Tooltip(
            "Leave empty to allow any party member."
        )]
        [SerializeField]
        private PartyMemberDefinition[]
            eligibleMembers;

        public AbilityDefinition Ability =>
            ability;

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

                return ability != null
                    ? $"Learn {ability.DisplayName}"
                    : name;
            }
        }

        public override string Description =>
            ability != null
                ? $"Acquire {ability.DisplayName}."
                : "Acquire a new ability.";

        public override Sprite Icon =>
            icon != null
                ? icon
                : ability != null
                    ? ability.Icon
                    : null;

        public override RunUpgradeRarity Rarity =>
            rarity;

        public override bool IsConfigured =>
            ability != null;

        public override bool CanApply(
            PartyMemberDefinition member,
            RunBuild build
        )
        {
            return
                member != null &&
                build != null &&
                ability != null &&
                IsEligibleMember(member) &&
                !build.HasAbility(ability);
        }

        public override bool TryApply(
            PartyMemberDefinition member,
            RunBuild build
        )
        {
            if (!CanApply(member, build))
                return false;

            return
                build.TryAcquireAbility(
                    ability
                );
        }

        private bool IsEligibleMember(
            PartyMemberDefinition member
        )
        {
            if (
                eligibleMembers == null ||
                eligibleMembers.Length == 0
            )
            {
                return true;
            }

            foreach (
                PartyMemberDefinition eligible
                in eligibleMembers
            )
            {
                if (
                    eligible != null &&
                    eligible.Id == member.Id
                )
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}