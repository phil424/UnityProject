using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal static class CharacterMasteryWorkbenchValidation
    {
        public static void CollectIssues(PartyMemberDefinition character, CharacterMasteryDefinition mastery, Vector2 nodeSize, List<string> output)
        {
            output.Clear();

            if (character == null || mastery == null)
                return;

            List<CharacterMasteryNodeDefinition> nodes = new();
            HashSet<CharacterMasteryNodeDefinition> nodeSet = new();

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node == null)
                {
                    output.Add("Mastery Definition contains a missing node reference.");
                    continue;
                }

                if (!nodeSet.Add(node))
                    output.Add($"'{node.DisplayName}' appears more than once in the Mastery Definition.");

                nodes.Add(node);
            }

            HashSet<string> ids = new();
            Dictionary<CharacterMasteryWeaponModNodeDefinition, CharacterMasteryWeaponModCombinationNodeDefinition> combinationTargets = new();

            int rootCount = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                CharacterMasteryNodeDefinition node = nodes[i];

                if (!node.IsConfigured)
                    output.Add($"'{node.name}' is not fully configured.");

                if (!ids.Add(node.Id))
                    output.Add($"Duplicate Mastery-node ID '{node.Id}'.");

                if (node.Prerequisites.Count == 0)
                    rootCount++;

                ValidateCombination(character, node, nodeSet, combinationTargets, output);
                ValidatePrerequisites(character, node, nodeSet, output);
                ValidatePosition(node, mastery.TreeSize, nodeSize, output);
                ValidateOverlap(node, i, nodes, nodeSize, output);
            }

            if (nodes.Count > 0 && rootCount == 0)
                output.Add("The Mastery graph has no root node.");

            if (ContainsCycle(nodes, nodeSet))
                output.Add("The Mastery prerequisite graph contains a cycle.");
        }

        private static void ValidateCombination(
            PartyMemberDefinition character,
            CharacterMasteryNodeDefinition node,
            HashSet<CharacterMasteryNodeDefinition> nodeSet,
            Dictionary<CharacterMasteryWeaponModNodeDefinition, CharacterMasteryWeaponModCombinationNodeDefinition> combinationTargets,
            List<string> output
        )
        {
            if (node is not CharacterMasteryWeaponModCombinationNodeDefinition combination)
                return;

            CharacterMasteryWeaponModNodeDefinition active = combination.ActiveMastery;
            CharacterMasteryWeaponModNodeDefinition inherited = combination.CombinedMastery;

            if (active != null && !nodeSet.Contains(active))
            {
                output.Add($"'{combination.DisplayName}' targets active Mastery '{active.DisplayName}', " + $"but that stance is not in '{character.DisplayName}' Mastery Definition.");
            }

            if (inherited != null && !nodeSet.Contains(inherited))
            {
                output.Add($"'{combination.DisplayName}' inherits from '{inherited.DisplayName}', " + $"but that stance is not in '{character.DisplayName}' Mastery Definition.");
            }

            if (active == null)
                return;

            if (combinationTargets.TryGetValue(active, out CharacterMasteryWeaponModCombinationNodeDefinition existing))
            {
                output.Add(
                    $"'{existing.DisplayName}' and '{combination.DisplayName}' both target "
                        + $"'{active.DisplayName}'. The pairwise combination model allows only "
                        + "one inherited Mastery per active stance."
                );

                return;
            }

            combinationTargets.Add(active, combination);
        }

        private static void ValidatePrerequisites(PartyMemberDefinition character, CharacterMasteryNodeDefinition node, HashSet<CharacterMasteryNodeDefinition> nodeSet, List<string> output)
        {
            HashSet<CharacterMasteryNodeDefinition> prerequisites = new();

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null)
                {
                    output.Add($"'{node.DisplayName}' has a missing prerequisite reference.");
                    continue;
                }

                if (prerequisite == node)
                    output.Add($"'{node.DisplayName}' cannot require itself.");

                if (!prerequisites.Add(prerequisite))
                {
                    output.Add($"'{node.DisplayName}' lists '{prerequisite.DisplayName}' more than once.");
                }

                if (!nodeSet.Contains(prerequisite))
                {
                    output.Add($"'{node.DisplayName}' requires '{prerequisite.DisplayName}', " + $"but that node is not in '{character.DisplayName}' Mastery Definition.");
                }
            }
        }

        private static void ValidatePosition(CharacterMasteryNodeDefinition node, Vector2 treeSize, Vector2 nodeSize, List<string> output)
        {
            Vector2 halfCanvas = treeSize * 0.5f;
            Vector2 halfNode = nodeSize * 0.5f;

            float maximumX = Mathf.Max(0f, halfCanvas.x - halfNode.x);
            float maximumY = Mathf.Max(0f, halfCanvas.y - halfNode.y);

            if (Mathf.Abs(node.TreePosition.x) > maximumX || Mathf.Abs(node.TreePosition.y) > maximumY)
            {
                output.Add($"'{node.DisplayName}' is outside the authored Mastery-tree bounds.");
            }
        }

        private static void ValidateOverlap(CharacterMasteryNodeDefinition node, int index, IReadOnlyList<CharacterMasteryNodeDefinition> nodes, Vector2 nodeSize, List<string> output)
        {
            Rect firstRect = GetNodeRect(node.TreePosition, nodeSize);

            for (int i = index + 1; i < nodes.Count; i++)
            {
                CharacterMasteryNodeDefinition other = nodes[i];

                if (other == null || other == node)
                    continue;

                if (!firstRect.Overlaps(GetNodeRect(other.TreePosition, nodeSize)))
                    continue;

                output.Add($"'{node.DisplayName}' overlaps '{other.DisplayName}' on the authored canvas.");
            }
        }

        private static bool ContainsCycle(IReadOnlyList<CharacterMasteryNodeDefinition> nodes, HashSet<CharacterMasteryNodeDefinition> nodeSet)
        {
            Dictionary<CharacterMasteryNodeDefinition, int> states = new();

            foreach (CharacterMasteryNodeDefinition node in nodes)
            {
                if (node != null && Visit(node, nodeSet, states))
                    return true;
            }

            return false;
        }

        private static bool Visit(CharacterMasteryNodeDefinition node, HashSet<CharacterMasteryNodeDefinition> nodeSet, Dictionary<CharacterMasteryNodeDefinition, int> states)
        {
            if (states.TryGetValue(node, out int state))
                return state == 1;

            states[node] = 1;

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null || !nodeSet.Contains(prerequisite))
                    continue;

                if (Visit(prerequisite, nodeSet, states))
                    return true;
            }

            states[node] = 2;
            return false;
        }

        private static Rect GetNodeRect(Vector2 position, Vector2 size)
        {
            return new Rect(position - size * 0.5f, size);
        }
    }
}
