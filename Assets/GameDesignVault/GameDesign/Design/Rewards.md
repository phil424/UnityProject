# Overview

> Reward is the delivery mechanism for progression.

A reward may provide:
- stat growth
- augment
- ability
- evolution
- material
- unlock
- other future content.

# Reward Lifetime

Rewards must explicitly communicate which progression lifetime they affect.

## Expedition Rewards

Temporary.

Examples:
- ability acquisition;
- ability level;
- evolution;
- temporary augment;
- temporary stat growth.

These reset when the expedition ends.

## Persistent Rewards

Survive expedition end.

Examples:
- permanent ability unlock;
- equipment;
- crafting material;
- persistent currency;
- character / support unlock;
- recipe / collection unlock.

Persistent rewards should not be hidden inside temporary reward state.

If a persistent reward is earned, its permanent ownership should be committed
through the persistent progression layer.

# Ability Unlock vs Acquisition

Ability Unlock:
- persistent availability.

Ability Acquisition:
- ownership during one expedition.

A reward may potentially do both.

Example:

Rare encounter reward
→ permanently unlocks Vortex
+
→ grants Vortex immediately during the current expedition.

The exact reward rules remain future design.

# Sources
Potential sources:
- normal enemies
- elites
- boss kills
- objectives
- ecological events
- threat milestones
- Apex.

# Tiers
Don't confuse this immediately with rarity.

Explore:
### Frequent
Small progression.

### Significant
Build-shaping.

### Major
Abilities/evolutions/transformation.

# Generation
Define conceptual process:
```
Reward Trigger
↓
Inspect Current Build
↓
Inspect Region / Ecology
↓
Build Candidate Pool
↓
Weight Synergies
↓
Weight Interesting Pivots
↓
Avoid Dead Choices
↓
Generate Offers
```

# Weighting
Principles:
- no pure RNG
- recognise the player's build
- support existing strategy
- occasionally present attractive pivots
- avoid three irrelevant offers
- don't guarantee the perfect combination.

# Pending Rewards
Document queue, button, travel interaction and deferral.

