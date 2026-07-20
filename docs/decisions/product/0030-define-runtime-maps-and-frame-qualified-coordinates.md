# ADR 0030: Define runtime maps and frame-qualified coordinates

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0005, 0006, 0011-0016, 0019-0021, 0023-0025, 0029

## The question

What is a running map, and what must a developer know when a position belongs to a map, moving structure, parent object, or screen?

## The promise

Developers can tell which spatial frame gives a coordinate meaning. A world can instantiate several maps or several copies of one compiled map definition without positions, identities, physics, or network interest becoming accidentally interchangeable.

## Why this matters

ADR 0011 says a world may contain several maps, while ADR 0013 distinguishes structures and collection addresses from entities. Without an explicit spatial meaning, a vector such as `(10, 20)` can silently mix map, grid, parent-local, physics, and screen coordinates.

## How Robust Toolbox answers today

Robust Toolbox uses map and entity-relative coordinates, transform parents, and grid entities. This supports SS14's moving grids, but map, grid, transform, physics, anchoring, and networking semantics are closely coupled and are common migration pain points.

## How the Robusta prototype answers today

The predecessor has transform and grid foundations but no accepted multi-map, frame-qualified public contract matching the new world model.

## Options considered

### Option A: Runtime map roots with typed, frame-qualified coordinates

A runtime map is a world-local root spatial frame created from authored data or game rules. Coordinates name their frame, and conversion is explicit and may fail.

### Option B: One universal world plane

All maps and grids occupy one implicit coordinate plane. This simplifies arithmetic but makes disconnected spaces, duplicate templates, precision, interest, and transfer depend on arbitrary offsets.

### Option C: Treat every map, frame, and coordinate as an ordinary entity relationship

This maximizes uniformity but conflicts with ADR 0013's purpose-built-data boundary and makes dense spatial operations depend on general entity semantics.

## Decision

Robusta will use Option A.

The product contract is:

1. A **map source** is a readable authored input. A **map definition** is its immutable, package-qualified compiled catalog form and reusable template. A **runtime map** is one world-owned instantiation of a definition with a world-local identity and root spatial frame. Source identity, definition identity and fingerprint, save record identity, and runtime map identity are distinct.
2. One world may contain zero or more runtime maps. The same map definition may be instantiated more than once without sharing mutable spatial state.
3. A runtime map may contain zero or more static or moving spatial structures such as grids. The later grid decision selects their compact cell and topology model.
4. Authoritative spatial positions and directions carry a map or local spatial-frame reference. Grid-cell addresses, physics quantities, view coordinates, and screen coordinates are distinct typed domains with explicit adapters; they are not interchangeable parent frames.
5. Spatial-frame conversion requires an explicit relationship path and may return a clear failure when frames are disconnected, stale, from different worlds, or unavailable on that side. Adapters between other coordinate domains are likewise explicit and may fail.
6. There is no implicit universal plane connecting all maps and no magic far-away coordinate used as “nowhere.” An entity without spatial participation simply has no spatial capability.
7. Transform remains optional for an entity. Identity, lifecycle, timers, and non-spatial gameplay remain valid without a map position.
8. Changing an entity's root spatial frame inside one world is an explicit committed spatial operation. It does not create a second entity identity merely because the root frame changes; the accepted transfer contract will govern when that operation is admitted.
9. Map, structure, entity, network, catalog, and future durable identities remain distinct even when tools display them together.
10. Rendering may convert confirmed spatial state into view and screen coordinates, but screen coordinates never become authoritative world positions without an explicit admitted input conversion.
11. Runtime-map construction is prepared and validated while unpublished. Publication is one atomic world change; a failed construction exposes no partial map, structure, entity, attachment, or spatial index state.
12. A runtime map may end while its world continues. Ending removes the map and its frames from live resolution before cleanup, and every entity, structure, attachment, relation, timer, and pending operation owned by or located in that map follows an explicit end, detach, rehome, transfer, cancel, or block disposition under ADR 0015.
13. A stale runtime-map or frame reference fails clearly and can never resolve to a later map that reuses storage or a display name. Observers see one committed map publication or ending outcome, not a partially available frame graph.

## What we deliberately will not do

- Use raw vectors as interchangeable map, grid, local, physics, and screen positions.
- Make every map or ordinary cell a general-purpose entity by default.
- Represent absence from space with a sentinel map or coordinate.
- Infer cross-map distance, collision, visibility, or adjacency without a declared relationship.
- Let presentation transforms mutate authoritative spatial state.

## Consequences

### Benefits

- Multi-map worlds and duplicate map instances remain unambiguous.
- Moving structures and render transforms can be optimized behind typed contracts.
- Cross-world and stale-frame mistakes fail clearly.
- Non-spatial games and entities do not pay for mandatory transforms.

### Costs and limitations

- Coordinate conversions and frame identities appear in public APIs and diagnostics.
- Importers must classify legacy coordinate assumptions.
- Physics, networking interest, saves, and tools must preserve frame provenance.
- Grid topology and cross-map portal behavior remain separate decisions.

## How we will prove the decision works

- One world instantiates the same compiled map definition twice and equal local coordinates remain distinct.
- An entity moves between two maps through one observable commit while retaining its world-local entity identity.
- Failed map construction remains undiscoverable, and ending one of two maps applies every declared entity, structure, attachment, and pending-work disposition while the other map continues.
- Old map and frame references fail after removal and never alias a later runtime map.
- Typed APIs reject or fail conversion for unrelated maps, stale frames, and cross-world values.
- A non-spatial entity completes its full lifecycle without a transform or sentinel coordinate.
- Rendering at multiple camera scales and frame rates does not change authoritative positions.
- Inspection reports the complete map, structure, parent, local position, conversion path, and provenance where applicable.

## Implementation notes

No runtime map, frame-qualified coordinate, transform graph, grid, spatial query, physics, or rendering implementation is claimed.

## Follow-up decisions

- Transform graph and spatial-parent mechanics.
- Grid/cell topology, anchoring, splitting, and merging.
- Spatial queries, collision, physics authority, and numerical units.
- Interest, visibility, portals, and cross-map observation.
- Map and frame identities in saves and network schemas.
- Runtime-map construction, publication, ending, cleanup, and stale-reference mechanics.

## References

- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0013](0013-use-entities-for-independent-world-participants.md)
- [World-model questions 13-14](../../workshops/world-model-question-set.md#d-how-do-space-and-maps-work)
- [Robust Toolbox coordinate systems](https://docs.spacestation14.com/en/robust-toolbox/coordinate-systems.html)
- [Robust Toolbox grids](https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html)
