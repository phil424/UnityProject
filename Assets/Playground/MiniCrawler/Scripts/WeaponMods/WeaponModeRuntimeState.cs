using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    [DisallowMultipleComponent]
    public sealed class WeaponModRuntimeState : MonoBehaviour
    {
        private readonly List<WeaponModDefinition> activeMods = new();

        public IReadOnlyList<WeaponModDefinition> ActiveMods => activeMods;

        public void Configure(IReadOnlyList<WeaponModDefinition> mods)
        {
            activeMods.Clear();

            if (mods == null)
                return;

            foreach (WeaponModDefinition mod in mods)
            {
                if (mod == null || !mod.IsConfigured || Contains(mod))
                    continue;

                activeMods.Add(mod);
            }
        }

        public float ModifyBasicAttackDamage(CombatActionState actionState, Health target, float currentDamage)
        {
            if (currentDamage <= 0f || activeMods.Count == 0)
                return currentDamage;

            BasicAttackWeaponModContext context = new(gameObject, target, actionState);
            float modifiedDamage = currentDamage;

            foreach (WeaponModDefinition mod in activeMods)
            {
                if (mod == null)
                    continue;

                modifiedDamage = Mathf.Max(0f, mod.ModifyBasicAttackDamage(context, modifiedDamage));
            }

            return modifiedDamage;
        }

        private bool Contains(WeaponModDefinition mod)
        {
            foreach (WeaponModDefinition activeMod in activeMods)
            {
                if (activeMod != null && activeMod.Id == mod.Id)
                    return true;
            }

            return false;
        }
    }
}