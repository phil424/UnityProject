# Overview
Core philosophy:
> Ecology dynamically changes what opportunities are available during an expedition.

Also:
> Ecology should create tactical incentives rather than arbitrary hard counters.

# Dynamic Events
Potential event categories:
- migrations
- invasions
- rare sightings
- horde movement
- territorial conflict
- weather
- resource events
- monster population shifts.

Define events as potentially having:

```
Location
Duration
Warning Time
Threat
Reward Modifier
Affected Monster Types
Affected Build Mechanics
```


# Upsurges
### Definition:
A temporary dramatic increase in activity from a monster/resource family.

### Example:
> Zombie Upsurge

Effects could include:
- significantly more zombies
- harder encounters
- increased Zombie Crystal acquisition
- greater chance of zombie-specific rewards.

### Principle:
>**Threat + opportunity.**

# Boss Scheduling and Migration
- boss activity follows semi-dynamic schedules
- player can see upcoming events
- bosses may enter and leave zones
- bosses may move between zones
- schedules can overlap
- multiple bosses can share a location.

# Environment Modifiers
### Example:
> Heatwave - Potentially improves fire-related mechanics.

The point is not necessarily:
> Fire Damage +25%.

Could affect:
- status duration
- environmental interactions
- enemy susceptibility
- reward weighting
- event frequency.

### Principle:
> Encourage new opportunities without deleting existing builds.

# Rare Sightings

Ecology can create temporary high-value encounters.

Examples:
- rare monster variants;
- valuable migrating groups;
- unusual bosses;
- resource-rich monster populations.

Rare sightings should create:
> "Do I change my current plan to pursue this opportunity?"

A sighting may contain:
- encounter/location;
- warning or discovery time;
- duration;
- rarity / opportunity value;
- expected reward;
- notable monster.

Rare sightings should integrate with the map / encounter HUD.

# Global Activation Events

Ecological events may affect groups that already exist in the world.

Example:

## Zombie Surge

Possible behaviour:
- activate currently dormant zombie groups;
- begin additional zombie spawn schedules;
- increase activity in selected areas;
- improve zombie-specific reward opportunities.

A level/world schedule should issue commands into existing spawn and combat
engagement seams rather than containing special zombie-spawning logic itself.