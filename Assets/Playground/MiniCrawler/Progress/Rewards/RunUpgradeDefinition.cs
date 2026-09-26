using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(fileName = "New Run Upgrade", menuName = "Mini Crawler/Run Upgrade")]
    public class RunUpgradeDefinition : RunRewardDefinition
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        [Header("Rarity")]
        [SerializeField] private RunUpgradeRarity rarity = RunUpgradeRarity.Common;

        [Header("Effect")]
        [SerializeField] private CharacterStatModifierType effectType;
        [SerializeField] private float amount;

        public override string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public override string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public override string Description => CharacterStatModifierPresentation.FormatDescription(effectType, amount);
        public override Sprite Icon => icon;
        public override RunUpgradeRarity Rarity => rarity;
        public override RunRewardVector RewardVector => RunRewardVector.CharacterGrowth;

        public CharacterStatModifierType EffectType => effectType;
        public float Amount => amount;

        public override bool IsConfigured => amount > 0f;

        public override bool CanApply(PartyMemberDefinition member, RunBuild build)
        {
            return member != null && build != null && amount > 0f;
        }

        public override bool TryApply(PartyMemberDefinition member, RunBuild build)
        {
            if (!CanApply(member, build))
                return false;

            build.ApplyExpeditionStatModifier(effectType, amount);
            return true;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            amount = Mathf.Max(0f, amount);
        }
    }
}