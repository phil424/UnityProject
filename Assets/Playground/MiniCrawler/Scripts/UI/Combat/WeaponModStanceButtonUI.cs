using System;
using MiniCrawler.Progress;
using MiniCrawler.WeaponMods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class WeaponModStanceButtonUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text labelText;

        private Button button;
        private WeaponModStanceSlot stanceSlot;
        private RunWeaponModState state;
        private Action<RunWeaponModState> selected;

        public int SlotIndex => (int)stanceSlot;
        public RunWeaponModState State => state;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Bind(
            WeaponModStanceSlot slot,
            RunWeaponModState modState,
            bool isActive,
            Action<RunWeaponModState> onSelected)
        {
            stanceSlot = slot;
            state = modState;
            selected = onSelected;

            Refresh(isActive);
        }

        private void Refresh(bool isActive)
        {
            if (button == null)
                return;

            bool isCore = stanceSlot == WeaponModStanceSlot.Core;

            if (state?.Definition == null)
            {
                button.interactable = false;

                if (labelText != null)
                {
                    labelText.text = isCore
                        ? "1. CORE MASTERY\nLOCKED"
                        : $"{SlotIndex + 1}. MASTERY SLOT\nUNAVAILABLE";
                }

                return;
            }

            button.interactable = !isCore || !isActive;

            if (labelText != null)
            {
                string status;

                if (isCore)
                    status = isActive ? "DEFAULT" : "CORE";
                else
                    status = isActive ? "ACTIVE" : "READY";

                labelText.text =
                    $"{SlotIndex + 1}. {state.Definition.DisplayName}\n" +
                    $"Lv.{state.Level} - {status}";
            }
        }

        private void HandleClicked()
        {
            if (state?.Definition != null)
                selected?.Invoke(state);
        }
    }
}