using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal readonly struct GameContentAbilityCharacterUsage
    {
        public GameContentAbilityCharacterUsage(PartyMemberDefinition character, CharacterAbilityRole role, int startingLevel)
        {
            Character = character;
            Role = role;
            StartingLevel = startingLevel;
        }

        public PartyMemberDefinition Character { get; }
        public CharacterAbilityRole Role { get; }
        public int StartingLevel { get; }
    }

    internal readonly struct GameContentAbilityMasteryUsage
    {
        public GameContentAbilityMasteryUsage(PartyMemberDefinition character, CharacterMasteryAbilityLevelNodeDefinition node)
        {
            Character = character;
            Node = node;
        }

        public PartyMemberDefinition Character { get; }
        public CharacterMasteryAbilityLevelNodeDefinition Node { get; }
    }

    internal static class GameContentAbilityUtility
    {
        public static void CollectAbilities(List<AbilityDefinition> output)
        {
            output.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AbilityDefinition)}", new[] { GameContentAssetUtility.AbilityRoot });

            foreach (string guid in guids)
            {
                AbilityDefinition ability = AssetDatabase.LoadAssetAtPath<AbilityDefinition>(AssetDatabase.GUIDToAssetPath(guid));

                if (ability != null)
                    output.Add(ability);
            }

            output.Sort((first, second) => string.Compare(first.DisplayName, second.DisplayName, StringComparison.OrdinalIgnoreCase));
        }

        public static void CollectAllEvolutions(List<AbilityEvolutionDefinition> output)
        {
            output.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AbilityEvolutionDefinition)}", new[] { GameContentAssetUtility.AbilityEvolutionRoot });

            foreach (string guid in guids)
            {
                AbilityEvolutionDefinition evolution = AssetDatabase.LoadAssetAtPath<AbilityEvolutionDefinition>(AssetDatabase.GUIDToAssetPath(guid));

                if (evolution != null)
                    output.Add(evolution);
            }

            output.Sort((first, second) => string.Compare(first.DisplayName, second.DisplayName, StringComparison.OrdinalIgnoreCase));
        }

        public static void CollectEvolutions(AbilityDefinition ability, List<AbilityEvolutionDefinition> output)
        {
            output.Clear();

            if (ability == null)
                return;

            List<AbilityEvolutionDefinition> all = new();
            CollectAllEvolutions(all);

            foreach (AbilityEvolutionDefinition evolution in all)
            {
                if (evolution.TargetAbility == ability)
                    output.Add(evolution);
            }
        }

        public static void CollectCharacterUsages(PlayableCharacterRosterDefinition roster, AbilityDefinition ability, List<GameContentAbilityCharacterUsage> output)
        {
            output.Clear();

            if (roster == null || ability == null)
                return;

            foreach (PlayableCharacterRosterEntry rosterEntry in roster.Characters)
            {
                PartyMemberDefinition character = rosterEntry?.Character;

                if (character == null)
                    continue;

                foreach (AbilityLoadoutEntry entry in character.StartingAbilities)
                {
                    if (entry?.Ability != ability)
                        continue;

                    output.Add(new GameContentAbilityCharacterUsage(character, entry.Role, entry.Level));
                }
            }
        }

        public static void CollectMasteryUsages(PlayableCharacterRosterDefinition roster, AbilityDefinition ability, List<GameContentAbilityMasteryUsage> output)
        {
            output.Clear();

            if (roster == null || ability == null)
                return;

            foreach (PlayableCharacterRosterEntry rosterEntry in roster.Characters)
            {
                PartyMemberDefinition character = rosterEntry?.Character;

                if (character?.MasteryDefinition == null)
                    continue;

                foreach (CharacterMasteryNodeDefinition node in character.MasteryDefinition.Nodes)
                {
                    if (node is not CharacterMasteryAbilityLevelNodeDefinition abilityNode)
                        continue;

                    if (abilityNode.Ability != ability)
                        continue;

                    output.Add(new GameContentAbilityMasteryUsage(character, abilityNode));
                }
            }
        }

        public static void CollectRewardSources(RunRewardCatalogDefinition catalog, AbilityDefinition ability, List<RunRewardDefinition> output)
        {
            output.Clear();

            if (catalog == null || ability == null)
                return;

            List<RunRewardDefinition> rewards = new();

            GameContentRewardUtility.CollectUniqueRewards(catalog, rewards);

            foreach (RunRewardDefinition reward in rewards)
            {
                if (reward is AbilityUnlockRewardDefinition unlock && unlock.Ability == ability)
                {
                    output.Add(reward);
                    continue;
                }

                if (reward is AbilityEvolutionRewardDefinition evolutionReward && evolutionReward.Evolution != null && evolutionReward.Evolution.TargetAbility == ability)
                {
                    output.Add(reward);
                }
            }
        }

        public static void CollectEvolutionRewardSources(RunRewardCatalogDefinition catalog, AbilityEvolutionDefinition evolution, List<RunRewardDefinition> output)
        {
            output.Clear();

            if (catalog == null || evolution == null)
                return;

            List<RunRewardDefinition> rewards = new();

            GameContentRewardUtility.CollectUniqueRewards(catalog, rewards);

            foreach (RunRewardDefinition reward in rewards)
            {
                if (reward is AbilityEvolutionRewardDefinition evolutionReward && evolutionReward.Evolution == evolution)
                {
                    output.Add(reward);
                }
            }
        }

        public static bool TryGetCoreUsage(PartyMemberDefinition character, AbilityDefinition ability, out AbilityLoadoutEntry usage)
        {
            usage = null;

            if (character == null || ability == null)
                return false;

            foreach (AbilityLoadoutEntry entry in character.StartingAbilities)
            {
                if (entry?.Ability != ability)
                    continue;

                usage = entry;
                return true;
            }

            return false;
        }

        public static ActorAbility GetRuntimeBehaviour(AbilityDefinition ability)
        {
            if (ability?.RuntimePrefab == null)
                return null;

            ActorAbility[] behaviours = ability.RuntimePrefab.GetComponents<ActorAbility>();

            return behaviours.Length == 1 ? behaviours[0] : null;
        }

        public static int GetRuntimeBehaviourCount(AbilityDefinition ability)
        {
            return ability?.RuntimePrefab != null ? ability.RuntimePrefab.GetComponents<ActorAbility>().Length : 0;
        }

        public static string GetRuntimeBehaviourLabel(AbilityDefinition ability)
        {
            ActorAbility runtime = GetRuntimeBehaviour(ability);

            return runtime != null ? ObjectNames.NicifyVariableName(runtime.GetType().Name) : "Missing Runtime Behaviour";
        }

        public static AbilityDefinition FindAbilityForRuntimePrefab(GameObject runtimePrefab)
        {
            if (runtimePrefab == null)
                return null;

            List<AbilityDefinition> abilities = new();
            CollectAbilities(abilities);

            foreach (AbilityDefinition ability in abilities)
            {
                if (ability.RuntimePrefab == runtimePrefab)
                    return ability;
            }

            return null;
        }

        public static bool AbilityIdExists(string id, AbilityDefinition except = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            List<AbilityDefinition> abilities = new();
            CollectAbilities(abilities);

            foreach (AbilityDefinition ability in abilities)
            {
                if (ability != except && ability.Id == id)
                    return true;
            }

            return false;
        }

        public static bool EvolutionIdExists(string id, AbilityEvolutionDefinition except = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            List<AbilityEvolutionDefinition> evolutions = new();
            CollectAllEvolutions(evolutions);

            foreach (AbilityEvolutionDefinition evolution in evolutions)
            {
                if (evolution != except && evolution.Id == id)
                    return true;
            }

            return false;
        }
    }
}
