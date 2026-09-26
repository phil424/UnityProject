using UnityEngine;

namespace MiniCrawler.Combat
{
    [CreateAssetMenu(
        fileName = "New Projectile",
        menuName = "Mini Crawler/Combat/Projectile"
    )]
    public sealed class ProjectileDefinition : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField, Min(0.1f)] private float speed = 12f;
        [SerializeField, Min(0.01f)] private float hitDistance = 0.15f;
        [SerializeField, Min(0.1f)] private float maxLifetime = 3f;
        [SerializeField, Min(0f)] private float sourceForwardOffset = 0.35f;

        public GameObject Prefab => prefab;
        public float Speed => speed;
        public float HitDistance => hitDistance;
        public float MaxLifetime => maxLifetime;
        public float SourceForwardOffset => sourceForwardOffset;

        private void OnValidate()
        {
            speed = Mathf.Max(0.1f, speed);
            hitDistance = Mathf.Max(0.01f, hitDistance);
            maxLifetime = Mathf.Max(0.1f, maxLifetime);
            sourceForwardOffset = Mathf.Max(0f, sourceForwardOffset);
        }
    }
}