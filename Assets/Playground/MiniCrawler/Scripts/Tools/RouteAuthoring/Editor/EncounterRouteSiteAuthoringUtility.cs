using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    internal static class EncounterRouteSiteAuthoringUtility
    {
        public static LevelEncounter ResolveEncounterFromSelection()
        {
            GameObject active = Selection.activeGameObject;

            if (active == null)
                return null;

            LevelEncounter encounter =
                active.GetComponent<LevelEncounter>();

            if (encounter != null)
                return encounter;

            return active.GetComponentInParent<LevelEncounter>();
        }

        public static EncounterRouteSite BindFromSelection(
            AmbientRouteGraph graph,
            LevelEncounter encounter)
        {
            if (graph == null || encounter == null)
                return null;

            List<AmbientRouteNode> nodes =
                AmbientRouteAuthoringUtility.GetSelectedNodes(graph);

            List<AmbientRouteSegment> segments =
                AmbientRouteAuthoringUtility.GetSelectedSegments(graph);

            if (nodes.Count == 0 && segments.Count == 0)
                return null;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Bind Encounter Route Site");

            EncounterRouteSite site =
                encounter.GetComponent<EncounterRouteSite>();

            if (site == null)
            {
                site =
                    Undo.AddComponent<EncounterRouteSite>(
                        encounter.gameObject
                    );
            }

            Undo.RecordObject(
                site,
                "Configure Encounter Route Site"
            );

            SerializedObject serializedSite =
                new(site);

            serializedSite.Update();

            serializedSite
                .FindProperty("encounter")
                .objectReferenceValue = encounter;

            SetReferenceArray(
                serializedSite.FindProperty("nodes"),
                nodes
            );

            SetReferenceArray(
                serializedSite.FindProperty("segments"),
                segments
            );

            serializedSite.ApplyModifiedProperties();

            EditorUtility.SetDirty(site);

            if (encounter.gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(
                    encounter.gameObject.scene
                );
            }

            Selection.activeGameObject =
                encounter.gameObject;

            SceneView.RepaintAll();

            Undo.CollapseUndoOperations(undoGroup);

            return site;
        }
        
        public static bool SetStartEncounterOnTraversal(EncounterRouteSite site, bool value)
        {
            if (site == null || site.StartEncounterOnTraversal == value)
                return false;

            Undo.RecordObject(site, "Set Route Site Traversal Start");

            SerializedObject serializedSite = new(site);
            serializedSite.Update();
            serializedSite.FindProperty("startEncounterOnTraversal").boolValue = value;
            serializedSite.ApplyModifiedProperties();

            EditorUtility.SetDirty(site);

            if (site.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(site.gameObject.scene);

            return true;
        }

        public static List<EncounterRouteSite> GetSitesForGraph(
            AmbientRouteGraph graph)
        {
            List<EncounterRouteSite> result = new();

            if (graph == null)
                return result;

            foreach (EncounterRouteSite site in
                     Object.FindObjectsByType<EncounterRouteSite>(
                         FindObjectsSortMode.None))
            {
                if (site != null &&
                    site.ReferencesGraph(graph))
                {
                    result.Add(site);
                }
            }

            return result;
        }

        public static void AppendValidationIssues(
            AmbientRouteGraph graph,
            List<string> issues)
        {
            foreach (EncounterRouteSite site in
                     GetSitesForGraph(graph))
            {
                site.CollectValidationIssues(
                    graph,
                    issues
                );
            }
        }

        private static void SetReferenceArray<T>(
            SerializedProperty property,
            IReadOnlyList<T> items)
            where T : UnityEngine.Object
        {
            property.arraySize = items.Count;

            for (int i = 0; i < items.Count; i++)
            {
                property
                    .GetArrayElementAtIndex(i)
                    .objectReferenceValue = items[i];
            }
        }
    }
}