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