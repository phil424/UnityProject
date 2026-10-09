using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentAuthoringPalette
    {
        public static readonly Color WeaponMastery = new(0.2f, 0.75f, 0.9f, 1f);
        public static readonly Color MasteryLevel = new(0.3f, 0.5f, 0.95f, 1f);
        public static readonly Color Ability = new(0.7f, 0.4f, 0.9f, 1f);
        public static readonly Color CharacterGrowth = new(0.35f, 0.75f, 0.4f, 1f);
        public static readonly Color Combination = new(0.95f, 0.55f, 0.2f, 1f);
        public static readonly Color General = new(0.6f, 0.6f, 0.6f, 1f);

        public static Color GetMasteryNodeColor(CharacterMasteryNodeDefinition node)
        {
            return node switch
            {
                CharacterMasteryWeaponModCombinationNodeDefinition => Combination,
                CharacterMasteryWeaponModNodeDefinition => WeaponMastery,
                CharacterMasteryWeaponModLevelNodeDefinition => MasteryLevel,
                CharacterMasteryAbilityLevelNodeDefinition => Ability,
                CharacterMasteryStatNodeDefinition => CharacterGrowth,
                _ => General,
            };
        }

        public static Color GetRewardVectorColor(RunRewardVector vector)
        {
            return vector switch
            {
                RunRewardVector.CharacterGrowth => CharacterGrowth,
                RunRewardVector.Ability => Ability,
                RunRewardVector.WeaponMod => WeaponMastery,
                _ => General,
            };
        }
    }
}
