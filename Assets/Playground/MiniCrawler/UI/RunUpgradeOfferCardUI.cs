using System;
using MiniCrawler.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(Button))]
    public class RunUpgradeOfferCardUI : MonoBehaviour
    {
        [Header("Member")]
        [SerializeField]
        private Image memberPortraitImage;

        [SerializeField]
        private TMP_Text memberNameText;

        [Header("Upgrade")]
        [SerializeField]
        private Image upgradeIconImage;

        [SerializeField]
        private TMP_Text upgradeNameText;

        [SerializeField]
        private TMP_Text descriptionText;

        private Button chooseButton;
        private RunUpgradeOffer offer;
        private Action<RunUpgradeOffer> chooseRequested;

        private void Awake()
        {
            chooseButton =
                GetComponent<Button>();

            chooseButton.onClick.AddListener(
                HandleChooseClicked
            );
        }

        private void OnDestroy()
        {
            if (chooseButton != null)
            {
                chooseButton.onClick.RemoveListener(
                    HandleChooseClicked
                );
            }
        }

        public void Bind(
            RunUpgradeOffer runUpgradeOffer,
            Action<RunUpgradeOffer> onChooseRequested
        )
        {
            offer = runUpgradeOffer;
            chooseRequested = onChooseRequested;

            Refresh();
        }

        private void Refresh()
        {
            if (offer == null ||
                !offer.IsValid)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            memberNameText.text =
                offer.Member.DisplayName;

            RunUpgradeRarity rarity =
                offer.Upgrade.Rarity;

            string rarityLabel =
                RunUpgradeRarityPresentation.GetLabel(
                    rarity
                );

            Color rarityColor =
                RunUpgradeRarityPresentation.GetColor(
                    rarity
                );

            upgradeNameText.text =
                $"{rarityLabel} • " +
                $"{offer.Upgrade.DisplayName}";

            upgradeNameText.color =
                rarityColor;

            descriptionText.text =
                offer.Upgrade.Description;

            if (memberPortraitImage != null)
            {
                memberPortraitImage.sprite =
                    offer.Member.Portrait;

                memberPortraitImage.enabled =
                    offer.Member.Portrait != null;
            }

            if (upgradeIconImage != null)
            {
                upgradeIconImage.sprite =
                    offer.Upgrade.Icon;

                upgradeIconImage.enabled =
                    offer.Upgrade.Icon != null;
            }
        }

        private void HandleChooseClicked()
        {
            if (offer == null ||
                !offer.IsValid)
            {
                return;
            }

            chooseRequested?.Invoke(
                offer
            );
        }
    }
}