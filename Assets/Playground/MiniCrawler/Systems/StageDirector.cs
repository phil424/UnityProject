using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Systems
{
    public class StageDirector : MonoBehaviour
    {
        public enum LevelState
        {
            Idle,
            FightingMinions,
            FightingBoss
        }

        public static StageDirector Instance { get; private set; }

        public event Action<LevelState> StateChanged;

        public event Action<bool> LevelFinished;
        public event Action<int> CurrencyEarned;

        public event Action<
            PartyMemberDefinition,
            GameObject
        > PartyMemberSpawned;

        public event Action LevelCleared;

        [Header("Enemy Definitions")]
        [SerializeField] private ActorDefinition zombieDefinition;
        [SerializeField] private ActorDefinition bossDefinition;

        [Header("Spawn Points")]
        [SerializeField] private Transform partySpawnPoint;
        [SerializeField] private Transform[] zombieClusterSpawnPoints;
        [SerializeField] private Transform bossSpawnPoint;

        [Header("Party")]
        [SerializeField] private float partySpawnRadius = 1f;

        [Header("Zombies")]
        [SerializeField] private int clusterCount = 3;
        [SerializeField] private int zombiesPerCluster = 3;
        [SerializeField] private float zombieClusterRadius = 1.5f;

        private readonly List<GameObject> levelObjects = new();

        private LevelState state = LevelState.Idle;
        private int livingPartyMembers;
        private int livingMinions;
        private bool bossAlive;

        public LevelState State => state;
        public string StateName => state.ToString();
        public int LivingPartyMembers => livingPartyMembers;
        public int LivingMinions => livingMinions;
        public bool BossAlive => bossAlive;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Health.AnyDied += HandleActorDied;
        }

        private void OnDisable()
        {
            Health.AnyDied -= HandleActorDied;
        }

        [ContextMenu("Start Level")]
        public bool StartLevel(RunState runState)
        {
            if (SimulationPause.Instance != null)
                SimulationPause.Instance.Resume();

            if (runState == null)
            {
                Debug.LogWarning(
                    "Cannot start a level without run state."
                );

                return false;
            }

            if (runState.SelectedParty.Count <= 0)
            {
                Debug.LogWarning(
                    "Cannot start a level without a selected party."
                );

                return false;
            }

            ClearLevel();

            livingPartyMembers = 0;
            livingMinions = 0;
            bossAlive = false;

            SpawnSelectedParty(runState);

            if (livingPartyMembers <= 0)
            {
                Debug.LogError("No party members could be spawned.");
                ClearLevel();
                return false;
            }

            SetState(LevelState.FightingMinions);
            SpawnZombieClusters();

            Debug.Log($"Level started. Party: {livingPartyMembers}, Zombies: {livingMinions}");
            return true;
        }

        private void SpawnSelectedParty(RunState runState)
        {
            foreach (PartyMemberDefinition member in runState.SelectedParty)
            {
                if (member == null || member.ActorDefinition == null)
                {
                    Debug.LogWarning("A selected party member has no ActorDefinition.");
                    continue;
                }

                GameObject spawned = Spawn(
                    member.ActorDefinition,
                    RandomPointAround(partySpawnPoint.position, partySpawnRadius)
                );

                if (spawned == null)
                    continue;

                RunBuild build =
                    runState.GetBuild(member);

                PartyUpgradeApplicator.Apply(
                    spawned,
                    member,
                    build
                );

                PartyMemberSpawned?.Invoke(
                    member,
                    spawned
                );

                livingPartyMembers++;
            }
        }

        private void SpawnZombieClusters()
        {
            int count = Mathf.Min(clusterCount, zombieClusterSpawnPoints.Length);

            for (int cluster = 0; cluster < count; cluster++)
            {
                for (int i = 0; i < zombiesPerCluster; i++)
                {
                    GameObject spawned = Spawn(
                        zombieDefinition,
                        RandomPointAround(
                            zombieClusterSpawnPoints[cluster].position,
                            zombieClusterRadius
                        )
                    );

                    if (spawned != null)
                        livingMinions++;
                }
            }

            if (livingMinions <= 0)
                SpawnBoss();
        }

        private void SpawnBoss()
        {
            GameObject spawned = Spawn(bossDefinition, bossSpawnPoint.position);

            if (spawned == null)
                return;

            bossAlive = true;
            SetState(LevelState.FightingBoss);
            Debug.Log("Boss spawned.");
        }

        private GameObject Spawn(ActorDefinition definition, Vector3 position)
        {
            if (SimulationSpawner.Instance == null)
            {
                Debug.LogError("No SimulationSpawner in scene.");
                return null;
            }

            GameObject spawned = SimulationSpawner.Instance.Spawn(
                definition,
                position,
                Quaternion.identity,
                gameObject
            );

            if (spawned != null)
                levelObjects.Add(spawned);

            return spawned;
        }

        private void HandleActorDied(Health health)
        {
            if (health == null)
                return;

            GameObject deadObject = health.gameObject;

            if (!levelObjects.Contains(deadObject))
                return;

            if (deadObject.GetComponent<PartyMember>() != null)
            {
                livingPartyMembers = Mathf.Max(0, livingPartyMembers - 1);

                if (livingPartyMembers <= 0)
                    FinishLevel(false);

                return;
            }

            EnemyMember enemy = deadObject.GetComponent<EnemyMember>();

            if (enemy == null)
                return;

            CurrencyEarned?.Invoke(enemy.CurrencyReward);

            if (enemy.IsBoss)
            {
                bossAlive = false;
                FinishLevel(true);
                return;
            }

            livingMinions = Mathf.Max(0, livingMinions - 1);

            if (livingMinions <= 0 && state == LevelState.FightingMinions)
                SpawnBoss();
        }

        private void FinishLevel(bool won)
        {
            ClearLevel();

            LevelFinished?.Invoke(won);

            Debug.Log(won
                ? "Boss defeated. Level complete."
                : "Party defeated. Level ended.");
        }

        private void SetState(LevelState newState)
        {
            if (state == newState)
                return;

            state = newState;
            StateChanged?.Invoke(state);
        }

        private void ClearLevelObjects()
        {
            foreach (GameObject obj in levelObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }

            levelObjects.Clear();
        }

        private static Vector3 RandomPointAround(Vector3 center, float radius)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
            return center + new Vector3(offset.x, 0f, offset.y);
        }

        public void ClearLevel()
        {
            if (SimulationPause.Instance != null)
                SimulationPause.Instance.Resume();

            ClearLevelObjects();

            livingPartyMembers = 0;
            livingMinions = 0;
            bossAlive = false;

            LevelCleared?.Invoke();

            SetState(LevelState.Idle);
        }
    }
}
