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

        public bool IsConfigured =>
            targetAbility != null;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}