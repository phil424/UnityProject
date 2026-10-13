# Author Character Mastery

**Status: Current**

# Purpose

Use the MiniCrawler Content Workbench to create, configure and spatially arrange
a playable Character's persistent Mastery progression.

The Workbench edits the real project ScriptableObjects.

It does not maintain a second progression graph or editor-only content database.

# Open

In Unity:

Tools
→ MiniCrawler
→ Content Workbench.

Select the Character from the left-hand Character browser.

Choose:

Mastery.

# Shared Character Context

Character and Mastery are workspaces within the same Content Workbench.

Selecting Ranger and moving from:

Character
→ Mastery

continues editing Ranger.

Selecting Punchy while Mastery is active immediately opens Punchy's Mastery
graph.

# Quick Start

1. Open the Content Workbench.
2. Select the playable Character.
3. Open the `Mastery` workspace.
4. Review the current validation state.
5. Click `+ Node`.
6. Choose the Mastery-node type.
7. author its Display Name and stable ID.
8. create the node.
9. configure the selected node in the right-hand panel.
10. author Costs and Prerequisites.
11. drag the node into the intended tree position.
12. resolve any validation issues.
13. press the Workbench `Save All` action.
14. validate the result in Preparation → Progression.

# Creating Nodes

Use:

`+ Node`

or right-click empty graph space.

Current creation types:

- Stat;
- Ability Level;
- Weapon Mastery Stance;
- Weapon Mastery Level;
- Mastery Combination.

Creation is identity-first.

The Display Name and stable ID are supplied before the ScriptableObject asset is
created.

The Workbench therefore creates a meaningful asset filename immediately.

The new node is:

- stored beside the Character's Mastery Definition;
- automatically registered in that Definition;
- placed on the graph;
- selected for immediate editing.

# Editing Nodes

Select a node on the graph.

The right-hand editor exposes:

- Display Name;
- Description;
- Icon;
- node-type-specific gameplay data;
- Prerequisites;
- Costs;
- Tree Position;
- generated Effect Preview;
- related-content navigation.

The stable internal ID remains available through Advanced fields.

# Prerequisites

Prerequisites are the gameplay-authoritative graph.

Editing a node's `Prerequisites` updates the visible graph connection.

There is no separate visual connection database.

# Spatial Layout

Drag nodes directly around the bounded graph.

Node movement writes:

`CharacterMasteryNodeDefinition.TreePosition`.

Use Unity Undo / Ctrl+Z to revert accidental movement.

`Center` recentres the authoring canvas.

Canvas Size belongs to the selected Character's
`CharacterMasteryDefinition`.

# Tree Membership

Mastery-node assets and Mastery-tree membership are separate concepts.

A node may exist as a ScriptableObject asset without currently belonging to the
selected Character's Mastery tree.

## Add Existing Node

In the right-hand Tree panel, assign an existing
`CharacterMasteryNodeDefinition` to:

`Existing Node`

then choose:

`Add Existing Node`.

The asset is added to the selected Character's
`CharacterMasteryDefinition`.

Its existing authored identity, gameplay fields, prerequisites, costs and tree
position are preserved.

## Remove From Tree

Select a node and use:

`Content Actions`
→ `Remove From Tree`.

This removes the node from the selected Character's Mastery Definition but does
not delete its ScriptableObject asset.

Prerequisite references to that node inside the current tree are removed.

If the node is a Weapon Mastery stance used by a Mastery Combination, that direct
combination reference is cleared.

The surviving combination is not silently deleted.

Validation should then be used to decide whether the combination should be
repaired or removed.

Removal is Undo-compatible.

# Deleting Nodes

Select a node and use:

`Content Actions`
→ `Delete Node Asset...`.

This is a deliberately destructive operation.

The Workbench:

1. removes the node from the current Mastery tree;
2. cleans in-tree prerequisite references;
3. clears direct Mastery-combination references;
4. saves those reference changes;
5. permanently deletes the node ScriptableObject asset.

The deletion requires explicit confirmation.

Permanent asset deletion is not treated as an Undo operation.

Use `Remove From Tree` instead when content may be reused or redesigned later.

# Missing Reference Cleanup

When the selected Mastery contains broken/null node or prerequisite references,
the Tree panel exposes:

`Clean Missing Reference(s)`.

This is an explicit repair operation.

The Workbench does not silently mutate authored collections during ordinary
validation.

Only broken/null references are removed.

Valid Mastery content is preserved.

# Related Content

Relevant Mastery nodes expose navigation to referenced content.

Examples:

Ability Level
→ Ability definition.

Weapon Mastery Stance / Level
→ Weapon Mastery definition.

Combination
→ active and inherited Mastery nodes.

Selecting a related Mastery node also synchronises the Content Workbench graph
selection.

# Unity Project Selection

Selecting a Mastery node asset in the Project window may automatically:

1. identify the playable Character that owns the node;
2. select that Character in the Content Workbench;
3. switch to Mastery;
4. select the matching graph node.

Selecting a Character Mastery Definition similarly resolves its owning
Character.

# Validation

Healthy content displays:

`Validation: Healthy`.

Validation is non-destructive.

Current checks include:

- missing references;
- duplicate node references;
- duplicate IDs;
- incomplete configuration;
- invalid prerequisites;
- self prerequisites;
- duplicate prerequisites;
- overlap;
- out-of-bounds positions;
- cycles;
- missing roots;
- invalid combination references;
- pairwise combination conflicts.

Fix the authored content rather than expecting automatic repair.

# Saving

Use:

`Save All`

in the main Content Workbench toolbar.

Unity's normal asset dirty state, Undo and GUID behaviour remain authoritative.

# Manual Escape Hatch

All generated Mastery content remains normally editable through Unity's Project
window and Inspector.

Normal creation, tree membership, node removal, missing-reference cleanup and
node deletion should use the Content Workbench.

Use the manual Project / Inspector path for:

- exact diagnosis;
- unusual repairs;
- advanced related-content editing;
- situations not represented by the normal Workbench workflow.

# Runtime Validation

After meaningful Mastery changes:

1. enter Play Mode;
2. select the Character in Preparation;
3. open Progression;
4. inspect the spatial tree;
5. verify prerequisite connections;
6. inspect locked / available / unlocked states;
7. purchase an eligible node where appropriate;
8. start an Expedition when the node affects prepared gameplay;
9. verify the gameplay effect.

# Related Documentation

- `Design/Progression.md`
- `Design/Technical/Character Mastery.md`
- `Design/UI-UX.md`
- `Development Rules/Unity Authoring.md`