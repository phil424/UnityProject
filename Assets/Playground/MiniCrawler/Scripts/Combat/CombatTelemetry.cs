using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public class CombatTelemetry : MonoBehaviour
    {
        private readonly struct DamageSample
        {
            public float Timestamp { get; }
            public float OutgoingDamage { get; }
            public float IncomingDamage { get; }

            public DamageSample(
                float timestamp,
                float outgoingDamage,
                float incomingDamage
            )
            {
                Timestamp = timestamp;
                OutgoingDamage = outgoingDamage;
                IncomingDamage = incomingDamage;
            }
        }

        public static CombatTelemetry Instance
        {
            get;
            private set;
        }

        [Header("DPS")]
        [SerializeField, Min(0.5f)]
        private float dpsWindowSeconds = 3f;

        [Header("Detailed Log")]
        [SerializeField, Min(1)]
        private int maxLogEntries = 20;

        private readonly Queue<DamageSample>
            recentDamage = new();

        private readonly Queue<string>
            combatLog = new();

        private float outgoingDamageInWindow;
        private float incomingDamageInWindow;

        private StageDirector stageDirector;

        public float DpsWindowSeconds =>
            dpsWindowSeconds;

        public float OutgoingDps
        {
            get
            {
                PruneOldSamples();

                return outgoingDamageInWindow /
                    dpsWindowSeconds;
            }
        }

        public float IncomingDps
        {
            get
            {
                PruneOldSamples();

                return incomingDamageInWindow /
                    dpsWindowSeconds;
            }
        }

        public IEnumerable<string> CombatLog =>
            combatLog;

        public int CombatLogCount =>
            combatLog.Count;

        private void Awake()
        {
            if (
                Instance != null &&
                Instance != this
            )
            {
                Debug.LogWarning(
                    "Only one CombatTelemetry should exist."
                );

                enabled = false;
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            if (Instance != this)
                return;

            DamageResolver.DamageResolved +=
                RecordResolvedDamage;
        }
        private void Start()
        {
            stageDirector =
                GetComponent<StageDirector>();

            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (stageDirector != null)
            {
                stageDirector.StateChanged +=
                    HandleStageStateChanged;
            }
        }

        private void OnDisable()
        {
            if (Instance != this)
                return;

            DamageResolver.DamageResolved -=
                RecordResolvedDamage;
        }

        private void OnDestroy()
        {
            if (stageDirector != null)
            {
                stageDirector.StateChanged -=
                    HandleStageStateChanged;
            }

            if (Instance == this)
                Instance = null;
        }

        public void Clear()
        {
            recentDamage.Clear();
            combatLog.Clear();

            outgoingDamageInWindow = 0f;
            incomingDamageInWindow = 0f;
        }

        private void HandleStageStateChanged(
            StageDirector.LevelState newState
        )
        {
            if (
                newState ==
                StageDirector.LevelState.FightingMinions
            )
            {
                Clear();
            }
        }

        public void RecordResolvedDamage(
            DamageEvent damageEvent
        )
        {
            bool sourceIsParty =
                damageEvent.Source != null &&
                damageEvent.Source
                    .GetComponent<PartyMember>() != null;

            bool targetIsParty =
                damageEvent.Target != null &&
                damageEvent.Target
                    .GetComponent<PartyMember>() != null;

            float effectiveDamage =
                Mathf.Max(
                    0f,
                    damageEvent.HealthBefore -
                    damageEvent.HealthAfter
                );

            float outgoingDamage =
                sourceIsParty &&
                !targetIsParty
                    ? effectiveDamage
                    : 0f;

            float incomingDamage =
                !sourceIsParty &&
                targetIsParty
                    ? effectiveDamage
                    : 0f;

            if (
                outgoingDamage > 0f ||
                incomingDamage > 0f
            )
            {
                recentDamage.Enqueue(
                    new DamageSample(
                        Time.time,
                        outgoingDamage,
                        incomingDamage
                    )
                );

                outgoingDamageInWindow +=
                    outgoingDamage;

                incomingDamageInWindow +=
                    incomingDamage;
            }

            AddLogEntry(
                damageEvent
            );

            PruneOldSamples();
        }

        private void PruneOldSamples()
        {
            float cutoff =
                Time.time -
                dpsWindowSeconds;

            while (
                recentDamage.Count > 0 &&
                recentDamage.Peek().Timestamp < cutoff
            )
            {
                DamageSample expired =
                    recentDamage.Dequeue();

                outgoingDamageInWindow -=
                    expired.OutgoingDamage;

                incomingDamageInWindow -=
                    expired.IncomingDamage;
            }

            outgoingDamageInWindow =
                Mathf.Max(
                    0f,
                    outgoingDamageInWindow
                );

            incomingDamageInWindow =
                Mathf.Max(
                    0f,
                    incomingDamageInWindow
                );
        }

        private void AddLogEntry(
            DamageEvent damageEvent
        )
        {
            string sourceName =
                GetActorName(
                    damageEvent.Source
                );

            string targetName =
                damageEvent.Target != null
                    ? GetActorName(
                        damageEvent.Target.gameObject
                    )
                    : "Unknown";

            string actionName =
                string.IsNullOrWhiteSpace(
                    damageEvent.ActionName
                )
                    ? "Damage"
                    : damageEvent.ActionName;

            string entry =
                $"{sourceName} → {targetName} " +
                $"[{actionName}]\n" +
                $"{damageEvent.IncomingDamage:0.##} - " +
                $"{damageEvent.Armour:0.##} ARM = " +
                $"{damageEvent.FinalDamage:0.##} DMG | " +
                $"HP {damageEvent.HealthBefore:0.##} → " +
                $"{damageEvent.HealthAfter:0.##}";

            combatLog.Enqueue(entry);

            while (
                combatLog.Count >
                maxLogEntries
            )
            {
                combatLog.Dequeue();
            }
        }

        private static string GetActorName(
            GameObject actorObject
        )
        {
            if (actorObject == null)
                return "Environment";

            Actor actor =
                actorObject.GetComponent<Actor>();

            if (actor != null)
                return actor.ActorName;

            return actorObject.name;
        }

        private void OnValidate()
        {
            dpsWindowSeconds =
                Mathf.Max(
                    0.5f,
                    dpsWindowSeconds
                );

            maxLogEntries =
                Mathf.Max(
                    1,
                    maxLogEntries
                );
        }
    }
}