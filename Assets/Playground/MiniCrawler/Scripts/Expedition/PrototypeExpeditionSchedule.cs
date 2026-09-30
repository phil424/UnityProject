using System;
using System.Collections.Generic;
using MiniCrawler.Encounters;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Expedition
{
    [DefaultExecutionOrder(-450)]
    [DisallowMultipleComponent]
    public sealed class PrototypeExpeditionSchedule : MonoBehaviour
    {
        [Serializable]
        public sealed class Entry
        {
            [SerializeField] private string displayName = "Scheduled Event";
            [SerializeField, Min(0f)] private float worldMinutesAfterExpeditionStart = 30f;
            [SerializeField] private bool forecastOnly;

            [Header("Encounter Actions")]
            [SerializeField] private LevelEncounterActions onDue = new();

            [Header("World Event")]
            [SerializeField] private WorldEventDefinition worldEvent;
            [SerializeField, Min(1f)] private float worldEventDurationWorldMinutes = 180f;

            [NonSerialized] private WorldTimestamp scheduledTime;
            [NonSerialized] private bool isInitialized;
            [NonSerialized] private bool isResolved;
            [NonSerialized] private bool wasTriggered;
            [NonSerialized] private bool resolvedEarly;

            public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? "Scheduled Event" : displayName;
            public float WorldMinutesAfterExpeditionStart => worldMinutesAfterExpeditionStart;
            public bool ForecastOnly => forecastOnly;
            public WorldEventDefinition WorldEvent => worldEvent;

            public WorldTimestamp ScheduledTime => scheduledTime;

            public bool IsInitialized => isInitialized;
            public bool IsResolved => isResolved;
            public bool WasTriggered => wasTriggered;
            public bool ResolvedEarly => resolvedEarly;

            internal void Initialize(WorldTimestamp expeditionStartTime)
            {
                scheduledTime = expeditionStartTime.AddMinutes(Mathf.Max(0f, worldMinutesAfterExpeditionStart));

                isInitialized = true;
                isResolved = false;
                wasTriggered = false;
                resolvedEarly = false;
            }

            internal bool ShouldResolveEarly()
            {
                if (!isInitialized || isResolved || forecastOnly)
                    return false;

                if (worldEvent != null)
                    return false;

                return onDue == null || !onDue.HasPendingActions();
            }

            internal void ResolveEarly()
            {
                if (isResolved)
                    return;

                isResolved = true;
                resolvedEarly = true;
            }

            internal void Trigger()
            {
                if (!isInitialized || isResolved)
                    return;

                if (!forecastOnly)
                {
                    onDue?.Execute();

                    if (worldEvent != null && RunProgress.WorldEvents != null)
                    {
                        WorldTimestamp endsAt =
                            scheduledTime.AddMinutes(Mathf.Max(1f, worldEventDurationWorldMinutes));

                        RunProgress.WorldEvents.TryActivate(worldEvent, scheduledTime, endsAt);
                    }
                }

                wasTriggered = true;
                isResolved = true;
            }

            internal void ResetRuntime()
            {
                scheduledTime = default;
                isInitialized = false;
                isResolved = false;
                wasTriggered = false;
                resolvedEarly = false;
            }
        }

        public static PrototypeExpeditionSchedule Instance { get; private set; }

        [SerializeField] private List<Entry> entries = new();

        private readonly List<Entry> orderedEntries = new();

        private RunState activeRun;
        private bool initializedForRun;

        public event Action ScheduleChanged;
        public event Action<Entry> EntryTriggered;

        public IReadOnlyList<Entry> Entries => orderedEntries;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            RunState currentRun = RunProgress.CurrentRun;

            if (!ReferenceEquals(currentRun, activeRun))
                SetActiveRun(currentRun);

            if (activeRun == null || !activeRun.WorldClock.IsInitialized)
                return;

            if (!initializedForRun)
                InitializeForRun();

            EvaluateSchedule(activeRun.WorldClock.CurrentTime);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void GetUpcoming(List<Entry> results, int maxCount)
        {
            results.Clear();

            if (maxCount <= 0)
                return;

            foreach (Entry entry in orderedEntries)
            {
                if (entry == null || !entry.IsInitialized || entry.IsResolved)
                    continue;

                results.Add(entry);

                if (results.Count >= maxCount)
                    return;
            }
        }

        private void SetActiveRun(RunState runState)
        {
            activeRun = runState;
            initializedForRun = false;
            orderedEntries.Clear();

            foreach (Entry entry in entries)
                entry?.ResetRuntime();

            ScheduleChanged?.Invoke();
        }

        private void InitializeForRun()
        {
            orderedEntries.Clear();

            foreach (Entry entry in entries)
            {
                if (entry == null)
                    continue;

                entry.Initialize(activeRun.WorldClock.StartTime);
                orderedEntries.Add(entry);
            }

            orderedEntries.Sort((a, b) => a.ScheduledTime.CompareTo(b.ScheduledTime));

            initializedForRun = true;
            ScheduleChanged?.Invoke();
        }

        private void EvaluateSchedule(WorldTimestamp currentTime)
        {
            bool changed = false;

            foreach (Entry entry in orderedEntries)
            {
                if (entry == null || entry.IsResolved)
                    continue;

                if (entry.ShouldResolveEarly())
                {
                    entry.ResolveEarly();

                    Debug.Log($"Forecast resolved early: {entry.DisplayName}", this);

                    changed = true;
                    continue;
                }

                if (currentTime.CompareTo(entry.ScheduledTime) < 0)
                    continue;

                entry.Trigger();

                Debug.Log($"Scheduled event reached: {entry.DisplayName}", this);

                EntryTriggered?.Invoke(entry);
                changed = true;
            }

            if (changed)
                ScheduleChanged?.Invoke();
        }
    }
}