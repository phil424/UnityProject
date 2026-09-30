using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class ProjectileShotAbility : ActorAbility
    {
        [Header("Projectile Shot")]
        [SerializeField] private ProjectileDefinition projectile;
        [SerializeField] private AbilityLevelValue range;
        [SerializeField] private AbilityLevelValue windupDuration;
        [SerializeField] private AbilityLevelValue recoveryDuration;
        [SerializeField] private AbilityLevelValue damage;

        private Health target;
        private float elapsed;
        private bool launched;

        public float Range => Mathf.Max(0.1f, Evaluate(range));
        public float WindupDuration => Mathf.Max(0f, Evaluate(windupDuration));
        public float RecoveryDuration => Mathf.Max(0f, Evaluate(recoveryDuration));
        public float Damage => Mathf.Max(0f, Evaluate(damage));

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

                if (target == null || target.IsDead)
                {
                    EndShot();
                    return;
                }

                ProjectileRuntime runtime = ProjectileRuntime.Spawn(
                    projectile,
                    Owner,
                    target,
                    Damage,
                    AbilityName
                );

                if (runtime == null)
                {
                    Debug.LogWarning($"[Ability] {AbilityName} failed to launch its projectile.", this);
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