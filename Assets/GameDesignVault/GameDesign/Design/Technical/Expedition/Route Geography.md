# Route Geography

**Status: Working / Implemented Foundation**

Related:
- [[Expedition Scheduling and Encounter Generation]]
- [[Targeting, Tactics and Encounter Direction]]
- [[Level Authoring and Encounters]]
- [[Encounter Definitions]]
- [[Encounter Workshop]]

# Purpose

Route Geography describes the persistent spatial structure used to move through
a region.

It is distinct from an Encounter.

A Route answers:

> Where can actors meaningfully travel?

An Encounter answers:

> What particular combat/event occurrence is happening here?

The route persists for the Region lifetime.

Encounter occurrences may later occupy portions of that route.

# Current Model

3.0K establishes three primary geography concepts:

Route Graph
├── Route Nodes
├── Route Segments
└── Route Groups.

## Route Node

A meaningful travel location.

The Node's GameObject name is its authoring identity.

Examples:
- Well North;
- Church Gate;
- Cemetery Entrance;
- Market Corner.

A Node is a location, not a connection.

## Route Segment

A first-class traversable connection between exactly two Route Nodes.

Initial Segments are bidirectional.

Segments provide the future seam for concepts such as:
- encounter occupancy;
- traversal cost;
- route threat;
- World Event influence;
- semantic geography tags.

Do not return to storing all route meaning as a raw list of Node references.

## Route Group

An explicit authored collection of Nodes and Segments that form one meaningful
piece of geography.

Examples:
- Village Well Ring;
- Market Street;
- Church Approach;
- South Road.

A Group answers:

> Which exact pieces belong to this authored area?

This is different from a future semantic Tag.

# Authoring Workflow

## 3.0K2 — Editor-Assisted Route Authoring

Route geography remains scene-authored spatial data.

The runtime representation remains:

AmbientRouteGraph
├── AmbientRouteNode
├── AmbientRouteSegment
└── AmbientRouteGroup.

Designers should not normally need to construct that serialization manually.

The primary designer-facing workflow is the Route Geography Authoring Editor
tool.

It can:

- create missing authoring roots;
- create Route Nodes;
- connect selected Nodes;
- create hierarchy-ordered paths;
- create spatial loops;
- create Route Groups from selected geography;
- report route validation issues.

Generated objects remain ordinary Unity scene objects.

The designer may still:
- move Nodes;
- rename Nodes / Segments / Groups;
- edit Segment endpoint references;
- edit Group membership;
- add other compatible scene components.

The authoring tool is an acceleration layer rather than a proprietary file
format.

## Scene Data vs Portable Data

Route topology remains scene-owned for now because positions and connections are
part of concrete Region geography.

Do not move the entire route graph into ScriptableObjects merely to make
Inspector authoring easier.

Future reusable semantic content is a stronger candidate for asset-backed data,
including concepts such as:
- Route Tags;
- Encounter compatibility requirements;
- portable Workshop route contexts.

## Validation

Route validation is non-destructive.

Validation may report:
- missing Segment endpoints;
- duplicate Segments;
- isolated Nodes;
- disconnected geography;
- foreign references;
- invalid Group membership.

Validation should not automatically shrink or rewrite serialized collections
while the designer is editing them.

Manual Inspector editing remains supported.

# Group vs Tag

A Group identifies specific geography.

Example:

Village Well Ring
=
these exact eight Nodes and eight Segments.

A Tag describes what geography means.

Future examples:
- Village;
- Road;
- Well;
- Open;
- Narrow;
- Cemetery.

Tags are deliberately deferred until Encounter/Site compatibility needs them.

Do not implement a giant route-tag enum.

The likely future direction is data-authored tag definitions.

# Navigation

3.0K1 introduces persistent destination-oriented ambient travel.

No Encounter Directive
↓
join nearest Route Node
↓
choose reachable destination
↓
calculate shortest authored path
↓
follow intermediate Nodes
↓
reach destination
↓
choose another.

Local combat may temporarily interrupt the navigation intent.

It should not discard the route destination.

Explicit Encounter Direction continues to override ambient travel.

# Encounter Relationship

Routes themselves should not become `LevelEncounter`s.

A Region route is persistent geography.

An Encounter has occurrence lifecycle such as:
- Available;
- Selected;
- Started;
- Completed;
- Expired.

Conflating those lifetimes would make concepts such as:

"completed road"

or:

"expired route connection"

ambiguous.

Instead the working future model is:

Route Geography
↓
Encounter Site / Footprint
↓
Encounter Occurrence.

# 3.0K3 — Encounter Route Sites

## K3A — Route-Bound Encounter Sites

`EncounterRouteSite` is the first concrete bridge between persistent Region
geography and the current scene-authored `LevelEncounter` implementation.

The current prototype authoring workflow co-locates the Route Site component on
the `LevelEncounter` GameObject.

This is an authoring/runtime bridge, not a statement that Encounter Site and
Runtime Encounter Instance must remain the same object forever.

The long-term model remains:

Route Geography
↓
Encounter Site
↓
Encounter Occurrence.

3.2 may later separate Site and Runtime Instance ownership more completely.

## Footprint

A Site may currently contain:

- explicit Route Nodes;
- explicit Route Segments.

This composition supports:

One Node
→ point Site.

One Segment
→ Segment Site.

Several connected Segments
→ route/path Site.

Segment endpoints are derived for navigation.

Do not duplicate every Segment endpoint into the serialized Node list merely to
make pathfinding possible.

## Route Approach

For the current prototype, a Site derives its route-approach Node from the
footprint Node nearest the owning Encounter anchor.

Example:

Opening Horde
occupies
Well West ↔ Well NW

Encounter anchor lies northwest
↓
approach Node = Well NW.

Once the route-approach Node is reached, Encounter Direction switches to the
existing local Encounter target.

That target remains:

authored PartyProximity start boundary
or
default Encounter anchor.

Therefore:

Route Geography
answers regional traversal.

Encounter authoring
continues to own local start semantics.

## Optional Adoption

Encounter Route Sites are optional.

A `LevelEncounter` without an `EncounterRouteSite` continues to use the existing
direct navigation fallback.

This allows route geography to be adopted incrementally rather than requiring
all current content to be migrated at once.

# Future Encounter Footprints

A formal Encounter Site may eventually occupy:

- one Route Node;
- one Route Segment;
- a connected Route Path;
- one Route Group.

Example:

Road Blockade

Footprint:
Entry → Core → Exit.

Enemies from encounters already crossed by the party may continue pursuing when
the player travels toward another encounter.

This preserves the existing Encounter Membership / redirection design.

# Future Route Compatibility

Long-term generation may match:

Encounter Definition requirements
+
Route / Site geography
↓
compatible occurrence.

Example:

Site:
- Village
- Well
- Open

could accept encounter content designed for those semantic qualities.

This remains future work.

# 3.0K1 Proof

The first geography proof is the Village Well Ring:

8 Route Nodes
+
8 Route Segments
+
1 Route Group
+
central non-route landmark.

Ambient travel follows the ring around the landmark rather than treating the
centre as a traversable route point.