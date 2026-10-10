using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class HealingOverTimeRuntime : MonoBehaviour
    {
        private sealed class ActiveHealing
        {
            public string SourceId;
            public string ActionName;
            public float RemainingDuration;
            public float TickInterval;
            public float TickRemaining;
            public float HealingPerTick;
        }

        private readonly List<ActiveHealing> activeEffects = new();

        private Health health;

        public int ActiveEffectCount => activeEffects.Count;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        public bool HasEffect(string sourceId)
        {
            return FindEffect(sourceId) != null;
        }

        public bool ApplyOrRefresh(string sourceId, string actionName, float duration, float tickInterval, float healingPerTick)
        {
            if (string.IsNullOrWhiteSpace(sourceId) || duration <= 0f || tickInterval <= 0f || healingPerTick <= 0f)
            {
                return false;
            }

            ActiveHealing effect = FindEffect(sourceId);

            if (effect == null)
            {
                effect = new ActiveHealing { SourceId = sourceId };

                activeEffects.Add(effect);
            }

            effect.ActionName = string.IsNullOrWhiteSpace(actionName) ? "Healing Over Time" : actionName;

            effect.RemainingDuration = duration;
            effect.TickInterval = tickInterval;
            effect.TickRemaining = tickInterval;
            effect.HealingPerTick = healingPerTick;

            Debug.Log($"[Healing] {name} gained {effect.ActionName} for {duration:0.##}s | " + $"{healingPerTick:0.##} healing every {tickInterval:0.##}s.");

            return true;
        }

        private void Update()
        {
            if (SimulationPause.IsPaused || activeEffects.Count == 0)
            {
                return;
            }

            if (health == null || health.IsDead)
            {
                activeEffects.Clear();
                return;
            }

            float deltaTime = Time.deltaTime;

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                ActiveHealing effect = activeEffects[i];

                float activeDelta = Mathf.Min(deltaTime, effect.RemainingDuration);

                effect.RemainingDuration -= deltaTime;

                effect.TickRemaining -= activeDelta;

                while (effect.TickRemaining <= 0f)
                {
                    health.Heal(effect.HealingPerTick);

                    effect.TickRemaining += effect.TickInterval;
                }

                if (effect.RemainingDuration <= 0f)
                    activeEffects.RemoveAt(i);
            }
        }

        private ActiveHealing FindEffect(string sourceId)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                return null;

            foreach (ActiveHealing effect in activeEffects)
            {
                if (effect.SourceId == sourceId)
                    return effect;
            }

            return null;
        }

        private void OnDisable()
        {
            activeEffects.Clear();
        }
    }
}
