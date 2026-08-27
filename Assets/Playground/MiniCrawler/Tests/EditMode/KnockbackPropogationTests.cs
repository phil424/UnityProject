using MiniCrawler.Core;
using MiniCrawler.Movement;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class KnockbackPropagationTests
    {
        private GameObject sourceObject;
        private GameObject firstTargetObject;
        private GameObject secondTargetObject;

        private ForcedMotion sourceMotion;
        private ForcedMotion firstTargetMotion;
        private ForcedMotion secondTargetMotion;

        private Health firstTargetHealth;
        private Health secondTargetHealth;

        [SetUp]
        public void SetUp()
        {
            sourceObject =
                CreateActor(
                    "Source"
                );

            firstTargetObject =
                CreateActor(
                    "First Target"
                );

            secondTargetObject =
                CreateActor(
                    "Second Target"
                );

            sourceMotion =
                sourceObject
                    .GetComponent<ForcedMotion>();

            firstTargetMotion =
                firstTargetObject
                    .GetComponent<ForcedMotion>();

            secondTargetMotion =
                secondTargetObject
                    .GetComponent<ForcedMotion>();

            firstTargetHealth =
                firstTargetObject
                    .GetComponent<Health>();

            secondTargetHealth =
                secondTargetObject
                    .GetComponent<Health>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(
                sourceObject
            );

            Object.DestroyImmediate(
                firstTargetObject
            );

            Object.DestroyImmediate(
                secondTargetObject
            );
        }

        [Test]
        public void TryPropagate_UsesReducedRemainingDistanceAndSpeed()
        {
            sourceMotion.StartMotion(
                Vector3.right,
                4f,
                8f
            );

            sourceMotion.ConsumeDisplacement(
                0.25f
            );

            Assert.That(
                sourceMotion.RemainingDistance,
                Is.EqualTo(2f)
                    .Within(0.001f)
            );

            bool propagated =
                KnockbackPropagation.TryPropagate(
                    sourceMotion,
                    firstTargetHealth,
                    0.5f,
                    0.5f
                );

            Assert.That(
                propagated,
                Is.True
            );

            Assert.That(
                firstTargetMotion.IsActive,
                Is.True
            );

            Assert.That(
                firstTargetMotion.RemainingDistance,
                Is.EqualTo(1f)
                    .Within(0.001f)
            );

            Assert.That(
                firstTargetMotion.Speed,
                Is.EqualTo(4f)
                    .Within(0.001f)
            );

            Assert.That(
                Vector3.Distance(
                    firstTargetMotion.Direction,
                    Vector3.right
                ),
                Is.LessThan(0.001f)
            );
        }

        [Test]
        public void TryPropagate_ConsumesSourceOpportunity()
        {
            sourceMotion.StartMotion(
                Vector3.right,
                4f,
                8f
            );

            bool firstPropagation =
                KnockbackPropagation.TryPropagate(
                    sourceMotion,
                    firstTargetHealth,
                    0.5f,
                    0.5f
                );

            bool secondPropagation =
                KnockbackPropagation.TryPropagate(
                    sourceMotion,
                    secondTargetHealth,
                    0.5f,
                    0.5f
                );

            Assert.That(
                firstPropagation,
                Is.True
            );

            Assert.That(
                secondPropagation,
                Is.False
            );

            Assert.That(
                sourceMotion.PropagationAvailable,
                Is.False
            );

            Assert.That(
                secondTargetMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void TryPropagate_PropagatedMotionCannotPropagateAgain()
        {
            sourceMotion.StartMotion(
                Vector3.right,
                4f,
                8f
            );

            bool firstPropagation =
                KnockbackPropagation.TryPropagate(
                    sourceMotion,
                    firstTargetHealth,
                    0.5f,
                    0.5f
                );

            bool chainedPropagation =
                KnockbackPropagation.TryPropagate(
                    firstTargetMotion,
                    secondTargetHealth,
                    0.5f,
                    0.5f
                );

            Assert.That(
                firstPropagation,
                Is.True
            );

            Assert.That(
                chainedPropagation,
                Is.False
            );

            Assert.That(
                firstTargetMotion
                    .PropagationAvailable,
                Is.False
            );

            Assert.That(
                secondTargetMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void TryPropagate_FailedTargetDoesNotConsumeSourceOpportunity()
        {
            sourceMotion.StartMotion(
                Vector3.forward,
                4f,
                8f
            );

            GameObject invalidTarget =
                new GameObject(
                    "Invalid Target"
                );

            invalidTarget
                .AddComponent<Actor>();

            Health invalidHealth =
                invalidTarget
                    .AddComponent<Health>();

            try
            {
                bool propagated =
                    KnockbackPropagation
                        .TryPropagate(
                            sourceMotion,
                            invalidHealth,
                            0.5f,
                            0.5f
                        );

                Assert.That(
                    propagated,
                    Is.False
                );

                Assert.That(
                    sourceMotion
                        .PropagationAvailable,
                    Is.True
                );
            }
            finally
            {
                Object.DestroyImmediate(
                    invalidTarget
                );
            }
        }

        [Test]
        public void StartMotion_NewDirectMotionRefreshesPropagationOpportunity()
        {
            sourceMotion.StartMotion(
                Vector3.right,
                4f,
                8f
            );

            bool propagated =
                KnockbackPropagation.TryPropagate(
                    sourceMotion,
                    firstTargetHealth,
                    0.5f,
                    0.5f
                );

            Assert.That(
                propagated,
                Is.True
            );

            Assert.That(
                sourceMotion.PropagationAvailable,
                Is.False
            );

            bool restarted =
                sourceMotion.StartMotion(
                    Vector3.forward,
                    2f,
                    4f
                );

            Assert.That(
                restarted,
                Is.True
            );

            Assert.That(
                sourceMotion.PropagationAvailable,
                Is.True
            );
        }

        private GameObject CreateActor(
            string objectName
        )
        {
            GameObject actorObject =
                new GameObject(
                    objectName
                );

            actorObject
                .AddComponent<Actor>();

            actorObject
                .AddComponent<Health>();

            actorObject
                .AddComponent<ForcedMotion>();

            return actorObject;
        }
    }
}