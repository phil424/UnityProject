# ECS Unity Tutorial / MiniCrawler — Development Rules

**Status: Canonical Entry Point**

This file is the mandatory starting point for development work.

Detailed rules are split into focused documents so the relevant contract can be
read without repeatedly scanning one large monolithic file.

# Sources of Truth

Before giving implementation instructions:

1. Inspect the latest uploaded numbered `Complete.zip`.
2. Read this file.
3. Read the focused rule documents relevant to the task.
4. Inspect the embedded design/technical documentation relevant to the task.
5. Inspect the actual scripts, scenes, prefabs and assets being changed.

Do not reconstruct exact implementation details from memory when the current
project can be inspected.

If older chat context conflicts with the latest project archive, the latest
project archive wins unless the user explicitly says otherwise.

If implementation and current design disagree, identify the conflict rather
than silently choosing one.

# Focused Rulebooks

## [[Workflow and Validation]]

Read for every implementation task.

Covers:
- source-of-truth handling;
- step scope;
- snapshots;
- testing;
- explicit code instructions;
- evidence-driven diagnosis;
- validation;
- code formatting.

## [[Architecture and Lifetimes]]

Read whenever runtime ownership or architecture is affected.

Covers:
- incremental architecture;
- lifetime ownership;
- Persistent / Setup / Expedition / Region / Encounter / Actor boundaries;
- runtime versus progression versus presentation.

## [[Abilities and Buildcraft]]

Read when touching:
- abilities;
- rewards;
- RunBuild;
- augments/evolutions;
- weapons;
- buildcraft;
- forced movement / knockback.

## [[Unity Authoring]]

Read for scene, prefab and Inspector authoring work.

Covers:
- literal Unity Editor instructions;
- hierarchy ownership;
- auto-discovery;
- authoring ergonomics.

## [[UI Authoring]]

Mandatory for player-facing UI work.

Also read for substantial designer/developer tooling UI.

Covers:
- UI Work Contract;
- RectTransforms;
- TMP settings;
- LayoutGroups;
- prefab workflow;
- scene ownership;
- serialized references;
- reusable Views;
- validation;
- transient UI lifetime;
- numeric authoring controls.

## [[Documentation]]

Read when changing the embedded design vault.

Covers:
- documentation stewardship;
- status handling;
- technical-design responsibilities;
- keeping implementation and documentation aligned.

# Universal Architecture Direction

Prefer:
- small extensions;
- explicit seams;
- composition;
- data-driven behaviour;
- clear ownership;
- existing architecture where it already solves part of the problem.

Avoid:
- speculative rewrites;
- duplicate systems;
- giant managers;
- giant universal enums;
- premature future architecture;
- character/enemy special cases where a generic seam is appropriate.

# Lifetime Stack

The project currently uses:

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

Do not move state across these boundaries accidentally.

See [[Architecture and Lifetimes]] for the complete ownership rules.

# Implementation Step Contract

Every implementation step must begin with:

- Goal
- Deliberately NOT included
- Files to CREATE
- Files to MODIFY
- Files to DELETE

Unity Editor work must also identify exact scenes/prefabs/assets involved.

Player-facing UI work must additionally provide the mandatory UI Work Contract
from [[UI Authoring]].

Implementation instructions must be executable in the order presented.

Shared contract changes such as new types, constructor/signature changes,
abstract members and events require an explicit dependency/call-site audit before
instructions are given.

Exact edit instructions must identify the enclosing method, property, class or
documentation heading rather than relying on phrases such as:

- "the normal call";
- "that section";
- "the existing block";
- "update the manager".

See [[Workflow and Validation]] for the complete instruction-clarity contract.

# Testing

Do not routinely add automated/EditMode tests.

Default validation is short, focused manual validation unless:
- the user requests automated tests;
- the change genuinely warrants broader validation.

See [[Workflow and Validation]].

# Code Style

Use compact, conventional Visual Studio / Rider-style C#.

Default to horizontal readability.

Keep ordinary:
- signatures;
- calls;
- assignments;
- member access;
- lambdas;
- conditions

on one line when they remain readable.

Do not vertically explode code for presentation in chat.

Around 120–140 characters is an acceptable working range when the result is
still readable.

Do not mix unrelated formatting/refactoring churn into feature implementation.

Legacy code may instead be improved through the recurring milestone Code Health
Pass defined in [[Workflow and Validation]].

Compact single-line guards are preferred for trivial checks.

Use braces where lifecycle-critical scope must be unmistakable, especially
around spawning, encounter transitions, progression/lifetime changes, rewards,
persistence and cleanup.

See [[Workflow and Validation]] for the complete formatting rules.

# Documentation

The embedded vault is part of the project source of truth.

Update confirmed design/technical decisions alongside implementation when they
become relevant.

Do not convert exploratory ideas into requirements.

See [[Documentation]].

# Working Principle

Scope work around the next coherent, high-value gameplay or architecture
outcome.

Prefer work that:
- resolves an important design question;
- creates useful gameplay evidence;
- unlocks dependent systems;
- establishes a strong reusable foundation;
- materially improves the ability to evaluate the game.

Do not reduce a feature to the smallest possible implementation purely for the
sake of minimising scope when a broader coherent step would provide
substantially more value.

Work should still remain reviewable and should avoid unrelated speculative
expansion.

Preserve flexibility where future design is unresolved, but do not delay
playable progress to solve distant hypothetical requirements.