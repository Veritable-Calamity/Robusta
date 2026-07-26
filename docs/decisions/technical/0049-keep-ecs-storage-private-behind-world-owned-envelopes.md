# ADR 0049: Keep ECS storage private behind world-owned envelopes

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-24
- **Decision level:** Technical
- **Owners:** Runtime and SDK workstreams
- **Program ID:** `SIM-STORAGE`
- **Source queue IDs:** `E-STORE-01`
- **Supersedes:** None
- **Refines:** ADR 0019
- **Accepted predecessor:** ADR 0048 (`SIM-STATE`)
- **Accepted successors:** ADR 0050 (`SIM-QUERY`) and ADR 0051 (`SIM-COMMIT`); all retain separate implementation and evidence gates
- **Product decisions served:** 0002, 0003, 0006, 0011-0013, 0015, 0026, 0039-0041
- **Related decisions:** 0017, 0019, 0029, 0042-0048, 0050, 0051

## The question

Which ECS storage families will Robusta provide, what world and entity-lifecycle invariants must every family preserve, and which allocation, row, chunk, reuse, compaction, and fragmentation details must remain private so the Game SDK does not freeze one physical layout?

## The promise preserved

Game developers work with components, tags, world resources, entity references, generated queries, and committed observations rather than archetype rows, sparse indexes, chunk pointers, or allocator behavior. Every mutable ECS value belongs to exactly one live world. A stale, malformed, wrong-world, ending, or retired entity reference never aliases a replacement, and changing or compacting a private storage layout cannot change authoritative gameplay meaning.

## Why this matters

Accepted ADRs 0011-0013 make the world the mutable simulation boundary and entities the independently addressable participants inside it. ADRs 0015 and 0019 require atomic lifecycle publication, transactional structural change, and stale-safe generational entity references. ADR 0029 permits direct component-value writes only through phase-scoped leases while structural work remains buffered. ADR 0042 requires every accepted structural unit to update handle tables, component stores, and query indexes as one transaction.

A single physical ECS layout is unlikely to serve every workload well. Common, small, co-occurring component values benefit from dense columns. Rare, large, or highly optional values often benefit from sparse indexing and packed storage. Tags need membership but no value row. World resources are owned state but are not components on a magic singleton entity. Robusta therefore needs several internal storage families without making their row order, addresses, chunk capacity, relocation, or selection rules observable through the SDK.

The opposite failure is equally serious: calling every store private while allowing each implementation to invent its own handle validation, lifecycle states, structural staging, memory accounting, or fault behavior would make atomic commits and stale safety impossible to prove. This ADR must select one shared envelope and handle-table contract while retaining physical policy as measurable implementation detail.

## How the current Robusta implementation answers today

The repository contains an initial host, world, session, attachment, catalog-lease, and internal identity groundwork. It does not contain an entity reference, entity handle table, component or resource store, storage-family abstraction, structural staging area, allocator, compactor, fragmentation policy, generated query surface, or ECS benchmark corpus.

The existing ownership tests prove world separation and catalog-lease lifetime only. They make no ECS storage, stale-handle, structural atomicity, allocation, or compaction claim.

## Options considered

### Option A: World-owned hybrid private storage families behind one envelope

Each world owns one storage envelope containing a separate entity handle and generation table plus private dense-component, sparse-or-packed, tag-membership, and world-resource families. Generated schema metadata selects eligible families, while runtime policy selects and may evolve physical layouts. The SDK and downstream systems consume storage-agnostic access, query, command, and observation contracts.

This supports different workload shapes without exposing rows or pointers. One handle table centralizes lifecycle and stale validation, and one envelope gives structural commit and cleanup a coherent world boundary. It requires more internal machinery and equivalence testing than one universal store.

### Option B: One archetype-and-chunk layout for every component and tag

Every exact component set selects an archetype, and every entity occupies one row in an archetype chunk. Tags become zero-width archetype membership and resources use a separate special case.

This gives excellent dense iteration and a familiar structural-change model. Rare or large optional values, high-churn tags, and frequent composition changes can cause excessive movement and fragmentation. More importantly, exposing archetype behavior as the engine contract would constrain later optimization even where games never asked for it.

### Option C: One sparse-set store per component

Every component has an independent sparse index and packed dense value array. Queries intersect component membership.

This gives straightforward add/remove behavior and good component-local access. It gives up natural co-location for common component groups and still does not solve resources, tags, atomic cross-store publication, or handle lifecycle by itself. Adding optimized groups later would recreate Option A without having reserved the abstraction.

### Option D: Public pluggable storage providers

Let games and packages register custom stores implementing a common interface.

This maximizes experimentation but turns allocator, lifetime, aliasing, query ordering, fault containment, and serialization assumptions into a public extension boundary before Robusta can prove them. An in-process custom store could bypass world isolation or phase leases. Advanced storage adapters may be reconsidered only through the accepted extension ladder and a separately reviewed compatibility, trust, access, and fault contract.

## Decision

Robusta will use Option A: world-owned hybrid private storage families behind one storage-agnostic envelope.

The common technical contract is:

1. Every live world owns exactly one mutable ECS storage envelope. The envelope, its handle table, family instances, indexes, staging areas, allocation state, and maintenance work are world-owned capabilities. No mutable row, buffer, index, cache, resource cell, allocator, or compaction state is shared between worlds.
2. Worlds may share immutable catalog and generated schema metadata through declared leases. Sharing metadata never shares component values, resource values, entity membership, lifecycle state, dirty state, allocator state, or a mutable storage adapter.
3. The envelope exposes only internal storage-agnostic operations required by generated activation, phase access, query planning, structural commit, inspection projection, and owner cleanup. Game and package code receives no raw store, archetype, sparse index, chunk, page, row number, slot table, allocator, mutable span with an unbounded lifetime, or general storage-provider interface.
4. The initial logical storage families are:

| Family | Intended data shape | Required common behavior |
|---|---|---|
| Dense component family | Common, bounded component values that benefit from grouped columnar or chunked access | Stable logical membership and phase-scoped value access independent of row, chunk, or column layout |
| Sparse or packed component family | Rare, large, highly optional, or high-churn component values | Bounded membership lookup plus private indirection and relocation with no exposed sparse or packed index |
| Tag membership family | Entity-attached capabilities with no per-entity value payload | Presence and structural-change semantics equal to any other component capability without inventing a value row |
| World-resource family | Mutable state whose owner is the world rather than an entity, under a generated resource schema | Explicit world lifetime and access leases; never an entity component or a hidden singleton entity |

5. These are semantic family roles, not public layout names or permanent algorithms. A dense family may use chunks, pages, columns, or another measured representation; a sparse family may use sparse sets, paged indexes, hash-assisted lookup, or another bounded representation. Chunk capacity, column grouping, page size, load factor, row order, alignment, indirection depth, free-list shape, and allocator are private.
6. Accepted ADR 0048 owns component and world-resource schema meaning, identity, version, fields, defaults, bounds, side, authority, lifetime, and projection eligibility. This ADR owns physical family eligibility, layout descriptors, storage adapters, allocation, relocation, fragmentation, and the representation of internal change tracking. A storage family, layout descriptor, or optimization hint never becomes a `ComponentSchemaId` or `WorldResourceSchemaId`.
7. Until ADR 0048 is implemented through its reviewed manifest specification, only synthetic internal schemas may exercise this contract. This decision does not permit production component or resource declarations to bypass that retained gate.
8. A separate exact internal storage-layout descriptor may reference one semantic schema and record derived physical requirements and non-semantic policy inputs such as generated value size, alignment, reference-content classification, expected density class, expected mutation class, and relocatability constraints. That descriptor and its hints are not fields in the semantic state manifest, do not change schema identity, and promise no row layout, iteration order, allocation count, or family forever. A requirement that changes gameplay meaning belongs to the schema contract rather than a storage hint.
9. Runtime policy selects an eligible family from exact validated schema metadata and an exact internal storage profile. Family selection is never based on CLR reflection, type-name sorting, assembly enumeration, registration order, dictionary order, or a game-supplied arbitrary factory.
10. A family or layout may change between compatible runtime builds or through an explicitly fenced live maintenance operation only when all affected phase borrows have ended and structural publication is not in progress. Migration prepares replacement storage privately, validates complete membership and values, then swaps one envelope-internal routing binding atomically without replacing or rebinding the published world capability. Failure retains the old binding or produces integrity-unknown fault handling; it never exposes a partially migrated store.
11. Changing family, row, chunk, page, compaction schedule, allocator, or internal index is not a component add, remove, replace, lifecycle event, replication change, or game-visible value change. It must preserve every logical membership, value, handle resolution, committed structural version, authorized observation, and canonical replay projection.
12. Each world has one entity handle table separate from component and resource storage. The generated nominal `EntityRef` contains or resolves through its world scope, slot, and nonzero generation as required by ADRs 0019, 0043, and 0044. A smaller context-relative slot key may exist internally but is neither an `EntityRef` nor valid outside its already-validated world envelope.
13. Handle resolution validates nominal kind and initialization, world scope, slot bounds, nonzero generation, generation equality, lifecycle, and operation-specific access before consulting a component location. Possessing a valid `EntityRef` grants no read, write, structural, inspection, network, or durable authority.
14. The handle table is the sole authority for runtime entity slot generation and lifecycle state. Family stores cannot mint, reinterpret, revive, or independently resolve an entity identity. A store location is private mutable routing data associated with one currently matching live table entry.
15. The committed transition to `Ending` removes the entity from live discovery, invalidates ordinary family resolution, and advances the slot generation before cleanup begins, as required by ADR 0019. The slot remains ineligible for reuse until every required row and owned-resource disposition reaches its declared state. Checked generation exhaustion permanently retires the slot. Generation zero, arithmetic wrap, reset, search for an old gap, process restart reuse, or allocator compaction can never make an old reference resolve to a replacement.
16. Exact slot and generation widths remain reviewed ADR 0044 `EntityRef` declaration parameters justified by the CP03 scale and churn corpus. Exhausting the admitted slot space returns a typed capacity failure; it does not widen, truncate, wrap, or silently create a second table with ambiguous scope.
17. Reuse does not expose previous contents. In the first release, every mutable pool is owned by exactly one world envelope; reference-containing cells are cleared before a buffer can re-enter that world's private pool, and every newly published logical value is completely initialized from its schema-defined construction path. Buffers do not transfer between world pools. A future host-owned transfer pool would require a separately reviewed ownership-transfer, ADR 0046 ledger, cleanup, and fault contract. This is memory-safety and disclosure hygiene, not a promise of forensic erasure from managed or native memory.
18. Lifecycle state follows ADRs 0015 and 0019: preparing entities remain absent from ordinary discovery; only live entities participate in ordinary queries and gameplay delivery; ending first removes ordinary access; ended or released slots cannot return to live. Storage location and capacity state never become a second lifecycle truth.
19. Component add, remove, replace, entity birth, and entity ending are structural operations. They stage proposed handle-table transitions, capacity reservations, family rows, values, locations, index deltas, and cleanup dispositions inside one world-owned unpublished transaction. Ordinary phase code cannot invoke a family mutation that bypasses the ADR 0042 command and commit boundary.
20. Preparation performs every expected fallible allocation, schema validation, dependency check, relocation, acquisition, and inverse construction before publication. The publish section uses only prepared bounded operations whose failure model is declared. If an unexpected failure prevents proven reversal, world integrity becomes unknown and the applicable ADR 0046 fault profile closes or escalates the owner; partial authoritative continuation is forbidden.
21. A successful structural publication makes the handle table, every affected family, and private index routing agree at one frontier before the immutable ADR 0042 commit record becomes visible. A failed or rejected structural unit publishes none of its staged membership, lifecycle, location, or value changes. This ADR supplies storage capabilities to that transaction but does not define the command conflicts, transaction-group planning, result retention, or commit-record schemas governed by accepted ADR 0051 (`SIM-COMMIT`).
22. Direct component-value and world-resource writes are not structural when the schema permits them. They occur only through the caller's ADR 0029 phase-scoped write lease and generated access view. The store may use in-place, copy-on-write, journaled, or another private mechanism, but it cannot expand the caller's declared access or let a borrowed value escape the invocation.
23. Engine-owned values or registered immutable references follow ADR 0029 directly. A mutable reference graph, native buffer, unmanaged pointer, raw variable-sized allocation, service, task, or other external mutable representation is not placed in a component or resource family. A later accepted tracked exclusive adapter remains a separately activated ADR 0045 capability and ADR 0046 ledgered resource with declared ownership, aliasing, bounds, cleanup, fault radius, and lease behavior; calling it an adapter does not turn its state into a schema field or store row. Unknown or untracked representations are nonconforming rather than automatically placed in an exclusive store.
24. Storage enumeration order has no authoritative meaning. Row, chunk, page, sparse-index, free-list, hash, insertion, compaction, and allocation order cannot become query order, merge order, replication order, snapshot order, state-hash order, or replay order. `SIM-QUERY` must define canonical or scheduler-declared order independently of every physical family.
25. Borrowed references, spans, iterators, row tokens, and internal locations are phase-scoped and non-storable under ADR 0029. A borrow carries or is reached through the validated world, access key, schema, and scheduler-issued epoch. Ending the phase invalidates it; compaction, family migration, or structural publication cannot run concurrently with an affected live borrow.
26. Query membership indexes are private derived state. Accepted ADR 0050 (`SIM-QUERY`) owns the public query shape, canonical ordering, snapshot, change-filter, partition, allocation, and invalidation contract. This decision does not expose family indexes or assume that a query equals an archetype scan, sparse-set intersection, or tag-bitset traversal.
27. Internal change stamps, dirty bits, row versions, and relocation markers are representations rather than gameplay facts. If they later support public change filters, inspection, replication, persistence, or replay, the owning decision must define the semantic boundary, wrap behavior, false-positive policy, and canonical projection. No internal counter may silently create a false negative after wrap.
28. World resources use their own generated resource schemas and family cells. Under accepted ADR 0048, a live world has at most one admitted value for a logical resource schema, required resources prepare before world publication, and optional resource-presence changes are structural. Resources are not attached to entity slot zero, a permanent hidden entity, a global static, a host singleton, or a catalog object. Resource construction, replacement, access, observation, and cleanup use world ownership and the same phase/fault boundaries appropriate to their declared semantics.
29. Allocation uses checked arithmetic and enforces declared maxima before reserving slots, rows, pages, buffers, indexes, or staging space. Untrusted counts and schema sizes are validated before multiplication or allocation. Capacity exhaustion, invalid layout, unsupported representation, and allocation refusal produce stable typed results at a boundary that can still reject or reverse the operation.
30. Physical allocation success is never used as an ordering tie-breaker or gameplay random source. An environment-dependent inability to meet an admitted budget is an explicit resource or fault outcome, not permission to drop an arbitrary command, skip a component, partially create an entity, or publish a different row order as gameplay meaning.
31. Reuse, growth, pooling, compaction, and fragmentation policy are measurable internal policy. The implementation records bounded per-world and per-family evidence including live logical bytes, reserved bytes, allocator overhead, unused capacity, retired slots, reusable slots, relocations, growth attempts, allocation failures, maintenance work, and applicable workload-profile identity. Observational time and machine counters are diagnostics, never authoritative inputs or artifact identity.
32. Numeric capacity, fragmentation, compaction, latency, and throughput thresholds belong to a reviewed `FND-BUDGET` CP03 workload profile. This common ADR does not invent universal constants. Before that profile is approved, benchmarks characterize behavior but cannot establish a supported scale claim.
33. Maintenance is owner-admitted work. It runs only at a declared safe frontier, under a bounded world-owned budget, after affected borrows join, and without invoking arbitrary game code. It may decline or defer without changing logical semantics. A hard failure that leaves routing, membership, or values uncertain is an integrity violation rather than a recoverable optimization miss.
34. ADR 0039 inspection captures generated, authorized, redacted, immutable projections after the included value-write and structural boundaries agree. It receives no raw row, slot, pointer, padding, stale cell, allocator metadata, or mutable resource alias. Copying, pinning, page sharing, or snapshot caching remains private and cannot make a final trustworthy snapshot exist after integrity becomes unknown.
35. ADR 0040 Test SDK fixtures construct ordinary worlds and use ordinary generated schemas, activation, leases, structural commands, observations, and owner close. Test-only synthetic schemas and injected small capacities may force collision, exhaustion, allocation, relocation, and compaction paths, but tests receive no friend API that can mutate a production store around the supported boundaries.
36. ADR 0041 replay compares canonical committed authoritative projections, ordered inputs, and committed external-effect intents. Process memory, family choice, row order, chunk contents, free lists, allocator state, compaction history, and fragmentation metrics are outside replay equality. A layout change may still require an exact runtime compatibility result, but it cannot redefine a component schema or canonical state projection silently.
37. The envelope is constructed and atomically published as a generated world capability under ADR 0045. It is registered as a world-owned acquired resource in ADR 0046's ledger before world publication, admits no work after the owner fence, and reports its cleanup and leak postconditions through the reviewed owner profile. Ambient registries and post-publication capability rebinding are prohibited; clause 10's private layout migration remains internal to the same published envelope.
38. Closing a world prevents new entity, component, resource, allocation, borrow, and maintenance admission. Existing admitted phase work and structural preparation join according to the reviewed CP04 world/scheduler fault profile. Storage cannot claim successful close while owned native memory, pinned buffers, pooled references, maintenance work, or unresolved adapters remain live.
39. A violated handle-generation, membership, location, aliasing, or atomic-publication invariant affects authoritative integrity. The runtime records bounded stable evidence and applies ADRs 0026 and 0046; it does not catch the error, repair one row heuristically, and resume the world as known sound.
40. Private layout metadata is not a public ABI, serialization format, checkpoint format, network schema, replay record, component compatibility identity, or durable cache contract. A future native or advanced-extension storage adapter requires its own accepted extension, manifest, compatibility, trust, ownership, access, and fault profile and cannot weaken this envelope.

## Common ADR versus subordinate specifications

This decision selects the private family roles, one world-owned envelope, the separate handle-table authority, stale-safe reuse, structural staging duties, layout opacity, maintenance boundaries, and the fact that allocation and fragmentation are measured policy.

It deliberately leaves the following to separately reviewed work:

- Accepted ADR 0048 (`SIM-STATE`) owns component and world-resource declarations and semantic schemas.
- Accepted ADR 0050 (`SIM-QUERY`) owns public query ordering, borrowing, snapshots, change filters, partitions, invalidation, and allocation behavior.
- Accepted ADR 0051 (`SIM-COMMIT`) owns structural planning, conflicts, inverses, transaction groups, result retention, and commit records implementing ADR 0042.
- The ADR 0044 CP03 identity declaration owns exact `EntityRef` slot and generation widths and serialization permissions.
- `FND-BUDGET` owns numeric storage, churn, fragmentation, maintenance, and workload limits.
- The reviewed ADR 0046 CP02 ownership profile and later CP04 world/scheduler profile own cleanup postconditions, deadlines, integrity classification, and escalation.
- `SDK-MANIFEST` owns any package-contributed declaration envelope. No public custom storage-provider extension is implied.

## What we deliberately will not do

- Expose archetypes, chunks, sparse sets, bitsets, rows, slots, pointers, allocators, or compaction controls as ordinary Game SDK concepts.
- Require one physical store for every workload or let each family invent independent entity identity and lifecycle.
- Represent a world resource as a component on a magic entity or as ambient global mutable state.
- Treat physical row order, insertion order, hash order, memory address, allocation success, or compaction order as authoritative sequence.
- Reuse a slot after generation exhaustion, reset generations on compaction, or serialize an `EntityRef` as a network or durable identity.
- Let ordinary phase code perform structural mutation, retain a borrowed alias, or obtain a raw mutable store.
- Let games register arbitrary storage implementations in the first release.
- Define query semantics, structural conflict policy, checkpoint encoding, replication encoding, replay encoding, or public compatibility policy here.
- Claim a universal entity count, memory budget, compaction threshold, or throughput target without the versioned CP03 workload and measurements.
- Claim that this decision implements ECS storage or completes the CP03 implementation or evidence gate.

## Consequences

### Compatibility and migration

Game-facing component and resource meaning can remain stable while the runtime changes family, layout, allocator, and compaction policy. A runtime receipt may identify an exact internal layout implementation or profile where operations need it, but that identity is separate from component/resource schema identity and never creates a public row ABI.

Live family migration is optional and fenced. If it cannot prove complete atomic equivalence, the runtime retains the old layout or closes the world according to fault policy. Durable checkpoints, network replication, and replay use their own canonical projections rather than dumping private storage.

### Security and failure handling

World ownership, nominal handle validation, no-wrap retirement, bounded allocation, cleared pooled references, and non-escaping leases reduce cross-world access, stale aliasing, disclosure, and resource-exhaustion risk. They do not make trusted in-process game code a sandbox; unsafe, reflective, native, or deliberately escaping code remains nonconforming under ADR 0026.

Possessing a handle or schema identity grants no access. A malformed handle is rejected before indexing, and an internal invariant failure is treated conservatively as an integrity event.

### Operations and developer experience

Developers receive one storage-agnostic SDK even as engine implementations evolve. Diagnostics can explain capacity, churn, retirement, fragmentation, relocation, and failed structural preparation without revealing private component values or making allocator counters part of simulation semantics.

Maintaining several storage families, equivalence fixtures, and a reference model costs more than shipping one obvious layout. That cost buys room to optimize representative station-game workloads without making those optimizations permanent public contracts.

## Retained implementation gates

This acceptance authorizes only the private storage substrate described here. The complete `SIM-STATE`, `SIM-STORAGE`, `SIM-QUERY`, and `SIM-COMMIT` decision batch is accepted, but production CP03 ECS work still waits for the CP02 predecessor/evidence boundary and every retained profile, specification, and evidence gate required by the roadmap.

Acceptance does not remove these gates:

- Synthetic declarations may prove storage mechanics, but production component/resource storage waits for accepted and implemented `SIM-STATE`.
- No public query or borrow surface exists before the accepted `SIM-QUERY` contract is implemented through its reviewed declarations and adapters.
- No production structural mutation path exists before the accepted `SIM-COMMIT` contract is implemented through its reviewed planners, adapters, and result schemas.
- No supported numeric scale or fragmentation claim exists before the reviewed `FND-BUDGET` workload profile.
- World fault continuation and cleanup claims wait for the reviewed ADR 0046 CP04 and CP02 profiles respectively.
- No public storage extension, wire codec, checkpoint dump, replay encoding, or raw inspection projection is authorized.

## Bounded first implementation scope

When the retained gates permit work, the first implementation should contain:

- One internal world-owned storage envelope and one separate handle/generation/lifecycle table.
- Synthetic bounded schema fixtures for dense components, sparse-or-packed components, tags, and world resources.
- At least one deliberately simple private implementation of each family role, selected through exact internal fixture metadata rather than reflection.
- Private structural staging sufficient to prove atomic handle-table and family publication against a serial reference model, without pre-empting `SIM-COMMIT` command semantics.
- Checked slot/capacity allocation, no-wrap retirement, world-private pooled-reference clearing, a safe-frontier compaction fixture, bounded accounting snapshots, and injected failure seams.
- No public Game SDK query API, custom store interface, durable or wire encoding, or production schema activation.

## How we will prove the decision works

The core CP03 evidence is the storage reference model, stale-safety and isolation corpus, family equivalence, failure injection, allocation checks, and workload characterization below. The explicitly marked CP04 and CP12 integration qualifications validate later consumers and do not block closure of the core CP03 storage slice.

- `WrongWorldAndMalformedHandlesNeverIndexStorage` fuzzes default, malformed, truncated, wrong-world, out-of-range, stale, ending, ended, and retired references and observes typed failure before any family access.
- `GenerationExhaustionRetiresWithoutAliasing` uses tiny generated test widths to cycle every reusable slot, proves advance-before-reuse, and proves permanent retirement at exhaustion without wrap or replacement aliasing.
- `FailedBirthAndCapabilityChangePublishNothing` injects failure after each reservation, allocation, family-stage, value initialization, index-stage, and immediately pre-publication boundary; the handle table, membership, values, observations, and accounting equal the complete before-state.
- `CommitPublishesOneCompleteAfterState` varies affected family count and proves handle state, all component/tag memberships, resource state where applicable, and private routing agree before one immutable commit observation appears.
- A serial semantic reference model and each storage-family combination produce identical canonical entity membership and values over randomized births, writes, adds, removes, replacements, endings, and stale operations.
- Dense, sparse-or-packed, and tag representations can be exchanged in an internal equivalence fixture without changing logical membership, values, or the canonical committed-state projection inputs reserved for later query, inspection, persistence, replication, and replay consumers.
- Randomized compaction and relocation at admitted safe frontiers preserve every live handle and logical value, produce no lifecycle or gameplay change, and never run while an affected phase borrow is live.
- Compile-fail and runtime boundary fixtures reject retained borrows, raw stores, cross-world mutable captures, arbitrary provider registration, structure changes through value access, and reflection-based family selection.
- Two worlds using the same schemas and overlapping internal slot/generation values share no mutable storage, allocator, maintenance state, or resolvable handle; closing or compacting one leaves the other unchanged.
- Reference-containing pooled storage is cleared before reuse within its owning world, two world envelopes never exchange or concurrently own pooled buffers, and no authorized observation can read an ended row, padding, free-list content, or a previous occupant.
- Allocation and arithmetic fuzzing rejects oversized, overflowing, contradictory, or unsupported layout requests before allocation and never publishes partial membership after refusal.
- A versioned representative churn corpus records live and reserved bytes, overhead, unused capacity, retired slots, relocations, growth, allocation failures, and maintenance work for every family combination. Thresholds remain unclaimed until the reviewed `FND-BUDGET` profile.
- Later CP04 scheduler qualification: serial and parallel fixtures with identical declared partitions produce equal committed canonical state despite different row layouts, worker assignment, allocation order, and compaction schedules.
- Later CP12 consumer qualification: inspection receives only authorized immutable schema projections, Test SDK fixtures use ordinary boundaries, and replay ignores layout and allocator state while detecting any canonical committed-state difference.
- Later CP04 fault and owner-close qualification: closing and injected-fault fixtures account for family buffers, staged rows, native adapters, world-private pooled references, and maintenance work through the owner ledger; unknown postconditions cannot yield a known-clean world close.

## Implementation notes

No ECS store exists, and implementation status remains `Not started`.

Expected markers for later groundwork include:

- `// TODO(SIM-STATE): replace synthetic component and resource fixtures after accepted ADR 0048's manifest specification and generator are implemented.`
- `// TODO(SIM-STORAGE): keep row, chunk, sparse-index, allocator, and compaction types internal to the world-owned envelope.`
- `// TODO(SIM-STORAGE): implement checked no-wrap slot retirement and injected tiny-width exhaustion fixtures.`
- `// TODO(SIM-QUERY): expose only generated storage-agnostic phase queries and define canonical ordering and invalidation.`
- `// TODO(SIM-COMMIT): replace the private atomicity fixture with the accepted structural planner and commit-record path.`
- `// TODO(FND-BUDGET): approve versioned CP03 capacity, churn, fragmentation, and maintenance thresholds from measured workloads.`
- `// TODO(FND-FAULT/CP04): classify storage invariant, allocation, maintenance, and post-publication failures in the world fault profile.`

Logical folder organization is expected. A separate project is justified only by a real artifact, dependency, generation, or analyzer boundary; each storage family does not require its own project or folder.

## Dependencies and interaction with queued decisions

Accepted ADRs 0011-0013 and 0017 require each mutable envelope to belong to one explicit world scope. Accepted ADRs 0015 and 0019 supply lifecycle atomicity, the handle/generation model, and structural staging obligations. ADR 0026 defines the conforming-code boundary, while ADR 0029 defines phase leases, value access, and effect buffering.

Accepted ADR 0042 owns authoritative command and structural publication semantics. ADR 0043 keeps runtime identity separate from storage and durable identity. ADR 0044 supplies the scoped-generation declaration strategy but explicitly leaves production `EntityRef` widths and integration to CP03. ADR 0045 supplies generated world activation and capability ownership. ADR 0046 supplies admission fencing, acquired-resource accounting, integrity classification, and cleanup/fault profiles.

Accepted ADRs 0039-0041 consume only committed, authorized, canonical projections: inspection cannot expose storage, tests cannot gain private mutation authority, and replay cannot treat layout as equality.

Accepted ADR 0048 is the immediate semantic-schema predecessor. A later amendment or superseding decision that changes its contract must reconcile this storage decision before implementation continues; this ADR cannot bootstrap a production schema system independently.

Accepted ADR 0050 (`SIM-QUERY`) and accepted ADR 0051 (`SIM-COMMIT`) complete the CP03 public-access and structural-transaction design contract. Their acceptance supplies the coordinated design boundaries but no implementation evidence. `SIM-SYSTEM`, `SIM-MESSAGE`, and `SIM-WORLD-SERVICES` consume those boundaries at CP04. `SDK-MANIFEST`, persistence, networking, inspection, and replay decisions may project semantic schemas but may not serialize or depend on this private layout.

## Follow-up decisions and specifications

- Implementation of accepted ADR 0048's component and world-resource semantic manifests.
- Exact internal storage-profile and layout-descriptor schemas, eligibility validation, and receipt treatment.
- Implementation specifications and evidence for [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md) covering public ordering, borrows, snapshots, change filters, partitions, allocation, and invalidation.
- Implementation specifications and evidence for [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) covering structural planning, conflicts, inverses, transaction groups, result retention, and commit records.
- Reviewed ADR 0044 CP03 `EntityRef` declaration with evidence-backed slot and generation widths.
- CP03 storage reference model, family-equivalence corpus, allocation-failure matrix, and churn benchmark profile.
- `FND-BUDGET` CP03 numeric capacity, fragmentation, maintenance, latency, and throughput profile.
- ADR 0046 CP04 world/scheduler fault profile and storage-specific stable reason registry.
- Later persistence, replication, inspection, and replay projection specifications that never expose private layout.
- Any future advanced or native storage adapter only after the extension ladder defines its trust, ABI, access, compatibility, ownership, and containment contract.

## References

- [ADR 0011](../product/0011-define-world-as-isolated-simulation.md)
- [ADR 0012](../product/0012-separate-game-host-and-world-state.md)
- [ADR 0013](../product/0013-use-entities-for-independent-world-participants.md)
- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0017](0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
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
- [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md)
- [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md)
- [`SIM-STORAGE` program package](../../status/adr-development-program.md#ecs-scheduling-and-messages)
- [Platform development roadmap](../../status/platform-development-roadmap.md)
