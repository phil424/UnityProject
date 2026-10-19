using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class BurningDamageRuntime : MonoBehaviour
    {
        private Health health;
        private GameObject source;
        private float damagePerTick;
        private float tickInterval;
        private float remainingDuration;
        private float tickRemaining;
        private string actionName;

        public float RemainingDuration => Mathf.Max(0f, remainingDuration);

        public static bool Apply(GameObject source, Health target, float damagePerTick, float tickInterval, float duration, string actionName)
        {
            if (target == null || target.IsDead || damagePerTick <= 0f || tickInterval <= 0f || duration <= 0f)
            {
                return false;
            }

            BurningDamageRuntime runtime = target.GetComponent<BurningDamageRuntime>();

            if (runtime == null)
                runtime = target.gameObject.AddComponent<BurningDamageRuntime>();

            runtime.Configure(source, damagePerTick, tickInterval, duration, actionName);

            return true;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Configure(GameObject damageSource, float tickDamage, float interval, float burnDuration, string burnActionName)
        {
            string resolvedActionName = string.IsNullOrWhiteSpace(burnActionName) ? "Burn" : burnActionName;

            float resolvedInterval = Mathf.Max(0.05f, interval);

            float resolvedDuration = Mathf.Max(resolvedInterval, burnDuration);

            bool refreshingSameEffect = enabled && source == damageSource && actionName == resolvedActionName && remainingDuration > 0f;

            source = damageSource;
            damagePerTick = Mathf.Max(0f, tickDamage);
            tickInterval = resolvedInterval;
            actionName = resolvedActionName;

            if (refreshingSameEffect)
            {
                remainingDuration = Mathf.Max(remainingDuration, resolvedDuration);

                tickRemaining = Mathf.Clamp(tickRemaining, 0.01f, tickInterval);
            }
            else
            {
                remainingDuration = resolvedDuration;

                tickRemaining = tickInterval;
            }

            enabled = true;
        }

        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            if (health == null || health.IsDead)
            {
                Destroy(this);
                return;
            }

            float deltaTime = Time.deltaTime;

            remainingDuration = Mathf.Max(0f, remainingDuration - deltaTime);

            tickRemaining -= deltaTime;

            while (tickRemaining <= 0f && remainingDuration > 0f && !health.IsDead)
            {
                DamageResolver.ApplyDamage(source, health, damagePerTick, actionName);

                tickRemaining += tickInterval;
            }

            if (health.IsDead || remainingDuration <= 0f)
            {
                Destroy(this);
            }
        }
    }
}
