# ADR 0013: Use Entities for Independent World Participants, Not All Data

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0002, ADR 0003, ADR 0005, ADR 0006, ADR 0011, ADR 0012

## The question

What makes something a first-class game object, rather than ordinary data, a definition, a visual effect, a user-interface element, or one addressable entry in a larger structure?

## The promise

A developer can use one consistent object model for things that truly live independently in a world, without paying the cost or accepting the confusion of turning every tile cell, number, particle, menu control, or rule into an entity.

Addressable constructed terrain remains a first-class part of the game model even when each installed tile is not a separate entity. A whole grid or structure may have identity, while its cells use compact, purpose-built storage and construction rules.

## Why this matters

Entity-component-system designs are productive because many different game objects can be assembled from reusable capabilities. That strength can become a weakness when “make it an entity” becomes the answer to every modeling question.

An object needs more than storage. It needs identity, a lifetime, and a reason to be addressed as a whole. A health value, an installed floor cell, a sprite layer, and a particle may change over time, but that alone does not mean each should receive a general-purpose world identity.

The opposite mistake is also possible. Saying “tiles are data” must not imply that construction, deconstruction, grid growth, structural splitting, collision, saving, networking, or inspection are unimportant. Specialized data can participate in rich gameplay without each entry becoming a general-purpose entity.

Without a clear rule, games may create millions of unnecessary entities, hide world-wide state inside immortal singleton entities, make user-interface objects part of the authoritative simulation, or assume that every entity must be positioned, networked, saved, and created from a prototype.

## How Robust Toolbox answers today

Robust Toolbox defines an entity as a unique local identifier plus a collection of components. Components hold data and systems provide behavior. This supports composition: walls, players, items, projectiles, machines, and grids can be inspected and changed through a common model.

A Robust Toolbox grid is itself an entity with a grid component. The grid has its own transform, physics body, lifecycle, and identity. Its tile cells are stored in compact chunks rather than as one entity per cell. A tile is addressed through the grid and a coordinate.

Space Station 14 builds station flooring through those addressable cells:

- lattice declares space as its underlying turf;
- plating declares lattice as its underlying turf;
- ordinary station flooring declares plating as its underlying turf;
- each coordinate stores one current tile value rather than simultaneous lattice, plating, and floor entities;
- placing construction material replaces that current tile value; the apparent layers are represented through base-turf rules and, when needed, retained tile history;
- deconstruction restores the prior or declared underlying tile and may spawn a loose item entity;
- placing the first suitable structural tile into open space can create a new grid entity;
- removing connecting tiles can cause Robust Toolbox to split disconnected sections into new grid entities and move their contents with them.

This means “not one entity per installed tile” does **not** mean the structure lacks identity. The independently moving station, shuttle, or constructed section has identity as a grid entity. Each installed cell is an addressable part of that structure. Loose construction materials, machines, walls, and other independently living things at those coordinates may still be entities.

Robust Toolbox also demonstrates other non-entity models: user-interface controls use a separate UI model, prototypes are recipes rather than live objects, and clients may create local entities for effects that do not exist on the server.

This combination is highly productive. The main limitation is that the product-level boundary is mostly learned through code and convention. It is easy for a developer to know how to make an entity, but less obvious when a specialized data structure, presentation object, world-level record, or simple value is the better choice. Robust Toolbox also gives every ordinary entity core metadata and transform behavior, which makes the common physical-object case convenient but can blur which capabilities are truly universal.

## How the Robusta prototype answers today

The Robusta prototype exposes an opaque `GameEntity` scoped to one game world. Its public world API can create and delete entities, attach components, inspect components, and query entities. Internally, the entity manager automatically adds metadata and prototype spawning normally ensures a transform.

This is a clean early boundary: game code receives an identity without learning the storage implementation. The prototype does not yet state which concepts should be entities, which capabilities every entity must have, or how bulk data, constructed grids, world-level state, maps, UI, and presentation-only effects should relate to the object model.

The lesson is that an opaque entity handle is useful, but the platform still needs a plain rule for when that handle should exist.

## Options considered

### Make every changing or visible thing an entity

This creates one universal programming model and makes inspection uniform. It also creates needless identity and lifecycle overhead for tile cells, particles, values, and other data that are naturally managed in bulk. It encourages platform-specific workarounds for problems better represented directly.

### Reserve entities for visible physical objects

This is easy to explain, but it excludes important non-physical participants such as objectives, logical links, server-only actors, or other independently living concepts. It also incorrectly suggests that every visible effect or UI element belongs to the authoritative world.

### Use entities for independently living world participants and purpose-built data for larger structures

This gives meaningful objects a common identity and capability model while allowing compact, addressable representations for data whose individual entries do not have independent lives.

## Decision

Robusta uses the terms **entity** and **game object** for the same product concept: an independently living participant in one world.

A game object has:

- one world-scoped identity;
- a beginning and an end;
- state and capabilities that may be added, removed, or changed according to the lifecycle rules;
- the ability to be targeted, inspected, referenced, or acted upon as a whole.

Components describe the object's state and capabilities. Systems provide the rules that operate on those components and objects.

Identity is the deciding distinction. An object's identity follows that object as it moves or changes. By contrast, an address such as “cell 10, 20 on grid A” identifies a place within a larger structure. Replacing the installed material at that address does not necessarily preserve the same individual object.

Robusta does not treat every piece of mutable data as an entity. The following remain separate concepts unless a game has a specific reason to promote them:

- values and records, such as health numbers, colors, settings, and component fields;
- immutable definitions, such as prototypes, resources, and compiled catalog entries;
- dense or repetitive collections, such as tile cells, pixels, particles, navigation cells, and simulation fields;
- client presentation objects, such as ordinary menu controls, screen effects, and decorative animation records;
- host, session, durable, or world-wide state already assigned an owner by ADR 0012.

A collection entry may have a stable address, construction history, validation rules, events, permissions, saving, networking, collision, and editing without receiving a general-purpose entity identity.

### Constructed grids and installed tiles

For constructed tile structures, the default product model is:

- The independently moving or separately living **structure or grid may be an entity**.
- The grid's installed cells are **addressable data owned by that structure**, not automatically one entity per cell.
- Placing lattice, plating, flooring, terrain, or a similar installed material may change the current state of a cell and retain enough base or history information to reverse that construction correctly. Robust Toolbox and current SS14 use one current tile plus base/history rules; Robusta may later choose that model, explicit logical layers, or a documented hybrid.
- Changing cells may create, expand, shrink, join, split, or remove structure identities. These topology changes are explicit world operations, not proof that each cell was an entity.
- Loose construction material before placement, or an item produced by deconstruction, may be an entity because it can be carried, owned, stored, moved, and referenced independently.
- Walls, machines, cables, fixtures, or other independently living objects attached to a cell may remain entities even though the substrate beneath them is compact grid data.
- A game may model a particular tile-like object as an entity when it genuinely needs an independent identity and lifecycle. “Not one entity per tile” is a default modeling rule, not a prohibition.

This ADR does not yet decide the exact public representation of maps and grids in Robusta, nor whether a cell is exposed as one resolved tile with history or as several named logical layers. A later decision will apply the identity-and-lifecycle rule to those choices. The constructed-grid example establishes that a large entity may own richly interactive, addressable internal data without converting every entry or construction layer into another entity.

### Optional capabilities

Being an entity does **not** automatically mean that the object:

- has a position;
- has a display name;
- was created from a prototype;
- is visible or rendered;
- exists on both client and server;
- is synchronized over the network;
- is persistent or included in a save;
- is controlled by a player.

Those are separate capabilities or policies. Ordinary physical objects may receive convenient standard capabilities, but the minimal meaning of an entity is identity and lifecycle.

Promotion from data into an entity, or reduction from an entity into structured data, is an explicit game or platform operation when supported. The operation must define what happens to identity, references, saved state, and network visibility.

## What we deliberately will not do

- Turn every installed tile cell, particle, sprite layer, UI control, timer, or configuration value into an entity by default.
- Treat “not an entity” as meaning “not inspectable, interactive, saveable, networked, or important.”
- Require every entity to have a transform, name, prototype, renderer, network identity, or save record.
- Use one immortal “manager entity” as an undeclared substitute for world, host, or durable state.
- Treat a programming-language object or class instance as automatically being a game object.
- Make class inheritance determine game-object capabilities.
- Force specialized bulk data to use the same storage and lifecycle machinery as general-purpose entities.
- Hide promotion, demotion, grid creation, or grid splitting behind silent conversion that breaks references.

## Consequences

### Benefits

- Game objects retain the familiar component-and-system authoring model.
- Entity identity remains meaningful rather than becoming a universal row number for all data.
- Constructed grids can be individually movable, inspectable world participants while containing efficient tile data.
- Large tilemaps, particle systems, navigation data, and other dense structures can use efficient purpose-built representations.
- Invisible, non-spatial, local-only, and server-only entities remain possible where independent identity is useful.
- Networking, saving, rendering, and prototyping can be opted into independently.
- Tooling can explain whether the developer is inspecting an object, a definition, a collection entry, a structure, or presentation state.

### Costs and limitations

- Robusta must support more than one form of inspectable runtime data instead of assuming that the ECS contains everything.
- Documentation and tooling must help developers choose among an entity, an addressable structure, and a simple value.
- Constructed structures need explicit rules for cell replacement, base layers, topology changes, ownership, and identity changes.
- Some concepts are context-dependent. A physical projectile may be an entity, while an instant beam effect may be presentation data.
- Promotion and demotion require explicit reference and compatibility rules.
- Later ADRs must define which standard capabilities are automatically provided by common object templates without making them universal.

## How we will prove the decision works

- A player, door, item, and independently simulated projectile are normal entities assembled from components and operated on by systems.
- An entity can exist without a transform, display name, prototype origin, network synchronization, or save policy.
- A station-like test can place lattice in open space, add plating, add flooring, remove those layers in a defined order, and preserve correct saving, networking, collision, and inspection without creating one entity per installed cell.
- Creating the first structural cell may create a structure identity; adding cells may expand it; removing a connection may split it into separate structure identities with contained or attached objects reassigned correctly.
- A loose floor-tile or construction-material item can be an entity before placement, become installed cell state, and be recreated as an item during deconstruction according to game rules.
- A large tilemap remains editable, inspectable, validated, saved, and synchronized through a purpose-built model.
- A stack of twenty identical units can be one entity with quantity data; splitting the stack creates a second identity at the moment independent life begins.
- An instant visual beam or particle burst can be client presentation data, while a projectile requiring collision, ownership, and independent lifetime can be an entity.
- User-interface controls do not enter the authoritative world merely because they display or edit an entity.
- A world-wide rule or durable account record uses the ownership model from ADR 0012 rather than an immortal singleton entity.
- Development inspection clearly distinguishes entity identity, structure identity, collection address, network identity, and definition identity.
- Performance fixtures show that dense data is not forced through general-purpose entity lifecycle and networking paths.

## Implementation notes

No greenfield implementation exists. The prototype's opaque `GameEntity` and component operations are evidence for the familiar authoring model, not a final decision on entity storage, mandatory components, map or grid representation, or bulk-data facilities.

## Follow-up decisions

- Define how an object begins its life and when it becomes observable.
- Define how an object ends its life and how cleanup propagates.
- Define stale-reference behavior and whether handles carry a generation.
- Define when capabilities may be added or removed.
- Define the standard platform-owned capabilities offered to common world objects.
- Define maps, grids, tiles, coordinates, containment, topology changes, and structure identity.
- Define client-only presentation state and its relationship to authoritative objects.
- Define network, save, and durable identities separately from world-local entity identity.
- Define inspection and editing support for specialized bulk and structured data.

## References

- Workshop record: `../../workshops/2026-07-18-world-model-03-what-is-a-game-object.md`
- ADR 0011: `0011-define-world-as-isolated-simulation.md`
- ADR 0012: `0012-separate-game-host-and-world-state.md`
- Robust Toolbox `EntityUid`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/EntityUid.cs>
- Robust Toolbox `MapGridComponent`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/Components/MapGridComponent.cs>
- Robust Toolbox `MapChunk`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/MapChunk.cs>
- Robust Toolbox grid and grid-splitting guide: <https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html>
- Robust Toolbox `Tile`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/Tile.cs>
- Space Station 14 tile definitions: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Maps/ContentTileDefinition.cs>
- Space Station 14 lattice and plating prototypes: <https://github.com/space-wizards/space-station-14/blob/master/Resources/Prototypes/Tiles/plating.yml>
- Space Station 14 floor prototypes: <https://github.com/space-wizards/space-station-14/blob/master/Resources/Prototypes/Tiles/floors.yml>
- Space Station 14 floor placement: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Tiles/FloorTileSystem.cs>
- Space Station 14 tile replacement and deconstruction: <https://github.com/space-wizards/space-station-14/blob/master/Content.Shared/Maps/TileSystem.cs>
- Robusta prototype Game SDK world facade: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Game.Shared/GameWorld.cs>
- Robusta prototype entity manager: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/ECS/EntityManager.cs>
