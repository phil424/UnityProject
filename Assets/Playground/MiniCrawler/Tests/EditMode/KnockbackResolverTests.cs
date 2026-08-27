using MiniCrawler.Core;
using MiniCrawler.Movement;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class KnockbackResolverTests
    {
        private GameObject sourceObject;
        private GameObject targetObject;

        private Health targetHealth;
        private ForcedMotion forcedMotion;

        [SetUp]
        public void SetUp()
        {
            sourceObject =
                new GameObject(
                    "Source"
                );

            targetObject =
                new GameObject(
                    "Target"
                );

            targetObject
                .AddComponent<Actor>();

            targetHealth =
                targetObject
                    .AddComponent<Health>();

            forcedMotion =
                targetObject
                    .AddComponent<ForcedMotion>();
        }

        [TearDown]
        public void TearDown()
        {
            if (sourceObject != null)
            {
                Object.DestroyImmediate(
                    sourceObject
                );
            }

            if (targetObject != null)
            {
                Object.DestroyImmediate(
                    targetObject
                );
            }
        }

        [Test]
        public void TryApply_UsesHorizontalDirectionAwayFromSource()
        {
            sourceObject.transform.position =
                new Vector3(
                    0f,
                    0f,
                    0f
                );

            targetObject.transform.position =
                new Vector3(
                    3f,
                    10f,
                    4f
                );

            bool applied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    2.5f,
                    7f
                );

            Assert.That(
                applied,
                Is.True
            );

            Assert.That(
                forcedMotion.IsActive,
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
                Is.EqualTo(2.5f)
                    .Within(0.001f)
            );

            Assert.That(
                forcedMotion.Speed,
                Is.EqualTo(7f)
                    .Within(0.001f)
            );
        }

        [Test]
        public void TryApply_WithNullSource_IsRejected()
        {
            bool applied =
                KnockbackResolver.TryApply(
                    null,
                    targetHealth,
                    2f,
                    5f
                );

            Assert.That(
                applied,
                Is.False
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void TryApply_WithNullTarget_IsRejected()
        {
            bool applied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    null,
                    2f,
                    5f
                );

            Assert.That(
                applied,
                Is.False
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );
        }

        [Test]
        public void TryApply_WithInvalidMotionValues_IsRejected()
        {
            bool zeroDistance =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    0f,
                    5f
                );

            bool zeroSpeed =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    2f,
                    0f
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
        public void TryApply_WhenSourceAndTargetOverlap_UsesSourceForward()
        {
            sourceObject.transform.position =
                Vector3.zero;

            targetObject.transform.position =
                Vector3.zero;

            sourceObject.transform.rotation =
                Quaternion.LookRotation(
                    Vector3.right
                );

            bool applied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    2f,
                    5f
                );

            Assert.That(
                applied,
                Is.True
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.True
            );

            Assert.That(
                Vector3.Distance(
                    forcedMotion.Direction,
                    Vector3.right
                ),
                Is.LessThan(0.001f)
            );
        }

        [Test]
        public void TryApply_TargetWithoutForcedMotion_IsRejected()
        {
            GameObject otherTarget =
                new GameObject(
                    "Target Without Forced Motion"
                );

            otherTarget
                .AddComponent<Actor>();

            Health otherHealth =
                otherTarget
                    .AddComponent<Health>();

            otherTarget.transform.position =
                Vector3.forward;

            try
            {
                bool applied =
                    KnockbackResolver.TryApply(
                        sourceObject,
                        otherHealth,
                        2f,
                        5f
                    );

                Assert.That(
                    applied,
                    Is.False
                );
            }
            finally
            {
                Object.DestroyImmediate(
                    otherTarget
                );
            }
        }

        [Test]
        public void TryApply_WhileAlreadyMoving_ReplacesWithNewestRequest()
        {
            sourceObject.transform.position =
                Vector3.zero;

            targetObject.transform.position =
                Vector3.forward;

            bool firstApplied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    5f,
                    4f
                );

            Assert.That(
                firstApplied,
                Is.True
            );

            forcedMotion.ConsumeDisplacement(
                0.5f
            );

            Assert.That(
                forcedMotion.RemainingDistance,
                Is.EqualTo(3f)
                    .Within(0.001f)
            );

            sourceObject.transform.position =
                targetObject.transform.position -
                Vector3.right;

            bool secondApplied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    2f,
                    8f
                );

            Assert.That(
                secondApplied,
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
        public void TryApply_ToDeadTarget_IsRejected()
        {
            SerializedObject serializedHealth =
                new SerializedObject(
                    targetHealth
                );

            SerializedProperty destroyWhenDead =
                serializedHealth.FindProperty(
                    "destroyWhenDead"
                );

            destroyWhenDead.boolValue =
                false;

            serializedHealth.ApplyModifiedPropertiesWithoutUndo();

            targetHealth.Damage(
                targetHealth.MaxHealth
            );

            Assert.That(
                targetHealth.IsDead,
                Is.True
            );

            bool applied =
                KnockbackResolver.TryApply(
                    sourceObject,
                    targetHealth,
                    2f,
                    5f
                );

            Assert.That(
                applied,
                Is.False
            );

            Assert.That(
                forcedMotion.IsActive,
                Is.False
            );
        }
    }
}