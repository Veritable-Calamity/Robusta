# ADR 0029: Enforce phase-scoped access and buffered deterministic effects

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0006, 0011, 0012, 0015, 0016
- **Related decisions:** 0017-0020, 0023, 0026

## The question

What may a system read, mutate, retain, and publish while ADR 0020 executes systems concurrently, and how will the runtime prevent hidden aliases, thread affinity, I/O, exceptions, or reduction order from making authoritative results depend on worker scheduling?

## The promise preserved

A game system that is safe to run concurrently gets parallel execution without acquiring a second gameplay meaning. Code whose independence cannot be proved still runs correctly and predictably through an exclusive fallback.

## Why this matters

A dependency graph is safe only when its access declarations describe all reachable mutable state. Ordinary C# references, mutable statics, callbacks, native libraries, unordered collections, locks, background tasks, and direct I/O can bypass those declarations. Completion-order merging or recovery from a partially failed batch would then make machine timing part of the game rules.

Robust Toolbox currently topologically orders systems but calls their updates sequentially. Its systems can directly mutate entities and publish events, so existing Robust Toolbox and SS14 code is not evidence of parallel safety and must initially migrate conservatively.

## How Robust Toolbox answers today

[`EntitySystemManager`](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntitySystemManager.cs) resolves before/after dependencies, then invokes systems serially. [`EntityManager`](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntityManager.cs) permits immediate component access and mutation around a serial update, event, deletion, and culling sequence.

This is straightforward for current content but leaves data ownership and side effects implicit.

## How the Robusta prototype answers today

The predecessor prototype has no phase access leases, generated access manifests, deterministic effect channels, reducer registry, affinity lanes, or serial-oracle comparison runner.

## Options considered

### Option A: Generated phase leases with safe exclusive fallback

Generate access manifests from typed system inputs and queries, expose only phase-scoped data views, buffer externally observable effects, and permit parallel batches only when the generated manifest and analyzers prove independence. Anything unprovable receives exclusive world access or is rejected when exclusivity cannot make the operation safe.

This imposes constraints on authoritative code but makes ADR 0020 enforceable and allows imported systems to become parallel incrementally.

### Option B: Trust developer declarations and ordinary thread-safe collections

Let authors declare read/write sets manually and use locks or concurrent collections around shared state.

This is initially flexible, but declarations can drift from implementation and lock acquisition or concurrent-container ordering can become authoritative behavior. Review alone is not a sufficient safety boundary.

### Option C: Give every system a complete world snapshot

Run systems against isolated world copies and merge complete results afterward.

This gives strong isolation but makes identity, memory use, conflict resolution, and large-world performance unnecessarily expensive for the first release.

## Decision

Robusta will use Option A. This decision supplies the enforceable access-and-effect contract required by ADR 0020:

1. The scheduler invokes each system with a non-storable phase context and generated access views. Systems do not receive a raw world, service container, component store, effect queue, or scheduler.
2. An access key identifies a world store or resource and an optional scheduler-issued partition. Read/read access is compatible. Any write conflicts unless both accesses use partitions whose disjointness the scheduler establishes. Arbitrary predicates do not prove disjointness.
3. Typed system parameters, generated queries, effect writers, reducers, and random-stream access generate the system manifest. Analyzers verify known SDK calls against it. Explicit metadata may widen access but cannot hide observed access or suppress a violation.
4. If generation or analysis cannot prove a complete access set, the manifest becomes `ExclusiveWorld` and produces an actionable diagnostic. A system that claims parallel safety while retaining ambiguous access fails build or startup validation rather than silently racing.
5. Phase queries have a specified canonical order or a scheduler-defined stable partition order. Systems may not derive authoritative sequence from hash-table enumeration, worker identity, task completion, or another unspecified order.
6. Borrowed component references, spans, query enumerators, service views, buffers, and random streams cannot escape their invocation. They may not be stored in system fields, returned, captured by escaping delegates, placed in statics or thread-locals, or used by background work. Entity handles may be retained as values, but using one later requires a new phase access check.
7. Parallel-authoritative data is engine-owned value data or a registered immutable reference type. A world-owned mutable reference graph or raw array may use a tracked exclusive adapter. Shared mutable statics, unsafe pointers, reflection-based mutation, and untracked callbacks are rejected for conforming authoritative code because per-world exclusivity cannot contain them. An unregistered external effect is rejected even when the system is exclusive.
8. A phase callback may not block, await, spawn untracked tasks or threads, acquire game-visible locks, or use synchronization timing as input. Work that legitimately spans time is represented as host work whose result later enters the world as an ordered external input.
9. Thread affinity and data exclusivity are separate declarations. Work may target `AnyWorker`, the scheduler thread, or a registered named affinity lane. A native adapter declares its thread safety, reentrancy, memory ownership, deterministic inputs and outputs, and required lane. Unknown native code runs only through an approved exclusive adapter; UI, audio, rendering, and GPU effects remain outside authoritative phases.
10. Systems directly mutate only storage covered by their write lease. Structural operations, gameplay events, timer changes, replication output, external requests, and observations go to invocation-local buffers.
11. Each buffered record receives a stable merge key composed from phase, stable system identity, stable partition identity, and invocation-local sequence. Merge order never uses worker number, thread identity, wall time, or completion order. Duplicate or conflicting operations require an explicit channel policy; there is no implicit last-completer-wins rule.
12. Shared accumulation uses a registered deterministic reducer with a stable identity, input and output schemas, identity value, canonical contribution order, and versioned fold algorithm. Floating-point and other non-associative reductions use that fixed order; worker-local tree shape may not vary with worker count unless the exact tree is itself part of the receipt.
13. Authoritative code performs no filesystem, network, database, process, environment, locale, or wall-clock I/O during a phase. Reads occur through host services and become ordered inputs. External writes consume committed observations after the step and carry stable idempotency identities where retry is possible.
14. A worker fault is captured and reported in stable system-and-partition order after the batch joins. No buffer from the failed batch is published. A transaction-capable store may restore the prior committed boundary; otherwise the affected world enters the terminal fault state defined by the accepted fault policy. It never resumes from potentially partial mutation, and other workers are not asynchronously aborted according to which one faulted first.
15. Serial execution uses the same manifests, access views, query ordering, buffers, reducers, random streams, fault rules, and merge code. It may not call a simpler direct-mutation API. Canonical committed-state hashes and effect traces from serial and parallel runs form the correctness oracle.

## What we deliberately will not do

- Treat concurrent collections, locks, atomics, or thread safety alone as deterministic gameplay semantics.
- Accept manually asserted parallel safety that tooling cannot verify.
- Allow a captured component reference or raw world service to outlive its phase lease.
- Let exclusive systems perform arbitrary I/O or unregistered native calls.
- Merge effects or exceptions in task-completion order.
- Continue a world after exposing partial results from a failed parallel batch.
- Claim that agreement with the serial oracle proves race freedom; access enforcement remains required.

## Consequences

### Benefits

- Existing or opaque systems have a safe exclusive migration path.
- Parallel eligibility is reviewable in generated manifests and conflict graphs.
- Effects, faults, and reductions retain one meaning across worker counts.
- Native and thread-affine integrations become explicit instead of hidden scheduler constraints.

### Costs and limitations

- SDK types, generators, analyzers, runtime lease checks, stable reducers, and affinity adapters require substantial implementation.
- Some legal C# patterns are intentionally unavailable inside authoritative phases.
- Imported Robust Toolbox systems usually begin exclusive and need deliberate refactoring.
- Exclusive fallback preserves correctness but does not promise speedup.
- Recoverable batch faults may require transactional storage; implementations without rollback terminate the affected world.

## How we will prove the decision works

- `GeneratedManifestMatchesReachableAccess` verifies typed inputs and known SDK calls produce the same stable manifest on clean builds.
- Analyzer fixtures reject escaped leases, mutable statics, untracked tasks, unordered authoritative enumeration, undeclared I/O, unsafe aliases, and native calls without an adapter.
- Instrumented runtime tests detect forged, reflective, or dynamically selected access that exceeds a lease.
- Conflict tests prove read/write and overlapping partitions never share a batch, while proven-disjoint partitions can.
- Randomized worker counts, registration orders, task delays, and affinity-lane delays produce the same committed hash, effect trace, reducer result, random positions, or stable diagnostic as the serial oracle.
- Reducer tests include non-commutative operations and floating-point inputs and remain stable across partition and worker counts.
- Fault injection at every buffer and mutation point proves no failed-batch effect is published and no partially faulted world resumes.
- I/O tests prove external results become ordered inputs and retryable output uses stable committed identities.
- Representative migrated Robust Toolbox systems run exclusive first; each parallel reclassification requires analyzer evidence, conflict tests, oracle agreement, and measured crossover data.

## Implementation notes

Implementation should prefer compiler-enforced lifetime shapes such as `ref struct` and scoped references, supplemented by Roslyn analyzers and instrumented runtime validation. Generated manifests and reducer algorithms are exact-runtime receipt inputs.

## Follow-up decisions

- Standard store, relation, spatial, and partition capability vocabulary.
- Event categories, same-step visibility, command results, and post-commit continuations.
- Native physics determinism and internal parallelism contract.
- World fault recovery, checkpointing, and host restart policy.
- Performance thresholds for retaining a parallel classification.

## References

- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0020](0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [Unity job-system safety](https://docs.unity3d.com/6000.0/Documentation/Manual/job-system-native-container.html)
- [Bevy ECS](https://bevy.org/learn/quick-start/getting-started/ecs/)
- [Flecs systems and staging](https://www.flecs.dev/flecs/md_docs_2Systems.html)
- [.NET parallel-programming pitfalls](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/potential-pitfalls-in-data-and-task-parallelism)
- [C# `ref struct` safety rules](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct)
