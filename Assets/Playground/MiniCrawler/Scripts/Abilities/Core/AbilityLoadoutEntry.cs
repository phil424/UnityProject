using System;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    public enum CharacterAbilityRole
    {
        Unassigned,
        Signature,
        Utility,
        Sustain,
    }

    [Serializable]
    public sealed class AbilityLoadoutEntry
    {
        [SerializeField]
        private CharacterAbilityRole role;

        [SerializeField]
        private AbilityDefinition ability;

        [SerializeField, Min(1)]
        private int level = 1;

        public CharacterAbilityRole Role => role;
        public AbilityDefinition Ability => ability;
        public int Level => level;
    }
}
