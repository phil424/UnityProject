# Overview

Core principle:

> Equipment establishes persistent preparation identity before an expedition.

Weapons and armour primarily belong to the persistent progression lifetime.

They survive expedition success and failure.

Equipment is not primarily vertical item-level progression.

Current prototype Weapon / Armour / Focus levels are simplified stand-ins used
to prove that improving persistent preparation makes later expeditions more
capable.

Long-term equipment should become more identity-driven than flat-stat-driven.

# Slots and Loadouts
Questions:
- one weapon?
- weapon swapping?
- armour as full set or pieces?
- augmentation sockets?
- character restrictions?
- prepared ability sources?

# Equipment Progression Trees

Weapons and armour should eventually use authored persistent progression trees
rather than only linear equipment levels.

The current Weapon / Armour / Focus levels are prototype scaffolding for this
future system.

An equipment tree should mix:

- frequent incremental improvements;
- build-enabling unlocks;
- new mechanics;
- additional preparation options;
- occasional transformative milestone nodes.

Desired feeling:

small upgrade
↓
small upgrade
↓
useful breakpoint
↓
new mechanic / ability / slot
↓
new branch becomes attractive

The system should lean into the satisfaction of incremental progression while
still creating meaningful build decisions.

# Design Principle

Equipment progression should not be:

> Spend currency repeatedly for +1 stat forever.

It should be:

> Improve a piece of equipment over time and gradually reveal more of its
> identity and possibilities.

Flat-stat nodes remain valuable because frequent small progress is desirable,
but they should connect larger identity-defining nodes.

# Tree Ownership

Progression belongs to the persistent equipment / preparation lifetime.

Unlocked equipment-tree nodes survive expedition success and failure.

Their effects establish the starting state of future expeditions.

Expedition rewards may still temporarily modify or augment that prepared
equipment during one run.

# Future Technical Direction

Do not expand the current prototype by adding many hardcoded fields such as:

WeaponDamageLevel
WeaponRangeLevel
WeaponSpeedLevel
ArmourHealthLevel
ArmourMovementLevel
...

Future implementation should move toward:

Equipment Progression Definition
        ↓
authored Progression Nodes
        ↓
persistent Unlocked Node State
        ↓
reusable effects / modifiers
        ↓
Prepared Build
        ↓
fresh Expedition Build

Exact data structures should be designed when the first playable equipment tree
is implemented.