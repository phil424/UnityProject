using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    public static class AbilityPresentationResolver
    {
        public static string GetDisplayName(
            AbilityDefinition ability,
            IReadOnlyList<
                AbilityEvolutionDefinition
            > evolutions
        )
        {
            AbilityEvolutionDefinition
                presentationEvolution =
                    GetPresentationEvolution(
                        evolutions
                    );

            if (presentationEvolution != null)
            {
                return
                    presentationEvolution
                        .DisplayName;
            }

            return
                ability != null
                    ? ability.DisplayName
                    : string.Empty;
        }

        public static Sprite GetDisplayIcon(
            AbilityDefinition ability,
            IReadOnlyList<
                AbilityEvolutionDefinition
            > evolutions
        )
        {
            AbilityEvolutionDefinition
                presentationEvolution =
                    GetPresentationEvolution(
                        evolutions
                    );

            if (
                presentationEvolution != null &&
                presentationEvolution.Icon != null
            )
            {
                return
                    presentationEvolution.Icon;
            }

            return
                ability != null
                    ? ability.Icon
                    : null;
        }

        private static
            AbilityEvolutionDefinition
            GetPresentationEvolution(
                IReadOnlyList<
                    AbilityEvolutionDefinition
                > evolutions
            )
        {
            if (evolutions == null)
                return null;

            for (
                int index =
                    evolutions.Count - 1;
                index >= 0;
                index--
            )
            {
                AbilityEvolutionDefinition
                    evolution =
                        evolutions[index];

                if (
                    evolution != null &&
                    evolution
                        .ReplacesAbilityPresentation
                )
                {
                    return evolution;
                }
            }

            return null;
        }
    }
}