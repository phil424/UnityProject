using UnityEngine;

namespace MiniCrawler.Spawning
{
    [DisallowMultipleComponent]
    public class LevelSpawnSource : MonoBehaviour
    {
        public enum SpawnShape
        {
            Point,
            Circle,
            Box
        }

        [SerializeField] private SpawnShape shape = SpawnShape.Point;

        [Header("Circle")]
        [SerializeField, Min(0f)] private float radius = 1.5f;

        [Header("Box")]
        [SerializeField] private Vector2 boxSize = new(3f, 3f);
        
        private bool hasRuntimeConfigurationOverride;
        private SpawnShape runtimeShape;
        private float runtimeRadius;
        private Vector2 runtimeBoxSize;

        public SpawnShape AuthoredShape => shape;
        public float AuthoredRadius => radius;
        public Vector2 AuthoredBoxSize => boxSize;

        public SpawnShape Shape => hasRuntimeConfigurationOverride ? runtimeShape : shape;
        public float Radius => hasRuntimeConfigurationOverride ? runtimeRadius : radius;
        public Vector2 BoxSize => hasRuntimeConfigurationOverride ? runtimeBoxSize : boxSize;
        
        public void SetRuntimeConfigurationOverride(SpawnShape newShape, float newRadius, Vector2 newBoxSize)
        {
            hasRuntimeConfigurationOverride = true;
            runtimeShape = newShape;
            runtimeRadius = Mathf.Max(0f, newRadius);
            runtimeBoxSize = new Vector2(
                Mathf.Max(0f, newBoxSize.x),
                Mathf.Max(0f, newBoxSize.y)
            );
        }

        public void ClearRuntimeConfigurationOverride()
        {
            hasRuntimeConfigurationOverride = false;
        }

        public Pose GetSpawnPose()
        {
            Vector3 localOffset = Shape switch
            {
                SpawnShape.Circle => GetCircleOffset(),
                SpawnShape.Box => GetBoxOffset(),
                _ => Vector3.zero
            };

            return new Pose(
                transform.TransformPoint(localOffset),
                transform.rotation
            );
        }

        private Vector3 GetCircleOffset()
        {
            Vector2 offset = Random.insideUnitCircle * Radius;
            return new Vector3(offset.x, 0f, offset.y);
        }

        private Vector3 GetBoxOffset()
        {
            Vector2 effectiveBoxSize = BoxSize;

            float halfWidth = effectiveBoxSize.x * 0.5f;
            float halfDepth = effectiveBoxSize.y * 0.5f;

            return new Vector3(
                Random.Range(-halfWidth, halfWidth),
                0f,
                Random.Range(-halfDepth, halfDepth)
            );
        }

        private void OnValidate()
        {
            radius = Mathf.Max(0f, radius);
            boxSize.x = Mathf.Max(0f, boxSize.x);
            boxSize.y = Mathf.Max(0f, boxSize.y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            switch (Shape)
            {
                case SpawnShape.Point:
                    DrawPointGizmo();
                    break;

                case SpawnShape.Circle:
                    DrawCircleGizmo();
                    break;

                case SpawnShape.Box:
                    DrawBoxGizmo();
                    break;
            }
        }

        private void DrawPointGizmo()
        {
            Gizmos.DrawWireSphere(transform.position, 0.25f);
            Gizmos.DrawLine(
                transform.position,
                transform.position + transform.forward
            );
        }

        private void DrawCircleGizmo()
        {
            const int segments = 32;

            Vector3 previous = transform.TransformPoint(new Vector3(radius, 0f, 0f));

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;

                Vector3 localPoint = new(
                    Mathf.Cos(angle) * Radius,
                    0f,
                    Mathf.Sin(angle) * Radius
                );

                Vector3 current = transform.TransformPoint(localPoint);

                Gizmos.DrawLine(previous, current);
                previous = current;
            }
        }

        private void DrawBoxGizmo()
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                Vector3.one
            );

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(BoxSize.x, 0.1f, BoxSize.y));

            Gizmos.matrix = previousMatrix;
        }
    }
}