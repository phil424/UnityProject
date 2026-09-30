using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Objective Character Unlock Requirement",
        menuName = "Mini Crawler/Progression/Character Unlock/Objective Completion"
    )]
    public sealed class ObjectiveCompletionCharacterUnlockRequirementDefinition : CharacterUnlockRequirementDefinition
    {
        [Header("Objective")]
        [SerializeField] private ExpeditionObjectiveDefinition objective;

        public ExpeditionObjectiveDefinition Objective => objective;

        public override bool IsConfigured => base.IsConfigured && objective != null;

        public override int GetProgressContribution(RunState runState)
        {
            if (runState == null || objective == null)
                return 0;

            foreach (ExpeditionObjectiveState state in runState.Objectives)
            {
                if (state?.Definition == objective && state.IsComplete)
                    return 1;
            }

            return 0;
        }
    }
}