using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteFlipbookPlayer))]
    [RequireComponent(typeof(ActorLocomotionSpritePresenter))]
    public class ActorCombatSpritePresenter : MonoBehaviour
    {
        [System.Serializable]
        private class ActionPresentation
        {
            [SerializeField] private CombatActionDefinition action;
            [SerializeField] private SpriteFlipbookClip clip;

            public CombatActionDefinition Action => action;
            public SpriteFlipbookClip Clip => clip;
        }
        
        [Header("Basic Attack")]
        [SerializeField] private SpriteFlipbookClip attackClip;
        [SerializeField] private ActionPresentation[] actionPresentations;

        [Header("Hit Reaction")]
        [SerializeField] private SpriteFlipbookClip hitClip;
        [SerializeField, Min(0.01f)] private float hitReactionDuration = 0.15f;

        private CombatActionState actionState;
        private Health actorHealth;
        private SpriteFlipbookPlayer flipbookPlayer;
        private ActorLocomotionSpritePresenter locomotionPresenter;

        private bool hitReacting;
        private float hitReactionRemaining;

        private void Awake()
        {
            actionState = GetComponentInParent<CombatActionState>();
            actorHealth = GetComponentInParent<Health>();
            flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();
            locomotionPresenter = GetComponent<ActorLocomotionSpritePresenter>();

            if (actionState == null)
            {
                Debug.LogError($"{nameof(ActorCombatSpritePresenter)} requires a CombatActionState in its parent hierarchy.", this);
                enabled = false;
                return;
            }

            if (actorHealth == null)
            {
                Debug.LogError($"{nameof(ActorCombatSpritePresenter)} requires Health in its parent hierarchy.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (actionState != null)
            {
                actionState.Started += HandleActionStarted;
                actionState.Ended += HandleActionEnded;
            }

            DamageResolver.DamageResolved += HandleDamageResolved;
        }

        private void OnDisable()
        {
            if (actionState != null)
            {
                actionState.Started -= HandleActionStarted;
                actionState.Ended -= HandleActionEnded;
            }

            DamageResolver.DamageResolved -= HandleDamageResolved;

            hitReacting = false;
            hitReactionRemaining = 0f;

            if (locomotionPresenter != null)
                locomotionPresenter.SetPresentationSuppressed(false);
        }

        private void Update()
        {
            if (!hitReacting || Time.deltaTime <= 0f)
                return;

            hitReactionRemaining -= Time.deltaTime;

            if (hitReactionRemaining <= 0f)
                EndHitReaction();
        }

        private void HandleActionStarted(CombatActionState state)
        {
            locomotionPresenter.SetPresentationSuppressed(true);

            if (state.Target != null)
            {
                Vector3 toTarget = state.Target.transform.position - state.transform.position;
                locomotionPresenter.FaceWorldDirection(toTarget);
            }

            if (!hitReacting)
                PlayAttackPresentation(state);
        }

        private void HandleActionEnded(CombatActionState state)
        {
            if (hitReacting)
                return;

            locomotionPresenter.SetPresentationSuppressed(false);
        }

        private void HandleDamageResolved(DamageEvent damageEvent)
        {
            if (actorHealth == null ||
                damageEvent.Target != actorHealth ||
                damageEvent.FinalDamage <= 0f ||
                actorHealth.IsDead ||
                hitClip == null)
            {
                return;
            }

            BeginHitReaction();
        }

        private void BeginHitReaction()
        {
            hitReacting = true;
            hitReactionRemaining = hitReactionDuration;

            locomotionPresenter.SetPresentationSuppressed(true);

            float playbackSpeed =
                hitClip.Duration > 0f && hitReactionDuration > 0f
                    ? hitClip.Duration / hitReactionDuration
                    : 1f;

            flipbookPlayer.Play(hitClip, true, playbackSpeed);
        }

        private void EndHitReaction()
        {
            hitReacting = false;
            hitReactionRemaining = 0f;

            if (actionState != null && actionState.IsExecuting)
            {
                locomotionPresenter.SetPresentationSuppressed(true);
                PlayAttackPresentation(actionState);
                return;
            }

            locomotionPresenter.SetPresentationSuppressed(false);
        }

        private void PlayAttackPresentation(CombatActionState state)
        {
            SpriteFlipbookClip resolvedClip = ResolveAttackClip(state);

            if (resolvedClip == null)
                return;

            float playbackSpeed =
                state.Duration > 0f && resolvedClip.Duration > 0f
                    ? resolvedClip.Duration / state.Duration
                    : 1f;

            flipbookPlayer.Play(resolvedClip, true, playbackSpeed, state.NormalizedTime);
        }
        
        private SpriteFlipbookClip ResolveAttackClip(CombatActionState state)
        {
            if (state != null && state.CurrentDefinition != null && actionPresentations != null)
            {
                foreach (ActionPresentation presentation in actionPresentations)
                {
                    if (presentation != null &&
                        presentation.Action == state.CurrentDefinition &&
                        presentation.Clip != null)
                    {
                        return presentation.Clip;
                    }
                }
            }

            return attackClip;
}

    }
}