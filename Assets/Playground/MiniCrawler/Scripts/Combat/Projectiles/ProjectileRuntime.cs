using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public sealed class ProjectileRuntime : MonoBehaviour
    {
        private const float MinimumDirectionSquared = 0.0001f;

        private enum TravelMode
        {
            Targeted,
            Directional
        }

        private readonly struct HitCandidate
        {
            public HitCandidate(Health target, float alongDistance, float hitDistance)
            {
                Target = target;
                AlongDistance = alongDistance;
                HitDistance = hitDistance;
            }

            public Health Target { get; }
            public float AlongDistance { get; }
            public float HitDistance { get; }
        }

        private readonly HashSet<Health> hitTargets = new();
        private readonly List<HitCandidate> hitCandidates = new();

        private ProjectileDefinition definition;
        private TravelMode travelMode;

        private GameObject source;
        private Health target;
        private float damage;
        private string actionName;
        private float remainingLifetime;

        private float remainingDistance;
        private string targetFactionId;
        private float directionalHitRadius;
        private int remainingHits;
        private float knockbackDistance;
        private float knockbackSpeed;
        private float knockbackFanAngle;

        public Vector3 TravelDirection { get; private set; } = Vector3.right;

        public static ProjectileRuntime Spawn(
            ProjectileDefinition definition,
            GameObject source,
            Health target,
            float damage,
            string actionName)
        {
            if (definition == null || source == null || target == null || target.IsDead || damage <= 0f)
                return null;

            if (definition.Prefab == null)
            {
                Debug.LogError($"Projectile definition '{definition.name}' has no prefab assigned.", definition);
                return null;
            }

            Vector3 direction = target.transform.position - source.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= MinimumDirectionSquared)
                direction = source.transform.forward;

            direction.Normalize();

            Vector3 spawnPosition = source.transform.position + direction * definition.SourceForwardOffset;
            ProjectileRuntime runtime = CreateRuntime(definition, spawnPosition);

            if (runtime == null)
                return null;

            runtime.InitializeTargeted(definition, source, target, damage, actionName, direction);
            return runtime;
        }

        public static ProjectileRuntime SpawnDirectional(
            ProjectileDefinition definition,
            GameObject source,
            Vector3 direction,
            float damage,
            string actionName,
            float travelDistance,
            string targetFactionId,
            float hitRadius,
            int maxHits,
            float knockbackDistance,
            float knockbackSpeed,
            float knockbackFanAngle)
        {
            direction.y = 0f;

            if (definition == null ||
                source == null ||
                direction.sqrMagnitude <= MinimumDirectionSquared ||
                damage <= 0f ||
                travelDistance <= 0f ||
                string.IsNullOrWhiteSpace(targetFactionId) ||
                maxHits <= 0)
            {
                return null;
            }

            if (definition.Prefab == null)
            {
                Debug.LogError($"Projectile definition '{definition.name}' has no prefab assigned.", definition);
                return null;
            }

            direction.Normalize();

            Vector3 spawnPosition = source.transform.position + direction * definition.SourceForwardOffset;
            ProjectileRuntime runtime = CreateRuntime(definition, spawnPosition);

            if (runtime == null)
                return null;

            runtime.InitializeDirectional(
                definition,
                source,
                direction,
                damage,
                actionName,
                travelDistance,
                targetFactionId,
                hitRadius,
                maxHits,
                knockbackDistance,
                knockbackSpeed,
                knockbackFanAngle
            );

            return runtime;
        }

        private static ProjectileRuntime CreateRuntime(ProjectileDefinition definition, Vector3 spawnPosition)
        {
            GameObject projectileObject = Instantiate(definition.Prefab, spawnPosition, Quaternion.identity);
            ProjectileRuntime runtime = projectileObject.GetComponent<ProjectileRuntime>();

            if (runtime != null)
                return runtime;

            Debug.LogError(
                $"Projectile prefab '{definition.Prefab.name}' requires {nameof(ProjectileRuntime)}.",
                projectileObject
            );

            Destroy(projectileObject);
            return null;
        }

        private void InitializeTargeted(
            ProjectileDefinition projectileDefinition,
            GameObject projectileSource,
            Health projectileTarget,
            float projectileDamage,
            string projectileActionName,
            Vector3 initialDirection)
        {
            definition = projectileDefinition;
            travelMode = TravelMode.Targeted;

            source = projectileSource;
            target = projectileTarget;
            damage = projectileDamage;
            actionName = projectileActionName;
            remainingLifetime = definition.MaxLifetime;

            TravelDirection = initialDirection.normalized;
        }

        private void InitializeDirectional(
            ProjectileDefinition projectileDefinition,
            GameObject projectileSource,
            Vector3 direction,
            float projectileDamage,
            string projectileActionName,
            float travelDistance,
            string factionId,
            float hitRadius,
            int maxHits,
            float outgoingKnockbackDistance,
            float outgoingKnockbackSpeed,
            float fanAngle)
        {
            definition = projectileDefinition;
            travelMode = TravelMode.Directional;

            source = projectileSource;
            damage = projectileDamage;
            actionName = projectileActionName;
            remainingLifetime = definition.MaxLifetime;

            TravelDirection = direction.normalized;

            remainingDistance = Mathf.Max(0f, travelDistance);
            targetFactionId = factionId;
            directionalHitRadius = Mathf.Max(0.05f, hitRadius);
            remainingHits = Mathf.Max(1, maxHits);

            knockbackDistance = Mathf.Max(0f, outgoingKnockbackDistance);
            knockbackSpeed = Mathf.Max(0f, outgoingKnockbackSpeed);
            knockbackFanAngle = Mathf.Clamp(fanAngle, 0f, 89f);

            hitTargets.Clear();
        }

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            if (definition == null)
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

            switch (travelMode)
            {
                case TravelMode.Targeted:
                    TickTargeted();
                    break;

                case TravelMode.Directional:
                    TickDirectional();
                    break;
            }
        }

        private void TickTargeted()
        {
            if (target == null || target.IsDead)
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
                ResolveTargetedHit();
                return;
            }

            TravelDirection = toTarget / distance;

            float travelDistance = definition.Speed * Time.deltaTime;

            if (travelDistance >= distance - definition.HitDistance)
            {
                transform.position = targetPosition;
                ResolveTargetedHit();
                return;
            }

            transform.position += TravelDirection * travelDistance;
        }

        private void TickDirectional()
        {
            if (remainingDistance <= 0f || remainingHits <= 0)
            {
                Destroy(gameObject);
                return;
            }

            float stepDistance = Mathf.Min(definition.Speed * Time.deltaTime, remainingDistance);

            if (stepDistance <= 0f)
                return;

            Vector3 segmentStart = transform.position;
            Vector3 segmentEnd = segmentStart + TravelDirection * stepDistance;

            bool exhaustedHits = ResolveDirectionalHits(segmentStart, segmentEnd);

            transform.position = segmentEnd;
            remainingDistance = Mathf.Max(0f, remainingDistance - stepDistance);

            if (exhaustedHits || remainingDistance <= 0f)
                Destroy(gameObject);
        }

        private void ResolveTargetedHit()
        {
            if (target != null && !target.IsDead)
                DamageResolver.ApplyDamage(source, target, damage, actionName);

            Destroy(gameObject);
        }

        private bool ResolveDirectionalHits(Vector3 segmentStart, Vector3 segmentEnd)
        {
            hitCandidates.Clear();

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health possibleTarget in possibleTargets)
            {
                if (!IsValidDirectionalTarget(possibleTarget))
                    continue;

                float targetRadius = 0f;
                EntityAvoidance avoidance = possibleTarget.GetComponent<EntityAvoidance>();

                if (avoidance != null)
                    targetRadius = avoidance.WorldRadius;

                float hitDistance = directionalHitRadius + targetRadius;
                Vector3 closestPoint = ClosestPointOnSegment(possibleTarget.transform.position, segmentStart, segmentEnd);
                Vector3 offset = possibleTarget.transform.position - closestPoint;
                offset.y = 0f;

                if (offset.sqrMagnitude > hitDistance * hitDistance)
                    continue;

                float alongDistance = Vector3.Dot(closestPoint - segmentStart, TravelDirection);
                hitCandidates.Add(new HitCandidate(possibleTarget, alongDistance, hitDistance));
            }

            hitCandidates.Sort((a, b) => a.AlongDistance.CompareTo(b.AlongDistance));

            foreach (HitCandidate candidate in hitCandidates)
            {
                Health hitTarget = candidate.Target;

                if (!IsValidDirectionalTarget(hitTarget))
                    continue;

                hitTargets.Add(hitTarget);

                DamageResolver.ApplyDamage(source, hitTarget, damage, actionName);

                if (!hitTarget.IsDead && knockbackDistance > 0f && knockbackSpeed > 0f)
                {
                    Vector3 knockbackDirection = ResolveKnockbackDirection(
                        hitTarget,
                        segmentStart,
                        segmentEnd,
                        candidate.HitDistance
                    );

                    KnockbackResolver.TryApplyDirectional(
                        source,
                        hitTarget,
                        knockbackDirection,
                        knockbackDistance,
                        knockbackSpeed
                    );
                }

                remainingHits--;

                if (remainingHits <= 0)
                    return true;
            }

            return false;
        }

        private bool IsValidDirectionalTarget(Health possibleTarget)
        {
            if (possibleTarget == null ||
                possibleTarget.IsDead ||
                possibleTarget.gameObject == source ||
                hitTargets.Contains(possibleTarget))
            {
                return false;
            }

            FactionMember faction = possibleTarget.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(targetFactionId);
        }

        private Vector3 ResolveKnockbackDirection(
            Health hitTarget,
            Vector3 segmentStart,
            Vector3 segmentEnd,
            float hitDistance)
        {
            Vector3 closestPoint = ClosestPointOnSegment(hitTarget.transform.position, segmentStart, segmentEnd);
            Vector3 lateralDirection = hitTarget.transform.position - closestPoint;
            lateralDirection.y = 0f;

            float lateralDistance = lateralDirection.magnitude;

            if (lateralDistance <= MinimumDirectionSquared || knockbackFanAngle <= 0f)
                return TravelDirection;

            float spreadAmount = Mathf.Clamp01(lateralDistance / Mathf.Max(MinimumDirectionSquared, hitDistance));
            float deflection = knockbackFanAngle * spreadAmount;

            return KnockbackResolver.ResolveDeflectedDirection(TravelDirection, lateralDirection, deflection);
        }

        private static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            point.y = 0f;
            start.y = 0f;
            end.y = 0f;

            Vector3 segment = end - start;
            float lengthSquared = segment.sqrMagnitude;

            if (lengthSquared <= MinimumDirectionSquared)
                return start;

            float t = Vector3.Dot(point - start, segment) / lengthSquared;
            return start + segment * Mathf.Clamp01(t);
        }
    }
}