using System.Collections.Generic;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class EnemyMember : MonoBehaviour
    {
        [SerializeField] private bool isBoss;

        [Header("Expedition Rewards")]
        [SerializeField] private int currencyReward = 1;
        [SerializeField] private int rewardChoicesOnDeath;

        [Header("Persistent Resource Rewards")]
        [SerializeField] private List<PersistentResourceReward> persistentResourceRewards = new();

        public bool IsBoss => isBoss;
        public int CurrencyReward => currencyReward;
        public int RewardChoicesOnDeath => rewardChoicesOnDeath;
        public IReadOnlyList<PersistentResourceReward> PersistentResourceRewards => persistentResourceRewards;

        private void OnValidate()
        {
            currencyReward = Mathf.Max(0, currencyReward);
            rewardChoicesOnDeath = Mathf.Max(0, rewardChoicesOnDeath);
        }
    }
}