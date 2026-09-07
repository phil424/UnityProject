# Weapons
### Weapon Purpose
Weapons define autonomous playstyle.

### Weapon May Control
- damage behaviour
- combo sequence
- cadence
- range
- movement
- targeting behaviour
- innate abilities
- augmentation capacity
- status interactions
- defensive properties.

### Design Rule

> Two weapons should not merely be mathematically equivalent attacks occurring at different speeds.

### Weapon Identity Questions
For each weapon:
- What does it feel like?
- What does the player watch for?
- When is its combo worth completing?
- What risks does it take?
- What kinds of abilities naturally interact with it?
- What does it struggle with?

# Weapon Combo Model
Define a conceptual combo step:
```
Combo Step
├── Animation
├── Duration
├── Damage Coefficient
├── Range
├── Movement
├── Invulnerability
├── Knockback
├── Status
├── Cancel Window
└── Special Behaviour
```

Important:
A dash-through combo step may contain invulnerability.

A heavy finisher may be valuable enough that the player deliberately delays an ability.

# Weapon Catalogue
Create a reusable weapon template:

```
# Weapon Name

**Status:** Concept

## Fantasy

## Autonomous Playstyle

## Combo

### Attack 1

### Attack 2

### Attack 3

### Finisher

## Innate Behaviour

## Innate Ability

## Augment Slots

## Strengths

## Weaknesses

## Interesting Ability Synergies

## Open Questions
```

No need to design weapons yet.

# Persistent Weapon Progression Tree

Each weapon should have an authored progression tree that develops its
autonomous combat identity over long-term play.

Potential nodes include:

### Incremental
- damage;
- attack speed;
- attack range;
- movement speed during attacks;
- combo coefficients;
- knockback;
- critical / status behaviour where appropriate.

### Combo Development
- faster combo transitions;
- modified attack steps;
- additional combo step;
- stronger finisher;
- improved cancel windows;
- movement during specific attacks;
- invulnerability on appropriate steps.

### Unlock
- innate weapon ability;
- additional ability slot;
- augment slot;
- alternate weapon behaviour;
- new targeting interaction.

### Identity
- unique passive;
- weapon-specific mechanic;
- conditional attack behaviour;
- special interaction with abilities;
- unusual movement / defensive property.

Progression should gradually make the weapon feel more complete and expressive,
not merely numerically stronger.