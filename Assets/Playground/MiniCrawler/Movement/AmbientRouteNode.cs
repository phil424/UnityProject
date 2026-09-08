using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteNode : MonoBehaviour
    {
        [SerializeField] private List<AmbientRouteNode> connections = new();

        public IReadOnlyList<AmbientRouteNode> Connections => connections;
        public Vector3 Position => transform.position;

        public bool HasConnection(AmbientRouteNode node)
        {
            return node != null && connections.Contains(node);
        }

        private void OnValidate()
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i] == this)
                    connections[i] = null;
            }

            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i] == null)
                    continue;

                for (int j = 0; j < i; j++)
                {
                    if (connections[j] == null || connections[i] != connections[j])
                        continue;

                    connections[i] = null;
                    break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.35f);

            foreach (AmbientRouteNode connection in connections)
            {
                if (connection != null)
                    Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}