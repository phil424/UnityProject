using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct ProjectileLaunchRequest
    {
        public ProjectileLaunchRequest(ProjectileDefinition definition, GameObject source, ProjectileDelivery delivery, ProjectileImpactPayload impact)
        {
            Definition = definition;
            Source = source;
            Delivery = delivery;
            Impact = impact;
        }

        public ProjectileDefinition Definition { get; }
        public GameObject Source { get; }
        public ProjectileDelivery Delivery { get; }
        public ProjectileImpactPayload Impact { get; }

        public bool IsConfigured => Definition != null && Source != null && Delivery.IsConfigured && Impact.IsConfigured;
    }
}
