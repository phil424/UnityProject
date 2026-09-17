using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Ecology
{
    [DefaultExecutionOrder(-320)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMember))]
    public sealed class AmbientEnemy : MonoBehaviour
    {
        private const float WanderStoppingDistance = 0.25f;

        private CombatEngagementState engagementState;
        private ActorNavigationIntent navigationIntent;

        private Vector3 homePosition;
        private float engagementRadius = 3.5f;
        private float wanderRadius;
        private float wanderPauseMinSeconds;
        private float wanderPauseMaxSeconds;
        private float wanderPauseRemaining;

        public Vector3 HomePosition => homePosition;
        public bool IsEngaged => engagementState != null && engagementState.IsEngaged;

        private void Awake()
        {
            EnsureEngagementState();
            engagementState.SetEngaged(false);
        }

        public void Configure(Vector3 home, float radius)
        {
            Configure(home, radius, 0f, 0f, 0f);
        }

        public void Configure(
            Vector3 home,
            float radius,
            float localWanderRadius,
            float pauseMinSeconds,
            float pauseMaxSeconds)
        {
            homePosition = home;
            engagementRadius = Mathf.Max(0.1f, radius);
            wanderRadius = Mathf.Max(0f, localWanderRadius);
            wanderPauseMinSeconds = Mathf.Max(0f, pauseMinSeconds);
            wanderPauseMaxSeconds = Mathf.Max(wanderPauseMinSeconds, pauseMaxSeconds);

            EnsureEngagementState();
            EnsureNavigationIntent();

            ClearWanderIntent();
            engagementState.SetEngaged(false);
            ScheduleWanderPause();
        }

        private void Update()
        {
            if (SimulationPause.IsPaused || engagementState == null)
                return;

            if (engagementState.IsEngaged)
            {
                ClearWanderIntent();
                return;
            }

            if (TryEngageParty())
                return;

            UpdateWandering();
        }

        private bool TryEngageParty()
        {
            float radiusSquared = engagementRadius * engagementRadius;

            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                Vector3 difference = partyMember.transform.position - transform.position;
                difference.y = 0f;

                if (difference.sqrMagnitude > radiusSquared)
                    continue;

                ClearWanderIntent();

                engagementState.SetEngaged(true);
                return true;
            }

            return false;
        }

        private void UpdateWandering()
        {
            if (wanderRadius <= 0f)
            {
                ClearWanderIntent();
                return;
            }

            EnsureNavigationIntent();

            if (navigationIntent == null)
                return;

            if (navigationIntent.IsActive && !navigationIntent.IsOwnedBy(this))
                return;

            if (navigationIntent.IsOwnedBy(this))
            {
                if (!navigationIntent.HasArrived(transform))
                    return;

                navigationIntent.Clear(this);
                ScheduleWanderPause();
            }

            if (wanderPauseRemaining > 0f)
            {
                wanderPauseRemaining = Mathf.Max(0f, wanderPauseRemaining - Time.deltaTime);
                return;
            }

            BeginWander();
        }

        private void BeginWander()
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 destination = homePosition + new Vector3(offset.x, 0f, offset.y);

            navigationIntent.SetDestination(
                this,
                destination,
                WanderStoppingDistance,
                suppressCombatTargeting: false
            );
        }

        private void ScheduleWanderPause()
        {
            wanderPauseRemaining = Random.Range(wanderPauseMinSeconds, wanderPauseMaxSeconds);
        }

        private void ClearWanderIntent()
        {
            navigationIntent?.Clear(this);
        }

        private void EnsureEngagementState()
        {
            engagementState = GetComponent<CombatEngagementState>();

            if (engagementState == null)
                engagementState = gameObject.AddComponent<CombatEngagementState>();
        }

        private void EnsureNavigationIntent()
        {
            if (navigationIntent != null)
                return;

            navigationIntent = GetComponent<ActorNavigationIntent>();

            if (navigationIntent == null)
                navigationIntent = gameObject.AddComponent<ActorNavigationIntent>();
        }

        private static bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            Health health = partyMember.GetComponent<Health>();
            return health == null || !health.IsDead;
        }

        private void OnDisable()
        {
            ClearWanderIntent();
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            if (wanderRadius > 0f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(homePosition, wanderRadius);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, engagementRadius);
        }
    }
}