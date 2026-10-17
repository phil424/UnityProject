using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    internal sealed class GameContentAbilitiesWorkspace
    {
        private const float LibraryWidth = 285f;

        private readonly List<AbilityDefinition> abilities = new();

        private readonly List<AbilityEvolutionDefinition> allEvolutions = new();

        private readonly List<AbilityEvolutionDefinition> selectedEvolutions = new();

        private readonly List<string> validationIssues = new();

        private PartyMemberDefinition character;
        private PlayableCharacterRosterDefinition roster;
        private RunRewardCatalogDefinition catalog;

        private AbilityDefinition selectedAbility;
        private AbilityEvolutionDefinition selectedEvolution;

        private Vector2 libraryScroll;
        private Vector2 inspectorScroll;

        private string search;
        private bool showIssues;
        private bool showAdvancedAbility;
        private bool showAdvancedEvolution;

        public int AbilityCount => abilities.Count;

        public void SetContext(PartyMemberDefinition nextCharacter, PlayableCharacterRosterDefinition nextRoster)
        {
            character = nextCharacter;
            roster = nextRoster;

            if (catalog == null)
            {
                catalog = GameContentRewardUtility.FindCatalog();
            }
        }

        public void ResolveContent()
        {
            GameContentAbilityUtility.CollectAbilities(abilities);

            GameContentAbilityUtility.CollectAllEvolutions(allEvolutions);

            catalog = GameContentRewardUtility.FindCatalog();

            EnsureSelection();
            RefreshSelectedEvolutions();
            RefreshValidation();
        }

        public void HandleProjectChange()
        {
            ResolveContent();
        }

        public void HandleUndoRedo()
        {
            ResolveContent();
        }

        public void SelectAbility(AbilityDefinition ability)
        {
            if (ability == null)
                return;

            selectedAbility = ability;
            selectedEvolution = null;

            RefreshSelectedEvolutions();
        }

        public void SelectEvolution(AbilityEvolutionDefinition evolution)
        {
            if (evolution == null)
                return;

            selectedAbility = evolution.TargetAbility;

            selectedEvolution = evolution;

            RefreshSelectedEvolutions();
        }

        public bool TrySelectRuntimePrefab(GameObject runtimePrefab)
        {
            AbilityDefinition ability = GameContentAbilityUtility.FindAbilityForRuntimePrefab(runtimePrefab);

            if (ability == null)
                return false;

            SelectAbility(ability);
            return true;
        }

        public void Draw()
        {
            if (abilities.Count == 0)
                ResolveContent();

            DrawHeader();
            DrawValidation();

            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawLibrary();
                DrawInspector();
            }
        }

        private void DrawHeader()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel, GUILayout.Width(70f));

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField($"Abilities {abilities.Count}   " + $"Evolutions {allEvolutions.Count}", EditorStyles.miniLabel, GUILayout.Width(155f));

                if (GUILayout.Button("+ Ability", GUILayout.Width(75f)))
                {
                    GameContentAbilityCreateWindow.Open(character);
                }

                if (GUILayout.Button("Validate", GUILayout.Width(65f)))
                {
                    RefreshValidation();
                }
            }
        }

        private void DrawValidation()
        {
            if (validationIssues.Count == 0)
            {
                EditorGUILayout.LabelField("Validation: Healthy", EditorStyles.miniLabel);

                return;
            }

            EditorGUILayout.HelpBox($"Ability content has " + $"{validationIssues.Count} authoring issue(s).", MessageType.Warning);

            showIssues = EditorGUILayout.Foldout(showIssues, "Issues", true);

            if (!showIssues)
                return;

            foreach (string issue in validationIssues)
            {
                EditorGUILayout.LabelField($"- {issue}", EditorStyles.wordWrappedMiniLabel);
            }
        }

        private void DrawLibrary()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(LibraryWidth), GUILayout.ExpandHeight(true)))
            {
                EditorGUILayout.LabelField("Ability Library", EditorStyles.boldLabel);

                search = EditorGUILayout.TextField("Search", search);

                libraryScroll = EditorGUILayout.BeginScrollView(libraryScroll);

                foreach (AbilityDefinition ability in abilities)
                {
                    if (!MatchesSearch(ability))
                        continue;

                    DrawAbilityRow(ability);
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(6);

                if (GUILayout.Button("+ New Ability"))
                {
                    GameContentAbilityCreateWindow.Open(character);
                }
            }
        }

        private void DrawAbilityRow(AbilityDefinition ability)
        {
            AbilityLoadoutEntry coreUsage = null;

            GameContentAbilityUtility.TryGetCoreUsage(character, ability, out coreUsage);

            Color color = coreUsage != null ? GameContentAuthoringPalette.GetAbilityRoleColor(coreUsage.Role) : GameContentAuthoringPalette.Ability;

            Rect row = GUILayoutUtility.GetRect(0f, 48f, GUILayout.ExpandWidth(true));

            GUIStyle style = ability == selectedAbility ? EditorStyles.helpBox : GUI.skin.button;

            if (GUI.Button(row, GUIContent.none, style))
            {
                SelectAbility(ability);
                Selection.activeObject = ability;
            }

            Rect accent = new(row.x + 3f, row.y + 3f, 5f, row.height - 6f);

            EditorGUI.DrawRect(accent, color);

            Rect title = new(row.x + 13f, row.y + 5f, row.width - 18f, 20f);

            Rect detail = new(row.x + 13f, row.y + 25f, row.width - 18f, 18f);

            GUI.Label(title, ability.DisplayName, EditorStyles.boldLabel);

            string elementLabel = ability.Elements == AbilityElement.None ? "Neutral" : ability.Elements.ToString().Replace(", ", " / ");

            string usageLabel = coreUsage != null ? $"{coreUsage.Role} • Starting Lv.{coreUsage.Level}" : GameContentAbilityUtility.GetRuntimeBehaviourLabel(ability);

            string detailText = $"{elementLabel} • {usageLabel}";

            GUI.Label(detail, detailText, EditorStyles.miniLabel);
        }

        private void DrawInspector()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

                bool changed = GameContentAbilityInspector.DrawAbility(selectedAbility, character, roster, catalog, ref showAdvancedAbility);

                if (changed)
                {
                    RefreshValidation();
                    RefreshSelectedEvolutions();
                }

                DrawEvolutions();

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawEvolutions()
        {
            if (selectedAbility == null)
                return;

            EditorGUILayout.Space(14);

            EditorGUILayout.LabelField("Evolutions", EditorStyles.boldLabel);

            if (selectedEvolutions.Count == 0)
            {
                EditorGUILayout.HelpBox("This Ability currently has no authored Evolutions.", MessageType.Info);
            }
            else
            {
                foreach (AbilityEvolutionDefinition evolution in selectedEvolutions)
                {
                    GUIStyle style = evolution == selectedEvolution ? EditorStyles.miniButtonMid : EditorStyles.miniButton;

                    if (GUILayout.Button(evolution.DisplayName, style))
                    {
                        selectedEvolution = evolution;

                        Selection.activeObject = evolution;
                    }
                }
            }

            if (GUILayout.Button("+ New Evolution"))
            {
                GameContentAbilityEvolutionCreateWindow.Open(selectedAbility);
            }

            if (selectedEvolution == null)
                return;

            EditorGUILayout.Space(10);

            bool changed = GameContentAbilityInspector.DrawEvolution(selectedEvolution, catalog, ref showAdvancedEvolution);

            if (changed)
            {
                RefreshSelectedEvolutions();
                RefreshValidation();
            }
        }

        private bool MatchesSearch(AbilityDefinition ability)
        {
            if (string.IsNullOrWhiteSpace(search))
                return true;

            return ability.DisplayName.Contains(search, System.StringComparison.OrdinalIgnoreCase) || ability.Id.Contains(search, System.StringComparison.OrdinalIgnoreCase);
        }

        private void EnsureSelection()
        {
            if (selectedAbility != null && abilities.Contains(selectedAbility))
            {
                return;
            }

            selectedAbility = abilities.Count > 0 ? abilities[0] : null;

            selectedEvolution = null;
        }

        private void RefreshSelectedEvolutions()
        {
            GameContentAbilityUtility.CollectEvolutions(selectedAbility, selectedEvolutions);

            if (selectedEvolution != null && !selectedEvolutions.Contains(selectedEvolution))
            {
                selectedEvolution = null;
            }
        }

        private void RefreshValidation()
        {
            GameContentAbilityValidation.Collect(abilities, allEvolutions, validationIssues);
        }
    }
}
