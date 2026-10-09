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
        [SerializeField] private CharacterMasteryTreeUI masteryTreeUI;

        [Header("Carousel")]
        [SerializeField] private Transform characterButtonRoot;
        [SerializeField] private PreparationCharacterButtonUI characterButtonPrefab;

        [Header("Tabs")]
        [SerializeField] private GameObject overviewPanel;
        [SerializeField] private GameObject masteryTreeOverlay;
        [SerializeField] private Button overviewTabButton;
        [SerializeField] private Button progressionTabButton;
        [SerializeField] private Button masteryTreeCloseButton;

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
            TryActivate();
        }
        
        private void Start()
        {
            TryActivate();
        }

        private void OnDisable()
        {
            Unsubscribe();

            if (masteryTreeOverlay != null)
                masteryTreeOverlay.SetActive(false);

            combatShowcase?.Stop();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
        
        private void TryActivate()
        {
            if (runDirector == null)
                runDirector = RunDirector.Instance;

            if (runDirector?.Setup == null)
                return;

            Subscribe();
            RebuildCharacterButtons();
            EnsureSelection();
            Refresh();
        }

        private void EnsureInitialized()
        {
            if (initialized)
                return;

            if (overviewTabButton != null)
                overviewTabButton.onClick.AddListener(ShowOverview);

            if (progressionTabButton != null)
                progressionTabButton.onClick.AddListener(ShowProgression);
            
            if (masteryTreeCloseButton != null)
                masteryTreeCloseButton.onClick.AddListener(ShowOverview);

            if (startExpeditionButton != null)
                startExpeditionButton.onClick.AddListener(HandleStartExpedition);

            initialized = true;
            ShowOverview();
        }

        private void Subscribe()
        {
            if (subscribed || runDirector?.Setup == null)
                return;

            runDirector.Setup.Changed += HandleSetupChanged;
            PersistentProgression.Changed += HandlePersistentProgressionChanged;

            subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!subscribed)
                return;

            if (runDirector?.Setup != null)
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
            RunSetup setup = runDirector?.Setup;

            if (setup == null)
            {
                selectedCharacter = null;
                return;
            }

            if (setup.SelectedParty.Count > 0)
            {
                selectedCharacter = setup.SelectedParty[0];
                return;
            }

            PartyMemberDefinition firstCharacter = GetFirstUnlockedCharacter();

            if (firstCharacter != null && setup.SetSinglePartyMember(firstCharacter))
            {
                selectedCharacter = firstCharacter;
                return;
            }

            selectedCharacter = null;
        }

        private PartyMemberDefinition GetFirstUnlockedCharacter()
        {
            RunSetup setup = runDirector?.Setup;

            if (runDirector?.CharacterRoster == null || setup == null)
                return null;

            foreach (PlayableCharacterRosterEntry entry in runDirector.CharacterRoster.Characters)
            {
                PartyMemberDefinition character = entry?.Character;

                if (character != null && setup.CanSelectPartyMember(character))
                    return character;
            }

            return null;
        }

        private void SelectCharacter(PartyMemberDefinition character)
        {
            RunSetup setup = runDirector?.Setup;

            if (setup == null || character == null)
                return;

            if (!setup.SetSinglePartyMember(character))
                return;

            selectedCharacter = character;
            Refresh();
        }

        private void HandleSetupChanged()
        {
            RunSetup setup = runDirector?.Setup;

            selectedCharacter = setup != null && setup.SelectedParty.Count > 0 ? setup.SelectedParty[0] : null;

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
            RunSetup setup = runDirector?.Setup;

            selectedCharacter = setup != null && setup.SelectedParty.Count > 0 ? setup.SelectedParty[0] : null;

            if (persistentResourcesText != null)
            {
                persistentResourcesText.text =
                    $"RESOURCES   " +
                    $"L1 {PersistentProgression.GetResource(PersistentResourceTier.Level1)}   " +
                    $"L2 {PersistentProgression.GetResource(PersistentResourceTier.Level2)}   " +
                    $"L3 {PersistentProgression.GetResource(PersistentResourceTier.Level3)}";
            }

            overviewUI?.Bind(selectedCharacter);
            masteryTreeUI?.Bind(selectedCharacter);
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
                    setup != null &&
                    !RunProgress.HasActiveRun &&
                    selectedCharacter != null &&
                    setup.CanSelectPartyMember(selectedCharacter);
            }
        }

        private void ShowOverview()
        {
            if (overviewPanel != null)
                overviewPanel.SetActive(true);

            if (masteryTreeOverlay != null)
                masteryTreeOverlay.SetActive(false);

            if (overviewTabButton != null)
                overviewTabButton.interactable = false;

            if (progressionTabButton != null)
                progressionTabButton.interactable = true;
        }

        private void ShowProgression()
        {
            if (overviewPanel != null)
                overviewPanel.SetActive(true);

            if (masteryTreeOverlay != null)
                masteryTreeOverlay.SetActive(true);

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