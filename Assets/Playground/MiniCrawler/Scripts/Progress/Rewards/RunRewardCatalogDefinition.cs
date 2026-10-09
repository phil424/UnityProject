using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Run Reward Catalog",
        menuName = "Mini Crawler/Run Rewards/Reward Catalog"
    )]
    public sealed class RunRewardCatalogDefinition : ScriptableObject
    {
        [SerializeField] private List<RunRewardSetDefinition> rewardSets = new();

        public IReadOnlyList<RunRewardSetDefinition> RewardSets => rewardSets;

        public int CollectRewards(List<RunRewardDefinition> output)
        {
            if (output == null)
                return 0;

            output.Clear();

            HashSet<string> collectedIds = new(StringComparer.Ordinal);

            foreach (RunRewardSetDefinition rewardSet in rewardSets)
            {
                if (rewardSet == null)
                    continue;

                foreach (RunRewardDefinition reward in rewardSet.Rewards)
                {
                    if (reward == null || !reward.IsConfigured || string.IsNullOrWhiteSpace(reward.Id))
                        continue;

                    if (collectedIds.Add(reward.Id))
                        output.Add(reward);
                }
            }

            return output.Count;
        }
    }
}