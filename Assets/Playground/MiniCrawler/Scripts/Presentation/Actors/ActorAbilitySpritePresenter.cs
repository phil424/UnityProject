using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DefaultExecutionOrder(-40)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteFlipbookPlayer))]
    [RequireComponent(typeof(ActorLocomotionSpritePresenter))]
    public sealed class ActorAbilitySpritePresenter : MonoBehaviour
    {
        private enum AbilityFacingMode
        {
            None,
            CurrentTarget,
            ForcedMotionDirection
        }

        [Serializable]
        private sealed class AbilityPresentation
        {
            [SerializeField] private AbilityDefinition ability;
            [SerializeField] private SpriteFlipbookClip clip;
            [SerializeField, Min(0.01f)] private float presentationDuration = 0.5f;
            [SerializeField] private AbilityFacingMode facingMode = AbilityFacingMode.CurrentTarget;
            [SerializeField] private bool endWithAbilityExecution = true;

            public AbilityDefinition Ability => ability;
            public SpriteFlipbookClip Clip => clip;
            public float PresentationDuration => Mathf.Max(0.01f, presentationDuration);
            public AbilityFacingMode FacingMode => facingMode;
            public bool EndWithAbilityExecution => endWithAbilityExecution;
        }

        [SerializeField] private AbilityPresentation[] abilityPresentations;

        private readonly List<ActorAbility> subscribedAbilities = new();

        private Actor actor;
        private AutoTargetMover mover;
        private CombatActionState combatActionState;
        private SpriteFlipbookPlayer flipbookPlayer;
        private ActorLocomotionSpritePresenter locomotionPresenter;

        private ActorAbility currentAbility;
        private AbilityPresentation currentPresentation;

        private float presentationRemaining;
        private bool presenting;

        private void Start()
        {
            actor = GetComponentInParent<Actor>();
            flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();
            locomotionPresenter = GetComponent<ActorLocomotionSpritePresenter>();

            if (actor == null)
            {
                Debug.LogError($"{nameof(ActorAbilitySpritePresenter)} requires an Actor in its parent hierarchy.", this);
                enabled = false;
                return;
            }

            mover = actor.GetComponent<AutoTargetMover>();
            combatActionState = actor.GetComponent<CombatActionState>();

            SubscribeStartingAbilities();
        }

        private void Update()
        {
            if (!presenting || SimulationPause.IsPaused || Time.deltaTime <= 0f)
                return;

            if (currentPresentation != null &&
                currentPresentation.EndWithAbilityExecution &&
                currentAbility != null &&
                !currentAbility.IsExecuting)
            {
                EndPresentation();
                return;
            }

            presentationRemaining -= Time.deltaTime;

            if (presentationRemaining <= 0f)
                EndPresentation();
        }

        private void OnDisable()
        {
            foreach (ActorAbility ability in subscribedAbilities)
            {
                if (ability != null)
                    ability.Activated -= HandleAbilityActivated;
            }

            subscribedAbilities.Clear();
            EndPresentation();
        }

        private void SubscribeStartingAbilities()
        {
            ActorAbility[] abilities = actor.GetComponentsInChildren<ActorAbility>(true);

            foreach (ActorAbility ability in abilities)
            {
                if (ability == null || subscribedAbilities.Contains(ability))
                    continue;

                ability.Activated += HandleAbilityActivated;
                subscribedAbilities.Add(ability);
            }
        }

        private void HandleAbilityActivated(ActorAbility ability)
        {
            AbilityPresentation presentation = ResolvePresentation(ability);

            if (presentation?.Clip == null)
                return;

            ApplyFacing(presentation);

            locomotionPresenter.SetPresentationSuppressed(true);

            float playbackSpeed =
                presentation.Clip.Duration > 0f
                    ? presentation.Clip.Duration / presentation.PresentationDuration
                    : 1f;

            flipbookPlayer.Play(presentation.Clip, true, playbackSpeed);

            currentAbility = ability;
            currentPresentation = presentation;
            presentationRemaining = presentation.PresentationDuration;
            presenting = true;
        }

        private void ApplyFacing(AbilityPresentation presentation)
        {
            if (presentation == null || actor == null)
                return;

            switch (presentation.FacingMode)
            {
                case AbilityFacingMode.CurrentTarget:
                    if (mover?.CurrentTarget != null)
                    {
                        Vector3 toTarget = mover.CurrentTarget.transform.position - actor.transform.position;
                        locomotionPresenter.FaceWorldDirection(toTarget);
                    }
                    break;

                case AbilityFacingMode.ForcedMotionDirection:
                    ForcedMotion forcedMotion = actor.GetComponent<ForcedMotion>();

                    if (forcedMotion != null && forcedMotion.IsActive)
                        locomotionPresenter.FaceWorldDirection(forcedMotion.Direction);
                    break;
            }
        }

        private AbilityPresentation ResolvePresentation(ActorAbility ability)
        {
            if (ability?.Definition == null || abilityPresentations == null)
                return null;

            foreach (AbilityPresentation presentation in abilityPresentations)
            {
                if (presentation != null && presentation.Ability == ability.Definition)
                    return presentation;
            }

            return null;
        }

        private void EndPresentation()
        {
            if (!presenting)
                return;

            presenting = false;
            presentationRemaining = 0f;
            currentAbility = null;
            currentPresentation = null;

            if (locomotionPresenter != null &&
                (combatActionState == null || !combatActionState.IsExecuting))
            {
                locomotionPresenter.SetPresentationSuppressed(false);
            }
        }
    }
}