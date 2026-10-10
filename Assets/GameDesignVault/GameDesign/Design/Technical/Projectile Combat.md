# Projectile Combat

**Status: Implemented Composable Foundation**

# Purpose

Projectile Combat provides delayed ranged delivery for Basic Attacks and
Abilities.

Projectile travel and projectile impact behaviour are deliberately separate
concerns.

The current architecture supports composing proven travel shapes with proven
impact payloads without introducing a specialised spawn method for each
combination.

# High-Level Flow

Projectile consumer
↓
`ProjectileLaunchRequest`
↓
`ProjectileLauncher`
↓
`ProjectileRuntime`
↓
travel / hit discovery
↓
`ProjectileImpactResolver`
↓
damage / optional Burn / optional knockback.

# Projectile Definition

`ProjectileDefinition`

owns reusable projectile presentation / kinematic configuration:

- prefab;
- speed;
- hit distance;
- maximum lifetime;
- source-forward spawn offset.

It does not define the complete gameplay effect of every projectile launch.

The same Projectile Definition may therefore participate in different
deliveries and impact payloads.

# Launch Request

`ProjectileLaunchRequest`

describes one runtime launch.

It contains:

- Projectile Definition;
- source Actor;
- `ProjectileDelivery`;
- `ProjectileImpactPayload`.

Consumers construct a request and launch it through:

`ProjectileLauncher.Launch`.

`ProjectileRuntime` no longer owns prefab creation or the public launch API.

# Delivery

`ProjectileDelivery`

describes how the projectile reaches targets.

Current delivery modes are:

## Targeted

Projectile tracks one intended target.

Current examples:

- Ranger pre-Core Basic Attack;
- standalone Impact Arrows;
- standalone Fire Arrows;
- generic targeted projectile Ability behaviour.

## Targeted Piercing

Projectile tracks the intended primary target.

After the primary impact it continues in its current travel direction and may
hit additional valid targets.

Current examples:

- Piercing Arrows;
- Piercing Impact;
- Piercing Flame.

Delivery configuration owns:

- travel distance;
- target faction;
- hit radius;
- maximum hit count.

It does not own Damage, Burn or knockback.

## Directional

Projectile travels in a fixed authored direction.

Movement segments are queried for valid targets in travel order.

Current example:

Ranger Power Shot.

Directional delivery supports the same reusable impact payload as other
projectile travel shapes.

# Projectile Launcher

`ProjectileLauncher`

owns:

- launch validation;
- initial travel direction;
- spawn position;
- projectile prefab instantiation;
- `ProjectileRuntime` initialization.

This prevents launch/setup responsibilities from accumulating inside the live
projectile component.

# Projectile Runtime

`ProjectileRuntime`

owns the live projectile lifecycle and movement state.

Current responsibilities:

- lifetime;
- simulation-pause handling;
- current travel direction;
- targeted movement;
- targeted-piercing transition;
- directional movement;
- hit-count / distance exhaustion;
- already-hit tracking;
- handing discovered impacts to `ProjectileImpactResolver`.

It does not own:

- prefab spawning;
- Reward / Weapon Mastery logic;
- Burn implementation;
- knockback implementation;
- faction-query implementation;
- specialised Piercing+Burn or Piercing+Knockback launch APIs.

`TravelDirection` remains exposed for projectile presentation such as
`ProjectileSpritePresenter`.

# Directional Hit Query

`ProjectileHitQuery`

owns segment-based target discovery.

It handles:

- Health discovery;
- source exclusion;
- already-hit exclusion;
- faction filtering;
- EntityAvoidance radius contribution;
- closest-point segment distance;
- hit ordering along the travelled segment.

This query is separate from impact payload behaviour.

Discovering a target does not itself decide whether that target Burns,
receives knockback or only takes damage.

# Impact Payload

`ProjectileImpactPayload`

describes the proven effects delivered by a projectile hit.

Current payload capabilities:

- direct Damage;
- optional Knockback;
- optional Burn;
- optional Stun.

Knockback and Burn use focused typed payloads rather than a universal effect
enum/list.

Current composition examples:

Piercing Arrows
→ Targeted Piercing
+
Damage.

Impact Arrows
→ Targeted
+
Damage
+
Knockback.

Piercing Impact
→ Targeted Piercing
+
Damage
+
Knockback.

Fire Arrows
→ Targeted
+
Damage
+
Burn.

Piercing Flame
→ Targeted Piercing
+
Damage
+
Burn.

Power Shot
→ Directional
+
Damage
+
Knockback
+
Stun.

This allows existing behaviours to be recombined without adding another
specialised `ProjectileRuntime.Spawn...` method.

Stun uses the same focused typed-payload pattern as Knockback and Burn.

Projectile impact therefore remains composable without introducing a universal
effect enum or specialised Power-Shot projectile runtime.

# Impact Resolver

`ProjectileImpactResolver`

owns application of the current impact payload.

Resolution order is:

direct Damage
↓
Burn where configured and the target survives
↓
Knockback where configured and the target survives.

Damage continues through:

`DamageResolver`.

Burn continues through:

`BurningDamageRuntime`.

Knockback continues through:

`KnockbackResolver`.

Outgoing Knockback Power therefore continues to affect projectile knockback
through the shared combat-stat seam.

For directional hits, projectile geometry may provide an explicit directional /
fan knockback direction.

For normal targeted impacts, `KnockbackResolver` resolves ordinary
source-to-target knockback direction.

# Burning

Burn is currently a focused proven projectile impact primitive.

`BurningDamageRuntime`

owns the target-side timed damage state.

Current behaviour:

- one Burn runtime per target;
- new Burn application refreshes/replaces the existing Burn;
- Burn does not currently stack independently;
- simulation pause pauses Burn timing.

Burn is not currently represented by a generic status-effect framework.

Do not introduce one until additional concrete status gameplay demonstrates the
need.

# Weapon Mastery Integration

Weapon Mastery definitions decide which Delivery and Impact Payload to construct.

The projectile runtime does not understand:

- Core Mastery;
- Alternate Mastery;
- persistent synthesis;
- Weapon Mastery levels.

Examples:

Piercing Mastery
→ chooses Targeted Piercing delivery.

Impact Mastery
→ adds Knockback payload.

Fire Mastery
→ adds Burn payload.

Pairwise synthesis may change delivery while retaining the active Mastery's
impact identity.

This keeps Weapon Mastery buildcraft outside the generic projectile runtime.

# Ability Integration

Abilities use the same launch architecture.

Current Power Shot:

directional delivery
+
damage
+
directional knockback.

Future projectile Abilities should prefer composing existing delivery and
impact primitives where appropriate.

Do not build a second projectile framework for Abilities.

# Presentation

`ProjectileSpritePresenter`

remains separate from projectile gameplay.

It reads the live projectile's `TravelDirection` to orient camera-facing
projectile sprites.

# Current Limitations

Not yet implemented:

- ballistic trajectories;
- terrain collision;
- arbitrary physical interception for targeted shots;
- ricochet;
- projectile pooling;
- final elemental/status architecture;
- specialised projectile destruction/interception interactions.

These should be introduced only when representative gameplay requires them.

# Extension Rule

When adding new projectile content:

1. first ask whether the content can be represented by an existing Delivery;
2. then ask whether it can be represented by existing Impact Payload behaviour;
3. compose those existing seams where possible;
4. add a new focused primitive only when the content genuinely introduces new
   travel or impact behaviour.

Do not add another specialised launch method merely because two existing
behaviours are being used together.