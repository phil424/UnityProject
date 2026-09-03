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