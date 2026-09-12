using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Abilities;
using MiniCrawler.Expedition;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public sealed class RunState
    {
        private readonly List<PartyMemberDefinition> selectedParty = new();
        private readonly Dictionary<string, RunBuild> memberBuilds = new();
        private readonly List<PendingRewardChoice> pendingRewardChoices = new();

        public int Currency { get; private set; }
        public int TotalCurrencyEarned { get; private set; }
        public WorldClockState WorldClock { get; } = new();
        public WorldEventState WorldEvents { get; } = new();

        public IReadOnlyList<PartyMemberDefinition> SelectedParty => selectedParty;
        public IReadOnlyList<PendingRewardChoice> PendingRewardChoices => pendingRewardChoices;

        public int PendingRewardChoiceCount => pendingRewardChoices.Count;

        public bool HasPendingRewardChoice => pendingRewardChoices.Count > 0;

        public IReadOnlyList<RunUpgradeOffer> CurrentPendingRewardOffers => pendingRewardChoices.Count > 0
                    ? pendingRewardChoices[0].Offers
                    : Array.Empty<RunUpgradeOffer>();

        public RunState(RunStartConfiguration configuration)
        {
            if (configuration == null)
                return;

            foreach (PartyMemberDefinition member in configuration.Party
            )
            {
                if (member == null || selectedParty.Contains(member))
                {
                    continue;
                }

                selectedParty.Add(member);

                RunBuild build = GetBuild(member);
                PersistentMemberProgress persistentProgress = PersistentProgression.GetMemberProgress(member);

                build.InitializeGearLevels(
                    persistentProgress.WeaponLevel,
                    persistentProgress.ArmourLevel,
                    persistentProgress.FocusLevel
                );

                build.InitializeStartingAbilities(member.StartingAbilities);
            }
        }

        public void AddCurrency(int amount)
        {
            if (amount <= 0)
                return;

            Currency += amount;
            TotalCurrencyEarned += amount;
        }

        public bool IsSelected(
            PartyMemberDefinition definition
        )
        {
            return definition != null &&
                   selectedParty.Contains(definition);
        }

        public RunBuild GetBuild(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return new RunBuild();

            if (!memberBuilds.TryGetValue(
                    definition.Id,
                    out RunBuild build
                ))
            {
                build = new RunBuild();

                memberBuilds.Add(
                    definition.Id,
                    build
                );
            }

            return build;
        }
        
        public bool TryAcquireAbility(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            if (
                !IsSelected(definition) ||
                ability == null
            )
            {
                return false;
            }

            return
                GetBuild(definition)
                    .TryAcquireAbility(
                        ability
                    );
        }

        public bool EnqueueRewardChoice(IEnumerable<RunUpgradeOffer> offers)
        {
            if (offers == null)
                return false;

            List<RunUpgradeOffer> validOffers =
                new();

            foreach (
                RunUpgradeOffer offer
                    in offers
            )
            {
                if (
                    offer == null ||
                    !offer.IsValid ||
                    !IsSelected(offer.Member)
                )
                {
                    continue;
                }

                validOffers.Add(offer);
            }

            PendingRewardChoice choice =
                new PendingRewardChoice(
                    validOffers
                );

            if (!choice.IsValid)
                return false;

            pendingRewardChoices.Add(
                choice
            );

            return true;
        }
        
        public bool HasPendingOffer(PartyMemberDefinition member, RunRewardDefinition reward)
        {
            if (member == null || reward == null)
                return false;

            foreach (PendingRewardChoice choice in pendingRewardChoices)
            {
                if (choice == null)
                    continue;

                foreach (RunUpgradeOffer offer in choice.Offers)
                {
                    if (offer == null)
                        continue;

                    if (offer.Member == member && offer.Reward == reward)
                        return true;
                }
            }

            return false;
        }

        public bool TryChoosePendingReward(RunUpgradeOffer offer)
        {
            if (
                offer == null ||
                !offer.IsValid ||
                pendingRewardChoices.Count <= 0
            )
            {
                return false;
            }

            PendingRewardChoice currentChoice =
                pendingRewardChoices[0];

            if (
                !currentChoice.Contains(offer) ||
                !IsSelected(offer.Member)
            )
            {
                return false;
            }

            RunBuild build =
                GetBuild(
                    offer.Member
                );

            if (
                !offer.Reward.TryApply(
                    offer.Member,
                    build
                )
            )
            {
                return false;
            }

            pendingRewardChoices.RemoveAt(0);

            return true;
        }

        public int GetAbilityUpgradeCost(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            if (
                !IsSelected(definition) ||
                ability == null
            )
            {
                return int.MaxValue;
            }

            RunAbilityState state = GetBuild(definition).GetAbilityState(ability);

            if (
                state == null ||
                state.IsMaxLevel
            )
            {
                return int.MaxValue;
            }

            int completedUpgrades = Mathf.Max(0, state.Level - 1);

            return
                definition.BaseAbilityUpgradeCost +
                completedUpgrades *
                definition.AbilityUpgradeCostStep;
        }

        public bool TryBuyAbilityLevel(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            int cost = GetAbilityUpgradeCost(definition, ability);

            if (
                cost == int.MaxValue ||
                Currency < cost
            )
            {
                return false;
            }

            RunBuild build = GetBuild(definition);

            if (
                !build.TryIncreaseAbilityLevel(ability)
            )
            {
                return false;
            }

            Currency -= cost;

            return true;
        }

        public float GetDamageBonus(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return 0f;

            return GetBuild(definition).WeaponLevel *
                   definition.DamagePerWeaponLevel;
        }

        public float GetArmourBonus(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return 0f;

            return GetBuild(definition).ArmourLevel *
                   definition.ArmourPerArmourLevel;
        }

        public float GetHealthBonus(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return 0f;

            return GetBuild(definition).ArmourLevel *
                   definition.HealthPerArmourLevel;
        }

        public float GetHealingBonus(
            PartyMemberDefinition definition
        )
        {
            if (definition == null)
                return 0f;

            return GetBuild(definition).FocusLevel *
                   definition.HealingPerFocusLevel;
        }
    }
}