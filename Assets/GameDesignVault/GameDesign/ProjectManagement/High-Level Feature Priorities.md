# High-Level Feature Priorities

**Status:** Working Planning List

This document captures the major gameplay, progression, content, and UX areas that still need meaningful design, implementation, reassessment, or expansion.

The purpose is to keep the project focused on the major systems that define the game holistically without getting lost in lower-level implementation details.

This list is intentionally not ordered by priority yet.

We should prioritise and schedule these areas based on:

- expected value to the overall game;
- how much they improve our ability to evaluate the game;
- dependencies between systems;
- how strongly they contribute to the intended player experience;
- current development momentum;
- what feels most useful to tackle next.

We are **not** committing to implementing only the smallest possible version of each system. Scope should remain flexible where a broader implementation creates substantially more value or gives us a better foundation to evaluate.

---

## 1. Player Retention & Core Loop

Understand why the player wants to start another expedition after finishing the previous one.

The game needs a compelling repeatable loop even before relying heavily on narrative or large amounts of persistent progression.

Areas to explore:

- What makes the core combat/buildcraft loop intrinsically enjoyable?
- What makes one expedition feel different from another?
- What creates the feeling of "one more run"?
- How frequently should meaningful choices occur?
- What provides short-term goals during an expedition?
- What provides medium-term goals across several expeditions?
- What provides long-term reasons to continue playing?
- How much retention comes from build experimentation?
- How much comes from unlocking Characters, Weapon Mods, Abilities, content, or progression?
- How much comes from mastery/challenge rather than numerical progression?
- What makes a failed expedition still feel worthwhile?
- What makes a successful expedition create excitement for the next one?

This should eventually become the clearest description of the game's complete player loop.

---

## 2. Meta Progression & Resources

Finalise the persistent progression layer that exists outside individual expeditions.

This system should provide meaningful long-term development without making expedition buildcraft irrelevant.

Areas to design:

- Persistent currencies/resources.
- Character progression/mastery.
- Character unlocks.
- Weapon Mod unlocks.
- Ability unlocks.
- Cross-character unlocks.
- Persistent objectives/challenges.
- Resource sources.
- Resource sinks.
- Long-term progression pacing.
- Whether different resources have different purposes.
- Whether monster/boss/region-specific resources remain useful.
- What replaces the existing prototype Weapon / Armour / Focus progression.
- Whether any form of crafting remains useful.
- How much persistent numerical power is appropriate.
- How much progression should unlock new options rather than raw power.

---

## 3. UI Structure & Finalisation

Design the complete functional UI structure of the game before committing to a final visual skin.

The goal is to implement the skeleton of every important player-facing screen and interaction so the game's full information architecture can be tested.

Areas to cover:

- Main/preparation screen.
- Character selection.
- Character roster.
- Locked Character presentation.
- Character progression / mastery tree.
- Character stats.
- Weapon Mod preparation.
- Ability preparation.
- Expedition HUD.
- Core Ability HUD.
- Generic/acquired Ability presentation.
- Weapon Mod controls.
- Targeting controls.
- Health/status information.
- Encounter selection.
- Forecast/schedule.
- Strategic map/planner.
- Objectives.
- Rewards.
- Persistent resources.
- Expedition results.
- Character unlock presentation.
- Meta-progression notifications.
- Boss/major encounter presentation.
- Controller navigation and interaction flow.

Final UI art/skin should remain separable from the functional UI structure.

---

## 4. Character Stats & Expedition Buildcraft

Create a stronger and more intentional stat model for playable Characters and use it to expand expedition buildcraft.

Character stats should provide meaningful levers for:

- temporary rewards;
- Weapon Mods;
- Abilities;
- persistent progression;
- enemy effects;
- World Events;
- future Augments.

Potential stats to review include:

### Survival

- Max Health.
- Armour.
- Knockback Resistance.
- Healing / Sustain effectiveness if required.

### Mobility

- Movement Speed.

### Basic Attack / Weapon

- Attack Damage.
- Attack Speed.
- Attack Range.
- Knockback Power.

### Ability

- Ability Cooldown Recovery.
- Ability Power / Effect Power if justified.

Additional stats should only be introduced where they create meaningful gameplay interactions.

The buildcraft system should eventually allow the same Character to develop into significantly different expedition builds.

---

## 5. Expedition Objectives

Create meaningful objectives that give the player short-, medium-, and expedition-scale goals.

Objectives should help provide direction beyond simply travelling to the next Encounter.

Potential objective types:

- Defeat a particular enemy.
- Defeat an Elite.
- Defeat a Boss.
- Complete a particular Encounter.
- Complete several Encounters.
- Survive until a specific World Time.
- Survive a World Event.
- Reach a location.
- Discover something.
- Defeat enemies using a particular mechanic.
- Use a Character/Ability/Weapon Mod in a particular way.
- Complete an expedition restriction/challenge.
- Collect a resource.
- Protect or interact with a world element.

Objectives may reward:

- expedition rewards;
- persistent resources;
- Character unlock progress;
- Weapon Mod unlock progress;
- Ability unlock progress;
- mastery progress;
- special Encounter access.

We need to decide how objectives are generated, authored, tracked, presented, and rewarded.

---

## 6. Encounter Variety

Expand the number and type of Encounters available to the player.

The goal is not merely more spawn configurations, but situations that ask the player to respond differently.

Areas to explore:

- Different enemy compositions.
- Different Encounter phases.
- Reinforcements.
- Timed pressure.
- Survival.
- Elite-focused Encounters.
- Boss Encounters.
- Rare opportunity Encounters.
- Defend/protect objectives.
- Movement/location objectives.
- Resource opportunities.
- Environmental pressure.
- Encounter-specific World Events.
- Multi-stage Encounters.
- Different reward identities.

Encounter variety should make selecting one Encounter over another a meaningful strategic decision.

---

## 7. Ambient Enemies & World Population

Develop the ambient enemy system into a living population rather than a one-time spawn.

Current behaviour is largely:

`spawn population → population eventually dies → area remains empty`

This needs a longer-term population model.

Areas to design:

- Population capacity.
- Initial population.
- Repopulation rate.
- Respawn timing.
- Spawn frequency.
- Population pressure.
- Randomisation.
- Route/Area-specific populations.
- Escalation effects.
- World Time effects.
- World Event effects.
- Different enemy families.
- Rare ambient enemies.
- Migration between areas.
- Whether defeated areas temporarily become safer.
- Whether populations replenish continuously or in pulses.
- How far populations wander.
- What happens after enemies engage and survive combat.

The result should make the region feel populated and dynamic rather than pre-seeded once.

---

## 8. Weapon Mods

**Previously referred to as Stances.**

Weapon Mods are a major character customisation and cross-character progression system.

Each playable Character begins with Weapon Mods appropriate to their combat identity and can unlock additional Mods through progression.

Deep progression may eventually unlock that Character's Mods for use by other Characters.

Weapon Mods should intentionally remain generic enough to eventually work across different Characters.

### Example Mod families

#### Physical / Melee

- Slashing.
- Knockback.
- Stun.
- Bleed.
- Heavy impact.
- Armour breaking.

#### Magical / Elemental

- Fire.
- Lightning.
- Poison.
- Blood Magic.
- Ice or other future elemental identities.

#### Ranged / Projectile

- Piercing.
- Projectile Speed.
- Spread.
- Multi-shot.
- Ricochet.
- Range effects.

Weapon Mods may alter:

- damage type;
- attack behaviour;
- basic attack properties;
- Character stats;
- special effects;
- Ability interactions;
- status effects;
- knockback;
- projectile behaviour.

Key design questions still to resolve:

- How many Mods can be equipped/prepared?
- Are Mods always active or activated during combat?
- Can multiple Mods operate simultaneously?
- How strongly should a Mod transform the Character?
- How does Character compatibility work?
- How does cross-character unlocking work?
- How do Mods interact with expedition rewards?
- How does the Character mastery tree unlock them?

---

## 9. Abilities

Expand Ability variety and reassess how Abilities fit into the changing game.

The existing concept of three Character core Abilities remains useful, but acquisition and buildcraft need further evaluation.

Areas to design:

- Character-native Abilities.
- Signature / Unique Ability.
- Utility Ability.
- Sustain Ability.
- Generic Ability pool.
- Ability compatibility.
- Ability unlocks.
- Ability preparation.
- Ability acquisition during expeditions.
- Ability levels.
- Ability Evolutions.
- Ability Augments/Mutations.
- Charges.
- Cooldowns.
- Resources where appropriate.
- Active targeting/casting.
- Animation/VFX integration.

Key question:

> Should generic Abilities still primarily be acquired during expedition buildcraft, or should preparation/unlocking play a larger role?

This should be answered through gameplay rather than assumed.

---

## 10. Combat Targeting

Give the player more control over how autonomous Characters choose enemies.

The system should preserve automatic combat while allowing deliberate tactical priorities.

Potential targeting options:

- Closest.
- Weakest.
- Strongest.
- Highest Damage.
- Lowest Health.
- Prefer Elite.
- Prefer Boss.
- Prefer Rare / Valuable enemy.
- Prefer Ranged enemy.
- Prefer Support enemy.

Areas to design:

- Whether targeting uses one primary rule plus modifiers.
- Whether priorities can be changed during combat.
- How targeting is presented in the HUD.
- How targeting interacts with strategic Encounter direction.
- How quickly Characters switch targets.
- Whether player intervention can temporarily force a target.
- How future Companions use targeting/tactics.

---

## 11. Characters & Unlocks

Expand the playable Character roster and make unlocking Characters a meaningful part of persistent progression.

Each Character should provide a distinct package of:

- visual identity;
- weapon/basic attack identity;
- stats;
- combo behaviour;
- core Abilities;
- Weapon Mods;
- mastery/progression tree;
- compatible generic Abilities.

Areas to design:

- Character roster.
- Starting Character(s).
- Character unlock conditions.
- Character unlock objectives.
- Hidden/secret Characters.
- Unlock progress.
- Unlock presentation.
- Character selection UI.
- Character preview/stats.
- Character progression.
- Cross-character progression.
- Character-specific mastery.
- Future possibility of unlocked Characters appearing as AI Companions.

Near-term expeditions should use **one active playable Character**.

The underlying allied-actor architecture should remain flexible enough to support future Companions.

---

## 12. Expedition Pacing, Escalation & Endings

Define the overall dramatic and gameplay shape of an expedition.

The expedition should evolve rather than feel like the same level of pressure indefinitely.

Areas to design:

- Expedition length.
- Early-game pace.
- Mid-run development.
- Late-run pressure.
- Escalation.
- Encounter difficulty progression.
- Enemy population pressure.
- Elite frequency.
- Boss timing.
- World Event severity.
- Quiet periods versus high-pressure periods.
- Difficulty peaks and valleys.
- Apex/endgame encounters.
- Expedition success condition.
- Voluntary extraction/return.
- Continuing after success.
- Risk/reward for staying longer.
- Post-success endurance.
- Failure consequences.
- Reward banking.

The expedition should have a recognisable:

`beginning → development → escalation → climax → decision/end`

without requiring every expedition to play identically.

---

## 13. Enemy Roster & Combat Roles

Expand the actual enemy vocabulary used to construct Encounters and ambient populations.

Encounter variety cannot come only from rearranging the same enemy.

Enemy roles may include:

- Swarm.
- Basic pressure.
- Tank.
- Fast attacker.
- Ranged attacker.
- Support.
- Controller.
- High-damage threat.
- Summoner.
- Elite.
- Boss.
- Rare/high-value target.
- Environmental/special enemy.

Areas to design:

- Enemy families.
- Combat roles.
- Attack patterns.
- Targeting pressure.
- Status effects.
- Movement behaviour.
- Enemy synergies.
- Elite variants.
- Boss mechanics.
- Rare/value enemies.
- Family-specific rewards/resources.
- Ecology/Region identity.
- Interaction with Combat Targeting.
- Interaction with Weapon Mods and Abilities.

---

## 14. Rewards & Progression Flow

Define what different activities actually reward and how those rewards feed the rest of the game.

This is the connective layer between Encounters, Objectives, Characters, buildcraft, and Meta Progression.

Examples to define:

- Ambient enemies → ?
- Rare ambient enemies → ?
- Normal Encounters → ?
- Elite Encounters → ?
- Bosses → ?
- World Events → ?
- Objectives → ?
- Expedition completion → ?
- Long endurance → ?
- Character mastery milestones → ?

Rewards may include:

### Expedition Rewards

- stat upgrades;
- Ability acquisition;
- Ability levels;
- Evolutions;
- Weapon Mod interactions;
- temporary Augments;
- build pivots.

### Persistent Rewards

- resources;
- Character unlock progress;
- Weapon Mod unlocks;
- Ability unlocks;
- mastery progress;
- global unlocks;
- cosmetics;
- future crafting/material uses.

Areas to design:

- Reward rarity.
- Reward quality.
- Build-aware rewards.
- Compatibility filtering.
- Synergy weighting.
- Dead-choice prevention.
- Meaningful reward-source identity.
- Deferred/pending reward behaviour.
- Persistent versus expedition reward clarity.

---

## 15. Strategic World Layer Review

Reassess the strategic systems that have already been prototyped once the rest of the gameplay is richer.

Existing foundations include:

- Route geography.
- Encounter Sites.
- directed travel.
- quick Encounter choices.
- Encounter supply.
- World Time.
- forecasting.
- World Events.
- Ambient Activity Areas.
- Region map/minimap concepts.

Questions to revisit:

- How much strategic choice should the player have?
- How frequently should the player choose an Encounter?
- How much information should forecasts reveal?
- How important should route choice be?
- Should travel itself contain risk?
- How much should World Events affect planning?
- How much strategic information belongs in the quick HUD?
- What requires an expanded map/planner?
- How much of the current prototype should survive unchanged?
- Which systems are genuinely fun versus merely technically interesting?

This area should be reviewed rather than automatically expanded.

---

# High-Level Game Questions

The feature list above can be grouped into four major questions.

## Why do I want to play another expedition?

- Player Retention & Core Loop.
- Meta Progression & Resources.
- Expedition Objectives.
- Characters & Unlocks.
- Rewards & Progression Flow.

## Why does this expedition feel different?

- Character Stats & Expedition Buildcraft.
- Weapon Mods.
- Abilities.
- Expedition Pacing, Escalation & Endings.

## What am I actually fighting and deciding?

- Encounter Variety.
- Ambient Enemies & World Population.
- Combat Targeting.
- Enemy Roster & Combat Roles.
- Strategic World Layer.

## Can I understand and control all of this?

- UI Structure & Finalisation.

---

# Planning Principle

This list describes the major areas that currently define the intended game.

It is **not** a commitment to implementing the smallest possible subset of each feature.

Work should instead be prioritised according to what currently appears to provide the most value to the project.

A feature may warrant a larger implementation when doing so:

- gives significantly better gameplay evidence;
- creates a stronger reusable foundation;
- resolves an important design question;
- unlocks several other systems;
- makes the game substantially easier to evaluate;
- improves the overall cohesion of the game.

Likewise, features should remain open to reassessment when gameplay evidence suggests the current direction is not working.

The roadmap exists to guide development, not to prevent the project from changing.