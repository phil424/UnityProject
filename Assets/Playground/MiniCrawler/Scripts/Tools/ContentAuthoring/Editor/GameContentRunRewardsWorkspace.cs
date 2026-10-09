using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.UI;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentRunRewardsWorkspace
    {
        private const float SetColumnWidth = 235f;
        private const float RewardColumnWidth = 285f;
        private const float InspectorWidth = 390f;

        private readonly List<string> validationIssues = new();
        private readonly List<RunRewardDefinition> uniqueRewards = new();

        private RunRewardCatalogDefinition catalog;
        private RunRewardSetDefinition selectedSet;
        private RunRewardDefinition selectedReward;
        private PartyMemberDefinition character;

        private Vector2 setScroll;
        private Vector2 rewardScroll;
        private Vector2 inspectorScroll;

        private bool showIssues;
        private bool showAdvancedReward;

        private RunRewardSetDefinition existingSetToAdd;
        private RunRewardDefinition existingRewardToAdd;

        public RunRewardCatalogDefinition Catalog => catalog;

        public int UniqueRewardCount
        {
            get
            {
                RefreshUniqueRewards();
                return uniqueRewards.Count;
            }
        }

        public void ResolveCatalog()
        {
            if (catalog == null)
                catalog = GameContentRewardUtility.FindCatalog();

            EnsureSelection();
            RefreshValidation();
        }

        public void SetCharacter(PartyMemberDefinition nextCharacter)
        {
            character = nextCharacter;
        }

        public void SelectCatalog(RunRewardCatalogDefinition nextCatalog)
        {
            catalog = nextCatalog;
            selectedSet = null;
            selectedReward = null;

            EnsureSelection();
            RefreshValidation();
        }

        public void SelectSet(RunRewardSetDefinition rewardSet)
        {
            if (rewardSet == null)
                return;

            if (catalog == null)
                ResolveCatalog();

            selectedSet = rewardSet;
            selectedReward = FirstReward(rewardSet);

            RefreshValidation();
        }

        public void SelectReward(RunRewardDefinition reward)
        {
            if (reward == null)
                return;

            if (catalog == null)
                ResolveCatalog();

            selectedReward = reward;

            RunRewardSetDefinition containingSet = GameContentRewardUtility.FindFirstSetContaining(catalog, reward);

            if (containingSet != null)
                selectedSet = containingSet;
        }

        public void HandleProjectChange()
        {
            if (catalog == null)
                catalog = GameContentRewardUtility.FindCatalog();

            EnsureSelection();
            RefreshValidation();
        }

        public void HandleUndoRedo()
        {
            EnsureSelection();
            RefreshValidation();
        }

        public void Draw()
        {
            if (catalog == null)
            {
                EditorGUILayout.HelpBox("No Run Reward Catalogue could be found.", MessageType.Error);

                if (GUILayout.Button("Refresh Catalogue"))
                    ResolveCatalog();

                return;
            }

            DrawHeader();
            DrawValidation();

            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSetColumn();
                DrawRewardColumn();
                DrawInspector();
            }
        }

        private void DrawHeader()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Run Rewards", EditorStyles.boldLabel, GUILayout.Width(85f));

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(catalog, typeof(RunRewardCatalogDefinition), false, GUILayout.MinWidth(180f));
                }

                GUILayout.FlexibleSpace();

                RefreshUniqueRewards();

                EditorGUILayout.LabelField($"Sets {catalog.RewardSets.Count}   " + $"Rewards {uniqueRewards.Count}", EditorStyles.miniLabel, GUILayout.Width(150f));

                if (GUILayout.Button("+ Reward Set", GUILayout.Width(95f)))
                {
                    GameContentRewardSetCreateWindow.Open(catalog);
                }

                if (GUILayout.Button("Validate", GUILayout.Width(65f)))
                {
                    RefreshValidation();
                }
            }
        }

        private void DrawValidation()
        {
            if (validationIssues.Count == 0)
            {
                EditorGUILayout.LabelField("Validation: Healthy", EditorStyles.miniLabel);

                return;
            }

            EditorGUILayout.HelpBox($"Run Reward Catalogue has " + $"{validationIssues.Count} authoring issue(s).", MessageType.Warning);

            showIssues = EditorGUILayout.Foldout(showIssues, "Issues", true);

            if (!showIssues)
                return;

            foreach (string issue in validationIssues)
            {
                EditorGUILayout.LabelField($"- {issue}", EditorStyles.wordWrappedMiniLabel);
            }
        }

        private void DrawSetColumn()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(SetColumnWidth), GUILayout.ExpandHeight(true)))
            {
                EditorGUILayout.LabelField("Reward Sets", EditorStyles.boldLabel);

                setScroll = EditorGUILayout.BeginScrollView(setScroll);

                foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
                {
                    if (rewardSet == null)
                        continue;

                    string label = $"{rewardSet.name}  ({rewardSet.Rewards.Count})";

                    GUIStyle style = rewardSet == selectedSet ? EditorStyles.miniButtonMid : EditorStyles.miniButton;

                    if (GUILayout.Button(label, style))
                    {
                        selectedSet = rewardSet;
                        selectedReward = FirstReward(rewardSet);

                        Selection.activeObject = rewardSet;
                    }
                }

                EditorGUILayout.EndScrollView();

                if (selectedSet != null)
                {
                    EditorGUILayout.Space(6);

                    string nextName = EditorGUILayout.DelayedTextField("Asset Name", selectedSet.name);

                    if (nextName != selectedSet.name)
                    {
                        string desiredName = GameContentAssetUtility.GetRewardSetAssetName(nextName);

                        GameContentAssetUtility.TryRenameAsset(selectedSet, desiredName);
                    }

                    if (GUILayout.Button("Select Set Asset"))
                    {
                        Selection.activeObject = selectedSet;

                        EditorGUIUtility.PingObject(selectedSet);
                    }

                    if (GUILayout.Button("Remove Set From Catalogue"))
                    {
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Remove Reward Set",
                            $"Remove '{selectedSet.name}' from the active Catalogue?\n\n" + "The Reward Set asset and Reward assets will NOT be deleted.",
                            "Remove",
                            "Cancel"
                        );

                        if (confirmed)
                        {
                            GameContentRewardUtility.RemoveSet(catalog, selectedSet);

                            selectedSet = null;
                            selectedReward = null;

                            EnsureSelection();
                            RefreshValidation();
                        }
                    }
                }

                EditorGUILayout.Space(8);

                if (GUILayout.Button("+ New Reward Set"))
                {
                    GameContentRewardSetCreateWindow.Open(catalog);
                }

                EditorGUILayout.LabelField("Add Existing", EditorStyles.miniBoldLabel);

                existingSetToAdd = (RunRewardSetDefinition)EditorGUILayout.ObjectField(existingSetToAdd, typeof(RunRewardSetDefinition), false);

                using (new EditorGUI.DisabledScope(existingSetToAdd == null))
                {
                    if (GUILayout.Button("Add Existing Set"))
                    {
                        if (GameContentRewardUtility.AddSet(catalog, existingSetToAdd))
                        {
                            selectedSet = existingSetToAdd;

                            selectedReward = FirstReward(selectedSet);

                            existingSetToAdd = null;

                            RefreshValidation();
                        }
                    }
                }
            }
        }

        private void DrawRewardColumn()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(RewardColumnWidth), GUILayout.ExpandHeight(true)))
            {
                if (selectedSet == null)
                {
                    EditorGUILayout.HelpBox("Select or create a Reward Set.", MessageType.Info);

                    return;
                }

                EditorGUILayout.LabelField(selectedSet.name, EditorStyles.boldLabel);

                rewardScroll = EditorGUILayout.BeginScrollView(rewardScroll);

                foreach (RunRewardDefinition reward in selectedSet.Rewards)
                {
                    if (reward != null)
                        DrawRewardRow(reward);
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(6);

                if (GUILayout.Button("+ New Reward"))
                {
                    GameContentRewardCreateWindow.Open(catalog, selectedSet);
                }

                EditorGUILayout.LabelField("Add Existing", EditorStyles.miniBoldLabel);

                existingRewardToAdd = (RunRewardDefinition)EditorGUILayout.ObjectField(existingRewardToAdd, typeof(RunRewardDefinition), false);

                using (new EditorGUI.DisabledScope(existingRewardToAdd == null))
                {
                    if (GUILayout.Button("Add Existing Reward"))
                    {
                        if (GameContentRewardUtility.AddReward(selectedSet, existingRewardToAdd))
                        {
                            selectedReward = existingRewardToAdd;

                            existingRewardToAdd = null;

                            RefreshValidation();
                        }
                    }
                }

                if (selectedReward != null && GameContentRewardUtility.ContainsReward(selectedSet, selectedReward))
                {
                    EditorGUILayout.Space(6);

                    if (GUILayout.Button("Remove Selected From Set"))
                    {
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Remove Reward Membership",
                            $"Remove '{selectedReward.DisplayName}' from '{selectedSet.name}'?\n\n" + "The Reward asset will NOT be deleted.",
                            "Remove",
                            "Cancel"
                        );

                        if (confirmed)
                        {
                            RunRewardDefinition removed = selectedReward;

                            GameContentRewardUtility.RemoveReward(selectedSet, removed);

                            selectedReward = FirstReward(selectedSet);

                            RefreshValidation();
                        }
                    }
                }
            }
        }

        private void DrawRewardRow(RunRewardDefinition reward)
        {
            Rect row = GUILayoutUtility.GetRect(0f, 42f, GUILayout.ExpandWidth(true));

            GUIStyle style = reward == selectedReward ? EditorStyles.helpBox : GUI.skin.button;

            if (GUI.Button(row, GUIContent.none, style))
            {
                selectedReward = reward;
                Selection.activeObject = reward;
            }

            Color vectorColor = GameContentAuthoringPalette.GetRewardVectorColor(reward.RewardVector);

            Rect accent = new(row.x + 3f, row.y + 3f, 5f, row.height - 6f);

            EditorGUI.DrawRect(accent, vectorColor);

            Rect nameRect = new(row.x + 13f, row.y + 5f, row.width - 18f, 18f);

            Rect detailRect = new(row.x + 13f, row.y + 23f, row.width - 18f, 16f);

            GUI.Label(nameRect, reward.DisplayName, EditorStyles.boldLabel);

            string rarity = RunUpgradeRarityPresentation.GetLabel(reward.Rarity);

            GUI.Label(detailRect, $"{RunRewardVectorPresentation.GetDisplayName(reward.RewardVector)}  •  {rarity}", EditorStyles.miniLabel);
        }

        private void DrawInspector()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(InspectorWidth), GUILayout.ExpandHeight(true)))
            {
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

                bool changed = GameContentRewardInspector.Draw(selectedReward, character, catalog, ref showAdvancedReward);

                if (changed)
                    RefreshValidation();

                EditorGUILayout.EndScrollView();
            }
        }

        private void EnsureSelection()
        {
            if (catalog == null)
                return;

            if (selectedSet == null || !GameContentRewardUtility.ContainsSet(catalog, selectedSet))
            {
                selectedSet = FirstSet();
            }

            if (selectedReward == null)
            {
                selectedReward = FirstReward(selectedSet);

                return;
            }

            if (selectedSet != null && GameContentRewardUtility.ContainsReward(selectedSet, selectedReward))
            {
                return;
            }

            RunRewardSetDefinition containing = GameContentRewardUtility.FindFirstSetContaining(catalog, selectedReward);

            if (containing != null)
            {
                selectedSet = containing;
                return;
            }

            selectedReward = FirstReward(selectedSet);
        }

        private RunRewardSetDefinition FirstSet()
        {
            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet != null)
                    return rewardSet;
            }

            return null;
        }

        private static RunRewardDefinition FirstReward(RunRewardSetDefinition rewardSet)
        {
            if (rewardSet == null)
                return null;

            foreach (RunRewardDefinition reward in rewardSet.Rewards)
            {
                if (reward != null)
                    return reward;
            }

            return null;
        }

        private void RefreshUniqueRewards()
        {
            GameContentRewardUtility.CollectUniqueRewards(catalog, uniqueRewards);
        }

        private void RefreshValidation()
        {
            GameContentRewardValidation.Collect(catalog, validationIssues);
        }
    }
}
