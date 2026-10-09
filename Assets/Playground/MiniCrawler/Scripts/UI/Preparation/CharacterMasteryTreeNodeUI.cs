using System;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class CharacterMasteryTreeNodeUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Outline selectionOutline;

        [Header("States")]
        [SerializeField] private Color lockedColor = new(0.18f, 0.18f, 0.2f, 0.96f);
        [SerializeField] private Color availableColor = new(0.28f, 0.28f, 0.34f, 0.98f);
        [SerializeField] private Color unlockedColor = new(0.22f, 0.38f, 0.25f, 0.98f);

        private Button button;
        private PartyMemberDefinition character;
        private CharacterMasteryNodeDefinition node;
        private Action<CharacterMasteryNodeDefinition> selected;

        public CharacterMasteryNodeDefinition Node => node;
        public RectTransform RectTransform => transform as RectTransform;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();

            button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Bind(PartyMemberDefinition characterDefinition, CharacterMasteryNodeDefinition nodeDefinition, Action<CharacterMasteryNodeDefinition> onSelected)
        {
            character = characterDefinition;
            node = nodeDefinition;
            selected = onSelected;

            Refresh();
        }

        public void Refresh()
        {
            if (titleText != null)
                titleText.text = node != null ? node.DisplayName : "Mastery";

            if (node == null || character == null)
            {
                if (stateText != null)
                    stateText.text = "UNAVAILABLE";

                if (button != null)
                    button.interactable = false;

                return;
            }

            bool unlocked = PersistentProgression.IsMasteryNodeUnlocked(character, node);
            bool prerequisitesMet = PersistentProgression.AreMasteryPrerequisitesMet(character, node);

            if (stateText != null)
                stateText.text = unlocked ? "UNLOCKED" : prerequisitesMet ? "AVAILABLE" : "LOCKED";

            if (backgroundImage != null)
                backgroundImage.color = unlocked ? unlockedColor : prerequisitesMet ? availableColor : lockedColor;

            if (button != null)
                button.interactable = true;
        }

        public void SetSelected(bool isSelected)
        {
            if (selectionOutline != null)
                selectionOutline.enabled = isSelected;
        }

        private void HandleClicked()
        {
            if (node != null)
                selected?.Invoke(node);
        }
    }
}