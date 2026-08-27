using System;
using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class Health : MonoBehaviour
    {
        public static event Action<Health> AnyDied;

        public event Action<Health> Changed;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private bool destroyWhenDead = true;

        private float maxHealthBonus;
        private bool deathProcessed;

        public float MaxHealth =>
            maxHealth + maxHealthBonus;

        public float CurrentHealth =>
            currentHealth;

        public float Normalized =>
            MaxHealth <= 0f
                ? 0f
                : Mathf.Clamp01(
                    CurrentHealth / MaxHealth
                );

        public bool IsDead =>
            currentHealth <= 0f;

        public bool DestroyWhenDead =>
            destroyWhenDead;

        public void ApplyMaxHealthBonus(
            float bonus
        )
        {
            maxHealthBonus =
                Mathf.Max(
                    0f,
                    bonus
                );

            currentHealth =
                MaxHealth;

            deathProcessed =
                false;

            Changed?.Invoke(this);
        }

        public void Damage(
            float amount,
            GameObject source = null
        )
        {
            if (
                IsDead ||
                deathProcessed ||
                amount <= 0f
            )
            {
                return;
            }

            currentHealth =
                Mathf.Max(
                    0f,
                    currentHealth - amount
                );

            Changed?.Invoke(this);

            string sourceName =
                source != null
                    ? source.name
                    : "Unknown";

            Debug.Log(
                $"{name} took {amount:0.##} damage " +
                $"from {sourceName}. " +
                $"HP: {currentHealth:0.##}/{MaxHealth:0.##}"
            );

            if (IsDead)
                Die();
        }

        public void Heal(
            float amount
        )
        {
            if (
                IsDead ||
                amount <= 0f
            )
            {
                return;
            }

            float previousHealth =
                currentHealth;

            currentHealth =
                Mathf.Min(
                    currentHealth + amount,
                    MaxHealth
                );

            if (
                !Mathf.Approximately(
                    previousHealth,
                    currentHealth
                )
            )
            {
                Changed?.Invoke(this);
            }
        }

        private void Die()
        {
            if (deathProcessed)
                return;

            deathProcessed = true;

            Debug.Log(
                $"{name} died."
            );

            AnyDied?.Invoke(this);

            if (destroyWhenDead)
                Destroy(gameObject);
        }

        private void OnValidate()
        {
            maxHealth =
                Mathf.Max(
                    1f,
                    maxHealth
                );

            currentHealth =
                Mathf.Clamp(
                    currentHealth,
                    0f,
                    maxHealth
                );
        }
    }
}