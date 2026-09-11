using System.IO;
using MiniCrawler.Encounters;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MiniCrawler.Tools
{
    public static class EncounterWorkshopAssetPersistence
    {
        private const string DefaultAssetFolder =
            "Assets/Playground/MiniCrawler/Assets/Data/Encounters";

        public static bool IsAvailable
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public static bool Save(
            EncounterWorkshopDraft draft,
            EncounterDefinition definition)
        {
#if UNITY_EDITOR
            if (draft == null || definition == null)
                return false;

            draft.WriteToDefinition(definition);
            definition.EnsureId(definition.name);

            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();

            draft.MarkSaved();

            Debug.Log($"Saved Encounter Definition '{definition.name}'.");
            return true;
#else
            Debug.LogWarning(
                "Encounter Definition persistence is only available in the Unity Editor."
            );

            return false;
#endif
        }

        public static EncounterDefinition SaveAs(EncounterWorkshopDraft draft)
        {
#if UNITY_EDITOR
            if (draft == null)
                return null;

            EnsureDefaultFolder();

            string defaultName = MakeDefaultAssetName(draft.DisplayName);

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Encounter Definition",
                defaultName,
                "asset",
                "Choose where to save the Encounter Definition.",
                DefaultAssetFolder
            );

            if (string.IsNullOrWhiteSpace(path))
                return null;

            EncounterDefinition existing =
                AssetDatabase.LoadAssetAtPath<EncounterDefinition>(path);

            if (existing != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Overwrite Encounter Definition?",
                    $"'{existing.name}' already exists.\n\nOverwrite it with the current Workshop Draft?",
                    "Overwrite",
                    "Cancel"
                );

                if (!overwrite)
                    return null;

                if (!Save(draft, existing))
                    return null;

                Selection.activeObject = existing;
                EditorGUIUtility.PingObject(existing);

                return existing;
            }

            Object occupiedAsset = AssetDatabase.LoadMainAssetAtPath(path);

            if (occupiedAsset != null)
            {
                EditorUtility.DisplayDialog(
                    "Cannot Save Encounter Definition",
                    $"Another asset already exists at:\n\n{path}",
                    "OK"
                );

                return null;
            }

            EncounterDefinition definition =
                ScriptableObject.CreateInstance<EncounterDefinition>();

            draft.WriteToDefinition(definition);

            definition.EnsureId(
                Path.GetFileNameWithoutExtension(path)
            );

            AssetDatabase.CreateAsset(definition, path);
            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();

            draft.MarkSaved();

            Selection.activeObject = definition;
            EditorGUIUtility.PingObject(definition);

            Debug.Log($"Created Encounter Definition '{definition.name}' at '{path}'.");

            return definition;
#else
            Debug.LogWarning(
                "Encounter Definition persistence is only available in the Unity Editor."
            );

            return null;
#endif
        }

        public static EncounterDefinition Load()
        {
#if UNITY_EDITOR
            EnsureDefaultFolder();

            string absolutePath = EditorUtility.OpenFilePanel(
                "Load Encounter Definition",
                GetAbsoluteDefaultFolder(),
                "asset"
            );

            if (string.IsNullOrWhiteSpace(absolutePath))
                return null;

            string assetPath = ConvertAbsoluteToAssetPath(absolutePath);

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Encounter Definition",
                    "The selected asset must be inside this Unity project's Assets folder.",
                    "OK"
                );

                return null;
            }

            EncounterDefinition definition =
                AssetDatabase.LoadAssetAtPath<EncounterDefinition>(assetPath);

            if (definition == null)
            {
                EditorUtility.DisplayDialog(
                    "Invalid Encounter Definition",
                    "The selected file is not an EncounterDefinition asset.",
                    "OK"
                );

                return null;
            }

            Selection.activeObject = definition;
            EditorGUIUtility.PingObject(definition);

            return definition;
#else
            Debug.LogWarning(
                "Encounter Definition loading is only available in the Unity Editor."
            );

            return null;
#endif
        }

#if UNITY_EDITOR
        private static void EnsureDefaultFolder()
        {
            string absolutePath = GetAbsoluteDefaultFolder();

            if (Directory.Exists(absolutePath))
                return;

            Directory.CreateDirectory(absolutePath);
            AssetDatabase.Refresh();
        }

        private static string GetAbsoluteDefaultFolder()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);

            return Path.GetFullPath(
                Path.Combine(projectRoot, DefaultAssetFolder)
            );
        }

        private static string ConvertAbsoluteToAssetPath(string absolutePath)
        {
            string normalizedAbsolute =
                Path.GetFullPath(absolutePath).Replace('\\', '/');

            string normalizedAssets =
                Path.GetFullPath(Application.dataPath).Replace('\\', '/');

            if (!normalizedAbsolute.StartsWith(normalizedAssets))
                return null;

            return "Assets" + normalizedAbsolute.Substring(normalizedAssets.Length);
        }

        private static string MakeDefaultAssetName(string displayName)
        {
            string value = string.IsNullOrWhiteSpace(displayName)
                ? "NewEncounter"
                : displayName;

            foreach (char invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid.ToString(), string.Empty);

            value = value.Replace(" ", string.Empty);

            return string.IsNullOrWhiteSpace(value)
                ? "NewEncounterDefinition"
                : $"{value}Definition";
        }
#endif
    }
}