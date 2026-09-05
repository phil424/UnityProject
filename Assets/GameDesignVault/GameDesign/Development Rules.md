# ECS Unity Tutorial / MiniCrawler — Development Instructions

We are continuing an existing Unity MiniCrawler / autonomous-combat buildcraft project.

## 1. Sources of truth

Before implementing a new development step:

- inspect the latest uploaded numbered `{X.XX}Complete.zip` as the authoritative project baseline;
- if a newer working zip has been explicitly provided, inspect it when the requested work depends on changes made after the latest Complete snapshot;
- inspect the embedded Obsidian vault at:

  `Assets/GameDesignVault/GameDesign/`

  inside that project archive;
- treat the embedded `Development Rules.md` as the current development workflow;
- treat the embedded `Design/` knowledge base as the current living game-design and technical-design documentation;
- inspect the actual relevant scripts, prefabs, ScriptableObjects, scenes and existing architecture;
- do not reconstruct implementation details from memory;
- do not suggest creating a new system until checking whether an equivalent or related system already exists.

If code, older conversation context, or older standalone Design/Rules uploads disagree with the latest embedded project vault, prefer the latest project archive unless the user explicitly states otherwise.

If implementation and design appear inconsistent, identify the conflict rather than silently choosing one.

---

# 2. Design documentation is authoritative for game direction

Use the latest Design documentation when making architectural or gameplay recommendations.

The current design pillars include:

- autonomous baseline combat;
- deliberate player intervention through active abilities;
- buildcraft as a core part of the run;
- preparation establishes direction but does not guarantee the final build;
- permanent progression should primarily expand possibilities rather than endlessly increase raw power;
- reward generation should respect the player's existing build;
- level defeat does not automatically end the run; between-level continuation remains available once unresolved pending rewards have been handled;
- expeditions should escalate toward increasingly unusual/dangerous situations.

Where useful, distinguish design ideas as:

```
Confirmed
Working
Exploratory
Deprecated
```

Do not quietly turn exploratory ideas into implementation requirements.

---

# 3. Work in clear lettered development steps

Continue using:

```
2.9A
2.9B
2.9C
...
```

because the lettering is useful for defining scope.

However:

> A lettered step does NOT automatically require a new snapshot or a long validation pause.

Several related lettered steps may be completed consecutively as one coherent development slice.

At the start of each implementation step provide:

- Goal
- Deliberately NOT included
- Files to CREATE
- Files to MODIFY
- Files to DELETE

Keep the scope explicit.

---

# 4. Snapshot cadence

Do not create a zip after every minor change.

Create a new authoritative snapshot when:

- a coherent feature slice is complete;
- a significant architectural checkpoint has been reached;
- a milestone is complete;
- the change is risky enough that a stable rollback point is valuable.

### Mandatory rule

If one planned development step will:

```
CREATE + MODIFY + DELETE >= 10 files
```

a new snapshot zip is required at the end of that step before continuing.

This overrides the looser snapshot cadence.

If a validation/closure step requires no changes, do not modify files merely to create a new snapshot. The previous approved snapshot can remain authoritative.

---

# 5. Testing philosophy

Do **not** routinely create new automated/EditMode tests.

The user will explicitly request automated tests when desired.

Default validation should be:

- short;
- focused on the behaviour just changed;
- easy to perform manually;
- sufficient to catch obvious architectural/runtime failures.

Do not produce large repetitive regression checklists after every change.

Broader regression testing is appropriate only when:

- a change crosses important lifetime boundaries;
- a central shared system was modified;
- the change is particularly high-risk;
- there is evidence of a regression;
- the user explicitly requests it.

Development should remain deliberate without becoming dominated by test administration.

---

# 6. Preserve and extend existing architecture

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

---

# 7. Keep lifetimes explicit

The project deliberately separates:

```
Setup
  ↓
Run
  ↓
Level
```

### Setup-owned

Examples:

```
RunSetup
RunStartConfiguration
```

### Run-owned

Examples:

```
RunState
RunBuild
currency
acquired abilities
ability levels
future ability augments
```

These survive runtime actor recreation during the run.

### Level/runtime actor-owned

Examples:

```
Health
cooldowns
ForcedMotion
temporary buffs/debuffs
runtime ability instances
```

These disappear when the actor is destroyed.

Do not accidentally move temporary actor state into `RunBuild`, or persistent run progression into runtime components.

When a feature genuinely touches this boundary, explicitly validate actor recreation.

---

# 8. Ability architecture rules

Abilities are an important generic gameplay primitive.

## Ability behaviour must remain actor-generic

An ability should not inherently assume:

```
Player
Punchy
Enemy
Boss
```

Where practical, the same underlying ability should be usable by different actor types.

Keep this conceptual separation:

```
Ability behaviour
        !=
Ability ownership
        !=
Activation policy
```

Example:

```
WhirlwindAbility
    = gameplay behaviour

Player RunBuild
    = player owns Whirlwind

Boss loadout
    = boss owns Whirlwind

Ability HUD
    = player requests activation

Enemy/Boss AI
    = AI requests activation
```

Both should ultimately use the same ability execution seam.

---

# 9. Player abilities are deliberate interventions

Baseline combat remains autonomous:

- targeting;
- movement;
- weapon/basic attacks;
- support behaviour.

Player abilities are normally manually triggered tactical actions.

Do **not** bake automatic activation back into the generic `AbilitySystem`.

The normal model is:

```
Player input/UI
      ↓
activation request
      ↓
ActorAbility.TryActivate()
      ↓
ability gameplay
```

Optional autocasting may exist later as a separate policy.

---

# 10. Abilities must support different behaviour shapes

Do not design the ability architecture around only instant AoE attacks.

It should remain capable of supporting at least:

### Instant

```
Whirlwind
```

Immediate effect.

### Timed runtime effect

```
Rage
```

Applies temporary modifiers that expire.

### Movement-duration

```
Charge
```

Actor actively moves for a period/distance and can interact with enemies during that motion.

Future abilities may also be:

- targeted;
- directional;
- ground-targeted;
- cast;
- channelled;
- charge-based.

Do not implement all of these generically until required, but avoid assumptions that prohibit them.

---

# 11. Ability state has three conceptual layers

Keep these distinct:

```
ABILITY DEFINITION
"What is Whirlwind?"

Static data / runtime behaviour identity
```

```
RUN ABILITY STATE
"What has this build developed?"

owned
level
future augments/evolutions
```

```
RUNTIME ABILITY STATE
"What is happening right now?"

cooldown
active duration
temporary execution state
```

Runtime actors should reconstruct their abilities from run-owned state when appropriate.

Enemy/boss ability loadouts should be able to use the same generic ability definitions without requiring player `RunBuild` or currency systems.

---

# 12. Ability levels and ability expansions are different

Treat these as two separate progression axes.

### Ability level

Incremental predictable improvement:

```
Whirlwind Lv.1
→ Lv.2
→ Lv.3
```

Examples:

- damage;
- radius;
- cooldown;
- knockback;
- duration.

### Ability expansion / augment

Transformative buildcraft:

```
Whirlwind
├── applies Fire
├── creates a damage trail
├── knockback causes explosions
└── gains another designed behaviour
```

Do not reduce ability buildcraft to number increases alone.

Run-owned ability state should remain extensible enough to store acquired augments/evolutions later.

Avoid prematurely building a giant universal effect framework. Generic infrastructure should manage ownership/configuration; concrete abilities may interpret their own meaningful augment behaviour.

---

# 13. Temporary modifiers stay runtime-owned

Temporary effects such as Rage should use runtime modifier/effect state.

Do not permanently mutate:

```
base character values
AbilityDefinition
RunBuild
```

to implement a short-lived buff.

Temporary modifiers may eventually support:

- outgoing damage;
- attack speed;
- movement speed;
- armour;
- healing;
- debuffs;
- resistance;
- other temporary gameplay stats.

Prefer central stat/damage seams so modifiers affect all appropriate gameplay sources consistently.

---

# 14. Forced movement and knockback remain generic primitives

Existing combat-motion architecture includes:

```
ForcedMotion
KnockbackResolver
KnockbackPropagation
MovementSystem
AvoidanceSystem
```

Use these for new abilities where appropriate.

For example:

```
Charge
→ self movement
→ enemy contact
→ KnockbackResolver
```

Do not create a second movement/physics framework solely for an ability unless the existing architecture genuinely cannot represent the mechanic.

Future knockback systems should remain extensible toward:

- outgoing knockback modifiers;
- incoming knockback resistance/vulnerability;
- boss/heavy-actor resistance;
- knockback-triggered effects;
- extra damage caused by knockback.

Do not prematurely implement them without a current use case.

---

# 15. Reward architecture must stay generic

A reward is a delivery mechanism, not synonymous with a stat increase.

Rewards may eventually provide:

```
stat upgrade
ability
ability augment/evolution
material
persistent unlock
other future content
```

Do not grow `RunUpgradeEffectType` into a giant list of things such as:

```
AcquireCharge
AcquireRage
WhirlwindFire
WhirlwindRadius
...
```

Prefer generic reward-definition types that know:

```
Can this reward apply?
What does it display?
How does it modify the build?
```

Reward generation should increasingly understand:

- existing build;
- eligibility;
- synergies;
- avoiding dead choices;
- interesting pivots.

---

# 16. Major rewards and small purchases are distinct

Keep these progression concepts separate.

### Significant/random reward choices

Examples:

```
Learn Charge
Learn Rage
Whirlwind gains an augment
major build-changing option
```

### Small deterministic purchases

Examples:

```
Whirlwind Lv.1 → Lv.2
incremental gear/stat improvement
```

They may modify the same `RunBuild`, but they should not necessarily use the same economy or presentation.

---

# 17. Buildcraft should remain extensible

Current/future run builds may contain:

- abilities;
- ability levels;
- ability expansions/evolutions;
- temporary augments;
- stat progression;
- synergies;
- temporary runtime effects.

Avoid architecture that assumes the finished build is only a list of flat stat bonuses.

Future synergy/tag concepts may include:

```
Melee
Projectile
Area
Movement
Bleed
Fire
Poison
Explosion
Knockback
Critical
Healing
Shield
Attack
Ability
Cooldown
```

Do not implement a large tag system until there is a concrete need, but avoid design decisions that make one difficult later.

---

# 18. Future crafting/resources should not be prematurely implemented

Current future design direction includes possible:

- monster resources;
- persistent materials;
- Monster Hunter-inspired crafting;
- weapons/armour built from monster materials;
- gear talents/skills;
- interactions between gear, abilities and augments;
- longer/persistent expedition spaces.

These require a dedicated design pass.

Do **not** begin implementing crafting/resource architecture merely because future plans mention it.

Current development should only ensure that:

- gear/buildcraft remains modular;
- abilities remain extensible;
- persistent/run/runtime state boundaries stay clean.

---

# 19. Weapon architecture should remain compatible with future identity

Weapons are intended to define autonomous combat playstyle rather than act as simple stat sticks.

Future weapons may influence:

- combo sequence;
- cadence;
- range;
- movement;
- innate abilities;
- cancel windows;
- invulnerability;
- status behaviour;
- augmentation capacity.

Do not build current combat architecture around the assumption that every weapon is simply:

```
Damage + AttackSpeed
```

---

# 20. Runtime, progression, presentation and debug remain separate

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

---

# 21. Code instructions must be explicit

When giving implementation instructions:

- always provide full file paths;
- for manageable scripts, prefer complete replacement files;
- otherwise provide exact `find this → replace with this` instructions;
- include enough surrounding context to avoid editing the wrong location;
- explain architectural reasoning where it materially matters.

Do not give vague instructions such as:

> "Update your manager."

State exactly what to edit.

---

# 22. Unity Editor instructions must be literal

Assume Unity UI/editor instructions need to be explicit.

Use steps such as:

```
Select X
Right-click
Choose UI → Panel
Rename it Y
Add Component → Z
Drag A into field B
Save prefab
```

Show expected hierarchy when helpful.

Do not skip object creation or reference-assignment steps.

Also distinguish correctly between:

```
C# definition/class
```

and:

```
actual ScriptableObject asset instance
```

For example:

```
PartyMemberDefinition.cs
```

defines fields, while:

```
WarriorDefinition.asset
```

contains Punchy's editable values.

Inspect the actual asset inheritance before telling the user where a serialized value will appear.

---

# 23. RectTransform instructions

Always establish:

1. Anchor Min
2. Anchor Max
3. Pivot

before giving position/size values.

Remember:

### Fixed anchors

```
Anchor Min == Anchor Max
```

usually exposes:

```
Pos X
Pos Y
Width
Height
```

### Stretched anchors

usually expose:

```
Left
Right
Top
Bottom
```

Never instruct the user to enter fields that will not exist for the stated anchor configuration.

For purely visual values say:

> **Use this as a starting value and adjust visually.**

---

# 24. Diagnose problems from evidence

When something fails:

- inspect the exact error/screenshot/runtime behaviour;
- identify which layer is broken;
- change the smallest relevant part;
- do not modify several unrelated systems simultaneously.

Examples:

```
Correct health number, wrong position
→ UI/layout problem

NullReferenceException while binding UI
→ lifecycle/reference initialization problem

Run upgrade persists incorrectly
→ run/runtime lifetime problem
```

Do not treat every visible problem as a gameplay-system failure.

---

# 25. Preserve prototype values

Do not silently rebalance exaggerated prototype numbers.

Current values may intentionally be extreme so architectural effects are easy to see.

Separate:

```
Does the mechanic work?
```

from:

```
Is the mechanic balanced?
```

Balance changes should be intentional.

---

# 26. Acceptance criteria should stay concise

End implementation steps with a short checklist covering the feature just changed.

Do not automatically include:

- every previous feature;
- complete run regression;
- dozens of test cases.

Only expand validation when the change genuinely warrants it.

---

# 27. Development priority

Prefer proving **flexibility and meaningful build interactions** before producing large amounts of content.

For example:

> Three abilities that support acquisition, levels, augments, different activation/runtime patterns and build synergy are currently more valuable than twenty isolated hard-coded abilities.

Similarly, do not build huge quantities of monsters, gear or levels before the systems that make those things interact meaningfully are established.

---

## Condensed operating rule

When unsure, follow this sequence:

```
Read latest Design
        ↓
Inspect latest Complete.zip
        ↓
Understand existing seam
        ↓
Choose smallest extensible change
        ↓
State scope/files
        ↓
Implement explicitly
        ↓
Focused manual validation
        ↓
Continue if small
        ↓
Snapshot at meaningful checkpoint
or always if >=10 files changed
```

That’s the instruction set I’d use going forward. It retains the deliberate architecture-first approach we liked, while removing the parts that had become cumbersome: constant snapshots, ever-growing automated-test suites, and giant regression checklists.

# Documentation Stewardship

The Design knowledge base is a living part of development, not a one-time planning artifact.

As systems become relevant:

- inspect the related design documents;
- flesh out incomplete sections using decisions that have actually been made;
- record newly confirmed design decisions promptly;
- preserve Working / Exploratory ideas as such rather than silently promoting them to requirements;
- mark superseded decisions as Deprecated or replace them explicitly;
- identify contradictions between current implementation, current decisions and documentation.

Do not attempt to fully design every future system in advance.

Documentation should grow alongside implementation and should reduce ambiguity rather than create development overhead.

At the end of a meaningful development step, include:

Documentation Impact:
- None; or
- Design: documents
- Technical: documents

Trivial implementation changes do not require documentation updates.

# Technical Design Documentation

Maintain technical-design documents separately from player-facing game-design intent.

Use:

Design/Technical/

Game-design documents primarily describe:
- desired player experience;
- gameplay rules;
- motivations;
- content direction;
- confirmed and unresolved design questions.

Technical-design documents primarily describe:
- responsibilities and non-responsibilities;
- ownership and lifetime;
- important runtime state;
- data flow and event flow;
- architectural seams;
- invariants;
- extension points;
- known limitations;
- current implementation boundaries;
- unresolved technical questions.

Technical documents should describe intended structure, not every implementation detail.

Create or expand technical documents when a system becomes architecturally significant rather than creating documents speculatively for every possible future feature.

# Working State Versus Authoritative Snapshot

The latest numbered Complete.zip remains the authoritative rollback/checkpoint snapshot.

Several verified lettered steps may exist after that snapshot without requiring another Complete.zip.

If further implementation depends on files changed after the latest Complete.zip, inspect a current uploaded working zip before giving exact code changes.

A working zip does not automatically become an authoritative checkpoint and does not change snapshot cadence.

# C# Formatting Style

Code examples and replacement files should use conventional compact C# formatting.

Prefer:

- method signatures on one line when reasonably short;
- simple method calls on one line;
- simple conditions on one line;
- concise early returns;
- logical expressions grouped naturally;
- wrapping only when a line becomes genuinely long or readability materially improves.

Avoid:

- putting individual identifiers or arguments on separate lines unnecessarily;
- vertically exploding simple expressions;
- excessive nesting caused purely by formatting;
- wrapping short boolean expressions across many lines.

As a general guide, lines around 120–140 characters are acceptable when still readable, but use judgement rather than enforcing a rigid maximum.

Match the style already present in the latest project where practical.

# Authoring Ergonomics

Preserving runtime separation does not require exposing every runtime seam as
manual scene or Inspector wiring.

For designer-authored content:

- prefer hierarchy to express obvious ownership;
- automatically discover child components where ownership is unambiguous;
- avoid requiring the same object to be referenced in several separate arrays;
- make the common case understandable from one primary authoring root;
- keep advanced escape hatches only when there is a concrete need for them;
- do not create separate GameObjects merely because two runtime concepts are
  implemented by separate classes.

If authoring one ordinary gameplay concept requires following references across
several unrelated GameObjects or components, reassess the authoring surface
before adding more features.

Runtime architecture should remain modular while the authoring experience stays
coherent.