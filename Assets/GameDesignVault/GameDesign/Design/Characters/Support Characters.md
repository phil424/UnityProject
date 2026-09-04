# Support Characters

**Status: Working**

Related:
- [[Characters]]
- [[Combat]]
- [[Progression]]
- [[UI-UX]]
- [[Targeting, Tactics and Encounter Direction]]

# Overview

Support characters are unlockable characters that accompany the hero during an
expedition and contribute to the party without becoming directly controlled
action-RPG characters.

Supports may contribute through:
- direct combat;
- ranged attacks;
- healing;
- buffs;
- debuffs;
- protection;
- crowd control;
- utility;
- other specialised behaviour.

The support-character model should allow significantly different support roles
rather than assuming every support is a weaker copy of the hero.

# Persistent Progression

Current direction:

Support characters are persistent unlocks acquired outside the temporary run.

They are selected and configured during preparation / hub activity before or
between expeditions.

Potential preparation includes:
- selecting which supports accompany the hero;
- configuring tactics;
- selecting preset behaviour;
- future equipment / progression;
- future abilities or other support-specific customisation.

The maximum number of active support characters is intentionally unresolved.

# Player Control Philosophy

Support characters remain autonomous.

Player control comes from configuring how they make decisions rather than
manually issuing every individual combat command.

The desired inspiration is similar to:
- Dragon Age: Origins tactics;
- Final Fantasy XII gambits.

The system should allow deep customisation without requiring every player to
engage with that complexity.

# Three Levels of Tactics Complexity

## Default Behaviour

A player who does not configure anything should receive sensible behaviour.

A healer should generally heal sensible targets.

A combat support should generally participate effectively.

The player should never need to understand the tactics system simply to make a
support functional.

## Behaviour Presets

Players may choose prepared behaviour packages.

Examples:
- Combat Support
- Frontline
- Defensive Support
- Elite Killer
- Healer
- Hero Support
- Ranged Support

These are player-facing presets built from the same underlying tactics system
rather than separate hard-coded AI systems.

## Full Custom Tactics

Players who want deeper control can define ordered tactical rules.

Example:

1. Heal hero if health is below 50%.
2. Buff hero if buff is not currently active.
3. Attack an Elite if one is available.
4. Otherwise attack the highest-health enemy.

The exact rule authoring interface is unresolved.

# Tactics Model

Working direction:

A tactical rule conceptually answers:

IF
    some condition is true

DO
    some action

TO
    a selected target

Rules are evaluated according to player-defined priority.

Example:

Condition:
    Hero health < 50%

Action:
    Heal

Target:
    Hero

Another example:

Condition:
    Elite enemy exists

Action:
    Attack

Target:
    Highest-health Elite


# Relationship to Hero Targeting

Support tactics should share targeting primitives with the hero's autonomous
targeting wherever practical.

Shared concepts may include:
- closest target;
- weakest target;
- strongest / highest-health target;
- lowest-health target;
- Prefer Elite;
- Prefer Boss;
- Prefer Rare / Valuable;
- faction;
- combat engagement;
- encounter membership.

However:

Target selection
!=
Action selection
!=
Action execution.

A support deciding to heal should use the same general target-query concepts as
combat targeting without turning the hero targeting system into a support-AI
system.

# Open Questions

- How many supports can accompany the hero?
- Is support capacity fixed, unlockable, or build-dependent?
- Can supports equip weapons / armour / augments?
- How deeply do supports participate in run buildcraft?
- Are tactics configured per support or partly shared?
- Are custom tactics unlimited or constrained by slots / complexity?
- Can tactics be changed during an expedition?
- Can presets be edited as starting templates?
- Do supports receive individually selectable encounter directives?
- What happens when a support is defeated?
- Can support-specific progression unlock new tactical possibilities?