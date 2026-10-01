using MiniCrawler.Core;
using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Weapon Mod Level Reward",
        menuName = "Mini Crawler/Run Rewards/Weapon Mod Level"
    )]
    public sealed class WeaponModLevelRewardDefinition : RunRewardDefinition
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        [Header("Rarity")]
        [SerializeField] private RunUpgradeRarity rarity = RunUpgradeRarity.Rare;

        [Header("Weapon Mod")]
        [SerializeField] private WeaponModDefinition weaponMod;
        [SerializeField, Min(1)] private int levelIncrease = 1;

        public WeaponModDefinition WeaponMod => weaponMod;
        public int LevelIncrease => Mathf.Max(1, levelIncrease);

        public override string Id => string.IsNullOrWhiteSpace(id) ? name : id;

        public override string DisplayName =>
            !string.IsNullOrWhiteSpace(displayName)
                ? displayName
                : weaponMod != null
                    ? $"{weaponMod.DisplayName} Training"
                    : name;

        public override string Description =>
            weaponMod != null
                ? $"Increase {weaponMod.DisplayName}'s Expedition level by {LevelIncrease}."
                : "Increase a prepared Weapon Mod's Expedition level.";

        public override Sprite Icon => icon != null ? icon : weaponMod != null ? weaponMod.Icon : null;
        public override RunUpgradeRarity Rarity => rarity;
        public override RunRewardVector RewardVector => RunRewardVector.WeaponMod;
        public override bool IsConfigured => weaponMod != null && weaponMod.IsConfigured && levelIncrease > 0;
        public override bool AllowDuplicatePendingOffers => false;

        public override bool CanApply(PartyMemberDefinition member, RunBuild build)
        {
            return member != null &&
                   build != null &&
                   weaponMod != null &&
                   weaponMod.IsConfigured &&
                   weaponMod.IsCompatibleWith(member) &&
                   build.HasWeaponMod(weaponMod);
        }

        public override bool TryApply(PartyMemberDefinition member, RunBuild build)
        {
            if (!CanApply(member, build))
                return false;

            return build.TryIncreaseWeaponModLevel(weaponMod, LevelIncrease);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            levelIncrease = Mathf.Max(1, levelIncrease);
        }
    }
}