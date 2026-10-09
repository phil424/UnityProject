using MiniCrawler.Combat;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [CreateAssetMenu(fileName = "New Knockback Projectile Weapon Mod", menuName = "Mini Crawler/Weapon Mods/Knockback Projectile")]
    public sealed class KnockbackProjectileWeaponModDefinition : WeaponModDefinition
    {
        [Header("Knockback Projectile")]
        [SerializeField, Min(0.1f)]
        private float baseKnockbackDistance = 2.5f;

        [SerializeField, Min(0f)]
        private float knockbackDistancePerLevel = 0.75f;

        [SerializeField, Min(0.1f)]
        private float knockbackSpeed = 12f;

        [Header("Designed Combination")]
        [SerializeField]
        private PiercingProjectileWeaponModDefinition piercingCombinationSource;

        public float BaseKnockbackDistance => baseKnockbackDistance;
        public float KnockbackDistancePerLevel => knockbackDistancePerLevel;
        public float KnockbackSpeed => knockbackSpeed;

        public override bool IsConfigured => base.IsConfigured && baseKnockbackDistance > 0f && knockbackSpeed > 0f;

        public float GetKnockbackDistance(int modLevel)
        {
            return Mathf.Max(0f, baseKnockbackDistance + knockbackDistancePerLevel * Mathf.Max(0, modLevel));
        }

        public override bool TryResolveProjectileBasicAttack(
            BasicAttackWeaponModContext context,
            ProjectileCombatActionDefinition projectileAction,
            float currentDamage,
            string actionName,
            out bool connected
        )
        {
            connected = false;

            if (!context.UsesProjectile)
                return false;

            if (context.Source == null || context.Target == null || context.Target.IsDead || projectileAction?.Projectile == null)
            {
                return true;
            }

            if (TryResolvePiercingCombination(context, projectileAction, currentDamage, actionName, out connected))
            {
                return true;
            }

            ProjectileImpactPayload impact = new ProjectileImpactPayload(currentDamage, actionName).WithKnockback(GetKnockbackDistance(context.ModLevel), knockbackSpeed);

            ProjectileLaunchRequest request = new(projectileAction.Projectile, context.Source, ProjectileDelivery.Targeted(context.Target), impact);

            connected = ProjectileLauncher.Launch(request) != null;
            return true;
        }

        private bool TryResolvePiercingCombination(BasicAttackWeaponModContext context, ProjectileCombatActionDefinition projectileAction, float currentDamage, string actionName, out bool connected)
        {
            connected = false;

            if (piercingCombinationSource == null || !context.TryGetCombinedWeaponModLevel(piercingCombinationSource, out int piercingLevel))
            {
                return false;
            }

            AutoTargetMover mover = context.Source.GetComponent<AutoTargetMover>();
            CombatStats stats = context.Source.GetComponent<CombatStats>();

            if (mover == null || stats == null || string.IsNullOrWhiteSpace(mover.TargetFactionId))
            {
                Debug.LogWarning($"[Weapon Mod] {DisplayName} could not resolve its Piercing combination for '{context.Source.name}'. " + "Falling back to normal Impact delivery.", context.Source);

                return false;
            }

            ProjectileDelivery delivery = ProjectileDelivery.TargetedPiercing(
                context.Target,
                stats.AttackRange,
                mover.TargetFactionId,
                piercingCombinationSource.HitRadius,
                piercingCombinationSource.GetMaximumTargetsHit(piercingLevel)
            );

            ProjectileImpactPayload impact = new ProjectileImpactPayload(currentDamage, actionName).WithKnockback(GetKnockbackDistance(context.ModLevel), knockbackSpeed);

            ProjectileLaunchRequest request = new(projectileAction.Projectile, context.Source, delivery, impact);

            connected = ProjectileLauncher.Launch(request) != null;
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
