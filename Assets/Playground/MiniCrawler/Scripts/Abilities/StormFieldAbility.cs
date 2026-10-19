using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class StormFieldAbility : ActorAbility
    {
        [Header("Storm Field")]
        [SerializeField]
        private AbilityLevelValue duration;

        [SerializeField]
        private AbilityLevelValue radius;

        [SerializeField]
        private AbilityLevelValue pulseInterval;

        [SerializeField]
        private AbilityLevelValue damagePerPulse;

        private float durationRemaining;
        private float pulseRemaining;

        public float Duration => Mathf.Max(0.1f, Evaluate(duration));

        public float Radius => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(radius)));

        public float PulseInterval => Mathf.Max(0.05f, Evaluate(pulseInterval));

        public float DamagePerPulse => ResolveAbilityDamage(Evaluate(damagePerPulse));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || Duration <= 0f || Radius <= 0f || DamagePerPulse <= 0f || !CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            return mover != null && mover.isActiveAndEnabled && mover.CurrentIntent == TargetIntent.Combat && mover.CurrentTarget != null && !mover.CurrentTarget.IsDead;
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null || !BeginExecution(blockAutonomousActions: false))
            {
                return false;
            }

            durationRemaining = Duration;

            pulseRemaining = PulseInterval;

            int initialHits = ApplyPulse();

            Debug.Log($"[Ability] {Owner.name} activated " + $"{AbilityName} for " + $"{Duration:0.##}s | " + $"Radius {Radius:0.##} | " + $"Initial hits {initialHits}.");

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

            while (pulseRemaining <= 0f && durationRemaining > 0f)
            {
                ApplyPulse();

                pulseRemaining += PulseInterval;
            }

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

        private int ApplyPulse()
        {
            if (Owner == null)
                return 0;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return 0;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int hitCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target))
                {
                    continue;
                }

                DamageResolver.ApplyDamage(Owner, target, DamagePerPulse, AbilityName);

                hitCount++;
            }

            return hitCount;
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

            Vector3 offset = target.transform.position - Owner.transform.position;

            offset.y = 0f;

            return offset.sqrMagnitude <= Radius * Radius;
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, Radius);
        }
    }
}
