using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.WeaponMods
{
    public readonly struct BasicAttackWeaponModContext
    {
        public BasicAttackWeaponModContext(GameObject source, Health target, CombatActionState actionState)
        {
            Source = source;
            Target = target;
            ActionState = actionState;
        }

        public GameObject Source { get; }
        public Health Target { get; }
        public CombatActionState ActionState { get; }

        public int ComboStepIndex => ActionState != null ? ActionState.CurrentBasicAttackStep : 0;

        public int ComboStepCount
        {
            get
            {
                CombatActionSequenceDefinition sequence = ActionState != null ? ActionState.BasicAttackSequence : null;
                return sequence != null && sequence.StepCount > 0 ? sequence.StepCount : 1;
            }
        }

        public bool IsComboFinisher => ComboStepCount > 1 && ComboStepIndex == ComboStepCount - 1;
        public bool UsesProjectile => ActionState?.CurrentDefinition is ProjectileCombatActionDefinition;
    }

    public abstract class WeaponModDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField, TextArea(2, 4)] private string description;
        [SerializeField] private Sprite icon;

        [Header("Compatibility")]
        [Tooltip("Leave empty to allow any playable Character.")]
        [SerializeField] private List<PartyMemberDefinition> compatibleCharacters = new();

        public string Id => string.IsNullOrWhiteSpace(id) ? name : id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description ?? string.Empty;
        public Sprite Icon => icon;
        public IReadOnlyList<PartyMemberDefinition> CompatibleCharacters => compatibleCharacters;

        public virtual bool IsConfigured => !string.IsNullOrWhiteSpace(Id);

        public bool IsCompatibleWith(PartyMemberDefinition character)
        {
            if (character == null)
                return false;

            if (compatibleCharacters == null || compatibleCharacters.Count == 0)
                return true;

            foreach (PartyMemberDefinition compatibleCharacter in compatibleCharacters)
            {
                if (compatibleCharacter != null && compatibleCharacter.Id == character.Id)
                    return true;
            }

            return false;
        }

        public virtual float ModifyBasicAttackDamage(BasicAttackWeaponModContext context, float currentDamage)
        {
            return currentDamage;
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name;
        }
    }
}