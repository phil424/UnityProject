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

## 3.0I — Passive Strategic HUD

**Status: Active**

Prove:
- World Time and forecast use normal Canvas UI;
- Current World Events occupy the strategic top-right area;
- three quick encounter choices use normal Canvas UI;
- first schematic minimap;
- party position is visible;
- quick encounter anchors are visible;
- map and list use identical Triangle / Square / Circle slot identity;
- selecting from either presentation issues the same Encounter Directive;
- prototype IMGUI strategic panels are no longer the normal presentation.

## 3.0J — Encounter Workshop MVP

Purpose:

Reduce the iteration cost of designing encounters now that encounter feel has
become a meaningful gameplay bottleneck.

Create a dedicated encounter test environment using real runtime systems.

Prove:
- dedicated `EncounterWorkshop.unity`;
- isolated authored encounter;
- rapid replay / reset;
- Approach scenario;
- On Top scenario;
- pre-spawned versus spawn-on-arrival setup;
- Arrive While Pursued scenario;
- easy tuning without playing through a full expedition.

The Workshop should exist before the full data-driven encounter-generation
architecture.

Future Encounter Definition / Site / Runtime Instance work should integrate into
and improve the Workshop.

## 3.0K — Expanded Strategic Planner

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