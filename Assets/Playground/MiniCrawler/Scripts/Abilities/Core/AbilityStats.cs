using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public sealed class AbilityStats : MonoBehaviour
    {
        private float runCooldownRecoveryPercentBonus;

        public float CooldownRecoveryPercentBonus => runCooldownRecoveryPercentBonus;

        public void ApplyRunCooldownRecoveryBonus(float percentBonus)
        {
            runCooldownRecoveryPercentBonus = Mathf.Max(0f, percentBonus);
        }

        public float GetModifiedCooldown(float baseCooldown)
        {
            if (baseCooldown <= 0f)
                return 0f;

            float recoveryMultiplier = Mathf.Max(0.1f, 1f + runCooldownRecoveryPercentBonus / 100f);
            return Mathf.Max(0.1f, baseCooldown / recoveryMultiplier);
        }
    }
}