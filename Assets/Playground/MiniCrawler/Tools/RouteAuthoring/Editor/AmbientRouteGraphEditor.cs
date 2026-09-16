using System.Collections.Generic;
using MiniCrawler.Ecology;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    [CustomEditor(typeof(AmbientRouteGraph))]
    public sealed class AmbientRouteGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AmbientRouteGraph graph = (AmbientRouteGraph)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Route Geography", EditorStyles.boldLabel);

            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetOwnedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetOwnedSegments(graph);
            List<AmbientRouteGroup> groups = AmbientRouteAuthoringUtility.GetOwnedGroups(graph);

            int siteCount = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph).Count;
            int activityAreaCount = AmbientActivityAuthoringUtility.GetAreasForGraph(graph).Count;

            EditorGUILayout.LabelField("Nodes", nodes.Count.ToString());
            EditorGUILayout.LabelField("Segments", segments.Count.ToString());
            EditorGUILayout.LabelField("Groups", groups.Count.ToString());
            EditorGUILayout.LabelField("Encounter Sites", siteCount.ToString());
            EditorGUILayout.LabelField("Ambient Areas", activityAreaCount.ToString());

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Use the Route Geography Authoring window for assisted creation, wiring and Encounter Site binding. " +
                "Generated components remain normal editable scene objects.",
                MessageType.Info
            );

            if (GUILayout.Button("Open Route Geography Authoring"))
                RouteAuthoringWindow.OpenForGraph(graph);

            if (GUILayout.Button("Ensure Authoring Structure"))
                AmbientRouteAuthoringUtility.EnsureAuthoringStructure(graph);

            if (GUILayout.Button("Validate Route Geography"))
                AmbientRouteAuthoringUtility.LogValidation(graph);
        }
    }
}