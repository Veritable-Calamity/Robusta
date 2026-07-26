# ADR Development Program

- **Status:** Proposed planning and traceability artifact
- **Baseline date:** 2026-07-21; decision review updated 2026-07-26
- **Source inventory:** [`platform-development-roadmap.md`](platform-development-roadmap.md), 99 roadmap-local questions
- **Authority:** Lower than the platform constitution and accepted ADRs
- **Purpose:** Convert the raw question inventory into a dependency-ordered decision program that authorizes implementation checkpoint by checkpoint

## Result

The 99 questions are normalized into 70 work packages:

| Disposition | Count | Meaning |
|---|---:|---|
| New product ADR candidate | 7 | A user-, creator-, player-, or operator-visible promise must be accepted before dependent mechanisms freeze |
| New technical ADR candidate | 58 | A mechanism or invariant remains genuinely undecided and needs an accepted technical ADR before implementation freezes its contract |
| Specification first | 5 | An accepted ADR already chooses the architecture; write the exact schema or protocol as a reviewed specification and promote it to an ADR only if a material alternative appears |
| **Total work packages** | **70** | Every one of the 99 source IDs maps to exactly one package below |

This avoids 34 unnecessary ADR records while retaining every question. Consolidation does not mean that a package becomes one enormous implementation. An ADR chooses the invariant, alternatives, and compatibility boundary; schemas, field tables, algorithms, limits, fixtures, and protocol details live in subordinate versioned specifications and evidence.

Program IDs are stable planning identifiers, not reserved ADR numbers. When a draft is created, it receives the next repository ADR number and records its program ID and source queue IDs.

Drafting and review may proceed in dependency-safe batches. A batch is a scheduling convenience, not a combined decision: every ADR keeps its own options, explicit acceptance, implementation status, and predecessor gates. No proposal may borrow implementation authority from another proposal merely because both appear in the same batch.

## Consolidation rules

Questions belong in one ADR package only when all of these are true:

1. they protect one invariant or state-machine boundary;
2. their alternatives must be compared together;
3. they share a compatibility and failure boundary; and
4. one evidence packet can prove the decision without hiding an independent risk.

Questions remain separate when any of these apply:

- one is a product promise and the other is a mechanism;
- they have independently evolvable public, wire, durable, native, security, or operational contracts;
- combining them would collapse identity, authority, trust, or lifecycle boundaries; or
- one can be replaced without revisiting the other's consequences.

The following distinctions are therefore deliberate:

- authored map document is not a world checkpoint;
- live catalog adoption is not offline checkpoint migration;
- inspection, telemetry, testing, and replay are different capabilities;
- package description, publisher trust, and installation transaction are different contracts;
- configuration possession is not administrative authority;
- executable rollback is not writable-data reverse migration;
- public UGC, trusted managed extensions, and native adapters are different trust tiers;
- Multi-Z is not server meshing; and
- player recovery and operator recovery have related evidence but different promises.

## Decision and implementation guardrail

| Package state | Work allowed | Work not yet authorized |
|---|---|---|
| Queued | Read-only research, workload design, throwaway spikes, alternatives, and evidence collection | Freezing or merging a new public, durable, wire, native, trust, or operational contract |
| Draft | Reviewable prototypes, schema sketches, benchmark harnesses, and migration analysis | Treating the recommendation as accepted or making dependent implementation irreversible |
| Accepted ADR | Implementing the bounded slice named by the ADR and its checkpoint | Capabilities or guarantees explicitly deferred by the ADR |
| Reviewed specification | Implementing exact details under an already accepted ADR | Introducing a new architectural choice without promoting it to an ADR |
| Evidence ready | Checkpoint review and implementation-status update | Claiming checkpoint completion before the roadmap exit packet is accepted |

A checkpoint may begin private research before all its decisions are accepted. It may not freeze a governed contract or describe the capability as implemented until every required product ADR and technical ADR is accepted, every specification-first package is reviewed, and all predecessor checkpoints named by the roadmap are satisfied.

Existing ADRs remain the authority. In particular:

- `FND-*` packages refine ADRs 0017, 0028, and 0043; they do not reopen typed, scoped identities, purpose-bound mappings, the accepted compatibility outcomes and rules-engine model, or sibling world/session ownership.
- `SIM-*` packages refine ADRs 0019, 0020, 0029, and 0042; they do not reopen typed message kinds, deterministic phase execution, or transactional commit frontiers.
- `CONTENT-*`, `SDK-*`, and `DX-*` refine ADRs 0018, 0021, and 0024.
- spatial, persistence, and authoring packages implement accepted product ADRs 0030-0038.
- Accepted product ADRs 0039, 0040, and 0041 open `OBS-INSPECTION`, `TEST-RUNTIME`, and `REPLAY-AUTHORITATIVE` respectively. Their product outcomes are settled, but each technical ADR, subordinate profile, schema, authorization boundary, and evidence packet remains an independent CP12 gate. Persistence, catalog adoption, and map authoring continue under accepted ADRs 0035-0038.
- Multi-Z and server mesh mechanisms remain blocked on their new product ADRs; this program does not amend ADR 0014 by itself.

Repository ADR status applies to a complete ADR, never to a partial slice. Where a shared ADR has checkpoint-specific profiles, the ADR must choose the common invariant and the versioned extension mechanism before its first use. Each later profile is a subordinate specification requiring explicit review and approval; `Accepted` remains an ADR status. A profile that exceeds the common invariant requires a new amending ADR.

## Program ledger

### Product promises

| Program ID | Proposed decision boundary | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `PRD-PLAYER` | Define the install, update, connection, compatibility-failure, crash, and recovery experience as one player journey | `P-PLAYER-01` | CP10 contracts; evidence spans CP10-CP14 | ADRs 0004, 0006, 0008, 0014, 0027 |
| `PRD-ACCESS` | Define the accessibility conformance floor across client, input, UI, audio, localization, and creator tools | `P-ACCESS-01` | CP10; creator clauses inform CP07 | ADRs 0002, 0003, 0014 |
| `PRD-MULTIZ` | Define Multi-Z behavior, exclusions, support level, and whether any subset belongs to 1.0 | `P-MULTIZ-01`, `P-MULTIZ-02` | CP08 public spatial types; CP17/CP18 scope | ADRs 0014, 0030-0033 |
| `PRD-MESH` | Define server-meshing outcomes plus consistency, availability, handoff, and recovery exclusions | `P-MESH-01`, `P-MESH-02` | Any CP19 mechanism | ADRs 0011, 0012, 0014, 0028, 0032, 0035, 0043 |
| `PRD-STATION` | Decide representative station support versus a named parity amendment and set a quantitative migration pain budget | `P-MIGRATE-01`, `P-PARITY-01` | CP16 qualification and CP17 claims | ADRs 0010, 0014, 0025; pinned migration census |
| `PRD-OPS` | Define dedicated-server backup, restore, drain, recovery, and support outcomes | `P-OPS-01` | CP14 | ADRs 0014, 0022, 0026, 0035 |
| `PRD-EVOLUTION` | Define 1.0 support windows, deprecation expectations, and durable-format evolution promises | `P-EVOLVE-01` | Supported policy at CP17; operation at CP21 | ADRs 0008, 0014, 0018, 0043 |

`PRD-STATION` starts from ADR 0014's accepted representative-capability and no-full-parity boundary. Selecting named SS14 or RMC parity would be an explicit amendment to ADR 0014 and must reconcile first-release scope, schedule, budgets, and evidence; this program does not imply that outcome.

### Foundation, compatibility, and lifecycle

| Program ID | Decision boundary/current disposition | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `FND-IDENTITY` | [ADR 0044](../decisions/technical/0044-generate-bounded-identity-declarations.md) is accepted via Option A and selects the declaration schema plus per-kind allocation, incarnation, generation, exhaustion, collision, reuse, tombstone, codec, and redaction profiles implementing ADR 0043 | `F-ID-01`, `F-ID-02` | Implement before CP02 closes; profile extensions serve CP03 and later checkpoints | ADRs 0017, 0019, 0028, 0043 |
| `FND-MAPPING` | **Specification first under ADR 0043:** mapping-record schema, owner indexes, endpoint validation, tombstone windows, retention, and diagnostics for the accepted purpose-bound mapping model | `F-MAP-01` | Review before CP11/CP12 mapping tables | ADR 0043; `FND-IDENTITY` |
| `FND-COMPAT` | [ADR 0047](../decisions/technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md) is accepted via Option A and selects descriptor schemas, policy language, diagnostic taxonomy, and profile-extension rules implementing ADR 0043's accepted outcomes and operation-specific rules engine | `F-COMPAT-01`, `F-COMPAT-02`, `SDK-COMPAT-01` | Reviewed/approved core/Preview profile at CP01; launch at CP07; handshake at CP11; restore/adoption/map-preview, inspection, test-execution or ordinary world-construction, and replay-reexecution at CP12; install at CP13; extension at CP15; Supported at CP17; mesh at CP19 | ADRs 0018, 0022, 0043; `PRD-EVOLUTION` before Supported profile |
| `FND-ACTIVATION` | [ADR 0045](../decisions/technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md) is accepted via Option A and selects generated capability graphs, activation metadata, capture analysis, and runtime validation | `F-SCOPE-01`, `F-SCOPE-02` | CP02/CP06 | ADRs 0017, 0018, 0026, 0028, 0029; accepted ADR 0044 |
| `FND-FAULT` | [ADR 0046](../decisions/technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md) is accepted via Option A and selects the common cleanup/fault taxonomy, state transitions, escalation, and versioned profile-extension contract | `F-CLEAN-01`, `F-FAULT-01` | Reviewed/approved CP02 ownership; CP04 world/scheduler; CP12 replay-owner; CP07 creator-supervisor; CP10 client/device; CP14 operations; CP15 native-extension; CP19 mesh profiles | ADRs 0017, 0026, 0028, 0029, 0042 |
| `FND-BUDGET` | Declarative CPU, memory, queue, I/O, native, and output budgets with overload enforcement | `F-BUDGET-01` | CP04/CP15; profiles serve CP07/CP11/CP14 | ADRs 0020, 0026, 0029, 0042 |

### ECS, scheduling, and messages

| Program ID | Decision boundary/current disposition | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `SIM-STATE` | [ADR 0048](../decisions/technical/0048-generate-stable-component-and-world-resource-schemas.md) is accepted via Option A and selects generated typed declarations plus one normalized semantic manifest for components and world resources | `E-COMP-01`, `E-RESOURCE-01` | CP03/CP06 | ADRs 0013, 0015, 0018, 0019, 0029, 0043 |
| `SIM-STORAGE` | [ADR 0049](../decisions/technical/0049-keep-ecs-storage-private-behind-world-owned-envelopes.md) is accepted via Option A and selects world-owned hybrid private storage families behind one storage-agnostic envelope | `E-STORE-01` | CP03 | ADRs 0019, 0029; accepted ADR 0048 / `SIM-STATE` |
| `SIM-QUERY` | [ADR 0050](../decisions/technical/0050-generate-phase-scoped-queries-with-canonical-iteration.md) is accepted via Option A and selects generated phase-scoped queries with canonical logical iteration and scheduler-issued partitions | `E-QUERY-01` | CP03 | ADRs 0019, 0029; accepted ADRs 0048-0049 / `SIM-STATE`, `SIM-STORAGE` |
| `SIM-COMMIT` | [ADR 0051](../decisions/technical/0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) is accepted via Option A and selects deterministic prepared plans, a bounded apply journal, and one publication gate implementing ADR 0042 | `E-STRUCT-01` | CP03 lifecycle and CP04 frontier | ADRs 0019, 0042; accepted ADRs 0048-0050 / `SIM-STATE`, `SIM-STORAGE`, `SIM-QUERY` |
| `SIM-SYSTEM` | Generated system identity, activation, ordering, access partitions, inspection, and affinity lanes | `E-SYSTEM-01`, `E-ACCESS-01`, `E-AFFINITY-01` | CP04/CP06; client/native profiles at CP10 | ADRs 0018, 0020, 0029; `FND-ACTIVATION`, `SIM-STATE` |
| `SIM-MESSAGE` | Message manifests, cardinality, deterministic waves, fan-out, reducers, and delivery budgets | `E-EVENT-01`, `E-REDUCE-01` | CP04/CP06 | ADRs 0029, 0042; `SIM-SYSTEM`, `FND-BUDGET` |
| `SIM-WORLD-SERVICES` | Receipt-versioned deterministic time, timers, pause/catch-up/fairness, and random streams | `E-TIME-01`, `E-RANDOM-01` | CP04; replay domain at CP12 | ADRs 0016, 0020, 0029, 0043; `FND-BUDGET` |

### Content, SDK, and creator workflow

| Program ID | Disposition and decision boundary | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `CONTENT-SEMANTICS` | **Technical ADR:** canonical content identities, normalization, composition, patches, conflicts, and overrides | `C-ID-01`, `C-COMPOSE-01` | CP05 | ADRs 0005, 0021, 0043; `FND-IDENTITY` |
| `CONTENT-CATALOG` | **Technical ADR:** catalog encoding, semantic digest, provenance, and clean-equivalent incremental cache identity | `C-CANON-01`, `C-INCR-01` | CP05 | ADRs 0021, 0043; `CONTENT-SEMANTICS` |
| `CONTENT-ASSET` | **Technical ADR:** the stable asset identity/provenance envelope across import, side projection, runtime acquisition, streaming, and licensing; exact transforms, caches, and backend resource policies remain replaceable specifications | `C-ASSET-01` | ADR before CP05; reviewed/approved compile profile CP05, runtime profile CP10, packaging profile CP13 | ADRs 0005, 0021, 0022; `CONTENT-CATALOG`; `PRD-ACCESS` only for the CP10 client profile |
| `SDK-MANIFEST` | **Technical ADR:** one versioned declaration-manifest envelope across SDK generators | `SDK-GEN-01` | CP06; later fragments at CP11/CP12 | ADRs 0018, 0021, 0023, 0042, 0043; schema ADRs |
| `DX-WORKSPACE` | **Specification first under ADRs 0018/0024:** workspace, templates, samples, upgrade fixtures, and CLI grammar | `SDK-TEMPLATE-01`, `TOOL-CLI-01` | Review for CP06/CP07 | ADRs 0009, 0018, 0022, 0024; `SDK-MANIFEST` |
| `DX-SUPERVISOR` | **Specification first under ADRs 0024/0037:** readiness, process ownership, cancellation, cleanup, diagnostics, and change-impact outcomes | `TOOL-SUP-01`, `TOOL-EDIT-01` | Review for CP07; adoption extension at CP12 | ADRs 0024, 0037; `DX-WORKSPACE`, `FND-COMPAT` |

### Spatial, Multi-Z, client, networking, and mesh

| Program ID | Proposed decision boundary | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `SPATIAL-CORE` | Authoritative 2D units, numeric policy, stale-safe runtime maps/frames, and transform graph | `S-UNITS-01`, `S-MAP-01`, `S-XFORM-01` | CP08 | ADRs 0015, 0030, 0042, 0043; CP03 commits |
| `SPATIAL-RELATIONS` | Typed relation schemas and atomic spatial, containment, attachment, ownership, and reference mutation | `S-REL-01` | CP08 | ADRs 0031, 0042; `SPATIAL-CORE` |
| `SPATIAL-GRIDS` | Compact grid/cell/chunk representation, anchoring, topology mutation, and deferred split/merge boundary | `S-GRID-01`, `S-TOPO-01` | CP09 | ADRs 0031, 0033; spatial core and relation ADRs |
| `SPATIAL-QUERY` | Public query API, canonical ordering, partitions, allocation, and index-consistency contract | `S-QUERY-01` | CP09 | ADRs 0016, 0020, 0029, 0033; `SPATIAL-GRIDS` |
| `SPATIAL-PHYSICS` | Backend-private physics adapter, collider publication, contact ordering, determinism, native affinity, and fault contract | `S-PHYS-01` | CP09 | ADRs 0016, 0020, 0029, 0033; `SPATIAL-QUERY`, `SPATIAL-GRIDS` |
| `MULTIZ-TOPOLOGY` | Nominal layers, map/frame distinction, adjacency, portals, conversion paths, ownership, and ending | `S-MULTIZ-01`, `S-PORTAL-01` | Constrain CP08; implement CP18 | `PRD-MULTIZ`, spatial core, relations, identity |
| `MULTIZ-INTERACTION` | Cross-layer traversal, queries, physics, perception, audio, interest, secrecy, save, network, and replay projection | `S-MULTIZ-02` | CP18 | `MULTIZ-TOPOLOGY`; CP09-CP12 contracts |
| `CLIENT-PLATFORM` | Desktop stack selection plus native thread, lifetime, startup, and device recovery | `CL-PLATFORM-01` | Start of CP10 | ADRs 0014, 0016, 0017, 0033; `SIM-SYSTEM`, `FND-FAULT`, `PRD-ACCESS` |
| `CLIENT-RENDER` | SDK-owned presentation scene, rendering, resources, shaders, camera, text, and device-loss boundary | `CL-RENDER-01` | CP10 | `CLIENT-PLATFORM`, spatial/content contracts |
| `CLIENT-INTERACTION` | Actions, devices, rebinding, focus, IME, UI semantics, localization, and accessibility projection | `CL-INPUT-01`, `CL-UI-01` | CP10; prediction seam at CP11 | `PRD-ACCESS`, `CLIENT-PLATFORM`, SDK/content contracts |
| `CLIENT-AUDIO` | Audio graph, streams, devices, spatialization, affinity, loss, and accessibility | `CL-AUDIO-01` | CP10 | `PRD-ACCESS`, `CLIENT-PLATFORM`, `SPATIAL-CORE` |
| `NET-CONNECTION` | Protected rendezvous, transport capabilities, crypto/authentication, compatibility, denial, and attachment admission | `N-TRANS-01`, `N-HANDSHAKE-01` | ADR before CP07; local-rendezvous implementation CP07; full evidence CP11 | ADRs 0006, 0023, 0028, 0043; `FND-COMPAT` |
| `NET-SCHEMA` | Canonical generated replication layouts, bounded codecs, evolution, and secrecy metadata | `N-SCHEMA-01` | CP11; CP06 may build replaceable generator infrastructure under ADR 0023 | ADRs 0023, 0043; `SDK-MANIFEST`, `SIM-STATE` |
| `NET-INTEREST` | Authorization-first composition of space, containment, ownership, observation, and game rules | `N-INTEREST-01` | CP11 | ADRs 0006, 0023, 0031; spatial query and relation contracts |
| `NET-REPLICATION` | Snapshot, baseline, delta, acknowledgement, ordering, loss, tombstone, and resynchronization wire lifecycle | `N-SNAPSHOT-01` | CP11 | `NET-CONNECTION`, `NET-SCHEMA`, `NET-INTEREST`, `FND-MAPPING` |
| `NET-FLOW` | Bandwidth, queue, rate, congestion, backpressure, and disconnect policy | `N-OVERLOAD-01` | CP11 | `NET-CONNECTION`, `NET-SCHEMA`, `FND-BUDGET` |
| `NET-RECONNECT` | Session survival, fresh attachment and network mappings, route re-entry, and authoritative resynchronization | `N-RECONNECT-01` | CP11 | `NET-CONNECTION`, `NET-REPLICATION`, `NET-FLOW`, `FND-MAPPING` |
| `NET-PREDICTION` | Bounded prediction, input windows, correction, side-effect reconciliation, and presentation response | `N-PREDICT-01` | CP11 | ADRs 0006, 0016, 0023, 0042; client interaction/render; replication/flow |
| `MESH-AUTHORITY` | Control topology, membership, compatibility, routes, trust, regional authority, epochs, leases, and fencing | `MESH-TOPO-01`, `MESH-AUTH-01` | CP19 | `PRD-MESH`, ADRs 0028/0043, compatibility/fault/network/ops contracts |
| `MESH-PARTITIONING` | Region identity, partitioning, placement, sizing, and rebalance | `MESH-PART-01` | Model CP19; execute CP20 | `MESH-AUTHORITY`, spatial core, mesh workloads |
| `MESH-HANDOFF` | Fenced authority reconstruction, client route transition, reconnect, and presentation continuity | `MESH-HANDOFF-01`, `MESH-CLIENT-01` | CP20 | ADR 0032; mesh authority/partitioning; network reconnect; persistence |
| `MESH-INTERACTION` | Cross-region ordering, latency, idempotency, backpressure, and failure semantics | `MESH-XREGION-01` | CP20 | `PRD-MESH`, mesh authority/partition/handoff, scheduler/messages |
| `MESH-OPERATIONS` | Crash, partition, control/storage loss, slow-node behavior, drain, rollout, rollback, backup, and administration | `MESH-FAIL-01`, `MESH-OPS-01` | Failure model CP19; proof CP20 | `PRD-MESH`, `FND-FAULT`, CP14 operations, prior mesh ADRs |

### Persistence, authoring, delivery, operations, UGC, and migration

| Program ID | Disposition and decision boundary | Source IDs | Required before | Principal predecessors |
|---|---|---|---|---|
| `DATA-CHECKPOINT` | **Technical ADR:** canonical checkpoint envelope, repository transaction, chunking, integrity, limits, selection, and unpublished restore publication | `PERSIST-ENV-01` | CP12 | ADRs 0022, 0035; `DATA-IDENTITY`, `FND-COMPAT` |
| `DATA-MIGRATION` | **Technical ADR:** forward migration, backup/copy, retry, failure classification, publication, and operator workflow | `PERSIST-MIG-01` | CP12 | ADRs 0008, 0022, 0035; `DATA-CHECKPOINT`, `DATA-IDENTITY`, `FND-COMPAT` |
| `DATA-IDENTITY` | **Technical ADR:** durable domains, checkpoint-local records, issuers, collisions, cycles, missing targets, and external references | `PERSIST-ID-01` | Before CP12 checkpoint schema freeze | ADRs 0036, 0043; `FND-IDENTITY` |
| `DATA-ADOPTION` | **Technical ADR:** prepared, fenced, reversible catalog adoption and client-generation admission | `CAT-ADOPT-01` | CP12 | ADRs 0024, 0037, 0042; `CONTENT-CATALOG`, `SIM-COMMIT` |
| `AUTHOR-MAP` | **Technical ADR:** map document identity, revision, command, conflict, undo, collaboration, and provenance protocol | `DOC-MAP-01` | CP12 | ADR 0038; content, spatial, identity, and authorization contracts |
| `OBS-INSPECTION` | **Technical ADR:** owner-scoped authorized committed observation, attachment binding, consistency, discovery, redaction, bounded query, retention, and protocol | `INSPECT-01` | Accept for CP12; reviewed/approved inspection compatibility profile before cross-version, public, or remote admission | ADRs 0039, 0042-0051; `FND-BUDGET`; schema, lifecycle, identity/mapping, and authorization contracts; `OPS-ADMIN` before production operator endpoints and audit |
| `TEST-RUNTIME` | **Technical ADR:** published Test SDK using ordinary activation, production-semantic fixtures, manual time, generated owner-scoped adapters, bounded observations, and fault injection | `TEST-01` | Accept for CP12; reviewed/approved test-execution or ordinary world-construction compatibility profile before publication | ADRs 0039, 0040, 0042-0051; reviewed CP02 ownership and CP04 world-fault profiles; `OBS-INSPECTION`; exact artifacts |
| `REPLAY-AUTHORITATIVE` | **Technical ADR:** replay artifact and local identities, authoritative input ledger, replay-reexecution compatibility profile, suppressed-effect comparison, headless driver, and verifier | `REPLAY-01` | Accept for CP12; reviewed/approved replay-reexecution compatibility and replay-owner fault profiles before replay-world publication | ADRs 0039-0051; checkpoint identity/reconstruction, exact receipt, time/random, structural commit, purpose-bound mapping, inspection, and Test SDK contracts |
| `PACKAGE-SCHEMA` | **Technical ADR:** authored package manifest, exact release receipt, side/capability/license/provenance fields, canonical conversion, and versioned profile extension | `PKG-MANIFEST-01`, `PKG-RECEIPT-01` | ADR and reviewed/approved receipt-reference profile before CP12; full implementation/evidence CP13; CP01 prototypes remain replaceable under ADR 0022 | ADR 0022; identity and all receipt-contributing schemas |
| `PACKAGE-TRUST` | **Technical ADR:** trust roots, signatures, agility, rotation, revocation, downgrade, offline, and consent policy | `PKG-TRUST-01` | CP13 | ADRs 0007, 0022; `PACKAGE-SCHEMA`, `PRD-PLAYER` |
| `PACKAGE-INSTALL` | **Specification first under ADR 0022:** immutable layout, leases, staging, activation, interruption, retention, GC, and writable-data paths | `PKG-INSTALL-01` | Review for CP13 | ADR 0022; package schema/trust; compatibility |
| `OPS-CONFIG` | **Technical ADR:** configuration layering, schema validation, secret references, reload, and provenance | `OPS-CONFIG-01` | CP14 | Package schema; generated config schemas; secret-provider boundary |
| `OPS-ADMIN` | **Technical ADR:** control-plane authentication, authorization, commands, gameplay separation, and audit | `OPS-ADMIN-01` | CP14 | ADR 0026; identity/authentication/inspection contracts |
| `OPS-OBSERVABILITY` | **Technical ADR:** logs, metrics, traces, health, cardinality, privacy, redaction, and retention | `OPS-OBS-01` | CP14 | `FND-FAULT`, typed identities, privacy policy; distinct from inspection |
| `OPS-RECOVERY` | **Technical ADR:** drain, crash, restart, backup, restore, update, rollback, and restart-loop orchestration | `OPS-RECOVER-01` | CP14 | `PRD-OPS`, persistence, installer, ownership cleanup, fault policy |
| `UGC-OPERATIONS` | **Technical ADR:** finite public operation language with deterministic validation and fail-closed execution budgets | `UGC-LANG-01`, `UGC-BUDGET-01` | CP15 | ADRs 0007, 0033; content/capability/budget contracts |
| `EXT-MANAGED` | **Technical ADR:** trusted extension manifest, ABI, capabilities, affinity, fault, compatibility, and packaging | `EXT-ABI-01` | CP15 | ADRs 0018, 0026, 0029, 0034; package/compatibility contracts |
| `EXT-NATIVE` | **Technical ADR:** native loading, supply chain, isolation, lifetime, crash, exact-build, and support policy | `EXT-NATIVE-01` | Conditional research CP09/CP10; qualification CP15 | Managed extension, client/physics bakeoffs, package trust, fault taxonomy |
| `MIG-QUALIFICATION` | **Technical ADR:** pinned RT/SS14/RMC-relevant baselines, subsets, coverage, warning, manual, unsupported, and time gates | `MIG-BASE-01`, `MIG-GATE-01` | CP16/CP17 | `PRD-STATION`, migration census and workloads |
| `MIG-IR` | **Specification first under ADR 0025:** source-located items, rules, classifications, patches, diagnostics, and report schema | `MIG-IR-01` | Review for CP16 | ADR 0025; semantic/content front ends; source provenance |
| `MIG-COMPATIBILITY` | **Technical ADR:** temporary compatibility-package admission, dependency limits, support labeling, and retirement | `MIG-COMPAT-01` | Admission at CP16; support/retirement at CP17 | ADRs 0018, 0025, 0034; package/extension policy; `PRD-EVOLUTION` for the CP17 profile |

## Dependency waves

The waves control decision order, not team serialization. Research and evidence work may overlap when it does not freeze a later contract.

| Wave | Checkpoints | Decision outcome required | What it unlocks |
|---|---|---|---|
| 0 — Current foundation | CP01-CP02 | Implement the bounded first implementation scopes accepted by `FND-IDENTITY`, `FND-ACTIVATION`, `FND-FAULT`, and `FND-COMPAT`; review and approve the CP02 fault/cleanup and CP01 core/Preview compatibility profiles | Complete the ownership/ephemeral identity kernel and make external SDK/API compatibility explicit |
| 1 — Simulation kernel | CP03-CP04 | Implement the accepted `SIM-STATE`, `SIM-STORAGE`, `SIM-QUERY`, and `SIM-COMMIT` contracts only after the CP02 predecessor/evidence boundary; accept `FND-BUDGET`, `SIM-SYSTEM`, `SIM-MESSAGE`, and `SIM-WORLD-SERVICES` | One serial semantic oracle, deterministic parallel execution, and bounded committed simulation |
| 2 — Creator walking skeleton | CP05-CP07 | Accept content and SDK ADRs and `NET-CONNECTION`; review both `DX-*` specifications and implement only the ADR's bounded local-rendezvous slice at CP07 | Exact content, generated declarations, and the published W0 creator journey |
| 3 — Spatial and client floor | CP08-CP10 | Accept `PRD-MULTIZ` and `MULTIZ-TOPOLOGY` before spatial types freeze; accept spatial ADRs; accept `PRD-PLAYER`, `PRD-ACCESS`, and client ADRs | Typed 2D worlds and an accessible recoverable client without precluding layered space |
| 4 — Routine multiplayer | CP11 | Review `FND-MAPPING`; accept all `NET-*` ADRs; review and approve the handshake compatibility profile | Secure authoritative multiplayer, bounded replication, prediction, and fresh reconnect |
| 5 — State, authoring, and diagnosis | CP12 | Accept `DATA-*`, `AUTHOR-MAP`, `PACKAGE-SCHEMA`, `OBS-INSPECTION`, `TEST-RUNTIME`, and `REPLAY-AUTHORITATIVE`; review and approve the CP12 supervisor, receipt-reference, restore, adoption, map-preview, inspection, test-execution or ordinary world-construction, replay-reexecution, and replay-owner profiles | Persistence, adoption, authoring, inspection, supported testing, and bounded replay become governed implementation work |
| 6 — Delivery and operations | CP13-CP14 | Accept package schema/trust and operations product/technical ADRs; review installer specification | Exact installation plus a supported player and dedicated-server lifecycle |
| 7 — Extensibility | CP15 | Accept UGC and extension ADRs; review applicable budget and native profiles | Fail-closed public creation and explicitly trusted advanced integration |
| 8 — Migration and 1.0 | CP16-CP17 | Accept `PRD-STATION`, `PRD-EVOLUTION`, migration ADRs, and review `MIG-IR`; all 1.0 predecessors accepted and evidenced | Honest 1.0 qualification and a measurable assisted-migration claim |
| 9 — Multi-Z | CP18 | Implement the accepted `MULTIZ-TOPOLOGY`; accept and implement `MULTIZ-INTERACTION` under `PRD-MULTIZ` | Separately qualified layered 2D operation |
| 10 — Server mesh | CP19-CP20 | Accept `PRD-MESH`, then mesh authority/partitioning before handoff/interaction/operations | Separately qualified distributed authority without weakening single-host correctness |
| 11 — Evolution | CP21 | Operate `PRD-EVOLUTION`, compatibility profiles, fixtures, migrations, and recurring evidence | Sustained support across meaningful platform boundaries |

## Checkpoint decision sets

These are entry and contract-freeze gates. Roadmap deliverables and exit evidence still control checkpoint completion.

| Checkpoint | Required program decisions or reviewed specifications |
|---|---|
| CP00 | No new package; existing governance remains authoritative |
| CP01 | Accepted `FND-COMPAT` plus reviewed/approved core/Preview profile; package/receipt prototypes under ADR 0022 remain replaceable until `PACKAGE-SCHEMA` |
| CP02 | Accepted `FND-IDENTITY`, `FND-ACTIVATION`, and `FND-FAULT` plus reviewed/approved CP02 cleanup/fault profile |
| CP03 | Accepted `SIM-STATE`, `SIM-STORAGE`, `SIM-QUERY`, and `SIM-COMMIT` plus the CP02 predecessor/evidence boundary and each retained identity, activation, compatibility, fault, specification, and budget gate |
| CP04 | `FND-BUDGET`, `SIM-SYSTEM`, `SIM-MESSAGE`, `SIM-WORLD-SERVICES`, reviewed/approved world-fault profile under `FND-FAULT` |
| CP05 | `CONTENT-SEMANTICS`, `CONTENT-CATALOG`, compile-time slice of `CONTENT-ASSET` |
| CP06 | `SDK-MANIFEST`; reviewed `DX-WORKSPACE`; generator-facing portions of simulation/content schemas; network generator infrastructure is replaceable until `NET-SCHEMA` at CP11 |
| CP07 | Reviewed `DX-WORKSPACE` and `DX-SUPERVISOR`; accepted `NET-CONNECTION` with its bounded local-rendezvous implementation; reviewed/approved launch compatibility and creator-supervisor fault profiles |
| CP08 | Accepted `PRD-MULTIZ`, `SPATIAL-CORE`, `SPATIAL-RELATIONS`, and `MULTIZ-TOPOLOGY`; the Multi-Z topology is constrained but implemented at CP18 |
| CP09 | `SPATIAL-GRIDS`, `SPATIAL-QUERY`, `SPATIAL-PHYSICS`; `EXT-NATIVE` only if a selected backend crosses its governed boundary |
| CP10 | `PRD-PLAYER`, `PRD-ACCESS`, all `CLIENT-*` ADRs, runtime/client slice of `CONTENT-ASSET`, reviewed/approved client/device fault profiles |
| CP11 | Reviewed `FND-MAPPING`; all `NET-*` ADRs; reviewed/approved handshake profile under `FND-COMPAT` |
| CP12 | All `DATA-*`; `AUTHOR-MAP`; accepted `PACKAGE-SCHEMA`, `OBS-INSPECTION`, `TEST-RUNTIME`, and `REPLAY-AUTHORITATIVE`; reviewed/approved restore, catalog-adoption, map-preview, CP12 `DX-SUPERVISOR`, receipt-reference, inspection, test-execution or ordinary world-construction, replay-reexecution, and replay-owner profiles. The Test SDK additionally depends on the earlier CP02 ownership and CP04 world-fault profiles |
| CP13 | Full `PACKAGE-SCHEMA` implementation/evidence; `PACKAGE-TRUST`; reviewed `PACKAGE-INSTALL`; reviewed/approved install profile under `FND-COMPAT`; applicable `PRD-PLAYER` clauses |
| CP14 | `PRD-OPS`, all `OPS-*`, reviewed/approved operations profile under `FND-FAULT`, applicable `PRD-PLAYER` clauses |
| CP15 | `UGC-OPERATIONS`, `EXT-MANAGED`, `EXT-NATIVE`, applicable `FND-BUDGET` profiles, reviewed/approved extension-admission compatibility and native-extension fault profiles |
| CP16 | `PRD-STATION`, `MIG-QUALIFICATION`, reviewed `MIG-IR`, `MIG-COMPATIBILITY` |
| CP17 | `PRD-EVOLUTION`; reviewed/approved Supported compatibility profile; every package in the accepted 1.0 boundary implemented and evidenced |
| CP18 | Implement accepted `MULTIZ-TOPOLOGY`; accept and implement `MULTIZ-INTERACTION` under `PRD-MULTIZ` |
| CP19 | `PRD-MESH`, `MESH-AUTHORITY`, `MESH-PARTITIONING`, reviewed/approved mesh compatibility/fault profiles |
| CP20 | `MESH-HANDOFF`, `MESH-INTERACTION`, `MESH-OPERATIONS` |
| CP21 | No automatic new ADR; changes that exceed `PRD-EVOLUTION` or accepted compatibility policy enter this program before implementation |

## Foundation and first simulation batches

The current code is intentionally limited to internal runtime-only ownership identities and scope teardown. The rows below are dependency order, not a requirement to implement one package at a time. Rows 1-8 are accepted, independently statused decisions. Rows 5-8 satisfy the CP03 design gate but remain `Not started`; production implementation waits for the CP02 predecessor/evidence boundary and every retained specification and profile gate.

| Order | Package | Decision boundary | Next work and retained gates |
|---:|---|---|---|
| 1 | [`FND-IDENTITY` / accepted ADR 0044](../decisions/technical/0044-generate-bounded-identity-declarations.md) | Declaration schema, per-kind profiles, generation/exhaustion/collision rules, codec permissions, canonical diagnostics, and redaction | Generate internal host/world/session/attachment identities and bounded diagnostic projections; defer the canonical catalog identity and every production reversible codec to their owning decisions |
| 2 | [`FND-ACTIVATION` / accepted ADR 0045](../decisions/technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md) | Capability graph inputs, generated factories, lifetime-capture rules, analyzer diagnostics, and runtime fallback validation | Implement the internal graph, generator, analyzer, validator, and synthetic fixtures; generated factories replace current identity creation only after ADR 0044's CP02 slice, and fallible, budgeted, asynchronous, or escalation-bearing reversal waits until ADR 0046's CP02 profile is reviewed and approved |
| 3 | [`FND-FAULT` / accepted ADR 0046](../decisions/technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md) | Common state taxonomy, admission closure, reverse cleanup, concurrent close, fault aggregation, profile extension, cleanup budgets, leak evidence, and escalation | Implement the common models, coordinator, ledger, validation, and synthetic fixtures; review and approve the CP02 profile before replacing the current ownership close path |
| 4 | [`FND-COMPAT` / accepted ADR 0047](../decisions/technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md) | Descriptor envelope, ADR 0043 result/reason encoding, policy language and identity, deterministic evaluation, profile extension, and Preview SDK rules | First review and approve the common schema/canonical-encoding specification covering domain separation, SHA-256 identity, normalization, issuer rules, bounds, and known-answer fixtures; then implement the common artifacts, compiler, evaluator, and synthetic profiles; review and approve the CP01 profile before repository or external-SDK restore/build compatibility behavior |
| 5 | [`SIM-STATE` / accepted ADR 0048](../decisions/technical/0048-generate-stable-component-and-world-resource-schemas.md) | Component/resource distinction, schema identity/version, side and lifetime qualifications, generated metadata, compatibility, and projection eligibility | Write and review the manifest, identity, encoding, diagnostic, and known-answer specifications; use only bounded engine-owned conformance fixtures until the public package and operation profiles exist |
| 6 | [`SIM-STORAGE` / accepted ADR 0049](../decisions/technical/0049-keep-ecs-storage-private-behind-world-owned-envelopes.md) | Storage envelope, stale-safe reuse, allocation/fragmentation behavior, and storage-agnostic boundaries | Build the private reference model and family-equivalence, stale-handle, churn, allocation-failure, and cleanup fixtures without freezing a public layout |
| 7 | [`SIM-QUERY` / accepted ADR 0050](../decisions/technical/0050-generate-phase-scoped-queries-with-canonical-iteration.md) | Canonical order, borrows, observations, changes, partitions, allocation, and invalidation independent of storage layout | Specify generated identities and views and prove non-escape, canonical recomposition, changes, observations, and invalidation; public system parameters still wait for `SIM-SYSTEM` and `SDK-MANIFEST` |
| 8 | [`SIM-COMMIT` / accepted ADR 0051](../decisions/technical/0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) | Planner phases, resource keys, conflict policies, apply journal, publication, result retention, and commit record | Specify planners, result schemas, retention, and adapters and prove failure/reversal/agreement behavior; later domains join only under their own checkpoint decisions |

The `FND-COMPAT` common evaluator may be implemented in parallel with the CP02 foundation slices, but its CP01 profile still gates repository and external-SDK behavior and it does not broaden the current runtime kernel. The `FND-MAPPING` specification waits until concrete network and persistence endpoints exist. The four CP03 simulation ADRs were accepted independently via Option A on 2026-07-26 in order 0048 → 0049 → 0050 → 0051. Their design gate is satisfied, but CP03 production work still waits for the CP02 predecessor/evidence boundary and the retained gates; private measurement spikes may begin earlier without creating implementation evidence.

## Draft packet contract

Every ADR created from this program must include:

1. its program ID and every source queue ID it absorbs;
2. the accepted product and technical ADRs it serves or refines;
3. the checkpoint and exact implementation slice it authorizes;
4. alternatives, including the smallest viable option and the cost of deferral;
5. public, durable, wire, native, trust, security, operational, and migration consequences that apply;
6. subordinate specifications and evidence that must exist before implementation is complete;
7. measurable acceptance and fault-injection evidence; and
8. explicit non-goals so a broad platform aspiration does not leak into the current checkpoint.

## Change control and traceability

- The source inventory remains in the platform roadmap for historical context. This ledger is the normalized planning view.
- A source queue ID must occur in exactly one ledger row. Any split, merge, promotion, or demotion updates this document and the checkpoint set in the same change.
- [`AdrDevelopmentProgramTests.cs`](../../tests/Robusta.Architecture.Tests/AdrDevelopmentProgramTests.cs) enforces the source, package, uniqueness, and disposition counts.
- A specification-first package becomes an ADR only when review exposes a material architectural alternative, compatibility promise, or trust boundary not already decided by its parent ADR.
- A candidate may be split during drafting only when the split follows the separation rules above. The replacement rows inherit all source IDs and preserve a one-to-one source-to-package mapping.
- Accepting a product ADR unlocks technical decision work, not implementation by itself. Accepting a technical ADR authorizes only its stated bounded slice.
- Accepted ADR text wins over this program. A conflict updates the program; it never silently changes the accepted decision.

## Completion condition

This program succeeds when checkpoint work can answer, before implementation begins: which promises apply, which mechanisms are accepted, which exact specifications are reviewed, what predecessor evidence is required, and what bounded implementation slice is now authorized. It is not complete merely because every candidate has a filename.
