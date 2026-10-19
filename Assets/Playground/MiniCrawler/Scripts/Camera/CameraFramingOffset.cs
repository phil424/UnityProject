using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFramingOffset : MonoBehaviour
    {
        private static readonly Vector2 FullViewportCenter = new(0.5f, 0.5f);

        private Camera cameraComponent;

        private Vector2 currentViewportCenter = FullViewportCenter;
        private Vector2 transitionStart = FullViewportCenter;
        private Vector2 transitionTarget = FullViewportCenter;

        private float transitionDuration;
        private float transitionElapsed;

        public Vector2 CurrentViewportCenter => currentViewportCenter;
        public Vector2 TargetViewportCenter => transitionTarget;

        private void Awake()
        {
            cameraComponent = GetComponent<Camera>();
            SnapToViewportCenter(FullViewportCenter);
        }

        private void Update()
        {
            if (transitionDuration <= 0f)
                return;

            transitionElapsed = Mathf.Min(transitionDuration, transitionElapsed + Time.unscaledDeltaTime);

            float progress = Mathf.Clamp01(transitionElapsed / transitionDuration);

            float easedProgress = progress * progress * (3f - 2f * progress);

            currentViewportCenter = Vector2.Lerp(transitionStart, transitionTarget, easedProgress);

            if (progress < 1f)
                return;

            currentViewportCenter = transitionTarget;
            transitionDuration = 0f;
            transitionElapsed = 0f;
        }

        public void SetVisibleViewport(Rect visibleViewport, float transitionSeconds = 0f)
        {
            SetViewportCenter(visibleViewport.center, transitionSeconds);
        }

        public void SetViewportCenter(Vector2 viewportCenter, float transitionSeconds = 0f)
        {
            Vector2 clampedCenter = new(Mathf.Clamp01(viewportCenter.x), Mathf.Clamp01(viewportCenter.y));

            if (transitionSeconds <= 0f)
            {
                SnapToViewportCenter(clampedCenter);
                return;
            }

            transitionStart = currentViewportCenter;
            transitionTarget = clampedCenter;
            transitionDuration = transitionSeconds;
            transitionElapsed = 0f;
        }

        public void ResetFraming(float transitionSeconds = 0f)
        {
            SetViewportCenter(FullViewportCenter, transitionSeconds);
        }

        public Vector3 ResolveWorldOffset(Vector3 focusPoint)
        {
            if (cameraComponent == null)
                cameraComponent = GetComponent<Camera>();

            if (cameraComponent == null)
                return Vector3.zero;

            float depth = cameraComponent.WorldToViewportPoint(focusPoint).z;

            if (depth <= 0f)
                return Vector3.zero;

            Vector3 centeredWorld = cameraComponent.ViewportToWorldPoint(new Vector3(FullViewportCenter.x, FullViewportCenter.y, depth));

            Vector3 desiredWorld = cameraComponent.ViewportToWorldPoint(new Vector3(currentViewportCenter.x, currentViewportCenter.y, depth));

            return centeredWorld - desiredWorld;
        }

        private void SnapToViewportCenter(Vector2 viewportCenter)
        {
            currentViewportCenter = viewportCenter;
            transitionStart = viewportCenter;
            transitionTarget = viewportCenter;
            transitionDuration = 0f;
            transitionElapsed = 0f;
        }
    }
}
