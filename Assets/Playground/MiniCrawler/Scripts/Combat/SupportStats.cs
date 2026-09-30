using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Support
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor), typeof(PartyMember), typeof(AutoTargetMover))]
    public class SupportStats : MonoBehaviour
    {
        [Header("Healing")]
        [SerializeField] private string healName = "Basic Heal";
        [SerializeField] private float healAmount = 8f;
        [SerializeField] private float healRange = 2.5f;
        [SerializeField] private float healCooldown = 1.5f;

        private float gearHealingBonus;

        public string HealName => healName;
        public float HealAmount => healAmount + gearHealingBonus;
        public float HealRange => healRange;
        public float HealCooldown => healCooldown;

        public float HealTimer { get; set; }

        public void ApplyHealingBonus(float bonus)
        {
            gearHealingBonus = Mathf.Max(0f, bonus);
        }

        private void OnValidate()
        {
            healAmount = Mathf.Max(0f, healAmount);
            healRange = Mathf.Max(0.1f, healRange);
            healCooldown = Mathf.Max(0.1f, healCooldown);
        }
    }
}
