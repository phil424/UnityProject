using MiniCrawler.Encounters;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DisallowMultipleComponent]
    public sealed class ExpeditionObjectiveSystem : MonoBehaviour
    {
        private StageDirector stageDirector;

        private void Start()
        {
            stageDirector = StageDirector.Instance;

            if (stageDirector == null)
            {
                Debug.LogError($"{nameof(ExpeditionObjectiveSystem)} requires a StageDirector.", this);
                enabled = false;
                return;
            }

            stageDirector.EncounterCompleted += HandleEncounterCompleted;
        }

        private void OnDestroy()
        {
            if (stageDirector != null)
                stageDirector.EncounterCompleted -= HandleEncounterCompleted;
        }

        private void HandleEncounterCompleted(LevelEncounter encounter)
        {
            if (!RunProgress.HasActiveRun || encounter == null)
                return;

            foreach (ExpeditionObjectiveState objective in RunProgress.Objectives)
            {
                if (!ExpeditionObjectiveMatching.MatchesEncounterCompletion(objective, encounter))
                    continue;

                RunProgress.TryAddObjectiveProgress(objective.Definition, 1);
            }
        }
    }
}