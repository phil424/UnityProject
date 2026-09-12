# UI Authoring

**Status: Canonical**

Player-facing UI work must follow this document.

These rules exist because incomplete Unity UI instructions create expensive
serialization, layout and scene-ownership mistakes.

# RectTransform instructions

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

# UI Authoring and Presentation

If the user must guess which scene to edit, which Unity UI object type to create,
or a text/layout setting that materially affects the result, the UI instructions
are incomplete.

UI hierarchy is part of the designer-facing authoring API.

Runtime presentation architecture may remain modular, but ordinary UI setup
should be cohesive and difficult to misconfigure.

## Prefer Composite Views

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

## Build Repeated Widgets From One Canonical Child

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

## Prefer Group-Level Presentation

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

## Avoid Parallel Serialized Arrays

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

## Prefer Layout-Driven UI

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

## Keep Semantic Identity Separate From Visual Symbols

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

## Keep Wiring Local

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

## UI Work Contract — Mandatory

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

## Exact UI Creation Instructions

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

## Complete TMP Configuration

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

## Complete Layout Configuration

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

## Prefab Authoring Protocol

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

## Do Not Replace Referenced UI Subtrees Casually

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

## Preserve Scene-Specific Active State

Before editing UI that differs between scenes, record its intended active state.

Temporarily enabling a normally-disabled panel for authoring must not result in
saving it enabled.

Example:

`StrategicEncounterPanel`

may be active in normal expedition play but deliberately inactive in the
Encounter Workshop.

The final instructions must state the expected active/component state for every
scene-specific panel touched by the step.

## Scene Switching Protocol

For UI work involving multiple scenes:

1. finish one scene;
2. validate its hierarchy and Inspector references;
3. save it;
4. then open the next scene.

Do not rely on cross-scene copy/paste of complex player-facing UI containers.

Use shared prefab assets for shared widgets.

## Required Reference Validation

Before leaving a UI-edited scene:

- inspect every high-level UI controller touched by the change;
- verify required serialized references are not `None`;
- verify there are no `Missing` references;
- restore intended GameObject/component active states.

Player-facing UI controllers should fail loudly when required references are
missing instead of silently displaying stale placeholder content.

Where ownership is unambiguous, auto-discover child Views rather than requiring
fragile manual references.

## Two-Stage Shared UI Workflow

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

## UI Validation Gate

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

## UI Validation

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

## Internal Developer Tool UI

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

### Numeric Authoring Controls

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

## Transient UI Lifetime

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