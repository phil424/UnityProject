Every important word gets:

```
## Ability

A player-activatable combat behaviour that exists independently of the system that grants it.

Status: Working Definition
```

Eventually include:
- Ability
- Innate
- Utility
- Sustain
- Weapon
- Augment
- Decoration
- Evolution
- Mutation
- Upgrade
- Reward
- Pending Reward
- Expedition
- Region
- Zone
- Threat Level
- Apex
- Material
- Build
- Run Build
- Prepared Build

If we later rename something, this is where the canonical meaning lives.

## Support Character

An autonomous persistent character that accompanies the hero and acts according
to configured support / combat tactics.

Status: Working Definition

## Targeting Policy

The configured rules used by an autonomous actor to choose between valid combat
targets.

Status: Working Definition

## Targeting Priority

A preference layered onto a target policy, such as Prefer Elite or Prefer Rare.

Status: Working Definition

## Level Encounter

A player-understandable threat or opportunity in the level that exists above
low-level spawn-group implementation.

Status: Working Definition

## Encounter Directive

The player's current instruction for which encounter / destination the hero or
party should travel toward.

Status: Working Definition

## Threat Class

A description of an enemy's combat importance, such as Normal, Elite or Boss.

Status: Working Definition

## Opportunity Rarity

A description of how unusual / desirable a monster or encounter is as a reward
opportunity, independently from its combat threat.

Status: Working Definition

## Encounter Definition

Authored gameplay content describing what an encounter is independently from
one particular runtime occurrence or location.

Status: Working Definition

## Encounter Site

An authored world location capable of hosting one or more compatible encounters.

Status: Working Definition

## Runtime Encounter

One specific occurrence of an encounter during an active expedition.

Status: Working Definition

## Encounter Commitment

The state reached when the player selects or starts an encounter such that
normal expiry should no longer remove it.

Status: Working Definition

## Encounter Supply

The process responsible for maintaining appropriate current and future encounter
opportunities during an expedition.

Status: Working Definition

## Forecast / Schedule

The player-facing representation of planned upcoming encounters, world events
and expedition changes.

Status: Working Definition

## Escalation

The long-term increasing danger level of an expedition.

Status: Working Definition

## Intensity

The amount of immediate gameplay pressure being experienced at a particular
moment.

Intensity may rise and fall while Escalation continues rising.

Status: Working Definition

## World Event

A temporary expedition or regional condition that affects the world rather than
representing one selectable encounter.

Status: Working Definition

## Ambient Route

A designer-authored connected path used to guide ambient population and
automatic exploration through interesting parts of the map.

Status: Working Definition

## World Time

The accelerated 24-hour clock used to represent temporal progression during an
expedition.

Status: Working Definition

## Expedition Day

The current numbered world-time day within an expedition.

Status: Working Definition

## Daily Schedule Profile

Data describing recurring time-of-day tendencies or event windows used by the
expedition scheduling system.

Status: Exploratory Definition

## World Timestamp

A unique expedition time including both day and time-of-day.

Unlike a bare `18:00`, a world timestamp distinguishes Day 1 18:00 from Day 4
18:00.

Status: Working Definition