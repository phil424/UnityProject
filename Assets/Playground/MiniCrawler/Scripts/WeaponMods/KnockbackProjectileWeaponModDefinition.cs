using MiniCrawler.Combat;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [CreateAssetMenu(
        fileName = "New Knockback Projectile Weapon Mod",
        menuName = "Mini Crawler/Weapon Mods/Knockback Projectile"
    )]
    public sealed class KnockbackProjectileWeaponModDefinition : WeaponModDefinition
    {
        [Header("Knockback Projectile")]
        [SerializeField, Min(0.1f)] private float baseKnockbackDistance = 2.5f;
        [SerializeField, Min(0f)] private float knockbackDistancePerLevel = 0.75f;
        [SerializeField, Min(0.1f)] private float knockbackSpeed = 12f;

        public float BaseKnockbackDistance => baseKnockbackDistance;
        public float KnockbackDistancePerLevel => knockbackDistancePerLevel;
        public float KnockbackSpeed => knockbackSpeed;

        public override bool IsConfigured =>
            base.IsConfigured &&
            baseKnockbackDistance > 0f &&
            knockbackSpeed > 0f;

        public float GetKnockbackDistance(int modLevel)
        {
            return Mathf.Max(
                0f,
                baseKnockbackDistance +
                knockbackDistancePerLevel * Mathf.Max(0, modLevel)
            );
        }

        public override bool TryResolveProjectileBasicAttack(
            BasicAttackWeaponModContext context,
            ProjectileCombatActionDefinition projectileAction,
            float currentDamage,
            string actionName,
            out bool connected)
        {
            connected = false;

            if (!context.UsesProjectile)
                return false;

            if (context.Source == null ||
                context.Target == null ||
                context.Target.IsDead ||
                projectileAction?.Projectile == null)
            {
                return true;
            }

            ProjectileRuntime runtime = ProjectileRuntime.SpawnTargetedKnockback(
                projectileAction.Projectile,
                context.Source,
                context.Target,
                currentDamage,
                actionName,
                GetKnockbackDistance(context.ModLevel),
                knockbackSpeed
            );

            connected = runtime != null;
            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            baseKnockbackDistance = Mathf.Max(0.1f, baseKnockbackDistance);
            knockbackDistancePerLevel = Mathf.Max(0f, knockbackDistancePerLevel);
            knockbackSpeed = Mathf.Max(0.1f, knockbackSpeed);
        }
    }
}