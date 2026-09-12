using System.Collections.Generic;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    public class PendingRewardsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RunDirector runDirector;
        [SerializeField] private Button pendingRewardsButton;
        [SerializeField] private TMP_Text pendingRewardsButtonText;

        [Header("Reward Overlay")]
        [SerializeField] private GameObject rewardOverlay;
        [SerializeField] private Transform offerRoot;
        [SerializeField] private RunUpgradeOfferCardUI offerCardPrefab;
        [SerializeField] private Button closeButton;

        private readonly List<RunUpgradeOfferCardUI> offerCards = new();

        private bool pausedByPendingRewards;

        private bool IsInCombat =>
            runDirector != null &&
            runDirector.State == RunDirector.RunFlowState.InLevel;

        private void Start()
        {
            if (runDirector == null)
                runDirector = RunDirector.Instance;

            if (pendingRewardsButton != null)
                pendingRewardsButton.onClick.AddListener(OpenPendingRewards);

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePendingRewards);

            RunProgress.Changed += Refresh;

            if (runDirector != null)
                runDirector.StateChanged += HandleRunStateChanged;

            rewardOverlay?.SetActive(false);

            Refresh();
        }

        private void OnDestroy()
        {
            RunProgress.Changed -= Refresh;

            if (runDirector != null)
                runDirector.StateChanged -= HandleRunStateChanged;

            if (pendingRewardsButton != null)
                pendingRewardsButton.onClick.RemoveListener(OpenPendingRewards);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(ClosePendingRewards);

            ResumeIfNeeded();
        }

        public void OpenPendingRewards()
        {
            if (!IsInCombat || !RunProgress.HasPendingRewardChoice)
                return;

            if (rewardOverlay == null)
                return;

            pausedByPendingRewards =
                SimulationPause.Instance != null &&
                !SimulationPause.IsPaused;

            if (pausedByPendingRewards)
                SimulationPause.Instance.Pause();

            rewardOverlay.SetActive(true);
            RebuildOfferCards();
        }

        public void ClosePendingRewards()
        {
            if (rewardOverlay != null)
                rewardOverlay.SetActive(false);

            ClearOfferCards();
            ResumeIfNeeded();
        }

        private void HandleRunStateChanged(RunDirector.RunFlowState newState)
        {
            Refresh();
        }

        private void HandleOfferChosen(RunUpgradeOffer offer)
        {
            if (runDirector == null || !runDirector.TryChoosePendingReward(offer))
            {
                Debug.LogWarning("Could not resolve pending reward.");
                return;
            }

            if (!RunProgress.HasPendingRewardChoice)
                ClosePendingRewards();
        }

        private void Refresh()
        {
            int pendingCount = RunProgress.PendingRewardChoiceCount;
            bool showButton = IsInCombat && pendingCount > 0;

            if (pendingRewardsButton != null)
                pendingRewardsButton.gameObject.SetActive(showButton);

            if (pendingRewardsButtonText != null)
                pendingRewardsButtonText.text = $"PENDING REWARDS ({pendingCount})";

            if (!IsInCombat || pendingCount <= 0)
            {
                if (rewardOverlay != null && rewardOverlay.activeSelf)
                    ClosePendingRewards();

                return;
            }

            if (rewardOverlay != null && rewardOverlay.activeSelf)
                RebuildOfferCards();
        }

        private void RebuildOfferCards()
        {
            ClearOfferCards();

            if (offerRoot == null || offerCardPrefab == null)
                return;

            foreach (RunUpgradeOffer offer in RunProgress.CurrentPendingRewardOffers)
            {
                RunUpgradeOfferCardUI card = Instantiate(offerCardPrefab, offerRoot);
                card.Bind(offer, HandleOfferChosen);
                offerCards.Add(card);
            }
        }

        private void ClearOfferCards()
        {
            foreach (RunUpgradeOfferCardUI card in offerCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            offerCards.Clear();
        }

        private void ResumeIfNeeded()
        {
            if (pausedByPendingRewards && SimulationPause.Instance != null)
                SimulationPause.Instance.Resume();

            pausedByPendingRewards = false;
        }
    }
}