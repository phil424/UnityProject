# Unity Authoring

**Status: Canonical**

These rules govern Unity Editor instructions, scene/prefab authoring and
designer-facing authoring ergonomics.

**Editor validation must not obstruct authoring.**

`OnValidate()` may sanitize simple scalar values where safe, but should not automatically shrink, reorder, delete, or otherwise mutate designer-authored serialized collections during ordinary Inspector editing.

Collection problems should normally be reported through non-destructive validation. Automatic cleanup should require an explicit designer action.

# Unity Editor instructions must be literal

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

# Progressive Disclosure in Designer Tools

Designer-facing tools should be organised around what the designer is trying to
do, not around every runtime/data system involved in that task.

Prefer task-focused workflows such as:

Route Layout

Ambient Life

Encounter Placement

over one large surface exposing every underlying component simultaneously.

Normal authoring should:
- show only actions relevant to the current task and selection;
- keep the number of obvious primary actions small;
- use contextual actions rather than permanently-disabled controls where
  practical;
- hide advanced metadata and repair operations until requested;
- use short labels and tooltips rather than permanent explanatory paragraphs;
- keep healthy validation compact;
- reveal detailed diagnostics only when problems exist or the designer requests
  them.

Runtime/data terminology does not have to be the primary designer-facing
terminology.

For example:

`AmbientRouteNode`

may be presented to the designer as:

`Route Point`.

Internal structure should be created automatically by normal authoring actions
where ownership is unambiguous.

Do not make the designer perform setup/repair operations before the common
creation workflow can function.

The underlying generated objects and data should remain manually editable
through normal Unity Inspectors as an advanced escape hatch.

# Non-Destructive Editor Validation

Editor validation must not obstruct ordinary authoring.

`OnValidate()` may safely normalize simple scalar values where doing so does not
interfere with editing.

Do not use automatic validation to silently:

- shrink serialized collections;
- remove temporary null slots created while the designer expands a list;
- reorder designer-authored collections;
- delete references;
- rewrite authored topology.

Unity may invoke `OnValidate()` repeatedly during normal Inspector editing.

Therefore collection validation should normally:

author
↓
inspect
↓
report problems

rather than:

author
↓
automatically mutate data.

Where automatic cleanup would be useful, expose it as an explicit designer
action such as:

`Clean Safe Issues`

or an equivalent authoring-tool command.

Designer-facing tooling should prefer non-destructive validation and preserve
manual Inspector editing as an escape hatch.