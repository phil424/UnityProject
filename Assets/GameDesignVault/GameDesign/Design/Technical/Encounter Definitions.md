# Encounter Definitions

**Status: Working / Implemented Foundation**

Related:
- [[Encounter Workshop]]
- [[Level Authoring and Encounters]]
- [[Expedition Scheduling and Encounter Generation]]

# Purpose

`EncounterDefinition` is the first portable encounter-content artifact.

It represents:

> What gameplay content is this encounter?

rather than:

> Where in the world does this occurrence exist?

# Current Ownership Split

## Encounter Definition

Portable content currently owns:

- Display Name;
- Description;
- ordered phases;
- phase transition timing;
- Advance When Cleared;
- spawn groups;
- enemy definitions;
- enemy counts;
- start delays;
- batch size;
- spawn spacing;
- batch spacing;
- spawn-region shape;
- circle radius;
- box dimensions.

## Scene Encounter / Site

The scene currently still owns:

- physical encounter anchor;
- phase/group/source hierarchy;
- spawn-source Transform positions / rotations;
- strategic availability;
- encounter-site identity;
- reusable supply behaviour;
- region placement;
- future spectacle anchors.

## Runtime Encounter

`LevelEncounter` and its runtime components continue to own occurrence state:

- Known;
- Available;
- Selected / committed;
- Started;
- current phase;
- Completed;
- Expired;
- spawned actors;
- runtime phase progress.

# Authoring Pipeline

3.0J6 establishes:

Scene-authored Workshop Encounter
↓
EncounterWorkshopDraft
↓
play / tune
↓
Save / Save As
↓
EncounterDefinition.asset

A saved Definition may then be loaded back into the Workshop.

# Runtime Deployment

`LevelEncounterDefinitionBinding` connects a Definition to a compatible
scene-authored `LevelEncounter`.

At runtime:

EncounterDefinition
↓
EncounterDefinitionRuntimeApplicator
↓
runtime override seams
↓
normal encounter systems.

The expedition does not depend on Workshop classes.

# Runtime Content Materialization

3.0J7B removes the requirement for a scene Encounter to contain matching
phase/group/source topology.

An Encounter Definition may now materialize its runtime content beneath a
LevelEncounter.

Example:

EncounterDefinition

Phase 1
Phase 2
Phase 3

↓ runtime materialization

LevelEncounter
└── [Runtime Encounter Content]
    ├── Phase 1
    │   └── Spawn Group
    │       └── Spawn Source
    ├── Phase 2
    │   └── Spawn Group
    │       └── Spawn Source
    └── Phase 3
        └── Spawn Group
            └── Spawn Source

The generated hierarchy exists only for the runtime occurrence.

The scene no longer needs matching phase GameObjects.

## Fallback Scene Content

Existing scene-authored encounter content remains supported.

When no runtime Definition content is active:

LevelEncounter
→ consumes authored children.

When Definition content is active:

LevelEncounter
→ consumes its generated runtime-content root.

This allows gradual migration rather than requiring every existing encounter to
be converted immediately.

## Current Site Boundary

Generated Spawn Sources are currently relative to the Encounter anchor.

This is sufficient for generic Point / Circle / Box encounters.

Future Encounter Sites may expose named spatial sockets / anchors for fixed
spectacles and more geographically specific content.

# Save Semantics

Workshop editing distinguishes:

Saved Content
↓
Editable Draft
↓
Runtime Preview.

`Save`
updates the currently loaded/saved Definition.

`Save As`
creates or chooses another Encounter Definition asset.

`Load`
captures a compatible Definition into a fresh Draft.

Unsaved Draft experimentation never silently modifies the asset.

# Editor Boundary

Encounter Definition assets are normal runtime ScriptableObjects.

The Save / Load file-dialog workflow is Editor-only development tooling.

Normal gameplay does not reference `UnityEditor` APIs.

# Future Expansion

J7 will extend portable encounter content toward:

- composable triggers;
- encounter actions;
- phase transitions;
- spectacle hooks.

Later Encounter Site / Runtime Instance work should address:

- arbitrary compatible site construction;
- authored site tags;
- multiple spawn-anchor roles;
- generated encounter placement;
- runtime-instance identity;
- repetition / encounter-pool selection.

# 3.0J7 — Composable Encounter Rules

Encounter Definitions may contain portable one-shot Rules.

A Rule consists of:

Trigger
+
one or more ordered Actions.

Example:

Party Proximity
Radius 8m

→ Activate Encounter Combat
→ Begin Encounter Spawning.

Rules are composition, not Encounter Types.

Do not create combinations such as:

`ProximityAmbushEncounter`
`DelayedEliteEncounter`
`ChurchDoorEncounter`

as separate runtime encounter classes when the behaviour can be expressed by
reusable Trigger + Action vocabulary.

## Initial Trigger Vocabulary

3.0J7 supports:

- Party Proximity;
- Encounter Started;
- Delay After Encounter Started;
- Phase Started;
- Delay After Phase Started;
- Phase Cleared.

## Initial Action Vocabulary

3.0J7 supports:

- Activate Encounter Combat;
- Begin Encounter Spawning;
- Start Phase;
- Begin Spawn Group;
- Activate Spawn Group;
- Complete Encounter;
- Raise Signal.

Rules execute their Actions in authored order.

This matters for sequences such as:

Activate Combat
→
Begin Spawning.

## Rule Lifetime

Rules are occurrence-owned runtime behaviour.

Each Rule fires at most once per encounter occurrence in the initial
implementation.

Preparing/rearming the encounter resets Rule runtime state.

The portable Definition stores Rule configuration, not fired state or elapsed
runtime timers.

## Party Proximity

Party Proximity is expressed relative to the encounter anchor:

Encounter Transform
+
local trigger offset
+
radius.

This allows the Workshop to author useful approach behaviour without requiring
a new scene GameObject for every simple trigger.

Future Encounter Sites may expose named authored anchors when raw local offsets
are insufficient.

## Encounter Signals

`Raise Signal` provides a generic presentation/spectacle seam.

Example Signal IDs might include:

- `door-burst`;
- `ground-erupt`;
- `boss-arrival`;
- `church-bell`.

The encounter rule does not implement presentation.

Presentation systems may subscribe to the encounter signal and decide what the
signal means for the current site/content.

This keeps gameplay sequencing separate from VFX/audio implementation.

## Simple Phase Progression vs Rules

The J3 phase fields remain valid shorthand for the common case:

Timer
OR
Clear
→
next phase.

Rules are an advanced authoring escape hatch.

Do not require a Rule merely to express every ordinary two-wave encounter.