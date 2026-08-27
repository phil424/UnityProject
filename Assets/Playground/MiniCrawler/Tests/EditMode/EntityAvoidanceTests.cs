using MiniCrawler.Core;
using MiniCrawler.Movement;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class EntityAvoidanceTests
    {
        private GameObject actorObject;
        private EntityAvoidance avoidance;

        [SetUp]
        public void SetUp()
        {
            actorObject =
                new GameObject(
                    "Avoidance Test Actor"
                );

            actorObject
                .AddComponent<Actor>();

            avoidance =
                actorObject
                    .AddComponent<EntityAvoidance>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(
                actorObject
            );
        }

        [Test]
        public void WorldRadius_AtUnitScale_EqualsBaseRadius()
        {
            actorObject.transform.localScale =
                Vector3.one;

            Assert.That(
                avoidance.WorldRadius,
                Is.EqualTo(
                    avoidance.Radius
                ).Within(0.001f)
            );
        }

        [Test]
        public void WorldRadius_ScalesWithLargestHorizontalScale()
        {
            actorObject.transform.localScale =
                new Vector3(
                    2f,
                    2f,
                    1.5f
                );

            Assert.That(
                avoidance.WorldRadius,
                Is.EqualTo(
                    avoidance.Radius * 2f
                ).Within(0.001f)
            );
        }

        [Test]
        public void WorldRadius_DoesNotUseVerticalScale()
        {
            actorObject.transform.localScale =
                new Vector3(
                    1f,
                    5f,
                    1f
                );

            Assert.That(
                avoidance.WorldRadius,
                Is.EqualTo(
                    avoidance.Radius
                ).Within(0.001f)
            );
        }
    }
}