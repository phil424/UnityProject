using System;
using System.Collections.Generic;
using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunSetup
    {
        public event Action Changed;

        private readonly List<PartyMemberDefinition>
            selectedParty = new();

        private readonly int maximumPartySize;

        public IReadOnlyList<PartyMemberDefinition>
            SelectedParty =>
                selectedParty;

        public int MaximumPartySize =>
            maximumPartySize;

        public RunSetup(
            int maximumPartySize = 4
        )
        {
            this.maximumPartySize =
                Math.Max(
                    1,
                    maximumPartySize
                );
        }

        public bool IsSelected(
            PartyMemberDefinition definition
        )
        {
            return
                definition != null &&
                selectedParty.Contains(
                    definition
                );
        }

        public bool TogglePartyMember(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return false;

            if (
                selectedParty.Contains(
                    definition
                )
            )
            {
                selectedParty.Remove(
                    definition
                );

                Changed?.Invoke();

                return true;
            }

            if (
                selectedParty.Count >=
                maximumPartySize
            )
            {
                return false;
            }

            selectedParty.Add(
                definition
            );

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

        public RunStartConfiguration
            CreateConfiguration()
        {
            return new RunStartConfiguration(
                selectedParty
            );
        }
    }
}