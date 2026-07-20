# ADR 0033: Provide platform mechanics with game-defined semantics

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0001, 0003, 0006, 0007, 0009, 0011-0016, 0018-0021, 0023, 0024, 0026, 0029, 0030-0034

## The question

Which foundations must Robusta supply to every game, and which familiar gameplay concepts must remain owned by each game?

## The promise

Robusta supplies the reusable mechanics needed to build and operate a 2D game without embedding one game's rules into the engine. A station-like game can build inventories, combat, construction, atmosphere, and rounds, while a contrasting game can ignore those concepts without fighting platform assumptions.

## Why this matters

Space Station 14 demonstrates how much value a shared engine can provide through entities, maps, grids, physics, rendering, UI, audio, content, and networking. It also demonstrates the cost of letting station-specific gameplay vocabulary become an implicit engine architecture. ADR 0001 promises a complete platform for independent teams, not a minimal library and not a Space Station 14 framework.

## How Robust Toolbox answers today

Robust Toolbox provides a broad 2D engine foundation used by Space Station 14. The game supplies most station semantics, but long co-evolution and shared repository boundaries can make the division between reusable platform mechanics and content-specific assumptions difficult for an external adopter to see.

## How the Robusta prototype answers today

The predecessor has early runtime, ECS, transform, grid, rendering, and content foundations, but does not yet demonstrate the complete supported-platform floor or an independently maintained non-station game.

## Options considered

### Option A: A complete mechanical floor with game-defined semantics

The platform owns reusable lifecycle, spatial, presentation, input, content, networking, tooling, and operational mechanics. Games own the meanings and rules composed from them.

### Option B: A minimal kernel with nearly everything in game packages

The platform supplies only hosting and low-level ECS primitives. This keeps the core small but makes every team rebuild essential client, spatial, networking, tooling, and operational foundations.

### Option C: A batteries-included station-game framework

The platform supplies health, inventory, combat, factions, jobs, rounds, atmosphere, power, and similar domain systems. This accelerates one genre while contradicting the independent-game and contrasting-game promises.

## Decision

Robusta will use Option A.

The product contract is:

1. The supported platform floor includes host, session, world, entity, lifecycle, time, events, scheduling, content, diagnostics, configuration, and delivery contracts.
2. The 2D spatial floor includes runtime maps, frame-qualified positions, transforms, typed relations, an optional-to-consume static compact-grid and cell capability, spatial queries, and basic collision and physics mechanics. Later support labels govern dynamic grid construction, splitting, and merging.
3. The client floor includes cameras, 2D rendering and sprites, input actions, UI composition, audio, localization, presentation timing, and accessibility-relevant extension points.
4. The multiplayer floor includes authoritative state, declared synchronization, interest and visibility, connection and session handling, prediction and correction hooks, and inspection. Later ADRs define their exact limits.
5. These are capabilities, not mandatory fields on every entity or mandatory services in every game. An unadmitted capability performs no initialization or update work, allocates no per-world or per-entity state, starts no thread or native service, and does not force capability-specific client payloads or native dependencies. Package size, startup, steady-state memory, and step-time overhead for headless, non-spatial, and other minimal profiles are measured and budgeted rather than assumed zero.
6. Games own domain semantics including health and damage rules, inventory rules, combat, factions, jobs, quests, rounds, atmosphere, power, economy, crafting recipes, and game modes.
7. The platform may provide neutral mechanisms used by those semantics, such as containment, damage-event transport, resource graphs, tags, or state-machine support, but does not assign station-specific meaning to them.
8. Robusta may publish and maintain optional, versioned gameplay or genre packages, including batteries-included station-game building blocks. Each package is authored only through published Game SDK, content, creator, extension, and packaging contracts; it is built, validated, versioned, installed, and admitted through the same supported paths available to an independent game developer. Platform authorship grants no internal reference, hidden registry, private callback, default allow-list entry, scheduler bypass, or conformance exception.
9. ADR 0007 trust categories follow a package's contents and installation context, not who publishes it. An executable station-game package is trusted full-game or operator-installed material as applicable. Any projection offered as public UGC remains limited to the game-approved data and declarative capability boundary; publishing a trusted package does not grant executable powers to public UGC.
10. An optional genre package is not part of the universal platform floor, is not installed or activated implicitly, and may not become a dependency of minimal, contrasting, or unrelated games. A game may adopt, replace, fork, omit, or independently reimplement it through ordinary declared dependencies and side-specific projections.
11. An optional genre package may compose neutral platform mechanics but may not weaken or bypass an accepted authority, lifecycle, trust, side-separation, determinism, delivery, or creator-workflow contract. A need that cannot be met through the public SDK follows ADR 0034's extension or contribution ladder, not a private exception for a platform-maintained package.
12. Public SDK names, defaults, examples, and diagnostics use domain-neutral language unless a package is explicitly game- or genre-specific.
13. An optional gameplay abstraction is promoted from a game package into the platform when at least two meaningfully different games need substantially the same invariant, lifecycle, optimization boundary, or tool integration and cannot safely implement it through the supported SDK.
14. The two-game promotion test does not override an already accepted product obligation or a platform-wide security, compatibility, lifecycle, performance, accessibility, delivery, or operational invariant that must be enforced centrally. Those foundations still require representative evidence and the narrowest public contract that fulfills the obligation.
15. The station-like and contrasting reference games jointly decide whether the genre boundary is credible wherever both can exercise it. Success in only the station-like game is insufficient evidence for promoting optional gameplay abstractions.

## What we deliberately will not do

- Put station jobs, departments, atmospherics, power networks, combat rules, or round flow into the engine core.
- Make transform, physics, grids, networking, or presentation mandatory for every entity or world.
- Call the platform complete while requiring each game to rebuild basic rendering, input, UI, audio, networking, packaging, or diagnostics.
- Promote a game abstraction into the platform merely because Space Station 14 uses it heavily.
- Give a platform-maintained station kit powers or compatibility treatment unavailable to an independently maintained package in the same trust class.
- Hide genre assumptions in examples, default content, identifiers, or supposedly neutral APIs.

## Consequences

### Benefits

- Independent games receive a usable platform rather than a collection of primitives.
- Station-like mechanics can be rich without defining the engine's ontology.
- The contrasting reference game becomes a continuous check against accidental genre coupling.
- Reusable optimizations and tools have a clear home.

### Costs and limitations

- The platform floor is wider and more expensive than a minimal ECS kernel.
- Maintainers must defend the boundary during every API and package review.
- Some useful features begin as game packages and move only after cross-game evidence exists.
- Optional capability composition and diagnostics require deliberate design.
- Platform-maintained genre packages carry their own compatibility and support burden without expanding the universal conformance floor.

## How we will prove the decision works

- The station-like reference game builds nested inventory, construction, collision, UI, multiplayer interaction, and round flow through public contracts without engine forks.
- The contrasting reference game ships through the same platform while omitting station, grid-construction, persistent-world, job, and inventory assumptions.
- A server-only world and a non-spatial entity run without client or transform capabilities.
- A headless non-spatial package audit finds no grid, physics, renderer, audio, or UI payload dependency, and its startup, memory, and step-time measurements satisfy its declared minimal-profile budgets.
- Public API and package audits find no privileged station-game types in the platform foundation.
- At least one neutral mechanic is reused by both reference games with different game-defined meaning.
- A proposed optional gameplay abstraction is evaluated through the cross-game invariant test and can remain an ordinary game package without loss of support, while a required security or client-foundation obligation is not blocked by circular demand for two prior game implementations.
- A platform-maintained station kit and an independently maintained equivalent both build, install, run, upgrade, and pass conformance through the same public package and SDK contracts without privileged engine access.
- Omitting the station kit removes its payloads, dependencies, initialization, and runtime cost from contrasting and minimal games, while public UGC cannot load its executable assemblies.

## Implementation notes

No platform-foundation capability in this decision is claimed complete. ADR 0014 and the capability register remain the authority for first-release support labels.

## Follow-up decisions

- Transform graph, grid topology, collision, physics, and spatial-query contracts.
- 2D render, camera, sprite, UI, audio, localization, and platform-thread contracts.
- Networking interest, visibility, prediction, correction, and secrecy.
- Optional gameplay-package governance and promotion criteria.
- Capability admission, cost accounting, and headless composition.

## References

- [ADR 0001](0001-build-a-complete-game-platform.md)
- [ADR 0003](0003-preserve-straightforward-game-authoring.md)
- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0009](0009-one-supported-creator-workflow.md)
- [ADR 0013](0013-use-entities-for-independent-world-participants.md)
- [ADR 0014](0014-define-first-release-boundary-and-delivery.md)
- [ADR 0018](../technical/0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [ADR 0034](0034-use-a-declared-ladder-for-advanced-game-extensions.md)
- [World-model questions 17-18](../../workshops/world-model-question-set.md#e-what-foundations-belong-to-robusta)
