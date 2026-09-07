# World Time and Daily Cycle

**Status: Working**

Related:
- [[Expeditions]]
- [[Ecology]]
- [[UI-UX]]
- [[Expedition Scheduling and Encounter Generation]]

# Overview

Expeditions use a global accelerated 24-hour world clock.

World time provides a readable temporal structure for:
- encounter forecasting;
- world events;
- expedition pacing;
- escalation presentation;
- future ecology behaviour.

The speed of the world clock should be configurable.

Example prototype rate:

1 real simulation minute
=
1 in-game hour

At this rate one full game day lasts 24 real simulation minutes.

This value is not locked.

# World Time vs Escalation

World Time and Escalation are separate.

World Time answers:

> What part of the expedition day are we currently experiencing?

Escalation answers:

> How dangerous has this expedition become?

World time cycles through repeated days.

Escalation generally continues rising.

This allows the same daily phase to produce different content depending on how
far the expedition has progressed.

Example:

Day 1 Midnight
- normal zombie surge;
- occasional Elite.

Day 4 Midnight
- larger hordes;
- frequent Elites;
- harder encounter pools;
- greater ambient pressure.

# World Time vs Intensity

Intensity describes immediate pressure and may rise or fall throughout the day.

The daily cycle may influence target intensity but should not eliminate adaptive
peaks and valleys.

Example:

Night may have a higher pressure baseline.

The pacing system can still create quieter and more intense periods within that
night.

# Daily Rhythm

The world-time profile may define broad tendencies for different periods.

Exploratory example:

06:00–12:00
- lower pressure;
- recovery opportunities.

12:00–18:00
- normal encounter pressure.

18:00–00:00
- increased rare / unusual opportunity.

00:00–06:00
- increased horde pressure.

These exact periods and behaviours are not locked.

The daily cycle should create learnable rhythm without making every day identical.

# Schedule

The scheduler uses world time as a readable coordinate system for planned events.

Example:

Day 2

17:50
Church Horde

18:00
Rare Activity begins

20:00
Zombie Surge

00:00
Fiery Horde

The scheduler forecasts these events to the player.

The encounter-generation system remains responsible for supplying encounter
content appropriate to those conditions.

# Forecast Horizons

Immediate:
- precise countdowns around 20–30 seconds.

Near Future:
- detailed schedule for approximately the next 5–10 minutes.

Long Range:
- major events at future world times / expedition milestones.

# Repeating and One-Off Events

The system should support several scheduling shapes.

Possible examples:

Daily:
- increased zombie activity after midnight.

Specific:
- Day 3 18:00 rare migration.

Window:
- schedule one rare encounter event between 18:00 and 21:00.

Generated:
- expedition systems choose a suitable event and place it into the future forecast.

World time is therefore a scheduling coordinate system rather than one fixed
authored script.

# Player-Facing Value

World time should help players anticipate the expedition.

Desired decisions include:

> Night is approaching; should I finish this risky encounter now?

> Rare activity begins at dusk; can I prepare my build first?

> The next Zombie Surge begins at midnight; should I bank rewards before then?

The clock should improve planning rather than simply decorate the HUD.

# Future Possibilities

Exploratory:
- lighting / visual day-night cycle;
- time-specific enemy activity;
- time-specific rare encounters;
- resource behaviour;
- support/scouting effects;
- region-specific daily schedules;
- different world-time rates for different expedition modes.

These are not requirements for the initial prototype.

# Open Questions

- What world-day duration feels best?
- Should every expedition begin at the same world time?
- How much should world time influence baseline intensity?
- Should different regions use different daily profiles?
- How strongly should Day Number communicate escalation?
- Which events should repeat daily versus occur only as expedition milestones?