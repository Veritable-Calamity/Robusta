# ADR 0012: Separate Immutable Game Definitions, Host Sessions, and Mutable World State

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0003, ADR 0004, ADR 0005, ADR 0008, ADR 0009, ADR 0011

## The question

What belongs inside a world, and what should remain outside it as part of the exact game installation, running host, player session, or wider platform?

## The promise

A developer can reset, replace, test, or destroy one world without leaking mutable state into another, while safely sharing read-only game definitions and allowing player connections, administration, and durable data to outlive a particular world.

## Why this matters

ADR 0011 makes a world an explicit isolation boundary. That boundary is useful only if Robusta states what the world actually owns.

Putting everything in one global game context creates hidden state, incomplete reset, flaky tests, and cross-match contamination. Duplicating every prototype, asset, network listener, account, and package record inside each world is wasteful and confuses the difference between a game definition and a running simulation.

The platform therefore needs a plain ownership model that remains understandable before any particular dependency-injection mechanism is chosen.

## How Robust Toolbox answers today

Robust Toolbox makes several useful practical distinctions: the entity manager owns running entities and systems; maps and grids belong to that simulation; prototype and resource managers provide reusable definitions and files; and the player manager tracks connected sessions separately from the entity currently controlled by each session.

These distinctions support productive game development. Their ownership boundaries are nevertheless largely conventional and are commonly reached through one thread-local dependency collection. This makes the lifetime of a service less obvious when considering several isolated worlds, previews, or test simulations in one process.

## How the Robusta prototype answers today

The Robusta prototype explicitly creates host and world service scopes. Configuration, serialization, prototypes, resources, schemas, and networking are currently host-level. Components, entities, systems, timing, and network-entity mapping are currently world-level. `EngineWorld` owns and disposes its entities and world services.

This is good evidence for the direction, but it is not yet a complete product contract. The prototype normally creates one world per host and does not yet provide final public rules for player sessions, persistence, administration, catalog generations, cross-world services, or development reload.

## Options considered

### Put all game state in one global context

This is initially easy to access, but reset and isolation depend on convention. Hidden shared state becomes likely, and running several worlds safely becomes difficult.

### Duplicate every service and definition inside every world

This provides strong separation but wastes memory, duplicates immutable work, and makes player sessions, networking, persistence, and game identity difficult to represent cleanly.

### Use explicit ownership levels and share only immutable definitions by default

Game definitions can be reused safely, mutable simulations remain isolated, and state that legitimately spans worlds must declare its own contract.

## Decision

Robusta separates state into four product-level ownership areas:

1. **Platform and process infrastructure** owns process lifecycle, operating-system and device access, package verification, crash handling, and common diagnostic plumbing. It does not own ordinary mutable gameplay state.
2. **The exact game installation and content catalog** own immutable package identity, generated type and schema definitions, compiled prototypes, resource and localization catalogs, and default configuration. Worlds from the same exact catalog generation may share this information read-only.
3. **The host and live session layer** owns network listeners and connections, authenticated users, player sessions, administration, persistence gateways, world supervision, and other state intended to outlive a particular world.
4. **Each world** owns entities, components, mutable system state, simulation time, pause state, timers, maps, spatial and physics state, world events, random state, pending changes, world-specific rules, and replication state for that simulation.

Mutable state belongs to the narrowest owner that truly needs it. Mutable gameplay state defaults to world scope. Sharing mutable state between worlds requires an explicitly named cross-world or durable service with documented behavior.

A player's connection, account, and session live above the world. The player's current avatar is an entity inside one world. Detaching, replacing, or transferring that avatar is explicit.

A catalog generation is immutable once supplied to a running world. Development changes create a new catalog generation and are applied through a declared transactional reload or restart operation, never by silently changing shared definitions in place.

Configuration follows the same ownership model. Each setting declares whether it belongs to the installation, client user, server operator, exact game definition, one world, or one player session; whether it may change while running; whether it is authoritative; and whether it is persisted. Robusta does not expose one undifferentiated global settings bag.

## What we deliberately will not do

- Permit a process-global mutable singleton to act as undeclared gameplay state.
- Share mutable world systems, timers, random state, entities, or entity references across worlds.
- Treat the player account, network connection, and current avatar as one inseparable object.
- Silently change prototypes or configuration underneath all running worlds.
- Give ordinary world code direct access to launcher credentials, package-management state, or arbitrary operating-system facilities.
- Copy all immutable game data into every world merely to achieve isolation.
- Treat an in-process ownership boundary as a security sandbox.

## Consequences

### Benefits

- World reset and disposal have a complete, testable meaning.
- Several worlds can share expensive read-only game information without sharing mutable simulation state.
- Player sessions can survive round or world replacement.
- Cross-world campaigns, accounts, and economies become explicit product features rather than hidden global state.
- Prototype reload, persistence, networking, tests, logs, and metrics gain clear ownership.

### Costs and limitations

- Every service must declare and obey a lifetime.
- The Game SDK must distinguish world capabilities from host or durable services.
- Player transfer and cross-world state need explicit failure and consistency rules.
- Development reload must manage catalog generations rather than mutating definitions freely.
- Some games that previously used convenient global state will need deliberate services.

## How we will prove the decision works

- Two worlds share one read-only game catalog while all entity, timer, system, random, map, physics, and replication mutations remain isolated.
- One world can be destroyed and recreated while the host, network listener, authenticated sessions, and another world remain active.
- A player session can detach from an avatar in one world and attach to a new avatar in another through an explicit operation.
- A prototype recipe is shared, while every spawned object's values remain world-local.
- Platform tests reject undeclared mutable gameplay state registered at process or game-catalog scope.
- A declared durable service can serve several worlds without exposing one world's entities to another.
- A development catalog change is applied transactionally or causes a documented restart.
- Logs, metrics, saves, and network diagnostics identify the responsible game, host, session, and world.

## Implementation notes

No greenfield implementation exists. The prototype's host/world service split is evidence for the direction, not the final service taxonomy or public API.

## Follow-up decisions

- Define the public service and capability model for game systems.
- Define player identity, session, avatar ownership, and transfer.
- Define configuration ownership and snapshots.
- Define catalog generations and development reload.
- Define durable and cross-world service contracts.
- Define save repositories and world persistence.
- Define whether one network connection may observe multiple worlds.
- Define logging, metrics, and tracing identities.

## References

- Workshop record: `../../workshops/2026-07-18-world-model-02-what-belongs-where.md`
- ADR 0011: `0011-define-world-as-isolated-simulation.md`
- Robust Toolbox `IoCManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/IoC/IoCManager.cs>
- Robust Toolbox `IEntityManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/IEntityManager.cs>
- Robust Toolbox `IPrototypeManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Prototypes/IPrototypeManager.cs>
- Robust Toolbox `ISharedPlayerManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Player/ISharedPlayerManager.cs>
- Robusta prototype `EngineHost`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineHost.cs>
- Robusta prototype `EngineWorld`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineWorld.cs>
