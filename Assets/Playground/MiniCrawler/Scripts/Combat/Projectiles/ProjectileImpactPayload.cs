using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct ProjectileKnockbackPayload
    {
        public ProjectileKnockbackPayload(float distance, float speed, float fanAngle = 0f)
        {
            Distance = Mathf.Max(0f, distance);
            Speed = Mathf.Max(0f, speed);
            FanAngle = Mathf.Clamp(fanAngle, 0f, 89f);
        }

        public float Distance { get; }
        public float Speed { get; }
        public float FanAngle { get; }

        public bool IsConfigured => Distance > 0f && Speed > 0f;
    }

    public readonly struct ProjectileBurnPayload
    {
        public ProjectileBurnPayload(float damagePerTick, float tickInterval, float duration, string actionName)
        {
            DamagePerTick = Mathf.Max(0f, damagePerTick);
            TickInterval = Mathf.Max(0f, tickInterval);
            Duration = Mathf.Max(0f, duration);
            ActionName = string.IsNullOrWhiteSpace(actionName) ? "Burn" : actionName;
        }

        public float DamagePerTick { get; }
        public float TickInterval { get; }
        public float Duration { get; }
        public string ActionName { get; }

        public bool IsConfigured => DamagePerTick > 0f && TickInterval > 0f && Duration >= TickInterval;
    }

    public readonly struct ProjectileImpactPayload
    {
        private ProjectileImpactPayload(float damage, string actionName, ProjectileKnockbackPayload knockback, ProjectileBurnPayload burn)
        {
            Damage = Mathf.Max(0f, damage);
            ActionName = string.IsNullOrWhiteSpace(actionName) ? "Projectile" : actionName;
            Knockback = knockback;
            Burn = burn;
        }

        public ProjectileImpactPayload(float damage, string actionName)
            : this(damage, actionName, default, default) { }

        public float Damage { get; }
        public string ActionName { get; }

        public ProjectileKnockbackPayload Knockback { get; }
        public ProjectileBurnPayload Burn { get; }

        public bool IsConfigured => Damage > 0f || Knockback.IsConfigured || Burn.IsConfigured;

        public ProjectileImpactPayload WithKnockback(float distance, float speed, float fanAngle = 0f)
        {
            return new ProjectileImpactPayload(Damage, ActionName, new ProjectileKnockbackPayload(distance, speed, fanAngle), Burn);
        }

        public ProjectileImpactPayload WithBurn(float damagePerTick, float tickInterval, float duration, string actionName)
        {
            return new ProjectileImpactPayload(Damage, ActionName, Knockback, new ProjectileBurnPayload(damagePerTick, tickInterval, duration, actionName));
        }
    }
}
