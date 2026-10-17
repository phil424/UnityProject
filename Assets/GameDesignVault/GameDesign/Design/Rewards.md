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

`RunRewardVector.WeaponMod` now has its first correct concrete reward
implementation.

`WeaponModLevelRewardDefinition`

targets a specific Weapon Mod already present in the Character's current
`RunBuild`.

Eligibility requires:

- a valid Character;
- a valid RunBuild;
- a configured target Weapon Mod;
- Character compatibility;
- that Weapon Mod already being prepared in the current RunBuild.

Applying the reward increases that Mod's temporary Expedition level.

It does not:

- unlock the permanent stance;
- activate the stance;
- affect other Weapon Mods;
- persist the level beyond Expedition end.

The first proof reward is:

Heavy Finisher Training
→ Heavy Finisher +1 temporary level.

Current Weapon Mod level rewards:

Heavy Finisher Training
→ develops Punchy's Heavy Finisher.

Piercing Arrows Training
→ develops Ranger's Piercing Arrows.

The shared reward pool may contain rewards for several Characters.

Eligibility is derived from:

Character compatibility
+
the Weapon Mod states currently present in that Character's RunBuild.

The reward pool therefore does not require a separate Character-specific list.

## First Runtime Foundation

**Status: Implemented Foundation**

The runtime Reward Vector implementation currently supports:

- Character Growth;
- Ability;
- Weapon Mod.

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

Weapon Mod
→ temporary level/development rewards for prepared Weapon Mod states

Existing generic reward sources may request `Any`, which preserves the complete
eligible reward pool.

Future vectors such as persistent resources should be introduced alongside
their real reward implementations rather than predeclared speculatively.

Weapon Mod reward definitions remain separate from Weapon Mod definitions.

The Weapon Mod defines persistent stance identity, compatibility and combat
behaviour.

The Run Reward develops an existing Expedition `RunWeaponModState`.

Run Rewards do not normally unlock the underlying permanent stance.

## Quick Encounter Reward Focus

**Status: Implemented Foundation**

Reward Vectors can now influence which currently-selectable opportunities are
surfaced in the three quick Encounter slots.

Current focuses:

Balanced
→ normal oldest-selectable ordering.

Character Growth
→ Character Growth Encounters first.

Ability
→ Ability Encounters first.

Focus is deliberately a soft preference.

If fewer than three matching Encounters exist, remaining slots use the normal
general-purpose ordering.

Quick Encounter Focus does not change:

- Encounter generation;
- Encounter availability;
- expiry;
- authored Reward Vector;
- the reward produced when the Encounter completes.

It only changes the quick-access ranking of opportunities that already exist.

# Reward Provenance

**Status: Implemented Foundation**

A pending reward should retain enough context for the player to understand where
it came from.

Current reward context contains:

- Reward Vector;
- source label.

Examples:

South Road Horde
→ Growth Reward

Wide Graveyard Horde
→ Ability Reward

Elite Zombie
→ General Reward

Area Clear
→ General Reward

This becomes increasingly important as several reward sources can exist in the
same expedition and multiple reward choices may be queued before the player
opens them.

Reward source context should survive into Pending Rewards rather than being lost
when offers are generated.

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

# Reward Rarity

**Status: Implemented Foundation / Balance Working**

Reward rarity now affects normal Run Reward generation.

Current rarity vocabulary:

- Common;
- Uncommon;
- Rare;
- Epic;
- Legendary.

The default Reward Catalogue authors a rarity-weight profile.

Current working baseline:

| Rarity | Weight |
| --- | ---: |
| Common | 60 |
| Uncommon | 25 |
| Rare | 10 |
| Epic | 4 |
| Legendary | 1 |

These are Working balance values rather than permanent constants.

## Selection Order

Normal Reward generation currently follows:

Reward Source
→ Reward Vector filter
→ Character / build eligibility
→ candidate pool
→ represented rarity tiers
→ weighted rarity roll
→ Reward selection inside that rarity.

Rarity is therefore rolled only from tiers that have eligible candidates.

Example:

if the current eligible candidate pool contains only Common and Rare Rewards,
the Common / Rare weights are normalised against each other.

Unavailable Uncommon / Epic / Legendary tiers do not consume failed rolls.

Each selected offer is removed before the next card is generated.

## Content Count Does Not Define Rarity Chance

Adding several new Rewards to one rarity tier does not automatically make that
rarity tier more common.

The Catalogue rolls the rarity first.

It then chooses among the eligible Rewards in the selected tier.

This allows content breadth to expand independently from the baseline rarity
economy.

## Rarity and Reward Power

Rarity currently controls generation frequency.

It does not automatically scale a Reward's gameplay effect.

Reward Definitions still author their own actual effects.

O4C's Character Growth balance pass is responsible for aligning Reward values
with their intended rarity.

Rarity also remains separate from broader Reward-role concepts such as:

Frequent
Significant
Major.

Those concepts describe the role / magnitude of a Reward rather than its
selection probability.

# Character Growth Rewards

**Status: Implemented First Production Pass / Balance Working**

Character Growth is temporary Expedition stat development.

The active Character Growth pool contains:

32 Character Growth Rewards.

Current rarity distribution:

| Rarity    | Rewards |
| --------- | ------: |
| Common    |       9 |
| Uncommon  |       9 |
| Rare      |       9 |
| Epic      |       5 |
| Legendary |       0 |

There is deliberately no Legendary Character Growth reward in the current pass.

Legendary presentation should be reserved until content exists that merits that
level of excitement rather than merely supplying a larger generic stat number.

## Reward Roles

Character Growth should support several kinds of decision.

Safe value
→ useful even when it does not define the build.

Examples:

- Max Health;
- Armour;
- modest Basic Attack Damage;
- modest Movement Speed.

Directional development
→ becomes increasingly valuable when repeatedly invested into.

Examples:

- Ability Cooldown Recovery;
- Ability Damage;
- Ability Area Size;
- Basic Attack Speed;
- Knockback Power.

Character-specific development may also use the shared Character Growth system
when the stat is not meaningful for every Character.

Current example:

Expanded Guard
→ Ranger only
→ +1 Shield Capacity.

## Current Working Value Ladders

Basic Attack Damage:

Common
→ +1.

Uncommon
→ +2.

Rare
→ +3.

Epic
→ +4.

Basic Attack Speed:

Common
→ +10%.

Uncommon
→ +20%.

Rare
→ +30%.

Max Health:

Common
→ +6.

Uncommon
→ +10.

Rare
→ +16.

Movement Speed:

Common
→ +10%.

Uncommon
→ +15%.

Epic
→ +30%.

Ability Cooldown Recovery:

Common
→ +10%.

Uncommon
→ +20%.

Rare
→ +30%.

Ability Damage:

Common
→ +10%.

Uncommon
→ +20%.

Rare
→ +35%.

Epic
→ +50%.

Ability Area Size:

Common
→ +10%.

Uncommon
→ +20%.

Rare
→ +30%.

Epic
→ +45%.

Armour:

Common
→ +0.5.

Uncommon
→ +1.

Rare
→ +1.5.

Epic
→ +2.

Knockback Power:

Common
→ +15%.

Uncommon
→ +30%.

Rare
→ +50%.

Shield Capacity:

Rare
→ +1.
→ currently Ranger-only.

These are Working balance values.

Their purpose is to provide a coherent first playable economy for further
playtesting rather than define permanent final balance.

## O4C4 Balance Playtest Outcome

**Status: Initial Playtest Complete / Detailed Balance Deferred**

The first structured Character Growth balance pass produced:

|Area|Result|
|---|---|
|Common Reward usefulness|Mixed|
|Rarity power|Good|
|Build direction|Problem|
|Choice quality|Mixed|
|Reroll quality|Good|
|Progression balance|Mixed|

The playtest confirmed that Reward rarity and rerolls currently provide a useful foundation.

It also exposed that individual Character Growth stats cannot yet be judged confidently in isolation.

In particular, some directional stats may function correctly without yet feeling sufficiently meaningful because their gameplay value depends on wider systems such as:

- Ability interactions;
    
- enemy behaviour and combat roles;
    
- encounter pressure;
    
- positioning value;
    
- additional future Character stats;
    
- presentation and feedback;
    
- broader build combinations.
    

Knockback Power is an example: increased displacement is observable, but the value of creating that additional space depends heavily on the surrounding combat and buildcraft ecosystem.

The current Character Growth values therefore remain Working rather than final.

Detailed tuning of individual stats, additional stat directions and the overall relationship between preparation and Expedition growth is deliberately deferred until the prototype contains a broader representative set of:

- Expedition Abilities;
    
- persistent Mastery development;
    
- Character-specific buildcraft;
    
- enemy/combat content;
    
- build interactions.
    

This work should be revisited during the later integrated buildcraft and balance pass rather than blocking broader prototype development now.

## O4C4 Initial Balance Finding

**Status: Verified / Detailed Balance Deferred**

The first structured Character Growth balance pass found:

| Area | Result |
| --- | --- |
| Common Reward usefulness | Mixed |
| Rarity power | Good |
| Build direction | Problem |
| Choice quality | Mixed |
| Reroll quality | Good |
| Progression balance | Mixed |

This is not currently treated as a request for isolated numerical retuning.

The playtest demonstrated that several Character Growth stats need a broader
gameplay ecosystem before their value can be judged properly.

For example, Knockback Power visibly creates additional space but does not yet
have enough surrounding setup/payoff interactions to make that space reliably
feel like a complete build direction.

Detailed assessment is therefore deferred until O4G after the prototype contains
a broader Ability pool, additional Character stats, Active/Passive buildcraft,
Mastery development and richer combat interactions.

## Stacking

Character Growth modifiers accumulate for the active Expedition.

Different Rewards affecting the same Character stat also stack.

Example:

Heavy Impact
+
Crushing Impact

develop the same Knockback-oriented build direction at different rarity/value
levels.

Character Growth resets with the Expedition.

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

# Persistent Resource Rewards

**Status: Implemented Foundation**

Persistent resources are distinct from temporary Run Reward choices.

Current enemy proof:

Normal enemy
→ Level 1.

Elite
→ Level 1 + Level 2.

Boss
→ Level 1 + Level 2 + Level 3.

These values are authored per enemy through `EnemyMember`.

Persistent-resource earnings accumulate in the active Expedition and are banked
when the Expedition ends.

Showcase / Preparation actors do not grant resources because they remain outside
the normal StageDirector-owned Region actor lifecycle.

Resource Reward Vectors and resource-focused Encounter selection remain future
work.

Current Weapon Mastery level rewards include:

Heavy Finisher Training
→ Punchy Core Mastery.

Piercing Arrows Training
→ Ranger Core Mastery.

Impact Arrows Training
→ Ranger Alternate1 Mastery.

A Character may therefore have several simultaneously eligible Weapon Mastery
development rewards.

Each reward targets its own `RunWeaponModState`.

Developing one Mastery does not alter the temporary level of another.