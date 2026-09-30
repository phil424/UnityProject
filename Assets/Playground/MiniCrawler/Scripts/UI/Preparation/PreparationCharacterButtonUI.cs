using System;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class PreparationCharacterButtonUI : MonoBehaviour
    {
        [FormerlySerializedAs("label")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text statusText;

        private Button button;
        private PlayableCharacterRosterEntry rosterEntry;
        private PartyMemberDefinition definition;
        private Action<PartyMemberDefinition> selected;

        public PartyMemberDefinition Definition => definition;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (nameText == null)
                nameText = GetComponentInChildren<TMP_Text>(true);

            button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Bind(PlayableCharacterRosterEntry entry, Action<PartyMemberDefinition> onSelected)
        {
            rosterEntry = entry;
            definition = entry?.Character;
            selected = onSelected;

            RefreshPresentation(isSelected: false);
        }

        public void SetState(bool isSelected)
        {
            RefreshPresentation(isSelected);
        }

        private void RefreshPresentation(bool isSelected)
        {
            bool unlocked = PersistentProgression.IsCharacterUnlocked(definition);

            RefreshText(unlocked);

            if (button != null)
                button.interactable = definition != null && unlocked && !isSelected;
        }

        private void RefreshText(bool unlocked)
        {
            if (nameText != null)
            {
                if (definition == null)
                    nameText.text = "Character";
                else
                    nameText.text = unlocked ? definition.DisplayName : $"{definition.DisplayName} [LOCKED]";
            }

            if (statusText == null)
                return;

            string status = GetStatusText(unlocked);

            statusText.text = status;
            statusText.gameObject.SetActive(!string.IsNullOrWhiteSpace(status));
        }

        private string GetStatusText(bool unlocked)
        {
            if (definition == null || rosterEntry == null)
                return string.Empty;

            if (unlocked)
                return rosterEntry.UnlockedByDefault ? string.Empty : "UNLOCKED";

            CharacterUnlockRequirementDefinition requirement = rosterEntry.UnlockRequirement;

            if (requirement == null || !requirement.IsConfigured)
                return "LOCKED";

            int progress = PersistentProgression.GetCharacterUnlockProgress(definition);

            return $"{requirement.DisplayName}  {progress}/{requirement.RequiredProgress}";
        }

        private void HandleClicked()
        {
            if (definition != null && PersistentProgression.IsCharacterUnlocked(definition))
                selected?.Invoke(definition);
        }
    }
}