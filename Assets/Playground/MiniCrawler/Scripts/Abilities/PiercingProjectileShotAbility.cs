using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class PiercingProjectileShotAbility : ActorAbility
    {
        [Header("Piercing Projectile Shot")]
        [SerializeField]
        private ProjectileDefinition projectile;

        [SerializeField]
        private AbilityLevelValue range;

        [SerializeField]
        private AbilityLevelValue windupDuration;

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

        [SerializeField, Range(0f, 89f)]
        private float knockbackFanAngle = 35f;

        private Health target;
        private float elapsed;
        private bool launched;

        public float Range => Mathf.Max(0.1f, Evaluate(range));
        public float WindupDuration => Mathf.Max(0f, Evaluate(windupDuration));
        public float RecoveryDuration => Mathf.Max(0f, Evaluate(recoveryDuration));
        public float Damage => Mathf.Max(0f, Evaluate(damage));
        public float HitRadius => Mathf.Max(0.05f, Evaluate(hitRadius));
        public int MaximumTargetsHit => Mathf.Max(1, Mathf.RoundToInt(Evaluate(maximumTargetsHit)));
        public float KnockbackDistance => Mathf.Max(0f, Evaluate(knockbackDistance));
        public float KnockbackSpeed => Mathf.Max(0f, Evaluate(knockbackSpeed));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || projectile == null || Damage <= 0f)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null || mover.CurrentIntent != TargetIntent.Combat)
                return false;

            Health currentTarget = mover.CurrentTarget;

            if (currentTarget == null || currentTarget.IsDead)
                return false;

            Vector3 offset = currentTarget.transform.position - Owner.transform.position;
            offset.y = 0f;

            return offset.sqrMagnitude <= Range * Range;
        }

        protected override bool ExecuteAbility()
        {
            AutoTargetMover mover = Owner != null ? Owner.GetComponent<AutoTargetMover>() : null;
            Health currentTarget = mover != null ? mover.CurrentTarget : null;

            if (currentTarget == null || currentTarget.IsDead || !BeginExecution())
                return false;

            target = currentTarget;
            elapsed = 0f;
            launched = false;

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            elapsed += deltaTime;

            if (!launched && elapsed >= WindupDuration)
            {
                launched = true;

                if (!LaunchProjectile())
                {
                    EndShot();
                    return;
                }
            }

            if (elapsed >= WindupDuration + RecoveryDuration)
                EndShot();
        }

        protected override void OnDisable()
        {
            target = null;
            elapsed = 0f;
            launched = false;

            base.OnDisable();
        }

        private bool LaunchProjectile()
        {
            if (Owner == null || target == null || target.IsDead)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return false;

            Vector3 direction = target.transform.position - Owner.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return false;

            ProjectileDelivery delivery = ProjectileDelivery.Directional(direction, Range, mover.TargetFactionId, HitRadius, MaximumTargetsHit);

            ProjectileImpactPayload impact = new ProjectileImpactPayload(Damage, AbilityName).WithKnockback(KnockbackDistance, KnockbackSpeed, knockbackFanAngle);

            ProjectileLaunchRequest request = new(projectile, Owner, delivery, impact);

            return ProjectileLauncher.Launch(request) != null;
        }

        private void EndShot()
        {
            target = null;
            EndExecution();
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}
