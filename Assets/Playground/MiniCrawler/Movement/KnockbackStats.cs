using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public sealed class KnockbackStats : MonoBehaviour
    {
        private float runPowerPercentBonus;

        public float PowerPercentBonus => runPowerPercentBonus;

        public void ApplyRunPowerBonus(float percentBonus)
        {
            runPowerPercentBonus = Mathf.Max(0f, percentBonus);
        }

        public float GetModifiedOutgoingDistance(float baseDistance)
        {
            float multiplier = Mathf.Max(0f, 1f + runPowerPercentBonus / 100f);
            return Mathf.Max(0f, baseDistance) * multiplier;
        }
    }
}