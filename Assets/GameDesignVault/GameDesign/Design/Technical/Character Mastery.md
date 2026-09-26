# Character Mastery

**Status: Implemented Foundation**

# Purpose

Character Mastery is the primary Character-specific persistent progression
surface.

It survives Expedition end and establishes prepared starting development.

# Data Ownership

`PartyMemberDefinition`
↓
authored Mastery node list

`CharacterMasteryNodeDefinition`
↓
identity
description
prerequisites
persistent-resource costs
effect behaviour

`PersistentMemberProgress`
↓
unlocked Mastery node IDs

# Current Node Types

The first concrete implementation is:

`CharacterMasteryStatNodeDefinition`

It applies a Prepared Character stat modifier.

The abstract Mastery-node definition allows future concrete nodes to represent
other progression without expanding one universal effect enum.

Potential future examples:

- Ability unlock;
- Weapon Mod unlock;
- combo modification;
- Character mechanic;
- cross-Character/global milestone.

These should be introduced only alongside real gameplay consumers.

# Resource Costs

A Mastery node may require multiple generic resource tiers.

Unlock validation is atomic.

All required resources must be available before any are spent.

# Prerequisites

Nodes may reference other Mastery nodes as prerequisites.

A node cannot be purchased until every authored prerequisite has been unlocked
for that Character.

# Runtime Flow

PersistentProgression
↓
unlocked node state
↓
new Expedition
↓
RunState creates RunBuild
↓
PersistentProgression.ApplyMasteryToBuild
↓
Prepared modifiers copied into RunBuild

Temporary Expedition progression is applied separately.

Runtime Actor
↓
PartyUpgradeApplicator
↓
combined effective values

# Prepared vs Expedition Modifiers

`RunBuild` contains two stat-modifier sources:

Prepared
→ copied from persistent Character Mastery.

Expedition
→ acquired during the current run.

They are separate sources but expose a combined effective value to runtime
systems.

This preserves lifetime ownership while allowing natural stacking.

# Preparation Showcase

The live Preparation showcase reconstructs a temporary RunBuild from:

- current Character;
- current persistent prototype compatibility state;
- starting Abilities;
- Character Mastery.

Unlocking a node therefore updates the live showcase immediately.

# Current Limitations

The first proof contains only stat-oriented Punchy nodes.

It does not yet provide:

- final Mastery tree structure;
- graphical connection lines;
- respec;
- save-file persistence;
- non-stat node types;
- final costs/balance.

The remaining legacy Weapon / Armour / Focus data exists only as transitional
compatibility/tooling and is not the intended player-facing progression model.