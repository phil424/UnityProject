using MiniCrawler.Combat;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public class RageAbility :
        ActorAbility
    {
        [Header("Rage")]
        [SerializeField]
        private AbilityLevelValue duration;

        [SerializeField]
        private AbilityLevelValue
            outgoingDamagePercentBonus;

        [SerializeField]
        private AbilityLevelValue
            attackSpeedPercentBonus;

        [SerializeField]
        private AbilityLevelValue
            moveSpeedPercentBonus;

        [SerializeField]
        private AbilityLevelValue
            flatArmourBonus;

        public float Duration =>
            Mathf.Max(
                0.1f,
                Evaluate(
                    duration
                )
            );

        public float
            OutgoingDamagePercentBonus =>
                Evaluate(
                    outgoingDamagePercentBonus
                );

        public float AttackSpeedPercentBonus =>
            Evaluate(
                attackSpeedPercentBonus
            );

        public float MoveSpeedPercentBonus =>
            Evaluate(
                moveSpeedPercentBonus
            );

        public float FlatArmourBonus =>
            Evaluate(
                flatArmourBonus
            );

        protected override bool
            CanActivateAbility()
        {
            if (Owner == null)
                return false;

            RuntimeStatModifiers modifiers =
                Owner.GetComponent<
                    RuntimeStatModifiers
                >();

            return
                modifiers == null ||
                !modifiers.HasModifier(
                    ModifierSourceId
                );
        }

        protected override bool
            ExecuteAbility()
        {
            if (Owner == null)
                return false;

            RuntimeStatModifiers modifiers =
                Owner.GetComponent<
                    RuntimeStatModifiers
                >();

            if (modifiers == null)
            {
                modifiers =
                    Owner.AddComponent<
                        RuntimeStatModifiers
                    >();
            }

            bool applied =
                modifiers.ApplyOrRefresh(
                    ModifierSourceId,
                    Duration,
                    OutgoingDamagePercentBonus,
                    AttackSpeedPercentBonus,
                    MoveSpeedPercentBonus,
                    FlatArmourBonus
                );

            if (!applied)
                return false;

            Debug.Log(
                $"[Ability] {Owner.name} activated " +
                $"{AbilityName} for " +
                $"{Duration:0.##}s | " +
                $"Damage +" +
                $"{OutgoingDamagePercentBonus:0.#}% | " +
                $"Attack Speed +" +
                $"{AttackSpeedPercentBonus:0.#}% | " +
                $"Move Speed +" +
                $"{MoveSpeedPercentBonus:0.#}% | " +
                $"Armour +" +
                $"{FlatArmourBonus:0.#}."
            );

            return true;
        }

        private string ModifierSourceId =>
            Definition != null
                ? $"Ability:{Definition.Id}"
                : $"Ability:{GetType().Name}";

        private float Evaluate(
            AbilityLevelValue value
        )
        {
            return
                value != null
                    ? value.Evaluate(Level)
                    : 0f;
        }
    }
}