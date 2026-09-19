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

            AutoTargetMover[] movers = FindObjectsByType<AutoTargetMover>(FindObjectsSortMode.None);

            foreach (AutoTargetMover mover in movers)
            {
                if (mover.isActiveAndEnabled)
                    TryAttackTarget(mover);
            }
        }

        private void TryAttackTarget(AutoTargetMover mover)
        {
            Health selfHealth = mover.GetComponent<Health>();

            if (selfHealth != null && selfHealth.IsDead)
                return;

            CombatActionState actionState = mover.GetComponent<CombatActionState>();

            if (!CombatEngagementState.AllowsCombat(mover.gameObject))
            {
                ResetPendingCombo(actionState);
                return;
            }

            CombatStats attackerStats = mover.GetComponent<CombatStats>();

            if (attackerStats == null)
            {
                ResetPendingCombo(actionState);
                return;
            }

            bool usesActionTimeline = actionState != null && actionState.GetNextBasicAttack() != null;

            if (!usesActionTimeline)
                attackerStats.AttackTimer = Mathf.Max(0f, attackerStats.AttackTimer - Time.deltaTime);

            AbilityExecutionState abilityExecutionState = mover.GetComponent<AbilityExecutionState>();

            if (abilityExecutionState != null && abilityExecutionState.BlocksAutonomousActions)
            {
                ResetPendingCombo(actionState);
                return;
            }

            if (actionState != null && actionState.IsExecuting)
                return;

            if (mover.CurrentIntent != TargetIntent.Combat)
            {
                ResetPendingCombo(actionState);
                return;
            }

            Health target = mover.CurrentTarget;

            if (target == null ||
                target.IsDead ||
                !CombatEngagementState.AllowsCombat(target.gameObject) ||
                (!usesActionTimeline && attackerStats.AttackTimer > 0f))
            {
                ResetPendingCombo(actionState);
                return;
            }

            if (actionState != null &&
                actionState.HasPendingBasicAttackContinuation &&
                target != actionState.BasicAttackChainTarget)
            {
                actionState.ResetBasicAttackSequence();
            }

            float distance = Vector3.Distance(mover.transform.position, target.transform.position);

            if (distance > attackerStats.AttackRange)
            {
                ResetPendingCombo(actionState);
                return;
            }

            if (usesActionTimeline)
            {
                bool started = actionState.TryBeginBasicAttack(target, attackerStats.AttackCooldown);

                if (!started)
                    ResetPendingCombo(actionState);

                return;
            }

            DamageResolver.ApplyDamage(mover.gameObject, target, attackerStats.Damage, attackerStats.AttackName);
            attackerStats.AttackTimer = attackerStats.AttackCooldown;
        }

        private static void ResetPendingCombo(CombatActionState actionState)
        {
            if (actionState != null && actionState.HasPendingBasicAttackContinuation)
                actionState.ResetBasicAttackSequence();
        }
    }
}