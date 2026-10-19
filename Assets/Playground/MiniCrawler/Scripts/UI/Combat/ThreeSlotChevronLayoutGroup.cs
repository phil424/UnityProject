using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [AddComponentMenu("Layout/Three Slot Chevron Layout Group")]
    public sealed class ThreeSlotChevronLayoutGroup : LayoutGroup
    {
        [SerializeField, Min(1f)]
        private float slotSize = 92f;

        [SerializeField, Min(0f)]
        private float horizontalOffset = 46f;

        [SerializeField, Min(0f)]
        private float verticalOffset = 46f;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float preferredWidth = padding.horizontal + slotSize + horizontalOffset * 2f;

            SetLayoutInputForAxis(preferredWidth, preferredWidth, -1f, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float preferredHeight = padding.vertical + slotSize + verticalOffset;

            SetLayoutInputForAxis(preferredHeight, preferredHeight, -1f, 1);
        }

        public override void SetLayoutHorizontal()
        {
            float availableWidth = Mathf.Max(0f, rectTransform.rect.width - padding.horizontal);

            float centerX = padding.left + availableWidth * 0.5f;

            SetChildHorizontal(0, centerX);
            SetChildHorizontal(1, centerX - horizontalOffset);
            SetChildHorizontal(2, centerX + horizontalOffset);
        }

        public override void SetLayoutVertical()
        {
            float topCenterY = padding.top + slotSize * 0.5f;

            float bottomCenterY = topCenterY + verticalOffset;

            SetChildVertical(0, topCenterY);
            SetChildVertical(1, bottomCenterY);
            SetChildVertical(2, bottomCenterY);
        }

        private void SetChildHorizontal(int index, float centerX)
        {
            if (index < 0 || index >= rectChildren.Count)
                return;

            SetChildAlongAxis(rectChildren[index], 0, centerX - slotSize * 0.5f, slotSize);
        }

        private void SetChildVertical(int index, float centerY)
        {
            if (index < 0 || index >= rectChildren.Count)
                return;

            SetChildAlongAxis(rectChildren[index], 1, centerY - slotSize * 0.5f, slotSize);
        }
    }
}
