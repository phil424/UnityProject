using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    [DefaultExecutionOrder(-250)]
    public class RuntimeStatModifiers :
        MonoBehaviour
    {
        private sealed class ActiveModifier
        {
            public string SourceId;

            public float RemainingDuration;

            public float OutgoingDamagePercentBonus;

            public float AttackSpeedPercentBonus;

            public float MoveSpeedPercentBonus;

            public float FlatArmourBonus;
        }

        private readonly List<ActiveModifier>
            activeModifiers = new();

        public float OutgoingDamagePercentBonus
        {
            get;
            private set;
        }

        public float AttackSpeedPercentBonus
        {
            get;
            private set;
        }

        public float MoveSpeedPercentBonus
        {
            get;
            private set;
        }

        public float FlatArmourBonus
        {
            get;
            private set;
        }

        public bool HasModifier(
            string sourceId
        )
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                return false;

            foreach (
                ActiveModifier modifier
                    in activeModifiers
            )
            {
                if (
                    modifier.SourceId ==
                    sourceId
                )
                {
                    return true;
                }
            }

            return false;
        }

        public bool ApplyOrRefresh(
            string sourceId,
            float duration,
            float outgoingDamagePercentBonus,
            float attackSpeedPercentBonus,
            float moveSpeedPercentBonus,
            float flatArmourBonus
        )
        {
            if (
                string.IsNullOrWhiteSpace(sourceId) ||
                duration <= 0f
            )
            {
                return false;
            }

            ActiveModifier modifier =
                FindModifier(sourceId);

            if (modifier == null)
            {
                modifier =
                    new ActiveModifier
                    {
                        SourceId =
                            sourceId
                    };

                activeModifiers.Add(
                    modifier
                );
            }

            modifier.RemainingDuration =
                duration;

            modifier.OutgoingDamagePercentBonus =
                outgoingDamagePercentBonus;

            modifier.AttackSpeedPercentBonus =
                attackSpeedPercentBonus;

            modifier.MoveSpeedPercentBonus =
                moveSpeedPercentBonus;

            modifier.FlatArmourBonus =
                flatArmourBonus;

            RecalculateTotals();

            return true;
        }

        private ActiveModifier FindModifier(
            string sourceId
        )
        {
            foreach (
                ActiveModifier modifier
                    in activeModifiers
            )
            {
                if (
                    modifier.SourceId ==
                    sourceId
                )
                {
                    return modifier;
                }
            }

            return null;
        }

        private void Update()
        {
            if (
                SimulationPause.IsPaused ||
                activeModifiers.Count == 0
            )
            {
                return;
            }

            bool changed = false;

            for (
                int index =
                    activeModifiers.Count - 1;
                index >= 0;
                index--
            )
            {
                ActiveModifier modifier =
                    activeModifiers[index];

                modifier.RemainingDuration -=
                    Time.deltaTime;

                if (
                    modifier.RemainingDuration >
                    0f
                )
                {
                    continue;
                }

                activeModifiers.RemoveAt(
                    index
                );

                changed = true;
            }

            if (changed)
                RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            OutgoingDamagePercentBonus =
                0f;

            AttackSpeedPercentBonus =
                0f;

            MoveSpeedPercentBonus =
                0f;

            FlatArmourBonus =
                0f;

            foreach (
                ActiveModifier modifier
                    in activeModifiers
            )
            {
                OutgoingDamagePercentBonus +=
                    modifier
                        .OutgoingDamagePercentBonus;

                AttackSpeedPercentBonus +=
                    modifier
                        .AttackSpeedPercentBonus;

                MoveSpeedPercentBonus +=
                    modifier
                        .MoveSpeedPercentBonus;

                FlatArmourBonus +=
                    modifier
                        .FlatArmourBonus;
            }
        }

        private void OnDisable()
        {
            activeModifiers.Clear();

            RecalculateTotals();
        }
    }
}