# Level Authoring and Encounters — Technical Design

Status: Working

Related Design:
- World/Expeditions
- World/Ecology
- Enemies
- Combat/Combat
- UI-UX

# Purpose

Provide a scene-authorable framework for creating regions and encounters without
hard-coding individual enemy types, spawn locations, spawn rhythms or encounter
behaviour into StageDirector.

# Authoring Model

Runtime responsibilities remain separated, but ordinary scene authoring is
encounter-rooted.

The normal authoring unit is:

LevelEncounter
    ├── LevelSpawnGroup
    ├── LevelSpawnSource
    └── optional encounter triggers

For simple encounters, all three core components may live on the same
GameObject.

Example:

Old Cemetery
    LevelEncounter
    LevelSpawnGroup
    LevelSpawnSource

More complex encounters may use children:

Church Siege
    LevelEncounter

    Door Horde
        LevelSpawnGroup
        LevelSpawnSource

    Grave Reinforcements
        LevelSpawnGroup

        Grave Left
            LevelSpawnSource

        Grave Right
            LevelSpawnSource

    Approach Trigger
        LevelEncounterProximityTrigger

The hierarchy expresses ownership.

`LevelEncounter` automatically discovers `LevelSpawnGroup`s for which it is the
nearest parent encounter.

`LevelSpawnGroup` automatically discovers `LevelSpawnSource`s for which it is
the nearest parent spawn group.

Ordinary authoring should not require manually maintaining duplicate arrays of
encounters, groups and sources.

`StageDirector` receives one `Encounters Root` reference and discovers the
encounters and spawn groups beneath it.

# Authoring Principle

Runtime separation does not require authoring separation.

Keep separate runtime responsibilities when they protect flexibility, but hide
plumbing that can be inferred safely from scene hierarchy.

Prefer:

one understandable encounter root
        ↓
local components / children
        ↓
automatic ownership discovery

over:

encounter
        ↓ manual reference
spawn group
        ↓ manual reference
spawn source
        ↓ separate StageDirector reference

A common encounter should be understandable by selecting its root GameObject
and inspecting its immediate children.

# Core Separation

## WHERE

`LevelSpawnSource`

Defines valid physical spawn positions/areas.

Current shapes:
- Point
- Circle
- Box

## WHAT + SPAWN RHYTHM

`LevelSpawnGroup`

Defines:
- actor definitions;
- counts;
- initial delays;
- batch sizes;
- time between individual spawns;
- time between batches.

Multiple entries may overlap to create compound spawn rhythms such as:
- an immediate burst;
- a long stream;
- delayed elites;
- repeated waves.

## WHEN TO ENTER THE WORLD

`LevelSpawnGroup.BeginSpawning()`

Starting a spawn schedule is separate from combat engagement.

Future triggers and the level schedule may invoke this independently.

## WHEN TO ENGAGE IN COMBAT

`LevelSpawnGroup.ActivateCombat()`

Combat engagement is independent from whether actors already exist.

A spawned group may therefore be physically present but dormant.

`CombatEngagementState` controls whether an actor participates in normal
autonomous combat targeting.

Component absence currently means combat-active for backwards compatibility.

When a group becomes combat-active:
- already-spawned members become engaged;
- actors spawned later inherit the active state.

## HOW TO ENTER

Future spawn-presentation components.

Examples:
- church doorway;
- ground/grave burst;
- portal;
- tunnel;
- map edge.

Spawn presentation must remain separate from spawn location, schedule and combat
engagement.

## PLAYER-FACING IDENTITY

`LevelEncounter`

A `LevelEncounter` represents a meaningful player-understandable threat or
opportunity independently from low-level spawn machinery.

Current authored data includes:
- stable identity;
- display name;
- description;
- world-space anchor;
- initial knowledge;
- initial availability;
- one or more `LevelSpawnGroup` references.

# Encounter State Axes

Encounter lifecycle state is intentionally stored as independent concepts rather
than one mutually-exclusive gameplay enum.

## Known

`IsKnown`

Answers:

> Does the player currently know this encounter exists?

Unknown encounters may exist in the authored level without appearing in normal
player-facing encounter UI.

## Available

`IsAvailable`

Answers:

> Is the encounter currently accessible/selectable as an opportunity?

An encounter may be known but unavailable.

Example:

Nobleman's Procession
- known;
- rare;
- visible in the schedule;
- currently locked behind an Elite encounter.

## Completed

`IsCompleted`

Answers:

> Has this encounter been successfully completed?

For current spawn-group encounters this is derived when every configured group
has completed spawning and has no living actors.

## Expired

`IsExpired`

Answers:

> Has the opportunity window for this encounter ended?

Expiry is independent from completion.

The exact gameplay effect of expiry on an encounter that is already active is
still unresolved.

# Presentation State

`EncounterPresentationState` is derived for player-facing/debug presentation.

Current values:
- Unknown
- Locked
- Available
- Active
- Cleared
- Expired

Presentation state is not the authoritative lifecycle storage.

For example:

IsKnown = true
IsAvailable = false
IsExpired = false

derives:

Locked

`IsSelectable` currently requires:
- known;
- available;
- not completed;
- not expired.

# Encounter Commands and Actions

`LevelEncounterCommands` represents the common authored operations that can be
performed on an encounter:

- make known;
- make available;
- begin spawning;
- activate combat;
- expire.

Commands operate at encounter level and forward spawning/combat changes to the
encounter's owned spawn groups.

`LevelEncounterActions` contains one or more target encounter + command entries.

Example:

On Opening Horde Completed

Target:
    Reinforcements

Commands:
    Make Available

Future systems such as the level scheduler should reuse the same encounter
commands rather than manipulating spawn groups directly.

Low-level `LevelSpawnGroup.BeginSpawning()` and `ActivateCombat()` remain
runtime primitives but are no longer the normal scene-authoring interface.

# Encounter Completion Links

`LevelEncounter` owns its authored `On Completed` actions directly.

This avoids requiring a separate completion-trigger component whose only purpose
is to reference the encounter it already sits beside.

Example:

Elite Gauntlet
    LevelEncounter

    On Completed:
        Nobleman's Procession
            Make Available

The generic `Completed` runtime event remains available for systems that need to
observe completion programmatically.

# Current Runtime State

`StageDirector` distinguishes:

`LivingMinions`
- actors physically alive now.

`PendingMinionSpawns`
- actors committed to a spawn schedule but not yet spawned.

`UnstartedMinionSpawnGroups`
- currently required authored groups whose spawn schedule has not begun.

`LevelEncounter.State`
- player-facing runtime encounter state.

`LevelEncounter.AnchorPosition`
- world-space destination for future encounter direction, minimap and navigation.

`StageDirector.Encounters`
- current runtime seam through which future player-facing systems can discover
  the authored encounters for the current level.

These concepts must not be conflated.

# Important Invariants

Spawning and combat engagement are separate.

A spawned actor may exist without participating in combat.

Disengaged actors are excluded from normal autonomous combat target selection
in both directions.

Activating a group's combat affects both existing members and future members.

Unstarted authored groups are not currently scheduled spawns.

Level completion must not depend only on enemies currently alive.

Spawn presentation must not be encoded as a giant spawn-type enum.

Combat threat and reward/opportunity rarity are separate concepts.

Low-level spawn groups are not the same thing as player-facing encounters.

Trigger condition and trigger response are separate.

A proximity trigger does not inherently mean "spawn and activate".

Spawn-group actions should be reusable by future schedule and encounter systems.

# Proximity Trigger

`LevelEncounterProximityTrigger` is normally placed on the encounter itself or
on a child positioned at the desired trigger location.

It automatically controls its nearest parent `LevelEncounter`.

Its authored commands may independently:

- make the encounter known;
- make it available;
- begin spawning;
- activate combat;
- expire it.

A trigger can optionally require the encounter to already be available.

This allows a locked encounter to have a world-space trigger without that
trigger prematurely starting the encounter.

Proximity remains only one future trigger source. Scheduler, ecology and
objective systems should eventually use the same encounter-command vocabulary.

# Future Runtime Flow

Spawn trigger / Level Schedule
        ↓
BeginSpawning()
        ↓
LevelSpawnGroup
        ↓
LevelSpawnSource
        ↓
future Spawn Presentation
        ↓
Actor exists
        ↓
actor inherits group's current CombatEngagementState

Separate trigger / schedule event
        ↓
ActivateCombat()
        ↓
existing + future group members become combat-active

# Future Encounter Direction

Player selects `LevelEncounter`
        ↓
party receives encounter/travel directive
        ↓
party travels toward encounter
        ↓
encounter spawn/engagement rules operate
        ↓
targeting policy chooses actors relevant to combat

When no combat target exists, encounter/travel intent should eventually provide
a movement destination instead of leaving the party stationary.

# Future Targeting Direction

Unit targeting and encounter direction are separate layers.

Target resolution should eventually consider:

candidate eligibility / combat engagement
        ↓
selected encounter or travel directive bias
        ↓
base target rule
        ↓
priority modifiers
        ↓
fallback / tie-break

Possible base rules:
- Closest
- Weakest
- Strongest

Possible composable priorities:
- Prefer Elite
- Prefer Boss
- Prefer Rare / Valuable

Avoid representing every combination as one giant targeting enum.

# Future Schedule Direction

A level-wide schedule may issue commands into existing seams rather than
implementing spawning itself.

Examples:

Zombie Surge
- activate already-spawned hostile groups;
- begin additional reinforcement spawn groups.

Rare Sighting
- make a valuable encounter available;
- surface it to the player through encounter UI.

# Known Limitation

All current `StageDirector.MinionSpawnGroups` are treated as required encounter
work.

A dormant required group can therefore prevent minion-phase completion if the
party never reaches its trigger.

This is intentionally not solved through fake pending-spawn counts or oversized
aggro ranges.

Future encounter identity and encounter-directed movement should distinguish
required, optional and opportunistic encounters and provide travel intent when
no combat target exists.

`LevelEncounter` does not yet control level-completion semantics.

`StageDirector.MinionSpawnGroups` are still treated as required work separately
from encounter identity.

Encounter-directed movement is also not implemented yet.

A party can therefore still become stationary with no valid combat target while
an unstarted required group exists elsewhere.

# Open Questions

- Required versus optional encounter semantics.
- Encounter discovery rules.
- Encounter completion conditions.
- Ambient movement for disengaged enemies.
- Whether damage against a disengaged actor automatically engages it.
- Exact level-schedule authoring UI.
- Whole-party versus per-character encounter direction.
- How much encounter information is visible before discovery.