# ECS Unity Tutorial / MiniCrawler — Development Rules

**Status: Canonical Entry Point**

# Purpose

This document routes development work to the correct focused rulebooks.

It should remain concise.

Detailed rules belong in the focused documents under:

`Assets/GameDesignVault/GameDesign/Development Rules/`

# Required Reading Order

For every development task:

1. read `Core Instructions - Always Follow.md`;
    
2. inspect the latest relevant project archive;
    
3. read `Workflow and Validation.md`;
    
4. read the additional focused rulebooks relevant to the task;
    
5. inspect the relevant Design / Technical / Guide documentation;
    
6. inspect the actual project files being changed.
    

Do not infer exact implementation details from old chat history when the current project can be inspected.

# Focused Rulebooks

## [[Workflow and Validation]]

**Read for every implementation or housekeeping task.**

Covers:

- source-of-truth handling;
    
- milestone structure;
    
- documentation preflight;
    
- implementation contracts;
    
- dependency ordering;
    
- explicit edit instructions;
    
- validation;
    
- snapshots;
    
- code formatting;
    
- code-health work.
    

## [[Architecture and Lifetimes]]

Read whenever ownership, state lifetime or runtime architecture is affected.

Covers:

- Persistent / Meta;
    
- Setup;
    
- Expedition / Run;
    
- Region / Level;
    
- Encounter;
    
- Runtime Actor;
    
- runtime versus progression versus presentation responsibilities.
    

## [[Unity Authoring]]

Read whenever the task changes:

- scenes;
    
- prefabs;
    
- ScriptableObjects;
    
- Inspector wiring;
    
- designer-facing Unity tooling;
    
- authoring hierarchy.
    

## [[UI Authoring]]

Mandatory for meaningful player-facing UI work.

Also read for substantial developer/designer tooling UI.

Covers:

- UI Work Contracts;
    
- hierarchy migration;
    
- RectTransforms;
    
- LayoutGroups;
    
- reusable Views;
    
- prefab workflows;
    
- runtime-generated UI;
    
- serialized references;
    
- UI validation.
    

## [[Documentation]]

Read whenever the embedded GameDesign vault is changed.

Covers:

- Design versus Technical versus Guide responsibilities;
    
- documentation status;
    
- two-pass documentation maintenance;
    
- authoring guide requirements;
    
- final documentation reconciliation.
    

## [[Abilities and Buildcraft]]

Read when changing Ability, reward or buildcraft architecture.

This is a domain-specific architectural rulebook rather than a universal workflow document.

# Universal Constraints

Across all work:

- preserve working architecture where practical;
    
- prefer explicit reusable seams over duplicate systems;
    
- do not solve distant hypothetical requirements prematurely;
    
- keep state in the correct lifetime;
    
- keep runtime, progression, presentation and debug responsibilities separate;
    
- use compact conventional C#;
    
- avoid unrelated refactoring during feature implementation;
    
- default to focused manual validation.
    

# Conflict Handling

If current implementation, Design documentation and Development Rules disagree:

1. identify the conflict;
    
2. determine which source is actually authoritative for that responsibility;
    
3. make the discrepancy explicit in the development step;
    
4. do not silently reinterpret the project.
    

# Current Project State

Current milestone status, active feature work and completed history belong in:

`ProjectManagement/Roadmap.md`

and related ProjectManagement documents.

Do not duplicate that state here.