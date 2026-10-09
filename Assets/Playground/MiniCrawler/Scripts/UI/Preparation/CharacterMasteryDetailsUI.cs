using System.Text;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class CharacterMasteryDetailsUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text prerequisitesText;

        [SerializeField]
        private TMP_Text costText;

        [SerializeField]
        private Button unlockButton;

        [SerializeField]
        private TMP_Text unlockButtonText;

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

        public void Bind(PartyMemberDefinition characterDefinition, CharacterMasteryNodeDefinition nodeDefinition)
        {
            character = characterDefinition;
            node = nodeDefinition;

            Refresh();
        }

        public void Refresh()
        {
            if (node == null || character == null)
            {
                ShowEmpty();
                return;
            }

            if (titleText != null)
                titleText.text = node.DisplayName;

            if (descriptionText != null)
                descriptionText.text = string.IsNullOrWhiteSpace(node.Description) ? node.EffectDescription : $"{node.Description}\n\n{node.EffectDescription}";

            if (prerequisitesText != null)
                prerequisitesText.text = BuildPrerequisiteText();

            bool unlocked = PersistentProgression.IsMasteryNodeUnlocked(character, node);
            bool prerequisitesMet = PersistentProgression.AreMasteryPrerequisitesMet(character, node);

            if (unlocked)
            {
                if (costText != null)
                    costText.text = "PURCHASED";

                if (unlockButtonText != null)
                    unlockButtonText.text = "UNLOCKED";

                if (unlockButton != null)
                    unlockButton.interactable = false;

                return;
            }

            if (costText != null)
                costText.text = $"COST   {PersistentResourcePresentation.FormatCosts(node.Costs)}";

            if (unlockButtonText != null)
                unlockButtonText.text = prerequisitesMet ? "UNLOCK" : "LOCKED";

            if (unlockButton != null)
                unlockButton.interactable = PersistentProgression.CanUnlockMasteryNode(character, node);
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
            if (node.Prerequisites.Count == 0)
                return "ROOT MASTERY";

            StringBuilder builder = new("PREREQUISITES");

            foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
            {
                if (prerequisite == null)
                    continue;

                bool unlocked = PersistentProgression.IsMasteryNodeUnlocked(character, prerequisite);

                builder.Append('\n');
                builder.Append(unlocked ? "[X] " : "[ ] ");
                builder.Append(prerequisite.DisplayName);
            }

            return builder.ToString();
        }

        private void ShowEmpty()
        {
            if (titleText != null)
                titleText.text = "SELECT A MASTERY";

            if (descriptionText != null)
                descriptionText.text = "Select a node on the Mastery tree to inspect it.";

            if (prerequisitesText != null)
                prerequisitesText.text = string.Empty;

            if (costText != null)
                costText.text = string.Empty;

            if (unlockButtonText != null)
                unlockButtonText.text = "UNAVAILABLE";

            if (unlockButton != null)
                unlockButton.interactable = false;
        }
    }
}
