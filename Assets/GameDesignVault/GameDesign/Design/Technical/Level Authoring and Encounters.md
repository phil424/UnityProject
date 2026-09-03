# Level Authoring and Encounters — Technical Design

Status: Working

Related Design:
- World/Expeditions
- World/Ecology
- Enemies
- Combat/Combat
- UI-UX

# Purpose

Provide a scene-authorable framework for creating regions and encounters without
hard-coding individual enemy types or locations into StageDirector.

# Core Separation

WHERE
LevelSpawnSource
Defines valid physical spawn positions/areas.

WHAT + SPAWN RHYTHM
LevelSpawnGroup
Defines actors, counts, delays, batches and spawn intervals.

WHEN TO ENTER THE WORLD
Spawn triggers / future level schedule.
Begins a group's spawn schedule.

WHEN TO ENGAGE IN COMBAT
Combat engagement.
Independent from whether an actor already exists in the world.

HOW TO ENTER
Spawn presentation.
Future door, ground-burst, portal, cave, etc. behaviour.

PLAYER-FACING IDENTITY
LevelEncounter.
Future authored concept representing a meaningful opportunity/threat shown to
the player independently from low-level spawn machinery.

# Important Invariants

Spawning and combat engagement are separate.

A spawned actor may exist without participating in combat.

Activating a group affects both existing members and members spawned later.

Unstarted authored groups are not the same thing as currently scheduled spawns.

Level completion must not depend only on the number of enemies currently alive.

Spawn presentation must not be encoded as a giant spawn-type enum.

Combat threat and reward/opportunity rarity are separate concepts.

# Future Runtime Flow

Level Schedule / Trigger
        ↓
BeginSpawning()
        ↓
LevelSpawnGroup
        ↓
LevelSpawnSource
        ↓
Spawn Presentation
        ↓
Actor exists
        ↓
Combat engagement may be inactive

Separate trigger / schedule event
        ↓
ActivateCombat()
        ↓
existing + future group members become combat-active

# Future Encounter Direction

Player selects LevelEncounter
        ↓
party travels toward encounter
        ↓
encounter spawn/engagement rules operate
        ↓
targeting policy chooses actors inside relevant combat

# Open Questions

- Required versus optional encounters.
- How encounter discovery works.
- Encounter completion conditions.
- Ambient movement for disengaged enemies.
- Exact schedule authoring UI.
- Whether encounter direction applies to whole party or individual characters.