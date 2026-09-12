using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.UI
{
    public static class RunUpgradeRarityPresentation
    {
        public static string GetLabel(
            RunUpgradeRarity rarity
        )
        {
            switch (rarity)
            {
                case RunUpgradeRarity.Common:
                    return "COMMON";

                case RunUpgradeRarity.Uncommon:
                    return "UNCOMMON";

                case RunUpgradeRarity.Rare:
                    return "RARE";

                case RunUpgradeRarity.Epic:
                    return "EPIC";

                case RunUpgradeRarity.Legendary:
                    return "LEGENDARY";

                default:
                    return "COMMON";
            }
        }

        public static Color GetColor(
            RunUpgradeRarity rarity
        )
        {
            switch (rarity)
            {
                case RunUpgradeRarity.Common:
                    return new Color32(
                        100,
                        100,
                        100,
                        255
                    );

                case RunUpgradeRarity.Uncommon:
                    return new Color32(
                        35,
                        145,
                        70,
                        255
                    );

                case RunUpgradeRarity.Rare:
                    return new Color32(
                        45,
                        100,
                        210,
                        255
                    );

                case RunUpgradeRarity.Epic:
                    return new Color32(
                        135,
                        60,
                        190,
                        255
                    );

                case RunUpgradeRarity.Legendary:
                    return new Color32(
                        215,
                        115,
                        20,
                        255
                    );

                default:
                    return new Color32(
                        100,
                        100,
                        100,
                        255
                    );
            }
        }
    }
}