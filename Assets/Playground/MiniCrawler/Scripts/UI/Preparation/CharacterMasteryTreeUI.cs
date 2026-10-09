using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterMasteryTreeUI : MonoBehaviour
    {
        private const float MinZoom = 0.6f;
        private const float MaxZoom = 1.4f;
        private const float ZoomStep = 0.1f;

        [Header("Header")]
        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text resourcesText;

        [Header("Canvas")]
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform contentRoot;

        [SerializeField]
        private RectTransform connectionsRoot;

        [SerializeField]
        private RectTransform nodesRoot;

        [Header("Views")]
        [SerializeField]
        private CharacterMasteryTreeNodeUI nodePrefab;

        [SerializeField]
        private CharacterMasteryTreeConnectionUI connectionPrefab;

        [SerializeField]
        private CharacterMasteryDetailsUI detailsUI;

        private readonly List<CharacterMasteryTreeNodeUI> nodeViews = new();
        private readonly List<CharacterMasteryTreeConnectionUI> connectionViews = new();
        private readonly Dictionary<CharacterMasteryNodeDefinition, CharacterMasteryTreeNodeUI> nodeLookup = new();

        private PartyMemberDefinition definition;
        private CharacterMasteryNodeDefinition selectedNode;
        private float zoom = 1f;
        private bool built;

        private void OnEnable()
        {
            PersistentProgression.Changed += HandlePersistentProgressionChanged;
            Refresh();
        }

        private void OnDisable()
        {
            PersistentProgression.Changed -= HandlePersistentProgressionChanged;
        }

        public void Bind(PartyMemberDefinition member)
        {
            bool characterChanged = definition != member;
            definition = member;

            if (characterChanged || !built)
                Rebuild();

            Refresh();
        }

        public void Refresh()
        {
            if (titleText != null)
                titleText.text = definition != null ? $"{definition.DisplayName.ToUpperInvariant()} MASTERY" : "CHARACTER MASTERY";

            if (resourcesText != null)
            {
                resourcesText.text =
                    $"L1 {PersistentProgression.GetResource(PersistentResourceTier.Level1)}   "
                    + $"L2 {PersistentProgression.GetResource(PersistentResourceTier.Level2)}   "
                    + $"L3 {PersistentProgression.GetResource(PersistentResourceTier.Level3)}";
            }

            foreach (CharacterMasteryTreeNodeUI nodeView in nodeViews)
            {
                if (nodeView == null)
                    continue;

                nodeView.Refresh();
                nodeView.SetSelected(nodeView.Node == selectedNode);
            }

            foreach (CharacterMasteryTreeConnectionUI connectionView in connectionViews)
                connectionView?.Refresh();

            detailsUI?.Bind(definition, selectedNode);
        }

        public void ZoomAtScreenPoint(float direction, Vector2 screenPosition, Camera eventCamera)
        {
            if (contentRoot == null || Mathf.Abs(direction) <= 0.001f)
                return;

            float previousZoom = zoom;
            float nextZoom = Mathf.Clamp(zoom + Mathf.Sign(direction) * ZoomStep, MinZoom, MaxZoom);

            if (Mathf.Approximately(previousZoom, nextZoom))
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRoot, screenPosition, eventCamera, out Vector2 localPoint))
            {
                return;
            }

            zoom = nextZoom;
            contentRoot.localScale = Vector3.one * zoom;

            // Keep the point beneath the cursor visually stationary while scaling.
            contentRoot.anchoredPosition += localPoint * (previousZoom - zoom);

            StopScrollMovement();
            Canvas.ForceUpdateCanvases();
        }

        public void PanByScreenDelta(Vector2 screenDelta)
        {
            if (contentRoot == null)
                return;

            Canvas canvas = GetComponentInParent<Canvas>();
            float scaleFactor = canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;

            contentRoot.anchoredPosition += screenDelta / scaleFactor;
            StopScrollMovement();
        }

        private void Rebuild()
        {
            ClearViews();

            built = true;
            selectedNode = null;

            CharacterMasteryDefinition mastery = definition != null ? definition.MasteryDefinition : null;

            if (mastery == null || contentRoot == null || nodesRoot == null || connectionsRoot == null || nodePrefab == null || connectionPrefab == null)
            {
                return;
            }

            contentRoot.sizeDelta = mastery.TreeSize;

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node == null)
                    continue;

                CharacterMasteryTreeNodeUI nodeView = Instantiate(nodePrefab, nodesRoot);

                nodeView.Bind(definition, node, HandleNodeSelected);
                nodeView.RectTransform.anchoredPosition = node.TreePosition;

                nodeViews.Add(nodeView);
                nodeLookup[node] = nodeView;

                if (selectedNode == null)
                    selectedNode = node;
            }

            BuildConnections(mastery);

            zoom = 1f;
            contentRoot.localScale = Vector3.one;

            Canvas.ForceUpdateCanvases();

            if (scrollRect != null)
            {
                scrollRect.StopMovement();
                scrollRect.horizontalNormalizedPosition = 0.5f;
                scrollRect.verticalNormalizedPosition = 0.5f;
                scrollRect.velocity = Vector2.zero;
            }
        }

        private void BuildConnections(CharacterMasteryDefinition mastery)
        {
            if (mastery == null)
                return;

            foreach (CharacterMasteryNodeDefinition node in mastery.Nodes)
            {
                if (node == null || !nodeLookup.TryGetValue(node, out CharacterMasteryTreeNodeUI targetView))
                    continue;

                foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
                {
                    if (prerequisite == null)
                        continue;

                    if (!nodeLookup.TryGetValue(prerequisite, out CharacterMasteryTreeNodeUI prerequisiteView))
                    {
                        Debug.LogWarning(
                            $"Mastery node '{node.DisplayName}' references prerequisite '{prerequisite.DisplayName}' " + $"which is not in '{definition.DisplayName}' Mastery Definition.",
                            definition
                        );

                        continue;
                    }

                    CharacterMasteryTreeConnectionUI connection = Instantiate(connectionPrefab, connectionsRoot);

                    connection.Bind(definition, prerequisite, prerequisiteView.RectTransform, targetView.RectTransform);
                    connectionViews.Add(connection);
                }
            }
        }

        private void HandleNodeSelected(CharacterMasteryNodeDefinition node)
        {
            selectedNode = node;
            Refresh();
        }

        private void HandlePersistentProgressionChanged()
        {
            Refresh();
        }

        private void StopScrollMovement()
        {
            if (scrollRect == null)
                return;

            scrollRect.StopMovement();
            scrollRect.velocity = Vector2.zero;
        }

        private void ClearViews()
        {
            foreach (CharacterMasteryTreeNodeUI nodeView in nodeViews)
            {
                if (nodeView != null)
                    Destroy(nodeView.gameObject);
            }

            foreach (CharacterMasteryTreeConnectionUI connectionView in connectionViews)
            {
                if (connectionView != null)
                    Destroy(connectionView.gameObject);
            }

            nodeViews.Clear();
            connectionViews.Clear();
            nodeLookup.Clear();
        }
    }
}
