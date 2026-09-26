using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public enum PersistentResourceTier
    {
        Level1,
        Level2,
        Level3
    }

    [Serializable]
    public sealed class PersistentResourceCost
    {
        [SerializeField] private PersistentResourceTier tier = PersistentResourceTier.Level1;
        [SerializeField, Min(0)] private int amount = 1;

        public PersistentResourceTier Tier => tier;
        public int Amount => Mathf.Max(0, amount);
        public bool IsConfigured => Amount > 0;
    }

    [Serializable]
    public sealed class PersistentResourceReward
    {
        [SerializeField] private PersistentResourceTier tier = PersistentResourceTier.Level1;
        [SerializeField, Min(0)] private int amount = 1;

        public PersistentResourceTier Tier => tier;
        public int Amount => Mathf.Max(0, amount);
        public bool IsConfigured => Amount > 0;
    }

    public static class PersistentResourcePresentation
    {
        public static string GetShortLabel(PersistentResourceTier tier)
        {
            return tier switch
            {
                PersistentResourceTier.Level1 => "L1",
                PersistentResourceTier.Level2 => "L2",
                PersistentResourceTier.Level3 => "L3",
                _ => "?"
            };
        }

        public static string GetDisplayName(PersistentResourceTier tier)
        {
            return tier switch
            {
                PersistentResourceTier.Level1 => "Level 1 Resource",
                PersistentResourceTier.Level2 => "Level 2 Resource",
                PersistentResourceTier.Level3 => "Level 3 Resource",
                _ => "Unknown Resource"
            };
        }

        public static string FormatCosts(IReadOnlyList<PersistentResourceCost> costs)
        {
            if (costs == null || costs.Count == 0)
                return "FREE";

            StringBuilder builder = new();

            foreach (PersistentResourceCost cost in costs)
            {
                if (cost == null || !cost.IsConfigured)
                    continue;

                if (builder.Length > 0)
                    builder.Append(" + ");

                builder.Append(cost.Amount);
                builder.Append(' ');
                builder.Append(GetShortLabel(cost.Tier));
            }

            return builder.Length > 0 ? builder.ToString() : "FREE";
        }
    }
}