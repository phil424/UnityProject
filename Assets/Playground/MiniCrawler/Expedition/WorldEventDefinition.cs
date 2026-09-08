using UnityEngine;

namespace MiniCrawler.Expedition
{
    [CreateAssetMenu(fileName = "New World Event", menuName = "Mini Crawler/World Events/World Event")]
    public sealed class WorldEventDefinition : ScriptableObject
    {
        [SerializeField] private string id = "world-event";
        [SerializeField] private string displayName = "World Event";
        [SerializeField, TextArea] private string description;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id.Trim();
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description;
    }
}