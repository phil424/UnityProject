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

## 3.0F — Forecast Proof

**Status: Active**

Prove:
- World Time is visible during normal play;
- next three planned events are visible;
- immediate events use countdowns;
- longer-range events use World Time;
- one forecast entry causes a real encounter-state change;
- player action may resolve a planned entry early;
- scheduling remains separate from continuous encounter supply.

## 3.0G — Encounter Supply Transition

## 3.0H — First World Event

## 3.0I — Passive Strategic HUD

## 3.0J — Expanded Strategic Planner

## 3.0K — Strategic Expedition Integration

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

# 3.2 Data-Driven Encounter Generation + Encounter Workshop

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