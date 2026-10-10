using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal readonly struct GameContentCharacterCreationRequest
    {
        public GameContentCharacterCreationRequest(string displayName, string id, bool unlockedByDefault, bool visibleWhenLocked)
        {
            DisplayName = displayName;
            Id = id;
            UnlockedByDefault = unlockedByDefault;
            VisibleWhenLocked = visibleWhenLocked;
        }

        public string DisplayName { get; }
        public string Id { get; }
        public bool UnlockedByDefault { get; }
        public bool VisibleWhenLocked { get; }
    }

    internal static class GameContentCharacterFactory
    {
        public static PartyMemberDefinition Create(GameContentCharacterCreationRequest request, out string error)
        {
            error = Validate(request);

            if (!string.IsNullOrWhiteSpace(error))
                return null;

            PlayableCharacterRosterDefinition roster = GameContentRosterUtility.FindRoster();

            if (roster == null)
            {
                error = "No Playable Character Roster could be found.";
                return null;
            }

            string token = GameContentAssetUtility.ToFileToken(request.DisplayName);

            string masteryFolder = $"{GameContentAssetUtility.MasteryRoot}/{token}";

            if (!AssetDatabase.IsValidFolder(masteryFolder))
            {
                AssetDatabase.CreateFolder(GameContentAssetUtility.MasteryRoot, token);
            }

            string characterPath = $"{GameContentAssetUtility.PartyFolder}/" + $"{GameContentAssetUtility.GetCharacterAssetName(request.DisplayName)}.asset";

            string actorPath = $"{GameContentAssetUtility.ActorFolder}/" + $"{GameContentAssetUtility.GetActorAssetName(request.DisplayName)}.asset";

            string masteryPath = $"{masteryFolder}/" + $"{GameContentAssetUtility.GetMasteryAssetName(request.DisplayName)}.asset";

            ActorDefinition actor = ScriptableObject.CreateInstance<ActorDefinition>();

            CharacterMasteryDefinition mastery = ScriptableObject.CreateInstance<CharacterMasteryDefinition>();

            PartyMemberDefinition character = ScriptableObject.CreateInstance<PartyMemberDefinition>();

            actor.name = GameContentAssetUtility.GetActorAssetName(request.DisplayName);

            mastery.name = GameContentAssetUtility.GetMasteryAssetName(request.DisplayName);

            character.name = GameContentAssetUtility.GetCharacterAssetName(request.DisplayName);

            AssetDatabase.CreateAsset(actor, actorPath);
            AssetDatabase.CreateAsset(mastery, masteryPath);
            AssetDatabase.CreateAsset(character, characterPath);

            Undo.RegisterCreatedObjectUndo(actor, "Create Character Actor Definition");

            Undo.RegisterCreatedObjectUndo(mastery, "Create Character Mastery Definition");

            Undo.RegisterCreatedObjectUndo(character, "Create Playable Character");

            ConfigureActor(actor, request);
            ConfigureCharacter(character, actor, mastery, request);

            if (!GameContentRosterUtility.AddCharacter(roster, character, request.UnlockedByDefault, request.VisibleWhenLocked))
            {
                error = "The Character assets were created, but the Character could not be added to the playable roster.";

                return character;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = character;
            EditorGUIUtility.PingObject(character);

            return character;
        }

        public static string Validate(GameContentCharacterCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return "Display Name is required.";

            if (string.IsNullOrWhiteSpace(request.Id))
                return "Stable Character ID is required.";

            string token = GameContentAssetUtility.ToFileToken(request.DisplayName);

            if (string.IsNullOrWhiteSpace(token))
            {
                return "Display Name must contain at least one letter or number.";
            }

            foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(PartyMemberDefinition)}"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                PartyMemberDefinition existing = AssetDatabase.LoadAssetAtPath<PartyMemberDefinition>(path);

                if (existing != null && existing.Id == request.Id)
                {
                    return $"A Character already uses ID '{request.Id}'.";
                }
            }

            string characterPath = $"{GameContentAssetUtility.PartyFolder}/" + $"{GameContentAssetUtility.GetCharacterAssetName(request.DisplayName)}.asset";

            string actorPath = $"{GameContentAssetUtility.ActorFolder}/" + $"{GameContentAssetUtility.GetActorAssetName(request.DisplayName)}.asset";

            string masteryFolder = $"{GameContentAssetUtility.MasteryRoot}/{token}";

            string masteryPath = $"{masteryFolder}/" + $"{GameContentAssetUtility.GetMasteryAssetName(request.DisplayName)}.asset";

            if (AssetDatabase.LoadMainAssetAtPath(characterPath) != null)
                return $"Character asset already exists at '{characterPath}'.";

            if (AssetDatabase.LoadMainAssetAtPath(actorPath) != null)
                return $"Actor Definition already exists at '{actorPath}'.";

            if (AssetDatabase.LoadMainAssetAtPath(masteryPath) != null)
                return $"Mastery Definition already exists at '{masteryPath}'.";

            return null;
        }

        private static void ConfigureActor(ActorDefinition actor, GameContentCharacterCreationRequest request)
        {
            SerializedObject serialized = new(actor);

            SerializedProperty id = serialized.FindProperty("id");

            if (id != null)
                id.stringValue = $"{request.Id}_actor";

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(actor);
        }

        private static void ConfigureCharacter(PartyMemberDefinition character, ActorDefinition actor, CharacterMasteryDefinition mastery, GameContentCharacterCreationRequest request)
        {
            SerializedObject serialized = new(character);

            serialized.FindProperty("id").stringValue = request.Id;

            serialized.FindProperty("displayName").stringValue = request.DisplayName;

            serialized.FindProperty("actorDefinition").objectReferenceValue = actor;

            serialized.FindProperty("masteryDefinition").objectReferenceValue = mastery;

            SerializedProperty abilities = serialized.FindProperty("startingAbilities");

            abilities.arraySize = 3;

            CharacterAbilityRole[] roles = { CharacterAbilityRole.Signature, CharacterAbilityRole.Utility, CharacterAbilityRole.Sustain };

            for (int i = 0; i < abilities.arraySize; i++)
            {
                SerializedProperty entry = abilities.GetArrayElementAtIndex(i);

                entry.FindPropertyRelative("role").enumValueIndex = (int)roles[i];
                entry.FindPropertyRelative("ability").objectReferenceValue = null;
                entry.FindPropertyRelative("level").intValue = 1;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(character);
        }
    }
}
