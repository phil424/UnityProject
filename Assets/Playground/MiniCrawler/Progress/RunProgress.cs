using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Abilities;

namespace MiniCrawler.Progress
{
    public static class RunProgress
    {
        public static event Action Changed;

        public static RunState CurrentRun { get; private set; }

        public static bool HasActiveRun =>
            CurrentRun != null;

        public static int Currency =>
            CurrentRun != null
                ? CurrentRun.Currency
                : 0;

        public static IReadOnlyList<PartyMemberDefinition> SelectedParty =>
            CurrentRun != null
                ? CurrentRun.SelectedParty
                : Array.Empty<PartyMemberDefinition>();
        
        public static IReadOnlyList<RunUpgradeOffer> PendingUpgradeOffers =>
            CurrentRun != null
                ? CurrentRun.PendingUpgradeOffers
                : Array.Empty<RunUpgradeOffer>();

        public static bool HasPendingUpgradeChoice =>
            CurrentRun != null &&
            CurrentRun.HasPendingUpgradeChoice;

        public static bool BeginRun(
            RunStartConfiguration configuration
        )
        {
            if (CurrentRun != null)
                return false;

            if (configuration == null ||
                !configuration.IsValid)
            {
                return false;
            }

            CurrentRun = new RunState(configuration);

            Changed?.Invoke();

            return true;
        }

        public static void EndRun()
        {
            if (CurrentRun == null)
                return;

            CurrentRun = null;

            Changed?.Invoke();
        }

        public static void AddCurrency(int amount)
        {
            if (CurrentRun == null || amount <= 0)
                return;

            CurrentRun.AddCurrency(amount);

            Changed?.Invoke();
        }

        public static RunBuild GetBuild(
            PartyMemberDefinition definition
        )
        {
            if (CurrentRun == null)
                return new RunBuild();

            return CurrentRun.GetBuild(definition);
        }
        
        public static bool TryAcquireAbility(
            PartyMemberDefinition definition,
            AbilityDefinition ability
        )
        {
            if (CurrentRun == null)
                return false;

            bool acquired =
                CurrentRun.TryAcquireAbility(
                    definition,
                    ability
                );

            if (acquired)
                Changed?.Invoke();

            return acquired;
        }

        public static bool TryApplyRunUpgrade(
            PartyMemberDefinition definition,
            RunUpgradeDefinition upgrade
        )
        {
            if (CurrentRun == null)
                return false;

            bool applied =
                CurrentRun.TryApplyRunUpgrade(
                    definition,
                    upgrade
                );

            if (applied)
                Changed?.Invoke();

            return applied;
        }

        public static void SetRunUpgradeOffers(
            IEnumerable<RunUpgradeOffer> offers
        )
        {
            if (CurrentRun == null)
                return;

            CurrentRun.SetRunUpgradeOffers(
                offers
            );

            Changed?.Invoke();
        }

        public static bool TryChooseRunUpgrade(
            RunUpgradeOffer offer
        )
        {
            if (CurrentRun == null)
                return false;

            bool chosen =
                CurrentRun.TryChooseRunUpgrade(
                    offer
                );

            if (chosen)
                Changed?.Invoke();

            return chosen;
        }

        public static int GetUpgradeCost(
            PartyMemberDefinition definition,
            GearSlot slot
        )
        {
            if (CurrentRun == null)
                return int.MaxValue;

            return CurrentRun.GetUpgradeCost(
                definition,
                slot
            );
        }

        public static bool TryBuyUpgrade(
            PartyMemberDefinition definition,
            GearSlot slot
        )
        {
            if (CurrentRun == null)
                return false;

            bool purchased =
                CurrentRun.TryBuyUpgrade(
                    definition,
                    slot
                );

            if (purchased)
                Changed?.Invoke();

            return purchased;
        }

        public static float GetDamageBonus(
            PartyMemberDefinition definition
        )
        {
            return CurrentRun != null
                ? CurrentRun.GetDamageBonus(definition)
                : 0f;
        }

        public static float GetArmourBonus(
            PartyMemberDefinition definition
        )
        {
            return CurrentRun != null
                ? CurrentRun.GetArmourBonus(definition)
                : 0f;
        }

        public static float GetHealthBonus(
            PartyMemberDefinition definition
        )
        {
            return CurrentRun != null
                ? CurrentRun.GetHealthBonus(definition)
                : 0f;
        }

        public static float GetHealingBonus(
            PartyMemberDefinition definition
        )
        {
            return CurrentRun != null
                ? CurrentRun.GetHealingBonus(definition)
                : 0f;
        }
    }
}