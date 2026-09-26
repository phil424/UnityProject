using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public sealed class PersistentMemberProgress
    {
        private readonly HashSet<string> unlockedMasteryNodeIds = new();

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

        public bool HasMasteryNode(CharacterMasteryNodeDefinition node)
        {
            return node != null && unlockedMasteryNodeIds.Contains(node.Id);
        }

        public bool TryUnlockMasteryNode(CharacterMasteryNodeDefinition node)
        {
            return node != null && unlockedMasteryNodeIds.Add(node.Id);
        }
    }

    public static class PersistentProgression
    {
        public static event Action Changed;

        private static readonly Dictionary<string, PersistentMemberProgress> memberProgress = new();

        public static int Level1Resources { get; private set; }
        public static int Level2Resources { get; private set; }
        public static int Level3Resources { get; private set; }

        // Temporary compatibility seam while the old gear progression is retired.
        public static int Currency => Level1Resources;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            Level1Resources = 0;
            Level2Resources = 0;
            Level3Resources = 0;
            memberProgress.Clear();
        }

        public static int GetResource(PersistentResourceTier tier)
        {
            return tier switch
            {
                PersistentResourceTier.Level1 => Level1Resources,
                PersistentResourceTier.Level2 => Level2Resources,
                PersistentResourceTier.Level3 => Level3Resources,
                _ => 0
            };
        }

        public static void AddResource(PersistentResourceTier tier, int amount)
        {
            if (amount <= 0)
                return;

            SetResource(tier, GetResource(tier) + amount);
            Changed?.Invoke();
        }

        public static bool TrySpendResource(PersistentResourceTier tier, int amount)
        {
            if (amount <= 0 || GetResource(tier) < amount)
                return false;

            SetResource(tier, GetResource(tier) - amount);
            Changed?.Invoke();
            return true;
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

        public static bool IsMasteryNodeUnlocked(
            PartyMemberDefinition definition,
            CharacterMasteryNodeDefinition node)
        {
            return definition != null &&
                   node != null &&
                   GetMemberProgress(definition).HasMasteryNode(node);
        }

        public static bool AreMasteryPrerequisitesMet(
            PartyMemberDefinition definition,
            CharacterMasteryNodeDefinition node)
        {
            if (definition == null || node == null)
                return false;

            PersistentMemberProgress progress = GetMemberProgress(definition);

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null || !progress.HasMasteryNode(prerequisite))
                    return false;
            }

            return true;
        }

        public static bool CanAffordMasteryNode(CharacterMasteryNodeDefinition node)
        {
            return node != null && CanAffordCosts(node.Costs);
        }

        public static bool CanUnlockMasteryNode(
            PartyMemberDefinition definition,
            CharacterMasteryNodeDefinition node)
        {
            if (definition == null ||
                node == null ||
                !node.IsConfigured ||
                !ContainsMasteryNode(definition, node) ||
                IsMasteryNodeUnlocked(definition, node))
            {
                return false;
            }

            return AreMasteryPrerequisitesMet(definition, node) &&
                   CanAffordMasteryNode(node);
        }

        public static bool TryUnlockMasteryNode(
            PartyMemberDefinition definition,
            CharacterMasteryNodeDefinition node)
        {
            if (!CanUnlockMasteryNode(definition, node))
                return false;

            PersistentMemberProgress progress = GetMemberProgress(definition);

            if (!progress.TryUnlockMasteryNode(node))
                return false;

            SpendCosts(node.Costs);
            Changed?.Invoke();

            return true;
        }

        public static void ApplyMasteryToBuild(PartyMemberDefinition definition, RunBuild build)
        {
            if (definition == null || build == null)
                return;

            PersistentMemberProgress progress = GetMemberProgress(definition);

            foreach (CharacterMasteryNodeDefinition node in definition.MasteryNodes)
            {
                if (node == null || !node.IsConfigured || !progress.HasMasteryNode(node))
                    continue;

                node.ApplyPreparedBuild(build);
            }
        }

        public static float GetMasteryStatBonus(
            PartyMemberDefinition definition,
            CharacterStatModifierType type)
        {
            if (definition == null)
                return 0f;

            PersistentMemberProgress progress = GetMemberProgress(definition);
            float total = 0f;

            foreach (CharacterMasteryNodeDefinition node in definition.MasteryNodes)
            {
                if (node is not CharacterMasteryStatNodeDefinition statNode ||
                    !progress.HasMasteryNode(node) ||
                    statNode.ModifierType != type)
                {
                    continue;
                }

                total += statNode.Amount;
            }

            return total;
        }

        // Temporary compatibility seam for older callers during N4.
        public static void AddCurrency(int amount)
        {
            AddResource(PersistentResourceTier.Level1, amount);
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

            if (cost == int.MaxValue || Level1Resources < cost)
                return false;

            Level1Resources -= cost;
            GetMemberProgress(definition).Increase(slot);

            Changed?.Invoke();
            return true;
        }

        public static float GetDamageBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            float legacyGearBonus =
                GetMemberProgress(definition).WeaponLevel *
                definition.DamagePerWeaponLevel;

            return legacyGearBonus +
                   GetMasteryStatBonus(
                       definition,
                       CharacterStatModifierType.FlatBasicAttackDamage
                   );
        }

        public static float GetArmourBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            float legacyGearBonus =
                GetMemberProgress(definition).ArmourLevel *
                definition.ArmourPerArmourLevel;

            return legacyGearBonus +
                   GetMasteryStatBonus(
                       definition,
                       CharacterStatModifierType.FlatArmour
                   );
        }

        public static float GetHealthBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            float legacyGearBonus =
                GetMemberProgress(definition).ArmourLevel *
                definition.HealthPerArmourLevel;

            return legacyGearBonus +
                   GetMasteryStatBonus(
                       definition,
                       CharacterStatModifierType.FlatMaxHealth
                   );
        }

        public static float GetHealingBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetMemberProgress(definition).FocusLevel *
                   definition.HealingPerFocusLevel;
        }

        private static bool ContainsMasteryNode(
            PartyMemberDefinition definition,
            CharacterMasteryNodeDefinition node)
        {
            foreach (CharacterMasteryNodeDefinition candidate in definition.MasteryNodes)
            {
                if (candidate == node)
                    return true;
            }

            return false;
        }

        private static bool CanAffordCosts(IReadOnlyList<PersistentResourceCost> costs)
        {
            return
                GetResource(PersistentResourceTier.Level1) >=
                GetTotalCost(costs, PersistentResourceTier.Level1) &&

                GetResource(PersistentResourceTier.Level2) >=
                GetTotalCost(costs, PersistentResourceTier.Level2) &&

                GetResource(PersistentResourceTier.Level3) >=
                GetTotalCost(costs, PersistentResourceTier.Level3);
        }

        private static int GetTotalCost(
            IReadOnlyList<PersistentResourceCost> costs,
            PersistentResourceTier tier)
        {
            if (costs == null)
                return 0;

            int total = 0;

            foreach (PersistentResourceCost cost in costs)
            {
                if (cost != null && cost.IsConfigured && cost.Tier == tier)
                    total += cost.Amount;
            }

            return total;
        }

        private static void SpendCosts(IReadOnlyList<PersistentResourceCost> costs)
        {
            SetResource(
                PersistentResourceTier.Level1,
                Level1Resources - GetTotalCost(costs, PersistentResourceTier.Level1)
            );

            SetResource(
                PersistentResourceTier.Level2,
                Level2Resources - GetTotalCost(costs, PersistentResourceTier.Level2)
            );

            SetResource(
                PersistentResourceTier.Level3,
                Level3Resources - GetTotalCost(costs, PersistentResourceTier.Level3)
            );
        }

        private static void SetResource(PersistentResourceTier tier, int amount)
        {
            amount = Mathf.Max(0, amount);

            switch (tier)
            {
                case PersistentResourceTier.Level1:
                    Level1Resources = amount;
                    break;

                case PersistentResourceTier.Level2:
                    Level2Resources = amount;
                    break;

                case PersistentResourceTier.Level3:
                    Level3Resources = amount;
                    break;
            }
        }
    }
}