using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class RootRuntime : MonoBehaviour
    {
        private Health health;
        private float remainingDuration;

        public float RemainingDuration => Mathf.Max(0f, remainingDuration);
        public bool IsRooted => remainingDuration > 0f;

        public static bool Apply(GameObject source, Health target, float duration, string actionName)
        {
            if (target == null || target.IsDead || duration <= 0f)
                return false;

            RootRuntime runtime = target.GetComponent<RootRuntime>();

            if (runtime == null)
                runtime = target.gameObject.AddComponent<RootRuntime>();

            runtime.Configure(source, duration, actionName);

            return true;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Configure(GameObject source, float duration, string actionName)
        {
            remainingDuration = Mathf.Max(remainingDuration, duration);

            string sourceName = source != null ? source.name : "Unknown";

            string resolvedActionName = string.IsNullOrWhiteSpace(actionName) ? "Root" : actionName;

            Debug.Log($"[Control] {name} rooted by {sourceName} " + $"[{resolvedActionName}] for " + $"{remainingDuration:0.##}s.");

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

            ForcedMotion forcedMotion = GetComponent<ForcedMotion>();

            if (forcedMotion != null && forcedMotion.IsActive)
            {
                return;
            }

            remainingDuration = Mathf.Max(0f, remainingDuration - Time.deltaTime);

            if (remainingDuration <= 0f)
                Destroy(this);
        }

        [ContextMenu("Debug/Apply 5 Second Root")]
        private void DebugApplyRoot()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Debug Root can only be applied in Play Mode.", this);

                return;
            }

            Configure(gameObject, 5f, "Debug Root");
        }

        private void OnDisable()
        {
            remainingDuration = 0f;
        }
    }
}
