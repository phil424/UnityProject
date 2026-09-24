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

# Reward Vectors

An Encounter does not need to reveal the exact reward for the choice to be
meaningful.

Instead, it may communicate a Reward Vector.

Potential vectors include:

- Character Growth;
- Ability;
- Weapon Mod;
- Persistent Resource;
- High Currency / Resource Yield;
- Hunt Progress;
- Rare Opportunity;
- other future categories.

Example:

Encounter
├── Threat
├── Reward Vector
├── Expected Resource Yield
├── Objective / Hunt Relevance
└── Urgency

This allows informed choice while preserving uncertainty inside the actual
reward generation.

## First Runtime Foundation

**Status: Implemented Foundation**

The first runtime Reward Vector implementation supports:

- Character Growth;
- Ability.

Encounter Definitions may author:
- a Reward Vector;
- a number of reward choices granted on completion.

The quick Encounter HUD exposes the vector before selection.

When the Encounter completes, generated offers are filtered to eligible rewards
matching that advertised vector.

Current mapping:

Character Growth
→ `RunUpgradeDefinition`

Ability
→ Ability acquisition and Ability Evolution rewards

Existing generic reward sources may request `Any`, which preserves the complete
eligible reward pool.

Future vectors such as Weapon Mods or persistent resources should be introduced
alongside their real reward implementations rather than predeclared
speculatively.

# Intention-Changing Rewards

A reward does not always need to make the Character directly stronger.

Some rewards may intentionally change what the player wants to pursue next.

Examples:

- increased Undead resource gain for the next few Encounters;
- additional Elite rewards;
- improved rarity on the next Ability-related Encounter;
- increased Boss resource yield for a limited duration;
- temporary bonuses connected to a particular Encounter category.

These rewards create future intention.

They may help organically extend a short expedition into a longer one because
the player discovers a new opportunity worth pursuing.

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

