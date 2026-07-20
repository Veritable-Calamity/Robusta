# ADR 0041: Record versioned authoritative replays with declared determinism

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends if accepted:** ADR 0014 by making bounded authoritative diagnostic replay an explicit Robusta 1.0 diagnostics and qualification requirement
- **Related decisions:** 0002, 0004-0008, 0011-0016, 0020-0023, 0026, 0029, 0030-0037, 0039, 0040

## The question

What should Robusta record and reproduce when diagnosing an authoritative gameplay bug or validating a migration, and within which runtime, platform, numerical, physics, content, and extension boundaries may the result be called deterministic?

## The promise

Robusta can capture a bounded authoritative execution as exact initial state plus ordered external inputs and declared execution identities, then re-execute it without real external side effects. Within a validated compatibility domain, replay reaches the same canonical committed outcomes independently of rendering, worker count, and task completion order, or stops at the earliest detected divergence with useful provenance. The platform does not overclaim universal bitwise identity across platforms or versions.

## Why this matters

Fixed steps and deterministic merge order remove important sources of accidental variation, but they do not by themselves reproduce a production incident. Re-execution also needs the exact game and runtime, catalog and schema generations, initial authoritative state, external service results, player and operator inputs, random-stream state, catalog transitions, and the determinism class of native or advanced extensions.

Station-like games add physics, atmosphere, timers, procedural events, hidden information, many clients, and long rounds. Recording every memory byte every step is impractical, while ordinary logs cannot prove that re-running the same inputs reached the same state. A replay contract must also prevent diagnosis from sending purchases, webhooks, account updates, or other external effects a second time.

## How Robust Toolbox answers today

Robust Toolbox and Space Station 14 support client prediction by restoring earlier client state and re-simulating inputs, and SS14 servers can record full rounds into artifacts that the game and launcher can later load. This is strong evidence for playback, incident review, and state-history tooling. Robusta still needs to distinguish presentation playback from authoritative re-execution and to state exactly when equal results, cross-version compatibility, privacy, and external-effect suppression are promised.

## How the Robusta prototype answers today

The predecessor and current scaffold have no accepted replay artifact, authoritative input ledger, canonical committed-state comparison, compatibility-domain declaration, divergence report, replay-safe external-effect boundary, or retention policy. The accepted scheduler and effect ADRs define intended deterministic execution constraints but remain unimplemented.

## Options considered

### Option A: Versioned authoritative input replay inside a declared compatibility domain

Record or identify a complete committed starting state, exact execution and content identities, ordered external inputs, named random-stream state, relevant catalog transitions, and periodic canonical verification points. Re-execute through the ordinary authoritative scheduler with real external outputs replaced by a compare-only sink. Promise equivalent canonical authoritative results only inside a domain whose runtime, platform, numerical, physics, and extension constraints have executable evidence.

This supports diagnosis and migration corpora without making memory layout, presentation, or unsupported native behavior deterministic. It requires canonical state projections, complete input admission, bounded recording, exact artifact retention, and honest compatibility reports.

### Option B: Promise bit-for-bit replay across every supported platform and future release

This is easy to describe and attractive for lockstep networking. It would require every floating-point operation, physics backend, native dependency, collection order, compiler/runtime behavior, extension, and schema migration to remain bit-identical indefinitely. The claim is not defensible without much narrower platform and code constraints than the product currently accepts.

### Option C: Record state deltas or presentation events for playback without authoritative re-execution

This can efficiently show players or administrators what appeared to happen and resembles proven round-replay systems. It does not prove that the same inputs reproduce simulation, identify the first divergent committed state, or validate scheduler and migration behavior. It may be a later product built on related capture infrastructure, but it does not answer this decision alone.

## Current review position

Option A is recommended for review. No decision is accepted by this proposal.

Acceptance includes the stated amendment to ADR 0014's diagnostics and qualification floor. If authoritative diagnostic replay is not intended for 1.0, ADR 0014 must instead be revised explicitly rather than broadened through a replay implementation.

If accepted, the product contract will be:

1. An **authoritative replay artifact** is a versioned diagnostic record for re-executing a bounded sequence of committed world steps. It is distinct from a world checkpoint, map source, editor history, replication stream, client prediction buffer, crash dump, video, and presentation-oriented player replay.
2. A replay begins from either a validated world checkpoint or a canonical declared initial-world construction whose exact meaning is available. The starting envelope identifies the immutable runtime and game receipts, catalog and generated schema identities, world configuration, simulation rate and starting step, authoritative random-stream algorithms and state, and every compatibility dimension required before re-execution.
3. The artifact records every external fact admitted to authoritative simulation in its stable admitted order. This includes applicable player commands, session or authority changes, administrator gameplay operations, pause and resume, host or durable-service results, time facts explicitly admitted as inputs, and the world-local facts and outcomes produced by compatible catalog-adoption or migration transactions. A single-world artifact treats host-wide coordination as recorded external input; it does not claim to re-execute a multi-world transaction. Inputs are never inferred later from prose logs or wall-clock timestamps.
4. Deterministic authoritative work obtains no unrecorded wall clock, environment, filesystem, network, database, process, locale, entropy, task-completion, or native result. Such a fact becomes an ordered input, is supplied by a versioned deterministic adapter, or makes the affected execution profile ineligible for the supported replay claim.
5. A **replay compatibility domain** names the exact and ranged properties under which canonical outcome equivalence is promised: runtime and game receipts, schema and catalog identities, operating-system and architecture profile where relevant, numerical and physics profiles, authoritative extension identities and determinism classes, and migration or translation rules. Compatibility is decided before world publication. Same name, version string, or seed alone never implies compatibility.
6. The minimum first-release guarantee is reproducible canonical authoritative outcomes within an exact validated domain. Conforming execution cannot change because render rate, worker count, partition assignment, task delay, thread identity, or serial-versus-parallel scheduler mode changed. Cross-operating-system, cross-architecture, cross-physics-backend, and cross-release equivalence may be claimed only for domains with separate executable evidence; universal bitwise identity is not promised.
7. Replay equivalence concerns canonical committed authoritative state and declared committed effects, not process memory, storage layout, caches, logs, diagnostic timestamps, presentation state, compression bytes, or native-resource identities. Stable verification points identify the covered state projection and algorithm so an implementation change cannot silently redefine equality.
8. Re-execution uses the ordinary supported world lifecycle, scheduler, structural commits, timers, random streams, catalog rules, and fault policy. A replay-only direct mutation or alternate simulation engine is not the oracle. Serial execution uses the same semantics and remains the diagnostic oracle required by ADRs 0020 and 0029.
9. Authoritative external outputs are captured as committed effect observations with stable identities and declared result handling. During every verified authoritative replay, network, filesystem, database, process, economy, account, webhook, and similar real sinks are disabled; the verifier compares proposed outputs with recorded expectations and never repeats the original side effect merely to reproduce gameplay. Any deliberately effectful exercise is a separately named integration mode and cannot claim verified replay status.
10. Each verification point can report equal, divergent, incompatible, incomplete, corrupt, or resource-limit outcomes. Divergence reports the earliest checked committed boundary, affected canonical projection, exact identities, input position, and available inspection difference. Robusta does not continue a divergent replay and present later output as verified, nor claim that the first differing field is automatically the root cause.
11. Recording has explicit start, end, segmentation, retention, size, rate, and failure outcomes. If quota, overload, process loss, or missing input creates a gap, the artifact is visibly incomplete and cannot make a complete-replay claim. Recording may use checkpoints and bounded segments, but a partial segment never masquerades as a complete round.
12. Authority replay, client prediction diagnosis, and player-facing playback are different disclosure and compatibility products. An ordinary client receives only the replication and input history legitimately admitted to it. A server replay may contain hidden gameplay and personal data and is never distributed as a player replay without a separately declared redaction or projection.
13. Replay artifacts are untrusted structured input. Before constructing or publishing a replay world, the loader authenticates or identifies its source as required, verifies declared exact dependencies and integrity, enforces count, depth, allocation, decompression, duration, and work limits, and applies explicit privacy, access, encryption, retention, and deletion policy. A replay may name dependencies but cannot authorize, install, fetch, select trust for, or load executable game content. Exact dependencies are acquired and verified through the ordinary ADR 0004, 0007, and 0022 installation and consent path, then re-execution occurs in a dedicated game process. Launcher, package-manager, signing, and repository credentials stay outside that process. Credentials, tokens, private keys, and raw external-service secrets are never replay inputs.
14. A known committed trace ending before a fault can be replayed from its last valid starting boundary. State after an integrity-unknown fault is not reconstructed from arbitrary memory or treated as committed truth. Fault diagnosis uses the last trustworthy verification point, ordered inputs, structured fault record, and bounded observations.
15. A single-world replay segment may cross a compatible catalog adoption only after the original host transaction has produced a committed world-local outcome. It records the source and target generation, relevant host and client-admission facts as ordered inputs, and that world's prepared, committed, rejected, or reversed result. Re-executing the globally fenced multi-world transaction requires a separately accepted replay-bundle and coordinator contract and is outside the first-release floor. A runtime, component-layout, network-schema, persistence-schema, or other incompatible transition ends the segment unless an explicit replay migration translates and validates the artifact. Retaining old immutable artifacts enables old-domain replay but does not create a forward-compatibility promise.
16. Migration validation may run the same declared checkpoint and input corpus under a named target-domain migration and compare canonical product-level projections. A difference is accepted only through an explicit expected migration outcome. This does not imply that every old production replay automatically executes under every new release.
17. Nonconforming code or an advanced extension with unverified native, unsafe, external, numerical, or ordering behavior visibly narrows or removes the supported replay and determinism label for every influenced capability under ADRs 0026 and 0034. Recording its outputs may still support presentation playback or best-effort diagnosis, but cannot be represented as verified re-execution.
18. First release requires bounded single-world authority-side diagnostic capture, headless re-execution from an exact checkpoint or initial definition, complete ordered-input and random-state capture, canonical state and committed-effect verification, serial/parallel comparison, divergence reporting, resource limits, secret-aware access, and side-effect suppression. Multi-world transaction replay, indefinite archival, arbitrary mid-step rewind, live gameplay branching, universal cross-version playback, full client-prediction re-simulation, and polished player/spectator playback remain later capabilities.

## What we deliberately will not do

- Claim that fixed steps or a shared random seed alone make a run replayable.
- Promise universal bitwise determinism across operating systems, architectures, physics backends, native extensions, or future releases.
- Treat logs, replication packets, presentation events, or periodic screenshots as a complete authoritative input ledger.
- Reissue real external effects while replaying a diagnostic artifact.
- Continue after divergence and label later results verified.
- Publish authority-only secrets to an ordinary client or player replay.
- Treat an integrity-unknown partial world state as a committed replay starting point.
- Give nonconforming code the supported determinism label because its last recording happened to match once.

## Consequences

### Benefits

- Production bugs can be reproduced against exact artifacts and ordered facts instead of approximate logs.
- Serial and parallel scheduler behavior gains a durable end-to-end correctness oracle.
- Migration corpora can identify intentional and accidental authoritative changes at committed boundaries.
- Exact compatibility domains make strong local claims possible without an indefensible universal promise.
- Side-effect suppression makes diagnostic replay safe for worlds that interact with external services.

### Costs and limitations

- Every authoritative external fact, random stream, extension, numerical backend, and canonical state projection needs versioned identity and evidence.
- Recording complete inputs and useful verification points consumes storage, CPU, privacy review, and operational attention.
- Exact old-domain replay depends on retaining compatible immutable artifacts.
- Divergence localization may identify only the first changed projection, not the underlying defect.
- Player-facing playback, client prediction replay, and broad cross-version compatibility require separate product work.

## How we will prove the decision works

- A station-like scenario records two players moving between maps, nested containment changes, timers, physics interactions, random events, administrative pause, and an admitted external-service result. Headless replay from the exact starting checkpoint reaches the same canonical committed states and effect observations.
- The same artifact verifies under serial mode and several worker counts, partition assignments, task delays, and render rates within its declared domain. No comparison depends on thread identity, completion order, or presentation frames.
- Removing, duplicating, or reordering one admitted input, changing a random-stream state, or injecting unordered authoritative enumeration produces a divergence at the first affected verification boundary with input, projection, step, and provenance information.
- Replaying an effect that originally requested a durable-service write compares the same stable committed request and recorded response without contacting the real service or repeating the write.
- A compatible catalog adoption records both generations and reproduces one world's committed transition from recorded host/client coordination facts; it does not count as proof that the globally fenced multi-world transaction was re-executed. An incompatible layout or schema transition ends the segment with an explicit outcome rather than silently crossing receipts.
- A migration corpus restores one source checkpoint, applies one named forward migration, replays a bounded input corpus, and compares declared target projections while preserving the original checkpoint and replay artifact.
- Windows and Linux runs are each tested against their declared compatibility domains. A domain with demonstrated cross-platform equivalence verifies across them; a domain without that evidence reports incompatibility instead of a false deterministic guarantee.
- A conforming advanced extension proves its declared determinism class through the same replay corpus. An unknown native or unsafe fixture is rejected from the supported domain or produces a visible reduced-support report.
- Fault injection, process termination, quota exhaustion, and a missing external input produce an incomplete artifact ending at the last trustworthy committed boundary; none is accepted as a complete replay.
- Corrupt, truncated, oversized, decompression-bomb, extremely long, and dependency-mismatched artifacts fail within declared resource limits before world publication and cannot reach real external sinks. A malicious dependency reference cannot trigger package acquisition, consent, credential use, or assembly loading.
- Ordinary clients and unauthorized operators cannot obtain hidden authority replay material. Authorized retention, redaction, export, expiration, and deletion actions are audited, and secrets or credentials are absent from captured inputs.

## Implementation notes

No replay manifest, authoritative input ledger, random-state capture, compatibility-domain model, canonical committed-state projection, effect verifier, divergence report, safe replay host, or retention control exists. Implementation status remains `Not started`.

## Follow-up decisions

- Replay manifest, segment, input, verification-point, integrity, compression, and canonical encoding formats.
- Compatibility-domain vocabulary for runtime, operating system, architecture, numerics, physics, native code, and extensions.
- Canonical authoritative state and effect projections, fingerprint algorithms, inspection differences, and collision treatment.
- Input admission capture, random-stream algorithms and state, timer ordering, catalog-transition recording, and gap detection.
- Replay host isolation, external-effect compare sinks, network suppression, fault boundaries, and resource budgets.
- Privacy classification, access, redaction, encryption, retention, export, deletion, and incident-response policy.
- Replay migration, exact-artifact retention, forward-validation corpora, and compatibility reporting.
- Client prediction trace, presentation/player replay, spectator controls, and any live branching or time-travel product.

## References

- [ADR 0008](0008-explicit-versions-migrations-and-rollback.md)
- [ADR 0016](0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0020](../technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0029](../technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0034](0034-use-a-declared-ladder-for-advanced-game-extensions.md)
- [ADR 0035](0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [Proposed ADR 0039](0039-inspect-running-worlds-through-authorized-snapshots.md)
- [Proposed ADR 0040](0040-test-isolated-worlds-through-the-supported-runtime.md)
- [World-model question 26](../../workshops/world-model-question-set.md#26-how-should-replay-and-determinism-fit-the-world-model)
- [Space Station 14 server replay recording](https://docs.spacestation14.com/en/server-hosting/server-replay-recording.html)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
