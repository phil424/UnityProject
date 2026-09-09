using System;
using System.Collections.Generic;
using System.Text;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class StrategicHUD : MonoBehaviour
    {
        private const double ImmediateCountdownSeconds = 30d;

        [Header("Forecast")]
        [SerializeField] private TMP_Text worldTimeText;
        [SerializeField] private TMP_Text upcomingText;

        [Header("Current World Events")]
        [SerializeField] private GameObject currentWorldEventsRoot;
        [SerializeField] private TMP_Text currentWorldEventsText;

        [Header("Strategic Views")]
        [SerializeField] private QuickEncounterSlotGroup encounterList;
        [SerializeField] private StrategicMinimapView minimap;

        private readonly List<PrototypeExpeditionSchedule.Entry> upcoming = new();
        private readonly StringBuilder textBuilder = new();

        private void Awake()
        {
            encounterList?.Bind(SelectEncounterSlot);
            minimap?.Bind(SelectEncounterSlot);
        }

        private void Update()
        {
            RefreshForecast();
            RefreshWorldEvents();
            RefreshEncounters();
            RefreshPartyMarker();
        }

        private void SelectEncounterSlot(int slotIndex)
        {
            QuickEncounterChoices.Instance?.TrySelectSlot(slotIndex);
        }

        private void RefreshForecast()
        {
            WorldClockState worldClock = RunProgress.WorldClock;
            PrototypeExpeditionSchedule schedule = PrototypeExpeditionSchedule.Instance;

            if (worldClock == null || !worldClock.IsInitialized)
            {
                if (worldTimeText != null)
                    worldTimeText.text = "DAY --   --:--";

                if (upcomingText != null)
                    upcomingText.text = "No active expedition.";

                return;
            }

            WorldTimestamp currentTime = worldClock.CurrentTime;

            if (worldTimeText != null)
                worldTimeText.text = $"DAY {currentTime.DayNumber}   {currentTime.ClockText}";

            if (upcomingText == null)
                return;

            if (schedule == null)
            {
                upcomingText.text = "No expedition forecast.";
                return;
            }

            schedule.GetUpcoming(upcoming, 3);

            textBuilder.Clear();

            foreach (PrototypeExpeditionSchedule.Entry entry in upcoming)
            {
                double remainingSeconds = GetSimulationSecondsRemaining(currentTime, entry.ScheduledTime);
                string timeText = remainingSeconds <= ImmediateCountdownSeconds
                    ? FormatCountdown(remainingSeconds)
                    : FormatWorldTime(entry.ScheduledTime, currentTime);

                if (textBuilder.Length > 0)
                    textBuilder.AppendLine();

                textBuilder.Append($"{timeText}   {entry.DisplayName}");
            }

            upcomingText.text = textBuilder.Length > 0 ? textBuilder.ToString() : "No upcoming entries.";
        }

        private void RefreshWorldEvents()
        {
            WorldEventState worldEvents = RunProgress.WorldEvents;
            WorldClockState worldClock = RunProgress.WorldClock;

            bool hasEvents = worldEvents != null &&
                             worldClock != null &&
                             worldClock.IsInitialized &&
                             worldEvents.ActiveEvents.Count > 0;

            if (currentWorldEventsRoot != null)
                currentWorldEventsRoot.SetActive(hasEvents);

            if (!hasEvents || currentWorldEventsText == null)
                return;

            textBuilder.Clear();

            foreach (ActiveWorldEvent activeEvent in worldEvents.ActiveEvents)
            {
                if (activeEvent?.Definition == null)
                    continue;

                double remainingSeconds = GetSimulationSecondsRemaining(worldClock.CurrentTime, activeEvent.EndsAt);

                if (textBuilder.Length > 0)
                    textBuilder.AppendLine();

                textBuilder.Append($"{activeEvent.Definition.DisplayName}   {FormatCountdown(remainingSeconds)}");
            }

            currentWorldEventsText.text = textBuilder.ToString();
        }

        private void RefreshEncounters()
        {
            QuickEncounterChoices quickChoices = QuickEncounterChoices.Instance;
            EncounterDirectionController direction = EncounterDirectionController.Instance;
            PrototypeEncounterSupply supply = PrototypeEncounterSupply.Instance;
            StageDirector stageDirector = StageDirector.Instance;

            bool canShowChoices = quickChoices != null &&
                                  stageDirector != null &&
                                  stageDirector.State == StageDirector.LevelState.FightingMinions;

            bool selectedAppearsInQuickSlots = false;

            for (int i = 0; i < QuickEncounterChoices.SlotCount; i++)
            {
                LevelEncounter encounter = canShowChoices ? quickChoices.GetChoice(i) : null;
                bool selected = encounter != null && direction != null && direction.SelectedEncounter == encounter;

                QuickEncounterSlotView listSlot = encounterList?.GetSlot(i);

                if (listSlot != null)
                {
                    listSlot.SetVisible(encounter != null);

                    if (encounter != null)
                    {
                        listSlot.SetSelected(selected);
                        listSlot.SetLabel(encounter.DisplayName + BuildEncounterStatus(encounter, direction, supply));
                    }
                }

                minimap?.SetEncounterMarker(
                    i,
                    encounter != null,
                    selected,
                    encounter != null ? encounter.AnchorPosition : Vector3.zero
                );

                if (selected)
                    selectedAppearsInQuickSlots = true;
            }

            LevelEncounter selectedEncounter = direction != null ? direction.SelectedEncounter : null;

            minimap?.SetSelectedDirectiveMarker(
                selectedEncounter != null && !selectedAppearsInQuickSlots,
                selectedEncounter != null ? selectedEncounter.AnchorPosition : Vector3.zero
            );
        }

        private void RefreshPartyMarker()
        {
            PartyMember partyMember = FindLivingPartyMember();

            minimap?.SetPartyMarker(
                partyMember != null,
                partyMember != null ? partyMember.transform.position : Vector3.zero
            );
        }

        private PartyMember FindLivingPartyMember()
        {
            foreach (PartyMember partyMember in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
            {
                if (partyMember == null)
                    continue;

                Health health = partyMember.GetComponent<Health>();

                if (health == null || !health.IsDead)
                    return partyMember;
            }

            return null;
        }

        private static string BuildEncounterStatus(
            LevelEncounter encounter,
            EncounterDirectionController direction,
            PrototypeEncounterSupply supply)
        {
            string status = string.Empty;

            if (supply != null)
            {
                int occurrence = supply.GetOccurrence(encounter);

                if (occurrence > 1)
                    status += $" x{occurrence}";
            }

            bool selected = direction != null && direction.SelectedEncounter == encounter;

            if (selected || supply != null && supply.IsExpiryProtected(encounter))
                return status + "  COMMITTED";

            WorldClockState worldClock = RunProgress.WorldClock;

            if (supply == null ||
                worldClock == null ||
                !worldClock.IsInitialized ||
                !supply.TryGetExpiry(encounter, out WorldTimestamp expiry))
            {
                return status;
            }

            double remainingSeconds = GetSimulationSecondsRemaining(worldClock.CurrentTime, expiry);

            return status + $"  {FormatCountdown(remainingSeconds)}";
        }

        private static double GetSimulationSecondsRemaining(WorldTimestamp currentTime, WorldTimestamp targetTime)
        {
            double remainingWorldMinutes = Math.Max(0d, currentTime.MinutesUntil(targetTime));
            WorldClockSystem clockSystem = WorldClockSystem.Instance;
            float worldMinutesPerSimulationSecond =
                clockSystem != null ? clockSystem.WorldHoursPerSimulationMinute : 1f;

            return remainingWorldMinutes / Math.Max(0.01f, worldMinutesPerSimulationSecond);
        }

        private static string FormatCountdown(double seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            return $"{minutes:00}:{remainingSeconds:00}";
        }

        private static string FormatWorldTime(WorldTimestamp scheduledTime, WorldTimestamp currentTime)
        {
            return scheduledTime.DayNumber == currentTime.DayNumber
                ? scheduledTime.ClockText
                : $"D{scheduledTime.DayNumber} {scheduledTime.ClockText}";
        }
    }
}