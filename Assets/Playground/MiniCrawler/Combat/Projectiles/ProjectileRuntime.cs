using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public sealed class ProjectileRuntime : MonoBehaviour
    {
        private const float MinimumDirectionSquared = 0.0001f;

        private ProjectileDefinition definition;
        private GameObject source;
        private Health target;
        private float damage;
        private string actionName;
        private float remainingLifetime;

        public Vector3 TravelDirection { get; private set; } = Vector3.right;

        public static ProjectileRuntime Spawn(
            ProjectileDefinition definition,
            GameObject source,
            Health target,
            float damage,
            string actionName)
        {
            if (definition == null ||
                definition.Prefab == null ||
                source == null ||
                target == null ||
                target.IsDead ||
                damage <= 0f)
            {
                return null;
            }

            Vector3 direction = target.transform.position - source.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= MinimumDirectionSquared)
                direction = source.transform.forward;

            Vector3 spawnPosition =
                source.transform.position + direction.normalized * definition.SourceForwardOffset;

            GameObject projectileObject =
                Instantiate(definition.Prefab, spawnPosition, Quaternion.identity);

            ProjectileRuntime runtime = projectileObject.GetComponent<ProjectileRuntime>();

            if (runtime == null)
            {
                Debug.LogError(
                    $"Projectile prefab '{definition.Prefab.name}' requires {nameof(ProjectileRuntime)}.",
                    projectileObject
                );

                Destroy(projectileObject);
                return null;
            }

            runtime.Initialize(definition, source, target, damage, actionName, direction);
            return runtime;
        }

        private void Initialize(
            ProjectileDefinition projectileDefinition,
            GameObject projectileSource,
            Health projectileTarget,
            float projectileDamage,
            string projectileActionName,
            Vector3 initialDirection)
        {
            definition = projectileDefinition;
            source = projectileSource;
            target = projectileTarget;
            damage = projectileDamage;
            actionName = projectileActionName;
            remainingLifetime = definition.MaxLifetime;

            initialDirection.y = 0f;

            if (initialDirection.sqrMagnitude > MinimumDirectionSquared)
                TravelDirection = initialDirection.normalized;
        }

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            if (definition == null || target == null || target.IsDead)
            {
                Destroy(gameObject);
                return;
            }

            remainingLifetime -= Time.deltaTime;

            if (remainingLifetime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;

            Vector3 toTarget = targetPosition - transform.position;
            float distance = toTarget.magnitude;

            if (distance <= definition.HitDistance)
            {
                ResolveHit();
                return;
            }

            TravelDirection = toTarget / distance;

            float travelDistance = definition.Speed * Time.deltaTime;

            if (travelDistance >= distance - definition.HitDistance)
            {
                transform.position = targetPosition;
                ResolveHit();
                return;
            }

            transform.position += TravelDirection * travelDistance;
        }

        private void ResolveHit()
        {
            if (target != null && !target.IsDead)
                DamageResolver.ApplyDamage(source, target, damage, actionName);

            Destroy(gameObject);
        }
    }
}