using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Character Mastery Weapon Mod Node",
        menuName = "Mini Crawler/Progression/Character Mastery Weapon Mod Node"
    )]
    public sealed class CharacterMasteryWeaponModNodeDefinition : CharacterMasteryNodeDefinition
    {
        [Header("Weapon Mastery")]
        [SerializeField] private WeaponModDefinition weaponMod;
        [SerializeField] private WeaponModStanceSlot stanceSlot = WeaponModStanceSlot.Core;

        public WeaponModDefinition WeaponMod => weaponMod;
        public WeaponModStanceSlot StanceSlot => stanceSlot;

        public override bool IsConfigured =>
            base.IsConfigured &&
            weaponMod != null &&
            weaponMod.IsConfigured;

        public override string EffectDescription
        {
            get
            {
                if (weaponMod == null)
                    return "Prepare a Weapon Mastery stance.";

                return stanceSlot == WeaponModStanceSlot.Core
                    ? $"Replace the original Basic Attack with {weaponMod.DisplayName} as this Character's Core Mastery."
                    : $"Prepare {weaponMod.DisplayName} as an alternate Weapon Mastery stance.";
            }
        }

        public override void ApplyPreparedBuild(RunBuild build)
        {
            if (build == null || weaponMod == null)
                return;

            build.TryPrepareWeaponMod(weaponMod, stanceSlot);
        }
    }
}