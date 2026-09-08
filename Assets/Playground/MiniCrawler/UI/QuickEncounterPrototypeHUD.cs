using System;
using MiniCrawler.Encounters;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class QuickEncounterPrototypeHUD : MonoBehaviour
    {
        private const float Margin = 10f;
        private const float DebugButtonClearance = 45f;
        private const float PanelWidth = 340f;
        private const float PanelHeight = 175f;

        private static readonly string[] SlotGlyphs =
        {
            "△",
            "□",
            "○"
        };

        private void OnGUI()
        {
            RunDirector runDirector = RunDirector.Instance;
            StageDirector stageDirector = StageDirector.Instance;

            if (!RunProgress.HasActiveRun ||
                runDirector == null ||
                runDirector.State != RunDirector.RunFlowState.InLevel ||
                stageDirector == null ||
                stageDirector.State != StageDirector.LevelState.FightingMinions)
            {
                return;
            }

            QuickEncounterChoices quickChoices = QuickEncounterChoices.Instance;

            if (quickChoices == null)
                return;

            float width = Mathf.Min(PanelWidth, Screen.width - Margin * 2f);

            Rect panelRect = new(
                Screen.width - width - Margin,
                Margin + DebugButtonClearance,
                width,
                PanelHeight
            );

            GUILayout.BeginArea(panelRect, GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("ENCOUNTERS");
            GUILayout.FlexibleSpace();
            GUILayout.Label("R1");
            GUILayout.EndHorizontal();

            EncounterDirectionController direction = EncounterDirectionController.Instance;
            PrototypeEncounterSupply supply = PrototypeEncounterSupply.Instance;

            for (int i = 0; i < QuickEncounterChoices.SlotCount; i++)
            {
                LevelEncounter encounter = quickChoices.GetChoice(i);

                if (encounter == null)
                {
                    bool previousEnabled = GUI.enabled;
                    GUI.enabled = false;
                    GUILayout.Button($"{SlotGlyphs[i]}  No Encounter");
                    GUI.enabled = previousEnabled;
                    continue;
                }

                string suffix = BuildStatusSuffix(encounter, direction, supply);

                if (GUILayout.Button($"{SlotGlyphs[i]}  {encounter.DisplayName}{suffix}"))
                    quickChoices.TrySelectSlot(i);
            }

            GUILayout.EndArea();
        }

        private static string BuildStatusSuffix(
            LevelEncounter encounter,
            EncounterDirectionController direction,
            PrototypeEncounterSupply supply)
        {
            string suffix = string.Empty;

            if (supply != null)
            {
                int occurrence = supply.GetOccurrence(encounter);

                if (occurrence > 1)
                    suffix += $"  x{occurrence}";
            }

            bool selected = direction != null && direction.SelectedEncounter == encounter;

            if (selected)
                return suffix + "  ★ COMMITTED";

            if (supply != null && supply.IsExpiryProtected(encounter))
                return suffix + "  COMMITTED";

            WorldClockState worldClock = RunProgress.WorldClock;

            if (supply == null ||
                worldClock == null ||
                !worldClock.IsInitialized ||
                !supply.TryGetExpiry(encounter, out WorldTimestamp expiry))
            {
                return suffix;
            }

            double remainingWorldMinutes = Math.Max(0d, worldClock.CurrentTime.MinutesUntil(expiry));

            WorldClockSystem clockSystem = WorldClockSystem.Instance;
            float worldMinutesPerSimulationSecond =
                clockSystem != null ? clockSystem.WorldHoursPerSimulationMinute : 1f;

            double remainingSimulationSeconds =
                remainingWorldMinutes / Math.Max(0.01f, worldMinutesPerSimulationSecond);

            return suffix + $"  {FormatCountdown(remainingSimulationSeconds)}";
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