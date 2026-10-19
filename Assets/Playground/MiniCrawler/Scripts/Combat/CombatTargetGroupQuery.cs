using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct CombatTargetGroupResult
    {
        public CombatTargetGroupResult(Vector3 center, int count, Health anchor)
        {
            Center = center;
            Count = count;
            Anchor = anchor;
        }

        public Vector3 Center { get; }
        public int Count { get; }
        public Health Anchor { get; }
    }

    public static class CombatTargetGroupQuery
    {
        public static bool TryFindBestGroup(GameObject source, string targetFactionId, float searchRadius, float groupRadius, out CombatTargetGroupResult result)
        {
            result = default;

            if (source == null || string.IsNullOrWhiteSpace(targetFactionId) || searchRadius <= 0f || groupRadius <= 0f || !CombatEngagementState.AllowsCombat(source))
            {
                return false;
            }

            Health[] possibleTargets = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);

            List<Health> candidates = new();

            Vector3 sourcePosition = source.transform.position;

            float searchRadiusSquared = searchRadius * searchRadius;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(source, target, targetFactionId))
                {
                    continue;
                }

                Vector3 offset = target.transform.position - sourcePosition;

                offset.y = 0f;

                if (offset.sqrMagnitude > searchRadiusSquared)
                {
                    continue;
                }

                candidates.Add(target);
            }

            if (candidates.Count == 0)
                return false;

            int bestCount = 0;
            float bestDistanceSquared = float.MaxValue;

            Vector3 bestCenter = Vector3.zero;

            Health bestAnchor = null;

            float groupRadiusSquared = groupRadius * groupRadius;

            foreach (Health anchor in candidates)
            {
                Vector3 centerSum = Vector3.zero;

                int memberCount = 0;

                foreach (Health member in candidates)
                {
                    Vector3 offset = member.transform.position - anchor.transform.position;

                    offset.y = 0f;

                    if (offset.sqrMagnitude > groupRadiusSquared)
                    {
                        continue;
                    }

                    centerSum += member.transform.position;

                    memberCount++;
                }

                if (memberCount <= 0)
                    continue;

                Vector3 center = centerSum / memberCount;

                center.y = sourcePosition.y;

                Vector3 toCenter = center - sourcePosition;

                toCenter.y = 0f;

                float distanceSquared = toCenter.sqrMagnitude;

                bool hasMoreMembers = memberCount > bestCount;

                bool sameSizeButCloser = memberCount == bestCount && distanceSquared < bestDistanceSquared;

                if (!hasMoreMembers && !sameSizeButCloser)
                {
                    continue;
                }

                bestCount = memberCount;
                bestDistanceSquared = distanceSquared;
                bestCenter = center;
                bestAnchor = anchor;
            }

            if (bestCount <= 0 || bestAnchor == null)
            {
                return false;
            }

            result = new CombatTargetGroupResult(bestCenter, bestCount, bestAnchor);

            return true;
        }

        private static bool IsValidTarget(GameObject source, Health target, string targetFactionId)
        {
            if (target == null || target.IsDead || target.gameObject == source || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(targetFactionId);
        }
    }
}
