using System.Collections.Generic;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Ecology;
using MiniCrawler.Movement;
using MiniCrawler.Progress;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public sealed class PreparationCombatShowcase : MonoBehaviour
    {
        private const string ImmortalityModifierId = "PreparationShowcaseImmortality";

        [Header("World")]
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private Transform showcaseOrigin;
        [SerializeField] private GameObject combatFeedbackRoot;
        [SerializeField] private AmbientRouteGraph routeGraph;

        [Header("Enemies")]
        [SerializeField] private ActorDefinition[] enemyPool;
        [SerializeField, Min(1)] private int initialEnemyCount = 6;
        [SerializeField, Min(1)] private int maxLivingEnemies = 8;
        [SerializeField, Min(0.1f)] private float spawnIntervalMin = 1f;
        [SerializeField, Min(0.1f)] private float spawnIntervalMax = 2f;
        [SerializeField, Min(0f)] private float routeSpawnScatterRadius = 0.4f;

        [Header("Enemy Ambient Behaviour")]
        [SerializeField, Min(0.1f)] private float enemyEngagementRadius = 3.5f;
        [SerializeField, Min(0f)] private float enemyWanderRadius = 2.5f;
        [SerializeField, Min(0f)] private float enemyWanderPauseMinSeconds = 1f;
        [SerializeField, Min(0f)] private float enemyWanderPauseMaxSeconds = 3f;

        [Header("Route Travel")]
        [SerializeField, Min(0.1f)] private float routeArrivalDistance = 0.35f;

        [Header("Camera")]
        [SerializeField, Min(0f)] private float cameraFollowSharpness = 6f;

        [Header("Showcase Character")]
        [SerializeField, Min(0f)] private float immortalityArmourBonus = 1000000f;

        private readonly List<GameObject> spawnedEnemies = new();
        private readonly List<AmbientRouteSegment> validSegments = new();

        private PartyMemberDefinition currentDefinition;
        private GameObject showcaseCharacter;

        private Vector3 savedCameraPosition;
        private Quaternion savedCameraRotation;
        private Vector3 cameraOffset;

        private bool savedCombatFeedbackState;
        private bool running;
        private float spawnTimer;

        public void Show(PartyMemberDefinition definition)
        {
            if (definition == null)
            {
                Stop();
                return;
            }

            if (running && currentDefinition == definition && showcaseCharacter != null)
            {
                ApplyCharacterBuild();
                return;
            }

            if (!running)
                BeginShowcase();

            if (!running)
                return;

            currentDefinition = definition;

            ClearActors();
            SpawnCharacter();

            for (int i = 0; i < initialEnemyCount; i++)
                SpawnEnemy();

            ScheduleNextSpawn();
        }

        public void Stop()
        {
            if (!running)
                return;

            ClearActors();

            if (gameplayCamera != null)
            {
                gameplayCamera.transform.position = savedCameraPosition;
                gameplayCamera.transform.rotation = savedCameraRotation;
            }

            if (combatFeedbackRoot != null)
                combatFeedbackRoot.SetActive(savedCombatFeedbackState);

            currentDefinition = null;
            running = false;
        }

        private void Update()
        {
            if (!running || showcaseCharacter == null)
                return;

            if (CountLivingEnemies() >= maxLivingEnemies)
                return;

            spawnTimer -= Time.deltaTime;

            if (spawnTimer > 0f)
                return;

            SpawnEnemy();
            ScheduleNextSpawn();
        }

        private void LateUpdate()
        {
            if (!running || gameplayCamera == null || showcaseCharacter == null)
                return;

            Vector3 targetPosition = showcaseCharacter.transform.position + cameraOffset;

            if (cameraFollowSharpness <= 0f)
            {
                gameplayCamera.transform.position = targetPosition;
                return;
            }

            float interpolation = 1f - Mathf.Exp(-cameraFollowSharpness * Time.deltaTime);
            gameplayCamera.transform.position =
                Vector3.Lerp(gameplayCamera.transform.position, targetPosition, interpolation);
        }

        private void BeginShowcase()
        {
            if (gameplayCamera == null || showcaseOrigin == null || routeGraph == null)
            {
                Debug.LogError(
                    $"{nameof(PreparationCombatShowcase)} requires a Camera, Showcase Origin and Ambient Route Graph.",
                    this
                );

                return;
            }

            savedCameraPosition = gameplayCamera.transform.position;
            savedCameraRotation = gameplayCamera.transform.rotation;
            cameraOffset = gameplayCamera.transform.position - showcaseOrigin.position;

            if (combatFeedbackRoot != null)
            {
                savedCombatFeedbackState = combatFeedbackRoot.activeSelf;
                combatFeedbackRoot.SetActive(false);
            }

            running = true;
        }

        private void SpawnCharacter()
        {
            if (currentDefinition?.ActorDefinition == null ||
                SimulationSpawner.Instance == null ||
                showcaseOrigin == null)
            {
                return;
            }

            showcaseCharacter = SimulationSpawner.Instance.Spawn(
                currentDefinition.ActorDefinition,
                showcaseOrigin.position,
                showcaseOrigin.rotation,
                gameObject
            );

            if (showcaseCharacter == null)
                return;

            showcaseCharacter.name = $"{currentDefinition.DisplayName} Preparation Showcase";

            ApplyCharacterBuild();
            ConfigureRouteWanderer(showcaseCharacter);
        }

        private void ApplyCharacterBuild()
        {
            if (showcaseCharacter == null || currentDefinition == null)
                return;

            PersistentMemberProgress persistentProgress = PersistentProgression.GetMemberProgress(currentDefinition);

            RunBuild build = new();
            build.InitializeGearLevels(
                persistentProgress.WeaponLevel,
                persistentProgress.ArmourLevel,
                persistentProgress.FocusLevel
            );
            build.InitializeStartingAbilities(currentDefinition.StartingAbilities);
            PersistentProgression.ApplyMasteryToBuild(currentDefinition, build);

            PartyUpgradeApplicator.Apply(showcaseCharacter, currentDefinition, build);
            PartyAbilityApplicator.Apply(showcaseCharacter, build);
            PartyWeaponModApplicator.Apply(showcaseCharacter, build);

            RuntimeStatModifiers modifiers = showcaseCharacter.GetComponent<RuntimeStatModifiers>();

            if (modifiers == null)
                modifiers = showcaseCharacter.AddComponent<RuntimeStatModifiers>();

            modifiers.ApplyOrRefresh(
                ImmortalityModifierId,
                86400f,
                0f,
                0f,
                0f,
                immortalityArmourBonus
            );
        }

        private void SpawnEnemy()
        {
            if (SimulationSpawner.Instance == null ||
                showcaseCharacter == null ||
                enemyPool == null ||
                enemyPool.Length == 0 ||
                !TryGetRouteSpawnPose(out Pose pose))
            {
                return;
            }

            ActorDefinition definition = GetRandomEnemyDefinition();

            if (definition == null)
                return;

            GameObject spawned = SimulationSpawner.Instance.Spawn(
                definition,
                pose.position,
                pose.rotation,
                gameObject
            );

            if (spawned == null)
                return;

            spawnedEnemies.Add(spawned);
            ConfigureAmbientEnemy(spawned, pose.position);
        }

        private void ConfigureRouteWanderer(GameObject actor)
        {
            if (actor == null || routeGraph == null)
                return;

            PreparationRouteWanderer wanderer = actor.GetComponent<PreparationRouteWanderer>();

            if (wanderer == null)
                wanderer = actor.AddComponent<PreparationRouteWanderer>();

            wanderer.Configure(routeGraph, routeArrivalDistance);
        }
        
        private void ConfigureAmbientEnemy(GameObject enemy, Vector3 homePosition)
        {
            if (enemy == null)
                return;

            AmbientEnemy ambientEnemy = enemy.GetComponent<AmbientEnemy>();

            if (ambientEnemy == null)
                ambientEnemy = enemy.AddComponent<AmbientEnemy>();

            ambientEnemy.Configure(
                homePosition,
                enemyEngagementRadius,
                enemyWanderRadius,
                enemyWanderPauseMinSeconds,
                enemyWanderPauseMaxSeconds
            );
        }

        private bool TryGetRouteSpawnPose(out Pose pose)
        {
            validSegments.Clear();

            foreach (AmbientRouteSegment segment in routeGraph.Segments)
            {
                if (segment != null && segment.IsConfigured)
                    validSegments.Add(segment);
            }

            Vector3 position;

            if (validSegments.Count > 0)
            {
                AmbientRouteSegment segment = ChooseWeightedSegment();
                position = Vector3.Lerp(segment.NodeA.Position, segment.NodeB.Position, Random.value);
            }
            else
            {
                IReadOnlyList<AmbientRouteNode> nodes = routeGraph.Nodes;

                if (nodes.Count == 0)
                {
                    pose = default;
                    return false;
                }

                AmbientRouteNode node = nodes[Random.Range(0, nodes.Count)];

                if (node == null)
                {
                    pose = default;
                    return false;
                }

                position = node.Position;
            }

            Vector2 scatter = Random.insideUnitCircle * routeSpawnScatterRadius;
            position += new Vector3(scatter.x, 0f, scatter.y);

            pose = new Pose(
                position,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            return true;
        }

        private AmbientRouteSegment ChooseWeightedSegment()
        {
            float totalLength = 0f;

            foreach (AmbientRouteSegment segment in validSegments)
                totalLength += Mathf.Max(0f, segment.Length);

            if (totalLength <= 0f)
                return validSegments[Random.Range(0, validSegments.Count)];

            float remaining = Random.value * totalLength;

            foreach (AmbientRouteSegment segment in validSegments)
            {
                remaining -= Mathf.Max(0f, segment.Length);

                if (remaining <= 0f)
                    return segment;
            }

            return validSegments[validSegments.Count - 1];
        }

        private ActorDefinition GetRandomEnemyDefinition()
        {
            int startIndex = Random.Range(0, enemyPool.Length);

            for (int offset = 0; offset < enemyPool.Length; offset++)
            {
                int index = (startIndex + offset) % enemyPool.Length;

                if (enemyPool[index] != null)
                    return enemyPool[index];
            }

            return null;
        }

        private int CountLivingEnemies()
        {
            int living = 0;

            for (int index = spawnedEnemies.Count - 1; index >= 0; index--)
            {
                GameObject enemy = spawnedEnemies[index];

                if (enemy == null)
                {
                    spawnedEnemies.RemoveAt(index);
                    continue;
                }

                Health health = enemy.GetComponent<Health>();

                if (health == null || !health.IsDead)
                    living++;
            }

            return living;
        }

        private void ScheduleNextSpawn()
        {
            float minimum = Mathf.Min(spawnIntervalMin, spawnIntervalMax);
            float maximum = Mathf.Max(spawnIntervalMin, spawnIntervalMax);

            spawnTimer = Random.Range(minimum, maximum);
        }

        private void ClearActors()
        {
            if (showcaseCharacter != null)
            {
                showcaseCharacter.SetActive(false);
                Destroy(showcaseCharacter);
                showcaseCharacter = null;
            }

            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy == null)
                    continue;

                enemy.SetActive(false);
                Destroy(enemy);
            }

            spawnedEnemies.Clear();
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void OnValidate()
        {
            initialEnemyCount = Mathf.Max(1, initialEnemyCount);
            maxLivingEnemies = Mathf.Max(initialEnemyCount, maxLivingEnemies);

            spawnIntervalMin = Mathf.Max(0.1f, spawnIntervalMin);
            spawnIntervalMax = Mathf.Max(0.1f, spawnIntervalMax);

            routeSpawnScatterRadius = Mathf.Max(0f, routeSpawnScatterRadius);

            enemyEngagementRadius = Mathf.Max(0.1f, enemyEngagementRadius);
            enemyWanderRadius = Mathf.Max(0f, enemyWanderRadius);
            enemyWanderPauseMinSeconds = Mathf.Max(0f, enemyWanderPauseMinSeconds);
            enemyWanderPauseMaxSeconds = Mathf.Max(enemyWanderPauseMinSeconds, enemyWanderPauseMaxSeconds);

            routeArrivalDistance = Mathf.Max(0.1f, routeArrivalDistance);

            immortalityArmourBonus = Mathf.Max(0f, immortalityArmourBonus);
            cameraFollowSharpness = Mathf.Max(0f, cameraFollowSharpness);
        }
    }
}