using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class QuickEncounterSlotView : MonoBehaviour
    {
        [SerializeField] private QuickSlotSymbolGraphic symbol;
        [SerializeField] private TMP_Text label;

        private Button button;
        private Image background;

        private Color normalBackgroundColor = Color.white;
        private Color selectedBackgroundColor = Color.white;
        private bool isSelected;

        public int SlotIndex { get; private set; }
        public RectTransform RectTransform => transform as RectTransform;

        private void Awake()
        {
            CacheReferences();
        }

        private void OnValidate()
        {
            CacheReferences();
        }

        public void Bind(int slotIndex, Action<int> selected)
        {
            CacheReferences();

            SlotIndex = slotIndex;
            symbol?.SetSlotIndex(slotIndex);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => selected?.Invoke(SlotIndex));
        }

        public void ApplyPresentation(
            Color symbolColor,
            Color textColor,
            Color normalBackground,
            Color selectedBackground)
        {
            CacheReferences();

            normalBackgroundColor = normalBackground;
            selectedBackgroundColor = selectedBackground;

            if (symbol != null)
                symbol.color = symbolColor;

            if (label != null)
                label.color = textColor;

            RefreshBackground();
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetLabel(string value)
        {
            if (label != null)
                label.text = value;
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            RefreshBackground();
        }

        private void RefreshBackground()
        {
            if (background != null)
            {
                background.color = isSelected
                    ? selectedBackgroundColor
                    : normalBackgroundColor;
            }
        }

        private void CacheReferences()
        {
            button = GetComponent<Button>();
            background = GetComponent<Image>();

            if (symbol == null)
                symbol = GetComponentInChildren<QuickSlotSymbolGraphic>(true);

            if (label == null)
                label = GetComponentInChildren<TMP_Text>(true);
        }
    }
}