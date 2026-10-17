using MiniCrawler.Abilities;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class PartyAbilityApplicator
    {
        public static void Apply(GameObject spawnedActor, RunBuild build)
        {
            if (spawnedActor == null || build == null)
                return;

            foreach (RunAbilityState abilityState in build.Abilities)
            {
                if (abilityState?.Definition == null)
                    continue;

                ActorAbility runtimeAbility = FindRuntimeAbility(spawnedActor, abilityState.Definition);

                if (runtimeAbility == null)
                {
                    runtimeAbility = abilityState.Definition.CreateRuntime(spawnedActor, abilityState.Level, abilityState.Evolutions);
                }
                else
                {
                    runtimeAbility.ApplyProgressionState(abilityState.Level, abilityState.Evolutions);
                }

                if (runtimeAbility == null)
                {
                    Debug.LogWarning($"Could not create runtime ability " + $"'{abilityState.Definition.DisplayName}' " + $"for '{spawnedActor.name}'.");

                    continue;
                }

                runtimeAbility.enabled = build.IsAbilityActive(abilityState.Definition);
            }
        }

        private static ActorAbility FindRuntimeAbility(GameObject actor, AbilityDefinition definition)
        {
            ActorAbility[] existingAbilities = actor.GetComponentsInChildren<ActorAbility>(true);

            foreach (ActorAbility ability in existingAbilities)
            {
                if (ability?.Definition != null && ability.Definition.Id == definition.Id)
                    return ability;
            }

            return null;
        }
    }
}
