using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public sealed class SlowRuntime : MonoBehaviour
    {
        private const string ModifierSourceId = "Status:Slow";

        private Health health;
        private RuntimeStatModifiers modifiers;

        private float remainingDuration;
        private float slowPercent;

        public bool IsSlowed => remainingDuration > 0f;
        public float RemainingDuration => Mathf.Max(0f, remainingDuration);
        public float SlowPercent => Mathf.Max(0f, slowPercent);

        public static bool Apply(GameObject source, Health target, float slowPercent, float duration, string actionName)
        {
            if (target == null || target.IsDead || slowPercent <= 0f || duration <= 0f)
            {
                return false;
            }

            SlowRuntime runtime = target.GetComponent<SlowRuntime>();

            if (runtime == null)
                runtime = target.gameObject.AddComponent<SlowRuntime>();

            runtime.Configure(source, slowPercent, duration, actionName);

            return true;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Configure(GameObject source, float percent, float duration, string actionName)
        {
            slowPercent = Mathf.Clamp(percent, 0f, 95f);

            remainingDuration = Mathf.Max(0.05f, duration);

            modifiers = GetComponent<RuntimeStatModifiers>();

            if (modifiers == null)
            {
                modifiers = gameObject.AddComponent<RuntimeStatModifiers>();
            }

            modifiers.ApplyOrRefresh(ModifierSourceId, remainingDuration, outgoingDamagePercentBonus: 0f, attackSpeedPercentBonus: 0f, moveSpeedPercentBonus: -slowPercent, flatArmourBonus: 0f);

            string sourceName = source != null ? source.name : "Unknown";

            string resolvedActionName = string.IsNullOrWhiteSpace(actionName) ? "Slow" : actionName;

            Debug.Log($"[Control] {name} slowed by " + $"{sourceName} [{resolvedActionName}] | " + $"-{slowPercent:0.##}% movement for " + $"{remainingDuration:0.##}s.");

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

            remainingDuration = Mathf.Max(0f, remainingDuration - Time.deltaTime);

            if (remainingDuration <= 0f)
                Destroy(this);
        }

        private void OnDisable()
        {
            if (modifiers != null)
                modifiers.RemoveModifier(ModifierSourceId);

            remainingDuration = 0f;
            slowPercent = 0f;
        }
    }
}
