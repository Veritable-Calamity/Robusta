# ADR 0045: Generate typed capability graphs and closed activation plans

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-23
- **Decision level:** Technical
- **Owners:** Runtime and SDK workstreams
- **Program ID:** `FND-ACTIVATION`
- **Source queue IDs:** `F-SCOPE-01`, `F-SCOPE-02`
- **Supersedes:** None
- **Refines:** ADRs 0017, 0018, 0028, and 0029
- **Product decisions served:** 0001-0003, 0006, 0007, 0009, 0011-0015, 0026, 0033, 0034, 0039-0041
- **Related decisions:** 0017, 0018, 0026, 0028, 0029, 0043, 0044

## The question

How will declared owner-scoped capabilities become one deterministic graph, generated typed factories, and enforceable lifetime-capture rules without exposing a service locator, depending on reflection order, or publishing a partially activated scope?

## The promise preserved

Game and platform developers receive the capabilities appropriate to the current owner through explicit construction and generated activation. Two worlds cannot share mutable state accidentally, session and world scopes remain siblings, attachments see only narrow endpoint projections, and invalid captures produce source-quality diagnostics rather than late cross-world corruption.

## Why this matters

ADRs 0017 and 0028 accept explicit ownership scopes and prohibit arbitrary resolution. ADR 0018 selects a generated, capability-oriented SDK. ADR 0026 requires detectable lifetime and capability violations to fail with useful diagnostics. ADR 0029 limits phase-local access but cannot repair a mutable capability captured for the wrong lifetime. Accepted ADR 0044 supplies the nominal identities and owner tokens needed to bind generated activation safely.

The current kernel constructs scopes manually. That is useful groundwork, but it leaves constructor choice, resource preparation, endpoint projection, and capture safety as handwritten convention. A conventional nested dependency-injection container also does not naturally represent sibling world and session scopes joined by an attachment.

The activation mechanism must therefore distinguish lifecycle ownership from injectable capability access, validate every retained edge, and publish a scope only after its identity, bindings, resources, construction, and readiness checks agree.

## Options considered

### Option A: Typed declarations generate a canonical graph and closed factories

Authors declare capability schemas and activation constructors in typed source with stable versioned schema keys. An incremental generator emits a normalized language-neutral graph fragment, owner-specific typed factories and binding views, deterministic activation metadata, and analyzer facts.

The generated graph, rather than CLR names, reflection, assembly scanning, container registration order, or runtime discovery, is semantic authority. Future non-C# frontends may emit the same graph schema.

This preserves ordinary typed refactoring and constructor diagnostics while making the complete graph consumable by tools and receipts. It requires a generator, analyzer, graph schema, and strict separation between declarations and runtime activation.

### Option B: A hand-authored canonical manifest generates the bindings

Make a structured manifest the source of truth for capabilities, owners, dependencies, implementation bindings, and factories.

The artifact is language-neutral before compilation and resembles ADR 0044's identity declarations. It duplicates implementation shape already expressed by typed constructors, makes refactoring more cumbersome, and can drift from the actual C# construction hook. Activation is more language- and implementation-specific than identity-kind semantics.

### Option C: A validated runtime scoped container

Register services and lifetimes into a private container, validate the graph at startup, and create nested or joined scopes dynamically.

This is familiar and flexible. Sibling worlds and sessions plus attachment endpoint joins do not form a normal nested-container hierarchy, and late resolution, reflection, registration order, and accidental service location remain difficult to exclude. Static capture and endpoint evidence is substantially weaker.

## Decision

Robusta will use Option A: typed declarations generate a canonical graph and closed factories.

The technical contract is:

1. Every activated owner is published only from one generated and validated activation plan. Every retained capability edge names its owner and capture mode. Raw scopes, general service locators, reflection scans, and ambient mutable registries are not ordinary dependency paths.
2. A typed capability declaration records a stable versioned internal schema key; contract projection; owning domain; state class; permitted export targets; cardinality and requiredness; dependency edges and capture modes; implementation binding and construction hook; preparation and disposition requirements; diagnostic name; and authored source location. The CP02 key is engine-controlled graph metadata, explicitly not an ADR 0044 identity and not authority. `SDK-MANIFEST` must define the publisher, package, domain, normalization, and issuance contract before any public or package-contributed capability schema becomes an ADR 0044 `qualified-logical` identity.
3. CLR namespaces, type names, constructor order, assembly names, filenames, registration order, and reflection discovery are not capability identity or graph ordering.
4. The initial owner domains are `ProcessBootstrap`, `Host`, `World`, `Session`, and `SessionWorldAttachment`. Catalog generations are immutable artifacts retained through typed leases, not parent activation scopes.
5. State classes distinguish owner-mutable capability, immutable value or view, explicitly reviewed shared-host contract, lifecycle-control endpoint, and endpoint projection. Calling a class immutable does not make it shareable. A shareable immutable value is generated from immutable fields or passes through an approved immutable adapter. A shared-host contract cites its accepted owner contract and declares its sole mutation owner, thread-safety and reentrancy rules, fault radius, and applicable limits; the state-class tag alone cannot bypass isolation.
6. The graph uses distinct edges:

| Edge | Meaning | Retained by the consumer |
|---|---|---:|
| `Owns` | An owner registry controls a child scope's publication and ending | Lifecycle only; not injectable |
| `Capture` | A node retains a typed capability for its lifetime | Yes |
| `ActivationInput` | A factory borrows a value only during preparation | No |
| `ArtifactLease` | A scope retains one immutable artifact through an owned lease | Yes |
| `EndpointProjection` | An attachment retains a narrow session or world view | Yes |

7. Graph fragments compose explicitly and normalize by stable schema. Duplicate schemas, unsupported fragment versions, missing dependencies, ambiguous single-valued providers, construction or capture cycles, invalid owner relations, and undeclared exports fail with stable source-located diagnostics before a plan is usable.
8. A multiple-provider capability requires an explicitly declared generated aggregate and canonical member semantics. Registration or discovery enumeration never becomes application-visible ordering.
9. Only construction and capture dependencies must be acyclic. Lifecycle `Owns` edges and narrow child-to-owner close endpoints are separate from injectable construction edges and do not create a false capability cycle.
10. The production process entry point issues exactly one non-game-accessible `ProcessBootstrap` context, and that context may publish exactly one host. Additional injected bootstrap contexts are available only through test-only seams excluded from production entry points and game/package APIs. Process bootstrap captures no mutable child state. Host capabilities may capture bootstrap exports and same-host capabilities. A host registry may retain child lifecycle handles only through `Owns`; that does not permit host services to inject mutable child capabilities.
11. World capabilities may capture same-world capabilities, explicitly exported narrow host contracts, and declared immutable artifact leases. Session capabilities may capture same-session capabilities and explicitly exported narrow host contracts.
12. Worlds and sessions are incomparable siblings and cannot capture one another. An attachment may capture same-attachment capabilities, explicit host exports, and generated endpoint projections from exactly one admitted session and world.
13. An attachment receives no raw `WorldScope`, `SessionScope`, endpoint implementation, arbitrary endpoint resolver, or capability chosen merely by matching an identity. Its consumer-facing projection exposes only declared endpoint data, while the generated binding privately carries a nonserializable owner-issued liveness token or lease used to validate typed owner identity, live membership, projection generation, and terminal state. The token is neither a public identity nor authority outside that binding.
14. Knowledge or possession of a capability schema, plan, scope identity, or endpoint identity grants no authority. Cross-owner use requires an owner-issued typed binding validated for that operation.
15. A generated scope factory executes these stages:
    1. validate the immutable activation plan and owner admission;
    2. validate request, owner, and endpoint bindings;
    3. reserve the ADR 0044 identity;
    4. resolve every required declared binding;
    5. acquire declared leases and resources in deterministic plan order, recording each reversal or terminal disposition before the next fallible step;
    6. invoke nonpublishing construction hooks and immediately ledger every disposable constructed output;
    7. run nonpublishing declared validators and readiness checks;
    8. reconcile the complete preparation ledger and owner registrations;
    9. publish the complete scope into its owner registry atomically; and
    10. return the scope or one typed activation failure.
16. Work racing an owner admission fence either publishes one fully registered scope before the fence or reverses all preparation and publishes nothing. It cannot leave a lease, endpoint, child registry entry, background task, or diagnostic identity reachable.
17. Identity sequences consumed by failed preparation are never reused. Identity reservation precedes dependent acquisition or participates in the same reversible preparation under ADR 0044.
18. Preparers, construction hooks, and validators use a closed generated preparation/effect vocabulary. The generator and analyzer transitively classify calls as pure construction, declared ledgered acquisition, bounded validation over supplied inputs, or an accepted adapter effect; any opaque or unclassified call fails plan generation with `UnknownActivationEffects`. An accepted adapter contract must declare its bounded effects, publication behavior, preparation-ledger record, reversal, postcondition, and fault semantics. Hooks cannot publish themselves, resolve undeclared dependencies, acquire undeclared resources, start background work, subscribe undeclared handlers, perform irreversible external work, or retain activation inputs. Resource acquisition is a declared preparer with an explicit disposition committed to the preparation ledger before any later fallible action.
19. Expected closed-owner, stale or foreign endpoint, retired-artifact, missing-capability, validation, cancellation-before-publication, and publication-conflict outcomes are typed activation failures. Publication is the activation commit point: cancellation first observed afterward cannot reverse the scope or report that it was not created; the caller receives the published result or initiates a separate identified close operation. Exceptions are not the ordinary expected-failure protocol.
20. Failure or cancellation before publication reverses acquired work in reverse order and publishes no scope. Until the separate CP02 cleanup/fault profile under accepted ADR 0046 is reviewed and approved, implementation is limited to bounded idempotent preparation reversals; reversal failure is integrity-unknown and cannot become successful activation.
21. The lifetime-capture analyzer rejects detectable longer-lived capture of shorter-lived mutable capability; host capture of world, session, or attachment state; world/session sibling capture; attachment capture of raw endpoint scopes; and undeclared cross-owner capability use.
22. Capture analysis includes fields, properties, constructor arguments, returned delegates, closures, tasks, factories, lazy values, event subscriptions, cleanup callbacks, `static`, `ThreadLocal`, and `AsyncLocal` storage. Wrapping a shorter-lived capability does not lengthen its lifetime.
23. The analyzer rejects raw scope injection, arbitrary resolver or `IServiceProvider` access, runtime `GetService`, reflection activation, manually selected implementations, undeclared constructor parameters, direct generated-constructor calls, and known escapes from a generated binding.
24. An analyzer suppression cannot narrow the access or lifetime observed by generated metadata. Unprovable lifetime capture receives no `ExclusiveWorld` fallback: scheduler exclusivity cannot repair a cross-world alias.
25. Runtime validation consumes generated metadata without reflecting over the object graph. It verifies the exact explicit fragment set, supported graph version, provider cardinality, factory-plan agreement, owner kind and typed identity, live membership, endpoint state, active exports, and absence of undeclared dynamic bindings before preparation and again before publication.
26. Runtime validation is defense against stale bindings, graph mismatch, dynamic entry points, and internal defects. It does not claim to detect arbitrary reflection, unsafe code, native corruption, or deliberately hidden static aliases. Bypass remains nonconforming executable code under ADR 0026.
27. Generated graph metadata is immutable. Runtime code cannot add, replace, or reinterpret a capability, edge, export, owner, or factory. Dynamic extension admission remains behind its own manifest, trust, compatibility, and isolation decisions.
28. Exact plan receipt identity and compatibility remain `SDK-MANIFEST`, `PACKAGE-SCHEMA`, and `FND-COMPAT` work. This decision defines deterministic semantic graph output but does not freeze a public package envelope.

## What we deliberately will not do

- Introduce an ordinary `IServiceProvider`, global service locator, raw scope resolver, reflection scanner, or mutable registration registry.
- Treat lifecycle ownership as permission to inject child mutable state into its owner.
- Nest worlds beneath sessions, sessions beneath worlds, or mutable worlds beneath catalog generations.
- Let a factory, lazy value, delegate, closure, event, task, or cleanup callback hide a forbidden capture.
- Claim that analyzers or dependency injection sandbox arbitrary trusted code.
- Select ECS system activation, phase leases, query borrows, scheduler fallback, or message handler registration here.
- Select authentication, roles, avatar association, interest, network mapping, catalog adoption, hot reload, durable transfer, or extension loading here.
- Freeze public SDK graph packaging, receipt identity, compatibility policy, or support windows.
- Fall back to a handwritten object graph when generated activation fails.

## Consequences

### Developer experience and migration

Capability dependencies remain ordinary typed constructor inputs, but declarations and diagnostics make ownership visible. Existing direct constructors and `HostScope.Create*` methods may remain temporary internal façades only when they delegate completely to generated plans. Direct production construction sites must migrate behind those factories.

Some otherwise convenient patterns—ambient services, static mutable helpers, captured callbacks, or resolving a world from a host—become explicit errors. This is deliberate: the engine cannot promise multi-world isolation while retaining those escape paths.

### Security and failure handling

Generated bindings reduce accidental capability escalation but are not authorization or hostile-code containment. Every cross-owner endpoint is revalidated, and activation publishes nothing until the complete scope is ready.

Constructor faults, reversal faults, unsafe code, native code, and deliberate analyzer bypass retain ADR 0026's honest containment boundary. `FND-FAULT` owns escalation and hard containment.

### Operations

Normalized plans and typed activation failures make missing bindings, invalid endpoints, fragment mismatches, preparation stages, and publication conflicts observable without exposing raw identities. Plan cardinality and activation duration budgets remain later profile work.

## Bounded first implementation scope

This decision authorizes an internal CP02 graph schema and generator, synthetic analyzer fixtures, and generated plans for one process bootstrap, host, world, lifecycle-only session, and session-world attachment. It authorizes ADR 0044 identity issuance, world catalog-lease preparation, identity-facing endpoint projections backed by private nonserializable liveness bindings, narrow child-to-owner lifecycle endpoints, runtime plan validation, and temporary façade delegation.

The CP02 session remains lifetime scaffolding, not the authenticated production session promised by ADR 0028. `NET-CONNECTION` owns authenticated admission.

The slice excludes public SDK contribution, game capability injection, system activation, networking state, roles, avatar association, dynamic plugins, catalog adoption, hot replacement, plan compatibility, cleanup budgets, and fault escalation. Ownership refactoring that requires fallible, budgeted, asynchronous, or escalation-bearing reversal waits for the separate CP02 cleanup/fault profile under accepted ADR 0046 to be reviewed and approved; bounded idempotent prepublication reversals remain authorized by clause 20.

## How we will prove the decision works

- Identical declarations produce byte-identical normalized graph semantics and factory plans across fragment order, filesystem order, Windows, and Linux.
- Generator fixtures reject duplicate providers, missing requirements, cycles, unknown owners, invalid edges, ambiguous aggregates, and fragment-version mismatch at authored source.
- Compile-fail fixtures cover host-to-child capture, world/session sibling capture, raw attachment endpoints, cleanup-closure escape, static and ambient storage, service location, undeclared dependencies, direct construction, unproven capability escape, opaque helper calls, and unclassified activation effects. Accepted-adapter fixtures prove declared effect and reversal metadata reaches the plan.
- Existing two-world, two-session, attachment, reattachment, and teardown scenarios pass through generated factories.
- A production process cannot issue a second bootstrap context or publish a second host; multi-host tests use distinct test-only injected bootstrap contexts.
- Failure injection after identity reservation, lease acquisition, construction, validation, and immediately before publication leaves no child registry entry and restores every acquired resource count.
- A close-versus-attach race yields one complete live attachment or no attachment, never a partial owner or endpoint registration.
- Retired catalogs, closed owners, foreign-host endpoints, stale projections, forged bindings, and plan mismatches fail before publication with stable redacted diagnostics.
- Architecture and API tests find no runtime service locator, reflection registration, public scope resolver, or direct production construction path.
- Within the generated graph, instrumented conforming bindings, and statically detectable captures, two worlds share no mutable capability except an explicitly declared and reviewed narrow host export. ADR 0026's unsafe-code and deliberate-bypass boundary remains explicit.

## Implementation notes

Current scope construction and endpoint tests are groundwork evidence only; they do not implement this decision. No activation generator, graph schema, analyzer, process-bootstrap context, generated factory, runtime graph validator, or activation transaction exists. Implementation status remains `Not started`.

The first implementation should add these ownership markers where the corresponding work remains:

- `// TODO(FND-ACTIVATION): route scope construction through generated plans and narrow owner endpoints.`
- `// TODO(FND-FAULT): add activation reversal escalation, cleanup budgets, and leak evidence.`
- `// TODO(NET-CONNECTION): replace the lifecycle-only session seed with authenticated admission.`
- `// TODO(CONTENT-CATALOG): replace placeholder catalog identity and view with the canonical artifact contract.`
- `// TODO(SIM-SYSTEM): project owner capabilities into phase-scoped non-storable views.`

Logical folder organization is expected, but this decision does not require a project per capability, owner, generator concern, or folder.

## Dependencies and interaction with queued decisions

Accepted ADR 0044 supplies generated identities and scope tokens; its CP02 slice must exist before these factories replace current identity creation. Accepted ADR 0046 owns cleanup deadlines, leak evidence, and reversal-failure escalation; its CP02 profile remains required before production adoption of those semantics. Accepted ADR 0047 owns graph and profile compatibility.

`SDK-MANIFEST` owns public declarations, fragment envelopes, side/trust projections, and receipt membership. `SIM-SYSTEM` and ADR 0029 own phase-scoped non-storable system views. `NET-CONNECTION` owns authenticated sessions; later `NET-*` decisions own attachment roles and replication. ADR 0028 and this activation contract retain the base world-owned lease lifecycle; `CONTENT-CATALOG` owns exact catalog identity, the canonical immutable view, and repository integration. Extension decisions own dynamic or native capability admission.

## Follow-up decisions and specifications

- Capability declaration and normalized graph schemas, generator diagnostics, and source-mapping specification.
- CP02 internal owner matrix, preparation vocabulary, typed activation failure schema, and façade-removal plan.
- CP02 cleanup/reversal profile under accepted ADR 0046.
- Public `SDK-MANIFEST` capability fragment, side, trust, package, and receipt profiles.
- System activation and phase-view projection under `SIM-SYSTEM`.
- Authenticated session and attachment admission under `NET-CONNECTION` and later networking decisions.
- Dynamic managed and native extension activation under their explicit trust and isolation ADRs.

## References

- [ADR 0017](0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0018](0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [ADR 0026](../product/0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0028](0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0043](0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](0044-generate-bounded-identity-declarations.md)
- [ADR 0046](0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047](0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [`FND-ACTIVATION` program package](../../status/adr-development-program.md#foundation-compatibility-and-lifecycle)
- [Current host composition](../../../src/Robusta.Runtime.Shared/Hosting/HostScope.cs)
- [Current ownership tests](../../../tests/Robusta.Runtime.Tests/Hosting/OwnershipScopeTests.cs)
