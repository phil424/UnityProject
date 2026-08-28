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

        [Header("Runtime")]
        [SerializeField]
        [Min(0.1f)]
        private float cooldown = 3f;

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

        public float Cooldown =>
            cooldown;

        public GameObject RuntimePrefab =>
            runtimePrefab;

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
                level
            );

            return ability;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;

            cooldown =
                Mathf.Max(
                    0.1f,
                    cooldown
                );
        }
    }
}