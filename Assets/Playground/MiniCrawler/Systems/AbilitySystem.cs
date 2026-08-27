using MiniCrawler.Abilities;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-50)]
    public class AbilitySystem : MonoBehaviour
    {
        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            ActorAbility[] abilities =
                FindObjectsByType<ActorAbility>(
                    FindObjectsSortMode.None
                );

            foreach (
                ActorAbility ability in abilities
            )
            {
                if (
                    ability == null ||
                    !ability.isActiveAndEnabled
                )
                {
                    continue;
                }

                ability.TickCooldown(
                    Time.deltaTime
                );
            }
        }
    }
}