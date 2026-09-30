using System;
using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class ActiveEncounterListView : MonoBehaviour
    {
        [SerializeField] private ActiveEncounterView entryPrefab;
        [SerializeField, Min(1)] private int maxDisplayed = 4;

        private readonly List<ActiveEncounterView> views = new();
        private readonly List<LevelEncounter> activeEncounters = new();

        private void Awake()
        {
            EnsureViews();
        }

        private void Update()
        {
            Refresh();
        }

        private void EnsureViews()
        {
            if (entryPrefab == null)
                return;

            while (views.Count < maxDisplayed)
            {
                ActiveEncounterView view = Instantiate(entryPrefab, transform);
                view.SetVisible(false);
                views.Add(view);
            }
        }

        private void Refresh()
        {
            EnsureViews();
            activeEncounters.Clear();

            StageDirector stageDirector = StageDirector.Instance;

            if (stageDirector != null && stageDirector.State == StageDirector.LevelState.FightingMinions)
            {
                foreach (LevelEncounter encounter in stageDirector.Encounters)
                {
                    if (encounter != null &&
                        encounter.HasStarted &&
                        !encounter.IsCompleted &&
                        !encounter.IsExpired)
                    {
                        activeEncounters.Add(encounter);
                    }
                }

                activeEncounters.Sort(CompareByStartOrder);
            }

            for (int i = 0; i < views.Count; i++)
            {
                if (i < activeEncounters.Count)
                    views[i].SetEncounter(activeEncounters[i]);
                else
                    views[i].SetVisible(false);
            }
        }

        private static int CompareByStartOrder(LevelEncounter a, LevelEncounter b)
        {
            int sequenceComparison = a.StartedSequence.CompareTo(b.StartedSequence);

            return sequenceComparison != 0
                ? sequenceComparison
                : string.Compare(a.Id, b.Id, StringComparison.Ordinal);
        }
    }
}