using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private EncounterWorkshopBuildConfiguration testBuild = new();
        
        [Header("Encounter Authoring")]
        [SerializeField] private ActorDefinition[] authoringActorPalette = Array.Empty<ActorDefinition>();

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
        
        private readonly List<ActorDefinition> resolvedAuthoringActors = new();
        private readonly EncounterWorkshopAuthoringPanel authoringPanel = new();
        private readonly EncounterWorkshopNumericControls numericControls = new();

        private EncounterWorkshopDraft encounterDraft;
        private EncounterDefinition currentDefinition;

        private IEnumerator Start()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;

            if (encounterDirection == null)
                encounterDirection = EncounterDirectionController.Instance;

            testBuild.EnsureFor(partyMember);
            InitializeEncounterDraft();

            if (stageDirector != null)
                stageDirector.LevelFinished += HandleLevelFinished;

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

            encounterDraft?.ClearRuntimeOverrides();
            ClearSpawnGroupOverrides();
        }

        public bool StartScenario()
        {
            StopScenario(clearStatus: false);

            if (!ValidateConfiguration())
                return false;

            testBuild.EnsureFor(partyMember);
            
            EnsureEncounterDraft();
            encounterDraft.ApplyRuntimeOverrides();

            if (deterministicReplay)
                UnityEngine.Random.InitState(randomSeed);

            ConfigureEncounterStartMode();

            partySpawnPoint.position = CalculatePartyStartPosition();

            RunStartConfiguration configuration = new(new[] { partyMember });

            if (!RunProgress.BeginRun(configuration))
            {
                status = "Could not create Workshop RunState.";
                return false;
            }

            RunBuild build = RunProgress.CurrentRun.GetBuild(partyMember);
            testBuild.ApplyTo(partyMember, build);

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

            status = $"{arrivalScenario} / {encounterStartMode} / {testBuild.Preset}";
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
            LevelSpawnGroup[] groups = GetOwnedSpawnGroups();

            // Reset every group to an explicit Workshop baseline first.
            foreach (LevelSpawnGroup group in groups)
                group.SetRuntimeStartBehaviorOverride(false, false);

            if (encounterStartMode != EncounterStartMode.PreSpawnFromStart)
                return;

            if (encounter != null && encounter.PhaseCount > 0)
            {
                LevelEncounterPhase firstPhase = encounter.Phases[0];

                if (firstPhase == null)
                    return;

                foreach (LevelSpawnGroup group in firstPhase.SpawnGroups)
                    group?.SetRuntimeStartBehaviorOverride(true, false);

                return;
            }

            // Legacy/non-phased encounters retain the old Workshop behaviour.
            foreach (LevelSpawnGroup group in groups)
                group.SetRuntimeStartBehaviorOverride(true, false);
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
            if (encounter == null)
                return Array.Empty<LevelSpawnGroup>();

            LevelSpawnGroup[] groups =
                new LevelSpawnGroup[encounter.SpawnGroups.Count];

            for (int i = 0;
                i < encounter.SpawnGroups.Count;
                i++)
            {
                groups[i] = encounter.SpawnGroups[i];
            }

            return groups;
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

            status = won ? "Workshop level finished." : "Party defeated. Replay when ready.";
        }

        private void OnGUI()
        {
            DrawScenarioPanel();
            DrawBuildPanel();

            authoringPanel.Draw(
                encounterDraft,
                resolvedAuthoringActors,
                currentDefinition,
                () => StartScenario(),
                ResetEncounterDraftAndReplay,
                LoadEncounterDefinition,
                SaveEncounterDefinition,
                SaveEncounterDefinitionAs
            );
        }

        private void DrawScenarioPanel()
        {
            const float width = 460f;
            const float height = 300f;

            GUILayout.BeginArea(new Rect(10f, 10f, width, height), GUI.skin.box);

            GUILayout.Label("ENCOUNTER WORKSHOP");
            GUILayout.Label($"Encounter: {(encounter != null ? encounter.DisplayName : "Missing")}");
            GUILayout.Label($"Status: {status}");
            
            if (encounter != null && encounter.PhaseCount > 0)
            {
                LevelEncounterPhase currentPhase = encounter.CurrentPhase;
                string phaseName = currentPhase != null ? currentPhase.DisplayName : "Waiting";

                GUILayout.Label(
                    $"Phase: {encounter.CurrentPhaseNumber}/{encounter.PhaseCount}  {phaseName}"
                );
            }

            GUILayout.Space(6f);
            GUILayout.Label($"Arrival: {arrivalScenario}");
            
            float newApproachDistance =
                numericControls.DrawFloat(
                    "scenario.approachDistance",
                    "Start Distance",
                    approachDistance,
                    1f,
                    40f,
                    sliderStep: 0.25f,
                    labelWidth: 130f,
                    sliderWidth: 210f,
                    fieldWidth: 70f
                );

            if (!Mathf.Approximately(
                    newApproachDistance,
                    approachDistance))
            {
                approachDistance = newApproachDistance;
            }

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

        private void DrawBuildPanel()
        {
            if (testBuild == null)
                return;

            testBuild.EnsureFor(partyMember);

            const float width = 430f;
            const float height = 385f;

            GUILayout.BeginArea(
                new Rect(Screen.width - width - 10f, 10f, width, height),
                GUI.skin.box
            );

            GUILayout.Label("TEST BUILD");
            GUILayout.Label($"Preset: {testBuild.Preset}");

            GUILayout.BeginHorizontal();

            DrawPresetButton("EARLY", EncounterWorkshopPowerPreset.Early);
            DrawPresetButton("MID", EncounterWorkshopPowerPreset.Mid);
            DrawPresetButton("LATE", EncounterWorkshopPowerPreset.Late);
            DrawPresetButton("EXTREME", EncounterWorkshopPowerPreset.Extreme);

            GUILayout.EndHorizontal();

            GUILayout.Space(8f);

            int weaponLevel = numericControls.DrawInt(
                "build.weapon",
                partyMember != null ? partyMember.WeaponName : "Weapon",
                testBuild.WeaponLevel,
                0,
                testBuild.MaximumGearLevel,
                labelWidth: 175f,
                sliderWidth: 170f
            );

            if (weaponLevel != testBuild.WeaponLevel)
                testBuild.SetWeaponLevel(weaponLevel);

            int armourLevel = numericControls.DrawInt(
                "build.armour",
                partyMember != null ? partyMember.ArmourName : "Armour",
                testBuild.ArmourLevel,
                0,
                testBuild.MaximumGearLevel,
                labelWidth: 175f,
                sliderWidth: 170f
            );

            if (armourLevel != testBuild.ArmourLevel)
                testBuild.SetArmourLevel(armourLevel);

            int focusLevel = numericControls.DrawInt(
                "build.focus",
                "Focus",
                testBuild.FocusLevel,
                0,
                testBuild.MaximumGearLevel,
                labelWidth: 175f,
                sliderWidth: 170f
            );

            if (focusLevel != testBuild.FocusLevel)
                testBuild.SetFocusLevel(focusLevel);

            if (partyMember != null && partyMember.HealingPerFocusLevel <= 0f)
                GUILayout.Label("Focus currently has no stat effect for this character.");

            GUILayout.Space(8f);
            GUILayout.Label("Starting Abilities");

            for (int i = 0; i < testBuild.AbilityLevels.Count; i++)
            {
                EncounterWorkshopBuildConfiguration.AbilityLevelSetting ability = testBuild.AbilityLevels[i];

                if (ability?.Ability == null)
                    continue;

                int level = numericControls.DrawInt(
                    $"build.ability.{ability.Ability.Id}",
                    ability.Ability.DisplayName,
                    ability.Level,
                    1,
                    ability.Ability.MaxLevel,
                    labelWidth: 175f,
                    sliderWidth: 170f
                );

                if (level != ability.Level)
                    testBuild.SetAbilityLevel(i, level);
            }

            GUILayout.Space(8f);
            DrawEstimatedStats();

            GUILayout.Space(6f);
            GUILayout.Label("Changes apply when the scenario is replayed.");

            if (GUILayout.Button("APPLY BUILD & REPLAY"))
                StartScenario();

            GUILayout.EndArea();
        }

        private void DrawPresetButton(string label, EncounterWorkshopPowerPreset preset)
        {
            if (GUILayout.Button(label))
                testBuild.SelectPreset(preset, partyMember);
        }

        private void DrawEstimatedStats()
        {
            if (partyMember == null)
                return;

            float health = partyMember.BaseHealth +
                           testBuild.ArmourLevel * partyMember.HealthPerArmourLevel;

            float damage = partyMember.BaseDamage +
                           testBuild.WeaponLevel * partyMember.DamagePerWeaponLevel;

            float armour = partyMember.BaseArmour +
                           testBuild.ArmourLevel * partyMember.ArmourPerArmourLevel;

            float healing = partyMember.BaseHealing +
                            testBuild.FocusLevel * partyMember.HealingPerFocusLevel;

            string stats = healing > 0f
                ? $"Expected Spawn: HP {health:0}   DMG {damage:0.#}   ARM {armour:0.#}   HEAL {healing:0.#}"
                : $"Expected Spawn: HP {health:0}   DMG {damage:0.#}   ARM {armour:0.#}";

            GUILayout.Label(stats);
        }
        
        private void InitializeEncounterDraft()
        {
            currentDefinition = null;

            encounterDraft = new EncounterWorkshopDraft();
            encounterDraft.CaptureFrom(encounter);

            RefreshAuthoringActorPalette();
        }

        private void EnsureEncounterDraft()
        {
            if (encounterDraft != null && encounterDraft.IsInitialized)
                return;

            InitializeEncounterDraft();
        }

        private void ResetEncounterDraftAndReplay()
        {
            StopScenario(clearStatus: false);

            encounterDraft?.ClearRuntimeOverrides();

            EncounterWorkshopDraft replacement = new();

            if (currentDefinition != null)
            {
                if (!replacement.TryCaptureFromDefinition(
                        encounter,
                        currentDefinition,
                        out string error))
                {
                    Debug.LogError(
                        $"Could not reset Workshop Draft from '{currentDefinition.name}': {error}",
                        this
                    );

                    status = "Definition is no longer compatible with the Workshop scene.";
                    return;
                }
            }
            else
            {
                replacement.CaptureFrom(encounter);
            }

            encounterDraft = replacement;

            RefreshAuthoringActorPalette();
            StartScenario();
        }
        
        private void SaveEncounterDefinition()
        {
            EnsureEncounterDraft();

            if (currentDefinition == null)
            {
                SaveEncounterDefinitionAs();
                return;
            }

            if (!EncounterWorkshopAssetPersistence.Save(
                    encounterDraft,
                    currentDefinition))
            {
                status = "Encounter Definition save failed or was cancelled.";
                return;
            }

            status = $"Saved '{currentDefinition.name}'.";
        }

        private void SaveEncounterDefinitionAs()
        {
            EnsureEncounterDraft();

            EncounterDefinition saved =
                EncounterWorkshopAssetPersistence.SaveAs(encounterDraft);

            if (saved == null)
            {
                status = "Save As cancelled.";
                return;
            }

            currentDefinition = saved;
            status = $"Saved '{currentDefinition.name}'.";
        }

        private void LoadEncounterDefinition()
        {
            EncounterDefinition definition =
                EncounterWorkshopAssetPersistence.Load();

            if (definition == null)
                return;

            EncounterWorkshopDraft candidate = new();

            if (!candidate.TryCaptureFromDefinition(
                    encounter,
                    definition,
                    out string error))
            {
                Debug.LogError(
                    $"Encounter Definition '{definition.name}' is not compatible with " +
                    $"the Workshop scene: {error}",
                    this
                );

                status = "Selected Definition is not compatible with this Workshop structure.";
                return;
            }

            StopScenario(clearStatus: false);

            encounterDraft?.ClearRuntimeOverrides();

            encounterDraft = candidate;
            currentDefinition = definition;

            RefreshAuthoringActorPalette();

            status = $"Loaded '{currentDefinition.name}'.";
            StartScenario();
        }

        private void RefreshAuthoringActorPalette()
        {
            resolvedAuthoringActors.Clear();

            encounterDraft?.CollectActors(resolvedAuthoringActors);

            foreach (ActorDefinition actor in authoringActorPalette)
                AddAuthoringActor(actor);

            AddAuthoringActor(pursuerDefinition);
        }

        private void AddAuthoringActor(ActorDefinition actor)
        {
            if (actor != null && !resolvedAuthoringActors.Contains(actor))
                resolvedAuthoringActors.Add(actor);
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