# Author Abilities

**Status: Current**

# Purpose

Use the MiniCrawler Content Workbench to create, configure and inspect Ability
content.

The Workbench edits the real:

- `AbilityDefinition`;
- Ability runtime prefab;
- concrete `ActorAbility`;
- `AbilityEvolutionDefinition`;

assets.

It does not maintain a parallel Ability database.

# Open

Tools
→ MiniCrawler
→ Content Workbench
→ Abilities.

# Ability Architecture

Ability Definition
↓
static identity / level / cooldown configuration
↓
runtime prefab
↓
concrete `ActorAbility`.

Character ownership is separate.

A Character may use an Ability as:

- Signature;
- Utility;
- Sustain;

without that role becoming part of the Ability's generic identity.

# Ability Library

The Ability workspace discovers the project's real `AbilityDefinition` assets.

Selecting a Character in the Workbench sidebar provides Character-specific
context.

Core Ability rows use role presentation:

Signature
Utility
Sustain.

Abilities not in the selected Character's core kit remain generic Ability
content.

# Creating an Ability

Click:

`+ Ability`.

Author:

- Display Name;
- stable ID;
- content scope;
- runtime behaviour;
- Max Level;
- base cooldown;
- cooldown-per-level scaling.

Content Scope may be:

Shared
or
Selected Character.

Shared is the normal default.

Content Scope controls asset organisation and naming only.

It does not determine Ability ownership or eligibility.

Character-scoped assets are stored beneath that Character's Ability content
folder and receive Character-scoped filenames.

Use Character scope only when the content itself genuinely belongs to that
Character rather than merely because the Character currently uses it.

The creation flow automatically creates:

Ability Definition
+
runtime prefab
+
selected concrete `ActorAbility` component.

No temporary `New Ability` asset is required.

# Runtime Behaviour

The selected Ability exposes the concrete `ActorAbility` component from its
runtime prefab directly in the Workbench.

Edit its normal serialized gameplay values there.

Examples include:

- damage;
- range;
- duration;
- speed;
- knockback;
- movement;
- projectile;
- targeting distances.

The underlying prefab remains available through:

`Open Runtime Prefab`

as the advanced escape hatch.

# Level Preview

The Workbench previews:

Lv.1
versus
Lv.Max

for:

- cooldown;
- runtime `AbilityLevelValue` fields.

This is an authoring/balance aid.

The preview reads the real definition and runtime-prefab data.

# Character Usage

The Ability workspace displays playable Characters that currently use the
selected Ability in their core kit.

Character Ability roles remain Character-owned:

- Signature;
- Utility;
- Sustain.

Selecting a Character usage navigates to that Character.

# Persistent Progression

Character Mastery nodes that improve the selected Ability are displayed in the
Ability workspace.

Selecting a Mastery relationship navigates to the owning Character's Mastery
workspace.

# Run Reward Sources

Active Run Reward Catalogue entries that:

- unlock the Ability;
- grant one of its Evolutions;

are displayed as acquisition sources.

Selecting a Reward navigates to the Run Rewards workspace.

# Ability Evolutions

The selected Ability displays its authored `AbilityEvolutionDefinition` assets.

Use:

`+ New Evolution`

to create a new Evolution using identity-first authoring.

An Evolution Definition describes:

- identity;
- description;
- target Ability;
- prerequisites;
- presentation replacement.

It does not automatically invent gameplay.

Concrete Ability runtime behaviour must explicitly implement or bind an
Evolution's gameplay effect.

# Asset Identity

Ability Definitions, runtime prefabs and Evolutions support explicit asset-name
synchronisation.

Changing Display Name does not silently change stable gameplay IDs.

Unity GUID references remain authoritative when assets are renamed through the
Workbench.

Existing Abilities may also change authoring scope.

`Make Shared + Sync Names`

moves a Character-scoped Ability Definition and runtime prefab into the shared
Ability folders and removes the Character filename prefix while preserving Unity
GUID references.

A shared Ability may similarly be moved into the currently selected Character's
content scope.

Scope changes do not alter Character ownership and do not silently rewrite
stable gameplay IDs.

# Validation

Current Ability validation reports:

- duplicate Ability IDs;
- missing runtime prefabs;
- invalid root `ActorAbility` count;
- duplicate Evolution IDs;
- incomplete Evolution configuration;
- Evolution targets outside the authored Ability library.

Validation is non-destructive.

# Project Selection Integration

Selecting any of the following in Unity may synchronise the Content Workbench:

- `AbilityDefinition`;
- `AbilityEvolutionDefinition`;
- an Ability runtime prefab.

The Workbench resolves the appropriate Ability and opens the Abilities workspace.

# Manual Escape Hatch

All Ability assets remain ordinary Unity assets.

The normal Project window, Inspector and Prefab Mode remain available for:

- unusual repair work;
- runtime hierarchy changes;
- deleting assets;
- direct low-level inspection.

# Runtime Validation

After meaningful Ability changes:

1. enter Play Mode;
2. use a Character/build that owns the Ability;
3. verify activation;
4. verify cooldown;
5. verify level scaling;
6. verify runtime gameplay;
7. verify relevant Evolution behaviour;
8. verify Reward acquisition where applicable.

# Related Documentation

- `Design/Characters/Abilities.md`
- `Development Rules/Abilities and Buildcraft.md`
- `Guides/How-To/Author Character Mastery Trees.md`
- `Guides/How-To/Author Run Rewards.md`