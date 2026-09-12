using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Encounters
{
    public enum EncounterTriggerKind
    {
        PartyProximity,
        EncounterStarted,
        DelayAfterEncounterStarted,
        PhaseStarted,
        DelayAfterPhaseStarted,
        PhaseCleared
    }

    public enum EncounterActionKind
    {
        ActivateEncounterCombat,
        BeginEncounterSpawning,
        StartPhase,
        BeginSpawnGroup,
        ActivateSpawnGroup,
        CompleteEncounter,
        RaiseSignal
    }

    [Serializable]
    public sealed class EncounterTriggerDefinition
    {
        [SerializeField] private EncounterTriggerKind kind = EncounterTriggerKind.PartyProximity;

        [Header("Party Proximity")]
        [SerializeField] private bool requireEncounterAvailable = true;
        [SerializeField, Min(0.1f)] private float proximityRadius = 5f;
        [SerializeField] private Vector3 localOffset;

        [Header("Timing")]
        [SerializeField, Min(0f)] private float delaySeconds;

        [Header("Phase")]
        [SerializeField, Min(0)] private int phaseIndex;

        public EncounterTriggerKind Kind => kind;

        public bool RequireEncounterAvailable => requireEncounterAvailable;
        public float ProximityRadius => proximityRadius;
        public Vector3 LocalOffset => localOffset;

        public float DelaySeconds => delaySeconds;
        public int PhaseIndex => phaseIndex;

        public EncounterTriggerDefinition()
        {
        }

        public EncounterTriggerDefinition(
            EncounterTriggerKind kind,
            bool requireEncounterAvailable,
            float proximityRadius,
            Vector3 localOffset,
            float delaySeconds,
            int phaseIndex)
        {
            this.kind = kind;
            this.requireEncounterAvailable = requireEncounterAvailable;
            this.proximityRadius = proximityRadius;
            this.localOffset = localOffset;
            this.delaySeconds = delaySeconds;
            this.phaseIndex = phaseIndex;

            ClampValues();
        }

        public EncounterTriggerDefinition(EncounterTriggerDefinition source)
            : this(
                source?.kind ?? EncounterTriggerKind.PartyProximity,
                source?.requireEncounterAvailable ?? true,
                source?.proximityRadius ?? 5f,
                source?.localOffset ?? Vector3.zero,
                source?.delaySeconds ?? 0f,
                source?.phaseIndex ?? 0)
        {
        }

        public void ClampValues()
        {
            proximityRadius = Mathf.Max(0.1f, proximityRadius);
            delaySeconds = Mathf.Max(0f, delaySeconds);
            phaseIndex = Mathf.Max(0, phaseIndex);
        }
    }

    [Serializable]
    public sealed class EncounterActionDefinition
    {
        [SerializeField] private EncounterActionKind kind;

        [Header("Phase / Group Target")]
        [SerializeField, Min(0)] private int phaseIndex;
        [SerializeField, Min(0)] private int groupIndex;

        [Header("Signal")]
        [SerializeField] private string signalId;

        public EncounterActionKind Kind => kind;
        public int PhaseIndex => phaseIndex;
        public int GroupIndex => groupIndex;
        public string SignalId => signalId ?? string.Empty;

        public EncounterActionDefinition()
        {
        }

        public EncounterActionDefinition(
            EncounterActionKind kind,
            int phaseIndex,
            int groupIndex,
            string signalId)
        {
            this.kind = kind;
            this.phaseIndex = phaseIndex;
            this.groupIndex = groupIndex;
            this.signalId = signalId;

            ClampValues();
        }

        public EncounterActionDefinition(EncounterActionDefinition source)
            : this(
                source?.kind ?? EncounterActionKind.ActivateEncounterCombat,
                source?.phaseIndex ?? 0,
                source?.groupIndex ?? 0,
                source?.signalId)
        {
        }

        public void ClampValues()
        {
            phaseIndex = Mathf.Max(0, phaseIndex);
            groupIndex = Mathf.Max(0, groupIndex);
        }
    }

    [Serializable]
    public sealed class EncounterRuleDefinition
    {
        [SerializeField] private string displayName = "Encounter Rule";
        [SerializeField] private bool enabled = true;
        [SerializeField] private EncounterTriggerDefinition trigger = new();
        [SerializeField] private List<EncounterActionDefinition> actions = new();

        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName) ? "Encounter Rule" : displayName;

        public bool Enabled => enabled;
        public EncounterTriggerDefinition Trigger => trigger;
        public IReadOnlyList<EncounterActionDefinition> Actions => actions;

        public EncounterRuleDefinition()
        {
        }

        public EncounterRuleDefinition(
            string displayName,
            bool enabled,
            EncounterTriggerDefinition trigger,
            IReadOnlyList<EncounterActionDefinition> actions)
        {
            this.displayName = displayName;
            this.enabled = enabled;
            this.trigger = trigger != null
                ? new EncounterTriggerDefinition(trigger)
                : new EncounterTriggerDefinition();

            if (actions != null)
            {
                foreach (EncounterActionDefinition action in actions)
                {
                    if (action != null)
                        this.actions.Add(new EncounterActionDefinition(action));
                }
            }
        }

        public EncounterRuleDefinition(EncounterRuleDefinition source)
            : this(
                source?.displayName,
                source?.enabled ?? true,
                source?.trigger,
                source?.actions)
        {
        }

        public void ClampValues()
        {
            trigger ??= new EncounterTriggerDefinition();
            trigger.ClampValues();

            foreach (EncounterActionDefinition action in actions)
                action?.ClampValues();
        }
    }
}