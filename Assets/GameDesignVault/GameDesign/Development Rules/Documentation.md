# Documentation

**Status: Canonical**

These rules govern the embedded Obsidian vault and technical/design
documentation stewardship.

# Documentation Stewardship

The Design knowledge base is a living part of development, not a one-time planning artifact.

## Two-Pass Documentation Rule

Meaningful development work should normally inspect documentation twice.

### Pass 1 — Before implementation

The purpose is to establish a trustworthy starting point.

Inspect only documentation relevant to the upcoming work.

Correct stale or contradictory information when leaving it unchanged would make
the implementation plan ambiguous or misleading.

Do not perform unrelated documentation housekeeping merely because a development
step is beginning.

### Pass 2 — After verification

The purpose is to record what was actually implemented and verified.

Revisit the relevant documentation after runtime validation and:

- replace obsolete future-tense descriptions;
- remove resolved limitations;
- update names and terminology;
- record confirmed ownership/data-flow changes;
- update affected authoring instructions;
- preserve unresolved ideas as Working or Exploratory.

The final documentation pass should describe the verified result rather than the
original implementation plan.

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

# Authoring Guides and Tutorials

Technical documentation is not a replacement for designer-facing usage
documentation.

Whenever a system introduces or materially changes a designer-facing authoring
workflow, assess whether its Tutorial and/or How-To documentation must also be
created or updated.

Use:

`Guides/Tutorials/`

for guided end-to-end learning workflows.

Use:

`Guides/How-To/`

for focused task-oriented instructions.

## Authoring Guide Standard

A comprehensive authoring guide should normally document:

- purpose;
- exact Unity menu / asset / scene location;
- prerequisites;
- what the workflow creates or modifies;
- a short quick-start workflow;
- detailed step-by-step Unity instructions;
- at least one concrete project example;
- important authored fields and their meaning;
- expected result;
- validation;
- common mistakes and troubleshooting;
- manual Inspector escape hatch where relevant;
- related Design / Technical documentation.

Do not assume familiarity with an authoring tool merely because its technical
architecture is documented.

A designer should not need to inspect source code to understand how to use an
approved content-authoring workflow.


## Documentation Change Instruction Format

When the user is expected to update documentation locally, provide changes with the same precision as source-code edits.

For every documentation change:

1. Give the complete project-relative document path.
2. Name the exact existing heading.
3. State whether to insert, replace or delete content.
4. Provide complete paste-ready Markdown.
5. Identify obsolete text that must be removed.
6. Explain the intended final document state.
7. Identify any related documents that require consistency checks.

Prefer complete coherent section replacements.

When a document has become substantially stale, prefer a planned whole-document replacement over numerous scattered patches.

Never require the user to infer where suggested Markdown belongs.

Do not present a list of subjects to document as though it were a complete documentation-edit instruction.

Keep normal documentation closure after focused implementation verification, except for the small preflight corrections required to establish accurate starting context.


## Documentation Impact

At the end of a meaningful implementation step, assess separately:

- Design Documentation Impact;
- Technical Documentation Impact;
- Authoring Guide Impact.

When an existing authoring workflow changes, update its guide in the same
development step rather than allowing usage documentation to become stale.