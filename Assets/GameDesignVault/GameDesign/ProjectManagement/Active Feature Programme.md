# MiniCrawler Active Feature Programme

**Status: Canonical Planning**
**Rebaselined: 2026-09-22**

This document defines the current high-level gameplay feature programme for
MiniCrawler.

It answers:

> What major parts of the game are we actively working toward?

Detailed game-design intent belongs in the relevant documents under `Design/`.

Implementation scheduling and milestone history live in [[Roadmap]].

# Planning Principle

All feature areas in this document are part of the current active programme.

They are not divided into Now / Soon / Later categories.

Their order represents:

- useful dependencies;
- which decisions give later systems better context;
- which work is likely to provide the most value to the game;
- which systems improve our ability to evaluate later systems.

The project should not optimise for the smallest possible implementation merely
for the sake of reducing scope.

Scope should instead favour coherent, high-value work that:

- resolves important design questions;
- creates useful gameplay evidence;
- unlocks dependent systems;
- establishes a strong reusable foundation;
- materially improves the ability to evaluate the game.

Work should remain reviewable and avoid unrelated speculative expansion.

The roadmap is a current best development order, not a permanent design lock.

Later milestones may be reshaped when playtesting or implementation provides
better evidence.

# Active Feature Order

## 1. Player Retention & Core Loop

Understand why the player wants to begin another expedition.

Review:

- intrinsic enjoyment of combat and buildcraft;
- reasons one expedition differs from another;
- short-term expedition goals;
- medium-term goals across several expeditions;
- long-term motivations;
- progression versus mastery/challenge;
- how failure remains worthwhile;
- how success generates interest in another run.

This becomes the evaluation lens for the rest of the programme.

**Depends on:** Current playable foundations.

**Feeds:** Every later feature.

### Current N1 Finding

The current development frontier is the 1–3 minute intention horizon.

Expedition Objectives are the preferred first mechanism for strengthening that
horizon.

A small Objective Foundation should therefore be implemented during N1 rather
than waiting for the later full Objective-content programme.

The later Expedition Objectives feature remains responsible for breadth,
additional condition types, integration with richer content and large-scale
authoring.

---

## 2. Character Stats & Expedition Buildcraft

Create a deliberate Character stat vocabulary and stronger temporary buildcraft
foundation.

Review areas such as:

- Max Health;
- Armour;
- Movement Speed;
- Attack Damage;
- Attack Speed;
- Attack Range;
- Knockback Power;
- Knockback Resistance;
- Ability Cooldown Recovery;
- other stats only where concrete mechanics justify them.

Stats should provide useful levers for:

- expedition rewards;
- Weapon Mods;
- Abilities;
- Meta Progression;
- Objectives;
- World Events;
- future Augments.

Attack-specific properties should remain attack-specific where they are not
genuinely general Character stats.

**Depends on:** Retention / core-loop direction.

**Feeds:** Rewards, Meta Progression, Characters, Weapon Mods and Abilities.

---

## 3. Rewards & Progression Flow

Define what playing the game actually gives the player.

Review reward identity for:

- Ambient enemies;
- rare Ambient enemies;
- normal Encounters;
- Elites;
- Bosses;
- World Events;
- Objectives;
- expedition success;
- long-duration survival;
- Character progression milestones.

Separate:

- temporary Expedition rewards;
- persistent/meta rewards.

Review:

- reward rarity;
- build-aware rewards;
- compatibility;
- synergy weighting;
- dead-choice prevention;
- reward-source identity;
- persistent versus temporary clarity.

**Depends on:** Core-loop direction and useful stat/buildcraft vocabulary.

**Feeds:** Meta Progression, Objectives, Encounters and Character Unlocks.

---

## 4. Meta Progression & Resources

Finalise the persistent progression layer outside expeditions.

Review:

- persistent resources/currencies;
- Character progression;
- Character Mastery;
- Character unlocks;
- Weapon Mod unlocks;
- Ability unlocks;
- cross-character progression;
- progression pacing;
- resource sources;
- resource sinks;
- numerical power versus new options;
- role of monster/boss/region resources;
- whether any useful form of crafting remains.

The current Weapon / Armour / Focus prototype progression is not assumed to be
the final system.

**Depends on:** Reward flow and Character/buildcraft vocabulary.

**Feeds:** Characters, Weapon Mods, Abilities and Objectives.

---

## 5. Characters & Unlocks

Expand playable Characters and make Character ownership a meaningful persistent
goal.

Near-term expeditions use one active playable Character.

A Character should have a distinct combination of:

- visual identity;
- basic attack / weapon identity;
- stats;
- combo behaviour;
- core Abilities;
- native Weapon Mods;
- persistent progression;
- compatibility with shared content.

Review:

- roster;
- starting Characters;
- unlock conditions;
- unlock progress;
- hidden Characters;
- roster presentation;
- Character selection;
- cross-character progression.

Existing multi-allied-actor architecture should not be destructively removed;
future autonomous Companions may reuse it.

**Depends on:** Stats and Meta Progression.

**Feeds:** Weapon Mods, Abilities, Objectives and UI.

---

## 6. Weapon Mods

**Current development frontier**

**Foundation implemented — further content expansion deferred**

Weapon Mods have evolved into the Character Weapon Mastery system.

The established foundation includes:

- four total stance positions;
- one Core Mastery;
- three possible Alternate Masteries;
- Core replacing the pre-Mastery Basic Attack as the Character's permanent
  default identity;
- one active stance at a time;
- Alternate deactivation returning to Core;
- persistent stance unlock/development;
- temporary Expedition stance development;
- independent stance levels;
- pairwise persistent Mastery synthesis;
- representative Piercing, Knockback and Fire behaviours;
- spatial persistent Mastery progression;
- dedicated Character Mastery definitions;
- designer-facing Character Mastery Workbench;
- scalable Reward Catalogue integration;
- composable projectile delivery/impact architecture.

Representative Ranger gameplay currently proves:

Piercing Core;

Impact Alternate;

Fire Alternate;

Impact ← Piercing synthesis;

Fire ← Piercing synthesis.

The system has enough gameplay and architecture evidence to pause further
Weapon Mastery expansion while Abilities are reassessed.

Remaining Weapon Mastery work is primarily:

- additional content;
- final balance;
- final VFX/presentation;
- future combination breadth where gameplay demonstrates value.

Avoid reopening core Weapon Mastery architecture without new gameplay evidence.

**Feeds:** Buildcraft, Rewards, Abilities and combat interaction design.

## 7. Abilities

Expand Ability variety and reassess acquisition around the changing game.

Preserve the useful Character core:

- Signature / Unique;
- Utility;
- Sustain.

Review:

- Character-native Abilities;
- generic Ability pool;
- compatibility;
- preparation;
- expedition acquisition;
- Ability levels;
- Evolutions;
- Augments / mutations;
- charges;
- cooldowns;
- targeting;
- animation and VFX integration.

Do not assume that the current run-acquisition model must remain unchanged.

Use gameplay evidence to determine the final relationship between prepared,
native and expedition-acquired Abilities.

**Depends on:** Characters, Stats, Reward Flow and Meta Progression.

**Closely related to:** Weapon Mods.

---

## 8. Enemy Roster & Combat Roles

Expand the enemies used by Encounters and Ambient populations.

Develop meaningful combat roles such as:

- swarm;
- basic pressure;
- tank;
- fast attacker;
- ranged attacker;
- support;
- control;
- high-damage threat;
- summoner;
- Elite;
- Boss;
- rare/high-value target.

Review:

- enemy families;
- movement;
- attack behaviour;
- synergy;
- status interactions;
- rewards;
- ecology identity;
- Region identity;
- interaction with player buildcraft.

**Depends on:** Enough player combat vocabulary to know what enemies should
challenge.

**Feeds:** Targeting, Objectives, Encounter Variety and Ambient Population.

---

## 9. Combat Targeting

Give the player tactical influence over autonomous target selection.

Potential priorities include:

- Closest;
- Weakest;
- Strongest;
- Highest Damage;
- Lowest Health;
- Prefer Elite;
- Prefer Boss;
- Prefer Rare / Valuable;
- Prefer Ranged;
- Prefer Support.

Review:

- primary rule plus modifiers;
- changing priorities during combat;
- temporary forced targeting;
- HUD presentation;
- relationship with Encounter direction;
- future reuse by Companions.

**Depends on:** Meaningful Enemy roles.

**Feeds:** Combat decision-making and UI.

---

## 10. Expedition Objective Expansion

Create meaningful tracked goals within and across expeditions.

Potential objectives include:

- defeat a specific enemy;
- defeat an Elite;
- defeat a Boss;
- complete Encounters;
- survive until a World Time;
- survive a World Event;
- reach/discover a location;
- use a mechanic in a particular way;
- complete a restriction/challenge;
- collect something;
- protect/interact with world content.

Objectives may provide:

- Expedition rewards;
- persistent resources;
- Character unlock progress;
- Weapon Mod unlock progress;
- Ability unlock progress;
- mastery progress;
- special content access.

Review authored versus generated Objectives and how they are presented/tracked.

**Depends on:** Rewards, Meta Progression, Characters and Enemy vocabulary.

**Closely related to:** Encounter Variety.

A foundational tracked Objective system is pulled forward into 3.0N1.

This later feature expands it using the richer Character, Enemy, Reward and
Encounter systems created by the intervening programme.

---

## 11. Encounter Variety

Expand Encounters into meaningfully different situations.

Explore:

- different enemy compositions;
- phases;
- reinforcements;
- survival pressure;
- Elite encounters;
- Boss encounters;
- rare opportunity encounters;
- objective encounters;
- location/movement challenges;
- resource opportunities;
- environmental pressure;
- multi-stage encounters;
- distinct reward identities.

Encounter choice should eventually create a meaningful strategic decision.

**Depends on:** Enemy Roster, Rewards and Objectives.

**Feeds:** Pacing and Strategic World decisions.

---

## 12. Ambient Enemies & World Population

Move Ambient Activity from one-time population spawning toward a living world
population model.

Review:

- population capacity;
- initial population;
- repopulation;
- refresh cadence;
- spawn frequency;
- population pressure;
- Route/Area identity;
- Escalation effects;
- World Time effects;
- World Event effects;
- enemy families;
- rare Ambient enemies;
- migration;
- temporary local safety after clearing an area;
- post-engagement behaviour.

**Depends on:** Enemy Roster.

**Feeds:** Expedition Pacing and Strategic World behaviour.

---

## 13. Expedition Pacing, Escalation & Endings

Define the overall shape of an expedition.

Review:

- expected expedition duration;
- opening pace;
- mid-run development;
- late-run pressure;
- Escalation;
- Elite frequency;
- Boss/Apex timing;
- quiet periods and peaks;
- World Event pressure;
- success conditions;
- voluntary return;
- reward banking;
- continuing after success;
- risk/reward for staying;
- endurance;
- failure consequences.

The intended broad shape is:

Beginning
→ Development
→ Escalation
→ Climax
→ Decision / End

without requiring every expedition to follow an identical script.

**Depends on:** Encounters, Ambient Population, Objectives and Rewards.

**Feeds:** Retention evaluation and Strategic World review.

---

## 14. Strategic World Layer Review

Reassess the strategic systems already implemented once the surrounding game is
richer.

Existing foundations include:

- Route Geography;
- Encounter Sites;
- directed travel;
- quick Encounter choices;
- Encounter supply;
- World Time;
- forecast/schedule;
- World Events;
- Ambient Activity;
- minimap/planner concepts.

Review:

- how much strategic choice is actually useful;
- Encounter-choice frequency;
- forecast information;
- route risk;
- travel risk;
- World Event influence;
- quick HUD versus expanded planner responsibilities;
- which prototypes are genuinely fun;
- which systems should be simplified;
- which systems deserve expansion.

This is intentionally a review rather than an assumption that every prototype
must grow.

**Depends on:** Richer expedition gameplay.

---

## 15. UI Structure & Finalisation

Complete the functional UI structure of the game while keeping final UI skin
separate.

UI work begins throughout the programme rather than waiting until this final
item.

Each implemented feature should receive enough functional UI to use and evaluate
it.

Final structural closure should cover:

- preparation;
- Character selection;
- roster/unlocks;
- Character progression;
- stats;
- Weapon Mods;
- Abilities;
- Expedition HUD;
- targeting;
- Objectives;
- Encounter choices;
- forecast/schedule;
- strategic planner;
- Rewards;
- resources;
- expedition results;
- Character unlock presentation;
- controller navigation.

Final visual skinning remains independently replaceable.

**Depends on:** Player-facing contracts produced throughout the programme.

# Dependency Spine

The current preferred dependency order is:

Player Retention & Core Loop
↓
Character Stats & Expedition Buildcraft
↓
Rewards & Progression Flow
↕
Meta Progression & Resources
↓
Characters & Unlocks
↓
Weapon Mods + Abilities
↓
Enemy Roster & Combat Roles
↓
Combat Targeting
↓
Expedition Objectives
↔
Encounter Variety
↓
Ambient Enemies & World Population
↓
Expedition Pacing, Escalation & Endings
↓
Strategic World Layer Review
↓
UI Structure & Finalisation

This is not a rule that one system must be completely finished before the next
begins.

Later work may reveal a need to revisit an earlier layer.

That is expected iteration.

# Cross-Cutting Work

## Functional UI

Functional UI should evolve alongside each gameplay system.

The final UI milestone closes the overall information architecture; it does not
mark the first time UI work occurs.

## Art & Presentation

Environment, character, combat and UI visual work may continue independently
whenever it provides useful progress.

Visual work is not currently the blocker for the gameplay programme.

## Authoring / Production Workflow

The previous authoring-audit and Tutorial programme is intentionally parked.

It should resume when the systems that will be authored repeatedly have
stabilised enough that workflow improvements are unlikely to be immediately
invalidated.

# Reassessment Rule

Each major programme should end with a deliberate review.

Questions should include:

- Did implementation validate the design assumptions?
- Did another system become a more important dependency?
- Has a planned feature become unnecessary?
- Has a feature grown important enough to split into its own programme?
- Did playtesting expose a better order?
- Are later roadmap milestones still solving the right problems?

Changing the roadmap after obtaining better evidence is expected.