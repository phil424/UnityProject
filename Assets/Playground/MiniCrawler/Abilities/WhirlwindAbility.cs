using System;
using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public class WhirlwindAbility :
        ActorAbility
    {
        private const float
            MinimumDirectionSquared =
                0.0001f;

        private enum
            WhirlwindEvolutionBehaviour
        {
            Vortex
        }

        [Serializable]
        private sealed class
            EvolutionBehaviourBinding
        {
            [SerializeField]
            private AbilityEvolutionDefinition
                evolution;

            [SerializeField]
            private WhirlwindEvolutionBehaviour
                behaviour;

            public AbilityEvolutionDefinition
                Evolution =>
                    evolution;

            public WhirlwindEvolutionBehaviour
                Behaviour =>
                    behaviour;
        }

        [Header("Whirlwind")]
        [SerializeField]
        private AbilityLevelValue
            activationRadius;

        [SerializeField]
        private AbilityLevelValue
            damage;

        [SerializeField]
        private AbilityLevelValue
            knockbackDistance;

        [SerializeField]
        private AbilityLevelValue
            knockbackSpeed;

        [Header("Evolution Behaviours")]
        [SerializeField]
        private List<
            EvolutionBehaviourBinding
        > evolutionBehaviours = new();

        public float ActivationRadius =>
            Mathf.Max(
                0.1f,
                activationRadius != null
                    ? activationRadius
                        .Evaluate(Level)
                    : 0.1f
            );

        public float Damage =>
            Mathf.Max(
                0f,
                damage != null
                    ? damage.Evaluate(Level)
                    : 0f
            );

        public float KnockbackDistance =>
            Mathf.Max(
                0f,
                knockbackDistance != null
                    ? knockbackDistance
                        .Evaluate(Level)
                    : 0f
            );

        public float KnockbackSpeed =>
            Mathf.Max(
                0f,
                knockbackSpeed != null
                    ? knockbackSpeed
                        .Evaluate(Level)
                    : 0f
            );

        public bool UsesVortex =>
            HasEvolutionBehaviour(
                WhirlwindEvolutionBehaviour
                    .Vortex
            );

        protected override bool
            CanActivateAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover =
                Owner.GetComponent<
                    AutoTargetMover
                >();

            if (
                mover == null ||
                !mover.isActiveAndEnabled
            )
            {
                return false;
            }

            Health[] possibleTargets =
                FindObjectsByType<Health>(
                    FindObjectsSortMode.None
                );

            foreach (
                Health target
                    in possibleTargets
            )
            {
                if (
                    IsValidWhirlwindTarget(
                        mover,
                        target
                    )
                )
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool
            ExecuteAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover =
                Owner.GetComponent<
                    AutoTargetMover
                >();

            if (mover == null)
                return false;

            Health[] possibleTargets =
                FindObjectsByType<Health>(
                    FindObjectsSortMode.None
                );

            bool useVortex =
                UsesVortex;

            int hitCount = 0;

            int forcedMotionCount = 0;

            foreach (
                Health target
                    in possibleTargets
            )
            {
                if (
                    !IsValidWhirlwindTarget(
                        mover,
                        target
                    )
                )
                {
                    continue;
                }

                DamageResolver.ApplyDamage(
                    Owner,
                    target,
                    Damage,
                    AbilityName
                );

                hitCount++;

                if (target.IsDead)
                    continue;

                bool moved =
                    useVortex
                        ? TryApplyVortexPull(
                            target
                        )
                        : KnockbackResolver
                            .TryApply(
                                Owner,
                                target,
                                KnockbackDistance,
                                KnockbackSpeed
                            );

                if (moved)
                {
                    forcedMotionCount++;
                }
            }

            string movementDescription =
                useVortex
                    ? "pulled"
                    : "knocked back";

            Debug.Log(
                $"[Ability] {Owner.name} " +
                $"activated {AbilityName}, hit " +
                $"{hitCount} target(s) and " +
                $"{movementDescription} " +
                $"{forcedMotionCount} target(s)."
            );

            return hitCount > 0;
        }

        private bool HasEvolutionBehaviour(
            WhirlwindEvolutionBehaviour
                behaviour
        )
        {
            foreach (
                EvolutionBehaviourBinding binding
                    in evolutionBehaviours
            )
            {
                if (
                    binding == null ||
                    binding.Evolution == null ||
                    binding.Behaviour !=
                        behaviour
                )
                {
                    continue;
                }

                if (
                    HasEvolution(
                        binding.Evolution
                    )
                )
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryApplyVortexPull(
            Health target
        )
        {
            if (
                Owner == null ||
                target == null ||
                target.IsDead
            )
            {
                return false;
            }

            Vector3 towardOwner =
                Owner.transform.position -
                target.transform.position;

            towardOwner.y = 0f;

            float distanceToOwner =
                towardOwner.magnitude;

            if (
                distanceToOwner <=
                MinimumDirectionSquared
            )
            {
                return false;
            }

            float stopDistance =
                GetVortexStopDistance(
                    target
                );

            float availablePullDistance =
                Mathf.Max(
                    0f,
                    distanceToOwner -
                    stopDistance
                );

            float pullDistance =
                Mathf.Min(
                    KnockbackDistance,
                    availablePullDistance
                );

            if (pullDistance <= 0f)
                return false;

            return
                KnockbackResolver
                    .TryApplyDirectional(
                        Owner,
                        target,
                        towardOwner.normalized,
                        pullDistance,
                        KnockbackSpeed
                    );
        }

        private float GetVortexStopDistance(
            Health target
        )
        {
            float stopDistance = 0.1f;

            EntityAvoidance ownerAvoidance =
                Owner != null
                    ? Owner.GetComponent<
                        EntityAvoidance
                    >()
                    : null;

            if (ownerAvoidance != null)
            {
                stopDistance +=
                    ownerAvoidance.WorldRadius;
            }

            EntityAvoidance targetAvoidance =
                target != null
                    ? target.GetComponent<
                        EntityAvoidance
                    >()
                    : null;

            if (targetAvoidance != null)
            {
                stopDistance +=
                    targetAvoidance.WorldRadius;
            }

            return stopDistance;
        }

        private bool IsValidWhirlwindTarget(
            AutoTargetMover mover,
            Health target
        )
        {
            if (
                Owner == null ||
                target == null ||
                target.IsDead ||
                target.gameObject == Owner
            )
            {
                return false;
            }

            FactionMember faction =
                target.GetComponent<
                    FactionMember
                >();

            if (
                faction == null ||
                !faction.IsFaction(
                    mover.TargetFactionId
                )
            )
            {
                return false;
            }

            Vector3 offset =
                target.transform.position -
                Owner.transform.position;

            offset.y = 0f;

            return
                offset.sqrMagnitude <=
                ActivationRadius *
                ActivationRadius;
        }

        protected override void
            OnValidate()
        {
            base.OnValidate();
        }

        private void
            OnDrawGizmosSelected()
        {
            Vector3 centre =
                Owner != null
                    ? Owner.transform.position
                    : transform.position;

            Gizmos.DrawWireSphere(
                centre,
                ActivationRadius
            );
        }
    }
}