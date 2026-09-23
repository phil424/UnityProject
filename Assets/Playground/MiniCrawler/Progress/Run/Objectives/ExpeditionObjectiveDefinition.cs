using UnityEngine;

namespace MiniCrawler.Progress
{
    public abstract class ExpeditionObjectiveDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;

        [Header("Progress")]
        [SerializeField, Min(1)] private int requiredProgress = 1;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;
        public int RequiredProgress => Mathf.Max(1, requiredProgress);

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            requiredProgress = Mathf.Max(1, requiredProgress);
        }
    }
}