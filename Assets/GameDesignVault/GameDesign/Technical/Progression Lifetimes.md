# Progression Lifetimes — Technical Design

**Status: Working**

Related:
- [[Progression]]
- [[Buildcraft]]
- [[Rewards]]
- [[Equipment]]
- [[Expeditions]]
- [[World Time and Daily Cycle]]

# Core Lifetime Model

The game has several distinct progression lifetimes.

## Persistent / Meta Lifetime

Survives expedition success and failure.

Examples:
- characters;
- support characters;
- weapons;
- armour;
- persistent gear upgrades;
- permanent ability unlocks;
- crafting materials;
- persistent currencies;
- recipes;
- other account / preparation progression.

Persistent progression answers:

> What am I capable of bringing into the next expedition?

## Expedition / Run Lifetime

Begins when an expedition starts.

Ends when that expedition ends.

Examples:
- World Time;
- temporary RunBuild;
- abilities acquired during the expedition;
- ability levels;
- evolutions;
- temporary augments;
- temporary stat rewards;
- pending temporary rewards;
- escalation;
- intensity history;
- current schedule;
- current encounter state;
- expedition currency.

Expedition progression answers:

> What build have I created during this attempt?

## Region / Level Lifetime

Exists only while a particular map / region runtime is active.

Examples:
- scene encounter sites;
- local encounter supply;
- ambient routes;
- local world objects;
- region-specific runtime state.

A region transition does not inherently end the expedition.

## Encounter Lifetime

Exists for one runtime encounter occurrence.

Examples:
- availability;
- commitment;
- expiry;
- spawned encounter actors;
- completion.

## Runtime Actor Lifetime

Examples:
- Health;
- cooldowns;
- buffs / debuffs;
- ForcedMotion;
- runtime ability execution state;
- combat target;
- navigation intent.

# Expedition End

Party defeat ends the active expedition.

## Defeat Resolution vs Presentation Cleanup

Expedition defeat and visual level cleanup are separate transitions.

When the final party member becomes gameplay-dead:

- defeat is resolved immediately;
- expedition-owned `RunProgress` ends immediately;
- World Time and other expedition-owned runtime state stop immediately;
- persistent expedition-end bookkeeping may resolve immediately;
- the Run flow may enter a short ending/presentation state;
- dead party runtime actors may remain visible while authored death presentation completes;
- return to preparation and full Region/Level object cleanup occur after that bounded presentation lifetime.

Death presentation must never own, delay or reverse the gameplay defeat result.

Expected flow:

Party Defeated
↓
Current region runtime ends
↓
Expedition state ends
↓
World Time ends
↓
Temporary build is discarded
↓
Return to preparation
↓
Start a new expedition when ready

A new expedition receives:
- fresh World Time;
- fresh temporary build;
- fresh schedule;
- fresh escalation;
- fresh encounter state.

Persistent progression remains.

# Level / Region Clear

Clearing a normal region, level or boss encounter is not equivalent to winning
the expedition.

A normal clear may:
- grant rewards;
- allow travel / region transition;
- increase escalation;
- advance expedition progression.

The expedition continues.

# Expedition Success

The intended expedition success threshold is the Apex.

Defeating the Apex marks the expedition as successful.

The player may then:

Return
→ end the expedition successfully.

or:

Continue
→ enter post-Apex endurance.

If the player later dies during post-Apex endurance, the previously achieved
Apex success is not retroactively erased.

Exact Apex implementation remains future work.

# Persistent Power vs Expedition Power

Persistent progression should primarily improve:
- starting capability;
- build options;
- build identity;
- preparation flexibility.

It should not replace expedition buildcraft.

Desired relationship:

Persistent Build
"Gets me through the front door."

Expedition Build
"Determines how far this attempt can go."

The player should not eventually solve the entire expedition purely by grinding
flat permanent numbers.

# Ability Unlock vs Ability Acquisition

These are separate concepts.

## Ability Unlock

Persistent.

Means:

> This ability is now part of my available progression / preparation ecosystem.

## Ability Acquisition

Expedition-owned.

Means:

> This character currently owns this ability during this expedition.

A permanently unlocked ability might later:
- become selectable in preparation;
- enter reward pools;
- appear on equipment;
- become available from another acquisition source.

Unlocking an ability does not inherently mean every future expedition starts
with it equipped.

# Equipment

Weapons and armour belong primarily to persistent preparation.

Their future identity may include:
- autonomous attack behaviour;
- combo structure;
- defensive mechanics;
- innate abilities;
- augment capacity;
- unique properties.

Current prototype Weapon / Armour / Focus levels are temporary stand-ins for this
future persistent equipment layer.

# Prototype Persistent Progression

The initial 3.0C implementation may use in-memory persistence only.

Persistence currently means:

> Survives multiple expeditions during one Play session.

It does NOT yet mean:
- save file persistence;
- account persistence;
- final meta economy.

This is enough to prove the gameplay loop before building permanent storage.

# Expedition Currency and Persistent Resources

**Status: Implemented Foundation**

Expedition Currency and persistent resources are separate lifetimes.

## Expedition Currency

Owned by `RunState`.

Used for temporary expedition systems.

It resets when the expedition ends.

## Persistent Resources

Current placeholder tiers:

- Level 1;
- Level 2;
- Level 3.

Persistent-resource earnings are first recorded on `RunState`.

They survive Region transitions inside the same expedition.

When the expedition ends through Voluntary Return or Defeat, all earned
persistent resources are banked into `PersistentProgression`.

The old behaviour where total Expedition Currency automatically became
Persistent Currency is retired.

This prevents temporary run spending and persistent progression from silently
sharing the same economic identity.

The current `PersistentProgression.Currency` property remains only as a temporary
compatibility alias for Level 1 Resource while the old Weapon / Armour / Focus
prototype progression is being retired during N4.

# Invariants

Party defeat ends the expedition.

A new expedition receives a fresh World Clock.

Persistent progression survives defeat.

Temporary RunBuild progression does not.

Normal level / region victory does not equal expedition victory.

Apex is the intended expedition-success threshold.

Persistent and temporary rewards must not silently share ownership.

World Time belongs to the expedition lifetime, not persistent progression.