using UnityEngine;

namespace MiniCrawler.Combat
{
    [CreateAssetMenu(
        fileName = "New Projectile Combat Action",
        menuName = "Mini Crawler/Combat/Projectile Combat Action"
    )]
    public sealed class ProjectileCombatActionDefinition : CombatActionDefinition
    {
        [SerializeField] private ProjectileDefinition projectile;

        public ProjectileDefinition Projectile => projectile;
    }
}