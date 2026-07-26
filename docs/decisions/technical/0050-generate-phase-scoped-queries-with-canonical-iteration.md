# ADR 0050: Generate phase-scoped queries with canonical iteration

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-24
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Program ID:** `SIM-QUERY`
- **Source queue IDs:** `E-QUERY-01`
- **Supersedes:** None
- **Refines:** ADRs 0019 and 0029
- **Accepted predecessors:** ADR 0048 (`SIM-STATE`) and ADR 0049 (`SIM-STORAGE`)
- **Accepted successor:** ADR 0051 (`SIM-COMMIT`); all retain separate implementation and evidence gates
- **Product decisions served:** 0002, 0003, 0006, 0011-0013, 0015, 0016, 0039-0041
- **Related decisions:** 0017, 0018, 0020, 0026, 0042-0049, 0051

## The question

How will systems and supported tools select entity components and world resources, borrow them safely for one phase, observe changes, divide work, and obtain bounded immutable observations while keeping authoritative order independent of storage layout and worker timing?

## The promise preserved

Game code receives typed, efficient queries whose required and optional values have one explicit meaning. A query cannot leak mutable storage or outlive its authority, and the same committed world produces the same logical iteration and buffered-output order regardless of archetype layout, hash order, worker assignment, or task completion timing.

## Why this needs a separate mechanism decision

ADR 0019 keeps component storage private and requires structural changes to appear atomically. ADR 0029 requires generated phase access manifests, non-escapable borrows, canonical query order, scheduler-issued partitions, and serial-oracle agreement. Those decisions intentionally do not choose the query declaration model, row API, optional-value behavior, change baseline, snapshot boundary, allocation contract, or invalidation mechanism.

Those details cannot be allowed to emerge accidentally from the first storage implementation. Iterating an archetype's physical rows, a sparse set's dense array, or a dictionary is fast, but it makes compaction, allocation, and hash behavior visible as gameplay order. Returning ordinary references or lazy enumerables lets component access escape the phase that authorized it. Treating an absent optional component as `default(T)` confuses absence with a valid value. Treating every convenient copy as a snapshot creates an unbounded persistence and inspection surface.

The query contract must therefore be specific enough for generators, analyzers, the scheduler, storage adapters, inspection, and the Test SDK to implement one meaning, while leaving the physical ECS family private to accepted `SIM-STORAGE`.

## How the current Robusta implementation answers today

The greenfield runtime has no component or world-resource declaration generator, phase query API, access-manifest integration, canonical entity iterator, partition planner, change-stamp model, bounded observation builder, or lease invalidation machinery. Existing ownership and identity scaffolding is not ECS query implementation evidence.

## Options considered

### Option A: Generated typed phase views over a canonical logical query contract

Generate a closed query descriptor and typed row or resource view from declared component and world-resource schemas. Bind each query to one ADR 0029 phase lease and one exact world view. Enumerate through a versioned canonical logical order, let the scheduler project that order into ordered disjoint partitions, and require every storage family to adapt to the same semantics.

This makes access visible to build tooling, keeps borrows non-escapable, allows private storage optimization, and gives serial and parallel execution one oracle. It adds generator and analyzer work, logical-order indexes or merge cost, explicit invalidation, and bounded scratch planning.

### Option B: Expose storage-native iterators behind typed generic helpers

Give each archetype, sparse set, or other store typed iterators and rely on callers not to depend on their order or lifetime.

This is simpler and may benchmark well initially. Physical movement, compaction, free-list reuse, hash seeding, and a later storage-family replacement can change authoritative results. Ordinary generic enumerators also cannot by themselves prove that aliases and lazy callbacks do not escape their phase.

### Option C: Materialize every query as an immutable snapshot

Copy all matching rows before invoking a system and merge writes afterward.

This gives strong lifetime isolation but turns ordinary ECS access into allocation, copy, conflict, and merge work. It changes aliasing and write semantics, creates pressure to treat snapshots as persistence, and duplicates the transaction machinery needed for structural changes.

### Option D: Permit runtime-built dynamic predicates with optional sorting

Allow systems to construct component predicates at runtime and request a sort when deterministic order matters.

This is flexible for tools, but a runtime predicate hides access from ADR 0029 manifests, and optional sorting makes determinism a caller convention. Reflection and dynamic component selection also weaken side, authority, compatibility, and resource bounds.

## Decision

Robusta will use Option A: generated typed phase views over a canonical logical query contract.

Accepted ADR 0048 (`SIM-STATE`) supplies the state-schema contract and accepted ADR 0049 (`SIM-STORAGE`) supplies the private storage-adapter contract. This decision does not select a physical storage family. Accepted ADR 0051 (`SIM-COMMIT`) remains responsible for structural planning and publication.

The technical contract is:

1. A **query declaration** is a versioned, package-qualified semantic description compiled from the supported SDK declaration surface. Its logical `QuerySchemaId` uses ADR 0044's `qualified-logical` strategy and is distinct from component, resource, system, and message identities. Its exact normalized `QueryDescriptorId` uses ADR 0044's `canonical-artifact` strategy over the query version, exact state-manifest context, entity selector, required, optional, and excluded terms, read or write modes, change predicates, ordering algorithm, partition eligibility, side and authority restrictions, bounded observation permissions, compiler semantic version, and diagnostic projection. Neither identity activates a codec, grants authority, or makes a query executable without a current lease. CLR reflection order, generic-instantiation order, registration order, filenames, physical store names, and display names are not query identity.
2. The state declarations selected by `SIM-STATE` remain the authority for component and world-resource schema identity, version, side, lifetime, mutability, snapshot eligibility, and compatibility. The query generator may consume those descriptors but cannot invent an undeclared component, reinterpret a resource as a component, weaken a side restriction, or activate a codec.
3. Generation emits a nominal typed query view, row shape, optional-value shape, immutable normalized descriptor fragment and exact `QueryDescriptorId`, ADR 0029 access-manifest fragment, analyzer metadata, and storage-adapter requirements. Generated code and normalized descriptor facts must agree or activation fails. It emits no public raw store, untyped component dictionary, reflection query builder, mutable world reference, or general service resolver.
4. Every system query and world-resource parameter contributes its complete access keys to the ADR 0029 system manifest. A required or optional read contributes read access; any possible yielded write contributes write access even when no matching row exists at runtime. Exclusions and change predicates narrow membership but do not weaken the declared access. Unknown or dynamically selected access makes the system `ExclusiveWorld` or invalid according to ADR 0029; it never silently receives parallel eligibility.
5. A query opens only from the non-storable phase context supplied to one admitted system invocation. Its lease binds the exact host and world incarnation, owner-generation token, phase and invocation identity, system manifest, scheduler partition if any, committed structural version, and storage-view generation. Possessing a generated query type without that lease grants no access.
6. Query enumerators, row views, component borrows, mutable writers, optional borrows, resource views, partition views, and internal scratch spans are non-escapable scoped values. They cannot be stored in ordinary fields, returned, boxed, captured by an escaping delegate, placed in static or thread-local state, used by background work, or invoked after the phase. The implementation uses compiler-enforced scoped or `ref struct` shapes where possible, analyzers at supported-code boundaries, and generation-token checks in instrumented runtime profiles.
7. An `EntityRef` obtained as an owned value may outlive the query, but it carries no component alias. Resolving it later requires a new current lease, fresh lifecycle and world validation, and the required or optional failure behavior from ADR 0019.
8. Query membership is evaluated against one complete live structural view. A row exists only for a `Live` entity that contains every required component, contains no excluded component, satisfies every declared change predicate, and passes the declaration's side and authority constraints. Buffered structural commands do not add, remove, or replace query membership before accepted ADR 0051's publication frontier.
9. A required component term yields a borrow only when its exact declared schema is present in the bound structural view. Failure caused by an invalid declaration, incompatible schema, corrupt index, or impossible generated row is a typed integrity fault rather than a skipped row or `default(T)`.
10. An optional component term yields an explicit generated `Present` or `Absent` view. `Absent` is distinct from a present component whose value equals `default(T)`. An optional write may mutate an already present component under its write lease; it cannot construct an absent component. Addition, removal, replacement, and schema migration remain structural commands.
11. Duplicate or aliased terms normalize to one strongest access only when the generator can prove they name the same schema and view. Contradictory requirements, overlapping mutable aliases, an optional and excluded occurrence of the same schema, or two wrappers that obscure the same write are rejected at generation or activation.
12. World resources are resolved through generated phase-scoped resource views rather than entity iteration. A required resource is prepared before world publication under accepted ADR 0048; finding it absent from a live admitted world is a typed activation or integrity failure and no system callback runs. An optional resource yields explicit absence. Resource construction, replacement, and disposal remain owner or structural work, and a resource borrow follows the same non-escape and access-manifest rules as a component borrow.
13. Every query declaration selects a stable **canonical-order algorithm identity and version**. The first-release base profile orders matching entities by an opaque world-local logical entity-order key assigned or resolved deterministically at structural commit. That key is internal ordering metadata: it is not an SDK-visible slot, durable identity, network identity, authority token, or game-sort field.
14. Canonical query order is independent of component-store family, chunk or row address, sparse-set density, hash seed or enumeration, free-list traversal, compaction, cache migration, registration, worker number, thread identity, task completion, and wall clock. A storage adapter may scan, merge, index, or cache however it chooses, but it must produce the same logical row sequence and invalidation behavior.
15. Canonical order is not a promise that two separately created worlds assign equal entity-order keys. Cross-run equality comes only from the exact admitted inputs, compatibility domain, fixed partition scheme, and other replay conditions accepted by ADR 0041. Game code needing a domain-specific order declares and uses a separately generated bounded sort or index; it cannot infer gameplay meaning from the opaque base order.
16. Parallel iteration begins with a scheduler-issued partition plan created before worker dispatch from the exact bound query view and a versioned partition algorithm. Partitions are disjoint ordered ranges or another proven-disjoint projection of the canonical sequence. Each partition has a stable scheduler identity and canonical local order; concatenating partitions in scheduler-issued order reproduces the query's canonical global order.
17. A system cannot choose a worker, manufacture a partition identity, or claim disjointness from an arbitrary predicate. Buffered output and deterministic reductions carry the scheduler's partition order plus the row's canonical contribution position as required by ADR 0029. Completion order never affects merge. Any partition-profile change that can alter authoritative meaning is an exact compatibility and receipt change; otherwise it must prove serial-oracle equality across the permitted profiles.
18. A **change baseline** is an opaque typed value bound to one world incarnation, state-schema generation, component schema and version, change-stamp algorithm, and completed phase or committed frontier. It is not a step number, wall-clock value, entity identity, checkpoint cursor, or portable token. A baseline from another world, schema, algorithm, future frontier, expired retention window, or invalid owner fails with a stable typed result.
19. A component instance receives a non-wrapping logical change stamp when it becomes live or when a successful phase finalizes a declared mutable borrow according to the generated write-tracking profile. Because unrestricted by-reference struct mutation cannot always distinguish an equal rewrite from no mutation, the first-release conservative profile treats issuance and successful finalization of a mutable borrow as a write exposure. `ChangedSince` therefore means "created or exposed for write after this baseline," not "its serialized value is unequal." Exact semantic diffs require a separately declared comparer or change event.
20. Change stamps are assigned or finalized in canonical system, partition, and row order after the relevant work joins, never in worker-completion order. Stamp exhaustion closes admission or faults the affected world under the applicable separately reviewed and approved fault profile; it never wraps. Addition is visible as change when live; removal is observed through committed structural records rather than a row that no longer exists. Optional absence does not fabricate a component change.
21. Change-history retention is bounded by a reviewed workload profile. The runtime may compact history only after the oldest supported baseline. A query with an expired baseline returns `BaselineExpired` or the equivalent stable typed outcome; it never silently broadens to "all rows," narrows to "none," or guesses from current values.
22. Ordinary query opening and iteration allocate no managed object per row, perform no reflection, and do not grow an unbounded collection. The generated descriptor declares its maximum terms, adapters, join/index requirements, partition metadata, and scratch class. Any bounded scratch or index lease is acquired before user code observes the query; inability to satisfy its admitted budget produces a typed pre-invocation failure or the governing world fault, not a partially enumerated success.
23. An explicit **bounded immutable observation** API may copy selected query values out of a phase when every selected state declaration permits observation. Its request declares maximum rows, bytes, nesting, strings, and diagnostic disclosure before capture. The result owns canonical immutable values, records the source world and exact structural, phase, catalog, schema, and compatibility context, and carries no mutable alias, resolver, callback, lazy iterator, or authority.
24. An observation that exceeds its declared bound, encounters a forbidden or noncopyable field, loses its admitted owner/view fence, or cannot complete one consistent source view fails atomically with a stable typed result. It publishes no truncated value unless the declaration explicitly defines a deterministic pagination contract and continuation token.
25. A bounded immutable observation or query snapshot is not a checkpoint, save, replay artifact, map source, creator document, network replication frame, transaction journal, or promise that its source world can be reconstructed. It receives no restore operation. Persistence, replay, inspection export, and Test SDK serialization require their own accepted schemas, compatibility policy, authority, redaction, and retention.
26. A completed owned observation remains a value after its phase lease ends and may remain after its source owner closes according to its own bounded retention. Live enumerators, borrows, resource views, partition views, lazy builders, and unfinished capture operations do not. This distinction prevents owner closure from invalidating already copied diagnostics while preventing a copy wrapper from preserving live authority.
27. Phase completion invalidates every live query lease and borrow. A structural publication that commits at least one structural change and assigns a new structural or storage-view generation invalidates all views bound to the prior generation before the next phase opens. A rejection-only or semantic no-op frontier that retains the same structural version under accepted ADR 0051 does not create an extra structural invalidation, although the ordinary phase-completion boundary still invalidates phase-scoped views. Owner closing stops new query admission; already admitted phase work follows ADR 0046's bounded join and fault profile, after which its owner-generation token is terminally invalid. Use after any boundary is rejected in instrumented runtime profiles and remains prohibited even if a physical address happens not to have moved.
28. Structural publication and query execution are mutually fenced. An implementation must not repair an invalid iterator by skipping a moved row, restarting against a newer version, or retaining an old chunk. A version mismatch during an admitted query is an integrity violation; an expected caller-side stale view receives a typed stale result before user iteration.
29. `QuerySchemaId`, `QueryDescriptorId`, ordering, partition, change-stamp, optional-value, observation, and invalidation descriptors are exact runtime-receipt and compatibility inputs under ADRs 0043, 0044, and 0047. Changing physical layout or index strategy is private only while canonical membership, order, visibility, allocation bounds, failure results, and evidence remain equal. A semantic change requires a new query-schema version and exact descriptor identity plus an explicit operation-specific compatibility outcome.
30. Diagnostics expose only an ADR 0044-permitted exact, nominal, or redacted query-identity projection plus bounded source system and phase, required and optional term counts, row count, partition count, scratch class, baseline classification, stale or invalid-use outcome, observation truncation policy, and duration. Component values, entity identities, world identities, and provenance follow their own declared redaction projections. Diagnostics grant no query or resolution authority.
31. The first implementation is in-process and world-local. It covers generated entity-component queries, generated world-resource views, required, optional, and excluded terms, read and write borrows, conservative change filters, scheduler-issued ordered partitions, bounded immutable observations, and the indexes needed by the first-release ECS scenarios. Dynamic user-defined predicates, cross-world joins, distributed query execution, general reactive query graphs, durable snapshot restore, and arbitrary runtime sorting are outside this mechanism.

## Authority and retained implementation gates

This acceptance authorizes the `SIM-QUERY` semantic direction: generated typed phase views, manifest-derived access, canonical logical iteration, scheduler-issued ordered partitions, conservative versioned change filtering, explicit bounded observations, and invalidation at phase, publication, and owner boundaries.

It does not select an archetype or sparse-set layout, implement accepted ADRs 0048, 0049, or 0051, publish a Test SDK inspection format, activate a checkpoint or replay codec, approve numeric workload limits, or claim that query infrastructure exists.

Production implementation remains gated on:

- implementation of accepted `SIM-STATE` and `SIM-STORAGE` through compatible reviewed schema and storage-adapter contracts;
- implementation of accepted `SIM-COMMIT` before structural-index publication and invalidation are integrated;
- the accepted CP02 ownership, identity, activation, fault, and compatibility contracts, including the reviewed cleanup/fault profile for owner close;
- reviewed ADR 0044 `QuerySchemaId` and `QueryDescriptorId` declarations; package-contributed or public query declarations additionally wait for `SDK-MANIFEST`, while CP03 may use only engine-owned internal conformance fixtures;
- `SIM-SYSTEM` before generated queries become public system parameters or production scheduler graph inputs;
- the reviewed ADR 0046 CP04 world/scheduler fault profile before production scheduler integration, stamp-exhaustion handling, or continuation after query-related faults;
- a reviewed `FND-BUDGET` profile before numeric row, scratch, partition, change-history, and observation limits become production guarantees; and
- applicable inspection, Test SDK, checkpoint, network, and replay ADRs before any query observation is exposed on those surfaces.

## What we deliberately will not do

- Expose archetypes, chunks, sparse sets, dense rows, hash buckets, slot arithmetic, or component addresses as Game SDK query semantics.
- Treat storage iteration, allocation order, worker assignment, or completion timing as authoritative order.
- Return ordinary mutable references, `IEnumerable` instances, callbacks, or lazy iterators that can escape a phase.
- Represent optional absence as a default component value or let an optional write add a component.
- Infer exact value inequality from an unrestricted mutable borrow.
- Silently restart a query after structural publication or owner closure.
- Let a snapshot become a checkpoint, restore source, durable replay, or authority-bearing world handle.
- Allow a dynamic reflection query to bypass generated access manifests, side rules, or compatibility review.
- Promise zero scratch work, one permanent index strategy, cross-world joins, or allocation-free bounded snapshot capture.

## Consequences

### Compatibility and migration

Generated query declarations become part of the supported SDK and exact runtime receipt. Existing ECS code that depends on archetype order, raw component references, cached enumerators, runtime-built reflection predicates, or absent-as-default behavior requires an explicit migration. Storage implementations remain replaceable when they preserve the declared query semantics and evidence.

Changing a query's terms, access mode, canonical-order algorithm, partition semantics, change meaning, or observation schema is a compatibility event. Pure physical layout and indexing changes remain internal when serial-oracle, allocation, invalidation, and workload evidence are unchanged.

### Security and failure handling

Generated manifests and scoped borrows prevent a query from expanding its authority at runtime. Required and optional semantics avoid default-value confusion, while side and observation declarations prevent server-only or sensitive state from leaking through a convenient snapshot. Bounds on terms, joins, scratch, partitions, history, rows, bytes, nesting, and diagnostics prevent query and observation paths from becoming unbounded allocation or exfiltration primitives.

Invalid lease use, impossible generated rows, and mid-query structural-version changes are integrity signals. Expected stale callers, expired baselines, absent optional values, and pre-invocation resource limits use typed nonfatal outcomes. A fault after direct authoritative mutation follows ADRs 0026 and 0029 rather than pretending the query can roll back the world.

### Operations and performance

Canonical logical order may require maintained indexes, ordered merges, or bounded scratch that a storage-native scan would avoid. That cost is deliberate and must be measured against representative station-like and contrasting workloads. The private storage layer may specialize common query shapes, cache stable joins, and choose layout adaptively, but optimizations must retain the same row sequence, failure behavior, and invalidation rules.

Change filters reduce ordinary scanning only when retention and index cost justify them; they are not a substitute for committed lifecycle records. Bounded observations create explicit copy and retention cost, which must appear in workload metrics rather than being hidden inside inspection or tests.

## How we will prove the decision works

Core CP03 evidence covers exact engine-owned query identities and descriptors, access-manifest generation, borrow non-escape, required/optional semantics, canonical logical iteration against the storage reference model, conservative changes, structural invalidation, bounded observations, and allocation behavior. The explicitly marked CP04, CP06, and later-consumer qualifications do not block closure of the core CP03 query slice.

- `GeneratedQueryMatchesAccessManifest` proves each required, optional, excluded, read, write, resource, change, and partition declaration generates the exact ADR 0029 access keys and rejects hidden or contradictory aliases.
- `BorrowCannotEscapePhase` combines compile-fail, analyzer, and instrumented runtime fixtures for fields, boxing, escaping delegates, statics, thread-locals, background work, phase completion, structural publication, and owner closure.
- `RequiredOptionalAndDefaultAreDistinct` fuzzes missing, present-default, present-nondefault, added, removed, replaced, stale, wrong-world, wrong-side, and incompatible-schema cases without fabricating a row or alias.
- `CanonicalQueryOrderIgnoresStorageLayout` randomizes storage family, chunking, compaction, sparse density, hash seeds, allocation addresses, and registration, then feeds the same declared partitions through a synthetic CP03 executor with permuted completion order; it produces the same logical row and buffered-effect trace as the serial oracle.
- Core partition-contract evidence uses synthetic scheduler-issued plans to prove disjointness and exact recomposition of the canonical global sequence without completion-order influence; later CP04 scheduler qualification repeats that proof through production system batches and worker assignments.
- `ChangeBaselineIsVersionedAndBounded` covers creation, conservative write exposure, equal rewrites, optional absence, removal, schema changes, cross-world tokens, expiry, future baselines, stamp exhaustion, and synthetic partition-completion permutations.
- Later CP04 scheduler qualification varies production worker counts, partition-to-worker assignments, task delays, and completion order while preserving the same canonical rows, merged effects, and change-baseline outcomes.
- `StructuralPublicationInvalidatesPriorViews` injects publication and close races at every legal boundary and proves no stale borrow, iterator, resource view, partition, or unfinished capture can resolve against the new generation.
- `BoundedObservationIsNotCheckpoint` proves atomic row, byte, nesting, and disclosure rejection; canonical copy order; no mutable alias or resolver; validity of completed owned values after source close; and absence of any restore or world-activation path.
- Allocation benchmarks prove no managed per-row allocation in ordinary iteration, bounded pre-acquired scratch, deterministic pre-invocation failure, and separately measured observation-copy cost.
- Structural commit fixtures prove query indexes and storage membership expose one complete before-view or after-view and never an intermediate combination.
- Later CP06 external-SDK qualification: a station-like game and a contrasting game implement representative component and resource access through generated public queries without runtime-internal storage types or station-specific query primitives.

Evidence for these claims is tracked under `E-QUERY-01`. Until the core generated identities, declarations, analyzers, runtime checks, and randomized storage-reference corpus exist, implementation status remains `Not started` and the core CP03 evidence gate is not passing. CP04 scheduler integration, CP06 external-SDK use, CP12 inspection/Test SDK/replay consumers, and numeric supported-scale claims retain their own later evidence gates.

## Bounded first-release scope

The first-release query scope is generated, in-process, world-local access over one live structural view. It includes required, optional, and excluded component terms; generated world-resource views; read and mutable borrows; one versioned canonical base order; scheduler-issued disjoint ordered partitions; conservative bounded change baselines; and explicit bounded immutable observations for accepted consumers.

It excludes cross-world joins, arbitrary user comparers, reflection-built production queries, durable reactive subscriptions, general incremental dataflow, automatic semantic diffs, snapshot restore, distributed execution, and a public storage-layout contract.

## Implementation notes

No implementation is present; acceptance authorizes only bounded work under the retained gates. Likely implementation shapes include generated `ref struct` query and row views, scoped references, immutable descriptor tables composed at activation, analyzer-enforced escape rules, runtime generation tokens in diagnostic profiles, and storage-family adapters that expose logical order without revealing physical layout. These are implementation hypotheses, not evidence.

## Follow-up decisions and specifications

- `// TODO(SIM-QUERY):` Specify the canonical entity-order key and first-release algorithm after `SIM-STATE`, `SIM-STORAGE`, and `SIM-COMMIT` settle entity-table and publication details.
- `// TODO(FND-IDENTITY/SIM-QUERY):` Define `QuerySchemaId` and `QueryDescriptorId` profiles, normalized descriptor bytes, diagnostic projections, and default-deny codec permissions under ADR 0044.
- `// TODO(SIM-QUERY):` Define the authored query declaration grammar, generated C# shapes, diagnostics, and supported language-feature baseline.
- `// TODO(SDK-MANIFEST/SIM-SYSTEM):` Bind package-contributed query declarations and generated system parameters only after their owning decisions are accepted.
- `// TODO(SIM-QUERY):` Define exact optional borrow, resource admission, stale-view, baseline-expiry, and observation-limit result types.
- `// TODO(SIM-QUERY):` Select the conservative write-tracking implementation and prove how phase faults affect unfinalized change stamps.
- `// TODO(FND-BUDGET):` Approve numeric limits for query terms, joins, scratch, partitions, change-history retention, observations, and diagnostic sampling from versioned workloads.
- `// TODO(OBS-INSPECTION):` Define which committed query projections may enter authorized inspection and how pagination, consistency, redaction, and cost are reported.
- `// TODO(TEST-RUNTIME):` Define the Test SDK's supported query assertions and bounded observation helpers without exposing runtime internals.
- `// TODO(REPLAY-AUTHORITATIVE):` Bind query, order, partition, and change-stamp descriptor identities into the exact replay compatibility domain.

## Dependencies and interaction with other decisions

Accepted ADRs 0019 and 0029 supply the entity lifecycle, structural-view, lease, manifest, effect-buffer, and serial-oracle semantics this decision refines. ADRs 0020 and 0042 supply scheduler and frontier ordering; ADRs 0043, 0044, and 0047 supply typed identities, generated descriptors, and dimensional compatibility policy. None of those accepted decisions proves this query mechanism is implemented.

Accepted ADR 0048 defines the component and world-resource schema metadata consumed by generation. Accepted ADR 0049 defines private storage envelopes and adapter obligations capable of serving this logical contract. Accepted ADR 0051 defines atomic query-index publication and the structural-version fence. Their dependency order remains 0048, 0049, 0050, then 0051, while implementation and evidence remain independently gated.

Accepted ADR 0039 may consume only separately authorized bounded committed observations. Accepted ADR 0040 may drive the same generated query and resource paths through the published Test SDK. Accepted ADR 0041 may rely on the exact query, order, partition, and change semantics within its admitted replay domain. None gains a private store API, snapshot restore path, or broader authority from this decision.

## References

- [ADR 0013](../product/0013-use-entities-for-independent-world-participants.md)
- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0018](0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0020](0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0026](../product/0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md)
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
- [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md)
- [`SIM-QUERY` program package](../../status/adr-development-program.md#ecs-scheduling-and-messages)
- [Platform development roadmap](../../status/platform-development-roadmap.md)
