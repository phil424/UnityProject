using System;
using System.Collections.Generic;
using System.Reflection;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal enum GameContentAbilityScope
    {
        Shared,
        SelectedCharacter,
    }

    internal readonly struct GameContentAbilityCreationRequest
    {
        public GameContentAbilityCreationRequest(
            string displayName,
            string id,
            PartyMemberDefinition character,
            GameContentAbilityScope scope,
            Type runtimeType,
            int maxLevel,
            float cooldown,
            float cooldownPerLevel
        )
        {
            DisplayName = displayName;
            Id = id;
            Character = character;
            Scope = scope;
            RuntimeType = runtimeType;
            MaxLevel = maxLevel;
            Cooldown = cooldown;
            CooldownPerLevel = cooldownPerLevel;
        }

        public string DisplayName { get; }
        public string Id { get; }
        public PartyMemberDefinition Character { get; }
        public GameContentAbilityScope Scope { get; }
        public Type RuntimeType { get; }
        public int MaxLevel { get; }
        public float Cooldown { get; }
        public float CooldownPerLevel { get; }

        public PartyMemberDefinition ScopeCharacter => Scope == GameContentAbilityScope.SelectedCharacter ? Character : null;
    }

    internal static class GameContentAbilityFactory
    {
        public static void CollectRuntimeTypes(List<Type> output)
        {
            output.Clear();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<ActorAbility>())
            {
                if (type.IsAbstract)
                    continue;

                output.Add(type);
            }

            output.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));
        }

        public static AbilityDefinition Create(GameContentAbilityCreationRequest request, out string error)
        {
            error = ValidateCreation(request);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            PartyMemberDefinition scopeCharacter = request.ScopeCharacter;

            string dataFolder = GameContentAssetUtility.GetAbilityDataFolder(scopeCharacter);

            string runtimeFolder = GameContentAssetUtility.GetAbilityRuntimeFolder(scopeCharacter);

            GameContentAssetUtility.EnsureFolder(dataFolder);
            GameContentAssetUtility.EnsureFolder(runtimeFolder);

            string abilityName = GameContentAssetUtility.GetAbilityAssetName(scopeCharacter, request.DisplayName);

            string runtimeName = GameContentAssetUtility.GetAbilityRuntimePrefabName(scopeCharacter, request.DisplayName);

            string abilityPath = $"{dataFolder}/{abilityName}.asset";

            string runtimePath = $"{runtimeFolder}/{runtimeName}.prefab";

            GameObject temporary = new(runtimeName);

            ActorAbility runtime = temporary.AddComponent(request.RuntimeType) as ActorAbility;

            if (runtime == null)
            {
                UnityEngine.Object.DestroyImmediate(temporary);

                error = $"'{request.RuntimeType.Name}' could not be added as an ActorAbility.";

                return null;
            }

            InitializeAbilityLevelValues(runtime);

            GameObject runtimePrefab = PrefabUtility.SaveAsPrefabAsset(temporary, runtimePath, out bool prefabSaved);

            UnityEngine.Object.DestroyImmediate(temporary);

            if (!prefabSaved || runtimePrefab == null)
            {
                error = $"Could not create runtime prefab at '{runtimePath}'.";

                return null;
            }

            AbilityDefinition ability = ScriptableObject.CreateInstance<AbilityDefinition>();

            ability.name = abilityName;

            try
            {
                AssetDatabase.CreateAsset(ability, abilityPath);
            }
            catch (Exception exception)
            {
                AssetDatabase.DeleteAsset(runtimePath);

                UnityEngine.Object.DestroyImmediate(ability);

                error = exception.Message;
                return null;
            }

            Undo.RegisterCreatedObjectUndo(ability, "Create Ability Definition");

            SerializedObject serialized = new(ability);

            serialized.FindProperty("id").stringValue = request.Id;

            serialized.FindProperty("displayName").stringValue = request.DisplayName;

            serialized.FindProperty("maxLevel").intValue = Mathf.Max(1, request.MaxLevel);

            SerializedProperty cooldown = serialized.FindProperty("cooldown");

            if (cooldown != null)
            {
                cooldown.FindPropertyRelative("baseValue").floatValue = Mathf.Max(0.1f, request.Cooldown);

                cooldown.FindPropertyRelative("perLevel").floatValue = request.CooldownPerLevel;
            }

            serialized.FindProperty("runtimePrefab").objectReferenceValue = runtimePrefab;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(ability);
            AssetDatabase.SaveAssets();

            Selection.activeObject = ability;
            EditorGUIUtility.PingObject(ability);

            return ability;
        }

        public static AbilityEvolutionDefinition CreateEvolution(AbilityDefinition ability, string displayName, string id, string description, bool replacesPresentation, out string error)
        {
            error = ValidateEvolutionCreation(ability, displayName, id);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            string folder = GameContentAssetUtility.GetAbilityEvolutionFolder(ability);

            GameContentAssetUtility.EnsureFolder(folder);

            string assetName = GameContentAssetUtility.GetAbilityEvolutionAssetName(displayName);

            string path = $"{folder}/{assetName}.asset";

            AbilityEvolutionDefinition evolution = ScriptableObject.CreateInstance<AbilityEvolutionDefinition>();

            evolution.name = assetName;

            AssetDatabase.CreateAsset(evolution, path);

            Undo.RegisterCreatedObjectUndo(evolution, "Create Ability Evolution");

            SerializedObject serialized = new(evolution);

            serialized.FindProperty("id").stringValue = id;

            serialized.FindProperty("displayName").stringValue = displayName;

            serialized.FindProperty("description").stringValue = description ?? string.Empty;

            serialized.FindProperty("targetAbility").objectReferenceValue = ability;

            serialized.FindProperty("replacesAbilityPresentation").boolValue = replacesPresentation;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(evolution);
            AssetDatabase.SaveAssets();

            Selection.activeObject = evolution;
            EditorGUIUtility.PingObject(evolution);

            return evolution;
        }

        public static string ValidateCreation(GameContentAbilityCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return "Display Name is required.";

            if (string.IsNullOrWhiteSpace(request.Id))
                return "Stable Ability ID is required.";

            if (request.RuntimeType == null || !typeof(ActorAbility).IsAssignableFrom(request.RuntimeType) || request.RuntimeType.IsAbstract)
            {
                return "A concrete ActorAbility runtime behaviour is required.";
            }

            if (request.Scope == GameContentAbilityScope.SelectedCharacter && request.Character == null)
            {
                return "Selected Character scope requires a Character.";
            }

            if (GameContentAbilityUtility.AbilityIdExists(request.Id))
            {
                return $"Ability ID '{request.Id}' is already in use.";
            }

            PartyMemberDefinition scopeCharacter = request.ScopeCharacter;

            string dataPath = $"{GameContentAssetUtility.GetAbilityDataFolder(scopeCharacter)}/" + $"{GameContentAssetUtility.GetAbilityAssetName(scopeCharacter, request.DisplayName)}.asset";

            string runtimePath =
                $"{GameContentAssetUtility.GetAbilityRuntimeFolder(scopeCharacter)}/" + $"{GameContentAssetUtility.GetAbilityRuntimePrefabName(scopeCharacter, request.DisplayName)}.prefab";

            if (AssetDatabase.LoadMainAssetAtPath(dataPath) != null)
                return $"An Ability asset already exists at '{dataPath}'.";

            if (AssetDatabase.LoadMainAssetAtPath(runtimePath) != null)
                return $"A runtime prefab already exists at '{runtimePath}'.";

            return null;
        }

        public static string ValidateEvolutionCreation(AbilityDefinition ability, string displayName, string id)
        {
            if (ability == null)
                return "Target Ability is required.";

            if (string.IsNullOrWhiteSpace(displayName))
                return "Display Name is required.";

            if (string.IsNullOrWhiteSpace(id))
                return "Stable Evolution ID is required.";

            if (GameContentAbilityUtility.EvolutionIdExists(id))
                return $"Evolution ID '{id}' is already in use.";

            string folder = GameContentAssetUtility.GetAbilityEvolutionFolder(ability);

            string path = $"{folder}/" + $"{GameContentAssetUtility.GetAbilityEvolutionAssetName(displayName)}.asset";

            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
                return $"An Evolution asset already exists at '{path}'.";

            return null;
        }

        private static void InitializeAbilityLevelValues(ActorAbility runtime)
        {
            Type type = runtime.GetType();

            while (type != null && typeof(ActorAbility).IsAssignableFrom(type))
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType != typeof(AbilityLevelValue))
                    {
                        continue;
                    }

                    if (field.GetValue(runtime) == null)
                    {
                        field.SetValue(runtime, new AbilityLevelValue());
                    }
                }

                type = type.BaseType;
            }
        }
    }
}
