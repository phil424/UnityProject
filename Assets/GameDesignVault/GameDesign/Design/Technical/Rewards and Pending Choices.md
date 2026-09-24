# Rewards and Pending Choices

**Status: Working / Implemented Foundation**

# Purpose

The reward system separates:

- reward source;
- reward category;
- generated reward offers;
- queued player choice;
- reward application.

A reward trigger should not directly modify `RunBuild`.

# Current Flow

Reward Source
↓
`RunRewardContext`
↓
`RunUpgradeOfferGenerator`
↓
`PendingRewardChoice`
↓
Reward UI
↓
Player Selection
↓
`RunRewardDefinition.TryApply`
↓
`RunBuild`

# Reward Context

`RunRewardContext` currently preserves:

- Reward Vector;
- player-facing source label.

Examples:

South Road Horde
→ Character Growth

Wide Graveyard Horde
→ Ability

Elite Zombie
→ General

Area Clear
→ General

The context describes why the pending reward exists.

It does not itself contain or apply the reward.

# Pending Reward Lifetime

Pending reward choices are Expedition-owned through `RunState`.

They may remain unresolved while normal combat continues.

The combat HUD exposes them through the Pending Rewards interface.

When several rewards are queued, the oldest pending choice is resolved first.

Each queued choice retains its own reward context.

# Reward Definitions

`RunRewardDefinition` represents something that may be offered.

Current categories:

Character Growth
→ `RunUpgradeDefinition`

Ability
→ Ability acquisition / Evolution rewards

Reward eligibility remains owned by the concrete reward definition.

# Important Invariants

- reward sources request rewards; they do not mutate `RunBuild`;
- Reward Vector filtering happens before normal reward eligibility;
- pending choices retain their source context;
- reward presentation reads queued state rather than inventing source context;
- player-facing `GENERAL` corresponds to implementation `RunRewardVector.Any`;
- Reward Vectors should only expand alongside real reward systems;
- reward provenance should remain useful for future Encounter, Elite, Boss,
  Objective, World Event and other reward sources.