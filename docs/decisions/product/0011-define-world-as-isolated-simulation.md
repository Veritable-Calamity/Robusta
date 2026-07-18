# ADR 0011: Define a World as an Isolated Simulation Containing Multiple Maps

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0001, ADR 0003, ADR 0004, ADR 0006

## The question

What is a world, and which running state must be isolated within it?

## The promise

A developer can create, reset, pause, test, inspect, and destroy one simulation without contaminating another, while still placing several maps or playable spaces inside the same connected game simulation.

## Why this matters

If the whole process is treated as one implicit world, mutable global state makes tests, previews, parallel matches, restart, and cleanup difficult. If every map is treated as a separate world, ordinary movement between station spaces, shuttles, interiors, or temporary maps becomes needlessly complex.

Robust Toolbox ordinarily presents one active entity-and-system simulation containing multiple maps and grids. The Robusta prototype improves isolation by creating an explicit world with its own entities, systems, timing, and service scope, but currently creates one world per host and does not yet define the public product meaning of the term.

## Options considered

### A world is the whole process

Simple initially, but it encourages global mutable state and makes independent testing, previews, or multiple sessions difficult.

### A world is one map

Easy to visualize, but maps often need to share players, rules, time, references, and movement within one connected simulation.

### A world is one isolated mutable simulation containing maps

The simulation boundary is explicit, while maps remain spatial subdivisions within it. A trusted host may supervise several worlds when useful.

## Decision

Robusta defines a **world** as one isolated mutable simulation.

A world owns its entities, systems and world-specific system state, simulation clock, pause state, timers, maps, spatial and physics state, random state, events, pending structural changes, and other mutable services that advance with that simulation.

A map is a playable or logical space inside a world. One world may contain several maps.

A game package provides the code, content, and rules used to create worlds. A trusted host process may run one or more worlds belonging to the same exact game installation. Separate or untrusted game packages remain in separate processes.

Every entity belongs to exactly one world. Ordinary entity references cannot cross worlds. Moving durable state between worlds is an explicit transfer operation.

The first release may commonly run one primary world per client or server, but public contracts and tests must preserve the ability to create multiple isolated worlds.

## What we deliberately will not do

- Define a map and a world as the same thing.
- Make process-global mutable state part of ordinary gameplay.
- Allow entity identifiers to resolve across worlds implicitly.
- Treat an in-process world boundary as a security sandbox.
- Require multi-match production hosting as a 1.0 feature merely because multiple worlds are supported architecturally.

## Consequences

### Benefits

- Tests, editor previews, and parallel simulations can be isolated.
- Multiple maps can remain part of one connected game.
- World reset and shutdown have a clear ownership boundary.
- Future server deployment may choose one or several worlds per process without changing game concepts.

### Costs and limitations

- Services must be classified carefully as process-, game-, host-, or world-scoped.
- Cross-world player or state transfer needs an explicit contract.
- Logging, saves, networking, and metrics need world identity.
- Multi-world support adds conformance work even when typical applications use one world.

## How we will prove the decision works

- Two worlds in one trusted host have independent time, entities, systems, events, and mutable services.
- Identical local entity values cannot cause cross-world resolution.
- Destroying one world does not affect another.
- One world contains several maps and moves an entity between them.
- An editor preview or test world can run beside a normal world.
- Untrusted or incompatible games still require separate processes.

## Implementation notes

No greenfield implementation exists. The Robusta prototype's `EngineWorld` is evidence for the isolation direction, not the final contract.

## Follow-up decisions

- Define game-, host-, world-, and process-scoped services.
- Define entity identity and stale-reference behavior.
- Define map identity, coordinates, and transfer.
- Define world creation, reset, pause, save, and disposal lifecycle.
- Define player/session membership and cross-world transfer.

## References

- Workshop record: `../../workshops/2026-07-18-world-model-01-what-is-a-world.md`
- Robust Toolbox `IMapManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/Map/IMapManager.cs>
- Robust Toolbox `IEntityManager`: <https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/IEntityManager.cs>
- Robusta prototype `EngineWorld`: <https://github.com/Veritable-Calamity/Robusta/blob/Robusta-UGC/Robusta.Shared/Hosting/EngineWorld.cs>
