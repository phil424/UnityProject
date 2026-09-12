# Encounter Workshop — Technical Design

**Status: Working**

Related:
- [[Level Authoring and Encounters]]
- [[Expedition Scheduling and Encounter Generation]]
- [[Targeting, Tactics and Encounter Direction]]
- [[World Events]]

# Purpose

The Encounter Workshop is a dedicated designer iteration environment for
building, isolating, replaying and evaluating individual encounters.

Its primary question is:

> What does this encounter actually feel like under the different ways a player
> may arrive at it?

The strategic expedition can expose encounter-design problems, but it is too
slow and uncontrolled for rapid encounter tuning.

The workshop should allow repeated encounter iteration without requiring:

- a full expedition start;
- waiting for World Time;
- travelling across the complete map;
- satisfying unrelated prerequisites;
- clearing other encounters;
- waiting for encounter supply;
- reproducing a specific setup manually every run.

# Core Principle

The workshop must use the real runtime gameplay systems.

It should not implement a parallel fake combat simulator.

Where practical it should use the same:

- `LevelEncounter`;
- `LevelSpawnGroup`;
- spawn sources;
- `StageDirector`;
- actors;
- `Health`;
- `AbilitySystem`;
- targeting;
- movement;
- ForcedMotion;
- combat engagement;
- reward hooks.

The workshop owns test orchestration.

It does not own alternate encounter behaviour.

# Initial Workshop MVP

The first useful version should provide a dedicated:

`EncounterWorkshop.unity`

scene.

The scene should contain:
- minimal runtime systems;
- party test spawn;
- one workshop encounter root;
- camera;
- scenario controller;
- rapid reset / replay controls.

The first workshop does NOT require the final data-driven Encounter Definition
architecture.

A scene-authored encounter may initially be placed / duplicated into the
Workshop encounter root.

Later data-driven Encounter Definitions should make loading an arbitrary
encounter substantially easier.

# Scenario Harness

The workshop should support controlled starting scenarios.

## Approach

Party begins a configurable distance away.

Example:

Party
──────────── 15m ────────────
Encounter

The encounter is selected and normal encounter travel occurs.

Use this to evaluate:
- readability while approaching;
- spawn spectacle;
- pacing before first contact;
- whether enemies appear too early / too late.

## Threshold

Party begins immediately outside the encounter's activation / meaningful
engagement boundary.

Use this to repeatedly evaluate the transition from:

Travel
→
Encounter Begins.

## On Top

Party begins at or very near the encounter anchor before the encounter starts.

This represents:
- rapid redirection;
- unusual approach angles;
- encounter overlap;
- player arriving faster than expected.

Use this to discover encounters that only work when given generous setup time.

## Pre-Spawned Dormant

Encounter enemies exist before the test begins but combat is inactive.

Then combat activation occurs through the normal encounter path.

Use this for:
- ambushes;
- crowds;
- spectacles;
- pre-existing world populations.

## Spawn On Arrival

Encounter begins unspawned.

Reaching / starting the encounter causes its spawn sequence.

Use this for:
- ground bursts;
- door openings;
- reinforcement arrivals;
- wave spectacles.

## Arrive While Pursued

Party starts with one or more already-engaged enemies pursuing it.

The party then reaches the encounter.

Use this to evaluate:
- encounter stacking;
- chaos readability;
- difficulty under real strategic redirection;
- whether authored spawn timing survives unexpected pressure.

This scenario is particularly important because normal strategic play explicitly
allows enemies from abandoned encounters to pursue the hero.

## World Event Active

Begin the scenario with a selected World Event already active.

Examples:
- Zombie Surge;
- future Fiery Zombies.

Use this to evaluate encounter behaviour in altered expedition conditions.

This does not require every World Event to modify the encounter directly.

# Build / Party Presets

Long-term scenario setup should be able to select a prepared test state.

Examples:

Baseline
- default character;
- default persistent equipment;
- starting abilities.

Weak
- deliberately underpowered.

Expected
- approximate power level expected when this encounter commonly appears.

Strong
- stronger temporary expedition build.

Extreme
- stress-test build.

The Workshop should not silently hardcode balance assumptions into encounter
runtime logic.

These are test configurations only.

# Rapid Replay

Encounter iteration must be fast.

Desired flow:

Play Scenario
↓
observe
↓
press Reset / Replay
↓
same deterministic starting configuration
↓
encounter begins again

Where practical replay should reset:
- party;
- health;
- cooldowns;
- runtime actors;
- encounter state;
- spawn timing;
- target state;
- navigation state;
- scenario pursuers;
- relevant World Event state.

A full Unity scene reload may be acceptable for the earliest version if it keeps
implementation simple and reliable.

The important requirement is low iteration friction.

# Scenario Randomness

The workshop should support both:

Deterministic Replay
- same spawn seed / starting setup;
- useful when comparing small tuning changes.

Variable Replay
- normal random variation;
- useful for evaluating robustness.

Exact random-seed tooling can follow the first useful Workshop version.

# Lightweight Telemetry

The primary tool remains observation and feel.

Later useful metrics may include:
- time to first enemy contact;
- encounter duration;
- peak living enemy count;
- party damage taken;
- lowest party health;
- enemies spawned;
- enemies killed;
- reward output.

Do not turn encounter authoring into a spreadsheet exercise.

Telemetry should help explain what the designer just experienced.

# Authoring Workflow

Desired long-term loop:

Choose / Load Encounter
↓
Choose Scenario
↓
Choose Party / Build Preset
↓
Play
↓
Replay
↓
Tune Encounter
↓
Replay
↓
Satisfied
↓
Use encounter in Expedition

# Relationship to Future Encounter Architecture

The Workshop should exist before the complete generated-encounter architecture.

Future:

Encounter Definition
+
Encounter Site
+
Runtime Encounter Instance

should integrate into the Workshop.

Eventually the Workshop should be able to:

Choose Encounter Definition
↓
Choose compatible Site / test layout
↓
Instantiate runtime encounter
↓
run selected scenario.

Do not postpone encounter-quality iteration until this architecture is complete.

# 3.0J MVP Implementation

The first Workshop implementation uses a dedicated:

`EncounterWorkshop.unity`

scene.

It deliberately reuses the real runtime stack rather than creating a parallel
combat simulator.

## Test Orchestration

`EncounterWorkshopController` owns:
- scenario selection;
- temporary RunState creation;
- party starting position;
- encounter start-mode override;
- pursued-scenario setup;
- reset / replay.

It does not own combat behaviour.

## Arrival Scenarios

Implemented:

### Approach
Party begins a configurable distance from the encounter and receives a real
Encounter Directive.

### Threshold
Party begins immediately outside the real Encounter Directive arrival distance.

### On Top
Party begins directly at the encounter anchor.

### Arrive While Pursued
Party begins at approach distance while already-engaged real enemy actors pursue
from behind.

Pursuers remain separate from the isolated encounter itself.

## Encounter Start Modes

Implemented:

### Spawn On Arrival
Encounter spawn groups begin only when the real Encounter Directive reaches the
encounter.

### Pre-Spawn From Start
The encounter's normal authored spawn schedule begins when the Workshop scenario
starts, but combat remains inactive until the party reaches the encounter.

This mode intentionally preserves authored spawn timing.

It does not mean that every enemy is forcibly instantiated instantly.

A future `Fully Populated` scenario may be added if concrete encounter-design
needs justify it.

## Replay

Replay performs a fresh runtime reconstruction:

Workshop runtime
↓
clear level actors / state
↓
fresh temporary RunState
↓
fresh party actor
↓
fresh encounter state
↓
same scenario begins again.

The first Workshop does not bank persistent rewards or participate in expedition
progression.

## Deterministic Replay

The Workshop may initialize Unity's random state from a configured seed before
starting a scenario.

This allows repeated comparisons of encounter changes under approximately the
same random setup.

Variable replay remains available by disabling deterministic mode.

# Next Workshop Extensions

Add only when they materially improve encounter iteration:

- arbitrary encounter loading;
- party/build presets;
- World Event presets;
- Fully Populated setup;
- approach-angle presets;
- lightweight encounter telemetry;
- deterministic seed controls;
- scenario save presets;
- compatible Encounter Site testing.

The Workshop should grow in response to real encounter-authoring friction rather
than becoming a speculative tool platform.

# Encounter Phase Iteration

Encounter phases are a near-term Workshop requirement.

A phase represents a meaningful authored stage of an encounter.

A phase is not synonymous with a spawn batch.

Example:

Dormant Horde

Phase 1 — Initial Horde
- 4 zombies.

Phase 2 — Second Wave
- 4 zombies;
- scheduled after 8 seconds;
- may start early if Phase 1 is cleared.

This supports a pacing rule:

Scheduled Transition
OR
Current Phase Cleared Early
→
Start Next Phase

The purpose is to remove unnecessary dead time without eliminating authored
pressure when the player is struggling.

Phase progress must be scoped to the owning encounter.

Enemies from overlapping encounters must not prevent or accidentally advance
another encounter's phase.

Future phase completion may depend on:
- owned enemies defeated;
- timers;
- specific targets;
- objectives;
- interactions;
- authored conditions.

The first implementation only needs the concrete enemy-clear / timer proof.

# Encounter Presentation Testing

The Workshop should allow start / phase / completion presentation to be evaluated
under every supported arrival scenario.

Important cases:

Approach
→ does the encounter title appear at the right moment?

On Top
→ is the announcement still readable when combat begins immediately?

Pre-Spawned
→ does merely seeing dormant enemies incorrectly announce the encounter?

Arrive While Pursued
→ can encounter identity/progress still be understood during overlapping combat?

Encounter announcements should be short, non-blocking and capable of overlapping
with other announcements.

The Workshop should become the primary place for tuning this presentation.

# 3.0J2 — Test Build Configuration

The Workshop can configure representative temporary character power before
spawning the test party.

This configuration is test-only.

It does not modify:
- `PersistentProgression`;
- persistent currency;
- equipment progression;
- account/meta state.

Flow:

Workshop Test Build
↓
fresh RunState
↓
fresh RunBuild
↓
apply test configuration
↓
StageDirector spawns party
↓
normal PartyUpgradeApplicator
+
normal PartyAbilityApplicator

Therefore the encounter is still being tested against the real runtime power
systems.

## Current Power Axes

The current project does not have a general Character Level system.

Do not invent one purely for Workshop presentation.

The first Workshop build controls therefore expose the power concepts that
actually exist:

- Weapon Level;
- Armour Level;
- Focus Level;
- individually authored starting Ability Levels.

When future systems become real, the Workshop may also expose:
- equipment-tree states;
- ability acquisition;
- evolutions;
- augments;
- supports;
- prepared loadouts;
- character level if one is introduced.

## Prototype Power Presets

Initial presets provide convenient comparative bands:

Early
- baseline gear;
- authored starting ability levels.

Mid
- approximately 25% of the configured gear test range;
- abilities around 40% of their maximum level.

Late
- approximately 50% of the configured gear test range;
- abilities around 80% of their maximum level.

Extreme
- maximum configured test gear;
- maximum starting ability levels.

These are development conveniences.

They are not final balance definitions for expedition stages.

Direct slider changes move the configuration into `Custom`.

## Replay Rule

Test Build changes are applied when the scenario is reconstructed.

The Workshop does not mutate the already-spawned actor while sliders are being
edited.

This keeps comparisons clear:

configure
↓
Replay
↓
observe one fresh scenario.

# 3.0J3 — Encounter Phases & Adaptive Pacing

The Workshop now supports encounters authored as ordered phases.

The first proof converts the old:

8 zombies
→ two spawn batches
→ fixed 8 second gap

into:

Phase 1
→ 4 zombies

Phase 2
→ 4 zombies

with:

Phase 1 Cleared
OR
8 Seconds Elapsed
→
Start Phase 2

This moves meaningful encounter pacing out of low-level spawn batching and into
an explicit encounter structure.

## Pre-Spawn Behaviour

For phased encounters, Workshop `Pre-Spawn From Start` only starts the first
phase's spawn content.

Phase progression does not advance until encounter combat is activated.

Therefore:

Pre-Spawn
→ Phase 1 may visually exist while approaching.

Arrival
→ combat activates;
→ phase timing begins.

This prevents later phases from silently playing out before the party reaches
the encounter.

## Build Testing

3.0J2 Test Build controls are particularly useful for phased encounters.

Expected comparisons:

Early
→ later phases may overlap because timers are reached.

Late / Extreme
→ early clears may compress downtime and accelerate phases.

The Workshop should be used to decide whether this creates satisfying pacing
before adding more complex phase conditions.

# 3.0J4 — Encounter Presentation Testing

The Workshop now consumes the same encounter announcement system as normal play.

This makes arrival scenarios presentation tests as well as combat tests.

Important validations:

Approach
→ announcement occurs on actual encounter start.

Pre-Spawn
→ dormant visible enemies do not announce early.

On Top
→ announcement remains readable when combat begins immediately.

Arrive While Pursued
→ encounter identity remains understandable during existing combat pressure.

Replay
→ presentation state resets with the encounter.

The Workshop remains the preferred place for tuning future announcement timing,
animation, audio and encounter visual language.

# 3.0J5 — Editable Encounter Draft

The Workshop now maintains an editable runtime `EncounterWorkshopDraft`.

The Draft is initialized from the scene-authored Workshop encounter.

Flow:

Scene Encounter
↓
Capture Draft
↓
Edit Draft
↓
Apply Runtime Overrides
↓
Replay Scenario

Draft editing does not directly modify the serialized scene encounter.

Leaving Play Mode therefore discards unsaved Draft experimentation.

## Initial Editable Surface

3.0J5 exposes:

Encounter identity:
- Display Name;
- Description.

Phase configuration:
- Phase Display Name;
- advance delay;
- Advance When Cleared.

Spawn entry configuration:
- Actor Definition from the Workshop authoring palette;
- Count;
- Start Delay;
- Batch Size;
- Time Between Spawns;
- Time Between Batches.

Spawn-region configuration:
- Point;
- Circle;
- Box;
- Circle Radius;
- Box Width;
- Box Depth.

The first Draft intentionally edits existing encounter structure.

It does not yet:
- add or remove phases;
- add or remove spawn groups;
- add or remove spawn entries;
- add/remove triggers;
- save assets.

Those structural/export operations follow after the Draft data model has been
proven through actual encounter iteration.

## Runtime Override Seams

The Workshop uses explicit runtime override seams on:

- `LevelEncounter`;
- `LevelEncounterPhase`;
- `LevelSpawnGroup`;
- `LevelSpawnSource`.

Normal scene-authored gameplay continues consuming authored values when no
runtime override exists.

These seams provide a bridge for future:

`EncounterDefinition`
↓
runtime encounter construction / configuration

without requiring the Workshop to mutate serialized scene objects.

## Draft State

The Workshop distinguishes:

Source Copy
→ no Draft edits have been made.

Modified — Unsaved
→ Draft differs from its initial source capture.

Preview Current
→ the running scenario was created from the latest Draft revision.

Replay Required
→ Draft has changed since the running scenario was created.

This keeps encounter comparisons deliberate rather than mutating an active fight
halfway through.

# 3.0J6 — Encounter Definition Save / Load

The Workshop Draft may now be persisted into a portable
`EncounterDefinition` ScriptableObject.

Workflow:

Scene Source
↓
Draft
↓
Preview / Replay
↓
Save As
↓
EncounterDefinition.asset

A saved Definition can later be:

Load
↓
Draft
↓
Edit
↓
Replay
↓
Save.

## Source Semantics

When no Definition is loaded:

`Reset From Source`
returns to the scene-authored Workshop encounter.

When a Definition is loaded:

`Reset From Source`
returns to the latest saved state of that Definition.

## Safety

Draft modifications remain unsaved until explicit:

`Save`

or:

`Save As`.

Applying / replaying a Draft does not itself modify the asset.

## Structural Compatibility

J6 intentionally does not dynamically rebuild Workshop hierarchy.

Loaded Definitions must fit the current Workshop site's phase/group/source
structure.

This limitation keeps the first persistence step small while preserving a clear
path toward future Encounter Site / Runtime Instance architecture.

# 3.0J7 — Composable Trigger / Action Authoring

The Workshop can now add/remove portable encounter Rules while Play Mode is
running.

Initial authoring includes:
- approach/proximity rules;
- encounter-start rules;
- delayed rules;
- phase-start rules;
- phase-clear rules;
- multiple ordered actions;
- phase transitions;
- group spawn / activation actions;
- explicit encounter completion;
- generic spectacle Signals.

A convenience:

`Add Approach Start Rule`

creates:

Party Proximity
↓
Activate Encounter Combat
↓
Begin Encounter Spawning.

This is only a template.

The resulting Trigger and Actions remain normal editable Rule data.

Rules participate in the existing:

Draft
→ Replay
→ Save / Save As
→ EncounterDefinition.asset

workflow.

## Precision Numeric Authoring

Workshop slider controls now pair:

Slider
+
Numeric Field.

Sliders use useful snapping increments for rapid iteration.

Direct fields allow exact values.

This prevents serialized encounter assets accumulating accidental values such
as:

`5.042651`

when the intended authored value was:

`5`.

# Dynamic Encounter Structure

The Workshop is no longer limited by the phase count authored into
`EncounterWorkshop.unity`.

A Draft may now:

- start as a fresh one-phase encounter;
- add phases;
- duplicate phases;
- remove phases;
- save arbitrary phase counts;
- reload those Definitions;
- preview them through runtime-generated encounter content.

The default new encounter is intentionally minimal:

1 Phase
1 Spawn Group
1 Spawn Entry
4 Zombies
1 Circle Spawn Region.

This is the baseline from which encounter complexity should be added
deliberately.

The Workshop scene is therefore a test site / harness rather than the structural
template for authored Encounter Definitions.