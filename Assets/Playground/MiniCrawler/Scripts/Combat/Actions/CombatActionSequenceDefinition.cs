using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [CreateAssetMenu(
        fileName = "New Combat Action Sequence",
        menuName = "Mini Crawler/Combat/Combat Action Sequence"
    )]
    public class CombatActionSequenceDefinition : ScriptableObject
    {
        [SerializeField] private List<CombatActionDefinition> steps = new();

        public int StepCount => steps.Count;

        public CombatActionDefinition GetStep(int index)
        {
            if (index < 0 || index >= steps.Count)
                return null;

            return steps[index];
        }
    }
}