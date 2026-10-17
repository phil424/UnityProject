using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.WeaponMods;

namespace MiniCrawler.Progress
{
    public enum GearSlot
    {
        Weapon,
        Armour,
        Focus,
    }

    [Serializable]
    public sealed class RunBuild
    {
        public const int MaxWeaponModStances = 4;
        public const int MaxExpeditionAbilitySlots = 3;

        private readonly List<RunAbilityState> abilities = new();
        private readonly RunAbilityState[] expeditionAbilitySlots = new RunAbilityState[MaxExpeditionAbilitySlots];
        private readonly List<RunWeaponModState> weaponMods = new();
        private readonly CharacterStatModifierSet preparedStatModifiers = new();
        private readonly CharacterStatModifierSet expeditionStatModifiers = new();

        public int WeaponLevel { get; private set; }
        public int ArmourLevel { get; private set; }
        public int FocusLevel { get; private set; }

        public float FlatBasicAttackDamageBonus => GetEffectiveStatBonus(CharacterStatModifierType.FlatBasicAttackDamage);
        public float MoveSpeedPercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.MoveSpeedPercent);
        public float BasicAttackSpeedPercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.BasicAttackSpeedPercent);
        public float FlatArmourBonus => GetEffectiveStatBonus(CharacterStatModifierType.FlatArmour);
        public float FlatMaxHealthBonus => GetEffectiveStatBonus(CharacterStatModifierType.FlatMaxHealth);
        public float AbilityCooldownRecoveryPercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.AbilityCooldownRecoveryPercent);
        public float AbilityDamagePercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.AbilityDamagePercent);
        public float AbilityAreaSizePercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.AbilityAreaSizePercent);
        public float KnockbackPowerPercentBonus => GetEffectiveStatBonus(CharacterStatModifierType.KnockbackPowerPercent);
        public float ShieldCapacityBonus => GetEffectiveStatBonus(CharacterStatModifierType.ShieldCapacity);

        public IReadOnlyList<RunAbilityState> Abilities => abilities;
        public IReadOnlyList<RunAbilityState> ExpeditionAbilitySlots => expeditionAbilitySlots;
        public IReadOnlyList<RunWeaponModState> WeaponMods => weaponMods;

        public int GetLevel(GearSlot slot)
        {
            return slot switch
            {
                GearSlot.Weapon => WeaponLevel,
                GearSlot.Armour => ArmourLevel,
                GearSlot.Focus => FocusLevel,
                _ => 0,
            };
        }

        public void InitializeGearLevels(int weaponLevel, int armourLevel, int focusLevel)
        {
            WeaponLevel = Math.Max(0, weaponLevel);
            ArmourLevel = Math.Max(0, armourLevel);
            FocusLevel = Math.Max(0, focusLevel);
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

            foreach (AbilityLoadoutEntry entry in startingAbilities)
            {
                if (entry?.Ability == null || entry.Role == CharacterAbilityRole.Unassigned || HasAbility(entry.Ability) || GetPreparedAbilityState(entry.Role) != null)
                {
                    continue;
                }

                abilities.Add(new RunAbilityState(entry.Ability, entry.Level, entry.Role));
            }
        }

        public bool TryAcquireAbility(AbilityDefinition ability, int startingLevel = 1)
        {
            if (ability == null || HasAbility(ability))
                return false;

            RunAbilityState state = new(ability, startingLevel);
            abilities.Add(state);

            AutoAssignExpeditionAbility(state);

            return true;
        }

        public bool TryIncreaseAbilityLevel(AbilityDefinition ability)
        {
            RunAbilityState state = GetAbilityState(ability);
            return state != null && state.TryIncreaseLevel();
        }

        public bool ConfigureAbilityLevel(AbilityDefinition ability, int level)
        {
            if (ability == null)
                return false;

            RunAbilityState state = GetAbilityState(ability);

            if (state == null)
            {
                if (!TryAcquireAbility(ability, level))
                    return false;

                state = GetAbilityState(ability);
            }

            state.ConfigureLevel(level);
            return true;
        }

        public bool CanAcquireAbilityEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution?.TargetAbility == null)
                return false;

            RunAbilityState state = GetAbilityState(evolution.TargetAbility);
            return state != null && state.CanAddEvolution(evolution);
        }

        public bool TryAcquireAbilityEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution?.TargetAbility == null)
                return false;

            RunAbilityState state = GetAbilityState(evolution.TargetAbility);
            return state != null && state.TryAddEvolution(evolution);
        }

        public bool HasAbilityEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution?.TargetAbility == null)
                return false;

            RunAbilityState state = GetAbilityState(evolution.TargetAbility);
            return state != null && state.HasEvolution(evolution);
        }

        public bool HasAbility(AbilityDefinition ability)
        {
            return GetAbilityState(ability) != null;
        }

        public RunAbilityState GetAbilityState(AbilityDefinition ability)
        {
            if (ability == null)
                return null;

            foreach (RunAbilityState state in abilities)
            {
                if (state?.Definition != null && state.Definition.Id == ability.Id)
                    return state;
            }

            return null;
        }

        public RunAbilityState GetPreparedAbilityState(CharacterAbilityRole role)
        {
            if (role == CharacterAbilityRole.Unassigned)
                return null;

            foreach (RunAbilityState state in abilities)
            {
                if (state != null && state.PreparedRole == role)
                    return state;
            }

            return null;
        }

        public RunAbilityState GetExpeditionAbilityState(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxExpeditionAbilitySlots)
                return null;

            return expeditionAbilitySlots[slotIndex];
        }

        public bool IsAbilityActive(AbilityDefinition ability)
        {
            RunAbilityState state = GetAbilityState(ability);

            if (state == null)
                return false;

            if (state.IsPreparedCore)
                return true;

            return FindExpeditionAbilitySlot(state) >= 0;
        }

        public bool TrySetExpeditionAbilitySlot(int slotIndex, AbilityDefinition ability)
        {
            if (slotIndex < 0 || slotIndex >= MaxExpeditionAbilitySlots)
                return false;

            RunAbilityState currentState = expeditionAbilitySlots[slotIndex];

            if (ability == null)
            {
                if (currentState == null)
                    return false;

                expeditionAbilitySlots[slotIndex] = null;
                return true;
            }

            RunAbilityState requestedState = GetAbilityState(ability);

            if (requestedState == null || requestedState.IsPreparedCore)
                return false;

            int existingSlotIndex = FindExpeditionAbilitySlot(requestedState);

            if (existingSlotIndex == slotIndex)
                return false;

            if (existingSlotIndex >= 0)
                expeditionAbilitySlots[existingSlotIndex] = currentState;

            expeditionAbilitySlots[slotIndex] = requestedState;

            return true;
        }

        private void AutoAssignExpeditionAbility(RunAbilityState state)
        {
            if (state == null || state.IsPreparedCore || FindExpeditionAbilitySlot(state) >= 0)
                return;

            for (int i = 0; i < expeditionAbilitySlots.Length; i++)
            {
                if (expeditionAbilitySlots[i] != null)
                    continue;

                expeditionAbilitySlots[i] = state;
                return;
            }
        }

        private int FindExpeditionAbilitySlot(RunAbilityState state)
        {
            if (state == null)
                return -1;

            for (int i = 0; i < expeditionAbilitySlots.Length; i++)
            {
                if (expeditionAbilitySlots[i] == state)
                    return i;
            }

            return -1;
        }

        public RunWeaponModState CoreWeaponModState => GetWeaponModState(WeaponModStanceSlot.Core);

        public bool TryPrepareWeaponMod(WeaponModDefinition mod, WeaponModStanceSlot stanceSlot, int startingPreparedLevel = 0)
        {
            int slotIndex = (int)stanceSlot;

            if (mod == null || !mod.IsConfigured || HasWeaponMod(mod) || slotIndex < 0 || slotIndex >= MaxWeaponModStances || GetWeaponModState(stanceSlot) != null)
            {
                return false;
            }

            weaponMods.Add(new RunWeaponModState(mod, stanceSlot, startingPreparedLevel));
            weaponMods.Sort((a, b) => ((int)a.StanceSlot).CompareTo((int)b.StanceSlot));

            return true;
        }

        public bool HasWeaponMod(WeaponModDefinition mod)
        {
            return GetWeaponModState(mod) != null;
        }

        public RunWeaponModState GetWeaponModState(WeaponModDefinition mod)
        {
            if (mod == null)
                return null;

            foreach (RunWeaponModState state in weaponMods)
            {
                if (state?.Definition != null && state.Definition.Id == mod.Id)
                    return state;
            }

            return null;
        }

        public RunWeaponModState GetWeaponModState(WeaponModStanceSlot stanceSlot)
        {
            foreach (RunWeaponModState state in weaponMods)
            {
                if (state != null && state.StanceSlot == stanceSlot)
                    return state;
            }

            return null;
        }

        public bool TryIncreasePreparedWeaponModLevel(WeaponModDefinition mod, int amount = 1)
        {
            RunWeaponModState state = GetWeaponModState(mod);
            return state != null && state.TryIncreasePreparedLevel(amount);
        }

        public bool TryIncreaseWeaponModLevel(WeaponModDefinition mod, int amount = 1)
        {
            RunWeaponModState state = GetWeaponModState(mod);
            return state != null && state.TryIncreaseExpeditionLevel(amount);
        }

        public bool TryCombineWeaponMods(WeaponModDefinition activeMod, WeaponModDefinition combinedMod)
        {
            if (activeMod == null || combinedMod == null || activeMod.Id == combinedMod.Id)
                return false;

            RunWeaponModState activeState = GetWeaponModState(activeMod);
            RunWeaponModState combinedState = GetWeaponModState(combinedMod);

            if (activeState == null || combinedState == null)
                return false;

            return activeState.TryCombineWith(combinedState.Definition);
        }

        public void ApplyPreparedStatModifier(CharacterStatModifierType type, float amount)
        {
            preparedStatModifiers.Add(type, amount);
        }

        public void ApplyExpeditionStatModifier(CharacterStatModifierType type, float amount)
        {
            expeditionStatModifiers.Add(type, amount);
        }

        private float GetEffectiveStatBonus(CharacterStatModifierType type)
        {
            return preparedStatModifiers.Get(type) + expeditionStatModifiers.Get(type);
        }
    }
}
