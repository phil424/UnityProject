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
- party defeat ends the active expedition; normal level / region clear continues
  the same expedition; persistent progression survives expedition end while
  expedition-owned progression resets;
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

Snapshots exist to keep implementation context current, inspectable and
unambiguous.

Do not use a hard file-count threshold.

Create a new authoritative Complete.zip when it would materially improve our
ability to understand, verify or safely continue from the current project state.

Strong reasons to take a snapshot include:

- a coherent gameplay or architectural progression point is complete;
- an important runtime seam or ownership model has changed;
- scene, prefab, ScriptableObject or UI serialization has changed enough that
  inspecting the actual project matters;
- several smaller steps have accumulated and the previous snapshot no longer
  accurately represents the working project;
- a risky change would benefit from a stable rollback point;
- a milestone is complete.

During low-risk periods of small, well-understood code tweaks, several steps may
continue without a new snapshot when the previous project context remains
sufficient.

Prefer meaningful development slices large enough to justify a checkpoint where
practical, but do not interrupt productive iteration merely to satisfy snapshot
cadence.

The purpose of a snapshot is:

> keep development context trustworthy.

It is not:

> create administrative pauses after arbitrary amounts of work.

If further implementation depends on serialized scene/prefab/UI changes that
cannot be reliably reconstructed from the previous snapshot, request an updated
working or Complete zip before giving exact follow-up implementation changes.

If a validation/closure step requires no changes, do not modify files merely to
create a new snapshot.

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
Snapshot when the project has reached a meaningful checkpoint
or when refreshed project context would materially improve verification
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

### Braces Around Lifecycle-Critical Conditions

Compact single-line conditionals remain acceptable for trivial early returns and
simple assignments.

Use braces when a conditional gates important lifecycle behaviour such as:

- spawning;
- encounter start / completion;
- progression reset;
- Run / Level lifetime transitions;
- reward application;
- persistence;
- destruction / cleanup.

Also add braces when modifying an existing single-line conditional to place
additional statements immediately around it.

The goal is to make the guarded scope visually unambiguous and avoid accidental
fall-through during later edits.

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

## UI Authoring and Presentation

If the user must guess which scene to edit, which Unity UI object type to create,
or a text/layout setting that materially affects the result, the UI instructions
are incomplete.

UI hierarchy is part of the designer-facing authoring API.

Runtime presentation architecture may remain modular, but ordinary UI setup
should be cohesive and difficult to misconfigure.

### Prefer Composite Views

A repeated or multi-part UI widget should normally have one View component on
its root.

Example:

EncounterSlot
├── Symbol
└── Label

`EncounterSlotView` owns/discovers the Button, Symbol and Label.

A parent controller should reference the Slot View rather than independently
referencing:

Button
+
Text
+
Icon
+
RectTransform.

### Build Repeated Widgets From One Canonical Child

When several UI items share the same structure:

1. fully configure one canonical child;
2. validate that child;
3. duplicate it for the remaining siblings;
4. let the parent Group assign semantic identity from sibling order or explicit
   local data.

Do not independently construct several supposedly identical repeated widgets.

Example:

EncounterList
├── EncounterSlot0
├── EncounterSlot1
└── EncounterSlot2

`EncounterSlot0` should be completed first and then duplicated.

All repeated siblings should have structural component parity.

A fixed-count Group should validate the expected number of child Views when
possible.

If the Group expects three Views and discovers one or two, treat that as an
authoring error rather than silently continuing.

### Prefer Group-Level Presentation

When repeated UI items share styling, configure that styling on the parent
Group where practical.

Examples:
- symbol colour;
- text colour;
- normal background;
- selected background;
- spacing / layout behaviour.

Avoid repeating identical presentation configuration separately across every
slot unless individual variation is intentional.

### Avoid Parallel Serialized Arrays

Do not create index-coupled Inspector structures such as:

Button[]
TMP_Text[]
RectTransform[]
Image[]

when every entry represents different parts of the same repeated widget.

Prefer:

SlotView[]

or preferably:

```
SlotGroup
└── auto-discovers child SlotViews from hierarchy
```

ordered by sibling order or another explicit local property.

If several arrays must remain perfectly index-aligned for UI to function, treat  
that as an authoring smell and refactor before adding more UI.

### Prefer Layout-Driven UI

Repeated, stacked or tabular normal UI should use Unity layout components where  
appropriate:

- VerticalLayoutGroup;
- HorizontalLayoutGroup;
- GridLayoutGroup;
- LayoutElement;
- ContentSizeFitter where it genuinely simplifies sizing.

Do not manually position ordinary sibling UI elements one-by-one when a layout  
relationship describes their intent.

Manual `anchoredPosition` is appropriate for genuinely spatial/freeform  
presentation such as:

- minimap markers;
- drag/drop elements;
- world-to-screen indicators;
- deliberately overlapping presentation.

### Keep Semantic Identity Separate From Visual Symbols

Critical gameplay symbols, controller prompts, quick-slot identity and map  
markers must not depend on Unicode/font glyph support.

Do not use strings such as:

Triangle glyph  
Square glyph  
Circle glyph  
Star glyph  
Diamond glyph

as the authoritative presentation for gameplay controls.

Store semantic identity as data:

slot index  
enum  
action identity

and render it with:

- Sprite / Image;
- dedicated Graphic;
- another font-independent presentation component.

Text strings are for actual text/status information.

Changing the UI font must not break critical gameplay symbols.

### Keep Wiring Local

A high-level HUD/controller should reference a small number of cohesive panel or  
group roots.

Local View components may own their internal references.

Prefer:

StrategicHUD  
├── ForecastView  
├── EncounterListView  
└── MinimapView

over one central component containing references to every Text, Button, Icon and  
RectTransform in the hierarchy.

Automatically discover child components when ownership is unambiguous.

### UI Work Contract — Mandatory

Every player-facing UI implementation step must declare the UI authoring contract
before any Unity Editor actions are given.

It must explicitly state:

**Authoring Scene**
- the exact scene open while scene-owned UI is being edited.

**Target Scenes**
- every scene that will receive the resulting UI.

**Prefab Assets**
- every reusable prefab being created or modified.

**Do Not Modify**
- scene-owned UI roots/panels that must remain untouched;
- normally-disabled UI that must remain disabled.

**Final Hierarchy**
- the intended hierarchy after the step is complete.

The user should never have to infer which scene an instruction refers to.

### Exact UI Creation Instructions

For every new UI GameObject, instructions must state the exact Unity creation
action.

Examples:

`Right-click parent -> UI -> Panel`

`Right-click parent -> UI -> Text - TextMeshPro`

`Right-click parent -> Create Empty (UI)`

Do not use ambiguous instructions such as:

`create a text object`

`create a panel`

`add a UI object`

when several Unity object types could satisfy the wording.

### Complete TMP Configuration

Whenever a TextMeshPro UI element is created or materially changed,
instructions must explicitly state:

- placeholder / authored text;
- Font Size;
- Font Style;
- Auto Size ON/OFF;
- Horizontal Alignment;
- Vertical Alignment;
- Wrapping ON/OFF;
- Overflow mode;
- Raycast Target ON/OFF;
- LayoutElement sizing/flex where relevant.

Do not rely on TMP defaults for values that materially affect layout.

If a setting should remain at its Unity default, say so explicitly when that
default matters to the result.

The user should not have to visually diagnose an omitted text-layout setting.

### Complete Layout Configuration

For every LayoutGroup instructions must state:

- padding;
- spacing;
- child alignment;
- Control Child Size Width/Height;
- Force Expand Width/Height.

The instructions must also identify which object owns:

- position;
- width;
- height.

Avoid configurations where a LayoutGroup and manually-authored RectTransform
values fight over the same responsibility.

### Prefab Authoring Protocol

Reusable UI prefabs must have one explicit authoring workflow.

If a temporary scene object is needed to create the prefab:

1. name the exact source scene;
2. build one canonical object there;
3. drag that exact root into the stated Project folder;
4. delete the temporary scene instance if it is not needed;
5. immediately open the prefab asset in Prefab Mode.

After the prefab asset exists:

> all further shared-prefab editing should occur in Prefab Mode.

Other scenes should receive the UI through:

`Project Window -> drag prefab asset into the stated parent`

Do not copy the surrounding scene hierarchy between scenes merely to transfer a
shared widget.

Shared widgets travel between scenes as prefab instances.

Scene-owned containers remain scene-owned.

### Do Not Replace Referenced UI Subtrees Casually

Do not delete/recreate or replace an existing UI subtree simply to add a new
child if a runtime controller references that subtree.

Modify the existing hierarchy in place.

If replacement is genuinely necessary:

1. identify every serialized reference that points into that hierarchy;
2. perform the replacement;
3. immediately rewire every invalidated reference;
4. verify no required Inspector field shows `None` or `Missing`.

Replacing a referenced hierarchy without repairing its consumers is a broken
implementation step even if the hierarchy looks visually identical.

### Preserve Scene-Specific Active State

Before editing UI that differs between scenes, record its intended active state.

Temporarily enabling a normally-disabled panel for authoring must not result in
saving it enabled.

Example:

`StrategicEncounterPanel`

may be active in normal expedition play but deliberately inactive in the
Encounter Workshop.

The final instructions must state the expected active/component state for every
scene-specific panel touched by the step.

### Scene Switching Protocol

For UI work involving multiple scenes:

1. finish one scene;
2. validate its hierarchy and Inspector references;
3. save it;
4. then open the next scene.

Do not rely on cross-scene copy/paste of complex player-facing UI containers.

Use shared prefab assets for shared widgets.

### Required Reference Validation

Before leaving a UI-edited scene:

- inspect every high-level UI controller touched by the change;
- verify required serialized references are not `None`;
- verify there are no `Missing` references;
- restore intended GameObject/component active states.

Player-facing UI controllers should fail loudly when required references are
missing instead of silently displaying stale placeholder content.

Where ownership is unambiguous, auto-discover child Views rather than requiring
fragile manual references.

### Two-Stage Shared UI Workflow

For UI reused across scenes:

**Stage A — Prefab**
- create the canonical prefab;
- configure every text/layout setting;
- open it in Prefab Mode;
- validate the prefab itself.

**Stage B — Scene Integration**
- instantiate the prefab into the first target scene;
- validate;
- only then integrate it into additional scenes.

Do not author the prefab and simultaneously restructure several scenes in one
unverified sequence.

### UI Validation Gate

For a substantial player-facing UI step, validate before dependent UI work
continues:

1. canonical prefab in Prefab Mode;
2. first target scene at Canvas reference resolution;
3. one alternate aspect ratio;
4. no wrapping/clipping/overlap;
5. required controller references populated;
6. intended active states preserved.

When the next implementation depends on the serialized UI result, use a
screenshot or working-zip checkpoint before compounding additional UI changes.

### UI Validation

For UI-heavy changes, manually validate at:

- the Canvas reference resolution;
- at least one other common resolution/aspect ratio.

Check:

- no accidental overlap;
- no important content outside its parent;
- readable hierarchy;
- correct scaling;
- critical symbols render without relying on font fallback.

When a step substantially changes serialized Canvas/prefab state, use a working  
zip and/or screenshot checkpoint before building dependent UI work.

### Internal Developer Tool UI

The player-facing UI rules do not require every tiny internal development tool
to use production Canvas architecture.

A simple IMGUI overlay is acceptable for strictly developer-only tooling when:
- it is not part of normal player presentation;
- the tool has a small number of controls;
- using IMGUI materially reduces setup / iteration friction;
- the gameplay/runtime logic remains outside the UI code.

Examples:
- Encounter Workshop scenario buttons;
- temporary developer toggles;
- compact runtime diagnostics.

Do not use this exception to avoid proper architecture for normal game HUD,
menus or player-facing interfaces.

If a development tool becomes large, reusable or designer-facing enough that
IMGUI becomes cumbersome, promote it to a dedicated Editor/UI architecture.

#### Numeric Authoring Controls

When an internal authoring tool exposes meaningful numeric gameplay values,
sliders should not be the only input method.

Prefer:

label
+
slider
+
direct numeric field.

The slider should use a sensible domain-specific snapping increment where useful.

Examples:
- encounter distances: 0.25m;
- broad timing: 0.25s;
- fine spawn spacing: 0.05s;
- integer counts / levels: whole numbers.

The adjacent numeric field should permit direct exact entry within the allowed
range.

Direct entry should not fight the user's in-progress typing.

In particular, do not rebuild the text string from the parsed numeric value on
every frame while the field is focused.

Normalize the displayed value after editing instead.

Slider snapping is a usability aid.

Direct numeric entry may intentionally allow values between the slider's snap
increments when exact tuning is required.

### Transient UI Lifetime

Runtime-created transient UI must have an explicit gameplay/UI lifetime.

Examples:
- encounter announcements;
- floating notifications;
- temporary status banners;
- reward toasts;
- temporary prompts.

Do not assume an animation coroutine will finish naturally.

Unity stops coroutines when the owning GameObject becomes inactive.

Therefore unscaled time protects presentation from Simulation Time changes, but
does not protect presentation from hierarchy/lifecycle deactivation.

A transient View system should normally clean up its generated Views when:
- its owning gameplay lifetime ends;
- its owning UI root is disabled;
- the presentation system is re-enabled and stale children are discovered.

Prefer clearing through the gameplay lifecycle seam that actually owns the
presentation.

Example:

Encounter announcements
→ belong to the current Level/Region runtime
→ clear on `StageDirector.LevelCleared`.

Transient presentation must not survive into a new gameplay lifetime unless
persistence across that boundary is explicitly part of its design.