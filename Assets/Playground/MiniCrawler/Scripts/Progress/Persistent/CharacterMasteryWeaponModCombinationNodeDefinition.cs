using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Character Mastery Weapon Mod Combination Node",
        menuName = "Mini Crawler/Progression/Character Mastery Weapon Mod Combination Node"
    )]
    public sealed class CharacterMasteryWeaponModCombinationNodeDefinition : CharacterMasteryNodeDefinition
    {
        [Header("Weapon Mastery Combination")]
        [SerializeField] private CharacterMasteryWeaponModNodeDefinition activeMastery;
        [SerializeField] private CharacterMasteryWeaponModNodeDefinition combinedMastery;

        public CharacterMasteryWeaponModNodeDefinition ActiveMastery => activeMastery;
        public CharacterMasteryWeaponModNodeDefinition CombinedMastery => combinedMastery;

        public override bool IsConfigured =>
            base.IsConfigured &&
            activeMastery != null &&
            combinedMastery != null &&
            activeMastery != combinedMastery &&
            activeMastery.WeaponMod != null &&
            combinedMastery.WeaponMod != null &&
            activeMastery.WeaponMod.Id != combinedMastery.WeaponMod.Id;

        public override string EffectDescription
        {
            get
            {
                if (activeMastery?.WeaponMod == null || combinedMastery?.WeaponMod == null)
                    return "Combine two Weapon Masteries.";

                return
                    $"{activeMastery.WeaponMod.DisplayName} inherits designed behaviour " +
                    $"from {combinedMastery.WeaponMod.DisplayName}.";
            }
        }

        public override void ApplyPreparedBuild(RunBuild build)
        {
            if (build == null || !IsConfigured)
                return;

            // Keep application independent from MasteryNodes list order.
            activeMastery.ApplyPreparedBuild(build);
            combinedMastery.ApplyPreparedBuild(build);

            build.TryCombineWeaponMods(
                activeMastery.WeaponMod,
                combinedMastery.WeaponMod
            );
        }
    }
}