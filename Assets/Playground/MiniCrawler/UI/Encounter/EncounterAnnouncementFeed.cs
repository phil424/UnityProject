using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class EncounterAnnouncementFeed : MonoBehaviour
    {
        [SerializeField] private EncounterAnnouncementView announcementPrefab;

        [Header("Timing")]
        [SerializeField, Min(0f)] private float fadeInSeconds = 0.15f;
        [SerializeField, Min(0f)] private float holdSeconds = 0.8f;
        [SerializeField, Min(0f)] private float fadeOutSeconds = 0.35f;

        [Header("Stacking")]
        [SerializeField, Min(1)] private int maxVisible = 4;

        private readonly List<EncounterAnnouncementView> activeViews = new();

        private StageDirector stageDirector;

        private void OnEnable()
        {
            ClearAnnouncements();
            TryBindStageDirector();
        }

        private void Start()
        {
            TryBindStageDirector();
        }

        private void Update()
        {
            if (stageDirector == null)
                TryBindStageDirector();
        }

        private void OnDisable()
        {
            UnbindStageDirector();
            ClearAnnouncements();
        }

        private void TryBindStageDirector()
        {
            StageDirector candidate = StageDirector.Instance;

            if (candidate == stageDirector)
                return;

            UnbindStageDirector();
            stageDirector = candidate;

            if (stageDirector == null)
                return;

            stageDirector.EncounterStarted += HandleEncounterStarted;
            stageDirector.EncounterCompleted += HandleEncounterCompleted;
            stageDirector.LevelCleared += HandleLevelCleared;
        }

        private void UnbindStageDirector()
        {
            if (stageDirector == null)
                return;

            stageDirector.EncounterStarted -= HandleEncounterStarted;
            stageDirector.EncounterCompleted -= HandleEncounterCompleted;
            stageDirector.LevelCleared -= HandleLevelCleared;

            stageDirector = null;
        }

        private void HandleEncounterStarted(LevelEncounter encounter)
        {
            if (encounter == null)
                return;

            string subtitle = string.IsNullOrWhiteSpace(encounter.Description)
                ? "Encounter Started"
                : encounter.Description;

            Show(encounter.DisplayName, subtitle);
        }

        private void HandleEncounterCompleted(LevelEncounter encounter)
        {
            if (encounter != null)
                Show("ENCOUNTER COMPLETE", encounter.DisplayName);
        }

        private void HandleLevelCleared()
        {
            ClearAnnouncements();
        }

        private void Show(string title, string subtitle)
        {
            if (announcementPrefab == null)
            {
                Debug.LogWarning("EncounterAnnouncementFeed has no announcement prefab.", this);
                return;
            }

            activeViews.RemoveAll(view => view == null);

            while (activeViews.Count >= maxVisible)
            {
                EncounterAnnouncementView oldest = activeViews[0];
                activeViews.RemoveAt(0);

                if (oldest != null)
                    Destroy(oldest.gameObject);
            }

            EncounterAnnouncementView view = Instantiate(announcementPrefab, transform);
            activeViews.Add(view);

            view.Play(
                title,
                subtitle,
                fadeInSeconds,
                holdSeconds,
                fadeOutSeconds,
                HandleFinished
            );
        }

        private void HandleFinished(EncounterAnnouncementView view)
        {
            activeViews.Remove(view);

            if (view != null)
                Destroy(view.gameObject);
        }

        private void ClearAnnouncements()
        {
            activeViews.Clear();

            EncounterAnnouncementView[] views =
                GetComponentsInChildren<EncounterAnnouncementView>(true);

            foreach (EncounterAnnouncementView view in views)
            {
                if (view != null)
                    Destroy(view.gameObject);
            }
        }
    }
}