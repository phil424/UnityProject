using System;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public static class DamageResolver
    {
        public static event Action<DamageEvent> DamageResolved;

        public static void ApplyDamage(
            GameObject source,
            Health target,
            float incomingDamage,
            string actionName
        )
        {
            if (target == null || target.IsDead || incomingDamage <= 0f)
                return;

            CombatStats targetStats = target.GetComponent<CombatStats>();

            float armour = targetStats != null
                ? targetStats.FlatArmour
                : 0f;

            float finalDamage = targetStats != null
                ? targetStats.CalculateDamageTaken(incomingDamage)
                : incomingDamage;

            float healthBefore = target.CurrentHealth;

            target.Damage(finalDamage, source);

            float healthAfter = target.CurrentHealth;

            DamageEvent damageEvent = new DamageEvent(
                source,
                target,
                actionName,
                incomingDamage,
                armour,
                finalDamage,
                healthBefore,
                healthAfter
            );

            DamageResolved?.Invoke(damageEvent);

            Debug.Log(
                $"{source.name} -> {target.name} [{actionName}] | " +
                $"{incomingDamage:0.##} - {armour:0.##} armour = " +
                $"{finalDamage:0.##} | " +
                $"HP {healthBefore:0.##} -> {healthAfter:0.##}"
            );
        }
    }
}