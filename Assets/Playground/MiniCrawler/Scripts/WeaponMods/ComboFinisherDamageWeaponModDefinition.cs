using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [CreateAssetMenu(
        fileName = "New Combo Finisher Damage Weapon Mod",
        menuName = "Mini Crawler/Weapon Mods/Combo Finisher Damage"
    )]
    public sealed class ComboFinisherDamageWeaponModDefinition : WeaponModDefinition
    {
        [Header("Finisher")]
        [SerializeField, Min(0f)] private float bonusDamagePercent = 100f;
        [SerializeField, Min(0f)] private float bonusDamagePercentPerLevel = 50f;

        public float BonusDamagePercent => bonusDamagePercent;
        public float BonusDamagePercentPerLevel => bonusDamagePercentPerLevel;

        public override bool IsConfigured => base.IsConfigured && bonusDamagePercent > 0f;

        public override float ModifyBasicAttackDamage(BasicAttackWeaponModContext context, float currentDamage)
        {
            if (!context.IsComboFinisher)
                return currentDamage;

            float totalBonusPercent = bonusDamagePercent + bonusDamagePercentPerLevel * context.ModLevel;
            float multiplier = 1f + totalBonusPercent / 100f;

            return Mathf.Max(0f, currentDamage * multiplier);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            bonusDamagePercent = Mathf.Max(0f, bonusDamagePercent);
            bonusDamagePercentPerLevel = Mathf.Max(0f, bonusDamagePercentPerLevel);
        }
    }
}