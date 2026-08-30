using System;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    public abstract class ActorAbility :
        MonoBehaviour
    {
        private GameObject owner;

        private AbilityDefinition definition;

        private int level = 1;

        private float cooldownRemaining;

        private AbilityExecutionState
            executionState;

        public GameObject Owner =>
            owner;

        public AbilityDefinition Definition =>
            definition;

        public int Level =>
            level;

        public string AbilityName =>
            definition != null
                ? definition.DisplayName
                : GetType().Name;

        public float Cooldown =>
            definition != null
                ? definition.GetCooldown(
                    level
                )
                : 0f;

        public float CooldownRemaining =>
            cooldownRemaining;

        public bool IsReady =>
            cooldownRemaining <= 0f;

        public bool IsExecuting
        {
            get;
            private set;
        }

        public bool BlocksAutonomousActions
        {
            get;
            private set;
        }

        public bool IsInitialized =>
            owner != null &&
            definition != null;

        public bool CanActivateNow =>
            IsInitialized &&
            isActiveAndEnabled &&
            !SimulationPause.IsPaused &&
            IsOwnerAlive() &&
            IsReady &&
            !IsExecuting &&
            CanActivateAbility();

        public event Action<ActorAbility>
            Activated;

        public void Initialize(
            GameObject actorOwner,
            AbilityDefinition abilityDefinition,
            int abilityLevel
        )
        {
            EndExecution();

            owner =
                actorOwner;

            definition =
                abilityDefinition;

            level =
                abilityDefinition != null
                    ? abilityDefinition.ClampLevel(
                        abilityLevel
                    )
                    : Mathf.Max(
                        1,
                        abilityLevel
                    );

            cooldownRemaining = 0f;
        }

        public void TickRuntime(
            float deltaTime
        )
        {
            TickCooldown(
                deltaTime
            );

            if (!IsExecuting)
                return;

            if (!IsOwnerAlive())
            {
                EndExecution();

                return;
            }

            TickExecution(
                deltaTime
            );
        }

        public void TickCooldown(
            float deltaTime
        )
        {
            if (
                deltaTime <= 0f ||
                cooldownRemaining <= 0f
            )
            {
                return;
            }

            cooldownRemaining =
                Mathf.Max(
                    0f,
                    cooldownRemaining -
                    deltaTime
                );
        }

        public bool TryActivate()
        {
            if (!CanActivateNow)
                return false;

            if (!ExecuteAbility())
                return false;

            cooldownRemaining =
                Cooldown;

            Activated?.Invoke(this);

            return true;
        }

        public void ResetCooldown()
        {
            cooldownRemaining = 0f;
        }

        protected bool BeginExecution(
            bool blockAutonomousActions = true
        )
        {
            if (
                !IsInitialized ||
                IsExecuting
            )
            {
                return false;
            }

            IsExecuting = true;

            BlocksAutonomousActions =
                blockAutonomousActions;

            if (BlocksAutonomousActions)
            {
                EnsureExecutionState();

                executionState?.BeginBlocking(
                    this
                );
            }

            return true;
        }

        protected void EndExecution()
        {
            if (!IsExecuting)
                return;

            if (
                BlocksAutonomousActions &&
                executionState != null
            )
            {
                executionState.EndBlocking(
                    this
                );
            }

            IsExecuting = false;

            BlocksAutonomousActions = false;
        }

        protected abstract bool
            CanActivateAbility();

        protected abstract bool
            ExecuteAbility();

        protected virtual void TickExecution(
            float deltaTime
        )
        {
        }

        private void EnsureExecutionState()
        {
            if (
                executionState != null ||
                owner == null
            )
            {
                return;
            }

            executionState =
                owner.GetComponent<
                    AbilityExecutionState
                >();

            if (executionState == null)
            {
                executionState =
                    owner.AddComponent<
                        AbilityExecutionState
                    >();
            }
        }

        private bool IsOwnerAlive()
        {
            if (owner == null)
                return false;

            Health health =
                owner.GetComponent<Health>();

            return
                health == null ||
                !health.IsDead;
        }

        [ContextMenu("Debug/Try Activate")]
        private void DebugTryActivate()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning(
                    $"[Ability Debug] " +
                    $"{AbilityName} " +
                    "can only be activated " +
                    "in Play Mode."
                );

                return;
            }

            bool activated =
                TryActivate();

            Debug.Log(
                $"[Ability Debug] " +
                $"{AbilityName} activation " +
                $"request " +
                $"{(activated ? "succeeded" : "was rejected")}."
            );
        }

        protected virtual void OnDisable()
        {
            EndExecution();
        }

        protected virtual void OnValidate()
        {
        }
    }
}