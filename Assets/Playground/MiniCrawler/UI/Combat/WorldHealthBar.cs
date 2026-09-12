using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class WorldHealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Health health;

        [SerializeField]
        private RectTransform fillRect;

        private Camera targetCamera;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (health != null)
            {
                health.Changed +=
                    HandleHealthChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Changed -=
                    HandleHealthChanged;
            }
        }

        private void LateUpdate()
        {
            FaceCamera();
        }

        private void HandleHealthChanged(
            Health changedHealth
        )
        {
            Refresh();
        }

        private void Refresh()
        {
            if (
                health == null ||
                fillRect == null
            )
            {
                return;
            }

            Vector2 anchorMax =
                fillRect.anchorMax;

            anchorMax.x =
                health.Normalized;

            fillRect.anchorMax =
                anchorMax;
        }

        private void FaceCamera()
        {
            if (targetCamera == null)
            {
                targetCamera =
                    Camera.main;
            }

            if (targetCamera == null)
                return;

            transform.rotation =
                targetCamera.transform.rotation;
        }

        private void ResolveReferences()
        {
            if (health == null)
            {
                health =
                    GetComponentInParent<Health>();
            }

            if (targetCamera == null)
            {
                targetCamera =
                    Camera.main;
            }
        }
    }
}