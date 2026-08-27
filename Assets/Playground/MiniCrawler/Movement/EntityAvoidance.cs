using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class EntityAvoidance : MonoBehaviour
    {
        [SerializeField]
        private float radius = 0.45f;

        [SerializeField]
        private float separationSpeed = 4f;

        public float Radius =>
            radius;

        public float WorldRadius
        {
            get
            {
                Vector3 worldScale =
                    transform.lossyScale;

                float horizontalScale =
                    Mathf.Max(
                        Mathf.Abs(worldScale.x),
                        Mathf.Abs(worldScale.z)
                    );

                return
                    radius * horizontalScale;
            }
        }

        public float SeparationSpeed =>
            separationSpeed;

        private void OnValidate()
        {
            radius =
                Mathf.Max(
                    0.05f,
                    radius
                );

            separationSpeed =
                Mathf.Max(
                    0.1f,
                    separationSpeed
                );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                WorldRadius
            );
        }
    }
}