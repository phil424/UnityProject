using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Spawning;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    [DisallowMultipleComponent]
    public sealed class EncounterRuleRunner : MonoBehaviour
    {
        private sealed class RuntimeRule
        {
            public EncounterRuleDefinition Definition { get; }
            public bool Fired { get; set; }
            public float ElapsedSeconds { get; set; }

            public RuntimeRule(EncounterRuleDefinition definition)
            {
                Definition = new EncounterRuleDefinition(definition);
            }

            public void Reset()
            {
                Fired = false;
                ElapsedSeconds = 0f;
            }
        }

        private readonly List<RuntimeRule> rules = new();

        private LevelEncounter encounter;

        private void Awake()
        {
            encounter = GetComponent<LevelEncounter>();
        }

        private void OnEnable()
        {
            if (encounter == null)
                encounter = GetComponent<LevelEncounter>();

            if (encounter != null)
                encounter.RuntimeReset += HandleRuntimeReset;
        }

        private void OnDisable()
        {
            if (encounter != null)
                encounter.RuntimeReset -= HandleRuntimeReset;
        }

        private void Update()
        {
            if (encounter == null ||
                encounter.IsExpired ||
                encounter.IsCompleted ||
                rules.Count == 0)
            {
                return;
            }

            float deltaTime = Time.deltaTime;

            foreach (RuntimeRule runtimeRule in rules)
            {
                if (runtimeRule == null ||
                    runtimeRule.Fired ||
                    runtimeRule.Definition == null ||
                    !runtimeRule.Definition.Enabled)
                {
                    continue;
                }

                if (!IsTriggered(runtimeRule, deltaTime))
                    continue;

                runtimeRule.Fired = true;
                ExecuteActions(runtimeRule.Definition.Actions);
            }
        }

        public void Configure(IReadOnlyList<EncounterRuleDefinition> definitions)
        {
            rules.Clear();

            if (definitions == null)
                return;

            foreach (EncounterRuleDefinition definition in definitions)
            {
                if (definition != null)
                    rules.Add(new RuntimeRule(definition));
            }
        }

        public void ClearRules()
        {
            rules.Clear();
        }

        public static void ApplyTo(
            LevelEncounter encounter,
            IReadOnlyList<EncounterRuleDefinition> definitions)
        {
            if (encounter == null)
                return;

            EncounterRuleRunner runner =
                encounter.GetComponent<EncounterRuleRunner>();

            if (definitions == null || definitions.Count == 0)
            {
                runner?.ClearRules();
                return;
            }

            if (runner == null)
                runner = encounter.gameObject.AddComponent<EncounterRuleRunner>();

            runner.Configure(definitions);
        }

        public static void ClearFrom(LevelEncounter encounter)
        {
            encounter?.GetComponent<EncounterRuleRunner>()?.ClearRules();
        }

        private void HandleRuntimeReset(LevelEncounter source)
        {
            foreach (RuntimeRule rule in rules)
                rule?.Reset();
        }

        private bool IsTriggered(RuntimeRule runtimeRule, float deltaTime)
        {
            EncounterTriggerDefinition trigger = runtimeRule.Definition.Trigger;

            if (trigger == null)
                return false;

            switch (trigger.Kind)
            {
                case EncounterTriggerKind.PartyProximity:
                    return IsPartyWithinTrigger(trigger);

                case EncounterTriggerKind.EncounterStarted:
                    return encounter.HasStarted;

                case EncounterTriggerKind.DelayAfterEncounterStarted:
                    if (!encounter.HasStarted)
                        return false;

                    runtimeRule.ElapsedSeconds += deltaTime;
                    return runtimeRule.ElapsedSeconds >= trigger.DelaySeconds;

                case EncounterTriggerKind.PhaseStarted:
                    return IsPhaseStarted(trigger.PhaseIndex);

                case EncounterTriggerKind.DelayAfterPhaseStarted:
                    if (!IsPhaseStarted(trigger.PhaseIndex))
                        return false;

                    runtimeRule.ElapsedSeconds += deltaTime;
                    return runtimeRule.ElapsedSeconds >= trigger.DelaySeconds;

                case EncounterTriggerKind.PhaseCleared:
                    return IsPhaseCleared(trigger.PhaseIndex);

                default:
                    return false;
            }
        }

        private bool IsPartyWithinTrigger(EncounterTriggerDefinition trigger)
        {
            if (trigger.RequireEncounterAvailable && !encounter.IsAvailable)
                return false;

            Vector3 centre =
                encounter.transform.TransformPoint(trigger.LocalOffset);

            float radiusSquared =
                trigger.ProximityRadius * trigger.ProximityRadius;

            PartyMember[] partyMembers =
                FindObjectsByType<PartyMember>(FindObjectsSortMode.None);

            foreach (PartyMember partyMember in partyMembers)
            {
                if (partyMember == null)
                    continue;
                    
                StageDirector stageDirector = StageDirector.Instance;

                if (stageDirector != null &&
                    !stageDirector.IsCurrentPartyMember(partyMember))
                {
                    continue;
                }

                Health health = partyMember.GetComponent<Health>();

                if (health != null && health.IsDead)
                    continue;

                Vector3 difference =
                    partyMember.transform.position - centre;

                if (difference.sqrMagnitude <= radiusSquared)
                    return true;
            }

            return false;
        }

        private bool IsPhaseStarted(int phaseIndex)
        {
            if (phaseIndex < 0 || phaseIndex >= encounter.PhaseCount)
                return false;

            LevelEncounterPhase phase = encounter.Phases[phaseIndex];

            return phase != null && phase.IsStarted;
        }

        private bool IsPhaseCleared(int phaseIndex)
        {
            if (phaseIndex < 0 || phaseIndex >= encounter.PhaseCount)
                return false;

            LevelEncounterPhase phase = encounter.Phases[phaseIndex];

            return phase != null &&
                   phase.IsStarted &&
                   phase.IsCleared;
        }

        private void ExecuteActions(
            IReadOnlyList<EncounterActionDefinition> actions)
        {
            if (actions == null)
                return;

            foreach (EncounterActionDefinition action in actions)
                ExecuteAction(action);
        }

        private void ExecuteAction(EncounterActionDefinition action)
        {
            if (action == null)
                return;

            switch (action.Kind)
            {
                case EncounterActionKind.ActivateEncounterCombat:
                    encounter.ActivateCombat();
                    break;

                case EncounterActionKind.BeginEncounterSpawning:
                    encounter.BeginSpawning();
                    break;

                case EncounterActionKind.StartPhase:
                    encounter.TryStartPhase(action.PhaseIndex);
                    break;

                case EncounterActionKind.BeginSpawnGroup:
                    ResolveSpawnGroup(action)?.BeginSpawning();
                    break;

                case EncounterActionKind.ActivateSpawnGroup:
                    ResolveSpawnGroup(action)?.ActivateCombat();
                    break;

                case EncounterActionKind.CompleteEncounter:
                    encounter.Complete();
                    break;

                case EncounterActionKind.RaiseSignal:
                    encounter.RaiseSignal(action.SignalId);
                    break;
            }
        }

        private LevelSpawnGroup ResolveSpawnGroup(
            EncounterActionDefinition action)
        {
            if (encounter.PhaseCount > 0)
            {
                if (action.PhaseIndex < 0 ||
                    action.PhaseIndex >= encounter.PhaseCount)
                {
                    return null;
                }

                LevelEncounterPhase phase =
                    encounter.Phases[action.PhaseIndex];

                if (phase == null ||
                    action.GroupIndex < 0 ||
                    action.GroupIndex >= phase.SpawnGroups.Count)
                {
                    return null;
                }

                return phase.SpawnGroups[action.GroupIndex];
            }

            List<LevelSpawnGroup> groups = new();

            foreach (LevelSpawnGroup group in encounter.SpawnGroups)
            {
                if (group == null)
                    continue;

                if (group.GetComponentInParent<LevelEncounterPhase>() == null)
                    groups.Add(group);
            }

            groups.Sort(
                (a, b) =>
                    a.transform.GetSiblingIndex()
                        .CompareTo(b.transform.GetSiblingIndex())
            );

            return action.GroupIndex >= 0 &&
                   action.GroupIndex < groups.Count
                ? groups[action.GroupIndex]
                : null;
        }
        
        public bool TryGetPrimaryPartyProximityStart(
            out Vector3 worldCentre,
            out float radius)
        {
            worldCentre = Vector3.zero;
            radius = 0f;

            if (encounter == null)
                return false;

            foreach (RuntimeRule runtimeRule in rules)
            {
                EncounterRuleDefinition rule = runtimeRule?.Definition;

                if (rule == null ||
                    !rule.Enabled ||
                    rule.Trigger == null ||
                    rule.Trigger.Kind != EncounterTriggerKind.PartyProximity)
                {
                    continue;
                }

                bool startsEncounter = false;

                foreach (EncounterActionDefinition action in rule.Actions)
                {
                    if (action == null)
                        continue;

                    if (action.Kind == EncounterActionKind.ActivateEncounterCombat ||
                        action.Kind == EncounterActionKind.BeginEncounterSpawning)
                    {
                        startsEncounter = true;
                        break;
                    }
                }

                if (!startsEncounter)
                    continue;

                worldCentre = encounter.transform.TransformPoint(
                    rule.Trigger.LocalOffset
                );

                radius = Mathf.Max(
                    0.1f,
                    rule.Trigger.ProximityRadius
                );

                return true;
            }

            return false;
        }
    }
}