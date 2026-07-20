# First-Release Technical Scope Matrix

- **Status:** Planning baseline derived from accepted ADRs
- **Date:** 2026-07-19
- **Authority:** Lower than the platform constitution and accepted ADRs
- **Implementation claim:** None

## Purpose

This matrix separates the technical behavior required to qualify Robusta 1.0 from contracts that must be designed now but whose complete capability evidence is deliberately deferred. When this matrix conflicts with an accepted ADR, the ADR governs.

## Resolved scope boundaries

| Area | First-release requirement | Explicitly deferred or separately gated | Governing decisions |
|---|---|---|---|
| M2 local play | One command launches a client and a separate loopback authority through the ordinary generated authoritative path: protected launcher rendezvous, authenticated receipt/schema negotiation, one bounded generated input and state exchange, invalid-input rejection, readiness, and owned cleanup. | Prediction, broad replication, interest, reconnect/resynchronization breadth, correction, and network-fault qualification remain M4. | 0006, 0009, 0014, 0023, 0027 |
| Entity participation | An entity may omit transform, prototype origin, synchronization, presentation, and save policy. | No capability becomes mandatory merely because the M2 journey uses a client and authority. | 0013, 0033 |
| Maps and frames | Multiple runtime maps, duplicate definition instantiation, typed frame-qualified coordinates, explicit conversion failure, and stale-safe map/frame lifecycle. | Portals, arbitrary cross-map observation, and advanced moving-frame optimizations require later technical decisions and capability evidence. | 0030 |
| Relations | Separate spatial parent, containment, attachment, lifecycle ownership, and reference semantics; one simple nested-container and static attachment profile. | Dynamic topology split/merge and split-driven attachment reassignment are not 1.0 requirements. | 0015, 0031 |
| Compact grids | Purpose-built static grid/cell data, static construction/deconstruction, anchoring, inspection, persistence hooks, synchronization hooks, collision hooks, and representative scale evidence. | Dynamic grid split/merge is post-1.0 unless a later ADR explicitly expands the release boundary. | 0013, 0031, 0033 |
| Spatial queries and physics | Optional-to-consume basic spatial queries, collision, and authoritative 2D physics sufficient for both reference-game slices. | A stronger cross-platform numerical-determinism or replay promise remains gated by proposed ADR 0041. A broad native-adapter catalogue is not required. | 0014, 0026, 0029, 0033, 0034 |
| World changes | Same-world map relocation preserves world-local entity identity. Session-driven world replacement uses explicit detach and reattach with fresh target identities. | General durable cross-world object-graph transfer and distributed activation coordination are post-1.0. | 0028, 0032 |
| Persistence | Versioned declared checkpoints, typed durable references, forward migration, backup preservation, bounded corrupt-input handling, and unpublished restore. | Reverse migration, arbitrary process snapshots, and arbitrary cross-service atomic checkpoints are not promised. | 0008, 0035, 0036 |
| Catalog adoption | Future births may adopt a compatible generation; existing state changes only through a prepared reversible migration or restart. | Arbitrary postcommit gameplay rewind is prohibited. | 0024 as amended by 0037 |
| Map authoring | Canonical source-document editing, isolated preview, and authenticated collaborative creator sessions using revisioned document commands. | Arbitrary gameplay-world serialization and semantic branch merging are not required. | 0027, 0038 |
| Inspection, tests, replay | Existing ADRs require diagnostics and conformance evidence sufficient to design mechanisms. Proposed ADRs 0039-0041 now define a bounded Option A review position. | Public inspection, isolated-test, replay, and stronger determinism contracts remain gated until ADRs 0039-0041 are explicitly accepted. | World-model questions 24-26; proposed ADRs 0039-0041 |

## Decision critical path

1. Run two independent review lanes in parallel: product ADRs 0039 → 0040 → 0041, and foundational technical ADRs 0042 → 0043. Replay-specific mechanisms still wait for ADR 0041, and no implementation starts from a proposal.
2. Before M2, derive and accept the minimal client presentation/platform-thread boundary and the loopback authority/session contract covering protected rendezvous, authentication, receipt/schema compatibility, one generated input/state path, rejection, readiness, and cleanup. Run the documented client bakeoffs against that boundary without adopting a backend by familiarity.
3. In parallel for M3 and M5, derive map/frame and typed-relation mechanisms, plus checkpoint/durable-reference and compatibility mechanisms.
4. Derive compact-grid and spatial-query mechanisms for M3, then physics and network-interest contracts for their M3/M4 evidence slices.
5. Join spatial identities into map-source, checkpoint, networking, and catalog-adoption schemas without making one subsystem's storage model the public contract.
6. Accept creator-document, creator-authority, and isolated-preview mechanisms after the map and catalog contracts are stable.

## Qualification blockers

- The reference-game repositories, accountable independent maintainers, and exact source receipts are not yet named.
- Numeric performance and resource budgets remain unset until the versioned workloads have baseline measurements on the supported machines.
- Proposed answers to world-model questions 24-26 require explicit acceptance before their public SDK or durable formats freeze.
- Every queued or newly derived technical ADR remains unimplemented and must be accepted before opening production implementation work governed by it; accepted ADRs 0017-0029 are not reopened by this statement.

## References

- [Development plan](development-plan.md)
- [ADR coherence and first-release baseline audit](adr-coherence-and-first-release-baseline-2026-07-19.md)
- [Reference-game charters](reference-games.md)
- [World-model question set](../workshops/world-model-question-set.md)
- [Inspection, testing, and replay review set](../workshops/2026-07-19-world-model-06-inspection-testing-and-replay.md)
- [Technical ADR queue](../decisions/technical/README.md)
- [Technical evaluation workloads](../specifications/technical-evaluation-workloads.md)
- [2D client and platform options](../reference/2d-client-platform-options.md)
