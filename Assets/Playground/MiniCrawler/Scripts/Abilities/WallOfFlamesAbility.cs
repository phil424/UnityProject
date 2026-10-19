using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class WallOfFlamesAbility : ActorAbility
    {
        private const float MinimumDirectionSquared = 0.0001f;

        [Header("Placement")]
        [SerializeField]
        private AbilityLevelValue groupSearchRadius;

        [SerializeField]
        private AbilityLevelValue groupRadius;

        [SerializeField]
        private AbilityLevelValue placementDistance;

        [Header("Wall")]
        [SerializeField]
        private AbilityLevelValue wallDuration;

        [SerializeField]
        private AbilityLevelValue wallLength;

        [SerializeField]
        private AbilityLevelValue wallThickness;

        [SerializeField, Min(0.02f)]
        private float scanInterval = 0.1f;

        [Header("Burn")]
        [SerializeField]
        private AbilityLevelValue burnDamagePerTick;

        [SerializeField]
        private AbilityLevelValue burnTickInterval;

        [SerializeField]
        private AbilityLevelValue burnDuration;

        private Vector3 wallCenter;
        private Vector3 wallNormal;
        private Vector3 wallTangent;

        private float durationRemaining;
        private float scanRemaining;

        public float GroupSearchRadius => Mathf.Max(0.1f, Evaluate(groupSearchRadius));

        public float GroupRadius => Mathf.Max(0.1f, Evaluate(groupRadius));

        public float PlacementDistance => Mathf.Max(0.1f, Evaluate(placementDistance));

        public float WallDuration => Mathf.Max(0.1f, Evaluate(wallDuration));

        public float WallLength => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(wallLength)));

        public float WallThickness => Mathf.Max(0.1f, ResolveAbilityAreaSize(Evaluate(wallThickness)));

        public float BurnDamagePerTick => ResolveAbilityDamage(Evaluate(burnDamagePerTick));

        public float BurnTickInterval => Mathf.Max(0.05f, Evaluate(burnTickInterval));

        public float BurnDuration => Mathf.Max(BurnTickInterval, Evaluate(burnDuration));

        protected override bool CanActivateAbility()
        {
            if (Owner == null || !CombatEngagementState.AllowsCombat(Owner))
            {
                return false;
            }

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            return mover != null && mover.isActiveAndEnabled && mover.CurrentIntent == TargetIntent.Combat && mover.CurrentTarget != null && !mover.CurrentTarget.IsDead;
        }

        protected override bool ExecuteAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            if (mover == null)
                return false;

            Vector3 focusPoint = ResolveFocusPoint(mover);

            Vector3 toFocus = focusPoint - Owner.transform.position;

            toFocus.y = 0f;

            if (toFocus.sqrMagnitude <= MinimumDirectionSquared)
            {
                toFocus = Owner.transform.forward;

                toFocus.y = 0f;
            }

            if (toFocus.sqrMagnitude <= MinimumDirectionSquared)
            {
                return false;
            }

            wallNormal = toFocus.normalized;

            wallTangent = Vector3.Cross(Vector3.up, wallNormal).normalized;

            float focusDistance = toFocus.magnitude;

            float resolvedPlacementDistance = Mathf.Min(PlacementDistance, Mathf.Max(0.5f, focusDistance * 0.5f));

            wallCenter = Owner.transform.position + wallNormal * resolvedPlacementDistance;

            wallCenter.y = Owner.transform.position.y;

            if (!BeginExecution(blockAutonomousActions: false))
            {
                return false;
            }

            durationRemaining = WallDuration;

            scanRemaining = 0f;

            ApplyWallToTargets(mover);

            Debug.Log($"[Ability] {Owner.name} created " + $"{AbilityName} | " + $"Duration {WallDuration:0.##}s | " + $"Length {WallLength:0.##} | " + $"Thickness {WallThickness:0.##}.");

            return true;
        }

        protected override void TickExecution(float deltaTime)
        {
            if (Owner == null)
            {
                EndExecution();
                return;
            }

            durationRemaining = Mathf.Max(0f, durationRemaining - deltaTime);

            scanRemaining -= deltaTime;

            AutoTargetMover mover = Owner.GetComponent<AutoTargetMover>();

            while (mover != null && scanRemaining <= 0f && durationRemaining > 0f)
            {
                ApplyWallToTargets(mover);

                scanRemaining += Mathf.Max(0.02f, scanInterval);
            }

            DrawDebugWall();

            if (durationRemaining > 0f)
                return;

            durationRemaining = 0f;
            scanRemaining = 0f;

            EndExecution();
        }

        protected override void OnDisable()
        {
            durationRemaining = 0f;
            scanRemaining = 0f;

            base.OnDisable();
        }

        private Vector3 ResolveFocusPoint(AutoTargetMover mover)
        {
            if (CombatTargetGroupQuery.TryFindBestGroup(Owner, mover.TargetFactionId, GroupSearchRadius, GroupRadius, out CombatTargetGroupResult group))
            {
                return group.Center;
            }

            return mover.CurrentTarget != null ? mover.CurrentTarget.transform.position : Owner.transform.position + Owner.transform.forward;
        }

        private void ApplyWallToTargets(AutoTargetMover mover)
        {
            Health[] possibleTargets = FindObjectsByType<Health>(FindObjectsSortMode.None);

            foreach (Health target in possibleTargets)
            {
                if (!IsValidTarget(mover, target) || !IsInsideWall(target.transform.position))
                {
                    continue;
                }

                BurningDamageRuntime.Apply(Owner, target, BurnDamagePerTick, BurnTickInterval, BurnDuration, $"{AbilityName} Burn");
            }
        }

        private bool IsValidTarget(AutoTargetMover mover, Health target)
        {
            if (target == null || target.IsDead || target.gameObject == Owner || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            FactionMember faction = target.GetComponent<FactionMember>();

            return faction != null && faction.IsFaction(mover.TargetFactionId);
        }

        private bool IsInsideWall(Vector3 position)
        {
            Vector3 offset = position - wallCenter;

            offset.y = 0f;

            float alongWall = Mathf.Abs(Vector3.Dot(offset, wallTangent));

            float throughWall = Mathf.Abs(Vector3.Dot(offset, wallNormal));

            return alongWall <= WallLength * 0.5f && throughWall <= WallThickness * 0.5f;
        }

        private void DrawDebugWall()
        {
            Vector3 halfTangent = wallTangent * WallLength * 0.5f;

            Vector3 halfNormal = wallNormal * WallThickness * 0.5f;

            Vector3 a = wallCenter - halfTangent - halfNormal;

            Vector3 b = wallCenter + halfTangent - halfNormal;

            Vector3 c = wallCenter + halfTangent + halfNormal;

            Vector3 d = wallCenter - halfTangent + halfNormal;

            float debugDuration = Mathf.Max(0.02f, scanInterval * 2f);

            Debug.DrawLine(a, b, Color.red, debugDuration, false);

            Debug.DrawLine(b, c, Color.red, debugDuration, false);

            Debug.DrawLine(c, d, Color.red, debugDuration, false);

            Debug.DrawLine(d, a, Color.red, debugDuration, false);
        }

        private float Evaluate(AbilityLevelValue value)
        {
            return value != null ? value.Evaluate(Level) : 0f;
        }
    }
}
