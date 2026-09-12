using System.Globalization;

namespace MiniCrawler.Progress
{
    public static class RunUpgradeDescriptionFormatter
    {
        public static string Format(
            RunUpgradeEffectType effectType,
            float amount
        )
        {
            string formattedAmount =
                FormatAmount(amount);

            switch (effectType)
            {
                case RunUpgradeEffectType.FlatDamage:
                    return
                        $"Deal {formattedAmount} additional damage.";

                case RunUpgradeEffectType.MoveSpeedPercent:
                    return
                        $"Move {formattedAmount}% faster.";

                case RunUpgradeEffectType.AttackSpeedPercent:
                    return
                        $"Attack {formattedAmount}% faster.";

                case RunUpgradeEffectType.FlatArmour:
                    return
                        $"Gain {formattedAmount} armour.";

                default:
                    return
                        "Unknown upgrade effect.";
            }
        }

        private static string FormatAmount(
            float amount
        )
        {
            return amount.ToString(
                "0.##",
                CultureInfo.InvariantCulture
            );
        }
    }
}