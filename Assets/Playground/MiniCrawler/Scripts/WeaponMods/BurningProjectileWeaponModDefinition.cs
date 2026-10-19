using MiniCrawler.Combat;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [CreateAssetMenu(fileName = "New Burning Projectile Weapon Mod", menuName = "Mini Crawler/Weapon Mods/Burning Projectile")]
    public sealed class BurningProjectileWeaponModDefinition : WeaponModDefinition
    {
        [Header("Burn")]
        [SerializeField, Min(0.1f)]
        private float baseBurnDamagePerTick = 3f;

        [SerializeField, Min(0f)]
        private float burnDamagePerLevel = 2f;

        [SerializeField, Min(0.05f)]
        private float burnTickInterval = 1f;

        [SerializeField, Min(0.1f)]
        private float burnDuration = 3f;

        [Header("Designed Combination")]
        [SerializeField]
        private PiercingProjectileWeaponModDefinition piercingCombinationSource;

        public float BaseBurnDamagePerTick => baseBurnDamagePerTick;
        public float BurnDamagePerLevel => burnDamagePerLevel;
        public float BurnTickInterval => burnTickInterval;
        public float BurnDuration => burnDuration;

        public override bool IsConfigured => base.IsConfigured && baseBurnDamagePerTick > 0f && burnTickInterval > 0f && burnDuration >= burnTickInterval;

        public float GetBurnDamagePerTick(int modLevel)
        {
            return Mathf.Max(0f, baseBurnDamagePerTick + burnDamagePerLevel * Mathf.Max(0, modLevel));
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

            ProjectileImpactPayload impact = new ProjectileImpactPayload(currentDamage, actionName).WithBurn(
                GetBurnDamagePerTick(context.ModLevel),
                burnTickInterval,
                burnDuration,
                $"{DisplayName} Burn"
            );

            ProjectileLaunchRequest request = new(projectileAction.Projectile, context.Source, ProjectileDelivery.Targeted(context.Target), impact, isBasicAttack: true);

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
                Debug.LogWarning($"[Weapon Mod] {DisplayName} could not resolve its Piercing combination for " + $"'{context.Source.name}'. Falling back to normal Fire delivery.", context.Source);

                return false;
            }

            ProjectileDelivery delivery = ProjectileDelivery.TargetedPiercing(
                context.Target,
                stats.AttackRange,
                mover.TargetFactionId,
                piercingCombinationSource.HitRadius,
                piercingCombinationSource.GetMaximumTargetsHit(piercingLevel)
            );

            ProjectileImpactPayload impact = new ProjectileImpactPayload(currentDamage, actionName).WithBurn(
                GetBurnDamagePerTick(context.ModLevel),
                burnTickInterval,
                burnDuration,
                $"{DisplayName} Burn"
            );

            ProjectileLaunchRequest request = new(projectileAction.Projectile, context.Source, delivery, impact, isBasicAttack: true);

            connected = ProjectileLauncher.Launch(request) != null;
            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            baseBurnDamagePerTick = Mathf.Max(0.1f, baseBurnDamagePerTick);
            burnDamagePerLevel = Mathf.Max(0f, burnDamagePerLevel);
            burnTickInterval = Mathf.Max(0.05f, burnTickInterval);
            burnDuration = Mathf.Max(burnTickInterval, burnDuration);
        }
    }
}
