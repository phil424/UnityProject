# Ability Loadout and Runtime

**Status: Implemented Foundation**

# Purpose

This document describes current Ability ownership, Active-slot assignment and  
runtime reconstruction.

It complements the player-facing Ability design in:

`Design/Characters/Abilities.md`.

# Ownership Layers

Ability state has three relevant layers.

## Definition

`AbilityDefinition`

Defines reusable Ability identity and authored behaviour configuration.

It does not define:

- owning Character;
    
- Prepared/Core role;
    
- current Expedition slot;
    
- activation policy.
    

## Run-Owned Ability State

`RunBuild`  
→ `RunAbilityState`

Owns Expedition-lifetime development including:

- Ability ownership;
    
- Ability level;
    
- Evolutions;
    
- Prepared/Core role where applicable.
    

This state survives Region transitions.

It ends with the Expedition.

## Runtime Ability State

`ActorAbility`

Owns live Actor state such as:

- cooldown;
    
- current execution;
    
- execution-specific runtime state.
    

Runtime Ability objects are reconstructed from RunBuild when an Actor is  
spawned for a new Region.

# Prepared/Core Ability State

Prepared starting Abilities enter RunBuild through:

`PartyMemberDefinition.StartingAbilities`

Each prepared Ability records one:

- Signature;
    
- Utility;
    
- Sustain.
    

A Prepared/Core Ability is always considered active during the Expedition.

Prepared roles are stored in `RunAbilityState`.

They are not stored in `AbilityDefinition`.

# Expedition Ability Ownership

Abilities acquired during an Expedition are added to the same RunBuild-owned  
Ability collection.

Expedition-owned Abilities use:

`CharacterAbilityRole.Unassigned`

because they do not occupy one of the Prepared/Core Character roles.

Acquiring an Expedition Ability does not require an available Active slot.

Ownership may therefore exceed Active capacity.

# Expedition Active Slots

`RunBuild` owns exactly three Expedition Active Ability slots.

Acquisition behaviour:

owned Ability acquired  
→ first empty Expedition slot exists  
→ automatically assign it;

otherwise:

owned Ability acquired  
→ all Expedition slots occupied  
→ keep Ability owned but unslotted.

Only Expedition-owned Abilities may occupy these slots.

Prepared/Core Abilities cannot be assigned to them.

# Slot Reassignment

Slot mutation follows:

AbilityLoadoutUI  
→ RunDirector  
→ RunProgress  
→ RunState  
→ RunBuild.

Normal player-facing reassignment is accepted only while:

- an Expedition is active;
    
- the run is currently inside a level;
    
- simulation is paused;
    
- no pending Reward choice is unresolved.
    

Selecting an Ability already assigned to another Expedition slot swaps the two  
slot contents.

Selecting an unslotted Ability replaces the target slot's current Ability and  
leaves the displaced Ability owned but unslotted.

Selecting Empty clears the target slot without removing Ability ownership.

# Runtime Application

`PartyAbilityApplicator` iterates all RunBuild-owned Abilities.

For each owned Ability:

1. find its existing runtime instance;
    
2. create the runtime instance if required;
    
3. apply its Run-owned level and Evolutions;
    
4. enable the runtime component when the Ability is active;
    
5. disable the runtime component when an Expedition Ability is owned but  
    unslotted.
    

This avoids destroying Run ownership when Active-slot assignment changes.

`ActorAbility.OnDisable()` ends an executing Ability safely.

`AbilitySystem` already ignores disabled Ability components.

# Runtime Refresh

A successful slot change causes:

RunBuild slot mutation  
→ RunProgress.Changed  
→ RunDirector refreshes the live Party Member  
→ StageDirector.RefreshPartyMemberRuntime  
→ PartyAbilityApplicator reapplies Ability state  
→ PartyMemberRuntimeChanged  
→ AbilityHUD rebuilds presentation.

# Combat HUD

`AbilityHUD` presents six fixed Active positions.

Prepared group:

1. Signature;
2. Utility;
3. Sustain.

Expedition group:

1. Expedition Slot 1;
2. Expedition Slot 2;
3. Expedition Slot 3.

Empty Expedition positions remain visible.

`AbilityHUD` remains responsible for slot ordering and for resolving each
RunAbilityState to its live ActorAbility instance.

`AbilityButtonUI` is responsible for the presentation of one position.

The current button presentation uses:

- a diamond frame/background;
- the resolved Ability icon;
- current Ability level;
- resolved Ability display name;
- cooldown countdown;
- radial cooldown wash.

Cooldown fill uses:

`ActorAbility.CooldownFractionRemaining`.

The value represents the fraction of the effective cooldown still remaining.

`1`
→ full cooldown.

`0`
→ ready.

Ability presentation uses:

`ActorAbility.DisplayName`
and
`ActorAbility.DisplayIcon`.

These already resolve presentation-replacing Evolutions through
`AbilityPresentationResolver`.

The HUD therefore does not contain Ability-specific icon or name rules.

## Chevron Layout

Each three-slot Ability group is arranged by
`ThreeSlotChevronLayoutGroup`.

Direct-child order is semantic:

Child 0
→ top-centre.

Child 1
→ lower-left.

Child 2
→ lower-right.

Current geometry:

- slot size: `108`;
- horizontal centre offset: `60`;
- vertical centre offset: `54`.

The Expedition and Prepared/Core groups are stacked with `-54` vertical layout
spacing.

Matching the stack overlap to the internal vertical offset preserves one
consistent staggered lattice across all six diamonds.

## Diamond Input Geometry

`AbilityButtonUI` implements `ICanvasRaycastFilter`.

Unity layout still uses a rectangular AbilityButtonUI RectTransform.

Pointer interaction instead uses a diamond test based on the local point:

`abs(x) / halfWidth + abs(y) / halfHeight <= 1`.

A small authored inset keeps adjacent diamond boundaries from sharing the same
pointer region.

Only the AbilityButtonUI root Image should participate in raycasting.

Child icon, cooldown and TMP Graphics use:

`Raycast Target = Off`.

This prevents the deliberately staggered HUD layout from producing overlapping
rectangular click regions.

# Lifetime Rules

Persistent / Meta  
→ may later determine which Prepared Ability options are unlocked.

Setup  
→ chooses the future prepared loadout.

Run / Expedition  
→ owns Ability ownership, levels, Evolutions and Expedition slot assignment.

Runtime Actor  
→ owns cooldown and live execution state.

Do not move Expedition slot assignment into persistent progression.

Do not encode Prepared/Core roles permanently onto shared Ability Definitions.

# Current Boundaries

The current implementation does not yet provide:

- persistent alternative Prepared Ability selection;
    
- equipment-granted additional Active slots;
    
- Passive Abilities;
    
- drag-and-drop loadout interaction;
    
- hotkey/controller slot rebinding;
    

Those remain later roadmap work.