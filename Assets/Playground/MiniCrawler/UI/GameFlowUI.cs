using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    public class GameFlowUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PartyMemberDefinition[] availablePartyMembers;

        [Header("References")]
        [SerializeField] private StageDirector stageDirector;
        [SerializeField] private RunDirector runDirector;
        [SerializeField] private PartyCardUI partyCardPrefab;

        [Header("Panels")]
        [SerializeField] private GameObject partySelectionPanel;
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private GameObject combatHudPanel;

        [Header("Party Selection")]
        [SerializeField] private Transform selectionCardRoot;
        [SerializeField] private TMP_Text selectedCountText;
        [SerializeField] private Button startRunButton;

        [Header("Upgrade Screen")]
        [SerializeField] private Transform upgradeCardRoot;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text upgradeCurrencyText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button endRunButton;
        [SerializeField] private GameObject gearUpgradeSection;

        [Header("Run Upgrade Rewards")]
        [SerializeField] private GameObject runUpgradeRewardSection;
        [SerializeField] private Transform runUpgradeOfferRoot;
        [SerializeField] private RunUpgradeOfferCardUI runUpgradeOfferCardPrefab;

        [Header("Combat HUD")]
        [SerializeField] private TMP_Text combatCurrencyText;

        private readonly List<PartyCardUI> selectionCards = new();
        private readonly List<PartyCardUI> upgradeCards = new();
        private readonly List<RunUpgradeOfferCardUI> runUpgradeOfferCards = new();

        private void Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (runDirector == null)
                runDirector = RunDirector.Instance;

            BuildSelectionCards();

            RunProgress.Changed += RefreshAll;

            if (stageDirector != null)
                stageDirector.StateChanged += HandleStageStateChanged;

            if (runDirector != null)
            {
                runDirector.StateChanged += HandleRunStateChanged;

                runDirector.Setup.Changed += RefreshAll;
            }

            RefreshAll();
        }

        private void OnDestroy()
        {
            RunProgress.Changed -= RefreshAll;

            if (stageDirector != null)
                stageDirector.StateChanged -= HandleStageStateChanged;

            if (runDirector != null)
            {
                runDirector.StateChanged -= HandleRunStateChanged;

                runDirector.Setup.Changed -= RefreshAll;
            }
        }

        public void StartRun()
        {
            runDirector?.BeginRun();
        }

        public void ContinueRun()
        {
            runDirector?.ContinueRun();
        }

        public void EndRun()
        {
            runDirector?.EndRun();
        }

        public void ClearPartySelection()
        {
            if (RunProgress.HasActiveRun)
                return;

            runDirector?.Setup.Clear();
        }

        private void HandleStageStateChanged(StageDirector.LevelState newState)
        {
            RefreshAll();
        }

        private void HandleRunStateChanged(RunDirector.RunFlowState newState)
        {
            RefreshAll();
        }

        private void BuildSelectionCards()
        {
            if (runDirector == null)
                return;

            foreach (PartyMemberDefinition member in availablePartyMembers)
            {
                if (member == null)
                    continue;

                PartyCardUI card = Instantiate(partyCardPrefab, selectionCardRoot);

                card.BindSelection(member, runDirector.Setup);

                selectionCards.Add(card);
            }
        }

        private void RebuildUpgradeCards()
        {
            foreach (PartyCardUI card in upgradeCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            upgradeCards.Clear();

            foreach (PartyMemberDefinition member in RunProgress.SelectedParty)
            {
                PartyCardUI card = Instantiate(partyCardPrefab, upgradeCardRoot);

                card.BindUpgrade(member);

                upgradeCards.Add(card);
            }
        }

        private void RebuildRunUpgradeOfferCards()
        {
            foreach (RunUpgradeOfferCardUI card in runUpgradeOfferCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            runUpgradeOfferCards.Clear();

            if (runUpgradeOfferCardPrefab == null || runUpgradeOfferRoot == null)
            {
                return;
            }

            foreach (RunUpgradeOffer offer in RunProgress.CurrentPendingRewardOffers)
            {
                RunUpgradeOfferCardUI card = Instantiate(runUpgradeOfferCardPrefab, runUpgradeOfferRoot);

                card.Bind(offer, HandleRunUpgradeOfferChosen);

                runUpgradeOfferCards.Add(card);
            }
        }

        private void HandleRunUpgradeOfferChosen(RunUpgradeOffer offer)
        {
            if (runDirector == null || !runDirector.TryChoosePendingReward(offer))
                Debug.LogWarning("Could not resolve pending reward.");
        }

        private void RefreshAll()
        {
            foreach (PartyCardUI card in selectionCards)
            {
                card.Refresh();
            }

            RebuildUpgradeCards();
            RebuildRunUpgradeOfferCards();

            int setupPartyCount = runDirector != null ? runDirector.Setup.SelectedParty.Count : 0;

            selectedCountText.text = $"Party: {setupPartyCount}/{runDirector.Setup.MaximumPartySize}";

            upgradeCurrencyText.text = $"Run Currency: {RunProgress.Currency}";

            combatCurrencyText.text = $"Run Currency: {RunProgress.Currency}";

            startRunButton.interactable = runDirector != null && !RunProgress.HasActiveRun && setupPartyCount > 0;

            if (runDirector == null)
            {
                continueButton.interactable = false;
                return;
            }

            RunDirector.RunFlowState runState = runDirector.State;

            bool betweenLevels = runState == RunDirector.RunFlowState.BetweenLevels;

            continueButton.gameObject.SetActive(betweenLevels);
            continueButton.interactable = runDirector.CanContinueRun;

            if (endRunButton != null)
            {
                endRunButton.gameObject.SetActive(betweenLevels);
                endRunButton.interactable = !RunProgress.HasPendingRewardChoice;
            }
            
            if (runUpgradeRewardSection != null)
            {
                runUpgradeRewardSection.SetActive(RunProgress.HasPendingRewardChoice);
            }

            if (gearUpgradeSection != null)
            {
                gearUpgradeSection.SetActive(!RunProgress.HasPendingRewardChoice);
            }

            partySelectionPanel.SetActive(runState == RunDirector.RunFlowState.PreRun);

            upgradePanel.SetActive(runState == RunDirector.RunFlowState.BetweenLevels);

            combatHudPanel.SetActive(runState == RunDirector.RunFlowState.InLevel);

            if (runState == RunDirector.RunFlowState.BetweenLevels)
            {
                resultText.text = runDirector.LastLevelWon
                        ? "Level Complete - Victory"
                        : "Level Ended - Party Defeated";
            }
        }
    }
}