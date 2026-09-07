using MiniCrawler.Encounters;
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
        private const float PanelWidth = 300f;
        private const float PanelHeight = 155f;

        private static readonly string[] SlotGlyphs =
        {
            "△",
            "□",
            "○"
        };

        private void OnGUI()
        {
            RunDirector runDirector = RunDirector.Instance;

            if (!RunProgress.HasActiveRun ||
                runDirector == null ||
                runDirector.State != RunDirector.RunFlowState.InLevel)
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

                bool selected = direction != null && direction.SelectedEncounter == encounter;
                string selectedMarker = selected ? "  ★" : string.Empty;

                if (GUILayout.Button($"{SlotGlyphs[i]}  {encounter.DisplayName}{selectedMarker}"))
                    quickChoices.TrySelectSlot(i);
            }

            GUILayout.EndArea();
        }
    }
}