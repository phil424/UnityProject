using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public sealed class ProjectileRuntime : MonoBehaviour
    {
        private readonly HashSet<Health> hitTargets = new();
        private readonly List<ProjectileHitCandidate> hitCandidates = new();

        private ProjectileDefinition definition;
        private ProjectileDelivery delivery;
        private ProjectileImpactPayload impact;
        private ProjectileDeliveryMode travelMode;

        private GameObject source;
        private Health target;

        private float remainingLifetime;
        private float remainingDistance;
        private int remainingHits;

        public Vector3 TravelDirection { get; private set; } = Vector3.right;

        internal void Initialize(ProjectileLaunchRequest request, Vector3 initialDirection)
        {
            definition = request.Definition;
            delivery = request.Delivery;
            impact = request.Impact;
            travelMode = delivery.Mode;

            source = request.Source;
            target = delivery.Target;

            remainingLifetime = definition != null ? definition.MaxLifetime : 0f;

            remainingDistance = Mathf.Max(0f, delivery.TravelDistance);

            remainingHits = Mathf.Max(1, delivery.MaximumHits);

            TravelDirection = initialDirection.sqrMagnitude > 0f ? initialDirection.normalized : Vector3.right;

            hitTargets.Clear();
            hitCandidates.Clear();
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
                case ProjectileDeliveryMode.Targeted:
                    TickTargeted();
                    break;

                case ProjectileDeliveryMode.TargetedPiercing:
                    TickTargetedPiercing();
                    break;

                case ProjectileDeliveryMode.Directional:
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

        private void TickTargetedPiercing()
        {
            if (remainingDistance <= 0f || remainingHits <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (target == null || target.IsDead)
            {
                target = null;
                travelMode = ProjectileDeliveryMode.Directional;
                return;
            }

            Vector3 targetPosition = target.transform.position;

            targetPosition.y = transform.position.y;

            Vector3 toTarget = targetPosition - transform.position;

            float distance = toTarget.magnitude;

            if (distance <= definition.HitDistance)
            {
                ResolveTargetedPiercingHit();
                return;
            }

            TravelDirection = toTarget / distance;

            float stepDistance = Mathf.Min(definition.Speed * Time.deltaTime, remainingDistance);

            if (stepDistance <= 0f)
                return;

            if (stepDistance >= distance - definition.HitDistance)
            {
                transform.position = targetPosition;

                remainingDistance = Mathf.Max(0f, remainingDistance - distance);

                ResolveTargetedPiercingHit();
                return;
            }

            transform.position += TravelDirection * stepDistance;

            remainingDistance = Mathf.Max(0f, remainingDistance - stepDistance);

            if (remainingDistance <= 0f)
                Destroy(gameObject);
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
                ProjectileImpactResolver.Resolve(impact, ProjectileImpactContext.Targeted(source, target));

            Destroy(gameObject);
        }

        private void ResolveTargetedPiercingHit()
        {
            if (target != null && !target.IsDead && !hitTargets.Contains(target))
            {
                Health hitTarget = target;

                hitTargets.Add(hitTarget);

                ProjectileImpactResolver.Resolve(impact, ProjectileImpactContext.Targeted(source, hitTarget));

                remainingHits--;
            }

            target = null;

            if (remainingHits <= 0 || remainingDistance <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            travelMode = ProjectileDeliveryMode.Directional;
        }

        private bool ResolveDirectionalHits(Vector3 segmentStart, Vector3 segmentEnd)
        {
            ProjectileHitQuery.CollectSegmentTargets(source, delivery.TargetFactionId, hitTargets, TravelDirection, segmentStart, segmentEnd, delivery.HitRadius, hitCandidates);

            foreach (ProjectileHitCandidate candidate in hitCandidates)
            {
                Health hitTarget = candidate.Target;

                if (hitTarget == null || hitTarget.IsDead || hitTargets.Contains(hitTarget))
                    continue;

                hitTargets.Add(hitTarget);

                Vector3 knockbackDirection = ProjectileImpactResolver.ResolveDirectionalKnockbackDirection(
                    hitTarget,
                    candidate.ClosestPoint,
                    candidate.HitDistance,
                    TravelDirection,
                    impact.Knockback.FanAngle
                );

                ProjectileImpactResolver.Resolve(impact, ProjectileImpactContext.Directional(source, hitTarget, knockbackDirection));

                remainingHits--;

                if (remainingHits <= 0)
                    return true;
            }

            return false;
        }
    }
}
