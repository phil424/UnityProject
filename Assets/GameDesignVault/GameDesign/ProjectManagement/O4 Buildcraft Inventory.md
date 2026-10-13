# O4 Buildcraft Inventory

**Status: O4 Baseline / Living Register**

# Purpose

Record the actual current Character/buildcraft content before O4 substantially
expands Rewards, shared Abilities and Character Mastery.

This document separates:

- generic architecture;
- reusable gameplay primitives;
- authored shared content;
- Character-specific content;
- persistent progression;
- Expedition progression;
- prototype / proof content;
- known gaps.

The purpose is to establish what should be:

preserved
→ improved
→ expanded
→ replaced
→ retired.

# Shared Foundations

## Character Stat Vocabulary

Current shared Character stat modifiers:

| Stat | Lifetime support | Current notes |
| --- | --- | --- |
| Basic Attack Damage | Persistent + Expedition | Generic offence |
| Movement Speed | Persistent + Expedition | Generic mobility |
| Basic Attack Speed | Persistent + Expedition | Generic Basic Attack cadence |
| Armour | Persistent + Expedition | Flat incoming-damage mitigation |
| Max Health | Persistent + Expedition | Generic survivability |
| Ability Cooldown Recovery | Persistent + Expedition | Active Ability frequency |
| Knockback Power | Persistent + Expedition | Shared forced-movement scaling |
| Shield Capacity | Persistent + Expedition architecture | Current first consumer is Ranger; no authored progression source yet |

The vocabulary should expand only where concrete buildcraft needs justify another
shared Character stat.

## Combat / Control Vocabulary

Current reusable combat primitives include:

| Primitive | Current representative use |
| --- | --- |
| Damage | Basic Attacks / Abilities / projectiles |
| Projectile delivery | Ranger attacks and Power Shot |
| Piercing | Ranger projectile content |
| Forced Motion | Charge / Knockback |
| Knockback | Whirlwind / Blink / Power Shot / Weapon Mastery |
| Stun | Whirlwind / Blink / Power Shot |
| Fear | Smoke Step |
| Invulnerability | Blink |
| Hit Shields | Guard |
| Healing Over Time | Guard |
| Direct Healing | War Cry |
| Temporary Armour | War Cry |
| Temporary stat modifiers | Rage / debug effects |
| Burning | Fire Weapon Mastery |

These are generic runtime vocabulary.

Concrete Abilities should compose them where useful rather than creating
parallel implementations.

## Ability Architecture

Current conceptual layers:

Ability Definition
→ static authored Ability identity.

RunBuild Ability State
→ current Expedition ownership / level / evolution state.

Runtime Actor Ability
→ cooldown / execution / temporary runtime state.

Ability behaviour remains actor-generic where practical.

Character ownership is separate from Ability behaviour.

## Current Ability Library

Primary Ability definitions currently present:

| Ability | Current Character use | Current content status |
| --- | --- | --- |
| Blink | Punchy Signature | Core |
| Whirlwind | Punchy Utility | Core |
| War Cry | Punchy Sustain | Core; shared authored content |
| Power Shot | Ranger Signature | Core |
| Smoke Step | Ranger Utility | Core |
| Guard | Ranger Sustain | Core |
| Rage | Optional Expedition Ability | Active Reward currently Punchy-only |
| Charge | None in current core kits | Reusable / inactive content |

Existing Evolution proof assets:

| Evolution | Current status |
| --- | --- |
| Fan Volley | Parked; baseline fan behaviour moved into Power Shot |
| Vortex | Parked from normal Reward generation |

# Reward Architecture

Current runtime flow:

Reward Source
→ RunRewardContext
→ RunUpgradeOfferGenerator
→ PendingRewardChoice
→ player selection
→ RunRewardDefinition
→ RunBuild.

Current Reward Vectors:

- Character Growth;
- Ability;
- Weapon Mastery;
- Any / General.

Current concrete Reward families:

| Reward family | Purpose |
| --- | --- |
| RunUpgradeDefinition | temporary Character stat growth |
| AbilityUnlockRewardDefinition | acquire an Ability for this Expedition |
| AbilityEvolutionRewardDefinition | transform/develop an owned Ability |
| WeaponModLevelRewardDefinition | temporary level for a prepared Weapon Mastery |
## Reward Authoring Lifecycle

O4B2 completes the normal Reward-content lifecycle through the Content
Workbench.

Current supported operations include:

Reward Sets:

- create;
- add existing;
- remove from active Catalogue;
- permanently delete Set asset.

Rewards:

- create;
- add existing;
- remove from an individual Set;
- permanently delete Reward asset.

Permanent Reward deletion removes the Reward from every Reward Set found in the
project before deleting the ScriptableObject.

Permanent Reward Set deletion removes the Set from every Reward Catalogue found
in the project before deleting the Set asset.

Deleting a Reward Set does not delete the Reward assets it referenced.

Non-destructive membership changes remain Undo-compatible.

Permanent asset deletion requires confirmation and is not treated as Undo.

The Workbench also provides explicit cleanup for broken/null Catalogue and Set
memberships.

The Reward authoring surface is now suitable for the substantially larger
content pool planned during O4C/O4D.

## Active Default Reward Catalogue

Current default Catalogue contains:

- Core Character Growth;
- Core Abilities;
- Punchy Weapon Mastery;
- Ranger Weapon Mastery.
Current active Reward count:

28.

Breakdown:

Character Growth
→ 23.

Ability
→ 1.

Weapon Mastery
→ 4.

Common
→ 7.

Uncommon
→ 7.

Rare
→ 11.

Epic
→ 3.

Legendary
→ 0.

## Current Character Growth Rewards

O4C2 replaces the seven exaggerated proof Rewards with a 23-Reward first
production pool.

Current distribution:

| Rarity | Count |
| --- | ---: |
| Common | 7 |
| Uncommon | 7 |
| Rare | 6 |
| Epic | 3 |
| Legendary | 0 |

Current stat coverage:

| Stat | Current Reward development |
| --- | --- |
| Basic Attack Damage | Common / Uncommon / Rare / Epic |
| Basic Attack Speed | Common / Uncommon / Rare |
| Max Health | Common / Uncommon / Rare |
| Movement Speed | Common / Uncommon / Epic |
| Ability Cooldown Recovery | Common / Uncommon / Rare |
| Armour | Common / Uncommon / Epic |
| Knockback Power | Common / Uncommon / Rare |
| Shield Capacity | Rare Ranger-specific |

The pool intentionally contains both:

safe generic value
+
deeper stat-investment paths.

Current Reward values are a first production balance pass and remain Working.

Shield Capacity now has its first Expedition progression source:

Expanded Guard
→ Rare
→ Ranger only
→ +1 Shield Capacity.

## Current Ability Rewards

Active default catalogue:

| Reward | Rarity | Eligibility |
| --- | --- | --- |
| Learn Rage | Rare | Punchy |

Current result:

Punchy
→ has one active Expedition Ability-acquisition Reward.

Ranger
→ has no currently eligible Ability-acquisition Reward in the active catalogue.

Parked Ability Reward assets include:

- Fan Volley;
- Vortex.

## Current Weapon Mastery Rewards

| Character | Reward |
| --- | --- |
| Punchy | Heavy Finisher Training |
| Ranger | Piercing Arrows Training |
| Ranger | Impact Arrows Training |
| Ranger | Fire Arrow Training |

Each currently grants +1 temporary Expedition level to an already-prepared
compatible Weapon Mastery.

## Reward Rarity Audit

Runtime rarity vocabulary:

- Common;
- Uncommon;
- Rare;
- Epic;
- Legendary.

Presentation supports all five.

O4C1 makes rarity part of actual Reward generation.

The active Reward Catalogue now owns the working rarity profile:

Common
→ 60.

Uncommon
→ 25.

Rare
→ 10.

Epic
→ 4.

Legendary
→ 1.

Generation now:

filters Reward Vector
→ filters Character / build eligibility
→ determines represented rarity tiers
→ rolls rarity using Catalogue weights
→ selects an eligible Reward from that rarity.

Unavailable rarity tiers are skipped and the remaining weights are effectively
normalised.

The number of authored Rewards inside one rarity no longer determines that
rarity's baseline selection frequency.

Current Reward values themselves remain prototype-scale.

Aligning Reward effect magnitude, breadth and buildcraft role with rarity is the
next O4C target.

# Mastery Architecture

Current Mastery node types:

| Node type | Current purpose |
| --- | --- |
| Character Stat | permanent prepared Character stat |
| Ability Level | permanent starting Ability level |
| Weapon Mastery Stance | permanently prepare/unlock a stance |
| Weapon Mastery Level | increase permanent prepared stance level |
| Weapon Mastery Combination | pair compatible Weapon Masteries |

Current persistent Mastery ownership is binary.

A node is currently:

locked
→ purchased once
→ permanently unlocked.

Persistent progression stores unlocked Mastery-node IDs.

Rankable / multi-level nodes do not currently exist.

## Mastery Authoring

Current normal authoring surface:

Tools
→ MiniCrawler
→ Content Workbench
→ Mastery.

The Workbench currently supports:

- node creation;
- node editing;
- prerequisites;
- costs;
- spatial layout;
- node-type-specific fields;
- validation;
- related-content navigation;
- Undo-compatible graph movement.

O4B1 adds explicit Mastery-content lifecycle authoring through the Content
Workbench.

Current normal actions now include:

- Add Existing Node;
- Remove From Tree;
- Delete Node Asset;
- explicit missing-reference cleanup;
- automatic prerequisite cleanup;
- automatic direct Mastery-combination reference cleanup;
- Undo-compatible non-destructive membership changes.

Permanent node-asset deletion remains explicitly destructive and requires
confirmation.

Normal validation remains non-destructive.

The Unity Inspector / Project window remains an advanced escape hatch rather than
the required removal/deletion workflow.

Reward asset lifecycle cleanup remains the next O4B target.

This should be addressed before the trees grow substantially.

# Punchy

## Core Ability Kit

Signature
→ Blink.

Utility
→ Whirlwind.

Sustain
→ War Cry.

## Persistent Mastery

Current Punchy Mastery contains 5 authored nodes.

| Node | Type | Effect |
| --- | --- | --- |
| Heavy Finisher Mastery | Weapon Mastery Stance | prepares Heavy Finisher |
| Heavy Hands | Character Stat | +2 Basic Attack Damage |
| Combo Tempo | Character Stat | +15% Basic Attack Speed |
| Conditioning | Character Stat | +15 Max Health |
| Impact Training | Character Stat | +25% Knockback Power |

Current tree demonstrates the architecture but remains prototype-scale.

Current Punchy Mastery does not yet provide:

- persistent Ability-level branches;
- multiple Weapon Mastery directions;
- substantial branch depth;
- repeated/rankable development;
- major branch payoffs.

## Expedition Buildcraft

Currently available through the active Reward catalogue:

- generic Character Growth Rewards;
- Heavy Finisher temporary levels;
- Rage acquisition;
- Run Currency Ability levels.

## Existing Build Connections

Knockback Power can improve several Punchy behaviours through shared knockback
infrastructure.

Max Health and Armour support Punchy's endurance identity.

Basic Attack Damage / Attack Speed support Punchy's autonomous combo identity.

## Current Gaps

Punchy's Mastery tree is very small.

Punchy's active Ability Reward pool contains only Rage.

Much of Punchy's Expedition development currently comes from raw stat increases
rather than transformative buildcraft.

# Ranger

## Core Ability Kit

Signature
→ Power Shot.

Utility
→ Smoke Step.

Sustain
→ Guard.

## Persistent Mastery

Ranger currently has 12 resolvable authored Mastery-node assets plus one
additional unresolved node reference in `RangerMasteryDefinition`.

Current resolvable nodes include:

### Weapon Mastery Stances

- Piercing Mastery;
- Impact Arrows Mastery;
- Fire Arrows Mastery.

### Weapon Mastery Development

- Deep Penetration;
- Intense Heat.

### Weapon Mastery Combinations

- Piercing Impact;
- Piercing Flames.

### Ability Development

- Power Shot Drills.

### Character Stats

- Quick Draw;
- Mobility;
- Arrow Impact;
- Flaming Powershot prototype node.

## Prototype / Cleanup Finding

`Flaming Powershot` currently has the description:

`yeah baby`

and its actual gameplay effect is:

+5 Basic Attack Damage.

It does not currently make Power Shot flaming.

Treat this as prototype content requiring redesign or retirement.

The unresolved `RangerMasteryDefinition` node reference identified during O4A
was explicitly cleaned through the O4B1 Workbench repair workflow.

The active Ranger tree now contains 12 resolvable node memberships.

`Flaming Powershot` remains active prototype content pending its deliberate O4F
redesign / retirement decision.

## Expedition Buildcraft

Current active Weapon Mastery Reward paths:

- Piercing temporary level;
- Impact temporary level;
- Fire temporary level.

Generic Character Growth Rewards are also available.

Power Shot / Smoke Step / Guard may be developed through current Run Currency
Ability-level progression.

## Current Gaps

Ranger currently has no active eligible Ability-acquisition Reward in the default
Reward catalogue.

Shield Capacity is implemented as a shared Character stat but currently has no
authored persistent Mastery node or Expedition Character Growth Reward.

Several Ranger Mastery nodes remain proof/prototype content rather than a
finished persistent progression tree.

# Shared Expedition Content

## Existing

Rage is the only active additional Expedition Ability Reward.

Its current Reward eligibility restricts it to Punchy.

Charge exists as reusable Ability content but is not currently part of the active
Reward catalogue.

## Missing

There is not yet a meaningful shared Expedition Ability pool.

Future shared Ability content should provide run-specific build directions
without replacing the Character's Signature / Utility / Sustain identity.

Candidate areas for later design include:

- area damage;
- periodic lightning / storm effects;
- control;
- Fear;
- defensive effects;
- movement;
- utility;
- other designed gameplay roles.

These remain future O4D design work rather than current implementation
requirements.

# O4 Action Register

## Preserve

Preserve:

- Ability Definition / RunBuild / Runtime Actor separation;
- generic Ability execution architecture;
- generic combat/control vocabulary;
- Reward Catalogue + modular Reward Sets;
- Reward Vector filtering;
- concrete Reward eligibility;
- pending Reward choices;
- Character Mastery definitions;
- the Content Workbench as the primary authoring front end;
- persistent versus Expedition progression separation.

## Improve

Improve:

- Mastery node removal/deletion workflow;
- Reward asset cleanup/deletion workflow;
- Reward rarity generation;
- Reward weighting;
- Reward balance;
- Reward breadth;
- generic safe Reward choices;
- build-directed Reward choices;
- rerolls;
- Mastery depth;
- Mastery branch quality;
- persistent Ability development.

## Replace / Retire

Review for replacement or retirement:

- Flaming Powershot prototype Mastery content;
- stale/missing Ranger Mastery reference;
- prototype Reward tuning;
- parked Fan Volley Reward design;
- parked Vortex Reward design.

Proof assets may remain where they provide useful architectural evidence without
remaining active player-facing content.

## New Content Required

Likely O4 expansion areas:

- broader Character Growth Reward pool;
- meaningful rarity structure;
- Reward rerolls;
- shared Expedition Abilities;
- Ranger Ability Reward paths;
- Shield Capacity progression;
- deeper Punchy Mastery;
- deeper Ranger Mastery;
- rankable persistent development where justified;
- more substantial Character-specific build directions.