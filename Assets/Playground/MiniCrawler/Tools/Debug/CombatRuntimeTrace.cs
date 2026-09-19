using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Presentation;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DisallowMultipleComponent]
    public class CombatRuntimeTrace : MonoBehaviour
    {
        [Header("Output")]
        [SerializeField] private bool logActionEvents = true;
        [SerializeField] private bool logAnimationChanges = true;
        [SerializeField] private bool logDamage = true;

        [Header("Optional Filter")]
        [SerializeField] private string actorNameContains;

        private sealed class TrackedActor
        {
            public CombatActionState ActionState;
            public Actor Actor;
            public SpriteFlipbookPlayer FlipbookPlayer;
            public SpriteFlipbookClip LastClip;
            public int LastComboStep;
        }

        private readonly Dictionary<int, TrackedActor> trackedActors = new();

        private void OnEnable()
        {
            DamageResolver.DamageResolved += HandleDamageResolved;
        }

        private void OnDisable()
        {
            DamageResolver.DamageResolved -= HandleDamageResolved;

            foreach (TrackedActor tracked in trackedActors.Values)
                Unsubscribe(tracked);

            trackedActors.Clear();
        }

        private void Update()
        {
            DiscoverActors();
            PollTrackedActors();
            RemoveMissingActors();
        }

        private void DiscoverActors()
        {
            CombatActionState[] actionStates =
                FindObjectsByType<CombatActionState>(FindObjectsSortMode.None);

            foreach (CombatActionState actionState in actionStates)
            {
                if (actionState == null)
                    continue;

                int instanceId = actionState.GetInstanceID();

                if (trackedActors.ContainsKey(instanceId))
                    continue;

                Actor actor = actionState.GetComponent<Actor>();

                if (!ShouldTrace(actor))
                    continue;

                SpriteFlipbookPlayer flipbookPlayer =
                    actionState.GetComponentInChildren<SpriteFlipbookPlayer>();

                TrackedActor tracked = new TrackedActor
                {
                    ActionState = actionState,
                    Actor = actor,
                    FlipbookPlayer = flipbookPlayer,
                    LastClip = flipbookPlayer != null ? flipbookPlayer.CurrentClip : null,
                    LastComboStep = actionState.CurrentBasicAttackStep
                };

                trackedActors.Add(instanceId, tracked);

                actionState.Started += HandleActionStarted;
                actionState.Impacted += HandleActionImpacted;
                actionState.Ended += HandleActionEnded;

                Log(
                    $"TRACK actor={GetActorName(actor)} " +
                    $"step={actionState.CurrentBasicAttackStep + 1}"
                );
            }
        }

        private void PollTrackedActors()
        {
            foreach (TrackedActor tracked in trackedActors.Values)
            {
                if (tracked.ActionState == null)
                    continue;

                if (tracked.LastComboStep != tracked.ActionState.CurrentBasicAttackStep)
                {
                    Log(
                        $"COMBO actor={GetActorName(tracked.Actor)} " +
                        $"step {tracked.LastComboStep + 1} -> " +
                        $"{tracked.ActionState.CurrentBasicAttackStep + 1}"
                    );

                    tracked.LastComboStep = tracked.ActionState.CurrentBasicAttackStep;
                }

                if (!logAnimationChanges || tracked.FlipbookPlayer == null)
                    continue;

                SpriteFlipbookClip currentClip = tracked.FlipbookPlayer.CurrentClip;

                if (currentClip == tracked.LastClip)
                    continue;

                Log(
                    $"ANIM actor={GetActorName(tracked.Actor)} " +
                    $"clip={GetClipName(currentClip)} " +
                    $"action={GetActionName(tracked.ActionState.CurrentDefinition)} " +
                    $"step={tracked.ActionState.CurrentBasicAttackStep + 1}"
                );

                tracked.LastClip = currentClip;
            }
        }

        private void HandleActionStarted(CombatActionState state)
        {
            if (!logActionEvents || !TryGetTracked(state, out TrackedActor tracked))
                return;

            CombatActionDefinition definition = state.CurrentDefinition;

            Log(
                $"START actor={GetActorName(tracked.Actor)} " +
                $"step={state.CurrentBasicAttackStep + 1} " +
                $"action={GetActionName(definition)} " +
                $"target={GetTargetName(state.Target)} " +
                $"duration={state.Duration:0.000}s " +
                $"impact={GetImpactTime(definition):0.00} " +
                $"damageCoeff={GetDamageCoefficient(definition):0.00}"
            );
        }

        private void HandleActionImpacted(CombatActionState state)
        {
            if (!logActionEvents || !TryGetTracked(state, out TrackedActor tracked))
                return;

            Log(
                $"IMPACT actor={GetActorName(tracked.Actor)} " +
                $"step={state.CurrentBasicAttackStep + 1} " +
                $"action={GetActionName(state.CurrentDefinition)} " +
                $"normalized={state.NormalizedTime:0.00} " +
                $"clip={GetClipName(tracked.FlipbookPlayer != null ? tracked.FlipbookPlayer.CurrentClip : null)}"
            );
        }

        private void HandleActionEnded(CombatActionState state)
        {
            if (!logActionEvents || !TryGetTracked(state, out TrackedActor tracked))
                return;

            Log(
                $"END actor={GetActorName(tracked.Actor)} " +
                $"step={state.CurrentBasicAttackStep + 1} " +
                $"action={GetActionName(state.CurrentDefinition)}"
            );
        }

        private void HandleDamageResolved(DamageEvent damageEvent)
        {
            if (!logDamage || damageEvent.Source == null)
                return;

            Actor sourceActor = damageEvent.Source.GetComponent<Actor>();

            if (!ShouldTrace(sourceActor))
                return;

            CombatActionState actionState = damageEvent.Source.GetComponent<CombatActionState>();
            SpriteFlipbookPlayer flipbookPlayer =
                damageEvent.Source.GetComponentInChildren<SpriteFlipbookPlayer>();

            string gameplayAction =
                actionState != null
                    ? GetActionName(actionState.CurrentDefinition)
                    : damageEvent.ActionName;

            string comboStep =
                actionState != null
                    ? (actionState.CurrentBasicAttackStep + 1).ToString()
                    : "-";

            Log(
                $"DAMAGE actor={GetActorName(sourceActor)} " +
                $"step={comboStep} " +
                $"gameplayAction={gameplayAction} " +
                $"damageAction={damageEvent.ActionName} " +
                $"clip={GetClipName(flipbookPlayer != null ? flipbookPlayer.CurrentClip : null)} " +
                $"target={GetTargetName(damageEvent.Target)} " +
                $"final={damageEvent.FinalDamage:0.##} " +
                $"hp={damageEvent.HealthBefore:0.##}->{damageEvent.HealthAfter:0.##}"
            );
        }

        private bool TryGetTracked(CombatActionState state, out TrackedActor tracked)
        {
            tracked = null;

            if (state == null)
                return false;

            return trackedActors.TryGetValue(state.GetInstanceID(), out tracked);
        }

        private void RemoveMissingActors()
        {
            List<int> missing = null;

            foreach (KeyValuePair<int, TrackedActor> pair in trackedActors)
            {
                if (pair.Value.ActionState != null)
                    continue;

                missing ??= new List<int>();
                missing.Add(pair.Key);
            }

            if (missing == null)
                return;

            foreach (int instanceId in missing)
                trackedActors.Remove(instanceId);
        }

        private void Unsubscribe(TrackedActor tracked)
        {
            if (tracked?.ActionState == null)
                return;

            tracked.ActionState.Started -= HandleActionStarted;
            tracked.ActionState.Impacted -= HandleActionImpacted;
            tracked.ActionState.Ended -= HandleActionEnded;
        }

        private bool ShouldTrace(Actor actor)
        {
            if (actor == null)
                return false;

            if (string.IsNullOrWhiteSpace(actorNameContains))
                return true;

            return actor.ActorName.ToLowerInvariant().Contains(actorNameContains.ToLowerInvariant());
        }

        private static string GetActorName(Actor actor)
        {
            return actor != null ? $"{actor.ActorName}#{actor.GetInstanceID()}" : "Unknown";
        }

        private static string GetActionName(CombatActionDefinition definition)
        {
            return definition != null ? definition.name : "None";
        }

        private static string GetClipName(SpriteFlipbookClip clip)
        {
            return clip != null ? clip.name : "None";
        }

        private static string GetTargetName(Health health)
        {
            if (health == null)
                return "None";

            Actor actor = health.GetComponent<Actor>();
            return actor != null ? $"{actor.ActorName}#{actor.GetInstanceID()}" : health.name;
        }

        private static float GetImpactTime(CombatActionDefinition definition)
        {
            return definition != null ? definition.ImpactNormalizedTime : 0f;
        }

        private static float GetDamageCoefficient(CombatActionDefinition definition)
        {
            return definition != null ? definition.DamageCoefficient : 0f;
        }

        private static void Log(string message)
        {
            Debug.Log($"[CombatTrace] t={Time.time:0.000} {message}");
        }
    }
}