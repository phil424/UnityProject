**Status: Canonical**

These rules govern how implementation work is scoped, communicated, validated
and checkpointed.

# Sources of truth

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

# Design Documentation is Authoritative

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

# Development Step Structure

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

# Snapshot cadence

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

# Testing philosophy

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

# Code Instructions Must Be Explicit

When giving implementation instructions:

- always provide full file paths;
- for manageable scripts, prefer complete replacement files;
- otherwise provide exact `find this → replace with this` instructions;
- include enough surrounding context to avoid editing the wrong location;
- explain architectural reasoning where it materially matters.

Do not give vague instructions such as:

> "Update your manager."

State exactly what to edit.

# Diagnose Problems from Evidence

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

# Preserve Prototype Values

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

# Acceptance Criteria

End implementation steps with a short checklist covering the feature just changed.

Do not automatically include:

- every previous feature;
- complete run regression;
- dozens of test cases.

Only expand validation when the change genuinely warrants it.

# Development Priority

Prefer proving **flexibility and meaningful build interactions** before producing large amounts of content.

For example:

> Three abilities that support acquisition, levels, augments, different activation/runtime patterns and build synergy are currently more valuable than twenty isolated hard-coded abilities.

Similarly, do not build huge quantities of monsters, gear or levels before the systems that make those things interact meaningfully are established.

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