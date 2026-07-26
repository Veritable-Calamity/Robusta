# First-Release Technical Scope Matrix

- **Status:** Planning baseline derived from accepted ADRs
- **Date:** 2026-07-19
- **Last reconciled:** 2026-07-26
- **Authority:** Lower than the platform constitution and accepted ADRs
- **Implementation claim:** None

## Purpose

This matrix separates the technical behavior required to qualify Robusta 1.0 from contracts that must be designed now but whose complete capability evidence is deliberately deferred. When this matrix conflicts with an accepted ADR, the ADR governs.

## Resolved scope boundaries

| Area | First-release requirement | Explicitly deferred or separately gated | Governing decisions |
|---|---|---|---|
| M2 local play | One command launches a client and a separate loopback authority through the ordinary generated authoritative path: protected launcher rendezvous, authenticated receipt/schema negotiation, one bounded generated input and state exchange, invalid-input rejection, readiness, and owned cleanup. | Prediction, broad replication, interest, reconnect/resynchronization breadth, correction, and network-fault qualification remain M4. | 0006, 0009, 0014, 0023, 0027 |
| Entity participation | An entity may omit transform, prototype origin, synchronization, presentation, and save policy. | No capability becomes mandatory merely because the M2 journey uses a client and authority. | 0013, 0033 |
| ECS state, storage, queries, and commits | Generated stable component and world-resource semantics; private world-owned hybrid storage; generated phase-scoped queries with canonical logical iteration; and deterministic prepared structural plans with one atomic publication gate. | ADRs 0048-0051 are accepted via Option A but remain `Not started`. Production implementation still waits for the CP02 predecessor/evidence boundary and retained identity, activation, fault, compatibility, manifest, SDK, budget, and CP04 integration gates. | 0013, 0015, 0019, 0029, 0042-0051 |
| Maps and frames | Multiple runtime maps, duplicate definition instantiation, typed frame-qualified coordinates, explicit conversion failure, and stale-safe map/frame lifecycle. | Portals, arbitrary cross-map observation, and advanced moving-frame optimizations require later technical decisions and capability evidence. | 0030 |
| Relations | Separate spatial parent, containment, attachment, lifecycle ownership, and reference semantics; one simple nested-container and static attachment profile. | Dynamic topology split/merge and split-driven attachment reassignment are not 1.0 requirements. | 0015, 0031 |
| Compact grids | Purpose-built static grid/cell data, static construction/deconstruction, anchoring, inspection, persistence hooks, synchronization hooks, collision hooks, and representative scale evidence. | Dynamic grid split/merge is post-1.0 unless a later ADR explicitly expands the release boundary. | 0013, 0031, 0033 |
| Spatial queries and physics | Optional-to-consume basic spatial queries, collision, and authoritative 2D physics sufficient for both reference-game slices. | ADR 0041 accepts only canonical authoritative replay within an exact validated domain and fixed declared partition scheme. It does not promise universal bitwise or cross-platform, cross-backend, cross-partition-scheme, or cross-release numerical determinism. A broad native-adapter catalogue is not required. | 0014, 0026, 0029, 0033, 0034, 0041 |
| World changes | Same-world map relocation preserves world-local entity identity. Session-driven world replacement uses explicit detach and reattach with fresh target identities. | General durable cross-world object-graph transfer and distributed activation coordination are post-1.0. | 0028, 0032 |
| Persistence | Versioned declared checkpoints, typed durable references, forward migration, backup preservation, bounded corrupt-input handling, and unpublished restore. | Reverse migration, arbitrary process snapshots, and arbitrary cross-service atomic checkpoints are not promised. | 0008, 0035, 0036 |
| Catalog adoption | Future births may adopt a compatible generation; existing state changes only through a prepared reversible migration or restart. | Arbitrary postcommit gameplay rewind is prohibited. | 0024 as amended by 0037 |
| Map authoring | Canonical source-document editing, isolated preview, and authenticated collaborative creator sessions using revisioned document commands. | Arbitrary gameplay-world serialization and semantic branch merging are not required. | 0027, 0038 |
| Inspection, tests, replay | Accepted ADRs 0039-0041 require bounded authorized owner-scoped committed observations, a published Test SDK using ordinary runtime activation/input/cleanup semantics, and bounded authoritative replay with complete ordered inputs, declared random state, fresh runtime identities, covered canonical projections, and no real external sinks. | Implementation remains `Not started`. `OBS-INSPECTION`, `TEST-RUNTIME`, `REPLAY-AUTHORITATIVE`, their identity/schema/authorization/budget work, and reviewed inspection, test-execution or world-construction, replay-reexecution, and replay-owner profiles remain separate gates. Acceptance grants no authority and no universal determinism claim. | 0014 as amended by 0039-0041; 0042-0051 |

## Decision critical path

1. Implement the bounded first scopes accepted by technical ADRs 0044-0047 while reviewing and approving ADR 0046's CP02 cleanup/fault profile and ADR 0047's CP01 core/Preview compatibility profile.
2. Prepare bounded subordinate specifications, internal conformance fixtures, reference models, and workload characterization for accepted ADRs 0048 (`SIM-STATE`), 0049 (`SIM-STORAGE`), 0050 (`SIM-QUERY`), and 0051 (`SIM-COMMIT`). Their design gate is satisfied, but production CP03 implementation waits for the CP02 predecessor/evidence boundary and every retained gate.
3. Draft and review `OBS-INSPECTION`, `TEST-RUNTIME`, and `REPLAY-AUTHORITATIVE` under accepted ADRs 0039-0041, then review their inspection, test-execution or ordinary world-construction, replay-reexecution, and replay-owner profiles. No implementation starts from product acceptance or an unapproved subordinate profile alone.
4. Before M2, derive and accept the minimal client presentation/platform-thread boundary and the loopback authority/session contract covering protected rendezvous, authentication, receipt/schema compatibility, one generated input/state path, rejection, readiness, and cleanup. Run the documented client bakeoffs against that boundary without adopting a backend by familiarity.
5. In parallel for M3 and M5, derive map/frame and typed-relation mechanisms, plus checkpoint/durable-reference and compatibility mechanisms.
6. Derive compact-grid and spatial-query mechanisms for M3, then physics and network-interest contracts for their M3/M4 evidence slices.
7. Join spatial identities into map-source, checkpoint, networking, catalog-adoption, inspection, and replay schemas without making one subsystem's storage model the public contract.
8. Accept creator-document, creator-authority, and isolated-preview mechanisms after the map and catalog contracts are stable.

## Qualification blockers

- The reference-game repositories, accountable independent maintainers, and exact source receipts are not yet named.
- Numeric performance and resource budgets remain unset until the versioned workloads have baseline measurements on the supported machines.
- Acceptance of ADRs 0048-0051 is not CP03 implementation or evidence; no ECS capability, supported scale, query API, or structural-commit runtime may be claimed until its retained gates and executable proof pass.
- The accepted answers to world-model questions 24-26 still require separately accepted technical packages, reviewed and approved profiles, schemas, authorization boundaries, workload evidence, and package/format decisions before public inspection, Test SDK, or durable replay surfaces freeze.
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
