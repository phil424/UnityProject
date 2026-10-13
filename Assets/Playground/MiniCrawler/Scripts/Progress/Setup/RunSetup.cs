using System;
using System.Collections.Generic;
using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunSetup
    {
        public event Action Changed;

        private readonly List<PartyMemberDefinition> selectedParty = new();
        private readonly List<ExpeditionObjectiveDefinition> selectedObjectives = new();

        private readonly int maximumPartySize;
        private readonly PlayableCharacterRosterDefinition characterRoster;

        public IReadOnlyList<PartyMemberDefinition> SelectedParty => selectedParty;
        public IReadOnlyList<ExpeditionObjectiveDefinition> SelectedObjectives => selectedObjectives;

        public int MaximumPartySize => maximumPartySize;

        public RunSetup(int maximumPartySize, PlayableCharacterRosterDefinition characterRoster)
        {
            this.maximumPartySize = Math.Max(1, maximumPartySize);
            this.characterRoster = characterRoster;
        }

        public bool IsSelected(PartyMemberDefinition definition)
        {
            return definition != null && selectedParty.Contains(definition);
        }

        public bool CanSelectPartyMember(PartyMemberDefinition definition)
        {
            return definition != null && characterRoster != null && characterRoster.Contains(definition) && PersistentProgression.IsCharacterUnlocked(definition);
        }

        public bool SetSinglePartyMember(PartyMemberDefinition definition)
        {
            if (!CanSelectPartyMember(definition))
                return false;

            if (selectedParty.Count == 1 && selectedParty[0] == definition)
                return true;

            selectedParty.Clear();
            selectedParty.Add(definition);

            Changed?.Invoke();
            return true;
        }

        public bool TogglePartyMember(PartyMemberDefinition definition)
        {
            if (definition == null)
                return false;

            if (selectedParty.Contains(definition))
            {
                selectedParty.Remove(definition);
                Changed?.Invoke();
                return true;
            }

            if (!CanSelectPartyMember(definition) || selectedParty.Count >= maximumPartySize)
                return false;

            selectedParty.Add(definition);
            Changed?.Invoke();
            return true;
        }

        public void SetObjectives(IEnumerable<ExpeditionObjectiveDefinition> objectives)
        {
            selectedObjectives.Clear();

            if (objectives != null)
            {
                foreach (ExpeditionObjectiveDefinition objective in objectives)
                {
                    if (objective == null || selectedObjectives.Contains(objective))
                        continue;

                    selectedObjectives.Add(objective);
                }
            }

            Changed?.Invoke();
        }

        public void Clear()
        {
            if (selectedParty.Count == 0)
                return;

            selectedParty.Clear();
            Changed?.Invoke();
        }

        public RunStartConfiguration CreateConfiguration(int startingRewardRerolls)
        {
            return new RunStartConfiguration(selectedParty, selectedObjectives, startingRewardRerolls);
        }
    }
}
