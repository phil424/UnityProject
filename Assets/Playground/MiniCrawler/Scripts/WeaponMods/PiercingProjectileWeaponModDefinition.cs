using MiniCrawler.Combat;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [CreateAssetMenu(
        fileName = "New Piercing Projectile Weapon Mod",
        menuName = "Mini Crawler/Weapon Mods/Piercing Projectile"
    )]
    public sealed class PiercingProjectileWeaponModDefinition : WeaponModDefinition
    {
        [Header("Piercing Projectile")]
        [SerializeField, Min(0.05f)] private float hitRadius = 0.3f;
        [SerializeField, Min(2)] private int baseMaximumTargetsHit = 2;
        [SerializeField, Min(0)] private int additionalTargetsPerLevel = 1;

        public float HitRadius => hitRadius;
        public int BaseMaximumTargetsHit => baseMaximumTargetsHit;
        public int AdditionalTargetsPerLevel => additionalTargetsPerLevel;

        public override bool IsConfigured => base.IsConfigured && hitRadius > 0f && baseMaximumTargetsHit >= 2;

        public int GetMaximumTargetsHit(int modLevel)
        {
            return Mathf.Max(2, baseMaximumTargetsHit + additionalTargetsPerLevel * Mathf.Max(0, modLevel));
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

            if (context.Source == null || context.Target == null || context.Target.IsDead || projectileAction?.Projectile == null)
                return true;

            AutoTargetMover mover = context.Source.GetComponent<AutoTargetMover>();
            CombatStats stats = context.Source.GetComponent<CombatStats>();

            if (mover == null || stats == null || string.IsNullOrWhiteSpace(mover.TargetFactionId))
            {
                Debug.LogWarning($"[Weapon Mod] {DisplayName} could not resolve projectile targeting for '{context.Source.name}'.", context.Source);

                return true;
            }

            ProjectileRuntime runtime = ProjectileRuntime.SpawnTargetedPiercing(
                projectileAction.Projectile,
                context.Source,
                context.Target,
                currentDamage,
                actionName,
                stats.AttackRange,
                mover.TargetFactionId,
                hitRadius,
                GetMaximumTargetsHit(context.ModLevel)
            );

            connected = runtime != null;
            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            hitRadius = Mathf.Max(0.05f, hitRadius);
            baseMaximumTargetsHit = Mathf.Max(2, baseMaximumTargetsHit);
            additionalTargetsPerLevel = Mathf.Max(0, additionalTargetsPerLevel);
        }
    }
}