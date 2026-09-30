using MiniCrawler.Combat;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public class TimedStatBuffAbility : ActorAbility
    {
        [Header("Timed Stat Buff")]
        [SerializeField] private AbilityLevelValue duration;
        [SerializeField] private AbilityLevelValue outgoingDamagePercentBonus;
        [SerializeField] private AbilityLevelValue attackSpeedPercentBonus;
        [SerializeField] private AbilityLevelValue moveSpeedPercentBonus;
        [SerializeField] private AbilityLevelValue flatArmourBonus;

        [Header("Activation")]
        [SerializeField] private AbilityLevelValue activationLockDuration;

        private float activationLockRemaining;

        public float Duration => Mathf.Max(0.1f, Evaluate(duration));
        public float OutgoingDamagePercentBonus => Mathf.Max(0f, Evaluate(outgoingDamagePercentBonus));
        public float AttackSpeedPercentBonus => Mathf.Max(0f, Evaluate(attackSpeedPercentBonus));
        public float MoveSpeedPercentBonus => Mathf.Max(0f, Evaluate(moveSpeedPercentBonus));
        public float FlatArmourBonus => Mathf.Max(0f, Evaluate(flatArmourBonus));
        public float ActivationLockDuration => Mathf.Max(0f, Evaluate(activationLockDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null)
                return false;

            RuntimeStatModifiers modifiers = Owner.GetComponent<RuntimeStatModifiers>();
            return modifiers == null || !modifiers.HasModifier(ModifierSourceId);
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            float lockDuration = ActivationLockDuration;

            if (lockDuration > 0f && !BeginExecution())
                return false;

            RuntimeStatModifiers modifiers = Owner.GetComponent<RuntimeStatModifiers>();

            if (modifiers == null)
                modifiers = Owner.AddComponent<RuntimeStatModifiers>();

            bool applied = modifiers.ApplyOrRefresh(
                ModifierSourceId,
                Duration,
                OutgoingDamagePercentBonus,
                AttackSpeedPercentBonus,
                MoveSpeedPercentBonus,
                FlatArmourBonus
            );

            if (!applied)
            {
                EndExecution();
                return false;
            }

            activationLockRemaining = lockDuration;

            Debug.Log(
                $"[Ability] {Owner.name} activated {AbilityName} for {Duration:0.##}s | " +
                $"Damage +{OutgoingDamagePercentBonus:0.#}% | Attack Speed +{AttackSpeedPercentBonus:0.#}% | " +
                $"Move Speed +{MoveSpeedPercentBonus:0.#}% | Armour +{FlatArmourBonus:0.#}."
            );

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            activationLockRemaining -= deltaTime;

            if (activationLockRemaining <= 0f)
            {
                activationLockRemaining = 0f;
                EndExecution();
            }
        }

        protected override void OnDisable()
        {
            activationLockRemaining = 0f;
            base.OnDisable();
        }

        private string ModifierSourceId =>
            Definition != null ? $"Ability:{Definition.Id}" : $"Ability:{GetType().Name}";

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}