# Character Mastery

**Status: Implemented Spatial Foundation**

# Purpose

Character Mastery is the primary Character-specific persistent progression
surface.

It survives Expedition end and establishes prepared starting development.

Character Mastery currently supports:

- persistent stat development;
- persistent starting Ability development;
- Weapon Mastery stance unlocks;
- persistent Weapon Mastery level development.

# Data Ownership

Character Mastery content is owned by a focused:

`CharacterMasteryDefinition`.

Current ownership:

`PartyMemberDefinition`
↓
`CharacterMasteryDefinition`
↓
Mastery Nodes
+
Mastery Tree Size.

`PartyMemberDefinition`

therefore identifies which Mastery graph belongs to the Character without
directly owning the expanding node collection or its spatial canvas settings.

`CharacterMasteryDefinition`

owns:

- Character Mastery node membership;
- Character-specific spatial tree bounds.

`CharacterMasteryNodeDefinition`

owns:

- node identity;
- description;
- icon;
- tree position;
- prerequisites;
- persistent resource costs;
- concrete gameplay effect.

`PersistentMemberProgress`

continues to own only the player's unlocked Mastery-node IDs.

Persistent player state does not duplicate the Mastery graph.

This separation provides one coherent Character Mastery content source for the
runtime tree, persistent progression and designer authoring tools.

# Current Node Types

`CharacterMasteryStatNodeDefinition`

Applies persistent prepared Character-stat development.

`CharacterMasteryAbilityLevelNodeDefinition`

Raises the prepared starting level of an existing starting Ability.

`CharacterMasteryWeaponModNodeDefinition`

Prepares a Weapon Mastery stance in one of the four authored stance positions.

Current examples include:

- Punchy Heavy Finisher Core;
- Ranger Piercing Core;
- Ranger Impact Alternate1.

`CharacterMasteryWeaponModLevelNodeDefinition`

Permanently increases the prepared starting level of a Weapon Mastery.

Current example:

Ranger Deep Penetration
→ Piercing starting level +1.

`CharacterMasteryWeaponModCombinationNodeDefinition`

Unlocks a persistent designed relationship between two Weapon Masteries.

The node references the participating Weapon Mastery unlock nodes rather than
creating a second global combination inventory.

Its prepared-build effect records a directional combination on the active
Mastery's Run state.

Current proof:

Piercing Impact
→ active Mastery: Impact Arrows;
→ combined Mastery: Piercing Arrows.

Combination behaviour itself remains concrete/content-owned rather than being
implemented through a universal effect graph.

# Resource Costs

A Mastery node may require multiple persistent resource tiers.

Unlock validation is atomic.

All required resources must be available before any are spent.

# Prerequisites

Nodes reference other `CharacterMasteryNodeDefinition` assets as prerequisites.

A node cannot be purchased until every authored prerequisite has been unlocked
for that Character.

Prerequisites are the gameplay-authoritative progression graph.

# Prepared Runtime Flow

PersistentProgression
↓
unlocked Mastery-node state
↓
new Expedition
↓
RunState creates RunBuild
↓
starting Abilities are prepared
↓
`PersistentProgression.ApplyMasteryToBuild`
↓
prepared Character / Ability / Weapon Mastery development enters the RunBuild.

Temporary Expedition progression is applied separately.

# Prepared vs Expedition Development

Prepared
→ copied from persistent Character Mastery.

Expedition
→ earned during the current run.

Examples:

Persistent Piercing prepared level 1
+
temporary Piercing reward level 1
=
effective Piercing level 2 for that Expedition.

Next Expedition:

Piercing returns to its persistent prepared level 1.

# Spatial Presentation

Character Mastery uses a dedicated spatial Preparation overlay.

`CharacterMasteryDefinition.TreeSize`
→ Character-specific bounded canvas.

`CharacterMasteryNodeDefinition.TreePosition`
→ authored node position.

`CharacterMasteryNodeDefinition.Prerequisites`
→ sole gameplay-authoritative graph;
→ also generates runtime and Workbench connection lines.

Player navigation currently supports:

- bounded drag-to-pan;
- mouse-wheel pointer-focused zoom;
- optional middle-mouse pan;
- direct node selection.

Tree geometry is presentation only.

Gameplay progression remains defined by prerequisite data.

# Unified Content Workbench — Mastery Workspace

Normal Character Mastery authoring is embedded in:

Tools
→ MiniCrawler
→ Content Workbench
→ Mastery.

There is no separate persistent Character Mastery authoring window.

The shared Character selection is the context for both:

Character
and
Mastery

workspaces.

Selecting a different Character while the Mastery workspace is active swaps the
graph directly to that Character's `CharacterMasteryDefinition`.

The Mastery workspace operates around:

`PartyMemberDefinition`
↓
`CharacterMasteryDefinition`
↓
Mastery-node assets.

# Mastery Workspace

The normal Mastery surface provides:

- current Character context;
- Mastery Definition visibility;
- bounded spatial graph;
- prerequisite-derived connections;
- direct node selection;
- drag-to-position layout;
- Character-specific canvas editing;
- identity-first node creation;
- automatic node asset creation;
- automatic registration in the Mastery Definition;
- selected-node editing;
- prerequisite editing;
- persistent resource-cost editing;
- node-type-specific gameplay fields;
- direct navigation to related Ability / Weapon Mastery assets;
- Undo-compatible spatial editing;
- non-destructive graph/layout validation.

# Cross-Workspace Context

Character and Mastery authoring share one selected Character.

Character
→ Mastery

does not require reopening a tool or reselecting the Character.

Changing Character while Mastery is active immediately changes the authored
Mastery graph.

Selecting a `CharacterMasteryNodeDefinition` in Unity may also resolve its owning
playable Character and make that node active in the Content Workbench.

Selecting a `CharacterMasteryDefinition` similarly resolves its owning playable
Character and opens the Mastery workspace.

This keeps normal Unity Project-window navigation compatible with the central
authoring surface.

# Node Creation

Mastery nodes use identity-first creation.

The designer chooses:

- node type;
- Display Name;
- stable ID;

before the asset is created.

The asset therefore begins with a meaningful filename rather than a temporary
`New...` filename.

Supported node types currently include:

- Character Stat;
- Ability Level;
- Weapon Mastery Stance;
- Weapon Mastery Level;
- Weapon Mastery Combination.

New assets are:

1. created beside the Character's Mastery Definition;
2. given the authored identity;
3. automatically added to the Mastery Definition;
4. positioned on the graph;
5. selected for immediate editing.

# Progressive Disclosure

The selected-node editor displays common Mastery data plus only fields relevant
to the concrete node type.

Examples:

Stat
→ stat type + amount.

Ability Level
→ Ability + level increase.

Weapon Mastery Stance
→ Weapon Mod + stance slot.

Weapon Mastery Level
→ Weapon Mod + prepared level increase.

Combination
→ active Mastery + inherited Mastery.

This avoids turning the unified Workbench into a generic display of every
underlying runtime field.

# Validation

Mastery validation remains non-destructive.

Current checks include:

- missing Mastery-node references;
- duplicate node references;
- duplicate IDs;
- incomplete node configuration;
- invalid prerequisite membership;
- self prerequisites;
- duplicate prerequisites;
- node overlap;
- nodes outside authored bounds;
- prerequisite cycles;
- missing graph roots;
- invalid Mastery-combination membership;
- duplicate pairwise combination targets.

Validation reports problems without silently rewriting authored topology.

# Manual Escape Hatch

All underlying assets remain ordinary Unity ScriptableObjects.

The normal Inspector / Project-window path remains available for:

- diagnosis;
- direct Mastery Definition membership repair;
- deleting assets;
- unusual data repair;
- related content editing.

The Content Workbench is an authoring front end, not a parallel storage system.

# Current Limitations

Not yet implemented:

- direct drag-to-create prerequisite connections;
- safe Workbench-owned node deletion;
- node duplication;
- automatic graph layout;
- final Mastery-tree artwork;
- controller-specific runtime navigation;
- respec;
- save-file persistence;
- final progression balance.

These are not blockers for the current unified authoring foundation.