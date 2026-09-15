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