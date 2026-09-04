using System;
using UnityEngine;

namespace MiniCrawler.Spawning
{
    [Serializable]
    public class LevelSpawnGroupActions
    {
        [SerializeField] private LevelSpawnGroup[] beginSpawningGroups;
        [SerializeField] private LevelSpawnGroup[] activateCombatGroups;

        public bool HasPendingActions
        {
            get
            {
                if (HasPendingSpawnAction())
                    return true;

                return HasPendingCombatAction();
            }
        }

        public void Execute()
        {
            // Combat activation happens first so groups that are also beginning
            // to spawn will create their first actors in the correct state.
            ActivateCombat();
            BeginSpawning();
        }

        private void ActivateCombat()
        {
            if (activateCombatGroups == null)
                return;

            foreach (LevelSpawnGroup group in activateCombatGroups)
            {
                if (group != null)
                    group.ActivateCombat();
            }
        }

        private void BeginSpawning()
        {
            if (beginSpawningGroups == null)
                return;

            foreach (LevelSpawnGroup group in beginSpawningGroups)
            {
                if (group != null)
                    group.BeginSpawning();
            }
        }

        private bool HasPendingSpawnAction()
        {
            if (beginSpawningGroups == null)
                return false;

            foreach (LevelSpawnGroup group in beginSpawningGroups)
            {
                if (group != null && !group.IsSpawningStarted)
                    return true;
            }

            return false;
        }

        private bool HasPendingCombatAction()
        {
            if (activateCombatGroups == null)
                return false;

            foreach (LevelSpawnGroup group in activateCombatGroups)
            {
                if (group != null && !group.IsCombatActive)
                    return true;
            }

            return false;
        }
    }
}