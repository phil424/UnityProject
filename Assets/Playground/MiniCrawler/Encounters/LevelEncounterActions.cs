using System;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [Serializable]
    public class LevelEncounterActions
    {
        [SerializeField] private LevelEncounter[] makeKnownEncounters;
        [SerializeField] private LevelEncounter[] makeAvailableEncounters;
        [SerializeField] private LevelEncounter[] expireEncounters;

        public bool HasPendingActions =>
            HasPendingKnownAction() ||
            HasPendingAvailableAction() ||
            HasPendingExpireAction();

        public void Execute()
        {
            MakeKnown();
            MakeAvailable();
            Expire();
        }

        private void MakeKnown()
        {
            if (makeKnownEncounters == null)
                return;

            foreach (LevelEncounter encounter in makeKnownEncounters)
            {
                if (encounter != null)
                    encounter.MakeKnown();
            }
        }

        private void MakeAvailable()
        {
            if (makeAvailableEncounters == null)
                return;

            foreach (LevelEncounter encounter in makeAvailableEncounters)
            {
                if (encounter != null)
                    encounter.MakeAvailable();
            }
        }

        private void Expire()
        {
            if (expireEncounters == null)
                return;

            foreach (LevelEncounter encounter in expireEncounters)
            {
                if (encounter != null)
                    encounter.Expire();
            }
        }

        private bool HasPendingKnownAction()
        {
            if (makeKnownEncounters == null)
                return false;

            foreach (LevelEncounter encounter in makeKnownEncounters)
            {
                if (encounter != null && !encounter.IsKnown)
                    return true;
            }

            return false;
        }

        private bool HasPendingAvailableAction()
        {
            if (makeAvailableEncounters == null)
                return false;

            foreach (LevelEncounter encounter in makeAvailableEncounters)
            {
                if (encounter != null &&
                    !encounter.IsAvailable &&
                    !encounter.IsCompleted &&
                    !encounter.IsExpired)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasPendingExpireAction()
        {
            if (expireEncounters == null)
                return false;

            foreach (LevelEncounter encounter in expireEncounters)
            {
                if (encounter != null && !encounter.IsCompleted && !encounter.IsExpired)
                    return true;
            }

            return false;
        }
    }
}