**Status: Canonical**

These rules protect the generic ability, reward and buildcraft architecture.

# Ability architecture rules

Abilities are an important generic gameplay primitive.

## Ability behaviour must remain actor-generic

An ability should not inherently assume:

```
Player
Punchy
Enemy
Boss
```

Where practical, the same underlying ability should be usable by different actor types.

Keep this conceptual separation:

```
Ability behaviour
        !=
Ability ownership
        !=
Activation policy
```

Example:

```
WhirlwindAbility
    = gameplay behaviour

Player RunBuild
    = player owns Whirlwind

Boss loadout
    = boss owns Whirlwind

Ability HUD
    = player requests activation

Enemy/Boss AI
    = AI requests activation
```

Both should ultimately use the same ability execution seam.

---

# Player abilities are deliberate interventions

Baseline combat remains autonomous:

- targeting;
- movement;
- weapon/basic attacks;
- support behaviour.

Player abilities are normally manually triggered tactical actions.

Do **not** bake automatic activation back into the generic `AbilitySystem`.

The normal model is:

```
Player input/UI
      ↓
activation request
      ↓
ActorAbility.TryActivate()
      ↓
ability gameplay
```

Optional autocasting may exist later as a separate policy.

---

# Abilities must support different behaviour shapes

Do not design the ability architecture around only instant AoE attacks.

It should remain capable of supporting at least:

### Instant

```
Whirlwind
```

Immediate effect.

### Timed runtime effect

```
Rage
```

Applies temporary modifiers that expire.

### Movement-duration

```
Charge
```

Actor actively moves for a period/distance and can interact with enemies during that motion.

Future abilities may also be:

- targeted;
- directional;
- ground-targeted;
- cast;
- channelled;
- charge-based.

Do not implement all of these generically until required, but avoid assumptions that prohibit them.

---

# Ability state has three conceptual layers

Keep these distinct:

```
ABILITY DEFINITION
"What is Whirlwind?"

Static data / runtime behaviour identity
```

```
RUN ABILITY STATE
"What has this build developed?"

owned
level
future augments/evolutions
```

```
RUNTIME ABILITY STATE
"What is happening right now?"

cooldown
active duration
temporary execution state
```

Runtime actors should reconstruct their abilities from run-owned state when appropriate.

Enemy/boss ability loadouts should be able to use the same generic ability definitions without requiring player `RunBuild` or currency systems.

---

# Ability levels and ability expansions are different

Treat these as two separate progression axes.

### Ability level

Incremental predictable improvement:

```
Whirlwind Lv.1
→ Lv.2
→ Lv.3
```

Examples:

- damage;
- radius;
- cooldown;
- knockback;
- duration.

### Ability expansion / augment

Transformative buildcraft:

```
Whirlwind
├── applies Fire
├── creates a damage trail
├── knockback causes explosions
└── gains another designed behaviour
```

Do not reduce ability buildcraft to number increases alone.

Run-owned ability state should remain extensible enough to store acquired augments/evolutions later.

Avoid prematurely building a giant universal effect framework. Generic infrastructure should manage ownership/configuration; concrete abilities may interpret their own meaningful augment behaviour.

---

# Temporary modifiers stay runtime-owned

Temporary effects such as Rage should use runtime modifier/effect state.

Do not permanently mutate:

```
base character values
AbilityDefinition
RunBuild
```

to implement a short-lived buff.

Temporary modifiers may eventually support:

- outgoing damage;
- attack speed;
- movement speed;
- armour;
- healing;
- debuffs;
- resistance;
- other temporary gameplay stats.

Prefer central stat/damage seams so modifiers affect all appropriate gameplay sources consistently.

---

# Forced movement and knockback remain generic primitives

Existing combat-motion architecture includes:

```
ForcedMotion
KnockbackResolver
KnockbackPropagation
MovementSystem
AvoidanceSystem
```

Use these for new abilities where appropriate.

For example:

```
Charge
→ self movement
→ enemy contact
→ KnockbackResolver
```

Do not create a second movement/physics framework solely for an ability unless the existing architecture genuinely cannot represent the mechanic.

Future knockback systems should remain extensible toward:

- outgoing knockback modifiers;
- incoming knockback resistance/vulnerability;
- boss/heavy-actor resistance;
- knockback-triggered effects;
- extra damage caused by knockback.

Do not prematurely implement them without a current use case.

---

# Reward architecture must stay generic

A reward is a delivery mechanism, not synonymous with a stat increase.

Rewards may eventually provide:

```
stat upgrade
ability
ability augment/evolution
material
persistent unlock
other future content
```

Do not grow `RunUpgradeEffectType` into a giant list of things such as:

```
AcquireCharge
AcquireRage
WhirlwindFire
WhirlwindRadius
...
```

Prefer generic reward-definition types that know:

```
Can this reward apply?
What does it display?
How does it modify the build?
```

Reward generation should increasingly understand:

- existing build;
- eligibility;
- synergies;
- avoiding dead choices;
- interesting pivots.

---

# Major rewards and small purchases are distinct

Keep these progression concepts separate.

### Significant/random reward choices

Examples:

```
Learn Charge
Learn Rage
Whirlwind gains an augment
major build-changing option
```

### Small deterministic purchases

Examples:

```
Whirlwind Lv.1 → Lv.2
incremental gear/stat improvement
```

They may modify the same `RunBuild`, but they should not necessarily use the same economy or presentation.

---

# Buildcraft should remain extensible

Current/future run builds may contain:

- abilities;
- ability levels;
- ability expansions/evolutions;
- temporary augments;
- stat progression;
- synergies;
- temporary runtime effects.

Avoid architecture that assumes the finished build is only a list of flat stat bonuses.

Future synergy/tag concepts may include:

```
Melee
Projectile
Area
Movement
Bleed
Fire
Poison
Explosion
Knockback
Critical
Healing
Shield
Attack
Ability
Cooldown
```

Do not implement a large tag system until there is a concrete need, but avoid design decisions that make one difficult later.

---

# Future crafting/resources should not be prematurely implemented

Current future design direction includes possible:

- monster resources;
- persistent materials;
- Monster Hunter-inspired crafting;
- weapons/armour built from monster materials;
- gear talents/skills;
- interactions between gear, abilities and augments;
- longer/persistent expedition spaces.

These require a dedicated design pass.

Do **not** begin implementing crafting/resource architecture merely because future plans mention it.

Current development should only ensure that:

- gear/buildcraft remains modular;
- abilities remain extensible;
- persistent/run/runtime state boundaries stay clean.

---

# Weapon architecture should remain compatible with future identity

Weapons are intended to define autonomous combat playstyle rather than act as simple stat sticks.

Future weapons may influence:

- combo sequence;
- cadence;
- range;
- movement;
- innate abilities;
- cancel windows;
- invulnerability;
- status behaviour;
- augmentation capacity.

Do not build current combat architecture around the assumption that every weapon is simply:

```
Damage + AttackSpeed
```