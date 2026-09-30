using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class ActorNavigationIntent : MonoBehaviour
    {
        public bool IsActive { get; private set; }
        public Object Owner { get; private set; }
        public Vector3 Destination { get; private set; }
        public float StoppingDistance { get; private set; } = 1.5f;
        public bool SuppressCombatTargeting { get; private set; }

        public void SetDestination(Object owner, Vector3 destination, float stoppingDistance, bool suppressCombatTargeting = true)
        {
            Owner = owner;
            Destination = destination;
            StoppingDistance = Mathf.Max(0.1f, stoppingDistance);
            SuppressCombatTargeting = suppressCombatTargeting;
            IsActive = true;
        }

        public bool Clear(Object owner)
        {
            if (!IsActive || Owner != owner)
                return false;

            IsActive = false;
            Owner = null;
            SuppressCombatTargeting = false;
            return true;
        }

        public bool IsOwnedBy(Object owner)
        {
            return IsActive && Owner == owner;
        }

        public bool HasArrived(Transform actorTransform)
        {
            if (!IsActive || actorTransform == null)
                return false;

            Vector3 difference = Destination - actorTransform.position;
            difference.y = 0f;

            return difference.sqrMagnitude <= StoppingDistance * StoppingDistance;
        }

        private void OnDrawGizmosSelected()
        {
            if (!IsActive)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, Destination);
            Gizmos.DrawWireSphere(Destination, StoppingDistance);
        }
    }
}