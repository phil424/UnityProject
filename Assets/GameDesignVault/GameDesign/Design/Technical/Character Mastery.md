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

Character Mastery currently has two concrete node types.

## CharacterMasteryStatNodeDefinition

Applies a Prepared `CharacterStatModifierType`.

Current examples include:

- Basic Attack Damage;
- Attack Speed;
- Move Speed;
- Max Health;
- Knockback Power.

## CharacterMasteryAbilityLevelNodeDefinition

Raises the prepared starting level of an Ability that already exists in the
Character's starting RunBuild.

It does not acquire an Ability.

Runtime order is therefore important:

starting Abilities
↓
Character Mastery
↓
prepared Ability levels

The first consumer is Ranger Power Shot.

Power Shot Drills
↓
Power Shot begins each Expedition one level higher
↓
the existing Ability-level curve provides its authored gameplay improvements

This allows Character Mastery to develop an Ability without copying individual
Ability mechanics into the Mastery system.

Temporary Expedition Ability levels may still increase the Ability further
during a run.

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

Current proofs include:

Punchy
→ stat-oriented Mastery nodes.

Ranger
→ stat-oriented nodes plus the first persistent starting Ability-level node.

It does not yet provide:

- final Mastery tree structure;
- graphical connection lines;
- respec;
- save-file persistence;
- non-stat node types;
- final costs/balance.

The remaining legacy Weapon / Armour / Focus data exists only as transitional
compatibility/tooling and is not the intended player-facing progression model.