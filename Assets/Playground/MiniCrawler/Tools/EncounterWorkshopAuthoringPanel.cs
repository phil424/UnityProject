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
                new Rect(10f, 300f, width, height),
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
            GUILayout.Label("Changes exist only in Play Mode until J6 adds Save / Save As.");

            GUILayout.Space(6f);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            DrawIdentity(draft);

            GUILayout.Space(8f);

            for (int i = 0; i < draft.Phases.Count; i++)
                DrawPhase(draft, draft.Phases[i], i, draft.Phases.Count, actorPalette);

            if (draft.Phases.Count == 0)
            {
                GUILayout.Label("UNPHASED ENCOUNTER");

                for (int i = 0; i < draft.UnphasedGroups.Count; i++)
                    DrawGroup(draft, draft.UnphasedGroups[i], i, actorPalette);
            }

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

            string newName = GUILayout.TextField(phase.DisplayName ?? string.Empty);

            if (newName != phase.DisplayName)
            {
                phase.DisplayName = newName;
                draft.MarkModified();
            }

            if (phaseIndex < phaseCount - 1)
            {
                float newDelay = DrawFloatSlider(
                    "Advance After",
                    phase.AdvanceAfterSeconds,
                    0f,
                    30f
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
                DrawGroup(draft, phase.Groups[i], i, actorPalette);

            GUILayout.EndVertical();
        }

        private void DrawGroup(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnGroupDraft group,
            int groupIndex,
            IReadOnlyList<ActorDefinition> actorPalette)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"SPAWN GROUP {groupIndex + 1} — {group.Name}");

            for (int i = 0; i < group.Entries.Count; i++)
                DrawEntry(draft, group.Entries[i], i, actorPalette);

            for (int i = 0; i < group.Sources.Count; i++)
                DrawSource(draft, group.Sources[i], i);

            GUILayout.EndVertical();
        }

        private void DrawEntry(
            EncounterWorkshopDraft draft,
            EncounterWorkshopDraft.SpawnEntryDraft entry,
            int entryIndex,
            IReadOnlyList<ActorDefinition> actorPalette)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"ENTRY {entryIndex + 1}");

            DrawActorSelector(draft, entry, actorPalette);

            int newCount = DrawIntSlider("Count", entry.Count, 1, 50);

            if (newCount != entry.Count)
            {
                entry.Count = newCount;
                entry.BatchSize = Mathf.Clamp(entry.BatchSize, 1, entry.Count);
                draft.MarkModified();
            }

            float newStartDelay = DrawFloatSlider(
                "Start Delay",
                entry.StartDelay,
                0f,
                20f
            );

            if (!Mathf.Approximately(newStartDelay, entry.StartDelay))
            {
                entry.StartDelay = newStartDelay;
                draft.MarkModified();
            }

            int newBatchSize = DrawIntSlider(
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

            float newSpawnSpacing = DrawFloatSlider(
                "Spawn Spacing",
                entry.TimeBetweenSpawns,
                0f,
                5f
            );

            if (!Mathf.Approximately(newSpawnSpacing, entry.TimeBetweenSpawns))
            {
                entry.TimeBetweenSpawns = newSpawnSpacing;
                draft.MarkModified();
            }

            float newBatchSpacing = DrawFloatSlider(
                "Batch Spacing",
                entry.TimeBetweenBatches,
                0f,
                20f
            );

            if (!Mathf.Approximately(newBatchSpacing, entry.TimeBetweenBatches))
            {
                entry.TimeBetweenBatches = newBatchSpacing;
                draft.MarkModified();
            }

            GUILayout.EndVertical();
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
            int sourceIndex)
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
                    float newRadius = DrawFloatSlider(
                        "Radius",
                        source.Radius,
                        0f,
                        15f
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
                    float width = DrawFloatSlider(
                        "Width",
                        source.BoxSize.x,
                        0.5f,
                        20f
                    );

                    float depth = DrawFloatSlider(
                        "Depth",
                        source.BoxSize.y,
                        0.5f,
                        20f
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

        private static int DrawIntSlider(string label, int value, int minimum, int maximum)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{label}: {value}", GUILayout.Width(145f));

            float sliderValue = GUILayout.HorizontalSlider(
                value,
                minimum,
                maximum,
                GUILayout.Width(335f)
            );

            GUILayout.EndHorizontal();

            return Mathf.Clamp(
                Mathf.RoundToInt(sliderValue),
                minimum,
                maximum
            );
        }

        private static float DrawFloatSlider(
            string label,
            float value,
            float minimum,
            float maximum)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{label}: {value:0.00}", GUILayout.Width(145f));

            float sliderValue = GUILayout.HorizontalSlider(
                value,
                minimum,
                maximum,
                GUILayout.Width(335f)
            );

            GUILayout.EndHorizontal();

            return Mathf.Clamp(sliderValue, minimum, maximum);
        }
    }
}