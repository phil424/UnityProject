using System.Text;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class PreparationCharacterOverviewUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text abilitiesText;

        private readonly StringBuilder abilitiesBuilder = new();
        private PartyMemberDefinition definition;

        public void Bind(PartyMemberDefinition member)
        {
            definition = member;
            Refresh();
        }

        public void Refresh()
        {
            if (definition == null)
            {
                Clear();
                return;
            }

            nameText.text = definition.DisplayName;
            roleText.text = definition.Role;
            descriptionText.text = definition.Description;

            float health = definition.BaseHealth + PersistentProgression.GetHealthBonus(definition);
            float damage = definition.BaseDamage + PersistentProgression.GetDamageBonus(definition);
            float armour = definition.BaseArmour + PersistentProgression.GetArmourBonus(definition);
            float healing = definition.BaseHealing + PersistentProgression.GetHealingBonus(definition);

            statsText.text =
                $"HEALTH   {health:0}\n" +
                $"BASIC DAMAGE   {damage:0.#}\n" +
                $"ARMOUR   {armour:0.#}" +
                (healing > 0f ? $"\nHEALING   {healing:0.#}" : string.Empty);

            abilitiesBuilder.Clear();

            foreach (AbilityLoadoutEntry entry in definition.StartingAbilities)
            {
                if (entry?.Ability == null)
                    continue;

                if (abilitiesBuilder.Length > 0)
                    abilitiesBuilder.Append('\n');

                abilitiesBuilder.Append(entry.Ability.DisplayName);
                abilitiesBuilder.Append("   •   ");
                abilitiesBuilder.Append(entry.Ability.GetCooldown(entry.Level).ToString("0.#"));
                abilitiesBuilder.Append("s");
            }

            abilitiesText.text = abilitiesBuilder.Length > 0
                ? abilitiesBuilder.ToString()
                : "No starting abilities";
        }

        private void Clear()
        {
            nameText.text = string.Empty;
            roleText.text = string.Empty;
            descriptionText.text = string.Empty;
            statsText.text = string.Empty;
            abilitiesText.text = string.Empty;
        }
    }
}