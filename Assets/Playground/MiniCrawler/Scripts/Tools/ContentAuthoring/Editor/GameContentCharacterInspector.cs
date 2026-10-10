using System;
using System.Collections.Generic;
using System.IO;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentCharacterInspector
    {
        public static bool Draw(PartyMemberDefinition character, PlayableCharacterRosterDefinition roster, IReadOnlyList<GameContentAuthoringIssue> issues, ref bool showAdvanced, Action openMastery)
        {
            if (character == null)
            {
                EditorGUILayout.HelpBox("Select or create a playable Character.", MessageType.Info);

                return false;
            }

            EditorGUILayout.LabelField(character.DisplayName, EditorStyles.largeLabel);

            DrawReadiness(issues);

            EditorGUILayout.Space(10);

            DrawCharacterFields(character, ref showAdvanced);

            EditorGUILayout.Space(10);

            DrawActorFields(character.ActorDefinition, showAdvanced);

            EditorGUILayout.Space(10);

            DrawRosterFields(roster, character);

            EditorGUILayout.Space(10);

            DrawAssetIdentity(character);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Mastery", EditorStyles.boldLabel);

            if (character.MasteryDefinition == null)
            {
                EditorGUILayout.HelpBox("Character Mastery Definition is required.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.ObjectField("Definition", character.MasteryDefinition, typeof(CharacterMasteryDefinition), false);

                EditorGUILayout.LabelField("Nodes", character.MasteryDefinition.Nodes.Count.ToString());
            }

            using (new EditorGUI.DisabledScope(character.MasteryDefinition == null))
            {
                string label = character.MasteryDefinition != null ? $"Go to Mastery ({character.MasteryDefinition.Nodes.Count} nodes)" : "Go to Mastery";

                if (GUILayout.Button(label))
                {
                    openMastery?.Invoke();
                    return true;
                }
            }

            return false;
        }

        private static void DrawReadiness(IReadOnlyList<GameContentAuthoringIssue> issues)
        {
            int required = GameContentCharacterValidation.CountRequired(issues);

            EditorGUILayout.LabelField(required == 0 ? "Readiness: Expedition-ready" : $"Readiness: {required} required item(s) missing", EditorStyles.boldLabel);

            foreach (GameContentAuthoringIssue issue in issues)
            {
                EditorGUILayout.HelpBox($"{issue.Section}: {issue.Message}", issue.MessageType);
            }
        }

        private static void DrawCharacterFields(PartyMemberDefinition character, ref bool showAdvanced)
        {
            SerializedObject serialized = new(character);

            serialized.Update();

            EditorGUI.BeginChangeCheck();

            DrawSection("Identity", !string.IsNullOrWhiteSpace(character.Id) && !string.IsNullOrWhiteSpace(character.DisplayName));

            DrawProperty(serialized, "displayName");
            DrawProperty(serialized, "id");
            DrawProperty(serialized, "role");
            DrawProperty(serialized, "description");
            DrawProperty(serialized, "portrait");

            EditorGUILayout.Space(8);

            DrawSection("Runtime Actor", character.ActorDefinition != null && character.ActorDefinition.Prefab != null);

            DrawProperty(serialized, "actorDefinition");

            EditorGUILayout.Space(8);

            bool abilitiesReady =
                character.GetStartingAbility(CharacterAbilityRole.Signature)?.Ability != null
                && character.GetStartingAbility(CharacterAbilityRole.Utility)?.Ability != null
                && character.GetStartingAbility(CharacterAbilityRole.Sustain)?.Ability != null;

            DrawSection("Core Ability Kit", abilitiesReady);

            DrawCoreAbilityKit(serialized);

            EditorGUILayout.Space(8);

            DrawSection("Mastery Definition", character.MasteryDefinition != null);

            DrawProperty(serialized, "masteryDefinition");

            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Character Data", true);

            if (showAdvanced)
            {
                EditorGUI.indentLevel++;

                DrawProperty(serialized, "weaponName");
                DrawProperty(serialized, "armourName");
                DrawProperty(serialized, "focusName");

                EditorGUILayout.Space(4);

                DrawProperty(serialized, "damagePerWeaponLevel");
                DrawProperty(serialized, "armourPerArmourLevel");
                DrawProperty(serialized, "healthPerArmourLevel");
                DrawProperty(serialized, "healingPerFocusLevel");
                DrawProperty(serialized, "baseUpgradeCost");
                DrawProperty(serialized, "upgradeCostStep");
                DrawProperty(serialized, "baseAbilityUpgradeCost");
                DrawProperty(serialized, "abilityUpgradeCostStep");

                EditorGUI.indentLevel--;
            }

            bool changed = EditorGUI.EndChangeCheck();

            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
                EditorUtility.SetDirty(character);
        }

        private static void DrawCoreAbilityKit(SerializedObject serialized)
        {
            SerializedProperty abilities = serialized.FindProperty("startingAbilities");

            if (abilities == null)
                return;

            if (!HasExplicitCoreRoles(abilities))
            {
                EditorGUILayout.HelpBox(
                    "This Character uses the older anonymous three-slot Ability data. " + "Assign the current order as Signature / Utility / Sustain before continuing.",
                    MessageType.Warning
                );

                using (new EditorGUI.DisabledScope(abilities.arraySize != 3))
                {
                    if (GUILayout.Button("Assign Core Roles From Current Order"))
                        AssignCoreRolesFromCurrentOrder(abilities);
                }

                EditorGUILayout.PropertyField(abilities, true);
                return;
            }

            DrawCoreAbilityEntry(abilities, CharacterAbilityRole.Signature);
            DrawCoreAbilityEntry(abilities, CharacterAbilityRole.Utility);
            DrawCoreAbilityEntry(abilities, CharacterAbilityRole.Sustain);
        }

        private static bool HasExplicitCoreRoles(SerializedProperty abilities)
        {
            if (abilities.arraySize != 3)
                return false;

            HashSet<CharacterAbilityRole> found = new();

            for (int i = 0; i < abilities.arraySize; i++)
            {
                SerializedProperty entry = abilities.GetArrayElementAtIndex(i);
                SerializedProperty roleProperty = entry.FindPropertyRelative("role");

                if (roleProperty == null)
                    return false;

                CharacterAbilityRole role = (CharacterAbilityRole)roleProperty.enumValueIndex;

                if (role == CharacterAbilityRole.Unassigned || !found.Add(role))
                    return false;
            }

            return found.Contains(CharacterAbilityRole.Signature) && found.Contains(CharacterAbilityRole.Utility) && found.Contains(CharacterAbilityRole.Sustain);
        }

        private static void AssignCoreRolesFromCurrentOrder(SerializedProperty abilities)
        {
            if (abilities.arraySize != 3)
                return;

            CharacterAbilityRole[] roles = { CharacterAbilityRole.Signature, CharacterAbilityRole.Utility, CharacterAbilityRole.Sustain };

            for (int i = 0; i < abilities.arraySize; i++)
            {
                SerializedProperty entry = abilities.GetArrayElementAtIndex(i);
                SerializedProperty roleProperty = entry.FindPropertyRelative("role");

                if (roleProperty != null)
                    roleProperty.enumValueIndex = (int)roles[i];
            }
        }

        private static void DrawCoreAbilityEntry(SerializedProperty abilities, CharacterAbilityRole role)
        {
            int index = FindCoreAbilityIndex(abilities, role);

            if (index < 0)
            {
                EditorGUILayout.HelpBox($"{role} Ability entry is missing.", MessageType.Error);
                return;
            }

            SerializedProperty entry = abilities.GetArrayElementAtIndex(index);
            SerializedProperty ability = entry.FindPropertyRelative("ability");
            SerializedProperty level = entry.FindPropertyRelative("level");

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(role.ToString(), EditorStyles.boldLabel);

                if (ability != null)
                    EditorGUILayout.PropertyField(ability, new GUIContent("Ability"));

                if (level != null)
                    EditorGUILayout.PropertyField(level, new GUIContent("Starting Level"));
            }
        }

        private static int FindCoreAbilityIndex(SerializedProperty abilities, CharacterAbilityRole role)
        {
            for (int i = 0; i < abilities.arraySize; i++)
            {
                SerializedProperty entry = abilities.GetArrayElementAtIndex(i);
                SerializedProperty roleProperty = entry.FindPropertyRelative("role");

                if (roleProperty != null && (CharacterAbilityRole)roleProperty.enumValueIndex == role)
                {
                    return i;
                }
            }

            return -1;
        }

        private static void DrawActorFields(ActorDefinition actor, bool showAdvanced)
        {
            if (actor == null)
                return;

            SerializedObject serialized = new(actor);

            serialized.Update();

            EditorGUILayout.LabelField("Actor Definition", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            DrawProperty(serialized, "prefab");

            if (showAdvanced)
                DrawProperty(serialized, "id");

            bool changed = EditorGUI.EndChangeCheck();

            bool applied = serialized.ApplyModifiedProperties();

            if (changed || applied)
                EditorUtility.SetDirty(actor);
        }

        private static void DrawRosterFields(PlayableCharacterRosterDefinition roster, PartyMemberDefinition character)
        {
            EditorGUILayout.LabelField("Playable Roster", EditorStyles.boldLabel);

            if (roster == null)
            {
                EditorGUILayout.HelpBox("Playable Character Roster could not be found.", MessageType.Error);

                return;
            }

            int index = GameContentRosterUtility.FindEntryIndex(roster, character);

            if (index < 0)
            {
                EditorGUILayout.HelpBox("Character is not registered in the playable roster.", MessageType.Error);

                if (GUILayout.Button("Add To Playable Roster"))
                {
                    GameContentRosterUtility.AddCharacter(roster, character, false, true);
                }

                return;
            }

            SerializedObject serialized = new(roster);

            serialized.Update();

            SerializedProperty characters = serialized.FindProperty("characters");

            SerializedProperty entry = characters.GetArrayElementAtIndex(index);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(entry.FindPropertyRelative("unlockedByDefault"), new GUIContent("Unlocked By Default"));

            EditorGUILayout.PropertyField(entry.FindPropertyRelative("visibleWhenLocked"), new GUIContent("Visible When Locked"));

            EditorGUILayout.PropertyField(entry.FindPropertyRelative("unlockRequirement"), new GUIContent("Unlock Requirement"));

            if (EditorGUI.EndChangeCheck())
            {
                serialized.ApplyModifiedProperties();
                EditorUtility.SetDirty(roster);
            }
        }

        private static void DrawAssetIdentity(PartyMemberDefinition character)
        {
            EditorGUILayout.LabelField("Asset Identity", EditorStyles.boldLabel);

            DrawAssetPath("Character", character);

            DrawAssetPath("Actor", character.ActorDefinition);

            DrawAssetPath("Mastery", character.MasteryDefinition);

            if (GUILayout.Button("Sync Character Asset File Names"))
            {
                GameContentAssetUtility.SyncCharacterAssetNames(character);
            }

            EditorGUILayout.HelpBox("File renaming preserves Unity GUID references. Stable gameplay IDs are not automatically changed.", MessageType.None);
        }

        private static void DrawAssetPath(string label, Object asset)
        {
            if (asset == null)
            {
                EditorGUILayout.LabelField(label, "Missing");

                return;
            }

            string path = AssetDatabase.GetAssetPath(asset);

            EditorGUILayout.LabelField(label, Path.GetFileName(path));
        }

        private static void DrawSection(string label, bool ready)
        {
            EditorGUILayout.LabelField($"{label} {(ready ? "[OK]" : "[REQUIRED]")}", EditorStyles.boldLabel);
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
