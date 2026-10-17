using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public sealed class AbilityStats : MonoBehaviour
    {
        private float runCooldownRecoveryPercentBonus;
        private float runAbilityDamagePercentBonus;
        private float runAbilityAreaSizePercentBonus;

        public float CooldownRecoveryPercentBonus => runCooldownRecoveryPercentBonus;
        public float AbilityDamagePercentBonus => runAbilityDamagePercentBonus;
        public float AbilityAreaSizePercentBonus => runAbilityAreaSizePercentBonus;

        public void ApplyRunCooldownRecoveryBonus(float percentBonus)
        {
            runCooldownRecoveryPercentBonus = Mathf.Max(0f, percentBonus);
        }

        public void ApplyRunAbilityDamageBonus(float percentBonus)
        {
            runAbilityDamagePercentBonus = Mathf.Max(0f, percentBonus);
        }

        public void ApplyRunAbilityAreaSizeBonus(float percentBonus)
        {
            runAbilityAreaSizePercentBonus = Mathf.Max(0f, percentBonus);
        }

        public float GetModifiedCooldown(float baseCooldown)
        {
            if (baseCooldown <= 0f)
                return 0f;

            float recoveryMultiplier = Mathf.Max(0.1f, 1f + runCooldownRecoveryPercentBonus / 100f);
            return Mathf.Max(0.1f, baseCooldown / recoveryMultiplier);
        }

        public float GetModifiedAbilityDamage(float baseDamage)
        {
            if (baseDamage <= 0f)
                return 0f;

            float multiplier = Mathf.Max(0f, 1f + runAbilityDamagePercentBonus / 100f);
            return Mathf.Max(0f, baseDamage * multiplier);
        }

        public float GetModifiedAbilityAreaSize(float baseSize)
        {
            if (baseSize <= 0f)
                return 0f;

            float multiplier = Mathf.Max(0f, 1f + runAbilityAreaSizePercentBonus / 100f);
            return Mathf.Max(0f, baseSize * multiplier);
        }

        [ContextMenu("Debug/Log Current Bonuses")]
        private void DebugLogCurrentBonuses()
        {
            Debug.Log(
                $"[Ability Stats] {name} | "
                    + $"Cooldown Recovery +{CooldownRecoveryPercentBonus:0.##}% | "
                    + $"Ability Damage +{AbilityDamagePercentBonus:0.##}% | "
                    + $"Ability Area Size +{AbilityAreaSizePercentBonus:0.##}%.",
                this
            );
        }
    }
}
