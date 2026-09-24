# MiniCrawler Development Roadmap

This document tracks implementation direction.

Game-design intent lives under `Design/`.

Technical architecture lives under `Design/Technical/`.

# Active Baseline — 2026-09-22

The development roadmap has been rebaselined around the fifteen active gameplay
feature areas documented in [[Active Feature Programme]].

Current project state:

- 3.0K Region Route Geography / authoring foundations are complete;
- 3.0M core animated-combat/action presentation through M4 is complete;
- post-M4 environment / Tilemap / lighting experimentation is preserved as
  parallel visual work;
- 3.0L authoring documentation is intentionally parked;
- 3.0M5 combat-authoring audit / guides are intentionally parked;
- the previous 3.0N+ future roadmap is superseded by the programme beginning at
  3.0N below.

Current gameplay focus:

> Understand and strengthen the complete player loop through Character
> buildcraft, Rewards, Meta Progression, content variety, expedition structure
> and player-facing control.

All fifteen areas in [[Active Feature Programme]] are active programme concerns.

Their roadmap order expresses dependency and current expected value rather than
a Now / Soon / Later classification.

Visual / environment work remains available as an independent parallel track.

The roadmap may be reshaped when implementation or playtesting provides better
evidence.

# Completed Development History

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

## 3.0K — Region Route Geography & Traversal ✅

**Status: Complete**

Purpose:

Promote the 3.0E ambient-route proof into meaningful authored Region geography
that can support navigation, formal Encounter placement, ambient activity and
future strategic planning.

### 3.0K1 — Route Geography & Purposeful Traversal Proof ✅

Completed:
- first-class Route Nodes;
- first-class Route Segments;
- explicit Route Groups;
- graph reachability / shortest-path queries;
- route validation;
- persistent ambient destinations;
- intermediate route traversal;
- local combat interruption / route resumption;
- Encounter Directives continue to override ambient travel;
- Village Well ring greybox proof;
- central landmark remains non-traversable Route geography.

`3.0K1Complete.zip` is the completed K1 checkpoint.

### 3.0K2 — Route Geography Authoring Workflow ✅

Completed:
- dockable Route Geography authoring tool;
- automatic authoring-root creation;
- one-button Route Node creation;
- automatic Segment creation / endpoint wiring;
- duplicate-Segment protection;
- hierarchy-ordered path creation;
- spatial loop creation;
- Route Group creation from selected geography;
- automatic Group membership wiring;
- non-destructive validation;
- Unity Undo support;
- generated content remains manually editable;
- runtime code remains independent from Editor tooling.

The Village Well Ring can now be recreated without manually adding route
components or assigning Segment endpoint / Group membership references.

`3.0K2Complete.zip` is the completed K2 checkpoint.

### 3.0K3 — Route Encounter Sites & Directed Travel

Purpose:

Connect formal Encounter occurrences to persistent Region geography without
making Routes themselves become Encounter lifecycle objects.

#### 3.0K3A — Route-Bound Sites & Directed Travel ✅

Completed:
- `EncounterRouteSite` connects formal Encounters to Region Route Geography;
- Site footprints support Node / Segment / multi-Segment composition;
- Route Site setup is integrated into the Route Geography authoring window;
- explicit Encounter Direction follows authored Route paths;
- route approach hands off to existing authored/default Encounter-start
  semantics;
- unbound Encounters retain direct travel fallback;
- western Village Well route contains the first three formal Encounter Sites.

`3.0K3AComplete.zip` is the completed K3A checkpoint.

#### 3.0K3B — Route Crossing, Ambient Discovery & Multi-Encounter Pursuit ✅

Completed:
- Route Sites can opt into traversal-start behaviour;
- explicit Node Sites trigger when their Node is reached;
- Segment Sites trigger when their specific Segment is entered;
- explicit Encounter travel starts eligible crossed Encounters;
- crossed Encounters do not replace the selected strategic directive;
- enemies from crossed Encounters may continue pursuing;
- the selected destination is excluded from traversal start and retains its
  existing final local start semantics;
- passive Ambient Route exploration also starts eligible crossed Encounter
  Sites;
- ambient exploration pauses while the formal traversal-started Encounter is
  active;
- ambient exploration resumes its previous journey after Encounter resolution;
- player-issued strategic redirection can leave an ambient-started Encounter
  behind while its enemies remain active;
- Encounter Membership preserves independent Encounter completion during
  physical overlap;
- traversal-start authoring remains Site-owned and exposed through the Route
  Geography tool.

`3.0K3BComplete.zip` is the completed K3B checkpoint.

### 3.0K4 — Route / Encounter Compatibility

Purpose:

Allow authored Encounter content to describe which kinds of Region geography it
can sensibly occupy without hardcoding specific scene locations.

#### 3.0K4A — Semantic Route Tags & Site Inheritance ✅

Completed:
- reusable `RouteTagDefinition` assets;
- direct Tags on Route Nodes, Segments and Groups;
- Group Tags inherit onto contained geography;
- Encounter Route Sites derive effective semantic Tags from their footprint;
- Route Geography authoring can create Tag assets;
- authoring can add/remove Tags from selected geography;
- duplicate Tag assignment is prevented by tooling;
- duplicate/empty manual Tag references are reported non-destructively;
- first Village Well vocabulary established:
  - Village;
  - Road;
  - Well;
  - Open;
- western Encounter Sites resolve inherited Route semantics without duplicate
  Site authoring.

`3.0K4AComplete.zip` is the completed K4A checkpoint.

#### 3.0K4B — Encounter Compatibility & Placement Queries ✅

Completed:
- Encounter Definitions explicitly opt into automatic placement;
- Definitions support Required Route Tags;
- Definitions support Preferred Route Tags;
- Definitions support Forbidden Route Tags;
- Route compatibility compares portable Definition requirements against
  Encounter Route Site effective Tags;
- Required Tags act as hard requirements;
- Forbidden Tags act as hard exclusions;
- Preferred Tags provide soft fit scoring;
- compatible Site candidates can be collected and ranked by preference score;
- Route Geography authoring can configure compatibility roles without manual
  list wiring;
- authoring prevents one Tag occupying several placement roles through the
  normal tool workflow;
- manual conflicting Tag authoring is reported non-destructively;
- compatibility preview explains why each current Site is compatible or
  incompatible;
- automatic placement remains opt-in, preserving bespoke/manual Encounter
  content;
- no scheduler or generated Encounter supply is implemented yet;
- West Road gains the reusable `Narrow` semantic Tag;
- `DormantHordeDefinition` demonstrates preferred-site ranking;
- `WideHordeDefinition` demonstrates forbidden-site exclusion.

`3.0K4BComplete.zip` is the completed K4B checkpoint.

### 3.0K5 — Ambient Activity Authoring

Purpose:

Make lightweight Region population expressive and easy to author without
turning ordinary ambient enemies into formal Encounters.

#### 3.0K5A — Route-Bound Ambient Activity Areas ✅

Completed:
- `AmbientActivityArea` replaces the fixed per-Node `AmbientSpawnPoint`
  prototype;
- Activity Areas bind population to Route Nodes / Segments / Groups;
- Segment-backed Areas distribute spawn positions along authored Route
  geography;
- Area population uses ActorDefinition/count entries;
- one Area may support several Actor population entries;
- spawn scatter is authored per Area;
- engagement radius is authored per Area;
- Route Geography authoring creates Activity Areas from selected geography;
- population entries and Area settings can be edited through the authoring
  tool;
- generated Area data remains manually editable;
- validation is non-destructive;
- four Village Well Activity Areas preserve the previous four-Zombie baseline;
- Zombie Surge now adds population through compatible Activity Areas;
- legacy `AmbientSpawnPoint` is retired.

`3.0K5AComplete.zip` is the completed K5A checkpoint.

#### 3.0K5B — Local Ambient Wandering ✅

Completed:
- disengaged Ambient enemies wander modestly around their spawn/home position;
- wandering uses `ActorNavigationIntent` rather than bespoke Transform movement;
- Ambient Activity Areas author Wander Radius and wander pause range;
- each spawned actor retains its own local home and wander timing;
- party proximity immediately cancels wander navigation and activates combat;
- engaged Ambient enemies retain normal combat pursuit;
- engagement does not currently revert to ambient wandering;
- Wander Radius `0` disables wandering;
- Simulation Pause / Slow / Normal / Fast naturally affect wandering;
- Zombie Surge population inherits the Area's wander behaviour;
- Encounter Workshop pursuers retain their previous non-wandering behaviour.

`3.0K5BComplete.zip` is the completed K5B implementation state.

#### 3.0K5C — Region Authoring UX Consolidation ✅

Completed:
- the monolithic Route Geography authoring window is retired;
- `Region Authoring` provides focused Route / Ambient / Encounters modes;
- normal Route authoring exposes only contextual creation/connection/grouping
  actions;
- Route Tags and repair operations are hidden under Advanced;
- Ambient Activity has its own focused population/behaviour workflow;
- K5B wandering settings are integrated into Ambient authoring;
- Encounter Site binding has its own focused workflow;
- Encounter Definition compatibility is moved to the separate
  `Encounter Placement Rules` tool;
- reusable compatibility queries/rules remain unchanged;
- healthy Region validation collapses to a one-line status;
- detailed validation diagnostics appear only when requested;
- long explanatory HelpBoxes are removed from healthy normal workflows;
- internal Nodes / Segments are presented as designer-facing Points /
  Connections where appropriate;
- required authoring roots remain automatically created by common operations;
- generated scene/data content remains normally editable through Unity
  Inspectors;
- runtime/data architecture remains unchanged.

`3.0K5CComplete.zip` is the completed K5C checkpoint.

### 3.0K6 — Workshop Route Context ✅

Completed:
- Encounter Workshop contains a small real Route Geography test context;
- Workshop context uses semantic Entry / Core / Exit roles;
- replay supports None / Point / Segment / Path / Route Group contexts;
- route-aware scenarios use the real `EncounterDirectionController`;
- `EncounterRouteSite` supports runtime footprint override without rewriting its
  serialized fallback footprint;
- Workshop Route context uses normal Route Tags;
- loaded Encounter Definitions display read-only Route compatibility against the
  selected context;
- compatibility never prevents deliberate Workshop replay;
- existing Approach / Threshold / On Top / Pursued scenarios remain available;
- Definition Save / Save As does not persist Workshop Site geography;
- Spawn Sources remain Encounter-anchor-relative;
- Entry / Core / Exit are test-context roles rather than portable Definition
  sockets.

`3.0K6Complete.zip` is the completed K6 checkpoint.

3.0K closes at K6.

The previously planned K7 First Playable Region Demo moved later in the 3.0
roadmap after a production-readiness review identified three missing foundations:

- comprehensive designer-facing Tutorials / How-To documentation;
- data-driven animated combat/action authoring;
- the intended three-part Character ability identity and Combat HUD.

These are content-production prerequisites rather than Route Geography work.

## 3.0L — Authoring Documentation & Production Guides

**Status: Parked**

This programme remains valuable but is deliberately paused while the core
gameplay systems continue to evolve.

The practical authoring audit showed that documenting workflows before the
underlying gameplay/content model stabilises risks documenting unnecessary
implementation plumbing.

Resume this programme when the systems intended for repeated content production
are sufficiently stable to audit and improve their authoring workflows.

Purpose:

Make the project's approved authoring workflows understandable without requiring
the designer to inspect source code or reconstruct usage from technical
documentation.

### 3.0L1 — Authoring Documentation Standard & Guide Index

Develop:
- canonical `Guides/` documentation area;
- Tutorials vs How-To distinction;
- Authoring Guides Index;
- standard structure for authoring documentation;
- Development Rules requiring authoring-guide maintenance alongside relevant
  implementation work.

### 3.0L2 — Region Authoring Tutorial & How-To Pack

Document:
- Region Authoring tool;
- Route Points / Connections / Groups;
- Route Tags;
- Ambient Activity Areas;
- validation / diagnostics;
- worked examples.

### 3.0L3 — Encounter Authoring Tutorial & How-To Pack

Document:
- Encounter Workshop;
- Encounter Draft workflow;
- phases;
- spawn groups;
- trigger/action rules;
- save / load / Save As;
- Route Context;
- Encounter Site binding;
- Encounter Placement Rules;
- compatibility preview.

### 3.0L4 — Expedition Content How-To Pack

Document current manual/data-authored workflows for:
- forecast / expedition schedule;
- Encounter supply;
- World Events;
- Zombie Surge;
- relevant validation and troubleshooting.

Ability / combat-animation authoring guides are deliberately deferred until the
new production systems are implemented.

Success question:

> Can someone unfamiliar with the implementation follow the Guides and create,
> test and place Region / Encounter content correctly?

## 3.0M — Animated Combat & Action Authoring

Purpose:

Replace prototype instant basic attacks with a flexible data-driven combat
action and sprite-presentation pipeline suitable for real character/enemy art.

Core principles:
- gameplay action timing is authoritative;
- animation follows gameplay timing rather than owning damage logic;
- attack speed affects gameplay timing and visual playback coherently;
- Actor gameplay roots remain separate from swappable visual presentation;
- existing `DamageResolver`, targeting, movement and ability architecture remain
  authoritative where appropriate;
- presentation effects must not secretly own gameplay results.

### 3.0M1 — Sprite Presentation Foundation ✅

**Status: Complete**

Develop:
- imported sprite-sheet workflow;
- actor Visual Root convention;
- reusable animation/presentation data;
- Idle / Movement proof;
- Simulation Pause / speed compatibility;
- swappable character/enemy presentation.

### 3.0M2 — Data-Driven Combat Action Timeline ✅

**Status: Complete**

Develop:
- Windup;
- Impact;
- Recovery;
- autonomous action locking;
- damage at authored Impact rather than attack-start;
- coherent Attack Speed scaling;
- interrupt seam for player abilities.

### 3.0M3 — Weapon Combo Authoring ✅

**Status: Complete**

Develop:
- ordered basic-attack sequences;
- per-step timing;
- per-step damage coefficients;
- per-step presentation;
- combo progression/reset behaviour;
- different weapon cadence without rewriting autonomous combat.

### 3.0M4 — Hit Reactions, Death & Presentation Cues ✅

**Status: Complete**

Completed:
- damage-driven actor hit reactions;
- gameplay-authoritative Windup / Impact / Recovery presentation;
- gameplay death separated from bounded visual cleanup;
- animated enemy death presentation;
- animated party death / expedition-defeat presentation;
- semantic Action Impact and Damage Resolved presentation cues;
- reusable short-lived sprite VFX;
- player and enemy weapon-swipe presentation;
- damage-contact presentation;
- whiffs visibly distinguish attack execution from successful contact;
- camera-relative directional effect facing;
- real animated actor silhouette shadows preserved through attacks, reactions and death;
- first combat camera / lighting / effect-readability baseline.

### 3.0M5 — Combat Authoring Workflow & Guides

**Status: Parked**

The combat-authoring audit and final Tutorials / How-To guides remain planned.

They are intentionally deferred while Character Stats, Weapon Mods, Abilities
and additional Character/enemy content are still expected to change the combat
authoring workflow.

Resume M5 as part of the later production-readiness / authoring-audit programme.

Prove:
- an authored sprite set can be attached to an actor;
- a new basic attack / combo can be configured primarily through data;
- presentation can be swapped without rewriting combat logic;
- complete Tutorials / How-To guides exist for the approved workflow.

# Active Gameplay Programme

The active gameplay programme follows the dependencies documented in
[[Active Feature Programme]].

All fifteen feature areas are current programme work.

Milestone boundaries are intentionally reviewable.

A later milestone may be reshaped when earlier implementation provides better
evidence.

# 3.0N — Core Loop, Buildcraft & Persistent Progression

**Status: Active**

Purpose:

Establish the gameplay and progression vocabulary that the rest of the active
programme depends upon.

This programme answers:

> Why is an expedition worth playing, what can the player build during it, what
> does gameplay reward, and what persists afterward?

## 3.0N0 — Gameplay Roadmap Rebaseline ✅

**Status: Complete**

Known documentation reconciliation is required for older assumptions around:
- multi-character active parties;
- swappable weapon identity;
- equipment progression trees;
- Armour progression;
- crafting/resource purpose;
- Ability acquisition;
- persistent economy.

Reconcile these documents incrementally when their corresponding N/O feature
area becomes active rather than attempting a speculative full-vault rewrite.

Documentation-only checkpoint.

Complete:
- establish [[Active Feature Programme]];
- replace the obsolete future roadmap;
- park the authoring/tutorial programme;
- preserve visual work as a parallel track;
- update development scope guidance;
- identify existing design documents that require reconciliation.

No gameplay behaviour changes in N0.

## 3.0N1 — Player Retention & Core Loop Review ✅

**Status: Complete**

Purpose:

Establish a working model for why the player wants to:

- continue the current expedition;
- make another build choice;
- pursue another objective;
- complete another Encounter;
- begin another expedition;
- unlock and experiment with more content.

Review:
- intrinsic combat/buildcraft loop;
- short-term goals;
- expedition-level goals;
- persistent goals;
- failure value;
- success value;
- experimentation;
- mastery/challenge;
- Character/Mod/Ability collection.

This is a working evaluation framework, not an immutable final answer.

### 3.0N1A — Retention & Short-Horizon Design Closure ✅

**Status: Complete**

Documentation/design closure for:
- playable time horizons;
- variable expedition depth;
- safe voluntary Return;
- Progression Targets;
- Objectives;
- Hunts;
- Achievements;
- Reward Vectors;
- generic resource-tier direction;
- exploratory narrative discovery.

### 3.0N1B — Active Expedition Return ✅

**Status: Complete**

Expose the already-existing voluntary-expedition-return runtime path to the
player during normal active expedition play.

Purpose:

Prove that short expeditions can end deliberately once the player has achieved
their current intention.

### 3.0N1C — Expedition Objective Foundation ✅

**Status: Complete**

Implement the first data-driven tracked Objective foundation.

Initial proof should:
- be expedition-owned;
- track progress during normal gameplay;
- present compact progress;
- allow Encounter completion to advance Objectives;
- allow current Encounter opportunities to indicate Objective relevance;
- avoid assuming the final Meta/resource/reward economy.

Later Objective breadth remains scheduled under 3.0P.

## 3.0N2 — Character Stats & Expedition Buildcraft

**Status: Active**

### 3.0N2A — Core Character Stats & Max Health Buildcraft ✅

**Status: Complete**

Purpose:

Establish the first intentional Character-stat vocabulary using concrete
gameplay consumers rather than speculative RPG statistics.

This step:
- clarifies Damage as Basic Attack Damage;
- clarifies Attack Speed as Basic Attack Speed;
- preserves Movement Speed and Armour;
- adds Max Health as an Expedition stat;
- ensures the run stat readout includes actual Expedition stat bonuses;
- preserves the existing temporary/persistent/runtime lifetime boundaries.

Additional stats should only be introduced when their gameplay consumers are
clear.

### 3.0N2B — Tactical Stat Consumers

**Status: Active**

Purpose:

Add Character stats that materially change tactical play rather than simply
adding more numerical reward variants.

This step proves:

- Ability Cooldown Recovery;
- Knockback Power;
- actor-generic stat consumption;
- expedition-owned stat reconstruction across Region transitions;
- source-aware knockback scaling.

Ability Cooldown Recovery affects active Ability cadence rather than basic attack
cadence.

Knockback Power modifies outgoing forced-motion distance without changing the
Character's own Charge travel distance.

Knockback Resistance remains deferred until meaningful incoming forced-motion
gameplay exists.

Basic Attack Range remains deferred until its relationship with preferred combat
distance is deliberately designed.

## 3.0N3 — Rewards & Progression Flow

Purpose:

Define the reward bridge connecting gameplay activity to temporary and
persistent progression.

Review and implement the necessary foundations for:
- Ambient rewards;
- Encounter rewards;
- Elite/Boss rewards;
- Objective rewards;
- World Event rewards;
- expedition completion/failure rewards;
- temporary build rewards;
- persistent rewards;
- compatibility-aware reward generation;
- meaningful reward-source identity.

## 3.0N4 — Meta Progression & Resources

Purpose:

Replace the placeholder long-term progression assumptions with a coherent
persistent progression model.

Review/develop:
- persistent resource vocabulary;
- Character progression/mastery;
- unlock ownership;
- resource sources/sinks;
- persistent versus temporary power;
- cross-character progression direction;
- replacement of prototype Weapon / Armour / Focus progression;
- role, if any, of future crafting/material systems.

### 3.0N Review Gate

Before leaving N, reassess:

> Do the core loop, Character stats, reward flow and persistent progression form
> a coherent reason to play and replay expeditions?

If not, revisit the relevant N step before building large amounts of dependent
content.

---

# 3.0O — Characters, Weapon Mods & Abilities

Purpose:

Build the player's real combat/buildcraft identity on top of the N foundations.

## 3.0O1 — Characters & Unlocks

Develop/review:
- one active playable Character per expedition;
- Character Combat Identity;
- Character selection;
- persistent roster ownership;
- Character unlock conditions/progress;
- at least one additional meaningfully different playable Character;
- future Companion flexibility without making active parties the near-term
  design.

## 3.0O2 — Weapon Mods

Develop the shared Weapon Mod system.

Initial design space:
- Physical / Melee Mods;
- Magical / Elemental Mods;
- Ranged / Projectile Mods.

Prove:
- Character-native Mods;
- progression-based Mod unlocks;
- Mod effects on combat/buildcraft;
- compatibility;
- cross-character unlock direction;
- interaction with the Character stat system.

Do not lock the final capacity/activation model before gameplay evidence exists.

## 3.0O3 — Abilities

Reassess and expand:
- three Character core Ability roles;
- Character-native Abilities;
- generic Ability pool;
- compatibility;
- preparation versus expedition acquisition;
- levels;
- Evolutions;
- runtime/presentation integration.

Use gameplay testing to decide how much Ability identity comes from preparation
versus expedition buildcraft.

## 3.0O4 — Character Buildcraft Integration Review

Play the Character / Weapon Mod / Ability systems together.

Review:
- whether Mods and Abilities occupy distinct useful roles;
- whether different Characters genuinely feel different;
- whether temporary buildcraft still creates meaningful variation;
- whether cross-character progression remains desirable;
- whether the current stat vocabulary is sufficient.

### 3.0O Review Gate

> Do Characters, Weapon Mods and Abilities create enough identity and flexibility
> to justify expanding their content?

Later O/P work may be reshaped based on this answer.

---

# 3.0P — Combat Content, Objectives & World Population

Purpose:

Build richer problems for the player's Character/buildcraft systems to solve.

## 3.0P1 — Enemy Roster & Combat Roles

Expand the enemy vocabulary.

Develop representative roles such as:
- swarm;
- pressure;
- tank;
- ranged;
- support;
- control;
- high-damage threat;
- Elite;
- rare/high-value target;
- Boss where useful.

Focus on mechanical differentiation rather than raw enemy count.

## 3.0P2 — Combat Targeting

Develop player-controlled priorities for autonomous combat.

Potential targeting language:
- Closest;
- Weakest;
- Strongest;
- Highest Damage;
- Elite;
- Boss;
- Rare / Valuable;
- role-based preferences where useful.

Targeting should complement autonomous combat rather than becoming direct manual
unit control.

## 3.0P3 — Expedition Objective Expansion

Develop tracked goals capable of providing:
- expedition rewards;
- persistent progress;
- Character unlock progress;
- Mod/Ability progression;
- content direction.

Support objective types only as concrete gameplay/content needs emerge.

## 3.0P4 — Encounter Variety

Expand Encounter content using the richer Enemy and Objective vocabulary.

Develop different:
- compositions;
- phases;
- reinforcement patterns;
- pressure profiles;
- Elite/Boss opportunities;
- objective structures;
- reward identities.

Use the existing Encounter Workshop/Definition/Site architecture rather than
building a replacement authoring system.

## 3.0P5 — Ambient Enemies & World Population

Move Ambient Activity beyond one-shot population spawning.

Develop/review:
- population capacity;
- replenishment;
- refresh cadence;
- spawn rhythm;
- Area-specific population;
- World Time / World Event pressure;
- Escalation interaction;
- rare Ambient actors;
- migration or temporary local safety where useful.

### 3.0P Review Gate

> Does the world now present sufficiently different combat situations that
> Character buildcraft, targeting and strategic choices matter?

---

# 3.0Q — Expedition Shape, Strategy & UI Closure

Purpose:

Turn the richer gameplay systems into a coherent expedition experience and
complete the functional player-facing structure around them.

## 3.0Q1 — Expedition Pacing, Escalation & Endings

Design and implement the expedition's overall shape.

Review:
- expedition duration;
- opening pace;
- progression cadence;
- pressure growth;
- quiet periods;
- Elites;
- Boss/Apex pressure;
- World Events;
- success condition;
- voluntary return;
- continuing after success;
- risk/reward of staying;
- failure consequences.

Use real N/O/P gameplay content rather than designing pacing from placeholder
content alone.

## 3.0Q2 — Strategic World Layer Review

Reassess the existing:
- Route system;
- Encounter supply;
- quick choices;
- forecast;
- World Time;
- World Events;
- Encounter Sites;
- map/planner concepts.

Keep and expand the parts that create meaningful decisions.

Simplify or remove complexity that does not earn its place.

## 3.0Q3 — UI Structure & Finalisation

Functional UI work occurs throughout N/O/P/Q.

Q3 closes the overall UI architecture.

Complete/review the functional skeleton for:
- preparation;
- Character roster/selection;
- Character progression;
- stats;
- Weapon Mods;
- Abilities;
- combat HUD;
- targeting;
- Objectives;
- Encounter choices;
- forecast;
- strategic planning;
- Rewards;
- resources;
- results/unlocks;
- controller navigation.

Final visual skin remains a separable presentation pass.

## 3.0Q4 — Holistic Gameplay Review & Roadmap Reassessment

Play and evaluate the complete active feature programme.

Review:
- retention loop;
- expedition buildcraft;
- Meta Progression;
- Character identity;
- Mods;
- Abilities;
- Enemy/Encounter variety;
- Objectives;
- Ambient population;
- targeting;
- pacing;
- strategic decisions;
- UI comprehension.

Use the evidence from this review to define the next numbered programme.

Do not preserve N–Q assumptions merely because they were previously written into
the roadmap.

---

# Superseded Future Roadmap — 2026-09-22

The previous future-roadmap ordering is superseded by the N–Q programme above.

Its useful ideas remain part of the project and have been redistributed.

Previous:
- 3.0N Character Abilities & Combat HUD

Now:
- 3.0O Characters / Weapon Mods / Abilities;
- functional UI developed throughout;
- final UI structure closed in 3.0Q3.

Previous:
- 3.0O First Playable Demo

Now:
- no immediate standalone Demo milestone;
- the active programme develops the game holistically;
- a future demo / vertical-slice milestone may be scheduled when doing so becomes
  useful.

Previous:
- 3.0P Expanded Strategic Planner

Now:
- reviewed in 3.0Q2 after richer gameplay exists.

Previous:
- 3.1 Persistent Equipment Progression Trees

Now:
- superseded as the assumed primary progression model;
- persistent progression is reconsidered in 3.0N4;
- Character-focused progression/mastery is the current working direction.

Previous:
- 3.2 Data-Driven Encounter Generation + Resolution

Now:
- still valuable architecture;
- implementation priority will be reassessed through Encounter Variety and the
  Strategic World review rather than assumed as the immediate next programme.

Previous:
- 3.3 Escalation, Encounter Mutation + Intensity

Now:
- expedition-wide pacing/escalation is addressed in 3.0Q1;
- deeper generated Encounter mutation remains available when required.

Previous:
- 3.4 Ambient Ecology + World Events

Now:
- Ambient population development begins in 3.0P5;
- broader ecology remains extensible from that foundation.

Previous:
- 3.5 Targeting + Tactics

Now:
- active-Character Combat Targeting moves to 3.0P2;
- future Companion tactics remain separate later work.

Previous:
- 3.6 Strategic UI + Controller

Now:
- functional UI evolves continuously;
- structural UI closure occurs in 3.0Q3.

Previous:
- 3.7 Support Characters

Now:
- not part of the current fifteen-feature programme;
- existing architecture should preserve a future route to autonomous Companions
  without requiring their implementation now.

# Parallel Track — Art & Presentation

**Status: Available Parallel Track**

Current note:

Post-3.0M4 environment experimentation currently includes:
- Tilemap / RuleTile terrain;
- visual roads;
- environment props;
- lighting / shadow-receiving ground investigation;
- environmental mood exploration.

This work may continue independently when useful.

It is not currently a prerequisite for the active N–Q gameplay programme and
does not need to reach a formal visual-finalisation milestone before gameplay
development continues.

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


# Beyond the Active Programme

Do not pre-schedule another large future roadmap while N–Q are still producing
major design evidence.

Potential future programmes remain available, including:

- autonomous Support / Companion Characters;
- multiple production Regions;
- richer generated Encounter systems;
- deeper ecology;
- authoring / production-readiness audits and Guides;
- optional crafting/resource systems where justified;
- long-term/endgame challenge systems;
- persistent hub features if they earn a clear role;
- formal demo / vertical-slice packaging;
- larger-scale content production.

Promote these into numbered milestones when current gameplay evidence makes
their dependencies and value clear.