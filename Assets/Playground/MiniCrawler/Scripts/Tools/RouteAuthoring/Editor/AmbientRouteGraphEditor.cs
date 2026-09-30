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

            int nodeCount = AmbientRouteAuthoringUtility.GetOwnedNodes(graph).Count;
            int segmentCount = AmbientRouteAuthoringUtility.GetOwnedSegments(graph).Count;
            int activityCount = AmbientActivityAuthoringUtility.GetAreasForGraph(graph).Count;
            int encounterCount = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph).Count;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Region Authoring", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"Points {nodeCount}  |  Connections {segmentCount}  |  Ambient {activityCount}  |  Encounters {encounterCount}",
                EditorStyles.miniLabel
            );

            if (GUILayout.Button("Open Region Authoring"))
                RegionAuthoringWindow.OpenForGraph(graph);
        }
    }
}