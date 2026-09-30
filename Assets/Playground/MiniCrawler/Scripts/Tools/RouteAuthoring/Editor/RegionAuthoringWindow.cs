using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Ecology;
using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    public sealed class RegionAuthoringWindow : EditorWindow
    {
        private enum AuthoringMode
        {
            Route,
            Ambient,
            Encounters
        }

        private static readonly string[] ModeLabels =
        {
            "Route",
            "Ambient",
            "Encounters"
        };

        [SerializeField] private AmbientRouteGraph graph;
        [SerializeField] private AuthoringMode mode;
        [SerializeField] private AmbientActivityArea ambientActivityTarget;
        [SerializeField] private ActorDefinition ambientActorDefinition;
        [SerializeField] private LevelEncounter encounterTarget;
        [SerializeField] private RouteTagDefinition routeTag;

        [SerializeField] private bool showRouteAdvanced;
        [SerializeField] private bool showAmbientBehaviour;
        [SerializeField] private bool showIssues;

        private int ambientActorCount = 1;
        private float ambientSpawnScatterRadius = 1.5f;
        private float ambientEngagementRadius = 3.5f;
        private float ambientWanderRadius = 2.5f;
        private float ambientWanderPauseMin = 1f;
        private float ambientWanderPauseMax = 3f;

        private readonly List<string> validationIssues = new();
        private readonly List<RouteTagDefinition> siteEffectiveTags = new();

        private Vector2 scrollPosition;

        [MenuItem("Tools/MiniCrawler/Region Authoring")]
        public static void Open()
        {
            RegionAuthoringWindow window = GetWindow<RegionAuthoringWindow>("Region Authoring");
            window.AdoptSelection();
            window.RefreshValidation();
            window.Show();
        }

        public static void OpenForGraph(AmbientRouteGraph graph)
        {
            RegionAuthoringWindow window = GetWindow<RegionAuthoringWindow>("Region Authoring");
            window.graph = graph;
            window.RefreshValidation();
            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            AdoptSelection();
            RefreshValidation();
        }

        private void OnSelectionChange()
        {
            AdoptSelection();
            RefreshValidation();
            Repaint();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();

            if (graph == null)
            {
                EditorGUILayout.HelpBox("Open a Region scene or select its Ambient Route.", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space(4);
            mode = (AuthoringMode)GUILayout.Toolbar((int)mode, ModeLabels);
            EditorGUILayout.Space(6);

            switch (mode)
            {
                case AuthoringMode.Route:
                    DrawRouteMode();
                    break;

                case AuthoringMode.Ambient:
                    DrawAmbientMode();
                    break;

                case AuthoringMode.Encounters:
                    DrawEncounterMode();
                    break;
            }

            DrawStatus();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUI.BeginChangeCheck();

            AmbientRouteGraph newGraph = (AmbientRouteGraph)EditorGUILayout.ObjectField(
                "Region",
                graph,
                typeof(AmbientRouteGraph),
                true
            );

            if (EditorGUI.EndChangeCheck())
            {
                graph = newGraph;
                ambientActivityTarget = null;
                encounterTarget = null;
                RefreshValidation();
            }

            if (graph == null)
                return;

            int nodeCount = AmbientRouteAuthoringUtility.GetOwnedNodes(graph).Count;
            int segmentCount = AmbientRouteAuthoringUtility.GetOwnedSegments(graph).Count;
            int activityCount = AmbientActivityAuthoringUtility.GetAreasForGraph(graph).Count;
            int encounterCount = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph).Count;

            EditorGUILayout.LabelField(
                $"Points {nodeCount}  |  Connections {segmentCount}  |  Ambient {activityCount}  |  Encounters {encounterCount}",
                EditorStyles.miniLabel
            );
        }

        private void DrawRouteMode()
        {
            EditorGUILayout.LabelField("Route Layout", EditorStyles.boldLabel);

            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);
            List<AmbientRouteGroup> groups = AmbientRouteAuthoringUtility.GetSelectedGroups(graph);

            EditorGUILayout.LabelField("Selection", FormatSelection(nodes.Count, segments.Count, groups.Count));

            if (GUILayout.Button(new GUIContent("+ Route Point", "Create a Route Point at the current Scene view pivot.")))
            {
                AmbientRouteAuthoringUtility.CreateNode(graph, "Route Point");
                RefreshValidation();
            }

            if (nodes.Count == 2)
            {
                if (GUILayout.Button(new GUIContent("Connect", "Connect the two selected Route Points.")))
                {
                    AmbientRouteAuthoringUtility.ConnectSelectedPair(graph);
                    RefreshValidation();
                }
            }
            else if (nodes.Count >= 3)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(new GUIContent("Make Path", "Connect selected Route Points in hierarchy order.")))
                {
                    AmbientRouteAuthoringUtility.ConnectSelectionAsHierarchyPath(graph);
                    RefreshValidation();
                }

                if (GUILayout.Button(new GUIContent("Make Loop", "Connect selected Route Points spatially around their centre.")))
                {
                    AmbientRouteAuthoringUtility.ConnectSelectionAsSpatialLoop(graph);
                    RefreshValidation();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (nodes.Count > 0 || segments.Count > 0)
            {
                if (GUILayout.Button(new GUIContent("Group Selection", "Create a Route Group from the selected Route geography.")))
                {
                    AmbientRouteAuthoringUtility.CreateGroupFromSelection(graph, "Route Group");
                    RefreshValidation();
                }
            }

            EditorGUILayout.Space(6);
            showRouteAdvanced = EditorGUILayout.Foldout(showRouteAdvanced, "Advanced", true);

            if (!showRouteAdvanced)
                return;

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Geography Tags", EditorStyles.boldLabel);

            routeTag = (RouteTagDefinition)EditorGUILayout.ObjectField(
                "Tag",
                routeTag,
                typeof(RouteTagDefinition),
                false
            );

            int selectedCount = nodes.Count + segments.Count + groups.Count;

            using (new EditorGUI.DisabledScope(routeTag == null || selectedCount == 0))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add Tag"))
                {
                    RouteTagAuthoringUtility.AddTagToSelection(graph, routeTag);
                    RefreshValidation();
                }

                if (GUILayout.Button("Remove Tag"))
                {
                    RouteTagAuthoringUtility.RemoveTagFromSelection(graph, routeTag);
                    RefreshValidation();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("New Tag..."))
            {
                RouteTagDefinition created = RouteTagAuthoringUtility.CreateTagAsset();

                if (created != null)
                    routeTag = created;
            }

            EditorGUILayout.Space(4);

            if (GUILayout.Button(new GUIContent("Repair Missing Authoring Roots", "Recreate missing internal Region authoring roots.")))
            {
                AmbientRouteAuthoringUtility.EnsureAuthoringStructure(graph);
                RefreshValidation();
            }
        }

        private void DrawAmbientMode()
        {
            EditorGUILayout.LabelField("Ambient Life", EditorStyles.boldLabel);

            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);
            List<AmbientRouteGroup> groups = AmbientRouteAuthoringUtility.GetSelectedGroups(graph);

            int selectedGeographyCount = nodes.Count + segments.Count + groups.Count;

            EditorGUILayout.LabelField("Selection", FormatSelection(nodes.Count, segments.Count, groups.Count));

            if (selectedGeographyCount > 0)
            {
                if (GUILayout.Button(new GUIContent("+ Ambient Area Here", "Create an Ambient Activity Area from the selected Route geography.")))
                {
                    AmbientActivityArea created = AmbientActivityAuthoringUtility.CreateAreaFromSelection(graph, "Ambient Activity");

                    if (created != null)
                    {
                        ambientActivityTarget = created;
                        SyncAmbientActivityFields();
                    }

                    RefreshValidation();
                }
            }

            if (ambientActivityTarget == null || !ambientActivityTarget.ReferencesGraph(graph))
            {
                EditorGUILayout.HelpBox("Select an Ambient Activity Area to edit it.", MessageType.None);
                return;
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField(ambientActivityTarget.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Footprint",
                FormatSelection(
                    ambientActivityTarget.Nodes.Count,
                    ambientActivityTarget.Segments.Count,
                    ambientActivityTarget.Groups.Count
                )
            );

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Population", EditorStyles.boldLabel);

            if (ambientActivityTarget.Populations.Count == 0)
            {
                EditorGUILayout.LabelField("(No population)");
            }
            else
            {
                foreach (AmbientPopulationEntry population in ambientActivityTarget.Populations)
                {
                    string actorName = population?.ActorDefinition != null ? population.ActorDefinition.name : "(Missing Actor)";
                    int count = population != null ? population.Count : 0;
                    EditorGUILayout.LabelField(actorName, $"× {count}");
                }
            }

            ambientActorDefinition = (ActorDefinition)EditorGUILayout.ObjectField(
                "Actor",
                ambientActorDefinition,
                typeof(ActorDefinition),
                false
            );

            ambientActorCount = Mathf.Max(1, EditorGUILayout.IntField("Count", ambientActorCount));

            using (new EditorGUI.DisabledScope(ambientActorDefinition == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add / Update"))
                {
                    AmbientActivityAuthoringUtility.SetPopulation(
                        ambientActivityTarget,
                        ambientActorDefinition,
                        ambientActorCount
                    );

                    RefreshValidation();
                }

                if (GUILayout.Button("Remove"))
                {
                    AmbientActivityAuthoringUtility.RemovePopulation(ambientActivityTarget, ambientActorDefinition);
                    RefreshValidation();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(6);
            showAmbientBehaviour = EditorGUILayout.Foldout(showAmbientBehaviour, "Behaviour", true);

            if (!showAmbientBehaviour)
                return;

            ambientSpawnScatterRadius = EditorGUILayout.FloatField("Spawn Spread", ambientSpawnScatterRadius);
            ambientEngagementRadius = EditorGUILayout.FloatField("Engage Distance", ambientEngagementRadius);
            ambientWanderRadius = EditorGUILayout.FloatField("Wander Distance", ambientWanderRadius);
            ambientWanderPauseMin = EditorGUILayout.FloatField("Pause Min", ambientWanderPauseMin);
            ambientWanderPauseMax = EditorGUILayout.FloatField("Pause Max", ambientWanderPauseMax);

            if (GUILayout.Button("Apply Behaviour"))
            {
                AmbientActivityAuthoringUtility.SetSettings(
                    ambientActivityTarget,
                    ambientSpawnScatterRadius,
                    ambientEngagementRadius,
                    ambientWanderRadius,
                    ambientWanderPauseMin,
                    ambientWanderPauseMax
                );

                SyncAmbientActivityFields();
                RefreshValidation();
            }
        }

        private void DrawEncounterMode()
        {
            EditorGUILayout.LabelField("Encounter Placement", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            LevelEncounter newEncounter = (LevelEncounter)EditorGUILayout.ObjectField(
                "Encounter",
                encounterTarget,
                typeof(LevelEncounter),
                true
            );

            if (EditorGUI.EndChangeCheck())
            {
                encounterTarget = newEncounter;
                RefreshValidation();
            }

            if (encounterTarget == null)
            {
                EditorGUILayout.HelpBox("Select a Level Encounter in the Hierarchy.", MessageType.None);
                return;
            }

            List<AmbientRouteNode> nodes = AmbientRouteAuthoringUtility.GetSelectedNodes(graph);
            List<AmbientRouteSegment> segments = AmbientRouteAuthoringUtility.GetSelectedSegments(graph);

            EncounterRouteSite site = encounterTarget.GetComponent<EncounterRouteSite>();

            EditorGUILayout.LabelField("Selection", FormatSelection(nodes.Count, segments.Count, 0));

            if (nodes.Count > 0 || segments.Count > 0)
            {
                string buttonLabel = site == null ? "Bind To Selection" : "Replace Site With Selection";

                if (GUILayout.Button(buttonLabel))
                {
                    site = EncounterRouteSiteAuthoringUtility.BindFromSelection(graph, encounterTarget);
                    RefreshValidation();
                }
            }
            else if (site == null)
            {
                EditorGUILayout.LabelField("Select Route Points or Connections to place this Encounter.", EditorStyles.miniLabel);
            }

            if (site != null)
            {
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField(
                    "Current Site",
                    FormatSelection(site.Nodes.Count, site.Segments.Count, 0)
                );

                bool startOnTraversal = EditorGUILayout.Toggle("Starts When Crossed", site.StartEncounterOnTraversal);

                if (startOnTraversal != site.StartEncounterOnTraversal)
                {
                    EncounterRouteSiteAuthoringUtility.SetStartEncounterOnTraversal(site, startOnTraversal);
                    RefreshValidation();
                }

                site.CollectEffectiveTags(siteEffectiveTags);
                EditorGUILayout.LabelField("Tags", FormatTags(siteEffectiveTags));
            }

            EditorGUILayout.Space(6);

            EncounterDefinition definition = GetBoundDefinition(encounterTarget);
            EditorGUILayout.LabelField("Definition", definition != null ? definition.DisplayName : "(Not bound)");

            if (GUILayout.Button(new GUIContent("Placement Rules...", "Edit reusable automatic-placement rules and preview compatible Sites.")))
                EncounterPlacementWindow.OpenFor(definition, graph);
        }

        private void DrawStatus()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (validationIssues.Count == 0)
            {
                EditorGUILayout.LabelField("Status: Ready");
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Status: {validationIssues.Count} issue(s)");

                if (GUILayout.Button(showIssues ? "Hide" : "Show", GUILayout.Width(60f)))
                    showIssues = !showIssues;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            if (!showIssues || validationIssues.Count == 0)
                return;

            foreach (string issue in validationIssues)
                EditorGUILayout.HelpBox(issue, MessageType.Warning);
        }

        private void AdoptSelection()
        {
            TryAdoptGraphFromSelection();
            TryAdoptAmbientActivityFromSelection();
            TryAdoptEncounterFromSelection();
        }

        private void TryAdoptGraphFromSelection()
        {
            AmbientRouteGraph selectedGraph = AmbientRouteAuthoringUtility.ResolveGraphFromSelection();

            if (selectedGraph == null && graph == null)
                selectedGraph = Object.FindFirstObjectByType<AmbientRouteGraph>();

            if (selectedGraph != null)
                graph = selectedGraph;
        }

        private void TryAdoptAmbientActivityFromSelection()
        {
            AmbientActivityArea selectedArea = AmbientActivityAuthoringUtility.ResolveAreaFromSelection();

            if (selectedArea == null || selectedArea == ambientActivityTarget)
                return;

            ambientActivityTarget = selectedArea;
            mode = AuthoringMode.Ambient;
            SyncAmbientActivityFields();
        }

        private void TryAdoptEncounterFromSelection()
        {
            LevelEncounter selectedEncounter = EncounterRouteSiteAuthoringUtility.ResolveEncounterFromSelection();

            if (selectedEncounter == null || selectedEncounter == encounterTarget)
                return;

            encounterTarget = selectedEncounter;
            mode = AuthoringMode.Encounters;
        }

        private void SyncAmbientActivityFields()
        {
            if (ambientActivityTarget == null)
                return;

            ambientSpawnScatterRadius = ambientActivityTarget.SpawnScatterRadius;
            ambientEngagementRadius = ambientActivityTarget.EngagementRadius;
            ambientWanderRadius = ambientActivityTarget.WanderRadius;
            ambientWanderPauseMin = ambientActivityTarget.WanderPauseMinSeconds;
            ambientWanderPauseMax = ambientActivityTarget.WanderPauseMaxSeconds;

            ambientActorDefinition = null;
            ambientActorCount = 1;

            foreach (AmbientPopulationEntry population in ambientActivityTarget.Populations)
            {
                if (population?.ActorDefinition == null)
                    continue;

                ambientActorDefinition = population.ActorDefinition;
                ambientActorCount = population.Count;
                break;
            }
        }

        private void RefreshValidation()
        {
            validationIssues.Clear();

            if (graph == null)
                return;

            graph.CollectValidationIssues(validationIssues);
            EncounterRouteSiteAuthoringUtility.AppendValidationIssues(graph, validationIssues);
            AmbientActivityAuthoringUtility.AppendValidationIssues(graph, validationIssues);
        }

        private static EncounterDefinition GetBoundDefinition(LevelEncounter encounter)
        {
            if (encounter == null)
                return null;

            LevelEncounterDefinitionBinding binding = encounter.GetComponent<LevelEncounterDefinitionBinding>();
            return binding != null ? binding.Definition : null;
        }

        private static string FormatSelection(int nodeCount, int segmentCount, int groupCount)
        {
            List<string> parts = new();

            if (nodeCount > 0)
                parts.Add($"{nodeCount} point{(nodeCount == 1 ? string.Empty : "s")}");

            if (segmentCount > 0)
                parts.Add($"{segmentCount} connection{(segmentCount == 1 ? string.Empty : "s")}");

            if (groupCount > 0)
                parts.Add($"{groupCount} group{(groupCount == 1 ? string.Empty : "s")}");

            return parts.Count > 0 ? string.Join(", ", parts) : "Nothing selected";
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