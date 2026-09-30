using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [DisallowMultipleComponent]
    public sealed class AbilityExecutionState :
        MonoBehaviour
    {
        private readonly HashSet<ActorAbility>
            blockingAbilities = new();

        public bool BlocksAutonomousActions =>
            blockingAbilities.Count > 0;

        internal void BeginBlocking(
            ActorAbility ability
        )
        {
            if (ability == null)
                return;

            blockingAbilities.Add(
                ability
            );
        }

        internal void EndBlocking(
            ActorAbility ability
        )
        {
            if (ability == null)
                return;

            blockingAbilities.Remove(
                ability
            );
        }

        private void OnDisable()
        {
            blockingAbilities.Clear();
        }
    }
}