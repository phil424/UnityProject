using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterMasteryTreeConnectionUI : MonoBehaviour
    {
        [SerializeField] private Image lineImage;
        [SerializeField, Min(1f)] private float thickness = 4f;
        [SerializeField] private Color lockedColor = new(0.22f, 0.22f, 0.25f, 0.8f);
        [SerializeField] private Color fulfilledColor = new(0.45f, 0.72f, 0.48f, 0.95f);

        private PartyMemberDefinition character;
        private CharacterMasteryNodeDefinition prerequisite;

        public void Bind(
            PartyMemberDefinition characterDefinition,
            CharacterMasteryNodeDefinition prerequisiteNode,
            RectTransform prerequisiteRect,
            RectTransform targetRect)
        {
            character = characterDefinition;
            prerequisite = prerequisiteNode;

            if (prerequisiteRect != null && targetRect != null)
            {
                Vector2 fromCenter = prerequisiteRect.anchoredPosition;
                Vector2 toCenter = targetRect.anchoredPosition;
                Vector2 delta = toCenter - fromCenter;

                if (delta.sqrMagnitude > 0.0001f)
                {
                    Vector2 direction = delta.normalized;
                    Vector2 from = GetBoundaryPoint(fromCenter, prerequisiteRect.rect.size, direction);
                    Vector2 to = GetBoundaryPoint(toCenter, targetRect.rect.size, -direction);

                    SetEndpoints(from, to);
                }
                else
                {
                    SetEndpoints(fromCenter, toCenter);
                }
            }

            Refresh();
        }

        public void Refresh()
        {
            if (lineImage == null)
                return;

            bool fulfilled =
                character != null &&
                prerequisite != null &&
                PersistentProgression.IsMasteryNodeUnlocked(character, prerequisite);

            lineImage.color = fulfilled ? fulfilledColor : lockedColor;
        }

        private static Vector2 GetBoundaryPoint(Vector2 center, Vector2 size, Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
                return center;

            float xDistance = Mathf.Abs(direction.x) > 0.0001f
                ? size.x * 0.5f / Mathf.Abs(direction.x)
                : float.PositiveInfinity;

            float yDistance = Mathf.Abs(direction.y) > 0.0001f
                ? size.y * 0.5f / Mathf.Abs(direction.y)
                : float.PositiveInfinity;

            return center + direction * Mathf.Min(xDistance, yDistance);
        }

        private void SetEndpoints(Vector2 from, Vector2 to)
        {
            RectTransform rect = transform as RectTransform;

            if (rect == null)
                return;

            Vector2 delta = to - from;
            float distance = delta.magnitude;

            rect.anchoredPosition = (from + to) * 0.5f;
            rect.sizeDelta = new Vector2(distance, thickness);

            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}