using MiniCrawler.Combat;
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

        [SerializeField]
        private TMP_Text shieldText;

        private Health health;
        private HitShieldRuntime shields;

        public void Bind(PartyMemberDefinition definition, GameObject actor)
        {
            UnbindActorState();

            if (nameText != null)
            {
                nameText.text = definition != null ? definition.DisplayName : "Unknown";
            }

            health = actor != null ? actor.GetComponent<Health>() : null;

            shields = actor != null ? actor.GetComponent<HitShieldRuntime>() : null;

            if (health != null)
                health.Changed += HandleHealthChanged;

            if (shields != null)
                shields.Changed += HandleShieldsChanged;

            Refresh();
        }

        private void OnDestroy()
        {
            UnbindActorState();
        }

        private void HandleHealthChanged(Health changedHealth)
        {
            RefreshHealth();
        }

        private void HandleShieldsChanged(HitShieldRuntime changedShields)
        {
            RefreshShields();
        }

        private void Refresh()
        {
            RefreshHealth();
            RefreshShields();
        }

        private void RefreshHealth()
        {
            if (healthText == null || healthFillRect == null)
            {
                return;
            }

            float normalized = health != null ? health.Normalized : 0f;

            float current = health != null ? health.CurrentHealth : 0f;

            float maximum = health != null ? health.MaxHealth : 0f;

            healthText.text = $"{current:0.#} / {maximum:0.#}";

            Vector2 anchorMax = healthFillRect.anchorMax;

            anchorMax.x = normalized;

            healthFillRect.anchorMax = anchorMax;
        }

        private void RefreshShields()
        {
            if (shieldText == null)
                return;

            bool showShields = shields != null && shields.Capacity > 0 && shields.Capacity < int.MaxValue;

            shieldText.gameObject.SetActive(showShields);

            if (!showShields)
                return;

            shieldText.text = $"Shields {shields.ShieldCount} / {shields.Capacity}";
        }

        private void UnbindActorState()
        {
            if (health != null)
                health.Changed -= HandleHealthChanged;

            if (shields != null)
                shields.Changed -= HandleShieldsChanged;

            health = null;
            shields = null;
        }
    }
}
