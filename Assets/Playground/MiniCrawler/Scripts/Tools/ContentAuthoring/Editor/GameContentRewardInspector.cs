using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.UI;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentRewardInspector
    {
        public static bool Draw(RunRewardDefinition reward, PartyMemberDefinition character, RunRewardCatalogDefinition catalog, ref bool showAdvanced)
        {
            if (reward == null)
            {
                EditorGUILayout.HelpBox("Select a Run Reward to inspect or edit it.", MessageType.Info);

                return false;
            }

            Color vectorColor = GameContentAuthoringPalette.GetRewardVectorColor(reward.RewardVector);

            EditorGUILayout.LabelField(reward.DisplayName, EditorStyles.largeLabel);

            GUIStyle typeStyle = new(EditorStyles.miniBoldLabel);

            typeStyle.normal.textColor = vectorColor;

            EditorGUILayout.LabelField(GetTypeLabel(reward), typeStyle);

            EditorGUILayout.Space(6);

            SerializedObject serialized = new(reward);

            serialized.Update();

            EditorGUI.BeginChangeCheck();

            DrawSection("Identity");
            DrawProperty(serialized, "displayName");
            DrawProperty(serialized, "icon");
            DrawProperty(serialized, "rarity");

            EditorGUILayout.Space(8);

            DrawTypeSpecificProperties(serialized, reward);

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
                EditorUtility.SetDirty(reward);

            EditorGUILayout.Space(12);

            DrawPreview(reward, character);

            EditorGUILayout.Space(10);

            DrawMemberships(reward, catalog);

            EditorGUILayout.Space(10);

            DrawRelatedNavigation(reward);

            EditorGUILayout.Space(10);

            DrawAssetIdentity(reward);

            return changed || applied;
        }

        public static string GetTypeLabel(RunRewardDefinition reward)
        {
            return reward switch
            {
                RunUpgradeDefinition => "Character Growth Reward",

                AbilityUnlockRewardDefinition => "Ability Unlock Reward",

                AbilityEvolutionRewardDefinition => "Ability Evolution Reward",

                WeaponModLevelRewardDefinition => "Weapon Mastery Level Reward",

                _ => reward != null ? reward.GetType().Name : "Run Reward",
            };
        }

        private static void DrawTypeSpecificProperties(SerializedObject serialized, RunRewardDefinition reward)
        {
            switch (reward)
            {
                case RunUpgradeDefinition:
                    DrawSection("Character Growth");
                    DrawProperty(serialized, "effectType");
                    DrawProperty(serialized, "amount");

                    EditorGUILayout.Space(8);

                    DrawSection("Eligibility");
                    DrawProperty(serialized, "eligibleMembers", true);
                    break;

                case AbilityUnlockRewardDefinition:
                    DrawSection("Ability Unlock");
                    DrawProperty(serialized, "ability");
                    DrawProperty(serialized, "eligibleMembers", true);
                    break;

                case AbilityEvolutionRewardDefinition:
                    DrawSection("Ability Evolution");
                    DrawProperty(serialized, "evolution");
                    break;

                case WeaponModLevelRewardDefinition:
                    DrawSection("Weapon Mastery Development");
                    DrawProperty(serialized, "weaponMod");
                    DrawProperty(serialized, "levelIncrease");
                    break;
            }
        }

        private static void DrawPreview(RunRewardDefinition reward, PartyMemberDefinition character)
        {
            DrawSection("Player-Facing Preview");

            Color rarityColor = RunUpgradeRarityPresentation.GetColor(reward.Rarity);

            Rect accent = EditorGUILayout.GetControlRect(false, 5f);

            EditorGUI.DrawRect(accent, rarityColor);

            GUIStyle titleStyle = new(EditorStyles.boldLabel);

            titleStyle.normal.textColor = rarityColor;

            string rarityLabel = RunUpgradeRarityPresentation.GetLabel(reward.Rarity);

            EditorGUILayout.LabelField($"{rarityLabel}  •  {reward.DisplayName}", titleStyle);

            EditorGUILayout.LabelField(reward.Description, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField($"Reward Vector: " + $"{RunRewardVectorPresentation.GetDisplayName(reward.RewardVector)}", EditorStyles.miniLabel);

            if (reward.Icon != null)
            {
                Texture2D preview = AssetPreview.GetAssetPreview(reward.Icon) ?? AssetPreview.GetMiniThumbnail(reward.Icon);

                if (preview != null)
                {
                    GUILayout.Label(preview, GUILayout.Width(64f), GUILayout.Height(64f));
                }
            }

            EditorGUILayout.Space(6);

            DrawEligibility(reward, character);
        }

        private static void DrawEligibility(RunRewardDefinition reward, PartyMemberDefinition character)
        {
            RunRewardPreviewEligibility eligibility = GameContentRewardPreviewUtility.Evaluate(reward, character);

            switch (eligibility)
            {
                case RunRewardPreviewEligibility.NoCharacter:
                    EditorGUILayout.HelpBox("Select a Character in the left sidebar to preview Reward eligibility.", MessageType.Info);
                    break;

                case RunRewardPreviewEligibility.Unconfigured:
                    EditorGUILayout.HelpBox("Reward is not fully configured yet.", MessageType.Warning);
                    break;

                case RunRewardPreviewEligibility.StartingBuild:
                    EditorGUILayout.HelpBox($"Eligible for {character.DisplayName}'s authored starting build.", MessageType.Info);
                    break;

                case RunRewardPreviewEligibility.FullMastery:
                    EditorGUILayout.HelpBox($"Not eligible at {character.DisplayName}'s starting state, " + "but eligible after authored persistent Mastery progression.", MessageType.Info);
                    break;

                case RunRewardPreviewEligibility.Ineligible:
                    EditorGUILayout.HelpBox(
                        $"Not eligible in {character.DisplayName}'s starting/full-Mastery authoring preview. " + "It may still become eligible after other Expedition rewards.",
                        MessageType.Warning
                    );
                    break;
            }
        }

        private static void DrawMemberships(RunRewardDefinition reward, RunRewardCatalogDefinition catalog)
        {
            DrawSection("Catalogue Membership");

            List<RunRewardSetDefinition> memberships = new();

            GameContentRewardUtility.CollectMemberships(catalog, reward, memberships);

            if (memberships.Count == 0)
            {
                EditorGUILayout.HelpBox("Reward is not currently included in any Set in this Catalogue.", MessageType.Warning);

                return;
            }

            foreach (RunRewardSetDefinition rewardSet in memberships)
            {
                EditorGUILayout.ObjectField(rewardSet, typeof(RunRewardSetDefinition), false);
            }
        }

        private static void DrawRelatedNavigation(RunRewardDefinition reward)
        {
            Object related = null;
            string label = null;

            switch (reward)
            {
                case AbilityUnlockRewardDefinition unlock:
                    related = unlock.Ability;
                    label = "Select Ability";
                    break;

                case AbilityEvolutionRewardDefinition evolution:
                    related = evolution.Evolution;
                    label = "Select Evolution";
                    break;

                case WeaponModLevelRewardDefinition weaponMod:
                    related = weaponMod.WeaponMod;
                    label = "Select Weapon Mastery";
                    break;
            }

            if (related == null)
                return;

            DrawSection("Related Content");

            if (GUILayout.Button(label))
            {
                Selection.activeObject = related;
                EditorGUIUtility.PingObject(related);
            }
        }

        private static void DrawAssetIdentity(RunRewardDefinition reward)
        {
            DrawSection("Asset Identity");

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Reward Asset", reward, typeof(RunRewardDefinition), false);
            }

            string path = AssetDatabase.GetAssetPath(reward);

            string currentName = System.IO.Path.GetFileNameWithoutExtension(path);

            string desiredName = GameContentAssetUtility.GetRunRewardAssetName(reward.DisplayName);

            if (currentName != desiredName)
            {
                EditorGUILayout.HelpBox($"Asset filename is '{currentName}'. " + $"Authored identity suggests '{desiredName}'.", MessageType.Info);

                if (GUILayout.Button("Sync Reward Asset File Name"))
                {
                    GameContentAssetUtility.SyncRunRewardAssetName(reward);
                }
            }

            if (GUILayout.Button("Select Reward Asset"))
            {
                Selection.activeObject = reward;
                EditorGUIUtility.PingObject(reward);
            }
        }

        private static void DrawSection(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        private static void DrawProperty(SerializedObject serialized, string name, bool includeChildren = false)
        {
            SerializedProperty property = serialized.FindProperty(name);

            if (property != null)
            {
                EditorGUILayout.PropertyField(property, includeChildren);
            }
        }
    }
}
