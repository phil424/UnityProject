# Weapon Mods

**Status: Working — 3.0O2 Active**

# Purpose

Weapon Mods are a major Character customisation and buildcraft system.

They modify or extend a Character's existing Basic Attack / Weapon Identity
rather than replacing the Character with a freely interchangeable weapon
loadout.

Conceptually:

Character
+
Basic Attack / Weapon Identity
+
Weapon Mods
+
Abilities
+
Expedition progression
=
Build.

Weapon Mods may eventually affect:

- basic-attack behaviour;
- damage identity;
- combo behaviour;
- projectile behaviour;
- knockback;
- status effects;
- Character stats;
- special effects;
- Ability interactions.

# Character-Native and Shared Mods

Characters may contribute Weapon Mods designed around their own combat identity.

A Mod may nevertheless be compatible with more than one Character.

Long-term progression may therefore allow:

Character A
→ unlocks a Weapon Mod
→ Mod becomes available
→ Character B may use it if compatible.

Compatibility and ownership are separate concepts.

# Current O2A Direction

The first Weapon Mod proof is Expedition-owned.

A Weapon Mod definition is reusable content independent from the system that
grants it.

Current flow:

Weapon Mod Definition
↓
Run Reward
↓
RunBuild
↓
Runtime Character
↓
Basic Attack behaviour.

The first proof Mod is:

Heavy Finisher
→ Punchy-compatible;
→ final basic-attack combo step deals additional damage.

This is intended to prove:

- Weapon Mod identity;
- Run-owned Mod state;
- compatibility;
- reward acquisition;
- runtime combat integration;
- a clean extension point for later Mod behaviours.

# Compatibility

Initial compatibility is authored directly against playable Character
definitions.

An empty compatibility list means broadly compatible.

Explicit Character references are appropriate for the current small roster.

Do not build a large tag/query compatibility framework until real content makes
that necessary.

# Progression Lifetime

Current O2A active Mods belong to the Expedition Build.

They reset when the Expedition ends.

Persistent Mod availability/unlocking is separate and will be introduced during
O2B.

Long-term direction:

Persistent Progression
→ determines which Mods are available to discover/use.

Expedition Build
→ determines which Mods are currently active during this Expedition.

# Deliberately Unresolved

Do not yet lock:

- final Mod capacity;
- number of simultaneously active Mods;
- prepared Mod slots;
- whether some Mods begin active;
- whether Mods may be swapped during an Expedition;
- final compatibility vocabulary;
- final cross-Character unlock rules;
- final relationship between persistent preparation and Expedition acquisition.

These should be informed by actual O2 gameplay.