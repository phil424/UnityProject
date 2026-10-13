using System.Collections.Generic;
using System.Globalization;

namespace MiniCrawler.Progress
{
    public enum CharacterStatModifierType
    {
        FlatBasicAttackDamage,
        MoveSpeedPercent,
        BasicAttackSpeedPercent,
        FlatArmour,
        FlatMaxHealth,
        AbilityCooldownRecoveryPercent,
        KnockbackPowerPercent,
        ShieldCapacity,
    }

    internal sealed class CharacterStatModifierSet
    {
        private readonly Dictionary<CharacterStatModifierType, float> values = new();

        public float Get(CharacterStatModifierType type)
        {
            return values.TryGetValue(type, out float value) ? value : 0f;
        }

        public void Add(CharacterStatModifierType type, float amount)
        {
            if (amount <= 0f)
                return;

            values[type] = Get(type) + amount;
        }
    }

    public static class CharacterStatModifierPresentation
    {
        public static string FormatDescription(CharacterStatModifierType type, float amount)
        {
            string formattedAmount = amount.ToString("0.##", CultureInfo.InvariantCulture);

            return type switch
            {
                CharacterStatModifierType.FlatBasicAttackDamage => $"Basic attacks deal {formattedAmount} additional damage.",
                CharacterStatModifierType.MoveSpeedPercent => $"Move {formattedAmount}% faster.",
                CharacterStatModifierType.BasicAttackSpeedPercent => $"Basic attacks occur {formattedAmount}% faster.",
                CharacterStatModifierType.FlatArmour => $"Gain {formattedAmount} armour.",
                CharacterStatModifierType.FlatMaxHealth => $"Gain {formattedAmount} maximum health.",
                CharacterStatModifierType.AbilityCooldownRecoveryPercent => $"Recover ability cooldowns {formattedAmount}% faster.",
                CharacterStatModifierType.KnockbackPowerPercent => $"Increase outgoing knockback and pull distance by {formattedAmount}%.",
                CharacterStatModifierType.ShieldCapacity => $"Increase maximum Shields by {formattedAmount}.",
                _ => "Unknown stat modifier.",
            };
        }
    }
}
