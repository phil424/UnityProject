# Workflow and Validation

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

Every meaningful implementation or housekeeping step follows the same delivery
sequence.

The sequence exists to prevent implementation and documentation from drifting
apart.

## Phase 0 — Documentation Preflight & Cleanup

Before implementation instructions:

1. inspect the Design, Technical and Guide documents directly relevant to the work;
2. identify stale, contradictory or misleading information that would affect the
   implementation;
3. make only the small documentation corrections needed to establish a trustworthy
   starting point.

Do not turn this into a general documentation-cleaning exercise.

If the relevant documentation is already accurate, state:

`Documentation Preflight: No changes required.`

## Phase 1 — Implementation Contract

State:

- Goal
- Deliberately NOT included
- Files to CREATE
- Files to MODIFY
- Files to DELETE

Where relevant also provide:

- Unity Authoring Work Contract;
- UI Work Contract;
- prefab workflow;
- scene ownership;
- serialized-reference expectations.

## Phase 2 — Implementation

Provide exact instructions in dependency order.

Instructions must be executable from top to bottom.

Prefer:

Shared data/type
↓
shared contract
↓
concrete implementation
↓
call sites
↓
presentation
↓
Unity authoring

Do not mix unrelated cleanup into the implementation diff.

## Phase 3 — Focused Verification

Validate the behaviour actually changed.

Use:

- compilation gates where appropriate;
- focused manual runtime validation;
- broader regression testing only when the risk warrants it.

Do not advance when the relevant acceptance criteria are still failing.

## Phase 4 — Documentation Closure

After successful verification, inspect the relevant documentation again.

Update documentation to describe the verified final state.

Specifically check for:

- stale future-tense wording;
- obsolete Current Limitations;
- old names;
- changed ownership or data flow;
- changed authoring workflow;
- decisions that became Confirmed;
- ideas that remain Working or Exploratory.

Assess separately:

- Design Documentation Impact;
- Technical Documentation Impact;
- Authoring Guide Impact;
- Project Management / Roadmap Impact.

Do not change roadmap status merely because files were edited.
Only update roadmap state when the corresponding development outcome has actually
been verified.

## Phase 5 — Checkpoint Decision

After implementation, verification and documentation closure, decide whether the
current project is a useful new authoritative checkpoint.

A lettered step does not automatically require a new snapshot.

Several related small steps may be completed before creating the next Complete.zip.

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

# Instruction Clarity and Dependency Ordering

Implementation instructions must be executable from top to bottom without the
user having to infer missing dependencies or edit locations.

## Shared Contract Changes

Before changing any shared contract such as:

- constructor signatures;
- method signatures;
- events / delegates;
- abstract members;
- interfaces;
- shared enums;
- shared data types;

inspect the current project for:

- all direct call sites;
- all subclasses / implementers;
- editor or Workshop tooling that uses the same API;
- debug paths;
- runtime paths.

Every currently affected project-owned file must appear in the implementation
scope.

Do not rely on compilation errors to discover predictable call sites after the
instructions have already been given.

## Dependency Order

Instructions must introduce dependencies before their consumers.

Prefer:

Shared Type
↓
Shared Contract
↓
Concrete Implementers
↓
Call Sites
↓
Presentation
↓
Unity Authoring
↓
Documentation

When a new shared type is required, create it before instructing edits that
reference it.

After a substantial shared-contract change, include a compilation gate before
scene / prefab / asset authoring when practical.

## Exact Code Edit Locations

For partial-file edits:

- name the exact file;
- name the enclosing method / property / class;
- show a unique existing code block to find;
- show the exact replacement or exact insertion point;
- include enough surrounding context to distinguish similar call sites.

Avoid instructions such as:

> update the normal call

or:

> add this near the reward code

when multiple plausible locations exist.

Prefer:

> In `HandleLevelFinished(bool won)`, find:
>
> `GenerateRunUpgradeOffers();`
>
> replace it with:
>
> `GenerateRunRewardOffers();`

## Documentation Edit Locations

Documentation instructions follow the same standard.

Always identify:

- the exact document;
- the exact heading;
- whether text is being inserted before, after or replacing another block;
- the final Markdown to use.

Do not require the user to decide where a design paragraph belongs.

## Compile Failures After Instructions

When compilation fails:

1. inspect the actual errors;
2. identify whether they are one root error with cascading failures;
3. correct the smallest missing dependency / call site;
4. do not make unrelated changes merely because many compiler errors are shown.

A wall of compiler errors caused by one missing shared type should be treated as
one missing dependency, not many independent bugs.

## UI Instruction Checkpoints

For player-facing UI work, do not treat a long Unity hierarchy-edit sequence as
one uninterrupted instruction block.

When the change modifies existing hierarchy or layout ownership, split the work
into explicit checkpoints:

### Checkpoint A — Hierarchy

Verify:
- intended current-to-target migration is complete;
- obsolete containers have been removed;
- no duplicate conceptual roots remain.

### Checkpoint B — References

Verify:
- controller references still point to the intended moved objects;
- no required field is `None` or `Missing`.

### Checkpoint C — Layout

Verify:
- each axis has a clear owner;
- parent and child LayoutGroups are not competing;
- every LayoutElement-driven Preferred/Flexible axis has parent
  `Control Child Size` enabled for that axis;
- manually-sized axes are not simultaneously being controlled by a LayoutGroup;
- runtime-generated content has the correct root.

### Checkpoint D — Runtime Presentation

Validate with real runtime data rather than placeholders when the View is
runtime-generated.

Do not proceed to detailed styling when an earlier structural checkpoint is
still incorrect.

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
Choose coherent extensible change with scope driven by value and dependencies
        ↓
State scope/files
        ↓
Implement explicitly
        ↓
Focused manual validation
        ↓
Optional milestone Code Health Pass
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

Code examples, replacement methods and complete files must use compact,
conventional C# formatting consistent with Visual Studio / Rider-style Unity
code.

The default is horizontal readability.

Do not format code vertically merely because the answer is being presented in
chat.

## Default Style

Prefer:

- method signatures on one line when they remain readable;
- constructor calls on one line;
- ordinary method calls on one line;
- simple property access and member chains on one line;
- simple boolean conditions on one line;
- concise early returns without unnecessary braces;
- short lambdas on one line;
- short collection/index expressions on one line;
- normal assignments on one line;
- one blank line between logical blocks rather than between individual
  statements.

Examples:

```csharp
AmbientRouteNode start = routeGraph.FindNearestNode(leader.transform.position);

if (graph == null || start == null || !graph.ContainsNode(start))
    return false;

foreach (PartyMember member in FindObjectsByType<PartyMember>(FindObjectsSortMode.None))
{
    ...
}

navigation.SetDestination(this, targetNode.Position, arrivalDistance, suppressCombatTargeting: false);
```
``
# Code Health Pass

Legacy code should be improved gradually rather than preserved indefinitely or
rewritten through large cleanup projects.

When worthwhile, scope Code Health as its own small housekeeping development step
using the normal Development Step Structure.

Do not silently mix unrelated legacy cleanup into a feature implementation diff.

A recently completed feature may reveal a good Code Health candidate, but the
cleanup should remain separately scoped and separately validated.

At the end of each meaningful milestone or coherent lettered development slice,
consider an optional Code Health Pass.

The normal target is:

> one project-owned legacy source file.

The pass may include:

- formatting the file to the current compact C# conventions;
- removing obsolete formatting patterns;
- simplifying unnecessarily awkward local code;
- improving misleading names where the change is clearly behaviour-preserving;
- removing obviously dead local code where safe and verified;
- making small readability improvements;
- aligning the file with current Development Rules.

The pass is optional.

Skip it when:

- the milestone already involved unusually risky changes;
- there is no worthwhile low-risk candidate;
- additional cleanup would obscure an important gameplay diff;
- current project context is insufficient to verify behaviour safely.

## Selection Priority

Prefer candidates in roughly this order:

1. old project-owned files that have not been modernised since early development;
2. files with particularly poor or vertically exploded formatting;
3. files repeatedly encountered during normal development;
4. small stable files where behaviour-preserving cleanup is easy to verify;
5. larger architectural files only when their responsibilities are already well
   understood.

When several candidates are equally suitable, prefer the older file.

Do not modify third-party, package or imported asset code merely to match project
style.

## Scope

A normal Code Health Pass should remain small.

By default:

- choose one file;
- preserve runtime behaviour;
- do not introduce new gameplay;
- do not redesign architecture without a separately scoped implementation step;
- do not cascade into neighbouring files merely for stylistic consistency.

If inspection reveals a genuine architectural problem, record it or scope it as
future work rather than silently expanding the cleanup pass.

## Formatting

Code Health files should be brought to the current compact C# style.

Use normal Visual Studio / Rider-style formatting:

- readable signatures and calls remain horizontal;
- avoid unnecessary one-argument-per-line wrapping;
- use compact trivial guards;
- keep logical blocks visually grouped;
- preserve braces where lifecycle-critical scope needs to remain explicit;
- remove obsolete whitespace and formatting artifacts.

The aim is not clever or maximally compressed code.

The aim is ordinary, clean, readable C#.

## Validation

Because a Code Health Pass is intended to preserve behaviour, validation should
be proportional to the file being changed.

Normally:

- require successful compilation;
- perform one focused manual check of the behaviour owned by the file.

Do not perform a full-project regression pass for a formatting/readability-only
cleanup.

## Development Principle

Feature work and code-health work are separate concerns.

Prefer:

Feature Implementation
↓
Feature Validation
↓
Optional Code Health Pass
↓
Behaviour-Preservation Validation

rather than mixing unrelated cleanup throughout the feature diff.

Over time this process should steadily migrate the oldest parts of the project
toward current standards without requiring a disruptive repository-wide rewrite.