# Overview
Principles:
- combat information should be readable quickly;
- do not force unnecessary interruptions;
- detailed information can be optional/collapsible;
- major rewards deserve strong presentation;
- UI should communicate future threats, not only current state.

# Preparation / Character Selection

**Status: Working**

The current preparation screen does not yet provide the desired experience.

During the first 3.0N1 observational playtest it felt:

- visually uninteresting;
- information-heavy;
- easy to skip without meaningful consideration;
- more like an administrative prototype than an important part of the game.

The preparation screen should eventually become a visually appealing place where
the player can:

- choose a Character;
- understand how that Character plays;
- inspect the Character's current progression;
- compare unlocked Characters;
- prepare for the next expedition;
- become excited about beginning another run.

## Current Implemented Direction

The Preparation screen uses the actual gameplay world as its presentation.

Broad composition:

``` text
┌──────────────────────────────────────────┬─────────────────────┐
│                                          │                     │
│             LIVE GAME WORLD              │   PREPARATION UI    │
│                                          │                     │
│ selected Character travels the authored  │ Overview            │
│ Route, encounters ambient enemies and    │ Progression         │
│ fights using the normal runtime systems  │ Character select    │
│                                          │ Start Expedition    │
│                  ~2/3                    │        ~1/3         │
└──────────────────────────────────────────┴─────────────────────┘
```

## Character Unlock Presentation

**Status: Implemented Functional Foundation**

Visible locked Characters use the Character selector itself as the primary
functional unlock surface.

A locked Character currently communicates:

Character identity
+
locked state
+
authored unlock requirement
+
persistent progress.

When the requirement is satisfied, the same Character entry becomes selectable
and displays an `UNLOCKED` acknowledgement.

This is intentionally functional rather than final presentation.

A larger unlock animation, reveal or celebration may be added later if playtests
show that Character acquisition deserves stronger presentation.

# HUD Information Priority

The first N1 playtest exposed significant information overload.

Normal HUD information should be classified as:

## Immediate

Requires attention now.

## Near-Term

Relevant to an upcoming decision.

## Contextual

Useful when the player deliberately opens a larger strategic view.

The existence of a gameplay system does not automatically justify permanent HUD
space for it.

# Tracked Goals

Working direction:

The expedition HUD should eventually provide a compact tracked-goal area.

Potential tracked information:

- primary Progression Target;
- active Expedition Objectives;
- one active Hunt;
- manually pinned Achievement progress.

Only information relevant to the player's current intentions should normally be
shown.

The full set of goals belongs in an expanded log rather than the passive HUD.

# Combat HUD
Document current layout vision.

### Bottom Left
- hero portrait
- health
- buffs/debuffs
- possibly core character ability.

### Left
- active ability chevron/diamond.

### Bottom Right
- currencies
- materials
- loot feedback.

### Top Right
- DPS
- ability performance
- expandable analytics
- combat feed.

### Encounter Space
- normal enemy health above enemies
- major boss meter on HUD.

# Ability HUD

**Status: Implemented Diamond Six-Slot Presentation**

The combat Ability HUD represents six fixed Active Ability positions.

Current structure:

Prepared/Core
→ Signature
+ Utility
+ Sustain

Expedition
→ Slot 1
+ Slot 2
+ Slot 3.

The current functional HUD presents these as two groups of three compact diamond
Ability controls.

The overall Combat HUD position remains Working and may move later after the
Ability and Weapon Mod presentations are assessed together.

## Diamond Ability Presentation

Each occupied Ability position prioritises:

Ability icon
→ primary recognition.

Ability level
→ small `Lv.X` text.

Ability name
→ small supporting text beneath the icon.

Cooldown
→ numeric remaining time displayed over the icon.

Cooldown state
→ dark translucent radial wash that recedes as the Ability becomes ready.

When the Ability becomes ready:

- the cooldown wash disappears;
- the numeric countdown disappears;
- the icon remains unobscured.

Large role/status labels such as `SIGNATURE`, `UTILITY`, `READY` and
`EXPEDITION 1` are not displayed inside the combat button.

The Prepared/Core versus Expedition distinction is communicated by the two HUD
groups rather than repeated on every Ability.

## Empty Expedition Positions

Empty Expedition positions remain visible as empty diamonds.

They communicate remaining run-level Ability capacity.

An empty position displays:

- no Ability icon;
- no level;
- no cooldown;
- a small `EMPTY` label.

## Ability Artwork

Combat HUD artwork comes from the Ability's authored presentation.

Base Ability:
`AbilityDefinition.Icon`.

Presentation-replacing Evolution:
`AbilityEvolutionDefinition.Icon`.

Runtime presentation resolves through `AbilityPresentationResolver`, so evolved
Abilities may replace both their displayed name and icon without changing the
HUD implementation.

## Ability Loadout

The existing `ABILITY LOADOUT` control remains beneath the active Ability
presentation.

It opens the paused Expedition loadout-management surface and allows owned
Expedition Abilities to be slotted, swapped or unslotted.

Prepared/Core Abilities remain fixed during the Expedition.

## Current Boundary

The diamond presentation is now the intended functional Ability-button language.

Future UI work may still refine:

- exact Combat HUD placement;
- frame/art treatment;
- elemental colouring;
- input prompts;
- animation/pulse feedback;
- deeper cooldown polish.

Do not return to the previous large rectangular text-button presentation.

## Chevron Composition

The six Active Ability positions use two vertically stacked three-slot chevrons.

Expedition group:

- Slot 1 → top-centre;
- Slot 2 → lower-left;
- Slot 3 → lower-right.

Prepared/Core group:

- Signature → top-centre;
- Utility → lower-left;
- Sustain → lower-right.

The Ability cluster is anchored on the left side of the Combat HUD above the
Character / targeting controls rather than at bottom-centre.

This preserves the large central combat viewing area.

The current functional slot geometry uses approximately:

- 108 × 108 slot footprints;
- 60 horizontal centre offset;
- 54 vertical centre offset.

The visible diamond nearly fills that footprint.

A future Weapon Mod chevron may extend the same spatial language upward, but
Weapon Mod presentation remains unchanged for now.

The vertical stack uses `-54` spacing between the two chevron groups.

This intentionally matches the per-chevron `54` vertical offset.

The diagonal relationship is therefore consistent both inside a three-slot
chevron and across the boundary between Expedition and Prepared/Core groups.

Do not tighten the inter-group overlap independently from the chevron's own
vertical offset.

## Pointer Interaction

The rectangular RectTransform used by Unity layout is not the Ability's clickable
shape.

`AbilityButtonUI` performs diamond-shaped raycast filtering.

Clicks outside the visible diamond but inside its rectangular layout footprint
are rejected.

This allows neighbouring staggered diamonds to remain visually compact without
one button stealing pointer input from another.

Controller bindings remain future work.

# Weapon Mod HUD

**Status: Implemented Functional Foundation**

Weapon Mods use a separate tactical control surface from Abilities.

Abilities answer:

> What tactical action should I perform now?

Weapon Mod stances answer:

> How should my autonomous Basic Attacks behave now?

The functional O2C HUD uses four fixed Weapon Mod stance positions.

Each position should communicate:

- whether a stance is available;
- the Mod identity;
- its current Expedition level;
- whether it is currently active.

Before a Character has unlocked their Core Mastery, the HUD represents the
Character as using their original Basic Attack.

Once the Core Mastery is unlocked, Slot 1 becomes the Character's permanent
default stance.

The original Basic Attack is no longer selectable.

The four fixed positions then represent:

1. Core Mastery;
2. Alternate Mastery;
3. Alternate Mastery;
4. Alternate Mastery.

Selecting an alternate Mastery activates it.

Selecting the currently-active alternate Mastery again returns to the Core
Mastery.

Selecting the Core Mastery directly also returns to the Core Mastery.

When the Core Mastery itself is active, it cannot be toggled off into the
original Basic Attack.

A separate header displays either:

BASIC ATTACK
→ before Core Mastery ownership;

or:

the currently-active Weapon Mastery and its effective level.

When the Core Mastery is active, it is identified as the Character's default
Mastery.

This remains functional layout rather than final visual composition.

# Strategic Combat HUD

The passive combat HUD should remain glanceable.

Detailed information belongs in deliberately opened expanded panels.

## Information Hierarchy — N1 Finding

**Status: Working**

The first N1 observational playtest identified significant early-expedition
information overload.

The player was simultaneously presented with concepts including:

- World Time;
- forecast entries;
- Encounter choices;
- minimap information;
- active Abilities;
- Active Encounters;
- day/night state.

Although these systems are individually useful, exposing them with similar
visual priority makes the first minute difficult to parse.

The HUD should therefore use stronger progressive disclosure.

Working principle:

> Show what requires attention now.
> Keep useful context visible only when it can remain genuinely glanceable.
> Move explanation and lower-priority strategic detail into expanded views.

Future HUD review should classify information into at least:

### Immediate

Requires possible action in the next few seconds.

Examples:
- health;
- Ability availability;
- important active combat state;
- urgent threat/event warning.

### Near-Term

Relevant to a decision likely to occur soon.

Examples:
- selected Encounter;
- immediate Encounter alternatives;
- short forecast countdown;
- current Objective progress.

### Contextual

Useful when the player deliberately asks for strategic information.

Examples:
- full forecast;
- detailed World Time schedule;
- complete Encounter browser;
- expanded map;
- detailed World Event explanation.

The existence of a gameplay system does not automatically justify persistent
screen space for it.

Primary HUD questions:

Top Left:
> What is happening next?

Top Right:
> Where am I and where can I go?

Bottom Left:
> How am I fighting?

Bottom Right:
> What have I earned / what decisions are pending?

Large portions of the screen should remain unobstructed so watching combat stays
central to the experience.

## 3.0F Forecast HUD Prototype

The first normal-play schedule presentation occupies the top-left strategic
information area.

Prototype layout:

DAY 1  08:17

UPCOMING
00:13  Reinforcements Mobilise
13:00  Rare Activity Window
18:00  Zombie Surge

Presentation rule:

Immediate events
→ use an actionable countdown.

Longer-range events
→ use World Time.

The prototype immediate threshold is approximately 30 simulation seconds.

Because World Time uses scaled simulation time:
- Fast causes countdowns to fall faster in real time;
- Slow causes them to fall more slowly;
- Pause freezes them.

The passive forecast should remain small and glanceable.

Detailed schedule explanation belongs to the future expanded strategic planner.

## 3.0I Canvas Strategic HUD

3.0I begins migrating proven strategic information away from prototype IMGUI
panels into the real combat Canvas.

Top Left:
- World Time;
- next three forecast entries.

Top Right:
- Current World Events;
- schematic minimap;
- three quick encounter choices.

The first minimap is deliberately schematic.

It projects:
- party position;
- quick encounter anchors;
- current directive;

into configured world bounds.

It does not yet require:
- terrain rendering;
- enemy dots;
- fog of war;
- camera-based RenderTexture;
- full level geometry.

The purpose is to prove the relationship between:

Where is the opportunity?
+
Which quick slot represents it?
+
What have I currently selected?

Map markers and encounter-list entries use identical quick-slot symbols.

The slot owns its symbol:

Triangle
Square
Circle

If the encounter occupying a slot changes, the corresponding map marker changes
location with it.

The current Encounter Directive may additionally use a Star marker if it is not
currently represented by one of the three quick slots.

The Canvas implementation should continue consuming existing gameplay-facing
systems rather than owning encounter selection, supply, schedule or event state.

### Quick-Slot Presentation

Quick-slot identity is semantic data, not text.

The three prototype slots are:

Slot 0
Slot 1
Slot 2

Their current presentation is:

Triangle
Square
Circle

Critical slot symbols should use Sprite / Image or dedicated UI Graphics rather
than Unicode font characters.

This ensures:
- font replacement cannot break controller symbols;
- map and list can share identical visual assets;
- future platform-specific controller presentation can replace the icon layer
  without changing gameplay-facing slot identity.

### UI Authoring

Repeated strategic HUD items should use cohesive View + Group components.

The parent strategic HUD should consume:
- encounter list group;
- minimap view;
- forecast view/data;
- world-event view/data;

rather than maintaining parallel arrays for every child Button/Text/Marker.

# Upcoming Schedule

The passive HUD should display the next three upcoming encounter arrivals or
other immediately relevant schedule entries.

Example:

UPCOMING

00:18 Graveyard Horde
00:42 Nobleman's Procession
01:05 Elite Patrol

Short actionable transition / expiry countdowns should generally be around
20–30 seconds maximum.

The expanded strategic view may expose:
- the detailed next 5–10 minutes;
- major expedition events significantly further ahead.

Long-range presentation may resemble a planner / calendar.

# World Clock Presentation

The passive combat HUD should show current world time near the simulation /
schedule information.

Working presentation:

DAY 2
17:42

The clock should remain compact and immediately readable.

The expanded strategic view may use world time as the backbone of a larger
planner / calendar timeline.

Example:

NOW — 17:42

17:50
Church Horde

18:00
Rare Activity

20:00
Zombie Surge

00:00
Fiery Horde

Immediate events should still provide countdowns when they enter the short
20–30 second response window.

Long-range information may use world-time labels instead of continuously
displaying large countdown values.

# Current World Events

Current map-wide or regional effects should be visually distinct from encounters.

Examples:
- Zombie Surge;
- Fiery Zombies.

A compact status area near / over the minimap is currently preferred.

These should communicate:
> What rules or pressures are affecting the region right now?

rather than:
> Where should I go?

## 3.0H Prototype

The first active-event HUD displays currently active World Events separately
from encounter choices.

Prototype:

CURRENT WORLD EVENTS

Zombie Surge   02:14

The World Event status is positioned near the existing strategic encounter area.

This remains distinct from:

UPCOMING
→ future forecast.

ENCOUNTERS
→ selectable destinations.

CURRENT WORLD EVENTS
→ rules / pressures affecting the world now.

Final layout should consolidate these elements around the minimap / strategic
HUD according to the established passive-HUD wireframe.

# Combat Minimap

The minimap answers:

> Where?

Working information:
- hero / party;
- current encounter directive;
- three quick encounter choices;
- important rare opportunities;
- major world threats where appropriate.

Do not default to displaying every normal enemy.

Quick encounter symbols must remain consistent between:
- minimap;
- encounter list;
- notifications;
- controller prompts.

# Quick Encounter Slots

The passive HUD exposes exactly three quick encounter options.

The quick-access layer also supports a compact Reward Focus.

Current focus modes:

- Balanced;
- Growth;
- Ability.

The focus is a preference rather than a hard filter.

Matching opportunities appear first, but unused slots fall back to the normal
general-purpose ordering.

Current Balanced ordering:

> The three oldest selectable encounters.

Focused ordering:

> Matching selectable encounters first, ordered oldest-first, followed by
> non-matching selectable encounters using the same oldest-first ordering.

Reason:
Older encounters are generally nearer expiry and therefore more strategically
urgent.

The world may contain more than three known encounters.

The quick slots are only the immediate-access layer.

Controller working concept:

R1 + Triangle
R1 + Square
R1 + Circle

→ select one of the three quick encounters.

Mouse / keyboard should expose equivalent direct interaction.

There should never normally be fewer than three selectable quick encounter
options.

## 3.0D Prototype

The first playable quick-selection implementation uses a dedicated
`QuickEncounterChoices` gameplay-facing selection model.

The presentation layer does not determine encounter selection rules.

Flow:

LevelEncounter availability
        ↓
QuickEncounterChoices
        ↓
focus-aware ordering
        ↓
three quick Encounter opportunities
        ↓
HUD / future controller input
        ↓
EncounterDirectionController

Encounter age is currently represented by an availability sequence.

Future scheduler / encounter-generation work may replace or supplement this with
explicit creation / availability timestamps.

The unresolved question:

> Should the current selected encounter occupy one quick slot?

is deliberately exposed as a prototype toggle.

The quick-choice provider does NOT create replacement encounters.

Guaranteeing three continuously available opportunities belongs to Encounter
Supply rather than UI.

## 3.0G Expiry Presentation

Quick encounter choices now expose remaining opportunity time.

Prototype example:

△ Opening Horde       00:42
□ Dormant Horde       ★ COMMITTED
○ South Road Horde    01:11

Selected or started encounters use:

COMMITTED

instead of an expiry countdown.

Recycled prototype encounters may temporarily show an occurrence marker such as:

x2

This marker exists for development validation and is not necessarily final
player-facing presentation.

The important player-facing information is:

> How urgent is this opportunity?

and:

> Have I already committed to it?

# Encounter Selection

Selecting an encounter immediately changes the encounter directive.

The hero should:
- drop autonomous targets belonging to the previous encounter;
- immediately begin travelling toward the newly selected encounter.

Enemies from previous encounters may continue pursuing.

This can allow deliberate encounter stacking.

# Expanded Strategic View

Working input concept:

D-pad Right
→ open strategic view
→ strongly slow simulation.

Exact slow-motion versus full pause should be playtested.

The expanded view may contain:
- larger map;
- all known encounters;
- locked encounters;
- encounter details;
- full forecast;
- long-range major events;
- threat;
- opportunity rarity;
- known modifiers;
- expected reward information.

Sorting/filter behaviour remains unresolved until the detailed panel wireframe
has been evaluated.

# Encounter Information

Encounter selection should be informed.

The player should generally understand significant properties before committing.

Possible information:
- threat;
- opportunity rarity;
- enemy composition;
- notable Elite / rare enemies;
- world modifiers;
- encounter modifiers;
- expected reward category;
- expiry;
- prerequisites.

Use progressive disclosure.

The passive quick row should remain compact.

Detailed information belongs in the expanded strategic view.

# Targeting Controls

The normal HUD should show the current hero targeting policy near the
character/health/ability information.

Initial prototype choices:
- Closest;
- Strongest;
- Weakest.

Working controller concept:

L1 + face button
→ quick targeting choice.

Later targeting should support:
- composable priorities;
- targeting presets;
- support tactics.

Detailed targeting/tactics configuration belongs in an expanded panel, currently
associated with D-pad Down.

# Controller-Friendly UI Principle

Critical information and actions must be reachable through focus navigation.

Mouse hover may provide additional PC convenience but must never be required to:
- understand critical information;
- select an encounter;
- configure essential behaviour.

Current conceptual expanded-panel directions:

D-pad Right
→ world / encounters / schedule

D-pad Down
→ targeting / tactics

D-pad Left
→ character / build

D-pad Up
→ strong candidate for Pending Rewards

These bindings are working concepts rather than final controller mappings.

# Run Resources and Pending Rewards

Bottom-right remains the preferred passive location for:
- run currency;
- future collected resources;
- Pending Rewards.

Pending Rewards should receive stronger visual priority than passive currency
because they represent an actionable decision.

The normal HUD should communicate reward availability without forcing an
immediate interruption.

# Reward UI
Include:
- pending reward indicator
- voluntarily opened reward UI
- combat pause
- travel presentation
- reward acquisition order
- ability evolution presentation
- rarity feedback
- current reward source / provenance
- current Reward Vector

# Hub and Menus
Record options:

### 2D Frontend

### Physical Hub

### Hybrid

Do not choose.

Document that underlying persistent systems should not depend on presentation choice.

# Active Encounter Presentation

Quick Encounters answer:

> Where can I go?

Active Encounters answer:

> What have I already started and how far through it am I?

The normal HUD should support multiple simultaneous active encounters.

Prototype presentation may show:

Dormant Horde
Phase 2 / 3

Opening Horde
Phase 1 / 2

Encounter progress must not assume only one active encounter exists.

# Encounter Announcements

Encounter transitions deserve brief non-blocking presentation.

Encounter Start:

DORMANT HORDE
The dead stir along the road.

Encounter Complete:

ENCOUNTER COMPLETE
DORMANT HORDE

Announcements should:
- fade in quickly;
- remain briefly;
- fade out quickly;
- avoid blocking normal combat;
- support several announcements existing at once;
- use authored encounter identity;
- use reusable View / Group UI architecture.

Do not implement encounter announcements as one global Text field that later
events overwrite.

## 3.0J4 Prototype Implementation

Encounter identity is now surfaced during normal play.

### Active Encounters

The strategic HUD contains a separate Active Encounters section.

It displays encounters that:

`HasStarted`
+
not Completed
+
not Expired.

Phased encounters show:

`Phase N / Total`

Non-phased encounters show:

`Active`.

Multiple overlapping encounters are supported and ordered by when they entered
active gameplay.

### Transient Announcements

Encounter start and completion presentation use independent runtime
`EncounterAnnouncementView` instances inside a layout-driven feed.

This allows several announcements to coexist briefly.

Start:

Encounter Display Name
+
authored Description / fallback status.

Completion:

ENCOUNTER COMPLETE
+
Encounter Display Name.

Presentation animation uses unscaled UI time so simulation Slow / Fast does not
distort the readability window.

# Character Mastery Tree

**Status: Implemented Spatial Foundation**

Character Mastery uses a dedicated expanded Preparation overlay rather than a
flat list inside the narrow Character panel.

The gameplay-authoritative graph remains:

Mastery Nodes
+
Prerequisites.

Presentation adds:

- a bounded Character-specific freeform canvas;
- authored node positions;
- generated prerequisite connections;
- compact selectable graph nodes;
- locked / available / unlocked presentation;
- a selected-node detail / purchase panel.

Locked nodes remain inspectable.

## Navigation

The player-facing Mastery tree follows familiar freeform-canvas conventions.

Left-drag
→ pan the bounded canvas.

Mouse wheel
→ zoom around the pointer.

Middle-mouse drag
→ optional alternate desktop pan.

Click node
→ inspect that Mastery.

The tree deliberately does not expose permanent zoom / focus / reset controls
for ordinary navigation.

Ranger and Punchy may use different canvas sizes and completely different tree
silhouettes.

Current visual direction:

Ranger
→ bow / arrow-inspired progression shape.

Punchy
→ compact branching melee progression.

Tree geometry is presentation only.

`CharacterMasteryNodeDefinition.Prerequisites`

remains gameplay-authoritative.

## Authoring

Normal spatial authoring uses:

Tools
→ MiniCrawler
→ Mastery Tree Authoring.

The tool previews the Character's real bounded tree, displays prerequisite
connections and allows nodes to be repositioned directly.

Layout validation remains non-destructive.

Direct ScriptableObject Inspector editing remains available as an advanced
escape hatch.

The current UI is a functional foundation rather than final visual art.