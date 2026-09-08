using System;
using System.Collections.Generic;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class PrototypeForecastHUD : MonoBehaviour
    {
        private const float Margin = 10f;
        private const float TopOffset = 60f;
        private const float PanelWidth = 330f;
        private const float PanelHeight = 165f;
        private const double ImmediateCountdownSeconds = 30d;

        private readonly List<PrototypeExpeditionSchedule.Entry> upcoming = new();

        private void OnGUI()
        {
            RunDirector runDirector = RunDirector.Instance;

            if (!RunProgress.HasActiveRun ||
                runDirector == null ||
                runDirector.State != RunDirector.RunFlowState.InLevel)
            {
                return;
            }

            WorldClockState worldClock = RunProgress.WorldClock;
            PrototypeExpeditionSchedule schedule = PrototypeExpeditionSchedule.Instance;

            if (worldClock == null || !worldClock.IsInitialized || schedule == null)
                return;

            float width = Mathf.Min(PanelWidth, Screen.width - Margin * 2f);

            Rect panelRect = new(
                Margin,
                TopOffset,
                width,
                PanelHeight
            );

            GUILayout.BeginArea(panelRect, GUI.skin.box);

            WorldTimestamp currentTime = worldClock.CurrentTime;

            GUILayout.Label($"DAY {currentTime.DayNumber}   {currentTime.ClockText}");
            GUILayout.Space(4);
            GUILayout.Label("UPCOMING");

            schedule.GetUpcoming(upcoming, 3);

            if (upcoming.Count == 0)
            {
                GUILayout.Label("No forecast entries.");
                GUILayout.EndArea();
                return;
            }

            foreach (PrototypeExpeditionSchedule.Entry entry in upcoming)
            {
                double countdownSeconds = GetSimulationSecondsRemaining(currentTime, entry.ScheduledTime);

                string timeText = countdownSeconds <= ImmediateCountdownSeconds
                    ? FormatCountdown(countdownSeconds)
                    : FormatWorldTime(entry.ScheduledTime, currentTime);

                GUILayout.Label($"{timeText}   {entry.DisplayName}");
            }

            GUILayout.EndArea();
        }

        private static double GetSimulationSecondsRemaining(WorldTimestamp currentTime, WorldTimestamp scheduledTime)
        {
            double remainingWorldMinutes = Math.Max(0d, currentTime.MinutesUntil(scheduledTime));

            WorldClockSystem worldClockSystem = WorldClockSystem.Instance;

            float worldMinutesPerSimulationSecond =
                worldClockSystem != null ? worldClockSystem.WorldHoursPerSimulationMinute : 1f;

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
            if (scheduledTime.DayNumber == currentTime.DayNumber)
                return scheduledTime.ClockText;

            return $"D{scheduledTime.DayNumber} {scheduledTime.ClockText}";
        }
    }
}