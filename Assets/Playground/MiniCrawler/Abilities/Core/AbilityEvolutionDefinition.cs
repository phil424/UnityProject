using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [CreateAssetMenu(
        fileName = "New Ability Evolution",
        menuName =
            "Mini Crawler/Ability Evolution"
    )]
    public class AbilityEvolutionDefinition :
        ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id;

        [SerializeField]
        private string displayName;

        [SerializeField]
        [TextArea(2, 5)]
        private string description;

        [SerializeField]
        private Sprite icon;

        [Header("Ability")]
        [SerializeField]
        private AbilityDefinition targetAbility;

        [Header("Progression")]
        [SerializeField]
        private List<AbilityEvolutionDefinition>
            requiredEvolutions = new();

        [Header("Presentation")]
        [SerializeField]
        private bool replacesAbilityPresentation;

        public string Id =>
            string.IsNullOrWhiteSpace(id)
                ? name
                : id;

        public string DisplayName =>
            string.IsNullOrWhiteSpace(
                displayName
            )
                ? name
                : displayName;

        public string Description =>
            description ?? string.Empty;

        public Sprite Icon =>
            icon;

        public AbilityDefinition TargetAbility =>
            targetAbility;

        public IReadOnlyList<
            AbilityEvolutionDefinition
        > RequiredEvolutions =>
            requiredEvolutions;

        public bool ReplacesAbilityPresentation =>
            replacesAbilityPresentation;

        public bool IsConfigured =>
            targetAbility != null &&
            HasValidPrerequisites();

        private bool HasValidPrerequisites()
        {
            foreach (
                AbilityEvolutionDefinition required
                    in requiredEvolutions
            )
            {
                if (
                    required == null ||
                    required == this ||
                    required.TargetAbility == null ||
                    required.TargetAbility.Id !=
                        targetAbility.Id
                )
                {
                    return false;
                }
            }

            return true;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}