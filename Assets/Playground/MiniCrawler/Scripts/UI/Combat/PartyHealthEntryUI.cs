using MiniCrawler.Core;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    public class PartyHealthEntryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TMP_Text nameText;

        [SerializeField]
        private TMP_Text healthText;

        [SerializeField]
        private RectTransform healthFillRect;

        private Health health;

        public void Bind(
            PartyMemberDefinition definition,
            GameObject actor
        )
        {
            UnbindHealth();

            if (nameText != null)
            {
                nameText.text =
                    definition != null
                        ? definition.DisplayName
                        : "Unknown";
            }

            health =
                actor != null
                    ? actor.GetComponent<Health>()
                    : null;

            if (health != null)
            {
                health.Changed +=
                    HandleHealthChanged;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            UnbindHealth();
        }

        private void HandleHealthChanged(
            Health changedHealth
        )
        {
            Refresh();
        }

        private void Refresh()
        {
            if (
                healthText == null ||
                healthFillRect == null
            )
            {
                return;
            }

            float normalized =
                health != null
                    ? health.Normalized
                    : 0f;

            float current =
                health != null
                    ? health.CurrentHealth
                    : 0f;

            float maximum =
                health != null
                    ? health.MaxHealth
                    : 0f;

            healthText.text =
                $"{current:0.#} / {maximum:0.#}";

            Vector2 anchorMax =
                healthFillRect.anchorMax;

            anchorMax.x =
                normalized;

            healthFillRect.anchorMax =
                anchorMax;
        }

        private void UnbindHealth()
        {
            if (health != null)
            {
                health.Changed -=
                    HandleHealthChanged;
            }

            health = null;
        }
    }
}