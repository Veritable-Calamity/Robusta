# ADR 0048: Generate stable component and world-resource schemas

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-24
- **Decision level:** Technical
- **Owners:** Runtime and SDK workstreams
- **Program ID:** `SIM-STATE`
- **Source queue IDs:** `E-COMP-01`, `E-RESOURCE-01`
- **Supersedes:** None
- **Refines:** ADRs 0013, 0015, 0018, 0019, 0029, 0042, and 0043
- **Product decisions served:** 0002, 0003, 0005, 0006, 0011-0015, 0035-0041
- **Related decisions:** 0013, 0015, 0018-0021, 0029, 0039-0047, 0049-0051
- **Accepted CP03 companions:** [ADR 0049](0049-keep-ecs-storage-private-behind-world-owned-envelopes.md), [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md), and [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md); all four decisions were accepted independently via Option A and retain separate implementation and evidence gates

## The question

How will Robusta give components and world-owned resources stable, side-aware, authority-aware, bounded semantic schemas that ordinary game code can author ergonomically and every runtime, tool, test, inspector, persistence, replication, and replay workflow can identify without treating CLR types or physical storage as the contract?

## The promise preserved

Game developers can declare state close to the code that uses it and receive typed diagnostics and generated APIs without maintaining a second handwritten registry. The same declaration has one exact, language-neutral meaning across supported tools and platforms. Components remain capabilities of independently living entities, while world-wide state remains owned by the world rather than being disguised as a singleton entity.

The engine can change storage families, generated CLR names, internal layouts, and execution strategies without silently changing game-state meaning. Side leakage, unauthorized mutation, unbounded values, ambiguous defaults, stale references, and accidental serialization fail before a world becomes visible.

## Why this matters

ADRs 0013 and 0015 distinguish an entity's identity and lifecycle from ordinary owned data and require atomic capability changes. ADRs 0019, 0029, and 0042 require component presence, value access, structural publication, and observations to respect transactional and phase-scoped boundaries. ADR 0018 promises generated deterministic manifests through a layered public SDK. Those contracts are not implementable until "component" and "world resource" have exact declarations independent of a particular CLR class or ECS storage.

The same schemas will later be consumed by content compilation, inspection, isolated tests, replication, checkpoints, map preview, catalog adoption, and replay. If each consumer infers fields independently through reflection, attributes, field offsets, or serializer conventions, it can disagree about identity, defaults, authority, bounds, sensitivity, or compatibility. A private storage optimization could then become an accidental file or wire contract.

World-wide state also needs a first-class answer. Modeling clocks, rule state, environment state, or world indexes as immortal entities would give ordinary data an entity identity and lifecycle it does not have. Hiding the same state in ambient singletons or services would violate world isolation and phase-scoped access. A world-resource schema must therefore be distinct from both an entity component and an activated capability.

## How the current Robusta implementation answers today

The repository contains the initial host, world, session, attachment, catalog-generation, and ownership kernel. It does not yet contain an ECS, public component declarations, world-resource declarations, schema generation, component stores, resource stores, or generated state access views.

That absence is intentional groundwork, not an implicit code-first schema decision. No current CLR type, folder, assembly, reflection scan, or handwritten registry is grandfathered in as a component or world-resource contract.

## Options considered

### Option A: Typed source declarations generate one normalized semantic manifest

Authors use side-appropriate typed declarations in the published SDK. A build-time compiler validates all declarations and emits one bounded, normalized, language-neutral semantic manifest plus generated typed descriptors, construction validation, and analyzer metadata. Stable package-qualified identities and explicit schema versions, rather than CLR names, bind every consumer to the same meaning.

This gives ordinary C# authors immediate type checking and source-located diagnostics while preserving a non-CLR contract for content tools, inspectors, migration tools, and future frontends. It requires a deterministic compiler, a versioned manifest schema, and careful separation between semantic fields and later storage or codec artifacts.

### Option B: Hand-author a canonical manifest and generate all language bindings

Authors maintain a language-neutral manifest directly, and generators emit C# types and runtime descriptors from it.

This makes the semantic source obvious and serves non-C# tools naturally. It makes common game authoring noisier, separates declarations from the code that consumes them, and creates a poor incomplete-edit experience. Additional tooling would be needed to prevent generated partial types, handwritten helpers, and manifest declarations from drifting.

### Option C: Discover component and resource types at runtime

Use CLR marker interfaces or attributes, scan loaded assemblies, reflect fields, and register discovered types in a runtime ECS registry.

This is familiar and initially inexpensive. Assembly load order, CLR names, reflection shape, serializer behavior, trimming, and runtime registration become accidental schema and ordering authority. Side mistakes and duplicate meanings are detected late, non-C# tools need a second model, and an untrusted package can force reflective work before admission.

## Decision

Robusta will use Option A: typed source declarations that generate one normalized language-neutral semantic manifest.

The technical contract is:

1. The primary authoring surface is a closed set of typed component, world-resource, field, and value-schema declarations supplied by the appropriate published SDK layer. `Robusta.Game.Sdk` contains only side-neutral declaration primitives, identity-free value helpers, diagnostics, and generation hooks. Actual shared, authority-only, or presentation-only state declarations belong in `Robusta.Game.Shared`, `Robusta.Game.Server`, or `Robusta.Game.Client` according to ADR 0018; the common SDK cannot declare or expose authoritative or side-specific state by itself. The normalized language-neutral manifest emitted by the declaration compiler is the semantic artifact consumed by runtimes and tools. Generated CLR source and embedded descriptors must agree with that manifest or the build fails.
2. A declaration has a stable package-qualified logical identity and an explicit positive schema version. `ComponentSchemaId` and `WorldResourceSchemaId` are distinct nominal identity kinds using ADR 0044's `qualified-logical` strategy; neither can be converted to or compared with the other. A schema identity is not a CLR type, component instance, resource instance, catalog definition, authority, or permission.
3. The exact normalized manifest has its own domain-separated `SimulationStateManifestId` under ADR 0044's `canonical-artifact` strategy. It covers the manifest schema, every included declaration and exact version, normalized semantic fields, compiler semantic version, and declared limits. Source paths, spans, comments, whitespace, generated symbol names, assembly names, build timestamps, and diagnostic wording are separate provenance and cannot change this identity.
4. Platform schemas occupy a closed engine-owned package namespace. Game or package schemas use the publisher and package identity admitted by the eventual `SDK-MANIFEST` and package contracts. A package cannot claim a platform namespace, another publisher, or another package merely by spelling its qualified identity.
5. Component and world-resource namespaces remain nominally separate even when declarations have identical fields, local names, or generated CLR representations. Dynamic consumers validate declaration kind, package-qualified identity, version, and exact manifest context before lookup. There is no general runtime `StateType`, string-keyed component registry, or cross-kind equality path.
6. A **component** is entity-owned state describing one capability of one entity. It has no identity independent of its owning `EntityRef` and `ComponentSchemaId`. Its presence begins, changes, or ends only through the entity preparation and structural-commit rules of ADRs 0015, 0019, and 0042. A marker component with no fields is permitted, but it still has an exact schema and capability-presence meaning.
7. A **world resource** is one world-owned state slot identified by `WorldResourceSchemaId` within one `WorldInstanceId`. A live world has at most one admitted instance of a given logical resource schema. A resource receives no `EntityRef`, does not enter entity queries, cannot carry entity lifecycle observations, and is not replicated, saved, or inspected merely because it exists.
8. A world-resource lifetime is either `RequiredForOpenWorld` or `OptionalWorldState` in the initial contract. A required resource is validated and prepared before the ADR 0045 world activation plan publishes the world and remains present while the world owner remains `Open`; ordinary value writes and an explicitly admitted atomic replacement may be permitted by its schema, but removal or a replacement that creates an absent interval is invalid. Adding, replacing, or removing optional resource presence uses the admitted world structural-command boundary. All resource state becomes inaccessible when its world is fenced and leaves `Open`; it cannot transfer itself to another world or outlive that owner.
9. If state needs several independently addressable participants with their own identity and lifecycle, those participants are entities or another explicitly accepted model, not multiple hidden instances of one world resource. A bounded keyed collection may be a resource field only when its entries are ordinary value data with declared key, ordering, and bounds; the entries do not silently acquire entity semantics.
10. A schema declaration records at least its kind, package-qualified identity, version, authored SDK side, state authority, lifetime policy, generated accessibility, field set, construction policy, dependency references to value schemas, permitted projection set, sensitivity ceiling, diagnostic name, deprecation facts if any, and source provenance.
11. Every field records a stable schema-local field identity distinct from its CLR member name; semantic type and exact value-schema version; required, optional, or explicitly nullable presence; construction default or absence rule; scalar, byte, string, collection, nesting, and validation bounds; side and authority narrowing where applicable; reference kind and missing-target behavior where applicable; permitted projections; sensitivity; and source provenance.
12. The initial semantic type algebra is closed and versioned. It may contain booleans, explicitly sized integers, explicitly profiled numeric values, declared enums and flags, nominal identity values, immutable catalog references, bounded strings and bytes, bounded optional values, bounded records, and bounded sequences, sets, and maps with declared equality and canonical ordering. An unknown primitive, comparer, normalization rule, numeric profile, or collection rule is a build failure rather than a reflective extension point.
13. A value schema is acyclic by value expansion. Recursive gameplay relationships use nominal typed references with an explicit owner and scope; they do not create a recursively serialized CLR object graph. `EntityRef` remains world-local and stale-safe under ADRs 0019, 0043, and 0044, and no projection may turn it directly into a network, checkpoint, replay, document, or durable identity.
14. Authoritative parallel state is engine-owned value data or a registered immutable reference value as required by ADR 0029. Component and resource fields cannot expose arbitrary mutable objects, delegates, services, tasks, spans, raw arrays, pointers, reflection handles, process-global state, or untracked mutable reference graphs. A later accepted exclusive adapter may own mutable external state, but the adapter is an ADR 0045 capability and ADR 0046 ledgered resource, not a schema field made safe by annotation.
15. Defaults are semantic data, not CLR construction behavior. The manifest distinguishes required-without-default, explicit absence, explicit literal default, and a named deterministic construction expression from a closed versioned set. A constructor, static initializer, property initializer, `default(T)`, culture, environment value, clock, random source, filesystem value, or service lookup is never an implicit schema default.
16. A default applies only at the declaration's named construction or migration boundary. It does not silently fill an absent network, checkpoint, replay, document, or catalog field unless that surface's accepted schema and ADR 0047 profile select the same rule. Adding an optional field with a default is not automatically a compatible evolution.
17. Bounds are semantic maxima enforced before allocation or publication. Every variable-size value declares count and byte or code-unit limits, every nested value declares a depth contribution, and every schema declares aggregate field and validation-work limits. Bounds are part of the exact manifest identity. Operational budgets may be stricter, but no runtime or tool may accept a value that exceeds the semantic bound.
18. Side presence and state authority are separate facts. The initial side set distinguishes shared semantic declarations, authority-only declarations, and client-presentation-only declarations. The initial authority set distinguishes authoritative-world mutation, local-presentation mutation, and immutable-after-construction state. A field may narrow its schema's side or authority but cannot widen it. Test, creator, replay, or operator status does not create a new side or bypass the declared authority.
19. A shared declaration means that the schema may have admitted authority and client projections; it does not mean the instances are shared memory, that every field exists on both sides, or that a client may authoritatively mutate it. Client projections omit authority-only schemas and fields unless a later replication or inspection contract explicitly permits a redacted or transformed projection.
20. Projection eligibility is an allow-list over named semantic surfaces such as inspection, replication, checkpoint, replay verification, catalog construction, creator documents, and diagnostics. Absence from the allow-list means forbidden. Eligibility is necessary but not sufficient: it does not generate a serializer, activate an ADR 0044 codec surface, grant discovery or read authority, select redaction, establish compatibility, or authorize publication.
21. Each owning projection ADR or reviewed subordinate specification selects its envelope, canonical encoding, mapping rules, authorization, compatibility profile, retention, redaction, and tighter bounds. A projection cannot include a forbidden field, widen side or authority, infer a field from private storage, or reinterpret a default. The state manifest remains semantic input to that later artifact rather than becoming its wire or file format.
22. The declaration compiler validates the complete package graph before emitting a usable manifest. Duplicate or conflicting logical identities, invalid versions, undeclared packages, forbidden cross-side references, field-identity collisions, cyclic value expansion, unbounded values, unsupported defaults, illegal authority widening, unknown projections, ambiguous comparers, and unresolved schema references produce stable source-located diagnostics.
23. Manifest fragments compose in exact admitted package-dependency order and normalize by declaration kind, package-qualified identity, version, and field identity. Source-file order, project enumeration, syntax-tree order, assembly load order, reflection order, registration order, dictionary order, and operating-system path order never affect the normalized result.
24. The compiler emits immutable descriptor fragments, nominal schema constants, construction and value validators, exact field metadata, and analyzer facts. It may later emit storage and access adapter inputs, but it does not emit a general mutable component base class, service locator, runtime scanner, universal serializer, or public raw-store interface.
25. Runtime composition is explicit through ADR 0045's generated capability graph and closed activation plan. A world pins one exact admitted simulation-state manifest before preparation. Runtime code cannot add, replace, remove, or reinterpret schemas after that plan validates, and a plugin cannot make a type into a component or resource by loading an assembly.
26. Before a game-contributed state manifest or a world using it becomes visible, the containing operation authority selects the exact ADR 0047 policy, profile, state, and descriptors and receives one valid complete report admitting that publication mode. Successful manifest parsing, matching schema names, or the presence of generated CLR code is not a compatibility result.
27. Schema identity, explicit version, and exact manifest identity are separate facts. Any semantic change to an existing declaration requires a new explicit schema version, and any declaration or manifest-membership change produces a new exact manifest identity. Adding an unrelated declaration does not revise an unchanged declaration. A CLR rename, namespace move, source-file move, or generated layout change does not require a semantic version change when the explicit declaration is unchanged. Reusing a version for different semantics is an integrity failure.
28. A new field, removed field, changed default, changed bound, changed numeric or collection semantics, changed side, changed authority, changed lifetime, or changed projection eligibility has no built-in compatibility outcome. An ADR 0047 operation profile evaluates the exact source and target descriptors and any admitted migration or adapter. Version distance, SemVer syntax, successful decode, and field-name similarity never imply compatibility.
29. Component-value and resource-value access uses the non-storable generated phase views and read or write leases required by ADR 0029. Schema possession does not construct an access key or lease. A system can retain nominal identities as values where permitted, but it cannot retain a borrowed field reference, collection view, iterator, mutable alias, or raw store beyond its invocation.
30. Field-value writes permitted by a phase lease remain value writes and do not become structural commands merely because they are schema-described. Component presence changes and optional world-resource presence changes use ADR 0042 structural commands and immutable terminal results. Later `SIM-STORAGE`, `SIM-QUERY`, and `SIM-COMMIT` decisions define physical writes, change tracking, query invalidation, conflict planning, and publication without changing this distinction.
31. Authoritative schema validation and field access perform no arbitrary filesystem, network, database, environment, process, wall-clock, locale, or game callback work. External facts arrive as ordered inputs, and external effects leave as committed effect intents under ADRs 0029 and 0042. A validator is pure, bounded, deterministic for its exact inputs, and cannot publish state.
32. Inspection under ADR 0039 consumes only an authorized immutable committed projection whose fields are inspection-eligible and whose owning side, authority, sensitivity, and freshness are explicit. This ADR supplies state meaning and metadata, not an inspection query, cursor, endpoint, transport, authorization policy, or redaction profile. Private fields remain unavailable rather than being reflected from storage.
33. The Test SDK under ADR 0040 uses the same exact declarations, manifest validation, activation, authority, commands, leases, defaults, and bounds as ordinary worlds. A test may select declared adapters before activation and inspect permitted committed projections, but it cannot register test-only schemas after publication, replace field authority, or mutate a raw component or resource store.
34. Replay under ADR 0041 pins the exact simulation-state manifest and compares only replay-eligible canonical committed projections selected by the replay specification. Re-execution uses fresh runtime identities and ordinary state access. This ADR creates no replay artifact codec, runtime-memory snapshot, automatic historical migration, or guarantee for an unprojected field.
35. Content defaults and authored values compile through ADR 0021 against an exact admitted state manifest and preserve package, definition, field, and source provenance. A catalog definition and a component or resource schema remain distinct identities. Construction from a definition copies or derives validated initial values at a named boundary; later mutable world state does not mutate the immutable catalog.
36. A malformed, oversized, duplicate, incompatible, side-invalid, authority-invalid, or incomplete manifest fails before world publication with a bounded stable result and no partial schema registry. A declaration-compiler integrity contradiction, manifest-identity collision, or mismatch between generated code and the normalized manifest is an integrity failure, not a request to choose whichever representation loaded first.
37. Schema validation faults during unpublished preparation reverse through ADR 0045 and are accounted for by ADR 0046. A fault after authoritative mutation follows the applicable world-fault profile; the runtime does not continue from possibly partial state or manufacture an inspection or replay success. State data itself owns no cleanup callback, and any associated external resource remains separately ledgered.
38. Adding a declaration that uses the accepted type algebra and policies is an ordinary reviewed package change. Adding a new semantic declaration kind, type operator, default evaluator, authority class, lifetime class, projection meaning, runtime registration path, or mutable-reference exception requires an amending ADR. A subordinate specification may only instantiate and tighten this contract.

## Common ADR versus subordinate specifications

This ADR selects the distinction between components and world resources, their logical and exact identities, the required declaration facts, the closed value/default/bounds model, deterministic normalization, generation boundary, and the rule that projection eligibility never activates a codec or grants authority.

A separately reviewed `SIM-STATE` manifest specification must still define the exact manifest JSON Schema or equivalent language-neutral envelope, canonical byte encoding, identity domains, version widths, closed primitive and numeric profiles, collection ordering, diagnostic codes, semantic limits, and known-answer fixtures. It may choose representation details only within this contract.

Later decisions own separate mechanisms:

- accepted [ADR 0049](0049-keep-ecs-storage-private-behind-world-owned-envelopes.md) (`SIM-STORAGE`) owns storage families, allocation, layout, fragmentation, compaction, change-tracking representation, and physical layout descriptors;
- accepted [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md) (`SIM-QUERY`) owns entity-query order, borrow APIs, filters, partitions, invalidation, and allocation behavior;
- accepted [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) (`SIM-COMMIT`) owns structural planning, conflict policy, inverses, result retention, and commit records;
- `SIM-SYSTEM` owns system declarations, access-graph composition, scheduling, and generated system parameters;
- `NET-SCHEMA`, the persistence packages, `OBS-INSPECTION`, and `REPLAY-AUTHORITATIVE` own their respective codecs and projections; and
- `SDK-MANIFEST`, content, package, and receipt decisions own public package contribution, distribution, and exact artifact binding.

A storage layout may have its own exact receipt dimension, but field offsets, packing, archetype or sparse-set choice, page or chunk shape, compression, change-mask representation, and cache layout never become component or world-resource schema identity.

## Retained gates and authority

This acceptance opens only the bounded common `SIM-STATE` work. It does not claim implementation, complete CP03, or authorize production worlds backed by a new ECS.

The following gates remain:

- The CP03 four-decision design gate is satisfied by accepted ADRs 0048-0051, but production state still waits for the roadmap's CP02 predecessor/evidence boundary and every retained gate below.
- Public game-package declarations require `SDK-MANIFEST` and package identity/receipt contracts; until then, only engine-owned and bounded conformance fixtures may use provisional internal composition.
- External or production world publication requires the implemented ADR 0045 activation boundary and the applicable reviewed ADR 0047 profile of the containing operation, such as launch at CP07, Test SDK or ordinary test-world construction at CP12, or replay re-execution at CP12. CP03 engine-owned conformance fixtures may use an internal exact admission fixture that publishes no external launch, test, replay, or compatibility promise.
- Operational release requires measured workloads and the applicable `FND-BUDGET` and ADR 0046 world-fault profiles.
- Network, checkpoint, replay-artifact, document, catalog, receipt, and public diagnostic codecs remain forbidden until their owning decisions and reviewed ADR 0044 surface declarations activate them.
- Inspection, Test SDK, and replay product outcomes remain governed by accepted ADRs 0039-0041 and still require their queued technical ADRs and subordinate profiles.
- Public UGC, trusted extensions, native adapters, and compatibility support windows remain governed by their own trust, extension, budget, and evolution decisions.

No schema, manifest, descriptor, field identity, generated type, projection eligibility, compatibility report, or diagnostic token grants authority. Authority comes from the current owner-issued activation binding, phase lease, admitted command, or separately authenticated operation.

## What we deliberately will not do

- Treat every piece of world state, service, manager, subsystem, tile, or collection entry as an entity.
- Model world resources as immortal or hidden singleton entities.
- Give a component instance an identity separate from its owning entity and schema.
- Infer schema identity or version from a CLR namespace, type name, generic argument, assembly, file, attribute order, or field offset.
- Discover or register schemas by scanning loaded assemblies at runtime.
- Expose arbitrary mutable classes, service objects, delegates, tasks, raw arrays, pointers, or object graphs as conforming component or resource fields.
- Select archetype, sparse-set, table, page, chunk, structure-of-arrays, or another storage layout here.
- Define query syntax, structural planner internals, scheduler graph construction, or change-tracking representation here.
- Generate network, checkpoint, replay, document, receipt, or general-purpose serializers from field presence alone.
- Treat projection eligibility, successful decoding, identity knowledge, or a schema match as authorization or compatibility.
- Infer compatible evolution from SemVer, field addition, a default value, CLR assignability, or successful deserialization.
- Snapshot process memory or private storage as canonical inspection, persistence, or replay state.
- Let test, creator, operator, or replay code replace schemas or authority after world publication.

## Consequences

### Developer experience and migration

Ordinary authors declare state in typed SDK code and receive source-located generator and analyzer diagnostics. They do not maintain a runtime registration list. Explicit schema and field identities add some ceremony, but they make CLR renames and physical reorganizations safe and make migration intent reviewable.

Existing future prototypes must bind to exact schema fields rather than setters discovered by convention. Handwritten components introduced during scaffolding must migrate to declarations before they become supported API. There is currently no production ECS data requiring an in-place migration.

### Compatibility

Exact state manifests become separate compatibility facts under ADRs 0043 and 0047. Logical identity supports continuity, explicit versions describe revisions, and exact manifest identity detects semantic drift; none alone decides whether an operation may proceed.

Storage-layout changes may remain compatible when every semantic projection is unchanged, while a default, bound, side, authority, or projection change may be incompatible even if the CLR layout is identical. Persistence, networking, replay, inspection, and catalog adoption each evaluate only their declared dimensions and migrations.

### Security and failure handling

Build-time validation, side-specific projections, default-deny projection eligibility, hard semantic bounds, and the absence of runtime scanning reduce side leakage, arbitrary allocation, type-confusion, and reflective code-loading risks. They do not replace package trust, signature verification, authentication, authorization, process isolation, or resource budgets.

Unknown declaration semantics fail closed before publication. Runtime state remains owner-scoped, and a failed mutation follows the accepted transactional or fault contract rather than leaving a partially registered schema or partially visible capability.

### Operations and performance

Operators can correlate worlds, receipts, catalogs, and failures to exact state-manifest identities and stable schema diagnostics without relying on assembly versions or filenames. Useful metrics include declaration and field counts, normalized manifest bytes, generator and validation work, side-projection size, rejection reasons, schema cache occupancy, construction validation failures, and per-schema live counts where disclosure permits.

Build-time generation removes reflection scanning from startup and gives storage work exact bounds, but it does not promise a particular memory footprint or iteration rate. Storage and query workloads must measure those properties independently. Diagnostic cardinality and field values remain bounded and redacted under their owning profiles.

## Bounded first implementation scope

When the retained gates permit work, the first implementation slice is limited to:

- the common component, world-resource, field, and value-schema declaration model;
- engine-owned qualified logical schema identities and one exact manifest-artifact identity profile;
- a deterministic declaration compiler with source-located diagnostics;
- a bounded normalized language-neutral manifest and immutable descriptor reader;
- generated nominal schema constants, validators, construction helpers, and analyzer facts;
- side-projection and projection-eligibility validation without external codecs;
- one marker component, one bounded value component, one required world resource, and one optional world resource as synthetic conformance fixtures; and
- exact known-answer, negative, fuzz, engine-owned synthetic package-graph, and cross-platform normalization tests.

This slice would not include a production entity store, resource store, public query API, scheduler integration, structural planner, content binding, network schema, save format, replay artifact, inspection endpoint, Test SDK release, runtime hot reload, public package contribution, or Supported compatibility promise.

## How we will prove the decision works

Core CP03 evidence covers deterministic generation, semantic validation, state/resource invariants, integration with the other CP03 contracts, synthetic compatibility truth tables, adversarial bounds, storage equivalence, and workload characterization. The explicitly marked CP04, CP06, and CP12 qualifications validate later integrations and do not block closure of the core CP03 state-schema slice.

- Windows and Linux builds of the same declarations produce byte-identical normalized semantic manifests, exact manifest identities, generated descriptor facts, and stable diagnostics.
- Reordering files, syntax trees, declarations, attributes, or members; changing machine paths, namespaces, generated CLR names, assembly names, comments, locale, and build time does not change the semantic manifest.
- Changing a field identity, type, default, bound, side, authority, lifetime, projection eligibility, value-schema version, or compiler semantic version changes the exact manifest identity.
- Two engine-owned conformance package namespaces may use the same CLR type or local display name without collision, while duplicate package-qualified identities or reused versions with different semantics fail before emission; later CP06 external-package qualification repeats the namespace proof through `SDK-MANIFEST`.
- Compile-time and dynamic fixtures prove that component and world-resource identities cannot substitute for one another and that neither substitutes for a catalog definition, entity, message, or capability identity.
- Negative fixtures reject cyclic value expansion, unbounded collections, ambiguous comparers, implicit CLR defaults, mutable reference graphs, forbidden side references, authority widening, unsupported projections, malformed identities, and unknown semantic operators.
- Entity birth and component add, replace, remove, and failed preparation expose only complete old or new capability sets through the ADR 0042 frontier.
- Required resources exist before world publication and remain continuously present while the world owner is `Open`; permitted value writes or atomic replacement never create an absent interval. Optional resource presence changes atomically. Neither kind receives an `EntityRef` or appears in an entity query, and all resource access ends with the owning world.
- Analyzer and runtime fixtures reject phase-borrow escape, retained mutable aliases, access without a scheduler-issued lease, and schema possession used as authority.
- Later CP04 scheduler qualification: serial and parallel executions over the same exact state manifest produce equal permitted committed projections under the ADR 0029 oracle while varying worker count and incidental scheduling.
- Later CP12 inspection qualification: an inspection fixture exposes only authorized inspection-eligible committed fields with exact provenance and returns unavailable, omitted, redacted, or denied for other fields without reflecting private storage.
- Later CP06/CP12 SDK qualification: an external Test SDK fixture builds and runs against published packages using the ordinary manifest and activation path and cannot register or mutate through friend or internal access.
- Later CP12 replay qualification: a replay fixture pins the exact manifest, uses fresh runtime identities, compares only replay-eligible projections, and cannot invoke a real external sink or treat process-memory layout as state equivalence.
- Compatibility truth tables vary field addition, removal, default, bound, authority, side, and projection facts independently and prove that no change receives an inferred compatibility result outside the selected ADR 0047 profile.
- Adversarial manifest fixtures enforce byte, declaration, field, collection, nesting, reference, validation-work, and diagnostic bounds before allocation or world publication.
- Two conforming storage prototypes can use different private layouts while producing the same semantic manifest and committed projections; their separate layout facts may differ without redefining schema identity.
- Versioned workload evidence records generation time, manifest size, startup validation, construction validation, memory overhead, and access cost without promoting unmeasured targets into a release claim.

## Implementation notes

Implementation status remains `Not started`. No component or world-resource declaration API should be treated as implemented or stable before the retained manifest specification is reviewed, approved, implemented, and evidenced.

Expected markers include:

- `// TODO(SIM-STATE): replace provisional state markers with generated nominal schema declarations.`
- `// TODO(SIM-STATE/SCHEMA): define canonical manifest bytes, semantic limits, diagnostics, and known-answer fixtures.`
- `// TODO(SDK-MANIFEST): bind public package identities and state-manifest fragments into external SDK artifacts.`
- `// TODO(SIM-STORAGE): bind semantic schemas to private physical layouts without changing schema identity.`
- `// TODO(SIM-QUERY): issue non-storable typed access views and define invalidation semantics.`
- `// TODO(SIM-COMMIT): implement atomic component and optional-resource presence changes through structural commands.`
- `// TODO(OBS-INSPECTION): activate authorized committed inspection projections for eligible fields.`
- `// TODO(NET-SCHEMA/DATA-STATE/REPLAY-AUTHORITATIVE): define separate bounded codecs, mappings, and compatibility profiles.`

Logical folder organization is expected. Separate projects are justified only by real published-artifact, dependency, trust, generator-hosting, or runtime boundaries; this ADR does not require one project per schema family.

## Dependencies and interaction with queued decisions

ADRs 0013 and 0015 own the entity-versus-data distinction and observable lifecycle. ADRs 0019, 0029, and 0042 own stale-safe handles, structural publication, phase access, buffered work, and terminal results. ADR 0018 owns the layered public SDK; ADR 0021 owns canonical content and provenance.

ADRs 0043 and 0044 own nominal identity, exact artifact identity, non-substitution, and codec activation. ADR 0045 owns manifest composition into closed activation plans. ADR 0046 owns close, cleanup, and integrity outcomes. ADR 0047 owns operation-specific compatibility and publication admission.

Accepted ADRs 0039-0041 require inspection, Test SDK, and replay consumers to use these exact state semantics without granting this decision authority over their endpoints, artifacts, profiles, or retention. Their technical packages may proceed in design, but they cannot invent competing component or world-resource meaning.

Accepted [ADR 0049](0049-keep-ecs-storage-private-behind-world-owned-envelopes.md), [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md), and [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) are coordinated CP03 decisions with this ADR as their semantic predecessor. Their independent acceptance settles the four-decision design batch without lending implementation evidence between packages. CP03 production implementation still waits for the CP02 predecessor/evidence boundary and each decision's retained specifications, profiles, and evidence.

## Follow-up decisions and specifications

- Versioned language-neutral simulation-state manifest schema and canonical encoding.
- Qualified component, world-resource, field, value-schema, and exact manifest identity declarations under ADR 0044.
- Closed primitive, numeric, enum, reference, collection, ordering, default, and validation profiles.
- Generator API, analyzer rules, diagnostic catalog, fragment-composition rules, and external-package conformance corpus.
- Side-projection, sensitivity, and projection-eligibility specification.
- State-manifest dimensions and known-answer cases for the applicable containing-operation profiles under ADR 0047, including launch, Test SDK or ordinary test-world construction, and replay re-execution; this ADR creates no generic profile that bypasses their checkpoint gates.
- `SIM-STORAGE`, `SIM-QUERY`, `SIM-COMMIT`, and `SIM-SYSTEM`.
- `SDK-MANIFEST` package contribution and exact receipt integration.
- Content-to-state construction binding and catalog provenance rules.
- Separate inspection, network, checkpoint, creator-document, replay, and diagnostic projection specifications.
- Migration rules and tools for changing schema identity, version, fields, defaults, bounds, authority, side, or projection eligibility.
- Workload-calibrated semantic and operational limits under `FND-BUDGET`.

## References

- [ADR 0013](../product/0013-use-entities-for-independent-world-participants.md)
- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0018](0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0020](0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0021](0021-compile-content-into-a-canonical-provenance-catalog.md)
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
- [ADR 0049](0049-keep-ecs-storage-private-behind-world-owned-envelopes.md)
- [ADR 0050](0050-generate-phase-scoped-queries-with-canonical-iteration.md)
- [ADR 0051](0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md)
- [`SIM-STATE` program package](../../status/adr-development-program.md#ecs-scheduling-and-messages)
- [Platform development roadmap](../../status/platform-development-roadmap.md)
- [Technical evaluation workloads](../../specifications/technical-evaluation-workloads.md)
