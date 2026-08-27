using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class RunUpgradeOfferGenerator
    {
        public static IReadOnlyList<RunUpgradeOffer> Generate(
            IReadOnlyList<PartyMemberDefinition> party,
            IReadOnlyList<RunUpgradeDefinition> upgrades,
            int offerCount
        )
        {
            List<RunUpgradeOffer> candidates = new();

            if (party == null ||
                upgrades == null ||
                offerCount <= 0)
            {
                return candidates;
            }

            foreach (
                PartyMemberDefinition member
                in party
            )
            {
                if (member == null)
                    continue;

                foreach (
                    RunUpgradeDefinition upgrade
                        in upgrades
                )
                {
                    if (upgrade == null ||
                        upgrade.Amount <= 0f)
                    {
                        continue;
                    }

                    candidates.Add(
                        new RunUpgradeOffer(
                            member,
                            upgrade
                        )
                    );
                }
            }

            int resultCount =
                Mathf.Min(
                    offerCount,
                    candidates.Count
                );

            for (int i = 0; i < resultCount; i++)
            {
                int swapIndex =
                    Random.Range(
                        i,
                        candidates.Count
                    );

                (
                    candidates[i],
                    candidates[swapIndex]
                ) =
                (
                    candidates[swapIndex],
                    candidates[i]
                );
            }

            if (resultCount <
                candidates.Count)
            {
                candidates.RemoveRange(
                    resultCount,
                    candidates.Count -
                    resultCount
                );
            }

            return candidates;
        }
    }
}