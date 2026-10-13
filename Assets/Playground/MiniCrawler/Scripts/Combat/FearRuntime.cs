using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public sealed class FearRuntime : MonoBehaviour
    {
        private const float MinimumDirectionSquared = 0.0001f;

        private Health health;
        private float remainingDuration;

        private Vector3 fleeDirection;
        private Vector3 laneOrigin;

        private float funnelStrength;
        private float funnelDeadZone;

        public float RemainingDuration => Mathf.Max(0f, remainingDuration);
        public bool IsFeared => remainingDuration > 0f;

        public static bool Apply(GameObject source, Health target, Vector3 fleeDirection, Vector3 laneOrigin, float duration, float funnelStrength, float funnelDeadZone, string actionName)
        {
            if (target == null || target.IsDead || duration <= 0f)
                return false;

            fleeDirection.y = 0f;

            if (fleeDirection.sqrMagnitude <= MinimumDirectionSquared)
                return false;

            FearRuntime runtime = target.GetComponent<FearRuntime>();

            if (runtime == null)
                runtime = target.gameObject.AddComponent<FearRuntime>();

            runtime.Configure(source, fleeDirection, laneOrigin, duration, funnelStrength, funnelDeadZone, actionName);

            return true;
        }

        public Vector3 ResolveMovementDirection(Vector3 actorPosition)
        {
            if (!IsFeared || fleeDirection.sqrMagnitude <= MinimumDirectionSquared)
                return Vector3.zero;

            Vector3 fromOrigin = actorPosition - laneOrigin;
            fromOrigin.y = 0f;

            float alongLane = Vector3.Dot(fromOrigin, fleeDirection);

            Vector3 nearestPointOnLane = laneOrigin + fleeDirection * alongLane;

            Vector3 towardLane = nearestPointOnLane - actorPosition;

            towardLane.y = 0f;

            if (towardLane.sqrMagnitude <= funnelDeadZone * funnelDeadZone)
                return fleeDirection;

            Vector3 steeredDirection = fleeDirection + towardLane.normalized * funnelStrength;

            steeredDirection.y = 0f;

            return steeredDirection.sqrMagnitude > MinimumDirectionSquared ? steeredDirection.normalized : fleeDirection;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Configure(GameObject source, Vector3 newFleeDirection, Vector3 newLaneOrigin, float duration, float newFunnelStrength, float newFunnelDeadZone, string actionName)
        {
            remainingDuration = Mathf.Max(remainingDuration, duration);

            newFleeDirection.y = 0f;

            fleeDirection = newFleeDirection.normalized;

            laneOrigin = newLaneOrigin;

            funnelStrength = Mathf.Clamp01(newFunnelStrength);

            funnelDeadZone = Mathf.Max(0f, newFunnelDeadZone);

            CombatActionState actionState = GetComponent<CombatActionState>();

            if (actionState != null)
            {
                actionState.Cancel();
                actionState.ResetBasicAttackSequence();
            }

            string sourceName = source != null ? source.name : "Unknown";

            string resolvedActionName = string.IsNullOrWhiteSpace(actionName) ? "Fear" : actionName;

            Debug.Log($"[Control] {name} feared by {sourceName} " + $"[{resolvedActionName}] for {remainingDuration:0.##}s.");

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
    }
}
