using System;

namespace MiniCrawler.Progress
{
    public enum GearSlot
    {
        Weapon,
        Armour,
        Focus
    }

    [Serializable]
    public sealed class RunBuild
    {
        public int WeaponLevel { get; private set; }
        public int ArmourLevel { get; private set; }
        public int FocusLevel { get; private set; }

        public float FlatDamageBonus { get; private set; }
        public float MoveSpeedPercentBonus { get; private set; }
        public float AttackSpeedPercentBonus { get; private set; }
        public float FlatArmourBonus { get; private set; }

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

        public void ApplyRunUpgrade(
            RunUpgradeDefinition upgrade
        )
        {
            if (upgrade == null ||
                upgrade.Amount <= 0f)
            {
                return;
            }

            switch (upgrade.EffectType)
            {
                case RunUpgradeEffectType.FlatDamage:
                    FlatDamageBonus += upgrade.Amount;
                    break;

                case RunUpgradeEffectType.MoveSpeedPercent:
                    MoveSpeedPercentBonus += upgrade.Amount;
                    break;

                case RunUpgradeEffectType.AttackSpeedPercent:
                    AttackSpeedPercentBonus += upgrade.Amount;
                    break;

                case RunUpgradeEffectType.FlatArmour:
                    FlatArmourBonus += upgrade.Amount;
                    break;
            }
        }
    }
}