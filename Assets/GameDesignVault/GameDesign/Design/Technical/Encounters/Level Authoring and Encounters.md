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

# Current Strategic Movement Boundary

Encounter identity and explicit encounter-directed navigation are implemented.

Selecting another Encounter may immediately redirect the party while enemies from
the previous Encounter continue pursuing.

When no Encounter Directive is active:

Ambient Route Navigation
→ owns fallback movement.

Local combat may interrupt Ambient Route Navigation.

Explicit Encounter Navigation remains higher priority than local combat.

3.0K introduces structured Route Geography and destination-oriented ambient
travel.

Explicit Encounter Navigation now follows the Route Graph when the selected
Encounter has an authored `EncounterRouteSite`.

Unbound Encounters retain the previous direct-travel fallback.

That becomes relevant once Encounter Sites are connected to Route geography.

# Future Reusable Encounter Model

The current `LevelEncounter` implementation is scene-authored and is appropriate
for proving encounter behaviour.

Long-running expedition generation may later require three conceptual layers:

Encounter Definition
"What scenario is this?"

Encounter Site
"Where can this scenario occur?"

Runtime Encounter Instance
"This particular occurrence during this expedition."

Fixed spectacles may remain tightly authored:

Church Site
+
Church Outbreak

Generic encounters should be capable of using several compatible authored sites.

Do not refactor the current implementation into these layers until the strategic
expedition prototype provides concrete requirements.

# Route Geography Authoring

Detailed architecture:

[[Route Geography]]

Ambient navigation remains separate from formal Encounter occurrence state.

The primary authoring root is:

Ambient Route
├── Nodes
├── Segments
├── Groups
└── Landmarks.

## Nodes

Nodes are meaningful travel positions.

The GameObject name is the authoring label.

Avoid a duplicate serialized display-name field.

## Segments

Connections between Nodes are first-class `AmbientRouteSegment` objects.

A Segment owns:

Node A
+
Node B.

Initial route traversal is bidirectional.

Segment GameObjects are organisational/data objects.

Keep their Transform at identity.

The endpoint Nodes define their spatial geometry.

## Groups

`AmbientRouteGroup` identifies exact collections of Nodes and Segments.

Groups provide semantic authored geography without requiring a separate runtime
system for every location.

Route Groups are not formal Encounters.

## Validation

After meaningful route edits use:

Ambient Route
→ Ambient Route Graph
→ Debug
→ Validate Route Geography.

Validation should detect:
- unconfigured Segments;
- duplicate Segments;
- isolated Nodes;
- disconnected route islands;
- foreign references;
- invalid Group membership.

## Ambient Activity

Ambient population may exist along Route geography without becoming a formal
Encounter.

Current authoring uses `AmbientActivityArea`.

The common workflow should use:

`Tools -> MiniCrawler -> Route Geography Authoring`

An Activity Area is created from selected:
- Nodes;
- Segments;
- Groups.

The tool then allows the designer to:
- add/update population entries;
- remove population entries;
- set spawn scatter;
- set engagement radius;
- validate the authored footprint.

Generated Area objects remain normally editable through the Inspector.

Prefer Route Groups for broad coherent activity regions.

Prefer one or more Segments for corridor / street population.

Prefer Nodes for genuinely point-like local population.

Do not model every ambient enemy as a `LevelEncounter` merely to reuse Encounter
features.

Formal Encounter Sites and Ambient Activity Areas may coexist on the same Route
geography.

## Encounter Route Site Authoring

3.0K3A introduces `EncounterRouteSite`.

The common workflow should use:

`Tools -> MiniCrawler -> Route Geography Authoring`

rather than manually adding/wiring Site components.

Current workflow:

1. select the target `LevelEncounter`;
2. the Route Authoring tool pins it as `Target Encounter`;
3. select the desired Route Node(s) and/or Segment(s);
4. choose `Bind / Replace Site Footprint From Selection`.

The tool:
- adds `EncounterRouteSite` when missing;
- assigns the Encounter;
- writes the selected footprint;
- preserves Unity Undo;
- leaves the generated component manually editable.

Do not create a separate Site GameObject solely because Route Site and Encounter
Occurrence are conceptually different layers.

The current prototype may co-locate those components while their responsibilities
remain separate.

The long-term Site / Runtime Instance split belongs to generated Encounter
architecture once that split has concrete runtime requirements.

### Traversal Start Policy

A Route Site may enable:

`Start On Route Traversal`.

Use this when physically crossing the Site should itself commit/start the
Encounter occurrence.

Examples:
- road ambush;
- occupied street;
- blockade;
- hostile checkpoint;
- dangerous route corridor.

When enabled, the Site may start during both:
- explicit Encounter-directed route travel;
- passive Ambient Route exploration.

The reaction differs:

Explicit Encounter travel
→ crossed Encounter starts
→ current strategic directive continues.

Ambient exploration
→ crossed Encounter starts
→ ambient navigation pauses until the formal Encounter resolves
→ previous ambient journey resumes.

Disable traversal start when the Site exists only for geography/navigation and
the Encounter should wait for another start mechanism.

Examples may include:
- optional shrine;
- scripted spectacle;
- schedule-only event;
- Encounter with intentionally separate authored trigger semantics.

The currently selected strategic destination is excluded from traversal-start
activation while travelling to it.

Its existing final local start semantics remain authoritative.

This policy is Site-owned and does not modify the source
`EncounterDefinition`.

## Semantic Route Tag Authoring

Route Tags describe geography rather than Encounter identity.

Use:

`Tools -> MiniCrawler -> Route Geography Authoring`

to assign Tags to selected:
- Nodes;
- Segments;
- Groups.

Prefer Group-level Tags when the semantic quality applies to a coherent authored
area.

Example:

Village Well Ring
- Village;
- Well;
- Open.

Prefer Segment-level Tags for connection-specific qualities.

Example:

Village Well road Segments
- Road.

Do not manually duplicate inherited geography Tags onto every Encounter Site.

`EncounterRouteSite` derives its effective Tags from its footprint.

Encounter Definition compatibility with these Tags is introduced separately so
geography authoring does not become coupled to one specific Encounter.

## Encounter Compatibility Preview

The Route Geography Authoring tool can preview an `EncounterDefinition` against
the current graph's Encounter Route Sites.

The compatibility authoring surface provides:

- Allow Automatic Placement;
- Required Tags;
- Preferred Tags;
- Forbidden Tags;
- validation of conflicting placement roles;
- per-Site compatibility reasons;
- preferred-Tag fit;
- ranked compatible candidate Sites.

Setting a Tag role through the tool moves that Tag out of any previous role.

Manual Inspector editing remains supported.

Manual invalid states are reported rather than silently rewritten.

Compatibility preview does not bind or instantiate the Definition.

It is a query/authoring aid only.

# Prototype Reusable Encounter Sites

3.0G introduces a temporary authoring flag:

`Reusable By Prototype Supply`

This marks a scene-authored `LevelEncounter` as eligible to be rearmed after
completion / expiry during the current strategic expedition prototype.

This is not the final generated-encounter architecture.

It should primarily be used on generic encounter sites whose content can
reasonably repeat.

Fixed spectacle encounters should generally remain one-shot unless repetition is
deliberately authored.

For the initial expiry prototype, reusable generic encounters begin unspawned.

This avoids introducing incomplete actor-despawn presentation solely to support
expiry.

Future encounter lifecycle may support:
- pre-spawned dormant encounters that disappear cleanly;
- visual arrival / departure;
- site-specific spectacle;
- generated runtime encounter instances.

# Encounter Workshop

Encounter quality now requires a dedicated rapid-iteration workflow.

See:

[[Encounter Workshop]]

The Workshop is intentionally being promoted ahead of the complete data-driven
Encounter Definition / Site / Runtime Instance architecture.

Reason:

> Encounter feel should be iterated while the strategic MVP is still evolving,
> rather than waiting for the final authoring model.

The Workshop should consume the same encounter/runtime seams used by expedition
play rather than implementing alternate combat behaviour.

# Encounter Phases

An encounter may optionally contain ordered authored phases.

Hierarchy is the authoring API.

Example:

Dormant Horde Encounter
├── Phase 1 - Initial Horde
│   └── Spawn Group
└── Phase 2 - Second Wave
    └── Spawn Group

`LevelEncounterPhase` objects should normally be direct children of the owning
`LevelEncounter`.

Phase order is determined by hierarchy sibling order.

If an encounter contains phases, configured spawn groups should belong beneath a
phase rather than also existing unphased on the encounter root.

## Phase vs Spawn Batch

A phase is a meaningful stage of an encounter.

A spawn batch is low-level spawn rhythm inside a `LevelSpawnGroup`.

Do not use the concepts interchangeably.

Example:

Phase 1 — Courtyard Horde
- Spawn Group A
  - 12 zombies
  - batch size 3
  - 0.5 seconds between batches.

Phase 2 — Elite Arrival
- Spawn Group B
  - one Elite.

## 3.0J3 Phase Progression

The first progression rule supports:

Current Phase Cleared
OR
Configured Timer Reached
↓
Start Next Phase

Example:

Initial Horde
- 4 zombies;
- advance after 8 seconds;
- advance early when cleared.

If all four enemies die in 3 seconds:
→ next phase starts at 3 seconds.

If enemies remain after 8 seconds:
→ next phase starts at 8 seconds anyway.

This creates performance-sensitive encounter pressure without dynamically
changing enemy statistics.

## Phase Ownership

Phase completion is derived from the phase's owned spawn groups.

It must not use global enemy counts.

Therefore enemies from:
- another encounter;
- ambient ecology;
- Workshop pursued scenarios;

do not block or accidentally advance the current phase.

Per-actor Phase Membership should only be introduced when a concrete feature
requires direct actor-to-phase querying.

## Compatibility

Encounters without phases retain the existing behaviour:

LevelEncounter
→ directly owns Spawn Groups.

This allows phased authoring to be introduced incrementally rather than requiring
a migration of every existing encounter.

# Encounter Started Semantics

An encounter has an explicit runtime `Started` transition.

The transition occurs when:

- encounter content has begun;
- combat has been activated.

Therefore:

Pre-spawned dormant enemies
≠
Encounter Started.

Player reaches / activates that encounter
=
Encounter Started.

`LevelEncounter.HasStarted` is occurrence-owned runtime state and resets when a
reusable encounter is rearmed.

A runtime Start Sequence records the order in which simultaneous encounters
entered active gameplay.

This is distinct from Availability Sequence:

Availability Sequence
→ when an opportunity entered strategic supply.

Started Sequence
→ when the player actually began that encounter.

# Runtime Encounter Authoring Overrides

Scene-authored encounter components remain the current source content model.

3.0J5 introduces runtime authoring overrides so the Encounter Workshop can
preview experimental values without changing serialized scene content.

Current override surfaces:

`LevelEncounter`
- Display Name;
- Description.

`LevelEncounterPhase`
- Display Name;
- phase transition delay;
- advance-on-clear.

`LevelSpawnGroup`
- spawn-entry schedule.

`LevelSpawnSource`
- spawn shape;
- radius;
- box size.

When no override is present, normal authored component values are used.

The runtime override layer is not itself the final Encounter Definition model.

It is a preview/application seam that can later consume portable authored
Encounter Definition data.

# Encounter Definition Binding

A scene encounter may optionally receive its gameplay recipe from an
`EncounterDefinition`.

Authoring shape:

LevelEncounter
├── LevelEncounterDefinitionBinding
├── Phase...
└── ...

The scene hierarchy currently acts as the compatible physical/runtime template.

`LevelEncounterDefinitionBinding` applies the Definition through runtime
override seams before the expedition uses the encounter.

When no Definition Binding exists, the original scene-authored encounter
behaviour remains valid.

This allows data-driven encounter content to be introduced incrementally rather
than requiring an immediate migration of every existing encounter.

## Current Boundary

Encounter Definition answers:

> What happens?

Scene encounter/site answers:

> Where can it happen?

Runtime encounter answers:

> What is happening in this occurrence?

The boundary is still evolving.

Full Encounter Site / Runtime Encounter Instance separation remains future
architecture.

# Open Questions

- Required versus optional encounter semantics.
- Encounter discovery rules.
- Encounter completion conditions.
- Ambient movement for disengaged enemies.
- Whether damage against a disengaged actor automatically engages it.
- Exact level-schedule authoring UI.
- Whole-party versus per-character encounter direction.
- How much encounter information is visible before discovery.