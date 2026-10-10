using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class BackstepAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;
        private const float MinimumUsefulTravelDistance = 0.35f;

        [Header("Backstep")]
        [FormerlySerializedAs("distance")]
        [SerializeField]
        private AbilityLevelValue maximumDistance;

        [SerializeField]
        private AbilityLevelValue desiredSeparation;

        [SerializeField]
        private AbilityLevelValue speed;

        [Header("Departure Control")]
        [SerializeField]
        private AbilityLevelValue controlRadius = new();

        [SerializeField]
        private AbilityLevelValue knockbackDistance = new();

        [SerializeField]
        private AbilityLevelValue knockbackSpeed = new();

        [SerializeField]
        private AbilityLevelValue stunDuration = new();

        private ForcedMotion forcedMotion;

        public float MaximumDistance => Mathf.Max(0f, Evaluate(maximumDistance));
        public float DesiredSeparation => Mathf.Max(0f, Evaluate(desiredSeparation));
        public float Speed => Mathf.Max(0f, Evaluate(speed));
        public float ControlRadius => Mathf.Max(0f, Evaluate(controlRadius));
        public float KnockbackDistance => Mathf.Max(0f, Evaluate(knockbackDistance));
        public float KnockbackSpeed => Mathf.Max(0f, Evaluate(knockbackSpeed));
        public float StunDuration => Mathf.Max(0f, Evaluate(stunDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || MaximumDistance <= 0f || DesiredSeparation <= 0f || Speed <= 0f)
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

            Vector3 away = Owner.transform.position - target.transform.position;

            away.y = 0f;

            if (away.sqrMagnitude <= MinimumDirectionSquared)
                return false;

            float requiredTravel = DesiredSeparation - away.magnitude;

            return requiredTravel >= MinimumUsefulTravelDistance;
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover = Owner != null ? Owner.GetComponent<AutoTargetMover>() : null;

            Health target = mover != null ? mover.CurrentTarget : null;

            if (target == null || target.IsDead)
                return false;

            Vector3 away = Owner.transform.position - target.transform.position;

            away.y = 0f;

            float currentDistance = away.magnitude;

            if (currentDistance <= MinimumDirectionSquared)
                return false;

            float travelDistance = Mathf.Min(MaximumDistance, DesiredSeparation - currentDistance);

            if (travelDistance < MinimumUsefulTravelDistance)
                return false;

            forcedMotion = Owner.GetComponent<ForcedMotion>();

            if (forcedMotion == null || !BeginExecution())
                return false;

            bool started = forcedMotion.StartMotion(away.normalized, travelDistance, Speed, canPropagate: false);

            if (!started)
            {
                EndExecution();
                return false;
            }

            int controlledTargets = ApplyDepartureControl(mover);

            Debug.Log($"[Ability] {Owner.name} activated {AbilityName}, controlled " + $"{controlledTargets} nearby target(s) and backstepped {travelDistance:0.##}.");

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

        private int ApplyDepartureControl(AutoTargetMover mover)
        {
            if (Owner == null || mover == null || ControlRadius <= 0f)
            {
                return 0;
            }

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int controlledCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidControlTarget(mover, target))
                {
                    continue;
                }

                Vector3 offset = target.transform.position - Owner.transform.position;

                offset.y = 0f;

                if (offset.sqrMagnitude > ControlRadius * ControlRadius)
                {
                    continue;
                }

                bool moved = KnockbackResolver.TryApply(Owner, target, KnockbackDistance, KnockbackSpeed);

                bool stunned = StunRuntime.Apply(Owner, target, StunDuration, AbilityName);

                if (moved || stunned)
                    controlledCount++;
            }

            return controlledCount;
        }

        private bool IsValidControlTarget(AutoTargetMover mover, Health target)
        {
            if (Owner == null || target == null || target.IsDead || target.gameObject == Owner)
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(mover.TargetFactionId);
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, ControlRadius);
        }
    }
}
