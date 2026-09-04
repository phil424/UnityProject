using MiniCrawler.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniCrawler.Spawning
{
    [DisallowMultipleComponent]
    public class LevelSpawnProximityActivator : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float activationRadius = 5f;

        [FormerlySerializedAs("spawnGroups")]
        [SerializeField] private LevelSpawnGroup[] groups;

        public float ActivationRadius => activationRadius;

        private void Update()
        {
            if (!HasPendingGroupAction())
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

                TriggerGroups();
                return;
            }
        }

        private void TriggerGroups()
        {
            if (groups == null)
                return;

            foreach (LevelSpawnGroup group in groups)
            {
                if (group == null)
                    continue;

                // Combat state is applied first so an immediate first spawn
                // inherits the correct engagement state.
                group.ActivateCombat();
                group.BeginSpawning();
            }
        }

        private bool HasPendingGroupAction()
        {
            if (groups == null)
                return false;

            foreach (LevelSpawnGroup group in groups)
            {
                if (group != null && (!group.IsSpawningStarted || !group.IsCombatActive))
                    return true;
            }

            return false;
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