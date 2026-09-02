using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniCrawler.Systems
{
    public class RunDirector : MonoBehaviour
    {
        public enum RunFlowState
        {
            PreRun,
            InLevel,
            BetweenLevels
        }

        public static RunDirector Instance { get; private set; }

        public event Action<RunFlowState> StateChanged;

        [SerializeField] private StageDirector stageDirector;

        [Header("Setup")]
        [SerializeField, Min(1)] private int maximumPartySize = 4;

        [Header("Run Rewards")]
        [FormerlySerializedAs("availableRunUpgrades")]
        [SerializeField]private RunRewardDefinition[]availableRunRewards;

        [SerializeField, Min(1)]
        private int runUpgradeOfferCount = 3;

        private RunSetup setup;

        private RunFlowState state =
            RunFlowState.PreRun;

        public RunFlowState State => state;

        public string StateName =>
            state.ToString();

        public RunSetup Setup =>
            setup;

        public bool LastLevelWon { get; private set; }

        public bool CanContinueRun =>
            RunProgress.HasActiveRun &&
            state == RunFlowState.BetweenLevels;

        private void Awake()
        {
            Instance = this;

            setup =
                new RunSetup(
                    maximumPartySize
                );
        }

        private void Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (stageDirector == null)
            {
                Debug.LogError(
                    "RunDirector requires a StageDirector."
                );

                return;
            }

            stageDirector.LevelFinished +=
                HandleLevelFinished;

            stageDirector.CurrencyEarned +=
                HandleCurrencyEarned;

            stageDirector.ClearLevel();

            SetState(RunFlowState.PreRun);
        }

        private void OnDestroy()
        {
            if (stageDirector != null)
            {
                stageDirector.LevelFinished -=
                    HandleLevelFinished;

                stageDirector.CurrencyEarned -=
                    HandleCurrencyEarned;
            }

            if (Instance == this)
                Instance = null;
        }

        public bool BeginRun()
        {
            if (RunProgress.HasActiveRun)
            {
                Debug.LogWarning(
                    "Cannot begin a new run while another run is active."
                );

                return false;
            }

            RunStartConfiguration configuration =
                setup.CreateConfiguration();

            if (!RunProgress.BeginRun(configuration))
            {
                Debug.LogWarning(
                    "Cannot begin a run without a valid setup."
                );

                return false;
            }

            if (StartCurrentLevel(
                    RunFlowState.PreRun
                ))
            {
                return true;
            }

            RunProgress.EndRun();

            SetState(RunFlowState.PreRun);

            return false;
        }

        public bool ContinueRun()
        {
            if (!CanContinueRun)
            {
                Debug.LogWarning("The run cannot continue from the current state.");
                return false;
            }

            return StartCurrentLevel(RunFlowState.BetweenLevels);
        }
        
        public bool TryChoosePendingReward(RunUpgradeOffer offer)
        {
            if (!RunProgress.TryChoosePendingReward(offer))
                return false;

            RefreshLivePartyMember(offer.Member);
            return true;
        }

        private void RefreshLivePartyMember(PartyMemberDefinition member)
        {
            if (member == null || stageDirector == null || RunProgress.CurrentRun == null)
                return;

            RunBuild build = RunProgress.CurrentRun.GetBuild(member);
            stageDirector.RefreshPartyMemberRuntime(member, build);
        }

        public void EndRun()
        {
            if (RunProgress.HasPendingRewardChoice)
            {
                Debug.LogWarning("Resolve all earned pending rewards before ending the run.");
                return;
            }

            if (stageDirector != null)
                stageDirector.ClearLevel();

            RunProgress.EndRun();

            LastLevelWon = false;
            SetState(RunFlowState.PreRun);
        }

        private bool StartCurrentLevel(RunFlowState failureState)
        {
            if (RunProgress.CurrentRun == null ||
                stageDirector == null)
            {
                return false;
            }

            SetState(RunFlowState.InLevel);

            if (stageDirector.StartLevel(RunProgress.CurrentRun))
            {
                return true;
            }

            SetState(failureState);

            return false;
        }

        private void HandleLevelFinished(bool won)
        {
            LastLevelWon = won;

            if (won)
                GenerateRunUpgradeOffers();

            SetState(RunFlowState.BetweenLevels);
        }

        private void GenerateRunUpgradeOffers()
        {
            RunState runState = RunProgress.CurrentRun;

            if (runState == null)
                return;

            IReadOnlyList<RunUpgradeOffer> offers = RunUpgradeOfferGenerator.Generate(
                runState,
                availableRunRewards,
                runUpgradeOfferCount
            );

            if (offers.Count == 0)
            {
                Debug.Log(
                    "Level completed with no eligible run rewards. " +
                    "The run may continue normally."
                );

                return;
            }

            if (!RunProgress.EnqueueRewardChoice(offers))
            {
                Debug.LogWarning(
                    "Eligible run rewards were generated but could not be queued."
                );
            }
        }
        
        [ContextMenu("Debug/Queue Reward Choice")]
        private void DebugQueueRewardChoice()
        {
            if (!Application.isPlaying || state != RunFlowState.InLevel || !RunProgress.HasActiveRun)
            {
                Debug.LogWarning("A reward choice can only be queued during an active level.");
                return;
            }

            GenerateRunUpgradeOffers();
        }

        private void HandleCurrencyEarned(int amount)
        {
            RunProgress.AddCurrency(amount);
        }

        private void SetState(
            RunFlowState newState
        )
        {
            if (state == newState)
                return;

            state = newState;

            StateChanged?.Invoke(state);
        }
    }
}