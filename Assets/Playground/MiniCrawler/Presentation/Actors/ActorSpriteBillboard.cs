using UnityEngine;

namespace MiniCrawler.Presentation
{
    public enum ActorSpriteBillboardMode
    {
        CameraAligned,
        Vertical
    }

    [DisallowMultipleComponent]
    public class ActorSpriteBillboard : MonoBehaviour
    {
        [SerializeField] private ActorSpriteBillboardMode mode = ActorSpriteBillboardMode.CameraAligned;
        [SerializeField] private Transform cameraTransform;

        private void LateUpdate()
        {
            if (!TryResolveCamera())
                return;

            switch (mode)
            {
                case ActorSpriteBillboardMode.CameraAligned:
                    FaceCameraPlane();
                    break;

                case ActorSpriteBillboardMode.Vertical:
                    FaceCameraHorizontally();
                    break;
            }
        }

        private bool TryResolveCamera()
        {
            if (cameraTransform != null)
                return true;

            Camera mainCamera = Camera.main;

            if (mainCamera == null)
                return false;

            cameraTransform = mainCamera.transform;
            return true;
        }

        private void FaceCameraPlane()
        {
            transform.rotation = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
        }

        private void FaceCameraHorizontally()
        {
            Vector3 toCamera = cameraTransform.position - transform.position;
            toCamera.y = 0f;

            if (toCamera.sqrMagnitude <= 0.0001f)
                return;

            transform.rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
        }
    }
}