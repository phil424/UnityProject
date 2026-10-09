# Author Run Rewards

**Status: Current**

# Purpose

Use the MiniCrawler Content Workbench to create, edit, organise and preview Run
Reward content.

The Workbench edits the real Reward ScriptableObjects.

It does not maintain a second editor-only Reward database.

# Open

Tools
→ MiniCrawler
→ Content Workbench
→ Run Rewards.

# Current Structure

Default Run Reward Catalogue
↓
Reward Sets
↓
Reward Definitions.

Reward Sets organise content.

Reward Definitions remain responsible for actual eligibility and application.

# Quick Start

1. Open the Content Workbench.
2. Select `Run Rewards`.
3. Select or create a Reward Set.
4. Click `+ New Reward`.
5. choose the Reward type.
6. author Display Name and stable ID.
7. create the Reward.
8. configure its type-specific gameplay fields.
9. review the player-facing preview.
10. select Characters in the sidebar to inspect eligibility.
11. resolve validation warnings.
12. use `Save All`.
13. validate the Reward during a real Expedition.

# Reward Sets

The left Reward column shows the Sets included by the active Catalogue.

Use:

`+ New Reward Set`

to create a new Set and automatically add it to the Catalogue.

Use:

`Add Existing Set`

to reuse an existing Set.

Removing a Set from the Catalogue does not delete the Set asset or its Rewards.

# Creating Rewards

Select a Reward Set.

Click:

`+ New Reward`.

Current supported authoring types:

- Character Growth;
- Ability Unlock;
- Ability Evolution;
- Weapon Mastery Level.

Creation is identity-first.

Supply:

- Display Name;
- stable ID.

The Reward asset is then created with a meaningful filename and automatically
added to the selected Set.

# Character Growth Rewards

Configure:

- Rarity;
- Character Stat Modifier;
- Amount.

The player-facing description is generated from the existing stat-presentation
logic.

# Ability Unlock Rewards

Configure:

- Rarity;
- Ability;
- optional eligible Characters.

The related-content action can navigate directly to the Ability definition.

# Ability Evolution Rewards

Configure:

- Rarity;
- Ability Evolution definition.

The Reward derives its presentation from the configured Evolution where
appropriate.

# Weapon Mastery Level Rewards

Configure:

- Rarity;
- Weapon Mastery;
- Expedition level increase.

The Reward remains eligible only for Characters/builds with the relevant
prepared Mastery.

# Player-Facing Preview

The selected Reward editor previews:

- rarity;
- Display Name;
- Description;
- icon;
- Reward Vector.

Rarity styling reuses runtime Reward presentation.

# Character Eligibility Preview

The Character selected in the Workbench sidebar is the preview context.

The Workbench checks:

Starting Build
and
Full Authored Mastery Build.

This preview does not simulate arbitrary earlier Run rewards.

Use real Expedition testing for final gameplay validation.

# Membership

A Reward may belong to more than one Reward Set.

The Catalogue deduplicates resolved Rewards.

The Workbench displays all current Set memberships for the selected Reward.

Removing a Reward from a Set does not delete the Reward asset.

# Asset Naming

Changing Display Name may make the asset filename stale.

When this occurs the Workbench offers:

`Sync Reward Asset File Name`.

This renames the Unity asset while preserving GUID references.

Stable gameplay IDs are not changed automatically.

# Validation

A healthy Catalogue displays:

`Validation: Healthy`.

Current checks include:

- missing Reward Set references;
- duplicate Set references;
- missing Reward references;
- duplicate Reward membership inside one Set;
- incomplete Reward configuration;
- missing stable IDs;
- duplicate Reward IDs.

Validation is non-destructive.

# Manual Escape Hatch

All Catalogue, Set and Reward assets remain ordinary Unity ScriptableObjects.

Use the normal Project window / Inspector for unusual repair work or manual asset
deletion.

# Runtime Validation

After meaningful Reward changes:

1. enter Play Mode;
2. start an Expedition with a relevant Character;
3. queue or earn a Reward;
4. verify Reward Vector filtering;
5. verify eligibility;
6. verify player-facing Reward presentation;
7. choose the Reward;
8. verify its `RunBuild` effect;
9. verify temporary progression resets at Expedition end where applicable.

# Related Documentation

- `Design/Technical/Rewards and Pending Choices.md`
- `Design/Progression.md`
- `Development Rules/Abilities and Buildcraft.md`