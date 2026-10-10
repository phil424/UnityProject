using System.Collections.Generic;
using MiniCrawler.Abilities;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.ContentAuthoring
{
    public sealed class GameContentWorkbenchWindow : EditorWindow
    {
        private enum Workspace
        {
            Character,
            Mastery,
            RunRewards,
            Abilities,
        }

        private const float SidebarWidth = 220f;

        [SerializeField]
        private PartyMemberDefinition character;

        [SerializeField]
        private Workspace workspace;

        [SerializeField]
        private Vector2 sidebarScroll;

        [SerializeField]
        private Vector2 contentScroll;

        [SerializeField]
        private bool showAdvanced;

        private readonly List<GameContentAuthoringIssue> issues = new();

        private PlayableCharacterRosterDefinition roster;
        private GameContentMasteryWorkspace masteryWorkspace;
        private GameContentRunRewardsWorkspace runRewardsWorkspace;
        private GameContentAbilitiesWorkspace abilitiesWorkspace;

        [MenuItem("Tools/MiniCrawler/Content Workbench")]
        public static void Open()
        {
            GameContentWorkbenchWindow window = GetWindow<GameContentWorkbenchWindow>("MiniCrawler Content");

            window.ResolveRoster();
            window.EnsureMasteryWorkspace();
            window.EnsureRunRewardsWorkspace();
            window.EnsureAbilitiesWorkspace();

            window.AdoptSelection();
            window.EnsureCharacterSelection();

            window.SyncCharacterContext();
            window.Show();
        }

        private void OnEnable()
        {
            ResolveRoster();

            EnsureMasteryWorkspace();
            EnsureRunRewardsWorkspace();
            EnsureAbilitiesWorkspace();

            AdoptSelection();
            EnsureCharacterSelection();

            SyncCharacterContext();

            Undo.undoRedoPerformed += HandleUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= HandleUndoRedo;
        }

        private void OnSelectionChange()
        {
            EnsureMasteryWorkspace();
            EnsureRunRewardsWorkspace();
            EnsureAbilitiesWorkspace();

            if (Selection.activeObject is PartyMemberDefinition selectedCharacter)
            {
                SetCharacter(selectedCharacter);
                Repaint();
                return;
            }

            if (Selection.activeObject is CharacterMasteryNodeDefinition node && TryFindCharacter(node, out PartyMemberDefinition nodeOwner))
            {
                SetCharacter(nodeOwner);
                workspace = Workspace.Mastery;

                masteryWorkspace.SelectNode(node);

                Repaint();
                return;
            }

            if (Selection.activeObject is CharacterMasteryDefinition mastery && TryFindCharacter(mastery, out PartyMemberDefinition masteryOwner))
            {
                SetCharacter(masteryOwner);
                workspace = Workspace.Mastery;

                Repaint();
                return;
            }

            if (Selection.activeObject is AbilityEvolutionDefinition evolution)
            {
                workspace = Workspace.Abilities;

                abilitiesWorkspace.SelectEvolution(evolution);

                Repaint();
                return;
            }

            if (Selection.activeObject is AbilityDefinition ability)
            {
                workspace = Workspace.Abilities;

                abilitiesWorkspace.SelectAbility(ability);

                Repaint();
                return;
            }

            if (Selection.activeObject is GameObject selectedObject && abilitiesWorkspace.TrySelectRuntimePrefab(selectedObject))
            {
                workspace = Workspace.Abilities;

                Repaint();
                return;
            }

            if (Selection.activeObject is RunRewardCatalogDefinition catalog)
            {
                workspace = Workspace.RunRewards;

                runRewardsWorkspace.SelectCatalog(catalog);

                Repaint();
                return;
            }

            if (Selection.activeObject is RunRewardSetDefinition rewardSet)
            {
                workspace = Workspace.RunRewards;

                runRewardsWorkspace.SelectSet(rewardSet);

                Repaint();
                return;
            }

            if (Selection.activeObject is RunRewardDefinition reward)
            {
                workspace = Workspace.RunRewards;

                runRewardsWorkspace.SelectReward(reward);

                Repaint();
                return;
            }

            Repaint();
        }

        private void OnProjectChange()
        {
            ResolveRoster();
            EnsureCharacterSelection();

            EnsureMasteryWorkspace();
            EnsureRunRewardsWorkspace();

            SyncCharacterContext();

            masteryWorkspace.HandleProjectChange();
            runRewardsWorkspace.HandleProjectChange();
            abilitiesWorkspace.HandleProjectChange();

            Repaint();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (roster == null)
            {
                EditorGUILayout.HelpBox("Playable Character Roster could not be found.", MessageType.Error);

                return;
            }

            DrawWorkspaceTabs();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSidebar();
                DrawActiveWorkspace();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("MiniCrawler Content Workbench", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save All", EditorStyles.toolbarButton, GUILayout.Width(70f)))
                {
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(65f)))
                {
                    ResolveRoster();
                    EnsureCharacterSelection();

                    SyncCharacterContext();

                    masteryWorkspace.HandleProjectChange();
                    runRewardsWorkspace.HandleProjectChange();
                    abilitiesWorkspace.HandleProjectChange();

                    Repaint();
                }
            }
        }

        private void DrawWorkspaceTabs()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                DrawWorkspaceTab(Workspace.Character, "Character", 95f);

                string masteryLabel = character?.MasteryDefinition != null ? $"Mastery ({character.MasteryDefinition.Nodes.Count})" : "Mastery";

                DrawWorkspaceTab(Workspace.Mastery, masteryLabel, 110f);

                string rewardLabel = runRewardsWorkspace?.Catalog != null ? $"Run Rewards ({runRewardsWorkspace.UniqueRewardCount})" : "Run Rewards";

                DrawWorkspaceTab(Workspace.RunRewards, rewardLabel, 135f);

                string abilityLabel = abilitiesWorkspace != null ? $"Abilities ({abilitiesWorkspace.AbilityCount})" : "Abilities";

                DrawWorkspaceTab(Workspace.Abilities, abilityLabel, 115f);

                GUILayout.FlexibleSpace();

                if (character != null)
                {
                    GUILayout.Label($"Preview Character: {character.DisplayName}", EditorStyles.miniLabel);
                }
            }
        }

        private void DrawWorkspaceTab(Workspace target, string label, float width)
        {
            bool selected = workspace == target;

            bool nextSelected = GUILayout.Toggle(selected, label, EditorStyles.toolbarButton, GUILayout.Width(width));

            if (selected || !nextSelected)
                return;

            workspace = target;

            if (workspace == Workspace.Mastery)
                masteryWorkspace.SetCharacter(character);

            if (workspace == Workspace.RunRewards)
                runRewardsWorkspace.ResolveCatalog();

            if (workspace == Workspace.Abilities)
                abilitiesWorkspace.ResolveContent();
        }

        private void DrawSidebar()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(SidebarWidth), GUILayout.ExpandHeight(true)))
            {
                EditorGUILayout.LabelField("Characters", EditorStyles.boldLabel);

                if (GUILayout.Button("+ New Character"))
                {
                    GameContentCharacterCreateWindow.Open();
                }

                EditorGUILayout.Space(6);

                sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);

                foreach (PlayableCharacterRosterEntry entry in roster.Characters)
                {
                    PartyMemberDefinition candidate = entry?.Character;

                    if (candidate == null)
                        continue;

                    int requiredIssues = CountRequiredIssues(candidate);

                    string label = requiredIssues > 0 ? $"{candidate.DisplayName}  [{requiredIssues}]" : candidate.DisplayName;

                    GUIStyle style = candidate == character ? EditorStyles.miniButtonMid : EditorStyles.miniButton;

                    if (GUILayout.Button(label, style))
                        SetCharacter(candidate);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                if (workspace == Workspace.RunRewards)
                {
                    EditorGUILayout.HelpBox("The selected Character is used as the Reward eligibility preview context.", MessageType.None);
                }
                else if (workspace == Workspace.Abilities)
                {
                    EditorGUILayout.HelpBox("The selected Character is used as Ability ownership / core-role context.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox("A number beside a Character indicates required authoring items are still missing.", MessageType.None);
                }
            }
        }

        private void DrawActiveWorkspace()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                switch (workspace)
                {
                    case Workspace.Character:
                        if (character == null)
                        {
                            DrawMissingCharacterMessage();
                            return;
                        }

                        DrawCharacterWorkspace();
                        break;

                    case Workspace.Mastery:
                        if (character == null)
                        {
                            DrawMissingCharacterMessage();
                            return;
                        }

                        masteryWorkspace.Draw();
                        break;

                    case Workspace.RunRewards:
                        runRewardsWorkspace.Draw();
                        break;

                    case Workspace.Abilities:
                        abilitiesWorkspace.Draw();
                        break;
                }
            }
        }

        private void DrawCharacterWorkspace()
        {
            GameContentCharacterValidation.Collect(character, roster, issues);

            contentScroll = EditorGUILayout.BeginScrollView(contentScroll);

            GameContentCharacterInspector.Draw(character, roster, issues, ref showAdvanced, OpenMasteryWorkspace);

            EditorGUILayout.EndScrollView();
        }

        private void DrawMissingCharacterMessage()
        {
            EditorGUILayout.HelpBox("Select a Character or create a new one.", MessageType.Info);
        }

        private void OpenMasteryWorkspace()
        {
            workspace = Workspace.Mastery;
            masteryWorkspace.SetCharacter(character);
        }

        private void SetCharacter(PartyMemberDefinition nextCharacter)
        {
            if (character == nextCharacter)
                return;

            character = nextCharacter;
            contentScroll = Vector2.zero;
            showAdvanced = false;

            SyncCharacterContext();

            Selection.activeObject = character;

            Repaint();
        }

        private int CountRequiredIssues(PartyMemberDefinition candidate)
        {
            List<GameContentAuthoringIssue> candidateIssues = new();

            GameContentCharacterValidation.Collect(candidate, roster, candidateIssues);

            return GameContentCharacterValidation.CountRequired(candidateIssues);
        }

        private bool TryFindCharacter(CharacterMasteryNodeDefinition node, out PartyMemberDefinition owner)
        {
            owner = null;

            if (roster == null || node == null)
                return false;

            foreach (PlayableCharacterRosterEntry entry in roster.Characters)
            {
                PartyMemberDefinition candidate = entry?.Character;

                if (candidate?.MasteryDefinition == null)
                    continue;

                if (!candidate.MasteryDefinition.Contains(node))
                    continue;

                owner = candidate;
                return true;
            }

            return false;
        }

        private bool TryFindCharacter(CharacterMasteryDefinition mastery, out PartyMemberDefinition owner)
        {
            owner = null;

            if (roster == null || mastery == null)
                return false;

            foreach (PlayableCharacterRosterEntry entry in roster.Characters)
            {
                PartyMemberDefinition candidate = entry?.Character;

                if (candidate?.MasteryDefinition != mastery)
                    continue;

                owner = candidate;
                return true;
            }

            return false;
        }

        private void ResolveRoster()
        {
            roster = GameContentRosterUtility.FindRoster();
        }

        private void AdoptSelection()
        {
            if (Selection.activeObject is PartyMemberDefinition selected)
            {
                character = selected;
                return;
            }

            if (Selection.activeObject is CharacterMasteryNodeDefinition node && TryFindCharacter(node, out PartyMemberDefinition nodeOwner))
            {
                character = nodeOwner;
                workspace = Workspace.Mastery;

                EnsureMasteryWorkspace();
                masteryWorkspace.SetCharacter(character);
                masteryWorkspace.SelectNode(node);

                return;
            }

            if (Selection.activeObject is CharacterMasteryDefinition mastery && TryFindCharacter(mastery, out PartyMemberDefinition masteryOwner))
            {
                character = masteryOwner;
                workspace = Workspace.Mastery;
                return;
            }

            EnsureAbilitiesWorkspace();

            if (Selection.activeObject is AbilityEvolutionDefinition evolution)
            {
                workspace = Workspace.Abilities;
                abilitiesWorkspace.SelectEvolution(evolution);
                return;
            }

            if (Selection.activeObject is AbilityDefinition ability)
            {
                workspace = Workspace.Abilities;
                abilitiesWorkspace.SelectAbility(ability);
                return;
            }

            if (Selection.activeObject is GameObject selectedObject && abilitiesWorkspace.TrySelectRuntimePrefab(selectedObject))
            {
                workspace = Workspace.Abilities;
                return;
            }

            EnsureRunRewardsWorkspace();

            if (Selection.activeObject is RunRewardCatalogDefinition catalog)
            {
                workspace = Workspace.RunRewards;
                runRewardsWorkspace.SelectCatalog(catalog);
                return;
            }

            if (Selection.activeObject is RunRewardSetDefinition rewardSet)
            {
                workspace = Workspace.RunRewards;
                runRewardsWorkspace.SelectSet(rewardSet);
                return;
            }

            if (Selection.activeObject is RunRewardDefinition reward)
            {
                workspace = Workspace.RunRewards;
                runRewardsWorkspace.SelectReward(reward);
            }
        }

        private void EnsureCharacterSelection()
        {
            if (character != null || roster == null)
                return;

            foreach (PlayableCharacterRosterEntry entry in roster.Characters)
            {
                if (entry?.Character == null)
                    continue;

                character = entry.Character;
                return;
            }
        }

        private void EnsureMasteryWorkspace()
        {
            masteryWorkspace ??= new GameContentMasteryWorkspace();
        }

        private void EnsureRunRewardsWorkspace()
        {
            runRewardsWorkspace ??= new GameContentRunRewardsWorkspace();

            runRewardsWorkspace.ResolveCatalog();
        }

        private void EnsureAbilitiesWorkspace()
        {
            abilitiesWorkspace ??= new GameContentAbilitiesWorkspace();

            abilitiesWorkspace.SetContext(character, roster);

            abilitiesWorkspace.ResolveContent();
        }

        private void SyncCharacterContext()
        {
            EnsureMasteryWorkspace();
            EnsureRunRewardsWorkspace();
            EnsureAbilitiesWorkspace();

            masteryWorkspace.SetCharacter(character);
            runRewardsWorkspace.SetCharacter(character);
            abilitiesWorkspace.SetContext(character, roster);
        }

        private void HandleUndoRedo()
        {
            masteryWorkspace?.HandleUndoRedo();
            runRewardsWorkspace?.HandleUndoRedo();
            abilitiesWorkspace?.HandleUndoRedo();

            Repaint();
        }
    }
}
