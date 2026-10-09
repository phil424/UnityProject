using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public sealed class CharacterMasteryTreeViewportInput :
        MonoBehaviour,
        IScrollHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private CharacterMasteryTreeUI treeUI;

        private ScrollRect scrollRect;
        private bool middleMouseDragging;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();

            // Scroll wheel belongs to zoom.
            // ScrollRect still owns normal left-drag panning.
            scrollRect.scrollSensitivity = 0f;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (treeUI == null || Mathf.Abs(eventData.scrollDelta.y) <= 0.001f)
                return;

            treeUI.ZoomAtScreenPoint(
                eventData.scrollDelta.y,
                eventData.position,
                eventData.pressEventCamera
            );

            eventData.Use();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Middle)
                return;

            middleMouseDragging = true;
            eventData.Use();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!middleMouseDragging ||
                eventData.button != PointerEventData.InputButton.Middle)
            {
                return;
            }

            treeUI?.PanByScreenDelta(eventData.delta);

            eventData.Use();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Middle)
                return;

            middleMouseDragging = false;
            eventData.Use();
        }
    }
}