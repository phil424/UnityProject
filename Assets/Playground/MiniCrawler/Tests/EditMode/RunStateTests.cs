using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunStateTests
    {
        private PartyMemberDefinition punchy;
        private PartyMemberDefinition healer;

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
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(punchy);
            Object.DestroyImmediate(healer);
        }

        [Test]
        public void Constructor_CopiesStartingParty()
        {
            List<PartyMemberDefinition> startingParty =
                new()
                {
                    punchy,
                    healer
                };

            RunStartConfiguration configuration =
                new RunStartConfiguration(
                    startingParty
                );

            RunState state =
                new RunState(configuration);

            startingParty.Clear();

            Assert.That(
                state.SelectedParty.Count,
                Is.EqualTo(2)
            );

            Assert.That(
                state.IsSelected(punchy),
                Is.True
            );

            Assert.That(
                state.IsSelected(healer),
                Is.True
            );
        }

        [Test]
        public void EachPartyMember_HasIndependentBuild()
        {
            RunState state =
                CreateRunState(
                    punchy,
                    healer
                );

            RunBuild punchyBuild =
                state.GetBuild(punchy);

            RunBuild healerBuild =
                state.GetBuild(healer);

            punchyBuild.Increase(
                GearSlot.Weapon
            );

            RunUpgradeDefinition damageUpgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            punchyBuild.ApplyRunUpgrade(
                damageUpgrade
            );

            Assert.That(
                punchyBuild.WeaponLevel,
                Is.EqualTo(1)
            );

            Assert.That(
                healerBuild.WeaponLevel,
                Is.Zero
            );

            Assert.That(
                punchyBuild.FlatDamageBonus,
                Is.EqualTo(5f)
            );

            Assert.That(
                healerBuild.FlatDamageBonus,
                Is.Zero
            );

            Assert.That(
                punchyBuild,
                Is.Not.SameAs(healerBuild)
            );

            Object.DestroyImmediate(
                damageUpgrade
            );
        }

        [Test]
        public void Currency_StartsAtZero_AndOnlyAddsPositiveAmounts()
        {
            RunState state =
                CreateRunState(punchy);

            Assert.That(
                state.Currency,
                Is.Zero
            );

            state.AddCurrency(10);

            Assert.That(
                state.Currency,
                Is.EqualTo(10)
            );

            state.AddCurrency(0);
            state.AddCurrency(-10);

            Assert.That(
                state.Currency,
                Is.EqualTo(10)
            );
        }

        [Test]
        public void BuyUpgrade_DeductsCurrencyAndIncreasesBuild()
        {
            RunState state =
                CreateRunState(punchy);

            state.AddCurrency(20);

            Assert.That(
                state.GetUpgradeCost(
                    punchy,
                    GearSlot.Weapon
                ),
                Is.EqualTo(5)
            );

            bool purchased =
                state.TryBuyUpgrade(
                    punchy,
                    GearSlot.Weapon
                );

            Assert.That(
                purchased,
                Is.True
            );

            Assert.That(
                state.Currency,
                Is.EqualTo(15)
            );

            Assert.That(
                state
                    .GetBuild(punchy)
                    .WeaponLevel,
                Is.EqualTo(1)
            );

            Assert.That(
                state.GetUpgradeCost(
                    punchy,
                    GearSlot.Weapon
                ),
                Is.EqualTo(10)
            );
        }

        [Test]
        public void BuyUpgrade_WithInsufficientCurrency_DoesNothing()
        {
            RunState state =
                CreateRunState(punchy);

            state.AddCurrency(4);

            bool purchased =
                state.TryBuyUpgrade(
                    punchy,
                    GearSlot.Weapon
                );

            Assert.That(
                purchased,
                Is.False
            );

            Assert.That(
                state.Currency,
                Is.EqualTo(4)
            );

            Assert.That(
                state
                    .GetBuild(punchy)
                    .WeaponLevel,
                Is.Zero
            );
        }

        [Test]
        public void ApplyRunUpgrade_ToSelectedMember_UpdatesTheirBuild()
        {
            RunState state =
                CreateRunState(
                    punchy,
                    healer
                );

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            bool applied =
                state.TryApplyRunUpgrade(
                    punchy,
                    upgrade
                );

            Assert.That(
                applied,
                Is.True
            );

            Assert.That(
                state
                    .GetBuild(punchy)
                    .FlatDamageBonus,
                Is.EqualTo(5f)
            );

            Assert.That(
                state
                    .GetBuild(healer)
                    .FlatDamageBonus,
                Is.Zero
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_ToUnselectedMember_IsRejected()
        {
            RunState state =
                CreateRunState(punchy);

            PartyMemberDefinition outsider =
                TestPartyMemberFactory.Create(
                    "Outsider"
                );

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            bool applied =
                state.TryApplyRunUpgrade(
                    outsider,
                    upgrade
                );

            Assert.That(
                applied,
                Is.False
            );

            Assert.That(
                state
                    .GetBuild(punchy)
                    .FlatDamageBonus,
                Is.Zero
            );

            Object.DestroyImmediate(upgrade);
            Object.DestroyImmediate(outsider);
        }

        private static RunState CreateRunState(
            params PartyMemberDefinition[] party
        )
        {
            RunStartConfiguration configuration =
                new RunStartConfiguration(party);

            return new RunState(configuration);
        }
    }
}