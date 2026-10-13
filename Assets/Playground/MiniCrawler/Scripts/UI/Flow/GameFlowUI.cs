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
        [Header("References")]
        [SerializeField]
        private StageDirector stageDirector;

        [SerializeField]
        private RunDirector runDirector;

        [SerializeField]
        private PartyCardUI partyCardPrefab;

        [Header("Panels")]
        [SerializeField]
        private GameObject partySelectionPanel;

        [SerializeField]
        private GameObject upgradePanel;

        [SerializeField]
        private GameObject combatHudPanel;

        [Header("Upgrade Screen")]
        [SerializeField]
        private Transform upgradeCardRoot;

        [SerializeField]
        private TMP_Text resultText;

        [SerializeField]
        private TMP_Text upgradeCurrencyText;

        [SerializeField]
        private Button continueButton;

        [SerializeField]
        private Button endRunButton;

        [SerializeField]
        private GameObject gearUpgradeSection;

        [Header("Run Upgrade Rewards")]
        [SerializeField]
        private GameObject runUpgradeRewardSection;

        [SerializeField]
        private Transform runUpgradeOfferRoot;

        [SerializeField]
        private RunUpgradeOfferCardUI runUpgradeOfferCardPrefab;

        [SerializeField]
        private TMP_Text runUpgradeRewardTitleText;

        [SerializeField]
        private Button runUpgradeRerollButton;

        [SerializeField]
        private TMP_Text runUpgradeRerollButtonText;

        [Header("Combat HUD")]
        [SerializeField]
        private TMP_Text combatCurrencyText;

        [SerializeField]
        private Button returnExpeditionButton;

        private readonly List<PartyCardUI> upgradeCards = new();
        private readonly List<RunUpgradeOfferCardUI> runUpgradeOfferCards = new();

        private void Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (runDirector == null)
                runDirector = RunDirector.Instance;

            RunProgress.Changed += RefreshAll;
            PersistentProgression.Changed += RefreshAll;

            if (stageDirector != null)
                stageDirector.StateChanged += HandleStageStateChanged;

            if (runDirector != null)
            {
                runDirector.StateChanged += HandleRunStateChanged;

                runDirector.Setup.Changed += RefreshAll;
            }

            RefreshAll();
        }

        public void RerollPendingReward()
        {
            if (runDirector == null || !runDirector.TryRerollPendingReward())
            {
                Debug.LogWarning("Could not reroll pending reward.");
            }
        }

        private void RefreshRunRewardReroll()
        {
            int remaining = runDirector != null ? runDirector.RewardRerollsRemaining : 0;

            if (runUpgradeRerollButtonText != null)
            {
                runUpgradeRerollButtonText.text = $"REROLL ({remaining})";
            }

            if (runUpgradeRerollButton != null)
            {
                runUpgradeRerollButton.interactable = runDirector != null && runDirector.CanRerollPendingReward;
            }
        }

        private void OnDestroy()
        {
            RunProgress.Changed -= RefreshAll;
            PersistentProgression.Changed -= RefreshAll;

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

        private void HandleStageStateChanged(StageDirector.LevelState newState)
        {
            RefreshAll();
        }

        private void HandleRunStateChanged(RunDirector.RunFlowState newState)
        {
            RefreshAll();
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

        private void RefreshRunRewardTitle()
        {
            if (runUpgradeRewardTitleText == null)
                return;

            PendingRewardChoice choice = RunProgress.CurrentPendingRewardChoice;

            runUpgradeRewardTitleText.text = choice != null ? RunRewardContextPresentation.GetTitle(choice.Context) : "Choose a Run Reward";
        }

        private void RefreshAll()
        {
            RebuildUpgradeCards();
            RebuildRunUpgradeOfferCards();
            RefreshRunRewardTitle();
            RefreshRunRewardReroll();

            upgradeCurrencyText.text = $"Expedition Currency: {RunProgress.Currency}";
            combatCurrencyText.text = $"Expedition Currency: {RunProgress.Currency}";

            if (runDirector == null)
            {
                continueButton.interactable = false;
                return;
            }

            RunDirector.RunFlowState runState = runDirector.State;

            bool betweenLevels = runState == RunDirector.RunFlowState.BetweenLevels;

            if (returnExpeditionButton != null)
            {
                bool canReturn = runState == RunDirector.RunFlowState.InLevel && RunProgress.HasActiveRun && !RunProgress.HasPendingRewardChoice;

                returnExpeditionButton.gameObject.SetActive(runState == RunDirector.RunFlowState.InLevel);
                returnExpeditionButton.interactable = canReturn;
            }

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

            bool showCombatView = runState == RunDirector.RunFlowState.InLevel || runState == RunDirector.RunFlowState.EndingExpedition;

            combatHudPanel.SetActive(showCombatView);

            if (runState == RunDirector.RunFlowState.BetweenLevels)
            {
                resultText.text = "Area Cleared - Expedition Continues";
            }
        }
    }
}
