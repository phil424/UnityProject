using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal readonly struct GameContentAuthoringIssue
    {
        public GameContentAuthoringIssue(string section, string message, MessageType messageType, bool required)
        {
            Section = section;
            Message = message;
            MessageType = messageType;
            Required = required;
        }

        public string Section { get; }
        public string Message { get; }
        public MessageType MessageType { get; }
        public bool Required { get; }
    }

    internal static class GameContentCharacterValidation
    {
        private static readonly CharacterAbilityRole[] RequiredAbilityRoles = { CharacterAbilityRole.Signature, CharacterAbilityRole.Utility, CharacterAbilityRole.Sustain };

        public static void Collect(PartyMemberDefinition character, PlayableCharacterRosterDefinition roster, List<GameContentAuthoringIssue> output)
        {
            output.Clear();

            if (character == null)
                return;

            if (string.IsNullOrWhiteSpace(character.Id))
                AddRequired(output, "Identity", "Stable Character ID is missing.");

            if (string.IsNullOrWhiteSpace(character.DisplayName))
                AddRequired(output, "Identity", "Display Name is missing.");

            if (character.ActorDefinition == null)
            {
                AddRequired(output, "Runtime Actor", "Actor Definition is missing.");
            }
            else if (character.ActorDefinition.Prefab == null)
            {
                AddRequired(output, "Runtime Actor", "Actor prefab is missing.");
            }

            ValidateCoreAbilities(character, output);

            if (character.MasteryDefinition == null)
                AddRequired(output, "Mastery", "Character Mastery Definition is missing.");

            if (roster == null || !roster.Contains(character))
                AddRequired(output, "Playable Roster", "Character is not registered in the playable Character roster.");

            if (string.IsNullOrWhiteSpace(character.Role))
                AddRecommended(output, "Identity", "Role has not been authored.");

            if (string.IsNullOrWhiteSpace(character.Description))
                AddRecommended(output, "Identity", "Character description has not been authored.");

            if (character.Portrait == null)
                AddRecommended(output, "Presentation", "Character portrait has not been assigned.");

            if (character.MasteryDefinition != null && character.MasteryDefinition.Nodes.Count == 0)
                AddRecommended(output, "Mastery", "Mastery Definition currently contains no nodes.");
        }

        public static int CountRequired(IReadOnlyList<GameContentAuthoringIssue> issues)
        {
            int count = 0;

            foreach (GameContentAuthoringIssue issue in issues)
            {
                if (issue.Required)
                    count++;
            }

            return count;
        }

        private static void ValidateCoreAbilities(PartyMemberDefinition character, List<GameContentAuthoringIssue> output)
        {
            if (character.StartingAbilities.Count != 3)
            {
                AddRequired(output, "Core Abilities", $"Character should have exactly 3 core Ability entries; currently {character.StartingAbilities.Count}.");
            }

            HashSet<CharacterAbilityRole> roles = new();
            HashSet<string> abilityIds = new();

            foreach (AbilityLoadoutEntry entry in character.StartingAbilities)
            {
                if (entry == null)
                {
                    AddRequired(output, "Core Abilities", "Core Ability entry is missing.");
                    continue;
                }

                if (entry.Role == CharacterAbilityRole.Unassigned)
                {
                    AddRequired(output, "Core Abilities", "A core Ability has no Signature / Utility / Sustain role.");
                }
                else if (!roles.Add(entry.Role))
                {
                    AddRequired(output, "Core Abilities", $"Core Ability role '{entry.Role}' is assigned more than once.");
                }

                if (entry.Ability == null)
                {
                    string roleName = entry.Role == CharacterAbilityRole.Unassigned ? "Unassigned" : entry.Role.ToString();

                    AddRequired(output, "Core Abilities", $"{roleName} Ability is not assigned.");
                    continue;
                }

                if (!abilityIds.Add(entry.Ability.Id))
                    AddRequired(output, "Core Abilities", $"Ability '{entry.Ability.DisplayName}' occupies more than one core role.");
            }

            foreach (CharacterAbilityRole role in RequiredAbilityRoles)
            {
                if (!roles.Contains(role))
                    AddRequired(output, "Core Abilities", $"{role} role is missing.");
            }
        }

        private static void AddRequired(List<GameContentAuthoringIssue> output, string section, string message)
        {
            output.Add(new GameContentAuthoringIssue(section, message, MessageType.Error, true));
        }

        private static void AddRecommended(List<GameContentAuthoringIssue> output, string section, string message)
        {
            output.Add(new GameContentAuthoringIssue(section, message, MessageType.Warning, false));
        }
    }
}
