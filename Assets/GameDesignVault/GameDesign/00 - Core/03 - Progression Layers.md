# Progression Layers

**Status: Working**

MiniCrawler progression exists across several different lifetimes.

These layers should reinforce one another without becoming interchangeable.

# Persistent Collection

Things the player permanently owns or has discovered.

Potentially:

- Characters;
- Weapon Mods;
- Ability unlocks;
- persistent resources;
- Character Mastery;
- Hunts / authored unlock state;
- Achievements;
- Boss / enemy discoveries;
- cosmetics;
- narrative discoveries.

## Playable Character Ownership

**Status: Implemented Foundation**

Playable Character ownership is now stored in `PersistentProgression`.

The authored `PlayableCharacterRosterDefinition` describes which Character
Definitions belong to the playable roster and which begin unlocked.

Persistent ownership then records which of those Characters the player currently
owns.

Character selection for an Expedition may only choose an owned roster Character.

The first implemented roster contains Punchy as the starting Character.

Actual Character unlock conditions are deferred to later O1 work.

# Persistent Development

Long-term progression applied between expeditions.

Current working direction includes:

- Character Mastery;
- Character unlocks;
- Weapon Mod unlocks;
- Ability/global unlocks;
- resource investment;
- cross-Character progression.

Persistent progression should increasingly expand possibilities rather than
simply remove expedition difficulty through raw numerical power.

# Progression Targets

A player may identify a persistent progression goal they currently care about.

Example:

Heavy Impact

Required:
120 L1
1 L2

Progression Targets may influence:

- tracked HUD information;
- Expedition Objectives;
- Encounter relevance;
- reward/opportunity presentation.

See [[Objectives]].

# Starting Configuration

The subset deliberately brought into an expedition.

Working direction:

- one active Character;
- Character's persistent progression;
- Character-native combat identity;
- prepared Weapon Mods;
- prepared/starting Ability decisions where appropriate;
- selected/tracked goals where appropriate.

The starting configuration creates direction but should not guarantee the final
expedition build.

# Expedition Progression

Temporary progression that resets when the expedition ends.

Examples:

- temporary stats
- acquired Abilities
- Ability levels
- Evolutions
- temporary Augments
- temporary buffs
- synergies
- build-changing rewards

The expedition should provide the majority of dramatic short term power growth.

# Objectives / Hunts

Objectives create short term intention.

Hunts may persist across multiple expeditions and unlock content specific
progress.

They are progression systems, but they are not equivalent to generic currency.

See [[Objectives]].

# Narrative / Discovery State

Knowledge and narrative discoveries may persist across expeditions.

They are not economic resources and should not simply be purchasable through
generic resource conversion.

See [[Narrative]].

# World State

The expedition itself also progresses.

Potential dimensions:

- World Time;
- Escalation;
- Encounter supply;
- World Events;
- Ambient population;
- Boss availability;
- Apex state.

World progression changes which opportunities and threats exist.