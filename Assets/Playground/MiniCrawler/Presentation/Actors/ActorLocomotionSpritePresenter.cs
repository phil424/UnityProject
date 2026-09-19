using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteFlipbookPlayer))]
    public class ActorLocomotionSpritePresenter : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private SpriteFlipbookClip idleClip;
        [SerializeField] private SpriteFlipbookClip moveClip;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float movementSpeedThreshold = 0.05f;

        [Header("Facing")]
        [SerializeField, Range(0f, 1f)] private float facingDeadZone = 0.2f;
        [SerializeField] private bool sourceFacesRight;
        [SerializeField] private bool initialFacingRight = true;

        private Transform actorTransform;
        private Transform cameraTransform;
        private SpriteRenderer spriteRenderer;
        private SpriteFlipbookPlayer flipbookPlayer;
        private bool presentationSuppressed;
        
        private Vector3 baseLocalScale;

        private Vector3 previousPosition;
        private bool isMoving;
        private bool facingRight;

        private void Awake()
        {
            Actor actor = GetComponentInParent<Actor>();

            if (actor == null)
            {
                Debug.LogError($"{nameof(ActorLocomotionSpritePresenter)} requires an Actor in its parent hierarchy.", this);
                enabled = false;
                return;
            }

            actorTransform = actor.transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();

            baseLocalScale = transform.localScale;
            spriteRenderer.flipX = false;

            previousPosition = actorTransform.position;
            facingRight = initialFacingRight;

            ApplyFacing();
            flipbookPlayer.Play(idleClip);
        }

        private void Update()
        {
            if (actorTransform == null)
                return;

            Vector3 currentPosition = actorTransform.position;

            if (SimulationPause.IsPaused)
            {
                previousPosition = currentPosition;
                return;
            }
            
            if (presentationSuppressed)
            {
                previousPosition = currentPosition;
                return;
            }

            if (Time.deltaTime <= 0f)
            {
                previousPosition = currentPosition;
                return;
            }

            Vector3 movement = currentPosition - previousPosition;
            previousPosition = currentPosition;

            movement.y = 0f;

            float movementSpeed = movement.magnitude / Time.deltaTime;
            bool movingNow = movementSpeed >= movementSpeedThreshold;

            if (movingNow != isMoving)
            {
                isMoving = movingNow;
                flipbookPlayer.Play(isMoving ? moveClip : idleClip);
            }

            if (isMoving && movement.sqrMagnitude > 0.0001f)
                UpdateFacing(movement.normalized);
        }

        private void UpdateFacing(Vector3 movementDirection)
        {
            if (!TryResolveCamera())
                return;

            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;

            if (cameraRight.sqrMagnitude <= 0.0001f)
                return;

            cameraRight.Normalize();

            float horizontalMovement = Vector3.Dot(movementDirection, cameraRight);

            if (Mathf.Abs(horizontalMovement) < facingDeadZone)
                return;

            facingRight = horizontalMovement > 0f;
            ApplyFacing();
        }

        private void ApplyFacing()
        {
            bool mirrorHorizontally = sourceFacesRight ? !facingRight : facingRight;

            Vector3 scale = baseLocalScale;
            scale.x = Mathf.Abs(baseLocalScale.x) * (mirrorHorizontally ? -1f : 1f);

            transform.localScale = scale;
            spriteRenderer.flipX = false;
        }
        
        public void SetPresentationSuppressed(bool suppressed)
        {
            if (presentationSuppressed == suppressed)
                return;

            presentationSuppressed = suppressed;

            if (actorTransform != null)
                previousPosition = actorTransform.position;

            if (!presentationSuppressed)
            {
                isMoving = false;
                flipbookPlayer.Play(idleClip, true);
            }
        }

        public void FaceWorldDirection(Vector3 worldDirection)
        {
            worldDirection.y = 0f;

            if (worldDirection.sqrMagnitude <= 0.0001f)
                return;

            UpdateFacing(worldDirection.normalized);
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
    }
}