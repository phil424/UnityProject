using System;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class StrategicMinimapView : MonoBehaviour
    {
        [SerializeField] private QuickEncounterSlotGroup encounterMarkers;
        [SerializeField] private RectTransform partyMarker;
        [SerializeField] private RectTransform selectedDirectiveMarker;

        [Header("Prototype Map Bounds")]
        [SerializeField] private Vector2 worldMin = new(-25f, -25f);
        [SerializeField] private Vector2 worldMax = new(25f, 25f);

        private RectTransform rectTransform;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnValidate()
        {
            CacheReferences();
        }

        public void Bind(Action<int> selected)
        {
            CacheReferences();
            encounterMarkers?.Bind(selected);
        }

        public QuickEncounterSlotView GetEncounterMarker(int slotIndex)
        {
            return encounterMarkers?.GetSlot(slotIndex);
        }

        public void SetEncounterMarker(int slotIndex, bool visible, bool selected, Vector3 worldPosition)
        {
            QuickEncounterSlotView marker = GetEncounterMarker(slotIndex);

            if (marker == null)
                return;

            marker.SetVisible(visible);

            if (!visible)
                return;

            marker.SetSelected(selected);
            PositionMarker(marker.RectTransform, worldPosition);
        }

        public void SetPartyMarker(bool visible, Vector3 worldPosition)
        {
            if (partyMarker == null)
                return;

            partyMarker.gameObject.SetActive(visible);

            if (visible)
                PositionMarker(partyMarker, worldPosition);
        }

        public void SetSelectedDirectiveMarker(bool visible, Vector3 worldPosition)
        {
            if (selectedDirectiveMarker == null)
                return;

            selectedDirectiveMarker.gameObject.SetActive(visible);

            if (visible)
                PositionMarker(selectedDirectiveMarker, worldPosition);
        }

        private void PositionMarker(RectTransform marker, Vector3 worldPosition)
        {
            if (marker == null || rectTransform == null)
                return;

            float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPosition.x);
            float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPosition.z);

            Rect rect = rectTransform.rect;

            marker.anchoredPosition = new Vector2(
                (normalizedX - 0.5f) * rect.width,
                (normalizedY - 0.5f) * rect.height
            );
        }

        private void CacheReferences()
        {
            rectTransform = transform as RectTransform;

            if (encounterMarkers == null)
                encounterMarkers = GetComponentInChildren<QuickEncounterSlotGroup>(true);
        }
    }
}