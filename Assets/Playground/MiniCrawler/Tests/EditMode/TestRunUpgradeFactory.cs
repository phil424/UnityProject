using System.Reflection;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    internal static class TestRunUpgradeFactory
    {
        public static RunUpgradeDefinition Create(
            string id,
            RunUpgradeEffectType effectType,
            float amount,
            RunUpgradeRarity rarity =
                RunUpgradeRarity.Common
        )
        {
            RunUpgradeDefinition definition =
                ScriptableObject.CreateInstance<RunUpgradeDefinition>();

            definition.name = id;

            SetPrivateField(
                definition,
                "id",
                id
            );

            SetPrivateField(
                definition,
                "effectType",
                effectType
            );

            SetPrivateField(
                definition,
                "amount",
                amount
            );

            SetPrivateField(
                definition,
                "rarity",
                rarity
            );

            return definition;
        }

        private static void SetPrivateField<T>(
            RunUpgradeDefinition definition,
            string fieldName,
            T value
        )
        {
            FieldInfo field =
                typeof(RunUpgradeDefinition).GetField(
                    fieldName,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic
                );

            Assert.That(
                field,
                Is.Not.Null,
                $"Could not find field '{fieldName}'."
            );

            field.SetValue(
                definition,
                value
            );
        }
    }
}