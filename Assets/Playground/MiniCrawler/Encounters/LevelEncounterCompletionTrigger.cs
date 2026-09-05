using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public class LevelEncounterCompletionTrigger : MonoBehaviour
    {
        [SerializeField] private LevelEncounter sourceEncounter;

        [Header("On Completed")]
        [SerializeField] private LevelEncounterActions encounterActions = new();
        [SerializeField] private LevelSpawnGroupActions spawnGroupActions = new();

        private void OnEnable()
        {
            if (sourceEncounter != null)
                sourceEncounter.Completed += HandleEncounterCompleted;
        }

        private void OnDisable()
        {
            if (sourceEncounter != null)
                sourceEncounter.Completed -= HandleEncounterCompleted;
        }

        private void HandleEncounterCompleted(LevelEncounter encounter)
        {
            ExecuteActions();
        }

        [ContextMenu("Debug/Trigger Actions")]
        private void DebugTriggerActions()
        {
            ExecuteActions();
        }

        private void ExecuteActions()
        {
            encounterActions?.Execute();
            spawnGroupActions?.Execute();
        }
    }
}