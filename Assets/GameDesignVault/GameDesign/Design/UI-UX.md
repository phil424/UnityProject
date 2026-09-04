# Overview
Principles:
- combat information should be readable quickly;
- do not force unnecessary interruptions;
- detailed information can be optional/collapsible;
- major rewards deserve strong presentation;
- UI should communicate future threats, not only current state.

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
Current visual concept:
```
      Ability
   Ability Ability
 Utility Core Sustain
      Portrait
```

Not literal final geometry.

Document:
- innate visually central
- ability count unresolved
- autocast indicator potentially available
- cooldown readability
- ability state readability
- pending/cast information.

# Map, Minimap and Encounter Navigation

The map / schedule interface may become one of the game's primary strategic
control surfaces.

It should help answer:

- Where am I?
- What encounters currently exist?
- Which encounters can I pursue?
- What is happening now?
- What is coming next?
- Where will it happen?
- When will it happen?
- How dangerous is it?
- What valuable opportunities exist?
- Which encounter am I currently heading toward?

# Combat Minimap

Working direction:

The combat HUD should contain a compact spatial view of the current area.

Potential information:
- hero / party location;
- known encounter locations;
- selected encounter;
- important active threats;
- rare opportunities;
- bosses / elites where appropriate;
- relevant navigation destination.

The minimap should communicate useful strategic information without becoming a
complete omniscient representation of every unit.

Exactly how encounter discovery affects map visibility remains unresolved.

# Encounter List

The combat HUD should expose known encounters in a compact selectable list.

Example:

ENCOUNTERS

★ Nobleman's Procession
  RARE • 68m

◆ Cemetery
  ACTIVE • 25m

◇ Church Horde
  Incoming • 00:22

Selecting an encounter gives the hero / party a travel directive toward it.

The encounter list should surface information such as:
- name;
- state;
- distance / location;
- threat;
- opportunity / rarity;
- notable enemies;
- relevant timing.

The HUD should not become a giant quest log.

Use progressive disclosure so important decisions remain quick to read.

# Targeting Controls

The HUD should expose the hero's current autonomous target policy.

Example:

TARGETING
Closest
+ Prefer Elite

The normal HUD may display only the current configuration.

A secondary panel can provide deeper targeting configuration if required.

# Rare Opportunity Presentation

Rare or valuable encounters should be capable of interrupting the player's
attention without forcing immediate interaction.

Example:

RARE SIGHTING
Nobleman's Procession
North Cemetery
Available for a limited period

The player can then choose whether to redirect toward it.

Threat and opportunity rarity should be visually distinguishable.

# Reward UI
Include:
- pending reward indicator
- voluntarily opened reward UI
- combat pause
- travel presentation
- reward acquisition order
- ability evolution presentation
- rarity feedback.

# Hub and Menus
Record options:

### 2D Frontend

### Physical Hub

### Hybrid

Do not choose.

Document that underlying persistent systems should not depend on presentation choice.