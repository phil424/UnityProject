# Overview
> The hero autonomously handles baseline combat execution while the player makes high-value intervention decisions through abilities.

### Autonomous Layer
- target acquisition
- navigation
- weapon combos
- basic attacks.

### Player Layer
- ability timing
- ability targeting
- combo interruption
- tactical response.

# Autonomous Combat
- select target
- approach target
- execute weapon behaviour
- progress combo
- respond to target death
- pursue/reposition.

Important principle:
> Automatic behaviour must never make the character feel unresponsive to player ability input.

# Player Intervention
- active abilities controlled by player
- immediate response
- intentional cast times allowed
- abilities may cancel attacks
- deciding whether to cancel a combo is gameplay
- optional future autocast.

# Damage and Stats
### Core Combat Stats
Potential:
- Health
- Attack
- Armour
- Movement Speed
- Attack Speed
- Ability Cooldown
- Critical hits
- Dodge
- Resistance
- Healing power
- Cooldown reduction

### Weapon-Specific Values
- attack coefficients
- combo coefficients
- range
- cadence.

### Questions
- Status power?

# Status Effects
- Bleed
- Burning
- Poison
- Knockback
- Stun / control

For each future status use:
```
## Bleed

### Purpose

### Application

### Stacking

### Duration

### Interactions

### Resistant Enemies

### Related Augments
```

# Combat Resources
Questions:
- cooldown-only abilities?
- charges?
- rage?
- energy?
- character-specific resources?
- weapon-generated resources?

Record principle:
> Resources exist to create interesting timing decisions, not complexity for its own sake.

