using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class PartyUpgradeApplicatorTests
    {
        [Test]
        public void Apply_CombinesGearAndRunCombatBonuses()
        {
            GameObject actorObject =
                new GameObject("Test Actor");

            CombatStats combatStats =
                actorObject.AddComponent<CombatStats>();

            AutoTargetMover mover =
                actorObject.AddComponent<AutoTargetMover>();

            PartyMemberDefinition definition =
                ScriptableObject.CreateInstance<PartyMemberDefinition>();

            RunBuild build =
                new RunBuild();

            build.Increase(GearSlot.Weapon);
            build.Increase(GearSlot.Armour);

            RunUpgradeDefinition damageUpgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            RunUpgradeDefinition moveSpeedUpgrade =
                TestRunUpgradeFactory.Create(
                    "FleetFooted",
                    RunUpgradeEffectType.MoveSpeedPercent,
                    10f
                );

            RunUpgradeDefinition attackSpeedUpgrade =
                TestRunUpgradeFactory.Create(
                    "BattleRhythm",
                    RunUpgradeEffectType.AttackSpeedPercent,
                    10f
                );

            RunUpgradeDefinition armourUpgrade =
                TestRunUpgradeFactory.Create(
                    "ReinforcedPlate",
                    RunUpgradeEffectType.FlatArmour,
                    1f
                );

            build.ApplyRunUpgrade(damageUpgrade);
            build.ApplyRunUpgrade(moveSpeedUpgrade);
            build.ApplyRunUpgrade(attackSpeedUpgrade);
            build.ApplyRunUpgrade(armourUpgrade);

            PartyUpgradeApplicator.Apply(
                actorObject,
                definition,
                build
            );

            Assert.That(
                combatStats.Damage,
                Is.EqualTo(12f).Within(0.001f)
            );

            Assert.That(
                combatStats.FlatArmour,
                Is.EqualTo(2f).Within(0.001f)
            );

            Assert.That(
                combatStats.AttackCooldown,
                Is.EqualTo(1f / 1.1f).Within(0.001f)
            );

            Assert.That(
                mover.MoveSpeed,
                Is.EqualTo(2.2f).Within(0.001f)
            );

            Object.DestroyImmediate(damageUpgrade);
            Object.DestroyImmediate(moveSpeedUpgrade);
            Object.DestroyImmediate(attackSpeedUpgrade);
            Object.DestroyImmediate(armourUpgrade);
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(actorObject);
        }

        [Test]
        public void Apply_WithStackedRunUpgrades_UsesTotalBuildModifiers()
        {
            GameObject actorObject =
                new GameObject("Test Actor");

            CombatStats combatStats =
                actorObject.AddComponent<CombatStats>();

            AutoTargetMover mover =
                actorObject.AddComponent<AutoTargetMover>();

            PartyMemberDefinition definition =
                ScriptableObject.CreateInstance<PartyMemberDefinition>();

            RunBuild build =
                new RunBuild();

            RunUpgradeDefinition damageUpgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            RunUpgradeDefinition moveSpeedUpgrade =
                TestRunUpgradeFactory.Create(
                    "FleetFooted",
                    RunUpgradeEffectType.MoveSpeedPercent,
                    10f
                );

            RunUpgradeDefinition attackSpeedUpgrade =
                TestRunUpgradeFactory.Create(
                    "BattleRhythm",
                    RunUpgradeEffectType.AttackSpeedPercent,
                    10f
                );

            RunUpgradeDefinition armourUpgrade =
                TestRunUpgradeFactory.Create(
                    "ReinforcedPlate",
                    RunUpgradeEffectType.FlatArmour,
                    1f
                );

            build.ApplyRunUpgrade(damageUpgrade);
            build.ApplyRunUpgrade(damageUpgrade);

            build.ApplyRunUpgrade(moveSpeedUpgrade);
            build.ApplyRunUpgrade(moveSpeedUpgrade);

            build.ApplyRunUpgrade(attackSpeedUpgrade);
            build.ApplyRunUpgrade(attackSpeedUpgrade);

            build.ApplyRunUpgrade(armourUpgrade);
            build.ApplyRunUpgrade(armourUpgrade);

            PartyUpgradeApplicator.Apply(
                actorObject,
                definition,
                build
            );

            Assert.That(
                combatStats.Damage,
                Is.EqualTo(15f).Within(0.001f)
            );

            Assert.That(
                combatStats.FlatArmour,
                Is.EqualTo(2f).Within(0.001f)
            );

            Assert.That(
                combatStats.AttackCooldown,
                Is.EqualTo(1f / 1.2f).Within(0.001f)
            );

            Assert.That(
                mover.MoveSpeed,
                Is.EqualTo(2.4f).Within(0.001f)
            );

            Object.DestroyImmediate(damageUpgrade);
            Object.DestroyImmediate(moveSpeedUpgrade);
            Object.DestroyImmediate(attackSpeedUpgrade);
            Object.DestroyImmediate(armourUpgrade);
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(actorObject);
        }

        [Test]
        public void Apply_ReapplyingSameBuild_DoesNotDoubleBonuses()
        {
            GameObject actorObject =
                new GameObject("Test Actor");

            CombatStats combatStats =
                actorObject.AddComponent<CombatStats>();

            AutoTargetMover mover =
                actorObject.AddComponent<AutoTargetMover>();

            PartyMemberDefinition definition =
                ScriptableObject.CreateInstance<PartyMemberDefinition>();

            RunBuild build =
                new RunBuild();

            RunUpgradeDefinition damageUpgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            RunUpgradeDefinition moveSpeedUpgrade =
                TestRunUpgradeFactory.Create(
                    "FleetFooted",
                    RunUpgradeEffectType.MoveSpeedPercent,
                    10f
                );

            build.ApplyRunUpgrade(damageUpgrade);
            build.ApplyRunUpgrade(moveSpeedUpgrade);

            PartyUpgradeApplicator.Apply(
                actorObject,
                definition,
                build
            );

            PartyUpgradeApplicator.Apply(
                actorObject,
                definition,
                build
            );

            Assert.That(
                combatStats.Damage,
                Is.EqualTo(10f).Within(0.001f)
            );

            Assert.That(
                mover.MoveSpeed,
                Is.EqualTo(2.2f).Within(0.001f)
            );

            Object.DestroyImmediate(damageUpgrade);
            Object.DestroyImmediate(moveSpeedUpgrade);
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(actorObject);
        }
    }
}