# Documentation

**Status: Canonical**

These rules govern the embedded Obsidian vault and technical/design
documentation stewardship.

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