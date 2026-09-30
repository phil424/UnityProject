using System;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [Serializable]
    public sealed class AbilityLoadoutEntry
    {
        [SerializeField]
        private AbilityDefinition ability;

        [SerializeField]
        [Min(1)]
        private int level = 1;

        public AbilityDefinition Ability =>
            ability;

        public int Level =>
            level;
    }
}