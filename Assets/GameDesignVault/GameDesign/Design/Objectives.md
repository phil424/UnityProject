# Objectives, Targets, Hunts & Achievements

**Status: Working**

# Purpose

MiniCrawler needs goals operating across different time horizons.

The systems in this document exist to answer:

> What am I trying to accomplish right now?

without requiring every expedition to have the same duration or destination.

# Goal Vocabulary

## Progression Target

**Lifetime:** Persistent / across expeditions.

A Progression Target is something the player has decided they currently want
from the Meta layer.

Examples:
- a Character Mastery node
- a Weapon Mod unlock
- a Character unlock
- a persistent upgrade
- another persistent progression milestone.

A Progression Target may expose its requirements to the expedition layer.

Example:

Heavy Impact

Current:
47 L1
0 L2

Required:
120 L1
1 L2

The game may use this context to help surface relevant expedition
opportunities.

A Progression Target is not necessarily a quest.

It represents:

> This is what I currently care about progressing toward.

---

## Expedition Objective

**Lifetime:** Current expedition.

Expedition Objectives are intended to provide much of the missing 1–3 minute
motivation layer.

They create immediate intentions such as:

- complete specific Encounter types
- defeat an Elite
- defeat a Boss
- collect resources
- defeat particular enemy families
- visit/discover a location
- survive a World Event
- use a mechanic in a particular way
- progress a Hunt
- satisfy another short-term condition

An Objective should make Encounter selection more purposeful.

The intended question becomes:

> Which opportunity helps me accomplish what I am currently trying to do?

rather than:

> Which Encounter button should I press next?

Objectives may be:
- generated;
- authored;
- derived from Progression Targets;
- created by Hunts;
- created by World Events;
- granted by other future systems.

The final mixture is unresolved.

---

## Hunt

**Lifetime:** Potentially multiple expeditions.

A Hunt is an authored sequence of content specific progress.

Examples:
- collect Idols that reveal or summon a Boss
- complete encounters associated with a particular enemy
- collect Sigils across several expeditions
- follow an authored chain toward a Character unlock
- unlock access to a Weapon Mod branch
- progressively reveal a recurring enemy NPC

Hunts exist partly to preserve content specific goals that cannot simply be
purchased using generic resources.

Example:

THE BONE TYRANT

Bone Idols
2 / 3

Completing the Idol requirement may reveal or enable the Bone Tyrant encounter.

Hunts may provide:
- Boss access
- Character progression
- Weapon Mod access
- persistent resources
- narrative discoveries
- other authored unlocks

---

## Achievement

**Lifetime:** Persistent.

Achievements represent long term mastery, collection or unusual accomplishments.

Examples:
- cause enemies to whiff a large number of attacks
- complete particular build combinations
- defeat enemies using specific mechanics
- discover hidden content
- complete difficult expedition conditions

Achievements may provide rewards or unlocks.

They are not automatically active expedition instructions.

The player may eventually be able to pin selected Achievement progress where
useful.

# Playable Time Horizons

Objectives are especially important to the 1–3 minute gameplay horizon.

Current development model:

| Horizon | Primary Question |
| --- | --- |
| 10–30 seconds | What can I do right now? |
| 30–60 seconds | What should I fight next? |
| 1–3 minutes | What am I currently trying to accomplish? |
| 3–10 minutes | Where is this expedition/build going? |
| 10–30+ minutes | How far can I push this expedition? |
| Across expeditions | What am I progressing/unlocking long term? |

These are not mandatory expedition lengths.

A player may enter an expedition with a goal that can legitimately be completed
after only a few minutes.

# Variable Expedition Depth

A short expedition is valid.

Example:

Player wants:
- 20 L1 Resource;
- 1 L2 Resource.

Possible plan:

Resource Encounter
↓
Resource Encounter
↓
Early Boss
↓
requirements satisfied
↓
Return

The game may then tempt the player to remain through:

- a strong temporary build;
- rare opportunities;
- temporary resource multipliers;
- Hunts;
- Weapon Mod opportunities;
- Ability opportunities;
- rare enemies;
- deeper Bosses.

The player should be able to decide:

> I have achieved what I came for.

or:

> This run has become valuable enough that I want to continue.

Deeper gameplay should add opportunities rather than invalidate shallow play.

# Encounter Relevance

Encounter presentation should eventually communicate enough information for
Objectives and Progression Targets to influence selection.

Potential information includes:

- threat;
- Reward Vector;
- expected resource yield;
- Objective relevance;
- Hunt relevance;
- rarity;
- expiry / urgency.

The exact amount of information remains unresolved.

# Tracked Goals

The normal combat HUD should not display every possible goal.

Working direction:

- a small number of actively tracked Objectives;
- optional Progression Target;
- optional Hunt;
- additional detail in an expanded panel/log.

This should follow progressive disclosure and avoid recreating the existing HUD
information-overload problem.

# Objective Rewards

Objectives may eventually provide:

- Expedition rewards;
- persistent resources;
- Character progression;
- Weapon Mod progression;
- Ability progression;
- Hunt progress;
- special Encounter access;
- narrative discoveries.

The Objective system should not assume that every Objective simply grants
currency.

# Design Principle

Objectives should create intention, not chores.

A useful Objective changes how the player evaluates the available world.

If an Objective can be ignored without changing any decision, its purpose should
be questioned.

# Open Questions

- How many Expedition Objectives are normally active?
- Which Objectives are automatically generated?
- Which can the player select?
- Can Objectives be replaced/rerolled?
- How strongly should Progression Targets influence generated Objectives?
- When should an Objective reveal exact rewards versus Reward Vectors?
- How much Hunt progress should be possible in one expedition?
- Should some Objectives become optional bonus conditions inside Encounters?