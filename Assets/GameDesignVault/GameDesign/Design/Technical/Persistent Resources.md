# Persistent Resources

**Status: Implemented Foundation**

# Purpose

Persistent Resources provide the first generic economy for progression between
expeditions.

Current placeholder tiers:

- Level 1;
- Level 2;
- Level 3.

Names and balance are not final.

# Lifetime

Enemy / reward source
↓
active Expedition
↓
`RunState`
↓
earned resource totals
↓
Expedition ends
↓
`PersistentProgression`

Earned resources bank on:

- Voluntary Return;
- Defeat;
- future successful expedition end.

A normal Region transition does not bank or reset them.

# Separation From Expedition Currency

Expedition Currency:

- temporary;
- belongs to `RunState`;
- supports temporary expedition spending;
- resets when the expedition ends.

Persistent Resources:

- survive expeditions;
- fund long-term progression;
- remain after defeat.

They are deliberately separate economic concepts.

# Enemy Authoring

`EnemyMember` may author one or more `PersistentResourceReward` entries.

Each entry contains:

- Resource Tier;
- Amount.

Current proof values:

Zombie / Skeleton
→ 1 × Level 1.

Elite Zombie
→ 5 × Level 1;
→ 1 × Level 2.

Boss Zombie
→ 10 × Level 1;
→ 3 × Level 2;
→ 1 × Level 3.

These values exist for system validation and are not final balance.

# Runtime Flow

`StageDirector`
↓
enemy gameplay death
↓
`PersistentResourceEarned`
↓
`RunDirector`
↓
`RunProgress`
↓
`RunState`

At Expedition end:

`RunDirector`
↓
reads staged totals
↓
ends RunState
↓
banks totals into `PersistentProgression`

# Presentation

Preparation currently presents the three persistent balances in the frontend
Header.

The prototype Weapon / Armour / Focus progression temporarily consumes Level 1
Resources.

That spending model is transitional and will be replaced by Character Mastery
during later N4 work.

# First Persistent Resource Sink

Character Mastery is the first implemented long-term consumer of the tiered
resource economy.

Prototype Punchy nodes demonstrate:

Level 1-only costs
+
mixed Level 1 / Level 2 costs.

Multi-tier spending is atomic:

all required resources must be affordable before any are deducted.

Level 3 currently has no Mastery sink.

That is intentional until an appropriately significant progression milestone
exists.

# Future

Still unresolved / deferred:

- final resource names;
- tier conversion ratios;
- upward / downward conversion;
- Character Mastery costs;
- Weapon Mod costs;
- Ability unlock costs;
- Boss-specific materials;
- Character-specific resources;
- crafting;
- resource-focused Encounter Reward Vectors;
- balance by expedition depth.