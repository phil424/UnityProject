using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Tools.ContentAuthoring;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal sealed class CharacterMasteryNodeCreateWindow : EditorWindow
    {
        private PartyMemberDefinition character;
        private CharacterMasteryDefinition mastery;
        private CharacterMasteryNodeKind kind;
        private Vector2 treePosition;

        private string displayName;
        private string id;
        private bool idEdited;

        public static void Open(PartyMemberDefinition character, CharacterMasteryDefinition mastery, CharacterMasteryNodeKind kind, Vector2 treePosition)
        {
            CharacterMasteryNodeCreateWindow window = CreateInstance<CharacterMasteryNodeCreateWindow>();

            window.character = character;
            window.mastery = mastery;
            window.kind = kind;
            window.treePosition = treePosition;

            window.titleContent = new GUIContent("New Mastery Node");

            window.minSize = new Vector2(420f, 220f);

            window.maxSize = new Vector2(520f, 300f);

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Mastery Node", EditorStyles.largeLabel);

            EditorGUILayout.Space(6);

            EditorGUILayout.LabelField("Type", CharacterMasteryWorkbenchNodeFactory.GetMenuLabel(kind));

            EditorGUI.BeginChangeCheck();

            string nextDisplayName = EditorGUILayout.TextField("Display Name", displayName);

            if (EditorGUI.EndChangeCheck())
            {
                displayName = nextDisplayName;

                if (!idEdited)
                {
                    id = CharacterMasteryWorkbenchNodeFactory.GetSuggestedId(character, displayName);
                }
            }

            EditorGUI.BeginChangeCheck();

            string nextId = EditorGUILayout.TextField("Stable ID", id);

            if (EditorGUI.EndChangeCheck())
            {
                id = nextId;
                idEdited = true;
            }

            EditorGUILayout.Space(8);

            string error = CharacterMasteryWorkbenchNodeFactory.ValidateCreation(character, mastery, displayName, id);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                string assetName = CharacterMasteryWorkbenchNodeFactory.GetAssetName(character, displayName);

                EditorGUILayout.HelpBox($"Asset: {assetName}.asset", MessageType.None);
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error)))
            {
                if (GUILayout.Button("Create Mastery Node", GUILayout.Height(32f)))
                {
                    CharacterMasteryNodeDefinition node = CharacterMasteryWorkbenchNodeFactory.Create(character, mastery, kind, treePosition, displayName, id);

                    if (node == null)
                        return;

                    Selection.activeObject = node;
                    EditorGUIUtility.PingObject(node);

                    Close();
                }
            }
        }
    }
}
