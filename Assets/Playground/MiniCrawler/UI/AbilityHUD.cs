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
                stageDirector =
                    StageDirector.Instance;
            }

            if (stageDirector == null)
                return;

            stageDirector.PartyMemberSpawned +=
                HandlePartyMemberSpawned;

            stageDirector.LevelCleared +=
                HandleLevelCleared;
        }

        private void OnDisable()
        {
            if (stageDirector != null)
            {
                stageDirector.PartyMemberSpawned -=
                    HandlePartyMemberSpawned;

                stageDirector.LevelCleared -=
                    HandleLevelCleared;
            }

            ClearEntries();
        }

        private void HandlePartyMemberSpawned(
            PartyMemberDefinition definition,
            GameObject actor
        )
        {
            if (
                definition == null ||
                actor == null ||
                entryRoot == null ||
                abilityButtonPrefab == null
            )
            {
                return;
            }

            ActorAbility[] abilities = actor.GetComponentsInChildren<ActorAbility>(false);

            foreach (
                ActorAbility ability in abilities
            )
            {
                if (ability == null)
                    continue;

                AbilityButtonUI entry =
                    Instantiate(
                        abilityButtonPrefab,
                        entryRoot
                    );

                entry.Bind(
                    definition,
                    ability
                );

                entries.Add(entry);
            }
        }

        private void HandleLevelCleared()
        {
            ClearEntries();
        }

        private void ClearEntries()
        {
            foreach (
                AbilityButtonUI entry
                in entries
            )
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