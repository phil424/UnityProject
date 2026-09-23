using MiniCrawler.Encounters;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Encounter Completion Objective",
        menuName = "Mini Crawler/Objectives/Encounter Completion"
    )]
    public sealed class EncounterCompletionObjectiveDefinition : ExpeditionObjectiveDefinition
    {
        [Header("Encounter")]
        [SerializeField] private EncounterDefinition encounterDefinition;

        public EncounterDefinition EncounterDefinition => encounterDefinition;

        public bool Matches(LevelEncounter encounter)
        {
            if (encounter == null || encounterDefinition == null)
                return false;

            LevelEncounterDefinitionBinding binding = encounter.GetComponent<LevelEncounterDefinitionBinding>();

            return binding != null && binding.Definition == encounterDefinition;
        }
    }
}