using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class BlinkAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;

        [Header("Traversal")]
        [SerializeField]
        private AbilityLevelValue searchRadius = new();

        [SerializeField]
        private AbilityLevelValue maximumBounces = new();

        [SerializeField]
        private AbilityLevelValue bounceInterval = new();

        [SerializeField, Min(0f)]
        private float arrivalOffset = 0.35f;

        [SerializeField, Min(0f)]
        private float finishDelay = 0.1f;

        [Header("Impact")]
        [SerializeField]
        private AbilityLevelValue damage = new();

        [SerializeField]
        private AbilityLevelValue knockbackDistance = new();

        [SerializeField]
        private AbilityLevelValue knockbackSpeed = new();

        [SerializeField]
        private AbilityLevelValue stunDuration = new();

        private readonly List<Health> traversalRoute = new();

        private int nextRouteIndex;
        private float bounceRemaining;
        private float finishRemaining;
        private bool invulnerabilityActive;

        public float SearchRadius => Mathf.Max(0.1f, Evaluate(searchRadius));

        public int MaximumBounces => Mathf.Max(1, Mathf.RoundToInt(Evaluate(maximumBounces)));

        public float BounceInterval => Mathf.Max(0.05f, Evaluate(bounceInterval));

        public float Damage => Mathf.Max(0f, Evaluate(damage));

        public float KnockbackDistance => Mathf.Max(0f, Evaluate(knockbackDistance));

        public float KnockbackSpeed => Mathf.Max(0f, Evaluate(knockbackSpeed));

        public float StunDuration => Mathf.Max(0f, Evaluate(stunDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null || mover.CurrentIntent != TargetIntent.Combat)
                return false;

            return HasValidTarget(mover);
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover = Owner != null ? Owner.GetComponent<AutoTargetMover>() : null;

            if (mover == null || !BuildTraversalRoute(mover))
                return false;

            if (!BeginExecution())
                return false;

            if (!InvulnerabilityRuntime.Begin(Owner, InvulnerabilitySourceId))
            {
                traversalRoute.Clear();
                EndExecution();
                return false;
            }

            invulnerabilityActive = true;
            nextRouteIndex = 0;
            bounceRemaining = 0f;
            finishRemaining = finishDelay;

            if (!TryPerformNextBounce())
            {
                EndBlink();
                return false;
            }

            bounceRemaining = BounceInterval;

            Debug.Log($"[Ability] {Owner.name} activated {AbilityName} with " + $"{traversalRoute.Count} planned traversal point(s).");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (nextRouteIndex >= traversalRoute.Count)
            {
                finishRemaining -= deltaTime;

                if (finishRemaining <= 0f)
                    EndBlink();

                return;
            }

            bounceRemaining -= deltaTime;

            if (bounceRemaining > 0f)
                return;

            if (!TryPerformNextBounce())
            {
                nextRouteIndex = traversalRoute.Count;
                finishRemaining = finishDelay;
                return;
            }

            bounceRemaining = BounceInterval;
        }

        protected override void OnDisable()
        {
            ReleaseInvulnerability();

            traversalRoute.Clear();
            nextRouteIndex = 0;
            bounceRemaining = 0f;
            finishRemaining = 0f;

            base.OnDisable();
        }

        private bool HasValidTarget(AutoTargetMover mover)
        {
            Vector3 origin = Owner.transform.position;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (IsValidTarget(mover, target, origin))
                    return true;
            }

            return false;
        }

        private bool BuildTraversalRoute(AutoTargetMover mover)
        {
            traversalRoute.Clear();

            Vector3 origin = Owner.transform.position;

            List<Health> candidates = new();

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (IsValidTarget(mover, target, origin))
                    candidates.Add(target);
            }

            if (candidates.Count == 0)
                return false;

            Vector3 routePosition = origin;

            Health currentTarget = mover.CurrentTarget;

            if (currentTarget != null)
            {
                int currentTargetIndex = candidates.IndexOf(currentTarget);

                if (currentTargetIndex >= 0)
                {
                    traversalRoute.Add(currentTarget);
                    routePosition = currentTarget.transform.position;
                    candidates.RemoveAt(currentTargetIndex);
                }
            }

            while (traversalRoute.Count < MaximumBounces && candidates.Count > 0)
            {
                int selectedIndex = FindFarthestCandidateIndex(candidates, routePosition);

                Health selected = candidates[selectedIndex];

                traversalRoute.Add(selected);

                routePosition = selected.transform.position;

                candidates.RemoveAt(selectedIndex);
            }

            return traversalRoute.Count > 0;
        }

        private static int FindFarthestCandidateIndex(IReadOnlyList<Health> candidates, Vector3 from)
        {
            int selectedIndex = 0;
            float greatestDistanceSquared = -1f;

            for (int i = 0; i < candidates.Count; i++)
            {
                Health candidate = candidates[i];

                if (candidate == null)
                    continue;

                Vector3 offset = candidate.transform.position - from;

                offset.y = 0f;

                float distanceSquared = offset.sqrMagnitude;

                if (distanceSquared <= greatestDistanceSquared)
                    continue;

                greatestDistanceSquared = distanceSquared;

                selectedIndex = i;
            }

            return selectedIndex;
        }

        private bool TryPerformNextBounce()
        {
            while (nextRouteIndex < traversalRoute.Count)
            {
                Health target = traversalRoute[nextRouteIndex++];

                if (target == null || target.IsDead)
                    continue;

                PerformBounce(target);
                return true;
            }

            return false;
        }

        private void PerformBounce(Health target)
        {
            Vector3 previousPosition = Owner.transform.position;

            Vector3 targetPosition = target.transform.position;

            Vector3 approachDirection = targetPosition - previousPosition;

            approachDirection.y = 0f;

            if (approachDirection.sqrMagnitude <= MinimumDirectionSquared)
            {
                approachDirection = Owner.transform.forward;

                approachDirection.y = 0f;
            }

            if (approachDirection.sqrMagnitude <= MinimumDirectionSquared)
            {
                approachDirection = Vector3.forward;
            }

            approachDirection.Normalize();

            Vector3 destination = targetPosition - approachDirection * arrivalOffset;

            destination.y = previousPosition.y;

            Owner.transform.position = destination;

            if (Damage > 0f)
            {
                DamageResolver.ApplyDamage(Owner, target, Damage, AbilityName);
            }

            if (target.IsDead)
                return;

            KnockbackResolver.TryApplyDirectional(Owner, target, approachDirection, KnockbackDistance, KnockbackSpeed);

            StunRuntime.Apply(Owner, target, StunDuration, AbilityName);
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target, Vector3 origin)
        {
            if (Owner == null || mover == null || target == null || target.IsDead || target.gameObject == Owner)
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            if (faction == null || !faction.IsFaction(mover.TargetFactionId))
            {
                return false;
            }

            Vector3 offset = target.transform.position - origin;

            offset.y = 0f;

            return offset.sqrMagnitude <= SearchRadius * SearchRadius;
        }

        private void EndBlink()
        {
            ReleaseInvulnerability();

            traversalRoute.Clear();
            nextRouteIndex = 0;
            bounceRemaining = 0f;
            finishRemaining = 0f;

            EndExecution();
        }

        private void ReleaseInvulnerability()
        {
            if (!invulnerabilityActive || Owner == null)
            {
                return;
            }

            InvulnerabilityRuntime.End(Owner, InvulnerabilitySourceId);

            invulnerabilityActive = false;
        }

        private string InvulnerabilitySourceId => Definition != null ? $"Ability:{Definition.Id}" : $"Ability:{GetType().Name}";

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, SearchRadius);
        }
    }
}
