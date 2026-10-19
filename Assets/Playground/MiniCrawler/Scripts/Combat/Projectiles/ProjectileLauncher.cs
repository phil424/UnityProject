using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public static class ProjectileLauncher
    {
        private const float MinimumDirectionSquared = 0.0001f;

        public static ProjectileRuntime Launch(ProjectileLaunchRequest request)
        {
            if (!request.IsConfigured)
                return null;

            request = ResolveBasicAttackPenetration(request);

            if (!request.IsConfigured)
                return null;

            ProjectileDefinition definition = request.Definition;

            if (definition.Prefab == null)
            {
                Debug.LogError($"Projectile definition " + $"'{definition.name}' " + $"has no prefab assigned.", definition);

                return null;
            }

            if (!TryResolveInitialDirection(request, out Vector3 direction))
            {
                return null;
            }

            Vector3 spawnPosition = request.Source.transform.position + direction * definition.SourceForwardOffset;

            GameObject projectileObject = Object.Instantiate(definition.Prefab, spawnPosition, Quaternion.identity);

            ProjectileRuntime runtime = projectileObject.GetComponent<ProjectileRuntime>();

            if (runtime == null)
            {
                Debug.LogError($"Projectile prefab " + $"'{definition.Prefab.name}' " + $"requires " + $"{nameof(ProjectileRuntime)}.", projectileObject);

                Object.Destroy(projectileObject);

                return null;
            }

            runtime.Initialize(request, direction);

            return runtime;
        }

        private static ProjectileLaunchRequest ResolveBasicAttackPenetration(ProjectileLaunchRequest request)
        {
            if (!request.IsBasicAttack || request.Source == null)
            {
                return request;
            }

            CombatStats stats = request.Source.GetComponent<CombatStats>();

            if (stats == null || stats.BasicAttackPenetrationBonus <= 0)
            {
                return request;
            }

            int additionalTargets = stats.BasicAttackPenetrationBonus;

            ProjectileDelivery delivery = request.Delivery;

            switch (delivery.Mode)
            {
                case ProjectileDeliveryMode.Targeted:
                {
                    AutoTargetMover mover = request.Source.GetComponent<AutoTargetMover>();

                    if (mover == null || string.IsNullOrWhiteSpace(mover.TargetFactionId))
                    {
                        return request;
                    }

                    float hitRadius = Mathf.Max(0.05f, request.Definition.HitDistance);

                    delivery = ProjectileDelivery.TargetedPiercing(delivery.Target, stats.AttackRange, mover.TargetFactionId, hitRadius, 1 + additionalTargets);

                    break;
                }

                case ProjectileDeliveryMode.TargetedPiercing:
                    delivery = ProjectileDelivery.TargetedPiercing(delivery.Target, delivery.TravelDistance, delivery.TargetFactionId, delivery.HitRadius, delivery.MaximumHits + additionalTargets);

                    break;

                case ProjectileDeliveryMode.Directional:
                    delivery = ProjectileDelivery.Directional(delivery.Direction, delivery.TravelDistance, delivery.TargetFactionId, delivery.HitRadius, delivery.MaximumHits + additionalTargets);

                    break;
            }

            return request.WithDelivery(delivery);
        }

        private static bool TryResolveInitialDirection(ProjectileLaunchRequest request, out Vector3 direction)
        {
            direction = Vector3.zero;

            switch (request.Delivery.Mode)
            {
                case ProjectileDeliveryMode.Targeted:
                case ProjectileDeliveryMode.TargetedPiercing:
                {
                    if (request.Delivery.Target == null || request.Delivery.Target.IsDead)
                    {
                        return false;
                    }

                    direction = request.Delivery.Target.transform.position - request.Source.transform.position;

                    direction.y = 0f;

                    if (direction.sqrMagnitude <= MinimumDirectionSquared)
                    {
                        direction = request.Source.transform.forward;

                        direction.y = 0f;
                    }

                    break;
                }

                case ProjectileDeliveryMode.Directional:
                    direction = request.Delivery.Direction;

                    direction.y = 0f;
                    break;

                default:
                    return false;
            }

            if (direction.sqrMagnitude <= MinimumDirectionSquared)
            {
                return false;
            }

            direction.Normalize();

            return true;
        }
    }
}
