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

`RunBuild` owns temporary Character progression for one expedition.

Current Expedition stat modifiers include:

- Basic Attack Damage;
- Movement Speed;
- Basic Attack Speed;
- Armour;
- Max Health;
- Ability Cooldown Recovery;
- Knockback Power;
- Shield Capacity.

These survive Region / level transitions and disappear when the Expedition ends.

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