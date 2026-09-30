using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public class ActorCombatEffectPresenter : MonoBehaviour
    {
        [Header("Action Impact")]
        [SerializeField] private SpriteEffectDefinition actionImpactEffect;
        [SerializeField, Min(0f)] private float actionEffectDistance = 0.65f;

        [Header("Received Damage")]
        [SerializeField] private SpriteEffectDefinition receivedDamageEffect;

        private CombatActionState actionState;
        private Health actorHealth;

        private void Awake()
        {
            actionState = GetComponentInParent<CombatActionState>();
            actorHealth = GetComponentInParent<Health>();

            if (actorHealth == null)
            {
                Debug.LogError(
                    $"{nameof(ActorCombatEffectPresenter)} requires Health in its parent hierarchy.",
                    this
                );

                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (actionState != null)
                actionState.Impacted += HandleActionImpacted;

            DamageResolver.DamageResolved += HandleDamageResolved;
        }

        private void OnDisable()
        {
            if (actionState != null)
                actionState.Impacted -= HandleActionImpacted;

            DamageResolver.DamageResolved -= HandleDamageResolved;
        }

        private void HandleActionImpacted(CombatActionState state)
        {
            if (actionImpactEffect == null)
                return;

            Vector3 sourcePosition = state.transform.position;
            Vector3 toTarget = Vector3.zero;

            if (state.Target != null)
            {
                toTarget = state.Target.transform.position - sourcePosition;
                toTarget.y = 0f;
            }

            Vector3 effectPosition = sourcePosition;

            if (toTarget.sqrMagnitude > 0.0001f)
                effectPosition += toTarget.normalized * actionEffectDistance;

            SpriteEffectRuntime.Spawn(
                actionImpactEffect,
                effectPosition,
                ShouldMirrorHorizontally(toTarget, actionImpactEffect.SourceFacesRight)
            );
        }

        private void HandleDamageResolved(DamageEvent damageEvent)
        {
            if (receivedDamageEffect == null ||
                actorHealth == null ||
                damageEvent.Target != actorHealth ||
                damageEvent.FinalDamage <= 0f)
            {
                return;
            }

            SpriteEffectRuntime.Spawn(receivedDamageEffect, actorHealth.transform.position);
        }

        private static bool ShouldMirrorHorizontally(Vector3 worldDirection, bool sourceFacesRight)
        {
            worldDirection.y = 0f;

            if (worldDirection.sqrMagnitude <= 0.0001f)
                return false;

            Camera camera = Camera.main;

            if (camera == null)
                return false;

            Vector3 cameraRight = camera.transform.right;
            cameraRight.y = 0f;

            if (cameraRight.sqrMagnitude <= 0.0001f)
                return false;

            bool targetIsRight = Vector3.Dot(worldDirection.normalized, cameraRight.normalized) > 0f;

            return sourceFacesRight ? !targetIsRight : targetIsRight;
        }
    }
}