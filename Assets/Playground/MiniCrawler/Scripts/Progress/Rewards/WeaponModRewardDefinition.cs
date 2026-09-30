using MiniCrawler.Core;
using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Weapon Mod Reward",
        menuName = "Mini Crawler/Run Rewards/Weapon Mod"
    )]
    public sealed class WeaponModRewardDefinition : RunRewardDefinition
    {
        [Header("Identity")]
        [SerializeField] private string id;

        [Header("Rarity")]
        [SerializeField] private RunUpgradeRarity rarity = RunUpgradeRarity.Rare;

        [Header("Weapon Mod")]
        [SerializeField] private WeaponModDefinition weaponMod;

        public WeaponModDefinition WeaponMod => weaponMod;

        public override string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public override string DisplayName => weaponMod != null ? weaponMod.DisplayName : name;
        public override string Description => weaponMod != null ? weaponMod.Description : "Acquire a Weapon Mod.";
        public override Sprite Icon => weaponMod != null ? weaponMod.Icon : null;
        public override RunUpgradeRarity Rarity => rarity;
        public override RunRewardVector RewardVector => RunRewardVector.WeaponMod;
        public override bool IsConfigured => weaponMod != null && weaponMod.IsConfigured;
        public override bool AllowDuplicatePendingOffers => false;

        public override bool CanApply(PartyMemberDefinition member, RunBuild build)
        {
            return member != null &&
                   build != null &&
                   weaponMod != null &&
                   weaponMod.IsConfigured &&
                   weaponMod.IsCompatibleWith(member) &&
                   !build.HasWeaponMod(weaponMod);
        }

        public override bool TryApply(PartyMemberDefinition member, RunBuild build)
        {
            if (!CanApply(member, build))
                return false;

            return build.TryAcquireWeaponMod(weaponMod);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}