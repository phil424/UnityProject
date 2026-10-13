using System;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public sealed class ShieldCapacityStats : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private int baseCapacity = 3;

        private int runCapacityBonus;

        public event Action<ShieldCapacityStats> Changed;

        public int BaseCapacity => Mathf.Max(0, baseCapacity);
        public int RunCapacityBonus => Mathf.Max(0, runCapacityBonus);
        public int Capacity => BaseCapacity + RunCapacityBonus;

        public void ApplyRunCapacityBonus(float bonus)
        {
            int nextBonus = Mathf.Max(0, Mathf.RoundToInt(bonus));

            if (runCapacityBonus == nextBonus)
                return;

            runCapacityBonus = nextBonus;
            Changed?.Invoke(this);
        }

        private void OnValidate()
        {
            baseCapacity = Mathf.Max(0, baseCapacity);
        }
    }
}
