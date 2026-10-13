using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(fileName = "New Run Reward Catalog", menuName = "Mini Crawler/Run Rewards/Reward Catalog")]
    public sealed class RunRewardCatalogDefinition : ScriptableObject
    {
        [SerializeField]
        private List<RunRewardSetDefinition> rewardSets = new();

        [Header("Rarity Weights")]
        [SerializeField, Min(0f)]
        private float commonRarityWeight = 60f;

        [SerializeField, Min(0f)]
        private float uncommonRarityWeight = 25f;

        [SerializeField, Min(0f)]
        private float rareRarityWeight = 10f;

        [SerializeField, Min(0f)]
        private float epicRarityWeight = 4f;

        [SerializeField, Min(0f)]
        private float legendaryRarityWeight = 1f;

        public IReadOnlyList<RunRewardSetDefinition> RewardSets => rewardSets;

        public float TotalRarityWeight => CommonRarityWeight + UncommonRarityWeight + RareRarityWeight + EpicRarityWeight + LegendaryRarityWeight;

        public float CommonRarityWeight => Mathf.Max(0f, commonRarityWeight);
        public float UncommonRarityWeight => Mathf.Max(0f, uncommonRarityWeight);
        public float RareRarityWeight => Mathf.Max(0f, rareRarityWeight);
        public float EpicRarityWeight => Mathf.Max(0f, epicRarityWeight);
        public float LegendaryRarityWeight => Mathf.Max(0f, legendaryRarityWeight);

        public bool HasPositiveRarityWeight => TotalRarityWeight > 0f;

        public float GetRarityWeight(RunUpgradeRarity rarity)
        {
            return rarity switch
            {
                RunUpgradeRarity.Common => CommonRarityWeight,
                RunUpgradeRarity.Uncommon => UncommonRarityWeight,
                RunUpgradeRarity.Rare => RareRarityWeight,
                RunUpgradeRarity.Epic => EpicRarityWeight,
                RunUpgradeRarity.Legendary => LegendaryRarityWeight,
                _ => 0f,
            };
        }

        public float GetBaselineRarityChance(RunUpgradeRarity rarity)
        {
            float total = TotalRarityWeight;

            return total > 0f ? GetRarityWeight(rarity) / total : 0f;
        }

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
                    {
                        continue;
                    }

                    if (collectedIds.Add(reward.Id))
                        output.Add(reward);
                }
            }

            return output.Count;
        }

        private void OnValidate()
        {
            commonRarityWeight = Mathf.Max(0f, commonRarityWeight);
            uncommonRarityWeight = Mathf.Max(0f, uncommonRarityWeight);
            rareRarityWeight = Mathf.Max(0f, rareRarityWeight);
            epicRarityWeight = Mathf.Max(0f, epicRarityWeight);
            legendaryRarityWeight = Mathf.Max(0f, legendaryRarityWeight);
        }
    }
}
