using UnityEngine;

namespace MiniCrawler.Abilities
{
    [CreateAssetMenu(
        fileName = "New Ability",
        menuName = "Mini Crawler/Ability Definition"
    )]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private Sprite icon;

        [Header("Levels")]
        [SerializeField]
        [Min(1)]
        private int maxLevel = 5;

        [Header("Runtime")]
        [SerializeField]
        private AbilityLevelValue cooldown;

        [SerializeField]
        private GameObject runtimePrefab;

        public string Id =>
            string.IsNullOrWhiteSpace(id)
                ? name
                : id;

        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName)
                ? name
                : displayName;

        public Sprite Icon =>
            icon;

        public int MaxLevel =>
            maxLevel;

        public GameObject RuntimePrefab =>
            runtimePrefab;

        public int ClampLevel(
            int level
        )
        {
            return Mathf.Clamp(
                level,
                1,
                maxLevel
            );
        }

        public float GetCooldown(
            int level
        )
        {
            if (cooldown == null)
                return 0.1f;

            return Mathf.Max(
                0.1f,
                cooldown.Evaluate(
                    ClampLevel(level)
                )
            );
        }

        public ActorAbility CreateRuntime(
            GameObject owner,
            int level
        )
        {
            if (
                owner == null ||
                runtimePrefab == null
            )
            {
                return null;
            }

            GameObject instance =
                Instantiate(
                    runtimePrefab,
                    owner.transform
                );

            instance.name =
                $"{DisplayName} Ability Runtime";

            instance.transform.localPosition =
                Vector3.zero;

            instance.transform.localRotation =
                Quaternion.identity;

            instance.transform.localScale =
                Vector3.one;

            ActorAbility ability =
                instance.GetComponent<ActorAbility>();

            if (ability == null)
            {
                Debug.LogError(
                    $"Ability runtime prefab " +
                    $"'{runtimePrefab.name}' does not " +
                    $"contain an ActorAbility."
                );

                Destroy(instance);

                return null;
            }

            ability.Initialize(
                owner,
                this,
                ClampLevel(level)
            );

            return ability;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            maxLevel =
                Mathf.Max(
                    1,
                    maxLevel
                );
        }
    }
}