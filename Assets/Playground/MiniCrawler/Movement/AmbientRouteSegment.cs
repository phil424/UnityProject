using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteSegment : MonoBehaviour
    {
        [SerializeField] private AmbientRouteNode nodeA;
        [SerializeField] private AmbientRouteNode nodeB;

        public string DisplayName => gameObject.name;
        public AmbientRouteNode NodeA => nodeA;
        public AmbientRouteNode NodeB => nodeB;

        public bool IsConfigured =>
            nodeA != null &&
            nodeB != null &&
            nodeA != nodeB;

        public float Length
        {
            get
            {
                if (!IsConfigured)
                    return 0f;

                Vector3 difference = nodeB.Position - nodeA.Position;
                difference.y = 0f;

                return difference.magnitude;
            }
        }

        public Vector3 Midpoint =>
            IsConfigured
                ? (nodeA.Position + nodeB.Position) * 0.5f
                : transform.position;

        public bool Connects(AmbientRouteNode node)
        {
            return node != null &&
                   (node == nodeA || node == nodeB);
        }

        public AmbientRouteNode GetOther(AmbientRouteNode node)
        {
            if (node == nodeA)
                return nodeB;

            if (node == nodeB)
                return nodeA;

            return null;
        }

        private void OnDrawGizmos()
        {
            if (!IsConfigured)
                return;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(nodeA.Position, nodeB.Position);
            Gizmos.DrawWireSphere(Midpoint, 0.12f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!IsConfigured)
                return;

            UnityEditor.Handles.Label(
                Midpoint + Vector3.up * 0.35f,
                DisplayName
            );
        }
#endif
    }
}