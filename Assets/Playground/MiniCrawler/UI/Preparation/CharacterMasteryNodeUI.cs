using System.Text;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterMasteryNodeUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button unlockButton;
        [SerializeField] private TMP_Text unlockButtonText;

        private PartyMemberDefinition character;
        private CharacterMasteryNodeDefinition node;

        private void Awake()
        {
            if (unlockButton != null)
                unlockButton.onClick.AddListener(HandleUnlockClicked);
        }

        private void OnDestroy()
        {
            if (unlockButton != null)
                unlockButton.onClick.RemoveListener(HandleUnlockClicked);
        }

        public void Bind(
            PartyMemberDefinition characterDefinition,
            CharacterMasteryNodeDefinition nodeDefinition)
        {
            character = characterDefinition;
            node = nodeDefinition;
            Refresh();
        }

        public void Refresh()
        {
            if (character == null || node == null)
                return;

            if (titleText != null)
                titleText.text = node.DisplayName;

            if (descriptionText != null)
            {
                descriptionText.text = string.IsNullOrWhiteSpace(node.Description)
                    ? node.EffectDescription
                    : $"{node.Description}\n{node.EffectDescription}";
            }

            bool unlocked =
                PersistentProgression.IsMasteryNodeUnlocked(character, node);

            bool prerequisitesMet =
                PersistentProgression.AreMasteryPrerequisitesMet(character, node);

            if (unlocked)
            {
                if (costText != null)
                    costText.text = "UNLOCKED";

                if (unlockButtonText != null)
                    unlockButtonText.text = "UNLOCKED";

                if (unlockButton != null)
                    unlockButton.interactable = false;

                return;
            }

            if (!prerequisitesMet)
            {
                if (costText != null)
                    costText.text = BuildPrerequisiteText();

                if (unlockButtonText != null)
                    unlockButtonText.text = "LOCKED";

                if (unlockButton != null)
                    unlockButton.interactable = false;

                return;
            }

            if (costText != null)
                costText.text = $"COST   {PersistentResourcePresentation.FormatCosts(node.Costs)}";

            if (unlockButtonText != null)
                unlockButtonText.text = "UNLOCK";

            if (unlockButton != null)
                unlockButton.interactable =
                    PersistentProgression.CanUnlockMasteryNode(character, node);
        }

        private void HandleUnlockClicked()
        {
            if (character == null || node == null)
                return;

            PersistentProgression.TryUnlockMasteryNode(character, node);
            Refresh();
        }

        private string BuildPrerequisiteText()
        {
            StringBuilder builder = new("REQUIRES   ");

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null)
                    continue;

                if (builder.Length > "REQUIRES   ".Length)
                    builder.Append(", ");

                builder.Append(prerequisite.DisplayName);
            }

            return builder.ToString();
        }
    }
}