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

The first projectile uses lightweight target tracking.

This prioritises reliable autonomous ranged combat while the projectile is in
flight.

The current foundation does not yet implement:
- ballistic trajectories;
- collision with unintended targets;
- piercing;
- AoE impact;
- projectile pooling;
- terrain collision.

Those behaviours should be added only when concrete content requires them.

# First Consumer

Ranger basic attack
→ bow release
→ travelling arrow
→ target damage on arrow arrival.

The same architecture is intended to support later ranged enemy content such as
the supplied Arbalist Skeleton.