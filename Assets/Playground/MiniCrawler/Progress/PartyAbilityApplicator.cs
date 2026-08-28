using MiniCrawler.Abilities;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class PartyAbilityApplicator
    {
        public static void Apply(
            GameObject spawnedActor,
            RunBuild build
        )
        {
            if (
                spawnedActor == null ||
                build == null
            )
            {
                return;
            }

            foreach (
                RunAbilityState abilityState
                in build.Abilities
            )
            {
                if (
                    abilityState?.Definition == null
                )
                {
                    continue;
                }

                if (
                    HasRuntimeAbility(
                        spawnedActor,
                        abilityState.Definition
                    )
                )
                {
                    continue;
                }

                ActorAbility runtimeAbility =
                    abilityState.Definition
                        .CreateRuntime(
                            spawnedActor,
                            abilityState.Level
                        );

                if (runtimeAbility == null)
                {
                    Debug.LogWarning(
                        $"Could not create runtime " +
                        $"ability " +
                        $"'{abilityState.Definition.DisplayName}' " +
                        $"for '{spawnedActor.name}'."
                    );
                }
            }
        }

        private static bool HasRuntimeAbility(
            GameObject actor,
            AbilityDefinition definition
        )
        {
            ActorAbility[] existingAbilities =
                actor.GetComponentsInChildren<
                    ActorAbility
                >(true);

            foreach (
                ActorAbility ability
                in existingAbilities
            )
            {
                if (
                    ability != null &&
                    ability.Definition != null &&
                    ability.Definition.Id ==
                        definition.Id
                )
                {
                    return true;
                }
            }

            return false;
        }
    }
}