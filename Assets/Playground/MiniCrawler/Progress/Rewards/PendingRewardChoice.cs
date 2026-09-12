using System.Collections.Generic;

namespace MiniCrawler.Progress
{
    public sealed class PendingRewardChoice
    {
        private readonly List<RunUpgradeOffer>
            offers = new();

        public IReadOnlyList<RunUpgradeOffer>
            Offers => offers;

        public int OfferCount =>
            offers.Count;

        public bool IsValid =>
            offers.Count > 0;

        public PendingRewardChoice(
            IEnumerable<RunUpgradeOffer>
                rewardOffers
        )
        {
            if (rewardOffers == null)
                return;

            foreach (
                RunUpgradeOffer offer
                    in rewardOffers
            )
            {
                if (
                    offer == null ||
                    !offer.IsValid ||
                    offers.Contains(offer)
                )
                {
                    continue;
                }

                offers.Add(offer);
            }
        }

        public bool Contains(
            RunUpgradeOffer offer
        )
        {
            return
                offer != null &&
                offers.Contains(offer);
        }
    }
}