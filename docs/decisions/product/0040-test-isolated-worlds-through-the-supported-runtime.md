# ADR 0040: Test isolated worlds through the supported runtime

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Last reconciled:** 2026-07-24
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends:** ADR 0014 by making the bounded published Test SDK and qualification floor explicit Robusta 1.0 requirements
- **Related decisions:** 0002, 0003, 0006, 0007, 0009, 0011-0016, 0017-0021, 0023, 0026-0031, 0033, 0035-0039, 0042-0051

## The question

How easily should an external game test a small world with exact content, controlled time and inputs, known random state, and no mutable global contamination, including when several tests or previews run concurrently?

## The promise

An external game can construct, drive, inspect, and dispose an isolated headless world through published test artifacts and the ordinary supported runtime contracts. Tests do not sleep for simulation progress, receive no private engine privileges, and can run concurrently without sharing mutable world, clock, random, lifecycle, diagnostic, or service state.

## Why this matters

A world boundary is credible only if a test can create more than one world and prove they do not contaminate one another. Station-like behavior also needs repeatable tests for timers, containment, map movement, physics, prediction, and random gameplay without booting a graphical application or waiting for wall time.

A special mock ECS may make small tests fast while silently omitting the lifecycle, scheduler, schema, and authority behavior that matters in release. At the other extreme, forcing every test through one global client/server process makes tests slow, order-dependent, and difficult to diagnose. External games need an intentional ladder from a small in-process world fixture to a real supervised loopback network journey.

## How Robust Toolbox answers today

Robust Toolbox and Space Station 14 maintain extensive unit, shared, client, server, and integration-test infrastructure. Their production use demonstrates the value of headless fixtures and client/server integration tests. Robusta must turn that lesson into a published external-game contract with explicit ownership, exact content identity, manual time, parallel isolation, and cleanup rather than inheriting repository-specific test privileges.

## How the Robusta prototype answers today

The predecessor has test projects and useful architecture fixtures, but neither it nor the current scaffold implements the accepted world lifecycle, scheduler, map, checkpoint, or public inspection contracts. There is no supported external Test SDK, isolated world builder, manual host driver, ordered-input fixture, loopback journey fixture, or leak audit.

## Options considered

### Option A: A supported fixture over the ordinary runtime with generated owner-scoped test bindings

Construct ordinary worlds through the same generated and validated activation plan as other runtime profiles, atomically publish them into an isolated host owner registry, and optionally leave external discovery and networking disabled. Bind declared test-controlled capabilities through generated owner-scoped bindings before activation or through separately supervised owned resources; do not introduce a test-only lifecycle state or replace a published binding. Advance simulation manually, inspect committed observations, and close every owned scope through the ordinary owner coordinator. Use separately supervised loopback authority and clients when the behavior under test crosses the network boundary.

This keeps test and release semantics aligned while allowing small, fast, headless tests. It requires runtime services to be scope-owned and selected only at declared activation boundaries.

### Option B: Provide a lightweight mock world and mock ECS as the primary Game SDK test surface

Mocks are easy to control and can be very fast. They duplicate entity, lifecycle, scheduler, relation, and content behavior, so a passing test can disagree with the real runtime precisely where correctness matters.

### Option C: Require one real global game host and serialize all integration tests

This exercises production code but couples test order, wall time, mutable registries, ports, filesystem paths, and cleanup. It makes parallel evidence and small external-game tests unnecessarily slow and leaves global contamination as an accepted design constraint.

## Decision

Robusta will use Option A.

This decision includes the stated amendment to ADR 0014's published-SDK and qualification floor. Any later removal or narrowing of the supported Test SDK requires an explicit superseding decision rather than a tooling-only change.

The product contract is:

1. Robusta publishes a versioned **Test SDK** that creates worlds through the same host, catalog, lifecycle, scheduler, relationship, content, message, activation, and committed-observation contracts used by ordinary runtime profiles. The fixture is a published external-game surface, not a friend assembly, internal mutation hook, or repository-only test privilege.
2. A test fixture activates and supervises one ordinary in-process `HostScope` or one ordinary child game-host process; it introduces no alternate resolution or ownership scope. Each in-process host receives its own injected test bootstrap context under ADR 0044 rather than pretending that several production process incarnations share one bootstrap. That host owns sibling `WorldScope`, `SessionScope`, and `SessionWorldAttachmentScope` instances under ADR 0028, and each world owns its maps and simulation state. The fixture owns only its supervisory handle, declared test resources, diagnostics, temporary storage, and child processes. It never terminates or mutates a scope it does not own.
3. Every test host and world is constructed from ADR 0045's generated, validated, closed activation plan and becomes visible only through its ordinary atomic publication boundary. The fixture may disable external discovery and networking without creating an unpublished test lifecycle. A fake, recorded adapter, manual driver, or other test-controlled capability is a generated owner-scoped binding selected before activation or a separately supervised owned resource at a declared boundary; there is no post-publication binding replacement, ambient registry, or service locator.
4. Before a test host or world becomes visible, the operation authority selects the exact reviewed and approved test-execution or ordinary world-construction compatibility profile under ADR 0047 and receives a valid, complete `CompatibilityReport` whose coverage and findings admit that publication mode. A test adapter cannot omit a required dimension, choose the policy or profile, convert evaluation failure into a report, or weaken an ordinary construction requirement.
5. World construction names the exact runtime and game receipt, immutable catalog generation, generated activation plan, world configuration, simulation rate, initial definition or setup operations, admitted adapter set, and random-stream seed or state. An invalid combination fails with the same stable validation meaning used by creator and release workflows and publishes no partial owner.
6. Simulation time is manual by default. A test advances an exact number of numbered fixed steps or advances until a declared condition within an exact simulation-step and work bound, without sleeping. Exhausting either bound returns a typed condition-not-met result rather than implying success. Host time, durable time, presentation time, and ADR 0046's monotonic owner-cleanup deadline clock remain separate; pausing or advancing simulation never pauses or advances teardown.
7. External facts enter through ordinary typed ordered-input admission. A caller may provide only the correlation, intended-step or window, and source-sequence fields permitted by the input schema; the receiving authority authenticates and admits the input and assigns its stable admission sequence and origin merge key under ADR 0042. Caller-selected values never choose merge order. Tests do not mutate scheduler queues, component storage, timer heaps, command buffers, or network buffers directly, and a rejected, late, paused, unauthorized, or malformed input has the same product outcome as it does outside tests.
8. Test setup may create maps, spawn entities, apply components, establish relations, and admit sessions only through ordinary published construction and ADR 0042 command paths. A concise builder may compose those operations, but it cannot bypass validation, lifecycle preparation, structural commit, authority, terminal results, or source provenance.
9. The default small-world fixture is headless and denies real filesystem, network, process, environment, wall-clock, database, rendering, audio, and other external effects during authoritative phases. Fake, recorded, and real integrations cross the same ADR 0029 boundary as ordinary runtime code: authoritative work emits committed external-effect intents, an owned effect consumer runs outside the authoritative phase, and any response returns as a later ordered input. Every endpoint, input, committed intent, cleanup obligation, and conformance class is declared; an adapter cannot introduce arbitrary authoritative-phase I/O.
10. Random state belongs to the world and named streams defined by the scheduler contract. A test can set an admitted seed or captured stream state and can report permitted stream provenance, but cannot replace randomness through a mutable process-global singleton. Captured stream state is opaque and admitted only for the exact runtime and execution profile unless a later owning decision activates a bounded codec and compatibility profile. It is not a checkpoint or replay format. Repeating a seed has only the determinism promise accepted for the exact execution profile; it is not an implicit cross-version or cross-platform guarantee.
11. Several fixtures may share exact immutable definitions and executable artifacts while sharing no mutable world, timer, random, session, diagnostic, temporary-path, port-allocation, or fake-service state. Typed nominal identities and fixture-local handles remain owner-scoped. Assertions expose only ADR 0044-permitted exact, nominal, or redacted projections, so a handle, descriptor, continuation token, or observation from one fixture cannot resolve in another and raw reversible encodings do not become a test convenience.
12. Parallel in-process isolation is a guarantee only for conforming game code under ADR 0026. Qualification work that contains shared mutable statics, undeclared threads, unsafe code, unknown native state, or other nonconforming effects is rejected where detectable or runs in a separately supervised disposable process. It cannot coexist in one process with unrelated conforming fixtures whose results are being used as parallel-isolation evidence.
13. Test observations use accepted ADR 0039's authorized read-only snapshot model. Assertions wait for an observation boundary that includes the relevant committed value writes and structural publication, and no inspector can mutate setup or gameplay state. If ADR 0039 is later superseded, its replacement must preserve an equivalent contract or explicitly amend this decision.
14. Tests that exercise client authority, replication, prediction, reconnect, packaging, or process supervision use a separately declared loopback journey fixture. It launches ordinary side-specific projections through the supervised process path, assigns owned ephemeral resources, captures structured events, and closes each owner through its ordinary coordinator. An in-memory fake transport cannot stand in for the required release-level network journey.
15. Explicit close, setup failure, or classified owner fault joins ADR 0046's one cached close operation. Caller cancellation abandons only that caller's wait, while the owner-owned coordinator continues under its injected monotonic deadlines. The terminal outcome is exactly `Clean`, `ClosedWithContainedFaults`, or `ContainmentLost`; hard expiry, unresolved resources, or unproven postconditions produce containment loss rather than cleanup success. A fixture retains only bounded structured diagnostics and committed observations permitted by the applicable integrity and external-retention profile, never assumes a trustworthy final snapshot exists, and does not keep a partially faulted owner running, publish failed-batch effects, or silently retry arbitrary game code.
16. Published Test SDK builders, bounded assertion and difference capabilities, fake services, manual drivers, and fault-injection adapters are excluded from ordinary production client and authority payloads unless a separately reviewed operational capability explicitly requires a production-safe subset. They remain public test contracts where declared, not privileged friend access.
17. First release requires external-game fixtures for compatible headless world construction, exact content, manual bounded steps, ordinary ordered inputs, named random streams, lifecycle and relation setup, committed observations, one classified owner-close outcome, and conforming parallel multi-world isolation. It also requires supervised loopback client/authority journeys for the release scenarios that cross process or network boundaries. A fully virtual operating system, arbitrary distributed-cluster simulation, universal service emulators, long-running soak orchestration, and exhaustive failure exploration remain later tooling.

## Authority and retained implementation gates

This decision amends ADR 0014 and authorizes the product direction for `TEST-RUNTIME`; it does not by itself publish a Test SDK, activate an identity or random-state codec, approve a compatibility or fault profile, permit public capability contributions, or add a production operator endpoint.

Implementation remains gated on all of the following:

- The reviewed and accepted `OBS-INSPECTION` technical contract, including the owner-scoped committed-observation, attachment, authorization, redaction, bounding, and compatibility surfaces used by Test SDK assertions. The Test SDK cannot introduce a private substitute while that predecessor is unavailable.
- A reviewed and approved test-execution or ordinary world-construction `FND-COMPAT` profile under ADR 0047, selected by the operation authority and satisfied before host or world publication.
- The reviewed and approved ADR 0046 CP02 ownership cleanup/fault profile and CP04 scheduler/world-fault profile, including measured monotonic deadlines, postconditions, report bounds, and escalation.
- `TEST-RUNTIME` and `SDK-MANIFEST` specifications for package topology, the external-game API, generated owner-scoped test capability bindings, activation inputs, assertion bounds, and exclusion from production payloads.
- ADR 0044 identity declarations and only the reviewed serialization and redaction surfaces needed by the Test SDK; test convenience does not activate reserved network, checkpoint, replay, public diagnostic, or other codecs.
- The applicable ADR 0042 message schemas and ADR 0045 capability graph and activation-plan contracts. This decision does not create alternate input admission, structural commit, dependency resolution, publication, or cleanup semantics.

## What we deliberately will not do

- Make a mock ECS or private world implementation the conformance oracle.
- Require sleeps, render frames, ambient wall time, filesystem order, or test registration order to advance gameplay.
- Offer raw stores, scheduler queues, service containers, or private mutation as test conveniences.
- Reset process-global mutable state between tests and call that world isolation.
- Claim in-process hard containment for nonconforming, unsafe, or native game code.
- Use an in-memory transport as the only evidence for a supported client/server journey.
- Ship test fakes, fault injectors, or creator-only powers in ordinary production artifacts.

## Consequences

### Benefits

- External games can test real platform semantics quickly and without Robusta source access.
- Manual stepping removes timing sleeps and makes lifecycle, timer, and input boundaries precise.
- Parallel fixtures continuously prove the accepted world and ownership model.
- Small tests and clean-machine network journeys form an explicit evidence ladder rather than competing test architectures.
- Structured observations and owner-close reports make failures reproducible and CI-friendly.

### Costs and limitations

- Runtime dependencies must expose explicit ownership and testable boundaries instead of ambient globals.
- The Test SDK and its builders become versioned supported products requiring documentation and compatibility evidence.
- Real loopback process journeys remain slower than in-process fixtures and need cross-platform resource allocation and cleanup.
- Nonconforming code may require process isolation or serialized tests and cannot receive the full parallel-isolation claim.
- Service-specific emulators and broad fault exploration remain separate investments.

## How we will prove the decision works

- Clean-machine Windows and Linux projects restore the published Test SDK from the external feed, compile without Robusta repository or friend-assembly references, and run the same versioned fixture scenarios through only the documented external-game API.
- An external station-like game receives a complete admitting compatibility report, publishes a headless world from an exact activation plan and catalog, constructs a map, spawns a door and nested inventory item through ordinary paths, advances 100 steps without sleeping, and observes lifecycle, relation, timer, and spatial results at exact committed boundaries.
- A contrasting external game uses the same Test SDK without loading station packages, constructed grids, inventories, rounds, or station-specific helpers.
- Repeating a fixture with the same exact inputs, receipt, catalog, seed, and execution profile produces the canonical serial-oracle and admitted parallel outcomes already required by ADRs 0016, 0020, and 0029; changing an input or seed produces a source-located comparison rather than hidden ambient behavior. Any stronger durable replay claim remains gated by ADR 0041.
- A versioned concurrency workload runs the measured number and size of conforming fixtures under randomized parallel scheduling while sharing one immutable catalog and staying within its declared startup, throughput, memory, and cleanup budgets. World identities, entities, timers, random streams, sessions, diagnostics, temporary data, and fake-service calls never cross fixture boundaries.
- A typed handle, continuation token, session, map identity, or permitted observation captured from one fixture is rejected in another and never aliases an object created later; diagnostics preserve only the declared ADR 0044 projection.
- Invalid prototype data, illegal relation setup, stale entity use, paused input, and an authoritative system fault produce the same stable outcomes as their ordinary runtime paths; no setup helper can force partial state into publication.
- Leak fixtures deliberately leave timers, host tasks, files, sockets, and supervised child processes. Every waiter observes the same owner-close report; proven cleanup ends `Clean` or `ClosedWithContainedFaults`, while hard expiry, unresolved ownership, or an unknown postcondition ends `ContainmentLost` and widens containment.
- Manual simulation-time tests prove step advancement and pause cannot alter the injected monotonic cleanup deadlines, and caller cancellation abandons only one wait without canceling owner teardown.
- A loopback journey launches separately packaged authority and client projections, exercises connect, prediction, correction, disconnect, reconnect, and teardown, and proves through acquisition-ledger postconditions that no child process or port remains owned after a closed outcome.
- Analyzer and runtime fixtures reject mutable statics, escaped phase access, undeclared authoritative-phase I/O, post-publication binding replacement, and test-only private references. Unsafe and native qualification fixtures run in separately supervised disposable processes and never share a process with unrelated conforming parallel-isolation evidence.
- Random-state fixtures reject a captured state from a different exact runtime or execution profile and prove that the opaque state has no checkpoint, replay, or otherwise undeclared codec.
- Package scans prove the ordinary production client and server payloads contain no manual test driver, fake service registry, fault injector, permissive setup API, or test credential.

## Implementation notes

No Test SDK, isolated world fixture, manual driver, generated test-binding profile, loopback journey fixture, cleanup audit, test-execution compatibility profile, CP02 or CP04 fault profile, or conformance evidence exists. Accepted ADRs 0039 and 0042-0051 provide required semantics but do not implement this product decision. Implementation status remains `Not started`.

## Follow-up decisions

- Test SDK package topology, external-game compatibility policy, fixture ownership, builder API, and the reviewed test-execution or world-construction `FND-COMPAT` profile.
- Manual host driver, bounded-condition advancement, ordinary input admission, opaque random-state admission, and typed condition-not-met contracts.
- Generated owner-scoped fake and recorded service bindings, committed-effect boundaries, temporary resource allocation, and production-payload exclusion.
- Reviewed CP02 ownership cleanup/fault and CP04 scheduler/world-fault profiles, including monotonic deadlines, cleanup postconditions, and leak audits.
- Parallel fixture scheduling, immutable catalog sharing, measured workload budgets, leak detection, and separately supervised nonconforming-test classification.
- Loopback client/authority supervision, ephemeral port allocation, structured event capture, and cross-platform CI profiles.
- Failure artifact, published bounded assertion, observation-difference, snapshot-approval, and retention formats over ADR 0039 observations.
- Performance budgets for fixture startup, step throughput, memory, and large test suites.

## References

- [ADR 0002](0002-judge-quality-through-user-outcomes.md)
- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0016](0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0020](../technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0027](0027-run-offline-play-through-a-separate-local-authority.md)
- [ADR 0028](../technical/0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [ADR 0029](../technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0038](0038-edit-map-sources-and-preview-in-isolated-worlds.md)
- [ADR 0039](0039-inspect-running-worlds-through-authorized-snapshots.md)
- [ADR 0042](../technical/0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043](../technical/0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](../technical/0044-generate-bounded-identity-declarations.md)
- [ADR 0045](../technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [ADR 0046](../technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047](../technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [World-model question 25](../../workshops/world-model-question-set.md#25-how-easy-should-isolated-world-tests-be)
- [Robust Toolbox repository test projects](https://github.com/space-wizards/RobustToolbox)
