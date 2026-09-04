# Targeting, Tactics and Encounter Direction — Technical Design

**Status: Working**

Related Design:
- [[Combat]]
- [[Support Characters]]
- [[UI-UX]]
- [[Enemies]]
- [[Expeditions]]
- [[Level Authoring and Encounters]]

# Purpose

Define reusable decision-making seams for:

- hero autonomous targeting;
- support-character tactics;
- encounter-directed movement;
- future AI behaviour where appropriate.

The goal is shared primitives without forcing every actor to use the same
high-level policy.

# Core Separation

## Combat Eligibility

Answers:

"Can these two actors currently participate in autonomous combat with each other?"

Examples:
- alive / dead;
- faction;
- combat engagement;
- future encounter restrictions.

Current 2.9 direction uses combat engagement as an explicit runtime concept.

## Encounter Direction

Answers:

"Where does the player want this character / party to go?"

This is a navigation / strategic layer rather than an individual-target decision.

Example:

Selected Encounter:
    Nobleman's Procession

The party travels toward that encounter when it does not have a higher-priority
local combat requirement.

## Target Policy

Answers:

"Which valid combat target should this actor prefer?"

A target policy should be composable rather than represented by one giant enum.

## Action Policy / Tactics

Answers:

"What should an autonomous support do next?"

This may choose:
- attack;
- heal;
- buff;
- defend;
- use an ability;
- future support actions.

Action selection should remain separate from target selection.

## Action Execution

The chosen action is executed through the appropriate gameplay system.

Decision logic should not duplicate ability / attack / healing implementation.

# Hero Target Resolution

Working target resolution:

Eligible candidates
    ↓
Selected encounter / directive influence
    ↓
Priority modifiers
    ↓
Base target rule
    ↓
Fallback / tie-break

# Base Target Rules

Potential initial rules:
- Closest
- Weakest
- Strongest / Highest Health

More selectors can be added without requiring every combination to become a
separate mode.

# Composable Priorities

Examples:
- Prefer Elite
- Prefer Boss
- Prefer Rare / Valuable
- future status / build-specific preferences

Example:

Base:
    Closest

Priority:
    Prefer Elite

This should not require a dedicated mode called:

ClosestButPreferElite.

# Support Tactics

A support tactic conceptually contains:

Condition
Action
Target Query
Priority

Example:

Priority 1
Condition:
    Hero below 50% health
Action:
    Heal
Target:
    Hero

Priority 2
Condition:
    Elite exists
Action:
    Attack
Target:
    Highest-health Elite

Priority 3
Action:
    Attack
Target:
    Closest Enemy

The exact data representation is not yet locked.

Avoid a giant universal enum containing every possible combined behaviour.

# Shared Primitives

Where practical, hero targeting and support tactics should reuse concepts such as:

Target selectors:
- Closest
- Lowest Health
- Highest Health

Filters:
- Enemy
- Ally
- Alive
- Engaged
- Elite
- Boss
- Rare / Valuable

Priorities:
- Prefer Elite
- Prefer Boss
- Prefer Rare

These primitives may later also be useful to enemy AI, but enemy AI should not
be coupled to player-owned configuration.

# Runtime / Persistent Separation

Player configuration may be persistent/preparation-owned.

Runtime evaluation belongs to the active actor / runtime policy.

Do not store temporary evaluation state inside ScriptableObject definitions.

# Important Invariants

Encounter selection is not individual enemy selection.

Target selection is not action selection.

Action selection is not action execution.

Support tactics should not contain healing / ability gameplay implementation.

A targeting priority should influence candidate preference rather than create a
new hard-coded targeting mode for every possible combination.

# Open Questions

- Whole-party or per-character encounter directives?
- Can the hero override support encounter selection?
- How strongly should encounter selection bias target selection?
- Can priorities stack?
- How are conflicting priorities ordered?
- Can target policy be changed during combat?
- How much of support tactics can be edited during an expedition?
- How does an actor react if its chosen target becomes unavailable?