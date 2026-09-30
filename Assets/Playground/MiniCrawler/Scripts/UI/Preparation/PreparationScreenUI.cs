using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Presentation;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class PreparationScreenUI : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private RunDirector runDirector;
        [SerializeField] private PreparationCombatShowcase combatShowcase;

        [Header("Views")]
        [SerializeField] private PreparationCharacterOverviewUI overviewUI;
        [SerializeField] private PreparationCharacterProgressionUI progressionUI;

        [Header("Carousel")]
        [SerializeField] private Transform characterButtonRoot;
        [SerializeField] private PreparationCharacterButtonUI characterButtonPrefab;

        [Header("Tabs")]
        [SerializeField] private GameObject overviewPanel;
        [SerializeField] private GameObject progressionPanel;
        [SerializeField] private Button overviewTabButton;
        [SerializeField] private Button progressionTabButton;

        [Header("Header / Footer")]
        [FormerlySerializedAs("persistentCurrencyText")]
        [SerializeField] private TMP_Text persistentResourcesText;
        [SerializeField] private Button startExpeditionButton;

        private readonly List<PreparationCharacterButtonUI> characterButtons = new();

        private PartyMemberDefinition selectedCharacter;
        private bool initialized;
        private bool subscribed;

        private void OnEnable()
        {
            EnsureInitialized();
            Subscribe();
            RebuildCharacterButtons();
            EnsureSelection();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
            combatShowcase?.Stop();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void EnsureInitialized()
        {
            if (initialized)
                return;

            if (runDirector == null)
                runDirector = RunDirector.Instance;

            if (overviewTabButton != null)
                overviewTabButton.onClick.AddListener(ShowOverview);

            if (progressionTabButton != null)
                progressionTabButton.onClick.AddListener(ShowProgression);

            if (startExpeditionButton != null)
                startExpeditionButton.onClick.AddListener(HandleStartExpedition);

            initialized = true;
            ShowOverview();
        }

        private void Subscribe()
        {
            if (subscribed)
                return;

            if (runDirector != null)
                runDirector.Setup.Changed += HandleSetupChanged;

            PersistentProgression.Changed += HandlePersistentProgressionChanged;

            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed)
                return;

            if (runDirector != null)
                runDirector.Setup.Changed -= HandleSetupChanged;

            PersistentProgression.Changed -= HandlePersistentProgressionChanged;

            subscribed = false;
        }

        private void RebuildCharacterButtons()
        {
            foreach (PreparationCharacterButtonUI button in characterButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }

            characterButtons.Clear();

            if (runDirector?.CharacterRoster == null ||
                characterButtonRoot == null ||
                characterButtonPrefab == null)
            {
                return;
            }

            foreach (PlayableCharacterRosterEntry entry in runDirector.CharacterRoster.Characters)
            {
                PartyMemberDefinition character = entry?.Character;

                if (character == null)
                    continue;

                bool unlocked = PersistentProgression.IsCharacterUnlocked(character);

                if (!unlocked && !entry.VisibleWhenLocked)
                    continue;

                PreparationCharacterButtonUI button = Instantiate(characterButtonPrefab, characterButtonRoot);
                button.Bind(entry, SelectCharacter);
                characterButtons.Add(button);
            }
        }

        private void EnsureSelection()
        {
            if (runDirector == null)
                return;

            if (runDirector.Setup.SelectedParty.Count > 0)
            {
                selectedCharacter = runDirector.Setup.SelectedParty[0];
                return;
            }

            PartyMemberDefinition firstCharacter = GetFirstUnlockedCharacter();

            if (firstCharacter != null && runDirector.Setup.SetSinglePartyMember(firstCharacter))
            {
                selectedCharacter = firstCharacter;
                return;
            }

            selectedCharacter = null;
        }

        private PartyMemberDefinition GetFirstUnlockedCharacter()
        {
            if (runDirector?.CharacterRoster == null)
                return null;

            foreach (PlayableCharacterRosterEntry entry in runDirector.CharacterRoster.Characters)
            {
                PartyMemberDefinition character = entry?.Character;

                if (character != null && runDirector.Setup.CanSelectPartyMember(character))
                    return character;
            }

            return null;
        }

        private void SelectCharacter(PartyMemberDefinition character)
        {
            if (runDirector == null || character == null)
                return;

            if (!runDirector.Setup.SetSinglePartyMember(character))
                return;

            selectedCharacter = character;
            Refresh();
        }

        private void HandleSetupChanged()
        {
            selectedCharacter =
                runDirector != null && runDirector.Setup.SelectedParty.Count > 0
                    ? runDirector.Setup.SelectedParty[0]
                    : null;

            Refresh();
        }

        private void HandlePersistentProgressionChanged()
        {
            RebuildCharacterButtons();
            EnsureSelection();
            Refresh();
        }

        private void Refresh()
        {
            selectedCharacter =
                runDirector != null && runDirector.Setup.SelectedParty.Count > 0
                    ? runDirector.Setup.SelectedParty[0]
                    : null;

            if (persistentResourcesText != null)
            {
                persistentResourcesText.text =
                    $"RESOURCES   " +
                    $"L1 {PersistentProgression.GetResource(PersistentResourceTier.Level1)}   " +
                    $"L2 {PersistentProgression.GetResource(PersistentResourceTier.Level2)}   " +
                    $"L3 {PersistentProgression.GetResource(PersistentResourceTier.Level3)}";
            }

            overviewUI?.Bind(selectedCharacter);
            progressionUI?.Bind(selectedCharacter);
            combatShowcase?.Show(selectedCharacter);

            foreach (PreparationCharacterButtonUI button in characterButtons)
            {
                if (button == null)
                    continue;

                button.SetState(button.Definition == selectedCharacter);
            }

            if (startExpeditionButton != null)
            {
                startExpeditionButton.interactable =
                    runDirector != null &&
                    !RunProgress.HasActiveRun &&
                    selectedCharacter != null &&
                    runDirector.Setup.CanSelectPartyMember(selectedCharacter);
            }
        }

        private void ShowOverview()
        {
            if (overviewPanel != null)
                overviewPanel.SetActive(true);

            if (progressionPanel != null)
                progressionPanel.SetActive(false);

            if (overviewTabButton != null)
                overviewTabButton.interactable = false;

            if (progressionTabButton != null)
                progressionTabButton.interactable = true;
        }

        private void ShowProgression()
        {
            if (overviewPanel != null)
                overviewPanel.SetActive(false);

            if (progressionPanel != null)
                progressionPanel.SetActive(true);

            if (overviewTabButton != null)
                overviewTabButton.interactable = true;

            if (progressionTabButton != null)
                progressionTabButton.interactable = false;
        }

        private void HandleStartExpedition()
        {
            runDirector?.BeginRun();
        }
    }
}