using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Tools
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AmbientRouteGraph))]
    public sealed class EncounterWorkshopRouteContext : MonoBehaviour
    {
        public enum ContextKind
        {
            None,
            Point,
            Segment,
            Path,
            RouteGroup
        }

        [Header("Encounter")]
        [SerializeField] private EncounterRouteSite encounterSite;

        [Header("Semantic Roles")]
        [SerializeField] private AmbientRouteNode entry;
        [SerializeField] private AmbientRouteNode core;
        [SerializeField] private AmbientRouteNode exit;

        [Header("Route Geography")]
        [SerializeField] private AmbientRouteSegment entryToCore;
        [SerializeField] private AmbientRouteSegment coreToExit;
        [SerializeField] private AmbientRouteGroup routeGroup;

        private readonly List<AmbientRouteNode> nodes = new();
        private readonly List<AmbientRouteSegment> segments = new();

        public EncounterRouteSite EncounterSite => encounterSite;
        public AmbientRouteNode Entry => entry;
        public AmbientRouteNode Core => core;
        public AmbientRouteNode Exit => exit;

        public bool IsConfigured =>
            encounterSite != null &&
            entry != null &&
            core != null &&
            exit != null &&
            entryToCore != null &&
            coreToExit != null &&
            routeGroup != null;

        public bool Apply(ContextKind kind, out string error)
        {
            error = string.Empty;

            if (encounterSite == null)
            {
                error = "Workshop Route Context is missing its Encounter Route Site.";
                return false;
            }

            nodes.Clear();
            segments.Clear();

            if (kind == ContextKind.None)
            {
                encounterSite.SetRuntimeFootprintOverride(nodes, segments);
                return true;
            }

            if (!IsConfigured)
            {
                error = "Workshop Route Context is not fully configured.";
                return false;
            }

            switch (kind)
            {
                case ContextKind.Point:
                    AddUnique(nodes, core);
                    break;

                case ContextKind.Segment:
                    AddUnique(segments, entryToCore);
                    break;

                case ContextKind.Path:
                    AddUnique(segments, entryToCore);
                    AddUnique(segments, coreToExit);
                    break;

                case ContextKind.RouteGroup:
                    foreach (AmbientRouteNode node in routeGroup.Nodes)
                        AddUnique(nodes, node);

                    foreach (AmbientRouteSegment segment in routeGroup.Segments)
                        AddUnique(segments, segment);
                    break;
            }

            encounterSite.SetRuntimeFootprintOverride(nodes, segments);
            return true;
        }

        public void ClearRuntimeOverride()
        {
            encounterSite?.ClearRuntimeFootprintOverride();
        }

        public static string GetDisplayName(ContextKind kind)
        {
            return kind switch
            {
                ContextKind.None => "None",
                ContextKind.Point => "Point",
                ContextKind.Segment => "Segment",
                ContextKind.Path => "Path",
                ContextKind.RouteGroup => "Group",
                _ => kind.ToString()
            };
        }

        private static void AddUnique<T>(List<T> items, T item) where T : Object
        {
            if (item != null && !items.Contains(item))
                items.Add(item);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (entry != null)
                UnityEditor.Handles.Label(entry.Position + Vector3.up, "ENTRY");

            if (core != null)
                UnityEditor.Handles.Label(core.Position + Vector3.up, "CORE");

            if (exit != null)
                UnityEditor.Handles.Label(exit.Position + Vector3.up, "EXIT");
        }
#endif
    }
}