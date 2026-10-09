using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal static class GameContentRosterUtility
    {
        public static PlayableCharacterRosterDefinition FindRoster()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(PlayableCharacterRosterDefinition)}");

            if (guids.Length == 0)
                return null;

            if (guids.Length > 1)
            {
                Debug.LogWarning("More than one Playable Character Roster exists. " + "The Content Workbench is using the first result.");
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return AssetDatabase.LoadAssetAtPath<PlayableCharacterRosterDefinition>(path);
        }

        public static int FindEntryIndex(PlayableCharacterRosterDefinition roster, PartyMemberDefinition character)
        {
            if (roster == null || character == null)
                return -1;

            for (int i = 0; i < roster.Characters.Count; i++)
            {
                if (roster.Characters[i]?.Character == character)
                    return i;
            }

            return -1;
        }

        public static bool AddCharacter(PlayableCharacterRosterDefinition roster, PartyMemberDefinition character, bool unlockedByDefault, bool visibleWhenLocked)
        {
            if (roster == null || character == null || FindEntryIndex(roster, character) >= 0)
            {
                return false;
            }

            Undo.RecordObject(roster, "Add Playable Character");

            SerializedObject serialized = new(roster);

            SerializedProperty characters = serialized.FindProperty("characters");

            if (characters == null)
                return false;

            int index = characters.arraySize;
            characters.InsertArrayElementAtIndex(index);

            SerializedProperty entry = characters.GetArrayElementAtIndex(index);

            entry.FindPropertyRelative("character").objectReferenceValue = character;

            entry.FindPropertyRelative("unlockedByDefault").boolValue = unlockedByDefault;

            entry.FindPropertyRelative("visibleWhenLocked").boolValue = visibleWhenLocked;

            entry.FindPropertyRelative("unlockRequirement").objectReferenceValue = null;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(roster);

            return true;
        }
    }
}
