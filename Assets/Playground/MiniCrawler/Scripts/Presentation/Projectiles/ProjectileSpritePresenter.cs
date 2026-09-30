using MiniCrawler.Combat;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ProjectileSpritePresenter : MonoBehaviour
    {
        [SerializeField] private ProjectileRuntime projectile;
        [SerializeField] private bool spriteFacesRight = true;

        private void Awake()
        {
            if (projectile == null)
                projectile = GetComponentInParent<ProjectileRuntime>();
        }

        private void LateUpdate()
        {
            if (projectile == null)
                return;

            Camera camera = Camera.main;

            if (camera == null)
                return;

            Vector3 direction = projectile.TravelDirection;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            Vector3 cameraRight = camera.transform.right.normalized;
            Vector3 cameraUp = camera.transform.up.normalized;

            float screenX = Vector3.Dot(direction, cameraRight);
            float screenY = Vector3.Dot(direction, cameraUp);

            float angle = -Mathf.Atan2(screenY, screenX) * Mathf.Rad2Deg;

            if (spriteFacesRight)
                angle += 180f;

            Quaternion billboardRotation =
                Quaternion.LookRotation(-camera.transform.forward, camera.transform.up);

            transform.rotation =
                billboardRotation * Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}