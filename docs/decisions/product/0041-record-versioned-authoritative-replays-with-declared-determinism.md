# ADR 0041: Record versioned authoritative replays with declared determinism

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Last reconciled:** 2026-07-24
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends:** ADR 0014 by making bounded authoritative diagnostic replay an explicit Robusta 1.0 diagnostics and qualification requirement; ADRs 0042 and 0043 by removing only their replay-specific first-release deferrals, without reopening their typed-message, commit, identity, mapping, or compatibility-spine contracts
- **Related decisions:** 0002, 0004-0008, 0011-0016, 0020-0023, 0026, 0029, 0030-0040, 0042-0051

## The question

What should Robusta record and reproduce when diagnosing an authoritative gameplay bug or validating a migration, and within which runtime, platform, numerical, physics, content, and extension boundaries may the result be called deterministic?

## The promise

Robusta can capture a bounded authoritative execution as exact initial state plus ordered external inputs and declared execution descriptors, then re-execute it without real external side effects. Within a validated compatibility domain and one fixed declared partition scheme, replay reaches the same canonical committed outcomes independently of rendering, worker count, incidental worker scheduling, and task completion order, or stops at the earliest detected divergence with useful provenance. The platform does not overclaim universal bitwise identity across platforms, partition schemes, or versions.

## Why this matters

Fixed steps and deterministic merge order remove important sources of accidental variation, but they do not by themselves reproduce a production incident. Re-execution also needs the exact game and runtime, catalog and schema generations, initial authoritative state, external service results, player and operator inputs, random-stream state, catalog transitions, and the determinism class of native or advanced extensions.

Station-like games add physics, atmosphere, timers, procedural events, hidden information, many clients, and long rounds. Recording every memory byte every step is impractical, while ordinary logs cannot prove that re-running the same inputs reached the same state. A replay contract must also prevent diagnosis from sending purchases, webhooks, account updates, or other external effects a second time.

## How Robust Toolbox answers today

Robust Toolbox and Space Station 14 support client prediction by restoring earlier client state and re-simulating inputs, and SS14 servers can record full rounds into artifacts that the game and launcher can later load. This is strong evidence for playback, incident review, and state-history tooling. Robusta still needs to distinguish presentation playback from authoritative re-execution and to state exactly when equal results, cross-version compatibility, privacy, and external-effect suppression are promised.

## How the Robusta prototype answers today

The predecessor and current scaffold have no accepted replay artifact, authoritative input ledger, canonical committed-state comparison, compatibility-domain declaration, divergence report, replay-safe external-effect boundary, or retention policy. The accepted scheduler and effect ADRs define intended deterministic execution constraints but remain unimplemented.

## Options considered

### Option A: Versioned authoritative input replay inside a declared compatibility domain

Record or identify a complete committed starting state, exact execution and content identities, ordered external inputs, named random-stream state, relevant catalog transitions, and periodic canonical verification points. Re-execute through the ordinary authoritative scheduler with real external-effect capabilities absent and a post-commit compare-only sink. Promise equivalent canonical authoritative results only through a reviewed replay-reexecution compatibility profile whose runtime, platform, numerical, physics, partition, and extension constraints have executable evidence.

This supports diagnosis and migration corpora without making memory layout, presentation, or unsupported native behavior deterministic. It requires canonical state projections, complete input admission, bounded recording, exact artifact retention, and honest compatibility reports.

### Option B: Promise bit-for-bit replay across every supported platform and future release

This is easy to describe and attractive for lockstep networking. It would require every floating-point operation, physics backend, native dependency, collection order, compiler/runtime behavior, extension, and schema migration to remain bit-identical indefinitely. The claim is not defensible without much narrower platform and code constraints than the product currently accepts.

### Option C: Record state deltas or presentation events for playback without authoritative re-execution

This can efficiently show players or administrators what appeared to happen and resembles proven round-replay systems. It does not prove that the same inputs reproduce simulation, identify the first divergent committed state, or validate scheduler and migration behavior. It may be a later product built on related capture infrastructure, but it does not answer this decision alone.

## Decision

Robusta will use Option A.

This decision includes the stated amendment to ADR 0014's diagnostics and qualification floor and removes only the replay-specific first-release deferrals in accepted ADRs 0042 and 0043. Their typed message, transactional commit, nominal identity, purpose-bound mapping, and common compatibility contracts remain controlling constraints. Any later removal or narrowing of authoritative diagnostic replay requires an explicit superseding decision rather than an implementation-only change.

Accepted ADRs 0039 and 0040 remain independent product decisions. Replay reuses their authorized observation and supported headless-runtime contracts. If either is later superseded, its replacement must preserve the contract on which replay depends or explicitly amend this decision.

The product contract is:

1. An **authoritative replay artifact** is a versioned diagnostic record for re-executing a bounded sequence of committed world steps. It has a distinct canonical `ReplayArtifactId`, whose exact ADR 0044 declaration remains `REPLAY-AUTHORITATIVE` work, and is not substituted by its source checkpoint, runtime world, release receipt, file path, display name, or a bare untyped digest. It is distinct from a world checkpoint, map source, editor history, replication stream, client prediction buffer, crash dump, video, and presentation-oriented player replay.
2. A replay begins from either a validated world checkpoint or a canonical declared initial-world construction whose exact meaning is available. The starting envelope identifies the immutable runtime and game receipts, catalog and generated schema identities, world configuration, simulation rate and starting step, authoritative random-stream algorithms and state, and every compatibility dimension required before re-execution.
3. The artifact records every external fact admitted to authoritative simulation in its stable admitted order. Admission sequence, origin merge key, authority, schema identity, and terminal committed result are the facts assigned by the original operation authority under ADR 0042; an artifact producer or replayer cannot substitute caller-chosen ordering. Inputs include applicable player commands, session or authority changes, administrator gameplay operations, pause and resume, host or durable-service results, time facts explicitly admitted as inputs, and the world-local facts and outcomes produced by compatible catalog-adoption or migration transactions. A single-world artifact treats host-wide coordination as recorded external input; it does not claim to re-execute a multi-world transaction. Inputs are never inferred later from prose logs or wall-clock timestamps.
4. Deterministic authoritative work obtains no unrecorded wall clock, environment, filesystem, network, database, process, locale, entropy, task-completion, or native result. Such a fact becomes an ordered input, is supplied by a versioned deterministic adapter admitted by the replay operation, or makes the affected execution ineligible for the verified in-domain authoritative replay guarantee.
5. A **replay compatibility domain** is evaluated through a separately reviewed and approved FND-COMPAT replay-reexecution profile under ADRs 0043 and 0047. The containing replay operation authority selects the exact runtime and game receipts, schema and catalog descriptors, operating-system and architecture facts where relevant, numerical and physics facts, declared scheduler and partition facts, authoritative extension identities and determinism classes, policy definition, resolved profile, exact policy-state or graph snapshot, and admitted migration or adapter facts. Authored ranges, mutable channels, current directories, or ambient discovery resolve before evaluation and never appear as replay descriptor facts.
6. Before any replay world becomes visible, the operation authority must receive one valid complete compatibility report whose coverage and findings admit the ordinary `VerifiedAuthoritativeReexecution` mode using only `Exact` or `Compatible` findings. `ReadOnlyInspection` may admit bounded artifact metadata inspection, presentation playback, or a redirect to another inspection operation; it never admits verified authoritative re-execution. A denied valid report produces a replay-workflow `CompatibilityDenied` result before publication. A `CompatibilityEvaluationFailure` carries no report, mode, or publication authority and fails closed.
7. The minimum first-release guarantee is reproducible canonical authoritative outcomes within one exact validated domain and one fixed declared partition identity set, partition order, partitioning algorithm, and merge algorithm. Conforming execution cannot change because render rate, worker count, incidental worker-to-declared-partition assignment, task delay, thread identity, or serial-versus-parallel scheduler mode changed. A different partition identity set, order, or algorithm is a different compatibility domain and requires separate evidence. Cross-operating-system, cross-architecture, cross-physics-backend, cross-partition-scheme, and cross-release equivalence may be claimed only for domains with separate executable evidence; universal bitwise identity is not promised.
8. Replay equivalence concerns only the canonical committed authoritative state projections and committed external-effect-intent projections named by each verification point. It does not cover process memory, storage layout, caches, logs, diagnostic timestamps, presentation state, compression bytes, unprojected fields, or native-resource identities. Stable verification points identify every covered projection, projection schema, and comparison algorithm so an implementation change cannot silently redefine equality or imply guarantees about uncovered state.
9. Re-execution uses the ordinary world lifecycle, scheduler, structural commits, timers, random streams, catalog rules, and commit semantics inside a dedicated replay process whose host and world are constructed and atomically published through ordinary ADR 0045 generated activation plans and governed by a separately reviewed and approved ADR 0046 replay-owner and fault profile. The activation plan contains no real external-sink capability. A replay-only direct mutation or alternate simulation engine is not the oracle. Serial execution uses the same declared semantics and remains the diagnostic oracle required by ADRs 0020 and 0029. Divergence is a verifier result, not an owner fault; an integrity-affecting runtime failure closes or escalates the replay owner and cannot produce verification success.
10. Authoritative external outputs are captured and compared as committed external-effect intents with stable intent and idempotency identities. An external response that influenced simulation is a later ordered replay input, not an effect-sink return invented during re-execution. During `VerifiedAuthoritativeReexecution`, network, filesystem, database, process, economy, account, webhook, and similar real sink capabilities are absent. A compare-only consumer runs after commit and outside authoritative phases, has no real-sink authority, and never repeats the original side effect merely to reproduce gameplay. Any deliberately effectful exercise is a separately named integration mode and cannot claim verified replay status.
11. Replay verification reports `Equal`, `Divergent`, `Incomplete`, `Corrupt`, or `ResourceLimit` as mutually exclusive terminal results; compatibility denial and compatibility evaluation failure occur before replay-world publication and are not divergence results. Divergence reports the earliest checked committed boundary, affected covered projection, exact identities, input position, and available bounded difference. Robusta does not continue a divergent replay and present later output as verified, nor claim that the first differing field is automatically the root cause.
12. Recording has explicit start, end, segmentation, retention, size, rate, and failure outcomes. Starting or ending capture and retaining, redacting, exporting, expiring, or deleting an artifact are authenticated and authorized lifecycle operations with immutable terminal results and audit evidence; knowing any replay, checkpoint, world, session, account, or correlation identity grants no authority. If quota, overload, process loss, or missing input creates a gap, the artifact is visibly incomplete and cannot make a complete-replay claim. Recording may use checkpoints and bounded segments, but a partial segment never masquerades as a complete round.
13. Authority replay, client prediction diagnosis, and player-facing playback are different disclosure and compatibility products. An ordinary client receives only the replication and input history legitimately admitted to it. A server replay may contain hidden gameplay and personal data and is never distributed as a player replay without a separately declared redaction or projection.
14. Replay artifacts are untrusted structured input. Before constructing or publishing a replay world, the loader authenticates or identifies its source as required, verifies declared exact dependencies and integrity, enforces count, depth, allocation, decompression, duration, and work limits, and applies explicit privacy, access, encryption, retention, and deletion policy. A replay may name dependencies but cannot authorize, install, fetch, select trust for, or load executable game content. Exact dependencies are acquired and verified through the ordinary ADR 0004, 0007, and 0022 installation and consent path, then re-execution occurs inside the dedicated replay owner from clause 9. Launcher, package-manager, signing, and repository credentials stay outside that process. Credentials, tokens, private keys, and raw external-service secrets are never replay inputs.
15. Every durable artifact record has a bounded replay-local `ReplayRecordId` scoped to its `ReplayArtifactId` and distinct from all runtime identities. Every re-execution creates fresh host, world, entity, map, frame, attachment, operation, and other ephemeral runtime identities; it never recreates or aliases the original live values. Purpose-bound mappings may correlate one replay-local record identity to an original declared identity and separately to one re-execution identity only for the named comparison or diagnostic operation. Canonical comparisons pass through the replay-local records and covered projections, not raw runtime-ID equality. A mapping or identifier grants no read, mutation, loading, trust, or administration authority. Durable replay codecs and their ADR 0044 identity declarations remain a later reviewed REPLAY-AUTHORITATIVE specification rather than being activated by this product decision alone.
16. A known committed trace ending before a fault can be replayed from its last valid starting boundary. State after an integrity-unknown fault is not reconstructed from arbitrary memory or treated as committed truth. Fault diagnosis uses the last trustworthy verification point, ordered inputs, structured ADR 0046 fault record, and bounded authorized observations. Unknown integrity in the replay owner similarly prevents an `Equal` result and triggers the profile's required close or escalation.
17. A single-world replay segment may cross a compatible catalog adoption only after the original host transaction has produced a committed world-local outcome. It records the source and target generation, relevant host and client-admission facts as ordered inputs, and that world's prepared, committed, rejected, or reversed result. Re-executing the globally fenced multi-world transaction requires a separately accepted replay-bundle and coordinator contract and is outside the first-release floor. A runtime, component-layout, network-schema, persistence-schema, or other denied transition ends the segment unless an explicit replay migration translates and validates the artifact and a new exact compatibility evaluation admits re-execution. Retaining old immutable artifacts enables old-domain replay but does not create a forward-compatibility promise.
18. Migration validation may run the same declared checkpoint and input corpus under a named target-domain migration and compare canonical covered product-level projections. A difference is accepted only through an explicit expected migration outcome. This does not imply that every old production replay automatically executes under every new release.
19. Nonconforming code or an advanced extension with unverified native, unsafe, external, numerical, or ordering behavior visibly narrows or removes the verified in-domain authoritative replay guarantee for every influenced capability under ADRs 0026 and 0034. Recording its outputs may still support presentation playback or best-effort diagnosis, but cannot be represented as verified re-execution.
20. First release requires bounded single-world authority-side diagnostic capture, headless re-execution from an exact checkpoint or initial definition, complete ordered-input and random-state capture, reviewed replay-reexecution compatibility and replay-owner fault profiles, fresh runtime identities with purpose-bound comparison mappings, canonical covered-state and committed-effect-intent verification, serial/parallel comparison within one fixed partition scheme, divergence reporting, resource limits, secret-aware access, authenticated lifecycle operations, and real-sink capability exclusion. Multi-world transaction replay, indefinite archival, arbitrary mid-step rewind, live gameplay branching, universal cross-version playback, full client-prediction re-simulation, and polished player/spectator playback remain later capabilities.

## Authority and retained implementation gates

This decision amends the three decisions named in the metadata and authorizes the `REPLAY-AUTHORITATIVE` product direction. It does not by itself implement or approve a replay manifest or durable codec, activate an ADR 0044 replay identity profile, approve the `FND-COMPAT` replay-reexecution profile, approve the ADR 0046 replay-owner and fault profile, expose a public Test SDK or live-inspection capability, create a remote replay endpoint, select operator authentication or grants, establish artifact storage and cryptography, or define production audit and retention operations.

Implementation is additionally gated on the reviewed and accepted `OBS-INSPECTION` and `TEST-RUNTIME` technical contracts. Replay qualification cannot substitute private observation or headless-driver surfaces; executable replay evidence waits for the required predecessor implementation slices and evidence.

Those predecessor contracts and subordinate schemas, profiles, controls, workload budgets, and evidence remain reviewed implementation gates. A decoder that understands an artifact grants no compatibility, publication, disclosure, or execution authority.

## What we deliberately will not do

- Claim that fixed steps or a shared random seed alone make a run replayable.
- Promise universal bitwise determinism across operating systems, architectures, physics backends, native extensions, or future releases.
- Treat mutable channels, unresolved ranges, or a changed declared partition scheme as the same replay domain.
- Treat logs, replication packets, presentation events, or periodic screenshots as a complete authoritative input ledger.
- Reissue real external effects while replaying a diagnostic artifact.
- Continue after divergence and label later results verified.
- Treat `ReadOnlyInspection`, identifier possession, successful decoding, or a compatibility evaluation failure as authority to publish a verified replay world.
- Reuse original ephemeral runtime identities or compare re-executions through raw runtime-ID equality.
- Publish authority-only secrets to an ordinary client or player replay.
- Treat an integrity-unknown partial world state as a committed replay starting point.
- Give nonconforming code the verified in-domain guarantee because its last recording happened to match once.

## Consequences

### Benefits

- Production bugs can be reproduced against exact artifacts and ordered facts instead of approximate logs.
- Serial and parallel scheduler behavior gains a durable end-to-end correctness oracle.
- Migration corpora can identify intentional and accidental authoritative changes at committed boundaries.
- Exact compatibility domains make strong local claims possible without an indefensible universal promise.
- Side-effect suppression makes diagnostic replay safe for worlds that interact with external services.

### Costs and limitations

- Every authoritative external fact, random stream, extension, numerical backend, partition scheme, covered state or effect-intent projection, and operation profile needs versioned identity and evidence.
- Recording complete inputs and useful verification points consumes storage, CPU, privacy review, and operational attention.
- Exact old-domain replay depends on retaining compatible immutable artifacts.
- Divergence localization may identify only the first changed projection, not the underlying defect.
- Player-facing playback, client prediction replay, and broad cross-version compatibility require separate product work.

## How we will prove the decision works

- A station-like scenario records two players moving between maps, nested containment changes, timers, physics interactions, random events, administrative pause, and an admitted external-service response. Headless replay from the exact starting checkpoint reaches the same covered canonical committed states and committed effect-intent projections.
- The same artifact verifies under serial mode and several worker counts, incidental worker-to-fixed-partition assignments, task delays, and render rates while retaining the same declared partition identities, order, partitioning algorithm, and merge algorithm. No comparison depends on thread identity, completion order, or presentation frames. Changing that declared scheme requires a different compatibility domain and does not count as the same guarantee.
- Removing, duplicating, or reordering one admitted input, changing a random-stream state, or injecting unordered authoritative enumeration produces a divergence at the first affected verification boundary with input, projection, step, and provenance information.
- Replaying an effect that originally requested a durable-service write compares the same committed effect intent and idempotency identity, then admits the recorded service response at its later ordered-input position without contacting the real service or repeating the write.
- A compatible catalog adoption records both generations and reproduces one world's committed transition from recorded host/client coordination facts; it does not count as proof that the globally fenced multi-world transaction was re-executed. An incompatible layout or schema transition ends the segment with an explicit outcome rather than silently crossing receipts.
- A migration corpus restores one source checkpoint, applies one named forward migration, replays a bounded input corpus, and compares declared target projections while preserving the original checkpoint and replay artifact.
- Exact replay descriptors and the reviewed replay-reexecution profile produce the same complete report across Windows and Linux. A domain with demonstrated cross-platform equivalence admits `VerifiedAuthoritativeReexecution` across them; a domain without that evidence returns `CompatibilityDenied` before world publication. An injected evaluator failure produces no report, mode, or replay world, and a `ReadOnlyInspection` result never becomes verification.
- A conforming advanced extension proves its declared determinism class through the same replay corpus. An unknown native or unsafe fixture is denied from the verified domain or limited to a separately admitted inspection or playback mode in a disposable process.
- Re-execution creates fresh runtime identities and purpose-bound original-record-to-reexecution mappings. Stale, cross-artifact, cross-purpose, or forged mappings cannot resolve a target, raw runtime-ID equality is never the oracle, and possession of any artifact or mapping identity grants no authority.
- Fault injection, process termination, quota exhaustion, and a missing external input produce an incomplete artifact ending at the last trustworthy committed boundary; none is accepted as a complete replay.
- Divergence leaves replay-owner integrity `KnownSound` and returns a verifier result. An injected integrity-unknown replay-owner failure instead follows the reviewed ADR 0046 profile, closes or escalates the owner, and cannot return `Equal`.
- Corrupt, truncated, oversized, decompression-bomb, extremely long, and dependency-mismatched artifacts fail within declared resource limits before world publication and cannot reach real external sinks. A malicious dependency reference cannot trigger package acquisition, consent, credential use, or assembly loading.
- Ordinary clients and unauthorized operators cannot obtain hidden authority replay material or start re-execution. Concurrent and stale start, stop, retention, redaction, export, expiration, and deletion requests produce one authorized immutable lifecycle result and audit trail; secrets or credentials are absent from captured inputs.

## Implementation notes

No replay manifest, authoritative input ledger, random-state capture, replay identity declaration, replay-reexecution compatibility profile, replay-owner fault profile, canonical committed-state or effect-intent projection, effect verifier, divergence report, safe replay host, or retention control exists. Implementation status remains `Not started`.

## Follow-up decisions

- Replay manifest, segment, input, verification-point, integrity, compression, canonical encoding, `ReplayArtifactId`, replay-local identity, and purpose-bound mapping formats under ADR 0044.
- Reviewed FND-COMPAT replay-reexecution profile for exact runtime, operating-system, architecture, numerics, physics, partition, native-code, extension, migration, and admitted-mode facts.
- Canonical covered authoritative state and committed effect-intent projections, fingerprint algorithms, bounded differences, and collision treatment.
- Input admission capture, random-stream algorithms and state, timer ordering, catalog-transition recording, and gap detection.
- Reviewed ADR 0046 replay-owner/fault profile, replay host isolation, post-commit compare consumers, real-sink capability exclusion, and measured resource budgets.
- Authentication, authorization, immutable operation results, privacy classification, redaction, encryption, retention, export, deletion, audit, and incident-response policy.
- Replay-specific covered projection, bounded-difference, and headless-driver specifications built on ADRs 0039 and 0040.
- Replay migration, exact-artifact retention, forward-validation corpora, and compatibility reporting.
- Client prediction trace, presentation/player replay, spectator controls, and any live branching or time-travel product.

## References

- [ADR 0008](0008-explicit-versions-migrations-and-rollback.md)
- [ADR 0016](0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0020](../technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0029](../technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0034](0034-use-a-declared-ladder-for-advanced-game-extensions.md)
- [ADR 0035](0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [ADR 0039](0039-inspect-running-worlds-through-authorized-snapshots.md)
- [ADR 0040](0040-test-isolated-worlds-through-the-supported-runtime.md)
- [ADR 0042](../technical/0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043](../technical/0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](../technical/0044-generate-bounded-identity-declarations.md)
- [ADR 0045](../technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [ADR 0046](../technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047](../technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [World-model question 26](../../workshops/world-model-question-set.md#26-how-should-replay-and-determinism-fit-the-world-model)
- [Space Station 14 server replay recording](https://docs.spacestation14.com/en/server-hosting/server-replay-recording.html)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
