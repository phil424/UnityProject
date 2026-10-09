using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Tools.ContentAuthoring;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal static class CharacterMasteryWorkbenchInspector
    {
        public static bool Draw(PartyMemberDefinition character, CharacterMasteryNodeDefinition node, ref bool showAdvanced)
        {
            if (node == null)
            {
                EditorGUILayout.HelpBox("Select a Mastery node on the graph to inspect and edit it.", MessageType.Info);

                return false;
            }

            EditorGUILayout.LabelField(node.DisplayName, EditorStyles.largeLabel);
            EditorGUILayout.LabelField(GetTypeLabel(node), EditorStyles.miniBoldLabel);
            EditorGUILayout.Space(6);

            SerializedObject serialized = new(node);
            serialized.Update();

            EditorGUI.BeginChangeCheck();

            DrawSection("Identity");
            DrawProperty(serialized, "displayName");
            DrawProperty(serialized, "description");
            DrawProperty(serialized, "icon");

            EditorGUILayout.Space(8);
            DrawTypeSpecificProperties(serialized, node);

            EditorGUILayout.Space(8);
            DrawSection("Requirements");
            DrawProperty(serialized, "prerequisites", true);
            DrawProperty(serialized, "costs", true);

            EditorGUILayout.Space(8);
            DrawSection("Presentation");
            DrawProperty(serialized, "treePosition");

            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced", true);

            if (showAdvanced)
            {
                EditorGUI.indentLevel++;
                DrawProperty(serialized, "id");
                EditorGUI.indentLevel--;
            }

            bool changed = EditorGUI.EndChangeCheck();
            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
                EditorUtility.SetDirty(node);

            EditorGUILayout.Space(10);
            DrawSection("Effect Preview");

            EditorGUILayout.HelpBox(string.IsNullOrWhiteSpace(node.EffectDescription) ? "No effect description is currently available." : node.EffectDescription, MessageType.None);

            EditorGUILayout.Space(8);
            DrawRelatedNavigation(node);

            EditorGUILayout.Space(8);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Node Asset", node, typeof(CharacterMasteryNodeDefinition), false);
            }

            if (GUILayout.Button("Select Node Asset"))
            {
                Selection.activeObject = node;
                EditorGUIUtility.PingObject(node);
            }

            if (character != null)
            {
                string desiredName = GameContentAssetUtility.GetMasteryNodeAssetName(character, node);

                string currentPath = AssetDatabase.GetAssetPath(node);

                string currentName = System.IO.Path.GetFileNameWithoutExtension(currentPath);

                if (currentName != desiredName)
                {
                    EditorGUILayout.HelpBox($"Asset filename is '{currentName}'. " + $"Authored identity suggests '{desiredName}'.", MessageType.Info);

                    if (GUILayout.Button("Sync Asset File Name"))
                    {
                        GameContentAssetUtility.SyncMasteryNodeAssetName(character, node);
                    }
                }
            }

            return changed || applied;
        }

        public static string GetTypeLabel(CharacterMasteryNodeDefinition node)
        {
            return node switch
            {
                CharacterMasteryWeaponModCombinationNodeDefinition => "Mastery Combination",

                CharacterMasteryWeaponModNodeDefinition => "Weapon Mastery Stance",

                CharacterMasteryWeaponModLevelNodeDefinition => "Weapon Mastery Level",

                CharacterMasteryAbilityLevelNodeDefinition => "Ability Level",

                CharacterMasteryStatNodeDefinition => "Character Stat",

                _ => node != null ? node.GetType().Name : "Mastery Node",
            };
        }

        private static void DrawTypeSpecificProperties(SerializedObject serialized, CharacterMasteryNodeDefinition node)
        {
            switch (node)
            {
                case CharacterMasteryStatNodeDefinition:
                    DrawSection("Character Stat");
                    DrawProperty(serialized, "modifierType");
                    DrawProperty(serialized, "amount");
                    break;

                case CharacterMasteryAbilityLevelNodeDefinition:
                    DrawSection("Ability Development");
                    DrawProperty(serialized, "ability");
                    DrawProperty(serialized, "levelIncrease");
                    break;

                case CharacterMasteryWeaponModNodeDefinition:
                    DrawSection("Weapon Mastery Stance");
                    DrawProperty(serialized, "weaponMod");
                    DrawProperty(serialized, "stanceSlot");
                    break;

                case CharacterMasteryWeaponModLevelNodeDefinition:
                    DrawSection("Weapon Mastery Development");
                    DrawProperty(serialized, "weaponMod");
                    DrawProperty(serialized, "preparedLevelIncrease");
                    break;

                case CharacterMasteryWeaponModCombinationNodeDefinition:
                    DrawSection("Mastery Combination");
                    DrawProperty(serialized, "activeMastery");
                    DrawProperty(serialized, "combinedMastery");
                    break;
            }
        }

        private static void DrawRelatedNavigation(CharacterMasteryNodeDefinition node)
        {
            Object first = null;
            Object second = null;
            string firstLabel = null;
            string secondLabel = null;

            switch (node)
            {
                case CharacterMasteryAbilityLevelNodeDefinition abilityNode:
                    first = abilityNode.Ability;
                    firstLabel = "Select Ability";
                    break;

                case CharacterMasteryWeaponModNodeDefinition weaponNode:
                    first = weaponNode.WeaponMod;
                    firstLabel = "Select Weapon Mastery";
                    break;

                case CharacterMasteryWeaponModLevelNodeDefinition levelNode:
                    first = levelNode.WeaponMod;
                    firstLabel = "Select Weapon Mastery";
                    break;

                case CharacterMasteryWeaponModCombinationNodeDefinition combination:
                    first = combination.ActiveMastery;
                    firstLabel = "Select Active Mastery";

                    second = combination.CombinedMastery;
                    secondLabel = "Select Inherited Mastery";
                    break;
            }

            if (first == null && second == null)
                return;

            DrawSection("Related Assets");

            if (first != null && GUILayout.Button(firstLabel))
                SelectRelated(first);

            if (second != null && GUILayout.Button(secondLabel))
                SelectRelated(second);
        }

        private static void SelectRelated(Object related)
        {
            Selection.activeObject = related;
            EditorGUIUtility.PingObject(related);
        }

        private static void DrawSection(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        private static void DrawProperty(SerializedObject serialized, string name, bool includeChildren = false)
        {
            SerializedProperty property = serialized.FindProperty(name);

            if (property != null)
                EditorGUILayout.PropertyField(property, includeChildren);
        }
    }
}
