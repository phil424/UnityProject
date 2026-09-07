using MiniCrawler.Combat;
using MiniCrawler.Encounters;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Systems
{
    public class PrototypeDebugUI : MonoBehaviour
    {
        private const float Margin = 10f;

        private const float ButtonWidth = 90f;
        private const float ButtonHeight = 30f;

        private const float PanelWidth = 420f;
        private const float ClosedLogPanelHeight = 700f;
        private const float OpenLogPanelHeight = 860f;

        private const float PanelSpacing = 5f;

        private bool showDebugPanel;
        private bool showCombatLog;

        private Vector2 combatLogScrollPosition;

        private void OnGUI()
        {
            DrawToggleButton();

            if (!showDebugPanel)
                return;

            DrawDebugPanel();
        }

        private void DrawToggleButton()
        {
            Rect buttonRect = new Rect( Screen.width - ButtonWidth - Margin, Margin, ButtonWidth, ButtonHeight);

            if (GUI.Button(buttonRect, showDebugPanel ? "DEBUG ▲" : "DEBUG ▼"))
                showDebugPanel = !showDebugPanel;
        }

        private void DrawDebugPanel()
        {
            float availableWidth = Screen.width - (Margin * 2f);

            float width = Mathf.Min(PanelWidth, availableWidth);

            float panelTop = Margin + ButtonHeight + PanelSpacing;

            float desiredHeight = showCombatLog ? OpenLogPanelHeight : ClosedLogPanelHeight;

            float availableHeight = Screen.height - panelTop - Margin;

            float height = Mathf.Min(desiredHeight, availableHeight);

            Rect panelRect = new Rect(Screen.width - width - Margin, panelTop, width, height);

            GUILayout.BeginArea(panelRect, GUI.skin.box);

            RunDirector run = RunDirector.Instance;

            StageDirector stage = StageDirector.Instance;

            GUILayout.Label("Mini Crawler Prototype");

            GUILayout.Label($"Persistent Currency: {PersistentProgression.Currency}");
            GUILayout.Label($"Expedition Currency: {RunProgress.Currency}");

            GUILayout.Label($"Run Active: " + $"{(RunProgress.HasActiveRun ? "Yes" : "No")}");
            
            if (!RunProgress.HasActiveRun && run != null && run.LastExpeditionEndReason != RunDirector.ExpeditionEndReason.None)
            {
                GUILayout.Label($"Last Expedition: {run.LastExpeditionEndReason}");
                GUILayout.Label($"Persistent Currency Banked: {run.LastPersistentCurrencyBanked}");
            }

            if (run != null)
            {
                GUILayout.Label($"Setup Party: " + $"{run.Setup.SelectedParty.Count}/" + $"{run.Setup.MaximumPartySize}");

                if (RunProgress.HasActiveRun)
                    GUILayout.Label($"Run Party: " + $"{RunProgress.SelectedParty.Count}");

                GUILayout.Space(10);

                GUILayout.Label($"Run Flow: {run.StateName}");

                if (!RunProgress.HasActiveRun)
                {
                    if (GUILayout.Button("DEBUG: Begin Run"))
                        run.BeginRun();
                }
                else
                {
                    if (run.State == RunDirector.RunFlowState.BetweenLevels)
                        if (GUILayout.Button("DEBUG: Continue Run"))
                            run.ContinueRun();

                    if (GUILayout.Button("DEBUG: End Run"))
                        run.EndRun();
                }
            }
            else
            {
                GUILayout.Label("RunDirector: Missing");
            }

            GUILayout.Space(10);

            if (stage != null)
            {
                GUILayout.Label($"Level State: {stage.StateName}");
                GUILayout.Label($"Party Alive: " + $"{stage.LivingPartyMembers}");
                GUILayout.Label($"Minions Alive: " + $"{stage.LivingMinions}");
            }
            else
            {
                GUILayout.Label("StageDirector: Missing");
            }
            
            DrawWorldClock();

            DrawEncounterDirection(stage);
            DrawCombatTelemetry();

            GUILayout.EndArea();
        }
        
        private void DrawWorldClock()
        {
            GUILayout.Space(10);
            GUILayout.Label("World Clock");

            WorldClockState worldClock = RunProgress.WorldClock;

            if (worldClock == null)
            {
                GUILayout.Label("No active run.");
                return;
            }

            if (!worldClock.IsInitialized)
            {
                GUILayout.Label("Waiting for clock initialization.");
                return;
            }

            WorldTimestamp currentTime = worldClock.CurrentTime;

            GUILayout.Label($"World Time: Day {currentTime.DayNumber}  {currentTime.ClockText}");
            GUILayout.Label($"Expedition Elapsed: {worldClock.ExpeditionElapsedSimulationMinutes:0.0} sim min");

            WorldClockSystem system = WorldClockSystem.Instance;

            if (system == null)
            {
                GUILayout.Label("WorldClockSystem: Missing");
                return;
            }

            GUILayout.Label($"Clock Rate: {system.WorldHoursPerSimulationMinute:0.##} world h / sim min");
            GUILayout.Label($"World Day Length: {system.SimulationMinutesPerWorldDay:0.#} sim min");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("0.5 h/min"))
                system.SetWorldHoursPerSimulationMinute(0.5f);

            if (GUILayout.Button("1 h/min"))
                system.SetWorldHoursPerSimulationMinute(1f);

            if (GUILayout.Button("2 h/min"))
                system.SetWorldHoursPerSimulationMinute(2f);

            GUILayout.EndHorizontal();
        }
        
        private void DrawEncounterDirection(StageDirector stage)
        {
            GUILayout.Space(10);
            GUILayout.Label("Encounter Direction");

            EncounterDirectionController direction = EncounterDirectionController.Instance;

            if (direction == null)
            {
                GUILayout.Label("EncounterDirectionController: Missing");
                return;
            }

            if (direction.SelectedEncounter != null)
            {
                string state = direction.IsTravelling ? "Travelling" : "Engaged";
                GUILayout.Label($"Selected: {direction.SelectedEncounter.DisplayName} ({state})");
            }
            else
            {
                GUILayout.Label("Selected: None");
            }

            if (stage == null || stage.Encounters.Count == 0)
            {
                GUILayout.Label("No encounters available.");
                return;
            }

            foreach (LevelEncounter encounter in stage.Encounters)
            {
                if (encounter == null)
                    continue;

                bool previousEnabled = GUI.enabled;
                GUI.enabled = encounter.IsSelectable;

                string selectedPrefix = direction.SelectedEncounter == encounter ? "► " : string.Empty;
                string label = $"{selectedPrefix}{encounter.DisplayName} [{encounter.PresentationState}]";

                if (GUILayout.Button(label))
                    direction.SelectEncounter(encounter);

                GUI.enabled = previousEnabled;
            }

            if (direction.SelectedEncounter != null && GUILayout.Button("Clear Encounter Direction"))
                direction.ClearSelection();
        }

        private void DrawCombatTelemetry()
        {
            GUILayout.Space(10);

            GUILayout.Label("Combat Telemetry");

            CombatTelemetry telemetry = CombatTelemetry.Instance;

            if (telemetry == null)
            {
                GUILayout.Label("CombatTelemetry: Missing");
                return;
            }

            GUILayout.Label($"Party DPS: " + $"{telemetry.OutgoingDps:0.0}");

            GUILayout.Label($"Incoming DPS: " + $"{telemetry.IncomingDps:0.0}");

            if (GUILayout.Button(showCombatLog ? "Hide Combat Log" : "Show Combat Log"))
                showCombatLog = !showCombatLog;

            if (!showCombatLog)
                return;

            GUILayout.Space(5);

            GUILayout.Label("Detailed Combat Log");

            combatLogScrollPosition = GUILayout.BeginScrollView(combatLogScrollPosition);

            if (telemetry.CombatLogCount <= 0)
            {
                GUILayout.Label("No damage events recorded.");
            }
            else
            {
                foreach (string entry in telemetry.CombatLog)
                {
                    GUILayout.Label(entry);
                    GUILayout.Space(5);
                }
            }

            GUILayout.EndScrollView();
        }
    }
}