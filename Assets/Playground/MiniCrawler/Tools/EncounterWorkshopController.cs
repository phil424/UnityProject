using System.Collections;
using MiniCrawler.Combat;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Progress;
using MiniCrawler.Spawning;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Tools
{
    [DisallowMultipleComponent]
    public sealed class EncounterWorkshopController : MonoBehaviour
    {
        public enum ArrivalScenario
        {
            Approach,
            Threshold,
            OnTop,
            ArriveWhilePursued
        }

        public enum EncounterStartMode
        {
            SpawnOnArrival,
            PreSpawnFromStart
        }

        [Header("Workshop Runtime")]
        [SerializeField] private StageDirector stageDirector;
        [SerializeField] private EncounterDirectionController encounterDirection;
        [SerializeField] private LevelEncounter encounter;
        [SerializeField] private Transform partySpawnPoint;

        [Header("Test Party")]
        [SerializeField] private PartyMemberDefinition partyMember;

        [Header("Scenario")]
        [SerializeField] private ArrivalScenario arrivalScenario = ArrivalScenario.Approach;
        [SerializeField] private EncounterStartMode encounterStartMode = EncounterStartMode.SpawnOnArrival;
        [SerializeField] private Vector3 approachDirection = Vector3.forward;
        [SerializeField, Min(1f)] private float approachDistance = 15f;
        [SerializeField, Min(0.1f)] private float thresholdOffset = 0.5f;

        [Header("Pursued Scenario")]
        [SerializeField] private ActorDefinition pursuerDefinition;
        [SerializeField, Min(1)] private int pursuerCount = 4;
        [SerializeField, Min(0.5f)] private float pursuerDistance = 6f;
        [SerializeField, Min(0f)] private float pursuerSpacing = 1.5f;

        [Header("Replay")]
        [SerializeField] private bool autoStartOnPlay = true;
        [SerializeField] private bool deterministicReplay = true;
        [SerializeField] private int randomSeed = 12345;

        private bool scenarioRunning;
        private bool encounterCompletionReported;
        private string status = "Waiting to start.";

        private IEnumerator Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (encounterDirection == null)
                encounterDirection = EncounterDirectionController.Instance;

            if (stageDirector != null)
                stageDirector.LevelFinished += HandleLevelFinished;

            // Let camera/HUD/runtime listeners finish their own Start methods
            // before the Workshop spawns the first party.
            yield return null;

            if (autoStartOnPlay)
                StartScenario();
        }

        private void Update()
        {
            if (!scenarioRunning || encounter == null || encounterCompletionReported)
                return;

            if (!encounter.IsCompleted)
                return;

            encounterCompletionReported = true;
            status = "Encounter complete. Replay when ready.";
        }

        private void OnDestroy()
        {
            if (stageDirector != null)
                stageDirector.LevelFinished -= HandleLevelFinished;

            ClearSpawnGroupOverrides();
        }

        public bool StartScenario()
        {
            StopScenario(clearStatus: false);

            if (!ValidateConfiguration())
                return false;

            if (deterministicReplay)
                Random.InitState(randomSeed);

            ConfigureEncounterStartMode();

            partySpawnPoint.position = CalculatePartyStartPosition();

            RunStartConfiguration configuration = new(new[] { partyMember });

            if (!RunProgress.BeginRun(configuration))
            {
                status = "Could not create Workshop RunState.";
                return false;
            }

            if (!stageDirector.StartLevel(RunProgress.CurrentRun))
            {
                RunProgress.EndRun();

                status = "StageDirector could not start Workshop scenario.";
                return false;
            }

            if (arrivalScenario == ArrivalScenario.ArriveWhilePursued)
                SpawnPursuers();

            if (!encounterDirection.SelectEncounter(encounter))
            {
                stageDirector.ClearLevel();
                RunProgress.EndRun();

                status = "Workshop encounter could not be selected.";
                return false;
            }

            scenarioRunning = true;
            encounterCompletionReported = false;

            status = $"{arrivalScenario} / {encounterStartMode}";

            return true;
        }

        public void Replay()
        {
            StartScenario();
        }

        public void StopScenario(bool clearStatus = true)
        {
            encounterDirection?.ClearSelection();

            if (stageDirector != null && stageDirector.State != StageDirector.LevelState.Idle)
                stageDirector.ClearLevel();

            if (RunProgress.HasActiveRun)
                RunProgress.EndRun();

            scenarioRunning = false;
            encounterCompletionReported = false;

            if (clearStatus)
                status = "Stopped.";
        }

        private bool ValidateConfiguration()
        {
            if (stageDirector == null ||
                encounterDirection == null ||
                encounter == null ||
                partySpawnPoint == null ||
                partyMember == null)
            {
                status = "Workshop configuration is incomplete.";
                Debug.LogError(status, this);
                return false;
            }

            if (arrivalScenario == ArrivalScenario.ArriveWhilePursued && pursuerDefinition == null)
            {
                status = "Arrive While Pursued requires a Pursuer Definition.";
                Debug.LogError(status, this);
                return false;
            }

            return true;
        }

        private void ConfigureEncounterStartMode()
        {
            bool startSpawning = encounterStartMode == EncounterStartMode.PreSpawnFromStart;

            foreach (LevelSpawnGroup group in GetOwnedSpawnGroups())
                group.SetRuntimeStartBehaviorOverride(startSpawning, shouldStartCombatActive: false);
        }

        private void ClearSpawnGroupOverrides()
        {
            if (encounter == null)
                return;

            foreach (LevelSpawnGroup group in GetOwnedSpawnGroups())
                group.ClearRuntimeStartBehaviorOverride();
        }

        private LevelSpawnGroup[] GetOwnedSpawnGroups()
        {
            LevelSpawnGroup[] candidates = encounter.GetComponentsInChildren<LevelSpawnGroup>(true);
            System.Collections.Generic.List<LevelSpawnGroup> owned = new();

            foreach (LevelSpawnGroup group in candidates)
            {
                if (group == null)
                    continue;

                if (group.GetComponentInParent<LevelEncounter>() == encounter)
                    owned.Add(group);
            }

            return owned.ToArray();
        }

        private Vector3 CalculatePartyStartPosition()
        {
            Vector3 direction = GetApproachDirection();
            Vector3 anchor = encounter.AnchorPosition;

            float distance = arrivalScenario switch
            {
                ArrivalScenario.Approach => approachDistance,
                ArrivalScenario.Threshold => encounterDirection.ArrivalDistance + thresholdOffset,
                ArrivalScenario.OnTop => 0f,
                ArrivalScenario.ArriveWhilePursued => approachDistance,
                _ => approachDistance
            };

            Vector3 position = anchor - direction * distance;
            position.y = partySpawnPoint.position.y;

            return position;
        }

        private Vector3 GetApproachDirection()
        {
            Vector3 direction = approachDirection;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Vector3.forward;

            return direction.normalized;
        }

        private void SpawnPursuers()
        {
            Vector3 travelDirection = encounter.AnchorPosition - partySpawnPoint.position;
            travelDirection.y = 0f;

            if (travelDirection.sqrMagnitude <= 0.001f)
                travelDirection = GetApproachDirection();

            travelDirection.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, travelDirection).normalized;
            Vector3 centre = partySpawnPoint.position - travelDirection * pursuerDistance;

            for (int i = 0; i < pursuerCount; i++)
            {
                float centredIndex = i - (pursuerCount - 1) * 0.5f;
                Vector3 position = centre + right * centredIndex * pursuerSpacing;

                GameObject pursuer = stageDirector.SpawnAmbientActor(
                    pursuerDefinition,
                    new Pose(position, Quaternion.LookRotation(travelDirection)),
                    engagementRadius: 0.1f
                );

                if (pursuer == null)
                    continue;

                CombatEngagementState engagement = pursuer.GetComponent<CombatEngagementState>();

                if (engagement == null)
                    engagement = pursuer.AddComponent<CombatEngagementState>();

                engagement.SetEngaged(true);
            }
        }

        private void HandleLevelFinished(bool won)
        {
            scenarioRunning = false;
            encounterCompletionReported = false;

            if (RunProgress.HasActiveRun)
                RunProgress.EndRun();

            status = won
                ? "Workshop level finished."
                : "Party defeated. Replay when ready.";
        }

        private void OnGUI()
        {
            const float width = 460f;
            const float height = 255f;

            GUILayout.BeginArea(new Rect(10f, 10f, width, height), GUI.skin.box);

            GUILayout.Label("ENCOUNTER WORKSHOP");
            GUILayout.Label($"Encounter: {(encounter != null ? encounter.DisplayName : "Missing")}");
            GUILayout.Label($"Status: {status}");

            GUILayout.Space(6f);
            GUILayout.Label($"Arrival: {arrivalScenario}");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Approach"))
                SelectArrivalScenario(ArrivalScenario.Approach);

            if (GUILayout.Button("Threshold"))
                SelectArrivalScenario(ArrivalScenario.Threshold);

            if (GUILayout.Button("On Top"))
                SelectArrivalScenario(ArrivalScenario.OnTop);

            if (GUILayout.Button("Pursued"))
                SelectArrivalScenario(ArrivalScenario.ArriveWhilePursued);

            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label($"Encounter Start: {encounterStartMode}");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Spawn On Arrival"))
                SelectStartMode(EncounterStartMode.SpawnOnArrival);

            if (GUILayout.Button("Pre-Spawn From Start"))
                SelectStartMode(EncounterStartMode.PreSpawnFromStart);

            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            if (GUILayout.Button(scenarioRunning ? "REPLAY CURRENT SCENARIO" : "START SCENARIO"))
                StartScenario();

            if (scenarioRunning && GUILayout.Button("STOP"))
                StopScenario();

            GUILayout.EndArea();
        }

        private void SelectArrivalScenario(ArrivalScenario scenario)
        {
            if (arrivalScenario == scenario)
                return;

            arrivalScenario = scenario;
            StartScenario();
        }

        private void SelectStartMode(EncounterStartMode mode)
        {
            if (encounterStartMode == mode)
                return;

            encounterStartMode = mode;
            StartScenario();
        }

        private void OnValidate()
        {
            approachDirection.y = 0f;

            if (approachDirection.sqrMagnitude <= 0.001f)
                approachDirection = Vector3.forward;

            approachDistance = Mathf.Max(1f, approachDistance);
            thresholdOffset = Mathf.Max(0.1f, thresholdOffset);
            pursuerCount = Mathf.Max(1, pursuerCount);
            pursuerDistance = Mathf.Max(0.5f, pursuerDistance);
            pursuerSpacing = Mathf.Max(0f, pursuerSpacing);
        }
    }
}