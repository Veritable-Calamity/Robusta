# ADR 0003: Preserve Straightforward Game Authoring Behind a Supported SDK

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0000, ADR 0001, ADR 0002, ADR 0005, ADR 0006

## The question

What should a game developer normally write, and where is the line between the game and the platform?

## The promise

An ordinary game feature is implemented in the game workspace through components, systems, events, prototypes, interfaces, and assets. It requires no Robusta source modification, host edit, private engine reference, or manual registry work.

## Why this matters

One of Robust Toolbox's strongest qualities is its authoring model. Space Station 14 developers usually express state as components, behavior as systems, and reusable object recipes as prototypes, with shared, client, and server code separated by role.

The Robusta prototype preserves the outer shape through side-specific Game SDK projects and generated registration, but its public surface does not yet provide all the lifecycle, world, networking, UI, map, physics, and tooling behavior required by a substantial game.

The rewrite should preserve the productive mental model without allowing game code to become coupled to internal renderer, transport, host, filesystem, or storage types.

## Options considered

### Expose engine implementation projects directly

This provides rapid early access but turns implementation details into accidental compatibility commitments and makes side separation difficult to enforce.

### Create a public copy of every internal service

This creates a formal boundary but risks a huge mirrored abstraction layer that is difficult to maintain and still exposes implementation structure.

### Publish a capability-oriented Game SDK

The SDK exposes concepts and operations useful to game authors, while internal modules remain free to evolve. Analyzers and generators enforce the boundary and remove routine boilerplate.

## Decision

Robusta will publish shared, client, and server Game SDK contracts plus build tooling, analyzers, generators, templates, and package integration.

Game developers normally create:

- components that record game state;
- systems and events that express game rules;
- prototypes and maps that compose objects and worlds;
- UI, artwork, audio, localization, and configuration;
- game-specific services such as rounds, quests, jobs, abilities, or game modes.

The platform supplies normal infrastructure such as host startup, registration, transform and map foundations, resource loading, ordinary serialization, routine replication, connection handling, diagnostics, and package assembly.

Game projects may use only the published Game SDK and explicit advanced extension points. A missing game-facing capability is product-design feedback, not automatic permission to reference an internal class.

Generated metadata should remove routine registration, serialization, networking, and editor boilerplate while remaining inspectable by developers.

## What we deliberately will not do

- Recreate a process-global service locator as the normal escape hatch.
- Require hand-maintained component, system, prototype, or message registries.
- Require game authors to write packet encoders for ordinary replicated state.
- Expose low-level renderer, transport, launcher, credential, or mutable registry objects through the normal SDK.
- Preserve every Robust Toolbox API name when a clearer capability is available.

## Consequences

### Benefits

- The common development model remains familiar and productive.
- Engine internals can evolve without breaking every game.
- Client/server mistakes can be diagnosed at build time.
- Game source becomes easier to test and migrate.

### Costs and limitations

- Designing a useful narrow SDK requires repeated use by external games.
- Adapters are needed between public capabilities and internal implementations.
- Advanced games may need explicitly governed extension points.

## How we will prove the decision works

An external game must be able to add an interactive networked object with:

- custom components and systems;
- prototype-authored configuration;
- lifecycle and gameplay events;
- localization and appearance;
- an entity-bound UI;
- authoritative state replication;
- no engine source changes or internal references.

Architecture tests must reject internal and cross-side references. Generated metadata must be inspectable and deterministic.
