using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [Serializable]
    public class LevelEncounterActions
    {
        [Serializable]
        public class Entry
        {
            [SerializeField] private LevelEncounter target;
            [SerializeField] private LevelEncounterCommands commands = new();

            public bool HasPending(LevelEncounter context)
            {
                LevelEncounter resolvedTarget = target != null ? target : context;
                return commands != null && commands.HasPending(resolvedTarget);
            }

            public void Execute(LevelEncounter context)
            {
                LevelEncounter resolvedTarget = target != null ? target : context;
                commands?.Execute(resolvedTarget);
            }
        }

        [SerializeField] private List<Entry> entries = new();

        public bool HasPendingActions(LevelEncounter context = null)
        {
            foreach (Entry entry in entries)
            {
                if (entry != null && entry.HasPending(context))
                    return true;
            }

            return false;
        }

        public void Execute(LevelEncounter context = null)
        {
            foreach (Entry entry in entries)
                entry?.Execute(context);
        }
    }
}