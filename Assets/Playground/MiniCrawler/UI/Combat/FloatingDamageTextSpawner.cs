using MiniCrawler.Combat;
using UnityEngine;

namespace MiniCrawler.UI
{
    public class FloatingDamageTextSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private FloatingDamageText floatingDamageTextPrefab;

        [SerializeField]
        private Transform spawnedTextParent;

        [Header("Position")]
        [SerializeField]
        private Vector3 worldOffset = new Vector3(0f, 1.25f, 0f);

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            DamageResolver.DamageResolved += HandleDamageResolved;
        }

        private void OnDisable()
        {
            DamageResolver.DamageResolved -= HandleDamageResolved;
        }

        private void HandleDamageResolved(DamageEvent damageEvent)
        {
            if (floatingDamageTextPrefab == null)
                return;

            if (damageEvent.Target == null)
                return;

            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector3 spawnPosition =
                damageEvent.Target.transform.position + worldOffset;

            FloatingDamageText spawnedText = Instantiate(
                floatingDamageTextPrefab,
                spawnPosition,
                Quaternion.identity,
                spawnedTextParent
            );

            spawnedText.Initialize(
                damageEvent.FinalDamage,
                mainCamera
            );
        }
    }
}