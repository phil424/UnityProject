using System.Collections.Generic;
using MiniCrawler.Abilities;
using UnityEditor;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal readonly struct GameContentAbilityLevelPreview
    {
        public GameContentAbilityLevelPreview(string label, float levelOne, float maximumLevel)
        {
            Label = label;
            LevelOne = levelOne;
            MaximumLevel = maximumLevel;
        }

        public string Label { get; }
        public float LevelOne { get; }
        public float MaximumLevel { get; }
    }

    internal static class GameContentAbilityPreviewUtility
    {
        public static void Collect(AbilityDefinition ability, List<GameContentAbilityLevelPreview> output)
        {
            output.Clear();

            if (ability == null)
                return;

            output.Add(new GameContentAbilityLevelPreview("Cooldown", ability.GetCooldown(1), ability.GetCooldown(ability.MaxLevel)));

            ActorAbility runtime = GameContentAbilityUtility.GetRuntimeBehaviour(ability);

            if (runtime == null)
                return;

            SerializedObject serialized = new(runtime);

            serialized.Update();

            SerializedProperty iterator = serialized.GetIterator();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.name == "m_Script")
                    continue;

                SerializedProperty baseValue = iterator.FindPropertyRelative("baseValue");

                SerializedProperty perLevel = iterator.FindPropertyRelative("perLevel");

                if (baseValue == null || perLevel == null || baseValue.propertyType != SerializedPropertyType.Float || perLevel.propertyType != SerializedPropertyType.Float)
                {
                    continue;
                }

                float levelOne = baseValue.floatValue;

                float maximum = levelOne + perLevel.floatValue * (ability.MaxLevel - 1);

                output.Add(new GameContentAbilityLevelPreview(ObjectNames.NicifyVariableName(iterator.name), levelOne, maximum));
            }
        }
    }
}
