using MiniCrawler.Core;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Ecology
{
    [DisallowMultipleComponent]
    public sealed class AmbientSpawnPoint : MonoBehaviour
    {
        [SerializeField] private ActorDefinition actorDefinition;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField, Min(0f)] private float spawnRadius = 1.5f;
        [SerializeField, Min(0.1f)] private float engagementRadius = 3.5f;

        private StageDirector stageDirector;
        private bool spawnedForLevel;

        public ActorDefinition ActorDefinition => actorDefinition;

        private void Start()
        {
            EnsureStageDirector();

            if (stageDirector == null)
            {
                Debug.LogWarning("AmbientSpawnPoint could not find StageDirector.", this);
                return;
            }

            stageDirector.StateChanged += HandleStageStateChanged;
            stageDirector.LevelCleared += HandleLevelCleared;

            if (stageDirector.State != StageDirector.LevelState.Idle)
                SpawnForLevel();
        }

        private void OnDestroy()
        {
            if (stageDirector == null)
                return;

            stageDirector.StateChanged -= HandleStageStateChanged;
            stageDirector.LevelCleared -= HandleLevelCleared;
        }

        public int SpawnAdditional(int additionalCount)
        {
            if (additionalCount <= 0)
                return 0;

            EnsureStageDirector();

            if (stageDirector == null ||
                stageDirector.State != StageDirector.LevelState.FightingMinions ||
                actorDefinition == null)
            {
                return 0;
            }

            return SpawnActors(additionalCount);
        }

        private void HandleStageStateChanged(StageDirector.LevelState state)
        {
            if (state == StageDirector.LevelState.FightingMinions)
                SpawnForLevel();
        }

        private void HandleLevelCleared()
        {
            spawnedForLevel = false;
        }

        private void SpawnForLevel()
        {
            if (spawnedForLevel || stageDirector == null || actorDefinition == null)
                return;

            spawnedForLevel = true;
            SpawnActors(count);
        }

        private int SpawnActors(int amount)
        {
            int spawnedCount = 0;

            for (int i = 0; i < amount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;

                Vector3 position = transform.position + new Vector3(offset.x, 0f, offset.y);
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                GameObject spawned = stageDirector.SpawnAmbientActor(
                    actorDefinition,
                    new Pose(position, rotation),
                    engagementRadius
                );

                if (spawned != null)
                    spawnedCount++;
            }

            return spawnedCount;
        }

        private void EnsureStageDirector()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;
        }

        private void OnValidate()
        {
            count = Mathf.Max(1, count);
            spawnRadius = Mathf.Max(0f, spawnRadius);
            engagementRadius = Mathf.Max(0.1f, engagementRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, engagementRadius);
        }
    }
}