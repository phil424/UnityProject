using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteGraph : MonoBehaviour
    {
        private AmbientRouteNode[] nodes = Array.Empty<AmbientRouteNode>();
        private AmbientRouteSegment[] segments = Array.Empty<AmbientRouteSegment>();
        private AmbientRouteGroup[] groups = Array.Empty<AmbientRouteGroup>();

        public IReadOnlyList<AmbientRouteNode> Nodes => nodes;
        public IReadOnlyList<AmbientRouteSegment> Segments => segments;
        public IReadOnlyList<AmbientRouteGroup> Groups => groups;

        private void OnEnable()
        {
            RefreshContent();
        }

        public bool ContainsNode(AmbientRouteNode node)
        {
            return OwnsNode(node);
        }

        public bool ContainsSegment(AmbientRouteSegment segment)
        {
            return OwnsSegment(segment);
        }

        public AmbientRouteNode FindNearestNode(Vector3 position)
        {
            AmbientRouteNode nearest = null;
            float nearestDistanceSquared = float.MaxValue;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                    continue;

                Vector3 difference = node.Position - position;
                difference.y = 0f;

                float distanceSquared = difference.sqrMagnitude;

                if (distanceSquared >= nearestDistanceSquared)
                    continue;

                nearestDistanceSquared = distanceSquared;
                nearest = node;
            }

            return nearest;
        }

        public AmbientRouteSegment FindSegment(
            AmbientRouteNode a,
            AmbientRouteNode b)
        {
            if (a == null || b == null)
                return null;

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null || !segment.IsConfigured)
                    continue;

                if ((segment.NodeA == a && segment.NodeB == b) ||
                    (segment.NodeA == b && segment.NodeB == a))
                {
                    return segment;
                }
            }

            return null;
        }

        public void GetConnectedSegments(
            AmbientRouteNode node,
            List<AmbientRouteSegment> results)
        {
            results.Clear();

            if (!OwnsNode(node))
                return;

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null &&
                    segment.IsConfigured &&
                    segment.Connects(node))
                {
                    results.Add(segment);
                }
            }
        }

        public void GetNeighbours(
            AmbientRouteNode node,
            List<AmbientRouteNode> results)
        {
            results.Clear();

            if (!OwnsNode(node))
                return;

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null ||
                    !segment.IsConfigured ||
                    !segment.Connects(node))
                {
                    continue;
                }

                AmbientRouteNode other = segment.GetOther(node);

                if (OwnsNode(other) && !results.Contains(other))
                    results.Add(other);
            }
        }

        public void GetReachableNodes(
            AmbientRouteNode start,
            List<AmbientRouteNode> results)
        {
            results.Clear();

            if (!OwnsNode(start))
                return;

            Queue<AmbientRouteNode> open = new();
            HashSet<AmbientRouteNode> visited = new();
            List<AmbientRouteNode> neighbours = new();

            open.Enqueue(start);
            visited.Add(start);

            while (open.Count > 0)
            {
                AmbientRouteNode current = open.Dequeue();
                results.Add(current);

                GetNeighbours(current, neighbours);

                foreach (AmbientRouteNode neighbour in neighbours)
                {
                    if (visited.Add(neighbour))
                        open.Enqueue(neighbour);
                }
            }
        }

        public bool TryFindShortestPath(
            AmbientRouteNode start,
            AmbientRouteNode destination,
            List<AmbientRouteNode> results)
        {
            results.Clear();

            if (!OwnsNode(start) || !OwnsNode(destination))
                return false;

            if (start == destination)
            {
                results.Add(start);
                return true;
            }

            Dictionary<AmbientRouteNode, float> distances = new();
            Dictionary<AmbientRouteNode, AmbientRouteNode> previous = new();
            HashSet<AmbientRouteNode> visited = new();
            List<AmbientRouteNode> open = new();
            List<AmbientRouteNode> neighbours = new();

            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null)
                    distances[node] = float.MaxValue;
            }

            distances[start] = 0f;
            open.Add(start);

            while (open.Count > 0)
            {
                int bestIndex = 0;
                float bestDistance = distances[open[0]];

                for (int i = 1; i < open.Count; i++)
                {
                    float candidateDistance = distances[open[i]];

                    if (candidateDistance >= bestDistance)
                        continue;

                    bestDistance = candidateDistance;
                    bestIndex = i;
                }

                AmbientRouteNode current = open[bestIndex];
                open.RemoveAt(bestIndex);

                if (!visited.Add(current))
                    continue;

                if (current == destination)
                    break;

                GetNeighbours(current, neighbours);

                foreach (AmbientRouteNode neighbour in neighbours)
                {
                    if (visited.Contains(neighbour))
                        continue;

                    float candidateDistance =
                        distances[current] +
                        GetPlanarDistance(
                            current.Position,
                            neighbour.Position
                        );

                    if (candidateDistance >= distances[neighbour])
                        continue;

                    distances[neighbour] = candidateDistance;
                    previous[neighbour] = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }

            if (!previous.ContainsKey(destination))
                return false;

            AmbientRouteNode step = destination;
            results.Add(step);

            while (step != start)
            {
                if (!previous.TryGetValue(step, out step))
                {
                    results.Clear();
                    return false;
                }

                results.Add(step);
            }

            results.Reverse();
            return true;
        }

        public bool CollectValidationIssues(List<string> issues)
        {
            issues.Clear();
            RefreshContent();

            if (nodes.Length == 0)
            {
                issues.Add(
                    $"Route graph '{name}' contains no route nodes."
                );

                return false;
            }

            if (segments.Length == 0)
            {
                issues.Add(
                    $"Route graph '{name}' contains no route segments."
                );

                return false;
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null)
                    continue;

                if (!segment.IsConfigured)
                {
                    issues.Add(
                        $"Segment '{segment.DisplayName}' does not have two distinct endpoints."
                    );

                    continue;
                }

                if (!OwnsNode(segment.NodeA) ||
                    !OwnsNode(segment.NodeB))
                {
                    issues.Add(
                        $"Segment '{segment.DisplayName}' references a node outside route graph '{name}'."
                    );
                }
            }

            for (int i = 0; i < segments.Length; i++)
            {
                AmbientRouteSegment a = segments[i];

                if (a == null || !a.IsConfigured)
                    continue;

                for (int j = i + 1; j < segments.Length; j++)
                {
                    AmbientRouteSegment b = segments[j];

                    if (b == null || !b.IsConfigured)
                        continue;

                    if (SameEndpoints(a, b))
                    {
                        issues.Add(
                            $"Segments '{a.DisplayName}' and '{b.DisplayName}' connect the same two nodes."
                        );
                    }
                }
            }

            List<AmbientRouteSegment> connectedSegments = new();

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                    continue;

                GetConnectedSegments(node, connectedSegments);

                if (connectedSegments.Count == 0)
                {
                    issues.Add(
                        $"Route node '{node.DisplayName}' is isolated."
                    );
                }
            }

            List<AmbientRouteNode> reachable = new();

            GetReachableNodes(nodes[0], reachable);

            if (reachable.Count != nodes.Length)
            {
                List<string> unreachableNames = new();

                foreach (AmbientRouteNode node in nodes)
                {
                    if (node != null && !reachable.Contains(node))
                        unreachableNames.Add(node.DisplayName);
                }

                issues.Add(
                    $"Route graph '{name}' contains disconnected nodes: " +
                    string.Join(", ", unreachableNames)
                );
            }

            foreach (AmbientRouteGroup group in groups)
            {
                if (group == null)
                    continue;

                foreach (AmbientRouteNode node in group.Nodes)
                {
                    if (!OwnsNode(node))
                    {
                        issues.Add(
                            $"Route group '{group.DisplayName}' references a node outside this graph."
                        );
                    }
                }

                foreach (AmbientRouteSegment segment in group.Segments)
                {
                    if (!OwnsSegment(segment))
                    {
                        issues.Add(
                            $"Route group '{group.DisplayName}' references a segment outside this graph."
                        );

                        continue;
                    }

                    if (segment != null &&
                        segment.IsConfigured &&
                        (!group.ContainsNode(segment.NodeA) ||
                         !group.ContainsNode(segment.NodeB)))
                    {
                        issues.Add(
                            $"Route group '{group.DisplayName}' contains segment " +
                            $"'{segment.DisplayName}' but not both endpoint nodes."
                        );
                    }
                }
            }

            return issues.Count == 0;
        }

        [ContextMenu("Debug/Validate Route Geography")]
        private void ValidateRouteGeography()
        {
            List<string> issues = new();

            if (CollectValidationIssues(issues))
            {
                Debug.Log(
                    $"Route graph '{name}' is valid: " +
                    $"{nodes.Length} nodes, " +
                    $"{segments.Length} segments, " +
                    $"{groups.Length} groups.",
                    this
                );

                return;
            }

            foreach (string issue in issues)
                Debug.LogWarning($"[Route Geography] {issue}", this);
        }

        private bool OwnsNode(AmbientRouteNode node)
        {
            return node != null &&
                   node.GetComponentInParent<AmbientRouteGraph>() == this;
        }

        private bool OwnsSegment(AmbientRouteSegment segment)
        {
            return segment != null &&
                   segment.GetComponentInParent<AmbientRouteGraph>() == this;
        }

        private static bool SameEndpoints(
            AmbientRouteSegment a,
            AmbientRouteSegment b)
        {
            return
                (a.NodeA == b.NodeA && a.NodeB == b.NodeB) ||
                (a.NodeA == b.NodeB && a.NodeB == b.NodeA);
        }

        private static float GetPlanarDistance(Vector3 a, Vector3 b)
        {
            Vector3 difference = b - a;
            difference.y = 0f;

            return difference.magnitude;
        }

        private void RefreshContent()
        {
            AmbientRouteNode[] nodeCandidates =
                GetComponentsInChildren<AmbientRouteNode>(true);

            AmbientRouteSegment[] segmentCandidates =
                GetComponentsInChildren<AmbientRouteSegment>(true);

            AmbientRouteGroup[] groupCandidates =
                GetComponentsInChildren<AmbientRouteGroup>(true);

            List<AmbientRouteNode> ownedNodes = new();
            List<AmbientRouteSegment> ownedSegments = new();
            List<AmbientRouteGroup> ownedGroups = new();

            foreach (AmbientRouteNode node in nodeCandidates)
            {
                if (OwnsNode(node))
                    ownedNodes.Add(node);
            }

            foreach (AmbientRouteSegment segment in segmentCandidates)
            {
                if (OwnsSegment(segment))
                    ownedSegments.Add(segment);
            }

            foreach (AmbientRouteGroup group in groupCandidates)
            {
                if (group != null &&
                    group.GetComponentInParent<AmbientRouteGraph>() == this)
                {
                    ownedGroups.Add(group);
                }
            }

            nodes = ownedNodes.ToArray();
            segments = ownedSegments.ToArray();
            groups = ownedGroups.ToArray();
        }

        private void OnValidate()
        {
            RefreshContent();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            RefreshContent();

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                    continue;

                UnityEditor.Handles.Label(
                    node.Position + Vector3.up * 0.55f,
                    node.DisplayName
                );
            }
        }
#endif
    }
}