# Technical ADRs

Technical ADRs 0017-0025 and 0028-0029 are accepted. ADR 0028 amends ADR 0017's original linear scope order. Accepted product ADR 0037 amends ADR 0024's catalog-adoption rollback contract.

Technical ADRs will select mechanisms that fulfill the accepted product decisions. They should remain more specific and more replaceable than the product ADRs.

Every technical ADR must include:

- the product ADRs it serves;
- the user-visible behavior it must preserve;
- alternatives considered;
- measurable acceptance evidence;
- compatibility, migration, security, and operational consequences;
- an implementation status separate from decision status.

## Accepted technical decisions

| ADR | Decision | Product ADRs served |
|---|---|---|
| 0017 | Enforce explicit runtime ownership scopes | 0000, 0001, 0002, 0004, 0007, 0011, 0012, 0014, 0015, 0016 |
| 0018 | Publish a layered Game SDK with capability boundaries | 0001, 0002, 0003, 0006, 0007, 0009, 0010, 0013, 0014 |
| 0019 | Use generational entity handles and transactional structural commits | 0002, 0003, 0006, 0011, 0012, 0013, 0015, 0016 |
| 0020 | Run fixed-step worlds through a deterministic phase scheduler | 0002, 0003, 0006, 0011, 0012, 0015, 0016 |
| 0021 | Compile content into a canonical provenance catalog | 0002, 0003, 0005, 0008, 0010, 0012, 0014 |
| 0022 | Install exact receipts into immutable content-addressed layouts | 0000, 0001, 0002, 0004, 0007, 0008, 0014 |
| 0023 | Generate versioned authoritative replication schemas | 0002, 0003, 0006, 0008, 0012, 0015, 0016 |
| 0024 | Supervise the creator loop as an observable transaction | 0001, 0002, 0003, 0005, 0009, 0012, 0014 |
| 0025 | Migrate through a source-located intermediate model and conformance corpus | 0002, 0003, 0005, 0008, 0010 |
| 0028 | Model sessions and worlds as sibling host scopes | 0002, 0004, 0006, 0011, 0012, 0015, 0016 |
| 0029 | Enforce phase-scoped access and buffered deterministic effects | 0002, 0003, 0006, 0011, 0012, 0015, 0016 |

## Accepted product gates for the next technical groups

The [space, persistence, and preview review set](../../workshops/2026-07-19-world-model-05-space-persistence-and-preview.md) records ADRs 0030-0038 as accepted via Option A. Their acceptance opens the following technical ADR work; it does not accept a particular mechanism or claim implementation:

- [ADRs 0030-0034](../product/0030-define-runtime-maps-and-frame-qualified-coordinates.md) govern runtime maps, coordinate frames, transforms, typed relations, containment, transfer, grids, cells, topology, spatial queries, physics, platform foundations, and the remaining advanced-extension mechanisms. ADR 0034 Option A confirms ADR 0018's unimplemented advanced-extension direction;
- [ADRs 0035-0037](../product/0035-persist-declared-world-state-through-versioned-checkpoints.md) govern save envelopes, durable identities, missing-reference policy, restore and forward migration, and catalog-adoption mechanisms. ADR 0037 amends ADR 0024 so rollback covers preparation rejection and proven reversible commit failure while targets remain fenced, without promising arbitrary postcommit world rewind;
- [ADR 0033](../product/0033-provide-platform-mechanics-with-game-defined-semantics.md) allows optional batteries-included station conveniences only as ordinary, separately versioned packages that use the published SDK and declared trust mechanisms available to independent developers, never privileged internals;
- [ADR 0038](../product/0038-edit-map-sources-and-preview-in-isolated-worlds.md) governs map-source, edit-transaction, inspector, isolated-preview, and authority-hosted collaborative-mapping mechanisms. Authenticated creators may collaborate through live or in-world presentation, but canonical source-document transactions remain the authored truth and arbitrary gameplay-world state is not serialized back into map sources; and
- world-model questions 24-26 still gate complete runtime inspection, isolated-test ergonomics, replay formats, and any stronger numerical determinism promise.

The grouped links name the first ADR in each dependency-ordered area. New technical ADRs derived from them remain unaccepted and unimplemented until reviewed on their own merits.

## Technical decision groups

The current and product-gated groups are:

1. process and host model;
2. public Game SDK topology;
3. entity and component lifecycle;
4. event model and system scheduling;
5. content intermediate representation and compiler;
6. package manifest, lock, and installation layout;
7. network schema and replication model;
8. renderer, windowing, input, audio, UI, and physics library choices;
9. save/map identity and migration;
10. creator CLI and process supervision;
11. assisted migration pipeline and conformance.
