using System.Collections.Generic;

namespace MiniCrawler.Progress
{
    public sealed class PendingRewardChoice
    {
        private readonly List<RunUpgradeOffer> offers = new();

        public IReadOnlyList<RunUpgradeOffer> Offers => offers;

        public RunRewardContext Context { get; }

        public int OfferCount => offers.Count;

        public bool IsValid => offers.Count > 0;

        public PendingRewardChoice(IEnumerable<RunUpgradeOffer> rewardOffers, RunRewardContext context)
        {
            Context = context;

            CollectValidOffers(rewardOffers, offers);
        }

        public bool Contains(RunUpgradeOffer offer)
        {
            return offer != null && offers.Contains(offer);
        }

        public bool ReplaceOffers(IEnumerable<RunUpgradeOffer> rewardOffers)
        {
            List<RunUpgradeOffer> replacements = new();

            CollectValidOffers(rewardOffers, replacements);

            if (replacements.Count == 0)
                return false;

            offers.Clear();
            offers.AddRange(replacements);

            return true;
        }

        private static void CollectValidOffers(IEnumerable<RunUpgradeOffer> source, List<RunUpgradeOffer> output)
        {
            if (source == null)
                return;

            foreach (RunUpgradeOffer offer in source)
            {
                if (offer == null || !offer.IsValid || ContainsEquivalent(output, offer))
                {
                    continue;
                }

                output.Add(offer);
            }
        }

        private static bool ContainsEquivalent(IReadOnlyList<RunUpgradeOffer> currentOffers, RunUpgradeOffer candidate)
        {
            foreach (RunUpgradeOffer current in currentOffers)
            {
                if (current != null && current.Member == candidate.Member && current.Reward == candidate.Reward)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
