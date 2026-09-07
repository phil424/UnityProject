# Overview
Principles:
- combat information should be readable quickly;
- do not force unnecessary interruptions;
- detailed information can be optional/collapsible;
- major rewards deserve strong presentation;
- UI should communicate future threats, not only current state.

# Combat HUD
Document current layout vision.

### Bottom Left
- hero portrait
- health
- buffs/debuffs
- possibly core character ability.

### Left
- active ability chevron/diamond.

### Bottom Right
- currencies
- materials
- loot feedback.

### Top Right
- DPS
- ability performance
- expandable analytics
- combat feed.

### Encounter Space
- normal enemy health above enemies
- major boss meter on HUD.

# Ability HUD
Current visual concept:
```
      Ability
   Ability Ability
 Utility Core Sustain
      Portrait
```

Not literal final geometry.

Document:
- innate visually central
- ability count unresolved
- autocast indicator potentially available
- cooldown readability
- ability state readability
- pending/cast information.

# Strategic Combat HUD

The passive combat HUD should remain glanceable.

Detailed information belongs in deliberately opened expanded panels.

Primary HUD questions:

Top Left:
> What is happening next?

Top Right:
> Where am I and where can I go?

Bottom Left:
> How am I fighting?

Bottom Right:
> What have I earned / what decisions are pending?

Large portions of the screen should remain unobstructed so watching combat stays
central to the experience.

# Upcoming Schedule

The passive HUD should display the next three upcoming encounter arrivals or
other immediately relevant schedule entries.

Example:

UPCOMING

00:18 Graveyard Horde
00:42 Nobleman's Procession
01:05 Elite Patrol

Short actionable transition / expiry countdowns should generally be around
20–30 seconds maximum.

The expanded strategic view may expose:
- the detailed next 5–10 minutes;
- major expedition events significantly further ahead.

Long-range presentation may resemble a planner / calendar.

# World Clock Presentation

The passive combat HUD should show current world time near the simulation /
schedule information.

Working presentation:

DAY 2
17:42

The clock should remain compact and immediately readable.

The expanded strategic view may use world time as the backbone of a larger
planner / calendar timeline.

Example:

NOW — 17:42

17:50
Church Horde

18:00
Rare Activity

20:00
Zombie Surge

00:00
Fiery Horde

Immediate events should still provide countdowns when they enter the short
20–30 second response window.

Long-range information may use world-time labels instead of continuously
displaying large countdown values.

# Current World Events

Current map-wide or regional effects should be visually distinct from encounters.

Examples:
- Zombie Surge;
- Fiery Zombies.

A compact status area near / over the minimap is currently preferred.

These should communicate:
> What rules or pressures are affecting the region right now?

rather than:
> Where should I go?

# Combat Minimap

The minimap answers:

> Where?

Working information:
- hero / party;
- current encounter directive;
- three quick encounter choices;
- important rare opportunities;
- major world threats where appropriate.

Do not default to displaying every normal enemy.

Quick encounter symbols must remain consistent between:
- minimap;
- encounter list;
- notifications;
- controller prompts.

# Quick Encounter Slots

The passive HUD exposes exactly three quick encounter options.

Current default ordering:

> The three oldest selectable encounters.

Reason:
Older encounters are generally nearer expiry and therefore more strategically
urgent.

The world may contain more than three known encounters.

The quick slots are only the immediate-access layer.

Controller working concept:

R1 + Triangle
R1 + Square
R1 + Circle

→ select one of the three quick encounters.

Mouse / keyboard should expose equivalent direct interaction.

There should never normally be fewer than three selectable quick encounter
options.

## 3.0D Prototype

The first playable quick-selection implementation uses a dedicated
`QuickEncounterChoices` gameplay-facing selection model.

The presentation layer does not determine encounter selection rules.

Flow:

LevelEncounter availability
        ↓
QuickEncounterChoices
        ↓
three oldest selectable encounters
        ↓
HUD / future controller input
        ↓
EncounterDirectionController

Encounter age is currently represented by an availability sequence.

Future scheduler / encounter-generation work may replace or supplement this with
explicit creation / availability timestamps.

The unresolved question:

> Should the current selected encounter occupy one quick slot?

is deliberately exposed as a prototype toggle.

The quick-choice provider does NOT create replacement encounters.

Guaranteeing three continuously available opportunities belongs to Encounter
Supply rather than UI.

# Encounter Selection

Selecting an encounter immediately changes the encounter directive.

The hero should:
- drop autonomous targets belonging to the previous encounter;
- immediately begin travelling toward the newly selected encounter.

Enemies from previous encounters may continue pursuing.

This can allow deliberate encounter stacking.

# Expanded Strategic View

Working input concept:

D-pad Right
→ open strategic view
→ strongly slow simulation.

Exact slow-motion versus full pause should be playtested.

The expanded view may contain:
- larger map;
- all known encounters;
- locked encounters;
- encounter details;
- full forecast;
- long-range major events;
- threat;
- opportunity rarity;
- known modifiers;
- expected reward information.

Sorting/filter behaviour remains unresolved until the detailed panel wireframe
has been evaluated.

# Encounter Information

Encounter selection should be informed.

The player should generally understand significant properties before committing.

Possible information:
- threat;
- opportunity rarity;
- enemy composition;
- notable Elite / rare enemies;
- world modifiers;
- encounter modifiers;
- expected reward category;
- expiry;
- prerequisites.

Use progressive disclosure.

The passive quick row should remain compact.

Detailed information belongs in the expanded strategic view.

# Targeting Controls

The normal HUD should show the current hero targeting policy near the
character/health/ability information.

Initial prototype choices:
- Closest;
- Strongest;
- Weakest.

Working controller concept:

L1 + face button
→ quick targeting choice.

Later targeting should support:
- composable priorities;
- targeting presets;
- support tactics.

Detailed targeting/tactics configuration belongs in an expanded panel, currently
associated with D-pad Down.

# Controller-Friendly UI Principle

Critical information and actions must be reachable through focus navigation.

Mouse hover may provide additional PC convenience but must never be required to:
- understand critical information;
- select an encounter;
- configure essential behaviour.

Current conceptual expanded-panel directions:

D-pad Right
→ world / encounters / schedule

D-pad Down
→ targeting / tactics

D-pad Left
→ character / build

D-pad Up
→ strong candidate for Pending Rewards

These bindings are working concepts rather than final controller mappings.

# Run Resources and Pending Rewards

Bottom-right remains the preferred passive location for:
- run currency;
- future collected resources;
- Pending Rewards.

Pending Rewards should receive stronger visual priority than passive currency
because they represent an actionable decision.

The normal HUD should communicate reward availability without forcing an
immediate interruption.

# Reward UI
Include:
- pending reward indicator
- voluntarily opened reward UI
- combat pause
- travel presentation
- reward acquisition order
- ability evolution presentation
- rarity feedback.

# Hub and Menus
Record options:

### 2D Frontend

### Physical Hub

### Hybrid

Do not choose.

Document that underlying persistent systems should not depend on presentation choice.