using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Ecology;
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
        [SerializeField] private EncounterDefinition compatibilityDefinition;
        [SerializeField] private RouteTagDefinition compatibilityTag;
        [SerializeField] private AmbientActivityArea ambientActivityTarget;
        [SerializeField] private ActorDefinition ambientActorDefinition;

        private string newNodeName = "Route Node";
        private string newGroupName = "Route Group";
        private string newAmbientActivityName = "Ambient Activity";

        private int ambientActorCount = 1;
        private float ambientSpawnScatterRadius = 1.5f;
        private float ambientEngagementRadius = 3.5f;

        private readonly List<string> validationIssues = new();
        private readonly List<string> compatibilityValidationIssues = new();
        private readonly List<RouteTagDefinition> siteEffectiveTags = new();
        private readonly List<EncounterRouteCompatibilityResult> compatibleSiteResults = new();
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
            window.TryAdoptCompatibilityDefinitionFromSelection();
            window.TryAdoptAmbientActivityFromSelection();

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
            TryAdoptCompatibilityDefinitionFromSelection();
            TryAdoptAmbientActivityFromSelection();
        }

        private void OnSelectionChange()
        {
            TryAdoptGraphFromSelection();
            TryAdoptEncounterFromSelection();
            TryAdoptCompatibilityDefinitionFromSelection();
            TryAdoptAmbientActivityFromSelection();

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
            DrawAmbientActivityTools();
            DrawEncounterSiteTools();
            DrawCompatibilityTools();
            DrawValidation();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSummary()
        {
            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetOwnedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetOwnedSegments(graph);
            List<AmbientRouteGroup> groups = AmbientRouteAuthoringUtility.GetOwnedGroups(graph);
            List<EncounterRouteSite> sites = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph);
            List<AmbientActivityArea> activityAreas = AmbientActivityAuthoringUtility.GetAreasForGraph(graph);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Current Geography", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Nodes", nodes.Count.ToString());
            EditorGUILayout.LabelField("Segments", segments.Count.ToString());
            EditorGUILayout.LabelField("Groups", groups.Count.ToString());
            EditorGUILayout.LabelField("Encounter Sites", sites.Count.ToString());
            EditorGUILayout.LabelField("Ambient Areas", activityAreas.Count.ToString());
        }

        private void DrawStructureTools()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Structure",
                EditorStyles.boldLabel
            );

            EditorGUILayout.HelpBox(
                "Creates any missing Nodes, Segments, Groups, Ambient Activity and Landmarks roots.",
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
        
        private void DrawAmbientActivityTools()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ambient Activity", EditorStyles.boldLabel);

            List<AmbientRouteNode> selectedNodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> selectedSegments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);
            List<AmbientRouteGroup> selectedGroups = AmbientRouteAuthoringUtility.GetSelectedGroups(graph);

            int selectedGeographyCount = selectedNodes.Count + selectedSegments.Count + selectedGroups.Count;

            newAmbientActivityName = EditorGUILayout.TextField("New Area Name", newAmbientActivityName);

            EditorGUILayout.LabelField(
                "Selected Footprint",
                $"{selectedNodes.Count} Nodes / {selectedSegments.Count} Segments / {selectedGroups.Count} Groups"
            );

            using (new EditorGUI.DisabledScope(selectedGeographyCount == 0))
            {
                if (GUILayout.Button("Create Ambient Activity Area From Selection"))
                {
                    AmbientActivityArea created = AmbientActivityAuthoringUtility.CreateAreaFromSelection(graph, newAmbientActivityName);

                    if (created != null)
                    {
                        ambientActivityTarget = created;
                        SyncAmbientActivityFields();
                    }

                    RefreshValidation();
                }
            }

            EditorGUI.BeginChangeCheck();

            ambientActivityTarget = (AmbientActivityArea)EditorGUILayout.ObjectField(
                "Target Activity Area",
                ambientActivityTarget,
                typeof(AmbientActivityArea),
                true
            );

            if (EditorGUI.EndChangeCheck())
                SyncAmbientActivityFields();

            if (ambientActivityTarget == null)
            {
                EditorGUILayout.HelpBox(
                    "Select existing Route geography and create an Ambient Activity Area, or select an existing Area.",
                    MessageType.None
                );
                return;
            }

            EditorGUILayout.LabelField(
                "Current Footprint",
                $"{ambientActivityTarget.Nodes.Count} Nodes / {ambientActivityTarget.Segments.Count} Segments / " +
                $"{ambientActivityTarget.Groups.Count} Groups"
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Population", EditorStyles.boldLabel);

            foreach (AmbientPopulationEntry population in ambientActivityTarget.Populations)
            {
                string actorName = population?.ActorDefinition != null ? population.ActorDefinition.name : "(missing Actor)";
                int count = population != null ? population.Count : 0;
                EditorGUILayout.LabelField(actorName, $"× {count}");
            }

            ambientActorDefinition = (ActorDefinition)EditorGUILayout.ObjectField(
                "Actor Definition",
                ambientActorDefinition,
                typeof(ActorDefinition),
                false
            );

            ambientActorCount = Mathf.Max(1, EditorGUILayout.IntField("Count", ambientActorCount));

            using (new EditorGUI.DisabledScope(ambientActorDefinition == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add / Update Population"))
                {
                    AmbientActivityAuthoringUtility.SetPopulation(
                        ambientActivityTarget,
                        ambientActorDefinition,
                        ambientActorCount
                    );

                    RefreshValidation();
                }

                if (GUILayout.Button("Remove Population"))
                {
                    AmbientActivityAuthoringUtility.RemovePopulation(ambientActivityTarget, ambientActorDefinition);
                    RefreshValidation();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Area Settings", EditorStyles.boldLabel);

            ambientSpawnScatterRadius = EditorGUILayout.FloatField("Spawn Scatter Radius", ambientSpawnScatterRadius);
            ambientEngagementRadius = EditorGUILayout.FloatField("Engagement Radius", ambientEngagementRadius);

            if (GUILayout.Button("Apply Activity Settings"))
            {
                AmbientActivityAuthoringUtility.SetSettings(
                    ambientActivityTarget,
                    ambientSpawnScatterRadius,
                    ambientEngagementRadius
                );

                SyncAmbientActivityFields();
                RefreshValidation();
            }

            EditorGUILayout.HelpBox(
                "Ambient actors spawn from the Area's Route footprint. Segments are preferred when available; " +
                "Nodes provide point-like fallback placement. Generated Area data remains manually editable.",
                MessageType.None
            );
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
        
        private void DrawCompatibilityTools()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Encounter Compatibility", EditorStyles.boldLabel);

            compatibilityDefinition = (EncounterDefinition)EditorGUILayout.ObjectField(
                "Encounter Definition",
                compatibilityDefinition,
                typeof(EncounterDefinition),
                false
            );

            if (compatibilityDefinition == null)
            {
                EditorGUILayout.HelpBox(
                    "Select an Encounter Definition asset to preview where it could be automatically placed.",
                    MessageType.None
                );
                return;
            }

            bool allowAutomaticPlacement = EditorGUILayout.Toggle(
                "Allow Automatic Placement",
                compatibilityDefinition.AllowAutomaticPlacement
            );

            if (allowAutomaticPlacement != compatibilityDefinition.AllowAutomaticPlacement)
                EncounterCompatibilityAuthoringUtility.SetAutomaticPlacement(compatibilityDefinition, allowAutomaticPlacement);

            EditorGUILayout.LabelField("Required", FormatTags(compatibilityDefinition.RequiredRouteTags));
            EditorGUILayout.LabelField("Preferred", FormatTags(compatibilityDefinition.PreferredRouteTags));
            EditorGUILayout.LabelField("Forbidden", FormatTags(compatibilityDefinition.ForbiddenRouteTags));

            compatibilityTag = (RouteTagDefinition)EditorGUILayout.ObjectField(
                "Constraint Tag",
                compatibilityTag,
                typeof(RouteTagDefinition),
                false
            );

            using (new EditorGUI.DisabledScope(compatibilityTag == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Set Required"))
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        compatibilityDefinition,
                        compatibilityTag,
                        EncounterCompatibilityTagRole.Required
                    );

                if (GUILayout.Button("Set Preferred"))
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        compatibilityDefinition,
                        compatibilityTag,
                        EncounterCompatibilityTagRole.Preferred
                    );

                if (GUILayout.Button("Set Forbidden"))
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        compatibilityDefinition,
                        compatibilityTag,
                        EncounterCompatibilityTagRole.Forbidden
                    );

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Constraint"))
                    EncounterCompatibilityAuthoringUtility.RemoveTagConstraint(compatibilityDefinition, compatibilityTag);
            }

            EditorGUILayout.HelpBox(
                "Required Tags are hard placement requirements. Forbidden Tags reject a Site. " +
                "Preferred Tags rank otherwise-compatible Sites without rejecting them.",
                MessageType.None
            );

            compatibilityValidationIssues.Clear();
            EncounterRouteCompatibility.CollectDefinitionValidationIssues(compatibilityDefinition, compatibilityValidationIssues);

            foreach (string issue in compatibilityValidationIssues)
                EditorGUILayout.HelpBox(issue, MessageType.Warning);

            List<EncounterRouteSite> sites = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph);

            EditorGUILayout.LabelField("Site Compatibility Preview", $"{sites.Count} Sites");

            if (sites.Count == 0)
            {
                EditorGUILayout.HelpBox("The selected Route Graph has no Encounter Route Sites to preview.", MessageType.None);
                return;
            }

            foreach (EncounterRouteSite site in sites)
                DrawCompatibilityResult(EncounterRouteCompatibility.Evaluate(compatibilityDefinition, site));

            EncounterRouteCompatibility.CollectCompatibleSites(compatibilityDefinition, sites, compatibleSiteResults);

            EditorGUILayout.LabelField("Ranked Compatible Candidates", compatibleSiteResults.Count.ToString());

            if (compatibleSiteResults.Count == 0)
            {
                EditorGUILayout.HelpBox("No current Route Sites are valid automatic-placement candidates.", MessageType.None);
                return;
            }

            for (int i = 0; i < compatibleSiteResults.Count; i++)
            {
                EncounterRouteCompatibilityResult result = compatibleSiteResults[i];
                EditorGUILayout.LabelField(
                    $"{i + 1}. {GetSiteDisplayName(result.Site)}",
                    FormatPreference(result)
                );
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
                graph.CollectValidationIssues(validationIssues);
                EncounterRouteSiteAuthoringUtility.AppendValidationIssues(graph, validationIssues);
                AmbientActivityAuthoringUtility.AppendValidationIssues(graph, validationIssues);
            }

            validationHasRun = true;
            Repaint();
        }
        
        private void TryAdoptAmbientActivityFromSelection()
        {
            AmbientActivityArea selectedArea = AmbientActivityAuthoringUtility.ResolveAreaFromSelection();

            if (selectedArea == null || selectedArea == ambientActivityTarget)
                return;

            ambientActivityTarget = selectedArea;
            SyncAmbientActivityFields();
        }
        
        private void SyncAmbientActivityFields()
        {
            if (ambientActivityTarget == null)
                return;

            ambientSpawnScatterRadius = ambientActivityTarget.SpawnScatterRadius;
            ambientEngagementRadius = ambientActivityTarget.EngagementRadius;
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
        
        private void TryAdoptCompatibilityDefinitionFromSelection()
        {
            EncounterDefinition selectedDefinition = Selection.activeObject as EncounterDefinition;

            if (selectedDefinition == null || selectedDefinition == compatibilityDefinition)
                return;

            compatibilityDefinition = selectedDefinition;
        }
        
        private static void DrawCompatibilityResult(EncounterRouteCompatibilityResult result)
        {
            string siteName = GetSiteDisplayName(result.Site);
            string message;
            MessageType messageType;

            if (!result.HasValidInputs)
            {
                message = $"{siteName}\nInvalid compatibility inputs.";
                messageType = MessageType.Error;
            }
            else if (!result.AutomaticPlacementEnabled)
            {
                message = $"{siteName}\nAutomatic placement disabled.";
                messageType = MessageType.None;
            }
            else if (result.IsCompatible)
            {
                message = $"{siteName}\nCompatible | {FormatPreference(result)}";
                messageType = MessageType.Info;
            }
            else
            {
                message = $"{siteName}\nIncompatible";
                messageType = MessageType.Warning;
            }

            message += $"\nSite Tags: {FormatTags(result.EffectiveSiteTags)}";

            if (result.MissingRequiredTags.Count > 0)
                message += $"\nMissing Required: {FormatTags(result.MissingRequiredTags)}";

            if (result.PresentForbiddenTags.Count > 0)
                message += $"\nForbidden Present: {FormatTags(result.PresentForbiddenTags)}";

            EditorGUILayout.HelpBox(message, messageType);
        }

        private static string FormatPreference(EncounterRouteCompatibilityResult result)
        {
            if (result.PreferredTagCount == 0)
                return "No preferred Tags";

            int percent = Mathf.RoundToInt(result.PreferenceScore * 100f);
            return $"Preferred {result.PreferredMatchCount}/{result.PreferredTagCount} ({percent}%)";
        }

        private static string GetSiteDisplayName(EncounterRouteSite site)
        {
            if (site == null)
                return "(missing Site)";

            return site.Encounter != null ? site.Encounter.DisplayName : site.name;
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