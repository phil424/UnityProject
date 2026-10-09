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

        private Vector2 inspectorScrollPosition;
        private bool showIssues;
        private bool showAdvancedNodeFields;

        private CharacterMasteryDefinition Mastery => character != null ? character.MasteryDefinition : null;

        public void SetCharacter(PartyMemberDefinition nextCharacter)
        {
            if (character == nextCharacter)
                return;

            character = nextCharacter;
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

                EditorGUILayout.LabelField($"Nodes {CountNodes()}   " + $"Connections {CountConnections()}", EditorStyles.miniLabel, GUILayout.Width(190f));

                if (GUILayout.Button("+ Node", GUILayout.Width(70f)))
                {
                    graph.ShowCreateMenuAtOpenPosition();
                }

                if (GUILayout.Button("Center", GUILayout.Width(60f)))
                {
                    graph.Center();
                }

                if (GUILayout.Button("Validate", GUILayout.Width(65f)))
                {
                    RefreshValidation();
                }
            }
        }

        private void DrawValidationSummary()
        {
            if (validationIssues.Count == 0)
            {
                EditorGUILayout.LabelField("Validation: Healthy", EditorStyles.miniLabel);

                return;
            }

            EditorGUILayout.HelpBox($"Character Mastery has " + $"{validationIssues.Count} authoring issue(s).", MessageType.Warning);

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

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawTreeSettings()
        {
            EditorGUILayout.LabelField("Tree", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            Vector2 nextSize = EditorGUILayout.Vector2Field("Canvas Size", Mastery.TreeSize);

            if (!EditorGUI.EndChangeCheck())
                return;

            nextSize.x = Mathf.Max(MinimumCanvasWidth, nextSize.x);

            nextSize.y = Mathf.Max(MinimumCanvasHeight, nextSize.y);

            SetCanvasSize(nextSize);
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
