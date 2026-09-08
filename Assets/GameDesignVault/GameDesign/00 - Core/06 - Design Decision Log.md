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

## 2026-09-07 — Party defeat ends the expedition

### Decision

Party defeat ends the active expedition.

The previous prototype behaviour allowing defeat → BetweenLevels → Continue is
superseded.

### Reason

World Time, escalation, temporary build progression and encounter momentum only
have meaningful stakes if expedition failure actually closes that attempt.

### Implications

- World Time resets on the next expedition.
- Temporary RunBuild progression is discarded.
- Pending temporary rewards are discarded.
- Persistent progression remains.
- normal level/region victory is no longer treated as expedition success.


## 2026-09-07 — Persistent and expedition progression are separate lifetimes

### Decision

Weapons, armour, permanent unlocks and future crafting progression belong to
persistent preparation.

Abilities acquired/levelled/evolved during an expedition and temporary build
upgrades remain expedition-owned.

### Reason

The player needs meaningful long-term growth without removing the fresh
buildcraft challenge from each expedition.

### Implications

- RunBuild must be seeded from persistent preparation.
- permanent rewards cannot live only inside RunState.
- equipment progression needs a persistent owner.


## 2026-09-07 — Apex defines expedition success

### Decision

Clearing the future Apex marks an expedition as successful.

After Apex the player may return or continue into endurance.

### Reason

Normal encounter/level victory needs to be distinct from actually beating the
expedition while still supporting effectively endless post-success play.

### Implications

- level clear != expedition victory;
- post-Apex death does not erase the previously achieved success;
- Apex can carry a meaningful permanent completion reward.

## 2026-09-07 — Explicit encounter travel and ambient travel use different combat priorities

### Decision

Explicit encounter travel suppresses normal autonomous combat targeting until
the selected encounter is reached.

Ambient fallback travel does not suppress combat.

Local combat interrupts ambient travel and the route resumes after combat ends.

### Reason

Player-issued strategic commands must produce immediate response, while ambient
navigation exists specifically to keep the autobattler active when the player
chooses not to intervene.

### Implications

- both behaviours reuse `ActorNavigationIntent`;
- encounter travel remains dominant over ambient combat;
- ambient roaming yields to nearby combat;
- ambient navigation resumes automatically after combat.

## 2026-09-08 — Expedition forecast is a plan, not an immutable script

### Decision

Forecast entries may resolve early or change when player actions make their
planned future state unnecessary.

### Example

Opening Horde completion may unlock Reinforcements before the forecasted
Reinforcement availability time.

The corresponding forecast entry then resolves early.

### Reason

The scheduler should communicate the expedition's current plan without making
player actions feel irrelevant.

### Implications

Future scheduling may:
- remove obsolete entries;
- replace planned events;
- move future events;
- react to encounter completion;
- react to escalation / intensity.

The player-facing forecast should remain trustworthy while still being dynamic.

## 2026-09-08 — World Events are run-owned state with domain-specific effects

### Decision

Active World Events belong to the expedition lifetime.

The scheduler may activate them, but concrete gameplay effects are implemented
by the systems that own the affected domain.

### Example

Prototype schedule
→ activates Zombie Surge.

ZombieSurgeAmbientEffect
→ increases ambient zombie population.

### Reason

World Events may affect very different gameplay systems.

A single central World Event effect switch would become difficult to extend and
would couple unrelated domains.

### Implications

- WorldEventDefinition primarily provides identity / presentation;
- WorldEventState owns active event lifetime;
- schedule owns event timing;
- individual systems react to relevant active events;
- World Events survive region transitions inside the same expedition;
- World Events reset when the expedition ends.


## 2026-09-08 — Ending a World Event does not rewrite generated actors

### Decision

When a World Event ends, already-generated actors or committed combat are not
automatically reverted or deleted.

### Example

Zombie Surge-generated ambient zombies remain after Zombie Surge expires.

No additional surge population is generated after expiry.

### Reason

World changes should influence future state without making existing combat
visibly mutate for purely systemic reasons.