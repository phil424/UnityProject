using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class ForcedMotion : MonoBehaviour
    {
        private Vector3 direction;
        private float remainingDistance;
        private float speed;
        private bool propagationAvailable;

        public bool IsActive =>
            remainingDistance > 0f &&
            speed > 0f &&
            direction.sqrMagnitude > 0f;

        public Vector3 Direction =>
            direction;

        public float RemainingDistance =>
            remainingDistance;

        public float Speed =>
            speed;

        public bool PropagationAvailable =>
            IsActive &&
            propagationAvailable;

        public bool StartMotion(
            Vector3 worldDirection,
            float distance,
            float unitsPerSecond,
            bool canPropagate = true
        )
        {
            worldDirection.y = 0f;

            if (
                worldDirection.sqrMagnitude <= 0f ||
                distance <= 0f ||
                unitsPerSecond <= 0f
            )
            {
                return false;
            }

            // Forced motion follows
            // "newest valid request wins" semantics.
            direction =
                worldDirection.normalized;

            remainingDistance =
                distance;

            speed =
                unitsPerSecond;

            propagationAvailable =
                canPropagate;

            return true;
        }

        public Vector3 ConsumeDisplacement(
            float deltaTime
        )
        {
            if (
                !IsActive ||
                deltaTime <= 0f
            )
            {
                return Vector3.zero;
            }

            float displacementDistance =
                Mathf.Min(
                    speed * deltaTime,
                    remainingDistance
                );

            Vector3 displacement =
                direction * displacementDistance;

            remainingDistance =
                Mathf.Max(
                    0f,
                    remainingDistance -
                    displacementDistance
                );

            if (remainingDistance <= 0f)
                Clear();

            return displacement;
        }

        public bool TryConsumePropagation()
        {
            if (!PropagationAvailable)
                return false;

            propagationAvailable =
                false;

            return true;
        }

        public void Clear()
        {
            direction =
                Vector3.zero;

            remainingDistance =
                0f;

            speed =
                0f;

            propagationAvailable =
                false;
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}