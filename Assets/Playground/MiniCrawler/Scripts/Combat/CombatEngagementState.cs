using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public class CombatEngagementState : MonoBehaviour
    {
        public bool IsEngaged { get; private set; } = true;

        public void SetEngaged(bool engaged)
        {
            IsEngaged = engaged;
        }

        public static bool AllowsCombat(GameObject actor)
        {
            if (actor == null)
                return false;

            CombatEngagementState state = actor.GetComponent<CombatEngagementState>();
            return state == null || state.IsEngaged;
        }
    }
}