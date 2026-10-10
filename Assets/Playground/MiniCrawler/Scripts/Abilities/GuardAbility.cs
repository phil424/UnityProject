using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class GuardAbility : ActorAbility
    {
        [Header("Shields")]
        [SerializeField]
        private AbilityLevelValue shieldsPerActivation = new();

        [SerializeField]
        private AbilityLevelValue hitsPerShield = new();

        [Header("Healing Over Time")]
        [SerializeField]
        private AbilityLevelValue healingDuration = new();

        [SerializeField]
        private AbilityLevelValue healingTickInterval = new();

        [SerializeField]
        private AbilityLevelValue healingPerTick = new();

        [Header("Activation")]
        [SerializeField]
        private AbilityLevelValue activationLockDuration = new();

        private float activationLockRemaining;

        public int ShieldsPerActivation => Mathf.Max(1, Mathf.RoundToInt(Evaluate(shieldsPerActivation)));

        public int HitsPerShield => Mathf.Max(1, Mathf.RoundToInt(Evaluate(hitsPerShield)));

        public float HealingDuration => Mathf.Max(0f, Evaluate(healingDuration));

        public float HealingTickInterval => Mathf.Max(0.05f, Evaluate(healingTickInterval));

        public float HealingPerTick => Mathf.Max(0f, Evaluate(healingPerTick));

        public float ActivationLockDuration => Mathf.Max(0f, Evaluate(activationLockDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null)
                return false;

            Health health = Owner.GetComponent<Health>();

            if (health == null || health.IsDead)
                return false;

            if (!CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover != null && (mover.CurrentIntent != TargetIntent.Combat || mover.CurrentTarget == null))
            {
                return false;
            }

            return true;
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            float lockDuration = ActivationLockDuration;

            if (lockDuration > 0f && !BeginExecution())
            {
                return false;
            }

            HitShieldRuntime shields = Owner.GetComponent<HitShieldRuntime>();

            if (shields == null)
            {
                shields = Owner.AddComponent<HitShieldRuntime>();
            }

            bool shieldApplied = shields.AddShields(Owner, ShieldsPerActivation, HitsPerShield, AbilityName);

            bool healingApplied = false;

            if (HealingDuration > 0f && HealingPerTick > 0f)
            {
                HealingOverTimeRuntime healing = Owner.GetComponent<HealingOverTimeRuntime>();

                if (healing == null)
                {
                    healing = Owner.AddComponent<HealingOverTimeRuntime>();
                }

                healingApplied = healing.ApplyOrRefresh(HealingSourceId, AbilityName, HealingDuration, HealingTickInterval, HealingPerTick);
            }

            if (!shieldApplied && !healingApplied)
            {
                EndExecution();
                return false;
            }

            activationLockRemaining = lockDuration;

            Debug.Log(
                $"[Ability] {Owner.name} activated {AbilityName} | "
                    + $"+{ShieldsPerActivation} shield(s), "
                    + $"{HitsPerShield} hit(s) each | "
                    + $"HoT {HealingPerTick:0.##} every "
                    + $"{HealingTickInterval:0.##}s for "
                    + $"{HealingDuration:0.##}s."
            );

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (!IsExecuting)
                return;

            activationLockRemaining -= deltaTime;

            if (activationLockRemaining > 0f)
                return;

            activationLockRemaining = 0f;
            EndExecution();
        }

        protected override void OnDisable()
        {
            activationLockRemaining = 0f;
            base.OnDisable();
        }

        private string HealingSourceId => Definition != null ? $"Ability:{Definition.Id}" : $"Ability:{GetType().Name}";

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}
