using MiniCrawler.Combat;
using MiniCrawler.Support;
using UnityEngine;

namespace MiniCrawler.Core
{
    [CreateAssetMenu(
        fileName = "New Party Member Definition",
        menuName = "Mini Crawler/Party Member Definition"
    )]
    public class PartyMemberDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string role;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite portrait;

        [Header("Spawned Actor")]
        [SerializeField] private ActorDefinition actorDefinition;

        [Header("Starting Gear")]
        [SerializeField] private string weaponName = "Basic Weapon";
        [SerializeField] private string armourName = "Basic Armour";
        [SerializeField] private string focusName = "Basic Focus";

        [Header("Upgrade Scaling")]
        [SerializeField] private float damagePerWeaponLevel = 2f;
        [SerializeField] private float armourPerArmourLevel = 1f;
        [SerializeField] private float healthPerArmourLevel = 10f;
        [SerializeField] private float healingPerFocusLevel = 2f;
        [SerializeField] private int baseUpgradeCost = 5;
        [SerializeField] private int upgradeCostStep = 5;

        public string Id => id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Role => role;
        public string Description => description;
        public Sprite Portrait => portrait;
        public ActorDefinition ActorDefinition => actorDefinition;

        public string WeaponName => weaponName;
        public string ArmourName => armourName;
        public string FocusName => focusName;

        public float DamagePerWeaponLevel => damagePerWeaponLevel;
        public float ArmourPerArmourLevel => armourPerArmourLevel;
        public float HealthPerArmourLevel => healthPerArmourLevel;
        public float HealingPerFocusLevel => healingPerFocusLevel;
        public int BaseUpgradeCost => baseUpgradeCost;
        public int UpgradeCostStep => upgradeCostStep;

        public GameObject Prefab => actorDefinition != null ? actorDefinition.Prefab : null;

        public float BaseHealth
        {
            get
            {
                Health health = Prefab != null ? Prefab.GetComponent<Health>() : null;
                return health != null ? health.MaxHealth : 0f;
            }
        }

        public float BaseDamage
        {
            get
            {
                CombatStats stats = Prefab != null ? Prefab.GetComponent<CombatStats>() : null;
                return stats != null ? stats.Damage : 0f;
            }
        }

        public float BaseArmour
        {
            get
            {
                CombatStats stats = Prefab != null ? Prefab.GetComponent<CombatStats>() : null;
                return stats != null ? stats.FlatArmour : 0f;
            }
        }

        public float BaseHealing
        {
            get
            {
                SupportStats stats = Prefab != null ? Prefab.GetComponent<SupportStats>() : null;
                return stats != null ? stats.HealAmount : 0f;
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            damagePerWeaponLevel = Mathf.Max(0f, damagePerWeaponLevel);
            armourPerArmourLevel = Mathf.Max(0f, armourPerArmourLevel);
            healthPerArmourLevel = Mathf.Max(0f, healthPerArmourLevel);
            healingPerFocusLevel = Mathf.Max(0f, healingPerFocusLevel);
            baseUpgradeCost = Mathf.Max(0, baseUpgradeCost);
            upgradeCostStep = Mathf.Max(0, upgradeCostStep);
        }
    }
}
