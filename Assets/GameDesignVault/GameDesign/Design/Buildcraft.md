# Overview
- player intentionally begins with a direction;
- game respects that intention;
- preparation should not fully guarantee the build;
- long-term player may influence perhaps ~30% of the build;
- ecology can alter opportunity cost;
- pivots should be tempting;
- several viable build paths are preferable to one optimal solution.

# Starting Build

**Status: Working**

The player begins an expedition with a deliberate direction rather than a
finished build.

Potential prepared elements include:

- active Character;
- Character's fixed basic attack / weapon identity;
- Character progression/mastery;
- Mastery-unlocked Weapon Mod stances prepared for that Character;
- starting/native Abilities;
- future configurable shared Abilities where compatible;
- tracked Progression Target;
- starting Objective context where appropriate.

Document:

> Starting setup establishes build intent rather than final build completion.

# Prepared / Persistent Build

The prepared build exists outside the expedition and survives expedition end.

The current preferred direction is Character-centric rather than centred on
swappable Weapon / Armour loadouts.

Potential persistent preparation includes:

- Character ownership;
- Character Mastery;
- Weapon Mod unlocks;
- Ability unlocks;
- selected compatible preparation options;
- cross-Character unlocks;
- persistent progression resources.

Character Mastery now provides the first implemented Character-centric
persistent progression surface.

The older Weapon / Armour / Focus persistent progression is no longer exposed
as the normal player-facing progression model.

Temporary compatibility/tooling seams may remain until their remaining
dependencies are cleaned up.

Persistent progression should broaden available possibilities without replacing
the need for meaningful Expedition buildcraft.

# Expedition Build
Expedition build can contain:

- acquired Abilities;
- Ability Evolutions;
- active Weapon Mods;
- temporary augments;
- stat levels;
- synergies;
- temporary effects.

Weapon Mods now have their first implemented RunBuild ownership seam.

Current O2A Mods are acquired during the Expedition and reset when the
Expedition ends.

Persistent Weapon Mod availability/unlocking remains separate O2 work.

Everything here resets when expedition ends.

Temporary Weapon Mod levels are now implemented.

Current Weapon Mastery lifetime:

Character begins before first Core Mastery
→ original Basic Attack.

Persistent Mastery unlock
→ Core Mastery becomes the Character's permanent default Basic Attack identity.

Persistent Mastery development
→ improves the Core Mastery's prepared baseline.

Expedition RunBuild
→ snapshots that persistent baseline;
→ owns temporary additional levels / future augments.

Runtime Actor
→ uses Core Mastery by default;
→ may later switch to an unlocked alternate Mastery.

Alternative Mastery deactivation
→ returns to Core Mastery, never to the obsolete pre-Mastery Basic Attack.

Switching stances does not alter temporary Weapon Mod progression.

Temporary Weapon Mod level resets when the Expedition ends.

Persistent Weapon Mastery progression has two implemented forms:

Stance unlock
→ permanently introduces a Core/alternate Mastery into the prepared build.

Mastery level development
→ permanently increases the stance's starting level.

The current Ranger proof is:

Piercing Mastery
↓
Deep Penetration
↓
Piercing begins future Expeditions one effective level higher.

Temporary Weapon Mod reward levels then stack on top of this persistent
baseline for the current Expedition only.

# Synergies and Tags
This could become hugely important later.

Seed tags:
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

Possible future uses:
- reward weighting
- compatibility
- build analysis
- UI
- procedural recommendation.

# Build Examples
Start putting hypothetical builds here.

Example:
```
# Bleed Explosion Whirlwind

## Starting Intent

Begin with reliable Bleed.

## Desired Finds

- Whirlwind
- Whirlwind applies Bleed
- Bleeding enemies explode
- Explosion reduces cooldown

## Possible Ecological Pivot

Heatwave occurs.

Player receives Fire options and may convert part of the build toward Burning explosions.

## Why This Build Is Interesting

The finished result cannot be fully guaranteed before the run.
```

This page will be incredibly valuable.