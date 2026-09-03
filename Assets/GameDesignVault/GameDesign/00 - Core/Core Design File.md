# My Game Design

## Working Design Baseline v0.1

### Core Combat Fantasy

This Game is an auto-battler in which the character handles routine combat execution while the player watches the encounter, evaluates threats, and decides when to intervene with active abilities.

The player should not spend combat repeatedly issuing basic attack or movement commands. Their primary moment-to-moment skill expression comes from:
- understanding the state of the battle;
- reading enemy behaviour and upcoming threats;
- understanding their weapon's automatic attack sequence;
- deciding whether to allow an attack combo to continue;
- deciding whether to interrupt that combo;
- timing active abilities;
- managing ability cooldowns and other combat resources;
- adapting their build to the opportunities presented during the run.

Player input should feel immediate. An ability may have an intentional cast or activation time, but the game should not introduce artificial input delay between pressing the ability and the character responding.

---
# 1. Weapons
Weapons are a primary source of playstyle identity.

A weapon is not primarily a collection of damage and attack-speed statistics.

Equipping a different weapon should materially change how the character behaves during autonomous combat.

A weapon may define:
- base attack characteristics;
- attack cadence;
- attack range;
- attack coefficients;
- number of attacks in an automatic combo;
- individual combo-step behaviours;
- movement during attacks;
- dash attacks;
- cleaves;
- projectiles;
- finishers;
- knockback;
- status application;
- innate abilities;
- augmentation slots;
- special rules;
- defensive properties during particular attacks;
- invulnerability frames during specific combo steps.

Individual combo attacks may have their own authored properties.

Example:
**Twin Blades**
Attack 1 → rapid slash  
Attack 2 → rapid slash  
Attack 3 → dash through target with temporary invulnerability  
Attack 4 → multi-hit finisher

This should feel fundamentally different from:
**Great Hammer**
Attack 1 → broad swing  
Attack 2 → overhead impact  
Attack 3 → slow charged slam with heavy knockback

Weapon design, balancing and playtesting should therefore eventually be treated as a major content-development discipline rather than simple item-stat balancing.

---
# 2. Automatic Attacks and Player Abilities
Weapon attacks are predominantly autonomous.

Player abilities are predominantly deliberate.

The character automatically executes their weapon attack sequences according to the combat AI.

The player observes these sequences and chooses whether an active ability is worth using immediately or whether allowing the current weapon combo to continue would be more valuable.

For example:
Attack 1  
→ Attack 2  
→ player has Whirlwind available  
→ Attack 3 would trigger a powerful weapon finisher

The player chooses:
**wait for Attack 3**

or

**cancel now and Whirlwind immediately**

The correct decision depends on the situation.
This timing trade-off is an intentional component of combat mastery.

---
# 3. Ability Interruption
Player-triggered abilities have authority over autonomous combat.

If the player activates an ability during an automatic weapon sequence, the character should respond appropriately and interrupt/cancel autonomous behaviour where the ability permits it.

The game should not make the player feel that their character is ignoring their input because an AI-controlled attack sequence has priority.

Different abilities may have different:
- cast times;
- animations;
- cancel behaviour;
- movement behaviour;
- targeting behaviour;
- recovery periods.

However, player agency remains the priority.

---
# 4. Abilities Are Independent Gameplay Objects
Abilities should exist independently from the systems that grant them.

For example:
**Whirlwind** is an ability.

It is not inherently:
**GreatAxeWhirlwind**

The same ability could potentially be granted by:
- a character;
- a weapon;
- prepared equipment;
- an augment;
- a starting loadout;
- a temporary reward during a run;
- another future system.

This allows the player to deliberately construct partial starting combinations while still relying upon the run to complete or transform the build.

Example:
A player may prepare:
* Weapon X  
+ Armour Y  
+ Decoration Z

and begin an expedition with three abilities they deliberately enjoy using.

The fourth major ability may then be something they intend to discover organically during the run.

---
# 5. Character Identity
Characters should have strong identities without prescribing a specific weapon or complete playstyle.

A character should not effectively mean:
> This is the Greatsword Character.

Instead, character identity should be constructed from several independent components.

The current working character foundation is:
### Innate Ability
The character's signature mechanic.
Examples might eventually include:
- rage;
- temporary invulnerability;
- charge;
- regeneration;
- stance mechanics;
- unique resource mechanics.

### Stat Profile
Characters have different tendencies.
Possible characteristics include:
- heavy;
- fast;
- agile;
- resilient;
- powerful;
- mobile;
- ability-focused.

These characteristics can overlap.
A character could be:
**Heavy + Agile**

while another could be:

**Fast + Agile**

This allows characters to feel fundamentally different without assigning them mandatory equipment.

### Combat Utility
Each character has a characteristic utility ability.
This might include:

- buffs;
- debuffs;
- mobility;
- defensive utility;
- crowd control;
- party utility.

### Sustain / Healing
Healing behaviour should itself be part of character identity.

Different characters may sustain themselves differently.

Examples:
- direct self-healing;
- healing through dealing damage;
- regeneration;
- consuming environmental resources;
- healing between zones;
- defensive conversion mechanics.

### Passives

Characters may additionally possess passive rules that further reinforce their identity.

---
# 6. Core Ability Presentation
The current UI fantasy is a staggered diamond/chevron-shaped ability display on the left side of the screen.

A likely central character cluster consists of:

**Utility**
↖

**Innate Ability**
→ visually central to character identity

**Healing / Sustain**
↗
  
These three abilities form the character's foundational combat identity.

Additional active combat abilities may extend upward from this core cluster.

The final number of ability slots is deliberately unresolved.

The system must therefore not currently assume a hard maximum such as four abilities.

A rough future layout could support approximately six abilities, but this is not yet a rule.

---

# 7. Ability Slot Philosophy
Different ability slots may eventually have different purposes.

Possibilities include:
- fixed innate slot;
- utility slot;
- healing/sustain slot;
- unrestricted active ability slots;
- combat-only ability slots.

This requires further design work.

The project should not prematurely encode rigid slot restrictions until the ability system has enough content to determine whether restrictions improve buildcraft or merely reduce player freedom.

---
# 8. Optional Autocast
Although active ability timing is a primary player interaction, individual abilities may eventually support an optional:

**Autocast** setting.

A player could choose to let an ability automatically activate when available.

Examples:
Whirlwind  
`Autocast: OFF`

Rage  
`Autocast: ON`

This can support:
- different player preferences;
- accessibility;
- low-attention builds;
- abilities whose tactical timing matters less;
- highly automated character configurations.

Autocast should be an optional build/player decision rather than the default assumption behind the ability architecture.

The player choosing manual control should retain the advantages that come from intelligent timing.

---
# 9. Duplicate Ability Rewards
Finding an ability already owned should primarily create opportunities for evolution rather than simply increasing a level number.

Example:
Player owns:
**Whirlwind**

Whirlwind appears again as a reward.

Instead of:
> Whirlwind Level 2: +10% damage

the player might receive an ability-specific evolution choice:
> **Vortex** - Whirlwind pulls nearby enemies inward.

> **Serrated Storm** - Whirlwind applies Bleed.

> **Cyclone** - Whirlwind becomes significantly larger.

Numerical improvements can still exist within evolutions, but the main purpose is behavioural transformation.

A developed ability should increasingly become a product of the particular run in which it was built.

---
# 10. Augments

The exact terminology remains provisional.

The current broad definition is:
> An augment is a modular modification that improves or alters some aspect of the build.

Augments may eventually include both behavioural modifications and simpler progression pieces.

Behavioural example:
> **Volatile Wounds** - Bleeding enemies explode when killed.

Numerical example:
>**Attack Level I** - Gain one stack of an attack improvement that increases weapon damage according to the weapon's coefficients.

Because several overlapping progression systems are still emerging, final terminology should be decided during a later dedicated taxonomy pass.

Terms that may eventually require explicit separation include:
- augment;
- decoration;
- evolution;
- mutation;
- upgrade;
- passive;
- ability modifier;
- weapon modifier.

---
# 11. Augment Compatibility
Permanent augments, if they remain part of the final progression model, should generally be flexible.

They should not normally be permanently locked to one specific item.

Compatibility restrictions may exist where they make thematic or mechanical sense.

Examples:
A projectile-specific augment should not apply to something with no projectile behaviour.
A weapon-combo augment may require an appropriate weapon.
An ability-specific modification may require the relevant ability.

Restrictions should preserve coherent behaviour rather than arbitrarily limiting buildcraft.

---
# 12. Permanent Versus Temporary Augments
The precise boundary between prepared permanent augments and run-specific upgrades remains unresolved.

The underlying goal is clear:
Preparation should allow experienced players to influence the direction of their build.

The run should still determine much of what that build eventually becomes.

A highly progressed player may eventually be able to establish roughly **30% of the intended build before entering the region**, but should not be able to guarantee the complete final combination.

This percentage is conceptual rather than a mathematical target.

---
# 13. Reward Generation
Reward generation must understand the player's current build.

Pure random generation is explicitly undesirable.

If the player intentionally starts with a Bleed-focused configuration, the reward system should recognise that and provide meaningful opportunities to continue developing it.

However, rewards should not simply hand the player the predetermined optimal path.

A desirable reward set may contain:
- a direct synergy;
- an adjacent possibility;
- an opportunity to pivot.

For example:
Player currently uses Bleed.

Reward offers:
> **A — Volatile Wounds** - Bleeding enemies explode.

> **B — Haemorrhage** - Bleed stacks more effectively against large enemies.

> **C — Flame Conversion** - Begin moving toward a Fire-based interaction enabled by the current world state.

All three can be meaningful without being identical.

---
# 14. Ecology Influences Buildcraft
The world itself should participate in build decisions.

Ecological events can temporarily change the relative value of particular mechanics.

Example:
**Skeleton Upsurge**

Skeleton enemies may be resistant or immune to certain Bleed interactions.

The player's Bleed build is not deleted or declared invalid.

Instead, the world presents a temporary tactical question:

> Do I continue pursuing the existing strategy, avoid this opportunity, or temporarily diversify?

Another run might generate:

**Heatwave**
Fire effects become unusually effective.

A player who did not originally intend to build around Fire may now have a compelling reason to consider:
- flaming projectiles;
- Meteor;
- burning trails;
- Fire-based ability evolutions.

World events should therefore create **opportunities**, not hard counters.

A central principle is:
> The game should create several attractive answers rather than constantly revealing one mathematically obvious correct build.

---
# 15. Reward Hierarchy
The reward structure requires a dedicated future design pass.

A promising working direction is to separate frequent incremental progression from major build-changing rewards.

For example:
### Common enemies / frequent rewards
Potentially provide:
- stat improvements;
- coefficient improvements;
- minor modifiers;
- incremental progression;
- supporting build resources.

### Elites / objectives / rarer encounters
Potentially provide:
- stronger augments;
- meaningful modifiers;
- ability-related choices.

### Bosses / major events
Potentially provide:
- new abilities;
- ability evolutions;
- mutations;
- transformative build mechanics;
- high-impact choices.

Basic numerical upgrades such as flat damage are therefore not currently removed from the design.

Their role needs to be reconsidered as part of a richer statistics and reward model.

Their purpose may be to create the frequent feedback/progression loop between larger transformative decisions.

---
# 16. Player Unit Structure
The design is currently moving toward:
> **One primary player-controlled hero**

rather than a party of several equally important player-controlled characters.

This is not irrevocably locked, but it is increasingly compatible with the ability-heavy interaction model.

If additional party members exist, the current preferred direction is:
> Hero + AI-controlled supporting characters.

The player could control:
- support character selection;
- equipment;
- weapons;
- abilities;
- behaviour/loadouts;

while those characters operate autonomously in combat.

Multiplayer could potentially allow each human player to control their own hero and active abilities.

Core architecture should therefore avoid requiring either exactly one actor or exactly four equal party members.

---
# 17. Buildcraft Philosophy
The player intentionally starts a run with a direction.

The game should respect that intention.

The purpose of randomness is not to destroy the player's plan.

Randomness should:
- transform it;
- challenge it;
- expand it;
- occasionally tempt the player away from it;
- create unexpected synergies.

A strong run should feel like:
> I started intending to build X, but then Y happened, so I adapted into this ridiculous X/Y/Z combination.

rather than:
> I wanted X and the game refused to give me anything relevant.

---
# 18. Core Buildcraft Principle
Preparation creates the foundation.

The expedition creates the build.

The ecology creates opportunities.

The player decides how to respond.