using System.Collections.Generic;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    internal static class RouteTagAuthoringUtility
    {
        private const string DefaultTagFolder = "Assets/Playground/MiniCrawler/Assets/Data/RouteTags";
        private const string DataFolder = "Assets/Playground/MiniCrawler/Assets/Data";

        public static RouteTagDefinition CreateTagAsset()
        {
            EnsureTagFolder();

            string path = EditorUtility.SaveFilePanelInProject(
                "Create Route Tag",
                "New Route Tag",
                "asset",
                "Create a reusable Route Geography Tag.",
                DefaultTagFolder
            );

            if (string.IsNullOrWhiteSpace(path))
                return null;

            RouteTagDefinition tag = ScriptableObject.CreateInstance<RouteTagDefinition>();

            AssetDatabase.CreateAsset(tag, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = tag;
            EditorGUIUtility.PingObject(tag);

            return tag;
        }

        public static int AddTagToSelection(AmbientRouteGraph graph, RouteTagDefinition tag)
        {
            if (graph == null || tag == null)
                return 0;

            int changed = 0;

            foreach (AmbientRouteNode node in AmbientRouteAuthoringUtility.GetSelectedNodes(graph))
            {
                if (AddTag(node, tag))
                    changed++;
            }

            foreach (AmbientRouteSegment segment in AmbientRouteAuthoringUtility.GetSelectedSegments(graph))
            {
                if (AddTag(segment, tag))
                    changed++;
            }

            foreach (AmbientRouteGroup group in AmbientRouteAuthoringUtility.GetSelectedGroups(graph))
            {
                if (AddTag(group, tag))
                    changed++;
            }

            return changed;
        }

        public static int RemoveTagFromSelection(AmbientRouteGraph graph, RouteTagDefinition tag)
        {
            if (graph == null || tag == null)
                return 0;

            int changed = 0;

            foreach (AmbientRouteNode node in AmbientRouteAuthoringUtility.GetSelectedNodes(graph))
            {
                if (RemoveTag(node, tag))
                    changed++;
            }

            foreach (AmbientRouteSegment segment in AmbientRouteAuthoringUtility.GetSelectedSegments(graph))
            {
                if (RemoveTag(segment, tag))
                    changed++;
            }

            foreach (AmbientRouteGroup group in AmbientRouteAuthoringUtility.GetSelectedGroups(graph))
            {
                if (RemoveTag(group, tag))
                    changed++;
            }

            return changed;
        }

        private static bool AddTag(Object target, RouteTagDefinition tag)
        {
            SerializedObject serialized = new(target);
            serialized.Update();

            SerializedProperty tags = serialized.FindProperty("tags");

            if (tags == null)
                return false;

            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).objectReferenceValue == tag)
                    return false;
            }

            Undo.RecordObject(target, "Add Route Tag");

            tags.arraySize++;
            tags.GetArrayElementAtIndex(tags.arraySize - 1).objectReferenceValue = tag;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);
            MarkSceneDirty(target);

            return true;
        }

        private static bool RemoveTag(Object target, RouteTagDefinition tag)
        {
            SerializedObject serialized = new(target);
            serialized.Update();

            SerializedProperty tags = serialized.FindProperty("tags");

            if (tags == null)
                return false;

            int index = -1;

            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).objectReferenceValue == tag)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
                return false;

            Undo.RecordObject(target, "Remove Route Tag");

            for (int i = index; i < tags.arraySize - 1; i++)
                tags.GetArrayElementAtIndex(i).objectReferenceValue = tags.GetArrayElementAtIndex(i + 1).objectReferenceValue;

            tags.arraySize--;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);
            MarkSceneDirty(target);

            return true;
        }

        private static void EnsureTagFolder()
        {
            if (!AssetDatabase.IsValidFolder(DefaultTagFolder))
                AssetDatabase.CreateFolder(DataFolder, "RouteTags");
        }

        private static void MarkSceneDirty(Object target)
        {
            if (target is Component component && component.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
        }
    }
}