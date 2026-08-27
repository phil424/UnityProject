using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeDefinitionTests
    {
        [Test]
        public void NewDefinition_DefaultsToCommonRarity()
        {
            RunUpgradeDefinition definition =
                ScriptableObject.CreateInstance<RunUpgradeDefinition>();

            Assert.That(
                definition.Rarity,
                Is.EqualTo(RunUpgradeRarity.Common)
            );

            Object.DestroyImmediate(definition);
        }

        [Test]
        public void Rarity_ReturnsConfiguredRarity()
        {
            RunUpgradeDefinition definition =
                TestRunUpgradeFactory.Create(
                    "RareUpgrade",
                    RunUpgradeEffectType.FlatDamage,
                    5f,
                    RunUpgradeRarity.Rare
                );

            Assert.That(
                definition.Rarity,
                Is.EqualTo(RunUpgradeRarity.Rare)
            );

            Object.DestroyImmediate(definition);
        }
    }
}