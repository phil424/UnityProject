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
- Visible When Locked.

`PartyMemberDefinition` does not itself decide whether the player currently owns
the Character.

This allows the same Character content to exist independently of one player's
persistent progression state.

# Persistent Ownership

`PersistentProgression` owns session-lifetime Character ownership.

It tracks:

- known playable Character IDs;
- unlocked playable Character IDs.

Current API includes:

- `InitializeCharacterRoster`;
- `IsCharacterKnown`;
- `IsCharacterUnlocked`;
- `TryUnlockCharacter`.

Character ownership survives Expedition start/end.

Final save-file persistence remains future work.

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

Current behaviour:

Unlocked
→ visible;
→ selectable.

Locked + Visible When Locked
→ visible;
→ non-interactable;
→ presented as locked.

Locked + not Visible When Locked
→ omitted from the roster presentation.

When persistent ownership changes, the Character selector rebuilds so newly
unlocked hidden Characters may appear without a scene reload.

# Current Roster

Current implemented playable roster:

Punchy
→ unlocked by default.

Legacy Support content such as Healy is not automatically treated as a playable
Character.

# Current Limitations

O1A does not define:

- final unlock conditions;
- unlock costs;
- Character unlock Objectives/Hunts;
- progress toward an unlock;
- final locked-card art;
- save-backed ownership;
- the second playable Character.

Those belong to subsequent O1 work.