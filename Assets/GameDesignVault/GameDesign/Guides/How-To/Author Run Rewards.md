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

# Rarity Profile

The active Reward Catalogue owns the baseline Reward rarity weights.

Open:

Tools
→ MiniCrawler
→ Content Workbench
→ Run Rewards.

Expand:

`Rarity Profile`.

Current working baseline:

- Common: `60`;
- Uncommon: `25`;
- Rare: `10`;
- Epic: `4`;
- Legendary: `1`.

The Workbench shows the normalised baseline chance when all five tiers are
represented.

Weights are relative.

They do not need to total 100.

A value of `0` disables that rarity from selection.

Runtime generation only includes rarity tiers that actually have eligible
Rewards for the current Reward Vector / Character / build.

Therefore a Rare-only eligible pool still produces Rare Rewards even though
Rare has a lower normal baseline weight than Common.

Individual Reward rarity remains authored on the Reward Definition itself.

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

## Remove Set From Catalogue

`Remove Set From Catalogue`

removes only the membership between the selected Set and the active Catalogue.

The Reward Set asset remains.

Its Reward assets remain.

This operation is suitable when content may be reused or reactivated later and
is Undo-compatible.

## Delete Reward Set Asset

`Delete Reward Set Asset...`

is an explicitly destructive operation.

Before deletion the Workbench removes the Set from every
`RunRewardCatalogDefinition` found in the project.

The Reward Set ScriptableObject is then permanently deleted.

Rewards referenced by the Set are NOT deleted.

Permanent Set-asset deletion requires confirmation and is not treated as an
Undo operation.

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

## Character Growth Eligibility

Character Growth Rewards may optionally restrict which Characters can receive
them.

In the Reward inspector:

`Eligibility`
→ `Eligible Members`.

Leave the collection empty when the Reward is generic.

Example:

Combat Training
→ Eligible Members empty
→ any Character.

Expanded Guard
→ Eligible Members contains Ranger
→ Ranger only.

Use Character eligibility when a shared Character stat is not meaningful for
every current Character.

Do not create a separate Character-specific Reward type solely for this purpose.

Eligibility participates in the normal Reward candidate filtering before rarity
selection.

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

## Remove Selected From Set

This removes only the selected Reward / Set membership.

The Reward asset remains available for reuse.

The operation is Undo-compatible.

## Delete Reward Asset

`Delete Selected Reward Asset...`

is explicitly destructive.

Before deleting the Reward asset, the Workbench searches all project
`RunRewardSetDefinition` assets and removes every membership referencing that
Reward.

The Reward ScriptableObject is then permanently deleted.

The confirmation dialog reports how many Set memberships will be removed.

Permanent Reward-asset deletion is not treated as an Undo operation.

Use membership removal rather than asset deletion when content may be reused or
redesigned later.

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

Validation itself is non-destructive.

If the active Catalogue contains broken/null Set or Reward memberships, the
Workbench exposes:

`Clean Missing Reference(s)`.

This is an explicit repair action.

Only broken/null memberships are removed.

Valid Set and Reward assets are preserved.

# Manual Escape Hatch

All Catalogue, Set and Reward assets remain ordinary Unity ScriptableObjects.

Normal creation, membership management, missing-reference repair, Reward
deletion and Reward Set deletion should use the Content Workbench.

Use the Project window / Inspector for:

- diagnosis;
- unusual repairs;
- advanced content inspection;
- workflows not represented by the normal Workbench.

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