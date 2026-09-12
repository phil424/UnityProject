using System;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class QuickEncounterSlotGroup : MonoBehaviour
    {
        [Header("Structure")]
        [SerializeField, Min(1)] private int expectedSlotCount = 3;

        [Header("Presentation")]
        [SerializeField] private Color symbolColor = new(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color textColor = new(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color normalBackgroundColor = Color.white;
        [SerializeField] private Color selectedBackgroundColor = new(0.7f, 0.85f, 1f, 1f);

        private QuickEncounterSlotView[] slots = Array.Empty<QuickEncounterSlotView>();

        public int Count => slots.Length;

        private void Awake()
        {
            RefreshSlots();
            ApplyPresentation();
            ValidateStructure();
        }

        private void OnValidate()
        {
            RefreshSlots();
            ApplyPresentation();
        }

        public void Bind(Action<int> selected)
        {
            RefreshSlots();
            ApplyPresentation();
            ValidateStructure();

            for (int i = 0; i < slots.Length; i++)
                slots[i].Bind(i, selected);
        }

        public QuickEncounterSlotView GetSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < slots.Length ? slots[slotIndex] : null;
        }

        private void RefreshSlots()
        {
            slots = GetComponentsInChildren<QuickEncounterSlotView>(true);

            Array.Sort(slots, (a, b) =>
                a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        }

        private void ApplyPresentation()
        {
            foreach (QuickEncounterSlotView slot in slots)
            {
                slot?.ApplyPresentation(
                    symbolColor,
                    textColor,
                    normalBackgroundColor,
                    selectedBackgroundColor
                );
            }
        }

        private void ValidateStructure()
        {
            if (expectedSlotCount <= 0 || slots.Length == expectedSlotCount)
                return;

            Debug.LogWarning(
                $"{name} expects {expectedSlotCount} QuickEncounterSlotView children but found {slots.Length}.",
                this
            );
        }
    }
}