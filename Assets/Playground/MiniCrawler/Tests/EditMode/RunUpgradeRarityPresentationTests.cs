using System.Collections.Generic;
using MiniCrawler.Progress;
using MiniCrawler.UI;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeRarityPresentationTests
    {
        [TestCase(
            RunUpgradeRarity.Common,
            "COMMON"
        )]
        [TestCase(
            RunUpgradeRarity.Uncommon,
            "UNCOMMON"
        )]
        [TestCase(
            RunUpgradeRarity.Rare,
            "RARE"
        )]
        [TestCase(
            RunUpgradeRarity.Epic,
            "EPIC"
        )]
        [TestCase(
            RunUpgradeRarity.Legendary,
            "LEGENDARY"
        )]
        public void GetLabel_ReturnsExpectedLabel(
            RunUpgradeRarity rarity,
            string expected
        )
        {
            Assert.That(
                RunUpgradeRarityPresentation
                    .GetLabel(rarity),
                Is.EqualTo(expected)
            );
        }

        [Test]
        public void EveryRarity_HasDistinctColour()
        {
            HashSet<Color32> colours =
                new();

            foreach (
                RunUpgradeRarity rarity
                in System.Enum.GetValues(
                    typeof(RunUpgradeRarity)
                )
            )
            {
                Color32 colour =
                    RunUpgradeRarityPresentation
                        .GetColor(rarity);

                colours.Add(colour);
            }

            Assert.That(
                colours.Count,
                Is.EqualTo(5)
            );
        }
    }
}