# ADR 0044: Generate bounded identity declarations and per-kind profiles

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-23
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Program ID:** `FND-IDENTITY`
- **Source queue IDs:** `F-ID-01`, `F-ID-02`
- **Supersedes:** None
- **Refines:** ADR 0043
- **Product decisions served:** 0002-0006, 0008, 0011, 0012, 0015, 0027, 0030-0032, 0035-0041
- **Related decisions:** 0017-0019, 0021-0023, 0028, 0042, 0043

## The question

How will one versioned identity-kind declaration and its generated strategy profiles produce the nominal, scoped, bounded, and redaction-aware identity values required by ADR 0043, while allowing each later subsystem to choose its own canonical payload and retention details?

## The promise preserved

Developers and operators receive concrete identity types that cannot be substituted accidentally, expose only their permitted representations, fail safely when stale or malformed, and preserve useful redacted correlation. Adding an identity kind does not require handwritten equality, parsing, formatting, or registration code, and it does not create a universal identifier or grant authority.

## Why this matters

ADR 0043 accepts nominal generated identities, explicit scope and issuer semantics, bounded codecs, non-wrapping generation, domain-separated artifact identity, default-deny serialization, and redaction-aware diagnostics. It intentionally leaves the declaration schema, generation toolchain, allocation profiles, exact failure behavior, and generated API shape open.

If those details remain handwritten, two superficially similar structs can disagree about default values, scope binding, collision behavior, formatting, codec availability, or redaction. A later network, checkpoint, document, or package codec could then serialize a runtime-only value merely because its backing primitive happens to be convenient. Conversely, one universal allocator or representation would hide the real differences between a process incarnation, a world-local generational handle, a canonical artifact digest, a package-qualified definition, and an issuer-owned durable identity.

This decision must make identity generation actionable without selecting the content digest algorithm, entity-store layout, network encoding, checkpoint format, document format, package grammar, durable issuer repository, or purpose-bound mapping retention owned by later decisions.

## How Robust Toolbox answers today

Robust Toolbox uses compact subsystem-specific identities such as `EntityUid`. The pinned implementation is useful for local lookup and cache-friendly storage, but the current Robusta audit records that `EntityUid` is a compact local integer rather than a generation-safe public handle. Other identity distinctions are enforced through their individual APIs and conventions rather than one generated declaration and codec policy.

The lesson is to preserve compact, purpose-specific values while making scope, stale safety, serialization, and substitution mechanically enforceable.

## How the current Robusta implementation answers today

The ownership kernel has internal handwritten `HostInstanceId`, `WorldInstanceId`, `SessionId`, and `SessionWorldAttachmentId` structs backed by private non-empty `Guid` values. They expose typed equality, no parser or raw-value property, and a nonrevealing `ToString`. Focused tests prove that runtime scopes receive fresh kind-specific values and reject uninitialized identities at important boundaries.

`CatalogGenerationId` is also nominal, but its random `NewPlaceholder` factory is deliberately test scaffolding rather than the canonical artifact identity required by ADRs 0021 and 0043. There is no identity declaration schema, generator, allocation injection, collision/exhaustion fixture, generated descriptor, surface codec, redaction formatter, or per-kind conformance matrix. Default validation is repeated manually and is not uniform across kinds.

## Options considered

### Option A: Canonical semantic manifest with an incremental generator

Make a versioned, language-neutral identity manifest the semantic source of truth. Validate it against a published schema, normalize fragments by stable identity-kind schema, and let an incremental build generator emit concrete nominal value types, immutable metadata fragments, kind-specific issuer hooks, only the active permitted codecs, and redaction-aware diagnostic projections.

Allocation and representation use a closed set of named strategy profiles. Later subsystem ADRs instantiate those profiles and provide their own canonical payload, encoding, retention, and compatibility parameters without changing the common identity semantics.

This keeps CLR names from becoming identity, lets non-C# tools consume the same declaration, gives exact artifacts one reviewable input, and makes forbidden codecs absent from generated APIs. It adds a schema, generator, deterministic-output burden, and a small amount of manifest authoring beside ordinary code.

### Option B: Attributed C# marker declarations as the semantic source of truth

Declare identity kinds through attributes or partial marker types and use a Roslyn generator to emit the concrete values and an exported descriptor table.

This gives excellent authored-source diagnostics and a concise .NET workflow. It makes CLR syntax and Roslyn compilation the primary semantic authority for identities also consumed by content tools, package tooling, inspectors, migration utilities, and future non-C# frontends. Keeping the exported descriptor canonical and available before compilation becomes an additional synchronization problem.

### Option C: Canonical manifest compiled by a standalone pre-build tool

Keep the language-neutral manifest but invoke a dedicated CLI or MSBuild task that writes generated source and descriptor artifacts before compilation.

This can serve several languages and make generated files directly inspectable. It adds a separately supervised build process, worsens incomplete-edit IDE behavior, creates stale generated-file hazards, and duplicates incremental dependency tracking that the C# compiler already provides for the current platform.

## Decision

Robusta will use Option A: a canonical semantic manifest with an incremental generator.

The technical contract is:

1. A versioned semantic identity manifest is the source of truth for platform identity kinds. Its authoring form is validated structured data, initially JSON with a repository-owned JSON Schema. CLR namespaces, type names, assembly names, filenames, paths, registration order, and reflection discovery are not identity-kind schema.
2. A build may contain several logical manifest fragments. The generator normalizes them by stable package-qualified kind schema, validates the complete referenced scope and issuer graph, and rejects duplicate, ambiguous, incomplete, cyclic, or unauthorized declarations with source-located diagnostics before emitting a usable type.
3. Each declaration records at least the stable kind schema and version; generated type and accessibility; scope kind and embedded scope-token representation where the kind is scoped; issuer kind; value strategy and parameters; equality and optional ordering policy; initialization/default policy; maximum in-memory and encoded sizes; reuse and exhaustion policy; forbidden, reserved, and active serialization surfaces; diagnostic name; redaction class; tombstone owner; and the compatibility identity of every generated representation.
4. Platform kinds are closed to the platform manifest. A game or package may later contribute package-qualified identity domains only through the published SDK manifest and the restrictions already accepted by ADR 0043. A contribution cannot redefine a platform kind, claim another issuer, weaken a scope, or select an undeclared codec.
5. An incremental generator emits one concrete `readonly` nominal value type per declaration, immutable descriptor metadata, kind-specific construction or issuer hooks, equality and hashing over every identity-bearing field, optional declared ordering, initialization validation, and only the active declared formatters or codecs. Generated output contains no public general `Identity`, `Identity<TKind>`, raw-value interface, implicit primitive conversion, service locator, or reflection-discovered registry.
6. Generated metadata is an immutable fragment composed explicitly by later activation and SDK-manifest work. Runtime registration cannot replace a kind, change its strategy, add a codec, or reinterpret old bytes. The exact manifest, generator version, and normalized descriptor identity become receipt inputs when package and receipt schemas are accepted.
7. The all-zero or all-default representation is invalid for every foundational identity unless a later accepted decision explicitly gives a kind a different non-object meaning. Default values never resolve. Construction APIs reject invalid values before owner lookup, allocation, resource acquisition, or publication.
8. Equality exists only on the generated nominal type. Every comparable scoped identity embeds an opaque scope token in the value, and equality and hashing include it. An owner may use a smaller context-relative storage key internally, but that key is not the nominal identity, has no context-free equality contract, and cannot cross an owner boundary. Dynamic comparison first validates kind, scope, and encoding. Ordering is absent unless the declaration gives it a canonical operation-specific meaning.
9. The built-in **process-incarnation** profile uses a nonzero 128-bit value from the operating-system cryptographic random source for a process bootstrap or other accepted root that has no narrower issuer. Production activation creates at most one `HostScope` from one game-host process-bootstrap context. Tests that need several hosts use distinct injected bootstrap contexts rather than pretending that several production process incarnations share one bootstrap. The profile never uses simulation random state, wall-clock order, worker identity, object hash codes, or a deterministic game seed. Randomness is collision resistance, not authentication, authorization, or a claim of mathematically impossible global uniqueness.
10. The built-in **host-sequence** profile contains the owning `HostInstanceId` plus a nonzero unsigned 64-bit sequence from a separate monotonic issuer per identity kind. World, session, and attachment values are never reused within a host incarnation. Checked sequence exhaustion closes admission for that kind and requires a fresh host incarnation; it never wraps, resets, searches for a gap, or preserves gameplay order through identity ordering. The generated value exposes equality but no ordering.
11. A kind-specific issuer reserves a candidate before dependent resources are acquired or makes reservation part of the same reversible preparation. Zero or a collision in that issuer's local reservation set causes a bounded retry; a retry-budget or sequence-space exhaustion returns a typed allocation failure and publishes no scope, lease, mapping, or diagnostic identity. CP02 creates no global process-incarnation registry and does not claim it can pre-detect a collision between independent processes. Any later handshake, supervisor, or diagnostic merger that observes an equal process root with conflicting provenance reports an integrity collision rather than merging records.
12. The built-in **scoped-generation** profile contains or resolves through the declared owner scope, slot, and nonzero generation. A released slot advances its generation before reuse. Checked generation exhaustion retires the slot permanently rather than wrapping, resetting, or aliasing. Exact slot and generation widths are per-kind profile parameters and must be proven against the owning subsystem's scale and memory evidence.
13. The built-in **canonical-artifact** profile accepts only values produced from a domain-separated canonical payload and a versioned algorithm selected by the artifact-owning ADR. The identity generator does not hash object memory, `ToString`, paths, timestamps, mutable channels, or collection enumeration. Duplicate declarations or a digest collision with different canonical payloads are integrity failures.
14. The built-in **qualified-logical** profile composes already declared publisher, package, domain, kind, and local-name identities according to the normalization grammar selected by the owning content or package ADR. Display text and physical paths remain provenance, not identity.
15. The built-in **issuer-durable** profile records the declared issuer, package-qualified domain, and local value. Issuance, collision recovery, tombstone retention, privacy, federation, and repository recovery remain owned by `DATA-IDENTITY`; this profile alone creates no global registry or continuity guarantee.
16. Adding a new instance of an accepted strategy is a reviewed declaration or subordinate profile change. Changing an existing kind's scope, issuer, strategy, equality, reuse, or value meaning requires a new kind or explicit migration. Activating or changing an encoding is a separate compatibility event governed by clause 17 and the owning surface decision; it does not change identity meaning by itself. Adding a strategy with new semantics requires an amending technical ADR.
17. Representation and serialization are an allow-list by surface, including runtime-only, diagnostic descriptor, supervision journal, network, checkpoint, catalog, document, package, and receipt. Every surface is **forbidden**, **reserved**, or **active**. The default is forbidden. A reserved surface names its future owner but emits no codec and grants no implementation authority; activation requires the accepted owning ADR or reviewed subordinate specification, an explicit encoding version and bounds, and compatibility evidence. Permission for one surface never implies another, and a dynamic descriptor codec does not become a nominal parser or resolution capability.
18. Every generated reversible codec is versioned, canonical, length-bounded, and allocation-bounded. Decode validates surface permission, kind, scope, issuer, encoding version, exact length, canonical form, and profile constraints before lookup. Unknown kinds or versions, missing scope, trailing data, noncanonical values, oversize values, and unsupported surfaces return stable typed failures without reflective construction or fallback guessing.
19. Diagnostic formatting is separate from reversible serialization. Generated `ToString` reveals at most the non-secret diagnostic kind and initialization state. Each declaration sets a maximum disclosure class of **public**, **correlatable**, or **omit**. The fail-closed ordinary output is kind only for a known initialized platform kind and omission for an unknown or invalid dynamic value; a sink must have separate authority to use the declaration's maximum.
20. Correlatable output uses HMAC-SHA-256 over a domain-separated, length-delimited tuple of stable kind, canonical scope, and canonical value with a rotating diagnostic-domain key, truncated to a 128-bit canonical base32 token. Its versioned envelope includes opaque non-secret correlation-domain and key-epoch identifiers so tools never compare tokens produced under different keys accidentally. The key scope and retention are explicit in the diagnostic bundle. The token cannot be parsed into the runtime value or reused as a mapping, credential, durable identity, or authorization input. Public exact output still requires sink-specific authority, and an omit declaration never exposes a value.
21. A declaration names who owns reuse tombstones and the terminal policy. Process-incarnation and host-sequence kinds declare no reuse and retain only bounded diagnostic records. Scoped-generation kinds retain slot generation state and permanently retired exhausted slots. Network reorder windows, mapping tombstones, checkpoint and durable retention, and repository garbage collection remain in their owning specifications.
22. Production allocation is injectable behind kind-specific internal issuer contracts so tests can force zero, collision, retry, and exhaustion outcomes. Test injection never changes the production strategy identity, and generated SDK code cannot obtain an issuer merely by possessing an identity type.
23. The initial CP02 implementation matrix is:

| Kind | Strategy and scope | CP02 output | Deferred detail |
|---|---|---|---|
| `HostInstanceId` | Process incarnation issued by process bootstrap | Generated internal value; correlatable diagnostic maximum; supervision-journal surface reserved | Journal encoding and retention |
| `WorldInstanceId` | Host sequence under one embedded host incarnation | Generated internal value; correlatable diagnostic maximum | Checkpoint-source mapping and later map/entity scopes |
| `SessionId` | Host sequence under one embedded host incarnation and authentication lifecycle | Generated internal value; correlatable diagnostic maximum | Account/durable-subject mapping, authentication, and privacy policy |
| `SessionWorldAttachmentId` | Host sequence under one embedded host incarnation for one admitted session/world join | Generated internal value; correlatable diagnostic maximum | Network epoch, reconnect mapping, and reorder tombstones |
| `CatalogGenerationId` | Canonical artifact owned by the catalog compiler | Profile validation only; retain the current internal placeholder exclusively as test scaffolding | Generated runtime type, canonical catalog payload, digest algorithm, and catalog/receipt encodings under `CONTENT-CATALOG` |

24. The scoped-generation profile receives a tiny bounded conformance fixture in CP02. Accepted ADRs 0048-0050 now govern production `EntityRef` state, storage, and query integration, but exact width declarations, implementation, and evidence remain gated by the reviewed CP03 identity profile and workloads. A separate synthetic identity in the generator test project activates one small bounded reversible codec so canonical encoding, rejection, and fuzz behavior can be proven without shipping a production codec. Neither fixture can freeze an entity-store layout, reserve a product kind, or become a public identity or codec.
25. Generated declaration or formatter code does not establish resolution, mapping, compatibility, trust, or authority. `FND-MAPPING` owns mapping-record storage and retention; `FND-COMPAT` owns compatibility descriptors and operation policy; activation and lifetime capture remain `FND-ACTIVATION` work.

## What we deliberately will not do

- Reconsider ADR 0043's nominal kinds, scopes, non-substitution rules, mapping model, or compatibility model.
- Use one universal 128-bit representation or allocator for runtime, generational, artifact, logical, and durable identities.
- Expose raw GUIDs, integers, strings, generic identity wrappers, `Parse`, or `TryParse` merely because generated code can do so.
- Infer identity kind, scope, issuer, serialization permission, or redaction from a CLR name or attribute convention at runtime.
- Treat random identity bits, a diagnostic correlation token, a digest, or possession of an identifier as authentication or authorization.
- Select the catalog digest, content-name grammar, network wire layout, save format, document format, package/receipt encoding, or durable issuer repository here.
- Let `CatalogGenerationId.NewPlaceholder` become a production exact-artifact factory.
- Wrap generation, reset an exhausted slot, guess a missing scope, retry forever, or acquire resources before identity reservation without a proven reversal path.
- Create a global mutable identity directory that retains every runtime or durable target.

## Consequences

### Compatibility and migration

Identity meaning moves from duplicated source code into a versioned semantic declaration. The current handwritten runtime types can migrate without a public compatibility promise because they are internal and have no codec, but their call sites must move to generated kind-specific issuers. Generated type names may change without changing identity-kind schema. Changing a kind's scope, strategy, or value meaning normally requires a new kind or explicit migration; activating a reserved surface or changing an active encoding requires a new encoding version and the owning surface's compatibility and migration evidence.

The manifest and generated descriptor become inputs to later SDK, package, receipt, network, checkpoint, and document compatibility. Understanding an old descriptor encoding does not by itself make its target compatible or resolvable for a requested operation.

### Security and failure handling

Default-deny codec generation reduces accidental disclosure and confused-deputy paths. Bounded parsing rejects malformed input before allocation or lookup. Diagnostic pseudonyms reduce routine exposure but do not erase all traffic, timing, or cardinality side channels, and identifiers remain non-secret values rather than credentials.

Allocation collision, invalid scope, corrupt descriptor, unsupported encoding, and generation exhaustion are typed failures. Identity reservation happens before dependent resource acquisition or inside one reversible preparation so failure cannot leak a catalog lease, scope registration, mapping, or partially published object.

### Operations

Structured kind, scope, strategy, redaction, collision, retry, exhaustion, and retired-slot diagnostics become consistent. Diagnostic-domain correlation improves support without requiring raw session or attachment identifiers in ordinary logs. Generator and schema versions add build and receipt material, and high-cardinality exact identity storage remains opt-in and budgeted by later observability policy.

## Bounded first implementation scope

This decision authorizes only the CP02 declaration schema, incremental generator, immutable metadata fragments, process-incarnation and host-sequence issuers, safe diagnostic projection, replacement of the four handwritten host/world/session/attachment types, and synthetic scoped-generation and reversible-codec conformance fixtures. It validates the canonical-artifact profile shape but does not replace `CatalogGenerationId`, choose its canonical payload or digest, or create a production artifact factory.

It does not authorize public SDK identity declarations, dynamic inspection descriptors, network codecs, checkpoint or durable codecs, canonical catalog hashing, entity storage, map/frame identities, network-entity tables, package or receipt formats, mapping tables, or the compatibility rules engine. Those remain behind their program gates.

## How we will prove the decision works

- Schema tests reject duplicate kinds, unknown strategies, invalid scope/issuer graphs, unauthorized platform-kind contributions, missing bounds, contradictory reuse/tombstone rules, undeclared codec surfaces, and unsafe redaction defaults with stable source locations.
- Clean and incremental builds on Windows and Linux generate byte-identical source semantics and descriptor identities from the same exact manifest independently of filesystem order, locale, machine path, and process timing.
- Compile-fail fixtures reject assignment, equality, ordering, construction, raw-value access, and serialization across every incompatible pair among the CP02 generated types and synthetic conformance kinds.
- Reflection and API-surface tests prove generated identities have no public primitive constructor, raw primitive member, generic identity interface, implicit conversion, general parser, or codec for an undeclared surface.
- Forced zero and root-collision streams prove bounded retry; concurrent host-sequence allocation proves per-kind uniqueness and embedded host scope; a tiny sequence profile proves checked exhaustion, closed admission, no wrap, and no published scope or acquired catalog lease on failure.
- A tiny-bit scoped-generation fixture repeatedly reuses slots, rejects stale and cross-scope values, permanently retires an exhausted slot, and never wraps into a live value.
- The synthetic test-only codec's conformance and fuzz tests cover wrong surface, kind, scope, issuer, version, length, noncanonical form, trailing bytes, oversize inputs, and unknown profiles with bounded allocation and stable failures. Each production surface repeats the applicable evidence when its codec is activated.
- Redaction tests prove `ToString` is nonrevealing, kind-only output carries no raw value, HMAC correlation tokens match only inside one authorized diagnostic domain, key rotation breaks cross-domain correlation, public exact output still requires sink authority, and omitted values remain absent.
- Domain-separation fixtures feed equal canonical payload bytes to distinct artifact domains and receive distinct declared digest inputs; injected equal digests with unequal payload provenance fault without selecting a winner. The fixture does not select the production catalog digest.
- Existing two-host/two-world/two-session/reattachment and cleanup scenarios continue to pass after generated identities replace handwritten types; multi-host fixtures construct distinct injected process-bootstrap contexts.

## Implementation notes

The current internal types and focused runtime tests are useful pre-decision groundwork, not implementation of this proposal. No declaration schema, generator, issuer abstraction, generated metadata, descriptor identity, codec, or redaction service exists. `CatalogGenerationId.NewPlaceholder` remains an explicitly non-production test scaffold until `CONTENT-CATALOG` selects and implements the canonical artifact contract; CP02 must not route production construction through it. Implementation status remains `Not started`.

The current world-creation path acquires a catalog-generation lease before inserting the world into its identity-keyed owner table. Collision injection must prove that the generated issuer reserves a unique identity before acquisition or that construction reverses the lease before failure; astronomically unlikely production randomness is not a substitute for transactional proof.

## Dependencies and interaction with queued decisions

ADR 0043 owns the identity kinds, scope/lifetime distinctions, non-substitution rules, mapping semantics, compatibility outcomes, and non-authority rule. ADRs 0017 and 0028 own host, world, session, and attachment lifetimes. ADR 0019 owns the entity handle's world/slot/generation meaning and no-wrap stale safety. ADRs 0021-0023 own catalog identity, package/receipt integration, and network identity consumers.

`FND-ACTIVATION` will compose generated metadata and reject invalid lifetime capture. `FND-MAPPING` will specify purpose-bound mapping records. `FND-COMPAT` will specify descriptor and operation-policy language. None of those may add a new identity strategy or codec surface silently.

## Follow-up decisions and specifications

- Identity declaration JSON Schema, normalized descriptor encoding, generator diagnostics, and manifest-fragment composition specification.
- CP02 process-incarnation and host-sequence issuer lifetimes, retry/exhaustion parameters, correlation-key lifecycle, and diagnostic bundle specification.
- CP03 `EntityRef` representation widths and storage integration under `SIM-STATE`, `SIM-STORAGE`, and `SIM-QUERY`.
- Canonical catalog-generation payload and digest under `CONTENT-CATALOG`.
- Map/frame generation parameters under `SPATIAL-CORE`.
- Attachment-local network identity and reorder tombstones under `NET-SCHEMA`, `NET-REPLICATION`, and `NET-RECONNECT`.
- Checkpoint-local and durable issuer profiles under `DATA-IDENTITY`.
- Package, receipt, document, and public SDK declaration profiles under their owning ADRs.
- Purpose-bound mapping-record schema under `FND-MAPPING` and compatibility descriptor/policy language under `FND-COMPAT`.

## References

- [ADR 0017](0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0021](0021-compile-content-into-a-canonical-provenance-catalog.md)
- [ADR 0023](0023-generate-versioned-authoritative-replication-schemas.md)
- [ADR 0028](0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [ADR 0042](0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043](0043-use-a-typed-identity-and-compatibility-spine.md)
- [`FND-IDENTITY` program package](../../status/adr-development-program.md#foundation-compatibility-and-lifecycle)
- [Current identity contract tests](../../../tests/Robusta.Runtime.Tests/Hosting/IdentityContractTests.cs)
- [Pinned Robust Toolbox `EntityUid`](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntityUid.cs)
