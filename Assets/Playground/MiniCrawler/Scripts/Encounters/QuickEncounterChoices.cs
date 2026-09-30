using System;
using System.Collections.Generic;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(StageDirector), typeof(EncounterDirectionController))]
    public sealed class QuickEncounterChoices : MonoBehaviour
    {
        public const int SlotCount = 3;

        public static QuickEncounterChoices Instance { get; private set; }

        [Header("Quick Slots")]
        [SerializeField] private bool includeSelectedEncounter = true;
        [SerializeField] private QuickEncounterFocus focus = QuickEncounterFocus.Balanced;

        private readonly LevelEncounter[] choices = new LevelEncounter[SlotCount];
        private readonly List<LevelEncounter> candidates = new();

        private StageDirector stageDirector;
        private EncounterDirectionController directionController;

        public event Action ChoicesChanged;
        public event Action<QuickEncounterFocus> FocusChanged;

        public IReadOnlyList<LevelEncounter> Choices => choices;
        public bool IncludeSelectedEncounter => includeSelectedEncounter;
        public QuickEncounterFocus Focus => focus;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            stageDirector = GetComponent<StageDirector>();
            directionController = GetComponent<EncounterDirectionController>();
        }

        private void Update()
        {
            RefreshChoices();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public LevelEncounter GetChoice(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
                return null;

            return choices[slotIndex];
        }

        public bool TrySelectSlot(int slotIndex)
        {
            RefreshChoices();

            LevelEncounter encounter = GetChoice(slotIndex);

            return encounter != null &&
                   directionController != null &&
                   directionController.SelectEncounter(encounter);
        }

        public void SetIncludeSelectedEncounter(bool value)
        {
            if (includeSelectedEncounter == value)
                return;

            includeSelectedEncounter = value;
            RefreshChoices();
        }

        public void SetFocus(QuickEncounterFocus value)
        {
            if (focus == value)
                return;

            focus = value;
            FocusChanged?.Invoke(focus);
            RefreshChoices();
        }

        public void RefreshChoices()
        {
            candidates.Clear();

            if (stageDirector != null && stageDirector.State == StageDirector.LevelState.FightingMinions)
            {
                LevelEncounter selectedEncounter =
                    directionController != null ? directionController.SelectedEncounter : null;

                foreach (LevelEncounter encounter in stageDirector.Encounters)
                {
                    if (encounter == null || !encounter.IsSelectable)
                        continue;

                    if (!includeSelectedEncounter && encounter == selectedEncounter)
                        continue;

                    candidates.Add(encounter);
                }

                candidates.Sort(CompareCandidates);
            }

            bool changed = false;

            for (int i = 0; i < SlotCount; i++)
            {
                LevelEncounter newChoice = i < candidates.Count ? candidates[i] : null;

                if (choices[i] == newChoice)
                    continue;

                choices[i] = newChoice;
                changed = true;
            }

            if (changed)
                ChoicesChanged?.Invoke();
        }

        private int CompareCandidates(LevelEncounter a, LevelEncounter b)
        {
            bool aMatchesFocus = MatchesFocus(a);
            bool bMatchesFocus = MatchesFocus(b);

            if (aMatchesFocus != bMatchesFocus)
                return aMatchesFocus ? -1 : 1;

            return CompareByAvailability(a, b);
        }

        private bool MatchesFocus(LevelEncounter encounter)
        {
            if (focus == QuickEncounterFocus.Balanced || encounter == null)
                return false;

            LevelEncounterDefinitionBinding binding = encounter.GetComponent<LevelEncounterDefinitionBinding>();
            EncounterDefinition definition = binding != null ? binding.Definition : null;

            if (definition == null || definition.RewardChoicesOnCompletion <= 0)
                return false;

            return focus switch
            {
                QuickEncounterFocus.CharacterGrowth =>
                    definition.RewardVector == RunRewardVector.CharacterGrowth,

                QuickEncounterFocus.Ability =>
                    definition.RewardVector == RunRewardVector.Ability,

                _ => false
            };
        }

        private static int CompareByAvailability(LevelEncounter a, LevelEncounter b)
        {
            long aOrder = a.AvailabilitySequence > 0 ? a.AvailabilitySequence : long.MaxValue;
            long bOrder = b.AvailabilitySequence > 0 ? b.AvailabilitySequence : long.MaxValue;

            int orderComparison = aOrder.CompareTo(bOrder);

            if (orderComparison != 0)
                return orderComparison;

            return string.Compare(a.Id, b.Id, StringComparison.Ordinal);
        }
    }
}