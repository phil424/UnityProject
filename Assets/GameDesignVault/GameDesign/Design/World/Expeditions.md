# Overview
### Definition
An expedition is one complete temporary run.

### Starts With
- hero
- prepared equipment
- prepared augmentations
- starting abilities
- region selection
- potentially objectives.

### Persists During Expedition
- temporary build
- pending rewards
- current world state
- threat escalation
- resources collected.

### Ends

An expedition ends when:
- the party is defeated;
- the player voluntarily returns / ends the expedition;
- a future successful-return flow ends an Apex-completed expedition.

Party defeat is an expedition-ending failure.

### Party Defeat

Party defeat ends the active expedition.

The following expedition-owned state is lost/reset:
- World Time;
- temporary build progression;
- acquired run abilities / levels / evolutions;
- temporary augments;
- pending temporary rewards;
- escalation;
- schedule;
- current encounters;
- other expedition-owned runtime state.

Persistent progression already earned remains available.

The next attempt is a new expedition rather than a continuation of the defeated
one.

### Normal Level / Region Clear

Clearing a normal level or region is NOT expedition victory.

It represents progress inside the same expedition.

The expedition may continue into:
- another region;
- another encounter cycle;
- greater escalation;
- additional bosses;
- the eventual Apex.

### Major Phases
- Early expedition
- Established build
- Escalating threats
- Apex preparation
- Apex
- Post-Apex survival.

## Example Sequence:

```
Preparation
↓
Region Entry
↓
Initial Schedule Assessment
↓
Choose First Target
↓
Combat / Farming
↓
Acquire Run Progression
↓
Region Escalation
↓
Major Events / Boss Overlaps
↓
Apex Forecast
↓
Prepare for Apex
↓
Apex
↓
Post-Apex Escalation
↓
Death
↓
Rewards / Collection
↓
Preparation
```

Add sections later for pacing.

# Continuous Encounter Supply

An expedition is not intended to be a finite sequence of:

Encounter A
→ Encounter B
→ Encounter C
→ Boss
→ End

The region should continually provide new encounter opportunities for as long as
the temporary build can survive.

The encounter system should maintain enough opportunities that the player is
never left without meaningful options.

Current strong working direction:

> There should always be at least three selectable encounters available during
> normal expedition play.

Completed and expired opportunities are replaced by new encounters appropriate
to the current expedition state.

# Encounter Selection

The player chooses which encounter the hero / party should pursue.

Encounter choice may consider:
- distance;
- threat;
- opportunity rarity;
- known encounter modifiers;
- expected reward;
- rare enemies;
- current build;
- future schedule changes;
- current world events.

Encounter information should generally be generous enough to support an
informed decision.

The intention is not:
> Pick one of three mystery doors.

The player should understand the important properties of the challenge they are
choosing.

# Immediate Redirection

Selecting another encounter is an immediate command.

Expected behaviour:

Encounter A currently targeted
↓
player selects Encounter B
↓
hero drops targets belonging to Encounter A
↓
hero begins moving toward Encounter B immediately

Enemies from Encounter A do not automatically disengage.

They may continue pursuing the hero.

This allows the player to deliberately pull several encounters together into a
larger fight when their build is capable of handling the additional pressure.

# Encounter Commitment

Selecting or starting an encounter commits to that opportunity.

Default expiry behaviour:

Unselected / unstarted encounter
→ may disappear when its expiry window ends.

Selected encounter
→ remains available.

Engaged encounter
→ remains active until resolved.

This creates meaningful decisions around encounters that are close to expiring:

> Commit to it now or lose the opportunity.

# Encounter Relationships

Encounters may influence the availability of other encounters.

Example:

Elite Gauntlet
        ↓ clear
Nobleman's Procession becomes available

A valuable encounter may already be known and expiring while its prerequisite
remains incomplete.

Encounter relationships should remain optional strategic opportunities rather
than turning every region into one fixed linear chain.

# Passive Expedition Play

The game remains an autobattler even when the player does not actively choose
an encounter.

With no encounter directive:

hero
↓
follows ambient activity
↓
travels through authored interesting routes
↓
fights lighter ambient enemies
↓
naturally moves through the region

The player can therefore intervene heavily or allow the expedition to continue
more autonomously.

Ignoring encounter progression should not trigger bespoke punishment.

Instead, rising escalation naturally outpaces a build that is not acquiring
enough new power.

# Threat Escalation

Escalation is the long-term increase in expedition danger.

Examples:
- increased encounter supply;
- larger groups;
- Elites appearing more frequently;
- higher-difficulty encounter pools;
- mixed enemy families;
- additional bosses;
- world events;
- stronger ambient populations;
- eventual Apex pressure.

Escalation should support effectively endless post-Apex survival.

# Intensity and Pacing

Escalation and current intensity are separate.

Escalation generally trends upward.

Intensity should rise and fall.

Desired pacing:

pressure
↓
peak
↓
recovery
↓
new opportunity
↓
greater pressure

The expedition / encounter systems should communicate enough runtime pressure
information to avoid producing endlessly increasing chaos without recovery.

Potential runtime observations may include:
- hostile population;
- encounter overlap;
- damage taken;
- healing frequency;
- current party health;
- recent encounter completion rate.

Exact metrics remain exploratory.

The system should normally adjust future content rather than secretly changing
enemies already committed to combat.

# Forecasting

Future danger should be visible enough to create planning decisions.

The player may receive:
- precise short-term countdowns;
- a detailed near-future schedule;
- major long-range expedition events.

Example:

> Fiery Horde begins in 10 minutes.

The desired feeling is:

> Things are going to change. What should I accomplish before then?

# Apex System

The Apex is the intended expedition-success threshold.

## Convergence Apex

Several / all major region threats converge.

## Ascendant Apex

A heavily empowered form of an existing boss.

## Unique Apex

A special boss exclusive to Apex encounters.

## Apex Philosophy

- major climax;
- highly telegraphed;
- meaningful strategic preparation;
- significant persistent reward;
- marks expedition success;
- does not necessarily force the expedition to end.

## Apex Success

Defeating the Apex marks the expedition as successfully completed.

The player may then choose:

Return
→ finish the expedition successfully.

Continue
→ enter post-Apex endurance.

Post-Apex death does not retroactively erase the achieved success.

## Post-Apex

Post-Apex play supports the endurance fantasy:

> How far can this build go?

Potential characteristics:
- continued escalation;
- boss combinations;
- increasingly rare opportunities;
- extreme encounter overlap;
- prestige / record chasing;
- increasingly valuable rewards.


# Rewards and Pacing
### Problem
Forced upgrade screens can interrupt exciting combat.

### Solution
Rewards can become Pending.

### Manual Resolution
Player may press Pending Rewards and deliberately pause to choose.

### Travel Resolution
Travel/transition periods naturally offer a quieter opportunity to resolve pending rewards.

### Deferral
The player can potentially leave rewards pending.

### Future Opportunities
Challenges could interact with unresolved rewards.

# World Time

Expeditions operate within an accelerated 24-hour world cycle.

The clock gives long runs a readable rhythm and allows future events to be
expressed as meaningful world times rather than only elapsed-time counters.

An expedition may survive through multiple game days.

World time should support decisions such as:

> Dusk is approaching.

> The next rare-activity period begins at 18:00.

> Zombie activity increases after midnight.

The world-time cycle does not replace Escalation.

Repeated days should become progressively more dangerous as the expedition
continues to escalate.