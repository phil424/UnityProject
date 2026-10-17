using System;
using System.Text;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class AbilityLoadoutUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private RunDirector runDirector;

        [SerializeField]
        private Button openButton;

        [Header("Overlay")]
        [SerializeField]
        private GameObject overlayRoot;

        [SerializeField]
        private Transform slotRoot;

        [SerializeField]
        private TMP_Text ownedAbilitiesText;

        [SerializeField]
        private Button closeButton;

        private readonly StringBuilder ownedAbilitiesBuilder = new();

        private AbilityLoadoutSlotUI[] slotViews = Array.Empty<AbilityLoadoutSlotUI>();
        private bool pausedByLoadout;

        private bool IsInCombat => runDirector != null && runDirector.State == RunDirector.RunFlowState.InLevel;

        private PartyMemberDefinition CurrentMember => RunProgress.SelectedParty.Count > 0 ? RunProgress.SelectedParty[0] : null;

        private bool CanOpen => IsInCombat && RunProgress.HasActiveRun && !RunProgress.HasPendingRewardChoice && CurrentMember != null;

        private void Start()
        {
            if (runDirector == null)
                runDirector = RunDirector.Instance;

            DiscoverSlotViews();

            if (openButton != null)
                openButton.onClick.AddListener(OpenLoadout);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseLoadout);

            RunProgress.Changed += Refresh;

            if (runDirector != null)
                runDirector.StateChanged += HandleRunStateChanged;

            overlayRoot?.SetActive(false);

            Refresh();
        }

        private void OnDisable()
        {
            if (overlayRoot != null && overlayRoot.activeSelf)
                overlayRoot.SetActive(false);

            ResumeIfNeeded();
        }

        private void OnDestroy()
        {
            RunProgress.Changed -= Refresh;

            if (runDirector != null)
                runDirector.StateChanged -= HandleRunStateChanged;

            if (openButton != null)
                openButton.onClick.RemoveListener(OpenLoadout);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(CloseLoadout);

            ResumeIfNeeded();
        }

        public void OpenLoadout()
        {
            if (!CanOpen || overlayRoot == null)
                return;

            pausedByLoadout = SimulationPause.Instance != null && !SimulationPause.IsPaused;

            if (pausedByLoadout)
                SimulationPause.Instance.Pause();

            overlayRoot.SetActive(true);

            RefreshOverlay();
        }

        public void CloseLoadout()
        {
            if (overlayRoot != null)
                overlayRoot.SetActive(false);

            ResumeIfNeeded();
        }

        private void HandleRunStateChanged(RunDirector.RunFlowState newState)
        {
            Refresh();
        }

        private void HandleSlotSelection(int slotIndex, AbilityDefinition ability)
        {
            PartyMemberDefinition member = CurrentMember;

            if (member == null || runDirector == null || !runDirector.TrySetExpeditionAbilitySlot(member, slotIndex, ability))
            {
                Debug.LogWarning("Could not change the Expedition Ability slot.");
                RefreshOverlay();
                return;
            }

            RefreshOverlay();
        }

        private void Refresh()
        {
            if (openButton != null)
                openButton.interactable = CanOpen;

            if (overlayRoot == null || !overlayRoot.activeSelf)
                return;

            if (!CanOpen)
            {
                CloseLoadout();
                return;
            }

            RefreshOverlay();
        }

        private void RefreshOverlay()
        {
            PartyMemberDefinition member = CurrentMember;

            if (member == null)
                return;

            RunBuild build = RunProgress.GetBuild(member);

            foreach (AbilityLoadoutSlotUI slotView in slotViews)
                slotView?.Bind(build, HandleSlotSelection);

            RefreshOwnedAbilities(build);
        }

        private void RefreshOwnedAbilities(RunBuild build)
        {
            if (ownedAbilitiesText == null)
                return;

            ownedAbilitiesBuilder.Clear();

            if (build != null)
            {
                foreach (RunAbilityState state in build.Abilities)
                {
                    if (state?.Definition == null || state.IsPreparedCore)
                    {
                        continue;
                    }

                    if (ownedAbilitiesBuilder.Length > 0)
                        ownedAbilitiesBuilder.Append('\n');

                    string status = build.IsAbilityActive(state.Definition) ? "SLOTTED" : "UNSLOTTED";

                    ownedAbilitiesBuilder.Append(state.Definition.DisplayName);
                    ownedAbilitiesBuilder.Append($" Lv.{state.Level}");
                    ownedAbilitiesBuilder.Append($"   •   {status}");
                }
            }

            ownedAbilitiesText.text = ownedAbilitiesBuilder.Length > 0 ? ownedAbilitiesBuilder.ToString() : "No Expedition Abilities owned yet.";
        }

        private void DiscoverSlotViews()
        {
            if (slotRoot == null)
            {
                slotViews = Array.Empty<AbilityLoadoutSlotUI>();
                return;
            }

            slotViews = slotRoot.GetComponentsInChildren<AbilityLoadoutSlotUI>(true);

            Array.Sort(slotViews, (left, right) => left.SlotIndex.CompareTo(right.SlotIndex));

            if (slotViews.Length != RunBuild.MaxExpeditionAbilitySlots)
            {
                Debug.LogError($"Ability Loadout requires exactly " + $"{RunBuild.MaxExpeditionAbilitySlots} " + $"Expedition slot Views, but found " + $"{slotViews.Length}.", this);
            }
        }

        private void ResumeIfNeeded()
        {
            if (pausedByLoadout && SimulationPause.Instance != null)
            {
                SimulationPause.Instance.Resume();
            }

            pausedByLoadout = false;
        }
    }
}
