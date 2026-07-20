# ADR 0042: Use typed message kinds and transactional structural commits

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0005, 0006, 0011, 0012, 0013, 0015, 0016, 0026, 0030, 0031, 0032, 0035, 0037, 0038
- **Related decisions:** 0018-0021, 0023, 0024, 0028, 0029, 0043

## The question

How will game code ask for information, request authoritative changes, exchange gameplay facts, and observe committed outcomes without callback reentrancy, worker completion order, or a partially applied structural change becoming gameplay semantics?

## The promise preserved

Developers can tell whether a message is a synchronous question, an authoritative command, a same-step gameplay event, or an observation of an already committed outcome. Entity, map, frame, relationship, timer, and catalog-generation changes become visible only at declared boundaries, and every rejected or conflicting command has one inspectable result.

## Why this matters

ADRs 0019, 0020, and 0029 require structural staging, deterministic parallel output merging, and buffered effects, but they intentionally leave event categories, same-step visibility, command results, conflict policy, and post-commit continuations open. If one general event bus fills that gap, an apparently harmless handler can mutate structural state reentrantly, depend on registration order, observe half-updated indexes, or publish an external effect before the state that justified it commits.

The spatial contracts add multi-object operations whose invariants span maps, frames, containment, attachment, and lifecycle. Catalog adoption adds a fenced transaction whose success must not be confused with arbitrary gameplay rollback. These operations need a common commit vocabulary without making every game-state write event-sourced or distributed.

## Options considered

### Option A: Typed phase-bound channels with deterministic commit frontiers

Give requests, commands, gameplay events, and committed notifications distinct APIs and schemas. Buffer authoritative outputs per ADR 0029, process events in bounded deterministic waves, and apply structural commands through validated all-or-none transaction units at the structural-commit frontier.

This preserves ergonomic in-process interaction while making visibility, authority, ordering, and failure explicit. It requires generated declarations, several channel APIs, commit planning, result retention, and disciplined migration from immediate callbacks.

### Option B: One synchronous event bus with immediate mutation

Publish every question and change through one dispatcher and let handlers mutate the world immediately. This is familiar to many ECS users and can make small features concise.

It makes queries depend on handler order, permits reentrant lifecycle changes, conflicts with deterministic parallel batches, and cannot guarantee that indexes, replication, and observers see one complete structural outcome.

### Option C: Persist every state change as an append-only event and rebuild by replay

Make the event log the primary authoritative state and derive world state from it. This can offer powerful audit and replay properties.

It would force event-sourcing constraints onto high-frequency spatial and simulation state before the product has selected replay guarantees, and it would make schema evolution and station-like workloads substantially more expensive. Durable logs may later consume this ADR's committed records without becoming the first-release mutation model.

## Current review position

Option A is recommended for review. This is a proposed technical position, not an accepted decision and not implementation authority.

If accepted, the mechanism contract would be:

1. Robusta exposes four nominal message kinds with separate generated manifests and SDK entry points:
   - a **request** is a synchronous, directed question against the caller's current phase-scoped read capabilities and returns one typed result;
   - a **command** is an authoritative intent whose success may change world structure or another boundary-owned resource and whose final outcome is available only after its commit frontier;
   - a **gameplay event** is a world-local authoritative fact delivered to zero or more declared handlers in deterministic event waves; and
   - a **notification** is an immutable observation of a completed commit or terminal rejection and cannot alter that outcome.
2. A message declaration has a stable package-qualified schema identity, version, side, authority requirement, source and target capability, payload bounds, delivery phase, handler cardinality, result or reduction policy, and diagnostic name. Reflection discovery, CLR type names, and subscription order are not message identity or ordering.
3. A request has exactly one declared handler or one declared deterministic reducer. Zero handlers, ambiguous handlers, a handler fault, or an exceeded resource bound returns a typed failure. A request may read only through the caller's current ADR 0029 lease, may not outlive it, and may not publish an event, command, timer change, notification, external effect, or structural mutation while being evaluated. Work needing those effects uses a command instead.
4. Requests are never used to obtain mutable component aliases, raw stores, scope resolvers, callbacks, or deferred access. A returned value is an owned value, registered immutable value, bounded snapshot, or opaque capability whose later use performs a fresh access and authority check.
5. Commands submitted during authoritative work enter the invocation-local command buffer from ADR 0029. Each receives a world-local operation identity, origin merge key, submitting authority, target boundary, schema identity, preconditions, and optional transaction-group identity. Submission acknowledges only successful enqueueing; it does not report that the command committed.
6. External commands first pass authentication, side, rate, payload, step-window, lifecycle, and authority admission. Their stable admission sequence becomes their origin ordering key. A client-chosen operation identity is correlation data only and cannot choose merge order, bypass validation, or become a runtime entity identity.
7. The systems-and-events portion of each ADR 0020 step has fixed scheduler nodes: admit and order external inputs without executing game handlers; make due timers eligible; drain the input-and-timer seed event frontier; then, for each declared system batch in stable graph order, complete and merge that batch and drain every causally produced gameplay-event wave before the next batch begins. Events emitted by handlers enter the next wave. A handler is never invoked recursively from `Publish`, and a later wave cannot become visible before the complete current wave has merged. A later system batch observes permitted component-value writes from earlier drained frontiers under the declared dependency and lease graph, but no system or handler sees buffered structural changes before the structural-commit frontier.
8. Event envelopes are ordered by scheduler phase, stable producer identity, stable partition identity, invocation-local sequence, event schema identity, and declared target key. Handler order is generated from stable handler identities and dependency edges. Worker number, task completion, registration, hash enumeration, and wall time never order delivery.
9. Event handlers run through the same access leases, isolated buffers, reducers, random streams, affinity rules, and serial oracle as systems. Direct component-value writes are governed by their declared phase leases; structural changes, further events, timers, replication output, external-effect intents, and observations remain buffered. `External-effect intent` is the precise term for the buffered work called an external request in ADR 0029; it is not this ADR's synchronous read-only request kind.
10. Event declarations and input admission enforce receipt-versioned limits for wave count, events, bytes, target fan-out, and work per step. A statically invalid or over-budget input may be rejected only before any authoritative handler executes. Exhaustion discovered after a handler begins is an integrity-affecting world fault under ADR 0026 unless a separately accepted store contract proves that every affected value write and buffered output can be discarded back to the pre-handler boundary. The runtime never drops an arbitrary suffix, reports a partially executed input as rejected, or continues with an undisclosed partial gameplay meaning.
11. Ordinary queries and event handlers during the systems-and-events phase observe the committed structural shape from the start of that frontier. They do not see entities, components, maps, frames, or relations merely because a structural command has been buffered. This rule does not delay value writes already permitted and ordered by ADRs 0020 and 0029.
12. At the structural-commit frontier, the world joins every successful producer, deterministically merges command buffers, validates targets and preconditions against the committed before-state plus earlier accepted plans, and classifies each command as accepted, rejected, stale, unauthorized, conflicting, superseded by an explicitly declared policy, or faulted.
13. Conflict policies are declared per command schema and structural resource. Supported policies are reject the later ordered claimant, coalesce provably equivalent intents, or use a registered deterministic resolver with a versioned algorithm. There is no implicit last-writer, last-completer, handler-order, or dictionary-order policy. A resolver cannot grant authority or weaken a lifecycle, relationship, spatial, or catalog invariant.
14. One command is the default atomic transaction unit. Commands that must preserve a cross-object invariant name one bounded transaction group before merge. A group declares its maximum members, affected stores and indexes, prepare order, and failure result; dynamic enrollment during commit is prohibited. An invalid member rejects the whole group without publication.
15. An accepted structural unit prepares allocations, relationship dispositions, resource acquisitions, index deltas, identity-table changes, and any required inverse while unpublished. It then applies storage, handle or generation tables, maps and frames, relationship indexes, timer ownership, query indexes, and replication-source state as one transaction. Failure before publication reverses acquired work and exposes no partial structural state.
16. Structural publication creates one immutable commit record containing the world, step, frontier, operation and group identities, before and after structural versions, schema and catalog-generation context, outcome, and source provenance. Lifecycle, map, frame, relation, catalog-adoption, replication, inspection, and persistence consumers read that record rather than observing internal mutation callbacks.
17. Each command receives exactly one final immutable result after its transaction unit terminates. Results distinguish rejection before preparation, validation or precondition failure, deterministic conflict, successful commit, proven reversal before publication, and integrity-unknown fault. A missing target is never reported as success merely because the command was best effort; explicitly best-effort schemas still publish their terminal absence result.
18. Code that depends on a command result registers a bounded post-commit continuation or consumes the result through a later phase or step. Continuations are ordered buffered work, not callbacks inside commit. They cannot change the completed transaction, and any new authoritative intent enters the next eligible command frontier.
19. Notifications are emitted only from immutable terminal results or other committed observations after all affected stores and indexes agree. Notification handlers cannot receive mutable world access or invoke same-step authoritative work. A handler may update diagnostics or presentation, enqueue an explicitly ordered future input, or request an external side effect carrying the committed idempotency identity.
20. Replication publishes only committed births, structural changes, removals, and catalog-generation outcomes. Clients never receive preparing entities or a notification whose corresponding committed state is unavailable to that client's admitted catalog and schema generation.
21. Runtime-map birth and ending, same-world frame relocation, relation and attachment changes, lifecycle changes, and entity capability changes use structural commands and commit records. Their specialized validators remain separate; this ADR does not turn a map, frame, compact cell, or relation into an entity.
22. Catalog adoption uses the same command-result and committed-notification vocabulary only for each world-local prepared delta. ADR 0037 still owns multi-world fencing, client-generation admission, reversal before success publication, and integrity-unknown fault behavior. An ordinary event or notification cannot initiate an unfenced generation switch.
23. Checkpoint capture, creator-document editing, and general cross-world transfer are distinct transactions. They may correlate their work to committed world records, but neither a notification stream nor a structural command buffer is a checkpoint, map-source journal, transfer activation record, or durable event log.
24. Generated message manifests and commit schemas are exact runtime-receipt inputs. Compatible changes may add an optional field with a defined default or add an unrelated message schema; changes to authority, ordering, handler cardinality, reducer, conflict policy, transaction meaning, or required data require an explicit compatibility outcome and usually a restart, reconnect, or migration.
25. Diagnostics expose queue size, oldest age, wave depth, handler and reducer identity, rejected admission, command latency, conflict and stale rates, transaction preparation and commit duration, reversal, notification lag, and bounded redacted provenance. Operators may inspect metadata and committed records but gain no authority merely by knowing their identities.

## What we deliberately will not do

- Use one term such as "event" for a synchronous query, authoritative intent, gameplay fact, and committed observation.
- Permit recursive dispatch or structural mutation from a request or notification handler.
- Report command submission as command success.
- Let producer completion, event subscription, or collection enumeration decide authoritative order.
- Expose a half-published entity, map, frame, relation, or catalog generation.
- Claim whole-step rollback for arbitrary component writes or external effects.
- Treat notifications as a durable replay log, checkpoint, or exactly-once external message bus.
- Make all game state event-sourced for the first release.

## Consequences

### Compatibility and migration

Robust Toolbox-style event calls and prototype callbacks require semantic classification rather than mechanical renaming. Pure directed questions become requests; authoritative intent becomes commands; same-step facts become gameplay events; and lifecycle or structural observations become committed notifications. Immediate component mutations that remain legal use ADR 0029 leases, while immediate structural mutation must move to commands and continuations.

Message, reducer, conflict-policy, and commit-record schemas are versioned separately. Migration tools can classify synchronous handlers that combine questions and effects as `ManualPort` until their responsibilities are split.

### Security and failure handling

Admission validates authority before buffering and again against the live target at commit. Payload, fan-out, wave, queue, transaction-group, and result-retention bounds prevent a declared message path from becoming an unbounded allocation or event-storm primitive. Rejection is available only before authoritative execution begins; a runtime limit reached after direct value mutation follows the world-fault rule unless a complete accepted rollback boundary exists. Side-specific generation and notification projection prevent server-only facts from leaking through a shared event schema.

A request failure changes no state. A failed event handler or exhausted runtime event budget publishes no buffered output and follows ADRs 0026 and 0029 for already-mutated authoritative data; without a proven complete rollback boundary, the world faults rather than reporting input rejection. A failed structural preparation publishes no structural change. A post-publication notification failure cannot undo committed gameplay; it faults or retries only its declared non-authoritative consumer. Integrity-unknown commit failure faults the affected world or host and is never mislabeled as rejection or rollback.

### Operations

The world needs bounded queues, result retention, trace sampling, redaction, and backpressure policies. Commit records improve lifecycle, network, persistence, and creator diagnostics but add memory and schema cost. External effect consumers need idempotency and dead-letter policy independently of simulation commit.

## Bounded first-release scope

The proposed 1.0 scope is in-process, world-local dispatch for generated SDK messages; deterministic event waves; requests with one handler or registered reducer; buffered entity, component-structure, runtime-map, spatial-frame, standard-relation, attachment, timer, and catalog-generation commands; immutable results and notifications; and the serial-oracle comparison required by ADR 0029.

The 1.0 scope does not include a durable event store, distributed request bus, arbitrary cross-world commands, general cross-world graph transfer, semantic branch merging, postcommit world rewind, exactly-once external delivery, or a replay-file guarantee. Those capabilities may reuse committed records only after their own product and technical decisions.

## How we will prove the decision works

- Generated-manifest tests reject ambiguous request handlers, undeclared effects, side leakage, unstable identities, and unbounded payloads.
- `RequestIsPureAndLeaseBound` proves a request cannot retain aliases, publish work, or outlive its phase and produces the same value under serial and parallel execution.
- `DeterministicEventWaves` randomizes registration, worker count, task delays, partitions, and hash seeds and produces the same handler trace, buffered outputs, limits, and stable diagnostics as the serial oracle.
- A reentrancy corpus proves event publication creates a later wave, structural commands remain invisible until commit, and notifications cannot invoke same-step authority.
- `AtomicBirthChangeAndEnding` injects failure into every entity and component structural stage and exposes either the complete before-state or complete after-state with one terminal result.
- `AtomicMapAndRelationCommit` creates, relocates, and ends maps and frames while changing spatial, containment, attachment, and lifecycle relations; all stores, indexes, stale-reference tables, and replication sources agree at every observable boundary.
- Conflict fixtures submit equal, stale, unauthorized, overlapping, equivalent, and grouped commands in randomized producer schedules and receive the same accepted set and result ordering.
- Catalog-adoption fault injection proves prepare rejection and known reversible commit failure publish no success, while an integrity-unknown postcommit fault stops under ADRs 0026 and 0037 rather than claiming rewind.
- Network tests prove no preparing or incompatible-generation state reaches a client and every committed lifecycle change yields one compatible replication outcome.
- Event-storm, oversized-payload, fan-out, result-retention, and slow-notification fixtures enforce published resource and backpressure limits: provably invalid work is rejected before handlers run, while exhaustion after authoritative mutation produces the declared world fault unless a separately proven complete rollback boundary applies.
- An external station-like game and a contrasting game implement interaction chains through published messages without runtime-internal access or station-specific foundation types.

## Implementation notes

No generated message manifest, request dispatcher, event frontier, structural planner, conflict resolver, command-result store, commit record, continuation scheduler, or notification dispatcher exists. Existing project scaffolds provide no implementation evidence for this proposal.

## Dependencies and interaction with queued product decisions

Acceptance should require the already accepted access, scheduler, lifecycle, ownership, and fault contracts in ADRs 0017, 0019, 0020, 0026, 0028, and 0029. It also relies on the product meanings of maps, relations, catalog adoption, checkpoints, and editing in ADRs 0030, 0031, 0035, 0037, and 0038. Concrete spatial stores may follow later without changing the channel distinctions.

Proposed product [ADR 0039](../product/0039-inspect-running-worlds-through-authorized-snapshots.md), [ADR 0040](../product/0040-test-isolated-worlds-through-the-supported-runtime.md), and [ADR 0041](../product/0041-record-versioned-authoritative-replays-with-declared-determinism.md) may later select runtime-inspection, isolated-test, and replay or determinism outcomes. This proposal deliberately does not require their acceptance: inspection can consume immutable manifests and commit records without acquiring authority; isolated tests can drive the same dispatcher and frontiers; and a future replay format can select which admitted inputs, committed outcomes, hashes, or diagnostics it records. This ADR does not preselect those product outcomes.

ADR 0043 is complementary rather than a prerequisite: this proposal requires typed operation and schema identities, while ADR 0043 proposes their common encoding, mapping, and compatibility rules. Either proposal can be reviewed independently, but their final schemas must agree before implementation begins.

## Follow-up decisions

- Exact message, result, commit-record, and generated-manifest schemas.
- Standard structural resource keys, conflict policies, transaction-group limits, and result-retention budgets.
- Component-value visibility between scheduler batches and any snapshot or copy-on-write store mechanics.
- External effect outbox, retry, idempotency, and dead-letter policy.
- Catalog-adoption planner and client-generation admission protocol.
- Durable delayed work, cross-world transfer, and replay integration after their respective gates.

## References

- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0020](0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0026](../product/0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0030](../product/0030-define-runtime-maps-and-frame-qualified-coordinates.md)
- [ADR 0031](../product/0031-separate-spatial-containment-attachment-and-lifecycle-relations.md)
- [ADR 0035](../product/0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [ADR 0037](../product/0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [ADR 0038](../product/0038-edit-map-sources-and-preview-in-isolated-worlds.md)
