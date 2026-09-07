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

## 3.0C — Expedition Lifetime & Persistent Progression Bridge

**Status: Active**

Prove:
- defeat ends expedition;
- new expedition resets World Time;
- persistent progression survives defeat;
- persistent Weapon / Armour / Focus levels seed a fresh RunBuild;
- temporary ability/build progression resets;
- failure → upgrade → retry is playable.

Use in-memory persistence only.

The prototype persistent economy is deliberately temporary.

## 3.0D — Quick Encounter Choice Proof

Prototype:
- three quick encounter choices;
- oldest selectable ordering;
- same EncounterDirection API;
- temporary presentation.

## 3.0E — Ambient Route Navigation

## 3.0F — Forecast Proof

## 3.0G — Encounter Supply Transition

## 3.0H — First World Event

## 3.0I — Passive Strategic HUD

## 3.0J — Expanded Strategic Planner

## 3.0K — Strategic Expedition Integration

---

# 3.0 — Strategic Expedition Prototype

## Goal

Prove the central strategic expedition loop before building the full procedural /
adaptive director.

The prototype should answer:

> Is it fun to watch autonomous combat while choosing between continuously
> changing encounter opportunities using a readable forecast?

A short 5–10 minute prototype is sufficient.

Encounter supply may initially be fake / predefined.

## 3.0B — World Clock Foundation

**Status: Active**

Establish:
- run-owned accelerated World Time;
- Day N + HH:MM;
- configurable world-clock rate;
- simulation-speed integration;
- basic debug presentation.

No scheduler behaviour is required yet.

## 3.0C — Quick Encounter Choice Proof

Prototype:
- three quick encounter choices;
- same `EncounterDirectionController` API as debug selection;
- oldest selectable encounter ordering;
- temporary presentation before final HUD work.

Controller architecture should remain compatible, but controller polish is not
required yet.

## 3.0D — Ambient Route Navigation

Create the first designer-authored ambient route proof.

Prove:
- authored travel nodes / connections;
- no Encounter Directive → ambient autopilot;
- Encounter Directive overrides ambient travel;
- manually placed ambient enemies are sufficient initially.

Do not build the complete ambient population system yet.

## 3.0E — Forecast Proof

Use predefined / fake schedule data.

Prove:
- next three upcoming encounters;
- World Time timestamps;
- short actionable countdown windows;
- longer-range major-event information.

The purpose is to test whether forecast information changes player decisions.

## 3.0F — Encounter Supply Transition

Introduce the first continuously cycling encounter supply.

Prove:
- encounter arrival;
- expiry;
- selected/committed encounter protection;
- automatic backfill;
- three selectable encounter opportunities remain available.

Deliberately address the old finite StageDirector boss-gating assumptions rather
than hiding endless encounter supply behind them.

## 3.0G — First World Event

Implement one representative world event.

Recommended proof:
- Zombie Surge;
or
- Fiery Zombies.

Prove:
- schedule integration;
- activation / expiry;
- affects future / unengaged content;
- does not rewrite already-engaged combat.

## 3.0H — Passive Strategic HUD

Implement the passive wireframe direction:

Top-left:
- World Time;
- simulation controls;
- next three upcoming schedule entries.

Top-right:
- active world-event status;
- minimap;
- three quick encounter slots.

Bottom-left:
- existing combat / ability information;
- targeting-policy prototype.

Bottom-right:
- Pending Rewards;
- currency / run resources.

Focus on information hierarchy rather than visual polish.

## 3.0I — Expanded Strategic Planner

Use the detailed strategic-panel wireframe as the basis.

Prototype:
- expanded map;
- known / locked encounters;
- forecast timeline;
- encounter information;
- selection;
- strong simulation slow-motion.

## 3.0J — Strategic Expedition Integration

Run a deliberate 5–10 minute strategic expedition proof.

Validate:
- encounter selection creates useful strategic agency;
- redirection feels immediate;
- pulling encounters together is useful/fun;
- ambient autopilot prevents inactivity;
- World Time is readable;
- forecast changes decisions;
- expiry creates useful urgency;
- world events influence planning;
- passive HUD remains understandable.

Do not build the full adaptive endless director until this loop is enjoyable.

------

# 3.1 — Data-Driven Encounter Generation and Authoring

Purpose:
Turn the proven static prototype into reusable encounter content.

Planned direction:
- Encounter Definition;
- Encounter Site;
- Runtime Encounter Instance;
- encounter compatibility / placement;
- reusable generic encounter sites;
- fixed spectacle sites;
- encounter pools;
- repetition control;
- dynamic encounter supply.

Major feature:
- Encounter Workshop scene/tool for isolated encounter authoring and testing.

---

# 3.2 — Escalation and Intensity

Purpose:
Create the effectively endless pacing engine.

Planned direction:
- escalation tiers;
- difficulty pools;
- composition modifiers;
- Elite frequency;
- encounter density;
- target intensity;
- peaks / valleys;
- major long-range events;
- runtime pressure telemetry;
- recovery behaviour;
- post-Apex endless escalation.

Avoid invisible rubber-banding of already-active combat.

World Time Phase + Expedition Day + Escalation + Intensity ↓ future pressure decisions

---

# 3.3 — Ambient Ecology and World Events

Expand the 3.0 ambient proof into a richer world system.

Potential work:
- multiple ambient routes;
- route selection;
- route intersections;
- population scaling;
- migrations;
- Zombie Surges;
- Fiery Zombies;
- ecological event interactions;
- ambient reward opportunities.

---

# 3.4 — Targeting and Tactics

Build the reusable decision primitives shared by hero targeting and future
support AI.

Initial targeting:
- Closest;
- Strongest;
- Weakest.

Composable priorities:
- Prefer Elite;
- Prefer Boss;
- Prefer Rare / Valuable.

Later:
- targeting presets;
- conditions;
- target queries;
- support tactics infrastructure.

---

# 3.5 — Strategic UI and Controller Pass

Turn the strategic UI prototypes into a cohesive player-facing system.

Areas:
- passive combat HUD;
- minimap;
- encounter quick slots;
- full strategic panel;
- schedule timeline;
- world-event presentation;
- Pending Rewards;
- run economy;
- targeting panels;
- controller focus navigation;
- keyboard/mouse equivalents;
- slow-motion / pause interaction.

---

# 3.6 — Support Characters

Implement the first support-character slice.

Goals:
- persistent support unlock;
- pre-run selection;
- sensible default behaviour;
- behaviour presets;
- first custom tactics;
- shared targeting primitives;
- support HUD / configuration proof.

Do not lock final support capacity prematurely.

---

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