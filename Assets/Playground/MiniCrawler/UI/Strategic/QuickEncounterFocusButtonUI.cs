using MiniCrawler.Encounters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class QuickEncounterFocusButtonUI : MonoBehaviour
    {
        [SerializeField] private QuickEncounterFocus focus = QuickEncounterFocus.Balanced;
        [SerializeField] private TMP_Text buttonText;

        private Button button;
        private QuickEncounterChoices quickChoices;

        private void Awake()
        {
            CacheReferences();
            ApplyLabel();
        }

        private void OnEnable()
        {
            Bind();
            Refresh();
        }

        private void Start()
        {
            Bind();
            Refresh();
        }

        private void OnDisable()
        {
            Unbind();
        }

        private void OnValidate()
        {
            CacheReferences();
            ApplyLabel();
        }

        private void Bind()
        {
            QuickEncounterChoices current = QuickEncounterChoices.Instance;

            if (quickChoices == current)
                return;

            Unbind();

            quickChoices = current;

            if (quickChoices != null)
                quickChoices.FocusChanged += HandleFocusChanged;

            if (button != null)
                button.onClick.AddListener(HandleClicked);
        }

        private void Unbind()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);

            if (quickChoices != null)
                quickChoices.FocusChanged -= HandleFocusChanged;

            quickChoices = null;
        }

        private void HandleClicked()
        {
            if (quickChoices == null)
                Bind();

            quickChoices?.SetFocus(focus);
        }

        private void HandleFocusChanged(QuickEncounterFocus _)
        {
            Refresh();
        }

        private void Refresh()
        {
            ApplyLabel();

            if (button != null)
                button.interactable = quickChoices == null || quickChoices.Focus != focus;
        }

        private void ApplyLabel()
        {
            if (buttonText == null)
                return;

            buttonText.text = focus switch
            {
                QuickEncounterFocus.Balanced => "BALANCED",
                QuickEncounterFocus.CharacterGrowth => "GROWTH",
                QuickEncounterFocus.Ability => "ABILITY",
                _ => "FOCUS"
            };
        }

        private void CacheReferences()
        {
            button = GetComponent<Button>();

            if (buttonText == null)
                buttonText = GetComponentInChildren<TMP_Text>(true);
        }
    }
}