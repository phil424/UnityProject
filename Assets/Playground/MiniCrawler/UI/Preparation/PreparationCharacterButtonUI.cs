using System;
using MiniCrawler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class PreparationCharacterButtonUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private Button button;
        private PartyMemberDefinition definition;
        private Action<PartyMemberDefinition> selected;

        public PartyMemberDefinition Definition => definition;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (label == null)
                label = GetComponentInChildren<TMP_Text>(true);

            button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Bind(PartyMemberDefinition member, Action<PartyMemberDefinition> onSelected)
        {
            definition = member;
            selected = onSelected;

            if (label != null)
                label.text = definition != null ? definition.DisplayName : "Character";
        }

        public void SetSelected(bool value)
        {
            if (button != null)
                button.interactable = !value;
        }

        private void HandleClicked()
        {
            if (definition != null)
                selected?.Invoke(definition);
        }
    }
}