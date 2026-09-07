This should be a **living backlog of questions**, not ideas.

For example:

```
## Ability Capacity

**Question:** How many player abilities can be active simultaneously?

**Current Thinking:**
Potentially around six, but slot structure is unresolved.

**Blocks:**
- Final ability HUD
- Full-loadout reward behaviour

**Need to decide by:**
Before ability loadout architecture is locked.
```

This lets me tell you:
> We don't need to answer this yet.

or:

> This has now become a blocker.


## Support Capacity

**Question:** How many support characters can accompany the hero?

**Current Thinking:**
Support characters are intended to be a meaningful persistent progression layer,
but no fixed capacity has been chosen.

**Blocks:**
- final support loadout UI
- some encounter balance assumptions

**Need to decide by:**
Before support party architecture becomes fixed.


## Support Tactics Complexity

**Question:** Are full custom tactics unlimited or constrained by slots / complexity?

**Current Thinking:**
Default behaviour, presets and full custom tactics should all exist.

**Blocks:**
- final tactics UI
- persistent support configuration data

**Need to decide by:**
Before implementing full tactics authoring.


## Encounter Direction Scope

**Question:** Does encounter selection direct the whole party or individual characters?

**Current Thinking:**
Whole-party direction is the simpler initial model, but individual support
directives may eventually be valuable.

**Blocks:**
- final encounter-directed movement model
- support navigation behaviour


## Encounter Discovery

**Question:** How much information about encounters is visible before they are discovered?

**Current Thinking:**
The map should provide strategic information, but complete omniscience may reduce
exploration and surprise.

**Blocks:**
- minimap presentation
- rare sighting presentation
- encounter HUD information model

## Active Encounter in Quick Slots

**Question:** Should the currently selected / active encounter continue to occupy
one of the three quick encounter slots?

**Current Thinking:**
The three slots display the oldest selectable encounters, but it is unresolved
whether the current destination should consume one slot or whether all three
should represent alternative destinations.

**Blocks:**
- final quick encounter HUD behaviour;
- encounter-slot promotion logic.

**Need to decide by:**
Before the quick encounter HUD leaves prototype stage.


## Ambient Combat During Encounter Travel

**Question:** How should the hero react to ambient enemies while travelling
toward an explicitly selected encounter?

**Current Thinking:**
Encounter redirection should remain dominant. It is unresolved whether the hero
should completely ignore ambient targets, attack convenient targets while
continuing to move, or use another lightweight travel-combat rule.

**Blocks:**
- final encounter-directed movement;
- ambient targeting behaviour.

**Need to decide by:**
During the strategic expedition prototype.


## Intensity Measurement

**Question:** Which runtime signals should determine current expedition intensity?

**Current Thinking:**
Potential signals include hostile population, active encounters, incoming
damage, player damage taken, healing frequency and party health.

The system should influence future pacing rather than visibly weakening already
committed fights.

**Blocks:**
- self-balancing pacing;
- escalation/intensity director.

**Need to decide by:**
Before implementing adaptive pacing.


## Generic Encounter Site Selection

**Question:** How should the encounter system select a site for a generic
encounter?

**Current Thinking:**
Generic encounters should use designer-authored compatible sites rather than
arbitrary world positions.

Potential considerations include:
- distance;
- travel flow;
- recent site usage;
- encounter compatibility;
- current ambient route;
- avoiding repetition.

**Blocks:**
- data-driven encounter generation;
- runtime encounter placement.

**Need to decide by:**
Before replacing the static encounter prototype with runtime generation.


## Strategic View Simulation Speed

**Question:** Should expanded strategic panels pause or slow simulation?

**Current Thinking:**
Strong slow-motion is currently preferred because it retains world motion while
giving the player time to plan.

Full pause may prove more comfortable.

**Blocks:**
- final strategic UI interaction.

**Need to decide by:**
During strategic-panel playtesting.


## Expanded Encounter Browser

**Question:** How much sorting, filtering and management should the full
encounter browser provide?

**Current Thinking:**
Possible views include Oldest, Newest and Expiring Soon, but the detailed panel
wireframe should be designed before committing to browser features.

**Blocks:**
- expanded strategic panel implementation.

**Need to decide by:**
After detailed strategic-panel wireframe review.

## World Day Duration

**Question:** How much real simulation time should one complete 24-hour world day
take?

**Current Thinking:**
A useful initial prototype is:

1 real simulation minute = 1 world hour

therefore:

24 real simulation minutes = 1 world day.

This is explicitly tunable.

**Need to decide by:**
World-clock playtesting.


## Expedition Start Time

**Question:** Does every expedition begin at the same world time?

**Current Thinking:**
A consistent starting phase may improve learnability, while variable start times
could increase long-term variety.

**Need to decide by:**
Before daily schedules become content-heavy.


## Day Cycle vs Escalation

**Question:** How strongly should world-time phases influence target intensity?

**Current Thinking:**
World time should create predictable rhythm while Escalation remains the
long-term difficulty authority.

**Need to decide by:**
Adaptive pacing implementation.


## Region-Specific Daily Rhythm

**Question:** Do different expedition regions eventually use different daily
schedule profiles?

**Current Thinking:**
Likely valuable, but not required for the first strategic expedition prototype.

**Need to decide by:**
Multiple-region content production.


## Daily Recurrence

**Question:** Which events should repeat every game day and which should be
one-off expedition milestones?

**Current Thinking:**
The system should support both.

**Need to decide by:**
Full scheduler implementation.

## Persistent Economy

**Question:** What resources should actually fund persistent equipment and
progression?

**Current Thinking:**
3.0C temporarily converts total expedition currency earned into an in-memory
Persistent Currency so the failure → upgrade → retry loop can be tested.

Long term, monster materials / crafting resources may replace or supplement this
currency.

**Blocks:**
- final crafting economy;
- persistent upgrade economy.

**Need to decide by:**
Before persistent progression leaves prototype stage.


## Voluntary Expedition Return

**Question:** When can the player voluntarily end an expedition and what rewards
are secured?

**Current Thinking:**
The player should eventually be able to return voluntarily rather than being
required to die.

The exact risk/reward consequences remain unresolved.

**Blocks:**
- final expedition-end UX;
- banking/reward rules.

**Need to decide by:**
Before endurance / extraction-from-expedition behaviour is implemented.


## Apex Success Reward

**Question:** What permanent reward marks the first successful Apex clear?

**Current Thinking:**
Apex should provide more significance than simply another normal reward.

Potential rewards:
- major unlock;
- region progression;
- unique material;
- equipment access;
- new difficulty / expedition tier.

**Need to decide by:**
Before Apex implementation.


## Persistent Save Lifetime

**Question:** When should prototype in-memory persistent progression become
save-file-backed progression?

**Current Thinking:**
Do not build save persistence merely to prove the current gameplay loop.

3.0C persistence only needs to survive multiple expeditions during one Play
session.

**Need to decide by:**
Before persistent progression content becomes expensive to recreate manually.

## Open design backlog

Do equipment trees branch with mutually exclusive choices?

Can a completed tree eventually unlock everything?

Can nodes be respecced?

Are skill-tree points equipment-specific or a shared persistent currency?

Does using a weapon/armour earn mastery for that item?

Do monster materials unlock particular branches?

Are abilities granted directly by tree nodes or merely permanently unlocked?

Does every weapon copy share progression, or is progression attached to a
weapon type/set?

Is Armour a complete set or multiple independently progressed pieces?

How many prepared ability / augment slots can equipment ultimately expose?

Exactly what form does the universal Heal action take?