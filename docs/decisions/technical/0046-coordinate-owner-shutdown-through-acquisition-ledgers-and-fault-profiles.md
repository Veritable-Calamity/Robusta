# ADR 0046: Coordinate owner shutdown through acquisition ledgers and fault profiles

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-23
- **Decision level:** Technical
- **Owners:** Runtime and operations workstreams
- **Program ID:** `FND-FAULT`
- **Source queue IDs:** `F-CLEAN-01`, `F-FAULT-01`
- **Supersedes:** None
- **Refines:** ADRs 0017, 0026, 0028, 0029, and 0042
- **Product decisions served:** 0002, 0004, 0006, 0007, 0009, 0011, 0012, 0015, 0016, 0026, 0027, 0035, 0037, 0039-0041
- **Related decisions:** 0017, 0020, 0024, 0026, 0028, 0029, 0037, 0042, 0044, 0045

## The question

What common owner state, admission fence, cleanup ledger, fault report, and profile-extension contract will let scopes, worlds, clients, tools, operations, and future mesh owners close predictably without claiming that caller cancellation, a timeout wrapper, or an exception aggregate proves containment?

## The promise preserved

After shutdown or an owner-closing fault begins, an owner is permanently fenced and cannot be published, resolved, or used through supported APIs as though it remained healthy. A localized conforming failure can end only its proven containment boundary, every acquired resource remains accounted for even when unresolved, concurrent close callers see the same coordination truth, and integrity-unknown or still-running work is reported and escalated rather than hidden.

## Why this matters

ADR 0017 requires admission closure, bounded cleanup, aggregation, and leak detection. ADR 0028 requires attachments to end before session and world endpoints and host shutdown to report every owned resource. ADR 0026 permits another world to continue only when the failed boundary and shared integrity are known. ADRs 0029 and 0042 prohibit publishing authoritative work after an integrity-affecting fault.

The current kernel proves useful basics: explicit open/closing/closed states, attachment-first shutdown, reverse callback execution, and exception aggregation. It does not preserve the first failed close result, separate caller cancellation from owner teardown, bound a stuck cleanup, prove resource release, classify containment, or distinguish a terminated cleanup failure from an operation that may still be running.

A generic `DisposeAsync`, timeout wrapper, or large fault enum cannot supply those missing guarantees. The common contract must keep lifecycle, integrity, disposition, and operation outcome distinct, then let reviewed profiles provide owner-specific topology, budgets, and escalation.

## How the current Robusta implementation answers today

`OwnershipScope.BeginClose` changes `Open` to `Closing` and returns a snapshot of named cleanup delegates. `CloseResourcesAsync` invokes those delegates in reverse registration order and stores raw exceptions. `HostScope` serializes lifecycle work through a semaphore held across awaited cleanup, closes attachment, session, and world type buckets, and always calls `CompleteClose`.

A later close call receives global success because the original terminal result is not cached. The initiating caller's token controls semaphore acquisition and is passed into owner cleanup. A callback may hang indefinitely, cross-owner cleanup can deadlock through the held lifecycle semaphore, dictionary enumeration contributes child order, and a scope becomes `Closed` even when a resource may remain live.

## Options considered

### Option A: Per-owner close coordinator, acquisition ledger, and versioned profiles

Give every owner one close coordinator. The first explicit close or owner-closing fault atomically fences admission, installs one shared close operation, snapshots a committed acquisition ledger, and releases lifecycle locks before cleanup begins. All callers observe one immutable terminal report.

Owner-local reverse acquisition handles ordinary dependencies; explicit ownership topology orders attachments, endpoints, and parents. Versioned profiles supply resource classes, budgets, postconditions, and minimum escalation without weakening the common invariants.

This fits the explicit-scope architecture, remains implementable in ordinary .NET, and scales to later profiles. It requires a richer state machine, typed registrations, an owned deadline mechanism, leak probes, and structured reports.

### Option B: A full resource dependency graph and parallel cleanup scheduler

Register every child and resource with dependency edges, close in reverse topological order, and run independent branches concurrently.

This can model complicated native, client, and mesh resources and may salvage more cleanup after one branch stalls. It introduces cycle handling, graph versioning, concurrency safety, and scheduler semantics before CP02 needs them. A later profile may use a dependency graph internally only if it preserves this ADR's observable invariants.

### Option C: Enhanced `IAsyncDisposable` conventions

Keep the current state and callback list, add timeout wrappers, cancellation, and logging around `DisposeAsync`.

This is familiar and inexpensive. It cannot prove acquisition and release pairing, gives caller cancellation ambiguous power over teardown, loses terminal evidence unless another coordinator is built, and cannot classify a still-running cleanup or integrity radius. It is insufficient as the platform contract.

## Decision

Robusta will use Option A: a per-owner close coordinator, acquisition ledger, and versioned profiles.

The common technical contract is:

1. Expected rejection, absence, stale input, denied authority, or cancellation before admitted work is an operation result, not an owner fault. A fault begins only when the declared boundary or its integrity is affected.
2. Owner lifecycle, integrity, fault class, disposition, and close outcome are separate typed fields. No single catch-all `Faulted` state or raw exception type controls recovery.
3. The common lifecycle states are:

| State | Meaning |
|---|---|
| `Preparing` | Acquisitions are unpublished and must be reversible |
| `Open` | The owner is published and may admit declared work |
| `Closing` | Admission is permanently fenced and the one close operation is running |
| `Closed` | Every applicable cleanup terminated and ledger postconditions reconciled |
| `ContainmentLost` | Cleanup or a required postcondition cannot be established; broader containment is required |

4. `Preparing` publishes to `Open` only at the declared atomic boundary. `Preparing` or `Open` transitions to `Closing` once. `Closing` reaches only `Closed` or `ContainmentLost`. No state reopens, retries the owner, or reuses its identity.
5. Integrity is `KnownSound`, `KnownCompromised`, or `Unknown`. Known-compromised state may support orderly teardown but never ordinary continuation. A truly contained non-authoritative fault leaves owner integrity `KnownSound`. Unknown integrity never resumes.
6. Common fault classes are `ContainedOperational`, `ContractViolation`, `IntegrityViolation`, and `ContainmentFailure`. Profiles may add stable reason codes, not weaken these meanings.
7. Common dispositions are `Continue`, `CloseOwner`, `EscalateParent`, `TerminateProcess`, and `RequireExternalTermination`. `Continue` is legal only for a profile-declared bounded incident while the owner remains `Open`, integrity is known sound, and no prohibited publication occurred. It records the incident without fencing admission or installing the close operation. `CloseOwner` or any stronger disposition initiates owner close and permanently fences admission.
8. Mutually exclusive close outcomes are `Clean`, `ClosedWithContainedFaults`, and `ContainmentLost`. The first two map to lifecycle `Closed`; the last maps to lifecycle `ContainmentLost`. Escalation is recorded only as a disposition, so an outcome and disposition do not compete.
9. Profiles map owner kind, lifecycle phase, stable fault code, integrity evidence, resource class, and integrity radius to a minimum disposition. Game callbacks and exception types cannot choose or shrink their own containment radius.
10. ADR 0026 remains the minimum escalation rule: a conforming world-local fault may close one world only when shared integrity is known; a shared, native, hung, corrupt, or integrity-unknown failure escalates to the host process or external supervisor.
11. Every owner has exactly one close coordinator. The first explicit close request or fault classified as `CloseOwner` or stronger atomically installs and caches one close operation and fences admission. A `Continue` incident uses the open owner's bounded incident record and does not start close. Before the close report is sealed, concurrent owner-closing causes join monotonically: integrity may only move `KnownSound` to `KnownCompromised` to `Unknown`, disposition may only widen from `CloseOwner` through `EscalateParent`, `TerminateProcess`, and `RequireExternalTermination`, and bounded causes are retained in canonical order.
12. `RequestClose` is nonblocking and always returns the one cached close-operation handle. Concurrent and later external callers may await that handle and receive the same report. Code executing inside that close operation cannot await itself; such an attempt returns a stable `ReentrantWaitRejected` result while retaining the same handle for observation after the callback returns. No caller receives synthetic success that erases an earlier cleanup fault.
13. The admission fence is a linearization point. Child creation, attachment, lease acquisition, cleanup registration, owned-task start, and other declared admission after it fail with typed closed-admission outcomes.
14. Before any external acquisition begins, admission creates a ledger entry in `Reserved` state and joins the owner's in-flight preparation set. Entry states are `Reserved`, `Acquiring`, `Committed`, `Transferring`, `Reversing`, and one accounting terminal state. The fence rejects new reservations and the close operation joins every pre-fence preparation or transfer before releasing its prerequisites or finalizing the cleanup snapshot. A pre-fence operation that has not completed its publication linearization when the fence wins may settle an acquisition only into the ledger for accounting and reversal; it must publish no scope, child, endpoint, lease, task, or other capability. Only publication that completely linearized before the fence remains visible and joins the close snapshot. A preparation or transfer that cannot settle within its owned bound causes containment loss; shutdown never races past it. ADR 0044 identity reservations are recorded preparation facts and consumed values are never reused even when activation reverses.
15. Lifecycle locks are never held while awaiting or invoking construction, cleanup, user, extension, native, cross-owner, or diagnostic callbacks.
16. A resource acquisition and its ledger commitment form one preparation transaction. Successful acquisition moves its pre-existing entry to `Committed` before any later fallible action. Failure moves it through `Reversing` to an accounting terminal state or leaves it explicitly `Unresolved`. A registration records stable acquisition ordinal, resource class, cleanup class, integrity radius, profile budget class, and a verifiable immediate quiescence postcondition or leak probe.
17. Cleanup order is reverse successful acquisition within one owner, not reverse arbitrary callback registration or dictionary enumeration. The owner topology separately determines child-before-parent order.
18. Accounting terminal states are `Released`, `AlreadyReleased`, `Transferred`, `RetainedImmutable`, `Unresolved`, and `SkippedDependency`. A transfer first atomically moves the source entry from `Committed` to `Transferring` and joins the source in-flight set before target admission; source close joins that attempt and cannot independently clean the resource. It then reserves and commits the corresponding target-ledger entry under the target owner's admission fence. The handoff linearizes only when one immutable transfer record references both committed ledger ordinals and the source entry becomes `Transferred`. Failure before target commitment reverses the target reservation and returns the source to responsible `Committed` state or marks it `Unresolved`. Failure after target commitment but before handoff must either reverse the target to a proven terminal release while restoring source responsibility or mark both entries `Unresolved` and escalate; two committed owners may not survive the attempt. Retention is legal only for a declared immutable diagnostic or artifact record under a named external retention authority and proven postcondition; live or mutable work must transfer or remain unresolved. Throwing is a cleanup fault, not a normal disposition.
19. After a cleanup exception, the immediate quiescence probe decides whether earlier prerequisites may unwind. If it proves no ongoing use, the exception is aggregated and safe cleanup continues. If it fails, is unknown, or the callback exceeds its hard bound, that dependency stack stops, unresolved and skipped entries are recorded, lifecycle becomes `ContainmentLost`, and escalation occurs. Independent branches continue only where the profile proves they share no prerequisite or integrity radius. Any `Unresolved` resource, or any `SkippedDependency` resource that is live, mutable, or lacks a proven terminal postcondition, prevents `Closed`, forces the `ContainmentLost` outcome, and requires a disposition broader than `CloseOwner`; profiles may only strengthen that rule.
20. Waiting with a timeout never claims that managed or native work was aborted. Stack overflow, fail-fast, corrupted native state, process termination, and other uncatchable conditions remain external-containment cases.
21. Caller cancellation cancels only that caller's wait. Once fenced, teardown is owner-owned work. The fence signals an owner-ending token to admitted work; cleanup receives a separate coordinator-owned deadline token.
22. Deadlines use an injected monotonic host clock, never simulation time or an adjustable wall clock. A simulation pause cannot pause teardown.
23. Profiles supply per-resource soft allowance, owner soft deadline, owner hard deadline, parent reserve, diagnostic reserve, leak-check deadline, and report limits. Numeric values are reviewed profile data based on a versioned workload, not constants chosen by this common ADR.
24. Soft expiry requests cooperative cancellation and records overrun. Hard expiry establishes containment loss, not successful cancellation. Child budgets must fit within the parent's remaining hard bound.
25. Cleanup failures are ordered canonically by ownership topology, acquisition ordinal, and stable local fault ordinal, never by task completion, exception text, worker number, or dictionary order. Once the close report is sealed, a later completion, fault, or new containment observation becomes an append-only late-incident chain carrying the current effective escalation; it may widen external escalation but never rewrites or downgrades the sealed report. The chain uses canonical ordering plus profile count and byte bounds. Overflow is compacted into one stable saturating summary with suppressed-count evidence and minimum escalation rather than retaining unbounded incidents.
26. A structured report records exact profile and version; owner kind and ADR 0044-safe identity projection; lifecycle phase; resource class and ordinal; stable reason code; duration and budget outcome; sanitized exception classification; cleanup disposition; leak evidence; integrity; minimum disposition; `dispositionAtSeal`; and skipped dependent work. Consumers derive the current effective disposition from the sealed report plus its late-incident chain rather than treating `dispositionAtSeal` as permanently final.
27. Human messages and raw exceptions are protected diagnostic projections, not stable reason identity. Sealed-report and late-incident counts and bytes are independently bounded. Overflow produces a stable summary and escalation evidence rather than silent truncation.
28. Garbage collection, absence from a dictionary, a completed `Task.WaitAsync`, or lack of current resolution is not leak proof. The coordinator reconciles its acquisition ledger with release, transfer, immutable-retention, unresolved, or skipped records and kind-specific postconditions.
29. Unknown, failed, or contradictory postcondition evidence defaults to leak suspected and the profile's minimum escalation. A resource cannot disappear from accounting merely because cleanup threw.
30. Later profiles may add owner kinds, fault codes, resource classes, measured budgets, stricter dispositions, and explicit independent cleanup branches. They may not reopen a fenced owner, continue after unknown integrity, publish post-fault authoritative work, lose a cleanup failure, weaken parent containment, or reinterpret a prior report.
31. Cleanup callbacks may request another owner to close but cannot await an arbitrary owner close handle. Only the ownership-topology coordinator may await cross-owner close, and only in its validated acyclic child-before-parent order. The supported close handle rejects callback-originated or cyclic cross-owner waits with stable `CrossOwnerWaitRejected` evidence; direct self-await remains `ReentrantWaitRejected`.

## Common ADR versus subordinate profiles

This ADR selects only the lifecycle, integrity, fencing, coordination, ledger, cancellation, report, leak-evidence, minimum-escalation, and versioned-extension invariants.

The separately reviewed CP02 cleanup/fault profile must select the host, world, session, attachment, and catalog-lease resource classes; ADR 0028 topology; exact admission calls; reverse child admission order; cleanup postconditions; measured soft and hard deadlines; report limits; redaction fields; and CP02 escalation matrix.

Later reviewed profiles own CP04 scheduler and world-fault frontiers, client and device recovery, creator supervision, native extension faults, CP14 watchdog and process restart, and CP19 mesh fencing. Partial profile text carries no implementation authority.

## What we deliberately will not do

- Treat `DisposeAsync`, caller cancellation, a timeout wrapper, forced garbage collection, or dictionary removal as proof of containment.
- Catch an exception, continue authoritative simulation, and call the owner healthy when integrity is unknown.
- Promise whole-step rollback, automatic world retry, system quarantine, or transparent process restart.
- Abort arbitrary managed threads or claim that an incomplete callback stopped using its prerequisites.
- Let game code select its fault radius, escalation parent, cleanup deadline, or stable fault reason.
- Freeze CP04, client, tooling, operations, native, or mesh-specific state matrices in the common ADR.
- Select general CPU, memory, queue, I/O, or output workload quotas owned by `FND-BUDGET`.
- Store unbounded raw exceptions, identifiers, or cleanup reports.

## Consequences

### Compatibility and migration

Current scope close methods must migrate from nullable snapshots and per-call results to a shared immutable close receipt. Named callbacks become typed acquired-resource registrations. `DisposeAsync` may remain an idiomatic adapter that throws from the structured report, but it is not the report authority.

Profile and report schemas are versioned. Changing minimum escalation, outcome meaning, or deadline semantics is a compatibility event rather than a logging tweak.

### Security and failure handling

The contract makes containment claims conservative. A localized failure is cheaper only when ledger and integrity evidence prove it. Unknown state escalates even when that is operationally inconvenient.

Structured reports and ADR 0044 projections reduce accidental identity and exception disclosure. They do not make in-process trusted code a sandbox.

### Operations

Operators receive one causal report for a close/fault storm, deterministic failure ordering, deadline phase, skipped dependency evidence, suspected leaks, and escalation. Later observability policy owns retention, indexing, privacy, and high-cardinality budgets.

## Bounded first implementation scope

This decision authorizes the common state and normalized report models, per-owner close coordinator, acquisition-ledger abstraction, injected monotonic test clock, profile-model validation, and synthetic concurrency, timeout, aggregation, and leak fixtures. Exact serialized profile and report schemas remain reviewed specifications required before durable or cross-process use.

It does not authorize replacing the current ownership close path until the CP02 subordinate profile is separately reviewed and approved. It does not implement world scheduler faults, process watchdogs, restart, client recovery, native containment, operations recovery, or mesh behavior.

## How we will prove the decision works

- Randomized admission-fence races either include and clean a complete child/resource or reject and reverse it; no partial registry entry or catalog lease remains.
- Many simultaneous and later close callers trigger cleanup once and receive the same sealed report; an in-close self-await receives `ReentrantWaitRejected` without starting another close and may observe the same handle afterward.
- Canceling the first caller's wait does not cancel owner teardown; another waiter receives the unchanged terminal report.
- Attachment-first topology and reverse successful acquisition hold independently of dictionary order, type buckets, callback delay, and task completion order.
- Injected failures in several resources attempt every safe remaining cleanup and produce structurally equivalent normalized report models across randomized timing.
- A never-completing cleanup under a tiny test profile produces `ContainmentLost`, one escalation, skipped-dependency evidence, and never a false `Closed`.
- A terminated cleanup exception permits earlier resources, including catalog leases, to release only when its immediate quiescence probe proves they are no longer used; failed or unknown proof records containment loss.
- Omitted release records, orphan attachments, retained tasks, lease mismatches, and failed probes report suspected leaks; forced garbage collection proves nothing.
- Known-contained attachment or world faults leave unrelated owners open only when postconditions prove shared integrity; unknown integrity escalates.
- Manual monotonic-clock tests prove soft and hard deadlines are unaffected by simulation pause, caller cancellation, locale, or wall-clock change.
- Cleanup callbacks run without lifecycle locks; callback-originated cross-owner waits, topology cycles, and self-reentrant awaits are rejected deterministically without deadlock or a second close.
- Reports remain bounded and redacted while retaining stable reason, owner, resource, budget, integrity, and disposition evidence.

## Implementation notes

The current kernel is useful groundwork but does not implement this decision. In particular, it forgets prior close failures, forwards caller cancellation into teardown, holds a lifecycle semaphore across callbacks, has no deadlines or leak evidence, and marks scopes closed after failures without proving containment.

Implementation status remains `Not started`. Current TODOs should remain or be refined as:

- `// TODO(FND-FAULT): replace per-call close snapshots with one cached owner close operation and terminal report.`
- `// TODO(FND-FAULT/CP02): register acquired resources transactionally with typed cleanup and leak probes.`
- `// TODO(FND-FAULT/CP02): apply measured monotonic deadlines and the reviewed CP02 escalation matrix.`
- `// TODO(OPS-RECOVERY): add supervisor health, external termination, restart, and recovery policy.`

## Dependencies and interaction with queued decisions

Accepted ADR 0045 may generate acquisition and disposition metadata, but this ADR does not depend on its implementation: handwritten internal fixtures can prove the common contract. Production generated activation must not claim reversal safety until the applicable common contracts from both accepted ADRs are implemented and the CP02 profile is separately reviewed and approved.

Accepted ADR 0044 owns safe identity projection. `FND-BUDGET` owns ordinary workload quotas. `SIM-*` and ADR 0042 own authoritative commit boundaries; the CP04 fault profile will map their integrity outcomes. `DX-SUPERVISOR` and operations decisions own process health and restart. Later client, native, and mesh decisions own their containment boundaries.

## Follow-up decisions and specifications

- Fault/profile/report schemas, stable reason registry, exception sanitization, and canonical ordering.
- Reviewed CP02 ownership cleanup/fault profile with measured numeric deadlines and leak probes.
- CP04 world/scheduler fault profile after the simulation ADR batch.
- Creator-supervisor, client/device, native-extension, operations, and mesh profiles at their owning checkpoints.
- General resource budget declarations under `FND-BUDGET`.
- Supported fault-injection APIs only after the relevant testing product decision is accepted.

## References

- [ADR 0017](0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0026](../product/0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0028](0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0042](0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0044](0044-generate-bounded-identity-declarations.md)
- [ADR 0045](0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [`FND-FAULT` program package](../../status/adr-development-program.md#foundation-compatibility-and-lifecycle)
- [Current ownership state](../../../src/Robusta.Runtime.Shared/Hosting/OwnershipScope.cs)
- [Current close result](../../../src/Robusta.Runtime.Shared/Hosting/ScopeCloseResult.cs)
- [Current ownership tests](../../../tests/Robusta.Runtime.Tests/Hosting/OwnershipScopeTests.cs)
