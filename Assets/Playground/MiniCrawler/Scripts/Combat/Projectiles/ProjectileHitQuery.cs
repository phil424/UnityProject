using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Combat
{
    internal readonly struct ProjectileHitCandidate
    {
        public ProjectileHitCandidate(
            Health target,
            float alongDistance,
            float hitDistance,
            Vector3 closestPoint)
        {
            Target = target;
            AlongDistance = alongDistance;
            HitDistance = hitDistance;
            ClosestPoint = closestPoint;
        }

        public Health Target { get; }
        public float AlongDistance { get; }
        public float HitDistance { get; }
        public Vector3 ClosestPoint { get; }
    }

    internal static class ProjectileHitQuery
    {
        private const float MinimumDirectionSquared = 0.0001f;

        public static void CollectSegmentTargets(GameObject source, string targetFactionId, HashSet<Health> alreadyHit, Vector3 travelDirection, Vector3 segmentStart, Vector3 segmentEnd, float projectileHitRadius, List<ProjectileHitCandidate> output)
        {
            if (output == null)
                return;

            output.Clear();

            if (source == null || string.IsNullOrWhiteSpace(targetFactionId) || travelDirection.sqrMagnitude <= MinimumDirectionSquared)
                return;

            Health[] possibleTargets = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health possibleTarget in possibleTargets)
            {
                if (!IsValidTarget(possibleTarget, source, targetFactionId, alreadyHit))
                    continue;

                float targetRadius = 0f;

                EntityAvoidance avoidance = possibleTarget.GetComponent<EntityAvoidance>();

                if (avoidance != null)
                    targetRadius = avoidance.WorldRadius;

                float hitDistance = Mathf.Max(0f, projectileHitRadius) + targetRadius;

                Vector3 closestPoint = ClosestPointOnSegment(possibleTarget.transform.position, segmentStart, segmentEnd);

                Vector3 offset = possibleTarget.transform.position - closestPoint;

                offset.y = 0f;

                if (offset.sqrMagnitude > hitDistance * hitDistance)
                    continue;

                float alongDistance = Vector3.Dot(closestPoint - segmentStart, travelDirection);

                output.Add(new ProjectileHitCandidate(possibleTarget, alongDistance, hitDistance, closestPoint));
            }

            output.Sort((a, b) => a.AlongDistance.CompareTo(b.AlongDistance));
        }

        private static bool IsValidTarget(Health possibleTarget, GameObject source, string targetFactionId, HashSet<Health> alreadyHit)
        {
            if (possibleTarget == null || possibleTarget.IsDead || possibleTarget.gameObject == source || (alreadyHit != null && alreadyHit.Contains(possibleTarget)))
                return false;

            FactionMember faction = possibleTarget.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(targetFactionId);
        }

        private static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 start, Vector3 end)
        {
            point.y = 0f;
            start.y = 0f;
            end.y = 0f;

            Vector3 segment = end - start;
            float lengthSquared = segment.sqrMagnitude;

            if (lengthSquared <= MinimumDirectionSquared)
                return start;

            float t = Vector3.Dot(point - start, segment) / lengthSquared;

            return start + segment * Mathf.Clamp01(t);
        }
    }
}