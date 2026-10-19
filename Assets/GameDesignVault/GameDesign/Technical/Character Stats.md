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

Shield Capacity controls the maximum number of hit-based Shield layers that may
exist simultaneously on an Actor.

Runtime representation:

`ShieldCapacityStats`
→ capacity.

`HitShieldRuntime`
→ active Shield layers.

Current base capacity:

`3`.

Expedition Shield Capacity modifiers may increase that value where the Reward is
eligible.

## Source-Aware Shield Layers

Shield layers may record:

- their creating GameObject;
- a stable source ID;
- their creating action name.

`HitShieldRuntime` raises `ShieldBroken` when one complete Shield layer is
consumed.

The event reports both:

- the Shield layer's source identity;
- the attack that broke it.

This allows reactive systems to respond only to Shields they actually created.

Current representative consumer:

Glacial Aegis
→ adds source-identified Glacial Shields
→ listens for break events matching its own source ID
→ triggers Frost Nova.

Guard continues using normal Shield layers and does not trigger Glacial Aegis.

Owned Ability cleanup may remove Shield layers belonging to that Ability's source
ID when the Ability is disabled/unslotted.

## Runtime Temporary Modifiers

Short-duration Character effects belong to Runtime Actor state.

`RuntimeStatModifiers` currently supports temporary effects including:

- outgoing damage;
- Basic Attack Speed;
- Movement Speed;
- Armour;
- Basic Attack Range;
- Basic Attack Penetration.

Wind Whip is the current representative player-facing consumer.

Temporary Runtime effects must not permanently mutate:

- authored Character values;
- Ability Definitions;
- RunBuild Character-stat progression.

When the temporary source expires or is removed, the effective Character values
return to the RunBuild/authored baseline.

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

Base range remains authored on `CombatStats`.

Temporary Runtime modifiers may increase or decrease effective Basic Attack
Range.

Current formula:

Effective Basic Attack Range = Base Attack Range × (1 + Runtime Range Percent / 100).

Normal combat movement adjusts its stopping distance by the difference between
base and effective Attack Range.

This preserves each Actor's existing authored spacing while allowing temporary
range effects to make the Character genuinely fight from farther away.

Basic Attack Range remains separate from:

- Ability range;
- Ability Area Size;
- projectile hit radius;
- targeting/search radius.

It is currently a Runtime modifier rather than a normal Character Growth stat.

## Basic Attack Penetration

Temporary Basic Attack Penetration allows one Basic Attack to affect additional
targets aligned beyond its primary target.

The current value represents:

additional targets.

Therefore:

`0`
→ primary target only.

`1`
→ primary + one additional target.

`2`
→ primary + two additional targets.

Melee Basic Attacks use the shared
`BasicAttackPenetrationResolver`.

Projectile Basic Attacks preserve their projectile delivery and are promoted to
or extended as piercing delivery through `ProjectileLauncher`.

This stat is intentionally Basic-Attack-specific.

Ability projectiles do not inherit Basic Attack Penetration unless their own
Ability behaviour explicitly provides piercing.

## Ability Cooldown Recovery

Actor-wide modifier for active Ability cooldown duration.

Current formula:

Effective Cooldown = Base Cooldown / (1 + Cooldown Recovery Percent / 100)

This is deliberately distinct from Basic Attack Speed.

Current consumer:
`ActorAbility` through actor-owned `AbilityStats`.

Ability cooldown recovery remains actor-generic and does not depend directly on
player `RunBuild`.

## Ability Damage

Actor-wide percentage modifier for damage explicitly produced by compatible
Abilities.

Current formula:

Effective Ability Damage = Authored Ability Damage × (1 + Ability Damage Percent / 100).

Current runtime seam:

`ActorAbility`
→ `ResolveAbilityDamage(...)`
→ actor-owned `AbilityStats`.

Current representative consumers include:

- Blink;
- Whirlwind;
- Earthshatter contact damage;
- Earthshatter landing damage;
- Power Shot / Piercing Projectile Shot;
- Storm Field pulse damage.

Basic Attacks do not use Ability Damage.

General Runtime outgoing-damage modifiers are applied separately through the
existing damage pipeline and may affect both Basic Attack and Ability damage
when that effect is intentionally universal.

## Ability Area Size

Actor-wide percentage modifier for compatible Ability effect geometry.

Current formula:

Effective Linear Ability Size = Authored Size × (1 + Ability Area Size Percent / 100).

Runtime seam:

`ActorAbility`
→ `ResolveAbilityAreaSize(...)`
→ actor-owned `AbilityStats`.

Current representative consumers include:

Whirlwind
→ activation/effect radius.

Blink
→ opening control radius.

Smoke Step
→ smoke/Fear radius.

Earthshatter
→ rush contact radius;
→ landing shockwave radius.

Storm Field
→ persistent field radius.

Ability Area Size is deliberately opt-in.

It does not automatically alter:

- targeting/search distance;
- movement distance;
- projectile travel distance;
- projectile collision radius;
- Basic Attack range;
- Knockback distance;
- Earthshatter group-search radius;
- Earthshatter rush distance.

Concrete Abilities explicitly decide which geometry represents their scalable
effect area.

## Knockback Power

Controls outgoing forced-motion distance produced through the shared Knockback
pipeline.

Current behaviour:

- scales outgoing Knockback distance;
- scales compatible pull distance;
- does not scale forced-motion speed;
- does not alter an Ability's own self-movement distance;
- propagated motion inherits already-modified source motion rather than applying
  Knockback Power a second time.

Current representative consumers include:

Whirlwind
→ radial Knockback / compatible pull behaviour.

Earthshatter
→ rush-contact displacement;
→ landing shockwave displacement.

Knockback Power therefore becomes more meaningful as Abilities create additional
setup/payoff interactions around enemy positioning.

# Important Invariants

- `RunBuild` owns Expedition progression, not live Actor components.
- live Actor components are reconstructed from progression state.
- temporary timed effects stay Runtime-owned.
- Ability stats must remain actor-generic rather than depending directly on
  player `RunBuild`.
- adding a stat requires a concrete gameplay consumer.
- avoid a giant universal stat enum until concrete systems demonstrate that one
  is actually useful.