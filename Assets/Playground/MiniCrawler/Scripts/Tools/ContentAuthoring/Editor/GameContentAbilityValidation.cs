using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentAbilityValidation
    {
        public static void Collect(IReadOnlyList<AbilityDefinition> abilities, IReadOnlyList<AbilityEvolutionDefinition> evolutions, List<string> output)
        {
            output.Clear();

            Dictionary<string, AbilityDefinition> abilityIds = new(StringComparer.Ordinal);

            HashSet<AbilityDefinition> abilitySet = new();

            foreach (AbilityDefinition ability in abilities)
            {
                if (ability == null)
                    continue;

                abilitySet.Add(ability);

                if (abilityIds.TryGetValue(ability.Id, out AbilityDefinition existing) && existing != ability)
                {
                    output.Add($"Abilities '{existing.DisplayName}' and " + $"'{ability.DisplayName}' share ID '{ability.Id}'.");
                }
                else
                {
                    abilityIds[ability.Id] = ability;
                }

                if (ability.RuntimePrefab == null)
                {
                    output.Add($"Ability '{ability.DisplayName}' has no runtime prefab.");

                    continue;
                }

                int runtimeCount = GameContentAbilityUtility.GetRuntimeBehaviourCount(ability);

                if (runtimeCount != 1)
                {
                    output.Add($"Ability '{ability.DisplayName}' runtime prefab must contain " + $"exactly one root ActorAbility; currently {runtimeCount}.");
                }
            }

            Dictionary<string, AbilityEvolutionDefinition> evolutionIds = new(StringComparer.Ordinal);

            foreach (AbilityEvolutionDefinition evolution in evolutions)
            {
                if (evolution == null)
                    continue;

                if (evolutionIds.TryGetValue(evolution.Id, out AbilityEvolutionDefinition existing) && existing != evolution)
                {
                    output.Add($"Ability Evolutions '{existing.DisplayName}' and " + $"'{evolution.DisplayName}' share ID '{evolution.Id}'.");
                }
                else
                {
                    evolutionIds[evolution.Id] = evolution;
                }

                if (!evolution.IsConfigured)
                {
                    output.Add($"Ability Evolution '{evolution.DisplayName}' is not fully configured.");

                    continue;
                }

                if (!abilitySet.Contains(evolution.TargetAbility))
                {
                    output.Add($"Ability Evolution '{evolution.DisplayName}' targets an Ability " + "outside the authored Ability library.");
                }
            }
        }
    }
}
