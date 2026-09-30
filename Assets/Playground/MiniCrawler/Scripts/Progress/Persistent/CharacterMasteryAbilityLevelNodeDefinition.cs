using MiniCrawler.Abilities;
using UnityEngine;

namespace MiniCrawler.Progress
{
    [CreateAssetMenu(
        fileName = "New Character Mastery Ability Level Node",
        menuName = "Mini Crawler/Progression/Character Mastery Ability Level Node"
    )]
    public sealed class CharacterMasteryAbilityLevelNodeDefinition : CharacterMasteryNodeDefinition
    {
        [Header("Ability Progression")]
        [SerializeField] private AbilityDefinition ability;
        [SerializeField, Min(1)] private int levelIncrease = 1;

        public AbilityDefinition Ability => ability;
        public int LevelIncrease => Mathf.Max(1, levelIncrease);

        public override bool IsConfigured => base.IsConfigured && ability != null && levelIncrease > 0;

        public override string EffectDescription
        {
            get
            {
                if (ability == null)
                    return "Increase a starting Ability level.";

                string suffix = LevelIncrease == 1 ? "level" : "levels";
                return $"Begin each Expedition with {ability.DisplayName} +{LevelIncrease} {suffix}.";
            }
        }

        public override void ApplyPreparedBuild(RunBuild build)
        {
            if (build == null || ability == null || levelIncrease <= 0)
                return;

            RunAbilityState state = build.GetAbilityState(ability);

            if (state == null)
                return;

            build.ConfigureAbilityLevel(ability, state.Level + levelIncrease);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            levelIncrease = Mathf.Max(1, levelIncrease);
        }
    }
}