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
            EndingExpedition,
            BetweenLevels
        }
        
        public enum ExpeditionEndReason
        {
            None,
            Defeat,
            VoluntaryReturn,
            Success
        }

        public static RunDirector Instance { get; private set; }

        public event Action<RunFlowState> StateChanged;

        [SerializeField] private StageDirector stageDirector;

        [Header("Setup")]
        [SerializeField] private PlayableCharacterRosterDefinition characterRoster;
        [SerializeField, Min(1)] private int maximumPartySize = 4;
        
        [Header("Prototype Objectives")]
        [SerializeField] private ExpeditionObjectiveDefinition[] defaultExpeditionObjectives;

        [Header("Run Rewards")]
        [FormerlySerializedAs("availableRunUpgrades")]
        [SerializeField] private RunRewardDefinition[] availableRunRewards;

        [SerializeField, Min(1)]
        private int runUpgradeOfferCount = 3;

        private RunSetup setup;

        private RunFlowState state = RunFlowState.PreRun;
        private bool returnToPreRunWhenLevelCleared;

        public RunFlowState State => state;

        public string StateName => state.ToString();

        public RunSetup Setup => setup;
        public PlayableCharacterRosterDefinition CharacterRoster => characterRoster;

        public bool LastLevelWon { get; private set; }
        public ExpeditionEndReason LastExpeditionEndReason { get; private set; }
        public int LastLevel1ResourcesBanked { get; private set; }
        public int LastLevel2ResourcesBanked { get; private set; }
        public int LastLevel3ResourcesBanked { get; private set; }

        public bool CanContinueRun => RunProgress.HasActiveRun && state == RunFlowState.BetweenLevels && !RunProgress.HasPendingRewardChoice;

        private void Awake()
        {
            Instance = this;

            if (characterRoster == null)
                Debug.LogError("RunDirector requires a playable Character roster.", this);

            PersistentProgression.InitializeCharacterRoster(characterRoster);

            setup = new RunSetup(maximumPartySize, characterRoster);
            setup.SetObjectives(defaultExpeditionObjectives);
        }

        private void Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (stageDirector == null)
            {
                Debug.LogError("RunDirector requires a StageDirector.");
                return;
            }

            stageDirector.LevelFinished += HandleLevelFinished;
            stageDirector.LevelCleared += HandleLevelCleared;
            stageDirector.CurrencyEarned += HandleCurrencyEarned;
            stageDirector.PersistentResourceEarned += HandlePersistentResourceEarned;
            stageDirector.RewardChoiceEarned += HandleRewardChoiceEarned;

            stageDirector.ClearLevel();

            SetState(RunFlowState.PreRun);
        }

        private void OnDestroy()
        {
            if (stageDirector != null)
            {
                stageDirector.LevelFinished -= HandleLevelFinished;
                stageDirector.LevelCleared -= HandleLevelCleared;
                stageDirector.CurrencyEarned -= HandleCurrencyEarned;
                stageDirector.PersistentResourceEarned -= HandlePersistentResourceEarned;
                stageDirector.RewardChoiceEarned -= HandleRewardChoiceEarned;
            }

            if (Instance == this)
                Instance = null;
        }

        public bool BeginRun()
        {
            if (state != RunFlowState.PreRun)
            {
                Debug.LogWarning("Cannot begin a new expedition until the current flow has returned to preparation.");
                return false;
            }
            
            if (RunProgress.HasActiveRun)
            {
                Debug.LogWarning("Cannot begin a new run while another run is active.");
                return false;
            }
            
            LastExpeditionEndReason = ExpeditionEndReason.None;
            LastLevel1ResourcesBanked = 0;
            LastLevel2ResourcesBanked = 0;
            LastLevel3ResourcesBanked = 0;
            LastLevelWon = false;

            RunStartConfiguration configuration = setup.CreateConfiguration();

            if (!RunProgress.BeginRun(configuration))
            {
                Debug.LogWarning("Cannot begin a run without a valid setup.");
                return false;
            }

            if (StartCurrentLevel(RunFlowState.PreRun))
                return true;

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
                Debug.LogWarning("Resolve all earned pending rewards before ending the expedition voluntarily.");
                return;
            }

            FinishExpedition(ExpeditionEndReason.VoluntaryReturn);
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

            if (!won)
            {
                FinishExpedition(ExpeditionEndReason.Defeat, waitForLevelCleanup: true);
                return;
            }

            GenerateRunRewardOffers(new RunRewardContext(RunRewardVector.Any, "Area Clear"));
            SetState(RunFlowState.BetweenLevels);
        }
        
        private void HandleLevelCleared()
        {
            if (!returnToPreRunWhenLevelCleared)
                return;

            returnToPreRunWhenLevelCleared = false;
            SetState(RunFlowState.PreRun);
        }
        
        private void FinishExpedition(ExpeditionEndReason reason, bool waitForLevelCleanup = false)
        {
            RunState runState = RunProgress.CurrentRun;

            if (runState == null)
                return;

            int level1ResourcesEarned = runState.GetPersistentResourceEarned(PersistentResourceTier.Level1);
            int level2ResourcesEarned = runState.GetPersistentResourceEarned(PersistentResourceTier.Level2);
            int level3ResourcesEarned = runState.GetPersistentResourceEarned(PersistentResourceTier.Level3);

            if (!waitForLevelCleanup && stageDirector != null && stageDirector.State != StageDirector.LevelState.Idle)
                stageDirector.ClearLevel();

            ApplyCharacterUnlockProgress(runState);

            RunProgress.EndRun();

            if (level1ResourcesEarned > 0)
                PersistentProgression.AddResource(PersistentResourceTier.Level1, level1ResourcesEarned);

            if (level2ResourcesEarned > 0)
                PersistentProgression.AddResource(PersistentResourceTier.Level2, level2ResourcesEarned);

            if (level3ResourcesEarned > 0)
                PersistentProgression.AddResource(PersistentResourceTier.Level3, level3ResourcesEarned);

            LastLevel1ResourcesBanked = level1ResourcesEarned;
            LastLevel2ResourcesBanked = level2ResourcesEarned;
            LastLevel3ResourcesBanked = level3ResourcesEarned;

            LastExpeditionEndReason = reason;
            LastLevelWon = false;

            returnToPreRunWhenLevelCleared = waitForLevelCleanup;

            SetState(waitForLevelCleanup ? RunFlowState.EndingExpedition : RunFlowState.PreRun);

            Debug.Log(
                $"Expedition ended: {reason}. Persistent resources banked: " +
                $"L1 {level1ResourcesEarned}, L2 {level2ResourcesEarned}, L3 {level3ResourcesEarned}.",
                this
            );
        }
        
        private void ApplyCharacterUnlockProgress(RunState runState)
        {
            if (runState == null || characterRoster == null)
                return;

            foreach (PlayableCharacterRosterEntry entry in characterRoster.Characters)
            {
                PartyMemberDefinition character = entry?.Character;
                CharacterUnlockRequirementDefinition requirement = entry?.UnlockRequirement;

                if (character == null ||
                    requirement == null ||
                    !requirement.IsConfigured ||
                    PersistentProgression.IsCharacterUnlocked(character))
                {
                    continue;
                }

                int contribution = requirement.GetProgressContribution(runState);

                if (contribution <= 0 ||
                    !PersistentProgression.TryAddCharacterUnlockProgress(character, requirement, contribution))
                {
                    continue;
                }

                int progress = PersistentProgression.GetCharacterUnlockProgress(character);
                string status = PersistentProgression.IsCharacterUnlocked(character)
                    ? "UNLOCKED"
                    : $"{progress}/{requirement.RequiredProgress}";

                Debug.Log($"Character unlock progress: {character.DisplayName} - {status}.", this);
            }
        }

        private void GenerateRunRewardOffers(RunRewardContext context)
        {
            RunState runState = RunProgress.CurrentRun;

            if (runState == null)
                return;

            IReadOnlyList<RunUpgradeOffer> offers = RunUpgradeOfferGenerator.Generate(
                runState,
                availableRunRewards,
                runUpgradeOfferCount,
                context.RewardVector
            );

            if (offers.Count == 0)
            {
                Debug.Log(
                    $"No eligible run rewards were available for reward vector '{context.RewardVector}' " +
                    $"from source '{context.SourceLabel}'."
                );

                return;
            }

            if (!RunProgress.EnqueueRewardChoice(offers, context))
                Debug.LogWarning("Eligible run rewards were generated but could not be queued.");
        }
        
        [ContextMenu("Debug/Queue Reward Choice")]
        private void DebugQueueRewardChoice()
        {
            if (!Application.isPlaying ||
                state != RunFlowState.InLevel ||
                !RunProgress.HasActiveRun ||
                stageDirector == null)
            {
                Debug.LogWarning("A reward choice can only be queued during an active level.");
                return;
            }

            if (!stageDirector.TryAwardRewardChoice(RunRewardVector.Any, "Debug Reward"))
                Debug.LogWarning("The current level could not award a reward choice.");
        }
        
        [ContextMenu("Debug/Queue Weapon Mod Reward")]
        private void DebugQueueWeaponModReward()
        {
            if (!Application.isPlaying ||
                state != RunFlowState.InLevel ||
                !RunProgress.HasActiveRun ||
                stageDirector == null)
            {
                Debug.LogWarning("A Weapon Mod reward can only be queued during an active level.");
                return;
            }

            if (!stageDirector.TryAwardRewardChoice(RunRewardVector.WeaponMod, "Debug Weapon Mod Reward"))
                Debug.LogWarning("The current level could not award a Weapon Mod reward choice.");
        }
        
        [ContextMenu("Debug/Unlock All Roster Characters")]
        private void DebugUnlockAllRosterCharacters()
        {
            if (!Application.isPlaying || characterRoster == null)
                return;

            foreach (PlayableCharacterRosterEntry entry in characterRoster.Characters)
            {
                if (entry?.Character != null)
                    PersistentProgression.TryUnlockCharacter(entry.Character);
            }
        }
        
        private void HandleRewardChoiceEarned(RunRewardContext context)
        {
            GenerateRunRewardOffers(context);
        }

        private void HandleCurrencyEarned(int amount)
        {
            RunProgress.AddCurrency(amount);
        }
        
        private void HandlePersistentResourceEarned(PersistentResourceTier tier, int amount)
        {
            RunProgress.AddPersistentResource(tier, amount);
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