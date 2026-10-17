using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(Button))]
    public class AbilityButtonUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField]
        private TMP_Text labelText;

        private Button button;
        private PartyMemberDefinition ownerDefinition;
        private RunAbilityState abilityState;
        private ActorAbility ability;
        private string slotLabel;

        public PartyMemberDefinition OwnerDefinition => ownerDefinition;
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
            Refresh();
        }

        public void Bind(PartyMemberDefinition definition, string displaySlotLabel, RunAbilityState state, ActorAbility actorAbility)
        {
            ownerDefinition = definition;
            slotLabel = displaySlotLabel;
            abilityState = state;
            ability = actorAbility;

            Refresh();
        }

        private void HandleClicked()
        {
            if (ability == null)
                return;

            ability.TryActivate();
            Refresh();
        }

        private void Refresh()
        {
            if (button == null)
                return;

            if (abilityState?.Definition == null)
            {
                button.interactable = false;

                if (labelText != null)
                {
                    labelText.text = $"{slotLabel}\n" + "EMPTY";
                }

                return;
            }

            if (ability == null)
            {
                button.interactable = false;

                if (labelText != null)
                {
                    labelText.text = $"{slotLabel}\n" + $"{abilityState.Definition.DisplayName} Lv.{abilityState.Level}\n" + "UNAVAILABLE";
                }

                return;
            }

            string stateText =
                ability.IsExecuting ? "EXECUTING"
                : ability.IsReady ? "READY"
                : $"{ability.CooldownRemaining:0.0}s";

            if (labelText != null)
            {
                labelText.text = $"{slotLabel}\n" + $"{ability.DisplayName} Lv.{ability.Level}\n" + stateText;
            }

            button.interactable = ability.CanActivateNow;
        }
    }
}
