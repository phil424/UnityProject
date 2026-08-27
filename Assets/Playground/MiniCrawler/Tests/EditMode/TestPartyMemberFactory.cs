using System.Reflection;
using MiniCrawler.Core;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    internal static class TestPartyMemberFactory
    {
        public static PartyMemberDefinition Create(
            string id,
            int baseUpgradeCost = 5,
            int upgradeCostStep = 5
        )
        {
            PartyMemberDefinition definition =
                ScriptableObject.CreateInstance<PartyMemberDefinition>();

            definition.name = id;

            SetPrivateField(definition, "id", id);
            SetPrivateField(
                definition,
                "baseUpgradeCost",
                baseUpgradeCost
            );
            SetPrivateField(
                definition,
                "upgradeCostStep",
                upgradeCostStep
            );

            return definition;
        }

        private static void SetPrivateField<T>(
            PartyMemberDefinition definition,
            string fieldName,
            T value
        )
        {
            FieldInfo field =
                typeof(PartyMemberDefinition).GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

            Assert.That(
                field,
                Is.Not.Null,
                $"Could not find field '{fieldName}'."
            );

            field.SetValue(definition, value);
        }
    }
}