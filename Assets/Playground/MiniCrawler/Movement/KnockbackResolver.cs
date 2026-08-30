using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    public static class KnockbackResolver
    {
        private const float MinimumDirectionSquared =
            0.0001f;

        public static bool TryApply(
            GameObject source,
            Health target,
            float distance,
            float speed
        )
        {
            if (
                source == null ||
                target == null ||
                target.IsDead ||
                distance <= 0f ||
                speed <= 0f
            )
            {
                return false;
            }

            Vector3 direction =
                ResolveDirection(
                    source,
                    target
                );

            return TryApplyDirectional(
                source,
                target,
                direction,
                distance,
                speed
            );
        }

        public static bool TryApplyDirectional(
            GameObject source,
            Health target,
            Vector3 worldDirection,
            float distance,
            float speed,
            bool canPropagate = true
        )
        {
            if (
                source == null ||
                target == null ||
                target.IsDead ||
                distance <= 0f ||
                speed <= 0f
            )
            {
                return false;
            }

            worldDirection.y = 0f;

            if (
                worldDirection.sqrMagnitude <=
                MinimumDirectionSquared
            )
            {
                return false;
            }

            ForcedMotion forcedMotion =
                target.GetComponent<ForcedMotion>();

            if (forcedMotion == null)
                return false;

            return forcedMotion.StartMotion(
                worldDirection,
                distance,
                speed,
                canPropagate
            );
        }

        public static Vector3
            ResolveDeflectedDirection(
                Vector3 baseDirection,
                Vector3 outwardDirection,
                float maxDeflectionDegrees
            )
        {
            baseDirection.y = 0f;
            outwardDirection.y = 0f;

            if (
                baseDirection.sqrMagnitude <=
                MinimumDirectionSquared
            )
            {
                return Vector3.zero;
            }

            Vector3 normalizedBase =
                baseDirection.normalized;

            if (
                outwardDirection.sqrMagnitude <=
                    MinimumDirectionSquared ||
                maxDeflectionDegrees <= 0f
            )
            {
                return normalizedBase;
            }

            Vector3 normalizedOutward =
                outwardDirection.normalized;

            float signedAngle =
                Vector3.SignedAngle(
                    normalizedBase,
                    normalizedOutward,
                    Vector3.up
                );

            float maximumAngle =
                Mathf.Clamp(
                    maxDeflectionDegrees,
                    0f,
                    180f
                );

            float deflectedAngle =
                Mathf.Clamp(
                    signedAngle,
                    -maximumAngle,
                    maximumAngle
                );

            return
                Quaternion.AngleAxis(
                    deflectedAngle,
                    Vector3.up
                ) *
                normalizedBase;
        }

        private static Vector3 ResolveDirection(
            GameObject source,
            Health target
        )
        {
            Vector3 direction =
                target.transform.position -
                source.transform.position;

            direction.y = 0f;

            if (
                direction.sqrMagnitude >
                MinimumDirectionSquared
            )
            {
                return direction.normalized;
            }

            direction =
                source.transform.forward;

            direction.y = 0f;

            if (
                direction.sqrMagnitude >
                MinimumDirectionSquared
            )
            {
                return direction.normalized;
            }

            return Vector3.forward;
        }
    }
}