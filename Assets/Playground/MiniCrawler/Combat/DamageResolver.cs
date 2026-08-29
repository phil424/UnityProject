using System;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public static class DamageResolver
    {
        public static event Action<DamageEvent>
            DamageResolved;

        public static void ApplyDamage(
            GameObject source,
            Health target,
            float incomingDamage,
            string actionName
        )
        {
            if (
                target == null ||
                target.IsDead ||
                incomingDamage <= 0f
            )
            {
                return;
            }

            float modifiedIncomingDamage =
                ApplyOutgoingDamageModifiers(
                    source,
                    incomingDamage
                );

            CombatStats targetStats =
                target.GetComponent<CombatStats>();

            float armour =
                targetStats != null
                    ? targetStats.FlatArmour
                    : 0f;

            float finalDamage =
                targetStats != null
                    ? targetStats
                        .CalculateDamageTaken(
                            modifiedIncomingDamage
                        )
                    : modifiedIncomingDamage;

            float healthBefore =
                target.CurrentHealth;

            target.Damage(
                finalDamage,
                source
            );

            float healthAfter =
                target.CurrentHealth;

            DamageEvent damageEvent =
                new DamageEvent(
                    source,
                    target,
                    actionName,
                    modifiedIncomingDamage,
                    armour,
                    finalDamage,
                    healthBefore,
                    healthAfter
                );

            DamageResolved?.Invoke(
                damageEvent
            );

            string sourceName =
                source != null
                    ? source.name
                    : "Unknown";

            Debug.Log(
                $"{sourceName} -> " +
                $"{target.name} " +
                $"[{actionName}] | " +
                $"{modifiedIncomingDamage:0.##} " +
                $"- {armour:0.##} armour = " +
                $"{finalDamage:0.##} | " +
                $"HP {healthBefore:0.##} -> " +
                $"{healthAfter:0.##}"
            );
        }

        private static float
            ApplyOutgoingDamageModifiers(
                GameObject source,
                float damage
            )
        {
            if (source == null)
                return damage;

            RuntimeStatModifiers modifiers =
                source.GetComponent<
                    RuntimeStatModifiers
                >();

            if (modifiers == null)
                return damage;

            float multiplier =
                Mathf.Max(
                    0f,
                    1f +
                    modifiers
                        .OutgoingDamagePercentBonus /
                    100f
                );

            return
                damage *
                multiplier;
        }
    }
}