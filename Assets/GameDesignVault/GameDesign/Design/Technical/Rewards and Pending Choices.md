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

# Reward Catalogue and Definitions

`RunDirector` references one:

`RunRewardCatalogDefinition`.

The Catalogue is composed from reusable:

`RunRewardSetDefinition`

assets.

Current default Catalogue:

Default Run Reward Catalog
├── Core Character Growth
├── Core Abilities
├── Punchy Weapon Mastery
└── Ranger Weapon Mastery.

Reward Sets reference ordinary `RunRewardDefinition` assets.

The Catalogue resolves those Sets into a unique configured reward pool for Run
generation.

Duplicate Reward IDs are collapsed by runtime Catalogue resolution.

The Content Workbench therefore validates duplicate IDs explicitly so accidental
identity collisions are visible during authoring.

Current Reward families include:

Character Growth
→ `RunUpgradeDefinition`.

Ability Unlock
→ `AbilityUnlockRewardDefinition`.

Ability Evolution
→ `AbilityEvolutionRewardDefinition`.

Weapon Mastery development
→ `WeaponModLevelRewardDefinition`.

Concrete Reward definitions remain responsible for:

- configuration;
- Reward Vector;
- rarity;
- player-facing description;
- compatibility / eligibility;
- applying their effect to `RunBuild`.

The Catalogue defines available content.

It does not replace concrete Reward eligibility.

# Rarity Selection

`RunRewardCatalogDefinition` owns the baseline rarity-weight profile.

Current working profile:

Common
→ 60.

Uncommon
→ 25.

Rare
→ 10.

Epic
→ 4.

Legendary
→ 1.

`RunUpgradeOfferGenerator` does not weight individual Reward assets directly by
their count.

For each offer slot it:

1. builds the eligible Reward candidate pool;
2. identifies rarity tiers represented by those candidates;
3. sums the Catalogue weights for those represented tiers;
4. performs a weighted rarity roll;
5. selects one eligible offer from the chosen rarity;
6. removes that offer;
7. repeats for the next slot.

A rarity with no eligible candidates contributes no weight to that roll.

A rarity with weight `0` is disabled for selection while that profile remains
authored.

If no represented rarity has positive weight, no offer can be selected.

Reward Vector and concrete Reward eligibility remain earlier filters and are
not bypassed by rarity.

# Unified Content Workbench — Run Rewards

Normal Run Reward authoring uses:

Tools
→ MiniCrawler
→ Content Workbench
→ Run Rewards.

The workspace exposes:

Catalogue
↓
Reward Sets
↓
Reward Definitions.

The selected playable Character remains visible as an authoring context.

This allows Reward eligibility to be inspected without changing Reward
ownership.

The normal workspace supports:

- Catalogue browsing;
- Reward Set creation;
- adding existing Reward Sets;
- Reward Set membership removal;
- Reward browsing;
- identity-first Reward creation;
- automatic registration in the selected Reward Set;
- type-specific Reward editing;
- Reward rarity/category presentation;
- player-facing title/description preview;
- related-content navigation;
- selected-Character eligibility preview;
- adding existing Rewards to Sets;
- membership removal without asset deletion;
- explicit Reward asset filename synchronisation;
- non-destructive Catalogue validation.

# Reward Authoring Preview

The Workbench preview displays the same authored Reward data used by runtime
presentation:

- Display Name;
- Description;
- Icon;
- Rarity;
- Reward Vector.

Rarity colour/labels reuse the runtime Reward rarity presentation.

Reward-category colour is an editor-only authoring aid.

# Eligibility Preview

The selected Character in the Workbench sidebar is used as eligibility context.

The editor evaluates two temporary authoring builds:

Starting Build
→ Character Starting Abilities.

Full Mastery Authoring Build
→ Starting Abilities plus all configured persistent Mastery content.

This preview does not modify persistent progression.

It also does not simulate every possible sequence of earlier Expedition rewards.

Therefore an otherwise valid Reward may still become eligible during gameplay
even when the static authoring preview reports it currently ineligible.

The preview is a content-authoring aid rather than a full Run simulator.

# Asset Identity

New Reward Sets and Rewards use identity-first creation.

For Rewards the designer provides:

- Reward type;
- Display Name;
- stable ID;

before the ScriptableObject is created.

The asset therefore receives a meaningful filename immediately.

Changing a Reward Display Name does not automatically rewrite its stable ID.

An explicit asset-name synchronisation action may rename the Unity asset while
preserving GUID-based references.

# Current Catalogue Scope

The current Catalogue does not yet define:

- Region-specific pools;
- Boss-specific pools;
- encounter-specific catalogue composition.

These should be introduced only alongside concrete gameplay requirements rather
than being guessed in advance.

# Important Invariants

- reward sources request rewards; they do not mutate `RunBuild`;
- Reward Vector filtering happens before normal reward eligibility;
- pending choices retain their source context;
- reward presentation reads queued state rather than inventing source context;
- player-facing `GENERAL` corresponds to implementation `RunRewardVector.Any`;
- Reward Vectors should only expand alongside real reward systems;
- reward provenance should remain useful for future Encounter, Elite, Boss,
  Objective, World Event and other reward sources.