using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public sealed class ActorAbilityEffectPresenter : MonoBehaviour
    {
        [Serializable]
        private sealed class AbilityEffectPresentation
        {
            [SerializeField] private AbilityDefinition ability;
            [SerializeField] private SpriteEffectDefinition activationEffect;
            [SerializeField] private SpriteEffectDefinition activeEffect;
            [SerializeField] private bool useTimedBuffDuration;
            [SerializeField, Min(0.01f)] private float activeEffectDuration = 1f;

            public AbilityDefinition Ability => ability;
            public SpriteEffectDefinition ActivationEffect => activationEffect;
            public SpriteEffectDefinition ActiveEffect => activeEffect;

            public float ResolveActiveEffectDuration(ActorAbility runtimeAbility)
            {
                if (useTimedBuffDuration && runtimeAbility is TimedStatBuffAbility timedBuff)
                    return timedBuff.Duration;

                return Mathf.Max(0.01f, activeEffectDuration);
            }
        }

        [SerializeField] private AbilityEffectPresentation[] abilityEffects;

        private readonly List<ActorAbility> subscribedAbilities = new();

        private Actor actor;

        private void Start()
        {
            actor = GetComponentInParent<Actor>();

            if (actor == null)
            {
                Debug.LogError($"{nameof(ActorAbilityEffectPresenter)} requires an Actor in its parent hierarchy.", this);
                enabled = false;
                return;
            }

            SubscribeStartingAbilities();
        }

        private void OnDisable()
        {
            foreach (ActorAbility ability in subscribedAbilities)
            {
                if (ability != null)
                    ability.Activated -= HandleAbilityActivated;
            }

            subscribedAbilities.Clear();
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
            AbilityEffectPresentation presentation = ResolvePresentation(ability);

            if (presentation == null)
                return;

            if (presentation.ActivationEffect != null)
                SpriteEffectRuntime.Spawn(presentation.ActivationEffect, actor.transform.position);

            if (presentation.ActiveEffect != null)
            {
                SpriteEffectRuntime.SpawnAttached(
                    presentation.ActiveEffect,
                    actor.transform,
                    presentation.ResolveActiveEffectDuration(ability)
                );
            }
        }

        private AbilityEffectPresentation ResolvePresentation(ActorAbility ability)
        {
            if (ability?.Definition == null || abilityEffects == null)
                return null;

            foreach (AbilityEffectPresentation presentation in abilityEffects)
            {
                if (presentation != null && presentation.Ability == ability.Definition)
                    return presentation;
            }

            return null;
        }
    }
}