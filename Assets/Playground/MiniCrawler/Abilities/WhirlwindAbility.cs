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

        public float ActivationRadius =>
            activationRadius;

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
            // 2.6A establishes activation only.
            //
            // Damage and knockback are deliberately
            // added in later 2.6 steps through the
            // existing generic combat seams.
            Debug.Log(
                $"[Ability] {name} activated " +
                $"{AbilityName}."
            );

            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            activationRadius =
                Mathf.Max(
                    0.1f,
                    activationRadius
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