using System.Collections.Generic;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using MiniCrawler.WeaponMods;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [DisallowMultipleComponent]
    public sealed class WeaponModHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StageDirector stageDirector;
        [SerializeField] private TMP_Text stanceText;
        [SerializeField] private Transform slotRoot;

        private readonly List<WeaponModStanceButtonUI> slots = new();
        private WeaponModRuntimeState runtimeState;

        private void Awake()
        {
            DiscoverSlots();
        }

        private void OnEnable()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (stageDirector != null)
            {
                stageDirector.PartyMemberSpawned += HandlePartyMemberSpawned;
                stageDirector.PartyMemberRuntimeChanged += HandlePartyMemberRuntimeChanged;
                stageDirector.LevelCleared += HandleLevelCleared;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (stageDirector != null)
            {
                stageDirector.PartyMemberSpawned -= HandlePartyMemberSpawned;
                stageDirector.PartyMemberRuntimeChanged -= HandlePartyMemberRuntimeChanged;
                stageDirector.LevelCleared -= HandleLevelCleared;
            }

            BindRuntimeState(null);
        }

        private void DiscoverSlots()
        {
            slots.Clear();

            if (slotRoot == null)
                return;

            if (slotRoot.childCount != RunBuild.MaxWeaponModStances)
            {
                Debug.LogError(
                    $"Weapon Mastery HUD requires exactly {RunBuild.MaxWeaponModStances} direct stance slots, " +
                    $"but '{slotRoot.name}' contains {slotRoot.childCount}.",
                    this
                );
            }

            for (int i = 0; i < slotRoot.childCount; i++)
            {
                WeaponModStanceButtonUI slot = slotRoot.GetChild(i).GetComponent<WeaponModStanceButtonUI>();

                if (slot == null)
                {
                    Debug.LogError(
                        $"Weapon Mastery HUD child {i} '{slotRoot.GetChild(i).name}' has no WeaponModStanceButtonUI.",
                        this
                    );

                    continue;
                }

                slots.Add(slot);
            }
        }

        private void HandlePartyMemberSpawned(Core.PartyMemberDefinition definition, GameObject actor)
        {
            BindActor(actor);
        }

        private void HandlePartyMemberRuntimeChanged(Core.PartyMemberDefinition definition, GameObject actor)
        {
            BindActor(actor);
        }

        private void HandleLevelCleared()
        {
            BindRuntimeState(null);
        }

        private void BindActor(GameObject actor)
        {
            WeaponModRuntimeState nextState =
                actor != null ? actor.GetComponent<WeaponModRuntimeState>() : null;

            BindRuntimeState(nextState);
        }

        private void BindRuntimeState(WeaponModRuntimeState nextState)
        {
            if (runtimeState != null)
                runtimeState.Changed -= HandleRuntimeStateChanged;

            runtimeState = nextState;

            if (runtimeState != null)
                runtimeState.Changed += HandleRuntimeStateChanged;

            Refresh();
        }

        private void HandleRuntimeStateChanged(WeaponModRuntimeState state)
        {
            Refresh();
        }

        private void HandleSlotSelected(RunWeaponModState state)
        {
            if (runtimeState == null || state?.Definition == null)
                return;

            runtimeState.ToggleWeaponMod(state.Definition);
        }

        private void Refresh()
        {
            if (stanceText != null)
            {
                if (runtimeState == null)
                {
                    stanceText.text = "WEAPON MASTERY: UNAVAILABLE";
                }
                else if (runtimeState.IsUsingBasicAttack)
                {
                    stanceText.text = "WEAPON MASTERY: BASIC ATTACK";
                }
                else
                {
                    string suffix = runtimeState.IsCoreActive ? " (DEFAULT)" : string.Empty;

                    stanceText.text =
                        $"WEAPON MASTERY: {runtimeState.ActiveMod.DisplayName} " +
                        $"Lv.{runtimeState.ActiveModState.Level}{suffix}";
                }
            }

            if (slots.Count != RunBuild.MaxWeaponModStances)
                return;

            for (int i = 0; i < slots.Count; i++)
            {
                WeaponModStanceSlot stanceSlot = (WeaponModStanceSlot)i;
                RunWeaponModState state = runtimeState?.GetAvailableMod(stanceSlot);

                bool active =
                    state?.Definition != null &&
                    runtimeState?.ActiveMod != null &&
                    runtimeState.ActiveMod.Id == state.Definition.Id;

                slots[i].Bind(stanceSlot, state, active, HandleSlotSelected);
            }
        }
    }
}