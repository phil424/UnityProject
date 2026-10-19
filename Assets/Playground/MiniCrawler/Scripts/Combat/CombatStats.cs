using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class CombatStats : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField]
        private string attackName = "Basic Attack";

        [SerializeField]
        private float damage = 5f;

        [SerializeField]
        private float attackRange = 1.5f;

        [SerializeField]
        private float attackCooldown = 1f;

        [Header("Defence")]
        [SerializeField]
        private float flatArmour;

        private float gearDamageBonus;
        private float gearArmourBonus;
        private float runBasicAttackDamageBonus;
        private float runBasicAttackSpeedPercentBonus;
        private float runArmourBonus;

        public string AttackName => attackName;

        public float Damage => damage + gearDamageBonus + runBasicAttackDamageBonus;

        public float BaseAttackRange => attackRange;

        public float AttackRange
        {
            get
            {
                float rangeBonus = GetRuntimeBasicAttackRangePercentBonus();

                float multiplier = Mathf.Max(0.1f, 1f + rangeBonus / 100f);

                return Mathf.Max(0.1f, attackRange * multiplier);
            }
        }

        public int BasicAttackPenetrationBonus => Mathf.Max(0, GetRuntimeBasicAttackPenetrationBonus());

        public float AttackCooldown
        {
            get
            {
                float attackSpeedBonus = runBasicAttackSpeedPercentBonus + GetRuntimeAttackSpeedBonus();

                float speedMultiplier = Mathf.Max(0.1f, 1f + attackSpeedBonus / 100f);

                return attackCooldown / speedMultiplier;
            }
        }

        public float FlatArmour => Mathf.Max(0f, flatArmour + gearArmourBonus + runArmourBonus + GetRuntimeFlatArmourBonus());

        public float AttackTimer { get; set; }

        public void ApplyGearBonuses(float damageBonus, float armourBonus)
        {
            gearDamageBonus = Mathf.Max(0f, damageBonus);

            gearArmourBonus = Mathf.Max(0f, armourBonus);
        }

        public void ApplyRunBonuses(float basicAttackDamageBonus, float basicAttackSpeedPercentBonus, float armourBonus)
        {
            runBasicAttackDamageBonus = Mathf.Max(0f, basicAttackDamageBonus);

            runBasicAttackSpeedPercentBonus = Mathf.Max(0f, basicAttackSpeedPercentBonus);

            runArmourBonus = Mathf.Max(0f, armourBonus);
        }

        public float CalculateDamageTaken(float incomingDamage)
        {
            if (incomingDamage <= 0f)
                return 0f;

            return Mathf.Max(1f, incomingDamage - FlatArmour);
        }

        private float GetRuntimeAttackSpeedBonus()
        {
            RuntimeStatModifiers modifiers = GetComponent<RuntimeStatModifiers>();

            return modifiers != null ? modifiers.AttackSpeedPercentBonus : 0f;
        }

        private float GetRuntimeFlatArmourBonus()
        {
            RuntimeStatModifiers modifiers = GetComponent<RuntimeStatModifiers>();

            return modifiers != null ? modifiers.FlatArmourBonus : 0f;
        }

        private float GetRuntimeBasicAttackRangePercentBonus()
        {
            RuntimeStatModifiers modifiers = GetComponent<RuntimeStatModifiers>();

            return modifiers != null ? modifiers.BasicAttackRangePercentBonus : 0f;
        }

        private int GetRuntimeBasicAttackPenetrationBonus()
        {
            RuntimeStatModifiers modifiers = GetComponent<RuntimeStatModifiers>();

            return modifiers != null ? modifiers.BasicAttackPenetrationBonus : 0;
        }

        private void OnValidate()
        {
            damage = Mathf.Max(0f, damage);
            attackRange = Mathf.Max(0.1f, attackRange);
            attackCooldown = Mathf.Max(0.1f, attackCooldown);
            flatArmour = Mathf.Max(0f, flatArmour);
        }
    }
}
