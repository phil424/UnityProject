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

# Compatibility

3.0J6 uses compatible scene topology.

A Definition may currently be applied when:

- phased/unphased structure matches;
- phase count matches;
- spawn-group count per phase matches;
- spawn-source count per group matches.

Spawn-entry count does not need to match because the runtime schedule may be
replaced completely.

This is an intentional intermediate architecture.

Future:

Encounter Definition
+
Encounter Site
+
Runtime Encounter Instance

should allow more general construction / compatibility without requiring
pre-existing matching phase/group hierarchy.

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