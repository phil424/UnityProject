using System;
using MiniCrawler.Abilities;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class RunAbilityState
    {
        [SerializeField]
        private AbilityDefinition definition;

        [SerializeField]
        private int level = 1;

        public AbilityDefinition Definition =>
            definition;

        public int Level =>
            level;

        public RunAbilityState(
            AbilityDefinition abilityDefinition,
            int startingLevel = 1
        )
        {
            definition =
                abilityDefinition;

            level =
                Mathf.Max(
                    1,
                    startingLevel
                );
        }

        public void IncreaseLevel()
        {
            level++;
        }
    }
}