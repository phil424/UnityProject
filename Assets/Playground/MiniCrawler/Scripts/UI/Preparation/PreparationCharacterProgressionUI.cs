using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class PreparationCharacterProgressionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text masteryHeaderText;
        [SerializeField] private Transform masteryNodeRoot;
        [SerializeField] private CharacterMasteryNodeUI masteryNodePrefab;

        private readonly List<CharacterMasteryNodeUI> nodeViews = new();

        private PartyMemberDefinition definition;
        private bool built;

        public void Bind(PartyMemberDefinition member)
        {
            bool characterChanged = definition != member;

            definition = member;

            if (characterChanged || !built)
                RebuildNodes();

            Refresh();
        }

        public void Refresh()
        {
            if (masteryHeaderText != null)
            {
                masteryHeaderText.text = definition != null
                    ? $"{definition.DisplayName.ToUpperInvariant()} MASTERY"
                    : "CHARACTER MASTERY";
            }

            foreach (CharacterMasteryNodeUI nodeView in nodeViews)
                nodeView?.Refresh();
        }

        private void RebuildNodes()
        {
            foreach (CharacterMasteryNodeUI nodeView in nodeViews)
            {
                if (nodeView != null)
                    Destroy(nodeView.gameObject);
            }

            nodeViews.Clear();
            built = true;

            if (definition == null || masteryNodeRoot == null || masteryNodePrefab == null)
                return;

            foreach (CharacterMasteryNodeDefinition node in definition.MasteryNodes)
            {
                if (node == null)
                    continue;

                CharacterMasteryNodeUI nodeView =
                    Instantiate(masteryNodePrefab, masteryNodeRoot);

                nodeView.Bind(definition, node);
                nodeViews.Add(nodeView);
            }
        }
    }
}