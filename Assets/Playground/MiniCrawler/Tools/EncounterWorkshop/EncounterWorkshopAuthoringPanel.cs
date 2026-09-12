using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Encounters;
using MiniCrawler.Spawning;
using UnityEngine;

namespace MiniCrawler.Tools
{
    public sealed class EncounterWorkshopAuthoringPanel
    {
        private readonly string[] spawnShapeNames = { "Point", "Circle", "Box" };
        private readonly string[] triggerNames =
        {
            "Party Proximity",
            "Encounter Started",
            "Delay After Encounter Start",
            "Phase Started",
            "Delay After Phase Start",
            "Phase Cleared"
        };

        private readonly string[] actionNames =
        {
            "Activate Encounter Combat",
            "Begin Encounter Spawning",
            "Start Phase",
            "Begin Spawn Group",
            "Activate Spawn Group",
            "Complete Encounter",
            "Raise Signal"
        };
        private readonly EncounterWorkshopNumericControls numericControls = new();

        private Vector2 scrollPosition;

        public void Draw(
            EncounterWorkshopDraft draft,
            IReadOnlyList<ActorDefinition> actorPalette,
            EncounterDefinition currentDefinition,
            Action applyAndReplay,
            Action resetAndReplay,
            Action loadDefinition,
            Action saveDefinition,
            Action saveDefinitionAs)
        {
            if (draft == null || !draft.IsInitialized)
                return;

            const float width = 560f;

            float height = Mathf.Max(320f, Screen.height - 470f);

            GUILayout.BeginArea(
                new Rect(10f, 320f, width, height),
                GUI.skin.box
            );

            GUILayout.Label("ENCOUNTER DRAFT");

            string sourceState;

            if (draft.HasUnsavedChanges)
            {
                sourceState = "MODIFIED — UNSAVED";
            }
            else if (currentDefinition != null)
            {
                sourceState = "SAVED ASSET";
            }
            else
            {
                sourceState = "SCENE COPY";
            }

            string previewState = draft.HasUnappliedChanges
                ? "REPLAY REQUIRED"
                : "CURRENT";

            GUILayout.Label($"Draft: {sourceState}");
            GUILayout.Label($"Preview: {previewState}");
            GUILayout.Label(
                currentDefinition != null
                    ? $"Asset: {currentDefinition.name}"
                    : "Asset: None — Scene Source"
            );

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("LOAD..."))
                loadDefinition?.Invoke();

            bool previousEnabled = GUI.enabled;

            GUI.enabled =
                previousEnabled &&
                currentDefinition != null &&
                EncounterWorkshopAssetPersistence.IsAvailable;

            if (GUILayout.Button("SAVE"))
                saveDefinition?.Invoke();

            GUI.enabled =
                previousEnabled &&
                EncounterWorkshopAssetPersistence.IsAvailable;

            if (GUILayout.Button("SAVE AS..."))
                saveDefinitionAs?.Invoke();

            GUI.enabled = previousEnabled;

            GUILayout.EndHorizontal();
            GUILayout.Label("Draft changes apply on Replay. SAVE / SAVE AS persists the current Draft.");

            GUILayout.Space(6f);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            DrawIdentity(draft);
            
            GUILayout.Space(6f);
            GUILayout.Label("STRUCTURE");

            if (GUILayout.Button(
                    "NEW BASIC ENCOUNTER — DISCARD CURRENT DRAFT"))
            {
                ActorDefinition defaultActor =
                    actorPalette != null && actorPalette.Count > 0
                        ? actorPalette[0]
                        : null;

                draft.CreateBasic(
                    draft.SourceEncounter,
                    defaultActor
                );
            }

            if (GUILayout.Button("+ ADD PHASE"))
            {
                ActorDefinition defaultActor =
                    actorPalette != null && actorPalette.Count > 0
                        ? actorPalette[0]
                        : null;

                draft.AddPhase(defaultActor);
            }

            GUILayout.Space(8f);

            for (int i = 0; i < draft.Phases.Count; i++)
                DrawPhase(draft, draft.Phases[i], i, draft.Phases.Count, actorPalette);

            if (draft.Phases.Count == 0)
            {
                GUILayout.Label("UNPHASED ENCOUNTER");

                for (int i = 0; i < draft.UnphasedGroups.Count; i++)
                    DrawGroup(
                        draft,
                        draft.UnphasedGroups[i],
                        i,
                        actorPalette,
                        $"unphased.group.{i}"
                    );
            }
            
            GUILayout.Space(10f);
            DrawRules(draft);

            GUILayout.EndScrollView();

            GUILayout.Space(6f);

            if (GUILayout.Button("APPLY DRAFT & REPLAY"))
                applyAndReplay?.Invoke();

            if (GUILayout.Button("RESET FROM SOURCE & REPLAY"))
                resetAndReplay?.Invoke();

            GUILayout.EndArea();
        }

        private static void DrawIdentity(EncounterWorkshopDraft draft)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("IDENTITY");

            GUILayout.Label("Encounter Name");

            string newDisplayName = GUILayout.TextField(draft.DisplayName ?? string.Empty);

            if (newDisplayName != draft.DisplayName)
            {
                draft.DisplayName = newDisplayName;
                draft.MarkModified();
            }

            GUILayout.Label("Description");

            string newDescription = GUILayout.TextArea(
                draft.Description ?? string.Empty,
                GUILayout.Height(48f)
            );

            if (newDescription != draft.Description)
            {
                draft.Description = newDescription;
                draft.MarkModified();
            }

            GUILayout.EndVertical();
        }

        private void DrawPhase(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.PhaseDraft phase,
            int phaseIndex,
            int phaseCount,
            IReadOnlyList<ActorDefinition> actorPalette)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label($"PHASE {phaseIndex + 1}");
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("DUPLICATE PHASE"))
            {
                draft.DuplicatePhase(phaseIndex);

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }

            GUI.enabled = draft.Phases.Count > 1;

            if (GUILayout.Button("REMOVE PHASE"))
            {
                draft.RemovePhase(phaseIndex);

                GUI.enabled = true;

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            string newName = GUILayout.TextField(phase.DisplayName ?? string.Empty);

            if (newName != phase.DisplayName)
            {
                phase.DisplayName = newName;
                draft.MarkModified();
            }

            if (phaseIndex < phaseCount - 1)
            {
                float newDelay = numericControls.DrawFloat(
                    $"phase.{phaseIndex}.advance",
                    "Advance After",
                    phase.AdvanceAfterSeconds,
                    0f,
                    30f,
                    sliderStep: 0.25f
                );

                if (!Mathf.Approximately(newDelay, phase.AdvanceAfterSeconds))
                {
                    phase.AdvanceAfterSeconds = newDelay;
                    draft.MarkModified();
                }

                bool advanceWhenCleared = GUILayout.Toggle(
                    phase.AdvanceWhenCleared,
                    "Advance When Cleared"
                );

                if (advanceWhenCleared != phase.AdvanceWhenCleared)
                {
                    phase.AdvanceWhenCleared = advanceWhenCleared;
                    draft.MarkModified();
                }
            }
            else
            {
                GUILayout.Label("Final phase — no successor transition.");
            }

            GUILayout.Space(4f);

            for (int i = 0; i < phase.Groups.Count; i++)
                DrawGroup(
                    draft,
                    phase.Groups[i],
                    i,
                    actorPalette,
                    $"phase.{phaseIndex}.group.{i}"
                );

            GUILayout.EndVertical();
        }

        private void DrawGroup(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnGroupDraft group,
            int groupIndex,
            IReadOnlyList<ActorDefinition> actorPalette,
            string path)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"SPAWN GROUP {groupIndex + 1} — {group.Name}");

            for (int i = 0; i < group.Entries.Count; i++)
                DrawEntry(
                    draft,
                    group.Entries[i],
                    i,
                    actorPalette,
                    $"{path}.entry.{i}"
                );

            for (int i = 0; i < group.Sources.Count; i++)
                DrawSource(
                    draft,
                    group.Sources[i],
                    i,
                    $"{path}.source.{i}"
                );

            GUILayout.EndVertical();
        }

        private void DrawEntry(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnEntryDraft entry,
            int entryIndex,
            IReadOnlyList<ActorDefinition> actorPalette,
            string path)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"ENTRY {entryIndex + 1}");

            DrawActorSelector(draft, entry, actorPalette);

            int newCount = numericControls.DrawInt(
                $"{path}.count",
                "Count",
                entry.Count,
                1,
                50
            );

            if (newCount != entry.Count)
            {
                entry.Count = newCount;
                entry.BatchSize = Mathf.Clamp(entry.BatchSize, 1, entry.Count);
                draft.MarkModified();
            }

            float newStartDelay = numericControls.DrawFloat(
                $"{path}.startDelay",
                "Start Delay",
                entry.StartDelay,
                0f,
                20f,
                sliderStep: 0.25f
            );

            if (!Mathf.Approximately(newStartDelay, entry.StartDelay))
            {
                entry.StartDelay = newStartDelay;
                draft.MarkModified();
            }

            int newBatchSize = numericControls.DrawInt(
                $"{path}.batchSize",
                "Batch Size",
                entry.BatchSize,
                1,
                Mathf.Max(1, entry.Count)
            );

            if (newBatchSize != entry.BatchSize)
            {
                entry.BatchSize = newBatchSize;
                draft.MarkModified();
            }

            float newSpawnSpacing = numericControls.DrawFloat(
                $"{path}.spawnSpacing",
                "Spawn Spacing",
                entry.TimeBetweenSpawns,
                0f,
                5f,
                sliderStep: 0.05f
            );

            if (!Mathf.Approximately(newSpawnSpacing, entry.TimeBetweenSpawns))
            {
                entry.TimeBetweenSpawns = newSpawnSpacing;
                draft.MarkModified();
            }

            float newBatchSpacing = numericControls.DrawFloat(
                $"{path}.batchSpacing",
                "Batch Spacing",
                entry.TimeBetweenBatches,
                0f,
                20f,
                sliderStep: 0.25f
            );

            if (!Mathf.Approximately(newBatchSpacing, entry.TimeBetweenBatches))
            {
                entry.TimeBetweenBatches = newBatchSpacing;
                draft.MarkModified();
            }

            GUILayout.EndVertical();
        }
        
        private void DrawRules(EncounterWorkshopDraft draft)
        {
            GUILayout.Label("ENCOUNTER RULES");

            for (int i = 0; i < draft.Rules.Count; i++)
            {
                if (!DrawRule(draft, draft.Rules[i], i))
                    continue;

                draft.Rules.RemoveAt(i);
                draft.MarkModified();
                i--;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+ ADD APPROACH START RULE"))
            {
                draft.Rules.Add(
                    EncounterWorkshopDraft.RuleDraft.CreateApproachStartRule()
                );

                draft.MarkModified();
            }

            if (GUILayout.Button("+ ADD EMPTY RULE"))
            {
                draft.Rules.Add(new EncounterWorkshopDraft.RuleDraft());
                draft.MarkModified();
            }

            GUILayout.EndHorizontal();
        }

        private bool DrawRule(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.RuleDraft rule,
            int ruleIndex)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();

            GUILayout.Label($"RULE {ruleIndex + 1}");

            if (GUILayout.Button("REMOVE RULE", GUILayout.Width(120f)))
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return true;
            }

            GUILayout.EndHorizontal();

            string newName =
                GUILayout.TextField(rule.DisplayName ?? string.Empty);

            if (newName != rule.DisplayName)
            {
                rule.DisplayName = newName;
                draft.MarkModified();
            }

            bool enabled =
                GUILayout.Toggle(rule.Enabled, "Enabled");

            if (enabled != rule.Enabled)
            {
                rule.Enabled = enabled;
                draft.MarkModified();
            }

            GUILayout.Space(4f);
            GUILayout.Label("WHEN");

            int triggerIndex = GUILayout.SelectionGrid(
                (int)rule.TriggerKind,
                triggerNames,
                2
            );

            EncounterTriggerKind triggerKind =
                (EncounterTriggerKind)triggerIndex;

            if (triggerKind != rule.TriggerKind)
            {
                rule.TriggerKind = triggerKind;
                draft.MarkModified();
            }

            DrawTriggerSettings(draft, rule, ruleIndex);

            GUILayout.Space(6f);
            GUILayout.Label("DO");

            for (int actionIndex = 0;
                actionIndex < rule.Actions.Count;
                actionIndex++)
            {
                if (!DrawRuleAction(
                        draft,
                        rule.Actions[actionIndex],
                        ruleIndex,
                        actionIndex))
                {
                    continue;
                }

                rule.Actions.RemoveAt(actionIndex);
                draft.MarkModified();
                actionIndex--;
            }

            if (GUILayout.Button("+ ADD ACTION"))
            {
                rule.Actions.Add(
                    new EncounterWorkshopDraft.RuleActionDraft()
                );

                draft.MarkModified();
            }

            GUILayout.EndVertical();
            return false;
        }
        
        private void DrawTriggerSettings(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.RuleDraft rule,
            int ruleIndex)
        {
            string path = $"rule.{ruleIndex}.trigger";

            switch (rule.TriggerKind)
            {
                case EncounterTriggerKind.PartyProximity:
                {
                    bool requireAvailable = GUILayout.Toggle(
                        rule.RequireEncounterAvailable,
                        "Require Encounter Available"
                    );

                    if (requireAvailable != rule.RequireEncounterAvailable)
                    {
                        rule.RequireEncounterAvailable = requireAvailable;
                        draft.MarkModified();
                    }

                    float radius = numericControls.DrawFloat(
                        $"{path}.radius",
                        "Radius",
                        rule.ProximityRadius,
                        0.5f,
                        30f,
                        sliderStep: 0.25f
                    );

                    if (!Mathf.Approximately(radius, rule.ProximityRadius))
                    {
                        rule.ProximityRadius = radius;
                        draft.MarkModified();
                    }

                    float offsetX = numericControls.DrawFloatField(
                        $"{path}.offsetX",
                        "Offset X",
                        rule.LocalOffset.x,
                        -30f,
                        30f
                    );

                    float offsetZ = numericControls.DrawFloatField(
                        $"{path}.offsetZ",
                        "Offset Z",
                        rule.LocalOffset.z,
                        -30f,
                        30f
                    );

                    Vector3 offset = new(
                        offsetX,
                        rule.LocalOffset.y,
                        offsetZ
                    );

                    if (offset != rule.LocalOffset)
                    {
                        rule.LocalOffset = offset;
                        draft.MarkModified();
                    }

                    break;
                }

                case EncounterTriggerKind.DelayAfterEncounterStarted:
                {
                    float delay = numericControls.DrawFloat(
                        $"{path}.delay",
                        "Delay",
                        rule.DelaySeconds,
                        0f,
                        60f,
                        sliderStep: 0.25f
                    );

                    if (!Mathf.Approximately(delay, rule.DelaySeconds))
                    {
                        rule.DelaySeconds = delay;
                        draft.MarkModified();
                    }

                    break;
                }

                case EncounterTriggerKind.PhaseStarted:
                case EncounterTriggerKind.PhaseCleared:
                {
                    DrawRulePhaseIndex(draft, rule, path);
                    break;
                }

                case EncounterTriggerKind.DelayAfterPhaseStarted:
                {
                    DrawRulePhaseIndex(draft, rule, path);

                    float delay = numericControls.DrawFloat(
                        $"{path}.delay",
                        "Delay",
                        rule.DelaySeconds,
                        0f,
                        60f,
                        sliderStep: 0.25f
                    );

                    if (!Mathf.Approximately(delay, rule.DelaySeconds))
                    {
                        rule.DelaySeconds = delay;
                        draft.MarkModified();
                    }

                    break;
                }
            }
        }

        private void DrawRulePhaseIndex(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.RuleDraft rule,
            string path)
        {
            int phaseCount = Mathf.Max(1, draft.Phases.Count);

            int displayedPhase = numericControls.DrawInt(
                $"{path}.phase",
                "Phase",
                Mathf.Clamp(rule.PhaseIndex + 1, 1, phaseCount),
                1,
                phaseCount
            );

            int phaseIndex = displayedPhase - 1;

            if (phaseIndex != rule.PhaseIndex)
            {
                rule.PhaseIndex = phaseIndex;
                draft.MarkModified();
            }
        }
        
        private bool DrawRuleAction(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.RuleActionDraft action,
            int ruleIndex,
            int actionIndex)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();

            GUILayout.Label($"ACTION {actionIndex + 1}");

            if (GUILayout.Button("REMOVE", GUILayout.Width(80f)))
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return true;
            }

            GUILayout.EndHorizontal();

            int kindIndex = GUILayout.SelectionGrid(
                (int)action.Kind,
                actionNames,
                2
            );

            EncounterActionKind kind =
                (EncounterActionKind)kindIndex;

            if (kind != action.Kind)
            {
                action.Kind = kind;
                draft.MarkModified();
            }

            string path =
                $"rule.{ruleIndex}.action.{actionIndex}";

            switch (action.Kind)
            {
                case EncounterActionKind.StartPhase:
                {
                    int phaseCount = Mathf.Max(1, draft.Phases.Count);

                    int displayedPhase = numericControls.DrawInt(
                        $"{path}.phase",
                        "Target Phase",
                        Mathf.Clamp(action.PhaseIndex + 1, 1, phaseCount),
                        1,
                        phaseCount
                    );

                    int phaseIndex = displayedPhase - 1;

                    if (phaseIndex != action.PhaseIndex)
                    {
                        action.PhaseIndex = phaseIndex;
                        draft.MarkModified();
                    }

                    break;
                }

                case EncounterActionKind.BeginSpawnGroup:
                case EncounterActionKind.ActivateSpawnGroup:
                {
                    DrawActionGroupTarget(
                        draft,
                        action,
                        path
                    );

                    break;
                }

                case EncounterActionKind.RaiseSignal:
                {
                    GUILayout.Label("Signal ID");

                    string signal =
                        GUILayout.TextField(action.SignalId ?? string.Empty);

                    if (signal != action.SignalId)
                    {
                        action.SignalId = signal;
                        draft.MarkModified();
                    }

                    break;
                }
            }

            GUILayout.EndVertical();

            return false;
        }

        private void DrawActionGroupTarget(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.RuleActionDraft action,
            string path)
        {
            if (draft.Phases.Count > 0)
            {
                int phaseCount = draft.Phases.Count;

                int displayedPhase = numericControls.DrawInt(
                    $"{path}.phase",
                    "Target Phase",
                    Mathf.Clamp(action.PhaseIndex + 1, 1, phaseCount),
                    1,
                    phaseCount
                );

                int phaseIndex = displayedPhase - 1;

                if (phaseIndex != action.PhaseIndex)
                {
                    action.PhaseIndex = phaseIndex;
                    action.GroupIndex = 0;
                    draft.MarkModified();
                }

                int groupCount = Mathf.Max(
                    1,
                    draft.Phases[action.PhaseIndex].Groups.Count
                );

                int displayedGroup = numericControls.DrawInt(
                    $"{path}.group",
                    "Target Group",
                    Mathf.Clamp(action.GroupIndex + 1, 1, groupCount),
                    1,
                    groupCount
                );

                int groupIndex = displayedGroup - 1;

                if (groupIndex != action.GroupIndex)
                {
                    action.GroupIndex = groupIndex;
                    draft.MarkModified();
                }

                return;
            }

            int unphasedGroupCount =
                Mathf.Max(1, draft.UnphasedGroups.Count);

            int displayedUnphasedGroup = numericControls.DrawInt(
                $"{path}.group",
                "Target Group",
                Mathf.Clamp(action.GroupIndex + 1, 1, unphasedGroupCount),
                1,
                unphasedGroupCount
            );

            int unphasedGroupIndex = displayedUnphasedGroup - 1;

            if (unphasedGroupIndex != action.GroupIndex ||
                action.PhaseIndex != 0)
            {
                action.PhaseIndex = 0;
                action.GroupIndex = unphasedGroupIndex;
                draft.MarkModified();
            }
        }

        private void DrawActorSelector(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnEntryDraft entry,
            IReadOnlyList<ActorDefinition> actorPalette)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Enemy", GUILayout.Width(145f));

            if (GUILayout.Button("<", GUILayout.Width(30f)))
            {
                ActorDefinition actor = CycleActor(entry.Actor, actorPalette, -1);

                if (actor != entry.Actor)
                {
                    entry.Actor = actor;
                    draft.MarkModified();
                }
            }

            GUILayout.Label(GetActorLabel(entry.Actor), GUILayout.Width(265f));

            if (GUILayout.Button(">", GUILayout.Width(30f)))
            {
                ActorDefinition actor = CycleActor(entry.Actor, actorPalette, 1);

                if (actor != entry.Actor)
                {
                    entry.Actor = actor;
                    draft.MarkModified();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawSource(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnSourceDraft source,
            int sourceIndex,
            string path)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"SPAWN REGION {sourceIndex + 1} — {source.Name}");

            int newShapeIndex = GUILayout.Toolbar(
                (int)source.Shape,
                spawnShapeNames
            );

            LevelSpawnSource.SpawnShape newShape =
                (LevelSpawnSource.SpawnShape)newShapeIndex;

            if (newShape != source.Shape)
            {
                source.Shape = newShape;
                draft.MarkModified();
            }

            switch (source.Shape)
            {
                case LevelSpawnSource.SpawnShape.Circle:
                {
                    float newRadius = numericControls.DrawFloat(
                        $"{path}.radius",
                        "Radius",
                        source.Radius,
                        0f,
                        15f,
                        sliderStep: 0.25f
                    );

                    if (!Mathf.Approximately(newRadius, source.Radius))
                    {
                        source.Radius = newRadius;
                        draft.MarkModified();
                    }

                    break;
                }

                case LevelSpawnSource.SpawnShape.Box:
                {
                    float width = numericControls.DrawFloat(
                        $"{path}.width",
                        "Width",
                        source.BoxSize.x,
                        0.5f,
                        20f,
                        sliderStep: 0.25f
                    );

                    float depth = numericControls.DrawFloat(
                        $"{path}.depth",
                        "Depth",
                        source.BoxSize.y,
                        0.5f,
                        20f,
                        sliderStep: 0.25f
                    );

                    Vector2 newSize = new(width, depth);

                    if (newSize != source.BoxSize)
                    {
                        source.BoxSize = newSize;
                        draft.MarkModified();
                    }

                    break;
                }
            }

            GUILayout.EndVertical();
        }

        private static ActorDefinition CycleActor(
            ActorDefinition current,
            IReadOnlyList<ActorDefinition> palette,
            int direction)
        {
            if (palette == null || palette.Count == 0)
                return current;

            int currentIndex = -1;

            for (int i = 0; i < palette.Count; i++)
            {
                if (palette[i] == current)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex < 0)
                currentIndex = 0;

            int newIndex = (currentIndex + direction + palette.Count) % palette.Count;

            return palette[newIndex];
        }

        private static string GetActorLabel(ActorDefinition actor)
        {
            if (actor == null)
                return "None";

            return actor.Prefab != null ? actor.Prefab.name : actor.name;
        }
    }
}