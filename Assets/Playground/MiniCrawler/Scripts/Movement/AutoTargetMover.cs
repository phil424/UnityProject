using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    public enum TargetIntent
    {
        None,
        Combat,
        Support,
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class AutoTargetMover : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField]
        private string targetFactionId = "Undead";

        [SerializeField]
        private float searchRadius = 100f;

        [Header("Movement")]
        [SerializeField]
        private float moveSpeed = 2f;

        [SerializeField]
        private float combatStoppingDistance = 1.2f;

        [SerializeField]
        private float supportStoppingDistance = 2f;

        private float runMoveSpeedPercentBonus;

        public string TargetFactionId => targetFactionId;
        public float SearchRadius => searchRadius;

        public float MoveSpeed
        {
            get
            {
                float moveSpeedBonus = runMoveSpeedPercentBonus + GetRuntimeMoveSpeedBonus();

                float multiplier = Mathf.Max(0f, 1f + moveSpeedBonus / 100f);

                return moveSpeed * multiplier;
            }
        }

        public float CombatStoppingDistance => combatStoppingDistance;

        public float SupportStoppingDistance => supportStoppingDistance;

        public Health CurrentTarget { get; private set; }

        public TargetIntent CurrentIntent { get; private set; }

        public float CurrentStoppingDistance => CurrentIntent == TargetIntent.Support ? supportStoppingDistance : combatStoppingDistance;

        public void ApplyRunMoveSpeedBonus(float moveSpeedPercentBonus)
        {
            runMoveSpeedPercentBonus = Mathf.Max(0f, moveSpeedPercentBonus);
        }

        public void SetTarget(Health target, TargetIntent intent)
        {
            CurrentTarget = target;

            CurrentIntent = target != null ? intent : TargetIntent.None;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
            CurrentIntent = TargetIntent.None;
        }

        [ContextMenu("Debug/Log Best Combat Group")]
        private void DebugLogBestCombatGroup()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Combat-group debug is only available " + "in Play Mode.", this);

                return;
            }

            float debugSearchRadius = Mathf.Min(searchRadius, 12f);

            if (!CombatTargetGroupQuery.TryFindBestGroup(gameObject, targetFactionId, debugSearchRadius, 4f, out CombatTargetGroupResult group))
            {
                Debug.Log($"[Target Group] {name} found no " + $"valid group.", this);

                return;
            }

            Debug.Log($"[Target Group] {name} found " + $"{group.Count} targets around " + $"({group.Center.x:0.##}, " + $"{group.Center.z:0.##}).", this);

            Vector3 right = Vector3.right * 0.5f;

            Vector3 forward = Vector3.forward * 0.5f;

            Debug.DrawLine(transform.position, group.Center, Color.cyan, 3f, false);

            Debug.DrawLine(group.Center - right, group.Center + right, Color.cyan, 3f, false);

            Debug.DrawLine(group.Center - forward, group.Center + forward, Color.cyan, 3f, false);
        }

        private float GetRuntimeMoveSpeedBonus()
        {
            RuntimeStatModifiers modifiers = GetComponent<RuntimeStatModifiers>();

            return modifiers != null ? modifiers.MoveSpeedPercentBonus : 0f;
        }

        private void OnValidate()
        {
            searchRadius = Mathf.Max(0.1f, searchRadius);

            moveSpeed = Mathf.Max(0f, moveSpeed);

            combatStoppingDistance = Mathf.Max(0.1f, combatStoppingDistance);

            supportStoppingDistance = Mathf.Max(0.1f, supportStoppingDistance);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, searchRadius);

            if (CurrentTarget != null)
            {
                Gizmos.DrawLine(transform.position, CurrentTarget.transform.position);
            }
        }
    }
}
