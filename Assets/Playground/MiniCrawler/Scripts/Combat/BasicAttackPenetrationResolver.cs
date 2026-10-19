using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public static class BasicAttackPenetrationResolver
    {
        private const float MinimumDirectionSquared = 0.0001f;

        public static int ResolveAdditionalMeleeHits(GameObject source, Health primaryTarget, float damage, string actionName, float attackRange, int additionalTargets)
        {
            if (source == null || primaryTarget == null || damage <= 0f || attackRange <= 0f || additionalTargets <= 0)
            {
                return 0;
            }

            AutoTargetMover mover = source.GetComponent<AutoTargetMover>();

            if (mover == null || string.IsNullOrWhiteSpace(mover.TargetFactionId))
            {
                return 0;
            }

            Vector3 segmentStart = source.transform.position;

            Vector3 direction = primaryTarget.transform.position - segmentStart;

            direction.y = 0f;

            if (direction.sqrMagnitude <= MinimumDirectionSquared)
            {
                direction = source.transform.forward;
                direction.y = 0f;
            }

            if (direction.sqrMagnitude <= MinimumDirectionSquared)
            {
                return 0;
            }

            direction.Normalize();

            Vector3 segmentEnd = segmentStart + direction * attackRange;

            float primaryAlongDistance = Vector3.Dot(primaryTarget.transform.position - segmentStart, direction);

            HashSet<Health> alreadyHit = new() { primaryTarget };

            List<ProjectileHitCandidate> candidates = new();

            ProjectileHitQuery.CollectSegmentTargets(source, mover.TargetFactionId, alreadyHit, direction, segmentStart, segmentEnd, 0f, candidates);

            int hits = 0;

            foreach (ProjectileHitCandidate candidate in candidates)
            {
                if (candidate.Target == null || candidate.Target.IsDead || candidate.AlongDistance <= primaryAlongDistance + 0.01f || !CombatEngagementState.AllowsCombat(candidate.Target.gameObject))
                {
                    continue;
                }

                DamageResolver.ApplyDamage(source, candidate.Target, damage, actionName);

                hits++;

                if (hits >= additionalTargets)
                    break;
            }

            return hits;
        }
    }
}
