using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    public sealed class EncounterPlacementWindow : EditorWindow
    {
        [SerializeField] private EncounterDefinition definition;
        [SerializeField] private AmbientRouteGraph graph;
        [SerializeField] private RouteTagDefinition constraintTag;
        [SerializeField] private bool showDiagnostics;

        private readonly List<string> validationIssues = new();
        private readonly List<EncounterRouteCompatibilityResult> compatibleSites = new();

        private Vector2 scrollPosition;

        [MenuItem("Tools/MiniCrawler/Encounter Placement Rules")]
        public static void Open()
        {
            EncounterPlacementWindow window = GetWindow<EncounterPlacementWindow>("Encounter Placement");
            window.AdoptSelection();
            window.Show();
        }

        public static void OpenFor(EncounterDefinition definition, AmbientRouteGraph graph)
        {
            EncounterPlacementWindow window = GetWindow<EncounterPlacementWindow>("Encounter Placement");
            window.definition = definition;
            window.graph = graph;
            window.Show();
            window.Focus();
        }

        private void OnEnable()
        {
            AdoptSelection();
        }

        private void OnSelectionChange()
        {
            AdoptSelection();
            Repaint();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            definition = (EncounterDefinition)EditorGUILayout.ObjectField(
                "Encounter Definition",
                definition,
                typeof(EncounterDefinition),
                false
            );

            graph = (AmbientRouteGraph)EditorGUILayout.ObjectField(
                "Preview Region",
                graph,
                typeof(AmbientRouteGraph),
                true
            );

            if (definition == null)
            {
                EditorGUILayout.HelpBox("Select an Encounter Definition asset.", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space(6);
            DrawRules();

            EditorGUILayout.Space(10);
            DrawPreview();

            EditorGUILayout.EndScrollView();
        }

        private void DrawRules()
        {
            EditorGUILayout.LabelField("Placement Rules", EditorStyles.boldLabel);

            bool automaticPlacement = EditorGUILayout.Toggle(
                "Automatic Placement",
                definition.AllowAutomaticPlacement
            );

            if (automaticPlacement != definition.AllowAutomaticPlacement)
                EncounterCompatibilityAuthoringUtility.SetAutomaticPlacement(definition, automaticPlacement);

            EditorGUILayout.LabelField("Must Have", FormatTags(definition.RequiredRouteTags));
            EditorGUILayout.LabelField("Likes", FormatTags(definition.PreferredRouteTags));
            EditorGUILayout.LabelField("Avoid", FormatTags(definition.ForbiddenRouteTags));

            EditorGUILayout.Space(4);

            constraintTag = (RouteTagDefinition)EditorGUILayout.ObjectField(
                "Tag",
                constraintTag,
                typeof(RouteTagDefinition),
                false
            );

            using (new EditorGUI.DisabledScope(constraintTag == null))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Must Have"))
                {
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        definition,
                        constraintTag,
                        EncounterCompatibilityTagRole.Required
                    );
                }

                if (GUILayout.Button("Like"))
                {
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        definition,
                        constraintTag,
                        EncounterCompatibilityTagRole.Preferred
                    );
                }

                if (GUILayout.Button("Avoid"))
                {
                    EncounterCompatibilityAuthoringUtility.SetTagRole(
                        definition,
                        constraintTag,
                        EncounterCompatibilityTagRole.Forbidden
                    );
                }

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Rule"))
                    EncounterCompatibilityAuthoringUtility.RemoveTagConstraint(definition, constraintTag);
            }

            validationIssues.Clear();
            EncounterRouteCompatibility.CollectDefinitionValidationIssues(definition, validationIssues);

            foreach (string issue in validationIssues)
                EditorGUILayout.HelpBox(issue, MessageType.Warning);
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Current Region", EditorStyles.boldLabel);

            if (!definition.AllowAutomaticPlacement)
            {
                EditorGUILayout.LabelField("Manual placement only.");
                return;
            }

            if (graph == null)
            {
                EditorGUILayout.LabelField("No Region selected for preview.");
                return;
            }

            List<EncounterRouteSite> sites = EncounterRouteSiteAuthoringUtility.GetSitesForGraph(graph);

            if (sites.Count == 0)
            {
                EditorGUILayout.LabelField("No Encounter Sites in this Region.");
                return;
            }

            EncounterRouteCompatibility.CollectCompatibleSites(definition, sites, compatibleSites);

            EditorGUILayout.LabelField("Compatible Sites", $"{compatibleSites.Count} / {sites.Count}");

            if (compatibleSites.Count == 0)
            {
                EditorGUILayout.LabelField("No compatible Sites.");
            }
            else
            {
                for (int i = 0; i < compatibleSites.Count; i++)
                {
                    EncounterRouteCompatibilityResult result = compatibleSites[i];
                    EditorGUILayout.LabelField(
                        $"{i + 1}. {GetSiteDisplayName(result.Site)}",
                        FormatPreference(result)
                    );
                }
            }

            EditorGUILayout.Space(4);
            showDiagnostics = EditorGUILayout.Foldout(showDiagnostics, "Diagnostics", true);

            if (!showDiagnostics)
                return;

            foreach (EncounterRouteSite site in sites)
                DrawDiagnostic(EncounterRouteCompatibility.Evaluate(definition, site));
        }

        private void AdoptSelection()
        {
            EncounterDefinition selectedDefinition = Selection.activeObject as EncounterDefinition;

            if (selectedDefinition != null)
                definition = selectedDefinition;

            GameObject active = Selection.activeGameObject;

            if (active != null)
            {
                LevelEncounterDefinitionBinding binding = active.GetComponent<LevelEncounterDefinitionBinding>();

                if (binding == null)
                    binding = active.GetComponentInParent<LevelEncounterDefinitionBinding>();

                if (binding?.Definition != null)
                    definition = binding.Definition;
            }

            AmbientRouteGraph selectedGraph = AmbientRouteAuthoringUtility.ResolveGraphFromSelection();

            if (selectedGraph != null)
                graph = selectedGraph;
            else if (graph == null)
                graph = Object.FindFirstObjectByType<AmbientRouteGraph>();
        }

        private static void DrawDiagnostic(EncounterRouteCompatibilityResult result)
        {
            string siteName = GetSiteDisplayName(result.Site);
            string message = result.IsCompatible ? $"{siteName}: Compatible" : $"{siteName}: Incompatible";
            MessageType type = result.IsCompatible ? MessageType.Info : MessageType.Warning;

            message += $"\nTags: {FormatTags(result.EffectiveSiteTags)}";

            if (result.MissingRequiredTags.Count > 0)
                message += $"\nMissing: {FormatTags(result.MissingRequiredTags)}";

            if (result.PresentForbiddenTags.Count > 0)
                message += $"\nAvoided Tag Present: {FormatTags(result.PresentForbiddenTags)}";

            EditorGUILayout.HelpBox(message, type);
        }

        private static string FormatPreference(EncounterRouteCompatibilityResult result)
        {
            if (result.PreferredTagCount == 0)
                return "Compatible";

            return $"{result.PreferredMatchCount}/{result.PreferredTagCount} preferences";
        }

        private static string GetSiteDisplayName(EncounterRouteSite site)
        {
            if (site == null)
                return "(Missing Site)";

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