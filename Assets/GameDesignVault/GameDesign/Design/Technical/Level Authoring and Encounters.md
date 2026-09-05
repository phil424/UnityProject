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

# Encounter Actions

`LevelEncounterActions` is a reusable authored command set.

Current actions:
- make encounter known;
- make encounter available;
- expire encounter.

It should be reusable by:
- encounter completion triggers;
- future scheduler events;
- ecology events;
- objectives;
- debug tooling.

Trigger source and trigger response remain separate.

# Encounter Completion Links

`LevelEncounterCompletionTrigger` listens for one encounter completing and
executes configured:

- `LevelEncounterActions`;
- `LevelSpawnGroupActions`.

This allows authored relationships such as:

Elite Gauntlet
        ↓ complete
Nobleman's Procession
        ↓ make available

without embedding prerequisite references directly into `LevelEncounter`.

Encounter relationships should remain composable rather than requiring every
encounter to belong to a fixed linear dependency tree.

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

# Spawn Group Actions

`LevelSpawnGroupActions` is the reusable authored command set for manipulating
spawn groups.

It currently supports two independent actions:

- begin spawning;
- activate combat.

The same group may appear in both lists.

Combat activation is applied before spawning begins so an immediately spawned
actor inherits the intended engagement state.

This action set should be reusable by future systems such as:

- proximity triggers;
- level-schedule events;
- encounter events;
- global ecology events;
- debug tooling.

The trigger determines **when** the actions occur.

`LevelSpawnGroupActions` determines **what group state changes occur**.

# Proximity Trigger

`LevelSpawnProximityActivator` detects when a living party member enters an
authored radius and then executes its configured `LevelSpawnGroupActions`.

A proximity trigger may therefore:

- begin spawning without activating combat;
- activate already-spawned actors without beginning a spawn schedule;
- perform both;
- affect multiple groups at once.

Proximity is only one trigger source and should not be baked into the spawn
group itself.

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