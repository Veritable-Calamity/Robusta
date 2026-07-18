# World Model Workshop 02: What Belongs to a World, and What Belongs Above It?

- **Workshop status:** Accepted
- **Date:** 2026-07-18
- **Likely ADR:** ADR 0012
- **Implementation claim:** None
- **Decision outcome:** Accepted as written during the 2026-07-18 design session and recorded in ADR 0012.

## The question

Once a world is an isolated simulation, what should live inside that world, and what should belong to the game installation, running host, player session, or wider platform?

In simpler terms: when a world is reset or destroyed, what should disappear with it, and what should remain?

## Why this matters

This choice decides whether:

- resetting one match truly gives a clean start;
- two worlds can run without leaking state into one another;
- players can remain connected while a round or world restarts;
- expensive game definitions and assets can be shared safely;
- tests are repeatable;
- a global service quietly becomes a second, hidden world;
- changing a prototype unexpectedly changes already-running objects;
- cross-world features are deliberate rather than accidental.

There are two opposite failure modes. Putting everything in one global game context makes cleanup and isolation unreliable. Copying everything into every world wastes memory and makes shared game identity, networking, and persistence unnecessarily difficult.

## How Robust Toolbox answers

Robust Toolbox has a useful practical separation, even though it does not present it as one formal ownership model.

- The entity manager owns the running entities, components, systems, events, and simulation tick.
- The map manager manages the maps and grids inside that running simulation.
- Prototype and resource managers hold reusable definitions and files used to create and present objects.
- The player manager holds connected sessions and can attach a session to a particular entity.
- A dependency collection makes these services available throughout a running client or server instance.

In everyday terms, Robust Toolbox already distinguishes a recipe from the object made from that recipe, and a connected player from the body that player currently controls. Those are good lessons.

The limitation is that the lifetime boundaries are mostly understood through convention and implementation knowledge. Many services are reached through one ambient dependency collection, which makes it less obvious whether a service belongs to the whole running game, one world, one player, or one tool. This is workable in the normal single-simulation SS14 model, but it is harder to reason about when several isolated worlds, editor previews, or test worlds share a process.

## How the current Robusta prototype answers

The Robusta prototype takes an important step toward explicit ownership. Its host currently creates two visible groups of services:

- **Host services:** configuration, serialization, prototypes, resources, network schemas, and the network manager.
- **World services:** component registration, entities, systems, game time, and the mapping between network and local entities.

The `EngineWorld` owns its timing, entities, systems, and world service scope. Disposing that world deletes its entities and disposes its world services. The host then disposes the host-level services.

In plain language, the prototype has started putting reusable definitions and outside connections above the world, while putting changing simulation state inside the world. That is the right basic direction.

The prototype does not yet provide the complete product answer:

- it normally creates one world per host;
- the package and content catalog are not yet the immutable release artifacts envisioned by the newer design work;
- player identity, persistence, administration, menus, and cross-world transfer do not yet have a complete public model;
- some current placements are implementation experiments rather than stable Game SDK promises.

## How the new Robusta should answer

Robusta should use a small number of plainly named ownership levels. The rule is simple:

> Mutable state belongs to the narrowest owner that truly needs it. Sharing between worlds must be read-only or explicitly coordinated.

### 1. Platform and process

This is the machinery around the game:

- starting and stopping the process;
- operating-system and device access;
- package verification;
- crash reporting;
- common logging and metrics plumbing;
- the client window, graphics device, audio device, or server operating environment.

This level must not contain ordinary mutable gameplay state. It is infrastructure, not a hidden game world.

### 2. Exact game installation and content catalog

This is the immutable description of the game release:

- the exact package receipt and dependencies;
- component, system, event, and network type definitions;
- compiled prototypes and their origins;
- resource and localization catalogs;
- schema definitions;
- default configuration.

Several worlds created from the same exact game release may share this information because it is read-only. A prototype is a recipe; it is not a running object.

During development, a newly compiled catalog is a new catalog generation. Robusta may move a world to that generation only through a declared reload or restart operation. It must not silently change shared definitions underneath running worlds.

### 3. Host and live session

This is the running client or server around one or more worlds:

- network listeners and connections;
- authenticated users and player sessions;
- operator and administration services;
- persistence gateways;
- world creation and supervision;
- live server policy and configuration that applies across worlds;
- the client shell, main menus, installation state, and similar out-of-world presentation.

A player session may outlive a world reset. The player's account or connection is not the same thing as the player's avatar. An avatar is an object inside a world and may be replaced or transferred explicitly.

### 4. World

This is the changing simulation accepted in ADR 0011:

- entities and components;
- mutable system state;
- world time, pause state, timers, and delayed actions;
- maps, transforms, spatial indexes, physics, and containers;
- world events and pending structural changes;
- world-specific random state;
- round, match, or scenario state when the game places it there;
- replication, prediction, and visibility state for that world;
- a snapshot of settings chosen when the world was created.

Destroying a world must remove all of this state without damaging another world or the wider host.

### Explicit cross-world and durable services

Some games need state that spans worlds: accounts, achievements, a shared economy, matchmaking, character records, or a campaign. Such state should live in an explicitly named durable or host-level service with documented rules. It must not be implemented by quietly sharing one world's mutable system or entity objects with another world.

Saving is also an explicit boundary. A live world may ask a persistence service to store selected durable state, but the database or save repository is not itself part of the simulation.

### Configuration follows the same ownership rules

Robusta should not provide one undifferentiated global settings bag. A setting should say whether it belongs to the installation, client user, server operator, exact game definition, one world, or one player session. It should also say whether it can change while running, whether the server is authoritative for it, and whether it is saved.

### A simple ownership test

For any new service or piece of data, ask:

1. Should resetting World A reset it? If yes, it belongs to World A.
2. Can changing it alter World A's simulation result? If yes, it belongs to the world or must enter as an explicit, versioned input.
3. Can every world safely read the same value without changing it? If yes, it may belong to the exact game catalog.
4. Does it coordinate connections, players, storage, processes, or several worlds? If yes, it belongs above worlds and must target worlds explicitly.
5. Does it follow one person rather than one simulation? If yes, it belongs to the player session or durable player record, with explicit transfer into the world.

## Accepted decision statement

**Robusta separates platform infrastructure, immutable game definitions, host and player-session state, and mutable world state. A world owns everything that changes as its simulation advances. Worlds may share only immutable game information by default; mutable cross-world or durable state requires an explicit service and contract. A player's connection and identity live above the world, while the player's current avatar lives inside it.**

## Expected rules

1. Every service and piece of mutable state has a declared owner and lifetime.
2. Mutable gameplay state defaults to world scope.
3. Game definitions shared by worlds are immutable for the lifetime of a catalog generation.
4. Each world receives an explicit game-catalog generation and configuration snapshot.
5. World systems and their mutable state are not silently shared between worlds.
6. Network transport and authenticated sessions may outlive a world; replication state belongs to the world being observed.
7. A player identity or connection is not represented solely by an entity.
8. Destroying a world disposes all world-owned state and detaches or transfers sessions explicitly.
9. Cross-world state uses a named service with documented consistency, failure, and persistence behavior.
10. Logging and metrics infrastructure may be shared, but every world-originated record carries world identity.
11. No process-wide mutable singleton may act as undeclared gameplay state.
12. These ownership boundaries improve correctness; they do not replace process isolation for untrusted games.

## What this deliberately rules out

- One giant global object containing all game and simulation state.
- Sharing mutable system instances or entity references between worlds.
- Treating a network connection, user account, and in-world body as the same object.
- Silently applying prototype or configuration changes to every running world.
- Giving world code unrestricted access to launcher state, package credentials, or arbitrary operating-system facilities.
- Copying the entire game package and every immutable asset into each world merely to achieve isolation.
- Calling a shared database or economy “world state” without defining its cross-world behavior.

## How we would prove it

A conformance fixture should be able to:

1. Create two worlds that share one exact read-only content catalog.
2. Change entities, timers, random state, systems, and maps in one world without affecting the other.
3. Destroy and recreate one world while the host and the other world continue running.
4. Keep a player session connected while detaching its old avatar and attaching a new avatar in a replacement world.
5. Demonstrate that a prototype definition is shared while each spawned object's mutable values remain world-local.
6. Reject an attempt to place mutable gameplay state in an undeclared process-global service.
7. Demonstrate an explicit durable service shared by two worlds without allowing either world to access the other's entities.
8. Show that a development catalog update is applied transactionally through reload or restart rather than mutating a catalog in place.
9. Tag logs, metrics, saves, and network diagnostics with the responsible game, host, session, and world identities.
10. Verify that disposing a world releases world-owned resources and subscriptions.

## Technical questions deferred

- The exact dependency-injection or capability mechanism used to enforce lifetimes.
- Whether immutable resources are memory-mapped, cached, copied, or shared by reference.
- How a catalog generation is represented and swapped during development.
- Whether a network connection may observe several worlds at once.
- How player transfer transactions work.
- How durable services handle concurrency, failure, and transactions.
- Which configuration keys are platform-, host-, world-, session-, or player-scoped.
- How client menus and in-world UI are separated in the public SDK.
- Which services are visible to ordinary game systems and which require advanced capabilities.

## Outcome

Accepted on 2026-07-18 and recorded in ADR 0012. The accepted decision separates platform infrastructure, immutable game-catalog generations, host and player-session state, and mutable world state. It also places a player identity and connection above the world while keeping the player's current avatar inside one world.

## References

- ADR 0011: `../decisions/product/0011-define-world-as-isolated-simulation.md`
- Robust Toolbox `IoCManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/IoC/IoCManager.cs>
- Robust Toolbox `IEntityManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/IEntityManager.cs>
- Robust Toolbox `IMapManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/IMapManager.cs>
- Robust Toolbox `IPrototypeManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Prototypes/IPrototypeManager.cs>
- Robust Toolbox `IResourceManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/ContentPack/IResourceManager.cs>
- Robust Toolbox `ISharedPlayerManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Player/ISharedPlayerManager.cs>
- Robusta prototype `EngineHost`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineHost.cs>
- Robusta prototype `EngineWorld`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineWorld.cs>
