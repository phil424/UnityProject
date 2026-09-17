# Design Decision Log

**Status: Canonical**

This is the chronological record of design decisions that the project is
actively designing around.

Do not use this file as a duplicate technical specification.

Detailed behaviour belongs in the relevant design/technical document; this log
records the decision, reason and implications.

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

## 2026-09-14 — Route connections become first-class Region geography

### Decision

Region route authoring is composed from:

Route Nodes
+
Route Segments
+
Route Groups.

Connections are no longer represented only as raw Node-to-Node reference lists.

### Reason

Route connections now need to become meaningful Region geography capable of
supporting future encounter occupancy, strategic routing, Route Groups, semantic
tags and World Event interaction.

### Implications

Nodes represent meaningful positions.

Segments represent traversable relationships.

Groups identify exact authored collections of route geography.

Future semantic Tags remain separate from Groups.


## 2026-09-14 — Routes remain geography rather than Encounter occurrences

### Decision

A route, Route Segment or Route Group is not itself a `LevelEncounter`.

Formal Encounter occurrences may instead occupy route geography through future
Encounter Sites / footprints.

### Reason

Route geography persists for the Region lifetime.

Encounter state has occurrence lifecycle such as Available, Selected, Started,
Completed and Expired.

Combining those lifetimes would create ambiguous concepts such as a completed or
expired road.

### Implications

Future Encounter footprints may occupy:
- Nodes;
- Segments;
- connected paths;
- Route Groups.

Travelling toward one Encounter may cross other Encounter footprints.

Enemies from those encounters may continue pursuing, preserving existing
Encounter Membership and redirection behaviour.


## 2026-09-14 — Escalation uses encounter-content levers as well as statistics

### Decision

Long-term expedition Escalation should be capable of altering future encounter
composition and structure in addition to ordinary combat-stat scaling.

### Reason

Difficulty should become qualitatively more dangerous and varied rather than
only producing enemies with progressively larger numerical stats.

### Implications

Future Escalation levers may include:
- population;
- Elite frequency;
- composition;
- spawn rhythm;
- phase timing;
- additional phases;
- reinforcement behaviour;
- Encounter Rules;
- Encounter pools;
- ambient pressure;
- combat statistics.

Authored `EncounterDefinition` assets remain immutable base content.

A future encounter-resolution layer should create the modified occurrence.

The Encounter Workshop should eventually preview resolved encounters at chosen
Escalation / Intensity contexts.

## 2026-09-15 — Route geography uses editor-assisted scene authoring

### Decision

Route topology remains authored as scene-owned Nodes, Segments and Groups.

A dedicated Unity Editor authoring layer should automate common creation,
connection, grouping and validation work.

Generated route objects remain normal editable Unity objects.

### Reason

The 3.0K1 runtime representation is useful, but manually creating GameObjects,
adding components and wiring every relationship places unnecessary repetitive
work on the designer and is prone to reference mistakes.

Moving the entire spatial graph into a different data model would solve the
wrong problem.

The immediate problem is the authoring surface, not the runtime representation.

### Implications

The common workflow should be button/tool assisted.

Manual Inspector editing remains available as an advanced escape hatch.

Route validation should be non-destructive.

Runtime route code must not depend on UnityEditor tooling.

Portable concepts such as future Route Tags / Encounter requirements may still
use data assets when they have a concrete consumer.

## 2026-09-15 — Route traversal can start formal Encounters

### Decision

Traversal-enabled Encounter Sites may start when crossed during either explicit
Encounter travel or passive Ambient Route exploration.

The navigation context determines the party response.

Explicit Encounter travel:
- crossed Encounter starts;
- selected strategic directive remains active;
- party continues travelling;
- crossed enemies may pursue.

Ambient exploration:
- crossed Encounter starts;
- ambient navigation yields;
- party resolves the formal Encounter;
- the previous ambient journey resumes afterward.

### Reason

A Route Site should have a consistent spatial meaning regardless of why the
party entered that geography.

The important distinction is not whether the Site exists, but whether the player
currently has an explicit strategic navigation command.

This allows passive exploration to discover formal content naturally while still
supporting deliberate multi-Encounter strategic pulls.

### Implications

- `Start Encounter On Traversal` remains Site-owned;
- the selected destination is exempt from traversal start while travelling to it;
- ambient exploration can be interrupted by formal Encounter lifecycle, not only
  ordinary combat targeting;
- explicit strategic navigation remains higher priority than crossed Encounter
  combat;
- previously-started enemies may continue pursuing after redirection;
- Encounter Membership preserves independent completion during overlap.

## 2026-09-15 — Route semantics use data-authored Tags with Group inheritance

### Decision

Route Nodes, Segments and Groups may reference reusable `RouteTagDefinition`
assets.

Nodes and Segments inherit semantic Tags from Route Groups that explicitly
contain them.

Encounter Route Sites derive effective Tags from the geography in their
footprint rather than storing duplicate Site Tags.

### Reason

Encounter placement and future generation need semantic information such as:

- Road;
- Village;
- Well;
- Open.

A hardcoded enum would require runtime code changes whenever the geography
vocabulary expands.

Duplicating Tags onto every Site would create fragile parallel authoring data.

### Implications

- Route Tags are reusable data assets;
- Groups remain exact geographic collections;
- Tags remain semantic descriptions;
- Site effective Tags can be queried without duplicating data;
- Segment Tags do not automatically inherit endpoint Node Tags;
- future Encounter compatibility can consume Site effective Tags;
- validation remains non-destructive.

## 2026-09-16 — Encounter placement uses hard semantic constraints plus soft preferences

### Decision

Automatically placeable `EncounterDefinition`s describe geography suitability
through:

- Required Route Tags;
- Preferred Route Tags;
- Forbidden Route Tags.

Automatic placement is explicit opt-in.

### Reason

Encounter content needs to remain portable between Regions while still
expressing where it makes sense.

A simple yes/no Tag requirement is too restrictive for cases where several Sites
are valid but some are a better thematic/spatial fit.

### Implications

Required Tags:
- all must match.

Forbidden Tags:
- none may match.

Preferred Tags:
- affect fit/ranking only.

A compatibility query can return ranked Site candidates without choosing or
instantiating one.

Definitions that should remain bespoke may opt out of automatic placement.

Manual binding remains independent from generated-placement eligibility.

## 2026-09-16 — Ambient population is authored as Route-bound Activity Areas

### Decision

The original per-Node `AmbientSpawnPoint` prototype is replaced by
`AmbientActivityArea`.

An Activity Area owns:
- Route geography footprint;
- one or more ActorDefinition/count population entries;
- spawn scatter;
- engagement radius.

### Reason

Per-Node Spawn Points require repetitive scene wiring and treat ambient
population as isolated points rather than part of authored Region geography.

The Region Route system now provides enough spatial structure to author ambient
population more coherently.

### Implications

- ambient population remains Region / Level-owned;
- Activity Areas may occupy Nodes, Segments or Route Groups;
- Segment-backed Areas distribute actors along the Route;
- multiple Actor types can share one Area;
- the Area GameObject Transform is organisational rather than spatial authority;
- Ambient Activity remains separate from formal Encounter lifecycle;
- Zombie Surge affects compatible Activity Areas rather than fixed Spawn Points;
- the old `AmbientSpawnPoint` implementation is retired.

## 2026-09-17 — Ambient wandering uses actor navigation and one-way engagement

### Decision

Disengaged ambient enemies may wander locally around their spawn/home position.

Wandering uses `ActorNavigationIntent` and the normal `MovementSystem`.

When an Ambient enemy engages, its wander navigation is cleared and normal
combat targeting takes over.

Engagement does not currently revert to ambient wandering.

### Reason

Ambient actors should make the Region feel alive without introducing a parallel
movement implementation or a complex ecology state machine.

The existing navigation seam already provides movement arbitration, Simulation
Time behaviour and compatibility with combat takeover.

### Implications

- Ambient Activity Areas author wander radius and pause range;
- each actor owns runtime wander timing/destination state;
- the spawn position is the actor's local home;
- wandering remains local rather than traversing the whole Route Graph;
- Wander Radius `0` disables wandering;
- World Event-spawned ambient population inherits Area wandering settings;
- future migration/population-front systems remain separate work.

## 2026-09-17 — Region authoring uses task-focused progressive disclosure

### Decision

The accumulated Route Geography authoring surface is reorganised around three
designer tasks:

- Route Layout;
- Ambient Life;
- Encounter Placement.

Reusable Encounter Definition compatibility moves into a separate focused
Encounter Placement Rules tool.

Advanced semantic metadata, diagnostics and repair actions are hidden during
ordinary authoring.

### Reason

The underlying authoring systems became capable but the single scrolling window
exposed too many unrelated concepts simultaneously.

The designer should not need to understand the complete runtime/data architecture
to perform one ordinary content-authoring task.

### Implications

- normal Region authoring exposes only contextually useful actions;
- Route Tags remain available but are advanced authoring;
- Ambient population/behaviour is edited independently from Encounter placement;
- Definition compatibility remains available without occupying basic Region
  authoring;
- healthy validation becomes a compact status;
- detailed issues appear on demand;
- implementation terms may use simpler designer-facing labels;
- runtime/data architecture and manual Inspector escape hatches remain intact.