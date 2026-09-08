# Expedition Scheduling and Encounter Generation — Technical Design

**Status: Working**

Related Design:
- [[Expeditions]]
- [[Ecology]]
- [[UI-UX]]
- [[Level Authoring and Encounters]]
- [[Targeting, Tactics and Encounter Direction]]

# Purpose

Define the intended separation between:

- long-term expedition escalation;
- moment-to-moment intensity;
- encounter generation / supply;
- encounter lifecycle;
- player-facing forecasting;
- world events;
- ambient population.

The goal is to support expeditions that can remain interesting for as long as
the player's temporary build can survive.

# Core Responsibility Separation

## Expedition / Escalation State

Answers:

> How far has this expedition escalated?

Escalation generally rises over time.

Higher escalation may influence:
- available encounter pools;
- encounter frequency;
- enemy composition;
- Elite frequency;
- simultaneous encounter pressure;
- ambient population;
- world-event severity;
- future boss / Apex behaviour.

Example:

Escalation 0
- mostly normal encounters.

Escalation +1
- Elites appear more often.

Escalation +2
- encounter supply increases;
- higher-difficulty encounter pools become eligible.

Exact escalation rules should be data-driven.

## Intensity

Answers:

> How pressured is the player right now?

Intensity may rise and fall even while escalation continues rising.

Desired rhythm:

pressure
↓
peak
↓
recovery
↓
new pressure
↓
larger peak

High escalation should make both peaks and valleys more dangerous than they were
earlier in the expedition.

Potential intensity observations may eventually include:
- current hostile population;
- active encounters;
- Elite / Boss pressure;
- incoming damage;
- recent player damage taken;
- healing frequency;
- party health;
- encounter completion rate.

Exact telemetry and weighting remain exploratory.

Intensity monitoring should primarily influence what happens next.

Avoid secretly weakening enemies already committed to combat simply because the
player is struggling.

## Encounter Supply

Answers:

> What encounters should be available now and what encounters should arrive next?

Encounter generation and player-facing forecasting are separate responsibilities.

The encounter system / future encounter director should:
- choose appropriate authored encounter content;
- respect escalation;
- respect current intensity;
- maintain encounter supply;
- select compatible locations where required;
- avoid excessive repetition;
- create peaks and valleys;
- provide future encounters for the forecast.

There should always be at least three selectable encounter opportunities
available to the player during normal expedition play.

## 3.0G Prototype Encounter Supply

The first continuous-supply prototype deliberately reuses existing
scene-authored `LevelEncounter` sites.

This is temporary.

It exists to prove encounter lifecycle and strategic pacing before introducing
the future:

Encounter Definition
+
Encounter Site
+
Runtime Encounter Instance

model.

### Prototype Lifecycle

Reusable generic encounter:

Available
↓
expiry timer begins
↓
Selected / Started
OR
Ignored

Selected / Started
→ protected from expiry
→ resolve normally

Ignored
→ Expired

Completed / Expired
↓
reuse delay
↓
site rearmed
↓
new availability sequence
↓
new occurrence enters the back of supply

### Minimum Supply

Normal strategic play should expose at least three quick encounter choices.

`PrototypeEncounterSupply` therefore:
- maintains a configurable minimum;
- normally respects reuse delay;
- may rearm the oldest reusable site early if required to prevent a supply
  shortage.

If the selected encounter is excluded from quick slots, supply may maintain one
additional selectable encounter so the player still receives three alternative
destinations.

### Expiry Protection

Prototype rule:

Current selected encounter
→ protected.

Encounter whose spawning has begun
→ protected.

Unselected + unstarted encounter
→ may expire.

This means redirecting away from an encounter before reaching it may allow its
expiry timer to resume.

Once gameplay has actually begun, the encounter remains until resolved.

### Scene-Authored Reuse

The 3.0G prototype distinguishes:

Reusable baseline encounters
- provide regular strategic supply;
- may cycle repeatedly.

Special scheduled encounters
- may be one-shot;
- do not need to participate in baseline recycling.

Example:

West Road Horde
→ reusable baseline opportunity.

Reinforcements
→ scheduled special opportunity.

### Regional Boss Progression

Continuous encounter supply invalidates the previous rule:

all minion groups exhausted
→ automatically spawn boss.

`StageDirector` therefore no longer needs to infer regional progression from
encounter exhaustion.

For the current MVP, boss phase may be started explicitly through a debug seam
so level/region transition remains testable.

Future boss / Apex initiation should come from an explicit progression,
schedule, encounter or expedition rule.

Encounter Supply must not secretly own expedition-success logic.

## Forecast / Schedule

Answers:

> What does the player know is going to happen?

The schedule is primarily a player-facing forecast of planned expedition changes.

It should not need to construct encounter gameplay itself.

Potential forecast information includes:
- upcoming encounter arrivals;
- upcoming encounter activation;
- encounter expiry;
- major world events;
- escalation milestones;
- rare encounter periods;
- Apex-related events.

## 3.0F Prototype Implementation

The first playable forecast uses a lightweight `PrototypeExpeditionSchedule`.

The prototype schedule is anchored to:

`WorldClockState.StartTime`

rather than the time at which the schedule component happens to initialize.

This keeps schedule timing stable if:
- initialization occurs a frame later;
- simulation speed changes;
- World Time rate changes during the expedition.

Each prototype schedule entry contains:
- display name;
- World Time offset from expedition start;
- scheduled `WorldTimestamp`;
- optional existing `LevelEncounterActions`;
- forecast-only state;
- runtime resolved / triggered state.

### Gameplay-Backed Entries

A gameplay-backed entry may execute an existing authored action when its scheduled
time arrives.

Example:

Reinforcements Mobilise
↓
Make Reinforcement Encounter Available

This is a thin prototype execution bridge.

It is NOT the future Encounter Director and should not become responsible for
constructing arbitrary encounter content.

#### World Event Activation

As of 3.0H, a gameplay-backed schedule entry may also activate a
`WorldEventDefinition` for a configured World Time duration.

Example:

10:00
Zombie Surge
↓
WorldEventState activates Zombie Surge
↓
WorldEvent-specific systems react.

The scheduler owns:

> when the event begins.

The World Event system owns:

> whether the event is currently active.

Domain-specific effect systems own:

> what the event changes.

This prevents the scheduler from becoming a gameplay-effect manager.

### Forecast-Only Entries

Forecast-only entries communicate future information whose gameplay system does
not exist yet.

Current prototype example:
- Rare Activity Window.

Zombie Surge became the first gameplay-backed World Event in 3.0H.

These let the forecast presentation be tested before their full systems are
implemented.

### Early Resolution

A planned schedule entry may become unnecessary because of player action.

Example:

Opening Horde completed
↓
Reinforcements become available early
↓
scheduled Reinforcement availability action has nothing left to do
↓
forecast entry resolves early

This establishes:

> The schedule is a forecast of planned future state, not an immutable script.

Future generation systems may reschedule, replace or remove forecast entries as
the expedition changes.

### Prototype Boundary

3.0F does not maintain encounter supply.

It only proves:
- World Time-based scheduling;
- upcoming-event presentation;
- short actionable countdowns;
- longer-range forecast timestamps;
- one small interaction between player action and planned future state.

Continuous arrival / expiry / backfill remains the responsibility of 3.0G.

# Forecast Horizons

## Immediate

Approximately 0–30 seconds.

Precise actionable countdowns.

Examples:
- encounter appears in 00:20;
- encounter expires in 00:12;
- world event activates in 00:25.

## Near Future

Approximately the next 5–10 minutes.

The expanded strategic view may expose detailed planned encounters and events.

## Long-Range Milestones

Major forecast events may be visible significantly further ahead.

Example:

15:00
Zombie Upsurge

30:00
Rare Encounter Activity

45:00
Fiery Horde

60:00
Major Escalation Event

The exact event schedule does not need to be fixed across every expedition.

# World Clock

The expedition uses a run-owned accelerated world clock.
## 3.0B Prototype Implementation

The prototype implementation separates runtime time state from the system that
advances it.

`WorldTimestamp`
- represents a unique position in expedition World Time;
- includes expedition day and time-of-day;
- stores monotonic total world minutes;
- supports future schedule comparisons.

`WorldClockState`
- is owned by `RunState`;
- persists across level/runtime actor recreation;
- tracks current World Time;
- separately tracks elapsed simulation time.

`WorldClockSystem`
- advances the active run's `WorldClockState`;
- owns prototype authoring values for starting hour and clock rate;
- advances only while `RunDirector` is `InLevel`;
- uses scaled simulation delta time.

Therefore existing simulation controls naturally affect the clock:

Pause
→ World Time stops.

Slow
→ World Time advances more slowly.

Fast
→ World Time advances more quickly.

The initial prototype rate is:

1 world hour / simulation minute

which produces:

24 simulation minutes / world day.

This is a tuning value rather than a locked design constant.

`RunProgress.Changed` should not be emitted every frame for clock advancement.
Continuous World Time is separate from discrete progression-change notification.

The World Clock provides:

- current day index;
- time of day;
- conversion from simulation time to world time;
- a common temporal coordinate for schedule events.

Conceptually:

Simulation Delta Time
        ↓
World Time Rate
        ↓
World Timestamp
        ↓
Day N + HH:MM

World-clock speed is authored/configurable.

Simulation pause / speed changes should affect world-clock progression so that
strategic slow-motion also slows schedule countdowns.

# Time Concepts

Keep these concepts distinct:

## Simulation Time

Runtime simulation progression.

Affected by:
- pause;
- slow;
- normal;
- fast.

## Expedition Elapsed Time

Monotonic time since the expedition began.

Does not wrap at 24 hours.

## World Time

Cyclic presentation of expedition time as:

Day N
+
HH:MM

## Schedule Time

A planned event time.

Schedule events may reference:
- a specific world timestamp;
- a recurring time-of-day window;
- an expedition milestone;
- a generated future timestamp.

Do not use a bare time-of-day value as unique runtime event identity because the
same time occurs on multiple expedition days.

# World Time and Escalation

World Time must not own long-term difficulty.

Escalation remains a separate system/state.

The scheduler and encounter system may combine:

- current world-time phase;
- expedition day;
- escalation;
- current intensity;
- world events;

to decide what future content is appropriate.

Example:

Midnight
+
Escalation 1
→ modest Zombie Surge.

Midnight
+
Escalation 5
→ substantially stronger Zombie Surge.

# Daily Schedule Profiles

Future scheduling data may describe broad daily event tendencies.

Example:

Dawn
- recovery weighting.

Dusk
- rare encounter weighting.

Night
- horde weighting.

This should remain data-driven.

A daily profile may repeat while escalation changes the encounters generated
inside those windows.

# Schedule Authority

World Clock
= current temporal state.

Scheduler
= plans / forecasts future state changes.

Encounter Director
= supplies encounter content appropriate to that state.

Escalation / Intensity
= influence the pressure and content appropriate to the current period.

These responsibilities should remain separate.

# Encounter Content Model — Future Compatibility

The current implementation uses scene-authored `LevelEncounter` objects.

Long-term generation should remain compatible with separating:

## Encounter Definition

"What gameplay scenario is this?"

Examples:
- Ground-Burst Horde;
- Elite Gauntlet;
- Nobleman's Procession;
- Church Outbreak.

## Encounter Site

"Where could an encounter happen?"

Examples:
- Church;
- Village Square;
- Street Junction;
- generic encounter location.

## Runtime Encounter Instance

"This particular encounter happening during this expedition."

A fixed spectacle may tightly couple definition and site.

Example:

Church Outbreak
→ authored specifically around the church doorway.

A generic encounter may be placed at several compatible authored sites.

Do not refactor the current encounter implementation into these layers until the
strategic prototype demonstrates the concrete requirements.

# Authored Encounters

Encounter content should remain designer-authored.

Randomness should primarily decide:
- which authored encounter appears;
- when it appears;
- where compatible generic encounters appear;
- scaling / composition modifications;
- relationships with other encounters and events.

Avoid replacing authored encounter identity with completely arbitrary procedural
enemy soup.

# Encounter Authoring Workshop

Future development should provide a dedicated encounter-authoring/test scene.

Desired workflow:

Create / select encounter
↓
configure spawn groups / timing / presentation
↓
press Play
↓
encounter runs using the same runtime systems as the expedition

This should allow encounter iteration without playing through a complete run.

# Encounter Commitment and Expiry

An encounter may be:

Known
↓
Available
↓
Selected / Committed
↓
Engaged
↓
Completed

Default expiry behaviour:

Unselected and unstarted
+ expiry reached
→ encounter may disappear / deactivate.

Selected / committed
→ expiry no longer removes the encounter.

Started / engaged
→ encounter remains until resolved.

The exact visual/gameplay method used to remove an expired encounter remains
open.

# Encounter Redirection

Selecting a new encounter is an immediate player command.

Expected behaviour:

Current Encounter A
↓
player selects Encounter B
↓
drop autonomous targets belonging to A
↓
immediately begin travelling toward B

Enemies from A are not forced to disengage.

They may pursue the hero.

This allows a strong player to deliberately combine several encounters into one
larger fight.

The hero should not automatically reacquire the abandoned encounter while
redirecting toward the newly selected encounter.

# World-Event Snapshot Rule

World and escalation modifiers may affect:

- future encounters;
- available but unengaged encounters;
- unengaged ambient enemies.

Once an encounter or ambient enemy has been engaged, its important combat state
should generally be treated as committed.

Example:

Fiery Zombies activates.

Already-engaged zombies:
- remain unchanged.

Unengaged zombies / future zombie encounters:
- may gain the Fiery Zombie modification.

This keeps world changes readable and fair.

# Ambient Population

Ambient enemies are separate from formal encounters.

Their purpose includes:
- low-pressure combat;
- build experimentation;
- world population;
- travel continuity;
- guiding autonomous exploration.

With no selected encounter, the hero should be able to follow ambient activity
through the region.

# Ambient Route Graph

Ambient population should follow designer-authored spatial intent rather than
randomly scattering enemies over an area.

Working concept:

Ambient Route Graph

A ─ B ─ C
    │
    D ─ E

Designers place travel nodes and connections through interesting parts of the map.

Ambient enemies may:
- spawn ahead of the player;
- populate nodes / route segments;
- wander within a modest local radius;
- remain combat-inactive until engaged;
- chase once engaged.

The system should prefer authored street / route placement.

If additional population is required, it may fall back to scattering enemies
sensibly along the route rather than across arbitrary world coordinates.

## 3.0E Prototype Implementation

The first ambient-navigation prototype introduces:

`AmbientRouteNode`
- designer-authored world position;
- explicit connections to other nodes.

`AmbientRouteGraph`
- hierarchy-owned collection of route nodes;
- treats authored connections as bidirectional;
- provides route-neighbour and nearest-node queries.

`AmbientRouteNavigator`
- owns fallback whole-party travel;
- activates only when no Encounter Directive exists;
- uses `ActorNavigationIntent`;
- yields to local combat;
- resumes route travel when combat ends.

`AmbientSpawnPoint`
- prototype scene-authored ambient population;
- spawns lightweight ambient actors around selected route nodes.

`AmbientEnemy`
- begins combat-disengaged;
- becomes combat-active when the party approaches;
- once engaged, uses normal combat movement/targeting;
- does not count toward formal encounter/boss progression.

The prototype intentionally does NOT yet implement:
- local wandering;
- moving population fronts;
- spawn-ahead visibility rules;
- dynamic route population;
- escalation-based density;
- world-event modifications.

These belong to the later ambient ecology implementation.

The purpose of 3.0E is to prove:

> Does a designer-authored route plus light combat make the game feel naturally
> alive when the player chooses not to issue strategic commands?

# World Events

World events modify the expedition rather than representing one selectable
encounter.

Examples:
- Zombie Surge;
- Fiery Zombies;
- increased Elite activity;
- migrations;
- resource events.

They may affect:
- encounter generation;
- unengaged encounters;
- ambient population;
- reward opportunities;
- world intensity.

World-event state should be communicated separately from the encounter list.

# Important Invariants

Encounter generation and forecasting are separate responsibilities.

There should always be three selectable encounter options during normal play.

Escalation and current intensity are different concepts.

Intensity may fall while escalation continues rising.

Selecting an encounter must produce immediate navigation response.

Selected / engaged encounters are protected from normal expiry.

Already-engaged encounters should not be retroactively rewritten by new world
modifiers.

Ambient behaviour provides the autonomous fallback when no encounter is selected.

Randomness should operate inside authored spatial and encounter intent rather
than replacing that intent.

# Prototype Boundary

The first strategic-expedition prototype does NOT need an intelligent endless
encounter generator.

It may use:
- scene-authored encounters;
- a simple predefined future queue;
- fake escalation;
- one prototype world event.

The first question to prove is:

> Is continuously choosing between changing encounters while reading a forecast
> and managing autonomous travel actually fun?

Only after that is proven should the full encounter-generation system be built.