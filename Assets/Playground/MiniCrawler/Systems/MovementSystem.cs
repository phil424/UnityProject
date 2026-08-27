using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-200)]
    public class MovementSystem : MonoBehaviour
    {
        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            Actor[] actors =
                FindObjectsByType<Actor>(
                    FindObjectsSortMode.None
                );

            foreach (Actor actor in actors)
            {
                if (!CanMoveActor(actor))
                    continue;

                ForcedMotion forcedMotion =
                    actor.GetComponent<ForcedMotion>();

                if (
                    forcedMotion != null &&
                    forcedMotion.IsActive
                )
                {
                    ApplyForcedMotion(
                        actor.transform,
                        forcedMotion
                    );

                    continue;
                }

                AutoTargetMover mover =
                    actor.GetComponent<AutoTargetMover>();

                if (!CanMoveAutonomously(mover))
                    continue;

                MoveTowardsTarget(
                    mover,
                    mover.CurrentTarget
                );
            }
        }

        private bool CanMoveActor(
            Actor actor
        )
        {
            if (
                actor == null ||
                !actor.isActiveAndEnabled
            )
            {
                return false;
            }

            Health health =
                actor.GetComponent<Health>();

            return
                health == null ||
                !health.IsDead;
        }

        private bool CanMoveAutonomously(
            AutoTargetMover mover
        )
        {
            if (
                mover == null ||
                !mover.isActiveAndEnabled
            )
            {
                return false;
            }

            return
                mover.CurrentTarget != null &&
                !mover.CurrentTarget.IsDead;
        }

        private void ApplyForcedMotion(
            Transform actorTransform,
            ForcedMotion forcedMotion
        )
        {
            Vector3 displacement =
                forcedMotion.ConsumeDisplacement(
                    Time.deltaTime
                );

            actorTransform.position +=
                displacement;
        }

        private void MoveTowardsTarget(
            AutoTargetMover mover,
            Health target
        )
        {
            Vector3 current =
                mover.transform.position;

            Vector3 destination =
                target.transform.position;

            destination.y =
                current.y;

            Vector3 toTarget =
                destination - current;

            float distance =
                toTarget.magnitude;

            float stoppingDistance =
                GetStoppingDistance(mover);

            if (distance <= stoppingDistance)
                return;

            mover.transform.position =
                Vector3.MoveTowards(
                    current,
                    destination,
                    mover.MoveSpeed *
                    Time.deltaTime
                );

            FaceTarget(
                mover.transform,
                destination
            );
        }

        private float GetStoppingDistance(
            AutoTargetMover mover
        )
        {
            if (
                mover.CurrentIntent !=
                TargetIntent.Support
            )
            {
                return
                    mover.CombatStoppingDistance;
            }

            SupportStats supportStats =
                mover.GetComponent<SupportStats>();

            if (supportStats == null)
            {
                return
                    mover.SupportStoppingDistance;
            }

            // Never stop farther away than
            // the actor's healing range.
            return Mathf.Min(
                mover.SupportStoppingDistance,
                supportStats.HealRange
            );
        }

        private void FaceTarget(
            Transform moverTransform,
            Vector3 destination
        )
        {
            Vector3 toTarget =
                destination -
                moverTransform.position;

            toTarget.y =
                0f;

            if (toTarget.sqrMagnitude > 0.001f)
            {
                moverTransform.rotation =
                    Quaternion.LookRotation(
                        toTarget
                    );
            }
        }
    }
}