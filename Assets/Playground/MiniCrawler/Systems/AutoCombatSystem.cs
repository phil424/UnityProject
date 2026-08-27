using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(0)]
    public class AutoCombatSystem : MonoBehaviour
    {
        [Header("2.5B Prototype Knockback Validation")]
        [SerializeField]
        private bool prototypeKnockbackOnBasicAttack;

        [SerializeField]
        [Min(0f)]
        private float prototypeKnockbackDistance = 1.5f;

        [SerializeField]
        [Min(0f)]
        private float prototypeKnockbackSpeed = 6f;

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            AutoTargetMover[] movers =
                FindObjectsByType<AutoTargetMover>(
                    FindObjectsSortMode.None
                );

            foreach (AutoTargetMover mover in movers)
            {
                if (mover.isActiveAndEnabled)
                    TryAttackTarget(mover);
            }
        }

        private void TryAttackTarget(
            AutoTargetMover mover
        )
        {
            Health selfHealth =
                mover.GetComponent<Health>();

            if (
                selfHealth != null &&
                selfHealth.IsDead
            )
            {
                return;
            }

            CombatStats attackerStats =
                mover.GetComponent<CombatStats>();

            if (attackerStats == null)
                return;

            attackerStats.AttackTimer =
                Mathf.Max(
                    0f,
                    attackerStats.AttackTimer -
                    Time.deltaTime
                );

            if (
                mover.CurrentIntent !=
                TargetIntent.Combat
            )
            {
                return;
            }

            Health target =
                mover.CurrentTarget;

            if (
                target == null ||
                target.IsDead ||
                attackerStats.AttackTimer > 0f
            )
            {
                return;
            }

            float distance =
                Vector3.Distance(
                    mover.transform.position,
                    target.transform.position
                );

            if (
                distance >
                attackerStats.AttackRange
            )
            {
                return;
            }

            DamageResolver.ApplyDamage(
                mover.gameObject,
                target,
                attackerStats.Damage,
                attackerStats.AttackName
            );

            ApplyPrototypeKnockback(
                mover.gameObject,
                target
            );

            attackerStats.AttackTimer =
                attackerStats.AttackCooldown;
        }

        private void ApplyPrototypeKnockback(
            GameObject source,
            Health target
        )
        {
            // Temporary 2.5B validation hook.
            //
            // Basic attacks do not conceptually own
            // knockback in the final architecture.
            // This exists only so generic knockback can
            // be visually validated before abilities exist.
            if (!prototypeKnockbackOnBasicAttack)
                return;

            KnockbackResolver.TryApply(
                source,
                target,
                prototypeKnockbackDistance,
                prototypeKnockbackSpeed
            );
        }
    }
}