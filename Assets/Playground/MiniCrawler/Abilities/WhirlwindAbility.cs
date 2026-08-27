using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(AutoTargetMover))]
    public class WhirlwindAbility : ActorAbility
    {
        [Header("Whirlwind")]
        [SerializeField]
        [Min(0.1f)]
        private float activationRadius = 2f;

        [SerializeField]
        [Min(0f)]
        private float damage = 10f;

        [SerializeField]
        [Min(0f)]
        private float knockbackDistance = 3f;

        [SerializeField]
        [Min(0f)]
        private float knockbackSpeed = 8f;

        public float ActivationRadius =>
            activationRadius;

        public float Damage =>
            damage;

        public float KnockbackDistance =>
            knockbackDistance;

        public float KnockbackSpeed =>
            knockbackSpeed;

        protected override bool CanActivateAbility()
        {
            AutoTargetMover mover =
                GetComponent<AutoTargetMover>();

            if (
                mover == null ||
                !mover.isActiveAndEnabled ||
                mover.CurrentIntent !=
                    TargetIntent.Combat
            )
            {
                return false;
            }

            Health target =
                mover.CurrentTarget;

            if (
                target == null ||
                target.IsDead
            )
            {
                return false;
            }

            Vector3 offset =
                target.transform.position -
                transform.position;

            offset.y = 0f;

            return
                offset.sqrMagnitude <=
                activationRadius *
                activationRadius;
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover =
                GetComponent<AutoTargetMover>();

            if (mover == null)
                return false;

            Health[] possibleTargets =
                FindObjectsByType<Health>(
                    FindObjectsSortMode.None
                );

            int hitCount = 0;
            int knockbackCount = 0;

            foreach (
                Health target in possibleTargets
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
                    gameObject,
                    target,
                    damage,
                    AbilityName
                );

                hitCount++;

                if (
                    !target.IsDead &&
                    KnockbackResolver.TryApply(
                        gameObject,
                        target,
                        knockbackDistance,
                        knockbackSpeed
                    )
                )
                {
                    knockbackCount++;
                }
            }

            Debug.Log(
                $"[Ability] {name} activated " +
                $"{AbilityName}, hit " +
                $"{hitCount} target(s) and " +
                $"knocked back " +
                $"{knockbackCount} target(s)."
            );

            return hitCount > 0;
        }

        private bool IsValidWhirlwindTarget(
            AutoTargetMover mover,
            Health target
        )
        {
            if (
                target == null ||
                target.IsDead ||
                target.gameObject ==
                    gameObject
            )
            {
                return false;
            }

            FactionMember faction =
                target.GetComponent<FactionMember>();

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
                transform.position;

            offset.y = 0f;

            return
                offset.sqrMagnitude <=
                activationRadius *
                activationRadius;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            activationRadius =
                Mathf.Max(
                    0.1f,
                    activationRadius
                );

            damage =
                Mathf.Max(
                    0f,
                    damage
                );

            knockbackDistance =
                Mathf.Max(
                    0f,
                    knockbackDistance
                );

            knockbackSpeed =
                Mathf.Max(
                    0f,
                    knockbackSpeed
                );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                activationRadius
            );
        }
    }
}