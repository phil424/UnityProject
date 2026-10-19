using MiniCrawler.Abilities;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(Button))]
    public class AbilityButtonUI : MonoBehaviour, ICanvasRaycastFilter
    {
        [Header("Ability")]
        [SerializeField]
        private Image iconImage;

        [Header("Cooldown")]
        [SerializeField]
        private Image cooldownWashImage;

        [SerializeField]
        private TMP_Text cooldownText;

        [Header("Labels")]
        [SerializeField]
        private TMP_Text levelText;

        [SerializeField]
        private TMP_Text abilityNameText;

        [Header("Interaction")]
        [SerializeField, Min(0f)]
        private float raycastInset = 2f;

        private Button button;
        private RunAbilityState abilityState;
        private ActorAbility ability;

        public ActorAbility Ability => ability;

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

        private void Update()
        {
            RefreshRuntimeState();
        }

        public void Bind(RunAbilityState state, ActorAbility actorAbility)
        {
            abilityState = state;
            ability = actorAbility;

            RefreshPresentation();
            RefreshRuntimeState();
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            RectTransform rectTransform = (RectTransform)transform;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = rectTransform.rect;
            Vector2 centeredPoint = localPoint - rect.center;

            float halfWidth = Mathf.Max(0.01f, rect.width * 0.5f - raycastInset);

            float halfHeight = Mathf.Max(0.01f, rect.height * 0.5f - raycastInset);

            float normalizedDistance = Mathf.Abs(centeredPoint.x) / halfWidth + Mathf.Abs(centeredPoint.y) / halfHeight;

            return normalizedDistance <= 1f;
        }

        private void HandleClicked()
        {
            if (ability == null)
                return;

            ability.TryActivate();
            RefreshRuntimeState();
        }

        private void RefreshPresentation()
        {
            if (abilityState?.Definition == null)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.enabled = false;
                }

                if (levelText != null)
                    levelText.text = string.Empty;

                if (abilityNameText != null)
                    abilityNameText.text = "EMPTY";

                return;
            }

            Sprite displayIcon = ability != null ? ability.DisplayIcon : abilityState.Definition.Icon;

            string displayName = ability != null ? ability.DisplayName : abilityState.Definition.DisplayName;

            if (iconImage != null)
            {
                iconImage.sprite = displayIcon;
                iconImage.enabled = displayIcon != null;
            }

            if (levelText != null)
                levelText.text = $"Lv.{abilityState.Level}";

            if (abilityNameText != null)
                abilityNameText.text = displayName;
        }

        private void RefreshRuntimeState()
        {
            if (button == null)
                return;

            if (abilityState?.Definition == null || ability == null)
            {
                button.interactable = false;

                if (cooldownWashImage != null)
                {
                    cooldownWashImage.fillAmount = 0f;
                    cooldownWashImage.enabled = false;
                }

                if (cooldownText != null)
                    cooldownText.text = string.Empty;

                return;
            }

            button.interactable = ability.CanActivateNow;

            float cooldownFraction = ability.CooldownFractionRemaining;
            bool coolingDown = cooldownFraction > 0f;

            if (cooldownWashImage != null)
            {
                cooldownWashImage.fillAmount = cooldownFraction;
                cooldownWashImage.enabled = coolingDown;
            }

            if (cooldownText != null)
            {
                cooldownText.text = coolingDown ? FormatCooldown(ability.CooldownRemaining) : string.Empty;
            }
        }

        private static string FormatCooldown(float seconds)
        {
            if (seconds >= 10f)
                return Mathf.CeilToInt(seconds).ToString();

            return seconds.ToString("0.0");
        }
    }
}
