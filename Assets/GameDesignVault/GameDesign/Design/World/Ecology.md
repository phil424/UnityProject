# Overview
Core philosophy:
> Ecology dynamically changes what opportunities are available during an expedition.

Also:
> Ecology should create tactical incentives rather than arbitrary hard counters.

# Dynamic Events
Potential event categories:
- migrations
- invasions
- rare sightings
- horde movement
- territorial conflict
- weather
- resource events
- monster population shifts.

Define events as potentially having:

```
Location
Duration
Warning Time
Threat
Reward Modifier
Affected Monster Types
Affected Build Mechanics
```


# Upsurges
### Definition:
A temporary dramatic increase in activity from a monster/resource family.

### Example:
> Zombie Upsurge

Effects could include:
- significantly more zombies
- harder encounters
- increased Zombie Crystal acquisition
- greater chance of zombie-specific rewards.

### Principle:
>**Threat + opportunity.**

# Boss Scheduling and Migration
- boss activity follows semi-dynamic schedules
- player can see upcoming events
- bosses may enter and leave zones
- bosses may move between zones
- schedules can overlap
- multiple bosses can share a location.

# Environment Modifiers
### Example:
> Heatwave - Potentially improves fire-related mechanics.

The point is not necessarily:
> Fire Damage +25%.

Could affect:
- status duration
- environmental interactions
- enemy susceptibility
- reward weighting
- event frequency.

### Principle:
> Encourage new opportunities without deleting existing builds.

# Rare Sightings

Ecology can create temporary high-value encounters.

Examples:
- rare monster variants;
- valuable migrating groups;
- unusual bosses;
- resource-rich monster populations.

Rare sightings should create:
> "Do I change my current plan to pursue this opportunity?"

A sighting may contain:
- encounter/location;
- warning or discovery time;
- duration;
- rarity / opportunity value;
- expected reward;
- notable monster.

Rare sightings should integrate with the map / encounter HUD.

## Gated Rare Sightings

A rare sighting may become known before it becomes available.

Example:

Nobleman's Procession appears on the regional schedule.

The player can see:
- that it exists;
- that it is extremely rare;
- how long remains before it expires.

However, accessing it requires completing an Elite encounter first.

The expiry window may continue running while the rare encounter is locked.

This creates an optional risk/reward challenge:

> Take on a dangerous prerequisite now for a chance at the rare payoff, or
> ignore the opportunity and continue with the safer expedition plan.

The scheduler should own the timing window.

The encounter should own its current known / available / completed / expired
state.

# Current World Events

World events are temporary changes to the expedition rather than individual
selectable encounters.

Examples:

## Zombie Surge

Potential effects:
- increased ambient zombie population;
- more zombie encounters;
- increased encounter density;
- greater Zombie-specific reward opportunity.

### 3.0H Prototype

The first playable Zombie Surge implementation affects ambient population.

While active:

- additional ambient zombies appear when the event begins;
- additional population pulses occur periodically;
- ambient enemies continue to use normal combat and reward systems.

When Zombie Surge expires:

- additional surge spawning stops;
- already-generated enemies remain.

This intentionally proves the Engagement Snapshot Principle without introducing
enemy-stat mutation.

Future Zombie Surge versions may also influence:
- encounter supply;
- zombie encounter frequency;
- Elite probability;
- Zombie-specific reward opportunity;
- intensity targets.

Those are not required for the first implementation.

## Fiery Zombies

Potential effects:
- newly generated zombies gain Fire-related behaviour;
- unengaged zombie encounters may receive the modifier;
- ambient zombie populations may change;
- related reward opportunities may change.

# Engagement Snapshot Principle

World changes should generally not rewrite enemies the player has already
committed to fighting.

Example:

Fiery Zombies begins.

Already-engaged zombie encounter:
- unchanged.

Available but unengaged encounter:
- may be modified.

Future zombie encounter:
- may be generated with Fiery Zombies.

Ambient zombie that has not been engaged:
- may be affected.

Ambient zombie already chasing/fighting the player:
- remains unchanged.

# Ambient Ecology

Ambient enemies provide:
- world population;
- low-pressure combat;
- build experimentation;
- movement continuity.

They are not necessarily represented as formal encounters.

## 3.0K5A — Ambient Activity Areas

**Status: Implemented Foundation**

Ambient population is authored through Region-owned `AmbientActivityArea`s.

An Ambient Activity Area answers:

> What lightweight population naturally occupies this piece of Region
> geography?

An Area owns:
- a Route geography footprint;
- one or more population entries;
- spawn scatter;
- engagement distance.

A footprint may reference:
- Route Nodes;
- Route Segments;
- Route Groups.

When usable Segments exist, ambient actors are distributed along the authored
Segments.

Nodes provide point-like fallback placement.

This replaces the earlier fixed `AmbientSpawnPoint` prototype.

Ambient population remains distinct from formal Encounters.

Ambient actors:
- do not become Encounter choices;
- do not contribute to formal Encounter completion;
- may occupy the same geography as formal Encounter Sites;
- still provide normal lightweight combat and world population.

The initial Village Well migration preserves four ordinary ambient Zombies while
moving their placement onto Route geography.

## 3.0K5B — Local Ambient Wandering

**Status: Implemented Foundation**

Disengaged Ambient enemies may wander modestly around their individual
spawn/home position.

Wandering is configured by the owning `AmbientActivityArea`.

Current settings:
- Wander Radius;
- Minimum Wander Pause;
- Maximum Wander Pause.

Runtime flow:

Ambient actor spawns
↓
local home position captured
↓
wait
↓
choose local destination
↓
travel through `ActorNavigationIntent`
↓
wait
↓
repeat.

Ambient wandering uses normal actor movement.

`AmbientEnemy` does not directly translate its Transform.

When the party enters the actor's engagement radius:

wander navigation clears
↓
Combat Engagement becomes active
↓
normal targeting / combat pursuit takes over.

Engagement is currently one-way for the actor lifetime.

An engaged ambient enemy does not return home or resume wandering when the party
moves away.

This keeps ambient population lightweight and preserves the established combat
engagement model.

A Wander Radius of `0` disables local wandering for that Area's spawned actors.

# Ambient Route Graph

Ambient population should follow designer-authored paths through interesting
parts of the region.

Working model:

A ─── B ─── C
         │          │
         D ─── E

Nodes and connections are authored by the level designer.

The ambient population system may:
- populate routes ahead of the hero;
- favour routes leading toward new parts of the map;
- wander enemies locally after spawning;
- use route segments as fallback spawn areas;
- respond to world events and escalation.

The system should avoid arbitrary map-wide random scatter that causes the hero
to move between unrelated coordinates.

# Ecology and Intensity

World events and encounter generation should contribute to an overall intensity
rhythm.

Example:

Fiery Horde event
+
high zombie encounter density
→ rising pressure.

When pressure exceeds the desired range for the current escalation level:

future encounter density may decrease
+
world event may finish
+
lower-intensity opportunities may be served

This should create peaks and valleys without reducing long-term escalation.

# Time-of-Day Ecology

World time may influence ecological activity.

Working examples:
- zombie density increases at night;
- rare encounters become more common around particular phases;
- ambient populations change with world events;
- migrations may occur at scheduled times.

Time-of-day rules should influence unengaged / future content rather than
retroactively rewriting already-engaged combat.

The exact ecology effects of day/night remain exploratory.