using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentRewardCreateWindow : EditorWindow
    {
        private RunRewardCatalogDefinition catalog;
        private RunRewardSetDefinition rewardSet;

        private RunRewardAuthoringKind kind;
        private string displayName;
        private string id;
        private bool idEdited;

        public static void Open(RunRewardCatalogDefinition catalog, RunRewardSetDefinition rewardSet)
        {
            GameContentRewardCreateWindow window = CreateInstance<GameContentRewardCreateWindow>();

            window.catalog = catalog;
            window.rewardSet = rewardSet;

            window.titleContent = new GUIContent("New Run Reward");

            window.minSize = new Vector2(440f, 280f);
            window.maxSize = new Vector2(560f, 360f);

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Run Reward", EditorStyles.largeLabel);

            EditorGUILayout.Space(6);

            EditorGUILayout.ObjectField("Reward Set", rewardSet, typeof(RunRewardSetDefinition), false);

            kind = (RunRewardAuthoringKind)EditorGUILayout.EnumPopup("Reward Type", kind);

            EditorGUI.BeginChangeCheck();

            string nextName = EditorGUILayout.TextField("Display Name", displayName);

            if (EditorGUI.EndChangeCheck())
            {
                displayName = nextName;

                if (!idEdited)
                {
                    id = GameContentRewardFactory.GetSuggestedId(displayName);
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

            string error = GameContentRewardFactory.ValidateCreation(catalog, displayName, id, kind);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                string assetName = GameContentAssetUtility.GetRunRewardAssetName(displayName);

                string folder = GameContentRewardFactory.GetAssetFolder(kind);

                EditorGUILayout.HelpBox($"{GameContentRewardFactory.GetMenuLabel(kind)}\n" + $"{folder}/{assetName}.asset", MessageType.None);
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error) || rewardSet == null))
            {
                if (GUILayout.Button("Create Reward", GUILayout.Height(32f)))
                {
                    RunRewardDefinition reward = GameContentRewardFactory.Create(catalog, rewardSet, kind, displayName, id, out string createError);

                    if (reward == null)
                    {
                        EditorUtility.DisplayDialog("Reward Creation Failed", createError ?? "Unknown error.", "OK");

                        return;
                    }

                    Close();
                }
            }
        }
    }
}
