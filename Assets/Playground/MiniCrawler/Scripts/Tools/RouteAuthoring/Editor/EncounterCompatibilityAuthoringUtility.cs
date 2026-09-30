using MiniCrawler.Encounters;
using MiniCrawler.Movement;
using UnityEditor;

namespace MiniCrawler.Tools.RouteAuthoring
{
    internal enum EncounterCompatibilityTagRole
    {
        Required,
        Preferred,
        Forbidden
    }

    internal static class EncounterCompatibilityAuthoringUtility
    {
        private const string AllowAutomaticPlacementProperty = "allowAutomaticPlacement";
        private const string RequiredTagsProperty = "requiredRouteTags";
        private const string PreferredTagsProperty = "preferredRouteTags";
        private const string ForbiddenTagsProperty = "forbiddenRouteTags";

        public static bool SetAutomaticPlacement(EncounterDefinition definition, bool value)
        {
            if (definition == null || definition.AllowAutomaticPlacement == value)
                return false;

            Undo.RecordObject(definition, "Set Encounter Automatic Placement");

            SerializedObject serialized = new(definition);
            serialized.Update();
            serialized.FindProperty(AllowAutomaticPlacementProperty).boolValue = value;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(definition);
            return true;
        }

        public static bool SetTagRole(
            EncounterDefinition definition,
            RouteTagDefinition tag,
            EncounterCompatibilityTagRole role)
        {
            if (definition == null || tag == null || HasExclusiveRole(definition, tag, role))
                return false;

            Undo.RecordObject(definition, "Set Encounter Route Tag Role");

            SerializedObject serialized = new(definition);
            serialized.Update();

            SerializedProperty required = serialized.FindProperty(RequiredTagsProperty);
            SerializedProperty preferred = serialized.FindProperty(PreferredTagsProperty);
            SerializedProperty forbidden = serialized.FindProperty(ForbiddenTagsProperty);

            RemoveReference(required, tag);
            RemoveReference(preferred, tag);
            RemoveReference(forbidden, tag);

            AppendReference(GetRoleProperty(role, required, preferred, forbidden), tag);

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(definition);
            return true;
        }

        public static bool RemoveTagConstraint(EncounterDefinition definition, RouteTagDefinition tag)
        {
            if (definition == null || tag == null || !HasAnyRole(definition, tag))
                return false;

            Undo.RecordObject(definition, "Remove Encounter Route Tag Constraint");

            SerializedObject serialized = new(definition);
            serialized.Update();

            RemoveReference(serialized.FindProperty(RequiredTagsProperty), tag);
            RemoveReference(serialized.FindProperty(PreferredTagsProperty), tag);
            RemoveReference(serialized.FindProperty(ForbiddenTagsProperty), tag);

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(definition);
            return true;
        }

        private static SerializedProperty GetRoleProperty(
            EncounterCompatibilityTagRole role,
            SerializedProperty required,
            SerializedProperty preferred,
            SerializedProperty forbidden)
        {
            switch (role)
            {
                case EncounterCompatibilityTagRole.Required:
                    return required;

                case EncounterCompatibilityTagRole.Preferred:
                    return preferred;

                default:
                    return forbidden;
            }
        }

        private static bool HasExclusiveRole(
            EncounterDefinition definition,
            RouteTagDefinition tag,
            EncounterCompatibilityTagRole role)
        {
            bool required = ContainsTag(definition.RequiredRouteTags, tag);
            bool preferred = ContainsTag(definition.PreferredRouteTags, tag);
            bool forbidden = ContainsTag(definition.ForbiddenRouteTags, tag);

            switch (role)
            {
                case EncounterCompatibilityTagRole.Required:
                    return required && !preferred && !forbidden;

                case EncounterCompatibilityTagRole.Preferred:
                    return !required && preferred && !forbidden;

                default:
                    return !required && !preferred && forbidden;
            }
        }

        private static bool HasAnyRole(EncounterDefinition definition, RouteTagDefinition tag)
        {
            return ContainsTag(definition.RequiredRouteTags, tag) ||
                   ContainsTag(definition.PreferredRouteTags, tag) ||
                   ContainsTag(definition.ForbiddenRouteTags, tag);
        }

        private static bool ContainsTag(System.Collections.Generic.IReadOnlyList<RouteTagDefinition> tags, RouteTagDefinition target)
        {
            foreach (RouteTagDefinition tag in tags)
            {
                if (tag == target)
                    return true;
            }

            return false;
        }

        private static void AppendReference(SerializedProperty property, RouteTagDefinition tag)
        {
            property.arraySize++;
            property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = tag;
        }

        private static void RemoveReference(SerializedProperty property, RouteTagDefinition tag)
        {
            for (int i = property.arraySize - 1; i >= 0; i--)
            {
                if (property.GetArrayElementAtIndex(i).objectReferenceValue != tag)
                    continue;

                for (int j = i; j < property.arraySize - 1; j++)
                    property.GetArrayElementAtIndex(j).objectReferenceValue = property.GetArrayElementAtIndex(j + 1).objectReferenceValue;

                property.arraySize--;
            }
        }
    }
}