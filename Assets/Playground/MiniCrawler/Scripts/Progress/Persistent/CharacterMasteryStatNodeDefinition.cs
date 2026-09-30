using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Character Mastery Stat Node",
        menuName = "Mini Crawler/Progression/Character Mastery Stat Node"
    )]
    public sealed class CharacterMasteryStatNodeDefinition : CharacterMasteryNodeDefinition
    {
        [Header("Stat Modifier")]
        [SerializeField] private CharacterStatModifierType modifierType;
        [SerializeField] private float amount;

        public CharacterStatModifierType ModifierType => modifierType;
        public float Amount => amount;

        public override bool IsConfigured => base.IsConfigured && amount > 0f;

        public override string EffectDescription =>
            CharacterStatModifierPresentation.FormatDescription(modifierType, amount);

        public override void ApplyPreparedBuild(RunBuild build)
        {
            if (build == null || amount <= 0f)
                return;

            build.ApplyPreparedStatModifier(modifierType, amount);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            amount = Mathf.Max(0f, amount);
        }
    }
}