using MiniCrawler.Core;
using MiniCrawler.Progress;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal enum RunRewardPreviewEligibility
    {
        NoCharacter,
        Unconfigured,
        StartingBuild,
        FullMastery,
        Ineligible,
    }

    internal static class GameContentRewardPreviewUtility
    {
        public static RunRewardPreviewEligibility Evaluate(RunRewardDefinition reward, PartyMemberDefinition character)
        {
            if (reward == null)
                return RunRewardPreviewEligibility.Unconfigured;

            if (!reward.IsConfigured)
                return RunRewardPreviewEligibility.Unconfigured;

            if (character == null)
                return RunRewardPreviewEligibility.NoCharacter;

            RunBuild startingBuild = BuildPreview(character, false);

            if (reward.CanApply(character, startingBuild))
                return RunRewardPreviewEligibility.StartingBuild;

            RunBuild fullyMasteredBuild = BuildPreview(character, true);

            if (reward.CanApply(character, fullyMasteredBuild))
                return RunRewardPreviewEligibility.FullMastery;

            return RunRewardPreviewEligibility.Ineligible;
        }

        private static RunBuild BuildPreview(PartyMemberDefinition character, bool includeAllAuthoredMastery)
        {
            RunBuild build = new();

            build.InitializeStartingAbilities(character.StartingAbilities);

            if (!includeAllAuthoredMastery || character.MasteryDefinition == null)
            {
                return build;
            }

            foreach (CharacterMasteryNodeDefinition node in character.MasteryDefinition.Nodes)
            {
                if (node is CharacterMasteryWeaponModNodeDefinition && node.IsConfigured)
                {
                    node.ApplyPreparedBuild(build);
                }
            }

            foreach (CharacterMasteryNodeDefinition node in character.MasteryDefinition.Nodes)
            {
                if (node == null || !node.IsConfigured || node is CharacterMasteryWeaponModNodeDefinition || node is CharacterMasteryWeaponModCombinationNodeDefinition)
                {
                    continue;
                }

                node.ApplyPreparedBuild(build);
            }

            foreach (CharacterMasteryNodeDefinition node in character.MasteryDefinition.Nodes)
            {
                if (node is CharacterMasteryWeaponModCombinationNodeDefinition && node.IsConfigured)
                {
                    node.ApplyPreparedBuild(build);
                }
            }

            return build;
        }
    }
}
