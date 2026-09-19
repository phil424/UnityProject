using UnityEngine;

namespace MiniCrawler.Combat
{
    [CreateAssetMenu(
        fileName = "New Combat Action",
        menuName = "Mini Crawler/Combat/Combat Action"
    )]
    public class CombatActionDefinition : ScriptableObject
    {
        [Header("Timing")]
        [SerializeField, Min(0.01f)] private float durationMultiplier = 1f;
        [SerializeField, Range(0f, 1f)] private float impactNormalizedTime = 0.5f;

        [Header("Damage")]
        [SerializeField, Min(0f)] private float damageCoefficient = 1f;

        public float DurationMultiplier => durationMultiplier;
        public float ImpactNormalizedTime => impactNormalizedTime;
        public float DamageCoefficient => damageCoefficient;

        private void OnValidate()
        {
            durationMultiplier = Mathf.Max(0.01f, durationMultiplier);
            impactNormalizedTime = Mathf.Clamp01(impactNormalizedTime);
            damageCoefficient = Mathf.Max(0f, damageCoefficient);
        }
    }
}