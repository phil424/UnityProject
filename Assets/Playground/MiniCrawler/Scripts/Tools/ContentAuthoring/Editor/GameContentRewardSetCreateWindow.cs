using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentRewardSetCreateWindow : EditorWindow
    {
        private RunRewardCatalogDefinition catalog;
        private string setName;

        public static void Open(RunRewardCatalogDefinition catalog)
        {
            GameContentRewardSetCreateWindow window = CreateInstance<GameContentRewardSetCreateWindow>();

            window.catalog = catalog;
            window.titleContent = new GUIContent("New Reward Set");

            window.minSize = new Vector2(400f, 180f);
            window.maxSize = new Vector2(520f, 240f);

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Run Reward Set", EditorStyles.largeLabel);

            EditorGUILayout.Space(6);

            setName = EditorGUILayout.TextField("Set Name", setName);

            string assetName = GameContentAssetUtility.GetRewardSetAssetName(setName);

            string error = Validate(assetName);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox($"Asset: {assetName}.asset", MessageType.None);
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error)))
            {
                if (GUILayout.Button("Create Reward Set", GUILayout.Height(30f)))
                {
                    Create(assetName);
                }
            }
        }

        private string Validate(string assetName)
        {
            if (catalog == null)
                return "Run Reward Catalogue is required.";

            if (string.IsNullOrWhiteSpace(setName))
                return "Set Name is required.";

            if (string.IsNullOrWhiteSpace(assetName))
                return "Could not produce a valid asset filename.";

            string path = $"{GameContentAssetUtility.RunRewardSetFolder}/{assetName}.asset";

            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
                return $"An asset already exists at '{path}'.";

            return null;
        }

        private void Create(string assetName)
        {
            GameContentAssetUtility.EnsureFolder(GameContentAssetUtility.RunRewardSetFolder);

            string path = $"{GameContentAssetUtility.RunRewardSetFolder}/{assetName}.asset";

            RunRewardSetDefinition rewardSet = CreateInstance<RunRewardSetDefinition>();

            rewardSet.name = assetName;

            AssetDatabase.CreateAsset(rewardSet, path);

            Undo.RegisterCreatedObjectUndo(rewardSet, "Create Run Reward Set");

            GameContentRewardUtility.AddSet(catalog, rewardSet);

            AssetDatabase.SaveAssets();

            Selection.activeObject = rewardSet;
            EditorGUIUtility.PingObject(rewardSet);

            Close();
        }
    }
}
