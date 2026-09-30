using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Tools
{
    public enum EncounterWorkshopPowerPreset
    {
        Early,
        Mid,
        Late,
        Extreme,
        Custom
    }

    [Serializable]
    public sealed class EncounterWorkshopBuildConfiguration
    {
        public sealed class AbilityLevelSetting
        {
            public AbilityDefinition Ability { get; }
            public int StartingLevel { get; }
            public int Level { get; private set; }

            public AbilityLevelSetting(AbilityDefinition ability, int startingLevel)
            {
                Ability = ability;
                StartingLevel = ability != null ? ability.ClampLevel(startingLevel) : 1;
                Level = StartingLevel;
            }

            public void SetLevel(int level)
            {
                if (Ability == null)
                    return;

                Level = Ability.ClampLevel(level);
            }
        }

        [SerializeField, Min(1)] private int maximumGearLevel = 20;
        [SerializeField] private EncounterWorkshopPowerPreset preset = EncounterWorkshopPowerPreset.Early;

        [SerializeField, Min(0)] private int weaponLevel;
        [SerializeField, Min(0)] private int armourLevel;
        [SerializeField, Min(0)] private int focusLevel;

        [NonSerialized] private PartyMemberDefinition configuredMember;
        [NonSerialized] private List<AbilityLevelSetting> abilityLevels = new();
        [NonSerialized] private bool initialized;

        public EncounterWorkshopPowerPreset Preset => preset;
        public int MaximumGearLevel => Mathf.Max(1, maximumGearLevel);

        public int WeaponLevel => weaponLevel;
        public int ArmourLevel => armourLevel;
        public int FocusLevel => focusLevel;

        public IReadOnlyList<AbilityLevelSetting> AbilityLevels
        {
            get
            {
                EnsureCollections();
                return abilityLevels;
            }
        }

        public void EnsureFor(PartyMemberDefinition member)
        {
            EnsureCollections();

            if (initialized && configuredMember == member)
                return;

            configuredMember = member;
            initialized = true;

            RebuildAbilityLevels();

            if (preset != EncounterWorkshopPowerPreset.Custom)
                ApplyPresetValues(preset);
            else
                ClampGearLevels();
        }

        public void SelectPreset(EncounterWorkshopPowerPreset newPreset, PartyMemberDefinition member)
        {
            EnsureFor(member);

            if (newPreset == EncounterWorkshopPowerPreset.Custom)
            {
                preset = newPreset;
                return;
            }

            preset = newPreset;
            ApplyPresetValues(newPreset);
        }

        public void SetWeaponLevel(int level)
        {
            weaponLevel = Mathf.Clamp(level, 0, MaximumGearLevel);
            preset = EncounterWorkshopPowerPreset.Custom;
        }

        public void SetArmourLevel(int level)
        {
            armourLevel = Mathf.Clamp(level, 0, MaximumGearLevel);
            preset = EncounterWorkshopPowerPreset.Custom;
        }

        public void SetFocusLevel(int level)
        {
            focusLevel = Mathf.Clamp(level, 0, MaximumGearLevel);
            preset = EncounterWorkshopPowerPreset.Custom;
        }

        public void SetAbilityLevel(int index, int level)
        {
            EnsureCollections();

            if (index < 0 || index >= abilityLevels.Count)
                return;

            abilityLevels[index].SetLevel(level);
            preset = EncounterWorkshopPowerPreset.Custom;
        }

        public void ApplyTo(PartyMemberDefinition member, RunBuild build)
        {
            if (member == null || build == null)
                return;

            EnsureFor(member);

            build.InitializeGearLevels(weaponLevel, armourLevel, focusLevel);

            foreach (AbilityLevelSetting setting in abilityLevels)
            {
                if (setting?.Ability != null)
                    build.ConfigureAbilityLevel(setting.Ability, setting.Level);
            }
        }

        private void RebuildAbilityLevels()
        {
            abilityLevels.Clear();

            if (configuredMember == null)
                return;

            foreach (AbilityLoadoutEntry entry in configuredMember.StartingAbilities)
            {
                if (entry?.Ability == null || ContainsAbility(entry.Ability))
                    continue;

                abilityLevels.Add(new AbilityLevelSetting(entry.Ability, entry.Level));
            }
        }

        private bool ContainsAbility(AbilityDefinition ability)
        {
            foreach (AbilityLevelSetting setting in abilityLevels)
            {
                if (setting?.Ability != null && setting.Ability.Id == ability.Id)
                    return true;
            }

            return false;
        }

        private void ApplyPresetValues(EncounterWorkshopPowerPreset powerPreset)
        {
            float gearFraction = powerPreset switch
            {
                EncounterWorkshopPowerPreset.Early => 0f,
                EncounterWorkshopPowerPreset.Mid => 0.25f,
                EncounterWorkshopPowerPreset.Late => 0.5f,
                EncounterWorkshopPowerPreset.Extreme => 1f,
                _ => 0f
            };

            int gearLevel = Mathf.RoundToInt(MaximumGearLevel * gearFraction);

            weaponLevel = gearLevel;
            armourLevel = gearLevel;
            focusLevel = gearLevel;

            foreach (AbilityLevelSetting setting in abilityLevels)
            {
                if (setting?.Ability == null)
                    continue;

                int targetLevel = powerPreset switch
                {
                    EncounterWorkshopPowerPreset.Early => setting.StartingLevel,
                    EncounterWorkshopPowerPreset.Mid =>
                        Mathf.Max(setting.StartingLevel, Mathf.CeilToInt(setting.Ability.MaxLevel * 0.4f)),
                    EncounterWorkshopPowerPreset.Late =>
                        Mathf.Max(setting.StartingLevel, Mathf.CeilToInt(setting.Ability.MaxLevel * 0.8f)),
                    EncounterWorkshopPowerPreset.Extreme => setting.Ability.MaxLevel,
                    _ => setting.StartingLevel
                };

                setting.SetLevel(targetLevel);
            }
        }

        private void ClampGearLevels()
        {
            weaponLevel = Mathf.Clamp(weaponLevel, 0, MaximumGearLevel);
            armourLevel = Mathf.Clamp(armourLevel, 0, MaximumGearLevel);
            focusLevel = Mathf.Clamp(focusLevel, 0, MaximumGearLevel);
        }

        private void EnsureCollections()
        {
            if (abilityLevels == null)
                abilityLevels = new List<AbilityLevelSetting>();
        }
    }
}