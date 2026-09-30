using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class PlayableCharacterRosterEntry
    {
        [SerializeField] private PartyMemberDefinition character;
        [SerializeField] private bool unlockedByDefault;
        [SerializeField] private bool visibleWhenLocked = true;
        [SerializeField] private CharacterUnlockRequirementDefinition unlockRequirement;

        public PartyMemberDefinition Character => character;
        public bool UnlockedByDefault => unlockedByDefault;
        public bool VisibleWhenLocked => visibleWhenLocked;
        public CharacterUnlockRequirementDefinition UnlockRequirement => unlockRequirement;

        public bool IsConfigured => character != null;
    }

    [CreateAssetMenu(
        fileName = "Playable Character Roster",
        menuName = "Mini Crawler/Progression/Playable Character Roster"
    )]
    public sealed class PlayableCharacterRosterDefinition : ScriptableObject
    {
        [SerializeField] private List<PlayableCharacterRosterEntry> characters = new();

        public IReadOnlyList<PlayableCharacterRosterEntry> Characters => characters;

        public bool Contains(PartyMemberDefinition character)
        {
            if (character == null)
                return false;

            foreach (PlayableCharacterRosterEntry entry in characters)
            {
                if (entry?.Character == character)
                    return true;
            }

            return false;
        }
    }
}