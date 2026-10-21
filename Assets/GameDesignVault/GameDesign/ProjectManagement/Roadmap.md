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

# 3.0N — Core Loop, Buildcraft & Persistent Progression ✅

**Status: Complete**

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

## 3.0N2 — Character Stats & Expedition Buildcraft ✅

**Status: Complete**

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

### 3.0N2B — Project Health & Parallel Work Checkpoint ✅

**Status: Complete**

This checkpoint captured parallel project work while the N2 gameplay programme
remained active.

It includes:
- recurring milestone Code Health Pass rules;
- writer-facing concise game / player-loop / narrative briefing documents;
- initial legacy-code formatting cleanup;
- parallel Ability experimentation.

This checkpoint did not expand the Character-stat vocabulary.

### 3.0N2C — Tactical Stat Consumers ✅

**Status: Complete**

Purpose:

Add Character stats that materially alter tactical play rather than only
increasing general numerical power.

This step proves:
- Ability Cooldown Recovery;
- Knockback Power;
- actor-generic stat consumption;
- Expedition-owned stat reconstruction across Region transitions;
- source-aware outgoing knockback scaling.

Ability Cooldown Recovery affects active Ability cadence rather than autonomous
Basic Attack cadence.

Knockback Power modifies outgoing forced-motion distance without altering the
Character's own movement distance.

Knockback Resistance remains deferred until meaningful incoming forced-motion
gameplay exists.

Basic Attack Range remains deferred until preferred combat-distance behaviour is
designed alongside it.
### 3.0N2 Review

N2 establishes the first intentional Expedition stat vocabulary:

#### Survival
- Max Health;
- Armour.

#### Mobility
- Movement Speed.

#### Basic Attack
- Basic Attack Damage;
- Basic Attack Speed.

#### Ability
- Ability Cooldown Recovery.

#### Control
- Knockback Power.

This is sufficient vocabulary to continue developing Reward and Encounter flow.

Additional Character stats should now be introduced because later gameplay
creates a concrete need rather than continuing to expand the stat list for its
own sake.

Deferred:
- Basic Attack Range as a normal reward;
- Knockback Resistance;
- speculative RPG statistics without current consumers.


## 3.0N3 — Rewards & Progression Flow ✅

**Status: Complete**

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

### 3.0N3A — Encounter Reward Vectors ✅

**Status: Complete**

Purpose:

Connect Encounter selection to the type of Expedition reward the player is
pursuing.

The first implementation proves:
- reward definitions expose a Reward Vector;
- Encounter Definitions author a completion Reward Vector;
- completing an Encounter can request a vector-specific reward choice;
- reward generation filters eligible offers by that vector;
- quick Encounter choices telegraph the vector before commitment.

Initial implemented vectors:
- Character Growth;
- Ability.

`Any` preserves existing generic/debug/legacy reward sources.

Future vectors should only be added when their reward systems actually exist.

### 3.0N3B — Reward Provenance & Pending Choice Context ✅

**Status: Complete**

Purpose:

Preserve the reason a reward exists from the gameplay trigger through pending
reward presentation.

This step proves:

- reward triggers create a `RunRewardContext`;
- context carries Reward Vector and source label;
- pending reward choices preserve that context;
- Encounter rewards identify their Encounter occurrence;
- enemy reward choices identify the defeated actor;
- Area Clear rewards retain their own source identity;
- both combat and between-level reward UI expose the current reward context.

This establishes a reusable provenance seam before additional reward sources are
introduced.

### 3.0N3B1 — Pending Reward UI Presentation Polish ✅

**Status: Complete**

Purpose:

Improve the readability and visual hierarchy of the combat Pending Rewards
overlay without changing reward behaviour.

The pass established:

- a proper modal reward window;
- clear Header / Offer Root / Footer ownership;
- stronger background dimming;
- cleaner reward-card presentation;
- explicit runtime-generated content ownership.

The UI authoring rules were also tightened using the problems discovered during
this work, particularly around hierarchy migration, existing layout ownership
and structural validation checkpoints.

### 3.0N3C — Quick Encounter Reward Focus ✅

**Status: Complete**

Purpose:

Allow the player to bias the three quick Encounter opportunities toward the
kind of reward they currently care about.

Initial focus modes:

- Balanced;
- Character Growth;
- Ability.

Focused modes preferentially rank matching currently-selectable Encounters
before non-matching opportunities.

They do not hard-filter Encounter Supply.

When insufficient matching content exists, remaining slots fall back to the
normal oldest-selectable ordering.

This system changes quick-access presentation rather than Encounter generation,
availability or authored reward identity.

## 3.0N4 — Meta Progression & Resources

**Status: Active**

### 3.0N4A — Live Preparation Simulation Foundation ✅

**Status: Complete**

Completed:

- PreRun presentation remains inside `PlaygroundScene`;
- the actual gameplay world provides the preparation backdrop;
- the confirmed playable Character uses the real runtime Actor/combat pipeline;
- the Character follows authored Ambient Route geography;
- enemies populate across the Route and wander locally using normal ambient
  behaviour;
- proximity naturally transitions ambient actors into real combat;
- showcase actors do not participate in Expedition reward/Encounter lifecycle;
- the showcase Character is effectively immortal;
- normal Expedition systems remain dormant while the frontend simulation runs.

Purpose:

Replace the current administrative pre-expedition screen with the first proper
Preparation frontend.

This step establishes:

- one active playable Character selection;
- live in-world Character presentation;
- Character Overview presentation;
- a Character carousel;
- an Overview / Progression tab structure;
- preservation of the current prototype persistent upgrades;
- a functional visual shell for the rest of N4 Meta Progression.

The frontend remains inside `PlaygroundScene` for now.

A separate Unity frontend scene is deliberately deferred until scene transition
and persistent frontend/gameplay handoff requirements are concrete.

Final UI skinning remains later presentation work.

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

### 3.0N4B — Preparation Frontend UI & Flow Closure ✅

**Status: Complete**

Purpose:

Complete the player-facing Preparation frontend around the live simulation.

This step establishes:

- approximately two-thirds live gameplay-world presentation;
- approximately one-third Preparation UI;
- one active playable Character selection;
- Character Overview;
- persistent Progression tab;
- runtime-generated Character selector;
- persistent Currency presentation;
- Start Expedition flow;
- full Preparation → Expedition → Return → Preparation lifecycle.

The current persistent Weapon / Armour / Focus model remains prototype
progression and will be replaced/reassessed during later N4 work.

### 3.0N4C — Persistent Resource Foundation ✅

**Status: Complete**

Purpose:

Separate temporary Expedition Currency from the first real persistent
progression economy.

This step establishes:

- Level 1 / Level 2 / Level 3 placeholder persistent resources;
- explicit persistent-resource rewards on enemies;
- Expedition-owned earned-resource staging;
- banking on Voluntary Return and Defeat;
- separation from temporary Expedition Currency;
- persistent resource presentation in Preparation;
- Level 1 as the temporary compatibility cost for legacy persistent upgrades.

Exact names, conversion ratios and final balance remain future work.

### 3.0N4D — Punchy Character Mastery Foundation ✅

**Status: Complete**

Purpose:

Replace the player-facing prototype Weapon / Armour / Focus progression with
the first real Character-centric persistent progression system.

This step establishes:

- authored Character Mastery nodes;
- per-Character permanent node ownership;
- prerequisite relationships;
- multi-tier persistent-resource costs;
- Prepared stat modifiers;
- separation between Prepared and Expedition stat progression;
- runtime reconstruction when an Expedition begins;
- live Preparation showcase updates;
- runtime-generated Mastery presentation.

The first Punchy proof uses four stat-oriented nodes.

These nodes validate the progression architecture rather than define the final
Punchy tree.

Future Mastery node types may unlock:
- Abilities;
- Weapon Mods;
- combo/basic-attack modifications;
- Character-specific mechanics;
- cross-Character progression.

The legacy Weapon / Armour / Focus data remains temporary compatibility/tooling
infrastructure but is no longer the player-facing persistent progression model.

### 3.0N Review Gate ✅

**Status: Complete**

Review question:

> Do the core loop, Character stats, reward flow and persistent progression form
> a coherent reason to play and replay expeditions?

Current conclusion:

The N foundations are coherent enough to support the next programme.

The implemented loop now connects:

Preparation
↓
Character / Mastery intent
↓
Expedition
↓
Objectives and Encounter choice
↓
temporary build rewards
↓
persistent resource earnings
↓
Voluntary Return / Defeat
↓
resource banking
↓
Character Mastery
↓
next Expedition

Remaining shortcomings are primarily content and breadth problems scheduled
under O/P/Q rather than missing N architecture.

The roadmap should now favour expanding player-facing Character identity,
buildcraft options and content rather than continuing to add new foundational
N systems.

---

# 3.0O — Characters, Weapon Mods & Abilities

**Status: Active**

Purpose:

Build the player's real combat/buildcraft identity on top of the N foundations.

## 3.0O1 — Characters & Unlocks ✅

**Status: Complete**

Develop/review:
- one active playable Character per expedition;
- Character Combat Identity;
- Character selection;
- persistent roster ownership;
- Character unlock conditions/progress;
- at least one additional meaningfully different playable Character;
- future Companion flexibility without making active parties the near-term
  design.

### 3.0O1A — Persistent Playable Character Roster & Unlock Ownership ✅

**Status: Complete**

Purpose:

Separate the authored playable Character roster, persistent Character ownership
and current Expedition selection.

This step establishes:

- `PlayableCharacterRosterDefinition`;
- explicit starting-Character ownership;
- persistent unlocked-Character state;
- locked / hidden roster presentation rules;
- domain-level Setup validation;
- Preparation roster generation from the shared roster definition;
- a generic Character-unlock seam for later gameplay systems.

Current roster:

Punchy
→ unlocked by default.

No additional playable Character is introduced during O1A.

### 3.0O1B — Second Playable Character ✅

**Status: Active**

Build the first additional playable Character as a genuinely distinct combat
identity rather than promoting legacy Support content.

Prove:

- distinct visual identity;
- distinct basic attack / weapon identity;
- Character stats;
- authored combat presentation;
- core Ability identity;
- Character Mastery;
- Preparation showcase behaviour;
- real Expedition play.

#### 3.0O1B1 — Ranger Basic Identity & Projectile Combat ✅

**Status: Complete**

The first additional playable Character now exists as a ranged combat proof.

Working Character identity:

Ranger
→ lower durability;
→ long-range bow basic attack;
→ single-shot cadence;
→ real travelling projectile delivery.

The implementation also establishes generic projectile-based Combat Actions.

Projectile delivery remains separate from direct melee impact:

Combat Action impact
→ projectile launch

Projectile arrival
→ DamageResolver

The same seam is intentionally reusable by future ranged enemies and Abilities.

Ranger currently remains locked in the normal roster and is accessed through the
development unlock seam until O1C implements a real Character unlock path.

Ranger Abilities and Character Mastery remain O1B2 work.

#### 3.0O1B2 — Ranger Core Abilities & Character Mastery

**Status: Active**

Complete Ranger's playable Character identity through:

- Signature Ability;
- Utility Ability;
- Sustain Ability;
- first Ranger Character Mastery;
- buildcraft interactions appropriate to ranged play.

##### 3.0O1B2A — Ranger Core Ability Identity ✅

**Status: Complete**

Ranger's first complete three-part Character core is implemented.

#### Signature — Power Shot

Power Shot is a deliberate directional projectile attack.

Current behaviour:

Ranger
↓
SPECIAL ATTACK windup
↓
one straight Power Shot projectile
↓
pierces multiple enemies
↓
each enemy may be hit once
↓
offset enemies are knocked outward from the shot line
↓
the resulting displacement forms a forward fan / arc

Power Shot does not home after release.

Its authored Ability levels currently improve several aspects of the Ability,
including:

- damage;
- range;
- number of enemies pierced;
- knockback distance;
- cooldown.

Working future Evolution direction:

Power Shot
↓
three-shot / spread / rapid-volley form.

The multi-shot form is intentionally reserved as an Ability Evolution rather
than being part of the baseline Ability.

#### Utility — Backstep

Backstep restores Ranger's preferred combat spacing when an enemy gets too
close.

Current behaviour:

enemy breaches safe spacing
↓
Backstep becomes usable
↓
Ranger faces away from the threat
↓
DASH presentation
↓
dust burst at the starting position
↓
ForcedMotion carries Ranger away
↓
Ranger stops near useful bow range
↓
normal autonomous attacks resume

Backstep is therefore a contextual spacing tool rather than a generic movement
button.

#### Sustain — Guard

Guard is mitigation-based Sustain rather than direct healing.

Current behaviour:

Guard activation
↓
short BLOCK commitment
↓
temporary Armour increase
↓
blue defensive aura remains visible for the buff duration
↓
normal autonomous attacks resume while protection remains active

This confirms that a Character's Sustain role does not need to be a heal.

#### Shared Technical Seams

O1B2A also established:

- generic projectile-shot Ability behaviour;
- directional piercing projectile delivery;
- reusable Ability sprite presentation;
- reusable Ability effect presentation;
- attached timed Sprite Effects;
- a shared timed-stat-buff Ability implementation used by Ranger Guard and
  existing buff Abilities.

Ranger Character Mastery remains O1B2B work.

##### 3.0O1B2B — Ranger Character Mastery ✅

**Status: Complete**

Use Ranger's now-playable combat identity to create the first Ranger-specific
persistent Mastery proof.

The initial Mastery should reinforce:

- normal ranged cadence;
- spacing / mobility;
- Power Shot development;
- ranged battlefield control.

This step should also prove persistent starting Ability-level development rather
than limiting Character Mastery to stat nodes.

Implemented:

- Ranger-specific persistent Mastery;
- two small starting stat nodes;
- prerequisite relationships;
- persistent starting Ability-level development;
- Power Shot Mastery using the existing Ability level curve;
- Knockback Mastery using the existing generic KnockbackStats seam;
- live runtime Ability progression refresh without resetting cooldowns;
- prepared Ability levels shown in the Preparation Overview.

The first Ranger tree remains a gameplay proof rather than final progression
content.

#### 3.0O1B3 — Second Character Integration Review ✅

**Status: Complete**

Verified Punchy and Ranger through the shared Preparation / Expedition loop.

Confirmed:

- Punchy begins unlocked while Ranger begins visible but locked;
- persistent ownership gates Character selection independently of roster visibility;
- the existing development unlock refreshes Preparation without a scene reload;
- both Characters use the same Setup / Expedition architecture;
- Punchy creates close-range combo-pressure decisions;
- Ranger creates ranged spacing, projectile-lineup and defensive timing decisions;
- Ranger's Power Shot, Backstep and Guard remain functional in normal Expedition play;
- Character ownership survives Expedition end within the current session;
- session restart correctly returns ownership to authored roster defaults.

O1B is now closed with two mechanically distinct playable Characters using the
same Character, Ability, Mastery and Expedition foundations.

### 3.0O1C — First Character Unlock Path ✅

**Status: Complete**

Use the second playable Character to prove an actual persistent Character unlock.

The first unlock path should connect meaningful gameplay/progression to:

Locked Character
↓
unlock progress / requirement
↓
persistent ownership
↓
Preparation roster update
↓
Character selection
↓
real Expedition play.

#### 3.0O1C1 — Persistent Character Unlock Requirement Foundation ✅

**Status: Complete**

Implemented the first real persistent Character unlock path.

Current proof:

- roster entries may reference a data-authored Character unlock requirement;
- persistent Character unlock progress survives Expedition boundaries;
- completed Expedition Objectives may contribute to an unlock requirement;
- Ranger currently requires two completed Cull Dormant Hordes Objectives;
- unlock progress commits when the Expedition ends;
- completed progress may survive Defeat;
- satisfying the requirement grants normal persistent Character ownership;
- Preparation refreshes through the existing persistent-progression event path.

The Ranger requirement remains prototype progression content rather than a final
narrative unlock condition.

#### 3.0O1C2 — Character Unlock Presentation ✅

**Status: Complete**

Completed the first functional Character unlock presentation.

Preparation now:

- presents visible locked Characters directly in the Character selector;
- shows the Character's authored unlock requirement;
- shows current persistent unlock progress;
- refreshes that progress when the player returns from an Expedition;
- visibly transitions a progression-earned Character to `UNLOCKED`;
- makes the newly-owned Character selectable through the normal roster flow;
- rebuilds the selector when Preparation becomes active so Characters unlocked
  while the frontend was disabled can appear correctly.

Ranger now demonstrates the complete first Character acquisition loop:

Locked
↓
meaningful Expedition requirement
↓
persistent progress
↓
ownership
↓
visible unlock feedback
↓
Character selection
↓
real Expedition play.

The current presentation is functional rather than final visual polish.

## 3.0O2 — Weapon Mods ✅

**Status: Complete**

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

Current confirmed Mastery stance model:

- four total Weapon Mastery positions;
- Slot 1 is the Character's Core Mastery;
- Slots 2–4 are alternative Masteries;
- the original Basic Attack is a pre-Core-Mastery fallback, not a fifth stance;
- once Core Mastery is unlocked, it permanently replaces the original Basic
  Attack as the Character's default;
- deactivating an alternate Mastery returns to Core Mastery;
- only one Mastery controls Basic Attacks at a time.

Remaining progression, compatibility and combination details should continue to
be driven by representative gameplay.

### 3.0O2A — Weapon Mod Runtime & Combat Hook Proof ✅

**Status: Complete / Reframed by O2B**

Implemented the first vertical Weapon Mod combat proof.

O2A established:

- reusable Weapon Mod definitions;
- explicit Character compatibility;
- RunBuild Weapon Mod ownership seam;
- runtime projection through `PartyWeaponModApplicator`;
- Basic Attack Weapon Mod context;
- Weapon Mod Reward Vector;
- Basic Attack modification through Heavy Finisher.

Heavy Finisher demonstrated:

Punchy
→ final combo step
→ +100% damage.

O2A initially used direct reward acquisition and simultaneous passive Mod
application to validate the combat hook.

Those ownership and activation semantics were superseded by O2B.

The combat integration seam remains part of the current implementation.

### 3.0O2B — Weapon Mod Stance Reframe ✅

**Status: Complete**

Converted the O2A combat proof into the intended selectable Weapon Mod stance
architecture.

Current foundation:

- Character Mastery may prepare a Weapon Mod stance for every Expedition;
- `RunBuild` owns `RunWeaponModState` rather than a stack of always-active Mod
  definitions;
- prepared stances currently begin at temporary Level 0;
- `WeaponModRuntimeState` exposes available stances separately from the active
  stance;
- Default is represented by no active Weapon Mod;
- exactly one Weapon Mod may affect Basic Attacks at a time;
- selecting the active Mod again returns to Default;
- runtime refresh preserves an active stance where possible;
- new Actor reconstruction currently begins in Default;
- the O2A direct stance-acquisition Run Reward has been retired;
- `RunRewardVector.WeaponMod` remains for temporary Mod development rewards.

Punchy's first persistent Mod path is:

Heavy Hands
↓
Heavy Finisher Stance
↓
Heavy Finisher automatically prepared in future Punchy Expeditions.

Heavy Finisher remains the first combat proof:

Default
→ normal combo.

Heavy Finisher active
→ final combo strike deals +100% damage.

### 3.0O2C — Weapon Mod Combat HUD & Run Development ✅

**Status: Complete**

Implemented the first complete player-facing Weapon Mod stance and temporary
progression loop.

Current foundation:

- Combat HUD exposes four fixed Weapon Mod stance positions;
- unavailable stance positions remain visible;
- Default means no Weapon Mod stance is active;
- available stances show identity and temporary level;
- the HUD shows READY / ACTIVE state;
- selecting an inactive stance activates it;
- selecting the active stance returns to Default;
- `RunBuild` enforces four prepared Weapon Mod positions;
- `RunWeaponModState` owns temporary Expedition level;
- `WeaponModLevelRewardDefinition` develops an already-prepared stance;
- Weapon Mod rewards cannot grant an unprepared stance;
- receiving a Mod-level reward does not automatically activate it;
- switching away from a developed Mod preserves its level;
- switching back restores the developed behaviour;
- runtime refresh preserves both current stance and current level;
- Expedition end resets temporary Mod level while persistent Mastery retains
  stance availability.

Heavy Finisher remains the current validation stance:

Lv.0
→ +100% combo-finisher damage.

Lv.1
→ +150% combo-finisher damage.

These values remain exaggerated validation tuning.

### 3.0O2D — Weapon Mod Behaviour Breadth & Combination Proof ✅

**Status: Active**

Expand beyond Heavy Finisher using representative Weapon Mod behaviours.

#### 3.0O2D1 — Ranger Piercing Arrows ✅

**Status: Complete**

Implemented the first ranged/projectile Weapon Mod stance.

Current proof:

- Ranger Mastery unlocks Piercing Arrows;
- the existing four-position Weapon Mod HUD presents the stance without
  Character-specific UI;
- Default Ranger attacks retain targeted projectile delivery;
- Piercing Arrows overrides Basic Attack projectile delivery with a directional
  multi-hit projectile;
- the stance reuses the existing projectile runtime rather than introducing a
  parallel projectile system;
- Piercing Lv.0 may hit two aligned enemies;
- each temporary level adds one additional target;
- temporary levels change penetration rather than raw damage;
- the generic Weapon Mod reward pool correctly separates Punchy and Ranger
  rewards through compatibility and current RunBuild state.

This proves Weapon Mods can alter attack behaviour shape rather than merely
modify damage.

#### 3.0O2D2 — Core Mastery Baseline & Persistent Growth ✅

**Status: Complete**
Correct the Weapon Mod stance model around persistent Character identity.

Prove:

- one of the four stance positions is the permanent Core Mastery;
- the original Basic Attack exists only before Core Mastery ownership;
- Core Mastery automatically becomes the default once unlocked;
- Core Mastery cannot be deactivated back to the original Basic Attack;
- alternative Masteries return to Core when deactivated;
- stance positions are explicit rather than implied by list order;
- permanent Mastery development can improve a Core Mastery's starting state;
- temporary Expedition rewards stack on top of that persistent baseline.

Current prototype Core Masteries:

Punchy
→ Heavy Finisher.

Ranger
→ Piercing Arrows.

#### 3.0O2D3 — Knockback & Multi-Stance Proof ✅

**Status: Complete**

Implemented Ranger's first alternate Weapon Mastery.

Current configuration:

Core
→ Piercing Arrows.

Alternate1
→ Impact Arrows.

Impact Arrows:

- uses targeted single-target projectile delivery;
- applies shared knockback on impact;
- trades Piercing's multi-target throughput for battlefield control;
- receives temporary Expedition-level scaling;
- interacts with Ranger's existing persistent Knockback Power stat.

Verified:

Core
→ Alternate
→ Core

switching without returning to the pre-Mastery Basic Attack.

Ranger's existing Arrow Impact node now develops Impact Arrows through the
shared Character-stat architecture.

This is the final planned gameplay-node addition before replacing the temporary
flat Mastery-node list.

#### 3.0O2D4 — Character Mastery Tree Presentation & Authoring ✅

**Status: Complete**

Replaced the temporary flat Mastery list with a spatial Character-specific
progression graph and established the first approved authoring workflow.

Current foundation:

- dedicated Preparation Mastery overlay;
- bounded Character-specific canvas;
- authored node positions;
- prerequisite-derived connection lines;
- compact selectable graph nodes;
- selected-node detail / purchase presentation;
- locked / available / unlocked states;
- conventional drag-to-pan navigation;
- mouse-wheel pointer-focused zoom;
- optional middle-mouse pan;
- Character-specific tree silhouettes;
- visual drag-to-position authoring tool;
- non-destructive graph/layout validation;
- Inspector editing retained as an escape hatch.

No second progression graph was introduced.

`CharacterMasteryNodeDefinition.Prerequisites`

remains authoritative for gameplay progression.

#### 3.0O2D5 — Combination & Elemental Proof ✅

**Status: Complete**

Completed representative Weapon Mastery combination and elemental proofs.

Current Ranger Masteries demonstrate:

Piercing Core
→ multi-target projectile identity.

Impact Alternate
→ single-target knockback/control.

Fire Alternate
→ targeted Burning pressure.

Persistent pairwise synthesis now supports:

Impact
← Piercing

and:

Fire
← Piercing.

Pairwise combination ownership is explicitly limited to one inherited Mastery
per active stance.

This allows several separately-owned pairings without creating an unintended
three-way active combination.

Weapon Mastery now has sufficient gameplay evidence to pause further content
expansion and address architectural scaling pressure before Abilities expand.

### 3.0O2E — Buildcraft Architecture Readiness

**Status: Active**

Before expanding 3.0O3 Abilities, consolidate several systems that have reached
their prototype scaling limit.

This is a focused readiness pass rather than a broad refactor.

#### 3.0O2E1 — Reward Catalogue & Modular Reward Sets ✅

**Status: Complete**

Remove the expanding scene-owned Run Reward list from `RunDirector`.

Introduce data-authored reward catalogue/set definitions so reward content may
grow in modular groups without repeatedly editing the scene.

Preserve:

- existing Reward Vector filtering;
- concrete reward eligibility;
- pending reward behaviour;
- current random offer generation.

The catalogue defines available reward content.

Individual reward definitions remain responsible for eligibility/application.

#### 3.0O2E2 — Projectile Delivery & Impact Decomposition ✅

**Status: Complete**

Reduce `ProjectileRuntime` from a growing all-responsibilities implementation
into focused reusable projectile seams.

Current pressure includes:

- targeted travel;
- targeted piercing travel;
- directional travel;
- damage;
- knockback;
- Burning;
- several specialised spawn methods.

The goal is not merely fewer lines.

Separate:

- launch/configuration;
- travel/hit detection;
- impact payload/effects;
- runtime lifecycle.

New projectile behaviours should normally compose existing seams rather than
require another specialised `ProjectileRuntime.Spawn...` method.

Do not create a universal gameplay-effect graph.

Implemented a composable projectile launch architecture.

Current separation:

`ProjectileLaunchRequest`
→ describes one launch.

`ProjectileDelivery`
→ describes travel shape.

`ProjectileImpactPayload`
→ describes Damage / optional Knockback / optional Burn.

`ProjectileLauncher`
→ validates and instantiates.

`ProjectileRuntime`
→ owns live lifecycle and movement.

`ProjectileHitQuery`
→ owns directional segment target discovery.

`ProjectileImpactResolver`
→ owns impact application.

Retired the expanding specialised `ProjectileRuntime.Spawn...` API family.

Existing targeted, targeted-piercing, directional, Burning and knockback
behaviours now compose through the shared launch request.

Verified existing Ranger Basic Attack, Power Shot and Weapon Mastery projectile
behaviour without changing their intended gameplay.

#### 3.0O2E3 — Character Mastery Definition & Authoring Workbench

**Status: Complete**

Move Character Mastery content ownership out of `PartyMemberDefinition` and
establish one coherent authoring surface for the complete Character progression
graph.

##### 3.0O2E3A — Character Mastery Definition & Data Ownership ✅

**Status: Complete**

Introduce a focused `CharacterMasteryDefinition`.

Move:

- Mastery-node membership;
- Mastery-tree canvas size;

out of `PartyMemberDefinition`.

Preserve:

- persistent progression behaviour;
- prepared-build application;
- spatial runtime tree;
- current Mastery-tree authoring tool;
- all current Punchy and Ranger progression.

This establishes a clean content source before expanding the authoring tool.

##### 3.0O2E3B — Character Mastery Workbench ✅

**Status: Complete**

Evolve the current spatial layout tool into the primary Character Mastery
Workbench.

Target workflow:

select Character
↓
inspect the complete Mastery definition
↓
create a node from the Workbench
↓
configure its relevant gameplay data
↓
author prerequisites
↓
inspect/open related Ability / Weapon Mastery assets
↓
position the node spatially
↓
validate
↓
save.

Target Workbench capabilities:

- one-click node creation by supported node type;
- automatic asset creation in the Character's Mastery content folder;
- automatic addition to the Character Mastery Definition;
- central spatial graph;
- selected-node editing panel;
- prerequisite editing;
- relevant related-definition fields;
- direct navigation to related assets;
- Character-specific canvas editing;
- non-destructive graph/layout validation;
- Undo;
- normal Inspector editing retained as an advanced escape hatch.

Use progressive disclosure.

Do not expose every possible field for every node type simultaneously.

Do not create a second progression graph.

### 3.0O2 Exit Gate

After O2E:

- Weapon Mastery gameplay foundation is considered sufficient;
- reward content has a scalable source;
- projectile behaviour has a reusable extension seam;
- Character Mastery has a coherent content-authoring surface.

Then close 3.0O2 and activate 3.0O3 — Abilities.

## 3.0O3 — Abilities ✅

**Status: Complete**

Before substantially expanding Ability content, establish the unified content
authoring surface that future Character / Mastery / Reward / Ability content
will use.

### 3.0O3A — Unified Content Workbench ✅

**Status: Complete**

Evolve the successful Character Mastery Workbench into the primary authoring
front door for Character/buildcraft content.

The Workbench must edit the real ScriptableObject assets.

It must not create a parallel editor-only content database.

Target direction:

Character
↓
Character identity / runtime setup / starting loadout
↓
Character Mastery
↓
related Run Rewards
↓
Abilities
↓
related content and validation.

Normal authoring should remain task-focused through contextual workspaces rather
than exposing every possible data type at once.

#### 3.0O3A1 — Workbench Foundation, Character Creation & Asset Identity ✅

**Status: Complete**

Create the unified Content Workbench shell.

Implement:

- roster-driven Character browser;
- `New Character` workflow;
- identity-first asset creation;
- automatic creation of Character, Actor and Mastery Definition assets;
- automatic playable-roster registration;
- three starting Ability slots prepared for authoring;
- Character readiness / missing-data feedback;
- inline Character / Actor / roster editing;
- explicit asset-name synchronisation;
- Mastery node creation that asks for identity before creating the asset;
- explicit Mastery-node asset-name synchronisation.

Existing Character Mastery authoring remains available during this transitional
slice.

#### 3.0O3A2 — Embedded Mastery Workspace ✅

**Status: Complete**

Bring the existing Character Mastery graph directly into the unified Content
Workbench.

Retire the separate Character Mastery Workbench window.

Normal Character authoring should move between Character and Mastery contexts
without opening another tool or reselecting assets.

#### 3.0O3A3 — Run Reward Authoring & Preview ✅

**Status: Complete**

Add a Run Reward workspace around the existing:

Reward Catalogue
→ Reward Sets
→ Reward Definitions.

Target workflow:

browse catalogue
↓
select / create Reward Set
↓
create or edit Reward Definition
↓
manage set membership
↓
preview player-facing reward presentation
↓
inspect compatibility / eligibility context.

Avoid returning to an ever-growing central list.

### 3.0O3B — Ability System Reassessment & Expansion ✅

**Status: Complete**

Reassess the existing Ability architecture against the current Character,
Mastery, Reward and Expedition design before expanding content.

The existing generic Ability foundation remains valuable:

Ability Definition
↓
Run Ability State
↓
Runtime Ability State.

Avoid replacing working generic systems merely because the surrounding game has
evolved.

#### 3.0O3B1 — Core Ability Kit & Progression Contract ✅

**Status: Complete**

Make the confirmed playable-Character core explicit:

Signature
+
Utility
+
Sustain.

These are Character-loadout roles.

They are not categories stored on `AbilityDefinition`.

Preserve:

- reusable generic Ability definitions;
- three prepared Character core Abilities;
- extra Expedition-acquired Abilities beyond that core;
- persistent Mastery improvements to prepared Ability levels;
- temporary Expedition Ability levels;
- Ability Evolutions;
- generic Actor runtime execution.

Update the unified Content Workbench so the three core roles are clearly
authored rather than represented as anonymous list positions.

#### 3.0O3B2 — Ability Authoring Workspace ✅

**Status: Complete**

Add an Ability workspace to the unified Content Workbench once the core loadout
contract is explicit.

Target workflow:

browse Ability content
↓
select / create Ability
↓
edit identity and runtime definition
↓
inspect Character usage
↓
inspect Reward acquisition sources
↓
inspect Evolutions
↓
author / navigate related content
↓
validate.

Ability authoring should continue editing real ScriptableObject assets.

Do not create a parallel Ability database merely for the editor.

#### 3.0O3B3 — Representative Ability Buildcraft Proof ✅

**Status: Complete**

Use representative gameplay to challenge the confirmed Ability model.

Prove at least one meaningful Ability development path across:

- prepared Character ownership;
- persistent starting-level development;
- Expedition level development;
- Evolution / transformation;
- Reward eligibility;
- runtime presentation.

Ranger Power Shot is a strong current candidate because its projectile
architecture already supports meaningful directional / multi-target expansion,
but the final content choice should follow the Ability authoring audit.

#### 3.0O3B4 — Core Ability Content Review ✅

**Status: Complete**

The playable Character core-kit direction is now locked at the behavioural level.

Punchy:

Signature
→ Blink.

Utility
→ Whirlwind.

Sustain
→ reactive healing + Armour, increasingly effective at low Health.

Ranger:

Signature
→ Power Shot.

Utility
→ Smoke Step.

Sustain
→ Guard.

Current prototype content may be retired, repurposed or parked where it no
longer serves the confirmed Character role.

Charge remains valid reusable Ability content but is no longer planned as one of
Punchy's three core roles.

Vortex remains useful Evolution proof content but is no longer part of the
active Whirlwind direction and may later move to another Ability / Character.

Fan Volley remains part of the Power Shot direction.

##### 3.0O3B4A — Control Vocabulary & Utility Pass ✅

**Status: Complete**

Establish reusable Stun control.

Apply the first shared control language to:

Punchy Whirlwind
→ lower base damage emphasis;
→ stronger radial Knockback;
→ Stun;
→ primary purpose is creating breathing room.

Ranger Backstep
→ apply Knockback + Stun around the departure position;
→ then move Ranger away;
→ primary purpose is immediately restoring ranged spacing.

Forced movement must continue while a target is Stunned.

Stun should prevent normal movement, basic attacks, support actions and new
Ability activations.

Park Vortex from normal Reward generation while preserving the existing proof
assets for later reassessment.

##### 3.0O3B4B — Sustain Vocabulary & Core Sustain Pass ✅

**Status: Complete**

Establish the approved Sustain language.

Punchy Sustain:

- direct healing;
- stronger recovery at lower Health;
- temporary Armour;
- Armour/recovery strength may scale with missing Health;
- reactive brawler survival identity.

Ranger Guard:

- hit-based Shields rather than Armour;
- Shields have no time duration;
- Shields may stack;
- each Shield requires an authored number of hits to break;
- incoming hits are absorbed until the active Shields are exhausted;
- healing-over-time operates alongside the Shield;
- long-form defensive survival identity.

Introduce reusable hit-Shield and healing-over-time primitives only as required
by these abilities.

##### 3.0O3B4C — Signature Ability Pass ✅

**Status: Complete**

Rebuild the Character Signature abilities around the confirmed identities.

Punchy Blink:

- teleport / traverse between multiple valid enemies;
- traversal should visibly move around the combat space rather than behaving as
  a single-target stat buff;
- more valid enemies should create more useful traversal opportunities;
- each visited enemy may receive Knockback + Stun;
- Punchy is invulnerable during the active Blink sequence;
- timing the Ability into a populated battlefield should increase its value;
- exact bounce cap / timing is tuning rather than locked balance.

The existing extreme-value Blink stat-buff prototype should be retired from
Punchy's gameplay and preserved only through a generic developer acceleration
tool if that testing workflow remains useful.

Ranger Power Shot:

- fire three sequential shots matching the authored three-shot animation;
- begin with equivalent shots for the simplest readable implementation;
- each shot may Pierce;
- Knockback + Stun are part of the initial control language;
- Fan Volley remains the current Evolution and should be adapted around the
  three-shot burst rather than discarded.

##### 3.0O3B4D — Comparative Core-Kit Playtest ✅

**Status: Complete**

Initial comparative playtesting confirmed that Ranger's overall core-kit identity
is enjoyable and worthwhile, while exposing several concrete corrections before
the Ability programme can close.

Punchy also exposed Signature / Sustain issues that require a focused correction
pass before final comparative validation.

###### 3.0O3B4D1 — Ranger Playtest Corrections ✅

**Status: Complete**

Address the Ranger issues demonstrated during the first comparative playtest.

Shield progression:

- Ranger begins with a maximum Shield capacity of 3;
- Shield capacity is distinct from Shields granted per Guard activation;
- current / maximum Shields require player-facing HUD presentation;
- Shield Capacity joins the shared Character stat vocabulary so persistent
  Mastery and Expedition rewards may increase the cap;
- actual permanent Mastery / Reward placement should be decided during
  buildcraft review rather than invented during this correction pass.

Smoke Step:

- replaces the previous long-distance Backstep design;
- preserve the emergency ranged-disengagement role;
- Ranger performs only a short hop away from the immediate threat;
- a smoke effect is conceptually created at Ranger's departure position;
- nearby enemies become Feared rather than Knocked Back / Stunned;
- Fear prevents attacks, support actions and new Ability activations;
- Feared enemies remain mobile;
- their primary movement direction is opposite Ranger's hop direction;
- a weaker lateral steering force guides them toward the centre line of that
  direction;
- this should turn scattered close-range enemies into a narrower retreating
  formation that Ranger can exploit with Power Shot;
- normal combat resumes when Fear expires;
- the failed route-constrained / safe-destination Backstep experiments should be
  removed rather than retained as parallel tactical-navigation infrastructure.

Power Shot:

- the three-shot burst remains;
- the fan spread becomes the baseline firing pattern;
- piercing, Knockback and Stun remain;
- Fan Volley is parked from normal Reward generation because playtesting showed
  its spread behaviour belongs in the baseline Ability;
- preserve the Fan Volley proof assets for later redesign / reassessment.

###### 3.0O3B4D2 — Punchy Playtest Corrections ✅

**Status: Complete**

Correct the Punchy issues identified during comparative playtesting.

Blink:

- activation position becomes the fixed Sigil Centre for the cast;
- create an opening radial Knockback + strong Stun window;
- traverse valid enemies from that created space;
- damage is delivered during traversal;
- Punchy remains invulnerable throughout the sequence;
- return to the exact activation position when traversal completes.

War Cry:

- substantially reduce its excessive early cooldown;
- preserve stronger low-Health healing / Armour identity;
- prevent flat Armour from making weak valid attacks completely harmless.

Global Armour working rule:

after Invulnerability and Shields,
a positive damaging hit that reaches Armour should currently deal at least
1 Health damage.

This minimum-damage rule remains subject to reassessment once the enemy roster
contains broader damage profiles.

Whirlwind remains unchanged initially because the first playtest found the
Ability itself enjoyable.

Reassess Whirlwind only after Blink and War Cry are corrected.

###### 3.0O3B4D3 — Comparative Core-Kit Re-test ✅

**Status: Complete**

Comparative re-testing passed.

Punchy and Ranger now present distinct, useful core kits.

Punchy:

Blink
→ Signature battlefield control / traversal / return.

Whirlwind
→ immediate close-range breathing room.

War Cry
→ reactive recovery and Armour.

Ranger:

Power Shot
→ ranged formation payoff.

Smoke Step
→ disengagement plus tactical enemy herding.

Guard
→ persistent Shield defence plus healing over time.

The corrected kits are sufficiently coherent to move from isolated Ability
review into whole-Character buildcraft integration.

Re-test Punchy and Ranger after the evidence-driven corrections.

Close O3B only when:

- each core button solves a distinct useful combat problem;
- Ranger remains spatially coherent with authored Encounter geography;
- Shield progression is readable and bounded;
- baseline Power Shot is immediately satisfying;
- Blink reads clearly as Punchy's Signature;
- War Cry creates a useful survival decision without trivialising incoming
  damage.

## 3.0O4 — Character Buildcraft Foundation & Content Review

**Status: Active**

Move from isolated Character / Weapon Mastery / Ability foundations into a
coherent buildcraft game.

The current architecture supports several useful progression systems, but much
of the actual Reward and Mastery content remains prototype-scale.

Strengthen the buildcraft foundation and content in deliberate stages before
performing the final integrated review.

### 3.0O4A — Buildcraft Inventory & Classification ✅

**Status: Complete**

Establish a trustworthy map of the current buildcraft surface.

Classify:

- Character-specific content;
- shared / generic content;
- reusable runtime primitives;
- persistent progression;
- Expedition progression;
- Reward delivery;
- current authoring support;
- prototype / placeholder content;
- missing content paths.

Do not expand content during this step.

### 3.0O4B — Buildcraft Authoring Hygiene ✅

**Status: Complete**

Remove authoring friction before substantially increasing content volume.

Keep the Content Workbench as an editor for the real project assets rather than
creating a second content database.

#### 3.0O4B1 — Mastery Content Lifecycle Tools ✅

**Status: Complete**

Improve normal Mastery authoring so content can be safely iterated without
manual ScriptableObject-list repair.

Implement:

- Add Existing Node;
- Remove From Tree without deleting the node asset;
- Delete Node Asset with explicit confirmation;
- automatic prerequisite cleanup when a node leaves the tree;
- cleanup of direct Mastery-combination references;
- explicit cleanup of missing serialized node / prerequisite references;
- Undo support for non-destructive membership changes;
- non-obstructive validation after each operation.

Permanent asset deletion remains explicitly destructive and is not treated as an
Undo operation.

#### 3.0O4B2 — Reward Content Lifecycle Tools ✅

**Status: Complete**

Bring the same lifecycle quality to Run Reward authoring.

Review:

- deleting Reward assets through the Workbench;
- removing all Set memberships before deletion;
- deleting Reward Set assets safely;
- clear distinction between membership removal and asset deletion;
- explicit confirmation for destructive operations;
- validation and selection recovery after content changes.

### 3.0O4C — Run Reward Foundation & Balance Pass ✅

**Status: Complete**

Turn the current prototype Reward pool into a meaningful Expedition buildcraft
layer.

#### 3.0O4C1 — Rarity Generation Foundation ✅

**Status: Complete**

Make Reward rarity affect actual offer generation.

Introduce an authorable Catalogue rarity profile.

Reward selection should:

1. build the normal eligible candidate pool;
2. determine which rarity tiers are represented by those candidates;
3. roll a rarity using the Catalogue profile;
4. select an eligible Reward from that rarity;
5. remove that offer from the current choice;
6. repeat until the requested offer count is reached.

Unavailable rarity tiers should not consume probability.

Adding more Rewards of one rarity should not by itself increase that rarity's
overall chance of being rolled.

Initial working profile:

- Common: 60;
- Uncommon: 25;
- Rare: 10;
- Epic: 4;
- Legendary: 1.

These are balance values rather than permanent design constants.

#### 3.0O4C2 — Character Growth Reward Rebuild ✅

**Status: Complete**

Replace the current exaggerated prototype Character Growth pool with a broader,
coherently balanced first production pass.

Establish:

- rarity/value expectations;
- safe generic picks;
- offensive development;
- defensive development;
- utility development;
- build-directed stats;
- Shield Capacity progression where useful;
- multiple Rewards per useful stat where appropriate.

Do not make every Reward build-defining.

A healthy pool should contain both reliable general value and stronger
directional opportunities.

#### 3.0O4C3 — Reward Choice Quality & Rerolls ✅

**Status: Complete**

Improve the quality of individual Reward decisions.

Review:

- duplicate / repetitive choice pressure;
- offer diversity;
- Reward eligibility;
- pending-choice rerolls;
- Expedition-owned reroll count;
- reroll presentation;
- avoiding immediate repetition where enough alternatives exist.

Rerolls should regenerate the same Reward source / vector rather than changing
what kind of Reward was earned.

#### 3.0O4C4 — Reward Balance Playtest ✅

**Status: Complete / Detailed Balance Deferred**

Completed the first structured playtest of the rebuilt Character Growth Reward
pool.

Initial findings:

- Common Reward usefulness was mixed;
- rarity power felt good;
- build direction remained a problem;
- choice quality was mixed;
- reroll quality felt good;
- progression balance remained mixed.

The pass established that several individual Character Growth stats cannot yet
be judged usefully in isolation.

Some stats function correctly without yet producing sufficiently meaningful
gameplay decisions because their value depends on the wider Ability, enemy,
combat and buildcraft ecosystem.

Detailed stat tuning is therefore deliberately deferred.

Do not block broader prototype development on isolated numerical Reward changes.

Revisit:

- individual stat value and gameplay feedback;
- broader Character-stat vocabulary;
- preparation versus Expedition power;
- directional Reward quality;
- stat / Ability interactions;

during the later integrated O4G buildcraft pass after the Ability and Mastery
systems contain substantially more representative content.

### 3.0O4D — Elemental Ability & Active/Passive Buildcraft Expansion

**Status: Active**

Purpose:

Turn Abilities from a small proof set into a substantial Expedition buildcraft
axis.

O4D establishes:

- shared elemental Active Abilities;
- Ability-oriented Character stats;
- three additional Expedition Active Ability positions;
- explicit Active Ability loadout ownership;
- Ability selection and swapping;
- reusable combat primitives required by the Ability pool;
- Passive Abilities that consume no Active position;
- enough Ability content for materially different Expedition builds.

Target Active loadout:

Prepared Core
- Signature
+ Utility
+ Sustain

plus:

Expedition Active
- Slot 1
+ Slot 2
+ Slot 3.

Maximum simultaneously active Abilities:

6.

Ability ownership and active-slot assignment are distinct concepts.

A RunBuild may own an Expedition Ability without that Ability currently occupying
one of the three Expedition Active positions.

Passive Abilities are separate from Active Abilities.

Passive Ability
→ automatically active while owned
→ consumes no Active Ability position
→ may be continuous, periodic, reactive or event-driven.

#### 3.0O4D1 — Ability Damage, Area & Element Foundation ✅

**Status: Complete**

Establish the first shared Ability-oriented Character-stat semantics.

Implement:

- Ability Damage;
- Ability Area Size;
- actor-generic Ability stat application;
- Prepared + Expedition modifier support;
- runtime reconstruction;
- Character Growth Reward support;
- elemental Ability identity.

Confirmed damage semantics:

Basic Attack Damage
→ autonomous Basic Attack damage.

Ability Damage
→ damage explicitly produced by Active and future Passive Abilities.

Ability base damage remains authored by the Ability and its level.

Ability Damage does not implicitly scale from Basic Attack Damage.

Universal outgoing-damage modifiers may still affect both where explicitly
intended.

Ability Area Size modifies compatible linear area dimensions such as:

- radial effect radius;
- aura radius;
- effect width where explicitly opted in.

It does not automatically modify:

- movement distance;
- projectile travel range;
- Basic Attack range;
- Knockback distance;
- targeting/search range;
- projectile collision leniency.

Elemental identity initially supports:

- Ability identity;
- authoring;
- presentation;
- future synergies.

Do not introduce elemental resistances or a universal elemental damage system
during D1.

#### 3.0O4D2 — Active Ability Capacity, Loadout & Swapping ✅

**Status: Complete**

Establish:

- three Prepared/Core Active positions;
- three Expedition Active positions;
- RunBuild Ability ownership separate from active-slot assignment;
- automatic assignment into empty Expedition positions;
- owned-but-unslotted Expedition Abilities;
- paused/build-time Ability swapping;
- a fixed six-position Ability HUD;
- empty Expedition-slot presentation;
- safe runtime refresh after acquisition, levelling or swapping.

Prepared/Core positions remain fixed during the Expedition.

Expedition positions may be changed deliberately outside normal live combat.

Unslotted owned Abilities retain their Run-owned levels and development.

#### 3.0O4D2A — Diamond Ability HUD Presentation ✅

**Status: Complete**

Replace the temporary text-heavy six-slot Ability HUD presentation with the
intended compact icon-driven diamond presentation.

The six-slot gameplay model remains unchanged:

Prepared/Core
→ Signature
+ Utility
+ Sustain

Expedition
→ Slot 1
+ Slot 2
+ Slot 3.

Each occupied Ability position should communicate primarily through:

- a central Ability icon;
- current Ability level;
- a small Ability name;
- cooldown countdown;
- a faded cooldown wash that recedes as the Ability becomes ready.

Empty Expedition positions remain visibly present.

The combat HUD should no longer display large slot-role/status text inside each
Ability button.

The current bottom-centre Ability HUD position remains Working during this pass
so the new visual language can be assessed before wider Combat HUD
repositioning.

Do not reposition or redesign the Weapon Mod HUD during D2A.

#### 3.0O4D3 — Shared Ability Combat Primitives ✅

**Status: Complete**

Implement only generic primitives required by approved Ability content.

Initial requirements:

Root
→ prevents normal navigation while still allowing forced movement to resolve.

Temporary Basic Attack Range
→ supports Wind-based Basic Attack transformation.

Temporary Basic Attack Penetration
→ allows compatible Basic Attacks to affect additional aligned targets without
assuming projectile-only behaviour.

Enemy-group targeting
→ resolves useful hostile-group centres for Abilities such as Earthshatter.

Prefer extending existing combat, movement, projectile and runtime-modifier
seams rather than creating parallel systems.

#### 3.0O4D4 — Elemental Active Ability Wave 1 ✅

**Status: Complete**

Implement the first three confirmed shared elemental Expedition Abilities.

Earthshatter — Earth
→ rush toward a useful hostile-group centre
→ damage + Knockback enemies contacted during movement
→ land with a burst of Ability damage + radial Knockback
→ Root surviving affected enemies after forced motion resolves.

Wind Whip — Wind
→ temporary Basic Attack Speed increase
→ temporary Basic Attack Range increase
→ temporary Movement Speed increase
→ Basic Attacks gain additional penetration.

Storm Field — Lightning
→ mobile storm centred on the Character
→ persists for an authored duration
→ repeatedly strikes valid enemies inside the area
→ deals Ability damage.

#### 3.0O4D5 — Elemental Active Ability Wave 2 ✅

**Status: Complete**

Expand the shared elemental Expedition Ability pool from three to six
mechanically distinct Active Abilities.

##### Wall of Flames — Fire

**Status: Confirmed**

Role:

persistent positional damage / approach denial.

Activation:

resolve the current hostile approach direction
→ create a stationary Wall of Flames across that path
→ wall persists for an authored duration
→ enemies entering or remaining inside the wall are set on fire
→ Burn continues briefly after leaving the wall.

The wall should deliberately work with the current simple autonomous movement
model, where hostile actors commonly approach the Character directly.

Wall of Flames should not currently require sophisticated player pathing or
enemy navigation decisions.

Build interactions:

- Ability Damage increases Burn damage;
- Ability Area Size increases wall length / thickness;
- Ability Cooldown Recovery increases availability.

The earlier Flame Wake movement-trail concept remains a possible future design
to revisit when autonomous movement and pathing become substantially richer.

##### Glacial Aegis — Ice

**Status: Confirmed**

Role:

reactive defence / control.

Activation:

gain Glacial Shield layers
→ when one of those specific shield layers breaks
→ release a Frost Nova around the Character
→ nearby enemies take Ability damage
→ affected enemies are Slowed.

Required shared foundations:

- source-aware Shield layers / Shield-break notification;
- generic temporary Slow.

Build interactions:

- Shield Capacity controls how many Shields can coexist;
- Ability Damage increases Frost Nova damage;
- Ability Area Size increases Frost Nova radius;
- Ability Cooldown Recovery increases availability.

##### Tidal Vortex — Water

**Status: Confirmed**

Role:

persistent fixed-position battlefield grouping.

Activation:

resolve a useful hostile group
→ capture that world-space centre
→ create a temporary Tidal Vortex there
→ repeatedly pull nearby enemies toward the centre.

The Vortex remains at its captured world position rather than following the
Character.

Direct damage should remain absent or negligible during this first
implementation so battlefield control remains its primary identity.

Build interactions:

- Ability Area Size increases Vortex radius;
- Knockback Power increases pull distance;
- Ability Cooldown Recovery increases availability.

Wave 2 implementation may begin.

#### 3.0O4D6 — Elemental Active Ability Wave 3

**Status: Active / Designs Working**

Expand toward approximately nine meaningful shared Expedition Active Abilities.

Current Working candidates:

Ember Wisps — Fire
→ temporary autonomous offensive entities.

Solar Brand — Light
→ mark a priority target and build toward an area payoff.

Meteorfall — Fire / Earth
→ delayed high-impact area strike rewarding setup and enemy control.

The target is a varied buildcraft pool rather than an arbitrary Ability count.

#### 3.0O4D7 — Passive Ability Foundation

**Status: Planned**

Introduce Expedition-owned Passive Abilities separately from Active Abilities.

Passive Abilities:

- require no manual activation;
- consume no Active Ability position;
- remain active while owned and valid;
- may be continuous, periodic, reactive or defensive;
- reset with the Expedition unless a later persistent system explicitly prepares
  them.

Working first-content directions:

- Static Discharge;
- Permafrost Aura;
- Earthen Ward;
- Cinder Retort.

The architecture should support deep builds that increasingly automate offence,
defence and battlefield control without making early Expeditions hands-off by
default.

#### 3.0O4D8 — Ability Reward Pool & Integrated Ability Playtest

**Status: Planned**

Verify:

- both Characters can acquire shared elemental Abilities;
- shared Abilities behave correctly on different actors;
- three additional Expedition Active positions are enforced;
- owned but unslotted Abilities retain Run development;
- loadout swapping behaves predictably;
- Passive Abilities consume no Active positions;
- Ability Damage and Area Size produce useful build decisions;
- duplicate ownership behaves correctly;
- Ability choices create different Expedition directions;
- the six-position HUD remains readable;
- temporary Ability state survives Region transitions;
- Expedition Ability ownership resets correctly.

### 3.0O4E — Prepared Ability Selection & Persistent Ability Unlocks

**Status: Planned**

Connect Character Mastery to starting-Ability choice.

Characters retain three Prepared/Core roles:

- Signature;
- Utility;
- Sustain.

Ability definitions remain actor-generic and do not permanently own those roles.

Character Mastery
→ unlocks an Ability as a prepared option for a Character / role.

Preparation
→ selects among unlocked options.

New Expedition
→ snapshots that prepared loadout into RunBuild.

Develop:

- persistent prepared-Ability unlock ownership;
- Character / role compatibility;
- selected starting loadout persistence;
- Preparation UI for selecting unlocked options;
- locked/unlocked presentation.

### 3.0O4F — Character Mastery Progression & Punchy/Ranger Expansion

**Status: Planned**

Strengthen persistent Mastery and replace prototype-scale Punchy/Ranger trees.

Review:

- rankable Mastery nodes;
- increasing costs;
- prepared Ability starting levels;
- prepared Ability unlocks;
- prepared Weapon Mastery levels;
- Character stats;
- Ability-oriented stats;
- Character-specific progression;
- resource-tier pacing.

Punchy and Ranger should gain several meaningful progression routes rather than
one obvious linear path.

### 3.0O4G — Integrated Buildcraft Playtest & Balance

**Status: Planned**

Play developed Punchy and Ranger Expeditions using:

- Character Growth;
- Active Abilities;
- Passive Abilities;
- Active Ability loadouts;
- Weapon Masteries;
- prepared Ability choices;
- Character Mastery.

Confirm that:

- preparation establishes direction without completing the build;
- Expedition Rewards develop or pivot that direction;
- three Core plus three Expedition Active positions create meaningful capacity
  decisions;
- Passive Abilities provide useful automation;
- elemental Abilities create distinct gameplay;
- Ability Damage and Area Size support Ability-focused builds;
- Basic Attack, Ability, control, defensive and hybrid builds can coexist;
- individual Character Growth stats provide meaningful gameplay value;
- the stat vocabulary is broad enough for the builds now supported;
- safe generic choices remain useful;
- build-defining choices create meaningful pivots;
- Weapon Masteries and Abilities retain distinct roles;
- several builds are viable;
- Characters remain recognisably different;
- rarity and rerolls improve decisions rather than trivialising them.

Use O4G to revisit the balance issues intentionally deferred from O4C4.

### 3.0O Review Gate

> Do Characters, Weapon Masteries, Abilities, Rewards and Mastery progression
> now create enough identity, choice and flexibility to justify moving the
> development frontier into richer combat content?

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