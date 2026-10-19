using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class GlacialAegisAbility : ActorAbility
    {
        [Header("Glacial Shields")]
        [SerializeField]
        private AbilityLevelValue shieldsPerActivation;

        [SerializeField]
        private AbilityLevelValue hitsPerShield;

        [Header("Frost Nova")]
        [SerializeField]
        private AbilityLevelValue frostRadius;

        [SerializeField]
        private AbilityLevelValue frostDamage;

        [SerializeField]
        private AbilityLevelValue slowPercent;

        [SerializeField]
        private AbilityLevelValue slowDuration;

        [Header("Activation")]
        [SerializeField]
        private AbilityLevelValue activationLockDuration;

        private HitShieldRuntime boundShields;
        private float activationLockRemaining;

        public int ShieldsPerActivation => Mathf.Max(1, Mathf.RoundToInt(Evaluate(shieldsPerActivation)));

        public int HitsPerShield => Mathf.Max(1, Mathf.RoundToInt(Evaluate(hitsPerShield)));

        public float FrostRadius => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(frostRadius)));

        public float FrostDamage => ResolveAbilityDamage(Evaluate(frostDamage));

        public float SlowPercent => Mathf.Clamp(Evaluate(slowPercent), 0f, 95f);

        public float SlowDuration => Mathf.Max(0.1f, Evaluate(slowDuration));

        public float ActivationLockDuration => Mathf.Max(0f, Evaluate(activationLockDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null)
                return false;

            Health health = Owner.GetComponent<Health>();

            if (health == null || health.IsDead || !CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            return mover == null || (mover.CurrentIntent == TargetIntent.Combat && mover.CurrentTarget != null);
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            float lockDuration = ActivationLockDuration;

            if (lockDuration > 0f && !BeginExecution())
            {
                return false;
            }

            ShieldCapacityStats capacity = Owner.GetComponent<ShieldCapacityStats>();

            if (capacity == null)
            {
                capacity = Owner.AddComponent<ShieldCapacityStats>();
            }

            HitShieldRuntime shields = Owner.GetComponent<HitShieldRuntime>();

            if (shields == null)
            {
                shields = Owner.AddComponent<HitShieldRuntime>();
            }

            BindShields(shields);

            bool applied = shields.AddShields(Owner, ShieldsPerActivation, HitsPerShield, AbilityName, ShieldSourceId);

            if (!applied)
            {
                EndExecution();
                return false;
            }

            activationLockRemaining = lockDuration;

            Debug.Log($"[Ability] {Owner.name} activated " + $"{AbilityName} | " + $"+{ShieldsPerActivation} " + $"Glacial Shield(s), " + $"{HitsPerShield} hit(s) each.");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (!IsExecuting)
                return;

            activationLockRemaining -= deltaTime;

            if (activationLockRemaining > 0f)
                return;

            activationLockRemaining = 0f;
            EndExecution();
        }

        protected override void OnDisable()
        {
            if (boundShields != null)
            {
                boundShields.ShieldBroken -= HandleShieldBroken;

                boundShields.RemoveShieldsBySource(ShieldSourceId);
            }

            boundShields = null;
            activationLockRemaining = 0f;

            base.OnDisable();
        }

        private void BindShields(HitShieldRuntime shields)
        {
            if (boundShields == shields)
                return;

            if (boundShields != null)
            {
                boundShields.ShieldBroken -= HandleShieldBroken;
            }

            boundShields = shields;

            if (boundShields != null)
            {
                boundShields.ShieldBroken += HandleShieldBroken;
            }
        }

        private void HandleShieldBroken(ShieldBreakEvent shieldBreak)
        {
            if (shieldBreak.ShieldSourceId != ShieldSourceId)
            {
                return;
            }

            ApplyFrostNova();
        }

        private void ApplyFrostNova()
        {
            if (Owner == null)
                return;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return;

            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            int hitCount = 0;
            int slowCount = 0;

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target))
                {
                    continue;
                }

                DamageResolver.ApplyDamage(Owner, target, FrostDamage, $"{AbilityName} Frost Nova");

                hitCount++;

                if (!target.IsDead && SlowRuntime.Apply(Owner, target, SlowPercent, SlowDuration, $"{AbilityName} Frost Nova"))
                {
                    slowCount++;
                }
            }

            Debug.Log($"[Ability] {Owner.name} triggered " + $"{AbilityName} Frost Nova | " + $"Hit {hitCount} | " + $"Slowed {slowCount}.");
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target)
        {
            if (target == null || target.IsDead || target.gameObject == Owner || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            if (faction == null || !faction.IsFaction(mover.TargetFactionId))
            {
                return false;
            }

            Vector3 offset = target.transform.position - Owner.transform.position;

            offset.y = 0f;

            return offset.sqrMagnitude <= FrostRadius * FrostRadius;
        }

        private string ShieldSourceId => Definition != null ? $"Ability:{Definition.Id}:GlacialShield" : $"Ability:{GetType().Name}:GlacialShield";

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre = Owner != null ? Owner.transform.position : transform.position;

            Gizmos.DrawWireSphere(centre, FrostRadius);
        }
    }
}
