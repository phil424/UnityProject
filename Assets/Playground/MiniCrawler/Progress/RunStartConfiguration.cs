using System.Collections.Generic;
using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunStartConfiguration
    {
        private readonly List<PartyMemberDefinition> party = new();

        public IReadOnlyList<PartyMemberDefinition> Party => party;

        public bool IsValid => party.Count > 0;

        public RunStartConfiguration(
            IEnumerable<PartyMemberDefinition> startingParty
        )
        {
            if (startingParty == null)
                return;

            foreach (PartyMemberDefinition member in startingParty)
            {
                if (member == null || party.Contains(member))
                    continue;

                party.Add(member);
            }
        }
    }
}