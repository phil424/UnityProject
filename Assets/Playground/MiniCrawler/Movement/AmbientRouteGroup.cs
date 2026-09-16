using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteGroup : MonoBehaviour
    {
        [SerializeField] private List<AmbientRouteNode> nodes = new();
        [SerializeField] private List<AmbientRouteSegment> segments = new();
        [SerializeField] private List<RouteTagDefinition> tags = new();

        public string DisplayName => gameObject.name;
        public IReadOnlyList<AmbientRouteNode> Nodes => nodes;
        public IReadOnlyList<AmbientRouteSegment> Segments => segments;
        public IReadOnlyList<RouteTagDefinition> Tags => tags;

        public bool ContainsNode(AmbientRouteNode node)
        {
            return node != null && nodes.Contains(node);
        }

        public bool ContainsSegment(AmbientRouteSegment segment)
        {
            return segment != null && segments.Contains(segment);
        }

        public bool HasTag(RouteTagDefinition tag)
        {
            return tag != null && tags.Contains(tag);
        }

        private Vector3 GetCentroid()
        {
            Vector3 total = Vector3.zero;
            int count = 0;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                    continue;

                total += node.Position;
                count++;
            }

            return count > 0 ? total / count : transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null)
                    Gizmos.DrawWireSphere(node.Position, 0.6f);
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null && segment.IsConfigured)
                    Gizmos.DrawLine(segment.NodeA.Position + Vector3.up * 0.1f, segment.NodeB.Position + Vector3.up * 0.1f);
            }

#if UNITY_EDITOR
            UnityEditor.Handles.Label(GetCentroid() + Vector3.up * 0.85f, DisplayName);
#endif
        }
    }
}