using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class PartyUpgradeApplicator
    {
        public static void Apply(
            GameObject spawnedActor,
            PartyMemberDefinition definition,
            RunBuild build
        )
        {
            if (spawnedActor == null ||
                definition == null ||
                build == null)
            {
                return;
            }

            CombatStats combatStats =
                spawnedActor.GetComponent<CombatStats>();

            Health health =
                spawnedActor.GetComponent<Health>();

            SupportStats supportStats =
                spawnedActor.GetComponent<SupportStats>();

            AutoTargetMover mover =
                spawnedActor.GetComponent<AutoTargetMover>();

            float gearDamageBonus =
                build.WeaponLevel *
                definition.DamagePerWeaponLevel;

            float gearArmourBonus =
                build.ArmourLevel *
                definition.ArmourPerArmourLevel;

            float healthBonus =
                build.ArmourLevel *
                definition.HealthPerArmourLevel;

            float healingBonus =
                build.FocusLevel *
                definition.HealingPerFocusLevel;

            if (combatStats != null)
            {
                combatStats.ApplyGearBonuses(
                    gearDamageBonus,
                    gearArmourBonus
                );

                combatStats.ApplyRunBonuses(
                    build.FlatDamageBonus,
                    build.AttackSpeedPercentBonus,
                    build.FlatArmourBonus
                );
            }

            if (mover != null)
            {
                mover.ApplyRunMoveSpeedBonus(
                    build.MoveSpeedPercentBonus
                );
            }

            if (health != null)
            {
                health.ApplyMaxHealthBonus(
                    healthBonus
                );
            }

            if (supportStats != null)
            {
                supportStats.ApplyHealingBonus(
                    healingBonus
                );
            }
        }
    }
}