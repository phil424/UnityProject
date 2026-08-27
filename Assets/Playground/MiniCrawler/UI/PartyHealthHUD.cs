using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.UI
{
    public class PartyHealthHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private StageDirector stageDirector;

        [SerializeField]
        private Transform entryRoot;

        [SerializeField]
        private PartyHealthEntryUI entryPrefab;

        private readonly List<PartyHealthEntryUI>
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
                entryPrefab == null ||
                entryRoot == null
            )
            {
                return;
            }

            PartyHealthEntryUI entry =
                Instantiate(
                    entryPrefab,
                    entryRoot
                );

            entry.Bind(
                definition,
                actor
            );

            entries.Add(entry);
        }

        private void HandleLevelCleared()
        {
            ClearEntries();
        }

        private void ClearEntries()
        {
            foreach (
                PartyHealthEntryUI entry
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