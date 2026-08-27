using System;
using System.Collections.Generic;
using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunSetup
    {
        public const int MaximumPartySize = 4;

        public event Action Changed;

        private readonly List<PartyMemberDefinition> selectedParty = new();

        public IReadOnlyList<PartyMemberDefinition> SelectedParty =>
            selectedParty;

        public bool IsSelected(PartyMemberDefinition definition)
        {
            return definition != null &&
                   selectedParty.Contains(definition);
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

            if (selectedParty.Count >= MaximumPartySize)
                return false;

            selectedParty.Add(definition);

            Changed?.Invoke();

            return true;
        }

        public void Clear()
        {
            if (selectedParty.Count == 0)
                return;

            selectedParty.Clear();

            Changed?.Invoke();
        }

        public RunStartConfiguration CreateConfiguration()
        {
            return new RunStartConfiguration(selectedParty);
        }
    }
}