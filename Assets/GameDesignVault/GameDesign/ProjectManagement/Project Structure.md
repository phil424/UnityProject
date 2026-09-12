**Status: Canonical**

This document defines the intended organisation of the MiniCrawler Unity project
and embedded GameDesign vault.

It answers:

> Where should a new file go?

It describes ownership and organisation only.

Runtime architecture remains documented in the relevant Technical Design files.

# Unity Project

Primary runtime root:

`Assets/Playground/MiniCrawler/`

## Abilities

`Abilities/`

Generic ability infrastructure belongs under:

`Abilities/Core/`

Concrete ability behaviours remain under:

`Abilities/`

Do not create one folder per ability unless concrete content volume justifies it.

## Camera

`Camera/`

Camera-follow and camera-specific gameplay presentation.

## Combat

`Combat/`

Combat primitives and runtime combat-domain components.

Examples:
- Health;
- damage;
- attacks;
- buffs;
- forced motion;
- combat telemetry.

## Ecology

`Ecology/`

Ambient world-population and ecology behaviour.

## Encounters

`Encounters/`

Portable encounter definitions, encounter runtime state, encounter direction,
phase/rule behaviour, site bindings and encounter-domain commands/actions.

Do not place Workshop UI/tool orchestration here.

## Expedition

`Expedition/`

Expedition-owned strategic state such as World Time, schedules and World Events.

## Movement

`Movement/`

Actor movement intent / forced-motion primitives.

## Progress

`Progress/`

Progression is grouped by lifetime / responsibility.

### Progress/Persistent

Session/persistent progression that survives expedition boundaries.

Current prototype persistence is in-memory for the Play session.

### Progress/Run

Expedition-owned build/state and the seams that apply run progression to runtime
actors.

Examples:
- `RunState`;
- `RunProgress`;
- `RunBuild`;
- runtime ability/build applicators.

### Progress/Setup

Pre-expedition party/setup configuration.

### Progress/Rewards

Reward definitions, pending reward choices, offer generation and run-upgrade
presentation data.

Reward content should remain generic rather than rebuilding separate pipelines
for every reward type.

## Spawning

`Spawning/`

Generic spawn groups and spawn-source primitives.

## Systems

`Systems/`

High-level simulation processors and runtime directors.

Current organisation:

### Systems/Combat

Cross-actor combat simulation systems such as:
- targeting;
- automatic combat;
- ability updates;
- healing.

### Systems/Movement

High-level actor movement / avoidance processing.

### Systems/Flow

Run / level lifecycle directors.

### Systems/Expedition

World Clock and World Event runtime systems.

Shared systems that genuinely cross those boundaries may remain directly under
`Systems/`.

Current example:

`SimulationPause.cs`

## Tools

`Tools/`

Development-only / designer-facing tooling.

Tool-specific implementations should use their own subfolder.

Current tool areas:

`Tools/EncounterWorkshop/`
- encounter authoring and controlled encounter testing.

`Tools/Debug/`
- developer-only runtime diagnostics / controls.

Tool code may consume runtime architecture.

Runtime gameplay code should not depend on developer-tool implementations.

## UI

`UI/`

Player-facing UI code is grouped by responsibility.

### UI/Combat

Live combat presentation.

Examples:
- abilities;
- party health;
- damage presentation;
- combat telemetry;
- world-space health bars.

### UI/Encounter

Presentation of currently-running encounter state.

Examples:
- Active Encounter rows;
- encounter start/completion announcements.

### UI/Strategic

Strategic expedition HUD and destination/navigation presentation.

Examples:
- minimap;
- quick encounter choices;
- strategic HUD;
- slot symbols.

### UI/Progression

Setup, upgrades, rewards and run-build progression presentation.

### UI/Flow

High-level game-flow presentation/orchestration.

### UI/Common

Small shared UI primitives that do not belong to one presentation domain.

# Assets

Primary content root:

`Assets/Playground/MiniCrawler/Assets/`

## Data

`Assets/Data/`

ScriptableObject gameplay/content definitions.

Current domains include:
- Abilities;
- Encounters;
- Enemies;
- Party;
- Run Rewards;
- Run Upgrades;
- World Events.

Prefer domain subfolders over a flat asset collection.

## Prefabs

`Assets/Prefabs/`

Prefabs are grouped first by broad responsibility.

Current organisation:

- `Abilities/`
- `Actors/Party/`
- `Enemies/Zombies/`
- `UI/Combat/`
- `UI/Encounter/`
- `UI/Progression/`

Add a new category only when multiple related assets justify it.

Do not create deeply nested single-file folders merely for theoretical purity.

# Scenes

Playable/test scenes remain under:

`Assets/Scenes/`

Current primary scenes:

- `PlaygroundScene.unity` — normal expedition/playground;
- `EncounterWorkshop.unity` — encounter authoring/testing environment.

# GameDesign Vault

Canonical documentation root:

`Assets/GameDesignVault/GameDesign/`

## Development Rules

`Development Rules.md`

is the canonical entry point.

Focused rule documents live under:

`Development Rules/`

Current rulebooks:
- Workflow and Validation;
- Architecture and Lifetimes;
- Abilities and Buildcraft;
- Unity Authoring;
- UI Authoring;
- Documentation.

Do not recreate a large monolithic rules file.

## 00 - Core

Current high-level design authority:
- Game Vision;
- Design Pillars;
- Core Gameplay Loop;
- Progression Layers;
- Terminology;
- Open Design Questions;
- Design Decision Log.

The canonical navigation page is:

`00 - Game Design Index.md`

## Design

Current game-design documents.

Technical architecture lives under:

`Design/Technical/`

Technical documents should use meaningful domain folders when several documents
share that domain.

Current grouped domains:

`Design/Technical/Encounters/`

`Design/Technical/Expedition/`

Cross-cutting documents may remain directly under `Design/Technical/` rather
than creating one-file folders.

## ProjectManagement

Development planning, project organisation and reusable documentation templates.

Current:
- Roadmap;
- Project Structure;
- Templates.

## Archive

`Archive/`

Historical/deprecated design material retained for context.

Archive documents are not sources of current design authority.

Do not link archived notes from the primary Game Design Index unless historical
context is explicitly useful.

# Structural Rules

## Prefer Responsibility Over File Type

Do not create broad folders such as:

`Scripts/`
`Misc/`
`Managers/`
`Components/`

when a meaningful domain already exists.

## Avoid Premature Depth

A folder should normally exist because several related files benefit from being
grouped.

Do not produce deep trees containing one file per folder without a concrete
navigation benefit.

## Preserve Unity GUIDs During Moves

Move Unity assets through Unity's Project window.

Do not recreate an asset solely to move it.

Existing `.meta` files / GUIDs must remain attached to the asset so serialized
references remain valid.

## Folder Moves Do Not Require Namespace Changes

Namespace architecture and project-folder organisation are related but are not
required to mirror each other.

Housekeeping moves should not trigger broad namespace rewrites unless there is a
separate architectural reason.

## Behaviour-Preserving Housekeeping

A housekeeping step should not silently include gameplay architecture changes.

Prefer:

move
→ compile
→ validate
→ continue.

Structural refactoring and behavioural refactoring should remain separate
whenever practical.

## Automated Tests

The project does not currently maintain a standing automated/EditMode test
assembly.

Default validation follows `Development Rules.md`:
- short focused manual validation;
- automated tests only when explicitly requested or justified by a concrete
  high-risk need.

Do not preserve obsolete runtime compatibility APIs solely to support retired
test code.