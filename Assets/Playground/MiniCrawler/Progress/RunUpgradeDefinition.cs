using UnityEngine;

namespace MiniCrawler.Progress
{
    public enum RunUpgradeRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum RunUpgradeEffectType
    {
        FlatDamage,
        MoveSpeedPercent,
        AttackSpeedPercent,
        FlatArmour
    }

    [CreateAssetMenu(
        fileName = "New Run Upgrade",
        menuName = "Mini Crawler/Run Upgrade"
    )]
    public class RunUpgradeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        [Header("Rarity")]
        [SerializeField]
        private RunUpgradeRarity rarity =
            RunUpgradeRarity.Common;

        [Header("Effect")]
        [SerializeField]
        private RunUpgradeEffectType effectType;

        [SerializeField]
        private float amount;

        public string Id =>
            string.IsNullOrWhiteSpace(id)
                ? name
                : id;

        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName)
                ? name
                : displayName;

        public string Description =>
            RunUpgradeDescriptionFormatter.Format(
                effectType,
                amount
            );

        public Sprite Icon =>
            icon;

        public RunUpgradeRarity Rarity =>
            rarity;

        public RunUpgradeEffectType EffectType =>
            effectType;

        public float Amount =>
            amount;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            amount = Mathf.Max(0f, amount);
        }
    }
}