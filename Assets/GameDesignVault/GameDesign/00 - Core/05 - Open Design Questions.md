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