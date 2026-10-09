using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.WeaponMods;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-25)]
    public class CombatActionSystem : MonoBehaviour
    {
        private void Update()
        {
            if (SimulationPause.IsPaused)
                return;

            CombatActionState[] actions = FindObjectsByType<CombatActionState>(FindObjectsSortMode.None);

            foreach (CombatActionState action in actions)
            {
                if (action == null || !action.isActiveAndEnabled || !action.IsExecuting)
                    continue;

                TickAction(action);
            }
        }

        private void TickAction(CombatActionState action)
        {
            Health health = action.GetComponent<Health>();

            if (health != null && health.IsDead)
            {
                action.Cancel();
                return;
            }

            action.Advance(Time.deltaTime);

            if (action.ShouldApplyImpact)
            {
                bool connected = ResolveImpact(action);
                action.MarkImpactApplied(connected);
            }

            if (action.ShouldComplete)
                action.Complete();
        }

        private bool ResolveImpact(CombatActionState action)
        {
            CombatActionDefinition definition = action.CurrentDefinition;
            Health target = action.Target;

            if (definition == null || target == null || target.IsDead)
                return false;

            Health attackerHealth = action.GetComponent<Health>();

            if (attackerHealth != null && attackerHealth.IsDead)
                return false;

            if (!CombatEngagementState.AllowsCombat(action.gameObject) || !CombatEngagementState.AllowsCombat(target.gameObject))
            {
                return false;
            }

            CombatStats attackerStats = action.GetComponent<CombatStats>();

            if (attackerStats == null)
                return false;

            float distance = Vector3.Distance(action.transform.position, target.transform.position);

            if (distance > attackerStats.AttackRange)
                return false;

            float damage = attackerStats.Damage * definition.DamageCoefficient;

            WeaponModRuntimeState weaponMods = action.GetComponent<WeaponModRuntimeState>();

            if (weaponMods != null)
                damage = weaponMods.ModifyBasicAttackDamage(action, target, damage);

            if (definition is ProjectileCombatActionDefinition projectileAction)
            {
                if (weaponMods != null && weaponMods.TryResolveProjectileBasicAttack(action, target, projectileAction, damage, attackerStats.AttackName, out bool modConnected))
                    return modConnected;

                ProjectileLaunchRequest request = new(
                    projectileAction.Projectile,
                    action.gameObject,
                    ProjectileDelivery.Targeted(target),
                    new ProjectileImpactPayload(damage, attackerStats.AttackName)
                );

                return ProjectileLauncher.Launch(request) != null;
            }

            float healthBefore = target.CurrentHealth;

            DamageResolver.ApplyDamage(action.gameObject, target, damage, attackerStats.AttackName);

            return target.CurrentHealth < healthBefore;
        }
    }
}
