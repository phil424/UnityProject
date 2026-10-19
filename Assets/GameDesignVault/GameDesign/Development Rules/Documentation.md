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

Documentation maintenance should be supplied in complete coherent replacement
units.

The normal unit is a headed Markdown section.

For every documentation change:

1. give the complete project-relative document path;
2. name the exact heading involved;
3. state whether the complete section is being replaced, inserted or deleted;
4. identify the boundary using the next equal-or-higher-level heading;
5. provide complete paste-ready Markdown;
6. explain the intended final state where clarification is useful.

Preferred operations:

### Replace

Replace:

`## Existing Section`

through immediately before:

`## Next Section`

with the supplied complete section.

### Insert

Insert a complete headed section immediately before or after a named existing
heading.

### Delete

Delete the complete obsolete headed section.

Do not present a list of individual sentences, paragraphs or search terms that
the user must independently locate throughout the document when a complete
section replacement can express the intended result.

When several sections of a document are stale or contradictory, prefer a
planned whole-document replacement.

Small isolated factual corrections may still be given as direct value/text
replacements when that is genuinely simpler.

Documentation closure should describe verified implementation rather than the
original implementation plan.


## Documentation Impact

At the end of a meaningful implementation step, assess separately:

- Design Documentation Impact;
- Technical Documentation Impact;
- Authoring Guide Impact.

When an existing authoring workflow changes, update its guide in the same
development step rather than allowing usage documentation to become stale.