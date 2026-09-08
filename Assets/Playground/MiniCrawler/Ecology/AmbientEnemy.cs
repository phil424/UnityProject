using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Ecology
{
    [DefaultExecutionOrder(-320)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyMember))]
    public sealed class AmbientEnemy : MonoBehaviour
    {
        private CombatEngagementState engagementState;
        private Vector3 homePosition;
        private float engagementRadius = 3.5f;

        public Vector3 HomePosition => homePosition;
        public bool IsEngaged => engagementState != null && engagementState.IsEngaged;

        private void Awake()
        {
            EnsureEngagementState();
            engagementState.SetEngaged(false);
        }

        public void Configure(Vector3 home, float radius)
        {
            homePosition = home;
            engagementRadius = Mathf.Max(0.1f, radius);

            EnsureEngagementState();
            engagementState.SetEngaged(false);
        }

        private void Update()
        {
            if (engagementState == null || engagementState.IsEngaged)
                return;

            float radiusSquared = engagementRadius * engagementRadius;

            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                Vector3 difference = partyMember.transform.position - transform.position;
                difference.y = 0f;

                if (difference.sqrMagnitude > radiusSquared)
                    continue;

                engagementState.SetEngaged(true);
                return;
            }
        }

        private void EnsureEngagementState()
        {
            engagementState = GetComponent<CombatEngagementState>();

            if (engagementState == null)
                engagementState = gameObject.AddComponent<CombatEngagementState>();
        }

        private static bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            Health health = partyMember.GetComponent<Health>();
            return health == null || !health.IsDead;
        }
    }
}