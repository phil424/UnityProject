using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class RunAbilityState
    {
        private readonly AbilityDefinition definition;
        private readonly CharacterAbilityRole preparedRole;
        private readonly List<AbilityEvolutionDefinition> evolutions = new();

        private int level;

        public AbilityDefinition Definition => definition;
        public CharacterAbilityRole PreparedRole => preparedRole;
        public bool IsPreparedCore => preparedRole != CharacterAbilityRole.Unassigned;

        public int Level => level;
        public IReadOnlyList<AbilityEvolutionDefinition> Evolutions => evolutions;

        public bool IsMaxLevel => definition == null || level >= definition.MaxLevel;

        public RunAbilityState(AbilityDefinition abilityDefinition, int startingLevel = 1, CharacterAbilityRole startingPreparedRole = CharacterAbilityRole.Unassigned)
        {
            definition = abilityDefinition;
            preparedRole = startingPreparedRole;
            level = definition != null ? definition.ClampLevel(startingLevel) : 1;
        }

        public bool TryIncreaseLevel()
        {
            if (definition == null || IsMaxLevel)
                return false;

            level = definition.ClampLevel(level + 1);
            return true;
        }

        internal void ConfigureLevel(int newLevel)
        {
            if (definition == null)
                return;

            level = definition.ClampLevel(newLevel);
        }

        public bool HasEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution == null)
                return false;

            foreach (AbilityEvolutionDefinition owned in evolutions)
            {
                if (owned != null && owned.Id == evolution.Id)
                    return true;
            }

            return false;
        }

        public bool CanAddEvolution(AbilityEvolutionDefinition evolution)
        {
            if (definition == null || evolution == null || !evolution.IsConfigured || evolution.TargetAbility == null || evolution.TargetAbility.Id != definition.Id || HasEvolution(evolution))
            {
                return false;
            }

            foreach (AbilityEvolutionDefinition required in evolution.RequiredEvolutions)
            {
                if (!HasEvolution(required))
                    return false;
            }

            return true;
        }

        public bool TryAddEvolution(AbilityEvolutionDefinition evolution)
        {
            if (!CanAddEvolution(evolution))
                return false;

            evolutions.Add(evolution);
            return true;
        }
    }
}
