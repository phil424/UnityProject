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

# Initial Milestone Boundary

The 3.0J Workshop MVP should prove:

- dedicated workshop scene;
- isolated real encounter;
- Approach scenario;
- On Top scenario;
- Pre-Spawned / Spawn-On-Arrival configuration;
- Arrive While Pursued scenario;
- one-click or otherwise rapid replay;
- real combat systems.

World Event presets, build presets and telemetry may be added immediately if
cheap, but should not delay the first useful version.