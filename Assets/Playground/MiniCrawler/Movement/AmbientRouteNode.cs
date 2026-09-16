using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteNode : MonoBehaviour
    {
        [SerializeField] private List<RouteTagDefinition> tags = new();

        public string DisplayName => gameObject.name;
        public Vector3 Position => transform.position;
        public IReadOnlyList<RouteTagDefinition> Tags => tags;

        public bool HasTag(RouteTagDefinition tag)
        {
            return tag != null && tags.Contains(tag);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.55f, DisplayName);
        }
#endif
    }
}