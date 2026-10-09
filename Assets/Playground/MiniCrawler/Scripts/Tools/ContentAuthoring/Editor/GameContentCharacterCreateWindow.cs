using MiniCrawler.Core;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentCharacterCreateWindow : EditorWindow
    {
        private string displayName;
        private string id;
        private bool idEdited;
        private bool unlockedByDefault;
        private bool visibleWhenLocked = true;

        public static void Open()
        {
            GameContentCharacterCreateWindow window = CreateInstance<GameContentCharacterCreateWindow>();

            window.titleContent = new GUIContent("New Character");

            window.minSize = new Vector2(420f, 250f);

            window.maxSize = new Vector2(520f, 340f);

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Playable Character", EditorStyles.largeLabel);

            EditorGUILayout.Space(6);

            EditorGUILayout.HelpBox("Identity is captured before assets are created, so the new files begin with useful final names rather than placeholder 'New...' names.", MessageType.Info);

            EditorGUILayout.Space(6);

            EditorGUI.BeginChangeCheck();

            string nextDisplayName = EditorGUILayout.TextField("Display Name", displayName);

            if (EditorGUI.EndChangeCheck())
            {
                displayName = nextDisplayName;

                if (!idEdited)
                {
                    id = GameContentAssetUtility.ToIdToken(displayName);
                }
            }

            EditorGUI.BeginChangeCheck();

            string nextId = EditorGUILayout.TextField("Stable ID", id);

            if (EditorGUI.EndChangeCheck())
            {
                id = nextId;
                idEdited = true;
            }

            unlockedByDefault = EditorGUILayout.Toggle("Unlocked By Default", unlockedByDefault);

            visibleWhenLocked = EditorGUILayout.Toggle("Visible When Locked", visibleWhenLocked);

            EditorGUILayout.Space(8);

            GameContentCharacterCreationRequest request = new(displayName, id, unlockedByDefault, visibleWhenLocked);

            string error = GameContentCharacterFactory.Validate(request);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                string token = GameContentAssetUtility.ToFileToken(displayName);

                EditorGUILayout.HelpBox(
                    $"Will create:\n"
                        + $"{GameContentAssetUtility.GetCharacterAssetName(displayName)}.asset\n"
                        + $"{GameContentAssetUtility.GetActorAssetName(displayName)}.asset\n"
                        + $"Mastery/{token}/{GameContentAssetUtility.GetMasteryAssetName(displayName)}.asset",
                    MessageType.None
                );
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error)))
            {
                if (GUILayout.Button("Create Character", GUILayout.Height(32f)))
                {
                    PartyMemberDefinition created = GameContentCharacterFactory.Create(request, out string createError);

                    if (created == null)
                    {
                        EditorUtility.DisplayDialog("Character Creation Failed", createError ?? "Unknown creation error.", "OK");

                        return;
                    }

                    Close();
                }
            }
        }
    }
}
