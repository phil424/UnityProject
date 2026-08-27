using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunUpgradeLifetimeTests
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

            Object.DestroyImmediate(
                punchy
            );
        }

        [Test]
        public void RunUpgrade_SameRunBuildSurvivesFreshActorApplication()
        {
            BeginPunchyRun();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            Assert.That(
                RunProgress.TryApplyRunUpgrade(
                    punchy,
                    upgrade
                ),
                Is.True
            );

            RunBuild firstLevelBuild =
                RunProgress.GetBuild(
                    punchy
                );

            GameObject firstActor =
                CreateActor(
                    "Level 1 Punchy"
                );

            CombatStats firstStats =
                firstActor.GetComponent<CombatStats>();

            PartyUpgradeApplicator.Apply(
                firstActor,
                punchy,
                firstLevelBuild
            );

            Assert.That(
                firstStats.Damage,
                Is.EqualTo(10f)
                    .Within(0.001f)
            );

            Object.DestroyImmediate(
                firstActor
            );

            RunBuild secondLevelBuild =
                RunProgress.GetBuild(
                    punchy
                );

            Assert.That(
                secondLevelBuild,
                Is.SameAs(firstLevelBuild)
            );

            Assert.That(
                secondLevelBuild.FlatDamageBonus,
                Is.EqualTo(5f)
            );

            GameObject secondActor =
                CreateActor(
                    "Level 2 Punchy"
                );

            CombatStats secondStats =
                secondActor.GetComponent<CombatStats>();

            PartyUpgradeApplicator.Apply(
                secondActor,
                punchy,
                secondLevelBuild
            );

            Assert.That(
                secondStats.Damage,
                Is.EqualTo(10f)
                    .Within(0.001f)
            );

            Object.DestroyImmediate(
                secondActor
            );

            Object.DestroyImmediate(
                upgrade
            );
        }

        [Test]
        public void RunUpgrade_EndRunThenNewRun_StartsFresh()
        {
            BeginPunchyRun();

            RunUpgradeDefinition upgrade =
                TestRunUpgradeFactory.Create(
                    "SharpenedBlade",
                    RunUpgradeEffectType.FlatDamage,
                    5f
                );

            RunProgress.TryApplyRunUpgrade(
                punchy,
                upgrade
            );

            RunBuild oldBuild =
                RunProgress.GetBuild(
                    punchy
                );

            Assert.That(
                oldBuild.FlatDamageBonus,
                Is.EqualTo(5f)
            );

            RunProgress.EndRun();

            BeginPunchyRun();

            RunBuild newBuild =
                RunProgress.GetBuild(
                    punchy
                );

            Assert.That(
                newBuild,
                Is.Not.SameAs(oldBuild)
            );

            Assert.That(
                newBuild.FlatDamageBonus,
                Is.Zero
            );

            Assert.That(
                newBuild.MoveSpeedPercentBonus,
                Is.Zero
            );

            Assert.That(
                newBuild.AttackSpeedPercentBonus,
                Is.Zero
            );

            Assert.That(
                newBuild.FlatArmourBonus,
                Is.Zero
            );

            GameObject newRunActor =
                CreateActor(
                    "New Run Punchy"
                );

            CombatStats newRunStats =
                newRunActor.GetComponent<CombatStats>();

            PartyUpgradeApplicator.Apply(
                newRunActor,
                punchy,
                newBuild
            );

            Assert.That(
                newRunStats.Damage,
                Is.EqualTo(5f)
                    .Within(0.001f)
            );

            Object.DestroyImmediate(
                newRunActor
            );

            Object.DestroyImmediate(
                upgrade
            );
        }

        private void BeginPunchyRun()
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

        private static GameObject CreateActor(
            string name
        )
        {
            GameObject actor =
                new GameObject(name);

            actor.AddComponent<CombatStats>();

            return actor;
        }
    }
}