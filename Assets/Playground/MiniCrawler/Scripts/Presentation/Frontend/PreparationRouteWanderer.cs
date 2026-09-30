using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DefaultExecutionOrder(-250)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor), typeof(ActorNavigationIntent))]
    public sealed class PreparationRouteWanderer : MonoBehaviour
    {
        private readonly List<AmbientRouteNode> neighbours = new();

        private AmbientRouteGraph routeGraph;
        private ActorNavigationIntent navigationIntent;
        private AutoTargetMover mover;
        private Health health;

        private CombatEngagementState engagementState;
        private Transform engagementTarget;

        private AmbientRouteNode currentNode;
        private AmbientRouteNode previousNode;
        private AmbientRouteNode targetNode;

        private float arrivalDistance = 0.35f;
        private float engagementRadius = 3.5f;

        private bool requiresProximityEngagement;
        private bool wasInCombat;

        private void Awake()
        {
            navigationIntent = GetComponent<ActorNavigationIntent>();
            mover = GetComponent<AutoTargetMover>();
            health = GetComponent<Health>();
        }

        public void Configure(
            AmbientRouteGraph graph,
            float stoppingDistance,
            Transform proximityEngagementTarget = null,
            float proximityEngagementRadius = 3.5f)
        {
            routeGraph = graph;
            arrivalDistance = Mathf.Max(0.1f, stoppingDistance);

            engagementTarget = proximityEngagementTarget;
            engagementRadius = Mathf.Max(0.1f, proximityEngagementRadius);
            requiresProximityEngagement = engagementTarget != null;

            if (requiresProximityEngagement)
            {
                engagementState = GetComponent<CombatEngagementState>();

                if (engagementState == null)
                    engagementState = gameObject.AddComponent<CombatEngagementState>();

                engagementState.SetEngaged(false);
            }

            ResetRouteFromCurrentPosition();
        }

        private void Update()
        {
            if (SimulationPause.IsPaused || routeGraph == null)
                return;

            if (health != null && health.IsDead)
                return;

            if (requiresProximityEngagement && engagementState != null && !engagementState.IsEngaged)
            {
                if (CanEngageTarget())
                {
                    engagementState.SetEngaged(true);
                    ClearRouteIntent();
                    return;
                }

                UpdateRouteTravel();
                return;
            }

            if (HasLivingCombatTarget())
            {
                wasInCombat = true;
                ClearRouteIntent();
                return;
            }

            if (wasInCombat)
            {
                wasInCombat = false;
                ResetRouteFromCurrentPosition();
            }

            UpdateRouteTravel();
        }

        private bool CanEngageTarget()
        {
            if (engagementTarget == null)
                return false;

            Health targetHealth = engagementTarget.GetComponent<Health>();

            if (targetHealth != null && targetHealth.IsDead)
                return false;

            Vector3 difference = engagementTarget.position - transform.position;
            difference.y = 0f;

            return difference.sqrMagnitude <= engagementRadius * engagementRadius;
        }

        private bool HasLivingCombatTarget()
        {
            return mover != null &&
                   mover.CurrentTarget != null &&
                   !mover.CurrentTarget.IsDead;
        }

        private void UpdateRouteTravel()
        {
            if (navigationIntent == null || routeGraph == null)
                return;

            if (navigationIntent.IsActive && !navigationIntent.IsOwnedBy(this))
                return;

            if (navigationIntent.IsOwnedBy(this))
            {
                if (!navigationIntent.HasArrived(transform))
                    return;

                navigationIntent.Clear(this);

                previousNode = currentNode;
                currentNode = targetNode;
                targetNode = null;
            }

            if (currentNode == null)
            {
                ResetRouteFromCurrentPosition();
                return;
            }

            BeginNextLeg();
        }

        private void ResetRouteFromCurrentPosition()
        {
            ClearRouteIntent();

            previousNode = null;
            currentNode = null;
            targetNode = routeGraph != null ? routeGraph.FindNearestNode(transform.position) : null;

            if (targetNode == null || navigationIntent == null)
                return;

            navigationIntent.SetDestination(
                this,
                targetNode.Position,
                arrivalDistance,
                suppressCombatTargeting: false
            );
        }

        private void BeginNextLeg()
        {
            routeGraph.GetNeighbours(currentNode, neighbours);

            if (neighbours.Count == 0)
                return;

            AmbientRouteNode nextNode = ChooseNextNode();

            if (nextNode == null)
                return;

            targetNode = nextNode;

            navigationIntent.SetDestination(
                this,
                targetNode.Position,
                arrivalDistance,
                suppressCombatTargeting: false
            );
        }

        private AmbientRouteNode ChooseNextNode()
        {
            if (neighbours.Count == 1)
                return neighbours[0];

            int startIndex = Random.Range(0, neighbours.Count);

            for (int offset = 0; offset < neighbours.Count; offset++)
            {
                AmbientRouteNode candidate = neighbours[(startIndex + offset) % neighbours.Count];

                if (candidate != null && candidate != previousNode)
                    return candidate;
            }

            return neighbours[startIndex];
        }

        private void ClearRouteIntent()
        {
            navigationIntent?.Clear(this);
        }

        private void OnDisable()
        {
            ClearRouteIntent();
        }

        private void OnDrawGizmosSelected()
        {
            if (!requiresProximityEngagement)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, engagementRadius);
        }
    }
}