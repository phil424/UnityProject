using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Spawning
{
    [DisallowMultipleComponent]
    public class LevelSpawnProximityActivator : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float activationRadius = 5f;
        [SerializeField] private LevelSpawnGroup[] spawnGroups;

        public float ActivationRadius => activationRadius;

        private void Update()
        {
            if (!HasUnactivatedGroup())
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

                ActivateGroups();
                return;
            }
        }

        private void ActivateGroups()
        {
            if (spawnGroups == null)
                return;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null)
                    group.Activate();
            }
        }

        private bool HasUnactivatedGroup()
        {
            if (spawnGroups == null)
                return false;

            foreach (LevelSpawnGroup group in spawnGroups)
            {
                if (group != null && !group.IsActivated)
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