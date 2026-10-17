using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class SmokeStepAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;

        [Header("Smoke Step")]
        [SerializeField]
        private AbilityLevelValue hopDistance = new();

        [SerializeField]
        private AbilityLevelValue hopSpeed = new();

        [SerializeField]
        private AbilityLevelValue smokeRadius = new();

        [SerializeField]
        private AbilityLevelValue fearDuration = new();

        [Header("Fear Herding")]
        [SerializeField, Range(0f, 1f)]
        private float lateralFunnelStrength = 0.35f;

        [SerializeField, Min(0f)]
        private float funnelDeadZone = 0.75f;

        private ForcedMotion forcedMotion;

        public float HopDistance => Mathf.Max(0f, Evaluate(hopDistance));

        public float HopSpeed => Mathf.Max(0f, Evaluate(hopSpeed));

        public float SmokeRadius => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(smokeRadius)));

        public float FearDuration => Mathf.Max(0f, Evaluate(fearDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || HopDistance <= 0f || HopSpeed <= 0f || FearDuration <= 0f)
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null || mover.CurrentIntent != TargetIntent.Combat)
            {
                return false;
            }

            Health target = mover.CurrentTarget;

            if (target == null || target.IsDead || Owner.GetComponent<ForcedMotion>() == null)
            {
                return false;
            }

            Vector3 offset = Owner.transform.position - target.transform.position;

            offset.y = 0f;

            if (offset.sqrMagnitude <= MinimumDirectionSquared)
            {
                return false;
            }

            return offset.sqrMagnitude <= SmokeRadius * SmokeRadius;
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover = Owner != null ? Owner.GetComponent<AutoTargetMover>() : null;

            Health target = mover != null ? mover.CurrentTarget : null;

            if (mover == null || target == null || target.IsDead)
            {
                return false;
            }

            Vector3 hopDirection = Owner.transform.position - target.transform.position;

            hopDirection.y = 0f;

            if (hopDirection.sqrMagnitude <= MinimumDirectionSquared)
            {
                return false;
            }

            hopDirection.Normalize();

            Vector3 smokeCentre = Owner.transform.position;

            forcedMotion = Owner.GetComponent<ForcedMotion>();

            if (forcedMotion == null || !BeginExecution())
            {
                return false;
            }

            bool started = forcedMotion.StartMotion(hopDirection, HopDistance, HopSpeed, canPropagate: false);

            if (!started)
            {
                EndExecution();
                return false;
            }

            Vector3 fearDirection = -hopDirection;

            int fearedTargets = ApplySmokeFear(mover, smokeCentre, fearDirection);

            Debug.Log($"[Ability] {Owner.name} activated {AbilityName}, " + $"hopped {HopDistance:0.##} and feared " + $"{fearedTargets} target(s).");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (forcedMotion == null || !forcedMotion.IsActive)
            {
                EndExecution();
            }
        }

        protected override void OnDisable()
        {
            forcedMotion = null;
            base.OnDisable();
        }

        private int ApplySmokeFear(AutoTargetMover mover, Vector3 smokeCentre, Vector3 fearDirection)
        {
            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int fearedCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidSmokeTarget(mover, target, smokeCentre))
                {
                    continue;
                }

                if (FearRuntime.Apply(Owner, target, fearDirection, smokeCentre, FearDuration, lateralFunnelStrength, funnelDeadZone, AbilityName))
                {
                    fearedCount++;
                }
            }

            return fearedCount;
        }

        private bool IsValidSmokeTarget(AutoTargetMover mover, Health target, Vector3 smokeCentre)
        {
            if (Owner == null || target == null || target.IsDead || target.gameObject == Owner)
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            if (faction == null || !faction.IsFaction(mover.TargetFactionId))
            {
                return false;
            }

            Vector3 offset = target.transform.position - smokeCentre;

            offset.y = 0f;

            return offset.sqrMagnitude <= SmokeRadius * SmokeRadius;
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, SmokeRadius);
        }
    }
}
