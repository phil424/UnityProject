using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct ProjectileLaunchRequest
    {
        public ProjectileLaunchRequest(ProjectileDefinition definition, GameObject source, ProjectileDelivery delivery, ProjectileImpactPayload impact, bool isBasicAttack = false)
        {
            Definition = definition;
            Source = source;
            Delivery = delivery;
            Impact = impact;
            IsBasicAttack = isBasicAttack;
        }

        public ProjectileDefinition Definition { get; }
        public GameObject Source { get; }
        public ProjectileDelivery Delivery { get; }
        public ProjectileImpactPayload Impact { get; }
        public bool IsBasicAttack { get; }

        public bool IsConfigured => Definition != null && Source != null && Delivery.IsConfigured && Impact.IsConfigured;

        public ProjectileLaunchRequest WithDelivery(ProjectileDelivery delivery)
        {
            return new ProjectileLaunchRequest(Definition, Source, delivery, Impact, IsBasicAttack);
        }
    }
}
