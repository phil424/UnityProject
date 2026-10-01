using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Character Mastery Weapon Mod Level Node",
        menuName = "Mini Crawler/Progression/Character Mastery Weapon Mod Level Node"
    )]
    public sealed class CharacterMasteryWeaponModLevelNodeDefinition : CharacterMasteryNodeDefinition
    {
        [Header("Weapon Mastery Level")]
        [SerializeField] private WeaponModDefinition weaponMod;
        [SerializeField, Min(1)] private int preparedLevelIncrease = 1;

        public WeaponModDefinition WeaponMod => weaponMod;
        public int PreparedLevelIncrease => Mathf.Max(1, preparedLevelIncrease);

        public override bool IsConfigured =>
            base.IsConfigured &&
            weaponMod != null &&
            weaponMod.IsConfigured &&
            preparedLevelIncrease > 0;

        public override string EffectDescription =>
            weaponMod != null
                ? $"Permanently increase {weaponMod.DisplayName}'s starting Mastery level by {PreparedLevelIncrease}."
                : "Permanently increase a Weapon Mastery's starting level.";

        public override void ApplyPreparedBuild(RunBuild build)
        {
            if (build == null || weaponMod == null)
                return;

            build.TryIncreasePreparedWeaponModLevel(weaponMod, PreparedLevelIncrease);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            preparedLevelIncrease = Mathf.Max(1, preparedLevelIncrease);
        }
    }
}