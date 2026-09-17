using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Ecology;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    internal static class AmbientActivityAuthoringUtility
    {
        public static AmbientActivityArea ResolveAreaFromSelection()
        {
            GameObject active = Selection.activeGameObject;

            if (active == null)
                return null;

            AmbientActivityArea area = active.GetComponent<AmbientActivityArea>();
            return area != null ? area : active.GetComponentInParent<AmbientActivityArea>();
        }

        public static List<AmbientActivityArea> GetAreasForGraph(AmbientRouteGraph graph)
        {
            List<AmbientActivityArea> result = new();

            if (graph == null)
                return result;

            foreach (AmbientActivityArea area in graph.GetComponentsInChildren<AmbientActivityArea>(true))
            {
                if (area != null && area.ReferencesGraph(graph))
                    result.Add(area);
            }

            return result;
        }

        public static AmbientActivityArea CreateAreaFromSelection(AmbientRouteGraph graph, string requestedName)
        {
            if (graph == null)
                return null;

            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);
            List<AmbientRouteGroup> groups = AmbientRouteAuthoringUtility.GetSelectedGroups(graph);

            if (nodes.Count == 0 && segments.Count == 0 && groups.Count == 0)
                return null;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create Ambient Activity Area");

            Transform root = AmbientRouteAuthoringUtility.EnsureAmbientActivityRoot(graph);

            string baseName = string.IsNullOrWhiteSpace(requestedName) ? "Ambient Activity" : requestedName.Trim();
            GameObject areaObject = new(AmbientRouteAuthoringUtility.GetUniqueChildName(root, baseName));

            Undo.RegisterCreatedObjectUndo(areaObject, "Create Ambient Activity Area");
            Undo.SetTransformParent(areaObject.transform, root, "Parent Ambient Activity Area");

            areaObject.transform.localPosition = Vector3.zero;
            areaObject.transform.localRotation = Quaternion.identity;
            areaObject.transform.localScale = Vector3.one;

            AmbientActivityArea area = Undo.AddComponent<AmbientActivityArea>(areaObject);

            SerializedObject serialized = new(area);
            serialized.Update();

            SetReferenceArray(serialized.FindProperty("nodes"), nodes);
            SetReferenceArray(serialized.FindProperty("segments"), segments);
            SetReferenceArray(serialized.FindProperty("groups"), groups);

            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(area);
            MarkSceneDirty(area);

            Selection.activeGameObject = areaObject;
            SceneView.RepaintAll();

            Undo.CollapseUndoOperations(undoGroup);
            return area;
        }

        public static bool SetPopulation(AmbientActivityArea area, ActorDefinition actorDefinition, int count)
        {
            if (area == null || actorDefinition == null)
                return false;

            count = Mathf.Max(1, count);

            Undo.RecordObject(area, "Set Ambient Population");

            SerializedObject serialized = new(area);
            serialized.Update();

            SerializedProperty populations = serialized.FindProperty("populations");
            SerializedProperty targetEntry = null;

            for (int i = 0; i < populations.arraySize; i++)
            {
                SerializedProperty entry = populations.GetArrayElementAtIndex(i);

                if (entry.FindPropertyRelative("actorDefinition").objectReferenceValue == actorDefinition)
                {
                    targetEntry = entry;
                    break;
                }
            }

            if (targetEntry == null)
            {
                populations.arraySize++;
                targetEntry = populations.GetArrayElementAtIndex(populations.arraySize - 1);
                targetEntry.FindPropertyRelative("actorDefinition").objectReferenceValue = actorDefinition;
            }

            targetEntry.FindPropertyRelative("count").intValue = count;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(area);
            MarkSceneDirty(area);
            return true;
        }

        public static bool RemovePopulation(AmbientActivityArea area, ActorDefinition actorDefinition)
        {
            if (area == null || actorDefinition == null)
                return false;

            SerializedObject serialized = new(area);
            serialized.Update();

            SerializedProperty populations = serialized.FindProperty("populations");
            int removeIndex = -1;

            for (int i = 0; i < populations.arraySize; i++)
            {
                SerializedProperty entry = populations.GetArrayElementAtIndex(i);

                if (entry.FindPropertyRelative("actorDefinition").objectReferenceValue == actorDefinition)
                {
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex < 0)
                return false;

            Undo.RecordObject(area, "Remove Ambient Population");
            populations.DeleteArrayElementAtIndex(removeIndex);
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(area);
            MarkSceneDirty(area);
            return true;
        }

        public static bool SetSettings(AmbientActivityArea area, float spawnScatterRadius, float engagementRadius)
        {
            if (area == null)
                return false;

            return SetSettings(
                area,
                spawnScatterRadius,
                engagementRadius,
                area.WanderRadius,
                area.WanderPauseMinSeconds,
                area.WanderPauseMaxSeconds
            );
        }

        public static bool SetSettings(
            AmbientActivityArea area,
            float spawnScatterRadius,
            float engagementRadius,
            float wanderRadius,
            float wanderPauseMinSeconds,
            float wanderPauseMaxSeconds)
        {
            if (area == null)
                return false;

            spawnScatterRadius = Mathf.Max(0f, spawnScatterRadius);
            engagementRadius = Mathf.Max(0.1f, engagementRadius);
            wanderRadius = Mathf.Max(0f, wanderRadius);
            wanderPauseMinSeconds = Mathf.Max(0f, wanderPauseMinSeconds);
            wanderPauseMaxSeconds = Mathf.Max(wanderPauseMinSeconds, wanderPauseMaxSeconds);

            Undo.RecordObject(area, "Set Ambient Activity Settings");

            SerializedObject serialized = new(area);
            serialized.Update();

            serialized.FindProperty("spawnScatterRadius").floatValue = spawnScatterRadius;
            serialized.FindProperty("engagementRadius").floatValue = engagementRadius;
            serialized.FindProperty("wanderRadius").floatValue = wanderRadius;
            serialized.FindProperty("wanderPauseMinSeconds").floatValue = wanderPauseMinSeconds;
            serialized.FindProperty("wanderPauseMaxSeconds").floatValue = wanderPauseMaxSeconds;

            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(area);
            MarkSceneDirty(area);
            return true;
        }

        public static void AppendValidationIssues(AmbientRouteGraph graph, List<string> issues)
        {
            foreach (AmbientActivityArea area in GetAreasForGraph(graph))
                area.CollectValidationIssues(graph, issues);
        }

        private static void SetReferenceArray<T>(SerializedProperty property, IReadOnlyList<T> items) where T : Object
        {
            property.arraySize = items.Count;

            for (int i = 0; i < items.Count; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }

        private static void MarkSceneDirty(AmbientActivityArea area)
        {
            if (area != null && area.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(area.gameObject.scene);
        }
    }
}