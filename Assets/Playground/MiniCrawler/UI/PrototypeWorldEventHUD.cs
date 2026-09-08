using System;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class PrototypeWorldEventHUD : MonoBehaviour
    {
        private const float Margin = 10f;
        private const float TopOffset = 240f;
        private const float PanelWidth = 340f;
        private const float PanelHeight = 105f;

        private void OnGUI()
        {
            RunDirector runDirector = RunDirector.Instance;

            if (!RunProgress.HasActiveRun ||
                runDirector == null ||
                runDirector.State != RunDirector.RunFlowState.InLevel)
            {
                return;
            }

            WorldEventState worldEvents = RunProgress.WorldEvents;
            WorldClockState worldClock = RunProgress.WorldClock;

            if (worldEvents == null ||
                worldClock == null ||
                !worldClock.IsInitialized ||
                worldEvents.ActiveEvents.Count == 0)
            {
                return;
            }

            float width = Mathf.Min(PanelWidth, Screen.width - Margin * 2f);

            Rect panelRect = new(
                Screen.width - width - Margin,
                TopOffset,
                width,
                PanelHeight
            );

            GUILayout.BeginArea(panelRect, GUI.skin.box);

            GUILayout.Label("CURRENT WORLD EVENTS");

            int displayed = 0;

            foreach (ActiveWorldEvent activeEvent in worldEvents.ActiveEvents)
            {
                if (activeEvent?.Definition == null)
                    continue;

                double remainingSeconds =
                    GetSimulationSecondsRemaining(worldClock.CurrentTime, activeEvent.EndsAt);

                GUILayout.Label(
                    $"{activeEvent.Definition.DisplayName}   {FormatCountdown(remainingSeconds)}"
                );

                displayed++;

                if (displayed >= 3)
                    break;
            }

            GUILayout.EndArea();
        }

        private static double GetSimulationSecondsRemaining(
            WorldTimestamp currentTime,
            WorldTimestamp endsAt)
        {
            double remainingWorldMinutes = Math.Max(0d, currentTime.MinutesUntil(endsAt));

            WorldClockSystem worldClockSystem = WorldClockSystem.Instance;

            float worldMinutesPerSimulationSecond =
                worldClockSystem != null
                    ? worldClockSystem.WorldHoursPerSimulationMinute
                    : 1f;

            return remainingWorldMinutes /
                   Math.Max(0.01f, worldMinutesPerSimulationSecond);
        }

        private static string FormatCountdown(double seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));

            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            return $"{minutes:00}:{remainingSeconds:00}";
        }
    }
}