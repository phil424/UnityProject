using MiniCrawler.Combat;
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
        private const float ClosedLogPanelHeight = 440f;
        private const float OpenLogPanelHeight = 650f;

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
            Rect buttonRect =
                new Rect(
                    Screen.width -
                    ButtonWidth -
                    Margin,
                    Margin,
                    ButtonWidth,
                    ButtonHeight
                );

            if (
                GUI.Button(
                    buttonRect,
                    showDebugPanel
                        ? "DEBUG ▲"
                        : "DEBUG ▼"
                )
            )
            {
                showDebugPanel =
                    !showDebugPanel;
            }
        }

        private void DrawDebugPanel()
        {
            float availableWidth =
                Screen.width -
                (Margin * 2f);

            float width =
                Mathf.Min(
                    PanelWidth,
                    availableWidth
                );

            float panelTop =
                Margin +
                ButtonHeight +
                PanelSpacing;

            float desiredHeight =
                showCombatLog
                    ? OpenLogPanelHeight
                    : ClosedLogPanelHeight;

            float availableHeight =
                Screen.height -
                panelTop -
                Margin;

            float height =
                Mathf.Min(
                    desiredHeight,
                    availableHeight
                );

            Rect panelRect =
                new Rect(
                    Screen.width -
                    width -
                    Margin,
                    panelTop,
                    width,
                    height
                );

            GUILayout.BeginArea(
                panelRect,
                GUI.skin.box
            );

            RunDirector run =
                RunDirector.Instance;

            StageDirector stage =
                StageDirector.Instance;

            GUILayout.Label(
                "Mini Crawler Prototype"
            );

            GUILayout.Label(
                $"Run Currency: {RunProgress.Currency}"
            );

            GUILayout.Label(
                $"Run Active: " +
                $"{(RunProgress.HasActiveRun ? "Yes" : "No")}"
            );

            if (run != null)
            {
                GUILayout.Label(
                    $"Setup Party: " +
                    $"{run.Setup.SelectedParty.Count}/" +
                    $"{RunSetup.MaximumPartySize}"
                );

                if (RunProgress.HasActiveRun)
                {
                    GUILayout.Label(
                        $"Run Party: " +
                        $"{RunProgress.SelectedParty.Count}"
                    );
                }

                GUILayout.Space(10);

                GUILayout.Label(
                    $"Run Flow: {run.StateName}"
                );

                if (!RunProgress.HasActiveRun)
                {
                    if (
                        GUILayout.Button(
                            "DEBUG: Begin Run"
                        )
                    )
                    {
                        run.BeginRun();
                    }
                }
                else
                {
                    if (
                        run.State ==
                        RunDirector.RunFlowState.BetweenLevels
                    )
                    {
                        if (
                            GUILayout.Button(
                                "DEBUG: Continue Run"
                            )
                        )
                        {
                            run.ContinueRun();
                        }
                    }

                    if (
                        GUILayout.Button(
                            "DEBUG: End Run"
                        )
                    )
                    {
                        run.EndRun();
                    }
                }
            }
            else
            {
                GUILayout.Label(
                    "RunDirector: Missing"
                );
            }

            GUILayout.Space(10);

            if (stage != null)
            {
                GUILayout.Label(
                    $"Level State: {stage.StateName}"
                );

                GUILayout.Label(
                    $"Party Alive: " +
                    $"{stage.LivingPartyMembers}"
                );

                GUILayout.Label(
                    $"Minions Alive: " +
                    $"{stage.LivingMinions}"
                );
            }
            else
            {
                GUILayout.Label(
                    "StageDirector: Missing"
                );
            }

            DrawCombatTelemetry();

            GUILayout.EndArea();
        }

        private void DrawCombatTelemetry()
        {
            GUILayout.Space(10);

            GUILayout.Label(
                "Combat Telemetry"
            );

            CombatTelemetry telemetry =
                CombatTelemetry.Instance;

            if (telemetry == null)
            {
                GUILayout.Label(
                    "CombatTelemetry: Missing"
                );

                return;
            }

            GUILayout.Label(
                $"Party DPS: " +
                $"{telemetry.OutgoingDps:0.0}"
            );

            GUILayout.Label(
                $"Incoming DPS: " +
                $"{telemetry.IncomingDps:0.0}"
            );

            if (
                GUILayout.Button(
                    showCombatLog
                        ? "Hide Combat Log"
                        : "Show Combat Log"
                )
            )
            {
                showCombatLog =
                    !showCombatLog;
            }

            if (!showCombatLog)
                return;

            GUILayout.Space(5);

            GUILayout.Label(
                "Detailed Combat Log"
            );

            combatLogScrollPosition =
                GUILayout.BeginScrollView(
                    combatLogScrollPosition
                );

            if (telemetry.CombatLogCount <= 0)
            {
                GUILayout.Label(
                    "No damage events recorded."
                );
            }
            else
            {
                foreach (
                    string entry
                    in telemetry.CombatLog
                )
                {
                    GUILayout.Label(entry);
                    GUILayout.Space(5);
                }
            }

            GUILayout.EndScrollView();
        }
    }
}