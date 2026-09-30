# Playable Character Roster

**Status: Implemented Foundation**

# Purpose

The Playable Character Roster separates:

- which playable Characters exist in the game;
- which Characters the player currently owns;
- which owned Character is selected for the next Expedition.

These are separate responsibilities.

# Static Roster Content

`PlayableCharacterRosterDefinition` owns the authored playable roster.

Each `PlayableCharacterRosterEntry` currently contains:

- Character Definition;
- Unlocked By Default;
- Visible When Locked;
- optional Character Unlock Requirement.

`PartyMemberDefinition` does not itself decide whether the player currently owns
the Character.

This allows the same Character content to exist independently of one player's
persistent progression state.

# Persistent Ownership

`PersistentProgression` owns session-lifetime Character ownership.

It tracks:

- known playable Character IDs;
- unlocked playable Character IDs.
- per-Character unlock progress for locked roster Characters.

Current API includes:

- `InitializeCharacterRoster`;
- `IsCharacterKnown`;
- `IsCharacterUnlocked`;
- `TryUnlockCharacter`
- `GetCharacterUnlockProgress`;
- `TryAddCharacterUnlockProgress`.

Character ownership survives Expedition start/end.

Final save-file persistence remains future work.

# Character Unlock Requirements

Character unlock conditions are authored independently from Character runtime
content.

A locked roster entry may reference a `CharacterUnlockRequirementDefinition`.

The first concrete implementation is:

`ObjectiveCompletionCharacterUnlockRequirementDefinition`

which contributes persistent Character-unlock progress when its referenced
Expedition Objective was completed before the Expedition ended.

Current lifetime bridge:

Completed Expedition Objective
↓
`RunDirector.FinishExpedition`
↓
unlock requirement evaluates the finished `RunState`
↓
`PersistentProgression`
↓
persistent Character unlock progress
↓
Character ownership when the required progress is reached.

Partial Expedition Objective progress is not persisted.

The first Ranger proof requires two completed `Cull Dormant Hordes` Objectives
across separate Expeditions.

The authored requirement is a prototype gameplay proof rather than a final
Ranger-specific unlock condition.

## Future Requirement Composition

The current Objective-completion requirement is the first concrete unlock proof,
not the final vocabulary for all Character unlock conditions.

Future Characters may require multiple independent conditions, for example:

- ownership of a specific Character Mastery node;
- defeating a persistent number of a particular enemy;
- completing an Objective or Hunt;
- defeating a Boss;
- discovering content;
- owning another persistent unlock.

These should extend the existing Character Unlock Requirement seam rather than
introducing Character-specific ownership logic.

Persistent facts should remain owned by their natural systems.

For example:

- Mastery ownership remains Character Mastery state;
- persistent enemy kill totals should remain persistent combat/progression state;
- Objective completion totals should remain persistent progression state.

Character unlock requirements may query those facts.

Do not duplicate unrelated persistent state solely inside a Character unlock.

If multiple conditions are eventually required, prefer small composable,
domain-specific condition definitions over a giant generic condition/query system.

# Setup Validation

`RunSetup` validates Character selection against:

Playable Roster
+
Persistent Character Ownership.

A Character cannot enter the selected Setup merely because a UI supplied its
Definition.

The Character must:

- belong to the playable roster;
- be persistently unlocked.

This makes UI presentation a View of ownership rather than the authority for it.

# Preparation Presentation

`PreparationScreenUI` generates Character buttons from the current playable
roster.

Each runtime Character entry is represented by
`PreparationCharacterButtonUI`.

Current behaviour:

Unlocked By Default
→ visible;
→ selectable;
→ normal Character name presentation.

Locked + Visible When Locked
→ visible;
→ non-interactable;
→ Character name presented as locked;
→ configured unlock requirement name displayed;
→ current persistent unlock progress displayed.

Unlocked Through Progression
→ visible;
→ selectable;
→ presents an `UNLOCKED` acknowledgement.

Locked + not Visible When Locked
→ omitted.

`PreparationScreenUI` rebuilds the Character selector whenever Preparation
becomes active.

This ensures a Character unlocked while the Preparation panel was disabled can
appear when the player returns without requiring a scene reload.

`PersistentProgression.Changed` also refreshes/rebuilds the selector when
ownership or unlock progress changes while Preparation is already active.

# Current Roster

Current implemented playable roster:

Punchy
→ unlocked by default.

Ranger
→ present in the playable roster;
→ visible while locked;
→ not unlocked by default;
→ gameplay unlock requirement implemented through O1C1;
→ development unlock remains available for testing.

Legacy Support content such as Healy is not automatically treated as a playable
Character.

# Current Limitations

O1A does not define:

- final narrative/balance-tuned Character unlock requirements;
- additional Character unlock requirement types;
- final celebratory/polished unlock presentation;
- final locked-card art;
- save-backed ownership and unlock progress.

Those belong to subsequent O1 work.