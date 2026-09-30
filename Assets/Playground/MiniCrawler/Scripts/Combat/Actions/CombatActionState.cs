using System;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public enum CombatActionPhase
    {
        None,
        Windup,
        Recovery
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CombatStats))]
    public class CombatActionState : MonoBehaviour
    {
        [Header("Basic Attack")]
        [SerializeField] private CombatActionDefinition basicAttackAction;
        [SerializeField] private CombatActionSequenceDefinition basicAttackSequence;

        public event Action<CombatActionState> Started;
        public event Action<CombatActionState> Impacted;
        public event Action<CombatActionState> Ended;

        public CombatActionDefinition BasicAttackAction => basicAttackAction;
        public CombatActionSequenceDefinition BasicAttackSequence => basicAttackSequence;

        public CombatActionDefinition CurrentDefinition { get; private set; }
        public Health Target { get; private set; }

        public float Duration { get; private set; }
        public float Elapsed { get; private set; }

        public int CurrentBasicAttackStep { get; private set; }
        public Health BasicAttackChainTarget { get; private set; }

        public bool ImpactApplied { get; private set; }
        public bool ImpactConnected { get; private set; }
        public bool IsExecuting { get; private set; }
        public bool BlocksAutonomousActions => IsExecuting;

        public bool HasPendingBasicAttackContinuation =>
            !IsExecuting &&
            CurrentBasicAttackStep > 0 &&
            BasicAttackChainTarget != null;

        public float NormalizedTime => Duration <= 0f ? 1f : Mathf.Clamp01(Elapsed / Duration);

        public CombatActionPhase Phase =>
            !IsExecuting
                ? CombatActionPhase.None
                : ImpactApplied
                    ? CombatActionPhase.Recovery
                    : CombatActionPhase.Windup;

        internal bool ShouldApplyImpact =>
            IsExecuting &&
            !ImpactApplied &&
            CurrentDefinition != null &&
            NormalizedTime >= CurrentDefinition.ImpactNormalizedTime;

        internal bool ShouldComplete =>
            IsExecuting &&
            Elapsed >= Duration;

        public CombatActionDefinition GetNextBasicAttack()
        {
            if (basicAttackSequence == null || basicAttackSequence.StepCount <= 0)
                return basicAttackAction;

            int step = Mathf.Clamp(CurrentBasicAttackStep, 0, basicAttackSequence.StepCount - 1);
            return basicAttackSequence.GetStep(step);
        }

        public bool TryBeginBasicAttack(Health target, float baseDuration)
        {
            if (target == null || target.IsDead)
                return false;

            if (CurrentBasicAttackStep > 0 && target != BasicAttackChainTarget)
                ResetBasicAttackSequence();

            CombatActionDefinition definition = GetNextBasicAttack();

            if (definition == null)
                return false;

            float duration = baseDuration * definition.DurationMultiplier;

            if (!TryBegin(definition, target, duration))
                return false;

            if (CurrentBasicAttackStep == 0)
                BasicAttackChainTarget = target;

            return true;
        }

        public bool TryBegin(CombatActionDefinition definition, Health target, float duration)
        {
            if (IsExecuting || definition == null || target == null || target.IsDead || duration <= 0f)
                return false;

            CurrentDefinition = definition;
            Target = target;

            Duration = duration;
            Elapsed = 0f;
            ImpactApplied = false;
            ImpactConnected = false;
            IsExecuting = true;

            Started?.Invoke(this);

            return true;
        }

        internal void Advance(float deltaTime)
        {
            if (!IsExecuting || deltaTime <= 0f)
                return;

            Elapsed = Mathf.Min(Duration, Elapsed + deltaTime);
        }

        internal void MarkImpactApplied(bool connected)
        {
            if (!IsExecuting || ImpactApplied)
                return;

            ImpactApplied = true;
            ImpactConnected = connected;
            Impacted?.Invoke(this);
        }

        internal void Complete()
        {
            if (!IsExecuting)
                return;

            CombatActionDefinition completedDefinition = CurrentDefinition;
            Health completedTarget = Target;
            bool completedImpactConnected = ImpactConnected;

            IsExecuting = false;

            Ended?.Invoke(this);

            if (completedImpactConnected)
                AdvanceBasicAttackStep(completedDefinition, completedTarget);
            else
                ResetBasicAttackSequence();

            ClearCurrentAction();
        }

        public void Cancel()
        {
            if (!IsExecuting)
                return;

            IsExecuting = false;

            Ended?.Invoke(this);

            ResetBasicAttackSequence();
            ClearCurrentAction();
        }

        public void ResetBasicAttackSequence()
        {
            CurrentBasicAttackStep = 0;
            BasicAttackChainTarget = null;
        }

        private void AdvanceBasicAttackStep(CombatActionDefinition completedDefinition, Health completedTarget)
        {
            if (basicAttackSequence == null || basicAttackSequence.StepCount <= 0)
            {
                ResetBasicAttackSequence();
                return;
            }

            CombatActionDefinition expectedStep = basicAttackSequence.GetStep(CurrentBasicAttackStep);

            if (completedDefinition != expectedStep)
            {
                ResetBasicAttackSequence();
                return;
            }

            CurrentBasicAttackStep++;

            if (CurrentBasicAttackStep >= basicAttackSequence.StepCount)
            {
                ResetBasicAttackSequence();
                return;
            }

            BasicAttackChainTarget = completedTarget;
        }

        private void ClearCurrentAction()
        {
            CurrentDefinition = null;
            Target = null;
            Duration = 0f;
            Elapsed = 0f;
            ImpactApplied = false;
            ImpactConnected = false;
        }

        private void OnDisable()
        {
            IsExecuting = false;
            ResetBasicAttackSequence();
            ClearCurrentAction();
        }
    }
}