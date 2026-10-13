using System.Collections.Generic;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal static class CharacterMasteryWorkbenchContentUtility
    {
        public static bool AddNodeToTree(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition node)
        {
            if (mastery == null || node == null || mastery.Contains(node))
                return false;

            Undo.RecordObject(mastery, "Add Character Mastery Node");

            SerializedObject serialized = new(mastery);
            SerializedProperty nodes = serialized.FindProperty("nodes");

            int index = nodes.arraySize;
            nodes.InsertArrayElementAtIndex(index);
            nodes.GetArrayElementAtIndex(index).objectReferenceValue = node;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(mastery);

            return true;
        }

        public static int CountMissingReferences(CharacterMasteryDefinition mastery)
        {
            if (mastery == null)
                return 0;

            int count = 0;

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node == null)
                {
                    count++;
                    continue;
                }

                foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
                {
                    if (prerequisite == null)
                        count++;
                }
            }

            return count;
        }

        public static int CountReferencesToNode(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition target)
        {
            if (mastery == null || target == null)
                return 0;

            int count = 0;

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node == null || node == target)
                    continue;

                foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
                {
                    if (prerequisite == target)
                        count++;
                }

                if (node is not CharacterMasteryWeaponModCombinationNodeDefinition combination || target is not CharacterMasteryWeaponModNodeDefinition weaponMastery)
                {
                    continue;
                }

                if (combination.ActiveMastery == weaponMastery)
                    count++;

                if (combination.CombinedMastery == weaponMastery)
                    count++;
            }

            return count;
        }

        public static bool CleanMissingReferences(CharacterMasteryDefinition mastery)
        {
            if (mastery == null || CountMissingReferences(mastery) == 0)
                return false;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Clean Character Mastery Missing References");

            List<CharacterMasteryNodeDefinition> validNodes = new();

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node != null)
                    validNodes.Add(node);
            }

            bool changed = RemoveMissingNodeMemberships(mastery, true);

            foreach (CharacterMasteryNodeDefinition node in validNodes)
                changed |= RemoveMissingPrerequisites(node, true);

            Undo.CollapseUndoOperations(undoGroup);

            return changed;
        }

        public static bool RemoveNodeFromTree(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition node)
        {
            if (mastery == null || node == null)
                return false;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Remove Character Mastery Node");

            bool changed = RemoveNodeFromTree(mastery, node, true);

            Undo.CollapseUndoOperations(undoGroup);

            return changed;
        }

        public static bool DeleteNodeAsset(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition node)
        {
            if (node == null)
                return false;

            string path = AssetDatabase.GetAssetPath(node);

            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (mastery != null)
                RemoveNodeFromTree(mastery, node, false);

            AssetDatabase.SaveAssets();

            bool deleted = AssetDatabase.DeleteAsset(path);

            if (!deleted)
            {
                Debug.LogWarning($"Could not delete Character Mastery node asset '{path}'.", mastery);

                return false;
            }

            AssetDatabase.Refresh();
            return true;
        }

        private static bool RemoveNodeFromTree(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition target, bool recordUndo)
        {
            List<CharacterMasteryNodeDefinition> remainingNodes = new();

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node != null && node != target)
                    remainingNodes.Add(node);
            }

            bool changed = RemoveNodeMembership(mastery, target, recordUndo);

            foreach (CharacterMasteryNodeDefinition node in remainingNodes)
            {
                changed |= RemovePrerequisiteReferences(node, target, recordUndo);
                changed |= ClearCombinationReferences(node, target, recordUndo);
            }

            return changed;
        }

        private static bool RemoveNodeMembership(CharacterMasteryDefinition mastery, CharacterMasteryNodeDefinition target, bool recordUndo)
        {
            SerializedObject serialized = new(mastery);
            SerializedProperty nodes = serialized.FindProperty("nodes");

            bool found = false;

            for (int i = 0; i < nodes.arraySize; i++)
            {
                if (nodes.GetArrayElementAtIndex(i).objectReferenceValue == target)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            RecordIfNeeded(mastery, recordUndo, "Remove Character Mastery Node");

            for (int i = nodes.arraySize - 1; i >= 0; i--)
            {
                if (nodes.GetArrayElementAtIndex(i).objectReferenceValue == target)
                    DeleteObjectReference(nodes, i);
            }

            Apply(serialized, mastery, recordUndo);
            return true;
        }

        private static bool RemovePrerequisiteReferences(CharacterMasteryNodeDefinition node, CharacterMasteryNodeDefinition target, bool recordUndo)
        {
            bool found = false;

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == target)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            RecordIfNeeded(node, recordUndo, "Clean Mastery Prerequisite");

            SerializedObject serialized = new(node);
            SerializedProperty prerequisites = serialized.FindProperty("prerequisites");

            for (int i = prerequisites.arraySize - 1; i >= 0; i--)
            {
                if (prerequisites.GetArrayElementAtIndex(i).objectReferenceValue == target)
                    DeleteObjectReference(prerequisites, i);
            }

            Apply(serialized, node, recordUndo);
            return true;
        }

        private static bool ClearCombinationReferences(CharacterMasteryNodeDefinition node, CharacterMasteryNodeDefinition target, bool recordUndo)
        {
            if (node is not CharacterMasteryWeaponModCombinationNodeDefinition combination || target is not CharacterMasteryWeaponModNodeDefinition weaponMastery)
            {
                return false;
            }

            bool clearActive = combination.ActiveMastery == weaponMastery;
            bool clearCombined = combination.CombinedMastery == weaponMastery;

            if (!clearActive && !clearCombined)
                return false;

            RecordIfNeeded(node, recordUndo, "Clean Mastery Combination Reference");

            SerializedObject serialized = new(node);

            if (clearActive)
                serialized.FindProperty("activeMastery").objectReferenceValue = null;

            if (clearCombined)
                serialized.FindProperty("combinedMastery").objectReferenceValue = null;

            Apply(serialized, node, recordUndo);
            return true;
        }

        private static bool RemoveMissingNodeMemberships(CharacterMasteryDefinition mastery, bool recordUndo)
        {
            SerializedObject serialized = new(mastery);
            SerializedProperty nodes = serialized.FindProperty("nodes");

            bool found = false;

            for (int i = 0; i < nodes.arraySize; i++)
            {
                if (nodes.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            RecordIfNeeded(mastery, recordUndo, "Clean Missing Mastery Node Reference");

            for (int i = nodes.arraySize - 1; i >= 0; i--)
            {
                if (nodes.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    DeleteObjectReference(nodes, i);
            }

            Apply(serialized, mastery, recordUndo);
            return true;
        }

        private static bool RemoveMissingPrerequisites(CharacterMasteryNodeDefinition node, bool recordUndo)
        {
            bool found = false;

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            RecordIfNeeded(node, recordUndo, "Clean Missing Mastery Prerequisite");

            SerializedObject serialized = new(node);
            SerializedProperty prerequisites = serialized.FindProperty("prerequisites");

            for (int i = prerequisites.arraySize - 1; i >= 0; i--)
            {
                if (prerequisites.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    DeleteObjectReference(prerequisites, i);
            }

            Apply(serialized, node, recordUndo);
            return true;
        }

        private static void RecordIfNeeded(Object target, bool recordUndo, string name)
        {
            if (recordUndo)
                Undo.RecordObject(target, name);
        }

        private static void Apply(SerializedObject serialized, Object target, bool recordUndo)
        {
            if (recordUndo)
                serialized.ApplyModifiedProperties();
            else
                serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(target);
        }

        private static void DeleteObjectReference(SerializedProperty array, int index)
        {
            int previousSize = array.arraySize;

            array.DeleteArrayElementAtIndex(index);

            if (array.arraySize == previousSize)
                array.DeleteArrayElementAtIndex(index);
        }
    }
}
