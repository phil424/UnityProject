using System;
using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentAbilityCreateWindow : EditorWindow
    {
        private readonly List<Type> runtimeTypes = new();

        private PartyMemberDefinition character;

        private GameContentAbilityScope scope = GameContentAbilityScope.Shared;

        private int runtimeTypeIndex;

        private string displayName;
        private string id;
        private bool idEdited;

        private int maxLevel = 5;
        private float cooldown = 5f;
        private float cooldownPerLevel;

        public static void Open(PartyMemberDefinition character)
        {
            GameContentAbilityCreateWindow window = CreateInstance<GameContentAbilityCreateWindow>();

            window.character = character;
            window.scope = GameContentAbilityScope.Shared;

            window.titleContent = new GUIContent("New Ability");

            window.minSize = new Vector2(460f, 360f);

            window.maxSize = new Vector2(600f, 500f);

            window.RefreshRuntimeTypes();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            RefreshRuntimeTypes();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Ability", EditorStyles.largeLabel);

            EditorGUILayout.Space(6);

            EditorGUI.BeginChangeCheck();

            string nextName = EditorGUILayout.TextField("Display Name", displayName);

            if (EditorGUI.EndChangeCheck())
            {
                displayName = nextName;

                if (!idEdited)
                {
                    id = GameContentAssetUtility.ToIdToken(displayName);
                }
            }

            EditorGUI.BeginChangeCheck();

            string nextId = EditorGUILayout.TextField("Stable ID", id);

            if (EditorGUI.EndChangeCheck())
            {
                id = nextId;
                idEdited = true;
            }

            if (character != null)
            {
                scope = (GameContentAbilityScope)EditorGUILayout.EnumPopup("Content Scope", scope);

                if (scope == GameContentAbilityScope.SelectedCharacter)
                {
                    EditorGUILayout.ObjectField("Character", character, typeof(PartyMemberDefinition), false);
                    EditorGUILayout.HelpBox("Content Scope controls asset organisation and filenames only. " + "It does not determine which Character owns or can use the Ability.", MessageType.None);
                }
            }
            else
            {
                scope = GameContentAbilityScope.Shared;
            }

            EditorGUILayout.Space(6);

            if (runtimeTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No concrete ActorAbility runtime types were found.", MessageType.Error);

                return;
            }

            string[] runtimeLabels = new string[runtimeTypes.Count];

            for (int i = 0; i < runtimeTypes.Count; i++)
            {
                runtimeLabels[i] = ObjectNames.NicifyVariableName(runtimeTypes[i].Name);
            }

            runtimeTypeIndex = EditorGUILayout.Popup("Runtime Behaviour", Mathf.Clamp(runtimeTypeIndex, 0, runtimeTypes.Count - 1), runtimeLabels);

            maxLevel = Mathf.Max(1, EditorGUILayout.IntField("Max Level", maxLevel));

            cooldown = Mathf.Max(0.1f, EditorGUILayout.FloatField("Base Cooldown", cooldown));

            cooldownPerLevel = EditorGUILayout.FloatField("Cooldown Per Level", cooldownPerLevel);

            Type runtimeType = runtimeTypes[runtimeTypeIndex];

            GameContentAbilityCreationRequest request = new(displayName, id, character, scope, runtimeType, maxLevel, cooldown, cooldownPerLevel);

            string error = GameContentAbilityFactory.ValidateCreation(request);

            EditorGUILayout.Space(8);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                PartyMemberDefinition scopeCharacter = request.ScopeCharacter;

                EditorGUILayout.HelpBox(
                    $"Definition:\n"
                        + $"{GameContentAssetUtility.GetAbilityDataFolder(scopeCharacter)}/"
                        + $"{GameContentAssetUtility.GetAbilityAssetName(scopeCharacter, displayName)}.asset\n\n"
                        + $"Runtime:\n"
                        + $"{GameContentAssetUtility.GetAbilityRuntimeFolder(scopeCharacter)}/"
                        + $"{GameContentAssetUtility.GetAbilityRuntimePrefabName(scopeCharacter, displayName)}.prefab",
                    MessageType.None
                );

                EditorGUILayout.HelpBox(
                    "The runtime prefab will contain the selected ActorAbility behaviour. " + "Configure its gameplay values immediately after creation in the Ability workspace.",
                    MessageType.Info
                );
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error)))
            {
                if (GUILayout.Button("Create Ability", GUILayout.Height(32f)))
                {
                    AbilityDefinition created = GameContentAbilityFactory.Create(request, out string createError);

                    if (created == null)
                    {
                        EditorUtility.DisplayDialog("Ability Creation Failed", createError ?? "Unknown creation error.", "OK");

                        return;
                    }

                    Close();
                }
            }
        }

        private void RefreshRuntimeTypes()
        {
            GameContentAbilityFactory.CollectRuntimeTypes(runtimeTypes);

            runtimeTypeIndex = Mathf.Clamp(runtimeTypeIndex, 0, Mathf.Max(0, runtimeTypes.Count - 1));
        }
    }
}
