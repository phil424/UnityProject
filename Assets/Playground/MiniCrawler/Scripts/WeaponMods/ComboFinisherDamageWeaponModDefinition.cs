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

        public float BonusDamagePercent => bonusDamagePercent;

        public override bool IsConfigured => base.IsConfigured && bonusDamagePercent > 0f;

        public override float ModifyBasicAttackDamage(BasicAttackWeaponModContext context, float currentDamage)
        {
            if (!context.IsComboFinisher)
                return currentDamage;

            float multiplier = 1f + bonusDamagePercent / 100f;
            return Mathf.Max(0f, currentDamage * multiplier);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            bonusDamagePercent = Mathf.Max(0f, bonusDamagePercent);
        }
    }
}