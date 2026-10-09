using System;
using MiniCrawler.WeaponMods;

namespace MiniCrawler.Progress
{
    [Serializable]
    public sealed class RunWeaponModState
    {
        private WeaponModDefinition definition;
        private WeaponModDefinition combinedMastery;
        private WeaponModStanceSlot stanceSlot;
        private int preparedLevel;
        private int expeditionLevelBonus;

        public WeaponModDefinition Definition => definition;
        public WeaponModDefinition CombinedMastery => combinedMastery;
        public WeaponModStanceSlot StanceSlot => stanceSlot;
        public int PreparedLevel => preparedLevel;
        public int ExpeditionLevelBonus => expeditionLevelBonus;
        public int Level => preparedLevel + expeditionLevelBonus;

        public RunWeaponModState(
            WeaponModDefinition weaponModDefinition,
            WeaponModStanceSlot slot,
            int startingPreparedLevel = 0)
        {
            definition = weaponModDefinition;
            stanceSlot = slot;
            preparedLevel = Math.Max(0, startingPreparedLevel);
            expeditionLevelBonus = 0;
        }

        public bool IsCombinedWith(WeaponModDefinition mod)
        {
            return mod != null &&
                   combinedMastery != null &&
                   combinedMastery.Id == mod.Id;
        }

        internal bool TryIncreasePreparedLevel(int amount = 1)
        {
            if (amount <= 0)
                return false;

            preparedLevel += amount;
            return true;
        }

        internal bool TryIncreaseExpeditionLevel(int amount = 1)
        {
            if (amount <= 0)
                return false;

            expeditionLevelBonus += amount;
            return true;
        }

        internal bool TryCombineWith(WeaponModDefinition mod)
        {
            if (mod == null ||
                definition == null ||
                combinedMastery != null ||
                mod.Id == definition.Id)
            {
                return false;
            }

            combinedMastery = mod;
            return true;
        }
    }
}