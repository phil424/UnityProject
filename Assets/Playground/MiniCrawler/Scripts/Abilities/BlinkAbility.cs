using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;
using UnityEngine.Serialization;

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
        private float openingDelay = 0.15f;

        [SerializeField, Min(0f)]
        private float finishDelay = 0.1f;

        [Header("Opening Control")]
        [SerializeField]
        private AbilityLevelValue openingControlRadius = new();

        [FormerlySerializedAs("knockbackDistance")]
        [SerializeField]
        private AbilityLevelValue openingKnockbackDistance = new();

        [FormerlySerializedAs("knockbackSpeed")]
        [SerializeField]
        private AbilityLevelValue openingKnockbackSpeed = new();

        [FormerlySerializedAs("stunDuration")]
        [SerializeField]
        private AbilityLevelValue openingStunDuration = new();

        [Header("Traversal Impact")]
        [SerializeField]
        private AbilityLevelValue damage = new();

        private readonly List<Health> traversalRoute = new();

        private Vector3 sigilCentre;

        private int nextRouteIndex;
        private float openingRemaining;
        private float bounceRemaining;
        private float finishRemaining;

        private bool traversalStarted;
        private bool returnedToSigil;
        private bool invulnerabilityActive;

        public float SearchRadius => Mathf.Max(0.1f, Evaluate(searchRadius));
        public int MaximumBounces => Mathf.Max(1, Mathf.RoundToInt(Evaluate(maximumBounces)));
        public float BounceInterval => Mathf.Max(0.05f, Evaluate(bounceInterval));

        public float OpeningControlRadius => Mathf.Max(0.1f, Evaluate(openingControlRadius));

        public float OpeningKnockbackDistance => Mathf.Max(0f, Evaluate(openingKnockbackDistance));

        public float OpeningKnockbackSpeed => Mathf.Max(0f, Evaluate(openingKnockbackSpeed));

        public float OpeningStunDuration => Mathf.Max(0f, Evaluate(openingStunDuration));

        public float Damage => Mathf.Max(0f, Evaluate(damage));

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

            if (mover == null)
                return false;

            sigilCentre = Owner.transform.position;

            if (!BuildTraversalRoute(mover, sigilCentre))
                return false;

            if (!BeginExecution())
                return false;

            if (!InvulnerabilityRuntime.Begin(Owner, InvulnerabilitySourceId))
            {
                ResetRuntimeState();
                EndExecution();
                return false;
            }

            invulnerabilityActive = true;

            ForcedMotion forcedMotion = Owner.GetComponent<ForcedMotion>();

            forcedMotion?.Clear();

            nextRouteIndex = 0;
            openingRemaining = openingDelay;
            bounceRemaining = 0f;
            finishRemaining = finishDelay;
            traversalStarted = false;
            returnedToSigil = false;

            int controlledTargets = ApplyOpeningControl(mover, sigilCentre);

            Debug.Log(
                $"[Ability] {Owner.name} activated {AbilityName} from its Sigil Centre, " + $"controlled {controlledTargets} target(s) and planned " + $"{traversalRoute.Count} traversal point(s)."
            );

            if (openingRemaining <= 0f)
                BeginTraversal();

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (!traversalStarted)
            {
                openingRemaining -= deltaTime;

                if (openingRemaining > 0f)
                    return;

                BeginTraversal();
                return;
            }

            if (!returnedToSigil)
            {
                if (nextRouteIndex < traversalRoute.Count)
                {
                    bounceRemaining -= deltaTime;

                    if (bounceRemaining > 0f)
                        return;

                    if (TryPerformNextBounce())
                    {
                        bounceRemaining = BounceInterval;
                        return;
                    }
                }

                ReturnToSigilCentre();
                return;
            }

            finishRemaining -= deltaTime;

            if (finishRemaining <= 0f)
                EndBlink();
        }

        protected override void OnDisable()
        {
            ReleaseInvulnerability();
            ResetRuntimeState();

            base.OnDisable();
        }

        private void BeginTraversal()
        {
            traversalStarted = true;
            openingRemaining = 0f;

            if (!TryPerformNextBounce())
            {
                ReturnToSigilCentre();
                return;
            }

            bounceRemaining = BounceInterval;
        }

        private int ApplyOpeningControl(AutoTargetMover mover, Vector3 centre)
        {
            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int controlledCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target, centre, OpeningControlRadius))
                {
                    continue;
                }

                bool moved = KnockbackResolver.TryApply(Owner, target, OpeningKnockbackDistance, OpeningKnockbackSpeed);

                bool stunned = StunRuntime.Apply(Owner, target, OpeningStunDuration, AbilityName);

                if (moved || stunned)
                    controlledCount++;
            }

            return controlledCount;
        }

        private bool HasValidTarget(AutoTargetMover mover)
        {
            Vector3 origin = Owner.transform.position;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (IsValidTarget(mover, target, origin, SearchRadius))
                {
                    return true;
                }
            }

            return false;
        }

        private bool BuildTraversalRoute(AutoTargetMover mover, Vector3 origin)
        {
            traversalRoute.Clear();

            List<Health> candidates = new();

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (IsValidTarget(mover, target, origin, SearchRadius))
                {
                    candidates.Add(target);
                }
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
                {
                    continue;
                }

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

            destination.y = sigilCentre.y;

            Owner.transform.position = destination;

            if (Damage > 0f)
            {
                DamageResolver.ApplyDamage(Owner, target, Damage, AbilityName);
            }
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target, Vector3 origin, float radius)
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

            return offset.sqrMagnitude <= radius * radius;
        }

        private void ReturnToSigilCentre()
        {
            if (returnedToSigil || Owner == null)
            {
                return;
            }

            ForcedMotion forcedMotion = Owner.GetComponent<ForcedMotion>();

            forcedMotion?.Clear();

            Owner.transform.position = sigilCentre;

            returnedToSigil = true;
            finishRemaining = finishDelay;
        }

        private void EndBlink()
        {
            if (!returnedToSigil)
                ReturnToSigilCentre();

            ReleaseInvulnerability();
            ResetRuntimeState();

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

        private void ResetRuntimeState()
        {
            traversalRoute.Clear();

            sigilCentre = Vector3.zero;

            nextRouteIndex = 0;
            openingRemaining = 0f;
            bounceRemaining = 0f;
            finishRemaining = 0f;

            traversalStarted = false;
            returnedToSigil = false;
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

            Gizmos.DrawWireSphere(centre, OpeningControlRadius);
        }
    }
}
