using System.Text;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionObjectiveTrackerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField, Min(1)] private int maxVisibleObjectives = 3;

        private readonly StringBuilder textBuilder = new();

        private void Awake()
        {
            CacheReferences();

            if (objectiveText == null)
            {
                Debug.LogError($"{nameof(ExpeditionObjectiveTrackerView)} requires a TMP_Text child.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            RunProgress.Changed += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            RunProgress.Changed -= Refresh;
        }

        private void OnValidate()
        {
            CacheReferences();
            maxVisibleObjectives = Mathf.Max(1, maxVisibleObjectives);
        }

        private void Refresh()
        {
            if (objectiveText == null)
                return;

            textBuilder.Clear();
            textBuilder.Append("<b>OBJECTIVES</b>");

            if (!RunProgress.HasActiveRun || RunProgress.Objectives.Count == 0)
            {
                textBuilder.Append("\nNo active objectives.");
                objectiveText.text = textBuilder.ToString();
                return;
            }

            int shown = 0;

            foreach (ExpeditionObjectiveState objective in RunProgress.Objectives)
            {
                if (objective?.Definition == null || shown >= maxVisibleObjectives)
                    continue;

                textBuilder.Append('\n');
                textBuilder.Append(objective.Definition.DisplayName);
                textBuilder.Append("  ");

                if (objective.IsComplete)
                    textBuilder.Append("COMPLETE");
                else
                    textBuilder.Append($"{objective.Progress}/{objective.RequiredProgress}");

                shown++;
            }

            int hidden = RunProgress.Objectives.Count - shown;

            if (hidden > 0)
                textBuilder.Append($"\n+{hidden} more");

            objectiveText.text = textBuilder.ToString();
        }

        private void CacheReferences()
        {
            if (objectiveText == null)
                objectiveText = GetComponentInChildren<TMP_Text>(true);
        }
    }
}