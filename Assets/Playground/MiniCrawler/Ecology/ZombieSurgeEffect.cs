using MiniCrawler.Core;
using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Ecology
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class ZombieSurgeAmbientEffect : MonoBehaviour
    {
        [Header("Event")]
        [SerializeField] private WorldEventDefinition worldEvent;
        [SerializeField] private ActorDefinition affectedActorDefinition;

        [Header("Ambient Pressure")]
        [SerializeField, Min(0)] private int activationBonusPerSpawnPoint = 1;
        [SerializeField, Min(0)] private int pulseBonusPerSpawnPoint = 1;
        [SerializeField, Min(1f)] private float pulseIntervalWorldMinutes = 60f;

        private StageDirector stageDirector;
        private RunState observedRun;

        private bool appliedForCurrentLevel;
        private bool hasNextPulse;
        private WorldTimestamp nextPulseAt;

        private void Awake()
        {
            stageDirector = StageDirector.Instance;
        }

        private void Update()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            RunState currentRun = RunProgress.CurrentRun;

            if (!ReferenceEquals(currentRun, observedRun))
            {
                observedRun = currentRun;
                ResetLocalState();
            }

            if (currentRun == null ||
                worldEvent == null ||
                affectedActorDefinition == null ||
                !currentRun.WorldClock.IsInitialized)
            {
                return;
            }

            if (stageDirector == null ||
                stageDirector.State != StageDirector.LevelState.FightingMinions)
            {
                appliedForCurrentLevel = false;
                hasNextPulse = false;
                return;
            }

            if (!currentRun.WorldEvents.TryGetActive(worldEvent, out ActiveWorldEvent activeEvent))
            {
                appliedForCurrentLevel = false;
                hasNextPulse = false;
                return;
            }

            WorldTimestamp currentTime = currentRun.WorldClock.CurrentTime;

            if (!appliedForCurrentLevel)
            {
                SpawnSurgeActors(activationBonusPerSpawnPoint);

                appliedForCurrentLevel = true;
                nextPulseAt = currentTime.AddMinutes(pulseIntervalWorldMinutes);
                hasNextPulse = pulseBonusPerSpawnPoint > 0;
            }

            if (!hasNextPulse)
                return;

            while (currentTime.CompareTo(nextPulseAt) >= 0 &&
                   nextPulseAt.CompareTo(activeEvent.EndsAt) < 0)
            {
                SpawnSurgeActors(pulseBonusPerSpawnPoint);
                nextPulseAt = nextPulseAt.AddMinutes(pulseIntervalWorldMinutes);
            }
        }

        private void SpawnSurgeActors(int amountPerSpawnPoint)
        {
            if (amountPerSpawnPoint <= 0)
                return;

            int spawnedCount = 0;

            foreach (AmbientSpawnPoint spawnPoint in
                     FindObjectsByType<AmbientSpawnPoint>(FindObjectsSortMode.None))
            {
                if (spawnPoint == null || spawnPoint.ActorDefinition != affectedActorDefinition)
                    continue;

                spawnedCount += spawnPoint.SpawnAdditional(amountPerSpawnPoint);
            }

            if (spawnedCount > 0)
            {
                Debug.Log(
                    $"Zombie Surge added {spawnedCount} ambient enemies.",
                    this
                );
            }
        }

        private void ResetLocalState()
        {
            appliedForCurrentLevel = false;
            hasNextPulse = false;
            nextPulseAt = default;
        }
    }
}