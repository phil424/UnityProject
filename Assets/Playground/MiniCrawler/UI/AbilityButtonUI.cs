using MiniCrawler.Abilities;
using MiniCrawler.Core;
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
        private ActorAbility ability;

        private void Awake()
        {
            button =
                GetComponent<Button>();

            button.onClick.AddListener(
                HandleClicked
            );
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(
                    HandleClicked
                );
            }
        }

        private void Update()
        {
            Refresh();
        }

        public void Bind(
            PartyMemberDefinition definition,
            ActorAbility actorAbility
        )
        {
            ownerDefinition =
                definition;

            ability =
                actorAbility;

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

            string ownerName =
                ownerDefinition != null
                    ? ownerDefinition.DisplayName
                    : "Unknown";

            if (ability == null)
            {
                button.interactable =
                    false;

                if (labelText != null)
                {
                    labelText.text =
                        $"{ownerName}\n" +
                        "Ability unavailable";
                }

                return;
            }

            string state = ability.IsExecuting ? "EXECUTING" : ability.IsReady
                        ? "READY" : $"{ability.CooldownRemaining:0.0}s";

            if (labelText != null)
            {
                labelText.text =
                    $"{ownerName}\n" +
                    $"{ability.DisplayName} Lv.{ability.Level}\n" +
                    state;
            }

            button.interactable =
                ability.CanActivateNow;
        }
    }
}