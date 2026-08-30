using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    public static class KnockbackPropagation
    {
        public static bool TryPropagate(
            ForcedMotion sourceMotion,
            Health target,
            float distanceMultiplier,
            float speedMultiplier,
            float maxDeflectionDegrees
        )
        {
            if (
                sourceMotion == null ||
                !sourceMotion.isActiveAndEnabled ||
                !sourceMotion.PropagationAvailable ||
                target == null ||
                target.IsDead ||
                target.gameObject ==
                    sourceMotion.gameObject
            )
            {
                return false;
            }

            ForcedMotion targetMotion =
                target.GetComponent<ForcedMotion>();

            if (
                targetMotion == null ||
                targetMotion.IsActive
            )
            {
                return false;
            }

            Health sourceHealth =
                sourceMotion.GetComponent<Health>();

            if (
                sourceHealth != null &&
                sourceHealth.IsDead
            )
            {
                return false;
            }

            distanceMultiplier =
                Mathf.Clamp01(
                    distanceMultiplier
                );

            speedMultiplier =
                Mathf.Clamp01(
                    speedMultiplier
                );

            if (
                distanceMultiplier <= 0f ||
                speedMultiplier <= 0f
            )
            {
                return false;
            }

            float propagatedDistance =
                sourceMotion.RemainingDistance *
                distanceMultiplier;

            float propagatedSpeed =
                sourceMotion.Speed *
                speedMultiplier;

            Vector3 outwardDirection =
                target.transform.position -
                sourceMotion.transform.position;

            Vector3 propagatedDirection =
                KnockbackResolver
                    .ResolveDeflectedDirection(
                        sourceMotion.Direction,
                        outwardDirection,
                        maxDeflectionDegrees
                    );

            bool applied =
                KnockbackResolver
                    .TryApplyDirectional(
                        sourceMotion.gameObject,
                        target,
                        propagatedDirection,
                        propagatedDistance,
                        propagatedSpeed,
                        false
                    );

            if (!applied)
                return false;

            sourceMotion
                .TryConsumePropagation();

            return true;
        }
    }
}