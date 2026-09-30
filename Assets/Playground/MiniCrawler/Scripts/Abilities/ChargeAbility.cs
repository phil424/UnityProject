using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public class ChargeAbility :
        ActorAbility
    {
        private const float
            MinimumDirectionSquared =
                0.0001f;

        [Header("Charge")]
        [SerializeField]
        private AbilityLevelValue
            travelDistance;

        [SerializeField]
        private AbilityLevelValue
            travelSpeed;

        [SerializeField]
        private AbilityLevelValue
            contactRadius;

        [SerializeField]
        private AbilityLevelValue
            damage;

        [SerializeField]
        private AbilityLevelValue
            knockbackDistance;

        [SerializeField]
        private AbilityLevelValue
            knockbackSpeed;
            
        [SerializeField]
        [Range(0f, 89f)] private float knockbackFanAngle = 55f;

        private readonly HashSet<Health>
            hitTargets = new();

        private ForcedMotion forcedMotion;

        private Vector3 chargeDirection;

        private Vector3 lastSamplePosition;

        public float TravelDistance =>
            Mathf.Max(
                0f,
                Evaluate(
                    travelDistance
                )
            );

        public float TravelSpeed =>
            Mathf.Max(
                0f,
                Evaluate(
                    travelSpeed
                )
            );

        public float ContactRadius =>
            Mathf.Max(
                0.05f,
                Evaluate(
                    contactRadius
                )
            );

        public float Damage =>
            Mathf.Max(
                0f,
                Evaluate(
                    damage
                )
            );

        public float KnockbackDistance =>
            Mathf.Max(
                0f,
                Evaluate(
                    knockbackDistance
                )
            );

        public float KnockbackSpeed =>
            Mathf.Max(
                0f,
                Evaluate(
                    knockbackSpeed
                )
            );
            
        public float KnockbackFanAngle =>
            Mathf.Clamp(
                knockbackFanAngle,
                0f,
                89f
            );

        protected override bool
            CanActivateAbility()
        {
            if (
                Owner == null ||
                TravelDistance <= 0f ||
                TravelSpeed <= 0f
            )
            {
                return false;
            }

            AutoTargetMover mover =
                Owner.GetComponent<
                    AutoTargetMover
                >();

            if (
                mover == null ||
                !mover.isActiveAndEnabled ||
                mover.CurrentIntent !=
                    TargetIntent.Combat
            )
            {
                return false;
            }

            Health target =
                mover.CurrentTarget;

            if (
                target == null ||
                target.IsDead
            )
            {
                return false;
            }

            ForcedMotion motion =
                Owner.GetComponent<
                    ForcedMotion
                >();

            if (motion == null)
                return false;

            Vector3 direction =
                target.transform.position -
                Owner.transform.position;

            direction.y = 0f;

            return
                direction.sqrMagnitude >
                MinimumDirectionSquared;
        }

        protected override bool
            ExecuteAbility()
        {
            if (Owner == null)
                return false;

            AutoTargetMover mover =
                Owner.GetComponent<
                    AutoTargetMover
                >();

            if (mover == null)
                return false;

            Health target =
                mover.CurrentTarget;

            if (
                target == null ||
                target.IsDead
            )
            {
                return false;
            }

            Vector3 direction =
                target.transform.position -
                Owner.transform.position;

            direction.y = 0f;

            if (
                direction.sqrMagnitude <=
                MinimumDirectionSquared
            )
            {
                return false;
            }

            forcedMotion =
                Owner.GetComponent<
                    ForcedMotion
                >();

            if (forcedMotion == null)
                return false;

            if (!BeginExecution())
                return false;

            chargeDirection =
                direction.normalized;

            hitTargets.Clear();

            lastSamplePosition =
                Owner.transform.position;

            FaceChargeDirection();

            bool started =
                forcedMotion.StartMotion(
                    chargeDirection,
                    TravelDistance,
                    TravelSpeed,
                    canPropagate: false
                );

            if (!started)
            {
                EndExecution();

                return false;
            }

            TryHitTargets(
                lastSamplePosition,
                lastSamplePosition
            );

            Debug.Log(
                $"[Ability] {Owner.name} " +
                $"started {AbilityName} | " +
                $"Distance " +
                $"{TravelDistance:0.##} | " +
                $"Speed " +
                $"{TravelSpeed:0.##}."
            );

            return true;
        }

        protected override void
            TickExecution(
                float deltaTime
            )
        {
            if (Owner == null)
            {
                EndExecution();

                return;
            }

            Vector3 currentPosition =
                Owner.transform.position;

            TryHitTargets(
                lastSamplePosition,
                currentPosition
            );

            lastSamplePosition =
                currentPosition;

            FaceChargeDirection();

            if (
                forcedMotion == null ||
                !forcedMotion.IsActive
            )
            {
                EndExecution();
            }
        }

        private void TryHitTargets(
            Vector3 segmentStart,
            Vector3 segmentEnd
        )
        {
            if (Owner == null)
                return;

            AutoTargetMover mover =
                Owner.GetComponent<
                    AutoTargetMover
                >();

            if (mover == null)
                return;

            Health[] possibleTargets =
                FindObjectsByType<Health>(
                    FindObjectsSortMode.None
                );

            foreach (
                Health target in possibleTargets
            )
            {
                if (
                    !IsValidTarget(
                        mover,
                        target
                    ) ||
                    hitTargets.Contains(
                        target
                    )
                )
                {
                    continue;
                }

                float targetRadius = 0f;

                EntityAvoidance avoidance =
                    target.GetComponent<
                        EntityAvoidance
                    >();

                if (avoidance != null)
                {
                    targetRadius =
                        avoidance.WorldRadius;
                }

                float hitDistance =
                    ContactRadius +
                    targetRadius;

                float distanceSquared =
                    DistanceSquaredToSegment(
                        target.transform.position,
                        segmentStart,
                        segmentEnd
                    );

                if (
                    distanceSquared >
                    hitDistance *
                    hitDistance
                )
                {
                    continue;
                }

                hitTargets.Add(
                    target
                );

                DamageResolver.ApplyDamage(
                    Owner,
                    target,
                    Damage,
                    AbilityName
                );

                if (!target.IsDead)
                {
                    Vector3 knockbackDirection =
                        ResolveKnockbackDirection(
                            target,
                            segmentStart,
                            segmentEnd,
                            hitDistance
                        );

                    KnockbackResolver
                        .TryApplyDirectional(
                            Owner,
                            target,
                            knockbackDirection,
                            KnockbackDistance,
                            KnockbackSpeed
                        );
                }
            }
        }
        
        private Vector3 ResolveKnockbackDirection(
            Health target,
            Vector3 segmentStart,
            Vector3 segmentEnd,
            float hitDistance
        )
        {
            if (target == null)
                return chargeDirection;

            Vector3 closestPoint =
                ClosestPointOnSegment(
                    target.transform.position,
                    segmentStart,
                    segmentEnd
                );

            Vector3 lateralDirection =
                target.transform.position -
                closestPoint;

            lateralDirection.y = 0f;

            float lateralDistance =
                lateralDirection.magnitude;

            if (
                lateralDistance <= 0.0001f ||
                KnockbackFanAngle <= 0f
            )
            {
                return chargeDirection;
            }

            float spreadAmount =
                Mathf.Clamp01(
                    lateralDistance /
                    Mathf.Max(
                        0.0001f,
                        hitDistance
                    )
                );

            float deflection =
                KnockbackFanAngle *
                spreadAmount;

            return
                KnockbackResolver
                    .ResolveDeflectedDirection(
                        chargeDirection,
                        lateralDirection,
                        deflection
                    );
        }

        private bool IsValidTarget(
            AutoTargetMover mover,
            Health target
        )
        {
            if (
                Owner == null ||
                target == null ||
                target.IsDead ||
                target.gameObject ==
                    Owner
            )
            {
                return false;
            }

            FactionMember faction =
                target.GetComponent<
                    FactionMember
                >();

            return
                faction != null &&
                faction.IsFaction(
                    mover.TargetFactionId
                );
        }

        private float DistanceSquaredToSegment(
            Vector3 point,
            Vector3 start,
            Vector3 end
        )
        {
            Vector3 closest =
                ClosestPointOnSegment(
                    point,
                    start,
                    end
                );

            point.y = 0f;

            return
                (point - closest)
                .sqrMagnitude;
        }

        private Vector3 ClosestPointOnSegment(
            Vector3 point,
            Vector3 start,
            Vector3 end
        )
        {
            point.y = 0f;
            start.y = 0f;
            end.y = 0f;

            Vector3 segment =
                end - start;

            float lengthSquared =
                segment.sqrMagnitude;

            if (
                lengthSquared <=
                MinimumDirectionSquared
            )
            {
                return start;
            }

            float t =
                Vector3.Dot(
                    point - start,
                    segment
                ) /
                lengthSquared;

            t = Mathf.Clamp01(t);

            return
                start +
                segment * t;
        }

        private void FaceChargeDirection()
        {
            if (
                Owner == null ||
                chargeDirection.sqrMagnitude <=
                    MinimumDirectionSquared
            )
            {
                return;
            }

            Owner.transform.rotation =
                Quaternion.LookRotation(
                    chargeDirection,
                    Vector3.up
                );
        }

        private float Evaluate(
            AbilityLevelValue value
        )
        {
            return
                value != null
                    ? value.Evaluate(Level)
                    : 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centre =
                Owner != null
                    ? Owner.transform.position
                    : transform.position;

            Gizmos.DrawWireSphere(
                centre,
                ContactRadius
            );
        }
    }
}