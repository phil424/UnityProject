# MiniCrawler Development Roadmap

This document tracks implementation direction.

Game-design intent lives under `Design/`.

Technical architecture lives under `Design/Technical/`.

# Current Checkpoint

## 2.9E — Encounter Authoring Consolidation ✅

Completed:
- LevelEncounter foundation;
- separate spawning / combat activation;
- encounter lifecycle;
- encounter relationships;
- hierarchy-owned encounter authoring;
- reusable encounter commands;
- simplified StageDirector encounter discovery.

# 2.9F — Strategic Expedition Design Closure

**Status: Active**

Purpose:
- consolidate scheduler / encounter-generation design;
- document ambient exploration;
- define strategic HUD information hierarchy;
- capture remaining questions;
- establish the 3.x roadmap;
- review detailed strategic-panel wireframes.

No major gameplay implementation is required.

Completion:
- documentation updated;
- passive HUD wireframe accepted as working baseline;
- open questions recorded;
- 3.0 prototype scope agreed.

---

# 3.0 — Strategic Expedition Prototype

## Goal

Prove the central strategic expedition loop before building the full procedural /
adaptive director.

The prototype should answer:

> Is it fun to watch autonomous combat while choosing between continuously
> changing encounter opportunities using a readable forecast?

A short 5–10 minute prototype is sufficient.

The encounter supply may initially be fake / predefined.

```

3.0A — Encounter Directive + Membership
Immediate encounter redirection.

3.0B — World Clock Foundation
Day N + HH:MM
configurable clock speed
simulation-speed integration
basic debug presentation

3.0C — Prototype Encounter Supply
Always maintain three encounter choices.
Fake/predefined supply is fine.

3.0D — Forecast / Daily Timeline
Next 3 encounters
world-time timestamps
short countdown windows
long-range major events

3.0E — Quick Encounter Selection + Passive Strategic HUD
Minimap
three oldest selectable encounters
world clock
upcoming schedule

3.0F — Ambient Route Graph + Autopilot
No directive → follow ambient route/activity.

3.0G — First World Event
Zombie Surge / Fiery Zombies
scheduled using World Time.

3.0H — Expanded Strategic Planner
Your detailed wireframe
full timeline/calendar
known/locked encounters
strong slow-motion

3.0I — Strategic Expedition Integration
5–10 minute playable proof.
```


## 3.0A — Encounter Directive and Membership

Introduce runtime encounter membership for spawned actors.

Introduce a selected Encounter Directive.

Selecting another encounter:
- changes the directive immediately;
- drops targets belonging to the old encounter;
- begins travel toward the new encounter;
- does not force old enemies to disengage.

No polished UI required.

## 3.0B — Three Quick Encounter Choices

Prototype the three quick encounter slots.

Rules:
- always maintain three selectable choices;
- default display = three oldest selectable encounters;
- clicking / input selects Encounter Directive;
- use consistent Triangle / Square / Circle slot identity.

Controller architecture should remain compatible but full controller polish is
not required.

## 3.0C — Ambient Route Graph and Autopilot

Create the first designer-authored ambient route graph.

Prove:
- nodes;
- connections;
- ambient spawning ahead;
- local wandering;
- no selected encounter → follow ambient activity;
- selected encounter → encounter directive overrides ambient navigation.

Keep the first ambient population deliberately small.

## 3.0D — Simple Encounter Supply and Forecast

Do NOT build the final intelligent Encounter Director yet.

Use a controlled prototype queue.

Prove:
- three current selectable encounters;
- next three upcoming encounters;
- encounter arrival;
- encounter expiry;
- automatic backfill;
- selected encounter protected from expiry.

A handful of additional scene-authored encounters is sufficient.

## 3.0E — Passive Strategic HUD Proof

Implement the passive wireframe concepts:

Top-left:
- simulation controls;
- next three upcoming schedule entries.

Top-right:
- active world-event strip;
- minimap;
- three quick encounter slots.

Bottom-left:
- existing combat / ability information;
- targeting-policy prototype.

Bottom-right:
- Pending Rewards;
- currency / run resources.

Focus on information hierarchy, not visual polish.

## 3.0F — First World Event

Implement one representative world event.

Recommended first proof:
- Zombie Surge;
or
- Fiery Zombies.

Prove:
- passive HUD presentation;
- forecast entry;
- activation / expiry;
- affects unengaged encounter / ambient content;
- does not rewrite already-engaged combat.

## 3.0G — Expanded Strategic View

Use the user's detailed-panel wireframe as the basis.

Prototype:
- expanded map;
- known encounters;
- locked encounters;
- forecast / timeline;
- encounter details;
- selection;
- strong slow-motion.

Sorting/filtering only if required by the wireframe/playtest.

## 3.0H — Strategic Expedition Integration

Run a 5–10 minute prototype expedition.

Validate:
- three encounter choices remain available;
- encounter redirection feels immediate;
- old enemies can pursue into new fights;
- ambient autopilot works;
- forecast creates useful decisions;
- expiry creates urgency;
- passive HUD remains readable;
- world event affects planning.

Iterate based on actual play.

Do not implement the full endless director unless this loop is enjoyable.

---

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