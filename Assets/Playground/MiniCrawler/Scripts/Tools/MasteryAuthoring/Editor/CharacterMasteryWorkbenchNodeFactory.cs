using System;
using System.IO;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Tools.ContentAuthoring;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal enum CharacterMasteryNodeKind
    {
        Stat,
        AbilityLevel,
        WeaponMastery,
        WeaponMasteryLevel,
        WeaponMasteryCombination,
    }

    internal static class CharacterMasteryWorkbenchNodeFactory
    {
        public static CharacterMasteryNodeDefinition Create(
            PartyMemberDefinition character,
            CharacterMasteryDefinition mastery,
            CharacterMasteryNodeKind kind,
            Vector2 treePosition,
            string displayName,
            string id
        )
        {
            string error = ValidateCreation(character, mastery, displayName, id);

            if (!string.IsNullOrWhiteSpace(error))
            {
                Debug.LogWarning(error, mastery);
                return null;
            }

            string masteryPath = AssetDatabase.GetAssetPath(mastery);

            string folder = Path.GetDirectoryName(masteryPath)?.Replace('\\', '/');

            Type nodeType = GetNodeType(kind);

            CharacterMasteryNodeDefinition node = ScriptableObject.CreateInstance(nodeType) as CharacterMasteryNodeDefinition;

            if (node == null)
                return null;

            string assetName = GetAssetName(character, displayName);

            string assetPath = $"{folder}/{assetName}.asset";

            node.name = assetName;

            AssetDatabase.CreateAsset(node, assetPath);

            Undo.RegisterCreatedObjectUndo(node, "Create Character Mastery Node");

            ConfigureNewNode(node, displayName, id, treePosition);

            CharacterMasteryWorkbenchContentUtility.AddNodeToTree(mastery, node);

            AssetDatabase.SaveAssets();

            return node;
        }

        public static string GetMenuLabel(CharacterMasteryNodeKind kind)
        {
            return kind switch
            {
                CharacterMasteryNodeKind.Stat => "Stat",

                CharacterMasteryNodeKind.AbilityLevel => "Ability Level",

                CharacterMasteryNodeKind.WeaponMastery => "Weapon Mastery Stance",

                CharacterMasteryNodeKind.WeaponMasteryLevel => "Weapon Mastery Level",

                CharacterMasteryNodeKind.WeaponMasteryCombination => "Mastery Combination",

                _ => kind.ToString(),
            };
        }

        public static string GetSuggestedId(PartyMemberDefinition character, string displayName)
        {
            if (character == null || string.IsNullOrWhiteSpace(displayName))
            {
                return string.Empty;
            }

            string nodeToken = GameContentAssetUtility.ToIdToken(displayName);

            if (string.IsNullOrWhiteSpace(nodeToken))
                return string.Empty;

            return $"{character.Id}_{nodeToken}";
        }

        public static string GetAssetName(PartyMemberDefinition character, string displayName)
        {
            if (character == null)
                return string.Empty;

            return $"{GameContentAssetUtility.ToFileToken(character.DisplayName)}_" + $"{GameContentAssetUtility.ToFileToken(displayName)}";
        }

        public static string ValidateCreation(PartyMemberDefinition character, CharacterMasteryDefinition mastery, string displayName, string id)
        {
            if (character == null)
                return "Character is required.";

            if (mastery == null)
                return "Character Mastery Definition is required.";

            if (string.IsNullOrWhiteSpace(displayName))
                return "Display Name is required.";

            if (string.IsNullOrWhiteSpace(id))
                return "Stable Mastery-node ID is required.";

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node != null && node.Id == id)
                {
                    return $"Mastery node ID '{id}' already exists.";
                }
            }

            string masteryPath = AssetDatabase.GetAssetPath(mastery);

            string folder = Path.GetDirectoryName(masteryPath)?.Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(folder))
            {
                return "Could not determine the Mastery content folder.";
            }

            string assetName = GetAssetName(character, displayName);

            if (string.IsNullOrWhiteSpace(assetName))
                return "Could not create a valid asset filename.";

            string assetPath = $"{folder}/{assetName}.asset";

            if (AssetDatabase.LoadMainAssetAtPath(assetPath) != null)
            {
                return $"An asset already exists at '{assetPath}'.";
            }

            return null;
        }

        private static Type GetNodeType(CharacterMasteryNodeKind kind)
        {
            return kind switch
            {
                CharacterMasteryNodeKind.Stat => typeof(CharacterMasteryStatNodeDefinition),

                CharacterMasteryNodeKind.AbilityLevel => typeof(CharacterMasteryAbilityLevelNodeDefinition),

                CharacterMasteryNodeKind.WeaponMastery => typeof(CharacterMasteryWeaponModNodeDefinition),

                CharacterMasteryNodeKind.WeaponMasteryLevel => typeof(CharacterMasteryWeaponModLevelNodeDefinition),

                CharacterMasteryNodeKind.WeaponMasteryCombination => typeof(CharacterMasteryWeaponModCombinationNodeDefinition),

                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
            };
        }

        private static void ConfigureNewNode(CharacterMasteryNodeDefinition node, string displayName, string id, Vector2 treePosition)
        {
            SerializedObject serialized = new(node);

            serialized.FindProperty("id").stringValue = id;

            serialized.FindProperty("displayName").stringValue = displayName;

            serialized.FindProperty("treePosition").vector2Value = treePosition;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(node);
        }
    }
}
