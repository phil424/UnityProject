using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Combat
{
    internal readonly struct ProjectileImpactContext
    {
        private ProjectileImpactContext(GameObject source, Health target, bool hasExplicitKnockbackDirection, Vector3 knockbackDirection)
        {
            Source = source;
            Target = target;
            HasExplicitKnockbackDirection = hasExplicitKnockbackDirection;
            KnockbackDirection = knockbackDirection;
        }

        public GameObject Source { get; }
        public Health Target { get; }
        public bool HasExplicitKnockbackDirection { get; }
        public Vector3 KnockbackDirection { get; }

        public static ProjectileImpactContext Targeted(GameObject source, Health target)
        {
            return new ProjectileImpactContext(source, target, false, Vector3.zero);
        }

        public static ProjectileImpactContext Directional(GameObject source, Health target, Vector3 knockbackDirection)
        {
            return new ProjectileImpactContext(source, target, true, knockbackDirection);
        }
    }

    internal static class ProjectileImpactResolver
    {
        private const float MinimumDirectionSquared = 0.0001f;

        public static void Resolve(ProjectileImpactPayload payload, ProjectileImpactContext context)
        {
            Health target = context.Target;

            if (context.Source == null || target == null || target.IsDead)
                return;

            if (payload.Damage > 0f)
                DamageResolver.ApplyDamage(context.Source, target, payload.Damage, payload.ActionName);

            if (target.IsDead)
                return;

            if (payload.Burn.IsConfigured)
            {
                BurningDamageRuntime.Apply(context.Source, target, payload.Burn.DamagePerTick, payload.Burn.TickInterval, payload.Burn.Duration, payload.Burn.ActionName);
            }

            if (target.IsDead)
                return;

            if (payload.Knockback.IsConfigured)
            {
                if (context.HasExplicitKnockbackDirection && context.KnockbackDirection.sqrMagnitude > MinimumDirectionSquared)
                {
                    KnockbackResolver.TryApplyDirectional(context.Source, target, context.KnockbackDirection, payload.Knockback.Distance, payload.Knockback.Speed);
                }
                else
                {
                    KnockbackResolver.TryApply(context.Source, target, payload.Knockback.Distance, payload.Knockback.Speed);
                }
            }

            if (target.IsDead || !payload.Stun.IsConfigured)
                return;

            StunRuntime.Apply(context.Source, target, payload.Stun.Duration, payload.Stun.ActionName);
        }

        public static Vector3 ResolveDirectionalKnockbackDirection(Health target, Vector3 closestPoint, float hitDistance, Vector3 travelDirection, float fanAngle)
        {
            travelDirection.y = 0f;

            if (travelDirection.sqrMagnitude <= MinimumDirectionSquared)
                return Vector3.zero;

            travelDirection.Normalize();

            if (target == null || fanAngle <= 0f)
                return travelDirection;

            Vector3 lateralDirection = target.transform.position - closestPoint;
            lateralDirection.y = 0f;

            float lateralDistance = lateralDirection.magnitude;

            if (lateralDistance <= MinimumDirectionSquared)
                return travelDirection;

            float spreadAmount = Mathf.Clamp01(lateralDistance / Mathf.Max(MinimumDirectionSquared, hitDistance));

            float deflection = Mathf.Clamp(fanAngle, 0f, 89f) * spreadAmount;

            return KnockbackResolver.ResolveDeflectedDirection(travelDirection, lateralDirection, deflection);
        }
    }
}
