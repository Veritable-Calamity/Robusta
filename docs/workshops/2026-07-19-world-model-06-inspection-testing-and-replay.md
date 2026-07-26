# World Model Workshop 06: Inspection, isolated testing, and replay

- **Workshop status:** Accepted
- **Date:** 2026-07-19
- **Last reconciled:** 2026-07-24
- **Questions:** 24-26 from the world-model question set
- **Decision outcome:** Product ADRs 0039-0041 are accepted via Option A; implementation remains `Not started`

## Why these questions are together

Inspection defines how a developer explains one committed world state. Isolated testing defines how an external game constructs and drives that state without ambient time or mutable global contamination. Replay then defines which initial state, inputs, random state, replay-local records, runtime mappings, and comparisons are sufficient to reproduce a sequence of committed states.

Treating them as one review set prevents three tempting inconsistencies: a test-only private mutation path, a replay engine different from the ordinary scheduler, and an inspector that exposes implementation memory rather than the same canonical meaning used by tests and replay diagnostics.

This set remains at the product level. It does not select snapshot storage, a query language, a test framework, fixture APIs, hash algorithms, replay encoding, compression, physics backend, numeric representation, UI toolkit, remote protocol, or retention database.

## Accepted decisions

| ADR | Accepted product choice | First-release boundary | Principal rejected shortcuts |
|---|---|---|---|
| [0039](../decisions/product/0039-inspect-running-worlds-through-authorized-snapshots.md) | Option A: one capability-scoped, read-only snapshot and query model over committed state, with provenance, authorization, redaction, and resource limits | Structured local and authenticated operator inspection; headless queries; platform and game projections; relation, catalog, lifecycle, and network explanation; production projection audit | Private reflection; arbitrary read/write View Variables equivalent; checkpoint or memory dump as live inspection |
| [0040](../decisions/product/0040-test-isolated-worlds-through-the-supported-runtime.md) | Option A: a published Test SDK that constructs and manually drives ordinary runtime worlds through generated owner-scoped activation and adapters | Headless exact-content fixtures; manual steps; ordered inputs; named random streams; bounded inspection; one shared cleanup result; parallel conforming worlds; supervised loopback client/authority journeys where required | Mock ECS as oracle; sleeps and ambient globals; one serialized global host; in-memory transport as all network evidence |
| [0041](../decisions/product/0041-record-versioned-authoritative-replays-with-declared-determinism.md) | Option A: versioned authoritative input replay inside an explicit compatibility domain | Bounded single-world authority-side capture; exact starting state and resolved policy; complete admitted inputs and random state; replay-local identity mappings; canonical committed-state and effect-intent verification; fixed declared partitions with variable worker scheduling; safe headless replay; divergence and compatibility reports | Universal bitwise cross-platform/version promise; presentation recording as determinism proof; real external-effect repetition |

Each ADR has decision status `Accepted` and implementation status `Not started`. Option A is the accepted choice in every case; this workshop records the decisions but does not claim implementation.

Acceptance explicitly amends ADR 0014's first-release floor: ADR 0039 adds bounded inspection to diagnostics, ADR 0040 adds the published Test SDK and qualification floor, and ADR 0041 adds bounded single-world authoritative diagnostic replay. ADR 0041 also lifts only the replay-specific 1.0 deferrals in accepted ADRs 0042 and 0043; it does not reopen their typed message, commit, identity, mapping, or compatibility mechanisms. Any later narrowing requires an explicit superseding decision.

## Recommended review order and dependencies

1. **ADR 0039 first:** the observation envelope, committed-boundary rule, attachment binding, identity labels, provenance, and redaction model establish the common inspection contract.
2. **ADR 0040 second:** isolated fixtures consume that inspection contract and prove that accepted world, time, lifecycle, ownership, and scheduler boundaries are usable by an external game.
3. **ADR 0041 third:** authoritative replay consumes the inspection and supported-runtime contracts, uses ordinary world construction and scheduling, and reports compatibility and divergence through those accepted contracts.

The ADRs remain separate decisions despite their shared review, but their implementation dependency is strict. ADR 0040 uses accepted ADR 0039 through the reviewed `OBS-INSPECTION` technical contract and does not authorize a private test-local observation fallback. ADR 0041 uses the reviewed `OBS-INSPECTION` and `TEST-RUNTIME` technical contracts and does not authorize replay-private substitutes for observation or the supported headless driver. If one of those predecessors is later superseded, its replacement must preserve the depended-on contract or explicitly amend the dependent decision. ADR 0041 deliberately chooses verified in-domain outcome equivalence, rather than universal cross-platform bitwise identity, as the bounded 1.0 replay promise.

## Accepted constraints carried into the decisions

- A world remains one isolated mutable simulation, while several conforming worlds may share immutable catalog generations under ADRs 0011 and 0012.
- Entity, map, frame, network, durable, checkpoint, catalog, session, process, editor, observation, fixture, and replay identities remain distinct.
- Entity birth, structural mutation, relations, ending, pause, timers, and catalog adoption are observed only at complete committed boundaries under ADRs 0015, 0016, 0019, 0020, 0029, and 0037.
- The scheduler's serial mode remains a correctness oracle, and conforming parallel work may not derive authoritative behavior from worker count, thread identity, or completion order.
- Trusted game code receives world-isolation and deterministic-scheduling claims only while conforming to ADR 0026. Unknown native, unsafe, static, or external effects narrow those claims honestly under ADR 0034.
- Client presentation and prediction remain non-authoritative. Interest, visibility, authorization, secrecy, and mere discovery are different decisions; inspection cannot become a side channel around them.
- World checkpoints, map sources, editor histories, authoritative replay artifacts, client prediction buffers, and player-facing recordings are different artifacts.
- External facts must enter authoritative simulation as ordered inputs. Replaying committed outputs must not repeat real external side effects.
- External command callers provide correlation and intended-step data, while the admitting authority assigns the stable sequence and origin ordering key under ADR 0042.
- Observation, test, and replay identities remain typed, owner-scoped, purpose-bound, and non-authorizing under ADRs 0043 and 0044; reconstruction always allocates fresh runtime identities.
- Ordinary test and replay worlds use closed activation plans under ADR 0045, one shared close operation and declared fault profiles under ADR 0046, and valid complete operation-specific compatibility reports under ADR 0047 before publication.
- Development, test, creator, and replay powers are excluded from ordinary production artifacts unless a separately reviewed production-safe capability explicitly declares them.
- Public UGC receives no executable inspector, test fixture, replay host, or trusted extension power merely by using these products.

## Potential first-release baseline

With all three Option A decisions accepted, Robusta 1.0 gains a bounded diagnostic and evidence spine:

- an external developer can inspect one live or paused committed world through stable identities, provenance, redaction, and bounded queries;
- the same developer can create a small headless world from exact content, inject ordered inputs, advance exact steps without sleeping, inspect the result, and dispose it without contaminating another fixture;
- required network journeys can use separately supervised loopback authority and client projections rather than a test-only authority model; and
- an authorized diagnostic capture can reconstruct a bounded single-world authority sequence only after a valid replay-reexecution report admits `VerifiedAuthoritativeReexecution`, suppress real external sinks, compare canonical committed projections and effect intents, and identify the earliest checked divergence.

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

## Accepted review answers

- Discovery of a hidden object or relation is authorization-controlled.
- Arbitrary mutation remains outside the inspector and must use declared game, editor, migration, or operator operations.
- Parallel in-process test isolation is promised only for conforming code; stronger containment uses a separately supervised process.
- Real loopback process journeys remain required networking evidence even when smaller tests use in-process fixtures.
- The 1.0 replay guarantee is canonical outcome equivalence inside a declared exact domain; broader cross-platform domains are earned by evidence rather than assumed.
- Diagnostic replay suppresses all real external sinks and compares committed effect intents plus stable idempotency data, while external responses return only as later ordered inputs.

## Follow-up technical decisions

- Inspection envelope and projection schema; capture consistency; query limits; reviewed inspection compatibility profile; authorization and redaction; operator authentication, audit, and production protocol remain with the CP14 operations decisions.
- Test SDK topology and fixture ownership; ordinary activation; reviewed test-execution or ordinary world-construction compatibility profile plus the CP02 ownership and CP04 world-fault profiles; manual driver and authority-ordered inputs; fake-service contracts; leak audits; loopback supervision and CI budgets.
- Replay manifest and segments; replay-local identities and mappings; authoritative input ledger; random-state capture; reviewed replay-reexecution compatibility and replay-owner fault profiles; canonical state and effect-intent projections; safe replay host; privacy and retention.
- Shared structured differences and provenance that can explain inspection assertions, test failures, replay divergence, and migration comparisons without exposing secrets.

## Sources reviewed

- [World-model question set](world-model-question-set.md#g-testing-and-inspection)
- [Accepted workshop 04: entity lifecycle and simulation time](2026-07-18-world-model-04-entity-lifecycle-and-simulation-time.md)
- [Accepted workshop 05: space, persistence, and preview](2026-07-19-world-model-05-space-persistence-and-preview.md)
- [ADR 0020: deterministic phase scheduler](../decisions/technical/0020-run-fixed-step-worlds-through-a-deterministic-phase-scheduler.md)
- [ADR 0029: phase-scoped access and buffered effects](../decisions/technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [ADR 0035: versioned world checkpoints](../decisions/product/0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [ADR 0042: typed message kinds and transactional structural commits](../decisions/technical/0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043: typed identity and compatibility spine](../decisions/technical/0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044: bounded generated identity declarations](../decisions/technical/0044-generate-bounded-identity-declarations.md)
- [ADR 0045: typed capability graphs and closed activation plans](../decisions/technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [ADR 0046: owner shutdown, acquisition ledgers, and fault profiles](../decisions/technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047: bounded exact compatibility policy profiles](../decisions/technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [Space Station 14 admin tooling and View Variables](https://docs.spacestation14.com/en/community/admin/admin-tooling.html#view-variables)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
- [Space Station 14 server replay recording](https://docs.spacestation14.com/en/server-hosting/server-replay-recording.html)
- The pinned Robusta predecessor, Robust Toolbox, and Space Station 14 revisions listed in the [source notes](../reference/source-notes.md)
