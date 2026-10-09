using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(fileName = "New Character Mastery Definition", menuName = "Mini Crawler/Progression/Character Mastery Definition")]
    public sealed class CharacterMasteryDefinition : ScriptableObject
    {
        [Header("Tree")]
        [SerializeField]
        private Vector2 treeSize = new(1800f, 1000f);

        [SerializeField]
        private List<CharacterMasteryNodeDefinition> nodes = new();

        public Vector2 TreeSize => new(Mathf.Max(800f, treeSize.x), Mathf.Max(600f, treeSize.y));
        public IReadOnlyList<CharacterMasteryNodeDefinition> Nodes => nodes;

        public bool Contains(CharacterMasteryNodeDefinition node)
        {
            return node != null && nodes.Contains(node);
        }
    }
}
