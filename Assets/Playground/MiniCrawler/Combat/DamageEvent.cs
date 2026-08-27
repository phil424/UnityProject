using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct DamageEvent
    {
        public GameObject Source { get; }
        public Health Target { get; }
        public string ActionName { get; }

        public float IncomingDamage { get; }
        public float Armour { get; }
        public float FinalDamage { get; }

        public float HealthBefore { get; }
        public float HealthAfter { get; }

        public DamageEvent( 
            GameObject source, 
            Health target,
            string actionName,
            float incomingDamage,
            float armour,
            float finalDamage,
            float healthBefore,
            float healthAfter
        )
        {
            Source = source;
            Target = target;
            ActionName = actionName;

            IncomingDamage = incomingDamage;
            Armour = armour;
            FinalDamage = finalDamage;

            HealthBefore = healthBefore;
            HealthAfter = healthAfter;
        }
    }
}
