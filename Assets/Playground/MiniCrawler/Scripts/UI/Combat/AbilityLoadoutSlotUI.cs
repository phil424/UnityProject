using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class AbilityLoadoutSlotUI : MonoBehaviour
    {
        [SerializeField, Range(0, RunBuild.MaxExpeditionAbilitySlots - 1)]
        private int slotIndex;

        [SerializeField]
        private TMP_Text slotLabelText;

        [SerializeField]
        private TMP_Dropdown abilityDropdown;

        private readonly List<AbilityDefinition> optionDefinitions = new();

        private Action<int, AbilityDefinition> selectionRequested;
        private AbilityDefinition currentSelection;
        private bool suppressDropdownCallback;

        public int SlotIndex => slotIndex;

        private void Awake()
        {
            if (abilityDropdown != null)
                abilityDropdown.onValueChanged.AddListener(HandleDropdownChanged);
        }

        private void OnDestroy()
        {
            if (abilityDropdown != null)
                abilityDropdown.onValueChanged.RemoveListener(HandleDropdownChanged);
        }

        public void Bind(RunBuild build, Action<int, AbilityDefinition> onSelectionRequested)
        {
            selectionRequested = onSelectionRequested;

            if (slotLabelText != null)
                slotLabelText.text = $"EXPEDITION SLOT {slotIndex + 1}";

            RebuildOptions(build);
        }

        private void RebuildOptions(RunBuild build)
        {
            optionDefinitions.Clear();
            optionDefinitions.Add(null);

            List<TMP_Dropdown.OptionData> optionData = new() { new TMP_Dropdown.OptionData("EMPTY") };

            if (build != null)
            {
                foreach (RunAbilityState state in build.Abilities)
                {
                    if (state?.Definition == null || state.IsPreparedCore)
                    {
                        continue;
                    }

                    optionDefinitions.Add(state.Definition);

                    optionData.Add(new TMP_Dropdown.OptionData($"{state.Definition.DisplayName} Lv.{state.Level}"));
                }
            }

            RunAbilityState currentState = build != null ? build.GetExpeditionAbilityState(slotIndex) : null;

            currentSelection = currentState?.Definition;

            int selectedOption = 0;

            if (currentSelection != null)
            {
                for (int i = 1; i < optionDefinitions.Count; i++)
                {
                    if (optionDefinitions[i] != null && optionDefinitions[i].Id == currentSelection.Id)
                    {
                        selectedOption = i;
                        break;
                    }
                }
            }

            if (abilityDropdown == null)
                return;

            suppressDropdownCallback = true;

            abilityDropdown.ClearOptions();
            abilityDropdown.AddOptions(optionData);
            abilityDropdown.value = selectedOption;
            abilityDropdown.RefreshShownValue();
            abilityDropdown.interactable = optionDefinitions.Count > 1;

            suppressDropdownCallback = false;
        }

        private void HandleDropdownChanged(int optionIndex)
        {
            if (suppressDropdownCallback)
                return;

            if (optionIndex < 0 || optionIndex >= optionDefinitions.Count)
                return;

            AbilityDefinition requested = optionDefinitions[optionIndex];

            if (requested == currentSelection || (requested != null && currentSelection != null && requested.Id == currentSelection.Id))
            {
                return;
            }

            selectionRequested?.Invoke(slotIndex, requested);
        }
    }
}
