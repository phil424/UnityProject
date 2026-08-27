using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeOfferTests
    {
        private PartyMemberDefinition punchy;
        private PartyMemberDefinition healer;

        [SetUp]
        public void SetUp()
        {
            RunProgress.EndRun();

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
            RunProgress.EndRun();

            Object.DestroyImmediate(punchy);
            Object.DestroyImmediate(healer);
        }

        [Test]
        public void SetOffers_StoresValidSelectedMemberOffers()
        {
            BeginRun();

            RunUpgradeDefinition damageUpgrade =
                CreateDamageUpgrade();

            RunUpgradeOffer offer =
                new RunUpgradeOffer(
                    punchy,
                    damageUpgrade
                );

            RunProgress.SetRunUpgradeOffers(
                new[] { offer }
            );

            Assert.That(
                RunProgress.HasPendingUpgradeChoice,
                Is.True
            );

            Assert.That(
                RunProgress.PendingUpgradeOffers.Count,
                Is.EqualTo(1)
            );

            Assert.That(
                RunProgress.PendingUpgradeOffers[0],
                Is.SameAs(offer)
            );

            Object.DestroyImmediate(
                damageUpgrade
            );
        }

        [Test]
        public void SetOffers_RejectsMemberOutsideRun()
        {
            BeginPunchyOnlyRun();

            RunUpgradeDefinition damageUpgrade =
                CreateDamageUpgrade();

            RunUpgradeOffer offer =
                new RunUpgradeOffer(
                    healer,
                    damageUpgrade
                );

            RunProgress.SetRunUpgradeOffers(
                new[] { offer }
            );

            Assert.That(
                RunProgress.HasPendingUpgradeChoice,
                Is.False
            );

            Assert.That(
                RunProgress.PendingUpgradeOffers,
                Is.Empty
            );

            Object.DestroyImmediate(
                damageUpgrade
            );
        }

        [Test]
        public void ChooseOffer_AppliesUpgradeAndClearsAllOffers()
        {
            BeginRun();

            RunUpgradeDefinition damageUpgrade =
                CreateDamageUpgrade();

            RunUpgradeDefinition armourUpgrade =
                TestRunUpgradeFactory.Create(
                    "ReinforcedPlate",
                    RunUpgradeEffectType.FlatArmour,
                    1f
                );

            RunUpgradeOffer damageOffer =
                new RunUpgradeOffer(
                    punchy,
                    damageUpgrade
                );

            RunUpgradeOffer armourOffer =
                new RunUpgradeOffer(
                    healer,
                    armourUpgrade
                );

            RunProgress.SetRunUpgradeOffers(
                new[]
                {
                    damageOffer,
                    armourOffer
                }
            );

            bool chosen =
                RunProgress.TryChooseRunUpgrade(
                    damageOffer
                );

            Assert.That(
                chosen,
                Is.True
            );

            Assert.That(
                RunProgress
                    .GetBuild(punchy)
                    .FlatDamageBonus,
                Is.EqualTo(5f)
            );

            Assert.That(
                RunProgress
                    .GetBuild(healer)
                    .FlatArmourBonus,
                Is.Zero
            );

            Assert.That(
                RunProgress.HasPendingUpgradeChoice,
                Is.False
            );

            Assert.That(
                RunProgress.PendingUpgradeOffers,
                Is.Empty
            );

            Object.DestroyImmediate(
                damageUpgrade
            );

            Object.DestroyImmediate(
                armourUpgrade
            );
        }

        [Test]
        public void ChooseOffer_ThatWasNotOffered_IsRejected()
        {
            BeginRun();

            RunUpgradeDefinition damageUpgrade =
                CreateDamageUpgrade();

            RunUpgradeOffer offered =
                new RunUpgradeOffer(
                    punchy,
                    damageUpgrade
                );

            RunUpgradeOffer notOffered =
                new RunUpgradeOffer(
                    healer,
                    damageUpgrade
                );

            RunProgress.SetRunUpgradeOffers(
                new[] { offered }
            );

            bool chosen =
                RunProgress.TryChooseRunUpgrade(
                    notOffered
                );

            Assert.That(
                chosen,
                Is.False
            );

            Assert.That(
                RunProgress.HasPendingUpgradeChoice,
                Is.True
            );

            Assert.That(
                RunProgress
                    .GetBuild(healer)
                    .FlatDamageBonus,
                Is.Zero
            );

            Object.DestroyImmediate(
                damageUpgrade
            );
        }

        [Test]
        public void ChooseOffer_Success_RaisesChangedOnce()
        {
            BeginRun();

            RunUpgradeDefinition damageUpgrade =
                CreateDamageUpgrade();

            RunUpgradeOffer offer =
                new RunUpgradeOffer(
                    punchy,
                    damageUpgrade
                );

            RunProgress.SetRunUpgradeOffers(
                new[] { offer }
            );

            int changeCount = 0;

            void OnChanged()
            {
                changeCount++;
            }

            RunProgress.Changed += OnChanged;

            bool chosen =
                RunProgress.TryChooseRunUpgrade(
                    offer
                );

            RunProgress.Changed -= OnChanged;

            Assert.That(
                chosen,
                Is.True
            );

            Assert.That(
                changeCount,
                Is.EqualTo(1)
            );

            Object.DestroyImmediate(
                damageUpgrade
            );
        }

        private void BeginRun()
        {
            RunStartConfiguration configuration =
                new RunStartConfiguration(
                    new[]
                    {
                        punchy,
                        healer
                    }
                );

            Assert.That(
                RunProgress.BeginRun(
                    configuration
                ),
                Is.True
            );
        }

        private void BeginPunchyOnlyRun()
        {
            RunStartConfiguration configuration =
                new RunStartConfiguration(
                    new[] { punchy }
                );

            Assert.That(
                RunProgress.BeginRun(
                    configuration
                ),
                Is.True
            );
        }

        private static RunUpgradeDefinition CreateDamageUpgrade()
        {
            return TestRunUpgradeFactory.Create(
                "SharpenedBlade",
                RunUpgradeEffectType.FlatDamage,
                5f
            );
        }
    }
}