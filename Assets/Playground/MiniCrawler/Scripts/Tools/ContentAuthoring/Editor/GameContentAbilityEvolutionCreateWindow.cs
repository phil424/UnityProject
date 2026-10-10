using MiniCrawler.Abilities;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentAbilityEvolutionCreateWindow : EditorWindow
    {
        private AbilityDefinition ability;

        private string displayName;
        private string id;
        private string description;
        private bool idEdited;
        private bool replacesPresentation;

        public static void Open(AbilityDefinition ability)
        {
            if (ability == null)
                return;

            GameContentAbilityEvolutionCreateWindow window = CreateInstance<GameContentAbilityEvolutionCreateWindow>();

            window.ability = ability;
            window.titleContent = new GUIContent("New Ability Evolution");

            window.minSize = new Vector2(460f, 320f);

            window.maxSize = new Vector2(600f, 440f);

            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Ability Evolution", EditorStyles.largeLabel);

            EditorGUILayout.ObjectField("Ability", ability, typeof(AbilityDefinition), false);

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

            EditorGUILayout.LabelField("Description");

            description = EditorGUILayout.TextArea(description, GUILayout.MinHeight(60f));

            replacesPresentation = EditorGUILayout.Toggle("Replace Presentation", replacesPresentation);

            EditorGUILayout.Space(8);

            string error = GameContentAbilityFactory.ValidateEvolutionCreation(ability, displayName, id);

            if (!string.IsNullOrWhiteSpace(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"Asset:\n" + $"{GameContentAssetUtility.GetAbilityEvolutionFolder(ability)}/" + $"{GameContentAssetUtility.GetAbilityEvolutionAssetName(displayName)}.asset",
                    MessageType.None
                );

                EditorGUILayout.HelpBox(
                    "An Evolution definition describes progression and presentation. " + "Its concrete ActorAbility runtime must still explicitly implement or bind the gameplay change.",
                    MessageType.Info
                );
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!string.IsNullOrWhiteSpace(error)))
            {
                if (GUILayout.Button("Create Evolution", GUILayout.Height(32f)))
                {
                    AbilityEvolutionDefinition created = GameContentAbilityFactory.CreateEvolution(ability, displayName, id, description, replacesPresentation, out string createError);

                    if (created == null)
                    {
                        EditorUtility.DisplayDialog("Evolution Creation Failed", createError ?? "Unknown creation error.", "OK");

                        return;
                    }

                    Close();
                }
            }
        }
    }
}
