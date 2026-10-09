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
        private float duration;
        private float elapsed;
        private float nextTick;
        private string actionName;

        public static bool Apply(GameObject source, Health target, float damagePerTick, float tickInterval, float duration, string actionName)
        {
            if (target == null || target.IsDead || damagePerTick <= 0f || tickInterval <= 0f || duration <= 0f)
                return false;

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
            source = damageSource;
            damagePerTick = Mathf.Max(0f, tickDamage);
            tickInterval = Mathf.Max(0.05f, interval);
            duration = Mathf.Max(tickInterval, burnDuration);
            actionName = string.IsNullOrWhiteSpace(burnActionName) ? "Burn" : burnActionName;

            elapsed = 0f;
            nextTick = tickInterval;
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

            elapsed = Mathf.Min(duration, elapsed + Time.deltaTime);

            while (nextTick <= elapsed + 0.0001f && !health.IsDead)
            {
                DamageResolver.ApplyDamage(source, health, damagePerTick, actionName);
                nextTick += tickInterval;
            }

            if (health.IsDead || elapsed >= duration)
                Destroy(this);
        }
    }
}