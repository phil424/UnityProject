# ECS Unity Tutorial / MiniCrawler — Core Instructions

**Status: Canonical / Stable**

# Purpose

This file contains only the stable operating rules for working on the project.

It must remain small, durable and independent of the current milestone.

Current roadmap state, feature plans, implementation history and temporary project status belong elsewhere in the GameDesign vault.

# 1. Source of Truth

The latest numbered `Complete.zip` supplied by the user is the authoritative checkpoint for exact project state.

When a newer working archive has explicitly been supplied after that checkpoint, inspect it when the requested work depends on those newer changes.

Before giving exact implementation instructions:

1. inspect the relevant current project archive;
    
2. inspect the embedded GameDesign vault;
    
3. read `Development Rules.md`;
    
4. read the focused rulebooks relevant to the task;
    
5. inspect the actual scripts, scenes, prefabs, assets and documentation being changed.
    

Do not reconstruct exact implementation details from memory when the current project can be inspected.

If old conversation context conflicts with the current project, the current project wins unless the user explicitly says otherwise.

If implementation and current documentation disagree, identify the conflict rather than silently choosing one.

# 2. Working Mode

Always provide comprehensive instructions showing the user how to perform the work locally.

Do not complete the Unity/project implementation for the user.

Work milestone-by-milestone in coherent, reviewable increments.

Preserve working architecture wherever practical.

Prefer extending existing seams over:

- speculative rewrites;
    
- duplicate systems;
    
- parallel replacement architectures;
    
- premature future infrastructure.
    

Give explicit file paths and literal Unity Editor instructions where applicable.

# 3. Mandatory Development Step Structure

Every meaningful implementation or housekeeping step follows this sequence:

Documentation Preflight & Cleanup  
↓  
Implementation Contract  
↓  
Implementation  
↓  
Focused Verification  
↓  
Documentation Closure  
↓  
Checkpoint Decision

## Documentation Preflight & Cleanup

Before implementation:

- inspect documentation directly relevant to the work;
    
- correct stale or contradictory information that would make the implementation instructions misleading;
    
- do not perform unrelated documentation cleanup.
    

If no pre-implementation documentation change is required, explicitly state:

`Documentation Preflight: No changes required.`

## Implementation Contract

Before implementation instructions, state:

- Goal
    
- Deliberately NOT included
    
- Files CREATE
    
- Files MODIFY
    
- Files DELETE
    

Where relevant, also provide the required Unity, prefab, scene, UI or authoring work contract from the focused Development Rules.

## Implementation

Instructions must:

- be executable from top to bottom;
    
- identify exact files;
    
- identify exact classes, methods, properties, headings or Unity objects;
    
- introduce dependencies before their consumers;
    
- include all predictable affected call sites;
    
- avoid unrelated cleanup or formatting churn.
    

## Focused Verification

After implementation, provide proportionate validation for the work just performed.

Manual validation is the default.

Automated/EditMode tests are not routinely added unless explicitly requested or genuinely justified.

Broader regression testing is reserved for changes that affect central shared systems, important lifetime boundaries or other high-risk behaviour.

## Documentation Closure

After successful verification:

- reconcile affected Design documentation;
    
- reconcile affected Technical documentation;
    
- reconcile affected Authoring Guides where applicable;
    
- remove stale future-tense or limitation wording made obsolete by the implementation;
    
- preserve unresolved ideas as Working or Exploratory rather than silently confirming them.
    

If no documentation needs changing, explicitly state that no final documentation update is required.

Documentation should describe the verified implementation, not the implementation we hoped to create before testing.

## Checkpoint Decision

Take a new `Complete.zip` when the project has reached a meaningful, useful checkpoint or when refreshed project context would materially improve future verification.

Do not create snapshots merely because one small step was completed.

# 4. Development Rules

`Development Rules.md` is the mandatory rulebook entry point.

Focused rulebooks contain the detailed contracts for:

- workflow and validation;
    
- architecture and lifetimes;
    
- Unity authoring;
    
- UI authoring;
    
- documentation;
    
- relevant gameplay/domain architecture.
    

Read only the focused rulebooks relevant to the task in addition to the universal workflow rules.

# 5. Code Style

Use compact, conventional Visual Studio / Rider-style C#.

Prefer horizontal readability where reasonable.

Avoid unrelated formatting churn during feature work.

Use explicit braces where lifecycle-critical scope must remain unmistakable.

Detailed formatting rules live in `Development Rules/Workflow and Validation.md`.

# 6. Documentation Stewardship

The embedded GameDesign vault is part of the project's source of truth.

Keep these responsibilities separate:

Design  
→ what the game should be.

Technical  
→ how the system is structured and why.

Guides  
→ how the approved workflow is actually used.

Do not allow implementation and documentation to drift indefinitely.

Do not turn exploratory future ideas into implementation requirements without an explicit design decision.

# 7. Stable-File Rule

This file must NOT contain:

- the name of the current Complete snapshot;
    
- current roadmap status;
    
- current active feature;
    
- completed milestone history;
    
- temporary implementation details;
    
- feature-specific design plans;
    
- lists of current Characters, Abilities, Encounters or content;
    
- historical chat handoff material.
    

Those belong in the roadmap, Design/Technical documentation or project history.

This file should change only when the way we work on the project genuinely changes.