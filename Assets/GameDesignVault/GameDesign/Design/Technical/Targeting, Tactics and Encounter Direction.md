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

> Which encounter does the player want the hero / party to pursue?

Encounter direction is a strategic navigation layer and is separate from
individual enemy target selection.

Selecting an encounter is an immediate player command.

Expected flow:

Current Encounter A
        ↓
Player selects Encounter B
        ↓
Encounter Directive = B
        ↓
drop autonomous targets belonging to A
        ↓
immediately travel toward B

Actors belonging to Encounter A are not forced to disengage.

They may continue pursuing.

The hero should not automatically reacquire Encounter A while deliberately
redirecting toward B.

This permits deliberate encounter stacking:

Encounter A pursues
        +
hero reaches Encounter B
        ↓
both encounters may participate in the resulting combat

The exact travel/pathing implementation remains open.

## Ambient Travel Intent

If no encounter is currently selected, the hero should not become stationary.

The 3.0E prototype establishes:

No Encounter Directive
        ↓
Ambient Route Navigation
        ↓
local combat appears
        ↓
combat temporarily interrupts route travel
        ↓
combat resolves
        ↓
ambient route resumes

Ambient travel uses the same generic `ActorNavigationIntent` seam as explicit
encounter travel.

The priority difference is intentional.

### Explicit Encounter Travel

Encounter navigation uses:

`SuppressCombatTargeting = true`

The player has issued an immediate strategic command.

Normal combat acquisition should not prevent that command from being obeyed.

### Ambient Travel

Ambient navigation uses:

`SuppressCombatTargeting = false`

Ambient travel is fallback/autonomous behaviour.

Local combat may interrupt it.

Once combat has resolved, ambient travel resumes.

Working priority:

Forced Motion
↓
Ability Action Lock
↓
Explicit Encounter Navigation
↓
Local Combat
↓
Ambient Route Navigation

This distinction allows:

> Go to that encounter now.

to behave differently from:

> Nothing selected; keep exploring automatically.

## Current Encounter Foundation

`LevelEncounter` currently provides:
- encounter identity;
- world-space anchor;
- lifecycle state;
- selectability;
- owned spawn groups.

Future encounter-directed targeting will also require runtime actors to be
identifiable as belonging to a particular encounter.

The exact runtime representation of encounter membership is not yet implemented.

Conceptually:

Runtime Actor
        ↓
Encounter Membership
        ↓
Targeting can answer:
"Does this actor belong to the encounter I am leaving / pursuing?"

This should remain separate from faction and combat engagement.

### 3.0A Prototype Foundation

The first strategic-direction prototype introduces:

`EncounterMembership`
- runtime actor → owning `LevelEncounter`.

`ActorNavigationIntent`
- generic actor-owned destination intent;
- independent from combat target selection;
- owned by the system currently issuing the travel intent.

`EncounterDirectionController`
- prototype whole-party encounter directive;
- converts encounter selection into runtime navigation intents.

Current movement priority:

Forced Motion
↓
Ability autonomous-action lock
↓
Navigation Intent
↓
Combat / Support target movement

While travelling toward an explicitly selected encounter, normal combat target
acquisition is suppressed.

Support targeting may remain available independently from movement so future
support actors can continue providing nearby utility while following a strategic
travel directive.

For the 3.0A prototype, reaching the selected encounter's anchor performs the
common-case behaviour:

BeginSpawning
+
ActivateCombat

This is a prototype default rather than a final authored encounter-presentation
rule.

The whole-party versus per-character directive question remains open.

## Encounter Selection and Combat Eligibility

Encounter direction influences which valid targets the hero is willing to pursue.

It does not automatically change whether enemies are allowed to attack the hero.

Therefore:

Hero leaves Encounter A
→ Hero stops choosing A targets.

Encounter A enemies remain engaged
→ They may chase / attack the hero.

Once the new encounter is engaged, the resulting combat may include pursuing
actors from previous encounters.

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
Encounter directive / encounter-membership filtering
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