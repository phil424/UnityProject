PROJECT: ECS Unity Tutorial / MiniCrawler
DATE CONTEXT: September 2026

==================================================
CORE INSTRUCTIONS — ALWAYS FOLLOW
==================================================

1. Use the latest numbered `Complete.zip` supplied by the user as the authoritative
   codebase.

2. Before giving implementation instructions, inspect the latest snapshot rather
   than relying on older chat descriptions when exact code/files matter.

3. Always provide comprehensive, detailed instructions showing the user how to do
   the work locally.

4. Never complete the Unity/project work for the user.

5. Always follow the Development Rules contained in the latest codebase.

6. The codebase and embedded GameDesign vault are authoritative over old chat
   summaries if they disagree.

7. Work milestone-by-milestone and in small, reviewable increments.

8. Preserve existing working architecture wherever practical. Avoid speculative
   rewrites and parallel replacement systems.

9. Give explicit file paths and precise Unity Editor instructions.

10. Every meaningful implementation step should begin with:
    - Goal
    - Deliberately NOT included
    - Files CREATE
    - Files MODIFY
    - Files DELETE
    - Unity/authoring work contract where relevant

11. End implementation steps with focused manual validation / acceptance criteria.

12. Automated tests are NOT the default. The project intentionally removed its
    test framework. Prefer focused manual validation unless the user explicitly
    requests automated tests.

13. Take Complete.zip snapshots at meaningful progression/checkpoint points rather
    than after every tiny change.

==================================================
CURRENT AUTHORITATIVE STATE
==================================================

The user has now uploaded:

`3.0KComplete.zip`

This should be treated as the latest intended authoritative snapshot in the new
chat once inspected.

It contains the completed 3.0K Region Route Geography / Encounter Authoring work
and also contains a collection of art/sprite sheets supplied by the user for the
next production phase.

IMPORTANT:
Inspect the actual `3.0KComplete.zip` before giving exact implementation
instructions, especially:
- current code structure;
- Development Rules;
- GameDesign documentation;
- supplied sprite/art assets and their layouts/import state.

Do not prescribe sprite slicing/import settings until the actual supplied sheets
have been inspected.

==================================================
WORKFLOW / DEVELOPMENT RULES
==================================================

The project uses a living embedded Obsidian-style GameDesign vault:

`Assets/GameDesignVault/GameDesign/`

This is canonical for:
- Design truth
- Technical architecture
- Development Rules
- Project roadmap
- Authoring documentation

Development rules are split into focused rulebooks with a concise entry point.

Meaningful architecture/design changes should update documentation alongside
implementation.

Proactively suggest small documentation/rule changes when repeated friction
reveals a missing rule, but do not rewrite rulebooks unnecessarily.

==================================================
C# FORMATTING RULES
==================================================

Use compact, conventional Visual Studio / Rider-style C#.

Default to horizontal readability.

Prefer ordinary:
- assignments;
- method calls;
- signatures;
- member access;
- lambdas;
- simple boolean conditions

on one line when readable.

Do NOT vertically explode code just because it is being shown in chat.

Roughly 120–140 characters is an acceptable working range when still readable.

Prefer:

    AmbientRouteNode start = routeGraph.FindNearestNode(leader.transform.position);

over:

    AmbientRouteNode start =
        routeGraph.FindNearestNode(
            leader.transform.position
        );

Prefer:

    if (node == null)
        return false;

for trivial guards.

Use braces where lifecycle-critical scope must be unmistakable, especially:
- spawning;
- encounter start/completion;
- progression/lifetime transitions;
- reward application;
- persistence;
- destruction/cleanup.

A previous real bug was caused by incorrectly scoped encounter-start code, so
lifecycle braces are deliberate.

Do not reformat unrelated existing code during feature work.

A gameplay change should produce a diff primarily describing the gameplay change.

Avoid formatting-only churn.

Match surrounding existing file style when editing.

==================================================
UNITY AUTHORING RULES
==================================================

Authoring tools should optimize for designer intent, not expose the complete
runtime architecture.

Use progressive disclosure.

Normal authoring should:
- show only actions relevant to the current task and selection;
- keep visible primary actions small (rough target <= 5);
- hide advanced metadata until requested;
- keep healthy validation compact;
- reveal detailed diagnostics only when needed;
- use concise contextual guidance/tooltips instead of permanent walls of text.

Runtime/data terminology does not have to be the designer-facing terminology.

Example:
`AmbientRouteNode` may be shown as `Route Point`.

Required internal hierarchy/roots should normally be created automatically by
common authoring operations.

Repair/setup commands belong under Advanced rather than normal workflow.

Generated objects/data must remain normally manually editable through the Unity
Inspector as an escape hatch.

Validation must be NON-DESTRUCTIVE.

Do not use `OnValidate()` or similar editor callbacks to silently:
- shrink serialized lists;
- remove empty entries;
- reorder data;
- delete invalid references.

Validation should REPORT issues. Cleanup is an explicit designer action.

This rule came from a real issue where `AmbientRouteGroup.OnValidate()` fought
Unity's list editing.

==================================================
PLAYER-FACING UI RULES
==================================================

For meaningful player-facing UI changes, define a UI Work Contract before Unity
actions.

Explicitly state:
- scene/prefab to modify;
- objects to modify;
- objects not to modify;
- hierarchy expectations;
- layout ownership.

Use Unity layout components for repeated/stacked UI content.

Avoid parallel serialized arrays.

Repeated widgets should generally duplicate a canonical child/template rather
than manually maintain matching arrays.

Fixed-count View Groups should validate their expected child count.

Follow an explicit prefab workflow when prefabs are involved.

Avoid mixing IMGUI and Canvas for player-facing UI.

Small internal developer/editor tools may use IMGUI.

==================================================
ARCHITECTURAL LIFETIMES
==================================================

Canonical ownership hierarchy:

Persistent / Meta
    ↓
Setup
    ↓
Expedition / Run
    ↓
Region / Level
    ↓
Encounter
    ↓
Runtime Actor

Examples:

Persistent / Meta:
- long-lived progression outside one expedition.

Expedition / Run:
- temporary run progression;
- temporary abilities/build;
- world time/day;
- expedition events/schedule.

Region / Level:
- scene-authored Route geography;
- local encounter sites/supply;
- ambient activity;
- Region runtime objects.

Encounter:
- availability;
- commitment;
- expiry;
- phases;
- spawn-group state;
- encounter membership/completion.

Runtime Actor:
- Health;
- cooldowns;
- ForcedMotion;
- combat target;
- buffs;
- navigation intents;
- temporary action state.

Keep runtime/progression/presentation/debug responsibilities separate.

Do not create giant manager classes or giant universal enums.

Prefer composable data and focused systems.

==================================================
ROUTE / REGION ARCHITECTURE
==================================================

A Route is persistent geography.

A Route is NOT a `LevelEncounter`.

Current route model includes:

- `AmbientRouteNode`
  spatial Route Point.

- `AmbientRouteSegment`
  first-class bidirectional connection between two Nodes.

- `AmbientRouteGroup`
  exact authored collection of Nodes/Segments.

- `AmbientRouteGraph`
  Region-owned graph with validation/pathfinding.

- reusable `RouteTagDefinition` assets
  semantic geography such as:
  - Village
  - Road
  - Well
  - Open
  - Narrow

GROUP vs TAG:

Group:
"these exact authored pieces"

Example:
Village Well Ring.

Tag:
"this geography has this semantic quality"

Example:
Road / Village / Narrow.

Do NOT replace Route Tags with a giant hardcoded enum/string soup.

Tags should remain data-authored assets.

Nodes/Segments may inherit Tags from containing Groups.

Segments do NOT automatically inherit arbitrary semantic Tags from endpoint
Nodes because that becomes ambiguous at intersections.

==================================================
REGION AUTHORING TOOLS
==================================================

3.0K5C simplified the previous giant Route Geography tool.

Primary tool:

Tools
→ MiniCrawler
→ Region Authoring

It has focused modes:

1. Route
2. Ambient
3. Encounters

Normal use only shows the selected task.

Advanced Route Tags/repair actions are hidden under Advanced.

Separate advanced compatibility tool:

Tools
→ MiniCrawler
→ Encounter Placement Rules

`RegionAuthoringWindow` handles the scene-authoring workflow.

`EncounterPlacementWindow` handles reusable Encounter Definition placement
rules/compatibility.

Do not rebuild another giant all-features-at-once scrolling editor window.

==================================================
AMBIENT ACTIVITY
==================================================

The old per-node `AmbientSpawnPoint` system was retired.

Current system:

`AmbientActivityArea`

An Area owns:
- Route footprint:
  - Nodes
  - Segments
  - Groups
- one or more ActorDefinition/count population entries;
- Spawn Scatter;
- Engagement Distance;
- Wander Distance;
- Wander Pause Min;
- Wander Pause Max.

When usable Segments exist, spawn positions are sampled along Segments.

Segment selection is length-weighted.

Nodes are fallback point-like spawn geography.

The Area GameObject Transform is organisational; Route footprint owns spatial
meaning.

Ambient actors remain separate from formal Encounters.

Ambient enemies:
- spawn disengaged;
- wander locally around their individual spawn/home point;
- use `ActorNavigationIntent`;
- engage when the party approaches;
- engagement clears ambient wander navigation immediately;
- after engagement they behave as normal hostile actors;
- currently do not return to ambient wandering after engagement.

Zombie Surge also spawns through compatible Ambient Activity Areas.

==================================================
ENCOUNTER ARCHITECTURE
==================================================

`LevelEncounter`
is runtime/scene encounter identity and lifecycle.

Encounter phases are represented by `LevelEncounterPhase`.

Spawn groups/sources belong to the Encounter authoring hierarchy.

Encounter membership remains explicit so physically overlapping Encounters do
not merge completion state.

Encounter Definitions are portable content.

Current `EncounterDefinition` supports:
- identity;
- phases;
- groups;
- spawn content;
- Trigger rules;
- ordered Action rules;
- route-placement compatibility metadata.

Current Trigger concepts include:
- Party Proximity;
- Encounter Started;
- Delay After Encounter Started;
- Phase Started;
- Delay After Phase Started;
- Phase Cleared.

Current Action concepts include:
- Activate Encounter Combat;
- Begin Encounter Spawning;
- Start Phase;
- Begin Spawn Group;
- Activate Spawn Group;
- Complete Encounter;
- Raise Signal.

Do not replace meaningful encounter behaviour with giant universal enum/query
systems.

==================================================
ENCOUNTER ROUTE SITES
==================================================

`EncounterRouteSite`
bridges a formal Encounter to Region Route Geography.

A Site may occupy:
- explicit Nodes;
- Segments;
- paths/groups through multiple refs.

Site semantic Tags are derived from its Route footprint.

Do not duplicate Route Tags manually onto every Site.

Traversal-start behaviour is Site-owned because it describes placement behaviour,
not portable Encounter Definition content.

A Site may opt into:

`Start On Route Traversal`

Explicit encounter travel:
- crossing another eligible Site starts that Encounter;
- selected strategic directive remains unchanged;
- party continues toward selected destination;
- enemies from crossed Encounters may pursue;
- selected destination is not traversal-started and retains its own final local
  start semantics.

Ambient exploration:
- crossing eligible Site starts the formal Encounter;
- passive ambient navigation pauses;
- normal Encounter/combat resolves;
- previous ambient route journey resumes afterward.

Strategic redirection can abandon that ambient journey; already-started enemies
may remain active/pursue.

==================================================
ENCOUNTER COMPATIBILITY
==================================================

Encounter Definitions may explicitly opt into automatic placement.

Current portable placement metadata:

- Allow Automatic Placement
- Required Route Tags
- Preferred Route Tags
- Forbidden Route Tags

Hard compatibility:

automatic placement enabled
AND
all Required Tags present
AND
no Forbidden Tag present.

Preferred Tags do NOT invalidate a Site.

They provide soft fit/ranking among otherwise-compatible Sites.

Compatibility compares:

EncounterDefinition requirements
+
EncounterRouteSite effective Tags
↓
EncounterRouteCompatibility result.

The compatibility query can collect/rank compatible Sites.

It does NOT itself schedule or place encounters.

Bespoke/fixed Encounter content may keep automatic placement disabled.

==================================================
ENCOUNTER WORKSHOP
==================================================

The Workshop is an isolated real Encounter runtime harness.

It supports:
- rapid replay/reset;
- deterministic replay;
- editable Encounter Draft;
- Definition Save/Load;
- arbitrary phases/groups;
- Trigger/Action rules;
- Approach;
- Threshold;
- On Top;
- Arrive While Pursued;
- Spawn On Arrival;
- Pre-Spawn From Start;
- Test Build;
- route-context replay.

Current Route Context introduced in 3.0K6:

Entry → Core → Exit

using real:
- AmbientRouteGraph;
- Nodes;
- Segments;
- Group;
- EncounterRouteSite;
- EncounterDirectionController.

Context modes:
- None
- Point
- Segment
- Path
- Route Group

The Workshop may temporarily override the runtime footprint of its
`EncounterRouteSite`.

Serialized fallback Site geography remains unchanged.

Workshop semantic roles:
- Entry
- Core
- Exit

are currently TEST-CONTEXT roles only.

Do NOT persist them into EncounterDefinition Spawn Sources yet.

Spawn Sources remain Encounter-anchor-relative.

Workshop compatibility preview is informational and does not block deliberate
testing of incompatible placements.

==================================================
EXPEDITION / STRATEGIC FOUNDATIONS ALREADY BUILT
==================================================

Completed strategic foundations include:

3.0A
Encounter directive & movement arbitration.

3.0B/C
Expedition lifetime, World Clock, temporary-vs-persistent progression bridge.

3.0D
Quick Encounter Choices.

3.0E
Ambient Route Navigation.

3.0F
Forecast proof.

3.0G
Encounter Supply transition/reuse.

3.0H
First World Event: Zombie Surge.

3.0I
Passive Strategic HUD.

3.0J
Encounter Workshop/Definition authoring sequence.

3.0K
Region Route Geography, Route Sites, traversal encounters, semantic tags,
compatibility, Ambient Activity authoring, simplified Region authoring,
Workshop Route Context.

==================================================
DOCUMENTATION MODEL
==================================================

The project now needs THREE documentation layers:

DESIGN
"What should the game be?"

TECHNICAL
"How is it built and why?"

GUIDES
"How do I actually use it?"

New canonical area:

`Assets/GameDesignVault/GameDesign/Guides/`

Recommended structure:

Guides/
├── 00 - Authoring Guides Index.md
├── Tutorials/
└── How-To/

Tutorial:
guided end-to-end learning workflow producing a known result.

How-To:
focused task-oriented instruction for one practical task.

Authoring documentation should normally include:

- purpose;
- exact Unity/menu/asset location;
- prerequisites;
- what it creates/modifies;
- quick-start workflow;
- detailed step-by-step instructions;
- concrete project example;
- field meanings;
- expected result;
- validation;
- common mistakes/troubleshooting;
- manual Inspector escape hatch;
- related Design/Technical docs.

A designer should not need to inspect source code to discover how to use an
approved authoring workflow.

Whenever an authoring workflow materially changes, assess:

- Design Documentation Impact
- Technical Documentation Impact
- Authoring Guide Impact

Update relevant guides in the same development step.

==================================================
CURRENT ROADMAP RESCOPE
==================================================

3.0K — Region Route Geography & Traversal
STATUS: COMPLETE

K closes at K6.

The previously planned K7 playable demo was moved later because a production
readiness review identified missing foundations.

NEW ORDER:

3.0L — Authoring Documentation & Production Guides
3.0M — Animated Combat & Action Authoring
3.0N — Character Abilities & Combat HUD
3.0O — First Playable Demo
3.0P — Expanded Strategic Planner & Expedition Integration

Then later:
3.1 Persistent Equipment Progression Trees
3.2 Data-Driven Encounter Generation + Resolution
3.3 Escalation / Encounter Mutation / Intensity
etc.

==================================================
3.0L — AUTHORING DOCUMENTATION & PRODUCTION GUIDES
==================================================

Purpose:

Make approved authoring workflows usable without reading source code or
reconstructing them from Technical docs.

L1
Authoring Documentation Standard & Guide Index.

L2
Region Authoring Tutorial & How-To Pack:
- Region Authoring;
- Route Points/Connections/Groups;
- Route Tags;
- Ambient Activity;
- validation/diagnostics.

L3
Encounter Authoring Tutorial & How-To Pack:
- Encounter Workshop;
- Encounter Draft;
- phases;
- spawn groups;
- Trigger/Action rules;
- Save/Load/Save As;
- Route Context;
- Site binding;
- Placement Rules;
- compatibility preview.

L4
Expedition Content How-To Pack:
- forecast/schedule;
- Encounter supply;
- World Events;
- Zombie Surge;
- validation/troubleshooting.

Combat/animation and ability guides should be written when those new systems are
implemented rather than documenting hypothetical workflows now.

==================================================
3.0M — ANIMATED COMBAT & ACTION AUTHORING
==================================================

This is a major upcoming system.

The user has supplied sprite/art sheets in `3.0KComplete.zip`.

FIRST ACTION:
inspect those actual sheets before prescribing the detailed Unity import/slicing
pipeline.

Do not assume:
- frame size;
- row/column layout;
- directions;
- animation lengths;
- whether weapons are baked in;
- number of states;
- pixels-per-unit.

Core architectural principle:

GAMEPLAY TIMING IS AUTHORITATIVE.

Animation/presentation follows gameplay action timing.

Do NOT make animation events secretly own combat/damage.

Current basic combat is prototype-level:
attack cooldown ready
+
target in range
↓
DamageResolver immediately.

Target M architecture:

target in range
↓
begin combat action
↓
WINDUP
↓
IMPACT
↓
DamageResolver
↓
RECOVERY
↓
next autonomous decision.

Attack speed should coherently scale:
- gameplay timing;
- presentation playback.

Do not allow damage cadence and animation cadence to drift apart.

M1 — Sprite Presentation Foundation
- inspect/import sprite sheets;
- actor Visual Root convention;
- reusable animation/presentation data;
- Idle/Move proof;
- facing if supported by supplied art;
- simulation pause/speed compatibility;
- swappable actor presentation.

M2 — Data-Driven Combat Action Timeline
- Windup;
- Impact;
- Recovery;
- action locking;
- damage at authored impact;
- Attack Speed scaling;
- interrupt seam for abilities.

M3 — Weapon Combo Authoring
- ordered basic attack sequence;
- multiple combo steps;
- per-step timing;
- damage coefficient;
- animation/presentation;
- reset/progression behaviour;
- different weapon cadence without rewriting AutoCombat.

M4 — Hit Reactions, Death & Presentation Cues
- hit presentation;
- gameplay death separate from visual cleanup lifetime;
- death animation;
- presentation/effect cue seam;
- room for blood splashes;
- weapon swipe effects;
- impact effects;
- dust/etc.

Do not scatter `Instantiate(effectPrefab)` throughout gameplay systems.

Use a clean presentation cue seam.

Important existing issue:
`Health.Die()` currently destroys actors immediately in the prototype.

That must eventually be separated into:
- immediate gameplay death;
- delayed visual/presentation cleanup
so encounter completion/rewards/targeting remain correct while death animation
can finish.

M5 — Combat Authoring Workflow & Guides
- new animated combatant tutorial;
- sprite import guide;
- animation set guide;
- basic attack authoring guide;
- combo authoring guide;
- combat presentation effects guide.

==================================================
ABILITY / ACTION INTEGRATION PRINCIPLE
==================================================

Do NOT create one animation system for basic attacks and another unrelated one
for abilities.

The action/presentation foundation created in M should be reusable by abilities.

Examples:

Whirlwind
→ action presentation
→ gameplay effect at intended timing
→ optional swipe/wind cue
→ recovery.

Rage
→ cast/windup presentation
→ concrete Rage behaviour
→ effect cue.

Charge
→ action presentation
+
forced/movement-duration gameplay
+
contact effects.

Keep meaningful ability mechanics concrete.

Generic infrastructure owns:
- ownership;
- configuration;
- execution timing/presentation seams.

Concrete abilities own their actual mechanics.

Avoid turning all abilities into a giant generic effect graph.

==================================================
3.0N — CHARACTER ABILITIES & COMBAT HUD
==================================================

Confirmed character foundation:

EVERY CHARACTER SHOULD HAVE:

- Unique / Innate
- Utility
- Sustain

These are CHARACTER/LOADOUT ROLES.

Do NOT put a permanent "Utility/Sustain/etc" identity onto
`AbilityDefinition`.

The slot/loadout owns the role.

Abilities remain independent of acquisition source.

Additional run-earned abilities remain generic and unrestricted by the three
core roles.

Current implementation before N is incomplete:
- Punchy has only part of the intended starting ability set.
- Healy does not yet have the complete starting core ability set.
- current AbilityHUD is a simple generated list rather than the intended
  wireframe.

N1
Ability Action/Presentation integration using existing representative abilities
such as Whirlwind/Rage/Charge.

N2
Explicit Character core ability roles.

N3
Three starting core abilities for every currently playable Character.

N4
Combat Ability HUD wireframe pass.

Desired broad UI semantics:

core cluster:
- Character portrait / health;
- visually central Unique ability;
- Utility;
- Sustain.

additional acquired abilities:
- grow outward/upward from the core cluster;
- do not hard-cap the character at three abilities.

Use the player-facing UI Work Contract.

N5
Ability authoring/tutorial/how-to documentation.

==================================================
3.0O — FIRST PLAYABLE DEMO
==================================================

This is the major production/content milestone.

Target:
approximately 10–15 minute meaningful expedition.

By the time O starts we want:

Region authoring ✅
Encounter authoring ✅
Ambient authoring ✅
Route compatibility ✅
Workshop ✅
Authoring tutorials/guides ✅
Sprite presentation ✅
Data-driven attacks ✅
Weapon combos ✅
Hit/death presentation ✅
combat presentation cues ✅
Ability action integration ✅
three core abilities per character ✅
improved combat HUD ✅

Then O should prioritize MAKING CONTENT rather than more infrastructure.

Proposed O structure:

O1
Demo Region Layout & Visual Foundation.

O2
Character & Enemy Content Pass.

O3
Encounter Content Pack.

O4
Ability & Buildcraft Content Pass.

O5
Expedition Pacing & World State.

O6
Demo Playtest, Guide Validation & Closure.

Success questions:

"Is the expedition enjoyable to watch autonomously and strategically intervene
in?"

and

"Can more Characters, enemies, attacks, abilities, Encounters and Regions now be
authored without another architectural rewrite?"

==================================================
3.0P — EXPANDED STRATEGIC PLANNER
==================================================

The former 3.0L Expanded Strategic Planner milestone has moved to 3.0P.

Do not build it before the first proper content/combat slice.

The playable demo should inform what strategic information actually deserves to
be exposed.

==================================================
ESCALATION / FUTURE ENCOUNTER GENERATION
==================================================

Preserve this future direction:

EncounterDefinition base
+
Resolution Context
  - Escalation
  - World Events
  - Region state
  - Intensity
↓
Encounter Modifiers
↓
Resolved Encounter Occurrence
↓
Runtime content

Never mutate the source EncounterDefinition.

Once an Encounter is committed, its resolved combat content should generally
remain stable even if escalation changes later.

Escalation should eventually support many levers, not merely +damage/+health:

- stats;
- population;
- Elite presence;
- composition;
- phase structure;
- timing;
- spawn rhythm;
- rules;
- overlap;
- enemy pools;
- ambient pressure;
- bosses;
- World Events.

Potential future model:
Escalation = available/max pressure.
Intensity = how aggressively the encounter spends it.

Do not implement this prematurely during M/N/O unless a concrete demo requirement
forces a small seam.

==================================================
USER WORKING PREFERENCES
==================================================

The user is a solo developer.

They prefer:
- visible/playable progress;
- flexible modular systems;
- expressive authoring;
- avoiding rewrites;
- designer-friendly tooling;
- precise step-by-step implementation guidance;
- literal Unity instructions;
- compact conventional C#;
- manual validation rather than large test suites.

They particularly liked the Route authoring tooling when it automated tedious
GameObject/component/reference setup while leaving resulting objects manually
editable.

Strong tooling philosophy:

Automate repetitive setup.
Do not hide the resulting data from the designer.
Do not require the designer to understand internal architecture for simple tasks.

Simulation/autonomous gameplay remains an important identity of the project.

==================================================
NEXT CHAT — IMMEDIATE ACTION
==================================================

The latest `3.0KComplete.zip` contains art/sprite sheets.

Before implementing 3.0M, inspect those actual art assets carefully and report:

- available sheets/assets;
- pixel dimensions;
- apparent frame layout;
- likely frame dimensions;
- animation/state coverage;
- directional coverage;
- character/enemy/weapon separation;
- whether sheets appear consistent enough for a shared importer/workflow;
- any irregularities that require per-sheet configuration.

Then formulate the recommended Unity art-import/presentation workflow based on
the actual supplied assets.

However, the ROADMAP says 3.0L is the immediate active milestone, so first decide
whether to:

A. perform the short L1 documentation foundation and Guides index first, then
   begin M1,

or

B. inspect the art immediately while implementing L1, since inspection does not
   alter the project.

Do not spend multiple large milestones documenting hypothetical M/N workflows
before those systems exist.

==================================================
IMPORTANT FINAL RULE
==================================================

When giving exact implementation instructions:
USE THE LATEST SNAPSHOT.

Memory/chat summaries provide continuity, but the latest numbered Complete.zip
and its embedded Development Rules / GameDesign vault are the source of truth.