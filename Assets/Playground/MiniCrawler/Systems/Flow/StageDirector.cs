using System;
using System.Collections;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Ecology;
using MiniCrawler.Encounters;
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
        public event Action<PersistentResourceTier, int> PersistentResourceEarned;
        public event Action<RunRewardContext> RewardChoiceEarned;
        
        public event Action<LevelEncounter> EncounterStarted;
        public event Action<LevelEncounter, LevelEncounterPhase, int> EncounterPhaseStarted;
        public event Action<LevelEncounter> EncounterCompleted;

        public event Action<PartyMemberDefinition, GameObject> PartyMemberSpawned;
        public event Action<PartyMemberDefinition, GameObject> PartyMemberRuntimeChanged;
        public event Action LevelCleared;

        [Header("Enemy Definitions")]
        [SerializeField] private ActorDefinition bossDefinition;

        [Header("Spawn Points")]
        [SerializeField] private Transform partySpawnPoint;
        [SerializeField] private LevelSpawnSource bossSpawnSource;

        [Header("Party")]
        [SerializeField] private float partySpawnRadius = 1f;
        
        [Header("Prototype Region Progression")]
        [SerializeField] private bool autoSpawnBossWhenEncounterContentExhausted;

        [Header("Encounters")]
        [SerializeField] private Transform encountersRoot;

        private readonly List<GameObject> levelObjects = new();
        private readonly Dictionary<string, GameObject> spawnedPartyActors = new();
        
        private LevelEncounter[] encounters = Array.Empty<LevelEncounter>();
        private LevelSpawnGroup[] minionSpawnGroups = Array.Empty<LevelSpawnGroup>();

        public int PendingMinionSpawns => pendingMinionSpawns;
        public int UnstartedMinionSpawnGroups => CountUnstartedMinionSpawnGroups();
        public IReadOnlyList<LevelEncounter> Encounters => encounters;

        private LevelState state = LevelState.Idle;
        private int livingPartyMembers;
        private int livingMinions;
        private int pendingMinionSpawns;
        private bool bossAlive;
        private bool levelOutcomeResolved;
        private Coroutine defeatCleanupRoutine;

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
            pendingMinionSpawns = 0;
            bossAlive = false;
            levelOutcomeResolved = false;

            SpawnSelectedParty(runState);

            if (livingPartyMembers <= 0)
            {
                Debug.LogError("No party members could be spawned.");
                ClearLevel();
                return false;
            }

            SetState(LevelState.FightingMinions);
            StartMinionSpawnGroups();

            Debug.Log(
                $"Level started. Party: {livingPartyMembers}, Living minions: {livingMinions}, " +
                $"Scheduled: {pendingMinionSpawns}"
            );
            return true;
        }
        
        private void RefreshEncounterContent()
        {
            if (encountersRoot == null)
            {
                encounters = Array.Empty<LevelEncounter>();
                minionSpawnGroups = Array.Empty<LevelSpawnGroup>();

                Debug.LogWarning(
                    "StageDirector has no Encounters Root assigned.",
                    this
                );

                return;
            }

            encounters =
                encountersRoot.GetComponentsInChildren<LevelEncounter>(true);

            List<LevelSpawnGroup> ownedGroups = new();

            foreach (LevelEncounter encounter in encounters)
            {
                if (encounter == null)
                    continue;

                encounter.RefreshRuntimeContent();

                foreach (LevelSpawnGroup group in encounter.SpawnGroups)
                {
                    if (group != null && !ownedGroups.Contains(group))
                        ownedGroups.Add(group);
                }
            }

            minionSpawnGroups = ownedGroups.ToArray();
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

                GameObject spawned = Spawn(member.ActorDefinition, RandomPointAround(partySpawnPoint.position, partySpawnRadius));

                if (spawned == null)
                    continue;

                RunBuild build = runState.GetBuild(member);

                PartyUpgradeApplicator.Apply(spawned, member, build);

                PartyAbilityApplicator.Apply(spawned, build);

                spawnedPartyActors[member.Id] = spawned;

                PartyMemberSpawned?.Invoke(member, spawned);

                livingPartyMembers++;
            }
        }
        
        public bool RefreshPartyMemberRuntime(PartyMemberDefinition member, RunBuild build)
        {
            if (member == null || build == null)
                return false;

            if (!spawnedPartyActors.TryGetValue(member.Id, out GameObject actor) || actor == null)
                return false;

            PartyUpgradeApplicator.Apply(actor, member, build, restoreHealth: false);
            PartyAbilityApplicator.Apply(actor, build);

            PartyMemberRuntimeChanged?.Invoke(member, actor);

            return true;
        }
        
        public bool IsCurrentPartyMember(PartyMember partyMember)
        {
            if (partyMember == null)
                return false;

            foreach (GameObject actor in spawnedPartyActors.Values)
            {
                if (actor == partyMember.gameObject)
                    return true;
            }

            return false;
        }
        
        public bool TryAwardRewardChoice(RunRewardVector rewardVector = RunRewardVector.Any, string sourceLabel = null)
        {
            if (state == LevelState.Idle)
                return false;

            RewardChoiceEarned?.Invoke(new RunRewardContext(rewardVector, sourceLabel));
            return true;
        }
        
        private void StartMinionSpawnGroups()
        {
            StopMinionSpawnGroups();
            RefreshEncounterContent();
            SubscribeToEncounterLifecycle();

            pendingMinionSpawns = 0;

            if (minionSpawnGroups == null || minionSpawnGroups.Length == 0)
            {
                TryAdvanceFromMinionPhase();
                return;
            }

            foreach (LevelSpawnGroup group in minionSpawnGroups)
            {
                if (group == null)
                    continue;

                group.PrepareForLevel();

                if (group.ConfiguredSpawnCount <= 0)
                    continue;

                group.SpawningStarted += HandleMinionGroupSpawningStarted;
                group.SpawnRequested += HandleMinionSpawnRequested;
            }

            PrepareEncountersForLevel();

            foreach (LevelSpawnGroup group in minionSpawnGroups)
            {
                if (group != null && group.StartSpawning)
                    group.BeginSpawning();
            }

            TryAdvanceFromMinionPhase();
        }

        private void StopMinionSpawnGroups()
        {
            UnsubscribeFromEncounterLifecycle();

            if (minionSpawnGroups == null)
                return;

            foreach (LevelSpawnGroup group in minionSpawnGroups)
            {
                if (group == null)
                    continue;

                group.SpawningStarted -= HandleMinionGroupSpawningStarted;
                group.SpawnRequested -= HandleMinionSpawnRequested;
                group.StopSpawning();
            }
        }
        
        private void HandleMinionGroupSpawningStarted(LevelSpawnGroup group, int spawnCount)
        {
            pendingMinionSpawns += Mathf.Max(0, spawnCount);
        }

        private void HandleMinionSpawnRequested(LevelSpawnGroup group, ActorDefinition definition, Pose pose)
        {
            pendingMinionSpawns = Mathf.Max(0, pendingMinionSpawns - 1);

            GameObject spawned = Spawn(definition, pose);

            if (spawned != null)
            {
                livingMinions++;
                group?.RegisterSpawnedActor(spawned);
            }

            TryAdvanceFromMinionPhase();
        }
        
        private int CountUnstartedMinionSpawnGroups()
        {
            if (minionSpawnGroups == null)
                return 0;

            int count = 0;

            foreach (LevelSpawnGroup group in minionSpawnGroups)
            {
                if (group != null &&
                    group.ConfiguredSpawnCount > 0 &&
                    !group.IsSpawningStarted)
                {
                    count++;
                }
            }

            return count;
        }

        private void TryAdvanceFromMinionPhase()
        {
            if (state != LevelState.FightingMinions || !autoSpawnBossWhenEncounterContentExhausted)
                return;

            if (livingMinions > 0 || pendingMinionSpawns > 0 || UnstartedMinionSpawnGroups > 0)
                return;

            SpawnBoss();
        }
        
        public bool TryStartBossPhase()
        {
            if (state != LevelState.FightingMinions || bossAlive)
                return false;

            SpawnBoss();
            return bossAlive;
        }

        private void SpawnBoss()
        {
            if (bossSpawnSource == null)
            {
                Debug.LogWarning("No boss spawn source is assigned.");
                return;
            }

            GameObject spawned = Spawn(
                bossDefinition,
                bossSpawnSource.GetSpawnPose()
            );

            if (spawned == null)
                return;

            bossAlive = true;
            SetState(LevelState.FightingBoss);
            Debug.Log("Boss spawned.");
        }
        
        public GameObject SpawnAmbientActor(ActorDefinition definition, Pose pose, float engagementRadius)
        {
            return SpawnAmbientActor(definition, pose, engagementRadius, 0f, 0f, 0f);
        }

        public GameObject SpawnAmbientActor(
            ActorDefinition definition,
            Pose pose,
            float engagementRadius,
            float wanderRadius,
            float wanderPauseMinSeconds,
            float wanderPauseMaxSeconds)
        {
            if (state == LevelState.Idle)
                return null;

            GameObject spawned = Spawn(definition, pose);

            if (spawned == null)
                return null;

            AmbientEnemy ambientEnemy = spawned.GetComponent<AmbientEnemy>();

            if (ambientEnemy == null)
                ambientEnemy = spawned.AddComponent<AmbientEnemy>();

            ambientEnemy.Configure(
                pose.position,
                engagementRadius,
                wanderRadius,
                wanderPauseMinSeconds,
                wanderPauseMaxSeconds
            );

            return spawned;
        }
        
        private GameObject Spawn(ActorDefinition definition, Pose pose)
        {
            return Spawn(definition, pose.position, pose.rotation);
        }

        private GameObject Spawn(ActorDefinition definition, Vector3 position)
        {
            return Spawn(definition, position, Quaternion.identity);
        }

        private GameObject Spawn(
            ActorDefinition definition,
            Vector3 position,
            Quaternion rotation
        )
        {
            if (SimulationSpawner.Instance == null)
            {
                Debug.LogError("No SimulationSpawner in scene.");
                return null;
            }

            GameObject spawned = SimulationSpawner.Instance.Spawn(
                definition,
                position,
                rotation,
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

            foreach (PersistentResourceReward reward in enemy.PersistentResourceRewards)
            {
                if (reward == null || !reward.IsConfigured)
                    continue;

                PersistentResourceEarned?.Invoke(reward.Tier, reward.Amount);
            }

            if (enemy.RewardChoicesOnDeath > 0)
            {
                Actor deadActor = deadObject.GetComponent<Actor>();
                string rewardSourceLabel = deadActor != null ? deadActor.ActorName : deadObject.name;

                for (int i = 0; i < enemy.RewardChoicesOnDeath; i++)
                    TryAwardRewardChoice(RunRewardVector.Any, rewardSourceLabel);
            }

            if (deadObject.GetComponent<AmbientEnemy>() != null)
                return;

            if (enemy.IsBoss)
            {
                bossAlive = false;
                FinishLevel(true);
                return;
            }

            livingMinions = Mathf.Max(0, livingMinions - 1);
            TryAdvanceFromMinionPhase();
        }

        private void FinishLevel(bool won)
        {
            if (levelOutcomeResolved)
                return;

            levelOutcomeResolved = true;

            if (won)
            {
                ClearLevel();
                LevelFinished?.Invoke(true);

                Debug.Log("Boss defeated. Level complete.");
                return;
            }

            StopMinionSpawnGroups();

            LevelFinished?.Invoke(false);

            if (defeatCleanupRoutine != null)
                StopCoroutine(defeatCleanupRoutine);

            defeatCleanupRoutine = StartCoroutine(CompleteDefeatAfterPartyPresentation());

            Debug.Log("Party defeated. Expedition defeat resolved; waiting for party death presentation cleanup.");
        }
        
        private IEnumerator CompleteDefeatAfterPartyPresentation()
        {
            const float safetyTimeout = 5f;

            float remainingTimeout = safetyTimeout;

            yield return null;

            while (HasRemainingPartyRuntimeObjects() && remainingTimeout > 0f)
            {
                remainingTimeout -= Time.deltaTime;
                yield return null;
            }

            if (HasRemainingPartyRuntimeObjects())
            {
                Debug.LogWarning(
                    "Party defeat presentation did not clean up all party runtime actors before the safety timeout. " +
                    "The level will now be cleared.",
                    this
                );
            }

            defeatCleanupRoutine = null;
            ClearLevel();
        }

        private bool HasRemainingPartyRuntimeObjects()
        {
            foreach (GameObject actor in spawnedPartyActors.Values)
            {
                if (actor != null)
                    return true;
            }

            return false;
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
            spawnedPartyActors.Clear();
        }

        private static Vector3 RandomPointAround(Vector3 center, float radius)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
            return center + new Vector3(offset.x, 0f, offset.y);
        }

        public void ClearLevel()
        {
            if (defeatCleanupRoutine != null)
            {
                StopCoroutine(defeatCleanupRoutine);
                defeatCleanupRoutine = null;
            }

            if (SimulationPause.Instance != null)
                SimulationPause.Instance.Resume();

            StopMinionSpawnGroups();
            ClearEncounters();
            ClearLevelObjects();

            livingPartyMembers = 0;
            livingMinions = 0;
            pendingMinionSpawns = 0;
            bossAlive = false;

            LevelCleared?.Invoke();

            SetState(LevelState.Idle);
        }
        
        private void SubscribeToEncounterLifecycle()
        {
            if (encounters == null)
                return;

            foreach (LevelEncounter encounter in encounters)
            {
                if (encounter == null)
                    continue;

                encounter.Started += HandleEncounterStarted;
                encounter.PhaseStarted += HandleEncounterPhaseStarted;
                encounter.Completed += HandleEncounterCompleted;
            }
        }

        private void UnsubscribeFromEncounterLifecycle()
        {
            if (encounters == null)
                return;

            foreach (LevelEncounter encounter in encounters)
            {
                if (encounter == null)
                    continue;

                encounter.Started -= HandleEncounterStarted;
                encounter.PhaseStarted -= HandleEncounterPhaseStarted;
                encounter.Completed -= HandleEncounterCompleted;
            }
        }

        private void HandleEncounterStarted(LevelEncounter encounter)
        {
            EncounterStarted?.Invoke(encounter);
        }

        private void HandleEncounterPhaseStarted(LevelEncounter encounter, LevelEncounterPhase phase, int phaseIndex)
        {
            EncounterPhaseStarted?.Invoke(encounter, phase, phaseIndex);
        }

        private void HandleEncounterCompleted(LevelEncounter encounter)
        {
            EncounterCompleted?.Invoke(encounter);

            if (encounter == null)
                return;

            LevelEncounterDefinitionBinding binding = encounter.GetComponent<LevelEncounterDefinitionBinding>();
            EncounterDefinition definition = binding != null ? binding.Definition : null;

            if (definition == null || definition.RewardChoicesOnCompletion <= 0)
                return;

            for (int i = 0; i < definition.RewardChoicesOnCompletion; i++)
                TryAwardRewardChoice(definition.RewardVector, encounter.DisplayName);
        }
        
        private void PrepareEncountersForLevel()
        {
            if (encounters == null)
                return;

            foreach (LevelEncounter encounter in encounters)
            {
                if (encounter != null)
                    encounter.PrepareForLevel();
            }
        }

        private void ClearEncounters()
        {
            if (encounters == null)
                return;

            foreach (LevelEncounter encounter in encounters)
            {
                if (encounter != null)
                    encounter.ClearForLevel();
            }
        }
    }
}
