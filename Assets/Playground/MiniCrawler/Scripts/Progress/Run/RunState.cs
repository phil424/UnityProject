using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Expedition;
using UnityEngine;

namespace MiniCrawler.Progress
{
    public sealed class RunState
    {
        private readonly List<PartyMemberDefinition> selectedParty = new();
        private readonly Dictionary<string, RunBuild> memberBuilds = new();
        private readonly List<PendingRewardChoice> pendingRewardChoices = new();
        private readonly List<ExpeditionObjectiveState> objectives = new();

        public int Currency { get; private set; }
        public int TotalCurrencyEarned { get; private set; }
        public int Level1ResourcesEarned { get; private set; }
        public int Level2ResourcesEarned { get; private set; }
        public int Level3ResourcesEarned { get; private set; }
        public WorldClockState WorldClock { get; } = new();
        public WorldEventState WorldEvents { get; } = new();
        public int RewardRerollsRemaining { get; private set; }

        public IReadOnlyList<PartyMemberDefinition> SelectedParty => selectedParty;
        public IReadOnlyList<PendingRewardChoice> PendingRewardChoices => pendingRewardChoices;
        public IReadOnlyList<ExpeditionObjectiveState> Objectives => objectives;

        public int PendingRewardChoiceCount => pendingRewardChoices.Count;
        public bool HasPendingRewardChoice => pendingRewardChoices.Count > 0;

        public PendingRewardChoice CurrentPendingRewardChoice => pendingRewardChoices.Count > 0 ? pendingRewardChoices[0] : null;

        public IReadOnlyList<RunUpgradeOffer> CurrentPendingRewardOffers => CurrentPendingRewardChoice != null ? CurrentPendingRewardChoice.Offers : Array.Empty<RunUpgradeOffer>();

        public RunState(RunStartConfiguration configuration)
        {
            if (configuration == null)
                return;

            RewardRerollsRemaining = configuration.StartingRewardRerolls;

            foreach (ExpeditionObjectiveDefinition definition in configuration.Objectives)
            {
                if (definition == null)
                    continue;

                bool alreadyAdded = false;

                foreach (ExpeditionObjectiveState objective in objectives)
                {
                    if (objective.Definition != definition)
                        continue;

                    alreadyAdded = true;
                    break;
                }

                if (!alreadyAdded)
                    objectives.Add(new ExpeditionObjectiveState(definition));
            }

            foreach (PartyMemberDefinition member in configuration.Party)
            {
                if (member == null || selectedParty.Contains(member))
                    continue;

                selectedParty.Add(member);

                RunBuild build = GetBuild(member);
                PersistentMemberProgress persistentProgress = PersistentProgression.GetMemberProgress(member);

                build.InitializeGearLevels(persistentProgress.WeaponLevel, persistentProgress.ArmourLevel, persistentProgress.FocusLevel);

                build.InitializeStartingAbilities(member.StartingAbilities);
                PersistentProgression.ApplyMasteryToBuild(member, build);
            }
        }

        public void AddCurrency(int amount)
        {
            if (amount <= 0)
                return;

            Currency += amount;
            TotalCurrencyEarned += amount;
        }

        public int GetPersistentResourceEarned(PersistentResourceTier tier)
        {
            return tier switch
            {
                PersistentResourceTier.Level1 => Level1ResourcesEarned,
                PersistentResourceTier.Level2 => Level2ResourcesEarned,
                PersistentResourceTier.Level3 => Level3ResourcesEarned,
                _ => 0,
            };
        }

        public void AddPersistentResource(PersistentResourceTier tier, int amount)
        {
            if (amount <= 0)
                return;

            switch (tier)
            {
                case PersistentResourceTier.Level1:
                    Level1ResourcesEarned += amount;
                    break;

                case PersistentResourceTier.Level2:
                    Level2ResourcesEarned += amount;
                    break;

                case PersistentResourceTier.Level3:
                    Level3ResourcesEarned += amount;
                    break;
            }
        }

        public bool IsSelected(PartyMemberDefinition definition)
        {
            return definition != null && selectedParty.Contains(definition);
        }

        public RunBuild GetBuild(PartyMemberDefinition definition)
        {
            if (definition == null)
                return new RunBuild();

            if (!memberBuilds.TryGetValue(definition.Id, out RunBuild build))
            {
                build = new RunBuild();
                memberBuilds.Add(definition.Id, build);
            }

            return build;
        }

        public bool TryAcquireAbility(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            if (!IsSelected(definition) || ability == null)
                return false;

            return GetBuild(definition).TryAcquireAbility(ability);
        }

        public bool EnqueueRewardChoice(IEnumerable<RunUpgradeOffer> offers, RunRewardContext context)
        {
            if (offers == null)
                return false;

            List<RunUpgradeOffer> validOffers = new();

            foreach (RunUpgradeOffer offer in offers)
            {
                if (offer == null || !offer.IsValid || !IsSelected(offer.Member))
                    continue;

                validOffers.Add(offer);
            }

            PendingRewardChoice choice = new PendingRewardChoice(validOffers, context);

            if (!choice.IsValid)
                return false;

            pendingRewardChoices.Add(choice);

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
            if (offer == null || !offer.IsValid || pendingRewardChoices.Count <= 0)
                return false;

            PendingRewardChoice currentChoice = pendingRewardChoices[0];

            if (!currentChoice.Contains(offer) || !IsSelected(offer.Member))
                return false;

            RunBuild build = GetBuild(offer.Member);

            if (!offer.Reward.TryApply(offer.Member, build))
                return false;

            pendingRewardChoices.RemoveAt(0);

            return true;
        }

        public bool TryRerollCurrentPendingReward(IEnumerable<RunUpgradeOffer> replacementOffers)
        {
            if (RewardRerollsRemaining <= 0 || CurrentPendingRewardChoice == null || replacementOffers == null)
            {
                return false;
            }

            PendingRewardChoice replacement = new(replacementOffers, CurrentPendingRewardChoice.Context);

            if (!replacement.IsValid || replacement.OfferCount != CurrentPendingRewardChoice.OfferCount)
            {
                return false;
            }

            foreach (RunUpgradeOffer offer in replacement.Offers)
            {
                if (!IsSelected(offer.Member))
                    return false;
            }

            if (!CurrentPendingRewardChoice.ReplaceOffers(replacement.Offers))
            {
                return false;
            }

            RewardRerollsRemaining--;

            return true;
        }

        public bool TryAddObjectiveProgress(ExpeditionObjectiveDefinition definition, int amount)
        {
            if (definition == null || amount <= 0)
                return false;

            foreach (ExpeditionObjectiveState objective in objectives)
            {
                if (objective.Definition != definition)
                    continue;

                return objective.AddProgress(amount);
            }

            return false;
        }

        public int GetAbilityUpgradeCost(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            if (!IsSelected(definition) || ability == null)
                return int.MaxValue;

            RunAbilityState state = GetBuild(definition).GetAbilityState(ability);

            if (state == null || state.IsMaxLevel)
                return int.MaxValue;

            int completedUpgrades = Mathf.Max(0, state.Level - 1);

            return definition.BaseAbilityUpgradeCost + completedUpgrades * definition.AbilityUpgradeCostStep;
        }

        public bool TryBuyAbilityLevel(PartyMemberDefinition definition, AbilityDefinition ability)
        {
            int cost = GetAbilityUpgradeCost(definition, ability);

            if (cost == int.MaxValue || Currency < cost)
                return false;

            RunBuild build = GetBuild(definition);

            if (!build.TryIncreaseAbilityLevel(ability))
                return false;

            Currency -= cost;

            return true;
        }

        public float GetDamageBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            RunBuild build = GetBuild(definition);

            return build.WeaponLevel * definition.DamagePerWeaponLevel + build.FlatBasicAttackDamageBonus;
        }

        public float GetArmourBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            RunBuild build = GetBuild(definition);

            return build.ArmourLevel * definition.ArmourPerArmourLevel + build.FlatArmourBonus;
        }

        public float GetHealthBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            RunBuild build = GetBuild(definition);

            return build.ArmourLevel * definition.HealthPerArmourLevel + build.FlatMaxHealthBonus;
        }

        public float GetHealingBonus(PartyMemberDefinition definition)
        {
            if (definition == null)
                return 0f;

            return GetBuild(definition).FocusLevel * definition.HealingPerFocusLevel;
        }
    }
}
