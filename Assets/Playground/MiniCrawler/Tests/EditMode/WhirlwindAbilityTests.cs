using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class WhirlwindAbilityTests
    {
        private GameObject ownerObject;
        private GameObject targetObject;

        private AutoTargetMover mover;
        private WhirlwindAbility ability;
        private Health targetHealth;

        [SetUp]
        public void SetUp()
        {
            ownerObject =
                new GameObject(
                    "Ability Owner"
                );

            ownerObject
                .AddComponent<Actor>();

            ownerObject
                .AddComponent<Health>();

            mover =
                ownerObject
                    .AddComponent<AutoTargetMover>();

            ability =
                ownerObject
                    .AddComponent<WhirlwindAbility>();

            targetObject =
                new GameObject(
                    "Ability Target"
                );

            targetObject
                .AddComponent<Actor>();

            targetHealth =
                targetObject
                    .AddComponent<Health>();

            ownerObject.transform.position =
                Vector3.zero;

            targetObject.transform.position =
                Vector3.right;

            mover.SetTarget(
                targetHealth,
                TargetIntent.Combat
            );
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(
                ownerObject
            );

            Object.DestroyImmediate(
                targetObject
            );
        }

        [Test]
        public void TryActivate_WithCombatTargetInsideRadius_Activates()
        {
            bool activatedEventRaised =
                false;

            ability.Activated +=
                _ => activatedEventRaised = true;

            bool activated =
                ability.TryActivate();

            Assert.That(
                activated,
                Is.True
            );

            Assert.That(
                activatedEventRaised,
                Is.True
            );

            Assert.That(
                ability.IsReady,
                Is.False
            );

            Assert.That(
                ability.CooldownRemaining,
                Is.EqualTo(
                    ability.Cooldown
                ).Within(0.001f)
            );
        }

        [Test]
        public void TryActivate_DuringCooldown_IsRejected()
        {
            bool firstActivation =
                ability.TryActivate();

            bool secondActivation =
                ability.TryActivate();

            Assert.That(
                firstActivation,
                Is.True
            );

            Assert.That(
                secondActivation,
                Is.False
            );
        }

        [Test]
        public void TickCooldown_WhenCooldownCompletes_AllowsActivationAgain()
        {
            ability.TryActivate();

            ability.TickCooldown(
                ability.Cooldown
            );

            Assert.That(
                ability.IsReady,
                Is.True
            );

            bool activatedAgain =
                ability.TryActivate();

            Assert.That(
                activatedAgain,
                Is.True
            );
        }

        [Test]
        public void TryActivate_WithTargetOutsideRadius_IsRejected()
        {
            targetObject.transform.position =
                Vector3.right *
                (ability.ActivationRadius + 1f);

            bool activated =
                ability.TryActivate();

            Assert.That(
                activated,
                Is.False
            );

            Assert.That(
                ability.IsReady,
                Is.True
            );
        }

        [Test]
        public void TryActivate_WithoutCombatIntent_IsRejected()
        {
            mover.SetTarget(
                targetHealth,
                TargetIntent.Support
            );

            bool activated =
                ability.TryActivate();

            Assert.That(
                activated,
                Is.False
            );

            Assert.That(
                ability.IsReady,
                Is.True
            );
        }

        [Test]
        public void TryActivate_WithoutTarget_IsRejected()
        {
            mover.ClearTarget();

            bool activated =
                ability.TryActivate();

            Assert.That(
                activated,
                Is.False
            );

            Assert.That(
                ability.IsReady,
                Is.True
            );
        }
    }
}