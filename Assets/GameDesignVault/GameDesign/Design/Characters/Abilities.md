# Overview
> Abilities exist independently of their acquisition source.

Can come from:
- characters
- weapons
- equipment
- augments
- run rewards.

Player activation is normally deliberate.

# Ability Activation
Potential activation types:
- instant
- targeted enemy
- directional
- ground target
- cast
- channel
- charge.

Rules:
- immediate response to input
- player can interrupt autonomous attacks
- precision should not transform the game into a twitch action RPG.

# Ability Evolution
Core rule:
> Finding an already-owned ability should create an opportunity to transform it.

Example:
```
Whirlwind
├── Vortex
│   Pull enemies inward
├── Serrated Storm
│   Apply Bleed
└── Cyclone
    Larger area
```

Open questions:
- branching trees?
- mutually exclusive upgrades?
- multiple evolutions?
- evolution depth?
- difference between evolution and mutation?

# Ability Catalogue
Template:
```
# Ability Name

## Fantasy

## Activation

## Combat Role

## Base Behaviour

## Cooldown / Resource

## Targeting

## Interrupt Behaviour

## Possible Sources

## Evolution Ideas

## Synergy Tags

## Open Questions
```

# Core Character Actions

Some abilities/actions may belong to the baseline character control layer rather
than being normal expedition acquisitions.

Working example:

## Heal / Sustain

Every playable character should have access to a baseline sustain/healing action.

The exact behaviour is not yet locked.

Potential purposes:
- provide the player with a universal recovery decision;
- make ability timing meaningful even on simple starting builds;
- provide predictable sustain independent from random reward generation;
- occupy a consistent place in the combat-control layout.

The implementation should use the shared ability architecture rather than
creating bespoke healing logic separately on every character.

Different characters/equipment may eventually alter:
- heal amount;
- cooldown;
- delivery;
- secondary effects;
- charges;
- interactions with armour / weapons / augments.

Whether Heal is technically:
- a universal ability;
- a character innate;
- a prepared core slot;

remains open.

The player-facing principle is more important:

> Every character begins with a reliable sustain action.