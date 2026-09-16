using System.Collections.Generic;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public sealed class EncounterRouteSite : MonoBehaviour
    {
        [SerializeField] private LevelEncounter encounter;

        [Header("Route Footprint")]
        [SerializeField] private List<AmbientRouteNode> nodes = new();
        [SerializeField] private List<AmbientRouteSegment> segments = new();

        [Header("Traversal")]
        [SerializeField] private bool startEncounterOnTraversal = true;

        private readonly List<AmbientRouteNode> footprintNodes = new();
        private readonly List<RouteTagDefinition> tagBuffer = new();

        public LevelEncounter Encounter =>
            encounter != null
                ? encounter
                : GetComponent<LevelEncounter>();

        public IReadOnlyList<AmbientRouteNode> Nodes => nodes;
        public IReadOnlyList<AmbientRouteSegment> Segments => segments;
        public bool StartEncounterOnTraversal => startEncounterOnTraversal;

        public AmbientRouteGraph RouteGraph => ResolveRouteGraph();

        public bool HasFootprint
        {
            get
            {
                CollectFootprintNodes(footprintNodes);
                return footprintNodes.Count > 0;
            }
        }
        
        public bool ContainsExplicitNode(AmbientRouteNode node)
        {
            return node != null && nodes.Contains(node);
        }

        public bool ContainsNode(AmbientRouteNode node)
        {
            if (node == null)
                return false;

            if (nodes.Contains(node))
                return true;

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null &&
                    segment.IsConfigured &&
                    segment.Connects(node))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsSegment(AmbientRouteSegment segment)
        {
            return segment != null && segments.Contains(segment);
        }
        
        public void CollectEffectiveTags(List<RouteTagDefinition> results)
        {
            results.Clear();

            AmbientRouteGraph graph = RouteGraph;

            if (graph == null)
                return;

            foreach (AmbientRouteNode node in nodes)
            {
                graph.CollectTags(node, tagBuffer);

                foreach (RouteTagDefinition tag in tagBuffer)
                    AddUnique(results, tag);
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                graph.CollectTags(segment, tagBuffer);

                foreach (RouteTagDefinition tag in tagBuffer)
                    AddUnique(results, tag);
            }
        }
        
        public bool TryStartEncounterFromTraversal()
        {
            LevelEncounter owningEncounter = Encounter;

            if (!startEncounterOnTraversal ||
                owningEncounter == null ||
                !owningEncounter.IsSelectable ||
                owningEncounter.HasStarted)
            {
                return false;
            }

            owningEncounter.ActivateCombat();
            owningEncounter.BeginSpawning();

            return owningEncounter.HasStarted;
        }

        public bool ReferencesGraph(AmbientRouteGraph graph)
        {
            if (graph == null)
                return false;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null && graph.ContainsNode(node))
                    return true;
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null && graph.ContainsSegment(segment))
                    return true;
            }

            return false;
        }

        public bool TryGetApproachNode(
            AmbientRouteGraph graph,
            out AmbientRouteNode approachNode)
        {
            approachNode = null;

            if (graph == null)
                return false;

            CollectFootprintNodes(footprintNodes);

            Vector3 anchor =
                Encounter != null
                    ? Encounter.AnchorPosition
                    : transform.position;

            float nearestDistanceSquared = float.MaxValue;

            foreach (AmbientRouteNode candidate in footprintNodes)
            {
                if (!graph.ContainsNode(candidate))
                    continue;

                Vector3 difference = candidate.Position - anchor;
                difference.y = 0f;

                float distanceSquared = difference.sqrMagnitude;

                if (distanceSquared >= nearestDistanceSquared)
                    continue;

                nearestDistanceSquared = distanceSquared;
                approachNode = candidate;
            }

            return approachNode != null;
        }

        public bool TryBuildApproachPath(
            AmbientRouteGraph graph,
            AmbientRouteNode start,
            List<AmbientRouteNode> results,
            out AmbientRouteNode approachNode)
        {
            results.Clear();
            approachNode = null;

            if (graph == null ||
                start == null ||
                !graph.ContainsNode(start) ||
                !TryGetApproachNode(graph, out approachNode))
            {
                return false;
            }

            return graph.TryFindShortestPath(
                start,
                approachNode,
                results
            );
        }

        public void CollectValidationIssues(
            AmbientRouteGraph expectedGraph,
            List<string> issues)
        {
            LevelEncounter owningEncounter = Encounter;

            if (owningEncounter == null)
            {
                issues.Add(
                    $"Route site '{name}' has no LevelEncounter assigned."
                );
            }

            bool hasValidReference = false;

            HashSet<AmbientRouteNode> seenNodes = new();

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                {
                    issues.Add(
                        $"Route site '{name}' contains an empty Node reference."
                    );

                    continue;
                }

                hasValidReference = true;

                if (!seenNodes.Add(node))
                {
                    issues.Add(
                        $"Route site '{name}' contains duplicate Node '{node.DisplayName}'."
                    );
                }

                if (expectedGraph != null &&
                    !expectedGraph.ContainsNode(node))
                {
                    issues.Add(
                        $"Route site '{name}' references Node '{node.DisplayName}' outside the selected Route Graph."
                    );
                }
            }

            HashSet<AmbientRouteSegment> seenSegments = new();

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null)
                {
                    issues.Add(
                        $"Route site '{name}' contains an empty Segment reference."
                    );

                    continue;
                }

                hasValidReference = true;

                if (!seenSegments.Add(segment))
                {
                    issues.Add(
                        $"Route site '{name}' contains duplicate Segment '{segment.DisplayName}'."
                    );
                }

                if (!segment.IsConfigured)
                {
                    issues.Add(
                        $"Route site '{name}' uses unconfigured Segment '{segment.DisplayName}'."
                    );
                }

                if (expectedGraph != null &&
                    !expectedGraph.ContainsSegment(segment))
                {
                    issues.Add(
                        $"Route site '{name}' references Segment '{segment.DisplayName}' outside the selected Route Graph."
                    );
                }
            }

            if (!hasValidReference)
            {
                issues.Add(
                    $"Route site '{name}' has no Route footprint."
                );

                return;
            }

            if (expectedGraph != null &&
                !TryGetApproachNode(expectedGraph, out _))
            {
                issues.Add(
                    $"Route site '{name}' cannot resolve an approach Node on Route Graph '{expectedGraph.name}'."
                );
            }
        }

        private AmbientRouteGraph ResolveRouteGraph()
        {
            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                    continue;

                AmbientRouteGraph graph =
                    node.GetComponentInParent<AmbientRouteGraph>();

                if (graph != null)
                    return graph;
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null)
                    continue;

                AmbientRouteGraph graph =
                    segment.GetComponentInParent<AmbientRouteGraph>();

                if (graph != null)
                    return graph;
            }

            return null;
        }

        private void CollectFootprintNodes(
            List<AmbientRouteNode> results)
        {
            results.Clear();

            foreach (AmbientRouteNode node in nodes)
                AddUnique(results, node);

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null || !segment.IsConfigured)
                    continue;

                AddUnique(results, segment.NodeA);
                AddUnique(results, segment.NodeB);
            }
        }

        private static void AddUnique<T>(List<T> items, T item)
        {
            if (item != null && !items.Contains(item))
                items.Add(item);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null)
                    Gizmos.DrawWireSphere(node.Position, 0.7f);
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null || !segment.IsConfigured)
                    continue;

                Gizmos.DrawLine(
                    segment.NodeA.Position + Vector3.up * 0.15f,
                    segment.NodeB.Position + Vector3.up * 0.15f
                );

                Gizmos.DrawWireSphere(
                    segment.NodeA.Position,
                    0.7f
                );

                Gizmos.DrawWireSphere(
                    segment.NodeB.Position,
                    0.7f
                );
            }

            AmbientRouteGraph graph = RouteGraph;

            if (graph != null &&
                TryGetApproachNode(graph, out AmbientRouteNode approachNode))
            {
                Gizmos.DrawWireSphere(
                    approachNode.Position,
                    1f
                );

                Gizmos.DrawLine(
                    approachNode.Position,
                    Encounter != null
                        ? Encounter.AnchorPosition
                        : transform.position
                );
            }
        }
    }
}