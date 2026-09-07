using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public class LevelEncounterProximityTrigger : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float triggerRadius = 5f;
        [SerializeField] private bool requireEncounterAvailable = true;

        [Header("When Party Enters")]
        [SerializeField] private LevelEncounterCommands commands = new();

        private LevelEncounter encounter;

        private void Awake()
        {
            ResolveEncounter();
        }

        private void OnEnable()
        {
            ResolveEncounter();
        }

        private void Update()
        {
            if (encounter == null || commands == null)
                return;

            if (requireEncounterAvailable && !encounter.IsAvailable)
                return;

            if (!commands.HasPending(encounter))
                return;

            PartyMember[] partyMembers = FindObjectsByType<PartyMember>(FindObjectsSortMode.None);
            float radiusSquared = triggerRadius * triggerRadius;

            foreach (PartyMember partyMember in partyMembers)
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                Vector3 difference = partyMember.transform.position - transform.position;

                if (difference.sqrMagnitude > radiusSquared)
                    continue;

                commands.Execute(encounter);
                return;
            }
        }

        [ContextMenu("Debug/Trigger")]
        private void DebugTrigger()
        {
            commands?.Execute(encounter);
        }

        private void ResolveEncounter()
        {
            encounter = GetComponentInParent<LevelEncounter>();
        }

        private static bool IsLivingPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            Health health = partyMember.GetComponent<Health>();
            return health == null || !health.IsDead;
        }

        private void OnValidate()
        {
            triggerRadius = Mathf.Max(0.1f, triggerRadius);
            ResolveEncounter();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}