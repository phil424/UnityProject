# Weapon Mods

**Status: Implemented Stance Foundation**

# Responsibility

Weapon Mods are selectable Basic Attack stances.

They modify or override aspects of a Character's autonomous Basic Attack
behaviour while selected.

They are separate from:

- Ability execution;
- Character ownership;
- Character runtime prefab identity;
- Character Mastery ownership;
- Run Reward delivery.

# Definition

`WeaponModDefinition`

owns static Weapon Mod content:

- identity;
- display data;
- Character compatibility;
- concrete Basic Attack modification behaviour.

The definition does not own:

- whether a Character has unlocked it;
- its current Expedition level;
- whether it is currently selected.

# Persistent / Prepared Ownership

Character Mastery may prepare a Weapon Mod through:

`CharacterMasteryWeaponModNodeDefinition`.

Current flow:

persistent Mastery node ownership
↓
`PersistentProgression.ApplyMasteryToBuild`
↓
`CharacterMasteryWeaponModNodeDefinition.ApplyPreparedBuild`
↓
`RunBuild.TryPrepareWeaponMod`.

The first implementation is:

Punchy
→ Heavy Finisher Stance
→ Heavy Finisher.

No separate global Weapon Mod ownership registry currently exists.

# Run State

`RunWeaponModState`

represents one Weapon Mod carried into the current Expedition.

Current state:

- Definition;
- temporary Level.

A prepared stance currently begins at Level 0.

Future O2 work may extend this state with authored temporary development such as
levels, augments and evolutions where concrete content requires them.

`RunBuild`

owns all Weapon Mod Run States available to that Character during the
Expedition.

Availability in `RunBuild` does not mean the Mod is currently active.

`RunBuild.MaxWeaponModStances`

currently enforces four prepared Weapon Mod positions.

`RunWeaponModState`

currently owns:

- Definition;
- temporary Level.

`TryIncreaseWeaponModLevel`

changes Expedition-owned Mod progression without changing stance selection.

`RunWeaponModState`

now separates:

PreparedLevel
→ persistent Mastery development copied into the Expedition.

ExpeditionLevelBonus
→ temporary Run development.

Level
→ effective sum used by gameplay.

Therefore:

persistent Mastery Lv.1
+
Run reward +1
=
effective Lv.2 for this Expedition.

Next Expedition:
→ persistent Lv.1 restored.

# Runtime Projection

`PartyWeaponModApplicator`

projects the `RunBuild` Weapon Mod states onto:

`WeaponModRuntimeState`.

The applicator runs:

- when a party Actor is spawned;
- when an existing party Actor is refreshed;
- when the Preparation combat showcase constructs its preview build.

# Runtime Selection

`WeaponModRuntimeState`

owns the currently-active Weapon Mastery.

If no Core Mastery exists:

`ActiveModState = null`
→ original Basic Attack.

If Core Mastery exists:

new runtime Actor
→ Core Mastery automatically selected.

When an alternate Mastery is selected:

Core
→ Alternate.

When the currently-active alternate is selected again:

Alternate
→ Core.

Selecting Core directly also returns to Core.

Core itself cannot be toggled off into the original Basic Attack.

# Runtime Refresh

When `WeaponModRuntimeState.Configure` receives a refreshed RunBuild, it attempts
to preserve the current active Mod by Definition ID.

Therefore an ordinary RunBuild refresh does not turn an existing valid stance
off.

If the active stance is no longer available, runtime state safely falls back to
Default.

A newly reconstructed Actor currently starts in Default.

Whether selected stance should later survive Actor/Region reconstruction is not
yet decided.

# Basic Attack Integration

Current flow:

CombatStats
↓
CombatActionDefinition coefficient
↓
currently selected Weapon Mod
↓
projectile or melee delivery
↓
DamageResolver.

`BasicAttackWeaponModContext` currently provides:

- source;
- target;
- CombatAction state;
- combo step information;
- projectile/basic-attack context;
- current Run Weapon Mod level.

The first concrete Mod uses existing combo state to identify the final combo
step.

# Projectile Delivery Override

`WeaponModDefinition` may optionally take ownership of projectile Basic Attack
delivery through:

`TryResolveProjectileBasicAttack`.

The contract distinguishes:

false
→ this Mod does not override projectile delivery;
→ use the normal projectile path.

true
→ the Mod handled projectile delivery;
→ the returned connection result is authoritative for the current action.

`WeaponModRuntimeState` delegates this call only to the currently-active stance.

`CombatActionSystem` therefore follows:

calculate Basic Attack damage
↓
active Mod may modify damage
↓
if the attack is projectile-based:
    active Mod gets first refusal over projectile delivery
↓
otherwise use the normal targeted projectile path.

The first concrete implementation is:

`PiercingProjectileWeaponModDefinition`.

It reuses the existing:

`ProjectileRuntime.SpawnDirectional`

primitive already used by directional Ability content.

It does not introduce a second projectile runtime system.

# First Concrete Stance

`Heavy Finisher`

is Punchy-compatible.

When inactive:

→ no effect.

When active:

→ first two combo steps are unchanged;
→ final combo step deals +100% damage.

The Mod does not own combo progression.

It reads the existing gameplay-authoritative CombatAction combo state.

## Piercing Arrows

Ranger-compatible.

Uses the existing Ranger Basic Attack projectile asset.

Default Ranger delivery:

`ProjectileRuntime.Spawn`
→ targeted / homing.

Piercing Arrows:

`ProjectileRuntime.SpawnDirectional`
→ straight-line;
→ multi-target.

Current level behaviour:

base maximum targets = 2;
additional targets per level = 1.

No additional damage scaling is currently applied by the Mod.

# Compatibility

Compatibility remains authored on `WeaponModDefinition`.

Empty compatibility
→ broadly compatible.

Explicit Character references
→ only those Characters are valid users.

The first Heavy Finisher Mastery node is authored on Punchy and references a
Punchy-compatible Mod.

No generic tag/query compatibility system is currently required.

# Reward Integration

`RunRewardVector.WeaponMod`

now delivers temporary development for prepared Weapon Mod states.

The first concrete reward is:

`WeaponModLevelRewardDefinition`.

Eligibility requires the target Weapon Mod to already exist in the current
`RunBuild`.

Applying the reward increases its `RunWeaponModState.Level`.

The subsequent normal runtime refresh:

RunBuild changed
↓
`StageDirector.RefreshPartyMemberRuntime`
↓
`PartyWeaponModApplicator`
↓
`WeaponModRuntimeState.Configure`

preserves the selected stance when still available.

Because runtime state references the refreshed Run Weapon Mod state, the HUD and
combat behaviour immediately reflect the new level.

# Current Limitations

Not yet implemented:

- Mod augments/evolutions;
- elemental/status behaviours;
- permanent Mastery empowerment of an existing Mod;
- cross-Character progression;
- stance-switch presentation/VFX;
- active stance persistence across newly reconstructed Actors;
- save-backed persistent Mastery ownership.

# Combat HUD

`WeaponModHUD`

is the player-facing runtime controller.

It subscribes to:

- `StageDirector.PartyMemberSpawned`;
- `StageDirector.PartyMemberRuntimeChanged`;
- `StageDirector.LevelCleared`.

It binds to the spawned Actor's:

`WeaponModRuntimeState`.

The HUD discovers exactly four direct
`WeaponModStanceButtonUI` children from `WeaponModSlotRoot`.

Sibling order represents stance position.

No parallel Button/Text arrays are serialized.

Each slot receives the corresponding `RunWeaponModState` from
`WeaponModRuntimeState.AvailableMods`.

Unavailable positions remain visible but non-interactable.

The HUD manipulates only runtime stance selection.

It does not own Weapon Mod levels or persistent ownership.

# Stance Positions

Weapon Mastery uses four explicit positions represented by
`WeaponModStanceSlot`:

- Core;
- Alternate1;
- Alternate2;
- Alternate3.

The original Basic Attack is not represented by a stance slot.

It exists only as a fallback when no Core Mastery is currently prepared.