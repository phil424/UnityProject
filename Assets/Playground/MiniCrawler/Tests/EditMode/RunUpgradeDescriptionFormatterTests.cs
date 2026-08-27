using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeDescriptionFormatterTests
    {
        [TestCase(
            RunUpgradeEffectType.FlatDamage,
            5f,
            "Deal 5 additional damage."
        )]
        [TestCase(
            RunUpgradeEffectType.MoveSpeedPercent,
            100f,
            "Move 100% faster."
        )]
        [TestCase(
            RunUpgradeEffectType.AttackSpeedPercent,
            100f,
            "Attack 100% faster."
        )]
        [TestCase(
            RunUpgradeEffectType.FlatArmour,
            2f,
            "Gain 2 armour."
        )]
        [TestCase(
            RunUpgradeEffectType.MoveSpeedPercent,
            12.5f,
            "Move 12.5% faster."
        )]
        public void Format_GeneratesExpectedDescription(
            RunUpgradeEffectType effectType,
            float amount,
            string expected
        )
        {
            string result =
                RunUpgradeDescriptionFormatter.Format(
                    effectType,
                    amount
                );

            Assert.That(
                result,
                Is.EqualTo(expected)
            );
        }

        [Test]
        public void DefinitionDescription_UsesEffectData()
        {
            RunUpgradeDefinition definition =
                TestRunUpgradeFactory.Create(
                    "TestDamage",
                    RunUpgradeEffectType.FlatDamage,
                    7f
                );

            Assert.That(
                definition.Description,
                Is.EqualTo(
                    "Deal 7 additional damage."
                )
            );

            Object.DestroyImmediate(definition);
        }
    }
}