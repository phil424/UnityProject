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
        [SerializeField] private AbilityLevelValue activationRadius;

        [SerializeField] private AbilityLevelValue damage;

        [SerializeField] private AbilityLevelValue knockbackDistance;

        [SerializeField] private AbilityLevelValue knockbackSpeed;

        public float ActivationRadius =>
            Mathf.Max(0.1f, activationRadius != null ? activationRadius.Evaluate(Level) : 0.1f);

        public float Damage =>
            Mathf.Max(0f, damage != null ? damage.Evaluate(Level) : 0f);

        public float KnockbackDistance =>
            Mathf.Max(0f, knockbackDistance != null ? knockbackDistance.Evaluate(Level) : 0f);

        public float KnockbackSpeed =>
            Mathf.Max(0f, knockbackSpeed != null ? knockbackSpeed.Evaluate(Level) : 0f);

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
                    Damage,
                    AbilityName
                );

                hitCount++;

                if (
                    !target.IsDead &&
                    KnockbackResolver.TryApply(
                        Owner,
                        target,
                        KnockbackDistance,
                        KnockbackSpeed
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
                ActivationRadius *
                ActivationRadius;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        private void OnDrawGizmosSelected()
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