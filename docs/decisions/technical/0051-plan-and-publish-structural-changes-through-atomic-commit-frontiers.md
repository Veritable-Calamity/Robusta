# ADR 0051: Plan and publish structural changes through atomic commit frontiers

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-24
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Program ID:** `SIM-COMMIT`
- **Source queue IDs:** `E-STRUCT-01`
- **Supersedes:** None
- **Refines:** ADRs 0019, 0029, and 0042
- **Accepted predecessors:** ADR 0048 (`SIM-STATE`), ADR 0049 (`SIM-STORAGE`), and ADR 0050 (`SIM-QUERY`); all retain separate implementation and evidence gates
- **Product decisions served:** 0002, 0003, 0005, 0006, 0011-0016, 0026, 0030-0032, 0035, 0037-0041
- **Related decisions:** 0017, 0020, 0028, 0043-0050

## The question

How will the runtime turn deterministically merged structural commands into one complete entity, component, world-resource, map, frame, relation, timer-ownership, query-index, identity-table, and replication-source outcome without exposing partial state or pretending arbitrary gameplay writes can be rolled back?

## The promise preserved

A structural command has one stable outcome. Every observer sees either the complete committed before-state or the complete committed after-state, every affected store and index agrees at publication, and a failure is reported as rejection, proven pre-publication reversal, or an integrity-affecting fault rather than false success.

## Why this needs a separate mechanism decision

ADRs 0019 and 0042 already accept staged structural changes, deterministic command ordering, bounded transaction groups, immutable terminal results, and one commit frontier. They deliberately do not select the planner representation, structural resource-key model, prepare/apply journal, inverse requirements, publication token, or validation rules that make those semantics executable across storage families.

That gap cannot be filled independently by each component store. Entity birth touches a generation table, component storage, query indexes, lifecycle records, and replication sources. Map or frame ending additionally touches spatial and relation indexes. Catalog adoption and transfer preparation add fencing and external coordination. If each subsystem applies its part and reports its own success, the world can expose an entity that exists in one index but not another.

The mechanism also must not overclaim. Direct component-value writes performed under ADR 0029 leases are not automatically journaled structural work. A commit planner may prove reversal for its own unpublished allocations and structural deltas; it cannot promise whole-step rewind, undo an already emitted real side effect, or recover a world whose storage integrity is unknown.

## Options considered

### Option A: Deterministic prepared plans with a bounded apply journal and one publication gate

Normalize merged commands into typed plan fragments with explicit structural resource keys, dependencies, preconditions, write sets, acquisitions, inverses, and terminal-result rules. Validate and prepare all accepted units while unpublished. Apply them under one exclusive structural frontier, record each completed internal disposition in a bounded journal, and publish one new structural version only after every affected store and index agrees.

This fits the semantics already accepted by ADR 0042, allows private storage families, and gives fault injection an exact oracle. It requires closed plan schemas, careful inverse design, and a world-integrity fault when reversal or final agreement cannot be proven.

### Option B: Copy the entire structural world and swap one complete image

Build every frontier in a complete shadow copy and atomically replace the current image.

This makes publication simple in principle, but it turns maps, component stores, native resources, query indexes, timers, and large worlds into one expensive copy boundary. Sharing and external-resource ownership still require journals or reference accounting, so a complete image does not eliminate the hard transaction work.

### Option C: Apply each command directly to its target stores in canonical order

Lock the world, update each store, and stop on the first failure.

This is compact and may work in a prototype. It cannot distinguish a known reversible preparation failure from a partial integrity loss, and every new index silently expands the set of mutations that must agree. Canonical order alone does not make a partially applied command atomic.

## Decision

Robusta will use Option A: deterministic prepared plans with a bounded apply journal and one publication gate.

Accepted ADRs 0019, 0029, and 0042 control the product-visible structural semantics. This decision chooses only the internal planner, journal, and publication mechanism needed to implement them. It does not reopen message kinds, scheduler phases, entity lifecycle, identity rules, or fault outcomes.

The mechanism contract is:

1. A **structural plan fragment** is an immutable typed description of one proposed change against one exact committed structural version. It names the command and optional transaction-group identity, schema, origin merge key, authority decision, targets, preconditions, structural resource keys, dependencies, write set, prepared acquisitions, inverse requirements, result mapping, and bounded diagnostic provenance. It contains no executable game callback.
2. Structural resource keys come from a closed generated registry. At minimum the registry can distinguish entity slot and lifecycle, component presence or structural replacement, world-resource presence or replacement, runtime map, spatial frame, each relation kind, timer ownership, query-index membership, identity-table entry, replication-source entry, and catalog-generation pin. A key is conflict metadata, not a public identity, handle, permission, or storage address.
3. Each command schema declares the planner that may emit its fragment and the resource-key shapes it may claim. Planner registration order, reflection discovery, runtime type names, hash enumeration, worker identity, and task completion order do not select a planner or order fragments.
4. The frontier begins only after every authoritative producer in the preceding systems-and-events frontier has joined. Command buffers merge through ADRs 0029 and 0042 using stable phase, producer, partition, local-sequence, schema, and target keys. The commit coordinator never observes or publishes a partial producer buffer.
5. Planning reads one immutable committed before-view plus the projected results of earlier accepted plans in canonical order. It does not mutate live storage. A stale target, failed authority check, failed precondition, missing required schema, invalid group, or declared deterministic conflict produces the command's typed noncommit result before apply.
6. Conflict detection uses the complete declared structural read and write sets. Overlapping writes or a write that invalidates another plan's required read are resolved only by the command schema's accepted ADR 0042 policy: reject the later ordered claimant, coalesce provably equivalent intent, or invoke one versioned pure deterministic resolver. There is no implicit last-writer-wins rule.
7. One command is the default transaction unit. A bounded transaction group is declared before merge, has a stable group schema, fixed maximum membership and resource-key count, one dependency graph, one group-outcome policy, and a deterministic mapping from that outcome to every member command's required terminal result. Dynamic enrollment, unbounded graph expansion, and joining a group from inside planning or apply are rejected. No member can commit, reject, reverse, or fault independently of the group's all-members outcome, but every member retains its own operation identity and final result as required by ADR 0042.
8. Accepted units prepare every fallible allocation, capacity reservation, identity-table change, relationship disposition, timer transfer, catalog lease, external owned resource, index delta, and required inverse before publication. Preparation occurs under the narrowest owner and acquisition ledger that can prove cleanup. A prepared resource is not visible to ordinary lookup, queries, replication, inspection, or gameplay.
9. Every plan declares its **reversal class**:
   - `DiscardPrepared` means no live mutation occurred and prepared resources can be released;
   - `JournaledInverse` means every apply mutation has a prevalidated inverse and postcondition;
   - `InfallibleAfterPrepare` means the applicable storage contract proves the bounded apply sequence cannot fail once preparation succeeds.
   An unknown, undeclared, or conditionally fallible apply path is not admitted as a reversible plan.
10. Apply runs under the world's exclusive structural-commit lease. No system, query, inspector, replication producer, checkpoint capture, owner-close finalizer, or game callback may enter the affected world stores until the frontier either publishes or terminates through its fault path. Waiting is bounded by the applicable workload and fault profiles.
11. The coordinator applies only closed typed store operations. It records each completed internal mutation, acquisition disposition, inverse availability, and postcondition in a bounded apply journal. The journal is world-internal transaction state; it is not a durable event log, replay artifact, checkpoint, editor history, or public SDK surface.
12. Apply order is a versioned algorithm over transaction dependencies, structural domain, canonical resource keys, operation schema, and origin order. It is independent of physical row, chunk, sparse-set, hash-table, allocation, registration, and worker order. Storage families may optimize internally only when they preserve the same committed result and terminal-result ordering.
13. Storage, handle and generation tables, component-presence tables, world-resource families, maps, frames, typed relations, timer ownership, query indexes, catalog pins, and replication-source state all update behind the same unpublished gate. A subsystem cannot publish its own version or notification early.
14. A frontier that commits at least one structural unit performs a final agreement check across every touched store and index, seals every acquisition-ledger disposition, assigns exactly one new monotonic structural version, and atomically opens that version for observation. That opening is the publication point. A rejection-only frontier, or one whose accepted intents are proven semantic no-ops, changes no store, retains the prior structural version, and publishes only terminal results plus a frontier record whose before and after versions are equal; it does not invalidate queries as though structure changed. The structural version never wraps or resets within a world incarnation; exhaustion closes admission and requires a fresh world.
15. Each processed frontier creates one immutable frontier record and exactly one immutable terminal result for every command. A transaction group additionally creates one immutable group outcome; every member result names that group, preserves the member's own operation and schema identities, and carries a terminal disposition consistent with the one group outcome. No member result may be omitted or replaced by a group-only result. Records name before and after structural versions, exact schemas and catalog context, accepted and rejected units, stable reasons, bounded provenance, and any contained pre-publication reversals. Notifications, inspection, Test SDK assertions, persistence capture, replication, and replay verification consume these committed records or later projections rather than internal apply callbacks.
16. While the world is open, a world-owned operation-result store owns terminal results and indexes them by the non-reused world-local operation identity. The submitting result capability, a continuation registered before the applicable boundary, or a separately authorized diagnostic projection may retrieve a result; retrieval, notification, or continuation delivery never consumes or changes it. The full immutable payload is retained under reviewed numeric count, byte, and horizon limits. After payload expiry, a bounded tombstone makes result lookup return the typed `ResultExpired` outcome without reconstructing or replacing the operation's terminal result. After the separately bounded tombstone horizon, lookup returns `UnknownOrNoLongerRetained`. Expiry never changes the completed operation, reuses its identity, authorizes automatic retry, or turns a new submission into the old operation.
17. Owner close leaves no unresolved command result. If close wins before frontier admission, each command receives its declared closed-owner result; if bounded apply has begun, close joins its committed, reversed, or integrity-unknown terminal outcome. The result store completes registered waiters before disposal. Sealed immutable results and tombstones may outlive the world only when transferred through the ADR 0046 acquisition ledger to a named host or supervisor retention owner with authorization, bounds, and a proven terminal disposition; otherwise they expire with the store. If world integrity cannot prove its own result storage, the supervisor-owned close/fault record supplies only the unambiguous terminal `IntegrityUnknown` facts and never fabricates commit or rejection.
18. If preparation or apply fails before publication and every completed mutation has a proven inverse, the coordinator reverses in strict dependency-safe order, verifies the exact before-version postconditions, closes prepared acquisitions, and publishes only the typed `ReversedBeforePublication` or other declared noncommit results. It does not publish a transient after-state.
19. If an inverse fails, a postcondition is unknown, an unjournaled mutation is detected, agreement cannot be proven, or publication status is ambiguous, the frontier produces an `IntegrityUnknown` terminal result for every affected command and invokes ADR 0046 integrity and fault handling through the applicable separately reviewed and approved world-fault profile. The world never reopens, retries the frontier, or reports ordinary command rejection from that state.
20. Owner closing and structural commit race through one declared fence. If owner closing wins before the frontier begins, uncommitted commands terminate with the declared closed-owner result. If an admitted frontier has entered its noninterruptible bounded apply region, close waits for its published or integrity-fault terminal outcome and then continues through the same cached close operation. Cancellation by a caller never abandons the owner-owned transaction.
21. Direct component-value writes from ADR 0029 are outside this structural journal unless a later accepted value-store contract explicitly proves complete rollback. A failed parallel batch with possibly applied value writes follows ADR 0029's integrity rule even if all structural plan fragments were still unpublished.
22. Planning and apply perform no arbitrary filesystem, network, database, process, environment, wall-clock, random, script, reflection, native, or game-code work. External facts enter as ordered inputs; external outputs begin only from committed effect intents after publication.
23. Checkpoint, catalog-adoption, map-document, cross-world transfer, and replay workflows may coordinate around a world-local structural frontier, but they remain separate transactions. They cannot expand a transaction group across worlds or reinterpret one world-local commit record as global success.
24. Exact plan-fragment, resource-key, conflict-policy, result, frontier-record, and structural-version schemas are runtime-receipt inputs. A semantic change to planning, conflicts, apply order, reversal, agreement, publication, or result-retention lifecycle requires an explicit compatibility outcome. Private storage-layout changes do not require a schema change when all observable semantics and evidence remain equal.
25. Diagnostics expose bounded counts and durations for merge, planning, conflicts, preparation, apply, reversal, agreement, publication, result payloads, tombstones, expiry, and fault escalation. Identity and provenance use ADR 0044 projections; possession of a record or operation identity grants no lookup or mutation authority.
26. The first CP03 implementation is in-process and world-local. It covers entity birth and ending, component-presence and structural-replacement changes, world-resource presence and replacement, query-index membership, and the entity identity and generation tables required by that slice. Runtime maps and frames, standard relations, timer ownership, replication-source state, and catalog deltas join the same envelope only after their owning CP04, CP05, CP08, CP11, or CP12 decisions are accepted and their store adapters and agreement postconditions are reviewed. Cross-world atomic commit, distributed consensus, durable event sourcing, semantic undo, and arbitrary value rollback remain outside this mechanism.

## Authority and retained implementation gates

This acceptance authorizes the `SIM-COMMIT` planner, journal, and publication direction. It does not implement a component schema, choose an ECS storage family, publish query APIs, activate a durable commit-record codec, approve numeric transaction limits, claim rollback of arbitrary component values, or pull later map, relation, timer, replication, catalog, persistence, or replay mechanisms into CP03.

The four-decision CP03 design gate is satisfied by accepted `SIM-STATE`, `SIM-STORAGE`, `SIM-QUERY`, and `SIM-COMMIT`. Production implementation remains gated on the CP02 predecessor/evidence boundary; implementation of the accepted ownership, identity, activation, fault, and compatibility contracts; the reviewed CP02 cleanup/fault profile before its governed close path; and the applicable CP04 world-fault and workload profiles before production scheduler integration.

## What we deliberately will not do

- Expose storage rows, chunk addresses, slot arithmetic, or internal resource keys through the Game SDK.
- Let a planner or resolver execute game callbacks or discover writes dynamically.
- Publish one store or index before all affected state agrees.
- Treat canonical application order as a substitute for reversal and integrity handling.
- Promise whole-step rollback, component-value rollback, real-effect rollback, postcommit rewind, or distributed atomicity.
- Turn the apply journal or notification stream into a checkpoint or replay artifact.
- Retry an integrity-unknown frontier or reopen its world.

## Consequences

### Compatibility and migration

Command, plan, conflict, result, and commit-record schemas become exact runtime compatibility inputs. A storage-layout optimization remains private when it produces identical plans, outcomes, publication versions, and diagnostics. Changes to conflict or publication meaning need explicit compatibility treatment rather than a silent runtime upgrade.

### Security and failure handling

Closed plan operations and complete write sets prevent a command from smuggling reflective access or undeclared side effects into commit. Bounds on commands, groups, keys, dependencies, journal entries, result retention, and diagnostics prevent adversarial structural work from becoming unbounded allocation. Authority is checked before planning and again against the exact target state used by the plan.

Known reversible failure remains nonpublication. Unknown integrity is contained through ADR 0046 rather than mislabeled as rejection. Sensitive identities and provenance are projected only through declared redaction surfaces.

### Operations

The planner creates actionable conflict, reversal, and integrity diagnostics but adds memory and commit-frontier work. Operators need profile-governed budgets and alerts for queue age, plan size, preparation and apply time, reversal, retained results, and containment loss. Numeric budgets must come from measured versioned workloads, not this ADR.

## How we will prove the decision works

Core CP03 evidence covers planning, conflict, preparation, apply/reversal, agreement, publication/no-op behavior, per-command and group results, retention lifecycle, close races, and the core stores named in clause 26. The explicitly marked CP04, CP06, CP11, and CP12 qualifications validate later integrations and do not block closure of the core CP03 commit slice.

- `AtomicBirthChangeAndEnding` injects failure after every prepared and applied entity/component-presence mutation and exposes either the complete before-version or after-version with exactly one terminal result for every command.
- CP03 entity lifecycle, component-presence and structural-replacement, world-resource, query-index, and entity-generation fixtures prove every affected core store and stale-reference table agrees at publication. Later map, frame, relation, timer, catalog, and replication fixtures repeat the same proof when their separately governed adapters are introduced.
- A core synthetic merge harness permutes complete producer-buffer arrival, hash enumeration, allocation, physical-row layout, and synthetic completion order while retaining the same declared origin keys; it yields the same accepted plan set, conflict outcomes, apply trace, structural version, records, and serial-oracle state.
- Group fixtures cover invalid members, cycles, oversized groups, overlapping keys, equivalent coalescing, registered resolvers, and dependency-safe reversal without partial publication; each group has one group outcome and every member receives exactly one consistent correlated terminal result.
- Rejection-only and semantic-no-op frontiers retain the prior structural version, publish equal before/after versions and all required terminal results, and do not trigger structural query invalidation.
- Result-lifecycle fixtures prove authorized repeated retrieval, continuation delivery without consumption, payload expiry to a `ResultExpired` lookup outcome, tombstone expiry to `UnknownOrNoLongerRetained`, non-reuse of operation identities, no automatic retry, and ledgered or discarded close disposition without retaining a live world resolver.
- Fault injection proves `DiscardPrepared`, `JournaledInverse`, and `InfallibleAfterPrepare` behavior independently. A missing inverse, failed postcondition, ambiguous publication, or injected storage corruption closes or escalates the world and never returns ordinary rejection.
- Closing-race fixtures prove pre-frontier close rejects the work, an admitted bounded apply reaches one terminal outcome before teardown continues, every waiter receives the same close report, and caller cancellation cannot cancel the owner transaction.
- Later CP04 scheduler qualification varies production worker counts, producer scheduling, task delays, partition assignments, and completion order while preserving the same plan set, results, records, and structural publication.
- Later CP11/CP12 consumer qualification: inspection, Test SDK, checkpoint, replication, and replay consumers observe only immutable committed records and cannot access the internal plan or apply journal.
- Later CP06 external-SDK qualification: station-like and contrasting games perform representative structural interactions through published commands without private storage, planner, or callback access.
- Versioned workload fixtures measure planning, apply, reversal, allocation, and result-retention cost before any numeric budget is approved.

## Implementation notes

No structural planner, resource-key registry, prepared-plan representation, apply journal, agreement validator, structural-version publisher, or integrated terminal-result store exists. ADR 0042 defines the semantic contract but is also unimplemented. Implementation status remains `Not started`.

Expected markers for later groundwork include:

- `// TODO(SIM-COMMIT): replace synthetic plan fragments with generated closed command planners and resource keys.`
- `// TODO(SIM-COMMIT): prove prepare, bounded apply, reversal, agreement, and one structural publication point under injected failure.`
- `// TODO(SIM-QUERY): bind committed query-index deltas and observation invalidation to the published structural version.`
- `// TODO(FND-BUDGET): approve command, group, key, dependency, journal, full-result, tombstone, and frontier-work limits and horizons from measured CP03 workloads.`
- `// TODO(FND-FAULT/CP04): classify integrity-unknown commit outcomes in the reviewed world-fault profile.`

## Follow-up decisions and specifications

- Exact structural resource-key and plan-fragment schemas.
- Planner and resolver generation from message, state, lifecycle, relation, map, timer, and catalog declarations.
- Private store-operation adapters and agreement postconditions under `SIM-STORAGE`.
- Query-index delta and invalidation rules under `SIM-QUERY`.
- Transaction-group, journal, full-result/tombstone horizon and capacity, and diagnostic numeric bounds under `FND-BUDGET`; the result lifecycle itself is selected here.
- CP04 world-fault classification and scheduler-frontier integration.
- Catalog-adoption, checkpoint, transfer, inspection, Test SDK, and replay projections of committed records.

## References

- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0020](0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0037](../product/0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [ADR 0039](../product/0039-inspect-running-worlds-through-authorized-snapshots.md)
- [ADR 0040](../product/0040-test-isolated-worlds-through-the-supported-runtime.md)
- [ADR 0041](../product/0041-record-versioned-authoritative-replays-with-declared-determinism.md)
- [ADR 0042](0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043](0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](0044-generate-bounded-identity-declarations.md)
- [ADR 0045](0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [ADR 0046](0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047](0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [ADR 0048](0048-generate-stable-component-and-world-resource-schemas.md)
- [ADR 0049](0049-keep-ecs-storage-private-behind-world-owned-envelopes.md)
- [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md)
- [`SIM-COMMIT` program package](../../status/adr-development-program.md#ecs-scheduling-and-messages)
- [Platform development roadmap](../../status/platform-development-roadmap.md)
