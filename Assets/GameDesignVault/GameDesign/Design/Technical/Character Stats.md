# Character Stats

**Status: Working Technical**

# Purpose

Character stats are gameplay values consumed by concrete runtime systems.

The project deliberately separates:

- authored base values;
- Persistent / prepared progression;
- Expedition progression;
- temporary Runtime modifiers.

Do not collapse these lifetimes into one permanently mutated value.

# Ownership

## Authored Base Values

Base runtime values currently live on the Actor prefab components that consume
them.

Examples:

`Health`
- Max Health.

`CombatStats`
- Basic Attack Damage;
- Basic Attack Range;
- Basic Attack Cooldown;
- Armour.

`AutoTargetMover`
- Movement Speed.

This avoids creating a duplicate all-purpose base-stat database while the
runtime components already own meaningful authored values.

## Persistent / Prepared Modifiers

Character Mastery is the first implemented prepared-stat source.

Persistent ownership lives in `PersistentProgression`.

When an Expedition begins, unlocked Mastery nodes are copied into
`RunBuild` as Prepared stat modifiers.

`RunBuild` keeps:

Prepared modifiers
+
Expedition modifiers

as separate sources.

Runtime consumers receive their combined effective value.

The shared `CharacterStatModifierType` vocabulary exists because both
Character Mastery and Expedition stat rewards now modify the same concrete
runtime stats.

Do not add new stat types without a real gameplay consumer.

## Expedition Modifiers

`RunBuild` owns temporary Character progression for one Expedition.

Current Expedition stat modifiers include:

- Basic Attack Damage;
- Movement Speed;
- Basic Attack Speed;
- Armour;
- Max Health;
- Ability Cooldown Recovery;
- Ability Damage;
- Ability Area Size;
- Knockback Power;
- Shield Capacity.

Prepared and Expedition modifiers use the same shared
`CharacterStatModifierType` vocabulary.

They survive Region / level transitions and disappear when the Expedition ends
unless a modifier originated from prepared persistent progression.

## Shield Capacity

Controls the maximum number of hit-based Shield layers an Actor may currently
hold.

Authored base capacity lives on actor-owned `ShieldCapacityStats`.

Prepared Character Mastery and Expedition Character Growth rewards may increase
that capacity through the shared `CharacterStatModifierType.ShieldCapacity`
vocabulary.

Current consumers:

Ranger
→ authored base Shield Capacity `3`.

Expanded Guard
→ Rare Character Growth Reward;
→ Ranger-only;
→ +1 temporary Expedition Shield Capacity.

Persistent Mastery may later provide additional Shield Capacity through the same
shared stat vocabulary.

Shield layers themselves remain Runtime Actor state.

Current Shield layers remain Runtime Actor state and are not stored in
`RunBuild`.

## Runtime Temporary Modifiers

Short-duration effects belong to runtime actor state.

`RuntimeStatModifiers` currently supports effects such as Rage.

Examples:
- temporary outgoing damage;
- temporary Attack Speed;
- temporary Movement Speed;
- temporary Armour.

Temporary effects must not permanently mutate `RunBuild` or authored prefab
values.

# Application

Current player Character reconstruction flow:

RunBuild
+
Persistent prototype progression
        ↓
PartyUpgradeApplicator
        ↓
runtime Actor components

When the party Actor is recreated for another Region/level in the same
Expedition, its effective values are reconstructed from the surviving
`RunBuild`.

# Stat Semantics

## Max Health

Character-wide survival capacity.

## Armour

Character-wide flat incoming-damage reduction.

Current working calculation:

```
positive incoming damage
-
effective flat Armour
=
final damage,
```

with a current minimum of 1 final damage for a positive damaging hit that has
already passed Invulnerability and Shields.

Therefore Armour may heavily mitigate a weak hit but does not currently erase it
completely.

This minimum remains Working and should be reassessed against a broader enemy /
damage roster.

## Movement Speed

Normal navigation only.

## Basic Attack Damage

Autonomous basic-attack damage.

Does not imply Ability damage.

## Basic Attack Speed

Autonomous basic-attack cadence.

Does not imply Ability cooldown recovery.

## Basic Attack Range

Autonomous basic-attack reach.

Base support exists; normal Expedition modification requires further work.

## Ability Cooldown Recovery

Actor-wide modifier for active Ability cooldown duration.

Current formula:

Effective Cooldown
=
Base Cooldown / (1 + Cooldown Recovery Percent / 100)

This is deliberately distinct from Basic Attack Speed.

Current consumer:
`ActorAbility` through actor-owned `AbilityStats`.

Ability cooldown recovery remains actor-generic and does not depend directly on
player `RunBuild`.

## Ability Damage

Actor-wide percentage modifier for damage explicitly produced by compatible
Abilities.

Current formula:

Effective Ability Damage
=
Authored Ability Damage × (1 + Ability Damage Percent / 100).

Current runtime consumer:

`ActorAbility`
→ shared `ResolveAbilityDamage(...)`
→ actor-owned `AbilityStats`.

Concrete damaging Abilities opt into this shared helper.

Current consumers include:

- Blink;
- Whirlwind;
- Charge;
- Projectile Shot;
- Power Shot / Piercing Projectile Shot.

Basic Attacks do not use this calculation.

General Runtime outgoing-damage modifiers are applied separately through the
existing `DamageResolver` seam and may therefore affect both Basic Attack and
Ability damage when that temporary effect is intentionally universal.

## Ability Area Size

Actor-wide percentage modifier for compatible Ability effect geometry.

Current formula:

Effective Linear Ability Size = Authored Size × (1 + Ability Area Size Percent / 100).

Current runtime consumer:

`ActorAbility`
→ shared `ResolveAbilityAreaSize(...)`
→ actor-owned `AbilityStats`.

Current representative consumers:

Whirlwind
→ activation/effect radius.

Blink
→ opening control radius.

Smoke Step
→ smoke/Fear radius.

Ability Area Size is deliberately opt-in.

It does not automatically alter:

- targeting/search distance;
- movement distance;
- projectile travel range;
- projectile hit radius;
- Basic Attack range;
- Knockback distance.

This prevents one broad percentage stat from accidentally changing unrelated
mechanics merely because they are expressed as distances or radii.

## Knockback Power

Controls outgoing forced-motion distance produced through the generic knockback
pipeline.

Current behaviour:

- scales outgoing knockback distance;
- scales compatible pull distance;
- does not scale forced-motion speed;
- does not alter Charge's own travel distance;
- propagated motion inherits the already-modified source motion rather than
  applying Knockback Power a second time.

Current consumer:
`KnockbackResolver` through actor-owned `KnockbackStats`.

# Important Invariants

- `RunBuild` owns Expedition progression, not live Actor components.
- live Actor components are reconstructed from progression state.
- temporary timed effects stay Runtime-owned.
- Ability stats must remain actor-generic rather than depending directly on
  player `RunBuild`.
- adding a stat requires a concrete gameplay consumer.
- avoid a giant universal stat enum until concrete systems demonstrate that one
  is actually useful.