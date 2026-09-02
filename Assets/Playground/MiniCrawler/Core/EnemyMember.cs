using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class EnemyMember : MonoBehaviour
    {
        [SerializeField] private bool isBoss;
        [SerializeField] private int currencyReward = 1;
        [SerializeField] private int rewardChoicesOnDeath;

        public bool IsBoss => isBoss;
        public int CurrencyReward => currencyReward;
        public int RewardChoicesOnDeath => rewardChoicesOnDeath;

        private void OnValidate()
        {
            currencyReward = Mathf.Max(0, currencyReward);
            rewardChoicesOnDeath = Mathf.Max(0, rewardChoicesOnDeath);
        }
    }
}