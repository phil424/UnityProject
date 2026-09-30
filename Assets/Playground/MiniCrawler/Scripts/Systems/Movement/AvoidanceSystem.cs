using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-100)]
    public class AvoidanceSystem : MonoBehaviour
    {
        private const float MinimumDistanceSquared =
            0.0001f;

        [Header("Knockback Propagation")]
        [SerializeField]
        private bool enableKnockbackPropagation = true;

        [SerializeField]
        [Range(0f, 1f)]
        private float propagationDistanceMultiplier =
            0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float propagationSpeedMultiplier =
            0.5f;

        [SerializeField]
        [Range(0f, 89f)]
        private float propagationDeflectionDegrees =
            35f;

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            EntityAvoidance[] entities =
                FindObjectsByType<EntityAvoidance>(
                    FindObjectsSortMode.None
                );

            for (
                int firstIndex = 0;
                firstIndex < entities.Length;
                firstIndex++
            )
            {
                EntityAvoidance first =
                    entities[firstIndex];

                if (!CanSeparate(first))
                    continue;

                for (
                    int secondIndex =
                        firstIndex + 1;
                    secondIndex < entities.Length;
                    secondIndex++
                )
                {
                    EntityAvoidance second =
                        entities[secondIndex];

                    if (!CanSeparate(second))
                        continue;

                    SeparatePair(
                        first,
                        second
                    );
                }
            }
        }

        private bool CanSeparate(
            EntityAvoidance entity
        )
        {
            if (
                entity == null ||
                !entity.isActiveAndEnabled
            )
            {
                return false;
            }

            Health health =
                entity.GetComponent<Health>();

            return
                health == null ||
                !health.IsDead;
        }

        private void SeparatePair(
            EntityAvoidance first,
            EntityAvoidance second
        )
        {
            Vector3 firstPosition =
                first.transform.position;

            Vector3 secondPosition =
                second.transform.position;

            Vector3 offset =
                secondPosition -
                firstPosition;

            offset.y = 0f;

            float requiredDistance =
                first.WorldRadius +
                second.WorldRadius;

            float distanceSquared =
                offset.sqrMagnitude;

            if (
                distanceSquared >=
                requiredDistance *
                requiredDistance
            )
            {
                return;
            }

            Vector3 direction;
            float distance;

            if (
                distanceSquared <=
                MinimumDistanceSquared
            )
            {
                direction =
                    GetFallbackDirection(
                        first,
                        second
                    );

                distance = 0f;
            }
            else
            {
                distance =
                    Mathf.Sqrt(
                        distanceSquared
                    );

                direction =
                    offset / distance;
            }

            float overlap =
                requiredDistance -
                distance;

            float separationSpeed =
                Mathf.Min(
                    first.SeparationSpeed,
                    second.SeparationSpeed
                );

            float correctionDistance =
                Mathf.Min(
                    overlap,
                    separationSpeed *
                    Time.deltaTime
                );

            bool firstIsForced =
                HasActiveForcedMotion(first);

            bool secondIsForced =
                HasActiveForcedMotion(second);

            ApplyCorrection(
                first,
                second,
                firstPosition,
                secondPosition,
                direction,
                correctionDistance,
                firstIsForced,
                secondIsForced
            );

            TryPropagateKnockback(
                first,
                second,
                firstIsForced,
                secondIsForced
            );
        }

        private void ApplyCorrection(
            EntityAvoidance first,
            EntityAvoidance second,
            Vector3 firstPosition,
            Vector3 secondPosition,
            Vector3 direction,
            float correctionDistance,
            bool firstIsForced,
            bool secondIsForced
        )
        {
            if (
                firstIsForced &&
                !secondIsForced
            )
            {
                second.transform.position =
                    secondPosition +
                    direction *
                    correctionDistance;

                return;
            }

            if (
                !firstIsForced &&
                secondIsForced
            )
            {
                first.transform.position =
                    firstPosition -
                    direction *
                    correctionDistance;

                return;
            }

            Vector3 sharedCorrection =
                direction *
                (correctionDistance * 0.5f);

            first.transform.position =
                firstPosition -
                sharedCorrection;

            second.transform.position =
                secondPosition +
                sharedCorrection;
        }

        private void TryPropagateKnockback(
            EntityAvoidance first,
            EntityAvoidance second,
            bool firstIsForced,
            bool secondIsForced
        )
        {
            if (
                !enableKnockbackPropagation ||
                firstIsForced ==
                secondIsForced
            )
            {
                return;
            }

            EntityAvoidance source =
                firstIsForced
                    ? first
                    : second;

            EntityAvoidance target =
                firstIsForced
                    ? second
                    : first;

            ForcedMotion sourceMotion =
                source.GetComponent<ForcedMotion>();

            Health targetHealth =
                target.GetComponent<Health>();

            KnockbackPropagation.TryPropagate(
                sourceMotion,
                targetHealth,
                propagationDistanceMultiplier,
                propagationSpeedMultiplier,
                propagationDeflectionDegrees
            );
        }

        private bool HasActiveForcedMotion(
            EntityAvoidance entity
        )
        {
            ForcedMotion forcedMotion =
                entity.GetComponent<ForcedMotion>();

            return
                forcedMotion != null &&
                forcedMotion.isActiveAndEnabled &&
                forcedMotion.IsActive;
        }

        private Vector3 GetFallbackDirection(
            EntityAvoidance first,
            EntityAvoidance second
        )
        {
            int combinedId =
                first.GetInstanceID() ^
                second.GetInstanceID();

            return
                (combinedId & 1) == 0
                    ? Vector3.right
                    : Vector3.forward;
        }

        private void OnValidate()
        {
            propagationDistanceMultiplier =
                Mathf.Clamp01(
                    propagationDistanceMultiplier
                );

            propagationSpeedMultiplier =
                Mathf.Clamp01(
                    propagationSpeedMultiplier
                );

            propagationDeflectionDegrees =
                Mathf.Clamp(
                    propagationDeflectionDegrees,
                    0f,
                    89f
                );
        }
    }
}