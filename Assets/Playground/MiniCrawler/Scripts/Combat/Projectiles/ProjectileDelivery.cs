using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public enum ProjectileDeliveryMode
    {
        Targeted,
        TargetedPiercing,
        Directional,
    }

    public readonly struct ProjectileDelivery
    {
        private const float MinimumDirectionSquared = 0.0001f;

        private ProjectileDelivery(ProjectileDeliveryMode mode, Health target, Vector3 direction, float travelDistance, string targetFactionId, float hitRadius, int maximumHits)
        {
            Mode = mode;
            Target = target;
            Direction = direction;
            TravelDistance = travelDistance;
            TargetFactionId = targetFactionId;
            HitRadius = hitRadius;
            MaximumHits = maximumHits;
        }

        public ProjectileDeliveryMode Mode { get; }
        public Health Target { get; }
        public Vector3 Direction { get; }
        public float TravelDistance { get; }
        public string TargetFactionId { get; }
        public float HitRadius { get; }
        public int MaximumHits { get; }

        public bool IsConfigured
        {
            get
            {
                switch (Mode)
                {
                    case ProjectileDeliveryMode.Targeted:
                        return Target != null && !Target.IsDead;

                    case ProjectileDeliveryMode.TargetedPiercing:
                        return Target != null && !Target.IsDead && TravelDistance > 0f && !string.IsNullOrWhiteSpace(TargetFactionId) && HitRadius > 0f && MaximumHits > 0;

                    case ProjectileDeliveryMode.Directional:
                        return Direction.sqrMagnitude > MinimumDirectionSquared && TravelDistance > 0f && !string.IsNullOrWhiteSpace(TargetFactionId) && HitRadius > 0f && MaximumHits > 0;

                    default:
                        return false;
                }
            }
        }

        public static ProjectileDelivery Targeted(Health target)
        {
            return new ProjectileDelivery(ProjectileDeliveryMode.Targeted, target, Vector3.zero, 0f, null, 0f, 1);
        }

        public static ProjectileDelivery TargetedPiercing(Health target, float travelDistance, string targetFactionId, float hitRadius, int maximumHits)
        {
            return new ProjectileDelivery(ProjectileDeliveryMode.TargetedPiercing, target, Vector3.zero, travelDistance, targetFactionId, hitRadius, maximumHits);
        }

        public static ProjectileDelivery Directional(Vector3 direction, float travelDistance, string targetFactionId, float hitRadius, int maximumHits)
        {
            return new ProjectileDelivery(ProjectileDeliveryMode.Directional, null, direction, travelDistance, targetFactionId, hitRadius, maximumHits);
        }
    }
}
