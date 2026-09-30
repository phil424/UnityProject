using System.Collections.Generic;
using MiniCrawler.Core;

namespace MiniCrawler.Progress
{
    public sealed class RunStartConfiguration
    {
        private readonly List<PartyMemberDefinition> party = new();
        private readonly List<ExpeditionObjectiveDefinition> objectives = new();

        public IReadOnlyList<PartyMemberDefinition> Party => party;
        public IReadOnlyList<ExpeditionObjectiveDefinition> Objectives => objectives;

        public bool IsValid => party.Count > 0;

        public RunStartConfiguration(IEnumerable<PartyMemberDefinition> startingParty, IEnumerable<ExpeditionObjectiveDefinition> startingObjectives)
        {
            if (startingParty != null)
            {
                foreach (PartyMemberDefinition member in startingParty)
                {
                    if (member == null || party.Contains(member))
                        continue;

                    party.Add(member);
                }
            }

            if (startingObjectives == null)
                return;

            foreach (ExpeditionObjectiveDefinition objective in startingObjectives)
            {
                if (objective == null || objectives.Contains(objective))
                    continue;

                objectives.Add(objective);
            }
        }
    }
}