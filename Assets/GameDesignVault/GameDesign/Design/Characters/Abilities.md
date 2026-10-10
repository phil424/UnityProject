# Overview
> Abilities exist independently of their acquisition source.

Can come from:
- characters
- weapons
- equipment
- augments
- run rewards.

Player activation is normally deliberate.

# Ability Activation
Potential activation types:
- instant
- targeted enemy
- directional
- ground target
- cast
- channel
- charge.

Rules:
- immediate response to input
- player can interrupt autonomous attacks
- precision should not transform the game into a twitch action RPG.

# Ability Evolution
Core rule:
> Finding an already-owned ability should create an opportunity to transform it.

Example:
```
Whirlwind
├── Vortex
│   Pull enemies inward
├── Serrated Storm
│   Apply Bleed
└── Cyclone
    Larger area
```

Open questions:
- branching trees?
- mutually exclusive upgrades?
- multiple evolutions?
- evolution depth?
- difference between evolution and mutation?

# Ability Catalogue
Template:
```
# Ability Name

## Fantasy

## Activation

## Combat Role

## Base Behaviour

## Cooldown / Resource

## Targeting

## Interrupt Behaviour

## Possible Sources

## Evolution Ideas

## Synergy Tags

## Open Questions
```

# Core Character Ability Roles

**Status: Confirmed / Implemented Authoring Contract**

Every playable Character begins an Expedition with a three-part core:

Signature
+
Utility
+
Sustain.

These are roles in the Character's starting Ability loadout.

They are not permanent categories on `AbilityDefinition`.

A reusable Ability may therefore participate in different ownership /
acquisition contexts without changing its generic identity.

## Signature

Defines a major piece of the Character's deliberate combat identity.

It should normally provide something the Character's autonomous Basic Attack
does not already accomplish.

Confirmed core direction:

Punchy
→ Blink.

Ranger
→ Power Shot.

## Utility

Provides tactical intervention such as:

- movement;
- control;
- positioning;
- escape;
- setup;
- disruption.

Confirmed core direction:

Punchy
→ Whirlwind.

Ranger
→ Backstep.

Whirlwind creates breathing room through radial Knockback + Stun.

Backstep creates immediate departure control through Knockback + Stun before
Ranger retreats to restore ranged spacing.

Charge remains valid reusable Ability content but is no longer one of Punchy's
planned three core roles.
## Sustain

Provides a reliable survival intervention.

Sustain does not imply one shared defensive mechanic.

The current Character kits deliberately express Sustain differently.

### Punchy

**Status: Implemented Foundation / Balance Working**

Punchy's Sustain is reactive recovery.

It provides:

direct healing
+
temporary Armour.

Both scale upward as Punchy becomes more injured.

Healing combines:

flat healing
+
a percentage of currently missing Health.

Armour combines:

a base Armour increase
+
additional Armour scaled by missing-Health fraction.

The intended decision is:

use early
→ modest recovery / protection;

or

risk waiting until low Health
→ substantially stronger recovery / protection.

The final player-facing Ability name and exact numerical balance remain working.

### Ranger — Guard

**Status: Implemented Foundation / Balance Working**

Guard provides:

stackable hit-based Shields
+
healing over time.

A Shield has:

no time duration;
an authored number of hits required to break it.

Repeated Guard activations may add additional Shield layers.

Incoming hits are fully absorbed while Shield layers remain.

Guard's healing-over-time effect has its own duration and refreshes when Guard is
reapplied rather than stacking duplicate copies of itself.

This creates a different Sustain identity from Punchy:

Punchy
→ react to lost Health with stronger recovery and Armour.

Ranger
→ establish persistent hit protection and a longer regenerative window.

Dedicated Shield HUD / VFX presentation remains future presentation work.

## Core Versus Expedition Abilities

The three core roles define what a Character brings into an Expedition.

They do not impose a hard three-Ability limit on the completed Expedition build.

Additional Abilities may still be acquired through Expedition rewards.

Therefore:

Character core
→ Signature / Utility / Sustain.

Expedition build
→ core Abilities
+
additional acquired Abilities
+
levels
+
Evolutions.

This distinction preserves strong Character identity while allowing Ability
buildcraft to expand during the Expedition.

# Punchy Ability Development

## Blink — Signature

**Status: Implemented Foundation / Balance Working**

Blink is Punchy's Signature Ability.

The current implementation traverses a route through valid enemies in the
surrounding combat space.

The first traversal point prefers Punchy's current combat target.

Later traversal points deliberately favour distant unvisited enemies from the
previous point, causing Blink to cross back and forth through the enemy group.

This gives the Ability a sigil-like traversal shape without requiring a
separate authored path.

More valid enemies create more useful traversal opportunities up to the
Ability's current authored bounce cap.

Each visited enemy may receive:

- low base damage;
- Knockback;
- Stun.

Base damage is deliberately secondary.

Future buildcraft may allow players to lean further into damage or control.

Punchy is invulnerable for the active Blink traversal.

Invulnerability ends when the Ability execution finishes.

The current gameplay route does not yet have dedicated sigil / trail
presentation.

That can be layered onto the existing traversal points later without changing
Ability ownership or target selection.

The old extreme-stat Blink prototype is retired.

Its development/testing purpose is now served by a Character-agnostic debug
Party Overdrive action instead.

## Whirlwind — Utility

**Status: Implemented Foundation / Balance Working**

Whirlwind is Punchy's Utility / space-creation Ability.

Its primary purpose is:

radial Knockback
+
Stun

to create a breathing window in close combat.

Base damage remains intentionally secondary to control.

Vortex remains preserved as proof content but is parked from normal Reward
generation because its inward-pull behaviour currently opposes Whirlwind's core
space-creation role.

## War Cry — Sustain

**Status: Implemented Foundation / Balance Working**

War Cry is currently Punchy's Sustain Ability.

War Cry is shared Ability content rather than Punchy-owned behaviour.

Punchy currently uses it to provide reactive:

- direct healing;
- temporary Armour.

Both become more valuable as Punchy loses Health.

This allows the Ability behaviour to remain reusable by other Characters or
content sources while Punchy's Character loadout owns the current Sustain role.

# Ranger Ability Development

## Power Shot — Signature

**Status: Implemented Foundation / Balance Working**

Power Shot is Ranger's Signature Ability.

Baseline Power Shot is a three-shot sequential piercing burst.

The current firing sequence is intentionally aligned with the authored Ranger
Power Shot sprite animation.

Initial timing uses approximately:

Shot 1
→ 0.65 seconds.

Shot 2
→ 0.80 seconds.

Shot 3
→ 0.95 seconds.

Execution finishes at approximately 1.10 seconds, matching the current
presentation duration.

All three baseline shots:

- share the firing direction established on activation;
- may Pierce;
- deal damage;
- apply Knockback;
- apply Stun.

The direction remains valid for the entire burst even if the initial target dies
after an earlier shot.

Ability levels provide incremental development through values such as:

- damage;
- range;
- pierce count;
- cooldown;
- hit radius;
- knockback;
- Stun.

Persistent Character Mastery may increase Power Shot's starting Expedition
level.

Current proof:

`Power Shot Drills`
→ persistent prepared starting-level increase.

Expedition currency may then develop the same Ability level temporarily during
the active Expedition.

### Fan Volley

Fan Volley remains the current Power Shot Evolution.

Instead of adding additional simultaneous projectiles, Fan Volley transforms the
existing three-shot burst into an authored spread:

Shot 1
→ angled left.

Shot 2
→ centre.

Shot 3
→ angled right.

The side shots currently use reduced damage relative to the central shot.

Fan Volley therefore trades concentrated directional pressure for wider
battlefield coverage while preserving the same three-shot animation and burst
structure.

Exact timing, spread, damage and control values remain working balance.

## Backstep

A contextual spacing tool.

It should normally become useful when Ranger is closer to a threat than the
Character's preferred ranged spacing.

## Guard

A mitigation-based Sustain Ability.

The short blocking presentation communicates activation while the longer
defensive aura communicates the remaining buff duration.