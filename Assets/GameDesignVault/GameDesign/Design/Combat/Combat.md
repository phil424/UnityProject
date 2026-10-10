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

# Combat Death Lifetime

**Status: Confirmed**

Gameplay death and visual cleanup are separate lifetimes.

When Health reaches zero:

- the actor is gameplay-dead immediately;
- targeting, movement, combat and encounter completion must use gameplay death
  rather than waiting for the GameObject to be destroyed;
- rewards and progression may resolve immediately from gameplay death;
- presentation may keep the dead actor visible long enough to play authored
  death presentation;
- visual cleanup destroys the remaining runtime object only after that
  presentation lifetime finishes.

Death animation is presentation only. It must not own or delay gameplay death.

# Combat Presentation Cues

**Status: Confirmed**

Combat presentation observes authoritative gameplay events rather than owning
combat results.

Two current semantic presentation moments are distinct:

### Action Impact

An authored combat action reaches its Impact point.

This may produce source-side presentation such as:

- weapon swipes;
- attack trails;
- cast flashes;
- movement accents.

An Action Impact may still occur when the attack ultimately whiffs.

### Damage Resolved

`DamageResolver` successfully resolves positive damage against a target.

This may produce target-side presentation such as:

- impact sparks;
- blood;
- armour flashes;
- hit particles;
- damage sounds.

A whiff produces no Damage Resolved cue.

Presentation effects:

- do not apply damage;
- do not decide whether an attack connects;
- do not own death;
- do not determine combo progression;
- may outlive the gameplay event briefly as bounded visual-only runtime objects.

Gameplay remains authoritative.

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

## Current Character Stat Vocabulary

**Status: Working**

A stat should only become part of the general Character vocabulary when it has a
clear gameplay consumer.

### Survival

**Max Health**

Controls the Character's maximum Health capacity.

Current consumer:
`Health`.

**Armour**

Flat reduction against incoming damage.

Current consumer:
`CombatStats` / `DamageResolver`.

### Mobility

**Movement Speed**

Controls normal autonomous navigation speed.

Current consumer:
`AutoTargetMover` / `MovementSystem`.

Forced movement such as Charge or Knockback is not automatically scaled by
normal Movement Speed.

### Basic Attack

**Basic Attack Damage**

Controls baseline autonomous attack damage.

It does not automatically increase Ability damage.

Current consumer:
`CombatStats`.

**Basic Attack Speed**

Controls autonomous basic-attack cadence.

It does not automatically reduce Ability cooldowns.

Current consumer:
`CombatStats`.

**Basic Attack Range**

Controls valid range for autonomous basic-attack impact.

The base stat already exists.

Expedition modification / movement-positioning behaviour requires further N2
work before Range becomes a normal reward.

### Ability

**Ability Cooldown Recovery**

Reduces effective active-Ability cooldown duration through faster recovery.

It does not increase autonomous Basic Attack Speed.

### Control

**Knockback Power**

Increases outgoing forced-motion distance from compatible attacks / Abilities.

Current implementation scales displacement rather than speed.

This makes stronger Knockback both move targets farther and keep them in forced
motion for longer.

Knockback Resistance remains a future counterpart once enemies or other systems
can meaningfully apply forced movement to the playable Character.

## Deferred Candidate Stats

The following remain plausible but should not be implemented until their
gameplay behaviour is deliberately resolved.

### Basic Attack Range

Base support already exists.

A normal Expedition reward remains deferred because increasing attack range also
raises a movement/positioning question:

> Should autonomous combat deliberately maintain the newly increased preferred
> distance?

Do not treat Range as only a larger hit-validation radius.

### Knockback Resistance

Remain deferred until enemies or other gameplay systems provide meaningful
incoming forced movement against the playable Character.

A stat should not exist merely because it has a conceptual opposite.

## Deferred / Unproven Stats

Do not introduce these merely because they are common RPG statistics:

- Critical Chance;
- Dodge;
- generic Resistance;
- Healing Power;
- Ability Power;
- Status Power.

They remain available if later mechanics establish a clear need.

### Weapon-Specific Values
- attack coefficients
- combo coefficients
- range
- cadence.

### Questions
- Status power?

# Status Effects

## Stun

**Status: Implemented Foundation**

Stun is temporary runtime Actor control.

While Stunned, the current implementation prevents:

- normal autonomous navigation;
- autonomous basic attacks;
- autonomous support healing;
- new active Ability activation.

An executing basic attack is cancelled when Stun is applied.

Forced Motion remains valid while Stunned.

This is intentional so effects such as:

Knockback
+
Stun

can move the target away and then leave it unable to immediately re-engage.

Reapplication is non-additive.

A new Stun may refresh the remaining control window, but repeated hits do not
sum their full durations together.

Current first consumers:

Punchy Whirlwind
→ radial Knockback + Stun.

Ranger Backstep
→ departure Knockback + Stun.

The foundation does not yet forcibly interrupt every already-running
`ActorAbility` execution.

Extend that behaviour only when concrete enemy/player Ability interactions make
it necessary.

## Hit-Based Shields

**Status: Implemented Foundation**

Hit-based Shields are temporary Runtime Actor defence.

A Shield is a discrete layer with an authored number of hits required to break
it.

Example:

1 Shield
with
2 Hits Per Shield

absorbs two incoming damage applications before the Shield layer breaks.

While at least one Shield layer exists:

incoming hit
→ consumes one Shield hit
→ does not reach Armour
→ does not damage Health.

Shields have no time duration.

Additional Shields may be added as separate layers.

Existing partially-damaged Shield layers retain their remaining hit count when
new layers are added.

Current first consumer:

Ranger Guard.

Guard may add additional Shield layers on later activations.

Shield stacks remain Runtime Actor state and are not stored in persistent or
Expedition build progression.

Every current positive damage application through `DamageResolver` counts as one
Shield hit.

Do not introduce damage-type exceptions until concrete gameplay demonstrates a
need.

Dedicated Shield player-facing presentation is not implemented yet.

`HitShieldRuntime.Changed` provides the runtime seam for future presentation.

## Healing Over Time

**Status: Implemented Foundation**

Healing-over-time is temporary Runtime Actor state.

Each effect is identified by a source ID.

Reapplying the same source:

refreshes that healing effect

rather than:

creating another independent copy.

Different healing sources may coexist.

Healing ticks use the normal central `Health.Heal(...)` seam and therefore clamp
to Max Health.

Current first consumer:

Ranger Guard.

Guard's healing duration is independent from its Shield duration because Guard
Shields do not expire with time.

## Invulnerability

**Status: Implemented Foundation**

Invulnerability is temporary Runtime Actor state.

While at least one invulnerability source is active:

incoming damage
→ stops before Shield consumption;
→ stops before Armour;
→ does not damage Health.

Invulnerability may have more than one source.

Removing one source does not remove invulnerability supplied by another source.

Current first consumer:

Punchy Blink.

Blink applies invulnerability for the active traversal execution and removes it
when the traversal finishes.

Invulnerability is runtime-only and is not stored in persistent or Expedition
progression.

## Working / Future Status Vocabulary

- Bleed;
- Burning;
- Poison;
- Knockback;
- other control effects where concrete gameplay requires them.

For future statuses use:
## Status Name

### Purpose

### Application

### Stacking

### Duration

### Interactions

### Resistant Enemies

### Related Augments

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

