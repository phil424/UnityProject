using System.IO;
using System.Text;
using MiniCrawler.Abilities;
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
        public const string AbilityRoot = "Assets/Playground/MiniCrawler/Data/Abilities";
        public const string AbilityEvolutionRoot = AbilityRoot + "/Evolutions";
        public const string AbilityRuntimeRoot = "Assets/Playground/MiniCrawler/Assets/Prefabs/Abilities";

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

            string extension = Path.GetExtension(currentPath);
            string desiredPath = $"{folder}/{desiredName}{extension}";
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

        public static string GetAbilityDataFolder(PartyMemberDefinition character)
        {
            return character == null ? AbilityRoot : $"{AbilityRoot}/{ToFileToken(character.DisplayName)}";
        }

        public static string GetAbilityRuntimeFolder(PartyMemberDefinition character)
        {
            return character == null ? AbilityRuntimeRoot : $"{AbilityRuntimeRoot}/{ToFileToken(character.DisplayName)}";
        }

        public static string GetAbilityAssetName(PartyMemberDefinition character, string displayName)
        {
            string ability = ToFileToken(displayName);

            if (string.IsNullOrWhiteSpace(ability))
                return string.Empty;

            if (character == null)
                return ability;

            return $"{ToFileToken(character.DisplayName)}{ability}";
        }

        public static string GetAbilityRuntimePrefabName(PartyMemberDefinition character, string displayName)
        {
            string abilityName = GetAbilityAssetName(character, displayName);

            return string.IsNullOrWhiteSpace(abilityName) ? string.Empty : $"{abilityName}AbilityRuntime";
        }

        public static string GetAbilityEvolutionFolder(AbilityDefinition ability)
        {
            if (ability == null)
                return AbilityEvolutionRoot;

            return $"{AbilityEvolutionRoot}/{ToFileToken(ability.DisplayName)}";
        }

        public static string GetAbilityEvolutionAssetName(string displayName)
        {
            return ToFileToken(displayName);
        }

        public static GameContentAbilityScope GetAbilityScope(AbilityDefinition ability)
        {
            if (ability == null)
                return GameContentAbilityScope.Shared;

            string path = AssetDatabase.GetAssetPath(ability);
            string folder = Path.GetDirectoryName(path)?.Replace('\\', '/');

            return folder == AbilityRoot ? GameContentAbilityScope.Shared : GameContentAbilityScope.SelectedCharacter;
        }

        public static string GetAbilityScopeLabel(AbilityDefinition ability)
        {
            if (ability == null)
                return "Unknown";

            string path = AssetDatabase.GetAssetPath(ability);
            string folder = Path.GetDirectoryName(path)?.Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(folder))
                return "Unknown";

            if (folder == AbilityRoot)
                return "Shared";

            if (folder.StartsWith($"{AbilityRoot}/"))
                return Path.GetFileName(folder);

            return "External";
        }

        public static bool SyncAbilityAssetsToScope(AbilityDefinition ability, PartyMemberDefinition character, GameContentAbilityScope scope, out string error)
        {
            error = null;

            if (ability == null)
            {
                error = "Ability is required.";
                return false;
            }

            if (scope == GameContentAbilityScope.SelectedCharacter && character == null)
            {
                error = "A Character is required for Character-scoped Ability content.";
                return false;
            }

            PartyMemberDefinition scopeCharacter = scope == GameContentAbilityScope.SelectedCharacter ? character : null;

            string dataFolder = GetAbilityDataFolder(scopeCharacter);
            string runtimeFolder = GetAbilityRuntimeFolder(scopeCharacter);

            EnsureFolder(dataFolder);
            EnsureFolder(runtimeFolder);

            string abilityName = GetAbilityAssetName(scopeCharacter, ability.DisplayName);

            string abilityPath = $"{dataFolder}/{abilityName}.asset";

            string runtimePath = null;

            if (ability.RuntimePrefab != null)
            {
                string runtimeName = GetAbilityRuntimePrefabName(scopeCharacter, ability.DisplayName);

                runtimePath = $"{runtimeFolder}/{runtimeName}.prefab";
            }

            if (!CanPlaceAssetAtPath(ability, abilityPath, out error))
            {
                return false;
            }

            if (ability.RuntimePrefab != null && !CanPlaceAssetAtPath(ability.RuntimePrefab, runtimePath, out error))
            {
                return false;
            }

            string originalAbilityPath = AssetDatabase.GetAssetPath(ability);

            string originalRuntimePath = ability.RuntimePrefab != null ? AssetDatabase.GetAssetPath(ability.RuntimePrefab) : null;

            if (!TryMoveAssetToPath(ability, abilityPath, out error))
            {
                return false;
            }

            if (ability.RuntimePrefab != null && !TryMoveAssetToPath(ability.RuntimePrefab, runtimePath, out error))
            {
                if (originalAbilityPath != abilityPath)
                {
                    AssetDatabase.MoveAsset(abilityPath, originalAbilityPath);
                }

                return false;
            }

            EditorUtility.SetDirty(ability);
            AssetDatabase.SaveAssets();

            return true;
        }

        public static void SyncAbilityAssetNames(AbilityDefinition ability)
        {
            if (ability == null)
                return;

            string abilityName = GetScopedAbilityName(ability, ability.DisplayName, AbilityRoot);

            if (!string.IsNullOrWhiteSpace(abilityName))
                TryRenameAsset(ability, abilityName);

            if (ability.RuntimePrefab == null)
                return;

            string runtimeName = GetScopedAbilityName(ability.RuntimePrefab, ability.DisplayName, AbilityRuntimeRoot);

            if (!string.IsNullOrWhiteSpace(runtimeName))
                TryRenameAsset(ability.RuntimePrefab, $"{runtimeName}AbilityRuntime");
        }

        public static void SyncAbilityEvolutionAssetName(AbilityEvolutionDefinition evolution)
        {
            if (evolution == null)
                return;

            string desiredName = GetAbilityEvolutionAssetName(evolution.DisplayName);

            if (!string.IsNullOrWhiteSpace(desiredName))
                TryRenameAsset(evolution, desiredName);
        }

        private static string GetScopedAbilityName(Object asset, string displayName, string root)
        {
            string token = ToFileToken(displayName);

            if (asset == null || string.IsNullOrWhiteSpace(token))
                return string.Empty;

            string path = AssetDatabase.GetAssetPath(asset);
            string folder = Path.GetDirectoryName(path)?.Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(folder) || folder == root)
                return token;

            string scope = Path.GetFileName(folder);

            return token.StartsWith(scope, System.StringComparison.OrdinalIgnoreCase) ? token : $"{scope}{token}";
        }

        private static bool CanPlaceAssetAtPath(Object asset, string desiredPath, out string error)
        {
            error = null;

            if (asset == null || string.IsNullOrWhiteSpace(desiredPath))
            {
                error = "Asset and destination path are required.";
                return false;
            }

            Object existing = AssetDatabase.LoadMainAssetAtPath(desiredPath);

            if (existing != null && existing != asset)
            {
                error = $"Cannot move '{asset.name}' because " + $"'{desiredPath}' already exists.";

                return false;
            }

            return true;
        }

        private static bool TryMoveAssetToPath(Object asset, string desiredPath, out string error)
        {
            error = null;

            if (!CanPlaceAssetAtPath(asset, desiredPath, out error))
            {
                return false;
            }

            string currentPath = AssetDatabase.GetAssetPath(asset);

            if (currentPath == desiredPath)
                return true;

            string result = AssetDatabase.MoveAsset(currentPath, desiredPath);

            if (!string.IsNullOrWhiteSpace(result))
            {
                error = $"Could not move '{asset.name}' to " + $"'{desiredPath}': {result}";

                return false;
            }

            return true;
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
