# World Events — Technical Design

**Status: Working**

Related:
- [[Ecology]]
- [[World Time and Daily Cycle]]
- [[Expedition Scheduling and Encounter Generation]]
- [[UI-UX]]

# Purpose

World Events represent temporary expedition-wide or regional conditions.

They answer:

> What unusual rules, pressures or opportunities are affecting the world right
> now?

They are not selectable encounters.

Examples:
- Zombie Surge;
- Fiery Zombies;
- increased Elite activity;
- migration / rare activity;
- resource events.

# Lifetime

Active World Event state belongs to the Expedition / Run lifetime.

Therefore:

Region transition
→ event may remain active.

Expedition end
→ active World Events are discarded.

A new expedition receives fresh World Event state.

# Architecture

The prototype separates identity, runtime state, scheduling and concrete effects.

`WorldEventDefinition`
- persistent authored identity / presentation data;
- does not contain a giant effect enum.

`WorldEventState`
- owned by `RunState`;
- stores currently active event occurrences;
- records start / end WorldTimestamp;
- exposes activation / expiry events.

`WorldEventSystem`
- evaluates active-event expiry against the run-owned World Clock.

`PrototypeExpeditionSchedule`
- may forecast and activate a World Event;
- does not implement the event's concrete gameplay effect.

Concrete effect systems
- respond to active World Event state;
- affect only the gameplay domain they own.

Example:

Zombie Surge
↓
ZombieSurgeAmbientEffect
↓
AmbientSpawnPoint
↓
additional ambient zombies.

# 3.0H Zombie Surge Prototype

The first World Event is:

Zombie Surge.

Initial prototype effect:
- adds one extra zombie at each zombie Ambient Spawn Point when the event begins;
- periodically adds another zombie at each point while the event remains active;
- uses World Time for pulse timing;
- stops creating additional surge population when the event expires.

Ambient kills continue to provide normal reward / currency behaviour.

Therefore Zombie Surge is both:

Threat
+
Opportunity.

# Engagement Snapshot Principle

World Events should generally affect:

future content
+
unengaged / newly created content.

They should not silently rewrite actors the player has already committed to
fighting.

3.0H proves this through population rather than stat mutation.

Zombie Surge begins:
- existing enemies remain unchanged;
- additional ambient enemies are generated.

Zombie Surge ends:
- future surge spawning stops;
- already-created zombies remain in the world.

Future events such as Fiery Zombies should follow the same principle.

# Extension Rule

Do not create:

WorldEventType
→ giant switch
→ every possible gameplay effect.

Prefer:

World Event state
↓
domain-specific effect component / system.

Examples:

Zombie Surge
→ Ambient Ecology effect.

Fiery Zombies
→ future enemy-generation modifier.

Rare Activity
→ future Encounter Director weighting.

Resource Boom
→ future resource-generation modifier.

Multiple systems may react to the same World Event where appropriate.

# Prototype Limitations

3.0H does not yet provide:
- dynamic event generation;
- recurring daily-event rules;
- intensity-aware event selection;
- spawn-ahead visibility management;
- sophisticated event stacking;
- final UI;
- encounter-generation weighting from Zombie Surge.

These should only be implemented when the relevant systems exist.