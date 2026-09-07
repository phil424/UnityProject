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
        private enum CardMode
        {
            Selection,
            RunUpgrade
        }

        private CardMode mode;

        public void BindSelection(PartyMemberDefinition member, RunSetup runSetup)
        {
            definition = member;
            setup = runSetup;
            mode = CardMode.Selection;

            selectionControls.SetActive(true);
            upgradeControls.SetActive(true);

            if (abilityUpgradeSection != null)
                abilityUpgradeSection.SetActive(false);

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => setup?.TogglePartyMember(definition));

            weaponButton.onClick.RemoveAllListeners();
            armourButton.onClick.RemoveAllListeners();
            focusButton.onClick.RemoveAllListeners();

            weaponButton.onClick.AddListener(
                () => PersistentProgression.TryBuyUpgrade(definition, GearSlot.Weapon)
            );

            armourButton.onClick.AddListener(
                () => PersistentProgression.TryBuyUpgrade(definition, GearSlot.Armour)
            );

            focusButton.onClick.AddListener(
                () => PersistentProgression.TryBuyUpgrade(definition, GearSlot.Focus)
            );

            Refresh();
        }

        public void BindUpgrade(PartyMemberDefinition member)
        {
            definition = member;
            setup = null;
            mode = CardMode.RunUpgrade;

            selectionControls.SetActive(false);

            // Equipment is persistent preparation now, not an expedition purchase.
            upgradeControls.SetActive(false);

            RebuildAbilityUpgradeEntries();
            Refresh();
        }

        public void Refresh()
        {
            if (definition == null)
                return;

            if (portraitImage != null)
            {
                portraitImage.sprite = definition.Portrait;
                portraitImage.enabled = definition.Portrait != null;
            }

            nameText.text = definition.DisplayName;
            roleText.text = definition.Role;
            descriptionText.text = definition.Description;

            if (mode == CardMode.Selection)
            {
                PersistentMemberProgress progress = PersistentProgression.GetMemberProgress(definition);

                float health = definition.BaseHealth + PersistentProgression.GetHealthBonus(definition);
                float damage = definition.BaseDamage + PersistentProgression.GetDamageBonus(definition);
                float armour = definition.BaseArmour + PersistentProgression.GetArmourBonus(definition);
                float healing = definition.BaseHealing + PersistentProgression.GetHealingBonus(definition);

                statsText.text =
                    healing > 0f
                        ? $"HP {health:0}   DMG {damage:0.#}   ARM {armour:0.#}   HEAL {healing:0.#}"
                        : $"HP {health:0}   DMG {damage:0.#}   ARM {armour:0.#}";

                gearText.text =
                    $"{definition.WeaponName} Lv.{progress.WeaponLevel}  |  " +
                    $"{definition.ArmourName} Lv.{progress.ArmourLevel}" +
                    (definition.BaseHealing > 0f
                        ? $"  |  {definition.FocusName} Lv.{progress.FocusLevel}"
                        : string.Empty);

                RefreshSelectionControls();

                RefreshPersistentUpgradeButton(
                    weaponButton,
                    weaponButtonText,
                    "Weapon",
                    GearSlot.Weapon
                );

                RefreshPersistentUpgradeButton(
                    armourButton,
                    armourButtonText,
                    "Armour",
                    GearSlot.Armour
                );

                bool hasHealing = definition.BaseHealing > 0f;
                focusButton.gameObject.SetActive(hasHealing);

                if (hasHealing)
                {
                    RefreshPersistentUpgradeButton(
                        focusButton,
                        focusButtonText,
                        "Focus",
                        GearSlot.Focus
                    );
                }

                return;
            }

            RunBuild build = RunProgress.GetBuild(definition);

            float runHealth = definition.BaseHealth + RunProgress.GetHealthBonus(definition);
            float runDamage = definition.BaseDamage + RunProgress.GetDamageBonus(definition);
            float runArmour = definition.BaseArmour + RunProgress.GetArmourBonus(definition);
            float runHealing = definition.BaseHealing + RunProgress.GetHealingBonus(definition);

            statsText.text =
                runHealing > 0f
                    ? $"HP {runHealth:0}   DMG {runDamage:0.#}   ARM {runArmour:0.#}   HEAL {runHealing:0.#}"
                    : $"HP {runHealth:0}   DMG {runDamage:0.#}   ARM {runArmour:0.#}";

            gearText.text =
                $"{definition.WeaponName} Lv.{build.WeaponLevel}  |  " +
                $"{definition.ArmourName} Lv.{build.ArmourLevel}" +
                (definition.BaseHealing > 0f ? $"  |  {definition.FocusName} Lv.{build.FocusLevel}" : string.Empty);

            foreach (AbilityUpgradeEntryUI entry in abilityUpgradeEntries)
                entry?.Refresh();
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
                    setup.MaximumPartySize;
        }

        private void RefreshPersistentUpgradeButton(
            Button button,
            TMP_Text label,
            string slotName,
            GearSlot slot
        )
        {
            int cost = PersistentProgression.GetUpgradeCost(definition, slot);

            label.text = $"Upgrade {slotName} ({cost})";

            button.interactable = !RunProgress.HasActiveRun && PersistentProgression.Currency >= cost;
        }
    }
}