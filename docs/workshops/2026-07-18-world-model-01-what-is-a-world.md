# World Model Workshop 01: What Is a World?

- **Workshop status:** Accepted
- **Date:** 2026-07-18
- **Likely ADR:** ADR 0011
- **Implementation claim:** None
- **Decision outcome:** Accepted as written during the 2026-07-18 design session and recorded in ADR 0011.

## The question

What does Robusta mean when it says “world”?

Is a world the whole application, one map, one round, one match, or an isolated simulation that can contain several maps?

## Why this matters

The answer determines what can be created, reset, paused, tested, saved, or shut down independently.

It also determines whether:

- two tests contaminate one another;
- an editor preview can run beside a game session;
- one server can host several independent matches;
- maps can share players and game rules;
- global state quietly leaks between games or worlds;
- deleting one world can damage another.

## How Robust Toolbox answers

Robust Toolbox gives a running engine session one entity manager holding the active entities and systems. Its map manager describes itself as managing all maps and grids “in the world.” Several maps and grids can exist inside the same overall simulation, and maps can be paused or deleted individually.

This works well for a station-like game: the station, shuttles, test maps, and other spaces can remain part of one connected simulation.

The limitation is that “world” is largely an implicit boundary around the active engine session. Maps are clearly represented, but creating several completely independent simulations with separate time, entities, systems, and mutable services is not the ordinary game-facing model.

## How the Robusta prototype answers

The Robusta prototype introduces an explicit `EngineWorld`. It owns its own game timing, entity manager, system manager, and world-scoped services. An `EngineHost` currently constructs one such world and advances it through a fixed simulation loop.

This is a strong isolation lesson. Tests or tools can also create multiple independent hosts in one process.

The remaining problem is that the prototype largely binds one host to one world and has not published a plain-language contract explaining whether maps, rounds, matches, editor previews, and network sessions are worlds or things inside worlds.

## Recommended answer for the new Robusta

A **world is one isolated, mutable simulation**.

It owns the things that change together as game time advances:

- game objects and their components;
- game systems and their world-specific state;
- the simulation clock, pause state, and timers;
- maps and other playable spaces;
- spatial and physics state;
- world-scoped random state;
- world events and pending structural changes;
- the part of player and network state that belongs to that simulation.

A world is **not** the game package, the whole process, or one map.

- A **game package** supplies the code, content, and rules from which worlds are created.
- A **host process** runs the platform and may supervise one or more worlds belonging to one exact trusted game installation.
- A **map** is a space inside a world. One world may contain several maps whose objects can interact or transfer under the same simulation rules.
- A **match or round** may use one world, reset a world, or be one of several worlds; the game chooses within platform rules.

## Draft decision statement

**Robusta defines a world as an isolated mutable simulation containing its own entities, systems, game time, maps, spatial state, timers, and world-scoped services. Maps are spaces inside a world, not worlds themselves. A trusted host may run more than one world, but separate or untrusted game packages remain separate processes.**

## Expected rules

1. Every running entity belongs to exactly one world.
2. A world may contain zero, one, or many maps.
3. Time, pause, events, lifecycle, physics, and mutable system state are scoped to one world.
4. Worlds can be created, reset, tested, and destroyed independently.
5. World-local identifiers may be reused in another world without collision or confusion.
6. Ordinary references do not cross world boundaries.
7. Moving durable game state between worlds is an explicit transfer operation with defined success or failure behavior.
8. Immutable or read-only game information may be shared above worlds when doing so cannot leak mutable state.
9. A world boundary improves correctness and testing; it is not a security sandbox.
10. The first released client or server may commonly run one primary world, but the public architecture and conformance tests must not require that limitation.

## What this deliberately rules out

- Treating each map as a completely separate game world by default.
- Treating the entire process as one unavoidable global world.
- Allowing mutable game services to be silently shared across worlds.
- Letting an entity reference from one world accidentally resolve to an entity in another.
- Using in-process worlds as the security boundary between untrusted games.
- Requiring every game to support multiple live matches in one process before 1.0.

## How we would prove it

A conformance fixture should be able to:

1. Create two worlds from the same game package in one trusted host.
2. Advance their clocks independently.
3. Use the same local entity values without cross-resolution.
4. Raise events in one world without reaching the other.
5. Dispose one world while the other remains playable.
6. Run an editor preview or isolated test world beside a normal world.
7. Create several maps inside one world and move an entity between them without changing worlds.
8. Demonstrate that mutable service state is not shared accidentally.
9. Demonstrate that incompatible or untrusted game packages still launch in separate processes.

## Technical questions deferred

- Whether a host stores worlds directly or through a world supervisor.
- Whether systems are instantiated once per world or separated into shared definitions and world state.
- Which immutable catalogs may be shared safely.
- How sessions and players transfer between worlds.
- Whether a network connection may observe more than one world.
- How world identifiers appear in saves, logs, metrics, and network messages.
- Whether server process fault isolation requires one process per world in some deployment modes.
- Which world operations are available in the public Game SDK.

## Outcome

Accepted on 2026-07-18 and recorded in ADR 0011. The accepted decision treats a world as one isolated mutable simulation, allows several maps inside one world, permits several trusted worlds in one host, and retains process separation for different or untrusted executable games.

## References

- Robust Toolbox map manager: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/IMapManager.cs>
- Robust Toolbox entity manager: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/IEntityManager.cs>
- Robusta prototype `EngineWorld`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineWorld.cs>
- Robusta prototype `EngineHost`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineHost.cs>
