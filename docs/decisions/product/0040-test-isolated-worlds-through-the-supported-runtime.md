# ADR 0040: Test isolated worlds through the supported runtime

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends if accepted:** ADR 0014 by making the bounded published Test SDK and qualification floor explicit Robusta 1.0 requirements
- **Related decisions:** 0002, 0003, 0006, 0007, 0009, 0011-0016, 0017-0021, 0023, 0026-0031, 0033, 0035-0039

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

### Option A: A supported fixture over the ordinary runtime with explicit test-owned adapters

Construct real unpublished or test-published worlds through the supported host and world lifecycle, using an exact catalog and explicit test-owned adapters for time, random seed, external inputs, sessions, and optional services. Advance simulation manually, inspect committed observations, and dispose every owned scope. Use separately supervised loopback authority and clients when the behavior under test crosses the network boundary.

This keeps test and release semantics aligned while allowing small, fast, headless tests. It requires runtime services to be scope-owned and replaceable only at declared boundaries.

### Option B: Provide a lightweight mock world and mock ECS as the primary Game SDK test surface

Mocks are easy to control and can be very fast. They duplicate entity, lifecycle, scheduler, relation, and content behavior, so a passing test can disagree with the real runtime precisely where correctness matters.

### Option C: Require one real global game host and serialize all integration tests

This exercises production code but couples test order, wall time, mutable registries, ports, filesystem paths, and cleanup. It makes parallel evidence and small external-game tests unnecessarily slow and leaves global contamination as an accepted design constraint.

## Current review position

Option A is recommended for review. No decision is accepted by this proposal.

Acceptance includes the stated amendment to ADR 0014's published-SDK and qualification floor. If a supported Test SDK is not intended for 1.0, ADR 0014 must instead be revised explicitly rather than expanded or narrowed through tooling implementation.

If accepted, the product contract will be:

1. Robusta publishes a versioned **Test SDK** that creates worlds through the same supported host, catalog, lifecycle, scheduler, relationship, content, and inspection contracts used by ordinary runtime profiles. The fixture is a supported external-game surface, not a friend assembly or repository-only test hook.
2. A test fixture supervises one ordinary `HostScope` or one ordinary child game-host process; it introduces no alternate resolution or ownership scope. That host owns sibling `WorldScope`, `SessionScope`, and `SessionWorldAttachmentScope` instances under ADR 0028, and each world owns its maps and simulation state. The fixture owns only its supervisory handle, explicitly installed mutable adapters, diagnostics, temporary storage, and child processes. Disposal requests the ordinary host ownership tree to end in contract order, closes fixture-owned resources, and verifies cleanup within a bounded deadline. It never terminates or mutates a scope it does not own.
3. World construction names the exact runtime and game receipt, immutable catalog generation, world configuration, simulation rate, initial definition or setup operations, and random-stream seed or state. An invalid combination fails with the same stable validation meaning used by creator and release workflows.
4. Simulation time is manual by default. A test advances an exact number of numbered fixed steps or advances until a declared bounded condition without sleeping. Host time, durable time, and presentation time remain separate and enter only through explicit test adapters.
5. External facts enter as typed ordered inputs naming their target, authority, intended boundary, and stable ordering information. Tests do not mutate scheduler queues, component storage, timer heaps, or network buffers directly. A rejected, late, paused, unauthorized, or malformed input has the same product outcome as it does outside tests.
6. Test setup may create maps, spawn entities, apply components, establish relations, and admit sessions only through ordinary published construction and command paths. A concise builder may compose those operations, but it cannot bypass validation, lifecycle preparation, structural commit, authority, or source provenance.
7. The default small-world fixture is headless and denies real filesystem, network, process, environment, wall-clock, database, rendering, audio, and other external effects during authoritative phases. A test that needs one of those capabilities installs an explicit owned fake, recorded adapter, or real integration boundary whose inputs, outputs, cleanup, and conformance class are visible.
8. Random state belongs to the world and named streams defined by the scheduler contract. A test can set an admitted seed or captured stream state and can report stream provenance, but cannot replace randomness through a mutable process-global singleton. Repeating a seed has only the determinism promise accepted for the exact execution profile; it is not an implicit cross-version or cross-platform guarantee.
9. Several fixtures may share exact immutable definitions and executable artifacts while sharing no mutable world, timer, random, session, diagnostic, temporary-path, port-allocation, or fake-service state. Identifiers remain scoped, so a handle or observation from one fixture cannot resolve in another.
10. Parallel in-process isolation is a supported guarantee only for conforming game code under ADR 0026. A test containing shared mutable statics, undeclared threads, unknown native state, or other nonconforming effects is rejected where detectable or visibly loses the affected isolation and determinism claims; the framework does not pretend one process is a hard sandbox.
11. Test observations use the read-only inspection model proposed by ADR 0039. Assertions wait for committed boundaries and report canonical identities, provenance, step, and structured differences. Test-only mutation through an inspector is not a supported setup or assertion technique.
12. Tests that exercise client authority, replication, prediction, reconnect, packaging, or process supervision use a separately declared loopback journey fixture. It launches ordinary side-specific projections through the supervised process path, assigns owned ephemeral resources, captures structured events, and proves cleanup. An in-memory fake transport cannot stand in for the required release-level network journey.
13. A fixture failure preserves bounded structured diagnostics and the last trustworthy committed observation, then performs owned cleanup. It does not keep a partially faulted world running, publish failed-batch effects, or silently retry arbitrary game code.
14. Test support artifacts, fake services, manual drivers, privileged assertions, and fault-injection adapters are excluded from ordinary production client and authority payloads unless a separately reviewed operational capability explicitly requires a production-safe subset.
15. First release requires external-game fixtures for headless world construction, exact content, manual steps, ordered inputs, named random streams, ordinary lifecycle and relation setup, inspection, deterministic cleanup, and parallel multi-world isolation. It also requires supervised loopback client/authority journeys for the release scenarios that cross process or network boundaries. A fully virtual operating system, arbitrary distributed-cluster simulation, universal service emulators, long-running soak orchestration, and exhaustive failure exploration remain later tooling.

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
- Structured inspection and cleanup make failures reproducible and CI-friendly.

### Costs and limitations

- Runtime dependencies must expose explicit ownership and testable boundaries instead of ambient globals.
- The Test SDK and its builders become versioned supported products requiring documentation and compatibility evidence.
- Real loopback process journeys remain slower than in-process fixtures and need cross-platform resource allocation and cleanup.
- Nonconforming code may require process isolation or serialized tests and cannot receive the full parallel-isolation claim.
- Service-specific emulators and broad fault exploration remain separate investments.

## How we will prove the decision works

- An external station-like game creates a headless world from an exact catalog, constructs a map, spawns a door and nested inventory item through ordinary paths, advances 100 steps without sleeping, and inspects lifecycle, relation, timer, and spatial results at exact committed boundaries.
- A contrasting external game uses the same Test SDK without loading station packages, constructed grids, inventories, rounds, or station-specific helpers.
- Repeating a fixture with the same exact inputs, receipt, catalog, seed, and execution profile produces the canonical serial-oracle and admitted parallel outcomes already required by ADRs 0016, 0020, and 0029; changing an input or seed produces a source-located comparison rather than hidden ambient behavior. Any stronger durable replay claim remains gated by ADR 0041.
- Hundreds of fixtures run with randomized parallel scheduling while sharing one immutable catalog. World identities, entities, timers, random streams, sessions, diagnostics, temporary data, and fake-service calls never cross fixture boundaries.
- A handle, continuation token, session, map identity, or inspection observation captured from one fixture is rejected in another and never aliases an object created later.
- Invalid prototype data, illegal relation setup, stale entity use, paused input, and an authoritative system fault produce the same stable outcomes as their ordinary runtime paths; no setup helper can force partial state into publication.
- Leak fixtures deliberately leave timers, host tasks, files, sockets, and supervised child processes. Disposal cancels or closes only owned resources and reports every residue within the declared cleanup budget.
- A loopback journey launches separately packaged authority and client projections, exercises connect, prediction, correction, disconnect, reconnect, and teardown, and proves that no child process or port remains owned after completion.
- Analyzer and runtime fixtures reject mutable statics, escaped phase access, undeclared I/O, and test-only private references. An explicitly nonconforming native fixture receives the documented reduced claim rather than contaminating other evidence.
- Package scans prove the ordinary production client and server payloads contain no manual test driver, fake service registry, fault injector, permissive setup API, or test credential.

## Implementation notes

No Test SDK, isolated world fixture, manual driver, test-owned adapter model, loopback journey fixture, cleanup audit, or conformance evidence exists. Implementation status remains `Not started`.

## Follow-up decisions

- Test SDK package topology, compatibility policy, fixture ownership, and builder API.
- Manual host driver, bounded-condition advancement, input admission, random-state, and timeout contracts.
- Fake and recorded service adapter boundary, effect denial, temporary resource allocation, and cleanup audits.
- Parallel fixture scheduling, immutable catalog sharing, leak detection, and nonconforming-test classification.
- Loopback client/authority supervision, ephemeral port allocation, structured event capture, and cross-platform CI profiles.
- Failure artifact, structured assertion, inspection-diff, snapshot-approval, and retention formats.
- Performance budgets for fixture startup, step throughput, memory, and large test suites.

## References

- [ADR 0002](0002-judge-quality-through-user-outcomes.md)
- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0016](0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0020](../technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0027](0027-run-offline-play-through-a-separate-local-authority.md)
- [ADR 0029](../technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0038](0038-edit-map-sources-and-preview-in-isolated-worlds.md)
- [Proposed ADR 0039](0039-inspect-running-worlds-through-authorized-snapshots.md)
- [World-model question 25](../../workshops/world-model-question-set.md#25-how-easy-should-isolated-world-tests-be)
- [Robust Toolbox repository test projects](https://github.com/space-wizards/RobustToolbox)
