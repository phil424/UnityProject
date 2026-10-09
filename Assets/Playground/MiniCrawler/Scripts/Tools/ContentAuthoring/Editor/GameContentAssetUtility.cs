using System.IO;
using System.Text;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentAssetUtility
    {
        public const string PartyFolder = "Assets/Playground/MiniCrawler/Data/Party";
        public const string ActorFolder = "Assets/Playground/MiniCrawler/Data/Core";
        public const string MasteryRoot = "Assets/Playground/MiniCrawler/Data/Progression/Mastery";
        public const string RunUpgradeFolder = "Assets/Playground/MiniCrawler/Data/RunUpgrades";
        public const string RunRewardRoot = "Assets/Playground/MiniCrawler/Data/RunRewards";
        public const string RunRewardSetFolder = RunRewardRoot + "/Catalog/Sets";
        public const string AbilityRewardFolder = RunRewardRoot + "/Abilities";
        public const string WeaponMasteryRewardFolder = RunRewardRoot + "/WeaponMods";

        public static string ToFileToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            StringBuilder builder = new();

            foreach (char character in value)
            {
                if (char.IsLetterOrDigit(character))
                    builder.Append(character);
            }

            return builder.ToString();
        }

        public static string ToIdToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            StringBuilder builder = new();
            bool separator = false;

            foreach (char character in value)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(char.ToLowerInvariant(character));
                    separator = false;
                    continue;
                }

                if (separator || builder.Length == 0)
                    continue;

                builder.Append('_');
                separator = true;
            }

            return builder.ToString().Trim('_');
        }

        public static string GetCharacterAssetName(string displayName)
        {
            return $"{ToFileToken(displayName)}Definition";
        }

        public static string GetActorAssetName(string displayName)
        {
            return $"{ToFileToken(displayName)}ActorDefinition";
        }

        public static string GetMasteryAssetName(string displayName)
        {
            return $"{ToFileToken(displayName)}MasteryDefinition";
        }

        public static string GetRunRewardAssetName(string displayName)
        {
            string token = ToFileToken(displayName);
            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"{token}Reward";
        }

        public static string GetRewardSetAssetName(string name)
        {
            string token = ToFileToken(name);

            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            return token.EndsWith("Rewards", System.StringComparison.OrdinalIgnoreCase) ? token : $"{token}Rewards";
        }

        public static string GetMasteryNodeAssetName(PartyMemberDefinition character, CharacterMasteryNodeDefinition node)
        {
            if (character == null || node == null)
                return string.Empty;

            return $"{ToFileToken(character.DisplayName)}_{ToFileToken(node.DisplayName)}";
        }

        public static bool TryRenameAsset(Object asset, string desiredName)
        {
            if (asset == null || string.IsNullOrWhiteSpace(desiredName))
                return false;

            string currentPath = AssetDatabase.GetAssetPath(asset);

            if (string.IsNullOrWhiteSpace(currentPath))
                return false;

            string currentName = Path.GetFileNameWithoutExtension(currentPath);

            if (currentName == desiredName)
                return true;

            string folder = Path.GetDirectoryName(currentPath)?.Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(folder))
                return false;

            string desiredPath = $"{folder}/{desiredName}.asset";
            Object existing = AssetDatabase.LoadMainAssetAtPath(desiredPath);

            if (existing != null && existing != asset)
            {
                Debug.LogWarning($"Cannot rename '{currentName}' to '{desiredName}' because '{desiredPath}' already exists.", asset);

                return false;
            }

            string error = AssetDatabase.RenameAsset(currentPath, desiredName);

            if (!string.IsNullOrWhiteSpace(error))
            {
                Debug.LogWarning($"Could not rename '{currentName}' to '{desiredName}': {error}", asset);

                return false;
            }

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            return true;
        }

        public static void SyncCharacterAssetNames(PartyMemberDefinition character)
        {
            if (character == null || string.IsNullOrWhiteSpace(character.DisplayName))
                return;

            TryRenameAsset(character, GetCharacterAssetName(character.DisplayName));

            if (character.ActorDefinition != null)
            {
                TryRenameAsset(character.ActorDefinition, GetActorAssetName(character.DisplayName));
            }

            if (character.MasteryDefinition != null)
            {
                TryRenameAsset(character.MasteryDefinition, GetMasteryAssetName(character.DisplayName));
            }
        }

        public static void SyncMasteryNodeAssetName(PartyMemberDefinition character, CharacterMasteryNodeDefinition node)
        {
            string desiredName = GetMasteryNodeAssetName(character, node);

            if (!string.IsNullOrWhiteSpace(desiredName))
                TryRenameAsset(node, desiredName);
        }

        public static void SyncRunRewardAssetName(RunRewardDefinition reward)
        {
            if (reward == null)
                return;

            string desiredName = GetRunRewardAssetName(reward.DisplayName);

            if (!string.IsNullOrWhiteSpace(desiredName))
                TryRenameAsset(reward, desiredName);
        }

        public static void EnsureFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || AssetDatabase.IsValidFolder(folderPath))
                return;

            string[] parts = folderPath.Split('/');

            if (parts.Length == 0)
                return;

            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";

                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);

                current = next;
            }
        }
    }
}
