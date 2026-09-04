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
Current direction:
> Eventually the player's build can no longer survive.

### Level Defeat

Current implemented direction:

A level defeat does not automatically end the expedition.

The player may resolve pending rewards and continue to the next level.

### Expedition End / Final Failure

The eventual condition that truly ends a long expedition remains a separate
design problem.

Current long-term direction still expects expeditions to eventually reach a
point where the temporary build can no longer continue effectively.

Do not conflate a normal level defeat with the final expedition-ending state.

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

# Encounter Selection

Regions should contain multiple threats and opportunities rather than only one
mandatory next fight.

The player can assess known encounters and choose where to direct the hero /
party next.

Encounter choice may consider:
- distance;
- threat;
- rewards;
- rare enemies;
- ecology events;
- upcoming schedule changes;
- current build needs.

The player should sometimes face decisions such as:

> Continue fighting the nearby horde, or redirect toward a rare valuable target
> that has just appeared elsewhere?

This makes traversal part of strategic decision-making rather than only downtime
between fights.

# Threat Escalation - (Risk of Rain)
### Purpose
Create increasing tension without relying only on inflated enemy health/damage.

### Escalation Examples
- larger enemy groups
- stronger enemies
- mixed enemy families
- additional bosses
- multiple bosses simultaneously
- boss/minion combinations
- upsurges
- environmental events
- migration collisions.

### Pacing
Should contain ebbs and flows rather than monotonically increasing intensity.

### Telegraphing
Major escalation should be visible ahead of time through the region schedule.

Example:
> THREAT LEVEL INCREASE IN 10:00

### Principle
The player should feel:
> Things are about to get worse. What can I accomplish before then?


# Apex System
### Convergence Apex
Several/all region bosses converge.

### Ascendant Apex
A heavily empowered version of an existing boss.

### Unique Apex
A special boss exclusive to Apex encounters.

### Apex Philosophy
- major climax
- highly telegraphed
- significant reward
- not necessarily the end.

### Post-Apex
Clearing Apex can open effectively endless escalation.


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

