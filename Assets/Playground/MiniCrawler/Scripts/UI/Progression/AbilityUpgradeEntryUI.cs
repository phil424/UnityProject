using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(Button))]
    public class AbilityUpgradeEntryUI :
        MonoBehaviour
    {
        [SerializeField]
        private TMP_Text labelText;

        private Button button;

        private PartyMemberDefinition member;
        private AbilityDefinition ability;

        private void Awake()
        {
            EnsureReferences();

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

        public void Bind(
            PartyMemberDefinition partyMember,
            AbilityDefinition abilityDefinition
        )
        {
            EnsureReferences();

            member =
                partyMember;

            ability =
                abilityDefinition;

            Refresh();
        }

        public void Refresh()
        {
            EnsureReferences();

            if (
                member == null ||
                ability == null
            )
            {
                gameObject.SetActive(false);
                return;
            }

            RunBuild build =
                RunProgress.GetBuild(
                    member
                );

            if (build == null)
            {
                gameObject.SetActive(false);
                return;
            }

            RunAbilityState state =
                build.GetAbilityState(
                    ability
                );

            if (state == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            string displayName =
                AbilityPresentationResolver
                    .GetDisplayName(
                        ability,
                        state.Evolutions
                    );

            gameObject.SetActive(true);

            if (state.IsMaxLevel)
            {
                if (labelText != null)
                {
                    labelText.text =
                        $"{displayName} " +
                        $"Lv.{state.Level} — MAX";
                }

                button.interactable =
                    false;

                return;
            }

            int cost =
                RunProgress.GetAbilityUpgradeCost(
                    member,
                    ability
                );

            if (labelText != null)
            {
                labelText.text =
                    $"{displayName} " +
                    $"Lv.{state.Level} → " +
                    $"Lv.{state.Level + 1} " +
                    $"({cost})";
            }

            button.interactable =
                RunProgress.Currency >= cost;
        }

        private void HandleClicked()
        {
            if (
                member == null ||
                ability == null
            )
            {
                return;
            }

            RunProgress.TryBuyAbilityLevel(
                member,
                ability
            );
        }

        private void EnsureReferences()
        {
            if (button == null)
            {
                button =
                    GetComponent<Button>();
            }
        }
    }
}