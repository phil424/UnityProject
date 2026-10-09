using System;
using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [DisallowMultipleComponent]
    public sealed class WeaponModRuntimeState : MonoBehaviour
    {
        private readonly List<RunWeaponModState> availableMods = new();
        private RunWeaponModState activeModState;

        public IReadOnlyList<RunWeaponModState> AvailableMods => availableMods;
        public RunWeaponModState ActiveModState => activeModState;
        public RunWeaponModState CoreModState => FindState(WeaponModStanceSlot.Core);
        public WeaponModDefinition ActiveMod => activeModState?.Definition;

        public bool IsUsingBasicAttack => activeModState == null;

        public bool IsCoreActive =>
            activeModState != null &&
            activeModState.StanceSlot == WeaponModStanceSlot.Core;

        public event Action<WeaponModRuntimeState> Changed;

        public void Configure(IReadOnlyList<RunWeaponModState> mods)
        {
            string activeId = ActiveMod != null ? ActiveMod.Id : null;

            availableMods.Clear();

            if (mods != null)
            {
                foreach (RunWeaponModState state in mods)
                {
                    if (state?.Definition == null ||
                        !state.Definition.IsConfigured ||
                        Contains(state.Definition))
                    {
                        continue;
                    }

                    availableMods.Add(state);
                }
            }

            availableMods.Sort((a, b) => ((int)a.StanceSlot).CompareTo((int)b.StanceSlot));

            activeModState = FindState(activeId) ?? CoreModState;

            Changed?.Invoke(this);
        }

        public RunWeaponModState GetAvailableMod(WeaponModStanceSlot stanceSlot)
        {
            return FindState(stanceSlot);
        }
        
        public bool TryGetCombinedWeaponModLevel(WeaponModDefinition mod, out int level)
        {
            level = 0;

            if (activeModState == null || mod == null || !activeModState.IsCombinedWith(mod))
                return false;

            RunWeaponModState combinedState = FindState(mod);

            if (combinedState == null)
                return false;

            level = combinedState.Level;
            return true;
        }

        public bool ToggleWeaponMod(WeaponModDefinition mod)
        {
            RunWeaponModState state = FindState(mod);

            if (state == null)
                return false;

            if (state.StanceSlot == WeaponModStanceSlot.Core)
            {
                if (activeModState == state)
                    return false;

                activeModState = state;
            }
            else if (activeModState == state)
            {
                activeModState = CoreModState;
            }
            else
            {
                activeModState = state;
            }

            Changed?.Invoke(this);
            return true;
        }

        public bool ReturnToCore()
        {
            RunWeaponModState coreState = CoreModState;

            if (activeModState == coreState)
                return false;

            activeModState = coreState;
            Changed?.Invoke(this);

            return true;
        }

        public float ModifyBasicAttackDamage(
            CombatActionState actionState,
            Health target,
            float currentDamage)
        {
            if (currentDamage <= 0f || activeModState?.Definition == null)
                return currentDamage;

            BasicAttackWeaponModContext context = CreateContext(actionState, target);

            return Mathf.Max(
                0f,
                activeModState.Definition.ModifyBasicAttackDamage(context, currentDamage)
            );
        }

        public bool TryResolveProjectileBasicAttack(
            CombatActionState actionState,
            Health target,
            ProjectileCombatActionDefinition projectileAction,
            float currentDamage,
            string actionName,
            out bool connected)
        {
            connected = false;

            if (activeModState?.Definition == null)
                return false;

            BasicAttackWeaponModContext context = CreateContext(actionState, target);

            return activeModState.Definition.TryResolveProjectileBasicAttack(
                context,
                projectileAction,
                currentDamage,
                actionName,
                out connected
            );
        }

        private BasicAttackWeaponModContext CreateContext(CombatActionState actionState, Health target)
        {
            return new BasicAttackWeaponModContext(
                gameObject,
                target,
                actionState,
                activeModState != null ? activeModState.Level : 0,
                this
            );
        }

        private bool Contains(WeaponModDefinition mod)
        {
            return FindState(mod) != null;
        }

        private RunWeaponModState FindState(WeaponModDefinition mod)
        {
            return mod != null ? FindState(mod.Id) : null;
        }

        private RunWeaponModState FindState(string modId)
        {
            if (string.IsNullOrWhiteSpace(modId))
                return null;

            foreach (RunWeaponModState state in availableMods)
            {
                if (state?.Definition != null && state.Definition.Id == modId)
                    return state;
            }

            return null;
        }

        private RunWeaponModState FindState(WeaponModStanceSlot stanceSlot)
        {
            foreach (RunWeaponModState state in availableMods)
            {
                if (state != null && state.StanceSlot == stanceSlot)
                    return state;
            }

            return null;
        }

        [ContextMenu("Debug/Return To Core Mastery")]
        private void DebugReturnToCore()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[Weapon Mod] Masteries can only be changed in Play Mode.", this);
                return;
            }

            ReturnToCore();

            Debug.Log(
                ActiveMod != null
                    ? $"[Weapon Mod] Core Mastery '{ActiveMod.DisplayName}' active."
                    : "[Weapon Mod] No Core Mastery is available; original Basic Attack remains active.",
                this
            );
        }
    }
}