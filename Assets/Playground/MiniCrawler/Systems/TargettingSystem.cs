using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-300)]
    public class TargetingSystem : MonoBehaviour
    {
        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;
                
            AutoTargetMover[] movers =
                FindObjectsByType<AutoTargetMover>(FindObjectsSortMode.None);

            Health[] possibleTargets =
                FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (AutoTargetMover mover in movers)
            {
                if (!CanChooseTarget(mover))
                {
                    mover.ClearTarget();
                    continue;
                }

                SupportStats supportStats = mover.GetComponent<SupportStats>();

                if (supportStats != null)
                {
                    Health supportTarget = FindSupportTarget(mover, possibleTargets);

                    if (supportTarget != null)
                    {
                        mover.SetTarget(supportTarget, TargetIntent.Support);
                        continue;
                    }
                }

                Health combatTarget = FindNearestCombatTarget(mover, possibleTargets);
                mover.SetTarget(combatTarget, TargetIntent.Combat);
            }
        }

        private bool CanChooseTarget(AutoTargetMover mover)
        {
            if (mover == null || !mover.isActiveAndEnabled)
                return false;

            Health selfHealth = mover.GetComponent<Health>();

            return selfHealth == null || !selfHealth.IsDead;
        }

        private Health FindSupportTarget(
            AutoTargetMover mover,
            Health[] possibleTargets
        )
        {
            Health mostInjuredAlly = null;
            float lowestHealthRatio = float.MaxValue;
            float injuredDistanceSquared = float.MaxValue;

            Health nearestHealthyAlly = null;
            float healthyDistanceSquared = float.MaxValue;

            foreach (Health possibleTarget in possibleTargets)
            {
                if (!IsValidSupportTarget(mover, possibleTarget))
                    continue;

                float distanceSquared =
                    (possibleTarget.transform.position - mover.transform.position)
                    .sqrMagnitude;

                bool isInjured = possibleTarget.CurrentHealth < possibleTarget.MaxHealth;

                if (!isInjured)
                {
                    if (distanceSquared < healthyDistanceSquared)
                    {
                        healthyDistanceSquared = distanceSquared;
                        nearestHealthyAlly = possibleTarget;
                    }

                    continue;
                }

                float healthRatio = possibleTarget.CurrentHealth / possibleTarget.MaxHealth;

                bool hasLowerHealth = healthRatio < lowestHealthRatio;
                bool sameHealthButCloser =
                    Mathf.Approximately(healthRatio, lowestHealthRatio) &&
                    distanceSquared < injuredDistanceSquared;

                if (!hasLowerHealth && !sameHealthButCloser)
                    continue;

                lowestHealthRatio = healthRatio;
                injuredDistanceSquared = distanceSquared;
                mostInjuredAlly = possibleTarget;
            }

            // Injured allies take priority. If everyone is healthy, follow the nearest ally.
            return mostInjuredAlly != null
                ? mostInjuredAlly
                : nearestHealthyAlly;
        }

        private Health FindNearestCombatTarget(
            AutoTargetMover mover,
            Health[] possibleTargets
        )
        {
            Health bestTarget = null;
            float bestDistanceSquared = float.MaxValue;
            float maxDistanceSquared = mover.SearchRadius * mover.SearchRadius;

            foreach (Health possibleTarget in possibleTargets)
            {
                if (!IsValidCombatTarget(mover, possibleTarget))
                    continue;

                float distanceSquared =
                    (possibleTarget.transform.position - mover.transform.position)
                    .sqrMagnitude;

                if (distanceSquared > maxDistanceSquared)
                    continue;

                if (distanceSquared >= bestDistanceSquared)
                    continue;

                bestDistanceSquared = distanceSquared;
                bestTarget = possibleTarget;
            }

            return bestTarget;
        }

        private bool IsValidSupportTarget(
            AutoTargetMover mover,
            Health possibleTarget
        )
        {
            if (possibleTarget == null || possibleTarget.IsDead)
                return false;

            if (possibleTarget.gameObject == mover.gameObject)
                return false;

            return possibleTarget.GetComponent<PartyMember>() != null;
        }

        private bool IsValidCombatTarget(
            AutoTargetMover mover,
            Health possibleTarget
        )
        {
            if (possibleTarget == null || possibleTarget.IsDead)
                return false;

            if (possibleTarget.gameObject == mover.gameObject)
                return false;

            FactionMember faction = possibleTarget.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(mover.TargetFactionId);
        }
    }
}