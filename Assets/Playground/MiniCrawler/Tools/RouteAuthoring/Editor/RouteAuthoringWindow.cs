using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    public sealed class RouteAuthoringWindow : EditorWindow
    {
        [SerializeField] private AmbientRouteGraph graph;
        [SerializeField] private LevelEncounter encounterSiteTarget;
        [SerializeField] private RouteTagDefinition routeTag;

        private string newNodeName = "Route Node";
        private string newGroupName = "Route Group";

        private readonly List<string> validationIssues = new();
        private readonly List<RouteTagDefinition> siteEffectiveTags = new();
        private bool validationHasRun;
        private Vector2 scrollPosition;

        [MenuItem("Tools/MiniCrawler/Route Geography Authoring")]
        public static void Open()
        {
            RouteAuthoringWindow window =
                GetWindow<RouteAuthoringWindow>(
                    "Route Geography"
                );

            window.TryAdoptGraphFromSelection();
            window.TryAdoptEncounterFromSelection();

            window.Show();
        }

        public static void OpenForGraph(
            AmbientRouteGraph graph)
        {
            RouteAuthoringWindow window =
                GetWindow<RouteAuthoringWindow>(
                    "Route Geography"
                );

            window.graph = graph;
            window.validationHasRun = false;

            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            TryAdoptGraphFromSelection();
            TryAdoptEncounterFromSelection();
        }

        private void OnSelectionChange()
        {
            TryAdoptGraphFromSelection();
            TryAdoptEncounterFromSelection();

            Repaint();
        }

        private void OnGUI()
        {
            scrollPosition =
                EditorGUILayout.BeginScrollView(
                    scrollPosition
                );

            EditorGUILayout.LabelField(
                "Route Geography Authoring",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginChangeCheck();

            graph =
                (AmbientRouteGraph)
                EditorGUILayout.ObjectField(
                    "Route Graph",
                    graph,
                    typeof(AmbientRouteGraph),
                    true
                );

            if (EditorGUI.EndChangeCheck())
                validationHasRun = false;

            if (graph == null)
            {
                EditorGUILayout.HelpBox(
                    "Select an AmbientRouteGraph or any Route Node / Segment / Group beneath one.",
                    MessageType.Info
                );

                EditorGUILayout.EndScrollView();
                return;
            }

            DrawSummary();
            DrawStructureTools();
            DrawNodeTools();
            DrawConnectionTools();
            DrawGroupTools();
            DrawTagTools();
            DrawEncounterSiteTools();
            DrawValidation();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSummary()
        {
            List<AmbientRouteNode> nodes =
                AmbientRouteAuthoringUtility
                    .GetOwnedNodes(graph);

            List<AmbientRouteSegment> segments =
                AmbientRouteAuthoringUtility
                    .GetOwnedSegments(graph);

            List<AmbientRouteGroup> groups =
                AmbientRouteAuthoringUtility
                    .GetOwnedGroups(graph);

            List<EncounterRouteSite> sites =
                EncounterRouteSiteAuthoringUtility
                    .GetSitesForGraph(graph);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Current Geography",
                EditorStyles.boldLabel
            );

            EditorGUILayout.LabelField(
                "Nodes",
                nodes.Count.ToString()
            );

            EditorGUILayout.LabelField(
                "Segments",
                segments.Count.ToString()
            );

            EditorGUILayout.LabelField(
                "Groups",
                groups.Count.ToString()
            );

            EditorGUILayout.LabelField(
                "Encounter Sites",
                sites.Count.ToString()
            );
        }

        private void DrawStructureTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Structure",
                EditorStyles.boldLabel
            );

            EditorGUILayout.HelpBox(
                "Creates any missing Nodes, Segments, Groups and Landmarks roots. Existing roots are preserved.",
                MessageType.None
            );

            if (GUILayout.Button(
                    "Ensure Authoring Structure"))
            {
                AmbientRouteAuthoringUtility
                    .EnsureAuthoringStructure(graph);

                RefreshValidation();
            }
        }

        private void DrawNodeTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Nodes",
                EditorStyles.boldLabel
            );

            newNodeName =
                EditorGUILayout.TextField(
                    "New Node Name",
                    newNodeName
                );

            EditorGUILayout.HelpBox(
                "Creates a Route Node beneath the Nodes root at the active Scene view pivot. " +
                "The new Node remains a normal GameObject that can be moved and renamed manually.",
                MessageType.None
            );

            if (GUILayout.Button(
                    "Create Node At Scene View Pivot"))
            {
                AmbientRouteAuthoringUtility
                    .CreateNode(
                        graph,
                        newNodeName
                    );

                RefreshValidation();
            }
        }

        private void DrawConnectionTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Connections",
                EditorStyles.boldLabel
            );

            List<AmbientRouteNode> selectedNodes =
                AmbientRouteAuthoringUtility
                    .GetSelectedNodes(graph);

            EditorGUILayout.LabelField(
                "Selected Route Nodes",
                selectedNodes.Count.ToString()
            );

            EditorGUILayout.HelpBox(
                "Pair requires exactly two Nodes.\n\n" +
                "Hierarchy Path connects selected Nodes in their hierarchy order beneath Nodes.\n\n" +
                "Spatial Loop sorts selected Nodes around their X/Z centroid and closes the final connection.",
                MessageType.Info
            );

            using (new EditorGUI.DisabledScope(
                       selectedNodes.Count != 2))
            {
                if (GUILayout.Button(
                        "Connect Selected Pair"))
                {
                    AmbientRouteAuthoringUtility
                        .ConnectSelectedPair(graph);

                    RefreshValidation();
                }
            }

            using (new EditorGUI.DisabledScope(
                       selectedNodes.Count < 2))
            {
                if (GUILayout.Button(
                        "Connect Selection As Hierarchy Path"))
                {
                    AmbientRouteAuthoringUtility
                        .ConnectSelectionAsHierarchyPath(
                            graph
                        );

                    RefreshValidation();
                }
            }

            using (new EditorGUI.DisabledScope(
                       selectedNodes.Count < 3))
            {
                if (GUILayout.Button(
                        "Connect Selection As Spatial Loop"))
                {
                    AmbientRouteAuthoringUtility
                        .ConnectSelectionAsSpatialLoop(
                            graph
                        );

                    RefreshValidation();
                }
            }
        }

        private void DrawGroupTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Groups",
                EditorStyles.boldLabel
            );

            List<AmbientRouteNode> selectedNodes =
                AmbientRouteAuthoringUtility
                    .GetSelectedNodes(graph);

            List<AmbientRouteSegment> selectedSegments =
                AmbientRouteAuthoringUtility
                    .GetSelectedSegments(graph);

            EditorGUILayout.LabelField(
                "Selected Geography",
                $"{selectedNodes.Count} Nodes / {selectedSegments.Count} Segments"
            );

            newGroupName =
                EditorGUILayout.TextField(
                    "New Group Name",
                    newGroupName
                );

            EditorGUILayout.HelpBox(
                "If Segments are selected, their endpoint Nodes are included automatically.\n\n" +
                "If only Nodes are selected, existing Segments whose two endpoints are both selected are included automatically.",
                MessageType.None
            );

            using (new EditorGUI.DisabledScope(
                       selectedNodes.Count == 0 &&
                       selectedSegments.Count == 0))
            {
                if (GUILayout.Button(
                        "Create Group From Selection"))
                {
                    AmbientRouteAuthoringUtility
                        .CreateGroupFromSelection(
                            graph,
                            newGroupName
                        );

                    RefreshValidation();
                }
            }
        }
        
        private void DrawTagTools()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Semantic Tags", EditorStyles.boldLabel);

            routeTag = (RouteTagDefinition)EditorGUILayout.ObjectField("Route Tag", routeTag, typeof(RouteTagDefinition), false);

            if (GUILayout.Button("Create New Route Tag Asset..."))
            {
                RouteTagDefinition createdTag = RouteTagAuthoringUtility.CreateTagAsset();

                if (createdTag != null)
                    routeTag = createdTag;
            }

            List<AmbientRouteNode> selectedNodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> selectedSegments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);
            List<AmbientRouteGroup> selectedGroups = AmbientRouteAuthoringUtility.GetSelectedGroups(graph);

            int selectedCount = selectedNodes.Count + selectedSegments.Count + selectedGroups.Count;

            EditorGUILayout.LabelField(
                "Selected Geography",
                $"{selectedNodes.Count} Nodes / {selectedSegments.Count} Segments / {selectedGroups.Count} Groups"
            );

            EditorGUILayout.HelpBox(
                "Tags describe semantic geography such as Road, Village, Well or Open. " +
                "Group Tags are inherited by Nodes/Segments contained by that Group.",
                MessageType.None
            );

            using (new EditorGUI.DisabledScope(routeTag == null || selectedCount == 0))
            {
                if (GUILayout.Button("Add Tag To Selection"))
                {
                    RouteTagAuthoringUtility.AddTagToSelection(graph, routeTag);
                    RefreshValidation();
                }

                if (GUILayout.Button("Remove Tag From Selection"))
                {
                    RouteTagAuthoringUtility.RemoveTagFromSelection(graph, routeTag);
                    RefreshValidation();
                }
            }
        }

        private void DrawEncounterSiteTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Encounter Sites", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            encounterSiteTarget = (LevelEncounter)EditorGUILayout.ObjectField("Target Encounter", encounterSiteTarget, typeof(LevelEncounter), true);

            if (EditorGUI.EndChangeCheck())
                validationHasRun = false;

            EditorGUILayout.HelpBox(
                "Select an Encounter once to pin it as the Target Encounter. " +
                "Then select Route Nodes and/or Segments and bind them as its spatial footprint.",
                MessageType.Info
            );

            List<AmbientRouteNode> selectedNodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> selectedSegments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);

            EditorGUILayout.LabelField(
                "Selected Route Footprint",
                $"{selectedNodes.Count} Nodes / {selectedSegments.Count} Segments"
            );

            if (encounterSiteTarget != null)
            {
                EncounterRouteSite existingSite = encounterSiteTarget.GetComponent<EncounterRouteSite>();

                if (existingSite != null)
                {
                    EditorGUILayout.LabelField(
                        "Current Site",
                        $"{existingSite.Nodes.Count} Nodes / {existingSite.Segments.Count} Segments"
                    );
                    
                    existingSite.CollectEffectiveTags(siteEffectiveTags);

                    EditorGUILayout.LabelField(
                        "Effective Tags",
                        FormatTags(siteEffectiveTags)
                    );

                    bool startOnTraversal = EditorGUILayout.Toggle(
                        "Start On Route Traversal",
                        existingSite.StartEncounterOnTraversal
                    );
                    
                    EditorGUILayout.HelpBox(
                        "Enabled Sites may start when crossed during explicit Encounter travel or ambient exploration. " +
                        "Explicit travel continues toward its selected destination; ambient exploration pauses to resolve the crossed Encounter.",
                        MessageType.None
                    );

                    if (startOnTraversal != existingSite.StartEncounterOnTraversal)
                    {
                        EncounterRouteSiteAuthoringUtility.SetStartEncounterOnTraversal(existingSite, startOnTraversal);
                        RefreshValidation();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Current Site", "Not Bound");
                }
            }

            using (new EditorGUI.DisabledScope(
                       encounterSiteTarget == null ||
                       (selectedNodes.Count == 0 &&
                        selectedSegments.Count == 0)))
            {
                if (GUILayout.Button(
                        "Bind / Replace Site Footprint From Selection"))
                {
                    EncounterRouteSiteAuthoringUtility
                        .BindFromSelection(
                            graph,
                            encounterSiteTarget
                        );

                    RefreshValidation();
                }
            }
        }

        private void DrawValidation()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Validation",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                    "Refresh Validation"))
            {
                RefreshValidation();
            }

            if (!validationHasRun)
            {
                EditorGUILayout.HelpBox(
                    "Validation has not been refreshed in this tool session.",
                    MessageType.None
                );

                return;
            }

            if (validationIssues.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "Route geography and Encounter Sites are valid.",
                    MessageType.Info
                );

                return;
            }

            foreach (string issue in validationIssues)
            {
                EditorGUILayout.HelpBox(
                    issue,
                    MessageType.Warning
                );
            }

            if (GUILayout.Button(
                    "Log Route Validation To Console"))
            {
                AmbientRouteAuthoringUtility
                    .LogValidation(graph);
            }
        }

        private void RefreshValidation()
        {
            validationIssues.Clear();

            if (graph != null)
            {
                graph.CollectValidationIssues(
                    validationIssues
                );

                EncounterRouteSiteAuthoringUtility
                    .AppendValidationIssues(
                        graph,
                        validationIssues
                    );
            }

            validationHasRun = true;
            Repaint();
        }

        private void TryAdoptGraphFromSelection()
        {
            AmbientRouteGraph selectedGraph =
                AmbientRouteAuthoringUtility
                    .ResolveGraphFromSelection();

            if (selectedGraph == null ||
                selectedGraph == graph)
            {
                return;
            }

            graph = selectedGraph;
            validationHasRun = false;
        }

        private void TryAdoptEncounterFromSelection()
        {
            LevelEncounter selectedEncounter =
                EncounterRouteSiteAuthoringUtility
                    .ResolveEncounterFromSelection();

            if (selectedEncounter == null ||
                selectedEncounter == encounterSiteTarget)
            {
                return;
            }

            encounterSiteTarget = selectedEncounter;
            validationHasRun = false;
        }
        
        private static string FormatTags(IReadOnlyList<RouteTagDefinition> tags)
        {
            if (tags == null || tags.Count == 0)
                return "(none)";

            List<string> names = new();

            foreach (RouteTagDefinition tag in tags)
            {
                if (tag != null)
                    names.Add(tag.DisplayName);
            }

            return names.Count > 0 ? string.Join(", ", names) : "(none)";
        }
    }
}