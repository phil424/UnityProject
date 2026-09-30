using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public abstract class CharacterMasteryNodeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Requirements")]
        [SerializeField] private List<CharacterMasteryNodeDefinition> prerequisites = new();
        [SerializeField] private List<PersistentResourceCost> costs = new();

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description;
        public Sprite Icon => icon;

        public IReadOnlyList<CharacterMasteryNodeDefinition> Prerequisites => prerequisites;
        public IReadOnlyList<PersistentResourceCost> Costs => costs;

        public virtual bool IsConfigured => !string.IsNullOrWhiteSpace(Id);

        public abstract string EffectDescription { get; }

        public abstract void ApplyPreparedBuild(RunBuild build);

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}