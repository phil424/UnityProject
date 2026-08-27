using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunBuildTests
    {
        [Test]
        public void NewBuild_StartsWithNoUpgrades()
        {
            RunBuild build = new RunBuild();

            Assert.That(build.WeaponLevel, Is.Zero);
            Assert.That(build.ArmourLevel, Is.Zero);
            Assert.That(build.FocusLevel, Is.Zero);

            Assert.That(build.FlatDamageBonus, Is.Zero);
            Assert.That(build.MoveSpeedPercentBonus, Is.Zero);
            Assert.That(build.AttackSpeedPercentBonus, Is.Zero);
            Assert.That(build.FlatArmourBonus, Is.Zero);
        }

        [TestCase(GearSlot.Weapon, 1, 0, 0)]
        [TestCase(GearSlot.Armour, 0, 1, 0)]
        [TestCase(GearSlot.Focus, 0, 0, 1)]
        public void Increase_OnlyChangesRequestedSlot(
            GearSlot slot,
            int expectedWeapon,
            int expectedArmour,
            int expectedFocus
        )
        {
            RunBuild build = new RunBuild();

            build.Increase(slot);

            Assert.That(
                build.WeaponLevel,
                Is.EqualTo(expectedWeapon)
            );

            Assert.That(
                build.ArmourLevel,
                Is.EqualTo(expectedArmour)
            );

            Assert.That(
                build.FocusLevel,
                Is.EqualTo(expectedFocus)
            );
        }

        [TestCase(
            RunUpgradeEffectType.FlatDamage,
            5f,
            5f, 0f, 0f, 0f
        )]
        [TestCase(
            RunUpgradeEffectType.MoveSpeedPercent,
            10f,
            0f, 10f, 0f, 0f
        )]
        [TestCase(
            RunUpgradeEffectType.AttackSpeedPercent,
            10f,
            0f, 0f, 10f, 0f
        )]
        [TestCase(
            RunUpgradeEffectType.FlatArmour,
            1f,
            0f, 0f, 0f, 1f
        )]
        public void ApplyRunUpgrade_OnlyChangesRequestedModifier(
            RunUpgradeEffectType effectType,
            float amount,
            float expectedDamage,
            float expectedMoveSpeed,
            float expectedAttackSpeed,
            float expectedArmour
        )
        {
            RunBuild build = new RunBuild();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "TestUpgrade",
                    effectType,
                    amount
                );

            build.ApplyRunUpgrade(upgrade);

            Assert.That(
                build.FlatDamageBonus,
                Is.EqualTo(expectedDamage)
            );

            Assert.That(
                build.MoveSpeedPercentBonus,
                Is.EqualTo(expectedMoveSpeed)
            );

            Assert.That(
                build.AttackSpeedPercentBonus,
                Is.EqualTo(expectedAttackSpeed)
            );

            Assert.That(
                build.FlatArmourBonus,
                Is.EqualTo(expectedArmour)
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_SameEffectStacks()
        {
            RunBuild build = new RunBuild();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            build.ApplyRunUpgrade(upgrade);
            build.ApplyRunUpgrade(upgrade);

            Assert.That(
                build.FlatDamageBonus,
                Is.EqualTo(10f)
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_DoesNotChangeGearLevels()
        {
            RunBuild build = new RunBuild();

            build.Increase(GearSlot.Weapon);
            build.Increase(GearSlot.Armour);
            build.Increase(GearSlot.Focus);

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            build.ApplyRunUpgrade(upgrade);

            Assert.That(
                build.WeaponLevel,
                Is.EqualTo(1)
            );

            Assert.That(
                build.ArmourLevel,
                Is.EqualTo(1)
            );

            Assert.That(
                build.FocusLevel,
                Is.EqualTo(1)
            );

            Assert.That(
                build.FlatDamageBonus,
                Is.EqualTo(5f)
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_WithNullUpgrade_DoesNothing()
        {
            RunBuild build = new RunBuild();

            build.ApplyRunUpgrade(null);

            Assert.That(build.FlatDamageBonus, Is.Zero);
            Assert.That(build.MoveSpeedPercentBonus, Is.Zero);
            Assert.That(build.AttackSpeedPercentBonus, Is.Zero);
            Assert.That(build.FlatArmourBonus, Is.Zero);
        }
    }
}