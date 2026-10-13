using System.Collections.Generic;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public static class RunUpgradeOfferGenerator
    {
        private static readonly RunUpgradeRarity[] RarityOrder = { RunUpgradeRarity.Common, RunUpgradeRarity.Uncommon, RunUpgradeRarity.Rare, RunUpgradeRarity.Epic, RunUpgradeRarity.Legendary };

        public static IReadOnlyList<RunUpgradeOffer> Generate(
            RunState runState,
            RunRewardCatalogDefinition catalog,
            int offerCount,
            RunRewardVector rewardVector = RunRewardVector.Any,
            IReadOnlyList<RunUpgradeOffer> excludedOffers = null,
            bool excludeDiversityGroups = false
        )
        {
            List<RunUpgradeOffer> candidates = CollectCandidates(runState, catalog, rewardVector, excludedOffers, excludeDiversityGroups);

            return SelectOffers(candidates, offerCount, catalog);
        }

        public static int CountEligibleOfferGroups(
            RunState runState,
            RunRewardCatalogDefinition catalog,
            RunRewardVector rewardVector,
            IReadOnlyList<RunUpgradeOffer> excludedOffers = null,
            bool excludeDiversityGroups = false
        )
        {
            List<RunUpgradeOffer> candidates = CollectCandidates(runState, catalog, rewardVector, excludedOffers, excludeDiversityGroups);

            HashSet<string> groups = new();

            foreach (RunUpgradeOffer candidate in candidates)
            {
                if (candidate?.Reward == null || catalog.GetRarityWeight(candidate.Reward.Rarity) <= 0f)
                {
                    continue;
                }

                groups.Add(GetOfferGroupKey(candidate));
            }

            return groups.Count;
        }

        private static List<RunUpgradeOffer> CollectCandidates(
            RunState runState,
            RunRewardCatalogDefinition catalog,
            RunRewardVector rewardVector,
            IReadOnlyList<RunUpgradeOffer> excludedOffers,
            bool excludeDiversityGroups
        )
        {
            List<RunUpgradeOffer> candidates = new();

            if (runState == null || catalog == null)
            {
                return candidates;
            }

            List<RunRewardDefinition> rewards = new();

            catalog.CollectRewards(rewards);

            foreach (PartyMemberDefinition member in runState.SelectedParty)
            {
                if (member == null)
                    continue;

                RunBuild build = runState.GetBuild(member);

                foreach (RunRewardDefinition reward in rewards)
                {
                    if (reward == null || !reward.IsConfigured)
                    {
                        continue;
                    }

                    if (rewardVector != RunRewardVector.Any && reward.RewardVector != rewardVector)
                    {
                        continue;
                    }

                    if (!reward.CanApply(member, build))
                    {
                        continue;
                    }

                    if (IsExplicitlyExcluded(member, reward, excludedOffers, excludeDiversityGroups))
                    {
                        continue;
                    }

                    if (!reward.AllowDuplicatePendingOffers && runState.HasPendingOffer(member, reward))
                    {
                        continue;
                    }

                    candidates.Add(new RunUpgradeOffer(member, reward));
                }
            }

            return candidates;
        }

        private static bool IsExplicitlyExcluded(PartyMemberDefinition member, RunRewardDefinition reward, IReadOnlyList<RunUpgradeOffer> excludedOffers, bool excludeDiversityGroups)
        {
            if (excludedOffers == null)
                return false;

            foreach (RunUpgradeOffer excluded in excludedOffers)
            {
                if (excluded?.Member != member || excluded.Reward == null)
                {
                    continue;
                }

                if (excluded.Reward == reward)
                    return true;

                if (excludeDiversityGroups && excluded.Reward.OfferDiversityKey == reward.OfferDiversityKey)
                {
                    return true;
                }
            }

            return false;
        }

        private static IReadOnlyList<RunUpgradeOffer> SelectOffers(List<RunUpgradeOffer> candidates, int offerCount, RunRewardCatalogDefinition catalog)
        {
            List<RunUpgradeOffer> selected = new();

            if (offerCount <= 0)
                return selected;

            for (int i = 0; i < offerCount && candidates.Count > 0; i++)
            {
                int selectedIndex = SelectCandidateIndex(candidates, catalog);

                if (selectedIndex < 0)
                    break;

                RunUpgradeOffer selectedOffer = candidates[selectedIndex];

                selected.Add(selectedOffer);

                RemoveOfferGroup(candidates, selectedOffer);
            }

            return selected;
        }

        private static void RemoveOfferGroup(List<RunUpgradeOffer> candidates, RunUpgradeOffer selectedOffer)
        {
            string selectedGroup = GetOfferGroupKey(selectedOffer);

            for (int i = candidates.Count - 1; i >= 0; i--)
            {
                if (GetOfferGroupKey(candidates[i]) == selectedGroup)
                {
                    candidates.RemoveAt(i);
                }
            }
        }

        private static string GetOfferGroupKey(RunUpgradeOffer offer)
        {
            string memberId = offer?.Member != null ? offer.Member.Id : string.Empty;

            string rewardGroup = offer?.Reward != null ? offer.Reward.OfferDiversityKey : string.Empty;

            return $"{memberId}|{rewardGroup}";
        }

        private static int SelectCandidateIndex(IReadOnlyList<RunUpgradeOffer> candidates, RunRewardCatalogDefinition catalog)
        {
            if (candidates == null || candidates.Count == 0 || catalog == null)
            {
                return -1;
            }

            float totalWeight = 0f;

            foreach (RunUpgradeRarity rarity in RarityOrder)
            {
                if (!HasCandidateOfRarity(candidates, rarity))
                {
                    continue;
                }

                totalWeight += catalog.GetRarityWeight(rarity);
            }

            if (totalWeight <= 0f)
                return -1;

            float roll = Random.value * totalWeight;

            RunUpgradeRarity selectedRarity = RunUpgradeRarity.Common;

            bool raritySelected = false;

            foreach (RunUpgradeRarity rarity in RarityOrder)
            {
                if (!HasCandidateOfRarity(candidates, rarity))
                {
                    continue;
                }

                float weight = catalog.GetRarityWeight(rarity);

                if (weight <= 0f)
                    continue;

                selectedRarity = rarity;
                raritySelected = true;

                roll -= weight;

                if (roll <= 0f)
                    break;
            }

            if (!raritySelected)
                return -1;

            int matchCount = 0;

            foreach (RunUpgradeOffer candidate in candidates)
            {
                if (candidate?.Reward != null && candidate.Reward.Rarity == selectedRarity)
                {
                    matchCount++;
                }
            }

            if (matchCount <= 0)
                return -1;

            int selectedMatch = Random.Range(0, matchCount);

            int currentMatch = 0;

            for (int i = 0; i < candidates.Count; i++)
            {
                RunUpgradeOffer candidate = candidates[i];

                if (candidate?.Reward == null || candidate.Reward.Rarity != selectedRarity)
                {
                    continue;
                }

                if (currentMatch == selectedMatch)
                {
                    return i;
                }

                currentMatch++;
            }

            return -1;
        }

        private static bool HasCandidateOfRarity(IReadOnlyList<RunUpgradeOffer> candidates, RunUpgradeRarity rarity)
        {
            foreach (RunUpgradeOffer candidate in candidates)
            {
                if (candidate?.Reward != null && candidate.Reward.Rarity == rarity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
