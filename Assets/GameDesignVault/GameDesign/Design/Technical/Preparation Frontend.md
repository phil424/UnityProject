# Preparation Frontend

**Status: Working / Implemented Foundation**

# Purpose

The Preparation frontend is the player-facing state between expeditions.

It combines persistent preparation with a live demonstration of the selected
Character's actual gameplay behaviour.

# Composition

Current composition:

Live World / Combat Showcase
~2/3 screen

Preparation UI
~1/3 screen

The live world is not a RenderTexture or fake Character preview.

It is the actual `PlaygroundScene` rendered by the normal Main Camera.

# Live Showcase

During `RunDirector.PreRun`:

`PreparationCombatShowcase`
↓
spawns the selected Character using the real Actor Definition
↓
applies current persistent progression
↓
makes the showcase Character effectively immortal
↓
uses `PreparationRouteWanderer` to travel authored Ambient Route geography

Enemies:

Ambient Route geography
↓
route-distributed spawn positions
↓
real enemy Actor prefabs
↓
`AmbientEnemy`
↓
local wandering
↓
proximity engagement
↓
normal targeting / movement / combat / damage / death presentation

Showcase actors are presentation-lifetime actors.

They are not owned by normal Encounter / Region reward lifecycle and do not
grant Expedition rewards, currency or Objective progress.

# Preparation UI

`GameFlowUI`
owns PreRun panel visibility.

`PreparationScreenUI`
owns:
- selected playable Character;
- Overview / Progression tabs;
- runtime Character selector;
- Start Expedition action;
- live showcase selection.

`PreparationCharacterOverviewUI`
owns read-only Character information.

`PreparationCharacterProgressionUI`
owns the current prototype persistent upgrade controls.

`PreparationCharacterButtonUI`
owns one runtime-generated Character selector entry.

# Character Selection

The Preparation frontend presents the authored
`PlayableCharacterRosterDefinition`.

Persistent ownership is resolved through `PersistentProgression`.

Current presentation states:

Starting / unlocked
→ visible and selectable.

Locked / visible
→ visible but unavailable;
→ displays `[LOCKED]`;
→ displays the authored unlock requirement;
→ displays current persistent requirement progress.

Progression-unlocked
→ visible and selectable;
→ displays an `UNLOCKED` acknowledgement.

Locked / hidden
→ omitted until ownership is earned.

The selector is rebuilt when Preparation becomes active so ownership changes
earned during an Expedition are reflected immediately on return.

`RunSetup` independently validates roster membership and persistent ownership, so
the frontend is not the authority for whether a Character may begin an
Expedition.

The current implemented roster contains:

Punchy
→ starting unlocked Character.

Ranger
→ visible while locked;
→ gameplay unlock requirement implemented;
→ persistent requirement progress currently exists beneath the frontend.

O1C2 adds the first functional presentation of that unlock requirement and its
current progress directly to the Character selector.

Legacy Support actors are not automatically treated as selectable playable
Characters.

# Expedition Transition

Preparation
↓
Start Expedition
↓
`RunDirector.BeginRun`
↓
Preparation panel disables
↓
showcase actors are cleaned up
↓
normal Expedition starts

Return / Defeat
↓
Run returns to `PreRun`
↓
Preparation frontend enables
↓
live showcase begins again

No scene load is currently required.

# Progression Tab

The Preparation Progression tab now presents Character Mastery.

Mastery entries are runtime-generated from the selected
`PartyMemberDefinition`.

The current Punchy implementation shows the first authored Mastery nodes and
their:

- effects;
- prerequisite state;
- resource costs;
- unlock state.

Legacy Weapon / Armour / Focus progression is no longer the player-facing
Preparation progression model.

# Future

Potential later work:

- larger roster presentation;
- richer Character information;
- final UI skin;
- showcase Ability behaviour;
- frontend audio / transitions;
- save-backed persistent progression;
- possible dedicated frontend scene if future lifecycle requirements justify it.