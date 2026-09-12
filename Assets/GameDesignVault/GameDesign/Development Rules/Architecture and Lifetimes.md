**Status: Canonical**

These rules protect runtime ownership boundaries and favour incremental,
composable architecture over speculative rewrites.

# Preserve and extend existing architecture

Prefer:

- small extensions;
- explicit seams;
- reusable generic components;
- data-driven behaviour;
- composition;
- existing lifecycle ownership;
- existing systems where they already solve part of the problem.

Avoid:

- speculative rewrites;
- duplicate systems;
- giant manager classes;
- giant effect enums;
- special-case logic for individual characters/enemies where a generic seam is appropriate;
- solving distant future systems prematurely.

If an implementation already exists but is crude, inspect and evolve it rather than automatically replacing it.

# Keep lifetimes explicit

The project deliberately separates:

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

`Run` remains the current implementation term for the active Expedition
lifetime.

### Persistent / Meta-owned

Examples:

PersistentProgression
equipment progression
permanent ability unlocks
characters / supports
future materials / crafting resources

These survive expedition success and failure.

### Setup-owned

Examples:

RunSetup
RunStartConfiguration

### Expedition / Run-owned

Examples:

RunState
RunBuild
WorldClockState
expedition currency
acquired abilities
ability levels
evolutions
future temporary augments
schedule / escalation state

These survive region / level transitions inside the expedition.

They are discarded when the expedition ends.

### Region / Level-owned

Examples:

scene-authored encounter sites
ambient routes
local encounter supply
region runtime objects

### Encounter-owned

Examples:

availability
commitment
expiry
spawn-group state
encounter membership

### Runtime Actor-owned

Examples:

Health
cooldowns
ForcedMotion
temporary buffs/debuffs
runtime ability instances
combat targets
navigation intents

Do not accidentally move temporary actor state into `RunBuild`, expedition
state into persistent progression, or persistent progression into runtime
components.

When a feature genuinely touches these boundaries, explicitly validate the
relevant recreation/reset behaviour.

# Keep Runtime, Progression, Presentation and Debug Separate

Maintain the distinction:

```
Gameplay/runtime logic
        !=
Run/meta progression
        !=
Normal player-facing presentation
        !=
Developer/debug tools
```

Examples:

- UI buttons request ability activation; they do not execute Whirlwind logic.
- Reward cards request reward application; they do not mutate stats directly.
- Debug tools may inspect or trigger gameplay but should not become required gameplay paths.