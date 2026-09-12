using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public sealed class PersistentMemberProgress
    {
        public int WeaponLevel { get; private set; }
        public int ArmourLevel { get; private set; }
        public int FocusLevel { get; private set; }

        public int GetLevel(GearSlot slot)
        {
            return slot switch
            {
                GearSlot.Weapon => WeaponLevel,
                GearSlot.Armour => ArmourLevel,
                GearSlot.Focus => FocusLevel,
                _ => 0
            };
        }

        public void Increase(GearSlot slot)
        {
            switch (slot)
            {
                case GearSlot.Weapon:
                    WeaponLevel++;
                    break;

                case GearSlot.Armour:
                    ArmourLevel++;
                    break;

                case GearSlot.Focus:
                    FocusLevel++;
                    break;
            }
        }
    }

    public static class PersistentProgression
    {
        public static event Action Changed;

        private static readonly Dictionary<string, PersistentMemberProgress> memberProgress = new();

        public static int Currency { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            Currency = 0;
            memberProgress.Clear();
        }

        public static PersistentMemberProgress GetMemberProgress(PartyMemberDefinition definition)
        {
            if (definition == null)
                return new PersistentMemberProgress();

            if (!memberProgress.TryGetValue(definition.Id, out PersistentMemberProgress progress))
            {
                progress = new PersistentMemberProgress();
                memberProgress.Add(definition.Id, progress);
            }

            return progress;
        }

        public static void AddCurrency(int amount)
        {
            if (amount <= 0)
                return;

            Currency += amount;
            Changed?.Invoke();
        }

        public static int GetUpgradeCost(PartyMemberDefinition definition, GearSlot slot)
        {
            if (definition == null)
                return int.MaxValue;

            int currentLevel = GetMemberProgress(definition).GetLevel(slot);

            return definition.BaseUpgradeCost + currentLevel * definition.UpgradeCostStep;
        }

        public static bool TryBuyUpgrade(PartyMemberDefinition definition, GearSlot slot)
        {
            int cost = GetUpgradeCost(definition, slot);

            if (cost == int.MaxValue || Currency < cost)
                return false;

            Currency -= cost;
            GetMemberProgress(definition).Increase(slot);

            Changed?.Invoke();
            return true;
        }

        public static float GetDamageBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetMemberProgress(definition).WeaponLevel * definition.DamagePerWeaponLevel;
        }

        public static float GetArmourBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetMemberProgress(definition).ArmourLevel * definition.ArmourPerArmourLevel;
        }

        public static float GetHealthBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetMemberProgress(definition).ArmourLevel * definition.HealthPerArmourLevel;
        }

        public static float GetHealingBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetMemberProgress(definition).FocusLevel * definition.HealingPerFocusLevel;
        }
    }
}