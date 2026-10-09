using System;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal enum RunRewardAuthoringKind
    {
        CharacterGrowth,
        AbilityUnlock,
        AbilityEvolution,
        WeaponMasteryLevel,
    }

    internal static class GameContentRewardFactory
    {
        public static RunRewardDefinition Create(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet, RunRewardAuthoringKind kind, string displayName, string id, out string error)
        {
            error = ValidateCreation(catalog, displayName, id, kind);

            if (!string.IsNullOrWhiteSpace(error) || rewardSet == null)
            {
                return null;
            }

            Type type = GetRewardType(kind);

            RunRewardDefinition reward = ScriptableObject.CreateInstance(type) as RunRewardDefinition;

            if (reward == null)
            {
                error = $"Could not create reward type '{type.Name}'.";
                return null;
            }

            string folder = GetAssetFolder(kind);
            GameContentAssetUtility.EnsureFolder(folder);

            string assetName = GameContentAssetUtility.GetRunRewardAssetName(displayName);

            string assetPath = $"{folder}/{assetName}.asset";

            reward.name = assetName;

            AssetDatabase.CreateAsset(reward, assetPath);

            Undo.RegisterCreatedObjectUndo(reward, "Create Run Reward");

            SerializedObject serialized = new(reward);

            SerializedProperty idProperty = serialized.FindProperty("id");

            SerializedProperty displayProperty = serialized.FindProperty("displayName");

            if (idProperty != null)
                idProperty.stringValue = id;

            if (displayProperty != null)
                displayProperty.stringValue = displayName;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(reward);

            GameContentRewardUtility.AddReward(rewardSet, reward);

            AssetDatabase.SaveAssets();

            Selection.activeObject = reward;
            EditorGUIUtility.PingObject(reward);

            return reward;
        }

        public static string ValidateCreation(RunRewardCatalogDefinition catalog, string displayName, string id, RunRewardAuthoringKind kind)
        {
            if (catalog == null)
                return "Run Reward Catalogue is required.";

            if (string.IsNullOrWhiteSpace(displayName))
                return "Display Name is required.";

            if (string.IsNullOrWhiteSpace(id))
                return "Stable Reward ID is required.";

            string assetName = GameContentAssetUtility.GetRunRewardAssetName(displayName);

            if (string.IsNullOrWhiteSpace(assetName))
                return "Display Name cannot produce a valid asset filename.";

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet == null)
                    continue;

                foreach (RunRewardDefinition reward in rewardSet.Rewards)
                {
                    if (reward != null && reward.Id == id)
                        return $"Reward ID '{id}' is already in use.";
                }
            }

            string path = $"{GetAssetFolder(kind)}/{assetName}.asset";

            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
                return $"An asset already exists at '{path}'.";

            return null;
        }

        public static string GetSuggestedId(string displayName)
        {
            string token = GameContentAssetUtility.ToIdToken(displayName);

            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"reward_{token}";
        }

        public static string GetMenuLabel(RunRewardAuthoringKind kind)
        {
            return kind switch
            {
                RunRewardAuthoringKind.CharacterGrowth => "Character Growth",

                RunRewardAuthoringKind.AbilityUnlock => "Ability Unlock",

                RunRewardAuthoringKind.AbilityEvolution => "Ability Evolution",

                RunRewardAuthoringKind.WeaponMasteryLevel => "Weapon Mastery Level",

                _ => kind.ToString(),
            };
        }

        public static string GetAssetFolder(RunRewardAuthoringKind kind)
        {
            return kind switch
            {
                RunRewardAuthoringKind.CharacterGrowth => GameContentAssetUtility.RunUpgradeFolder,

                RunRewardAuthoringKind.AbilityUnlock => GameContentAssetUtility.AbilityRewardFolder,

                RunRewardAuthoringKind.AbilityEvolution => GameContentAssetUtility.AbilityRewardFolder,

                RunRewardAuthoringKind.WeaponMasteryLevel => GameContentAssetUtility.WeaponMasteryRewardFolder,

                _ => GameContentAssetUtility.RunRewardRoot,
            };
        }

        private static Type GetRewardType(RunRewardAuthoringKind kind)
        {
            return kind switch
            {
                RunRewardAuthoringKind.CharacterGrowth => typeof(RunUpgradeDefinition),

                RunRewardAuthoringKind.AbilityUnlock => typeof(AbilityUnlockRewardDefinition),

                RunRewardAuthoringKind.AbilityEvolution => typeof(AbilityEvolutionRewardDefinition),

                RunRewardAuthoringKind.WeaponMasteryLevel => typeof(WeaponModLevelRewardDefinition),

                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
            };
        }
    }
}
