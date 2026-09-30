using UnityEngine;

namespace MiniCrawler.Core
{
    [CreateAssetMenu(
        fileName = "New Actor Definition",
        menuName = "Mini Crawler/Actor Definition"
    )]
    public class ActorDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private GameObject prefab;

        public string Id => id;
        public GameObject Prefab => prefab;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}