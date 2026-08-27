using MiniCrawler.Core;
using MiniCrawler.Movement;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class ForcedMotionTests
    {
        private GameObject actorObject;
        private ForcedMotion forcedMotion;

        [SetUp]
        public void SetUp()
        {
            actorObject =
                new GameObject(
                    "Test Actor"
                );

            actorObject
                .AddComponent<Actor>();

            forcedMotion =
                actorObject
                    .AddComponent<ForcedMotion>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(
                actorObject
            );
        }

        [Test]
        public void StartMotion_NormalizesHorizontalDirection()
        {
            bool started =
                forcedMotion.StartMotion(
                    new Vector3(
                        3f,
                        10f,
                        4f
                    ),
                    5f,
                    10f
                );

            Assert.That(
                started,
                Is.True
            );

            Assert.That(
                Vector3.Distance(
                    forcedMotion.Direction,
                    new Vector3(
                        0.6f,
                        0f,
                        0.8f
                    )
                ),
                Is.LessThan(0.001f)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(5f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.Speed,
                Is.EqualTo(10f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.True
            );
        }

        [Test]
        public void ConsumeDisplacement_UsesSpeedAndDeltaTime()
        {
            forcedMotion.StartMotion(
                Vector3.forward,
                5f,
                4f
            );

            Vector3 displacement =
                forcedMotion.ConsumeDisplacement(
                    0.5f
                );

            Assert.That(
                Vector3.Distance(
                    displacement,
                    Vector3.forward * 2f
                ),
                Is.LessThan(0.001f)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(3f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.True
            );
        }

        [Test]
        public void ConsumeDisplacement_DoesNotOvershoot()
        {
            forcedMotion.StartMotion(
                Vector3.right,
                1f,
                10f
            );

            Vector3 displacement =
                forcedMotion.ConsumeDisplacement(
                    0.5f
                );

            Assert.That(
                Vector3.Distance(
                    displacement,
                    Vector3.right
                ),
                Is.LessThan(0.001f)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.Zero
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void StartMotion_WithInvalidValues_IsRejected()
        {
            bool zeroDirection =
                forcedMotion.StartMotion(
                    Vector3.zero,
                    1f,
                    1f
                );

            bool zeroDistance =
                forcedMotion.StartMotion(
                    Vector3.right,
                    0f,
                    1f
                );

            bool zeroSpeed =
                forcedMotion.StartMotion(
                    Vector3.right,
                    1f,
                    0f
                );

            Assert.That(
                zeroDirection,
                Is.False
            );

            Assert.That(
                zeroDistance,
                Is.False
            );

            Assert.That(
                zeroSpeed,
                Is.False
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void Clear_EndsActiveMotion()
        {
            forcedMotion.StartMotion(
                Vector3.forward,
                5f,
                4f
            );

            forcedMotion.Clear();

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );

            Assert.That(
                forcedMotion.Direction,
                Is.EqualTo(Vector3.zero)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.Zero
            );

            Assert.That(
                forcedMotion.Speed,
                Is.Zero
            );
        }

        [Test]
        public void StartMotion_WhileActive_ReplacesExistingMotion()
        {
            forcedMotion.StartMotion(
                Vector3.forward,
                5f,
                4f
            );

            forcedMotion.ConsumeDisplacement(
                0.5f
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(3f)
                    .Within(0.001f)
            );

            bool replaced =
                forcedMotion.StartMotion(
                    Vector3.right,
                    2f,
                    8f
                );

            Assert.That(
                replaced,
                Is.True
            );

            Assert.That(
                Vector3.Distance(
                    forcedMotion.Direction,
                    Vector3.right
                ),
                Is.LessThan(0.001f)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(2f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.Speed,
                Is.EqualTo(8f)
                    .Within(0.001f)
            );
        }

        [Test]
        public void InvalidStartMotion_DoesNotCancelExistingMotion()
        {
            forcedMotion.StartMotion(
                Vector3.forward,
                5f,
                4f
            );

            bool started =
                forcedMotion.StartMotion(
                    Vector3.zero,
                    10f,
                    10f
                );

            Assert.That(
                started,
                Is.False
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.True
            );

            Assert.That(
                Vector3.Distance(
                    forcedMotion.Direction,
                    Vector3.forward
                ),
                Is.LessThan(0.001f)
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(5f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.Speed,
                Is.EqualTo(4f)
                    .Within(0.001f)
            );
        }
    }
}