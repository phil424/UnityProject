using System;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [Serializable]
    public class LevelEncounterCommands
    {
        [SerializeField] private bool makeKnown;
        [SerializeField] private bool makeAvailable;
        [SerializeField] private bool beginSpawning;
        [SerializeField] private bool activateCombat;
        [SerializeField] private bool expire;

        public bool HasAny =>
            makeKnown ||
            makeAvailable ||
            beginSpawning ||
            activateCombat ||
            expire;

        public bool HasPending(LevelEncounter encounter)
        {
            if (encounter == null)
                return false;

            if (makeKnown && !encounter.IsKnown)
                return true;

            if (makeAvailable && !encounter.IsAvailable && !encounter.IsCompleted && !encounter.IsExpired)
                return true;

            if (activateCombat && encounter.HasInactiveCombatGroups)
                return true;

            if (beginSpawning && encounter.HasUnstartedSpawnGroups)
                return true;

            if (expire && !encounter.IsCompleted && !encounter.IsExpired)
                return true;

            return false;
        }

        public void Execute(LevelEncounter encounter)
        {
            if (encounter == null)
                return;

            if (makeKnown)
                encounter.MakeKnown();

            if (makeAvailable)
                encounter.MakeAvailable();

            // Combat first so immediate spawns inherit the intended state.
            if (activateCombat)
                encounter.ActivateCombat();

            if (beginSpawning)
                encounter.BeginSpawning();

            if (expire)
                encounter.Expire();
        }
    }
}