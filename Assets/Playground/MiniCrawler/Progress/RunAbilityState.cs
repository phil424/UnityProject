using System;
using MiniCrawler.Abilities;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class RunAbilityState
    {
        private AbilityDefinition definition;

        private int level;

        public AbilityDefinition Definition =>
            definition;

        public int Level =>
            level;

        public bool IsMaxLevel =>
            definition == null ||
            level >= definition.MaxLevel;

        public RunAbilityState(
            AbilityDefinition abilityDefinition,
            int startingLevel = 1
        )
        {
            definition =
                abilityDefinition;

            level =
                definition != null
                    ? definition.ClampLevel(
                        startingLevel
                    )
                    : 1;
        }

        public bool TryIncreaseLevel()
        {
            if (
                definition == null ||
                IsMaxLevel
            )
            {
                return false;
            }

            level =
                definition.ClampLevel(
                    level + 1
                );

            return true;
        }
    }
}