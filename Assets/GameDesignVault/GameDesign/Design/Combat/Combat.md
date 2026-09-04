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

# Targeting and Tactical Direction

Autonomous combat does not mean the player has no influence over target choice.

Working direction:

The player can configure how the hero evaluates combat targets.

Potential base targeting rules:
- Closest
- Weakest
- Strongest / Highest Health

Target rules may be combined with separate priorities such as:
- Prefer Elite
- Prefer Boss
- Prefer Rare / Valuable

Example:

Closest
+
Prefer Elite

This should mean:
> Prefer an appropriate Elite when available, otherwise behave according to the
> Closest targeting rule.

Avoid defining every possible combination as a separate targeting mode.

See [[Targeting, Tactics and Encounter Direction]].

# Player Intervention
- active abilities controlled by player
- immediate response
- intentional cast times allowed
- abilities may cancel attacks
- deciding whether to cancel a combo is gameplay
- optional future autocast.

Player agency operates at several scales:

### Strategic
- choose which encounter / opportunity to pursue;
- redirect toward rare or valuable encounters.

### Tactical Targeting
- configure autonomous target policy;
- adjust priorities such as Elite / Rare targets.

### Immediate Intervention
- activate abilities;
- interrupt autonomous combat at important moments.

The intent is not to directly control routine movement and attacks, but to give
the player meaningful control over what the autonomous character is trying to
achieve.

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

