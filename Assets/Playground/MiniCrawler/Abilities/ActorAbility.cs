using System;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Abilities
{
    [RequireComponent(typeof(Actor))]
    public abstract class ActorAbility : MonoBehaviour
    {
        [Header("Ability")]
        [SerializeField]
        private string abilityName = "Ability";

        [SerializeField]
        [Min(0.1f)]
        private float cooldown = 3f;

        private float cooldownRemaining;

        public string AbilityName =>
            string.IsNullOrWhiteSpace(abilityName)
                ? GetType().Name
                : abilityName;

        public float Cooldown =>
            cooldown;

        public float CooldownRemaining =>
            cooldownRemaining;

        public bool IsReady =>
            cooldownRemaining <= 0f;

        public event Action<ActorAbility> Activated;

        public void TickCooldown(
            float deltaTime
        )
        {
            if (
                deltaTime <= 0f ||
                cooldownRemaining <= 0f
            )
            {
                return;
            }

            cooldownRemaining =
                Mathf.Max(
                    0f,
                    cooldownRemaining - deltaTime
                );
        }

        public bool TryActivate()
        {
            if (
                !isActiveAndEnabled ||
                !IsOwnerAlive() ||
                !IsReady ||
                !CanActivateAbility()
            )
            {
                return false;
            }

            if (!ExecuteAbility())
                return false;

            cooldownRemaining =
                cooldown;

            Activated?.Invoke(this);

            return true;
        }

        public void ResetCooldown()
        {
            cooldownRemaining = 0f;
        }

        protected abstract bool CanActivateAbility();

        protected abstract bool ExecuteAbility();

        private bool IsOwnerAlive()
        {
            Health health =
                GetComponent<Health>();

            return
                health == null ||
                !health.IsDead;
        }

        protected virtual void OnValidate()
        {
            cooldown =
                Mathf.Max(
                    0.1f,
                    cooldown
                );
        }
    }
}