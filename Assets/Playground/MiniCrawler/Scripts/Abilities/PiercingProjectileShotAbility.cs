using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class PiercingProjectileShotAbility : ActorAbility
    {
        [Header("Piercing Projectile Burst")]
        [SerializeField]
        private ProjectileDefinition projectile;

        [SerializeField]
        private AbilityLevelValue range;

        [SerializeField]
        private AbilityLevelValue windupDuration;

        [SerializeField, Min(0.01f)]
        private float burstShotInterval = 0.15f;

        [SerializeField, Min(1)]
        private int burstShotCount = 3;

        [SerializeField]
        private AbilityLevelValue recoveryDuration;

        [SerializeField]
        private AbilityLevelValue damage;

        [SerializeField]
        private AbilityLevelValue hitRadius;

        [SerializeField]
        private AbilityLevelValue maximumTargetsHit;

        [SerializeField]
        private AbilityLevelValue knockbackDistance;

        [SerializeField]
        private AbilityLevelValue knockbackSpeed;

        [SerializeField]
        private AbilityLevelValue stunDuration = new();

        [SerializeField, Range(0f, 89f)]
        private float knockbackFanAngle = 35f;

        [Header("Burst Spread")]
        [FormerlySerializedAs("fanVolleySideAngle")]
        [SerializeField, Range(0f, 45f)]
        private float burstSpreadAngle = 15f;

        [FormerlySerializedAs("fanVolleySideDamageMultiplier")]
        [SerializeField, Range(0.1f, 1f)]
        private float sideShotDamageMultiplier = 0.65f;

        private Vector3 burstDirection;
        private string burstTargetFactionId;
        private float elapsed;
        private int shotsLaunched;

        public float Range => Mathf.Max(0.1f, Evaluate(range));
        public float WindupDuration => Mathf.Max(0f, Evaluate(windupDuration));
        public float BurstShotInterval => Mathf.Max(0.01f, burstShotInterval);
        public int BurstShotCount => Mathf.Max(1, burstShotCount);
        public float RecoveryDuration => Mathf.Max(0f, Evaluate(recoveryDuration));
        public float Damage => ResolveAbilityDamage(Evaluate(damage));
        public float HitRadius => Mathf.Max(0.05f, Evaluate(hitRadius));
        public int MaximumTargetsHit => Mathf.Max(1, Mathf.RoundToInt(Evaluate(maximumTargetsHit)));
        public float KnockbackDistance => Mathf.Max(0f, Evaluate(knockbackDistance));
        public float KnockbackSpeed => Mathf.Max(0f, Evaluate(knockbackSpeed));
        public float StunDuration => Mathf.Max(0f, Evaluate(stunDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || projectile == null || Damage <= 0f)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null || mover.CurrentIntent != TargetIntent.Combat)
            {
                return false;
            }

            Health currentTarget = mover.CurrentTarget;

            if (currentTarget == null || currentTarget.IsDead)
            {
                return false;
            }

            Vector3 offset = currentTarget.transform.position - Owner.transform.position;

            offset.y = 0f;

            return offset.sqrMagnitude <= Range * Range;
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover = Owner != null ? Owner.GetComponent<AutoTargetMover>() : null;

            Health currentTarget = mover != null ? mover.CurrentTarget : null;

            if (mover == null || currentTarget == null || currentTarget.IsDead)
            {
                return false;
            }

            Vector3 direction = currentTarget.transform.position - Owner.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                return false;
            }

            if (!BeginExecution())
                return false;

            burstDirection = direction.normalized;

            burstTargetFactionId = mover.TargetFactionId;

            elapsed = 0f;
            shotsLaunched = 0;

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            elapsed += deltaTime;

            while (shotsLaunched < BurstShotCount && elapsed >= GetShotTime(shotsLaunched))
            {
                LaunchBurstShot(shotsLaunched);
                shotsLaunched++;
            }

            if (shotsLaunched < BurstShotCount)
                return;

            float finalShotTime = GetShotTime(BurstShotCount - 1);

            if (elapsed >= finalShotTime + RecoveryDuration)
            {
                EndShot();
            }
        }

        protected override void OnDisable()
        {
            burstDirection = Vector3.zero;
            burstTargetFactionId = null;
            elapsed = 0f;
            shotsLaunched = 0;

            base.OnDisable();
        }

        private float GetShotTime(int shotIndex)
        {
            return WindupDuration + BurstShotInterval * Mathf.Max(0, shotIndex);
        }

        private void LaunchBurstShot(int shotIndex)
        {
            float angle = ResolveBurstAngle(shotIndex);

            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * burstDirection;

            float damageMultiplier = ResolveShotDamageMultiplier(angle);

            ProjectileDelivery delivery = ProjectileDelivery.Directional(direction, Range, burstTargetFactionId, HitRadius, MaximumTargetsHit);

            ProjectileImpactPayload impact = new ProjectileImpactPayload(Damage * damageMultiplier, AbilityName)
                .WithKnockback(KnockbackDistance, KnockbackSpeed, knockbackFanAngle)
                .WithStun(StunDuration, AbilityName);

            ProjectileLaunchRequest request = new(projectile, Owner, delivery, impact);

            ProjectileLauncher.Launch(request);
        }

        private float ResolveBurstAngle(int shotIndex)
        {
            if (BurstShotCount <= 1)
                return 0f;

            float normalizedIndex = shotIndex / (float)(BurstShotCount - 1);

            return Mathf.Lerp(-burstSpreadAngle, burstSpreadAngle, normalizedIndex);
        }

        private float ResolveShotDamageMultiplier(float angle)
        {
            if (Mathf.Abs(angle) <= 0.01f)
                return 1f;

            return Mathf.Clamp(sideShotDamageMultiplier, 0.1f, 1f);
        }

        private void EndShot()
        {
            burstDirection = Vector3.zero;
            burstTargetFactionId = null;
            elapsed = 0f;
            shotsLaunched = 0;

            EndExecution();
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}
