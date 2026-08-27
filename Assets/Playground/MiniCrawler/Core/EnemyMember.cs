using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class EnemyMember : MonoBehaviour
    {
        [SerializeField] private bool isBoss;
        [SerializeField] private int currencyReward = 1;

        public bool IsBoss => isBoss;
        public int CurrencyReward => currencyReward;

        private void OnValidate()
        {
            currencyReward = Mathf.Max(0, currencyReward);
        }
    }
}