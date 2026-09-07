using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public class EncounterMembership : MonoBehaviour
    {
        public LevelEncounter Encounter { get; private set; }

        public void SetEncounter(LevelEncounter encounter)
        {
            Encounter = encounter;
        }

        public bool BelongsTo(LevelEncounter encounter)
        {
            return Encounter != null && Encounter == encounter;
        }
    }
}