using System.Collections.Generic;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentRewardUtility
    {
        public static RunRewardCatalogDefinition FindCatalog()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(RunRewardCatalogDefinition)}");

            if (guids.Length == 0)
                return null;

            if (guids.Length > 1)
            {
                Debug.LogWarning("More than one Run Reward Catalogue exists. " + "The Content Workbench is using the first result.");
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return AssetDatabase.LoadAssetAtPath<RunRewardCatalogDefinition>(path);
        }

        public static bool ContainsSet(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet)
        {
            if (catalog == null || rewardSet == null)
                return false;

            foreach (RunRewardSetDefinition candidate in catalog.RewardSets)
            {
                if (candidate == rewardSet)
                    return true;
            }

            return false;
        }

        public static bool ContainsReward(RunRewardSetDefinition rewardSet, RunRewardDefinition reward)
        {
            if (rewardSet == null || reward == null)
                return false;

            foreach (RunRewardDefinition candidate in rewardSet.Rewards)
            {
                if (candidate == reward)
                    return true;
            }

            return false;
        }

        public static bool AddSet(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet)
        {
            return AddSetInternal(catalog, rewardSet, true);
        }

        public static bool RemoveSet(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet)
        {
            return RemoveSetInternal(catalog, rewardSet, true);
        }

        public static bool AddReward(RunRewardSetDefinition rewardSet, RunRewardDefinition reward)
        {
            return AddRewardInternal(rewardSet, reward, true);
        }

        public static bool RemoveReward(RunRewardSetDefinition rewardSet, RunRewardDefinition reward)
        {
            return RemoveRewardInternal(rewardSet, reward, true);
        }

        public static RunRewardSetDefinition FindFirstSetContaining(RunRewardCatalogDefinition catalog, RunRewardDefinition reward)
        {
            if (catalog == null || reward == null)
                return null;

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (ContainsReward(rewardSet, reward))
                {
                    return rewardSet;
                }
            }

            return null;
        }

        public static void CollectMemberships(RunRewardCatalogDefinition catalog, RunRewardDefinition reward, List<RunRewardSetDefinition> output)
        {
            output.Clear();

            if (catalog == null || reward == null)
                return;

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet != null && ContainsReward(rewardSet, reward))
                {
                    output.Add(rewardSet);
                }
            }
        }

        public static int CountProjectRewardMemberships(RunRewardDefinition reward)
        {
            if (reward == null)
                return 0;

            List<RunRewardSetDefinition> memberships = new();

            CollectProjectRewardMemberships(reward, memberships);

            return memberships.Count;
        }

        public static int CountProjectCatalogueMemberships(RunRewardSetDefinition rewardSet)
        {
            if (rewardSet == null)
                return 0;

            List<RunRewardCatalogDefinition> memberships = new();

            CollectProjectCatalogueMemberships(rewardSet, memberships);

            return memberships.Count;
        }

        public static void CollectUniqueRewards(RunRewardCatalogDefinition catalog, List<RunRewardDefinition> output)
        {
            output.Clear();

            if (catalog == null)
                return;

            HashSet<RunRewardDefinition> seen = new();

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet == null)
                    continue;

                foreach (RunRewardDefinition reward in rewardSet.Rewards)
                {
                    if (reward != null && seen.Add(reward))
                    {
                        output.Add(reward);
                    }
                }
            }
        }

        public static int CountMissingReferences(RunRewardCatalogDefinition catalog)
        {
            if (catalog == null)
                return 0;

            int count = 0;

            HashSet<RunRewardSetDefinition> visitedSets = new();

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet == null)
                {
                    count++;
                    continue;
                }

                if (!visitedSets.Add(rewardSet))
                    continue;

                foreach (RunRewardDefinition reward in rewardSet.Rewards)
                {
                    if (reward == null)
                        count++;
                }
            }

            return count;
        }

        public static bool CleanMissingReferences(RunRewardCatalogDefinition catalog)
        {
            if (catalog == null || CountMissingReferences(catalog) <= 0)
            {
                return false;
            }

            int undoGroup = Undo.GetCurrentGroup();

            Undo.SetCurrentGroupName("Clean Run Reward Missing References");

            HashSet<RunRewardSetDefinition> validSets = new();

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet != null)
                    validSets.Add(rewardSet);
            }

            bool changed = RemoveMissingSetReferences(catalog, true);

            foreach (RunRewardSetDefinition rewardSet in validSets)
            {
                changed |= RemoveMissingRewardReferences(rewardSet, true);
            }

            Undo.CollapseUndoOperations(undoGroup);

            return changed;
        }

        public static bool DeleteRewardAsset(RunRewardDefinition reward)
        {
            if (reward == null)
                return false;

            string path = AssetDatabase.GetAssetPath(reward);

            if (string.IsNullOrWhiteSpace(path))
                return false;

            List<RunRewardSetDefinition> memberships = new();

            CollectProjectRewardMemberships(reward, memberships);

            foreach (RunRewardSetDefinition rewardSet in memberships)
            {
                RemoveRewardInternal(rewardSet, reward, false);
            }

            AssetDatabase.SaveAssets();

            bool deleted = AssetDatabase.DeleteAsset(path);

            if (!deleted)
            {
                foreach (RunRewardSetDefinition rewardSet in memberships)
                {
                    AddRewardInternal(rewardSet, reward, false);
                }

                AssetDatabase.SaveAssets();

                Debug.LogWarning($"Could not delete Run Reward asset '{path}'.", reward);

                return false;
            }

            AssetDatabase.Refresh();
            return true;
        }

        public static bool DeleteRewardSetAsset(RunRewardSetDefinition rewardSet)
        {
            if (rewardSet == null)
                return false;

            string path = AssetDatabase.GetAssetPath(rewardSet);

            if (string.IsNullOrWhiteSpace(path))
                return false;

            List<RunRewardCatalogDefinition> memberships = new();

            CollectProjectCatalogueMemberships(rewardSet, memberships);

            foreach (RunRewardCatalogDefinition catalog in memberships)
            {
                RemoveSetInternal(catalog, rewardSet, false);
            }

            AssetDatabase.SaveAssets();

            bool deleted = AssetDatabase.DeleteAsset(path);

            if (!deleted)
            {
                foreach (RunRewardCatalogDefinition catalog in memberships)
                {
                    AddSetInternal(catalog, rewardSet, false);
                }

                AssetDatabase.SaveAssets();

                Debug.LogWarning($"Could not delete Run Reward Set asset '{path}'.", rewardSet);

                return false;
            }

            AssetDatabase.Refresh();
            return true;
        }

        private static bool AddSetInternal(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet, bool recordUndo)
        {
            if (catalog == null || rewardSet == null || ContainsSet(catalog, rewardSet))
            {
                return false;
            }

            if (recordUndo)
            {
                Undo.RecordObject(catalog, "Add Run Reward Set");
            }

            SerializedObject serialized = new(catalog);

            serialized.Update();

            SerializedProperty sets = serialized.FindProperty("rewardSets");

            int index = sets.arraySize;

            sets.InsertArrayElementAtIndex(index);

            sets.GetArrayElementAtIndex(index).objectReferenceValue = rewardSet;

            Apply(serialized, catalog, recordUndo);

            return true;
        }

        private static bool RemoveSetInternal(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet, bool recordUndo)
        {
            if (catalog == null || rewardSet == null)
            {
                return false;
            }

            SerializedObject serialized = new(catalog);

            serialized.Update();

            SerializedProperty sets = serialized.FindProperty("rewardSets");

            bool found = false;

            for (int i = sets.arraySize - 1; i >= 0; i--)
            {
                if (sets.GetArrayElementAtIndex(i).objectReferenceValue != rewardSet)
                {
                    continue;
                }

                if (!found && recordUndo)
                {
                    Undo.RecordObject(catalog, "Remove Run Reward Set");
                }

                found = true;

                DeleteObjectReference(sets, i);
            }

            if (!found)
                return false;

            Apply(serialized, catalog, recordUndo);

            return true;
        }

        private static bool AddRewardInternal(RunRewardSetDefinition rewardSet, RunRewardDefinition reward, bool recordUndo)
        {
            if (rewardSet == null || reward == null || ContainsReward(rewardSet, reward))
            {
                return false;
            }

            if (recordUndo)
            {
                Undo.RecordObject(rewardSet, "Add Run Reward");
            }

            SerializedObject serialized = new(rewardSet);

            serialized.Update();

            SerializedProperty rewards = serialized.FindProperty("rewards");

            int index = rewards.arraySize;

            rewards.InsertArrayElementAtIndex(index);

            rewards.GetArrayElementAtIndex(index).objectReferenceValue = reward;

            Apply(serialized, rewardSet, recordUndo);

            return true;
        }

        private static bool RemoveRewardInternal(RunRewardSetDefinition rewardSet, RunRewardDefinition reward, bool recordUndo)
        {
            if (rewardSet == null || reward == null)
            {
                return false;
            }

            SerializedObject serialized = new(rewardSet);

            serialized.Update();

            SerializedProperty rewards = serialized.FindProperty("rewards");

            bool found = false;

            for (int i = rewards.arraySize - 1; i >= 0; i--)
            {
                if (rewards.GetArrayElementAtIndex(i).objectReferenceValue != reward)
                {
                    continue;
                }

                if (!found && recordUndo)
                {
                    Undo.RecordObject(rewardSet, "Remove Run Reward");
                }

                found = true;

                DeleteObjectReference(rewards, i);
            }

            if (!found)
                return false;

            Apply(serialized, rewardSet, recordUndo);

            return true;
        }

        private static bool RemoveMissingSetReferences(RunRewardCatalogDefinition catalog, bool recordUndo)
        {
            SerializedObject serialized = new(catalog);

            serialized.Update();

            SerializedProperty sets = serialized.FindProperty("rewardSets");

            bool found = false;

            for (int i = 0; i < sets.arraySize; i++)
            {
                if (sets.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            if (recordUndo)
            {
                Undo.RecordObject(catalog, "Clean Missing Reward Set Reference");
            }

            for (int i = sets.arraySize - 1; i >= 0; i--)
            {
                if (sets.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    DeleteObjectReference(sets, i);
                }
            }

            Apply(serialized, catalog, recordUndo);

            return true;
        }

        private static bool RemoveMissingRewardReferences(RunRewardSetDefinition rewardSet, bool recordUndo)
        {
            SerializedObject serialized = new(rewardSet);

            serialized.Update();

            SerializedProperty rewards = serialized.FindProperty("rewards");

            bool found = false;

            for (int i = 0; i < rewards.arraySize; i++)
            {
                if (rewards.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            if (recordUndo)
            {
                Undo.RecordObject(rewardSet, "Clean Missing Reward Reference");
            }

            for (int i = rewards.arraySize - 1; i >= 0; i--)
            {
                if (rewards.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    DeleteObjectReference(rewards, i);
                }
            }

            Apply(serialized, rewardSet, recordUndo);

            return true;
        }

        private static void CollectProjectRewardMemberships(RunRewardDefinition reward, List<RunRewardSetDefinition> output)
        {
            output.Clear();

            if (reward == null)
                return;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(RunRewardSetDefinition)}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                RunRewardSetDefinition rewardSet = AssetDatabase.LoadAssetAtPath<RunRewardSetDefinition>(path);

                if (rewardSet != null && ContainsReward(rewardSet, reward))
                {
                    output.Add(rewardSet);
                }
            }
        }

        private static void CollectProjectCatalogueMemberships(RunRewardSetDefinition rewardSet, List<RunRewardCatalogDefinition> output)
        {
            output.Clear();

            if (rewardSet == null)
                return;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(RunRewardCatalogDefinition)}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                RunRewardCatalogDefinition catalog = AssetDatabase.LoadAssetAtPath<RunRewardCatalogDefinition>(path);

                if (catalog != null && ContainsSet(catalog, rewardSet))
                {
                    output.Add(catalog);
                }
            }
        }

        private static void Apply(SerializedObject serialized, Object target, bool recordUndo)
        {
            if (recordUndo)
            {
                serialized.ApplyModifiedProperties();
            }
            else
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorUtility.SetDirty(target);
        }

        private static void DeleteObjectReference(SerializedProperty array, int index)
        {
            int previousSize = array.arraySize;

            array.DeleteArrayElementAtIndex(index);

            if (array.arraySize == previousSize)
            {
                array.DeleteArrayElementAtIndex(index);
            }
        }
    }
}
