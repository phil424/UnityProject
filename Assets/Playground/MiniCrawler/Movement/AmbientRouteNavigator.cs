using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DefaultExecutionOrder(-250)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AmbientRouteGraph))]
    public sealed class AmbientRouteNavigator : MonoBehaviour
    {
        public static AmbientRouteNavigator Instance { get; private set; }

        [Header("Travel")]
        [SerializeField, Min(0.1f)] private float arrivalDistance = 1.5f;

        [Header("Destination Selection")]
        [SerializeField, Min(1)] private int preferredMinimumDestinationHops = 3;

        private readonly List<AmbientRouteNode> reachableNodes = new();
        private readonly List<AmbientRouteNode> preferredDestinations = new();
        private readonly List<AmbientRouteNode> fallbackDestinations = new();
        private readonly List<AmbientRouteNode> candidatePath = new();
        private readonly List<AmbientRouteNode> routePath = new();

        private AmbientRouteGraph routeGraph;

        private AmbientRouteNode currentNode;
        private AmbientRouteNode previousRouteOriginNode;
        private AmbientRouteNode destinationNode;
        private AmbientRouteNode targetNode;

        private int routePathIndex;

        public AmbientRouteNode CurrentNode => currentNode;
        public AmbientRouteNode DestinationNode => destinationNode;
        public AmbientRouteNode TargetNode => targetNode;
        public IReadOnlyList<AmbientRouteNode> CurrentPath => routePath;

        private void Awake()
        {
            Instance = this;
            routeGraph = GetComponent<AmbientRouteGraph>();
        }

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            if (!ShouldUseAmbientTravel())
            {
                ResetRoute();
                return;
            }

            PartyMember leader = FindLivingPartyMember();

            if (leader == null)
            {
                ClearPartyNavigationIntents();
                return;
            }

            if (HasAnyPartyCombatTarget())
            {
                ClearPartyNavigationIntents();
                return;
            }

            if (targetNode == null)
                BeginRouteFromParty(leader);

            if (targetNode == null)
            {
                ClearPartyNavigationIntents();
                return;
            }

            if (HasAnyPartyMemberArrived())
                HandleReachedTarget();

            ApplyPartyNavigationIntents();
        }

        private bool ShouldUseAmbientTravel()
        {
            RunDirector runDirector = RunDirector.Instance;

            if (runDirector == null ||
                runDirector.State != RunDirector.RunFlowState.InLevel)
            {
                return false;
            }

            StageDirector stageDirector = StageDirector.Instance;

            if (stageDirector == null ||
                stageDirector.State != StageDirector.LevelState.FightingMinions)
            {
                return false;
            }

            EncounterDirectionController encounterDirection =
                EncounterDirectionController.Instance;

            return encounterDirection == null ||
                   encounterDirection.SelectedEncounter == null;
        }

        private void BeginRouteFromParty(PartyMember leader)
        {
            currentNode = null;
            destinationNode = null;

            routePath.Clear();
            routePathIndex = 0;

            targetNode =
                routeGraph.FindNearestNode(
                    leader.transform.position
                );
        }

        private void HandleReachedTarget()
        {
            currentNode = targetNode;

            if (routePath.Count > 0 &&
                routePathIndex < routePath.Count - 1)
            {
                routePathIndex++;
                targetNode = routePath[routePathIndex];
                return;
            }

            SelectDestinationAndBuildPath();
        }

        private void SelectDestinationAndBuildPath()
        {
            destinationNode = null;
            targetNode = null;

            routePath.Clear();
            routePathIndex = 0;

            if (currentNode == null)
                return;

            routeGraph.GetReachableNodes(
                currentNode,
                reachableNodes
            );

            preferredDestinations.Clear();
            fallbackDestinations.Clear();

            foreach (AmbientRouteNode candidate in reachableNodes)
            {
                if (candidate == null ||
                    candidate == currentNode ||
                    candidate == previousRouteOriginNode)
                {
                    continue;
                }

                fallbackDestinations.Add(candidate);

                if (!routeGraph.TryFindShortestPath(
                        currentNode,
                        candidate,
                        candidatePath))
                {
                    continue;
                }

                int hopCount =
                    Mathf.Max(0, candidatePath.Count - 1);

                if (hopCount >= preferredMinimumDestinationHops)
                    preferredDestinations.Add(candidate);
            }

            if (fallbackDestinations.Count == 0)
            {
                foreach (AmbientRouteNode candidate in reachableNodes)
                {
                    if (candidate != null && candidate != currentNode)
                        fallbackDestinations.Add(candidate);
                }
            }

            List<AmbientRouteNode> destinationPool =
                preferredDestinations.Count > 0
                    ? preferredDestinations
                    : fallbackDestinations;

            if (destinationPool.Count == 0)
                return;

            AmbientRouteNode selectedDestination =
                destinationPool[
                    Random.Range(0, destinationPool.Count)
                ];

            if (!routeGraph.TryFindShortestPath(
                    currentNode,
                    selectedDestination,
                    routePath) ||
                routePath.Count < 2)
            {
                routePath.Clear();
                return;
            }

            previousRouteOriginNode = currentNode;
            destinationNode = selectedDestination;

            routePathIndex = 1;
            targetNode = routePath[routePathIndex];
        }

        private void ApplyPartyNavigationIntents()
        {
            if (targetNode == null)
                return;

            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover =
                    partyMember.GetComponent<AutoTargetMover>();

                if (mover == null || !mover.isActiveAndEnabled)
                    continue;

                ActorNavigationIntent navigation =
                    partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation == null)
                {
                    navigation =
                        partyMember.gameObject
                            .AddComponent<ActorNavigationIntent>();
                }

                navigation.SetDestination(
                    this,
                    targetNode.Position,
                    arrivalDistance,
                    suppressCombatTargeting: false
                );
            }
        }

        private bool HasAnyPartyMemberArrived()
        {
            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                ActorNavigationIntent navigation =
                    partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation != null &&
                    navigation.IsOwnedBy(this) &&
                    navigation.HasArrived(partyMember.transform))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyPartyCombatTarget()
        {
            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover =
                    partyMember.GetComponent<AutoTargetMover>();

                if (mover != null &&
                    mover.CurrentIntent == TargetIntent.Combat &&
                    mover.CurrentTarget != null &&
                    !mover.CurrentTarget.IsDead)
                {
                    return true;
                }
            }

            return false;
        }

        private PartyMember FindLivingPartyMember()
        {
            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                if (IsLivingPartyMember(partyMember))
                    return partyMember;
            }

            return null;
        }

        private bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            StageDirector stageDirector = StageDirector.Instance;

            if (stageDirector != null &&
                !stageDirector.IsCurrentPartyMember(partyMember))
            {
                return false;
            }

            Health health =
                partyMember.GetComponent<Health>();

            return health == null || !health.IsDead;
        }

        private void ClearPartyNavigationIntents()
        {
            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                ActorNavigationIntent navigation =
                    partyMember != null
                        ? partyMember
                            .GetComponent<ActorNavigationIntent>()
                        : null;

                navigation?.Clear(this);
            }
        }

        private void ResetRoute()
        {
            ClearPartyNavigationIntents();

            currentNode = null;
            previousRouteOriginNode = null;
            destinationNode = null;
            targetNode = null;

            routePath.Clear();
            routePathIndex = 0;
        }

        private void OnValidate()
        {
            arrivalDistance =
                Mathf.Max(0.1f, arrivalDistance);

            preferredMinimumDestinationHops =
                Mathf.Max(
                    1,
                    preferredMinimumDestinationHops
                );
        }

        private void OnDrawGizmosSelected()
        {
            if (routePath.Count < 2)
                return;

            Gizmos.color = Color.cyan;

            for (int i = 0;
                 i < routePath.Count - 1;
                 i++)
            {
                AmbientRouteNode from = routePath[i];
                AmbientRouteNode to = routePath[i + 1];

                if (from != null && to != null)
                {
                    Gizmos.DrawLine(
                        from.Position + Vector3.up * 0.1f,
                        to.Position + Vector3.up * 0.1f
                    );
                }
            }

            if (destinationNode != null)
            {
                Gizmos.DrawWireSphere(
                    destinationNode.Position,
                    0.65f
                );
            }
        }

        private void OnDestroy()
        {
            ClearPartyNavigationIntents();

            if (Instance == this)
                Instance = null;
        }
    }
}