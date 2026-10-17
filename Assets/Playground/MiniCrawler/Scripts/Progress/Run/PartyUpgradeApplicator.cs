using MiniCrawler.Abilities;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class PartyUpgradeApplicator
    {
        public static void Apply(GameObject spawnedActor, PartyMemberDefinition definition, RunBuild build, bool restoreHealth = true)
        {
            if (spawnedActor == null || definition == null || build == null)
                return;

            CombatStats combatStats = spawnedActor.GetComponent<CombatStats>();
            Health health = spawnedActor.GetComponent<Health>();
            SupportStats supportStats = spawnedActor.GetComponent<SupportStats>();
            AutoTargetMover mover = spawnedActor.GetComponent<AutoTargetMover>();
            AbilityStats abilityStats = spawnedActor.GetComponent<AbilityStats>();

            if (abilityStats == null)
                abilityStats = spawnedActor.AddComponent<AbilityStats>();

            KnockbackStats knockbackStats = spawnedActor.GetComponent<KnockbackStats>();

            if (knockbackStats == null)
                knockbackStats = spawnedActor.AddComponent<KnockbackStats>();

            ShieldCapacityStats shieldCapacityStats = spawnedActor.GetComponent<ShieldCapacityStats>();

            if (shieldCapacityStats != null && spawnedActor.GetComponent<HitShieldRuntime>() == null)
            {
                spawnedActor.AddComponent<HitShieldRuntime>();
            }

            float gearDamageBonus = build.WeaponLevel * definition.DamagePerWeaponLevel;
            float gearArmourBonus = build.ArmourLevel * definition.ArmourPerArmourLevel;
            float healthBonus = build.ArmourLevel * definition.HealthPerArmourLevel + build.FlatMaxHealthBonus;
            float healingBonus = build.FocusLevel * definition.HealingPerFocusLevel;

            if (combatStats != null)
            {
                combatStats.ApplyGearBonuses(gearDamageBonus, gearArmourBonus);
                combatStats.ApplyRunBonuses(build.FlatBasicAttackDamageBonus, build.BasicAttackSpeedPercentBonus, build.FlatArmourBonus);
            }

            if (mover != null)
            {
                mover.ApplyRunMoveSpeedBonus(build.MoveSpeedPercentBonus);
            }

            abilityStats.ApplyRunCooldownRecoveryBonus(build.AbilityCooldownRecoveryPercentBonus);
            abilityStats.ApplyRunAbilityDamageBonus(build.AbilityDamagePercentBonus);
            abilityStats.ApplyRunAbilityAreaSizeBonus(build.AbilityAreaSizePercentBonus);

            knockbackStats.ApplyRunPowerBonus(build.KnockbackPowerPercentBonus);

            if (shieldCapacityStats != null)
            {
                shieldCapacityStats.ApplyRunCapacityBonus(build.ShieldCapacityBonus);
            }

            if (health != null)
            {
                health.ApplyMaxHealthBonus(healthBonus, restoreHealth);
            }

            if (supportStats != null)
            {
                supportStats.ApplyHealingBonus(healingBonus);
            }
        }
    }
}
