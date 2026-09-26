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

Legacy allied or Support definitions are not automatically considered playable
Characters.

A roster entry may be visible while locked or hidden until unlocked.

The final gameplay conditions that unlock new Characters remain unresolved until
the first additional playable Character exists.

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
Healing/recovery mechanism.

# Character Sustain
Potential sustain identities:
- direct healing
- healing by dealing damage
- regeneration
- environmental healing
- between-zone healing
- shields
- resource conversion.

Important principle:
> Sustain can be part of character identity rather than every character owning the same heal button.


# Second Playable Character — Ranger

**Status: Working**

The second playable Character is currently a ranged skirmisher using the
SamuraiArcher visual asset as working production art.

Baseline contrast with Punchy:

Punchy
→ close-range;
→ fast combo chain;
→ aggressive movement;
→ knockback/control.

Ranger
→ long-range;
→ deliberate single-shot bow cadence;
→ projectile travel time;
→ lower durability.

The Ranger identity is not yet complete.

O1B2 must still establish:
- Signature Ability;
- Utility Ability;
- Sustain Ability;
- Character Mastery.

The final Character name and narrative identity remain flexible.