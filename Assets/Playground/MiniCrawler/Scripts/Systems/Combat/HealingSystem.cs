using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(0)]
    public class HealingSystem : MonoBehaviour
    {
        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;
                
            SupportStats[] supporters =
                FindObjectsByType<SupportStats>(FindObjectsSortMode.None);

            foreach (SupportStats supporter in supporters)
            {
                if (!supporter.isActiveAndEnabled)
                    continue;

                TryHealTarget(supporter);
            }
        }

        private void TryHealTarget(SupportStats supporter)
        {
            Health selfHealth = supporter.GetComponent<Health>();

            if (selfHealth != null && selfHealth.IsDead)
                return;

            supporter.HealTimer =
                Mathf.Max(
                    0f,
                    supporter.HealTimer -
                    Time.deltaTime
                );

            AbilityExecutionState
                abilityExecutionState =
                    supporter.GetComponent<AbilityExecutionState>();

            if (
                abilityExecutionState != null &&
                abilityExecutionState.BlocksAutonomousActions
            )
            {
                return;
            }

            AutoTargetMover mover = supporter.GetComponent<AutoTargetMover>();

            if (mover.CurrentIntent != TargetIntent.Support)
                return;

            Health target = mover.CurrentTarget;

            if (target == null || target.IsDead)
                return;

            if (target.CurrentHealth >= target.MaxHealth)
                return;

            if (supporter.HealTimer > 0f)
                return;

            float distance = Vector3.Distance(
                supporter.transform.position,
                target.transform.position
            );

            if (distance > supporter.HealRange)
                return;

            float healthBefore = target.CurrentHealth;
            target.Heal(supporter.HealAmount);
            float amountHealed = target.CurrentHealth - healthBefore;

            if (amountHealed <= 0f)
                return;

            Debug.Log(
                $"{supporter.name} used {supporter.HealName} on " +
                $"{target.name} for {amountHealed:0.##} health."
            );

            supporter.HealTimer = supporter.HealCooldown;
        }
    }
}