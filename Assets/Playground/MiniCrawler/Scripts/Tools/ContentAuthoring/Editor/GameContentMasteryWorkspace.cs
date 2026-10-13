using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Tools.MasteryAuthoring;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentMasteryWorkspace
    {
        private const float InspectorWidth = 380f;
        private const float MinimumCanvasWidth = 800f;
        private const float MinimumCanvasHeight = 600f;

        private readonly CharacterMasteryGraphEditor graph = new();
        private readonly List<string> validationIssues = new();

        private PartyMemberDefinition character;
        private CharacterMasteryNodeDefinition existingNodeToAdd;

        private Vector2 inspectorScrollPosition;
        private bool showIssues;
        private bool showAdvancedNodeFields;

        private CharacterMasteryDefinition Mastery => character != null ? character.MasteryDefinition : null;

        public void SetCharacter(PartyMemberDefinition nextCharacter)
        {
            if (character == nextCharacter)
                return;

            character = nextCharacter;
            existingNodeToAdd = null;
            inspectorScrollPosition = Vector2.zero;
            showIssues = false;
            showAdvancedNodeFields = false;

            graph.SetCharacter(character);
            RefreshValidation();
        }

        public bool OwnsNode(CharacterMasteryNodeDefinition node)
        {
            return graph.OwnsNode(node);
        }

        public void SelectNode(CharacterMasteryNodeDefinition node)
        {
            graph.SelectNode(node);
        }

        public void HandleProjectChange()
        {
            graph.HandleProjectChange();
            RefreshValidation();
        }

        public void HandleUndoRedo()
        {
            graph.HandleProjectChange();
            RefreshValidation();
        }

        public void Draw()
        {
            if (character == null)
            {
                EditorGUILayout.HelpBox("Select a Character to author Mastery.", MessageType.Info);

                return;
            }

            if (Mastery == null)
            {
                EditorGUILayout.HelpBox($"'{character.DisplayName}' has no Character Mastery Definition assigned.", MessageType.Error);

                return;
            }

            DrawHeader();
            DrawValidationSummary();

            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                bool graphChanged = graph.Draw();

                DrawInspector();

                if (graphChanged)
                    RefreshValidation();
            }
        }

        private void DrawHeader()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Character Mastery", EditorStyles.boldLabel, GUILayout.Width(125f));

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(Mastery, typeof(CharacterMasteryDefinition), false, GUILayout.MinWidth(180f));
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField($"Nodes {CountNodes()}   Connections {CountConnections()}", EditorStyles.miniLabel, GUILayout.Width(190f));

                if (GUILayout.Button("+ Node", GUILayout.Width(70f)))
                    graph.ShowCreateMenuAtOpenPosition();

                if (GUILayout.Button("Center", GUILayout.Width(60f)))
                    graph.Center();

                if (GUILayout.Button("Validate", GUILayout.Width(65f)))
                    RefreshValidation();
            }
        }

        private void DrawValidationSummary()
        {
            if (validationIssues.Count == 0)
            {
                EditorGUILayout.LabelField("Validation: Healthy", EditorStyles.miniLabel);

                return;
            }

            EditorGUILayout.HelpBox($"Character Mastery has {validationIssues.Count} authoring issue(s).", MessageType.Warning);

            showIssues = EditorGUILayout.Foldout(showIssues, "Issues", true);

            if (!showIssues)
                return;

            foreach (string issue in validationIssues)
            {
                EditorGUILayout.LabelField($"- {issue}", EditorStyles.wordWrappedMiniLabel);
            }
        }

        private void DrawInspector()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(InspectorWidth), GUILayout.ExpandHeight(true)))
            {
                inspectorScrollPosition = EditorGUILayout.BeginScrollView(inspectorScrollPosition);

                DrawTreeSettings();

                EditorGUILayout.Space(10);

                bool changed = CharacterMasteryWorkbenchInspector.Draw(character, graph.SelectedNode, ref showAdvancedNodeFields);

                if (changed)
                    RefreshValidation();

                DrawNodeActions();

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawTreeSettings()
        {
            EditorGUILayout.LabelField("Tree", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            Vector2 nextSize = EditorGUILayout.Vector2Field("Canvas Size", Mastery.TreeSize);

            if (EditorGUI.EndChangeCheck())
            {
                nextSize.x = Mathf.Max(MinimumCanvasWidth, nextSize.x);
                nextSize.y = Mathf.Max(MinimumCanvasHeight, nextSize.y);

                SetCanvasSize(nextSize);
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Tree Membership", EditorStyles.miniBoldLabel);

            existingNodeToAdd = (CharacterMasteryNodeDefinition)EditorGUILayout.ObjectField("Existing Node", existingNodeToAdd, typeof(CharacterMasteryNodeDefinition), false);

            bool canAdd = existingNodeToAdd != null && !Mastery.Contains(existingNodeToAdd);

            using (new EditorGUI.DisabledScope(!canAdd))
            {
                if (GUILayout.Button("Add Existing Node"))
                {
                    CharacterMasteryNodeDefinition added = existingNodeToAdd;

                    if (CharacterMasteryWorkbenchContentUtility.AddNodeToTree(Mastery, added))
                    {
                        existingNodeToAdd = null;
                        graph.SelectNode(added);

                        Selection.activeObject = added;
                        EditorGUIUtility.PingObject(added);

                        RefreshValidation();
                    }
                }
            }

            int missingReferences = CharacterMasteryWorkbenchContentUtility.CountMissingReferences(Mastery);

            if (missingReferences <= 0)
                return;

            EditorGUILayout.Space(6);

            EditorGUILayout.HelpBox($"This Mastery tree contains {missingReferences} missing serialized reference(s).", MessageType.Warning);

            string label = missingReferences == 1 ? "Clean 1 Missing Reference" : $"Clean {missingReferences} Missing References";

            if (!GUILayout.Button(label))
                return;

            bool confirmed = EditorUtility.DisplayDialog(
                "Clean Missing Mastery References",
                $"Remove {missingReferences} broken/null reference(s) from "
                    + $"'{character.DisplayName}' Mastery?\n\n"
                    + "Only missing node memberships and missing prerequisite entries "
                    + "will be removed. Valid assets are not deleted.",
                "Clean References",
                "Cancel"
            );

            if (!confirmed)
                return;

            if (CharacterMasteryWorkbenchContentUtility.CleanMissingReferences(Mastery))
            {
                graph.HandleProjectChange();
                RefreshValidation();
            }
        }

        private void DrawNodeActions()
        {
            CharacterMasteryNodeDefinition node = graph.SelectedNode;

            if (node == null)
                return;

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Content Actions", EditorStyles.boldLabel);

            int referenceCount = CharacterMasteryWorkbenchContentUtility.CountReferencesToNode(Mastery, node);

            if (referenceCount > 0)
            {
                EditorGUILayout.HelpBox(
                    $"{referenceCount} other in-tree reference(s) currently "
                        + "depend on this node. Removing or deleting it will clear "
                        + "those references and validation will report any content "
                        + "that then requires repair.",
                    MessageType.Info
                );
            }

            if (GUILayout.Button("Remove From Tree"))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Remove Mastery Node From Tree",
                    $"Remove '{node.DisplayName}' from "
                        + $"'{character.DisplayName}' Mastery?\n\n"
                        + "The node asset will remain in the project.\n\n"
                        + "Prerequisite and direct Mastery-combination references "
                        + "to this node inside the tree will be cleared.",
                    "Remove From Tree",
                    "Cancel"
                );

                if (confirmed && CharacterMasteryWorkbenchContentUtility.RemoveNodeFromTree(Mastery, node))
                {
                    existingNodeToAdd = node;
                    graph.SelectNode(null);
                    RefreshValidation();
                }
            }

            string assetPath = AssetDatabase.GetAssetPath(node);

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(assetPath)))
            {
                if (!GUILayout.Button("Delete Node Asset..."))
                    return;
            }

            bool deleteConfirmed = EditorUtility.DisplayDialog(
                "Delete Mastery Node Asset",
                $"Permanently delete '{node.DisplayName}'?\n\n"
                    + $"Asset:\n{assetPath}\n\n"
                    + "The node will first be removed from this Mastery tree and "
                    + "its in-tree prerequisite / combination references will be cleaned.\n\n"
                    + "The ScriptableObject asset will then be permanently deleted. "
                    + "Asset deletion is not treated as an Undo operation.",
                "Delete Asset",
                "Cancel"
            );

            if (!deleteConfirmed)
                return;

            if (existingNodeToAdd == node)
                existingNodeToAdd = null;

            Selection.activeObject = Mastery;
            graph.SelectNode(null);

            bool deleted = CharacterMasteryWorkbenchContentUtility.DeleteNodeAsset(Mastery, node);

            if (!deleted)
            {
                existingNodeToAdd = node;

                EditorUtility.DisplayDialog("Delete Failed", "The node asset could not be deleted. It has been left " + "available for inspection or re-adding to the tree.", "OK");
            }

            RefreshValidation();
        }

        private void SetCanvasSize(Vector2 size)
        {
            Undo.RecordObject(Mastery, "Resize Mastery Tree Canvas");

            SerializedObject serialized = new(Mastery);
            SerializedProperty property = serialized.FindProperty("treeSize");

            if (property == null)
                return;

            property.vector2Value = size;

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(Mastery);

            graph.Center();
            RefreshValidation();
        }

        private void RefreshValidation()
        {
            CharacterMasteryWorkbenchValidation.CollectIssues(character, Mastery, CharacterMasteryGraphEditor.NodeSize, validationIssues);
        }

        private int CountNodes()
        {
            if (Mastery == null)
                return 0;

            int count = 0;

            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node != null)
                    count++;
            }

            return count;
        }

        private int CountConnections()
        {
            if (Mastery == null)
                return 0;

            int count = 0;

            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node != null)
                    count += node.Prerequisites.Count;
            }

            return count;
        }
    }
}
