using MiniCrawler.Abilities;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(0)]
    public class AutoCombatSystem : MonoBehaviour
    {
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

            AbilityExecutionState
                abilityExecutionState =
                    mover.GetComponent<
                        AbilityExecutionState
                    >();

            if (
                abilityExecutionState != null &&
                abilityExecutionState
                    .BlocksAutonomousActions
            )
            {
                return;
            }

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

            attackerStats.AttackTimer =
                attackerStats.AttackCooldown;
        }
    }
}