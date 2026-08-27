using MiniCrawler.Core;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class HealthTests
    {
        private GameObject actorObject;
        private Health health;

        [SetUp]
        public void SetUp()
        {
            actorObject =
                new GameObject(
                    "Test Actor"
                );

            actorObject
                .AddComponent<Actor>();

            health =
                actorObject
                    .AddComponent<Health>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(
                actorObject
            );
        }

        [Test]
        public void Damage_UpdatesNormalizedHealth()
        {
            health.Damage(25f);

            Assert.That(
                health.CurrentHealth,
                Is.EqualTo(75f)
            );

            Assert.That(
                health.Normalized,
                Is.EqualTo(0.75f)
                    .Within(0.001f)
            );
        }

        [Test]
        public void Damage_RaisesChangedEvent()
        {
            int changedCount = 0;

            health.Changed +=
                _ => changedCount++;

            health.Damage(25f);

            Assert.That(
                changedCount,
                Is.EqualTo(1)
            );
        }

        [Test]
        public void Heal_RaisesChangedEvent()
        {
            health.Damage(25f);

            int changedCount = 0;

            health.Changed +=
                _ => changedCount++;

            health.Heal(10f);

            Assert.That(
                health.CurrentHealth,
                Is.EqualTo(85f)
            );

            Assert.That(
                changedCount,
                Is.EqualTo(1)
            );
        }
    }
}