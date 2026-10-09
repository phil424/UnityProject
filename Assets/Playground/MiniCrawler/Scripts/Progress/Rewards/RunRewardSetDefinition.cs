using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Run Reward Set",
        menuName = "Mini Crawler/Run Rewards/Reward Set"
    )]
    public sealed class RunRewardSetDefinition : ScriptableObject
    {
        [SerializeField] private List<RunRewardDefinition> rewards = new();

        public IReadOnlyList<RunRewardDefinition> Rewards => rewards;
    }
}