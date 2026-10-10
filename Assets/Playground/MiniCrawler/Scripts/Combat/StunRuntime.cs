using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class StunRuntime : MonoBehaviour
    {
        private Health health;
        private float remainingDuration;

        public float RemainingDuration => Mathf.Max(0f, remainingDuration);
        public bool IsStunned => remainingDuration > 0f;

        public static bool Apply(GameObject source, Health target, float duration, string actionName)
        {
            if (target == null || target.IsDead || duration <= 0f)
                return false;

            StunRuntime runtime = target.GetComponent<StunRuntime>();

            if (runtime == null)
            {
                runtime = target.gameObject.AddComponent<StunRuntime>();
            }

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

            CombatActionState actionState = GetComponent<CombatActionState>();

            if (actionState != null)
            {
                actionState.Cancel();
                actionState.ResetBasicAttackSequence();
            }

            string sourceName = source != null ? source.name : "Unknown";

            string resolvedActionName = string.IsNullOrWhiteSpace(actionName) ? "Stun" : actionName;

            Debug.Log($"[Control] {TargetName()} stunned by {sourceName} " + $"[{resolvedActionName}] for {remainingDuration:0.##}s.");

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

        private string TargetName()
        {
            return health != null ? health.name : name;
        }
    }
}
