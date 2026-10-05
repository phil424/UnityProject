# Overview

**Status: Working**

Near-term expeditions use one active playable Character.

A Character owns a distinct combat identity.

Current structure:

Character
├── Visual Identity
├── Base Stat Profile
├── Basic Attack / Weapon Identity
├── Core Abilities
├── Native Weapon Mods
├── Compatibility With Shared Content
└── Character Mastery / Progression

Current principle:

> A playable Character should have a recognisable baseline combat identity before
> Expedition buildcraft begins.

Near-term Characters do not assume freely interchangeable weapon identities.

Weapon Mods, Abilities, temporary stats and other Expedition progression then
allow the player to meaningfully alter that starting identity.

Existing multi-allied-actor runtime architecture should remain generic enough to
support future Companions or other allied actors.

# Playable Roster & Ownership

**Status: Implemented Foundation**

The playable Character roster is now an explicit persistent-progression concept.

Three states are intentionally separate:

Playable Roster
→ what Character content exists as playable content.

Persistent Ownership
→ which playable Characters the player has unlocked.

Expedition Setup
→ which owned Character is selected for the next Expedition.

Current roster:

Punchy
→ starting / unlocked Character.

Ranger
→ implemented second playable Character;
→ visible while locked;
→ not unlocked by default;
→ first gameplay unlock path implemented through persistent Objective-completion
  progress;
→ development unlock remains available for testing.

Legacy allied or Support definitions are not automatically considered playable
Characters.

A roster entry may be visible while locked or hidden until unlocked.

The first Character unlock path is now implemented.

Current prototype requirement:

Ranger
→ complete the Cull Dormant Hordes Objective on two Expeditions
→ bank one persistent unlock-progress point from each completed Objective
→ unlock Ranger when progress reaches 2/2.

This remains a gameplay/progression proof rather than Ranger's intended final
narrative unlock condition.

# Archetypes
Document that archetypes can combine.

Potential dimensions:
- Heavy
- Fast
- Agile
- Resilient
- Mobile
- Ability-focused

Examples:
**Heavy + Agile**
**Fast + Agile**

These are descriptive design characteristics, not necessarily literal in-game classes.

# Character Abilities
Document the current three-part character core. Every character has 3 abilities, 1 Unique, 1 Utility, 1 Sustain. These are defined per character but every character has one.

### Innate
Signature mechanic or ability.

### Utility
Combat-supporting behaviour.

### Sustain
Reliable survival / recovery mechanism.

Sustain does not require direct healing.

It may instead create survivability through mitigation, shielding, recovery,
spacing or another Character-specific defensive behaviour.

# Character Sustain
Potential sustain identities:
- direct healing;
- healing by dealing damage;
- regeneration;
- environmental healing;
- between-zone healing;
- shields;
- timed damage mitigation / guarding;
- temporary Armour;
- spacing / avoidance where sufficiently reliable;
- resource conversion.

Important principle:
> Sustain can be part of character identity rather than every character owning the same heal button.


# Second Playable Character — Ranger

**Status: Implemented Working Character**

The second playable Character is a ranged skirmisher using the SamuraiArcher
visual asset as working production art.

The final Character name and narrative identity remain flexible.

# Baseline Contrast

Punchy
→ close-range;
→ fast combo chain;
→ aggressive movement;
→ knockback / control.

Ranger
→ long-range;
→ deliberate single-shot bow cadence;
→ projectile travel;
→ lower durability;
→ spacing and defensive timing.

# Core Ability Identity

## Signature — Power Shot

Power Shot fires one heavy projectile in a straight line.

It can pierce multiple enemies and pushes affected enemies outward in a forward
fan.

Power Shot is intentionally different from the normal homing basic arrow.

Current Ability-level scaling includes:
- damage;
- range;
- pierce count;
- knockback;
- cooldown.

Working future Evolution:

Power Shot
↓
three-shot / spread / rapid-volley form.

The multi-shot version is not part of the baseline Ability.

## Utility — Backstep

Backstep is a spacing-restoration tool.

It is intended for situations where an enemy has breached Ranger's preferred
distance.

Ranger turns away from the threat, rapidly moves toward useful bow spacing and
then immediately returns to normal autonomous ranged combat.

A dust burst reinforces the movement presentation.

## Sustain — Guard

Guard provides temporary mitigation rather than healing.

Ranger briefly enters a readable blocking state and gains temporary Armour.

A defensive aura remains visible while the protection is active.

This gives Ranger a reliable survival decision without making Sustain synonymous
with direct healing.

# Ranger Character Mastery

**Status: Implemented Foundation**

Ranger's first Mastery proof reinforces the gameplay established by the
Character's baseline kit.

Current nodes:

Quick Draw
→ Basic Attack Speed.

Mobility
→ Move Speed.

Power Shot Drills
→ Power Shot begins each Expedition one Ability level higher.

Impact Arrows Mastery
→ first alternate Weapon Mastery;
→ trades Piercing's multi-target throughput for single-target battlefield
  control.

Arrow Impact
→ persistent Knockback Power development after Impact Mastery.

Piercing Arrows Stance
→ permanently prepares the Piercing Arrows Weapon Mod stance for each Ranger
  Expedition.

The current five-node proof is not the intended final Ranger tree.

Its purpose is to prove that Ranger Mastery can develop:
- autonomous ranged combat;
- mobility;
- Character-native Abilities;
- battlefield control
- selectable Basic Attack behaviour;

without duplicating Ability mechanics inside the Mastery system.