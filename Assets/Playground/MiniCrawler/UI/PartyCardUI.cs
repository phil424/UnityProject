using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    public class PartyCardUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private TMP_Text nameText;

        [SerializeField]
        private TMP_Text roleText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text statsText;

        [SerializeField]
        private TMP_Text gearText;

        [Header("Selection Controls")]
        [SerializeField]
        private GameObject selectionControls;

        [SerializeField]
        private Button selectButton;

        [SerializeField]
        private TMP_Text selectButtonText;

        [Header("Upgrade Controls")]
        [SerializeField]
        private GameObject upgradeControls;

        [SerializeField]
        private Button weaponButton;

        [SerializeField]
        private TMP_Text weaponButtonText;

        [SerializeField]
        private Button armourButton;

        [SerializeField]
        private TMP_Text armourButtonText;

        [SerializeField]
        private Button focusButton;

        [SerializeField]
        private TMP_Text focusButtonText;

        [Header("Ability Upgrade Controls")]
        [SerializeField]
        private GameObject abilityUpgradeSection;

        [SerializeField]
        private Transform abilityUpgradeRoot;

        [SerializeField]
        private AbilityUpgradeEntryUI
            abilityUpgradeEntryPrefab;

        private readonly List<AbilityUpgradeEntryUI>
            abilityUpgradeEntries = new();

        private PartyMemberDefinition definition;
        private RunSetup setup;
        private bool upgradeMode;

        public void BindSelection(
            PartyMemberDefinition member,
            RunSetup runSetup
        )
        {
            definition =
                member;

            setup =
                runSetup;

            upgradeMode =
                false;

            selectionControls.SetActive(true);
            upgradeControls.SetActive(false);

            if (abilityUpgradeSection != null)
            {
                abilityUpgradeSection.SetActive(
                    false
                );
            }

            selectButton.onClick.RemoveAllListeners();

            selectButton.onClick.AddListener(
                () =>
                    setup?.TogglePartyMember(
                        definition
                    )
            );

            Refresh();
        }

        public void BindUpgrade(
            PartyMemberDefinition member
        )
        {
            definition =
                member;

            setup =
                null;

            upgradeMode =
                true;

            selectionControls.SetActive(false);
            upgradeControls.SetActive(true);

            weaponButton.onClick.RemoveAllListeners();
            armourButton.onClick.RemoveAllListeners();
            focusButton.onClick.RemoveAllListeners();

            weaponButton.onClick.AddListener(
                () =>
                    RunProgress.TryBuyUpgrade(
                        definition,
                        GearSlot.Weapon
                    )
            );

            armourButton.onClick.AddListener(
                () =>
                    RunProgress.TryBuyUpgrade(
                        definition,
                        GearSlot.Armour
                    )
            );

            focusButton.onClick.AddListener(
                () =>
                    RunProgress.TryBuyUpgrade(
                        definition,
                        GearSlot.Focus
                    )
            );

            RebuildAbilityUpgradeEntries();

            Refresh();
        }

        public void Refresh()
        {
            if (definition == null)
                return;

            RunBuild build =
                RunProgress.GetBuild(
                    definition
                );

            if (portraitImage != null)
            {
                portraitImage.sprite =
                    definition.Portrait;

                portraitImage.enabled =
                    definition.Portrait != null;
            }

            nameText.text =
                definition.DisplayName;

            roleText.text =
                definition.Role;

            descriptionText.text =
                definition.Description;

            float health =
                definition.BaseHealth +
                RunProgress.GetHealthBonus(
                    definition
                );

            float damage =
                definition.BaseDamage +
                RunProgress.GetDamageBonus(
                    definition
                );

            float armour =
                definition.BaseArmour +
                RunProgress.GetArmourBonus(
                    definition
                );

            float healing =
                definition.BaseHealing +
                RunProgress.GetHealingBonus(
                    definition
                );

            statsText.text =
                healing > 0f
                    ? $"HP {health:0}   " +
                      $"DMG {damage:0.#}   " +
                      $"ARM {armour:0.#}   " +
                      $"HEAL {healing:0.#}"
                    : $"HP {health:0}   " +
                      $"DMG {damage:0.#}   " +
                      $"ARM {armour:0.#}";

            gearText.text =
                $"{definition.WeaponName} " +
                $"Lv.{build.WeaponLevel}  |  " +
                $"{definition.ArmourName} " +
                $"Lv.{build.ArmourLevel}" +
                (
                    definition.BaseHealing > 0f
                        ? $"  |  " +
                          $"{definition.FocusName} " +
                          $"Lv.{build.FocusLevel}"
                        : string.Empty
                );

            if (!upgradeMode)
            {
                RefreshSelectionControls();
                return;
            }

            RefreshUpgradeButton(
                weaponButton,
                weaponButtonText,
                "Weapon",
                GearSlot.Weapon
            );

            RefreshUpgradeButton(
                armourButton,
                armourButtonText,
                "Armour",
                GearSlot.Armour
            );

            bool hasHealing =
                definition.BaseHealing > 0f;

            focusButton.gameObject.SetActive(
                hasHealing
            );

            if (hasHealing)
            {
                RefreshUpgradeButton(
                    focusButton,
                    focusButtonText,
                    "Focus",
                    GearSlot.Focus
                );
            }

            foreach (
                AbilityUpgradeEntryUI entry
                    in abilityUpgradeEntries
            )
            {
                entry?.Refresh();
            }
        }

        private void RebuildAbilityUpgradeEntries()
        {
            foreach (
                AbilityUpgradeEntryUI entry
                    in abilityUpgradeEntries
            )
            {
                if (entry != null)
                {
                    Destroy(
                        entry.gameObject
                    );
                }
            }

            abilityUpgradeEntries.Clear();

            if (
                abilityUpgradeRoot == null ||
                abilityUpgradeEntryPrefab == null
            )
            {
                if (abilityUpgradeSection != null)
                {
                    abilityUpgradeSection.SetActive(
                        false
                    );
                }

                return;
            }

            RunBuild build =
                RunProgress.GetBuild(
                    definition
                );

            foreach (
                RunAbilityState state
                    in build.Abilities
            )
            {
                if (
                    state == null ||
                    state.Definition == null
                )
                {
                    continue;
                }

                AbilityUpgradeEntryUI entry =
                    Instantiate(
                        abilityUpgradeEntryPrefab,
                        abilityUpgradeRoot
                    );

                entry.Bind(
                    definition,
                    state.Definition
                );

                abilityUpgradeEntries.Add(
                    entry
                );
            }

            if (abilityUpgradeSection != null)
            {
                abilityUpgradeSection.SetActive(
                    abilityUpgradeEntries.Count > 0
                );
            }
        }

        private void RefreshSelectionControls()
        {
            if (setup == null)
            {
                selectButton.interactable =
                    false;

                return;
            }

            bool selected =
                setup.IsSelected(
                    definition
                );

            selectButtonText.text =
                selected
                    ? "Remove"
                    : "Add to Party";

            selectButton.interactable =
                selected ||
                setup.SelectedParty.Count <
                    RunSetup.MaximumPartySize;
        }

        private void RefreshUpgradeButton(
            Button button,
            TMP_Text label,
            string slotName,
            GearSlot slot
        )
        {
            int cost =
                RunProgress.GetUpgradeCost(
                    definition,
                    slot
                );

            label.text =
                $"Upgrade {slotName} ({cost})";

            button.interactable =
                RunProgress.Currency >= cost;
        }
    }
}