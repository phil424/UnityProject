using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeOfferGeneratorTests
    {
        private PartyMemberDefinition punchy;
        private PartyMemberDefinition healer;

        private RunUpgradeDefinition damage;
        private RunUpgradeDefinition movement;
        private RunUpgradeDefinition attackSpeed;
        private RunUpgradeDefinition armour;

        [SetUp]
        public void SetUp()
        {
            punchy =
                TestPartyMemberFactory.Create(
                    "Punchy"
                );

            healer =
                TestPartyMemberFactory.Create(
                    "Healer"
                );

            damage =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            movement =
                TestRunUpgradeFactory.Create(
                    "FleetFooted",
                    RunUpgradeEffectType.MoveSpeedPercent,
                    10f
                );

            attackSpeed =
                TestRunUpgradeFactory.Create(
                    "BattleRhythm",
                    RunUpgradeEffectType.AttackSpeedPercent,
                    10f
                );

            armour =
                TestRunUpgradeFactory.Create(
                    "ReinforcedPlate",
                    RunUpgradeEffectType.FlatArmour,
                    1f
                );
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(punchy);
            Object.DestroyImmediate(healer);

            Object.DestroyImmediate(damage);
            Object.DestroyImmediate(movement);
            Object.DestroyImmediate(attackSpeed);
            Object.DestroyImmediate(armour);
        }

        [Test]
        public void Generate_ReturnsRequestedNumberOfOffers()
        {
            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    new[]
                    {
                        punchy,
                        healer
                    },
                    AllUpgrades(),
                    3
                );

            Assert.That(
                offers.Count,
                Is.EqualTo(3)
            );
        }

        [Test]
        public void Generate_ReturnsOnlyValidPartyAndUpgradeCombinations()
        {
            PartyMemberDefinition[] party =
            {
                punchy,
                healer
            };

            RunUpgradeDefinition[] upgrades =
                AllUpgrades();

            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    party,
                    upgrades,
                    3
                );

            foreach (
                RunUpgradeOffer offer
                in offers
            )
            {
                Assert.That(
                    offer.IsValid,
                    Is.True
                );

                Assert.That(
                    party,
                    Does.Contain(offer.Member)
                );

                Assert.That(
                    upgrades,
                    Does.Contain(offer.Upgrade)
                );
            }
        }

        [Test]
        public void Generate_DoesNotReturnDuplicateCombinations()
        {
            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    new[]
                    {
                        punchy,
                        healer
                    },
                    AllUpgrades(),
                    3
                );

            HashSet<string> combinations =
                new HashSet<string>();

            foreach (
                RunUpgradeOffer offer
                in offers
            )
            {
                string key =
                    offer.Member.Id +
                    "|" +
                    offer.Upgrade.Id;

                Assert.That(
                    combinations.Add(key),
                    Is.True,
                    $"Duplicate offer generated: {key}"
                );
            }
        }

        [Test]
        public void Generate_WhenRequestExceedsCandidateCount_ReturnsAllCandidates()
        {
            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    new[] { punchy },
                    new[]
                    {
                        damage,
                        movement
                    },
                    10
                );

            Assert.That(
                offers.Count,
                Is.EqualTo(2)
            );
        }

        [Test]
        public void Generate_WithNoParty_ReturnsNoOffers()
        {
            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    new PartyMemberDefinition[0],
                    AllUpgrades(),
                    3
                );

            Assert.That(
                offers,
                Is.Empty
            );
        }

        private RunUpgradeDefinition[] AllUpgrades()
        {
            return new[]
            {
                damage,
                movement,
                attackSpeed,
                armour
            };
        }
    }
}