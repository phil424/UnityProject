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
→ Smoke Step.

Whirlwind creates breathing room through violent radial Knockback + Stun.

Smoke Step creates tactical ranged space through a short Ranger hop and
directional Fear.

Enemies caught near the departure point are guided broadly opposite Ranger's
hop direction while a weaker lateral steering force funnels them toward that
direction's centre corridor.

The intended result is:

disengagement
+
enemy formation setup

rather than long-distance Ranger repositioning.

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

Activation records Punchy's exact world position as the cast's Sigil Centre.

The Ability begins with a local control pulse around that centre:

radial Knockback
+
strong Stun.

This creates a deliberate breathing space before traversal begins.

After a short opening beat, Punchy becomes an invulnerable traversal threat and
moves through a route of valid enemies.

The route:

- prefers Punchy's current combat target first where valid;
- does not deliberately revisit an enemy during the same cast;
- favours distant unvisited targets from the previous traversal point;
- gains more value when more valid enemies are present;
- remains bounded by the authored search radius and maximum-bounce values.

Traversal impacts currently deal damage.

The opening phase owns the Ability's Knockback + strong Stun rather than
reapplying those control effects at every traversal point.

When traversal finishes, Punchy returns to the exact Sigil Centre recorded at
activation.

Invulnerability lasts through:

opening control
+
traversal
+
return.

The intended Signature rhythm is:

create space
→ disappear through the enemy group
→ return to the controlled centre.

Dedicated sigil / traversal-line VFX remain future presentation work.

The old extreme-stat Blink prototype remains retired.

Its development/testing purpose is handled by the Character-agnostic Party
Overdrive debug action.

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

War Cry remains shared Ability content rather than Punchy-owned behaviour.

Punchy uses it for reactive:

direct healing
+
temporary Armour.

Both become more valuable as Punchy loses Health.

War Cry now uses a substantially shorter baseline cooldown so that Sustain
participates more consistently in Punchy's drawn-out combat rhythm.

Current working cooldown:

Lv.1
→ 8 seconds.

Lv.5
→ 6 seconds before Character-wide Ability Cooldown Recovery.

The exact healing / Armour balance remains Working.

Current flat-Armour combat semantics prevent Armour alone from reducing a
positive damaging hit below 1 damage.

This preserves War Cry's strong low-Health protection without allowing weak
enemies to become completely incapable of damaging Punchy.

That minimum-damage rule is a working combat rule and should be reassessed once
the enemy roster contains broader damage profiles.

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

### Previous Evolution Proof — Fan Volley

**Status: Parked**

Comparative playtesting demonstrated that the three-shot spread is more
satisfying as baseline Power Shot behaviour rather than an acquired Evolution.

Baseline Power Shot now fires:

Shot 1
→ angled left.

Shot 2
→ centre.

Shot 3
→ angled right.

All three remain piercing projectile shots with Knockback + Stun.

The Fan Volley Evolution / Reward assets remain preserved as proof content but
are removed from normal Reward generation.

A future Power Shot Evolution should meaningfully transform the already-useful
baseline rather than merely restore behaviour the Ability needs from the start.

## Smoke Step — Utility

**Status: Implemented Foundation / Balance Working**

Smoke Step replaces the previous long-distance Backstep design.

Ranger performs only a short hop away from the immediate threat.

The Ability records Ranger's departure position and hop direction.

Enemies inside the Smoke radius become Feared.

Fear gives those enemies:

- a primary movement direction opposite Ranger's hop;
- a weaker lateral steering influence toward that movement axis;
- temporary prevention of basic attacks;
- temporary prevention of support actions;
- temporary prevention of new Ability activations.

Enemies remain mobile and move using their ordinary authored Move Speed.

The lateral steering creates a loose retreating corridor rather than pulling
every target to one point.

This gives Smoke Step two purposes:

immediate ranged disengagement
+
formation setup for follow-up attacks such as Power Shot.

The previous route-constrained and safe-destination Backstep experiments were
removed because tactical Ability movement should not require a parallel
navigation system.

Dedicated smoke presentation remains future presentation work.

## Guard — Sustain

**Status: Implemented Foundation / Balance Working**

Guard establishes longer-term defence rather than immediate disengagement.

Each activation may provide:

hit-based Shield layers
+
healing over time.

Ranger currently begins with a maximum Shield Capacity of 3.

Shield layers have no duration and remain until their authored hit capacity is
consumed.

Repeated Guard activations may add layers until the current Shield Capacity is
reached.

The healing-over-time effect has its own duration and refresh semantics.

This keeps Guard distinct from Smoke Step:

Smoke Step
→ escape immediate close-range pressure and reshape enemy formation.

Guard
→ prepare to remain under pressure.