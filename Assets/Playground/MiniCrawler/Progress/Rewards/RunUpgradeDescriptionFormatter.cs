using System.Globalization;

namespace MiniCrawler.Progress
{
    public static class RunUpgradeDescriptionFormatter
    {
        public static string Format(RunUpgradeEffectType effectType, float amount)
        {
            string formattedAmount = FormatAmount(amount);

            switch (effectType)
            {
                case RunUpgradeEffectType.FlatBasicAttackDamage:
                    return $"Basic attacks deal {formattedAmount} additional damage.";

                case RunUpgradeEffectType.MoveSpeedPercent:
                    return $"Move {formattedAmount}% faster.";

                case RunUpgradeEffectType.BasicAttackSpeedPercent:
                    return $"Basic attacks occur {formattedAmount}% faster.";

                case RunUpgradeEffectType.FlatArmour:
                    return $"Gain {formattedAmount} armour.";

                case RunUpgradeEffectType.FlatMaxHealth:
                    return $"Gain {formattedAmount} maximum health.";
                
                case RunUpgradeEffectType.AbilityCooldownRecoveryPercent:
                    return $"Recover ability cooldowns {formattedAmount}% faster.";

                case RunUpgradeEffectType.KnockbackPowerPercent:
                    return $"Increase outgoing knockback and pull distance by {formattedAmount}%.";

                default:
                    return "Unknown upgrade effect.";
            }
        }

        private static string FormatAmount(float amount)
        {
            return amount.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}