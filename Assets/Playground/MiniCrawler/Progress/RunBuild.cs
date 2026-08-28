using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;

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
        private readonly List<RunAbilityState>
            abilities = new();

        public int WeaponLevel { get; private set; }
        public int ArmourLevel { get; private set; }
        public int FocusLevel { get; private set; }

        public float FlatDamageBonus { get; private set; }
        public float MoveSpeedPercentBonus { get; private set; }
        public float AttackSpeedPercentBonus { get; private set; }
        public float FlatArmourBonus { get; private set; }

        public IReadOnlyList<RunAbilityState> Abilities =>
            abilities;

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

        public void InitializeStartingAbilities(IEnumerable<AbilityLoadoutEntry> startingAbilities)
        {
            if (startingAbilities == null)
                return;

            foreach (
                AbilityLoadoutEntry entry
                in startingAbilities
            )
            {
                if (
                    entry == null ||
                    entry.Ability == null
                )
                {
                    continue;
                }

                TryAcquireAbility(
                    entry.Ability,
                    entry.Level
                );
            }
        }

        public bool TryAcquireAbility(AbilityDefinition ability, int startingLevel = 1)
        {
            if (
                ability == null ||
                HasAbility(ability)
            )
            {
                return false;
            }

            abilities.Add(
                new RunAbilityState(
                    ability,
                    startingLevel
                )
            );

            return true;
        }
        
        public bool TryIncreaseAbilityLevel(AbilityDefinition ability)
        {
            RunAbilityState state = GetAbilityState(ability);

            return state != null && state.TryIncreaseLevel();
        }

        public bool HasAbility(
            AbilityDefinition ability
        )
        {
            return
                GetAbilityState(ability) != null;
        }

        public RunAbilityState GetAbilityState(
            AbilityDefinition ability
        )
        {
            if (ability == null)
                return null;

            foreach (
                RunAbilityState state
                in abilities
            )
            {
                if (
                    state?.Definition != null &&
                    state.Definition.Id ==
                        ability.Id
                )
                {
                    return state;
                }
            }

            return null;
        }

        public void ApplyRunUpgrade(
            RunUpgradeDefinition upgrade
        )
        {
            if (
                upgrade == null ||
                upgrade.Amount <= 0f
            )
            {
                return;
            }

            switch (upgrade.EffectType)
            {
                case RunUpgradeEffectType.FlatDamage:
                    FlatDamageBonus +=
                        upgrade.Amount;
                    break;

                case RunUpgradeEffectType.MoveSpeedPercent:
                    MoveSpeedPercentBonus +=
                        upgrade.Amount;
                    break;

                case RunUpgradeEffectType.AttackSpeedPercent:
                    AttackSpeedPercentBonus +=
                        upgrade.Amount;
                    break;

                case RunUpgradeEffectType.FlatArmour:
                    FlatArmourBonus +=
                        upgrade.Amount;
                    break;
            }
        }
    }
}