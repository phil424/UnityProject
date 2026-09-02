using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
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
        private Transform entryRoot;

        [SerializeField]
        private AbilityButtonUI abilityButtonPrefab;

        private readonly List<AbilityButtonUI>
            entries = new();

        private void OnEnable()
        {
            if (stageDirector == null)
            {
                stageDirector = StageDirector.Instance;
            }

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

            ClearEntries();
        }

        private void HandlePartyMemberSpawned(PartyMemberDefinition definition, GameObject actor)
        {
            AddMissingAbilityEntries(definition, actor);
        }

        private void HandlePartyMemberRuntimeChanged(PartyMemberDefinition definition, GameObject actor)
        {
            AddMissingAbilityEntries(definition, actor);
        }

        private void AddMissingAbilityEntries(PartyMemberDefinition definition, GameObject actor)
        {
            if (definition == null || actor == null || entryRoot == null || abilityButtonPrefab == null)
                return;

            ActorAbility[] abilities = actor.GetComponentsInChildren<ActorAbility>(false);

            foreach (ActorAbility ability in abilities)
            {
                if (ability == null || HasEntryFor(ability))
                    continue;

                AbilityButtonUI entry = Instantiate(abilityButtonPrefab, entryRoot);
                entry.Bind(definition, ability);

                entries.Add(entry);
            }
        }

        private bool HasEntryFor(ActorAbility ability)
        {
            foreach (AbilityButtonUI entry in entries)
            {
                if (entry != null && entry.Ability == ability)
                    return true;
            }

            return false;
        }

        private void HandleLevelCleared()
        {
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