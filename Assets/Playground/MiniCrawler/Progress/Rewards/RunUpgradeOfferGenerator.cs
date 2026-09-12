using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class RunUpgradeOfferGenerator
    {
        public static IReadOnlyList<RunUpgradeOffer> Generate(RunState runState, IReadOnlyList<RunRewardDefinition> rewards, int offerCount)
        {
            List<RunUpgradeOffer> candidates = new();

            if (runState == null || rewards == null || offerCount <= 0)
                return candidates;

            foreach (PartyMemberDefinition member in runState.SelectedParty)
            {
                if (member == null)
                    continue;

                RunBuild build = runState.GetBuild(member);

                foreach (RunRewardDefinition reward in rewards)
                {
                    if (reward == null || !reward.IsConfigured || !reward.CanApply(member, build))
                        continue;

                    if (!reward.AllowDuplicatePendingOffers && runState.HasPendingOffer(member, reward))
                        continue;

                    candidates.Add(new RunUpgradeOffer(member, reward));
                }
            }

            return SelectOffers(candidates, offerCount);
        }

        private static IReadOnlyList<RunUpgradeOffer> SelectOffers(List<RunUpgradeOffer> candidates, int offerCount)
        {
            int resultCount = Mathf.Min(offerCount, candidates.Count);

            for (int i = 0; i < resultCount; i++)
            {
                int swapIndex =Random.Range(i, candidates.Count);
                (candidates[i], candidates[swapIndex]) = (candidates[swapIndex], candidates[i]);
            }

            if (resultCount < candidates.Count)
                candidates.RemoveRange(resultCount, candidates.Count - resultCount);

            return candidates;
        }
    }
}