# World Model Workshop 06: Inspection, isolated testing, and replay

- **Workshop status:** Proposed
- **Date:** 2026-07-19
- **Questions:** 24-26 from the world-model question set
- **Decision outcome:** Review-ready Product ADRs 0039-0041 propose Option A; none is accepted and no implementation is authorized

## Why these questions are together

Inspection defines how a developer explains one committed world state. Isolated testing defines how an external game constructs and drives that state without ambient time or mutable global contamination. Replay then defines which initial state, inputs, random state, execution identities, and comparisons are sufficient to reproduce a sequence of committed states.

Treating them as one review set prevents three tempting inconsistencies: a test-only private mutation path, a replay engine different from the ordinary scheduler, and an inspector that exposes implementation memory rather than the same canonical meaning used by tests and replay diagnostics.

This set remains at the product level. It does not select snapshot storage, a query language, a test framework, fixture APIs, hash algorithms, replay encoding, compression, physics backend, numeric representation, UI toolkit, remote protocol, or retention database.

## Proposed decisions

| ADR | Recommended product choice | First-release boundary if accepted | Principal rejected shortcuts |
|---|---|---|---|
| [0039](../decisions/product/0039-inspect-running-worlds-through-authorized-snapshots.md) | Option A: one capability-scoped, read-only snapshot and query model over committed state, with provenance, authorization, redaction, and resource limits | Structured local and authenticated operator inspection; headless queries; platform and game projections; relation, catalog, lifecycle, and network explanation; production projection audit | Private reflection; arbitrary read/write View Variables equivalent; checkpoint or memory dump as live inspection |
| [0040](../decisions/product/0040-test-isolated-worlds-through-the-supported-runtime.md) | Option A: a published Test SDK that constructs and manually drives ordinary runtime worlds with explicit test-owned adapters | Headless exact-content fixtures; manual steps; ordered inputs; named random streams; inspection; cleanup; parallel conforming worlds; supervised loopback client/authority journeys where required | Mock ECS as oracle; sleeps and ambient globals; one serialized global host; in-memory transport as all network evidence |
| [0041](../decisions/product/0041-record-versioned-authoritative-replays-with-declared-determinism.md) | Option A: versioned authoritative input replay inside an explicit compatibility domain | Bounded single-world authority-side capture; exact starting state and dependencies; complete admitted inputs and random state; canonical committed-state and effect verification; serial/parallel comparison; safe headless replay; divergence and compatibility reports | Universal bitwise cross-platform/version promise; presentation recording as determinism proof; real external-effect repetition |

Each proposal has decision status `Proposed` and implementation status `Not started`. The current review position is Option A in every case; this workshop record does not accept them.

Acceptance would explicitly amend ADR 0014's first-release floor: ADR 0039 adds bounded inspection to diagnostics, ADR 0040 adds the published Test SDK and qualification floor, and ADR 0041 adds bounded single-world authoritative diagnostic replay. Reviewers should accept those amendments deliberately or revise the proposals; implementation must not broaden ADR 0014 silently.

## Recommended review order and dependencies

1. **ADR 0039 first:** the observation envelope, committed-boundary rule, identity labels, provenance, and redaction model become the common explanatory vocabulary.
2. **ADR 0040 second:** isolated fixtures consume the inspection meaning and prove that accepted world, time, lifecycle, ownership, and scheduler boundaries are usable by an external game.
3. **ADR 0041 third:** authoritative replay uses ordinary world construction and scheduling, then reports compatibility and divergence through the inspection and test vocabulary.

The proposals can be reviewed in one session, but accepting ADR 0040 or ADR 0041 while materially rejecting ADR 0039 requires their assertion and divergence clauses to be revised. Accepting ADR 0041 also requires a clear answer on whether exact-domain reproducibility, rather than universal cross-platform bitwise identity, is the intended 1.0 promise.

## Accepted constraints carried into the proposals

- A world remains one isolated mutable simulation, while several conforming worlds may share immutable catalog generations under ADRs 0011 and 0012.
- Entity, map, frame, network, durable, checkpoint, catalog, session, process, editor, observation, fixture, and replay identities remain distinct.
- Entity birth, structural mutation, relations, ending, pause, timers, and catalog adoption are observed only at complete committed boundaries under ADRs 0015, 0016, 0019, 0020, 0029, and 0037.
- The scheduler's serial mode remains a correctness oracle, and conforming parallel work may not derive authoritative behavior from worker count, thread identity, or completion order.
- Trusted game code receives world-isolation and deterministic-scheduling claims only while conforming to ADR 0026. Unknown native, unsafe, static, or external effects narrow those claims honestly under ADR 0034.
- Client presentation and prediction remain non-authoritative. Interest, visibility, authorization, secrecy, and mere discovery are different decisions; inspection cannot become a side channel around them.
- World checkpoints, map sources, editor histories, authoritative replay artifacts, client prediction buffers, and player-facing recordings are different artifacts.
- External facts must enter authoritative simulation as ordered inputs. Replaying committed outputs must not repeat real external side effects.
- Development, test, creator, and replay powers are excluded from ordinary production artifacts unless a separately reviewed production-safe capability explicitly declares them.
- Public UGC receives no executable inspector, test fixture, replay host, or trusted extension power merely by using these products.

## Potential first-release baseline

If all three Option A proposals are accepted, Robusta 1.0 gains a bounded diagnostic and evidence spine:

- an external developer can inspect one live or paused committed world through stable identities, provenance, redaction, and bounded queries;
- the same developer can create a small headless world from exact content, inject ordered inputs, advance exact steps without sleeping, inspect the result, and dispose it without contaminating another fixture;
- required network journeys can use separately supervised loopback authority and client projections rather than a test-only authority model; and
- an authorized diagnostic capture can reconstruct a bounded single-world authority sequence under an exact validated compatibility domain, suppress real external sinks, compare canonical outcomes, and identify the earliest checked divergence.

This is sufficient to support first-release conformance scenarios, external reference-game debugging, scheduler serial/parallel evidence, and forward-migration corpora. It is not a universal time-travel debugger or a complete round-spectator product.

## Explicitly deferred or excluded

- Private-memory browsing and arbitrary reflective method invocation.
- A universal inspection setter or mutation console.
- Historical fleet-wide live joins and automatic root-cause inference.
- A mock ECS as the release conformance oracle.
- A fully virtual operating system, universal service emulators, arbitrary distributed-cluster simulation, and exhaustive failure exploration.
- Indefinite replay archival, arbitrary mid-step rewind, live branching, and automatic forward execution of every old replay.
- Universal bitwise equality across operating systems, architectures, runtime releases, physics backends, and native extensions.
- Full client-prediction re-simulation and polished player or spectator replay in the first release.

## Review points requiring an explicit answer

- Whether discovery of a hidden object or relation is itself authorization-controlled, as recommended by ADR 0039 Option A.
- Whether arbitrary mutation remains outside the inspector and must use declared game, editor, migration, or operator operations.
- Whether parallel in-process test isolation is promised only for conforming code, with process isolation required for stronger containment.
- Whether real loopback process journeys remain required evidence for networking even when small tests use in-process fixtures.
- Whether 1.0 determinism means canonical outcome equivalence inside a declared exact domain, with broader cross-platform domains earned by evidence rather than assumed.
- Whether diagnostic replay suppresses all real external outputs and compares committed effect observations instead.

## Technical decisions that would follow acceptance

- Inspection envelope and projection schema; capture consistency; query limits; authorization, redaction, audit, and production protocol.
- Test SDK topology and fixture ownership; manual driver and ordered inputs; fake-service contracts; leak audits; loopback supervision and CI budgets.
- Replay manifest and segments; authoritative input ledger; random-state capture; compatibility-domain vocabulary; canonical state and effect projections; safe replay host; privacy and retention.
- Shared structured differences and provenance that can explain inspection assertions, test failures, replay divergence, and migration comparisons without exposing secrets.

## Sources reviewed

- [World-model question set](world-model-question-set.md#g-testing-and-inspection)
- [Accepted workshop 04: entity lifecycle and simulation time](2026-07-18-world-model-04-entity-lifecycle-and-simulation-time.md)
- [Accepted workshop 05: space, persistence, and preview](2026-07-19-world-model-05-space-persistence-and-preview.md)
- [ADR 0020: deterministic phase scheduler](../decisions/technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0029: phase-scoped access and buffered effects](../decisions/technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0035: versioned world checkpoints](../decisions/product/0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [Space Station 14 admin tooling and View Variables](https://docs.spacestation14.com/en/community/admin/admin-tooling.html#view-variables)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
- [Space Station 14 server replay recording](https://docs.spacestation14.com/en/server-hosting/server-replay-recording.html)
- The pinned Robusta predecessor, Robust Toolbox, and Space Station 14 revisions listed in the [source notes](../reference/source-notes.md)
