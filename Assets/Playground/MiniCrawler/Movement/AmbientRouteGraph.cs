using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteGraph : MonoBehaviour
    {
        private AmbientRouteNode[] nodes = Array.Empty<AmbientRouteNode>();

        public IReadOnlyList<AmbientRouteNode> Nodes => nodes;

        private void OnEnable()
        {
            RefreshNodes();
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

        public void GetNeighbours(AmbientRouteNode node, List<AmbientRouteNode> results)
        {
            results.Clear();

            if (node == null)
                return;

            foreach (AmbientRouteNode connection in node.Connections)
            {
                if (OwnsNode(connection) && !results.Contains(connection))
                    results.Add(connection);
            }

            foreach (AmbientRouteNode candidate in nodes)
            {
                if (candidate == null || candidate == node || !candidate.HasConnection(node))
                    continue;

                if (!results.Contains(candidate))
                    results.Add(candidate);
            }
        }

        private bool OwnsNode(AmbientRouteNode node)
        {
            return node != null && node.GetComponentInParent<AmbientRouteGraph>() == this;
        }

        private void RefreshNodes()
        {
            AmbientRouteNode[] candidates = GetComponentsInChildren<AmbientRouteNode>(true);
            List<AmbientRouteNode> ownedNodes = new();

            foreach (AmbientRouteNode node in candidates)
            {
                if (OwnsNode(node))
                    ownedNodes.Add(node);
            }

            nodes = ownedNodes.ToArray();
        }

        private void OnValidate()
        {
            RefreshNodes();
        }
    }
}