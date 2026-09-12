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

## 2026-09-10 — Encounter phases are distinct from spawn batches

### Decision

Encounters may contain explicit ordered phases above the existing spawn-group
and spawn-batch layers.

Phase
→ meaningful encounter stage.

Spawn Group
→ owned enemy/spawn content.

Spawn Batch
→ timing/rhythm within that content.

### Reason

Meaningful encounter progression should not be encoded indirectly inside spawn
batch timing.

Explicit phases provide a future home for:
- encounter progress UI;
- named stages;
- objectives;
- delayed transitions;
- spectacle/actions;
- non-wave completion rules.

### Implications

Existing non-phased encounters remain supported.

Phase ordering is authored through hierarchy.

Phase completion is scoped to owned content rather than global enemy counts.


## 2026-09-10 — Phase pacing may advance on timer or early clear

### Decision

A phase may start its successor when either:
- its authored maximum delay is reached; or
- its owned content is cleared earlier.

### Reason

Strong builds should not be forced through unnecessary dead time, while weaker
builds should still experience the intended pressure of overlapping phases.

### Implications

Player power can alter encounter rhythm without dynamically rewriting enemy
stats.

Phase timers use scaled simulation time and therefore respect Slow, Fast and
Pause.

## 2026-09-10 — Encounter start is an explicit gameplay transition

### Decision

An encounter is considered Started only once its content has begun and combat is
active.

Pre-spawned dormant content alone does not count as encounter start.

### Reason

Encounter identity presentation and Active Encounter UI should correspond to
actual player commitment, not merely world visibility.

### Implications

- `LevelEncounter` exposes explicit Started state/event;
- reusable occurrences reset Started state;
- Availability and Started ordering remain distinct;
- Workshop Pre-Spawn scenarios do not announce before arrival.


## 2026-09-10 — Transient encounter announcements are independent stacked views

### Decision

Encounter start/completion presentation does not use one mutable global banner.

Each event creates its own short-lived presentation View inside a layout-driven
feed.

### Reason

Strategic redirection allows several encounters to start or complete in rapid
succession.

### Implications

- announcements may overlap;
- later events do not erase earlier ones;
- layout owns stacking;
- presentation uses unscaled UI time.

## 2026-09-11 — Workshop editing uses an explicit Draft

### Decision

Encounter Workshop editing occurs against a temporary
`EncounterWorkshopDraft`.

Changing Workshop controls does not directly mutate saved scene encounter
content.

The Draft is explicitly applied to runtime override seams when a scenario is
replayed.

### Reason

Encounter experimentation should be low-risk.

A designer must be free to try extreme values without accidentally replacing
the saved source encounter.

### Implications

The encounter authoring workflow distinguishes:

Saved / Source Content
↓
Editable Draft
↓
Runtime Preview.

J6 will add an explicit Save / Save As operation.


## 2026-09-11 — Draft changes apply on fresh replay

### Decision

Changing encounter Draft values does not mutate an encounter already in
progress.

The latest Draft is applied when the Workshop reconstructs the scenario.

### Reason

Encounter comparisons should begin from a consistent fresh runtime state.

This also avoids having to define ambiguous live-edit semantics for already
spawned actors, running spawn coroutines or active phases.

### Implications

The authoring panel reports when Replay is required.

Future specialised live-tuning may exist, but it is not the default encounter
authoring behaviour.

## 2026-09-11 — Encounter gameplay content becomes portable data

### Decision

The first reusable encounter-content artifact is
`EncounterDefinition`, a ScriptableObject.

It stores portable encounter gameplay configuration rather than scene/runtime
occurrence state.

### Reason

Workshop-authored encounters must be reusable without rebuilding their gameplay
configuration manually at every location.

### Implications

Encounter content can be authored through:

Workshop
→ Draft
→ EncounterDefinition.asset.

Scene encounters may consume these Definitions through a binding/application
seam.


## 2026-09-11 — Encounter Definition and Encounter Site remain distinct

### Decision

J6 does not treat an exported scene prefab as the primary encounter gameplay
artifact.

`EncounterDefinition`
owns portable gameplay content.

Scene encounter/site authoring continues to own physical placement and
geographic references.

### Reason

The same authored encounter recipe should eventually be usable at multiple
compatible world locations.

Fixed spectacles may still use prefabs/site-specific content where appropriate.


## 2026-09-11 — J6 uses structural compatibility rather than dynamic hierarchy generation

### Decision

The first Definition binding requires compatible scene phase/group/source
topology.

### Reason

Dynamic encounter-instance construction is a larger architectural problem and
should be informed by actual Workshop usage rather than solved speculatively.

### Implications

J6 proves portable content now.

Future Encounter Definition / Site / Runtime Instance work will remove or relax
the current compatibility limitation.

## 2026-09-12 — Advanced encounter behaviour uses composable Rules

### Decision

Portable encounter behaviour beyond the common phase progression path is
authored as:

Trigger
+
ordered Actions.

### Reason

Encounter variety should come from composing reusable behaviours rather than
creating a growing set of hardcoded encounter types.

### Implications

The first portable trigger vocabulary includes proximity, lifecycle, delayed
and phase-state triggers.

The first action vocabulary includes encounter activation, spawning, phase/group
control, completion and generic Signals.

The vocabulary may grow with concrete design needs, but should remain
domain-focused rather than becoming a giant universal effect system.


## 2026-09-12 — Encounter spectacle uses a generic Signal seam

### Decision

Rules may raise authored Signal IDs without directly owning VFX/audio logic.

### Reason

Encounter sequencing should be able to request moments such as a door burst or
ground eruption without coupling portable gameplay data to one scene's visual
objects.

### Implications

Future sites/presentation systems may bind Signals to:
- animation;
- VFX;
- audio;
- environment state;
- camera/presentation events.

Signal meaning remains site/presentation-owned.


## 2026-09-12 — Workshop sliders require precision input

### Decision

Meaningful Workshop numeric sliders pair snapped slider adjustment with direct
numeric entry.

### Reason

Pure free-moving sliders produced difficult-to-reproduce floating-point values
and made exact encounter authoring unnecessarily frustrating.

### Implications

Sliders remain useful for exploratory tuning while direct fields support exact,
repeatable authored values.

## 2026-09-12 — Encounter Definition owns phase topology

### Decision

Encounter phase count and runtime phase/group/source structure are no longer
required to exist ahead of time as scene GameObjects.

Encounter Definitions may materialize their runtime content beneath an encounter
site.

### Reason

Portable encounter authoring must allow designers to create genuinely new
encounters in the Workshop rather than merely reconfigure a fixed scene
template.

### Implications

A fresh encounter may begin with one simple phase and grow entirely through
data.

Definitions with different phase counts may use the same encounter site.

Existing scene-authored encounter content remains a supported fallback.


## 2026-09-12 — Authored start Rules own their approach boundary

### Decision

When an Encounter contains an authored Party Proximity start Rule, that Rule
owns encounter-start timing.

EncounterDirectionController navigates the party to the authored trigger region
but does not also force the encounter to start at its legacy arrival distance.

### Reason

The previous behaviour allowed the default 2.5m directive arrival threshold to
override authored proximity values.

### Implications

Without a start Rule:
→ legacy/default directive arrival starts the encounter.

With a Party Proximity start Rule:
→ the authored trigger decides when the encounter begins.