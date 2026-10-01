# Overview

Progression has two primary layers:

Persistent Progression
+
Expedition Progression

They serve different purposes.

# Persistent Progression

Main rule:

> Persistent progression improves preparation, starting identity and player
> options.

It survives expedition success and failure.

Potential persistent progression:
- characters;
- support characters;
- weapons;
- armour;
- gear upgrades;
- ability unlocks;
- materials;
- recipes;
- crafting progression;
- cosmetics;
- achievements;
- monster discoveries.

Persistent power should help the player begin stronger and access more
strategies.

It should not make expedition buildcraft irrelevant.

# Incremental Equipment Progression

**Status: Deprecated Direction**

The Preparation frontend no longer exposes Weapon / Armour / Focus levels as
the primary persistent progression surface.

Character Mastery now occupies that role.

The previous plan centred persistent progression on separate Weapon and Armour
trees.

That is no longer the current primary direction.

Useful ideas from those tree designs should migrate into future Character
Mastery / Weapon Mod / Ability progression rather than being discarded.

A major form of persistent progression should be authored skill trees attached
to weapons and armour.

These trees intentionally combine two reward scales.

## Frequent Progress

Small nodes provide regular improvement.

Examples:
- +Damage;
- +Armour;
- +Health;
- +Attack Speed;
- +Movement Speed;
- +Range.

These nodes provide the satisfying incremental sense that almost every
expedition contributes something.

## Milestone Progress

Larger nodes meaningfully change preparation possibilities.

Examples:
- ability unlock;
- ability slot;
- augment slot;
- combo modification;
- defensive mechanic;
- weapon passive;
- armour passive;
- unique equipment behaviour.

The combination should create:

frequent small progress
+
longer-term anticipation of meaningful unlocks.

Persistent progression should broaden and strengthen the starting build without
removing the need to build significant temporary power during an expedition.

# Character Mastery

**Status: Implemented Foundation**

Character-focused progression is now the preferred major persistent progression
surface.

The first runtime Character Mastery implementation is Character-owned and
data-driven.

Current flow:

Persistent Resources
↓
Mastery Node purchase
↓
per-Character persistent ownership
↓
Prepared stat modifiers
↓
Expedition RunBuild snapshot
↓
runtime Actor reconstruction

The first Punchy proof contains:

- Heavy Hands;
- Conditioning;
- Combo Tempo;
- Impact Training.

Punchy's Character Mastery now also includes:

Heavy Finisher Stance
→ prepares the Heavy Finisher Weapon Mod stance in every future Punchy
  Expedition.

This is the first Mastery node that expands the Character's selectable Basic
Attack behaviour rather than directly increasing a stat or Ability level.

Weapon Mod stance ownership follows the existing prepared-build lifetime:

persistent Mastery ownership
↓
`ApplyMasteryToBuild`
↓
new Expedition `RunBuild`
↓
available Weapon Mod stance.

The Ranger proof extends Character Mastery beyond flat stats.

Current Ranger Mastery demonstrates:

Quick Draw
→ prepared Basic Attack Speed.

Mobility
→ prepared Move Speed.

Power Shot Drills
→ persistent starting Ability-level development.

Arrow Impact
→ prepared Knockback Power.

Piercing Arrows Stance
→ prepares Ranger's Piercing Arrows Weapon Mod every Expedition.

Ranger now provides the second Character Mastery → Weapon Mod proof.

Heavy Finisher and Piercing Arrows demonstrate that the same persistent Mastery
mechanism may prepare Mods with very different runtime behaviour.

Persistent Ability development and temporary Expedition Ability development
remain separate lifetimes.

Example:

Power Shot Drills
→ Expedition begins at Power Shot Lv.2

temporary run progression
→ Power Shot reaches Lv.3

Expedition ends
→ temporary Lv.3 progress is lost

next Expedition
→ persistent Lv.2 starting state remains.

These are prototype gameplay nodes rather than the intended final Punchy tree.

Mastery nodes may depend on other nodes and may require multiple persistent
resource tiers.

Prepared Mastery bonuses and temporary Expedition bonuses remain separate
sources even when they modify the same runtime stat.

Mastery now supports:

- prepared stat development;
- persistent starting Ability levels;
- persistent Weapon Mod stance unlocks.

Future node implementations may extend this to additional Character-specific
mechanics as concrete content requires them.

It may eventually contain:

- small stat development;
- autonomous combat / combo development;
- Ability progression;
- Weapon Mod unlocks;
- Character-specific mechanics;
- cross-Character/global unlock milestones.

Major progression should increasingly create new options rather than only raw
numerical superiority.

# Generic Resource Tiers

**Status: Implemented Foundation**

The first runtime implementation now supports three placeholder persistent
resource tiers:

- Level 1 Resource;
- Level 2 Resource;
- Level 3 Resource.

These names are explicitly temporary.

Current prototype acquisition demonstrates the intended progression shape:

Normal enemies
→ primarily Level 1.

Elites
→ Level 1 + Level 2.

Bosses
→ Level 1 + Level 2 + Level 3.

Exact quantities are prototype validation values rather than economy balance.

Resources earned during an expedition are staged in Expedition-owned state and
are banked into persistent progression when the expedition ends through either
Voluntary Return or Defeat.

Already-earned persistent resources are therefore not lost because the run
ended in failure.

Tier conversion remains unimplemented until useful ratios and sinks exist.

Persistent progression may use broadly reusable tiered resources.

Working placeholder vocabulary:

- Level 1 Resource;
- Level 2 Resource;
- Level 3 Resource.

Exact names and values are unresolved.

Resources may potentially convert between tiers.

Important principles:

- lower-tier resources remain useful later;
- deeper natural acquisition should be substantially more efficient for
  higher-tier resources than repeatedly farming shallow content;
- upward conversion should be possible where useful but intentionally
  expensive;
- downward conversion may help solve shortages of lower-tier resources;
- conversion must not create arbitrage;
- the economy should not make one generic farming strategy optimal for every
  progression goal.

Future balancing should compare expected resource yield by expedition depth
against conversion ratios and progression costs.

# Content-Specific Progression

Not every progression requirement should be fungible.

Potential non-generic requirements include:

- Idols;
- Sigils;
- Boss Marks;
- Hunt progress;
- Character-specific progression;
- narrative discoveries;
- special authored unlock conditions.

Generic resources provide economic flexibility.

Content-specific progression preserves reasons to engage with particular
content.

# Expedition Progression

Resets when the expedition ends.

Examples:
- acquired abilities;
- ability levels;
- evolutions;
- temporary augments;
- temporary stat upgrades;
- temporary run currency;
- pending temporary rewards.

The expedition build should provide the majority of the dramatic power growth
required to survive increasing escalation.

# Desired Relationship

Persistent Progression
→ creates stronger / broader starting possibilities.

Expedition Progression
→ determines how successfully the player adapts and scales during this attempt.

A useful shorthand:

> Persistent power gets you through the front door.
>
> Expedition buildcraft determines how deep you can go.

# Failure Progression

Failure should still advance the broader game.

A failed expedition may still retain:
- persistent currency;
- materials;
- permanent unlocks;
- discovered content;
- other committed persistent rewards.

The temporary expedition build is lost.

This creates:

Attempt
↓
earn permanent progression
↓
fail
↓
prepare better
↓
new attempt

without making failure equivalent to losing all progress.

# Character Unlocks

**Status: Implemented Foundation**

Playable Character ownership is persistent progression.

The first real Character unlock proof uses Ranger.

Working prototype flow:

Locked Ranger
↓
complete the Cull Dormant Hordes Expedition Objective
↓
finish the Expedition
↓
persistent Ranger unlock progress increases
↓
repeat until the authored requirement is satisfied
↓
Ranger becomes persistently owned
↓
Preparation refreshes
↓
Ranger becomes selectable.

For the first implementation proof, Ranger requires two completed Cull Dormant
Hordes Objectives across separate Expeditions.

This is a prototype progression requirement rather than Ranger's final narrative
or balance-tuned unlock condition.

Only a completed Objective contributes to the persistent unlock.

Partial in-Expedition Objective progress does not persist between Expeditions.

Completed unlock progress is committed when the Expedition ends through normal
Voluntary Return, Defeat or Success.

This intentionally follows the existing failure-progression principle:

meaningful permanent progress earned during an Expedition
→ survives the loss of the temporary Expedition build.

# Other Unlock Questions

Still unresolved:

- final Character unlock conditions;
- Weapon unlocks;
- recipes;
- areas;
- Ability unlocks;
- cosmetics;
- whether some later unlocks use resources, Hunts, bosses, discoveries or other
  authored requirements.

# Meta Progression
Put all current unresolved hub ideas here.

Potential:
- skill points
- permanent tree
- run milestones
- unlock currency
- hub upgrades.

Also explicitly say:
> A substantial meta progression system is not currently guaranteed to exist.

That prevents the documentation from quietly turning an idea into a requirement.

# Challenges and Milestones
Your pre-run goal system belongs here.

Potential:
- randomly generated
- rerollable
- selectable
- encourage different hunts
- reward persistent progress.

Examples:
- defeat specific boss family
- survive a threat level
- defeat overlapping bosses
- complete challenge with restricted setup
- reach Apex under unusual conditions.

# Long-Term Endgame
Core requirement:
> Completing the collection must not remove the reason to play.

Potential long-term motivations:
- endless post-Apex
- unusual builds
- challenge modes
- rare variants
- cosmetics
- achievements
- leaderboards
- difficulty modifiers
- high-rarity augments
- increasingly unusual ecology.

# Support Progression

Current working direction:

Support characters are persistent unlocks and are configured outside the active
run.

Their preparation may eventually include:
- selection;
- tactics / behaviour preset;
- full custom tactics;
- equipment or progression where appropriate.

The exact support progression model is unresolved.

See [[Support Characters]].