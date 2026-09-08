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

        [SerializeField, Min(0.1f)] private float arrivalDistance = 1.5f;

        private readonly List<AmbientRouteNode> neighbours = new();
        private readonly List<AmbientRouteNode> eligibleNeighbours = new();

        private AmbientRouteGraph routeGraph;
        private AmbientRouteNode previousNode;
        private AmbientRouteNode targetNode;

        public AmbientRouteNode TargetNode => targetNode;

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
                targetNode = routeGraph.FindNearestNode(leader.transform.position);

            if (targetNode == null)
            {
                ClearPartyNavigationIntents();
                return;
            }

            if (HasAnyPartyMemberArrived())
                AdvanceRoute();

            ApplyPartyNavigationIntents();
        }

        private bool ShouldUseAmbientTravel()
        {
            RunDirector runDirector = RunDirector.Instance;

            if (runDirector == null || runDirector.State != RunDirector.RunFlowState.InLevel)
                return false;

            StageDirector stageDirector = StageDirector.Instance;

            if (stageDirector == null || stageDirector.State != StageDirector.LevelState.FightingMinions)
                return false;

            EncounterDirectionController encounterDirection = EncounterDirectionController.Instance;

            return encounterDirection == null || encounterDirection.SelectedEncounter == null;
        }

        private void AdvanceRoute()
        {
            AmbientRouteNode reachedNode = targetNode;

            routeGraph.GetNeighbours(reachedNode, neighbours);

            if (neighbours.Count == 0)
            {
                previousNode = null;
                targetNode = null;
                return;
            }

            eligibleNeighbours.Clear();

            foreach (AmbientRouteNode neighbour in neighbours)
            {
                if (neighbour != previousNode)
                    eligibleNeighbours.Add(neighbour);
            }

            if (eligibleNeighbours.Count == 0)
                eligibleNeighbours.AddRange(neighbours);

            previousNode = reachedNode;
            targetNode = eligibleNeighbours[Random.Range(0, eligibleNeighbours.Count)];
        }

        private void ApplyPartyNavigationIntents()
        {
            if (targetNode == null)
                return;

            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover = partyMember.GetComponent<AutoTargetMover>();

                if (mover == null || !mover.isActiveAndEnabled)
                    continue;

                ActorNavigationIntent navigation = partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation == null)
                    navigation = partyMember.gameObject.AddComponent<ActorNavigationIntent>();

                navigation.SetDestination(this, targetNode.Position, arrivalDistance, suppressCombatTargeting: false);
            }
        }

        private bool HasAnyPartyMemberArrived()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                ActorNavigationIntent navigation = partyMember.GetComponent<ActorNavigationIntent>();

                if (navigation != null && navigation.IsOwnedBy(this) && navigation.HasArrived(partyMember.transform))
                    return true;
            }

            return false;
        }

        private bool HasAnyPartyCombatTarget()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover = partyMember.GetComponent<AutoTargetMover>();

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
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (IsLivingPartyMember(partyMember))
                    return partyMember;
            }

            return null;
        }

        private void ClearPartyNavigationIntents()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                ActorNavigationIntent navigation = partyMember != null
                    ? partyMember.GetComponent<ActorNavigationIntent>()
                    : null;

                navigation?.Clear(this);
            }
        }

        private void ResetRoute()
        {
            ClearPartyNavigationIntents();

            previousNode = null;
            targetNode = null;
        }

        private static bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            Health health = partyMember.GetComponent<Health>();
            return health == null || !health.IsDead;
        }

        private void OnDestroy()
        {
            ClearPartyNavigationIntents();

            if (Instance == this)
                Instance = null;
        }
    }
}