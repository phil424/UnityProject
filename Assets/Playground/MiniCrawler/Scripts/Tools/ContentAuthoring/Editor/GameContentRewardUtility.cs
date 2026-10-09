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
            if (catalog == null || rewardSet == null || ContainsSet(catalog, rewardSet))
                return false;

            Undo.RecordObject(catalog, "Add Run Reward Set");

            SerializedObject serialized = new(catalog);
            SerializedProperty sets = serialized.FindProperty("rewardSets");

            int index = sets.arraySize;
            sets.InsertArrayElementAtIndex(index);
            sets.GetArrayElementAtIndex(index).objectReferenceValue = rewardSet;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(catalog);

            return true;
        }

        public static bool RemoveSet(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet)
        {
            if (catalog == null || rewardSet == null)
                return false;

            SerializedObject serialized = new(catalog);
            SerializedProperty sets = serialized.FindProperty("rewardSets");

            for (int i = 0; i < sets.arraySize; i++)
            {
                if (sets.GetArrayElementAtIndex(i).objectReferenceValue != rewardSet)
                    continue;

                Undo.RecordObject(catalog, "Remove Run Reward Set");
                DeleteObjectReference(sets, i);

                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(catalog);

                return true;
            }

            return false;
        }

        public static bool AddReward(RunRewardSetDefinition rewardSet, RunRewardDefinition reward)
        {
            if (rewardSet == null || reward == null || ContainsReward(rewardSet, reward))
                return false;

            Undo.RecordObject(rewardSet, "Add Run Reward");

            SerializedObject serialized = new(rewardSet);
            SerializedProperty rewards = serialized.FindProperty("rewards");

            int index = rewards.arraySize;
            rewards.InsertArrayElementAtIndex(index);
            rewards.GetArrayElementAtIndex(index).objectReferenceValue = reward;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(rewardSet);

            return true;
        }

        public static bool RemoveReward(RunRewardSetDefinition rewardSet, RunRewardDefinition reward)
        {
            if (rewardSet == null || reward == null)
                return false;

            SerializedObject serialized = new(rewardSet);
            SerializedProperty rewards = serialized.FindProperty("rewards");

            for (int i = 0; i < rewards.arraySize; i++)
            {
                if (rewards.GetArrayElementAtIndex(i).objectReferenceValue != reward)
                    continue;

                Undo.RecordObject(rewardSet, "Remove Run Reward");

                DeleteObjectReference(rewards, i);

                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(rewardSet);

                return true;
            }

            return false;
        }

        public static RunRewardSetDefinition FindFirstSetContaining(RunRewardCatalogDefinition catalog, RunRewardDefinition reward)
        {
            if (catalog == null || reward == null)
                return null;

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (ContainsReward(rewardSet, reward))
                    return rewardSet;
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
                    output.Add(rewardSet);
            }
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
                        output.Add(reward);
                }
            }
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
