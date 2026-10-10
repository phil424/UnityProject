using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentAbilityInspector
    {
        public static bool DrawAbility(AbilityDefinition ability, PartyMemberDefinition character, PlayableCharacterRosterDefinition roster, RunRewardCatalogDefinition catalog, ref bool showAdvanced)
        {
            if (ability == null)
            {
                EditorGUILayout.HelpBox("Select or create an Ability.", MessageType.Info);

                return false;
            }

            EditorGUILayout.LabelField(ability.DisplayName, EditorStyles.largeLabel);

            GUIStyle typeStyle = new(EditorStyles.miniBoldLabel);

            typeStyle.normal.textColor = GameContentAuthoringPalette.Ability;

            EditorGUILayout.LabelField("Ability Definition", typeStyle);

            SerializedObject serialized = new(ability);

            serialized.Update();

            EditorGUI.BeginChangeCheck();

            DrawSection("Identity");
            DrawProperty(serialized, "displayName");
            DrawProperty(serialized, "icon");

            EditorGUILayout.Space(8);

            DrawSection("Level Progression");
            DrawProperty(serialized, "maxLevel");
            DrawProperty(serialized, "cooldown", true);

            EditorGUILayout.Space(8);

            DrawSection("Runtime");
            DrawProperty(serialized, "runtimePrefab");

            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced", true);

            if (showAdvanced)
            {
                EditorGUI.indentLevel++;
                DrawProperty(serialized, "id");
                EditorGUI.indentLevel--;
            }

            bool changed = EditorGUI.EndChangeCheck();

            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
                EditorUtility.SetDirty(ability);

            EditorGUILayout.Space(12);

            DrawLevelPreview(ability);

            EditorGUILayout.Space(12);

            if (DrawRuntimeBehaviour(ability))
                changed = true;

            EditorGUILayout.Space(12);

            DrawCharacterUsage(ability, character, roster);

            EditorGUILayout.Space(12);

            DrawMasteryUsage(ability, roster);

            EditorGUILayout.Space(12);

            DrawRewardSources(ability, catalog);

            EditorGUILayout.Space(12);

            DrawAssetIdentity(ability, character);

            return changed || applied;
        }

        public static bool DrawEvolution(AbilityEvolutionDefinition evolution, RunRewardCatalogDefinition catalog, ref bool showAdvanced)
        {
            if (evolution == null)
                return false;

            EditorGUILayout.LabelField(evolution.DisplayName, EditorStyles.largeLabel);

            GUIStyle evolutionStyle = new(EditorStyles.miniBoldLabel);

            evolutionStyle.normal.textColor = GameContentAuthoringPalette.Combination;

            EditorGUILayout.LabelField("Ability Evolution", evolutionStyle);

            SerializedObject serialized = new(evolution);

            serialized.Update();

            EditorGUI.BeginChangeCheck();

            DrawSection("Identity");
            DrawProperty(serialized, "displayName");
            DrawProperty(serialized, "description");
            DrawProperty(serialized, "icon");

            EditorGUILayout.Space(8);

            DrawSection("Progression");

            using (new EditorGUI.DisabledScope(true))
            {
                DrawProperty(serialized, "targetAbility");
            }

            DrawProperty(serialized, "requiredEvolutions", true);

            EditorGUILayout.Space(8);

            DrawSection("Presentation");

            DrawProperty(serialized, "replacesAbilityPresentation");

            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced", true);

            if (showAdvanced)
            {
                EditorGUI.indentLevel++;
                DrawProperty(serialized, "id");
                EditorGUI.indentLevel--;
            }

            bool changed = EditorGUI.EndChangeCheck();

            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
                EditorUtility.SetDirty(evolution);

            EditorGUILayout.Space(10);

            DrawEvolutionRewardSources(evolution, catalog);

            EditorGUILayout.Space(10);

            DrawEvolutionAssetIdentity(evolution);

            return changed || applied;
        }

        private static void DrawLevelPreview(AbilityDefinition ability)
        {
            DrawSection("Level Preview");

            List<GameContentAbilityLevelPreview> rows = new();

            GameContentAbilityPreviewUtility.Collect(ability, rows);

            if (rows.Count == 0)
            {
                EditorGUILayout.HelpBox("No level-scaled values are available.", MessageType.Info);

                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Parameter", EditorStyles.miniBoldLabel, GUILayout.Width(170f));

                GUILayout.Label("Lv.1", EditorStyles.miniBoldLabel, GUILayout.Width(70f));

                GUILayout.Label($"Lv.{ability.MaxLevel}", EditorStyles.miniBoldLabel, GUILayout.Width(70f));
            }

            foreach (GameContentAbilityLevelPreview row in rows)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(row.Label, GUILayout.Width(170f));

                    GUILayout.Label(row.LevelOne.ToString("0.##"), GUILayout.Width(70f));

                    GUILayout.Label(row.MaximumLevel.ToString("0.##"), GUILayout.Width(70f));
                }
            }
        }

        private static bool DrawRuntimeBehaviour(AbilityDefinition ability)
        {
            DrawSection("Runtime Behaviour");

            if (ability.RuntimePrefab == null)
            {
                EditorGUILayout.HelpBox("Runtime prefab is missing.", MessageType.Error);

                return false;
            }

            int runtimeCount = GameContentAbilityUtility.GetRuntimeBehaviourCount(ability);

            if (runtimeCount != 1)
            {
                EditorGUILayout.HelpBox($"Runtime prefab must contain exactly one root ActorAbility; currently {runtimeCount}.", MessageType.Error);

                return false;
            }

            ActorAbility runtime = GameContentAbilityUtility.GetRuntimeBehaviour(ability);

            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(runtime.GetType().Name), EditorStyles.miniBoldLabel);

            SerializedObject serialized = new(runtime);

            serialized.Update();

            SerializedProperty iterator = serialized.GetIterator();

            EditorGUI.BeginChangeCheck();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(iterator.Copy(), true);
            }

            bool changed = EditorGUI.EndChangeCheck();

            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
            {
                EditorUtility.SetDirty(runtime);

                PrefabUtility.SavePrefabAsset(ability.RuntimePrefab);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select Runtime Prefab"))
                {
                    Selection.activeObject = ability.RuntimePrefab;

                    EditorGUIUtility.PingObject(ability.RuntimePrefab);
                }

                if (GUILayout.Button("Open Runtime Prefab"))
                {
                    AssetDatabase.OpenAsset(ability.RuntimePrefab);
                }
            }

            return changed || applied;
        }

        private static void DrawCharacterUsage(AbilityDefinition ability, PartyMemberDefinition previewCharacter, PlayableCharacterRosterDefinition roster)
        {
            DrawSection("Character Usage");

            if (previewCharacter != null && GameContentAbilityUtility.TryGetCoreUsage(previewCharacter, ability, out AbilityLoadoutEntry previewUsage))
            {
                Color color = GameContentAuthoringPalette.GetAbilityRoleColor(previewUsage.Role);

                Rect accent = EditorGUILayout.GetControlRect(false, 4f);

                EditorGUI.DrawRect(accent, color);

                EditorGUILayout.LabelField($"{previewCharacter.DisplayName}: " + $"{previewUsage.Role} • Starting Lv.{previewUsage.Level}", EditorStyles.boldLabel);
            }

            List<GameContentAbilityCharacterUsage> usages = new();

            GameContentAbilityUtility.CollectCharacterUsages(roster, ability, usages);

            if (usages.Count == 0)
            {
                EditorGUILayout.HelpBox("No playable Character currently has this Ability in its core kit.", MessageType.Info);

                return;
            }

            foreach (GameContentAbilityCharacterUsage usage in usages)
            {
                if (GUILayout.Button($"{usage.Character.DisplayName} — " + $"{usage.Role} — Lv.{usage.StartingLevel}"))
                {
                    Selection.activeObject = usage.Character;

                    EditorGUIUtility.PingObject(usage.Character);
                }
            }
        }

        private static void DrawMasteryUsage(AbilityDefinition ability, PlayableCharacterRosterDefinition roster)
        {
            DrawSection("Persistent Mastery Development");

            List<GameContentAbilityMasteryUsage> usages = new();

            GameContentAbilityUtility.CollectMasteryUsages(roster, ability, usages);

            if (usages.Count == 0)
            {
                EditorGUILayout.HelpBox("No persistent Character Mastery node currently develops this Ability.", MessageType.Info);

                return;
            }

            foreach (GameContentAbilityMasteryUsage usage in usages)
            {
                if (GUILayout.Button($"{usage.Character.DisplayName} — " + $"{usage.Node.DisplayName}"))
                {
                    Selection.activeObject = usage.Node;

                    EditorGUIUtility.PingObject(usage.Node);
                }
            }
        }

        private static void DrawRewardSources(AbilityDefinition ability, RunRewardCatalogDefinition catalog)
        {
            DrawSection("Run Reward Sources");

            List<RunRewardDefinition> rewards = new();

            GameContentAbilityUtility.CollectRewardSources(catalog, ability, rewards);

            if (rewards.Count == 0)
            {
                EditorGUILayout.HelpBox("No active Catalogue Reward currently unlocks or evolves this Ability.", MessageType.Info);

                return;
            }

            foreach (RunRewardDefinition reward in rewards)
            {
                if (GUILayout.Button($"{reward.DisplayName} — " + $"{reward.GetType().Name}"))
                {
                    Selection.activeObject = reward;

                    EditorGUIUtility.PingObject(reward);
                }
            }
        }

        private static void DrawEvolutionRewardSources(AbilityEvolutionDefinition evolution, RunRewardCatalogDefinition catalog)
        {
            DrawSection("Run Reward Sources");

            List<RunRewardDefinition> rewards = new();

            GameContentAbilityUtility.CollectEvolutionRewardSources(catalog, evolution, rewards);

            if (rewards.Count == 0)
            {
                EditorGUILayout.HelpBox("No active Catalogue Reward currently grants this Evolution.", MessageType.Info);

                return;
            }

            foreach (RunRewardDefinition reward in rewards)
            {
                if (GUILayout.Button(reward.DisplayName))
                {
                    Selection.activeObject = reward;
                    EditorGUIUtility.PingObject(reward);
                }
            }
        }

        private static void DrawAssetIdentity(AbilityDefinition ability, PartyMemberDefinition character)
        {
            DrawSection("Asset Identity");

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Definition", ability, typeof(AbilityDefinition), false);

                EditorGUILayout.ObjectField("Runtime Prefab", ability.RuntimePrefab, typeof(GameObject), false);
            }

            GameContentAbilityScope currentScope = GameContentAssetUtility.GetAbilityScope(ability);

            EditorGUILayout.LabelField("Content Scope", GameContentAssetUtility.GetAbilityScopeLabel(ability));

            EditorGUILayout.HelpBox("Content Scope controls where the Ability assets live and how they are named. " + "Character ownership is authored separately.", MessageType.None);

            if (GUILayout.Button("Sync Names In Current Scope"))
            {
                GameContentAssetUtility.SyncAbilityAssetNames(ability);
            }

            if (currentScope == GameContentAbilityScope.SelectedCharacter)
            {
                if (GUILayout.Button("Make Shared + Sync Names"))
                {
                    if (!GameContentAssetUtility.SyncAbilityAssetsToScope(ability, null, GameContentAbilityScope.Shared, out string error))
                    {
                        EditorUtility.DisplayDialog("Ability Asset Sync Failed", error, "OK");
                    }
                    else
                    {
                        Selection.activeObject = ability;

                        EditorGUIUtility.PingObject(ability);
                    }
                }
            }
            else if (character != null)
            {
                if (GUILayout.Button($"Move To {character.DisplayName} + Sync Names"))
                {
                    if (!GameContentAssetUtility.SyncAbilityAssetsToScope(ability, character, GameContentAbilityScope.SelectedCharacter, out string error))
                    {
                        EditorUtility.DisplayDialog("Ability Asset Sync Failed", error, "OK");
                    }
                    else
                    {
                        Selection.activeObject = ability;

                        EditorGUIUtility.PingObject(ability);
                    }
                }
            }

            if (GUILayout.Button("Select Ability Asset"))
            {
                Selection.activeObject = ability;
                EditorGUIUtility.PingObject(ability);
            }
        }

        private static void DrawEvolutionAssetIdentity(AbilityEvolutionDefinition evolution)
        {
            DrawSection("Asset Identity");

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Evolution", evolution, typeof(AbilityEvolutionDefinition), false);
            }

            if (GUILayout.Button("Sync Evolution Asset File Name"))
            {
                GameContentAssetUtility.SyncAbilityEvolutionAssetName(evolution);
            }

            if (GUILayout.Button("Select Evolution Asset"))
            {
                Selection.activeObject = evolution;
                EditorGUIUtility.PingObject(evolution);
            }
        }

        private static void DrawSection(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        private static void DrawProperty(SerializedObject serialized, string name, bool includeChildren = false)
        {
            SerializedProperty property = serialized.FindProperty(name);

            if (property != null)
            {
                EditorGUILayout.PropertyField(property, includeChildren);
            }
        }
    }
}
