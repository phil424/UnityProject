using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public sealed class LevelEncounterDefinitionBinding : MonoBehaviour
    {
        [SerializeField] private EncounterDefinition definition;
        [SerializeField] private LevelEncounter encounter;

        public EncounterDefinition Definition => definition;

        private void Awake()
        {
            CacheEncounter();
        }

        private void Start()
        {
            ApplyDefinition();
        }

        [ContextMenu("Debug/Apply Encounter Definition")]
        public void ApplyDefinition()
        {
            CacheEncounter();

            if (definition == null || encounter == null)
                return;

            if (!EncounterDefinitionRuntimeApplicator.TryApply(
                    definition,
                    encounter,
                    out string error))
            {
                Debug.LogError(
                    $"Could not apply Encounter Definition '{definition.name}' " +
                    $"to '{name}': {error}",
                    this
                );

                return;
            }

            Debug.Log(
                $"Applied Encounter Definition '{definition.name}' to '{encounter.name}'.",
                this
            );
        }

        private void CacheEncounter()
        {
            if (encounter == null)
                encounter = GetComponent<LevelEncounter>();
        }

        private void OnValidate()
        {
            CacheEncounter();
        }
    }
}