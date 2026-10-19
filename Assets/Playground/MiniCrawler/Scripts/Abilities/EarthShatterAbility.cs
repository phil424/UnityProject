using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class EarthshatterAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;

        [Header("Targeting")]
        [SerializeField]
        private AbilityLevelValue groupSearchRadius;

        [SerializeField]
        private AbilityLevelValue groupRadius;

        [Header("Rush")]
        [SerializeField]
        private AbilityLevelValue maxTravelDistance;

        [SerializeField]
        private AbilityLevelValue travelSpeed;

        [SerializeField]
        private AbilityLevelValue contactRadius;

        [SerializeField]
        private AbilityLevelValue contactDamage;

        [SerializeField]
        private AbilityLevelValue contactKnockbackDistance;

        [SerializeField]
        private AbilityLevelValue contactKnockbackSpeed;

        [SerializeField, Range(0f, 89f)]
        private float knockbackFanAngle = 55f;

        [Header("Landing")]
        [SerializeField]
        private AbilityLevelValue landingRadius;

        [SerializeField]
        private AbilityLevelValue landingDamage;

        [SerializeField]
        private AbilityLevelValue landingKnockbackDistance;

        [SerializeField]
        private AbilityLevelValue landingKnockbackSpeed;

        [SerializeField]
        private AbilityLevelValue rootDuration;

        private readonly HashSet<Health> contactHitTargets = new();

        private ForcedMotion forcedMotion;
        private Vector3 rushDirection;
        private Vector3 lastSamplePosition;
        private bool landingResolved;

        public float GroupSearchRadius => Mathf.Max(0.1f, Evaluate(groupSearchRadius));
        public float GroupRadius => Mathf.Max(0.1f, Evaluate(groupRadius));

        public float MaxTravelDistance => Mathf.Max(0f, Evaluate(maxTravelDistance));
        public float TravelSpeed => Mathf.Max(0f, Evaluate(travelSpeed));
        public float ContactRadius => Mathf.Max(0.05f, ResolveAbilityAreaSize(Evaluate(contactRadius)));
        public float ContactDamage => ResolveAbilityDamage(Evaluate(contactDamage));
        public float ContactKnockbackDistance => Mathf.Max(0f, Evaluate(contactKnockbackDistance));
        public float ContactKnockbackSpeed => Mathf.Max(0f, Evaluate(contactKnockbackSpeed));
        public float KnockbackFanAngle => Mathf.Clamp(knockbackFanAngle, 0f, 89f);

        public float LandingRadius => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(landingRadius)));
        public float LandingDamage => ResolveAbilityDamage(Evaluate(landingDamage));
        public float LandingKnockbackDistance => Mathf.Max(0f, Evaluate(landingKnockbackDistance));
        public float LandingKnockbackSpeed => Mathf.Max(0f, Evaluate(landingKnockbackSpeed));
        public float RootDuration => Mathf.Max(0f, Evaluate(rootDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || MaxTravelDistance <= 0f || TravelSpeed <= 0f || !CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null || !mover.isActiveAndEnabled || mover.CurrentIntent != TargetIntent.Combat)
            {
                return false;
            }

            Health target = mover.CurrentTarget;

            if (target == null || target.IsDead)
                return false;

            Vector3 toTarget = target.transform.position - Owner.transform.position;

            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > GroupSearchRadius * GroupSearchRadius)
            {
                return false;
            }

            ForcedMotion motion = Owner.GetComponent<ForcedMotion>();

            return motion != null;
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return false;

            if (!CombatTargetGroupQuery.TryFindBestGroup(Owner, mover.TargetFactionId, GroupSearchRadius, GroupRadius, out CombatTargetGroupResult group))
            {
                return false;
            }

            forcedMotion = Owner.GetComponent<ForcedMotion>();

            if (forcedMotion == null)
                return false;

            Vector3 toGroup = group.Center - Owner.transform.position;

            toGroup.y = 0f;

            float distanceToGroup = toGroup.magnitude;

            rushDirection = distanceToGroup > 0.0001f ? toGroup / distanceToGroup : ResolveFallbackDirection(mover);

            if (rushDirection.sqrMagnitude <= MinimumDirectionSquared)
            {
                return false;
            }

            if (!BeginExecution())
                return false;

            contactHitTargets.Clear();
            landingResolved = false;
            lastSamplePosition = Owner.transform.position;

            FaceRushDirection();

            float rushDistance = Mathf.Min(MaxTravelDistance, distanceToGroup);

            TryHitContactTargets(lastSamplePosition, lastSamplePosition);

            if (rushDistance <= 0.05f)
            {
                ResolveLandingImpact();
                EndExecution();

                return true;
            }

            bool started = forcedMotion.StartMotion(rushDirection, rushDistance, TravelSpeed, canPropagate: false);

            if (!started)
            {
                EndExecution();
                return false;
            }

            Debug.Log($"[Ability] {Owner.name} started " + $"{AbilityName} toward a group of " + $"{group.Count} target(s) | " + $"Distance {rushDistance:0.##}.");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (Owner == null)
            {
                EndExecution();
                return;
            }

            Vector3 currentPosition = Owner.transform.position;

            TryHitContactTargets(lastSamplePosition, currentPosition);

            lastSamplePosition = currentPosition;

            FaceRushDirection();

            if (forcedMotion != null && forcedMotion.IsActive)
            {
                return;
            }

            ResolveLandingImpact();
            EndExecution();
        }

        private void TryHitContactTargets(Vector3 segmentStart, Vector3 segmentEnd)
        {
            if (Owner == null)
                return;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target) || contactHitTargets.Contains(target))
                {
                    continue;
                }

                float targetRadius = 0f;

                EntityAvoidance avoidance = target.GetComponent<EntityAvoidance>();

                if (avoidance != null)
                    targetRadius = avoidance.WorldRadius;

                float hitDistance = ContactRadius + targetRadius;

                float distanceSquared = DistanceSquaredToSegment(target.transform.position, segmentStart, segmentEnd);

                if (distanceSquared > hitDistance * hitDistance)
                {
                    continue;
                }

                contactHitTargets.Add(target);

                DamageResolver.ApplyDamage(Owner, target, ContactDamage, $"{AbilityName} Rush");

                if (target.IsDead)
                    continue;

                Vector3 knockbackDirection = ResolveContactKnockbackDirection(target, segmentStart, segmentEnd, hitDistance);

                KnockbackResolver.TryApplyDirectional(Owner, target, knockbackDirection, ContactKnockbackDistance, ContactKnockbackSpeed);
            }
        }

        private void ResolveLandingImpact()
        {
            if (landingResolved || Owner == null)
            {
                return;
            }

            landingResolved = true;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int hitCount = 0;
            int knockbackCount = 0;
            int rootCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target))
                    continue;

                Vector3 offset = target.transform.position - Owner.transform.position;

                offset.y = 0f;

                if (offset.sqrMagnitude > LandingRadius * LandingRadius)
                {
                    continue;
                }

                DamageResolver.ApplyDamage(Owner, target, LandingDamage, AbilityName);

                hitCount++;

                if (target.IsDead)
                    continue;

                Vector3 direction = ResolveLandingKnockbackDirection(target);

                bool moved = KnockbackResolver.TryApplyDirectional(Owner, target, direction, LandingKnockbackDistance, LandingKnockbackSpeed);

                if (!moved)
                    continue;

                knockbackCount++;

                if (RootRuntime.Apply(Owner, target, RootDuration, AbilityName))
                {
                    rootCount++;
                }
            }

            Debug.Log($"[Ability] {Owner.name} landed " + $"{AbilityName} | " + $"Hit {hitCount} | " + $"Knockback {knockbackCount} | " + $"Root {rootCount}.");
        }

        private Vector3 ResolveContactKnockbackDirection(Health target, Vector3 segmentStart, Vector3 segmentEnd, float hitDistance)
        {
            if (target == null)
                return rushDirection;

            Vector3 closestPoint = ClosestPointOnSegment(target.transform.position, segmentStart, segmentEnd);

            Vector3 lateralDirection = target.transform.position - closestPoint;

            lateralDirection.y = 0f;

            float lateralDistance = lateralDirection.magnitude;

            if (lateralDistance <= 0.0001f || KnockbackFanAngle <= 0f)
            {
                return rushDirection;
            }

            float spreadAmount = Mathf.Clamp01(lateralDistance / Mathf.Max(0.0001f, hitDistance));

            float deflection = KnockbackFanAngle * spreadAmount;

            return KnockbackResolver.ResolveDeflectedDirection(rushDirection, lateralDirection, deflection);
        }

        private Vector3 ResolveLandingKnockbackDirection(Health target)
        {
            Vector3 direction = target.transform.position - Owner.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > MinimumDirectionSquared)
            {
                return direction.normalized;
            }

            if (rushDirection.sqrMagnitude > MinimumDirectionSquared)
            {
                return rushDirection.normalized;
            }

            return Vector3.forward;
        }

        private Vector3 ResolveFallbackDirection(AutoTargetMover mover)
        {
            if (mover?.CurrentTarget != null)
            {
                Vector3 towardTarget = mover.CurrentTarget.transform.position - Owner.transform.position;

                towardTarget.y = 0f;

                if (towardTarget.sqrMagnitude > MinimumDirectionSquared)
                {
                    return towardTarget.normalized;
                }
            }

            Vector3 forward = Owner.transform.forward;

            forward.y = 0f;

            return forward.sqrMagnitude > MinimumDirectionSquared ? forward.normalized : Vector3.forward;
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target)
        {
            if (Owner == null || target == null || target.IsDead || target.gameObject == Owner || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(mover.TargetFactionId);
        }

        private static float DistanceSquaredToSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            Vector3 closest = ClosestPointOnSegment(point, start, end);

            point.y = 0f;

            return (point - closest).sqrMagnitude;
        }

        private static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            point.y = 0f;
            start.y = 0f;
            end.y = 0f;

            Vector3 segment = end - start;

            float lengthSquared = segment.sqrMagnitude;

            if (lengthSquared <= MinimumDirectionSquared)
            {
                return start;
            }

            float t = Vector3.Dot(point - start, segment) / lengthSquared;

            return start + segment * Mathf.Clamp01(t);
        }

        private void FaceRushDirection()
        {
            if (Owner == null || rushDirection.sqrMagnitude <= MinimumDirectionSquared)
            {
                return;
            }

            Owner.transform.rotation = Quaternion.LookRotation(rushDirection, Vector3.up);
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, ContactRadius);

            Gizmos.DrawWireSphere(centre, LandingRadius);
        }
    }
}
