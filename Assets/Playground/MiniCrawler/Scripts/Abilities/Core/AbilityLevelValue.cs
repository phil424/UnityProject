using System;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [Serializable]
    public sealed class AbilityLevelValue
    {
        [SerializeField]
        private float baseValue;

        [SerializeField]
        private float perLevel;

        public float BaseValue =>
            baseValue;

        public float PerLevel =>
            perLevel;

        public float Evaluate(
            int level
        )
        {
            int additionalLevels =
                Mathf.Max(
                    0,
                    level - 1
                );

            return
                baseValue +
                perLevel * additionalLevels;
        }
    }
}