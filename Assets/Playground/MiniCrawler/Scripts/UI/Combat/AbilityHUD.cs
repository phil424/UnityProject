using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    public class AbilityHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private StageDirector stageDirector;

        [SerializeField]
        private Transform preparedEntryRoot;

        [SerializeField]
        private Transform expeditionEntryRoot;

        [SerializeField]
        private AbilityButtonUI abilityButtonPrefab;

        private readonly List<AbilityButtonUI> entries = new();

        private PartyMemberDefinition currentDefinition;
        private GameObject currentActor;

        private void OnEnable()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (stageDirector == null)
                return;

            stageDirector.PartyMemberSpawned += HandlePartyMemberSpawned;
            stageDirector.PartyMemberRuntimeChanged += HandlePartyMemberRuntimeChanged;
            stageDirector.LevelCleared += HandleLevelCleared;
        }

        private void OnDisable()
        {
            if (stageDirector != null)
            {
                stageDirector.PartyMemberSpawned -= HandlePartyMemberSpawned;
                stageDirector.PartyMemberRuntimeChanged -= HandlePartyMemberRuntimeChanged;
                stageDirector.LevelCleared -= HandleLevelCleared;
            }

            currentDefinition = null;
            currentActor = null;

            ClearEntries();
        }

        private void HandlePartyMemberSpawned(PartyMemberDefinition definition, GameObject actor)
        {
            if (!IsDisplayedMember(definition))
                return;

            currentDefinition = definition;
            currentActor = actor;

            RebuildEntries();
        }

        private void HandlePartyMemberRuntimeChanged(PartyMemberDefinition definition, GameObject actor)
        {
            if (!IsDisplayedMember(definition))
                return;

            currentDefinition = definition;
            currentActor = actor;

            RebuildEntries();
        }

        private bool IsDisplayedMember(PartyMemberDefinition definition)
        {
            if (definition == null)
                return false;

            if (RunProgress.SelectedParty.Count <= 0)
                return true;

            PartyMemberDefinition selected = RunProgress.SelectedParty[0];

            return selected != null && selected.Id == definition.Id;
        }

        private void RebuildEntries()
        {
            ClearEntries();

            if (currentDefinition == null || currentActor == null || preparedEntryRoot == null || expeditionEntryRoot == null || abilityButtonPrefab == null)
            {
                return;
            }

            RunBuild build = RunProgress.GetBuild(currentDefinition);

            CreateEntry(preparedEntryRoot, "SIGNATURE", build.GetPreparedAbilityState(CharacterAbilityRole.Signature));

            CreateEntry(preparedEntryRoot, "UTILITY", build.GetPreparedAbilityState(CharacterAbilityRole.Utility));

            CreateEntry(preparedEntryRoot, "SUSTAIN", build.GetPreparedAbilityState(CharacterAbilityRole.Sustain));

            for (int i = 0; i < RunBuild.MaxExpeditionAbilitySlots; i++)
            {
                CreateEntry(expeditionEntryRoot, $"EXPEDITION {i + 1}", build.GetExpeditionAbilityState(i));
            }
        }

        private void CreateEntry(Transform root, string slotLabel, RunAbilityState abilityState)
        {
            AbilityButtonUI entry = Instantiate(abilityButtonPrefab, root);

            ActorAbility runtimeAbility = abilityState?.Definition != null ? FindRuntimeAbility(currentActor, abilityState.Definition) : null;

            entry.Bind(currentDefinition, slotLabel, abilityState, runtimeAbility);

            entries.Add(entry);
        }

        private static ActorAbility FindRuntimeAbility(GameObject actor, AbilityDefinition definition)
        {
            if (actor == null || definition == null)
                return null;

            ActorAbility[] abilities = actor.GetComponentsInChildren<ActorAbility>(true);

            foreach (ActorAbility ability in abilities)
            {
                if (ability?.Definition != null && ability.Definition.Id == definition.Id)
                {
                    return ability;
                }
            }

            return null;
        }

        private void HandleLevelCleared()
        {
            currentDefinition = null;
            currentActor = null;

            ClearEntries();
        }

        private void ClearEntries()
        {
            foreach (AbilityButtonUI entry in entries)
            {
                if (entry == null)
                    continue;

                entry.gameObject.SetActive(false);
                Destroy(entry.gameObject);
            }

            entries.Clear();
        }
    }
}
