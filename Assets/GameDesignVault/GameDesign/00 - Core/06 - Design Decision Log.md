Whenever we actually commit to something:

```
## 2026-08-29 — No extraction mechanics

### Decision
Resources earned during an expedition remain owned after death.

### Reason
The loss condition is the temporary run build ending.
The game should not create extraction-looter tension.

### Implications
- Material inventory persists.
- Death does not remove earned materials.
- Reward economy requires long-term resource sinks.
```

This might become one of the most valuable documents in the entire repository.

## 2026-09-06 — Encounter generation and forecasting are separate

### Decision

The system that selects / creates encounters is conceptually separate from the
player-facing schedule that forecasts upcoming expedition events.

### Reason

Encounter generation must reason about escalation, intensity, encounter pools
and placement, while the schedule exists primarily to make future information
actionable to the player.

### Implications

- Scheduler UI does not construct encounter gameplay.
- Encounter generation can evolve without rewriting forecast presentation.
- Both systems consume shared expedition state.


## 2026-09-06 — Always provide three quick encounter options

### Decision

Normal expedition play should maintain at least three selectable encounter
opportunities.

The passive HUD exposes three quick encounter slots, defaulting to the three
oldest selectable encounters.

### Reason

Three options provide meaningful route choice while mapping cleanly onto a
controller face-button layer without overwhelming the passive HUD.

### Implications

- Encounter supply must backfill completed / expired encounters.
- More than three encounters may exist in the wider strategic view.
- Quick slots are a presentation layer rather than the complete world state.


## 2026-09-06 — Encounter redirection responds immediately

### Decision

Selecting another encounter immediately changes the hero's travel directive and
drops autonomous targets belonging to the previous encounter.

Enemies from the abandoned encounter are not forced to disengage.

### Reason

Player commands should produce immediate visible response, consistent with
manual ability activation and future combat cancellation principles.

### Implications

- Encounter membership must become identifiable at runtime.
- Pursuing enemies can be dragged into other encounters.
- Encounter stacking becomes intentional player expression.


## 2026-09-06 — Selected or engaged encounters do not normally expire

### Decision

An unselected/unstarted encounter may disappear when its expiry ends.

Once selected or engaged, normal expiry no longer removes it.

### Reason

Selecting an expiring encounter should act as an intentional commitment rather
than allowing the player's chosen objective to disappear while travelling or
fighting.

### Implications

- Encounter lifecycle needs a commitment concept.
- Expiry presentation must distinguish claimable and committed encounters.


## 2026-09-06 — World modifiers do not rewrite committed combat

### Decision

New world modifiers may affect ambient actors and encounters that have not yet
been engaged.

They should not normally retroactively alter actors already committed to combat.

### Reason

World-state changes should be predictable and should influence future decisions
rather than unexpectedly rewriting the fight already underway.

### Implications

- Encounter generation should snapshot relevant modifiers before engagement.
- Ambient population needs an engaged/unengaged distinction.


## 2026-09-06 — Ambient activity provides default autonomous travel

### Decision

When the player has no selected encounter, the hero should follow ambient
activity through designer-authored routes rather than becoming stationary.

### Reason

The game remains an autobattler even when the player chooses not to issue
strategic commands.

### Implications

- Ambient route authoring becomes a future level-design primitive.
- Random ambient scatter should not determine travel direction.

## 2026-09-07 — Expeditions use an accelerated world clock

### Decision

Strategic expedition design should support a configurable accelerated 24-hour
world clock spanning multiple expedition days.

World Time provides a readable coordinate for schedule events and daily rhythm.

It remains separate from Escalation and current Intensity.

### Reason

A world clock makes long-range scheduling easier to understand, gives lengthy
expeditions a stronger sense of progression and allows recurring world/ecology
rhythms to become learnable player information.

### Implications

- scheduler events can reference world timestamps;
- repeated days can contain familiar rhythms at increasing Escalation;
- simulation speed / strategic slow-motion should affect world-clock progression;
- event identity must include day as well as time-of-day;
- the clock can later support visual/environment/ecology systems without those
  being required by the initial prototype.