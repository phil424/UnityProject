using System;
using System.Collections.Generic;
using MiniCrawler.Progress;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentRewardValidation
    {
        public static void Collect(RunRewardCatalogDefinition catalog, List<string> output)
        {
            output.Clear();

            if (catalog == null)
                return;

            HashSet<RunRewardSetDefinition> sets = new();
            Dictionary<string, RunRewardDefinition> rewardIds = new(StringComparer.Ordinal);

            foreach (RunRewardSetDefinition rewardSet in catalog.RewardSets)
            {
                if (rewardSet == null)
                {
                    output.Add("Reward Catalogue contains a missing Reward Set reference.");
                    continue;
                }

                if (!sets.Add(rewardSet))
                {
                    output.Add($"Reward Set '{rewardSet.name}' appears more than once in the Catalogue.");
                }

                HashSet<RunRewardDefinition> localRewards = new();

                foreach (RunRewardDefinition reward in rewardSet.Rewards)
                {
                    if (reward == null)
                    {
                        output.Add($"Reward Set '{rewardSet.name}' contains a missing Reward reference.");

                        continue;
                    }

                    if (!localRewards.Add(reward))
                    {
                        output.Add($"Reward '{reward.DisplayName}' appears more than once in '{rewardSet.name}'.");
                    }

                    if (!reward.IsConfigured)
                    {
                        output.Add($"Reward '{reward.DisplayName}' is not fully configured.");
                    }

                    if (string.IsNullOrWhiteSpace(reward.Id))
                    {
                        output.Add($"Reward '{reward.name}' has no stable ID.");

                        continue;
                    }

                    if (rewardIds.TryGetValue(reward.Id, out RunRewardDefinition existing) && existing != reward)
                    {
                        output.Add($"Rewards '{existing.DisplayName}' and '{reward.DisplayName}' share ID '{reward.Id}'.");
                    }
                    else
                    {
                        rewardIds[reward.Id] = reward;
                    }
                }
            }
        }
    }
}
