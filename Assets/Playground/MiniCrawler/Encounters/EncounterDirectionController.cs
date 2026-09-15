using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DefaultExecutionOrder(-350)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StageDirector))]
    public class EncounterDirectionController : MonoBehaviour
    {
        public static EncounterDirectionController Instance { get; private set; }

        [SerializeField, Min(0.1f)] private float arrivalDistance = 2.5f;

        private readonly List<AmbientRouteNode> routePath = new();

        private StageDirector stageDirector;

        private EncounterRouteSite selectedRouteSite;
        private AmbientRouteGraph selectedRouteGraph;

        private int routePathIndex = -1;
        private bool routeTravelActive;

        public event Action<LevelEncounter> SelectedEncounterChanged;

        public LevelEncounter SelectedEncounter { get; private set; }
        public bool IsTravelling { get; private set; }

        public float ArrivalDistance => arrivalDistance;

        public bool IsUsingRouteTravel => routeTravelActive;
        public IReadOnlyList<AmbientRouteNode> RoutePath => routePath;

        public AmbientRouteNode CurrentRouteTarget =>
            routeTravelActive &&
            routePathIndex >= 0 &&
            routePathIndex < routePath.Count
                ? routePath[routePathIndex]
                : null;

        private void Awake()
        {
            Instance = this;
            stageDirector = GetComponent<StageDirector>();
        }

        private void OnEnable()
        {
            if (stageDirector == null)
                stageDirector = GetComponent<StageDirector>();

            if (stageDirector == null)
                return;

            stageDirector.LevelCleared += HandleLevelCleared;
            stageDirector.StateChanged += HandleStageStateChanged;
        }

        private void OnDisable()
        {
            if (stageDirector == null)
                return;

            stageDirector.LevelCleared -= HandleLevelCleared;
            stageDirector.StateChanged -= HandleStageStateChanged;
        }

        private void OnDestroy()
        {
            ClearPartyNavigationIntents();

            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (!IsTravelling)
            {
                if (SelectedEncounter != null &&
                    (SelectedEncounter.IsCompleted ||
                     SelectedEncounter.IsExpired ||
                     !SelectedEncounter.IsKnown))
                {
                    ClearSelection();
                }

                return;
            }

            if (SelectedEncounter == null ||
                SelectedEncounter.IsCompleted ||
                SelectedEncounter.IsExpired ||
                !SelectedEncounter.IsKnown)
            {
                ClearSelection();
                return;
            }

            ApplyTravelIntentToParty();

            if (!HasAnyPartyMemberArrived())
                return;

            if (routeTravelActive)
            {
                HandleRouteWaypointReached();
                return;
            }

            ArriveAtSelectedEncounter();
        }

        public bool SelectEncounter(LevelEncounter encounter)
        {
            if (encounter == null || !encounter.IsSelectable)
                return false;

            if (SelectedEncounter == encounter && !IsTravelling)
                return false;

            SelectedEncounter = encounter;
            IsTravelling = true;

            PrepareRouteTravel();

            ClearPartyCombatTargets();
            ApplyTravelIntentToParty();

            SelectedEncounterChanged?.Invoke(
                SelectedEncounter
            );

            Debug.Log(
                routeTravelActive
                    ? $"Encounter directive selected: {encounter.DisplayName}. Following Route Geography."
                    : $"Encounter directive selected: {encounter.DisplayName}. Using direct travel fallback.",
                this
            );

            return true;
        }

        public void ClearSelection()
        {
            ClearPartyNavigationIntents();
            ClearRouteTravelState();

            SelectedEncounter = null;
            IsTravelling = false;

            SelectedEncounterChanged?.Invoke(null);
        }

        private void HandleLevelCleared()
        {
            ClearSelection();
        }

        private void HandleStageStateChanged(
            StageDirector.LevelState state)
        {
            if (state == StageDirector.LevelState.FightingBoss)
                ClearSelection();
        }

        private void PrepareRouteTravel()
        {
            ClearRouteTravelState();

            if (SelectedEncounter == null)
                return;

            EncounterRouteSite routeSite =
                SelectedEncounter
                    .GetComponent<EncounterRouteSite>();

            if (routeSite == null)
                return;

            AmbientRouteGraph routeGraph =
                routeSite.RouteGraph;

            PartyMember leader =
                FindLivingPartyMember();

            if (routeGraph == null || leader == null)
                return;

            AmbientRouteNode start =
                routeGraph.FindNearestNode(
                    leader.transform.position
                );

            if (start == null)
                return;

            if (!routeSite.TryBuildApproachPath(
                    routeGraph,
                    start,
                    routePath,
                    out _) ||
                routePath.Count == 0)
            {
                routePath.Clear();
                return;
            }

            selectedRouteSite = routeSite;
            selectedRouteGraph = routeGraph;

            routePathIndex = 0;
            routeTravelActive = true;
        }

        private void HandleRouteWaypointReached()
        {
            if (!routeTravelActive ||
                routePathIndex < 0 ||
                routePathIndex >= routePath.Count)
            {
                return;
            }

            if (routePathIndex < routePath.Count - 1)
            {
                routePathIndex++;
                ApplyTravelIntentToParty();
                return;
            }

            routeTravelActive = false;
            routePathIndex = -1;

            Debug.Log(
                $"Encounter directive reached Route Site for '{SelectedEncounter.DisplayName}'. Beginning final local approach.",
                this
            );

            ApplyTravelIntentToParty();
        }

        private void ArriveAtSelectedEncounter()
        {
            if (SelectedEncounter == null)
                return;

            bool hasAuthoredStart =
                SelectedEncounter
                    .TryGetAuthoredPartyProximityStart(
                        out _,
                        out _
                    );

            if (!hasAuthoredStart)
            {
                SelectedEncounter.ActivateCombat();
                SelectedEncounter.BeginSpawning();
            }

            ClearPartyNavigationIntents();
            ClearRouteTravelState();

            IsTravelling = false;

            Debug.Log(
                hasAuthoredStart
                    ? $"Encounter directive reached authored start boundary: {SelectedEncounter.DisplayName}"
                    : $"Encounter directive arrived: {SelectedEncounter.DisplayName}",
                this
            );
        }

        private void ApplyTravelIntentToParty()
        {
            if (SelectedEncounter == null)
                return;

            ResolveTravelTarget(
                out Vector3 destination,
                out float stoppingDistance
            );

            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                AutoTargetMover mover =
                    partyMember.GetComponent<AutoTargetMover>();

                if (mover == null ||
                    !mover.isActiveAndEnabled)
                {
                    continue;
                }

                ActorNavigationIntent navigation =
                    partyMember
                        .GetComponent<ActorNavigationIntent>();

                if (navigation == null)
                {
                    navigation =
                        partyMember.gameObject
                            .AddComponent<ActorNavigationIntent>();
                }

                navigation.SetDestination(
                    this,
                    destination,
                    stoppingDistance,
                    suppressCombatTargeting: true
                );
            }
        }

        private void ResolveTravelTarget(
            out Vector3 destination,
            out float stoppingDistance)
        {
            if (routeTravelActive &&
                routePathIndex >= 0 &&
                routePathIndex < routePath.Count)
            {
                destination =
                    routePath[routePathIndex].Position;

                stoppingDistance = arrivalDistance;
                return;
            }

            destination =
                SelectedEncounter != null
                    ? SelectedEncounter.AnchorPosition
                    : Vector3.zero;

            stoppingDistance = arrivalDistance;

            if (SelectedEncounter == null)
                return;

            if (SelectedEncounter
                .TryGetAuthoredPartyProximityStart(
                    out Vector3 triggerCentre,
                    out float triggerRadius))
            {
                destination = triggerCentre;
                stoppingDistance = triggerRadius;
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
                    partyMember
                        .GetComponent<ActorNavigationIntent>();

                if (navigation != null &&
                    navigation.IsOwnedBy(this) &&
                    navigation.HasArrived(
                        partyMember.transform))
                {
                    return true;
                }
            }

            return false;
        }

        private void ClearPartyCombatTargets()
        {
            foreach (PartyMember partyMember in
                     FindObjectsByType<PartyMember>(
                         FindObjectsSortMode.None))
            {
                AutoTargetMover mover =
                    partyMember != null
                        ? partyMember
                            .GetComponent<AutoTargetMover>()
                        : null;

                if (mover != null &&
                    mover.CurrentIntent ==
                    TargetIntent.Combat)
                {
                    mover.ClearTarget();
                }
            }
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

        private void ClearRouteTravelState()
        {
            selectedRouteSite = null;
            selectedRouteGraph = null;

            routePath.Clear();
            routePathIndex = -1;
            routeTravelActive = false;
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

        private bool IsLivingPartyMember(
            PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            if (stageDirector != null &&
                !stageDirector.IsCurrentPartyMember(
                    partyMember))
            {
                return false;
            }

            Health health =
                partyMember.GetComponent<Health>();

            return health == null || !health.IsDead;
        }

        private void OnDrawGizmosSelected()
        {
            if (routePath.Count < 2)
                return;

            Gizmos.color = Color.yellow;

            for (int i = 0;
                 i < routePath.Count - 1;
                 i++)
            {
                AmbientRouteNode from =
                    routePath[i];

                AmbientRouteNode to =
                    routePath[i + 1];

                if (from == null || to == null)
                    continue;

                Gizmos.DrawLine(
                    from.Position +
                    Vector3.up * 0.25f,
                    to.Position +
                    Vector3.up * 0.25f
                );
            }

            AmbientRouteNode currentTarget =
                CurrentRouteTarget;

            if (currentTarget != null)
            {
                Gizmos.DrawWireSphere(
                    currentTarget.Position,
                    0.8f
                );
            }
        }
    }
}