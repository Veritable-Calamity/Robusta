# World Model Workshop 03: What Is a Game Object?

- **Workshop status:** Accepted and recorded as ADR 0013
- **Date:** 2026-07-18
- **ADR:** ADR 0013
- **Implementation claim:** None

## The question

What makes something a first-class object in a running world rather than merely data?

Should every visible or changing thing become an entity, or should entities be reserved for things with an independent identity and life of their own?

## Why this matters

A component-and-system model makes it pleasantly easy to create new kinds of objects. The danger is that “create another entity” can become the automatic answer even when the thing being modeled is only:

- one number;
- one addressable cell in a large tilemap;
- one particle in a visual effect;
- one button in a menu;
- one entry in a navigation or atmosphere field;
- one global rule for the world.

The wrong boundary can make the platform wasteful, difficult to inspect, and confusing to save or network. The opposite mistake is also harmful: if only visible physical things can be entities, important non-physical participants lose the benefits of identity, events, components, and lifecycle.

The design needs a rule a game developer can apply without knowing how the ECS is stored.

## How Robust Toolbox answers

Robust Toolbox has a clear and productive core idea:

> An entity is a unique identifier with a collection of components. Components hold data, and systems provide behavior.

This lets developers build humans, items, walls, machines, chairs, projectiles, and other objects by combining reusable capabilities rather than constructing deep class hierarchies. Its inspection tools can show an entity's components and let authorized developers or administrators add, remove, or change them. Grids are represented as entities, which lets a station or shuttle participate in normal transforms, tooling, and physics.

Robust Toolbox also shows that the ECS does not need to contain everything:

- grid cells are stored compactly in chunks rather than as one entity per installed tile;
- UI controls have their own client-side model;
- prototypes are recipes used to create entities, not running entities themselves;
- client and server may have different local entities, including client-only effect entities;
- screen effects may be drawn directly without becoming authoritative world objects.

### Constructed grids in Space Station 14

The constructed-station case clarifies the distinction.

A grid is an entity. It has an identity, transform, physics body, lifecycle, and a set of tile cells. Those cells are addressed by the grid plus a coordinate and are stored in chunks.

Space Station 14's ordinary structural sequence is represented through tile definitions:

```text
Space → Lattice → Plating → Floor
```

Lattice names space as its underlying turf, plating names lattice, and normal station flooring names plating. At runtime, the coordinate has one current tile value: lattice, plating, and floor are not three simultaneously active tile entities. Placing material replaces that value. The tile system uses the declared base turf and, where needed, retained history to determine what should reappear during deconstruction. Deconstruction can also spawn a loose item entity.

When a player places the first suitable structural tile in open space and no grid exists there, the game can create a new grid entity and install the tile on it. Additional cells expand the same structure. If removing tiles disconnects a section, Robust Toolbox can create a new grid for that disconnected section and re-anchor or re-parent the entities on it.

The result is not “nothing is an entity.” It is a layered ownership model:

- the independently moving structure is an entity;
- installed lattice, plating, and floor are addressable state owned by that structure;
- a loose tile, rod, sheet, machine, wall, or other independently living thing can be an entity;
- changing compact cell data may create, alter, split, or remove higher-level structure identities.

This is a useful example of a large entity owning rich, interactive, networked, saved internal data without turning every addressable entry into another general-purpose entity.

### What works well

- Most interactive things share one familiar composition model.
- Capabilities can be added and removed dynamically.
- Systems, events, networking, debugging, and administration can address the same object identity.
- Constructed grids have identity and physics without requiring one entity per tile cell.
- Game developers do not need a new inheritance hierarchy for every combination of features.

### Where it creates pain or hidden assumptions

The practical line between “entity” and “other data” is learned through engine knowledge and precedent rather than one product rule.

Because the entity path is so capable, developers may use it even when independent identity is not meaningful. At the same time, Robust Toolbox's ordinary entities always carry core metadata and transform behavior, which makes physical world objects convenient but may suggest that position and presentation are inseparable from identity.

The difference between a local entity identity and a network identity is also important. A client may have entities the server does not have, so being an entity does not mean the object automatically exists everywhere.

## How the current Robusta prototype answers

The prototype exposes `GameEntity` as an opaque identity scoped to one world. Through the public world facade, game systems can:

- create and delete an entity;
- attach and retrieve components;
- ask whether it still exists;
- query entities by component.

Internally, spawning an entity automatically adds metadata, and prototype-based spawning normally ensures a transform. This follows the familiar Robust Toolbox model and keeps engine storage details away from game code.

### What is implemented

- World-scoped opaque game-entity handles.
- Entity creation and deletion.
- Component attachment and lookup.
- Component queries.
- Prototype-driven component creation in the internal manager.

### What is not yet answered

- Which concepts should be entities at all.
- Whether every entity must have metadata or position.
- How constructed grids, tiles, maps, particles, navigation data, and other dense structures should be represented.
- Where UI and purely visual state belong.
- Whether world-wide rules should be systems, records, services, or entities.
- How an item moves between structured data and independent entity form.

### What we learned

The opaque world-scoped identity is a good creator boundary. The new platform should keep it, while avoiding the assumption that every kind of runtime information must pass through that identity.

## Recommended answer for the new Robusta

A **game object**, also called an **entity**, means:

> A participant in one world that has an independent identity and lifecycle and can be addressed, inspected, changed, or acted upon as a whole.

The identity test matters more than whether the thing is visible.

A door is an entity because the same door can be opened, damaged, moved, saved, observed, and eventually destroyed. Its identity follows it.

A cell at coordinate 10, 20 on a grid is addressable, but the address names a place within a larger structure. Replacing the installed floor at that place does not necessarily mean the same individual object survived. The cell can therefore remain compact grid data unless the game deliberately gives it an independent life.

### Five useful categories

| Category | Plain-language meaning | Examples |
|---|---|---|
| Game object / entity | An independently living participant in a world | character, door, item, machine, vehicle, physical projectile, constructed grid |
| Value or record | Information describing something else | health value, open/closed state, color, damage amount |
| Definition | A checked recipe or resource used to create or describe things | prototype, texture, sound, localization entry |
| Structured, bulk, or field data | Addressable or repeated information owned by something larger | installed tile cells, particles, navigation cells, atmosphere cells |
| Presentation object | Client-side material used to show or control the game | menu control, tooltip, screen flash, decorative animation record |

These are product roles, not necessarily programming-language types. One implementation may use classes or structures for several categories without changing their meaning.

### Rules for entities

A thing is a strong candidate for an entity when several of these are true:

- it begins and ends independently;
- a reference should continue to mean the same thing as it moves or changes;
- it can gain or lose reusable capabilities;
- events or interactions target it as a whole;
- it may be inspected, saved, synchronized, owned, or administered individually;
- several systems need to agree that they are discussing the same participant.

A thing is a strong candidate for ordinary, structured, or bulk data when:

- it only has meaning as part of a larger owner;
- copying its value is equivalent to preserving it;
- its address is a position or index rather than a continuing identity;
- very many similar entries are processed together;
- it does not need an independent lifecycle or general-purpose capabilities.

No single checklist item forces the answer. The platform should provide guidance and examples rather than a rigid automatic rule.

### Constructed grids and installed tiles

“Not every tile is an entity” is shorthand for a more precise rule:

- A grid or structure may be an entity because the whole structure has independent identity, movement, physics, ownership, and lifecycle.
- Installed lattice, plating, flooring, or terrain cells may be addressable data owned by that structure.
- Cell construction can retain a base relationship or history, and deconstruction can restore the previous material. Robust Toolbox currently does this with one current tile plus base/history rules; a later Robusta map decision may instead expose explicit logical layers or a hybrid.
- Adding or removing cells may create, expand, shrink, join, split, or remove structure entities.
- A loose construction item may be an entity before placement and may become structured cell data when installed.
- Deconstruction may turn cell state back into a loose entity.
- Anchored or contained objects at a cell remain entities when they have independent identity.
- A game may make a particular tile-like item an entity when its gameplay genuinely requires independent identity and lifecycle.

This is a default modeling rule, not a ban on tile entities.

### Identity does not imply every other capability

The minimal promise of an entity is identity and lifecycle. It does not automatically have to be:

- spatial;
- visible;
- named;
- created from a prototype;
- present on both client and server;
- networked;
- saved;
- controlled by a player.

A server-only logical participant may be an entity. A visible particle may not be one. Networking, saving, rendering, ownership, and position are separate capabilities.

### Data can still be first-class and well tooled

Choosing not to make installed tiles or particles entities must not make them second-class for creators. Specialized data should still support appropriate:

- validation;
- source diagnostics;
- inspection;
- editing;
- events;
- saving;
- synchronization;
- topology and construction rules;
- performance measurements.

The goal is not “ECS for important things and opaque arrays for everything else.” The goal is to give each kind of information a model that matches its identity and scale.

### Context may change the answer

The same idea can be represented differently depending on gameplay:

- An instant laser line used only for feedback can be presentation data.
- A projectile with travel time, collision, ownership, and damage can be an entity.
- Twenty identical sheets can be one stack entity with a quantity.
- Splitting five sheets from that stack creates a second entity because a second independent object now exists.
- An installed floor cell can be grid data.
- A loose floor panel that can be carried, damaged, or owned is naturally an entity.

Moving between these forms must be explicit because identity, references, saving, and networking may change.

### World-wide state is not automatically an entity

ADR 0012 already gives world, host, session, and durable state explicit owners. Robusta should not create immortal “manager entities” merely because the ECS is convenient. A round score, campaign record, matchmaking queue, or account balance should use its proper owner unless it genuinely behaves like an independently living world participant.

### Maps remain a follow-up question

This decision gives us the test for maps and grids but does not pre-decide their final public representation. A later workshop will ask whether a map or grid should appear as a game object, a separate platform object, or both through an adapter.

## Accepted decision statement

**Robusta models an independently living participant in a world as an entity with world-scoped identity and lifecycle. Components describe its state and capabilities, and systems provide behavior. Values, definitions, structured or bulk collections, presentation objects, and host or world-wide state remain separate unless independent identity is genuinely required. A structure may be an entity while its addressable cells remain purpose-built data. Position, rendering, networking, saving, prototype origin, and player control are optional capabilities rather than part of the minimum meaning of an entity.**

## What this deliberately rules out

- Making every visible or mutable thing an entity.
- Making one entity for every installed tile cell, particle, pixel, UI control, or field value by default.
- Treating structured data as unimportant or poorly tooled because it is not an entity.
- Assuming every entity has a position, name, prototype, network identity, or save record.
- Using an immortal singleton entity as hidden global state.
- Treating ordinary C# object identity as game-world identity.
- Deciding object kinds through class inheritance.
- Silently converting data into entities or entities into data while references still exist.

## How we will prove it

A station-like reference game should demonstrate all of the following:

1. A player, door, item, machine, physical projectile, and constructed grid use appropriate entity identities.
2. A non-spatial server-side entity can exist without an automatic transform or renderer.
3. A large tiled room uses compact tile data rather than one entity per installed tile.
4. Lattice, plating, and flooring can be installed and removed in a defined order with good editing, validation, collision, saving, networking, and inspection.
5. Placing the first structural cell can create a grid identity; removing a connection can split the structure and correctly reassign attached objects.
6. A loose floor or construction item can be an entity before placement, become installed cell state, and reappear as an entity during deconstruction according to game rules.
7. A stack remains one entity until it is split into independently addressable stacks.
8. A particle burst and a screen flash do not enter the authoritative simulation.
9. A projectile that needs collision and ownership does enter the entity model.
10. UI controls can display and operate an entity without themselves becoming world entities.
11. An entity can be local-only, replicated, predicted, server-only, saved, or temporary according to separate declarations.
12. Developer tools clearly show whether an identifier is an entity identity, structure identity, definition identity, network identity, or collection address.

A second, non-station reference game should validate that the rule does not depend on doors, grids, or station terminology.

## Technical questions deferred

- The concrete entity-handle representation and stale-reference protection.
- ECS storage layout and whether components are classes, structures, or generated records.
- Whether any components are physically mandatory in storage.
- Standard object templates for common physical entities.
- Bulk and structured-data APIs for tiles, particles, fields, navigation, and similar structures.
- Grid topology, splitting, joining, and identity rules.
- How local presentation entities, if supported, relate to the authoritative world.
- Separate local, network, persistent, and editor identities.
- Promotion and demotion mechanics.
- Whether maps and grids are entities, platform objects, or both at different layers.
- Inspection and source mapping across all runtime-data categories.

## Workshop outcome

The proposal was accepted on 2026-07-18 with one requested clarification: the phrase “not every tile is an entity” must not obscure the way Space Station 14 constructs lattice, plating, and floors or the way Robust Toolbox creates and splits grid entities.

The accepted clarification is:

> The independently living structure may be an entity, while its installed cells are addressable structured data. Construction and deconstruction can move material between loose entity form and installed cell form, and cell changes can create, expand, split, or remove structure identities.

No implementation is claimed by this acceptance.

## References

- ADR 0013: `../decisions/product/0013-use-entities-for-independent-world-participants.md`
- Robust Toolbox entity definition: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/EntityUid.cs>
- Robust Toolbox `MapGridComponent`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/Components/MapGridComponent.cs>
- Robust Toolbox `MapChunk`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/MapChunk.cs>
- Robust Toolbox grids and splitting: <https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html>
- Robust Toolbox tile value: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/Tile.cs>
- Space Station 14 tile definition: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Maps/ContentTileDefinition.cs>
- Space Station 14 lattice and plating prototypes: <https://github.com/space-wizards/space-station-14/blob/master/Resources/Prototypes/Tiles/plating.yml>
- Space Station 14 floor prototypes: <https://github.com/space-wizards/space-station-14/blob/master/Resources/Prototypes/Tiles/floors.yml>
- Space Station 14 floor placement: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Tiles/FloorTileSystem.cs>
- Space Station 14 tile replacement and deconstruction: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Maps/TileSystem.cs>
- Robusta prototype Game SDK world facade: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Game.Shared/GameWorld.cs>
- Robusta prototype entity manager: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/ECS/EntityManager.cs>
