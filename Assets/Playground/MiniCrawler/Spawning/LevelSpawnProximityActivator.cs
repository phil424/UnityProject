using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Spawning
{
    [DisallowMultipleComponent]
    public class LevelSpawnProximityActivator : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float activationRadius = 5f;

        [Header("Actions")]
        [SerializeField] private LevelSpawnGroupActions actions = new();

        public float ActivationRadius => activationRadius;

        private void Update()
        {
            if (actions == null || !actions.HasPendingActions)
                return;

            PartyMember[] partyMembers = FindObjectsByType<PartyMember>(FindObjectsSortMode.None);
            float activationRadiusSquared = activationRadius * activationRadius;

            foreach (PartyMember partyMember in partyMembers)
            {
                if (!IsLivingPartyMember(partyMember))
                    continue;

                Vector3 difference = partyMember.transform.position - transform.position;

                if (difference.sqrMagnitude > activationRadiusSquared)
                    continue;

                actions.Execute();
                return;
            }
        }

        [ContextMenu("Debug/Trigger Actions")]
        private void DebugTriggerActions()
        {
            actions?.Execute();
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
            activationRadius = Mathf.Max(0.1f, activationRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, activationRadius);
        }
    }
}