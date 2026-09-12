# MiniCrawler Development Roadmap

This document tracks implementation direction.

Game-design intent lives under `Design/`.

Technical architecture lives under `Design/Technical/`.

# Current Checkpoint

## 2.9F — Strategic Expedition Design Closure ✅

Completed:
- scheduler / encounter-generation design;
- World Time / daily-cycle design;
- ambient exploration direction;
- strategic HUD information hierarchy;
- open design questions;
- initial 3.x roadmap.

## 3.0A — Strategic Direction Foundation ✅

Completed:
- runtime encounter membership;
- generic navigation intent;
- immediate encounter redirection.

## 3.0B — World Clock Foundation ✅

Completed:
- run-owned accelerated World Time;
- Day N + HH:MM;
- configurable clock speed;
- simulation-speed integration.

## 3.0C — Expedition Lifetime & Persistent Progression Bridge ✅

Completed:
- defeat ends the expedition;
- World Time resets between expeditions;
- persistent progression survives;
- fresh RunBuilds inherit persistent preparation;
- temporary progression resets;
- failure → upgrade → retry loop is playable.

## 3.0D — Quick Encounter Choice Proof ✅

Completed:
- three quick encounter slots;
- oldest selectable ordering;
- reusable quick-choice gameplay seam;
- prototype strategic HUD;
- consistent Triangle / Square / Circle slot identity.

## 3.0E — Ambient Route Navigation ✅

Completed:
- designer-authored ambient route graph;
- autonomous fallback exploration;
- local ambient combat;
- ambient travel resumes after combat;
- explicit Encounter Directive overrides ambient travel;
- ambient enemies remain separate from formal encounter progression.

## 3.0F — Forecast Proof ✅

Completed:
- passive World Time;
- next-three forecast;
- near-event countdowns;
- longer-range World Time presentation;
- first gameplay-backed scheduled event;
- player actions can resolve planned forecast entries early.

## 3.0G — Encounter Supply Transition ✅

Completed:
- reusable baseline encounter supply;
- encounter expiry;
- commitment protection;
- automatic backfill;
- more known opportunities than quick slots;
- one-shot scheduled encounters remain separate;
- encounter exhaustion no longer controls boss progression.

## 3.0H — First World Event ✅

Completed:
- run-owned World Event state;
- scheduled Zombie Surge;
- World Time-driven event duration;
- ambient-population pressure;
- active World Event HUD;
- already-generated actors are not rewritten on event expiry.

## 3.0I — Passive Strategic HUD ✅

Completed:
- strategic information migrated to normal Canvas UI;
- World Time and forecast presentation;
- Current World Events presentation;
- schematic minimap;
- unified map/list quick-slot identity;
- font-independent quick-slot graphics;
- reusable View / Group UI authoring foundation;
- stronger UI authoring rules and layout practices.

## 3.0J — Encounter Workshop & Encounter Iteration

### 3.0J1 — Replay Harness ✅

Completed:
- dedicated `EncounterWorkshop.unity`;
- real runtime systems;
- isolated authored encounter;
- one-click replay;
- deterministic replay;
- Approach;
- Threshold;
- On Top;
- Spawn On Arrival;
- Pre-Spawn From Start;
- Arrive While Pursued;
- manual ability use remains available.

`3.0JComplete.zip` is the completed J1 checkpoint.

### 3.0J2 — Test Build Configuration ✅

Completed:
- Early / Mid / Late / Extreme presets;
- direct Weapon / Armour / Focus testing;
- starting Ability Level controls;
- runtime stat preview;
- fresh replay applies selected test build;
- Workshop testing does not mutate persistent progression.

### 3.0J3 — Encounter Phases & Adaptive Pacing ✅

Completed:
- explicit hierarchy-authored Encounter Phases;
- phases distinct from spawn batches;
- phase-scoped progression;
- timer OR early-clear advancement;
- overlapping external enemies do not affect phase completion;
- phased Pre-Spawn behaviour;
- reusable phase-state reset;
- Dormant Horde normal-play proof.

### 3.0J4 — Encounter Identity & Presentation ✅

Completed:
- explicit Encounter Started lifecycle;
- authored start presentation;
- completion presentation;
- stacked transient announcements;
- Active Encounters HUD;
- phase progress;
- overlapping active encounter presentation;
- reusable encounter presentation reset;
- Workshop presentation testing;
- transient level-owned announcement cleanup.

### 3.0J5 — Editable Encounter Draft ✅

Completed:
- editable runtime Draft;
- identity editing;
- phase timing editing;
- enemy selection;
- spawn count/timing editing;
- Point/Circle/Box editing;
- runtime-preview application;
- Reset From Source;
- unsaved Play Mode experimentation does not mutate scene content.

### 3.0J6 — Encounter Definition Save / Load ✅

Completed:
- portable `EncounterDefinition` assets;
- Save / Save As / Load;
- Draft reconstruction from saved content;
- normal expedition Definition binding;
- one physical site can consume different compatible gameplay recipes;
- structural compatibility validation;
- runtime gameplay remains independent from Workshop Editor tooling.

### 3.0J7 — Composable Encounter Authoring

#### 3.0J7A — Trigger / Action Rules ✅

Completed:
- precision numeric authoring;
- portable Trigger + Action Rules;
- proximity/lifecycle/phase triggers;
- ordered encounter actions;
- spectacle Signal seam;
- Rule persistence.

#### 3.0J7B — Dynamic Encounter Topology ✅

Completed:
- authored proximity start rules own their approach boundary;
- default directive arrival remains the fallback when no authored start rule exists;
- stale Workshop actors cannot trigger new encounter occurrences;
- fresh one-phase encounters can be created entirely from data;
- phases can be added, duplicated and removed in the Workshop;
- Encounter Definitions own runtime phase/group/source topology;
- arbitrary Definition phase counts no longer require matching scene GameObjects;
- runtime encounter content is materialized from Definition data;
- the same encounter site may consume Definitions with different topology;
- scene-authored encounter content remains a fallback path;
- Phase 1 no longer incorrectly starts during level preparation.

`3.0J7Complete.zip` is the completed J7 checkpoint.

## 3.0J8 — Project Housekeeping & Structure

Purpose:

Consolidate the project after the major 3.0J encounter-authoring sequence before
another large feature area is added.

This is a behaviour-preserving structural pass.

### 3.0J8A — UI, Tooling & Prefab Structure ✅

Completed:
- player-facing UI scripts organised by responsibility;
- Encounter Workshop tooling grouped under `Tools/EncounterWorkshop`;
- reusable UI prefabs separated from gameplay actor prefabs;
- party prefabs grouped under `Prefabs/Actors/Party`;
- retired prototype strategic HUD components/scripts removed;
- Unity asset GUID/reference integrity preserved;
- canonical `Project Structure.md` created;
- Playground and Encounter Workshop preserved after structural moves.

`3.0J8AComplete.zip` is the completed J8A checkpoint.

### 3.0J8B — Runtime Code & Legacy Cleanup ✅

Completed:
- Progress organised into Persistent / Rewards / Run / Setup;
- Systems organised into Combat / Expedition / Flow / Movement;
- active PrototypeDebugUI moved into developer tooling rather than deleted;
- retired EditMode test assembly removed;
- direct project Test Framework dependency removed;
- test-only reward/progression compatibility APIs removed;
- unused SimulationPause pause-only compatibility event removed;
- obsolete Encounter Definition compatibility alias removed;
- namespaces and runtime behaviour preserved.

### 3.0J8C — Documentation & Development Rules ✅

Completed:
- Development Rules split into focused canonical rulebooks;
- concise `Development Rules.md` retained as the mandatory entry point;
- UI rules isolated into a dedicated high-detail rulebook;
- technical documentation grouped by meaningful domain;
- stale hypothetical links removed from the Game Design Index;
- current Game Design Index now mirrors the actual vault;
- legacy monolithic design baselines archived and explicitly deprecated;
- design-document template moved out of Core;
- initial Core-document scaffolding converted into current documentation;
- Open Questions and Decision Log examples converted into real entries;
- Project Structure updated with the canonical documentation model;
- Decision Log retained as one chronological authority;
- oversized technical documents audited without arbitrary splitting;
- standing test-package declaration cleanup completed.

### 3.0J8D — Structure Validation & Closure ✅

Completed:
- project compiles cleanly after the housekeeping moves;
- no Missing Script / Missing asset references found in primary scenes;
- normal expedition flow validated;
- strategic HUD / encounter presentation validated;
- current reward path validated;
- active developer debug tooling validated;
- Encounter Workshop replay / dynamic topology / authored approach behaviour validated;
- existing Encounter Definitions load correctly;
- Game Design Index resolves to current documentation;
- focused Development Rules are navigable from the canonical entry point;
- stale placeholder wiki-links removed from active documentation;
- archived design baselines are explicitly non-authoritative;
- final Unity and documentation structure is described by `Project Structure.md`.

`3.0J8Complete.zip` is the completed housekeeping checkpoint.

## 3.0K — Expanded Strategic Planner

**Status: Active**

## 3.0L — Strategic Expedition Integration

# 3.1 — Persistent Equipment Progression Trees

Purpose:

Replace the prototype linear Weapon / Armour / Focus levels with authored
persistent equipment progression.

Prove:
- first weapon progression tree;
- first armour progression tree;
- incremental stat nodes;
- meaningful unlock / identity nodes;
- persistent unlocked-node state;
- preparation UI;
- reusable effect/modifier application.

Trees should combine frequent incremental progress with larger mechanical
milestones.

Do not allow persistent equipment progression to replace expedition buildcraft.

# 3.2 — Data-Driven Encounter Generation + Workshop Integration
Purpose:

Replace prototype scene-site recycling with the long-term encounter content
model.

Develop:
- Encounter Definition;
- Encounter Site;
- Runtime Encounter Instance;
- compatibility / placement rules;
- encounter pools;
- repetition control;
- generated supply.

Integrate these definitions directly into the existing Encounter Workshop so
designers can load and test authored encounter content without duplicating it
into the workshop scene.

# 3.3 Escalation + Intensity

# 3.4 Ambient Ecology + World Events

# 3.5 Targeting + Tactics

# 3.6 Strategic UI + Controller

# 3.7 Support Characters

# Parallel Track — Art & Presentation

**Status: Available Parallel Track**

Art and presentation should be developed incrementally alongside the playable
game rather than postponed until all systems are complete.

This track may temporarily become active whenever visual readability, feel or
content presentation becomes the primary development bottleneck.

It does not replace the main gameplay roadmap.

## Environment Presentation

Potential work:
- first authored environment art pass;
- roads / terrain / buildings / landmarks;
- encounter-site visual language;
- ambient-route readability;
- lighting / World Time presentation;
- region identity;
- replacing prototype greybox geometry.

## Character / Enemy Presentation

Potential work:
- character models / sprites;
- enemy family appearance;
- Elite / Boss readability;
- animation integration;
- attack / locomotion presentation;
- damage / hit reactions;
- rare / valuable enemy visual language.

## Combat VFX

Potential work:
- ability VFX;
- Charge / Whirlwind / Rage presentation;
- knockback impact;
- spawn spectacles;
- damage / healing feedback;
- World Event effects;
- encounter-start presentation.

## UI Visual Design

Potential work:
- final HUD visual language;
- controller / quick-slot icon assets;
- ability icons;
- equipment icons;
- portraits;
- encounter rarity / threat icons;
- World Event icons;
- minimap styling;
- replacing prototype procedural graphics with authored assets where useful.

Gameplay identity should remain semantic and independent from the specific icon
asset used to render it.

## Animation

Potential work:
- locomotion;
- basic attack combos;
- ability animation;
- hit reactions;
- death;
- spawn / emerge;
- Elite / Boss presentation;
- equipment-specific combat animation.

Animation should consume existing gameplay events/state rather than become the
authority for unrelated gameplay rules unless explicitly designed otherwise.

## Audio Presentation

Future:
- attacks / impacts;
- abilities;
- encounter arrival;
- forecast warnings;
- World Events;
- UI feedback;
- region ambience;
- music / pressure layers.

## Art Integration Principle

Prefer replacing prototype presentation around already-working gameplay seams.

Avoid rewriting gameplay architecture solely to accommodate one visual asset
unless the visual requirement reveals a genuine missing gameplay concept.

Art passes should preserve the game's continuously playable MVP.


# Later 3.x

Potential major directions:
- encounter rarity / Nobleman-style rare targets;
- encounter reward previews;
- build-aware encounter generation;
- richer enemy families;
- weapon identity expansion;
- crafting / monster resources;
- persistent hub;
- Apex expansion;
- full vertical-slice integration.

These should be promoted into numbered milestones when their dependencies become
clear.