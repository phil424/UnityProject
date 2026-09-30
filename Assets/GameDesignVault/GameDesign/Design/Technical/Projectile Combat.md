# Projectile Combat

**Status: Implemented Foundation**

# Purpose

Projectile Combat provides delayed ranged delivery for Combat Actions.

It is separate from direct action-impact damage.

# Runtime Flow

CombatActionState
↓
action reaches authored Impact time
↓
CombatActionSystem validates source / target / range
↓
ProjectileCombatActionDefinition
↓
ProjectileRuntime spawned
↓
projectile travels
↓
target reached
↓
DamageResolver

Direct CombatActionDefinitions continue resolving damage immediately at action
impact.

# Ownership

`ProjectileCombatActionDefinition`
defines which Projectile Definition a Combat Action launches.

`ProjectileDefinition`
owns projectile runtime configuration such as:
- prefab;
- speed;
- hit distance;
- maximum lifetime;
- source-forward offset.

`ProjectileRuntime`
owns:
- target tracking;
- travel;
- lifetime;
- final damage delivery.

`ProjectileSpritePresenter`
owns camera-facing 2D projectile presentation.

# Current Behaviour

Projectile Combat currently supports two delivery modes.

## Targeted Projectile

Used by Ranger's normal bow attack.

Flow:

launch
↓
projectile tracks the intended target
↓
target reached
↓
DamageResolver

This prioritises reliable autonomous basic ranged combat.

## Directional Projectile

Used by Ranger Power Shot.

Flow:

ability chooses direction at release
↓
projectile travels straight
↓
movement segment checks valid targets along the path
↓
targets are resolved in travel order
↓
each target may be hit once
↓
projectile continues until maximum range or hit count is exhausted

Directional projectiles therefore support piercing without using the homing
behaviour of normal Ranger arrows.

# Knockback

Directional projectiles may apply directional knockback when configured.

Power Shot uses lateral target offset from the projectile line to deflect
knockback outward.

This produces a forward fan / arc of displaced enemies while retaining a single
straight projectile path.

Outgoing `KnockbackStats` still modify final knockback distance through the
existing generic `KnockbackResolver` seam.

# Current Consumers

Ranger basic attack
→ Combat Action impact launches a targeted projectile;
→ damage occurs on projectile arrival.

Ranger Power Shot
→ Ability windup launches a directional projectile;
→ projectile may pierce multiple enemies;
→ damage and knockback occur as the projectile reaches each enemy.

Both use the shared:

`ProjectileDefinition`
+
`ProjectileRuntime`

delivery architecture.

# Current Limitations

The projectile foundation does not yet implement:

- ballistic trajectories;
- terrain collision;
- projectile pooling;
- physical interception by arbitrary unintended targets for targeted shots;
- ricochet;
- projectile lifetime interactions beyond configured travel/lifetime rules.

These should be added only when real content requires them.

# Future Extension

The same foundation is intended to support future ranged content such as:

- Arbalist Skeleton bolts;
- magical projectiles;
- Ability Evolutions;
- multi-shot directional volleys.

The current working Power Shot Evolution direction is a three-shot spread /
rapid volley built from multiple directional projectile launches rather than a
new projectile framework.