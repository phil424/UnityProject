using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunProgressTests
    {
        private PartyMemberDefinition punchy;

        [SetUp]
        public void SetUp()
        {
            RunProgress.EndRun();

            punchy =
                TestPartyMemberFactory.Create(
                    "Punchy"
                );
        }

        [TearDown]
        public void TearDown()
        {
            RunProgress.EndRun();

            Object.DestroyImmediate(punchy);
        }

        [Test]
        public void BeginRun_WithoutSelectedParty_Fails()
        {
            RunStartConfiguration configuration =
                new RunStartConfiguration(
                    new PartyMemberDefinition[0]
                );

            bool started =
                RunProgress.BeginRun(
                    configuration
                );

            Assert.That(
                started,
                Is.False
            );

            Assert.That(
                RunProgress.HasActiveRun,
                Is.False
            );
        }

        [Test]
        public void EndRun_DestroysRunState()
        {
            BeginPunchyRun();

            RunProgress.AddCurrency(10);

            RunProgress
                .GetBuild(punchy)
                .Increase(GearSlot.Weapon);

            Assert.That(
                RunProgress.Currency,
                Is.EqualTo(10)
            );

            Assert.That(
                RunProgress
                    .GetBuild(punchy)
                    .WeaponLevel,
                Is.EqualTo(1)
            );

            RunProgress.EndRun();

            Assert.That(
                RunProgress.HasActiveRun,
                Is.False
            );

            Assert.That(
                RunProgress.Currency,
                Is.Zero
            );
        }

        [Test]
        public void NewRun_GetsFreshBuildAndCurrency()
        {
            BeginPunchyRun();

            RunProgress.AddCurrency(25);

            RunProgress
                .GetBuild(punchy)
                .Increase(GearSlot.Weapon);

            RunProgress
                .GetBuild(punchy)
                .Increase(GearSlot.Armour);

            RunProgress.EndRun();

            BeginPunchyRun();

            RunBuild newBuild =
                RunProgress.GetBuild(punchy);

            Assert.That(
                newBuild.WeaponLevel,
                Is.Zero
            );

            Assert.That(
                newBuild.ArmourLevel,
                Is.Zero
            );

            Assert.That(
                newBuild.FocusLevel,
                Is.Zero
            );

            Assert.That(
                RunProgress.Currency,
                Is.Zero
            );
        }

        [Test]
        public void ApplyRunUpgrade_DuringActiveRun_UpdatesCurrentBuild()
        {
            BeginPunchyRun();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            bool applied =
                RunProgress.TryApplyRunUpgrade(
                    punchy,
                    upgrade
                );

            Assert.That(
                applied,
                Is.True
            );

            Assert.That(
                RunProgress
                    .GetBuild(punchy)
                    .FlatDamageBonus,
                Is.EqualTo(5f)
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_WithoutActiveRun_IsRejected()
        {
            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            bool applied =
                RunProgress.TryApplyRunUpgrade(
                    punchy,
                    upgrade
                );

            Assert.That(
                applied,
                Is.False
            );

            Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void ApplyRunUpgrade_Success_RaisesChanged()
        {
            BeginPunchyRun();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            int changeCount = 0;

            void OnChanged()
            {
                changeCount++;
            }

            RunProgress.Changed += OnChanged;

            RunProgress.TryApplyRunUpgrade(
                punchy,
                upgrade
            );

            RunProgress.Changed -= OnChanged;

            Assert.That(
                changeCount,
                Is.EqualTo(1)
            );

            Object.DestroyImmediate(upgrade);
        }

        private void BeginPunchyRun()
        {
            RunStartConfiguration configuration =
                new RunStartConfiguration(
                    new[] { punchy }
                );

            Assert.That(
                RunProgress.BeginRun(configuration),
                Is.True
            );
        }
    }
}