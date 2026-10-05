# Weapon Mods

**Status: Implemented Core Mastery Foundation — 3.0O2 Active**

# Purpose

Weapon Mods are selectable combat stances that change how a Character's
autonomous Basic Attacks behave.

They do not replace the Character's core weapon identity.

Instead:

Character Basic Attack
+
selected Weapon Mod stance
+
persistent Mastery development
+
Expedition Mod progression
=
current Basic Attack behaviour.

Weapon Mods are intended to create tactical intervention without turning Basic
Attacks into manually-triggered attacks.

The Character continues to fight autonomously.

The player decides how those autonomous attacks should currently behave.

# Stance Model

**Status: Confirmed**

A developed Character ultimately has four Weapon Mastery stance positions.

These are:

Core Mastery
+
Alternate Mastery 1
+
Alternate Mastery 2
+
Alternate Mastery 3.

The original Basic Attack is not a fifth stance.

## Pre-Mastery State

A newly-owned Character may initially use their original Basic Attack.

This is the Character's temporary starting combat behaviour while the player is
earning access to their first Weapon Mastery.

Conceptually:

New Character
↓
Basic Attack
↓
earn progression
↓
unlock Core Mastery.

## Core Mastery

The first Weapon Mastery stance is the Character's Core Mastery.

Once unlocked:

- it permanently replaces the original Basic Attack as the Character's normal
  attack identity;
- it automatically enters every future Expedition;
- it is automatically selected as the Character's default stance;
- the player cannot deactivate it back to the original Basic Attack.

The Core Mastery should become a major expression of Character identity and a
major destination for long-term Character Mastery progression.

Example:

Ranger
↓
original targeted arrow
↓
unlock Piercing Mastery
↓
Piercing becomes Ranger's new default Basic Attack identity.

## First Alternate Mastery Proof

Ranger now demonstrates a real Core / Alternate Mastery relationship.

Core:

Piercing Arrows
→ multi-target penetration;
→ Ranger's normal post-Mastery combat identity.

Alternate:

Impact Arrows
→ reliable single-target projectile;
→ strong knockback / spacing control.

Runtime relationship:

Piercing Core
↓ activate Impact
Impact Alternate
↓ deactivate Impact
Piercing Core.

The original pre-Mastery Basic Attack is never re-entered.

Impact Arrows currently occupies `Alternate1`.

Slots 2 and 3 remain unused for future Masteries.

## Permanent Alternate-Mastery Development

Ranger's existing Arrow Impact node now follows the Impact Mastery unlock.

Current flow:

Impact Arrows Mastery
↓
Arrow Impact
↓
+20% persistent outgoing Knockback Power.

Because Impact Arrows uses the shared `KnockbackResolver`, this persistent stat
naturally improves the stance without a stance-specific permanent-stat system.

Temporary Impact levels separately increase the stance's authored base knockback
distance for the current Expedition.

## Alternate Masteries

The remaining three positions are alternative Weapon Masteries.

They are not necessarily stronger than the Core Mastery.

Instead they provide different tactical answers.

Example:

Ranger Core Mastery
→ Piercing.

Alternate Mastery
→ Knockback.

Activating Knockback temporarily replaces Piercing as the active Basic Attack
stance.

Deactivating Knockback returns Ranger to Piercing.

It does not return Ranger to the original pre-Mastery arrow.

## Four-Position Model

Final stance vocabulary:

Slot 1
→ Core Mastery / permanent default.

Slot 2
→ Alternate Mastery.

Slot 3
→ Alternate Mastery.

Slot 4
→ Alternate Mastery.

Only one stance controls Basic Attacks at a time.

Before Core Mastery is unlocked, the Character uses their original Basic Attack
as a temporary fallback outside this four-Mastery model.

# Player Intervention

Abilities answer:

> What tactical action should I perform now?

Weapon Mod stances answer:

> How should my autonomous Basic Attacks behave now?

The player should not need to manually activate every Basic Attack.

Changing stance should alter subsequent autonomous attacks until the stance is
changed or disabled.

# Persistent Character Progression

Weapon Mastery is a major persistent Character-development system.

The Core Mastery should receive substantial long-term development over many
Expeditions.

Persistent progression may:

- unlock the Core Mastery;
- unlock alternative Masteries;
- increase a Mastery's persistent starting level;
- improve specific Mastery behaviours;
- remove or reduce weaknesses;
- add new effects;
- unlock interactions between Masteries;
- eventually allow the Core Mastery to combine with alternative Masteries.

Persistent improvements are not lost when an Expedition ends.

Example:

Ranger unlocks Piercing Mastery
↓
Piercing becomes Ranger's permanent default stance.

Later:

Deep Penetration
↓
Piercing permanently begins at a higher Mastery level.

Later still:

Advanced Piercing Mastery
↓
may unlock interaction with another stance such as Knockback.

Temporary Expedition rewards sit on top of this persistent foundation.

# Expedition Weapon Mod State

Each prepared Weapon Mod receives Expedition-owned state.

Conceptually:

Fire
├── Definition
├── temporary Level
├── future Augments
└── future Evolutions / combinations.

Lightning
├── Definition
├── temporary Level
└── future development.

Switching the active stance does not remove another Mod's Expedition
progression.

Example:

Fire Lv.2
Lightning Lv.1
Piercing Lv.0

Active
→ Lightning.

Switching to Fire later restores the already-developed Fire Lv.2 state.

All temporary Weapon Mod progression resets when the Expedition ends.

# Weapon Mod Rewards

Run Rewards do not normally unlock the base stance.

The persistent/prepared layer determines which Weapon Mod stances the Character
brings into the Expedition.

Weapon Mod Run Rewards instead develop those prepared stances.

Potential reward behaviour includes:

- increase Mod level;
- add Mod-specific augments;
- evolve a Mod;
- combine compatible Mod identities;
- create designed interactions between Mod types.

Example:

Fire stance permanently unlocked
+
Fire Lv.1 Run Reward
=
Fire stance becomes more powerful for the current Expedition.

The player may still switch to Lightning while retaining Fire's current Run
progression.

# Combination Direction

Weapon Mod combinations are an intended future buildcraft direction.

Examples may include:

Fire
+
Piercing
→ burning penetrating attacks.

Fire
+
Lightning
→ a designed elemental interaction.

These should be authored as meaningful gameplay interactions.

Do not turn Weapon Mods into a universal generic effect graph merely to support
hypothetical combinations.

Build concrete combination infrastructure when representative content requires
it.

# Compatibility

A Weapon Mod may be Character-native while still being compatible with other
Characters.

Compatibility and unlock source are separate concepts.

For example:

Character A Mastery
→ unlocks a Mod or progression opportunity.

Character B
→ may use it later if the Mod is compatible with Character B and the relevant
  progression rules allow it.

Initial compatibility remains authored through explicit playable Character
references.

An empty compatibility list means broadly compatible.

Do not introduce a larger tag/query compatibility framework until actual Mod
content requires it.

Current Character-native examples:

Punchy
→ Heavy Finisher.

Ranger
→ Piercing Arrows.

Both use the same Weapon Mod runtime, HUD and Run Reward infrastructure despite
modifying different parts of Basic Attack behaviour.

# Current Implemented Proof

Weapon Mods now have their first complete player-facing tactical and Expedition
progression loop.

Current Punchy flow:

Current Punchy flow:

Persistent Mastery
↓
Heavy Finisher Core Mastery
↓
new Expedition automatically begins with Heavy Finisher active.

Before Core Mastery ownership:

Punchy
→ original Basic Attack combo.

After Core Mastery ownership:

Punchy
→ Heavy Finisher becomes the permanent default Mastery;
→ the original pre-Mastery Basic Attack is no longer selectable.

Temporary progression remains layered on top of the persistent Core Mastery:

persistent prepared level
+
temporary Expedition level bonus
=`
effective Mastery level.

Temporary progression proof:

Heavy Finisher Lv.0
↓
Weapon Mod Run Reward
↓
Heavy Finisher Lv.1.

Changing active stance does not remove or reset the Mod's Expedition level.

The current first level tuning is deliberately exaggerated:

Lv.0
→ +100% final-combo-step damage.

Lv.1
→ +150% final-combo-step damage.

This is validation tuning rather than final balance.

## Ranged / Projectile Proof

Piercing Arrows is Ranger's first Core Weapon Mastery.

Persistent flow:

Piercing Mastery
↓
Piercing replaces Ranger's original Basic Attack
↓
future Ranger Expeditions automatically begin in Piercing Mastery.

Current Piercing delivery:

targeted arrow
↓
tracks and reliably strikes the selected primary target
↓
continues forward after impact
↓
may penetrate additional aligned enemies.

Current level progression increases maximum targets hit.

Piercing therefore evolves Ranger's original attack rather than requiring the
player to trade back to a less reliable Basic Attack.

Ranger also demonstrates persistent Mastery development:

Piercing Mastery
↓
Deep Penetration
↓
higher prepared Piercing level in every future Expedition.

Temporary Run Reward levels stack on top of that prepared baseline.

## Core Mastery Proof

Weapon Mastery now has an explicit four-position model.

Slot 1
→ Core Mastery.

Slots 2–4
→ alternate Masteries.

Before Core Mastery ownership, the original Basic Attack remains a temporary
fallback.

After Core Mastery ownership:

- Core Mastery automatically enters every Expedition;
- Core Mastery is automatically active;
- the original Basic Attack is no longer selectable;
- deactivating a future alternate Mastery returns to Core Mastery.

Current prototype Core Masteries:

Punchy
→ Heavy Finisher.

Ranger
→ Piercing Arrows.

Ranger now also proves persistent Core-Mastery development:

Piercing Mastery
↓
Deep Penetration
↓
persistent starting Piercing level +1.

Temporary Run Reward levels stack on top of the persistent starting level and
reset back to that persistent baseline at Expedition end.

# Runtime / Progression Lifetimes

Persistent Character Mastery
→ determines which Weapon Mod stances enter the prepared build.

RunBuild
→ owns the Weapon Mod states available during the current Expedition;
→ will later own temporary Mod levels / augments / evolutions.

Runtime Actor
→ owns which available Weapon Mod stance is currently selected.

Current selected stance is tactical runtime state.

For the first proof, a newly reconstructed Actor may begin in Default even if a
stance was active on the previous Actor.

Whether active stance selection should later survive Region/Actor
reconstruction should be decided from gameplay experience rather than assumed
now.

Current selected stance is Actor-runtime state.

A runtime refresh of the same Actor preserves the selected stance when that Mod
remains available.

A newly reconstructed Actor currently begins in Default.

Cross-Actor / cross-Region stance persistence remains unresolved pending
gameplay evidence.

# Deliberately Unresolved

Still unresolved:

- final polished visual layout of the currently-functional four stance controls
- whether stance switching has any cooldown or lockout;
- exact Mod-level limits;
- exact reward level curves;
- final permanent Mod-empowerment vocabulary;
- exact rules for cross-Character unlock sharing;
- final Mod-combination vocabulary;
- whether active stance should survive Region reconstruction;
- final VFX/audio treatment for stance changes.

These should be resolved from representative gameplay rather than speculative
infrastructure.