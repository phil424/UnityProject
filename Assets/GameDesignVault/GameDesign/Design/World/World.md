# Overview
- authored geography
- regions
- zones
- connected-world fantasy
- no requirement for procedural map layouts
- region identity
- world supports strategic hunting rather than merely linear progression.

# Regions and Zones
Define hierarchy.

```
World
└── Region
    ├── Zone
    ├── Zone
    ├── Zone
    └── Zone
```

Document that zones can have:
- enemy populations
- boss activity
- resource identity
- environmental modifiers
- connections to other zones.

Add example:

```
Ruined Valley
├── Graveyard
├── Village
├── Forest
└── Caverns
```

Clearly mark example names as placeholders.

# Traversal
Document three possible models.

### Direct Selection
Map selection leads quickly to destination.

### Autonomous Physical Travel
Player selects destination and character navigates automatically.

### Transition-Based Travel
Player selects destination and an authored travel sequence bridges the zones.

Current leading hypothesis:
>**Transition-based / fake-seamless travel.**

Open questions:
- Can events happen during travel?
- Can the player change destination?
- Is travel duration fixed?
- Can anything interrupt travel?
- Should travel ever involve combat?

# Transitions
### Goal
Create the perception of a coherent continuous world without requiring every zone to exist simultaneously.

### Potential Flow

```
Zone A
↓
Destination Selected
↓
Exit Sequence
↓
Travel Animation / Transitional Environment
↓
Pending Rewards / Schedule / Build Management
↓
Load / Prepare Zone B
↓
Arrival Sequence
↓
Zone B
```

### Advantages
- preserves pacing
- provides downtime
- hides loading
- simplifies world implementation
- gives rewards natural presentation time
- allows ecology to update
- makes authored transitions possible.

### Principle

> Seamlessness should be judged by player perception, not technical implementation.

# Map and Schedule
Include things the strategic map can eventually communicate:
- player location
- connected zones
- boss locations
- boss arrival timers
- upcoming threat increases
- upsurges
- environmental events
- boss migrations
- resource opportunities
- Apex forecast.

