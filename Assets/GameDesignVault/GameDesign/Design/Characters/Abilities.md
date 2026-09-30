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

# Core Character Ability Roles

**Status: Confirmed Direction**

Every playable Character begins with a three-part core:

Signature
+
Utility
+
Sustain

These are Character / loadout roles.

They are not permanent categories stored on `AbilityDefinition`.

The same generic Ability architecture remains usable by other Characters,
enemies, bosses and reward systems.

## Signature

Defines a major piece of the Character's active combat identity.

It should normally provide something the Character's basic autonomous combat
does not already accomplish.

Current examples:

Punchy
→ Whirlwind.

Ranger
→ Power Shot.

## Utility

Provides tactical intervention such as:

- movement;
- control;
- positioning;
- escape;
- setup;
- disruption.

Current Ranger example:

Backstep
→ restore useful ranged spacing after an enemy closes distance.

## Sustain

Provides a reliable survival decision.

Sustain does not imply direct healing.

Potential Sustain forms include:

- healing;
- regeneration;
- shielding;
- guarding;
- temporary mitigation;
- Armour;
- avoidance;
- conversion of another resource into survivability.

Current Ranger example:

Guard
→ short defensive commitment;
→ temporary Armour;
→ clear active protection presentation.

The important player-facing principle is:

> Every Character should begin with a reliable way to intervene when survival
> becomes threatened, but that intervention should reinforce Character identity.

# Ranger Ability Development

## Power Shot

Baseline:

one directional piercing projectile.

Current level scaling may improve:
- damage;
- range;
- pierce count;
- knockback;
- cooldown.

Working Evolution direction:

three-shot / spread / rapid volley.

This Evolution should build on the existing directional projectile seam rather
than creating another projectile system.

## Backstep

A contextual spacing tool.

It should normally become useful when Ranger is closer to a threat than the
Character's preferred ranged spacing.

## Guard

A mitigation-based Sustain Ability.

The short blocking presentation communicates activation while the longer
defensive aura communicates the remaining buff duration.