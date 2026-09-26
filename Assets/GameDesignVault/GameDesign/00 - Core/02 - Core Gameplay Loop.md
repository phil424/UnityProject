# Core Gameplay Loop

**Status: Working**

This document describes the current intended gameplay loops and the working
retention hypothesis behind them.

It should evolve as playtesting provides better evidence.

# Core Retention Hypothesis

MiniCrawler should primarily retain the player through:

- enjoyable autonomous combat that is satisfying to watch;
- meaningful moments of player intervention;
- experimentation with Characters, Abilities, Weapon Mods and builds;
- visible consequences when tactical actions succeed;
- anticipation of meaningful build development;
- world opportunities whose risk, difficulty and rewards are understandable;
- persistent progression that creates new possibilities rather than simply
  allowing the player to overpower future expeditions.

The current prototype provides promising evidence for the first two points.

The surrounding buildcraft, Encounter-choice and Meta Progression loops are not
yet strong enough to validate the remainder.

This is a working hypothesis rather than a locked design rule.

# Playable Time Horizons

MiniCrawler's development can be evaluated by how long the game sustains
meaningful player intention before the player runs out of reasons to continue.

These horizons are not mandatory expedition lengths and are not scope limits.

| Horizon | Primary Player Question | Current State |
| --- | --- | --- |
| 10–30 seconds | What can I do right now? | Strong current foundation: autonomous combat, Abilities and visible tactical consequences. |
| 30–60 seconds | What should I fight next? | Functional foundation: Encounter selection, travel and differing fight composition. |
| 1–3 minutes | What am I currently trying to accomplish? | Current development frontier. Objectives, reward relevance and Progression Targets need to provide intention. |
| 3–10 minutes | Where is this expedition/build going? | Requires deeper build development, Objectives, opportunities and rising pressure. |
| 10–30+ minutes | How far can I push this expedition? | Requires pacing, deeper threats, Bosses/Apex opportunities and continued reasons to stay. |
| Across expeditions | Why do I want another run? | Characters, progression, unlocks, experimentation, Hunts and discovery. |

A successful expedition does not need to reach every horizon.

A player may complete a meaningful goal after only a few minutes and return.

The game should earn continued play by presenting new opportunities rather than
requiring every expedition to reach an arbitrary minimum length.

# Current Retention Frontier

The strongest current evidence is:

Simulation creates a combat situation
↓
Player recognises an opportunity/threat
↓
Player intervenes with an Ability
↓
Combat visibly responds

Examples such as using Charge to escape committed enemy attacks and causing
those attacks to visibly whiff demonstrate the intended relationship between
autonomous combat and deliberate intervention.

The current weakest horizon is approximately 1–3 minutes.

The player can choose Encounters, but currently lacks enough intention to care
which Encounter they choose.

Working direction:

Progression Target
+
Expedition Objectives
+
Encounter Reward / Resource information
+
Organic expedition opportunities

should provide that missing intention.

# Moment-to-Moment Loop

## Current Intended Loop

Observe Combat
↓
Read Threat / Opportunity
↓
Watch Autonomous Weapon Behaviour
↓
Choose Whether / When to Intervene
↓
Activate Ability / Tactical Control
↓
Observe Outcome
↓
Reassess

## Why Continue?

The strongest currently observed moment-to-moment appeal is the interaction
between autonomous combat and deliberate player intervention.

During the first N1 observational playtest:

- learning what the existing Abilities could do was engaging;
- Knockback created immediately understandable combat consequences;
- using Charge to escape a group caused enemies to commit to attacks and whiff;
- watching those attacks visibly miss because of the player's decision felt
  particularly satisfying.

This suggests that MiniCrawler should preserve situations where:

> the simulation creates a problem,
> the player intervenes,
> and the consequences of that intervention are immediately visible.

The player should not need to directly control every basic action for their
decisions to feel impactful.

## Current Weaknesses / Unknowns

- The first minute currently presents a large amount of information
  simultaneously.
- World Time, Encounter choices, minimap information, Abilities, Active
  Encounters and day/night information compete for attention before the player
  understands what deserves focus.
- The passive combat HUD currently exposes more strategic information than a new
  or returning player can immediately prioritise.
- Combat intervention is promising, but enemy and targeting variety are still
  too limited to know how deep the tactical layer can become.

# Encounter-to-Encounter Loop

## Current Intended Loop

Read Current World / Available Opportunities
↓
Choose Encounter
OR
Allow Ambient Travel
↓
Travel Through Region
↓
Ambient Combat / Encounter Approach
↓
Commit to Encounter
↓
Auto Combat + Player Intervention
↓
Earn Reward / Progress
↓
Reassess Available Options

## Why Continue?

The intended purpose of Encounter selection is to give the player meaningful
strategic choices about what opportunity or threat to pursue next.

The current prototype does not yet achieve this.

During the first N1 observational playtest:

- Encounter choices did not clearly communicate their expected reward;
- difficulty/threat was not meaningfully communicated;
- there was little information explaining why one Encounter should be selected
  over another;
- Encounter selection therefore became a mechanical way to continue the
  playtest rather than a meaningful decision.

The eventual Encounter loop should create questions such as:

- Is this worth the risk?
- What does this Encounter offer my current build?
- Is this threat harder but more valuable?
- Does this help an Objective or unlock?
- Should I pursue this now or another opportunity first?

## Current Weaknesses / Unknowns

- Encounter reward identity is unclear.
- Difficulty/threat identity is unclear.
- Encounter purpose is unclear.
- The current player often has development knowledge that substitutes for
  missing player-facing information.
- The correct amount of pre-Encounter information remains unresolved.

# Buildcraft Loop

## Current Intended Loop

Begin With Character Identity
↓
Gain Expedition Reward
↓
Choose Upgrade / Ability / Weapon Mod Interaction
↓
Build Develops
↓
Fight New Threats
↓
Evaluate Whether The Choice Worked
↓
Make Another Build Decision

## Why Continue?

Buildcraft should create anticipation:

> I want to see what my next choice is and how it changes this expedition.

The current prototype does not yet provide strong evidence for this loop.

During the first N1 observational playtest:

- the first reward appeared abruptly;
- the reason the reward was being offered was not clear;
- the available choices felt shallow;
- the reward did not create significant anticipation or excitement;
- the player did not feel they were making an interesting build direction
  decision.

This does not currently demonstrate that the reward/buildcraft concept itself is
wrong.

The prototype reward vocabulary is intentionally limited and does not yet
represent the planned Character Stats, Weapon Mods, Ability interactions or
future build synergies.

## Current Weaknesses / Unknowns

- Reward arrival lacks enough context/presentation.
- Reward choices currently have limited mechanical depth.
- Build direction is not clearly visible.
- Current rewards do not strongly encourage experimentation.
- The player cannot yet anticipate meaningful synergies.
- The relationship between preparation, Character identity and expedition
  acquisition still needs reassessment.

# Expedition Loop

## Current Intended Loop

Prepare Character
↓
Start Expedition
↓
Travel / Fight / Pursue Opportunities
↓
Develop Temporary Build
↓
Complete Objectives / Encounters
↓
World Pressure Develops
↓
Increasingly Valuable / Dangerous Opportunities
↓
Major Threat / Expedition Goal
↓
Return / Continue / Fail
↓
Resolve Expedition Outcome
↓
Progression Target / Objectives
↓
Encounter Decisions
↓
Build Development
↓
Return when satisfied
OR
continue because new opportunities justify staying

## Why Continue?

The expedition should create increasing investment in:

- the temporary build;
- current Objectives;
- valuable opportunities;
- escalating threats;
- rewards that would be lost or banked;
- anticipation of major expedition milestones.

The current prototype does not yet provide enough structure to create this
feeling consistently.

During the first N1 observational playtest:

- Encounters did not provide strong reasons to pursue them;
- there was little explicit sense of a larger expedition objective;
- continuing largely happened because the playtest required continued
  interaction;
- voluntary Return is now available during active expedition play;
- the missing piece is meaningful short-term intention that tells the player
  when returning actually makes sense.
- the prototype does not yet create a meaningful risk/reward decision about when
  to leave.

## Current Weaknesses / Unknowns

- Voluntary Return now exists as a normal player decision.
- Objectives / rewards / progression do not yet create enough context for the
  timing of that decision to feel meaningful.
- Expedition success/end conditions remain incomplete.
- The first Expedition Objective foundation now provides a concrete
  short-horizon reason to prefer relevant Encounters.
- Objective selection, variety and rewards are still intentionally shallow.
- Escalation/pacing is not yet strong enough to create a clear expedition arc.
- Safe voluntary Return exists, but Rewards and deeper opportunities do not yet
  make continuing versus returning a rich decision.

# Meta Loop

## Current Intended Loop

Finish / Fail / Leave Expedition
↓
Retain Persistent Progress
↓
Advance Character / Unlock Options / Gain Resources
↓
Make New Preparation Decisions
↓
Enter Another Expedition With New Possibilities

## Why Start Another Expedition?

The intended long-term motivation is not simply:

> make permanent numbers large enough that the expedition stops being
> threatening.

Persistent progression should increasingly create:

- new Characters;
- new Weapon Mods;
- new Abilities;
- new Character progression choices;
- cross-character build possibilities;
- additional ways to approach familiar gameplay.

The current prototype Meta Progression does not yet represent this intended
system.

During the first N1 observational playtest:

- Armour was upgraded after returning;
- the numerical improvement was powerful enough to dramatically reduce or remove
  incoming damage;
- this encouraged immediately starting another run rather than considering the
  preparation screen;
- the preparation/meta screen itself did not create interest or anticipation;
- permanent numerical power currently risks bypassing the combat/buildcraft
  decisions that are supposed to make an expedition interesting.

This is strong evidence that the current Weapon / Armour / Focus prototype
should not be used to judge the intended final Meta Progression system.

## Current Weaknesses / Unknowns

- Persistent progression is currently shallow and primarily numerical.
- Permanent numerical power can trivialise expedition combat.
- The preparation screen does not currently encourage reflection or
  anticipation.
- Character progression and collection do not yet exist in their intended form.
- Cross-character unlocks do not yet exist.
- Persistent resources do not yet have a final purpose.

# Failure

## Current Working Direction

Failure should end the expedition immediately at the gameplay level while still
allowing the attempt to feel productive.

Failure may preserve appropriate persistent rewards/progress while temporary
expedition power is lost.

The player should ideally finish a failed expedition thinking:

> I lost that attempt, but I learned something, progressed something, or now have
> another reason to try again.

The exact reward consequences of failure remain unresolved.

# Success

## Current Working Direction

Success should represent a meaningful expedition milestone rather than simply
the moment the Character becomes numerically unstoppable.

Potential roles include:

- completion of a major expedition objective;
- access to significant persistent rewards;
- the option to return and bank progress;
- the option to continue into greater risk/reward;
- progression toward additional content.

The exact success/Apex/continuation structure remains unresolved.

# Motivation Horizons

| Horizon | Current Prototype Answer | Desired Direction |
| --- | --- | --- |
| Next 10 seconds | Watch autonomous combat and use existing Abilities. This is currently the strongest part of the loop. | Read combat, identify a threat/opportunity, intervene and see a satisfying consequence. |
| Next 1–3 minutes | Select another Encounter largely to continue the prototype. | Anticipate a specific reward, Objective, threat, build interaction or opportunity. |
| Rest of expedition | Accumulate enough prototype rewards to become very powerful. | Develop a distinctive temporary build while pursuing increasingly meaningful opportunities and managing rising risk. |
| Next expedition | Spend prototype persistent currency and restart. | Try a new build, Character, Mod, Ability, unlock, objective or progression direction. |
| Several expeditions | Increase Weapon / Armour / Focus levels. | Develop Characters and unlock new gameplay possibilities across the account. |
| Long term | Not yet meaningfully represented. | Character mastery, collection, cross-character buildcraft, challenge, discovery and continued experimentation. |

# First N1 Observational Findings

## Pre-Run

### Observation

The current pre-run screen feels visually overwhelming while also being
uninteresting to remain on.

The player tends to begin the expedition quickly rather than spending time
considering preparation.

### Classification

- Presentation problem.
- Information architecture problem.
- Missing long-term preparation/progression depth.

### Working Direction

The preparation screen should eventually feel like a desirable place to spend
time rather than an administrative menu.

A current visual concept is documented in [[UI-UX]].

---

## First Minute

### Observation

Too many systems compete for attention immediately:

- World Time;
- Encounter choices;
- minimap;
- Abilities;
- Active Encounters;
- day/night information.

### Classification

Information hierarchy / onboarding problem.

### Working Direction

The normal combat HUD should prioritise immediately actionable information.

Secondary strategic information should use progressive disclosure and expanded
views rather than demanding equal attention at all times.

---

## First Meaningful Fight

### Observation

Experimenting with Ability behaviour is compelling.

Charge and Knockback create understandable tactical consequences.

Causing enemies to commit to attacks and visibly whiff because the player moved
away is particularly satisfying.

### Classification

Strong positive gameplay evidence.

### Working Direction

Preserve and expand the pattern:

Simulation creates situation
→ Player intervenes
→ Combat visibly responds.

---

## First Reward

### Observation

The reward appears abruptly and currently feels shallow.

The player does not strongly understand why the reward appeared or become
excited about the available choices.

### Classification

- Content-depth problem.
- Presentation/context problem.
- Buildcraft problem.

### Working Direction

Rewards should:

- have understandable sources;
- create anticipation;
- provide visibly meaningful build choices;
- make the player want to test the result.

---

## Mid-Expedition

### Observation

Encounter choices currently communicate too little about:

- expected reward;
- difficulty;
- purpose;
- why they matter to the current expedition.

Selection therefore becomes procedural rather than strategic.

### Classification

Missing information and incomplete strategic/reward design.

### Working Direction

Encounter choices should eventually communicate enough risk, opportunity and
relevance for alternatives to feel meaningfully different.

---

## Expedition End

### Observation

There is currently no normal voluntary return flow.

The player can feel trapped in the expedition when they would prefer to bank
progress or change their setup.

### Classification

Missing system / incomplete expedition structure.

### Working Direction

Voluntary return and the consequences of leaving versus continuing need explicit
design during the Expedition Pacing / Ending work.

---

## Return to Preparation

### Observation

The current permanent Armour upgrade can rapidly remove meaningful incoming
damage.

After purchasing it, the player immediately begins another expedition without
engaging with the preparation screen.

### Classification

- Prototype balance problem.
- Meta-progression design problem.
- Preparation-screen presentation problem.

### Working Direction

Persistent progression should avoid replacing the interesting parts of combat
with overwhelming permanent numerical advantage.

Preparation should create anticipation for a new run rather than functioning
only as an upgrade transaction.

# Current Post-N Gaps

The N programme established the first coherent loop connecting Preparation,
Expedition buildcraft, rewards and persistent progression.

The remaining gaps now primarily belong to the later O/P/Q programmes rather
than missing N foundations.

## Moment-to-Moment

- limited enemy combat roles;
- no player-controlled combat-targeting priorities;
- limited tactical situation variety;
- the passive combat HUD still carries substantial information pressure.

## Character / Buildcraft

- only one confirmed playable Character exists;
- Character Mastery currently proves only stat-oriented nodes;
- Weapon Mods do not yet exist;
- Ability breadth and Character-specific Ability identity remain limited;
- cross-Character progression does not yet exist.

## Expedition Direction

- the Objective foundation exists but condition/content breadth is still small;
- Encounter selection now exposes Reward Vectors and Reward Focus but Encounter
  variety remains limited;
- expedition pacing and escalation are still incomplete;
- the final success / deep-expedition ending structure remains unresolved.

## Rewards

- Reward Vectors and reward provenance are implemented;
- temporary Character Growth and Ability rewards exist;
- persistent resource rewards exist;
- Objective, World Event and richer content-specific rewards remain limited;
- reward/build synergy depth remains early.

## Meta Progression

- three generic persistent resource tiers are implemented;
- Punchy Character Mastery is implemented as the first persistent progression
  sink;
- Character unlock ownership does not yet exist;
- there is no cross-Character progression;
- persistent state remains session-memory rather than final save-file
  persistence;
- final resource names, conversion rules and long-term pacing remain unresolved.

## Content Variety

- one confirmed playable Character;
- limited enemy roles;
- limited Encounter identities;
- limited persistent unlock content.

The next development programme should therefore expand player-facing identity
and content rather than continue building additional N foundations.

# Working Retention Principles

The first N1 observational pass currently supports these working principles.

1. **Autonomous combat should create readable situations for the player to
   influence rather than requiring constant direct control.**

2. **Player intervention should create visible, understandable consequences.**

3. **Build choices should make the player curious to see what happens next in
   the same expedition.**

4. **Encounter choices should communicate enough risk, reward and purpose to
   make choosing between them meaningful.**

5. **Persistent progression should increasingly unlock possibilities rather
   than allowing raw permanent power to bypass expedition gameplay.**

6. **The player should regularly have something concrete to anticipate within
   the next few minutes.**

7. **Information should be revealed according to current relevance rather than
   every strategic system demanding attention simultaneously.**

8. **Preparation should create excitement for the next expedition rather than
   feel like an administrative step between runs.**

# Open N1 Questions

Questions that materially affect upcoming N2–N4 work:

- What temporary Character stat/buildcraft changes are significant enough to be
  immediately noticeable?
- What should an Encounter communicate before the player chooses it?
- How should reward sources communicate what kind of value they provide?
- How much permanent numerical power should Meta Progression provide before it
  begins damaging expedition buildcraft?
- What should the player normally be anticipating during the next 1–3 minutes?
- What should voluntary return preserve, lose or bank?
- What preparation decisions should be important enough that the player does not
  immediately press Start?