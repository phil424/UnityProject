using System;
using System.Collections.Generic;
using MiniCrawler.Progress;
using UnityEngine;

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

        [Header("Run Upgrade Rewards")]
        [SerializeField]
        private RunUpgradeDefinition[] availableRunUpgrades;

        [SerializeField, Min(1)]
        private int runUpgradeOfferCount = 3;

        private readonly RunSetup setup = new();

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
            state == RunFlowState.BetweenLevels &&
            !RunProgress.HasPendingUpgradeChoice;

        private void Awake()
        {
            Instance = this;
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
            if (!RunProgress.HasActiveRun)
            {
                Debug.LogWarning(
                    "Cannot continue without an active run."
                );

                return false;
            }

            if (state != RunFlowState.BetweenLevels)
            {
                Debug.LogWarning(
                    "Can only continue from the between-level state."
                );

                return false;
            }

            if (RunProgress.HasPendingUpgradeChoice)
            {
                Debug.LogWarning(
                    "Choose a run upgrade before continuing."
                );

                return false;
            }

            return StartCurrentLevel(
                RunFlowState.BetweenLevels
            );
        }

        public void EndRun()
        {
            if (stageDirector != null)
                stageDirector.ClearLevel();

            RunProgress.EndRun();

            LastLevelWon = false;

            SetState(RunFlowState.PreRun);
        }

        private bool StartCurrentLevel(
            RunFlowState failureState
        )
        {
            if (RunProgress.CurrentRun == null ||
                stageDirector == null)
            {
                return false;
            }

            SetState(RunFlowState.InLevel);

            if (stageDirector.StartLevel(
                    RunProgress.CurrentRun
                ))
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
            {
                GenerateRunUpgradeOffers();
            }
            else
            {
                RunProgress.SetRunUpgradeOffers(
                    null
                );
            }

            SetState(
                RunFlowState.BetweenLevels
            );
        }

        private void GenerateRunUpgradeOffers()
        {
            IReadOnlyList<RunUpgradeOffer> offers =
                RunUpgradeOfferGenerator.Generate(
                    RunProgress.SelectedParty,
                    availableRunUpgrades,
                    runUpgradeOfferCount
                );

            RunProgress.SetRunUpgradeOffers(
                offers
            );

            if (offers.Count <= 0)
            {
                Debug.LogWarning(
                    "Level was won but no run upgrade offers could be generated."
                );
            }
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