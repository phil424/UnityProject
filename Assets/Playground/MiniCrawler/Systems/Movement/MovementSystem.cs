using MiniCrawler.Abilities;
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

            Actor[] actors = FindObjectsByType<Actor>(FindObjectsSortMode.None);

            foreach (Actor actor in actors)
            {
                if (!CanMoveActor(actor))
                    continue;

                ForcedMotion forcedMotion = actor.GetComponent<ForcedMotion>();

                if (forcedMotion != null && forcedMotion.IsActive)
                {
                    ApplyForcedMotion(actor.transform, forcedMotion);
                    continue;
                }

                AbilityExecutionState abilityExecutionState = actor.GetComponent<AbilityExecutionState>();

                if (abilityExecutionState != null && abilityExecutionState.BlocksAutonomousActions)
                    continue;

                AutoTargetMover mover = actor.GetComponent<AutoTargetMover>();
                ActorNavigationIntent navigation = actor.GetComponent<ActorNavigationIntent>();

                if (navigation != null && navigation.IsActive)
                {
                    if (mover != null && mover.isActiveAndEnabled)
                    {
                        MoveTowardsDestination(
                            actor.transform,
                            navigation.Destination,
                            navigation.StoppingDistance,
                            mover.MoveSpeed
                        );
                    }

                    continue;
                }

                if (!CanMoveAutonomously(mover))
                    continue;

                MoveTowardsTarget(mover, mover.CurrentTarget);
            }
        }

        private bool CanMoveActor(Actor actor)
        {
            if (actor == null || !actor.isActiveAndEnabled)
                return false;

            Health health = actor.GetComponent<Health>();
            return health == null || !health.IsDead;
        }

        private bool CanMoveAutonomously(AutoTargetMover mover)
        {
            return mover != null &&
                   mover.isActiveAndEnabled &&
                   mover.CurrentTarget != null &&
                   !mover.CurrentTarget.IsDead;
        }

        private void ApplyForcedMotion(Transform actorTransform, ForcedMotion forcedMotion)
        {
            actorTransform.position += forcedMotion.ConsumeDisplacement(Time.deltaTime);
        }

        private void MoveTowardsTarget(AutoTargetMover mover, Health target)
        {
            MoveTowardsDestination(
                mover.transform,
                target.transform.position,
                GetStoppingDistance(mover),
                mover.MoveSpeed
            );
        }

        private void MoveTowardsDestination(Transform moverTransform, Vector3 destination, float stoppingDistance, float moveSpeed)
        {
            Vector3 current = moverTransform.position;

            destination.y = current.y;

            Vector3 toDestination = destination - current;
            float distance = toDestination.magnitude;

            if (distance <= stoppingDistance)
                return;

            moverTransform.position = Vector3.MoveTowards(current, destination, moveSpeed * Time.deltaTime);

            FaceTarget(moverTransform, destination);
        }

        private float GetStoppingDistance(AutoTargetMover mover)
        {
            if (mover.CurrentIntent != TargetIntent.Support)
                return mover.CombatStoppingDistance;

            SupportStats supportStats = mover.GetComponent<SupportStats>();

            if (supportStats == null)
                return mover.SupportStoppingDistance;

            return Mathf.Min(mover.SupportStoppingDistance, supportStats.HealRange);
        }

        private void FaceTarget(Transform moverTransform, Vector3 destination)
        {
            Vector3 toTarget = destination - moverTransform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > 0.001f)
                moverTransform.rotation = Quaternion.LookRotation(toTarget);
        }
    }
}