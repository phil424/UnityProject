using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class RunAbilityState
    {
        private AbilityDefinition definition;

        private int level;

        private readonly List<
            AbilityEvolutionDefinition
        > evolutions = new();

        public AbilityDefinition Definition =>
            definition;

        public int Level =>
            level;

        public IReadOnlyList<
            AbilityEvolutionDefinition
        > Evolutions =>
            evolutions;

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

        public bool HasEvolution(
            AbilityEvolutionDefinition evolution
        )
        {
            if (evolution == null)
                return false;

            foreach (
                AbilityEvolutionDefinition owned
                    in evolutions
            )
            {
                if (
                    owned != null &&
                    owned.Id == evolution.Id
                )
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanAddEvolution(
            AbilityEvolutionDefinition evolution
        )
        {
            if (
                definition == null ||
                evolution == null ||
                !evolution.IsConfigured ||
                evolution.TargetAbility == null ||
                evolution.TargetAbility.Id !=
                    definition.Id ||
                HasEvolution(evolution)
            )
            {
                return false;
            }

            foreach (
                AbilityEvolutionDefinition required
                    in evolution.RequiredEvolutions
            )
            {
                if (!HasEvolution(required))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryAddEvolution(
            AbilityEvolutionDefinition evolution
        )
        {
            if (!CanAddEvolution(evolution))
                return false;

            evolutions.Add(
                evolution
            );

            return true;
        }
    }
}