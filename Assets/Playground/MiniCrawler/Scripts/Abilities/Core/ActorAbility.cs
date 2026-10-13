using System;
using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    public abstract class ActorAbility : MonoBehaviour
    {
        private GameObject owner;

        private AbilityDefinition definition;

        private int level = 1;

        private IReadOnlyList<AbilityEvolutionDefinition> evolutions = Array.Empty<AbilityEvolutionDefinition>();

        private float cooldownRemaining;

        private AbilityExecutionState executionState;

        public GameObject Owner => owner;

        public AbilityDefinition Definition => definition;

        public int Level => level;

        public IReadOnlyList<AbilityEvolutionDefinition> Evolutions => evolutions;

        public string AbilityName => definition != null ? definition.DisplayName : GetType().Name;

        public string DisplayName => AbilityPresentationResolver.GetDisplayName(definition, evolutions);

        public Sprite DisplayIcon => AbilityPresentationResolver.GetDisplayIcon(definition, evolutions);

        public float Cooldown
        {
            get
            {
                float baseCooldown = definition != null ? definition.GetCooldown(level) : 0f;

                AbilityStats stats = owner != null ? owner.GetComponent<AbilityStats>() : null;
                return stats != null ? stats.GetModifiedCooldown(baseCooldown) : baseCooldown;
            }
        }

        public float CooldownRemaining => cooldownRemaining;

        public bool IsReady => cooldownRemaining <= 0f;

        public bool IsExecuting { get; private set; }

        public bool BlocksAutonomousActions { get; private set; }

        public bool IsInitialized => owner != null && definition != null;

        public bool CanActivateNow =>
            IsInitialized && isActiveAndEnabled && !SimulationPause.IsPaused && IsOwnerAlive() && !IsOwnerStunned() && !IsOwnerFeared() && IsReady && !IsExecuting && CanActivateAbility();

        public event Action<ActorAbility> Activated;

        public void Initialize(GameObject actorOwner, AbilityDefinition abilityDefinition, int abilityLevel, IReadOnlyList<AbilityEvolutionDefinition> abilityEvolutions = null)
        {
            EndExecution();

            owner = actorOwner;

            definition = abilityDefinition;

            ApplyProgressionState(abilityLevel, abilityEvolutions);

            cooldownRemaining = 0f;
        }

        public void ApplyProgressionState(int abilityLevel, IReadOnlyList<AbilityEvolutionDefinition> abilityEvolutions = null)
        {
            if (definition == null)
                return;

            level = definition.ClampLevel(abilityLevel);
            evolutions = abilityEvolutions ?? Array.Empty<AbilityEvolutionDefinition>();
        }

        public void TickRuntime(float deltaTime)
        {
            TickCooldown(deltaTime);

            if (!IsExecuting)
                return;

            if (!IsOwnerAlive())
            {
                EndExecution();

                return;
            }

            TickExecution(deltaTime);
        }

        public void TickCooldown(float deltaTime)
        {
            if (deltaTime <= 0f || cooldownRemaining <= 0f)
                return;

            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - deltaTime);
        }

        public bool TryActivate()
        {
            if (!CanActivateNow)
                return false;

            if (!ExecuteAbility())
                return false;

            cooldownRemaining = Cooldown;

            Activated?.Invoke(this);

            return true;
        }

        public void ResetCooldown()
        {
            cooldownRemaining = 0f;
        }

        public bool HasEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution == null)
                return false;

            foreach (AbilityEvolutionDefinition owned in evolutions)
            {
                if (owned != null && owned.Id == evolution.Id)
                    return true;
            }

            return false;
        }

        protected bool BeginExecution(bool blockAutonomousActions = true)
        {
            if (!IsInitialized || IsExecuting)
                return false;

            IsExecuting = true;

            BlocksAutonomousActions = blockAutonomousActions;

            if (BlocksAutonomousActions)
            {
                CombatActionState combatActionState = owner.GetComponent<CombatActionState>();
                combatActionState?.Cancel();

                EnsureExecutionState();
                executionState?.BeginBlocking(this);
            }

            return true;
        }

        protected void EndExecution()
        {
            if (!IsExecuting)
                return;

            if (BlocksAutonomousActions && executionState != null)
                executionState.EndBlocking(this);

            IsExecuting = false;

            BlocksAutonomousActions = false;
        }

        protected abstract bool CanActivateAbility();

        protected abstract bool ExecuteAbility();

        protected virtual void TickExecution(float deltaTime) { }

        private void EnsureExecutionState()
        {
            if (executionState != null || owner == null)
                return;

            executionState = owner.GetComponent<AbilityExecutionState>();

            if (executionState == null)
                executionState = owner.AddComponent<AbilityExecutionState>();
        }

        private bool IsOwnerAlive()
        {
            if (owner == null)
                return false;

            Health health = owner.GetComponent<Health>();

            return health == null || !health.IsDead;
        }

        private bool IsOwnerStunned()
        {
            if (owner == null)
                return false;

            StunRuntime stun = owner.GetComponent<StunRuntime>();

            return stun != null && stun.IsStunned;
        }

        private bool IsOwnerFeared()
        {
            if (owner == null)
                return false;

            FearRuntime fear = owner.GetComponent<FearRuntime>();

            return fear != null && fear.IsFeared;
        }

        [ContextMenu("Debug/Try Activate")]
        private void DebugTryActivate()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning($"[Ability Debug] " + $"{AbilityName} " + "can only be activated " + "in Play Mode.");
                return;
            }

            bool activated = TryActivate();

            Debug.Log($"[Ability Debug] " + $"{AbilityName} activation " + $"request " + $"{(activated ? "succeeded" : "was rejected")}.");
        }

        protected virtual void OnDisable()
        {
            EndExecution();
        }

        protected virtual void OnValidate() { }
    }
}
