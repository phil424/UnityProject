using UnityEngine;

namespace MiniCrawler.Movement
{
    [CreateAssetMenu(fileName = "New Route Tag", menuName = "Mini Crawler/Route Geography/Route Tag")]
    public sealed class RouteTagDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;
    }
}