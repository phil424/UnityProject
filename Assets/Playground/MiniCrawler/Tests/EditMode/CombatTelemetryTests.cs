using MiniCrawler.Combat;
using MiniCrawler.Core;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class CombatTelemetryTests
    {
        private GameObject telemetryObject;
        private CombatTelemetry telemetry;

        private GameObject partyObject;
        private GameObject enemyObject;

        private Health partyHealth;
        private Health enemyHealth;

        [SetUp]
        public void SetUp()
        {
            telemetryObject =
                new GameObject(
                    "CombatTelemetry"
                );

            telemetry =
                telemetryObject
                    .AddComponent<CombatTelemetry>();

            partyObject =
                new GameObject(
                    "Party"
                );

            partyObject
                .AddComponent<Actor>();

            partyObject
                .AddComponent<PartyMember>();

            partyHealth =
                partyObject
                    .AddComponent<Health>();

            enemyObject =
                new GameObject(
                    "Enemy"
                );

            enemyObject
                .AddComponent<Actor>();

            enemyHealth =
                enemyObject
                    .AddComponent<Health>();

            telemetry.Clear();

            // EditMode tests do not rely on the normal
            // MonoBehaviour OnEnable subscription.
            //
            // Remove first so this remains safe even if
            // Unity happens to have subscribed it already.
            DamageResolver.DamageResolved -=
                telemetry.RecordResolvedDamage;

            DamageResolver.DamageResolved +=
                telemetry.RecordResolvedDamage;
        }

        [TearDown]
        public void TearDown()
        {
            DamageResolver.DamageResolved -=
                telemetry.RecordResolvedDamage;

            Object.DestroyImmediate(
                partyObject
            );

            Object.DestroyImmediate(
                enemyObject
            );

            Object.DestroyImmediate(
                telemetryObject
            );
        }

        [Test]
        public void PartyDamage_CountsAsOutgoingDps()
        {
            DamageResolver.ApplyDamage(
                partyObject,
                enemyHealth,
                12f,
                "Test Attack"
            );

            Assert.That(
                telemetry.OutgoingDps,
                Is.EqualTo(4f)
                    .Within(0.001f)
            );

            Assert.That(
                telemetry.IncomingDps,
                Is.EqualTo(0f)
                    .Within(0.001f)
            );
        }

        [Test]
        public void DamageToParty_CountsAsIncomingDps()
        {
            DamageResolver.ApplyDamage(
                enemyObject,
                partyHealth,
                9f,
                "Test Attack"
            );

            Assert.That(
                telemetry.OutgoingDps,
                Is.EqualTo(0f)
                    .Within(0.001f)
            );

            Assert.That(
                telemetry.IncomingDps,
                Is.EqualTo(3f)
                    .Within(0.001f)
            );
        }

        [Test]
        public void DamageResolved_AddsDetailedLogEntry()
        {
            DamageResolver.ApplyDamage(
                partyObject,
                enemyHealth,
                5f,
                "Test Attack"
            );

            Assert.That(
                telemetry.CombatLogCount,
                Is.EqualTo(1)
            );
        }
    }
}