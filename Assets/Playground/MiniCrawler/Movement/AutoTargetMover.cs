using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Movement
{
    public enum TargetIntent
    {
        None,
        Combat,
        Support
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class AutoTargetMover : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private string targetFactionId = "Undead";
        [SerializeField] private float searchRadius = 100f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float combatStoppingDistance = 1.2f;
        [SerializeField] private float supportStoppingDistance = 2f;

        private float runMoveSpeedPercentBonus;

        public string TargetFactionId =>
            targetFactionId;

        public float SearchRadius =>
            searchRadius;

        public float MoveSpeed =>
            moveSpeed *
            (1f + (runMoveSpeedPercentBonus / 100f));

        public float CombatStoppingDistance =>
            combatStoppingDistance;

        public float SupportStoppingDistance =>
            supportStoppingDistance;

        public Health CurrentTarget { get; private set; }

        public TargetIntent CurrentIntent { get; private set; }

        public float CurrentStoppingDistance =>
            CurrentIntent == TargetIntent.Support
                ? supportStoppingDistance
                : combatStoppingDistance;

        public void ApplyRunMoveSpeedBonus(
            float moveSpeedPercentBonus
        )
        {
            runMoveSpeedPercentBonus =
                Mathf.Max(0f, moveSpeedPercentBonus);
        }

        public void SetTarget(
            Health target,
            TargetIntent intent
        )
        {
            CurrentTarget = target;

            CurrentIntent =
                target != null
                    ? intent
                    : TargetIntent.None;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
            CurrentIntent = TargetIntent.None;
        }

        private void OnValidate()
        {
            searchRadius = Mathf.Max(0.1f, searchRadius);
            moveSpeed = Mathf.Max(0f, moveSpeed);
            combatStoppingDistance =
                Mathf.Max(0.1f, combatStoppingDistance);

            supportStoppingDistance =
                Mathf.Max(0.1f, supportStoppingDistance);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                searchRadius
            );

            if (CurrentTarget != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    CurrentTarget.transform.position
                );
            }
        }
    }
}