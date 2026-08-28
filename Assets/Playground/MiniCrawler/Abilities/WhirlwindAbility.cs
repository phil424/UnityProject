using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
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
            if (Owner == null)
                return false;

            AutoTargetMover mover =
                Owner.GetComponent<AutoTargetMover>();

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
                Health target in possibleTargets
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

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover =
                Owner.GetComponent<AutoTargetMover>();

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
                    Owner,
                    target,
                    damage,
                    AbilityName
                );

                hitCount++;

                if (
                    !target.IsDead &&
                    KnockbackResolver.TryApply(
                        Owner,
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
                $"[Ability] {Owner.name} activated " +
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
                Owner == null ||
                target == null ||
                target.IsDead ||
                target.gameObject ==
                    Owner
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
                Owner.transform.position;

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
            Vector3 centre =
                Owner != null
                    ? Owner.transform.position
                    : transform.position;

            Gizmos.DrawWireSphere(
                centre,
                activationRadius
            );
        }
    }
}