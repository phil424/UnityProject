using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class ReactiveArmourRecoveryAbility : ActorAbility
    {
        [Header("Recovery")]
        [SerializeField]
        private AbilityLevelValue flatHealing = new();

        [SerializeField]
        private AbilityLevelValue missingHealthHealingPercent = new();

        [Header("Armour")]
        [SerializeField]
        private AbilityLevelValue armourDuration = new();

        [SerializeField]
        private AbilityLevelValue baseArmourBonus = new();

        [SerializeField]
        private AbilityLevelValue lowHealthAdditionalArmourBonus = new();

        [Header("Activation")]
        [SerializeField]
        private AbilityLevelValue activationLockDuration = new();

        private float activationLockRemaining;

        public float FlatHealing => Mathf.Max(0f, Evaluate(flatHealing));

        public float MissingHealthHealingPercent => Mathf.Max(0f, Evaluate(missingHealthHealingPercent));

        public float ArmourDuration => Mathf.Max(0f, Evaluate(armourDuration));

        public float BaseArmourBonus => Mathf.Max(0f, Evaluate(baseArmourBonus));

        public float LowHealthAdditionalArmourBonus => Mathf.Max(0f, Evaluate(lowHealthAdditionalArmourBonus));

        public float ActivationLockDuration => Mathf.Max(0f, Evaluate(activationLockDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null)
                return false;

            Health health = Owner.GetComponent<Health>();

            return health != null && !health.IsDead && health.CurrentHealth < health.MaxHealth;
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            Health health = Owner.GetComponent<Health>();

            if (health == null || health.IsDead)
                return false;

            float missingHealth = Mathf.Max(0f, health.MaxHealth - health.CurrentHealth);

            if (missingHealth <= 0f)
                return false;

            float missingHealthFraction = 1f - health.Normalized;

            float requestedHealing = FlatHealing + missingHealth * MissingHealthHealingPercent * 0.01f;

            float armourBonus = BaseArmourBonus + LowHealthAdditionalArmourBonus * missingHealthFraction;

            float lockDuration = ActivationLockDuration;

            if (lockDuration > 0f && !BeginExecution())
            {
                return false;
            }

            float healthBefore = health.CurrentHealth;

            health.Heal(requestedHealing);

            float actualHealing = health.CurrentHealth - healthBefore;

            bool armourApplied = false;

            if (armourBonus > 0f && ArmourDuration > 0f)
            {
                RuntimeStatModifiers modifiers = Owner.GetComponent<RuntimeStatModifiers>();

                if (modifiers == null)
                {
                    modifiers = Owner.AddComponent<RuntimeStatModifiers>();
                }

                armourApplied = modifiers.ApplyOrRefresh(ModifierSourceId, ArmourDuration, 0f, 0f, 0f, armourBonus);
            }

            if (actualHealing <= 0f && !armourApplied)
            {
                EndExecution();
                return false;
            }

            activationLockRemaining = lockDuration;

            Debug.Log(
                $"[Ability] {Owner.name} activated {AbilityName} | "
                    + $"Healed {actualHealing:0.##} | "
                    + $"Armour +{armourBonus:0.##} for {ArmourDuration:0.##}s | "
                    + $"Missing Health {missingHealthFraction * 100f:0.#}%."
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

        private string ModifierSourceId => Definition != null ? $"Ability:{Definition.Id}" : $"Ability:{GetType().Name}";

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}
