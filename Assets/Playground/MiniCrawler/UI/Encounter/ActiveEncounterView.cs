using MiniCrawler.Encounters;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class ActiveEncounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text encounterNameText;
        [SerializeField] private TMP_Text progressText;

        public void SetEncounter(LevelEncounter encounter)
        {
            if (encounter == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            encounterNameText.text = encounter.DisplayName;

            progressText.text = encounter.PhaseCount > 0
                ? $"Phase {encounter.CurrentPhaseNumber}/{encounter.PhaseCount}"
                : "Active";
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}