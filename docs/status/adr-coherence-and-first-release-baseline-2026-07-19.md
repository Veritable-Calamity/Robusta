# ADR Coherence, Implementation Pain, and First-Release Baseline Audit

- **Audit status:** Complete for accepted ADRs 0000-0038
- **Resolution update:** ADRs 0026-0038 are accepted; product ADRs 0039-0041 and technical ADRs 0042-0043 are proposed for review, while implementation and the remaining spatial, persistence, client, networking, operations, UGC, and delivery mechanisms remain open
- **Date:** 2026-07-19
- **Decision authority:** Advisory; accepted ADRs remain authoritative
- **Implementation baseline:** Greenfield scaffold; no gameplay capability is demonstrated

## Executive verdict

The accepted ADR set establishes a defensible direction, but it is not yet an implementation-complete architecture or a qualified 1.0 plan.

The strongest decisions are server authority, exact release receipts, side-specific application packaging, explicit state ownership, immutable catalog generations, atomic entity lifecycle, separate simulation and presentation time, outcome-based evidence, and semantic rather than binary Robust Toolbox migration. These choices address real weaknesses visible in the predecessor prototype and migration hazards visible in Robust Toolbox and Space Station 14.

The audit found one direct technical contradiction, one unresolved product-topology collision, and several release-blocking gaps:

1. ADR 0017 nests catalog, host, session, and world scopes in an order that contradicts ADR 0012's independent session and world lifetimes.
2. One-click offline authority has no selected topology that also preserves the required client/server payload separation.
3. ADR 0020's deterministic parallel-first promise lacks the access, aliasing, event, side-effect, native-thread-affinity, and failure contract required to enforce it.
4. Maps, coordinates, relations, transfer, and the platform/game boundary lacked product answers even though a 2D station-like game and the 1.0 boundary require them.
5. Persistence, prototype adoption, writable-data rollback, map preview, public UGC execution, publisher trust lifecycle, transport security, server operations, and quantitative migration coverage remain incomplete.

ADRs 0026-0029 subsequently resolved the decision-level scope graph, offline-authority topology, supported-code boundary, and deterministic access/effect contract. Product ADRs 0030-0038 now resolve the spatial, platform-boundary, persistence, catalog-adoption, and map-authoring questions through Option A. ADR 0033 keeps platform-maintained station components on ordinary public package and declared-trust paths, while ADR 0038 supports authenticated server-hosted collaborative document editing without serializing arbitrary gameplay state. Proposed product ADRs 0039-0041 make inspection, isolated testing, and bounded replay reviewable, and proposed technical ADRs 0042-0043 make the event/commit and identity/compatibility gaps concrete. No proposal is accepted and all implementations remain not started.

The prudent path is therefore to keep ADR 0014's 1.0 promise intact, make the missing contracts visible as gates, and resist calling the scaffold or a walking skeleton “1.0.” If the intended first release is materially narrower than ADR 0014, ADR 0014 must be superseded explicitly rather than narrowed through a roadmap note.

## Scope and evidence method

This audit compared:

- every accepted product ADR 0000-0016 and 0026-0027;
- every accepted technical ADR 0017-0025 and 0028-0029;
- accepted product ADRs 0030-0038 as the disposition of world-model questions 13-23;
- proposed product ADRs 0039-0041 and proposed technical ADRs 0042-0043 as unaccepted dispositions of open findings;
- the platform constitution, world-model question set, development plan, capability registry, evidence ledger, migration census, reference-game charters, and metrics baseline;
- the local greenfield implementation;
- the Robusta predecessor snapshot at [`61c71c068202c61575e48d6587ba53f300bed69b`](https://github.com/Veritable-Calamity/Robusta/tree/61c71c068202c61575e48d6587ba53f300bed69b);
- Robust Toolbox at [`537c4cb02f9555fa18f489e7b05694d288887d0e`](https://github.com/space-wizards/RobustToolbox/tree/537c4cb02f9555fa18f489e7b05694d288887d0e);
- Space Station 14 at [`b587d28e41ec33ffda6c1cac32e138e136d232ef`](https://github.com/space-wizards/space-station-14/tree/b587d28e41ec33ffda6c1cac32e138e136d232ef); and
- primary documentation from established engines and platform-security specifications.

The upstream revisions are point-in-time comparison evidence, not dependencies or compatibility promises. The local repository does not retain the predecessor commit, so ADR 0025 work must archive a source-baseline receipt before its conformance results can be reproduced independently.

## Current implementation reality

### Greenfield Robusta

The current repository is intentionally a scaffold. The runtime client, runtime server, creator CLI, content compiler, package verifier, and migration tool report that they are not implemented. Architecture tests and the project-reference graph provide useful governance evidence, but no world, entity runtime, scheduler, renderer, network session, installer, supervisor, or migration capability exists yet.

The existing `Game SDK -> Shared -> Client/Server` project topology is consistent with ADR 0018. It is the only substantial technical shape that can be treated as an implementation constraint today.

### Robusta predecessor prototype

The predecessor usefully proves several ideas:

- side-specific SDK and game projects;
- deterministic generated registration metadata and boundary analyzers;
- fixed-rate server simulation separated from render frequency;
- bounded networking, handshake and schema checks, separate traffic lanes, prediction/interpolation, and fault tests; and
- explicit host/world ownership and deterministic shutdown as design goals.

It does not implement the newly accepted contracts. The last audited snapshot uses one mutable world, monotonic raw entity IDs, immediate structural mutation, mutable reference-type components, a serial update loop, registration-order tie breaking, and dropped excess catch-up work. Its runtime prototype and resource managers are mutable, and its packaging, launcher, content-addressed store, public UGC boundary, creator supervisor, and migration pipeline do not satisfy ADRs 0021, 0022, 0024, or 0025.

The appropriate reuse boundary is:

| Reuse class | Predecessor material |
|---|---|
| Carry forward substantially | Network bounds, handshake/fault fixtures, side-specific project organization, generator/analyzer patterns, deterministic registration tests |
| Carry forward as design fixtures | Fixed-step accumulator, archetype queries, host/world ownership, package identities, build projections, validator diagnostics |
| Rewrite around accepted ADRs | Scope composition, entity identity/lifecycle, scheduler, catalog compiler, install/launcher/store, creator supervisor, migration tool |
| Do not transplant | Raw entity IDs, partial entity visibility, immediate mutation semantics, registration-order scheduling, skipped simulation work, broad service location, mutable prototype authority, validator-as-sandbox assumptions |

### Robust Toolbox and Space Station 14

Robust Toolbox is strong evidence that a shared/client/server ECS, content prototypes, authoritative networking, prediction, grids, containers, UI, and operational tooling can support a very large live game. Space Station 14 also shows why these systems cannot be assessed in isolation: transforms affect physics and networking; [containers](https://docs.spacestation14.com/en/robust-toolbox/user-interface/containers.html) affect visibility and PVS; [grid splitting](https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html) affects tiles, physics, anchoring, entity parenting, and replication; and [prediction](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html) affects audiovisual side effects, spawns, deletion, and reconciliation.

At the pinned revision, the SS14 tree contains 42,390 files, including 7,747 C# files, 2,085 prototype files, 211 map files, 28,996 texture files, and 1,371 audio files. This scale is evidence for representative migration pressure, not a first-release parity target. ADR 0014 correctly excludes full Space Station 14 parity.

Robust Toolbox's current runtime also exposes the main migration discontinuities:

- systems are dependency-sorted but ordinarily tick sequentially in [`EntitySystemManager`](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntitySystemManager.cs);
- gameplay commonly relies on immediate shared mutation and nested event effects;
- [`EntityUid`](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntityUid.cs) is a compact local integer rather than a generation-safe public handle;
- creation and component assembly have observable intermediate lifecycle stages;
- [global IoC access](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/IoC/IoCManager.cs) and broad manager lifetimes are common;
- [prototype inheritance, composition, serialization hooks, and reload](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/Prototypes/PrototypeManager.cs) are runtime behaviors; and
- maps, grids, transforms, containers, physics, PVS, prediction, and UI are deeply coupled.

Robusta's selected generational handles, atomic birth, explicit scopes, canonical catalogs, and semantic migration are prudent improvements. They are behavioral migrations, not mechanical renames.

## Best-practice cross-check

| Practice | Evidence | ADR implication |
|---|---|---|
| Schedule parallel work only from enforceable data access | [Unity Entities scheduling](https://docs.unity.cn/Packages/com.unity.entities%401.3/manual/systems-scheduling-jobs.html), [Bevy ECS scheduling](https://bevy.org/learn/quick-start/getting-started/ecs/) | ADR 0020 needs inferred or declared read/write access, alias controls, and an exclusive default. A dependency graph alone is insufficient. |
| Defer structural changes while parallel readers run | [Unity job dependencies and structural changes](https://docs.unity.cn/Packages/com.unity.entities%401.0/manual/scheduling-jobs-dependencies.html), [Flecs systems and staging](https://www.flecs.dev/flecs/md_docs_2Systems.html) | ADRs 0019 and 0020 are directionally correct, but same-step visibility, command results, event categories, and merge conflicts must be defined. |
| Treat native engines and presentation APIs as thread-affine or exclusive unless proven otherwise | [Godot thread-safe APIs](https://docs.godotengine.org/en/4.6/tutorials/performance/thread_safe_apis.html), [SDL main-thread query](https://wiki.libsdl.org/SDL3/SDL_IsMainThread), [Box2D foundation](https://box2d.org/documentation/md_foundation.html) | Renderer, windowing, input, audio, UI, physics, and Box2D-style internal task systems need declared affinity and oversubscription policy. |
| Never infer deterministic order from a general task scheduler | [.NET `TaskScheduler`](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-threading-tasks-taskscheduler) | ADR 0020 correctly requires stable merge order and a serial oracle; worker completion order must remain unobservable. |
| Do not call a load context a sandbox | [.NET assembly unloading](https://learn.microsoft.com/en-us/dotnet/standard/assembly/load-unload), [.NET `AssemblyLoadContext`](https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/understanding-assemblyloadcontext) | ADRs 0007 and 0018 are correct. World-isolation and deterministic guarantees still need a supported-code conformance boundary. |
| Secure update requires more than payload hashes and one signature | [The Update Framework specification](https://theupdateframework.github.io/specification/draft/), [SLSA provenance tracks](https://slsa.dev/spec/v1.2/tracks) | ADR 0022 needs trust-root rotation, revocation, rollback/freeze protection, threshold policy, and provenance verification before 1.0. |

## Conflict and pain-point register

### Blockers before affected public contracts freeze

| ID | Affected ADRs | Finding | Required disposition |
|---|---|---|---|
| C-01 | 0012, 0017, 0023 | ADR 0017 declares `process -> catalog -> host -> session -> world`. ADR 0012 requires sessions and worlds to be host-owned with independent lifetimes, and many sessions may participate in one world. A catalog generation is immutable input, not a world owner. | **Decision resolved:** ADR 0028 amends ADR 0017 with sibling session/world scopes and explicit attachment ownership. Implementation evidence remains open. |
| C-02 | 0004, 0006, 0014, 0022, 0023 | Offline play must use the same local authority, while a client payload must contain no server-only material. The receipt can describe both sides, but launch/install behavior is not selected. | **Decision resolved:** ADR 0027 selects a separately launched loopback authority installed as an explicit side payload. Implementation evidence remains open. |
| C-03 | 0011, 0012, 0017, 0018, 0020 | Arbitrary trusted C# can mutate statics, retain aliases, start threads, call native code, or block. Scopes and analyzers cannot make hard multi-world isolation or deterministic recovery true for all executable code. | **Decision resolved:** ADR 0026 defines the supported conformance and fault-escalation boundary without mislabeling trusted code as sandboxed. Implementation evidence remains open. |
| C-04 | 0003, 0018, 0019, 0020 | Parallel-first scheduling lacks enforceable phase access, alias lifetime, side-effect, reducer, I/O, exception, and thread-affinity rules. The predecessor and Robust Toolbox authoring models both expose mutable references and serial effects. | **Decision resolved:** ADR 0029 makes unproven code exclusive and preserves the serial oracle. Implementation evidence remains open. |
| C-05 | 0011, 0013-0016, 0023-0025, 0030-0038 | **Decision resolved:** accepted Option A answers now define maps, coordinate spaces, typed relations, transfer, platform foundations, persistence, prototype adoption, and source-oriented collaborative map editing. | Derive the spatial, persistence, catalog-adoption, and creator-authority technical ADRs before SDK, wire, save, or collaborative-edit surfaces freeze. Implementation evidence remains open. |
| C-06 | 0001, 0003, 0014, 0026, 0033, 0034 | A supported 2D desktop client is required, but no renderer, window, input, audio, UI, physics, or platform-threading contract has been chosen. The options assessment establishes a backend-neutral boundary and bakeoff plan, not a selection. | Accept the public abstraction and affinity contracts, run the documented clean-machine bakeoffs, then select the smallest coherent stack before the native gameplay vertical slice. |
| C-07 | 0002, 0011, 0014, 0015, 0016, 0020, 0029, 0039-0041 | Inspection, isolated-test, replay, and stronger determinism promises were open product questions. ADRs 0039-0041 now recommend bounded Option A answers. | Explicitly accept, revise, or reject each proposal before its public SDK, production protocol, or durable replay format freezes. |

### High-priority completeness gaps

| ID | Affected ADRs | Finding | Required disposition |
|---|---|---|---|
| P-01 | 0003, 0015, 0019, 0020, 0029, 0042 | Event categories and same-step semantics require an explicit contract: synchronous requests, commands, gameplay events, notifications, reentrancy, structural visibility, operation results, conflicts, and post-commit continuation. Proposed ADR 0042 recommends typed kinds and transactional commit frontiers. | Review ADR 0042 before ordinary systems/events API freezes. No implementation proceeds from the proposal alone. |
| P-02 | 0016, 0020 | “Never skip steps” can accumulate unbounded stale work. Per-world fairness, backlog limits, input bounds, load shedding, disconnect, restart, and operator policy are not defined. | Simulation overload and admission/backpressure ADR before production hosting. |
| P-03 | 0012, 0021, 0024, 0037 | **Product decision resolved:** accepted ADR 0037 keeps existing birth state stable unless an explicit migration commits and amends ADR 0024 so rollback covers preparation rejection and known reversible commit failure while all targets and client publication remain fenced, never arbitrary postcommit rewind. | Define the catalog-adoption transaction and client-generation admission matrix in technical ADRs and executable scenarios. |
| P-04 | 0006, 0023 | Generated schema is only part of networking. Transport, handshake cryptography, secrecy/owner-only state, PVS, late-input windows, predicted side effects, spawn/deletion reconciliation, correction budgets, and overload policy are missing. | Network transport/session and prediction/interest ADRs before the M4 contract freezes. |
| P-05 | 0008, 0014, 0022, 0035, 0036 | **Product decision resolved:** accepted ADRs 0035-0036 require versioned declared checkpoints, scoped durable identities, explicit missing-reference behavior, forward migration, and backup preservation without reverse-migration promises. Exact executable rollback and writable-data rollback remain distinct operations. | Derive technical save, repository, identity, and migration ADRs; never imply reverse migration. |
| P-06 | 0004, 0007, 0008, 0022 | Content addressing proves identity, not publisher authority. Trust roots, key rotation, revocation, downgrade/freeze defense, offline policy, leases, and garbage-collection safety remain open. | Distribution trust-lifecycle ADR before publication is Supported. |
| P-07 | 0007, 0014, 0018 | Public UGC is required for 1.0, but its declarative operation model, validation, capabilities, deterministic semantics, memory/CPU budgets, and denial behavior are not bounded. | Define 1.0 UGC narrowly as validated data/assets plus a finite game-declared operation set; accept a resource-budget ADR. |
| P-08 | 0001, 0012, 0014 | Dedicated-server configuration and operation lack layering, secret injection, admin authentication, health/readiness, graceful drain, crash recovery, and backup coordination. | Server operations/configuration ADR before M5. |
| P-09 | 0018, 0021-0023, 0030-0038, 0043 | Generated code manifest, content catalog, network schema, release receipt, runtime, durable, document, session, and spatial identities need explicit linkage without substitution. Proposed ADR 0043 recommends nominal scoped identities, purpose-bound mappings, and operation-specific compatibility. | Review ADR 0043 before identity types or compatibility descriptors become public or durable. |
| P-10 | 0005, 0008, 0021 | Content identifier grammar, Unicode/case normalization, merge/inheritance/patch semantics, localization/resource identities, semantic digest envelope, and live-generation adoption remain follow-ups. | Settle before identifiers become public or durable. |
| P-11 | 0010, 0025 | Migration has no immutable predecessor receipt, named Robust Toolbox baseline, weighted coverage target, allowed `ManualPort`/`Unsupported` rate, or compatibility-package retirement policy. | Archive baselines and accept a quantitative migration release profile before M7 qualification. |
| P-12 | 0002, 0014 | Reference-game repositories and named independent maintainers are not yet provisioned; every performance baseline is `null`. Versioned workload inputs now make measurement taskable but do not provide evidence or pass/fail budgets. | Treat external use and representative budgets as release blockers, not documentation placeholders; assign owners and measure before setting numeric gates. |
| P-13 | 0003, 0018 | The post-Preview SDK compatibility, deprecation, support-window, and source/binary versioning policy is unspecified. | Accept before publishing a Stable SDK. |

## ADR-by-ADR assessment

| ADR | Assessment | Main action or pain point |
|---|---|---|
| 0000 | Defensible | Keep the constitution above mechanisms and retain evidence honesty. |
| 0001 | Defensible but broad | Complete client-stack and server-operations contracts. |
| 0002 | Strong | Provision real external repositories and replace null metrics with measured budgets. |
| 0003 | Strong product goal | Preserve simple authoring by inferring safe access and defaulting advanced/unknown work to exclusive execution. |
| 0004 | Strong | Resolve local-authority payload/install topology; keep delivery processes outside games. |
| 0005 | Strong | Freeze content identity and composition semantics before public/durable use. |
| 0006 | Strong | Prediction, interest, secrecy, and transport behavior are release work, not schema details. |
| 0007 | Strong and unusually honest | Add bounded UGC semantics and publisher trust/revocation policy. |
| 0008 | Strong | Separate immutable application rollback from writable-data migration and rollback. |
| 0009 | Strong | Reload behavior depends on catalog-adoption and persistence decisions. |
| 0010 | Correct strategy | Add a pinned predecessor baseline and quantitative release profile. |
| 0011 | Strong | State exactly which conforming code receives in-process isolation guarantees; settle maps/space. |
| 0012 | Strong | Correct ADR 0017's scope graph; define configuration and durable transactions. |
| 0013 | Strong | Settle spatial foundations, compact grids, containment, and extension boundary. |
| 0014 | Coherent aspiration, incomplete executable boundary | Keep 1.0 breadth or supersede it explicitly; define bounded workloads and support labels. |
| 0015 | Strong | Define event/commit visibility, cross-entity transactions, cleanup access, and relationship dispositions. |
| 0016 | Strong | Add overload, fairness, admission, and restart policy. |
| 0017 | Coherent as amended | ADR 0028 corrects the original session/world nesting and catalog ownership. Implementation must follow the amended graph. |
| 0018 | Strong | Add conformance boundary, ergonomic access inference, SDK version policy, and advanced-extension consequences. |
| 0019 | Strong | Define provisional entity results, internal cleanup context, conflicting commands, and same-step visibility. |
| 0020 | Highest technical risk | ADR 0029 supplies an enforceable access/effect model; serial equivalence and crossover still need representative evidence. |
| 0021 | Strong | Define exact composition semantics, identity grammar, and catalog adoption. |
| 0022 | Strong | Add trust lifecycle, publication protocol, side-specific offline payload behavior, and writable-data policy. |
| 0023 | Strong schema foundation | Complete transport/session security, interest, prediction, secrecy, and overload behavior. |
| 0024 | Strong workflow shape, amended | Apply ADR 0037's fenced reversible catalog-adoption meaning; do not imply whole-world rollback. |
| 0025 | Correct and prudent | Pin baselines, keep blocked cases visible, and quantify 1.0 migration acceptance. |
| 0026 | Defensible boundary | Keep supported in-process conformance distinct from security isolation and escalate integrity-unknown faults honestly. |
| 0027 | Coherent topology | Prove one-click supervision, loopback authentication, side-payload separation, cleanup, and resource budgets. |
| 0028 | Necessary amendment | Preserve sibling session/world lifetimes and make attachment teardown fully explicit. |
| 0029 | Necessary enforcement | Keep unknown work exclusive and prove buffered effects against the serial oracle. |
| 0030-0034 | Accepted spatial/foundation disposition | Derive frame, typed-relation, transfer, genre-neutral platform, and extension-ladder mechanisms; preserve bounded 1.0 proofs for static grids and same-world relocation. |
| 0035-0038 | Accepted persistence/tooling disposition | Derive checkpoint, durable-reference, catalog-adoption, collaborative document, and isolated-preview mechanisms without conflating source documents with gameplay snapshots. |
| 0039-0041 | Proposed inspection/testing/replay disposition | Review the bounded Option A promises; do not freeze public inspection, Test SDK, or replay artifacts before acceptance. |
| 0042-0043 | Proposed foundational technical disposition | Review typed message/commit semantics and the typed identity/compatibility spine before their public or durable contracts freeze. |

## First-release baseline

ADR 0014 defines “first release” as Robusta 1.0. The following is the minimum bounded acceptance profile that makes that promise testable. It does not add SS14 parity and does not silently remove public UGC or migration from 1.0.

### Reference-game workload

The station-like reference game must demonstrate, through public published artifacts:

- a launcher-supervised local authority and a separately launchable dedicated authority;
- one authoritative server and two clients;
- one world with at least two logical map spaces, one fixed tile map, and one static constructed grid;
- transforms, explicit lifecycle ownership, anchoring or attachment, a simple nested container, collision/spatial query, and defined deletion/ejection behavior;
- predicted local movement and one predicted interaction, remote interpolation, authoritative correction, interest entry/exit, owner-only state, entity-bound UI, reconnect, and complete resynchronization;
- spawn, capability change, containment transfer, and deletion through atomic structural boundaries;
- prototype inheritance/composition or patching, assets, localization, provenance, resolved-form inspection, and source diagnostics;
- whole-world pause, timers, overload reporting, deterministic named random streams, a serial scheduler oracle, and at least one measured parallel-safe engine workload;
- a versioned save envelope with one forward migration, backup, explicit missing-reference behavior, and documented executable/data rollback outcomes;
- installation, verification, update interruption recovery, exact release rollback, and side-leak inspection;
- structured server configuration, health, logs, graceful shutdown, crash diagnosis, and restart; and
- migration reporting against a pinned Robust Toolbox baseline for a deliberately bounded component/prototype/map/UI subset.

The contrasting external game must prove that the same SDK supports rapid world replacement, short-lived entities, spectator sessions, data-oriented arena state, and prediction without station, grid-construction, container-inventory, or persistent-world assumptions.

### Public UGC floor

For 1.0, public UGC should be bounded to validated data, assets, and a finite game-declared operation vocabulary. General loops, arbitrary reflection, filesystem/network/process access, native calls, and arbitrary executable downloads are outside this floor. CPU, memory, recursion/expansion, output, and per-step budgets must fail closed with creator-facing diagnostics.

### Determinism and performance evidence

Before release qualification:

- the same admitted-input trace must produce the same canonical authoritative trace through the serial oracle and every supported worker-count/interleaving configuration;
- structural, event, timer, replication, and outbox merges must be independent of worker completion order;
- the station-like fault fixture must run under a versioned latency/loss/reorder/duplication matrix and publish correction and disconnect results;
- p99 step time, backlog recovery, memory, startup, content-build, edit-to-visible, install-to-playable, and network budgets must have numeric values, environment metadata, and raw evidence; and
- parallel crossover must be measured. Correctness may not depend on a speedup, and workloads below crossover remain serial or exclusive.

Universal engine-wide numeric targets are not defensible before the versioned fixtures exist. The current `null` metrics are honest for a scaffold, but every applicable value must become numeric before its capability is labeled Supported.

### Preview is not 1.0

A published walking skeleton may be called Preview when it proves external package consumption, two-world isolation, atomic entity lifecycle, a canonical content path, one local-authority 2D interaction, structured diagnostics, and the serial scheduler semantics that parallel execution will preserve. It is not Robusta 1.0 until the complete ADR 0014 journey and the bounded profile above are qualified.

## Decision sequence

1. **Completed:** accept ADRs 0026-0029 for conformance/fault boundary, offline authority, corrected scope graph, and deterministic access/effects.
2. **Completed:** accept ADRs 0030-0034 for maps, coordinates, typed relations, transfer, platform foundations, and extension boundaries.
3. **Completed:** accept ADRs 0035-0038 for saves, durable references, prototype change, and source-oriented collaborative map editing with isolated preview.
4. **Review next in parallel lanes:** product ADRs 0039 → 0040 → 0041, and foundational technical ADRs 0042 → 0043. Replay-specific mechanisms wait for ADR 0041 even though the foundational lane is independent.
5. Derive and review the spatial, persistence, client-boundary, networking, overload, trust, UGC, and operations technical ADRs against the common evaluation workloads.
6. Implement one serial semantic path and deterministic buffers first; enable parallel batches over the same path only after validation and oracle evidence exist.
7. Deliver the external walking skeleton, then freeze networking interest/prediction and the 2D client foundation from real use.
8. Complete distribution trust, operations, persistence, public UGC, and migration release profiles before 1.0 qualification.

## Additional ADRs generated by this audit

- [ADR 0026: Define the supported in-process game-code conformance and fault-containment boundary](../decisions/product/0026-define-supported-game-code-conformance-and-fault-containment.md) — Accepted via Option A.
- [ADR 0027: Run offline play through a separately installed local authority](../decisions/product/0027-run-offline-play-through-a-separate-local-authority.md) — Accepted via Option A.
- [ADR 0028: Model sessions and worlds as sibling host scopes](../decisions/technical/0028-model-sessions-and-worlds-as-sibling-host-scopes.md) — Accepted via Option A; amends ADR 0017.
- [ADR 0029: Enforce phase-scoped data access and buffered deterministic effects](../decisions/technical/0029-enforce-phase-scoped-access-and-buffered-effects.md) — Accepted via Option A; supplies ADR 0020's access/effect contract.

The four decisions are accepted; implementation remains not started.

The queued product answers generated from the same audit are now an accepted dependency-ordered decision set:

- [ADRs 0030-0034](../workshops/2026-07-19-world-model-05-space-persistence-and-preview.md) cover runtime maps and coordinates, typed relations, world transfer, the platform/game boundary, and advanced extensions.
- ADRs 0035-0038 in that review set cover declared checkpoints, durable identities and missing references, catalog adoption, and source-oriented isolated map preview.

Each decision accepts Option A. ADR 0033 explicitly denies privileged platform-only station-package paths, ADR 0037 amends ADR 0024, and ADR 0038 adds authenticated server-hosted collaborative document editing while rejecting arbitrary gameplay-world serialization. No implementation is claimed.

The next generated review set remains proposed:

- [ADRs 0039-0041](../workshops/2026-07-19-world-model-06-inspection-testing-and-replay.md) recommend Option A for authorized committed-state inspection, supported-runtime isolated testing, and authoritative replay within a declared compatibility domain.
- [ADR 0042](../decisions/technical/0042-use-typed-message-kinds-and-transactional-structural-commits.md) recommends typed message kinds and transactional structural commit frontiers.
- [ADR 0043](../decisions/technical/0043-use-a-typed-identity-and-compatibility-spine.md) recommends nominal scoped identities, purpose-bound mappings, and operation-specific compatibility.

None is accepted or implementation authority.

## Remaining ADR queue

| Priority | Decision |
|---|---|
| P0 | Spatial technical set after ADRs 0030-0034: transform graph, compact grids and topology, spatial queries, physics ordering, interest, and transfer coordination |
| P0 | Persistence technical set after ADRs 0035-0038: save format and repository, durable identity tables, migration transactions, catalog adoption, and editor protocols |
| P0 | Review proposed ADR 0042 for event and structural-commit semantics |
| P0 | Review proposed ADR 0043 for identity and compatibility linkage |
| P0 | Minimal M2 client/authority session contract: loopback transport, protected rendezvous, authentication, receipt/schema compatibility, one generated input/state path, rejection, readiness, and cleanup |
| P0 | 2D client foundation and platform-thread affinity after the documented backend-neutral contract and bakeoffs |
| P0 | Review proposed product ADRs 0039-0041 and their explicit ADR 0014 first-release amendments |
| P1 | Network transport/session security, interest/secrecy, prediction side effects, and overload |
| P1 | Simulation overload, admission, fairness, and host recovery |
| P1 | Public UGC operation language, validation, and resource budgets |
| P1 | Publisher trust roots, rotation, revocation, downgrade/freeze defense, and offline behavior |
| P1 | Server configuration, administration, health, drain, backup, and crash recovery |
| P1 | Code-manifest, catalog, schema, and receipt identity linkage |
| P2 | SDK compatibility/deprecation policy and migration release profile |
| P2 | Derive replay implementation mechanisms only after ADR 0041 acceptance and persistence compatibility work |

## Conclusion

Robusta's ADRs describe a logical successor rather than a clone: they preserve the productive shared/client/server, entity/system, content, and authoritative-networking shape while correcting identity, lifecycle, packaging, trust, and evidence weaknesses. That direction is defensible.

It becomes prudent only if the project implements the resolved scope, conformance, deterministic-effect, spatial, persistence, catalog-adoption, and collaborative-authoring decisions and qualifies the bounded 1.0 workload before freezing public APIs. The largest engineering risk is ADR 0020, and the largest product risk is treating ADR 0014's broad promise as delivered while its technical contracts and evidence remain open.
