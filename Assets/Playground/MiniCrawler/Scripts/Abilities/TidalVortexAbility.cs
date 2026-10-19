using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class TidalVortexAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;

        [Header("Targeting")]
        [SerializeField]
        private AbilityLevelValue groupSearchRadius;

        [SerializeField]
        private AbilityLevelValue groupRadius;

        [Header("Vortex")]
        [SerializeField]
        private AbilityLevelValue duration;

        [SerializeField]
        private AbilityLevelValue radius;

        [SerializeField]
        private AbilityLevelValue pulseInterval;

        [SerializeField]
        private AbilityLevelValue pullDistance;

        [SerializeField]
        private AbilityLevelValue pullSpeed;

        private Vector3 vortexCenter;

        private float durationRemaining;
        private float pulseRemaining;

        public float GroupSearchRadius => Mathf.Max(0.1f, Evaluate(groupSearchRadius));

        public float GroupRadius => Mathf.Max(0.1f, Evaluate(groupRadius));

        public float Duration => Mathf.Max(0.1f, Evaluate(duration));

        public float Radius => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(radius)));

        public float PulseInterval => Mathf.Max(0.05f, Evaluate(pulseInterval));

        public float PullDistance => Mathf.Max(0f, Evaluate(pullDistance));

        public float PullSpeed => Mathf.Max(0f, Evaluate(pullSpeed));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || !CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            return mover != null && mover.isActiveAndEnabled && mover.CurrentIntent == TargetIntent.Combat && mover.CurrentTarget != null && !mover.CurrentTarget.IsDead;
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

            if (!BeginExecution(blockAutonomousActions: false))
            {
                return false;
            }

            vortexCenter = group.Center;

            durationRemaining = Duration;

            pulseRemaining = PulseInterval;

            int initialPulls = ApplyPullPulse(mover);

            Debug.Log($"[Ability] {Owner.name} created " + $"{AbilityName} around " + $"{group.Count} target(s) | " + $"Radius {Radius:0.##} | " + $"Initial pulls {initialPulls}.");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (Owner == null)
            {
                EndExecution();
                return;
            }

            durationRemaining = Mathf.Max(0f, durationRemaining - deltaTime);

            pulseRemaining -= deltaTime;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            while (mover != null && pulseRemaining <= 0f && durationRemaining > 0f)
            {
                ApplyPullPulse(mover);

                pulseRemaining += PulseInterval;
            }

            Debug.DrawLine(vortexCenter - Vector3.right, vortexCenter + Vector3.right, Color.cyan, 0f, false);

            Debug.DrawLine(vortexCenter - Vector3.forward, vortexCenter + Vector3.forward, Color.cyan, 0f, false);

            if (durationRemaining > 0f)
                return;

            durationRemaining = 0f;
            pulseRemaining = 0f;

            EndExecution();
        }

        protected override void OnDisable()
        {
            durationRemaining = 0f;
            pulseRemaining = 0f;

            base.OnDisable();
        }

        private int ApplyPullPulse(AutoTargetMover mover)
        {
            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int pullCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target))
                {
                    continue;
                }

                Vector3 direction = vortexCenter - target.transform.position;

                direction.y = 0f;

                if (direction.sqrMagnitude <= MinimumDirectionSquared)
                {
                    continue;
                }

                bool moved = KnockbackResolver.TryApplyDirectional(Owner, target, direction.normalized, PullDistance, PullSpeed, canPropagate: false);

                if (moved)
                    pullCount++;
            }

            return pullCount;
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target)
        {
            if (target == null || target.IsDead || target.gameObject == Owner || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            if (faction == null || !faction.IsFaction(mover.TargetFactionId))
            {
                return false;
            }

            Vector3 offset = target.transform.position - vortexCenter;

            offset.y = 0f;

            return offset.sqrMagnitude <= Radius * Radius;
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (!IsExecuting)
                return;

            Gizmos.DrawWireSphere(vortexCenter, Radius);
        }
    }
}
