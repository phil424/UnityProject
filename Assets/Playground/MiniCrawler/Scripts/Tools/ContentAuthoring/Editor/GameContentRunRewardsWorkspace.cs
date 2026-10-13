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
        private bool showRarityProfile = true;
        private bool showAdvancedReward;
        private bool validationDirty;

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
            DrawRarityProfile();
            DrawValidation();
            DrawMissingReferenceRepair();

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

        private void DrawRarityProfile()
        {
            showRarityProfile = EditorGUILayout.Foldout(showRarityProfile, "Rarity Profile", true);

            if (!showRarityProfile)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Baseline Rarity Weights", EditorStyles.miniBoldLabel);

                EditorGUILayout.LabelField(
                    "Each Reward slot rolls a rarity from the tiers represented by its " + "eligible candidate pool. Unavailable tiers are skipped.",
                    EditorStyles.wordWrappedMiniLabel
                );

                SerializedObject serialized = new(catalog);

                serialized.Update();

                EditorGUI.BeginChangeCheck();

                DrawRarityWeight(serialized, "commonRarityWeight", "Common");

                DrawRarityWeight(serialized, "uncommonRarityWeight", "Uncommon");

                DrawRarityWeight(serialized, "rareRarityWeight", "Rare");

                DrawRarityWeight(serialized, "epicRarityWeight", "Epic");

                DrawRarityWeight(serialized, "legendaryRarityWeight", "Legendary");

                if (EditorGUI.EndChangeCheck())
                {
                    serialized.ApplyModifiedProperties();

                    RefreshValidation();
                }

                if (!catalog.HasPositiveRarityWeight)
                {
                    EditorGUILayout.HelpBox("At least one rarity must have a positive weight.", MessageType.Warning);

                    return;
                }

                EditorGUILayout.Space(3);

                EditorGUILayout.LabelField("Baseline chance when all five tiers are represented:", EditorStyles.miniLabel);

                string chanceSummary =
                    $"Common {FormatChance(RunUpgradeRarity.Common)}   "
                    + $"Uncommon {FormatChance(RunUpgradeRarity.Uncommon)}   "
                    + $"Rare {FormatChance(RunUpgradeRarity.Rare)}   "
                    + $"Epic {FormatChance(RunUpgradeRarity.Epic)}   "
                    + $"Legendary {FormatChance(RunUpgradeRarity.Legendary)}";

                EditorGUILayout.LabelField(chanceSummary, EditorStyles.wordWrappedMiniLabel);
            }
        }

        private static void DrawRarityWeight(SerializedObject serialized, string propertyName, string label)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);

            if (property == null)
                return;

            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        private string FormatChance(RunUpgradeRarity rarity)
        {
            return $"{catalog.GetBaselineRarityChance(rarity) * 100f:0.#}%";
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

        private void DrawMissingReferenceRepair()
        {
            int missingReferences = GameContentRewardUtility.CountMissingReferences(catalog);

            if (missingReferences <= 0)
                return;

            EditorGUILayout.Space(4);

            EditorGUILayout.HelpBox($"The active Reward Catalogue contains " + $"{missingReferences} missing serialized reference(s).", MessageType.Warning);

            string label = missingReferences == 1 ? "Clean 1 Missing Reference" : $"Clean {missingReferences} Missing References";

            if (!GUILayout.Button(label))
                return;

            bool confirmed = EditorUtility.DisplayDialog(
                "Clean Missing Reward References",
                $"Remove {missingReferences} broken/null Reward "
                    + "Catalogue reference(s)?\n\n"
                    + "Only missing Set memberships and missing Reward "
                    + "memberships will be removed.\n\n"
                    + "Valid assets are not deleted.",
                "Clean References",
                "Cancel"
            );

            if (!confirmed)
                return;

            if (GameContentRewardUtility.CleanMissingReferences(catalog))
            {
                EnsureSelection();
                RefreshValidation();
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

                    if (GUILayout.Button("Delete Reward Set Asset..."))
                    {
                        RunRewardSetDefinition setToDelete = selectedSet;

                        string assetPath = AssetDatabase.GetAssetPath(setToDelete);

                        int catalogueMemberships = GameContentRewardUtility.CountProjectCatalogueMemberships(setToDelete);

                        int rewardCount = setToDelete.Rewards.Count;

                        bool confirmed = EditorUtility.DisplayDialog(
                            "Delete Reward Set Asset",
                            $"Permanently delete '{setToDelete.name}'?\n\n"
                                + $"Asset:\n{assetPath}\n\n"
                                + $"Catalogue memberships: {catalogueMemberships}\n"
                                + $"Contained Reward references: {rewardCount}\n\n"
                                + "The Reward Set will be removed from every Run Reward "
                                + "Catalogue in the project.\n\n"
                                + "The Reward assets contained by this Set will NOT be deleted.\n\n"
                                + "The Reward Set asset itself will be permanently deleted. "
                                + "Asset deletion is not treated as an Undo operation.",
                            "Delete Set Asset",
                            "Cancel"
                        );

                        if (confirmed)
                        {
                            if (existingSetToAdd == setToDelete)
                            {
                                existingSetToAdd = null;
                            }

                            Selection.activeObject = catalog;

                            bool deleted = GameContentRewardUtility.DeleteRewardSetAsset(setToDelete);

                            if (deleted)
                            {
                                selectedSet = null;
                                selectedReward = null;

                                EnsureSelection();
                                RefreshValidation();
                            }
                            else
                            {
                                Selection.activeObject = setToDelete;

                                EditorUtility.DisplayDialog("Delete Failed", "The Reward Set asset could not be deleted. " + "Its catalogue memberships were restored.", "OK");
                            }
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

                if (selectedReward != null)
                {
                    EditorGUILayout.Space(6);

                    if (GameContentRewardUtility.ContainsReward(selectedSet, selectedReward))
                    {
                        if (GUILayout.Button("Remove Selected From Set"))
                        {
                            bool confirmed = EditorUtility.DisplayDialog(
                                "Remove Reward Membership",
                                $"Remove '{selectedReward.DisplayName}' " + $"from '{selectedSet.name}'?\n\n" + "The Reward asset will NOT be deleted.",
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

                    int projectMemberships = GameContentRewardUtility.CountProjectRewardMemberships(selectedReward);

                    if (GUILayout.Button("Delete Selected Reward Asset..."))
                    {
                        RunRewardDefinition rewardToDelete = selectedReward;

                        string assetPath = AssetDatabase.GetAssetPath(rewardToDelete);

                        bool confirmed = EditorUtility.DisplayDialog(
                            "Delete Run Reward Asset",
                            $"Permanently delete "
                                + $"'{rewardToDelete.DisplayName}'?\n\n"
                                + $"Asset:\n{assetPath}\n\n"
                                + $"Reward Set memberships across project: "
                                + $"{projectMemberships}\n\n"
                                + "The Reward will first be removed from every "
                                + "Reward Set in the project.\n\n"
                                + "The Reward ScriptableObject will then be "
                                + "permanently deleted.\n\n"
                                + "Asset deletion is not treated as an Undo operation.",
                            "Delete Reward Asset",
                            "Cancel"
                        );

                        if (confirmed)
                        {
                            if (existingRewardToAdd == rewardToDelete)
                            {
                                existingRewardToAdd = null;
                            }

                            Selection.activeObject = selectedSet != null ? selectedSet : catalog;

                            bool deleted = GameContentRewardUtility.DeleteRewardAsset(rewardToDelete);

                            if (deleted)
                            {
                                selectedReward = FirstReward(selectedSet);

                                EnsureSelection();
                                RefreshValidation();
                            }
                            else
                            {
                                Selection.activeObject = rewardToDelete;

                                EditorUtility.DisplayDialog("Delete Failed", "The Reward asset could not be deleted. " + "Its Reward Set memberships were restored.", "OK");
                            }
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

            Color rarityColor = RunUpgradeRarityPresentation.GetColor(reward.Rarity);

            Rect accent = new(row.x + 3f, row.y + 3f, 5f, row.height - 6f);

            EditorGUI.DrawRect(accent, vectorColor);

            DrawRewardRarityOutline(row, rarityColor);

            Rect nameRect = new(row.x + 13f, row.y + 5f, row.width - 18f, 18f);

            Rect detailRect = new(row.x + 13f, row.y + 23f, row.width - 18f, 16f);

            GUI.Label(nameRect, reward.DisplayName, EditorStyles.boldLabel);

            Rect vectorRect = new(detailRect.x, detailRect.y, detailRect.width * 0.6f, detailRect.height);

            Rect rarityRect = new(vectorRect.xMax, detailRect.y, detailRect.width * 0.4f, detailRect.height);

            GUI.Label(vectorRect, RunRewardVectorPresentation.GetDisplayName(reward.RewardVector), EditorStyles.miniLabel);

            GUIStyle rarityStyle = new(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleRight };

            rarityStyle.normal.textColor = rarityColor;

            GUI.Label(rarityRect, RunUpgradeRarityPresentation.GetLabel(reward.Rarity), rarityStyle);
        }

        private static void DrawRewardRarityOutline(Rect rect, Color color)
        {
            Rect outline = new(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);

            Handles.BeginGUI();
            Handles.color = color;

            Vector3[] points = { new(outline.xMin, outline.yMin), new(outline.xMax, outline.yMin), new(outline.xMax, outline.yMax), new(outline.xMin, outline.yMax), new(outline.xMin, outline.yMin) };

            Handles.DrawAAPolyLine(1.5f, points);

            Handles.EndGUI();
        }

        private void DrawInspector()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(InspectorWidth), GUILayout.ExpandHeight(true)))
            {
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

                bool changed = GameContentRewardInspector.Draw(selectedReward, character, catalog, ref showAdvancedReward);

                if (changed)
                    validationDirty = true;

                if (validationDirty && !EditorGUIUtility.editingTextField)
                {
                    RefreshValidation();
                }

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
            validationDirty = false;

            GameContentRewardValidation.Collect(catalog, validationIssues);
        }
    }
}
