# ADR 0039: Inspect running worlds through authorized snapshots

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Last reconciled:** 2026-07-24
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends:** ADR 0014 by making the bounded inspection floor an explicit Robusta 1.0 diagnostics requirement
- **Related decisions:** 0002, 0003, 0006, 0007, 0009, 0011-0016, 0019-0024, 0026-0029, 0030, 0031, 0033-0038, 0042-0051

## The question

How should a developer or authorized operator inspect a running world, including an object's components, definition origin, relationships, position, network state, and lifecycle, without changing the world or exposing information the observer is not permitted to know?

## The promise

A supported Robusta capability can explain its current committed meaning through one published, source-aware inspection model. Inspection is read-only, identifies its target and observation boundary, respects authority and secrecy, remains bounded on large worlds, and never requires private reflection or an engine-only back door.

## Why this matters

Station-like games combine thousands of entities, inherited content, nested containment, multiple coordinate frames, prediction, interest management, hidden roles, and frequent lifecycle changes. A raw object dump cannot explain which value came from a prototype, which value is now mutable world state, why an object is not replicated, or whether a reference is stale. It can also disclose antagonist roles, inventory contents, administrator data, or other secrets merely by proving that an otherwise hidden object exists.

Inspection is part of the quality bar in ADR 0002. If every subsystem invents a private debug window, external games cannot rely on it, headless tests cannot consume it, and release operators receive behavior different from creators. Conversely, an unrestricted read/write variable browser would make debugging convenient by silently bypassing validation, authorization, lifecycle, and transaction boundaries.

## How Robust Toolbox answers today

Robust Toolbox and Space Station 14 provide productive debug overlays, console tooling, entity lists, and View Variables for examining client and server components. The SS14 prediction guide recommends comparing server and client values to diagnose misprediction. These tools prove the value of live in-context inspection, while their powerful variable mutation path and administrator visibility also show why read authority, write authority, secrecy, and supported game operations need explicit separation.

## How the Robusta prototype answers today

The predecessor and current scaffold have no accepted inspection schema, committed-snapshot contract, source-provenance view, redaction policy, remote inspection protocol, or resource-bounded query surface. No runtime inspector implementation is claimed.

## Options considered

### Option A: One capability-scoped, read-only snapshot and query model

Publish schema-derived inspection projections for worlds, maps, entities, relations, time, content provenance, and networking. Capture immutable post-phase or post-step observations only after every included value write and structural publication agrees, identify freshness and owning authority, apply role- and subject-aware redaction before results leave the owning process, and route any requested authoritative mutation through a separate declared command.

This provides a common source for creator UI, command-line tools, automated tests, crash diagnostics, and authorized operations. It requires every supported capability to define an inspection projection, sensitivity, limits, and unavailable outcomes.

### Option B: Let each subsystem expose private debug APIs or reflective object views

This is quick for maintainers and can expose every implementation field. It makes internal layout an accidental contract, produces inconsistent identity and authorization rules, invokes unsafe getters or callbacks, and excludes external games from supported tooling.

### Option C: Inspect by serializing a world checkpoint or copying arbitrary runtime memory

This reuses persistence or debugger machinery, but a checkpoint intentionally excludes transient network, prediction, cache, and presentation state, while raw memory contains native resources, aliases, credentials, and half-updated implementation detail. Neither is an honest live inspection model.

## Decision

Robusta will use Option A.

This decision includes the stated amendment to ADR 0014's diagnostics floor. Any later removal or narrowing of first-release inspection requires an explicit superseding decision; it must not happen through implementation alone.

Accepted ADRs 0042-0051 constrain the command, identity, activation, fault, compatibility, state, storage, query, and structural-publication boundaries used below. Their acceptance does not implement this product outcome or activate an inspection schema, codec, endpoint, or SDK surface.

The product contract is:

1. An **inspection observation** is a read-only, typed view of one named target at one identified observation boundary. Its envelope keeps the owning-authority identity, target kind and target identity, session-world attachment identity where relevant, simulation step, catalog-generation identity, schema identity, and capture freshness in distinct typed fields; none substitutes for another. It is not a runtime handle, checkpoint, replay, editor document, mapping, or permission grant.
2. Authoritative world inspection captures one immutable post-phase or post-step observation only after all included phase-local component-value writes and the corresponding structural publication, indexes, and commit records agree. A structural frontier alone does not stabilize earlier or concurrent value writes. Capture never exports a phase borrow, retains mutable storage, races a live writer, or invokes game code to reconstruct a value. It reports one complete boundary or a typed changed, ended, or unavailable outcome rather than combining values from different boundaries.
3. The first-release inspection floor covers world health and time; runtime map and frame identity; entity identity and lifecycle; component and capability presence; declared authoritative field values; definition, package, schema, birth-generation, and migration provenance; spatial parent, containment, attachment, lifecycle ownership, and reference relations; frame-qualified position; admitted timers and delayed-work summaries; and applicable authority, prediction, confirmation, interest, visibility, and replication state. Each projected field group names exactly one owning domain and publication boundary.
4. A supported platform or Game SDK capability supplies a stable inspection projection sufficient to explain its user-visible outcomes. Generated schema metadata identifies names, types, provenance, sensitivity, and whether a value is authoritative, predicted, derived, cached, presentation-only, or unavailable. An unavailable or unsupported value has an explicit reason; tools do not guess by reflecting private fields.
5. Cross-version inspection admission uses exact immutable compatibility descriptors, exact resolved policy state, and a reviewed and approved inspection profile under ADR 0047. Before a cross-version observation or endpoint becomes visible, the operation authority must receive one valid complete compatibility report selecting the permitted inspection mode. Successful decoding proves neither compatibility nor authority, and `ReadOnlyInspection` can admit only its named read-only mode; it grants no discovery, resolution, or mutation power.
6. Inspection distinguishes prototype birth inputs, current mutable world values, explicit current-catalog references, resolved inheritance, applied migrations, and derived values. Display names and source locations aid people but never replace canonical identities.
7. Client, authority, host, session, editor, and durable-service observations remain separately labeled. Interest, visibility, replication, prediction, confirmation, avatar association, and other attachment-scoped facts name exactly one owning domain and are captured only through an owner-issued live attachment binding carrying its `SessionWorldAttachmentId` and protocol epoch. Detach, reconnect, world replacement, or binding retirement invalidates attachment-scoped targets, mappings, and cursors. A client-side inspector sees only state legitimately admitted to that client. Client-versus-authority comparison consists of two separately authorized and separately captured observations with independent boundaries and freshness; it promises no cross-owner simultaneity and never copies authority-only secrets into the ordinary client projection.
8. Read authority, discovery authority, and mutation authority are distinct. Learning that a world, entity, component, relation, field, or hidden subject exists can itself require authorization. Redaction happens in the process that owns the unredacted state, before serialization, logging, caching, or transmission, and uses stable omitted, redacted, denied, ended, and unavailable outcomes without revealing hidden values through diagnostic detail. Each production protocol declares its discovery and side-channel threat model, response-shape policy, measurement environment, and timing or traffic tolerance; this contract does not claim absolute non-disclosure through every shared-resource timing channel.
9. Local creator tools, automated tests, authenticated creator-authority sessions, and remote operator tools use the same versioned inspection meaning but receive different declared capabilities. Public UGC cannot acquire executable inspection power, and administrator or creator status never implies launcher, package-manager, credential, or unrestricted host authority.
10. Inspection capture and projection are read-only and perform no arbitrary I/O, game getters, callbacks, script execution, filesystem access, network access, or simulation work. A separately governed transport may transmit only an already authorized and redacted result. Any requested change to authoritative world state enters a separately authorized and validated ADR 0042 command and receives that command's immutable terminal result; submission alone is not success. Creator-document transactions, catalog-adoption workflows, and non-world operator operations remain separately governed and cannot become a private component mutation path. There is no general `set field`, `invoke method`, or private component mutation promise.
11. Queries declare scope, projection, filtering, ordering, maximum work, maximum result size, timeout, and cancellation behavior. A continuation cursor is opaque, bounded, scoped to exactly one immutable observation, and carries no resolution or authority; it becomes invalid when that observation or its required live attachment binding ends. Any external cursor encoding remains forbidden until the inspection owner activates a reviewed ADR 0044 surface. Enumeration either remains on its named observation or explicitly reports that a fresh live capture would cross a generation boundary. A large or adversarial query cannot pause a world indefinitely, allocate without bound, or starve gameplay, networking, supervision, or diagnostics.
12. A paused but still-open world remains inspectable at its most recent published observation boundary. Once the ADR 0046 owner-closing admission fence wins, no new inspection query resolves or executes against that owner. Inspection may then expose only a pre-fence immutable observation captured while the relevant integrity was `KnownSound` and retained or transferred through the acquisition ledger under a named external retention authority with a proven terminal postcondition, plus host- or supervisor-owned sealed close and fault reports and their bounded late-incident chain. If that evidence is absent, contradictory, or untrustworthy, especially when host integrity is `Unknown`, inspection returns `Unavailable`; it does not promise that a last snapshot always exists or run more game code to manufacture one.
13. Inspection results and audit records follow explicit retention, privacy, and disclosure rules. Secret values, credentials, tokens, raw personal data, and undeclared native memory are never included merely because a caller has general debug access. Production endpoints are disabled unless an operator deliberately configures the appropriate authenticated projection.
14. Ordinary production client and authority packages contain only the minimum inspection producers needed for supported diagnostics and declared operator use. Creator-only UI, workspace access, private draft inspection, and permissive development endpoints remain in separately declared creator artifacts and are absent from ordinary production projections.
15. First release requires structured local and authenticated operator inspection, headless query support, source provenance, relation and network explanation, redaction, pagination, and capture at committed boundaries. Arbitrary historical time travel, fleet-wide distributed joins, private-memory browsing, a universal mutation console, and automatic root-cause inference are later capabilities or explicit non-goals.

## Authority and retained gates

This decision amends only ADR 0014's diagnostics floor and unlocks the `OBS-INSPECTION` technical work package. It does not by itself define or implement the observation schema, activate an ADR 0044 diagnostic or protocol codec, authorize public Game SDK inspection contributions, approve the ADR 0047 inspection compatibility profile, create a remote transport or endpoint, or grant any caller inspection authority.

`OBS-INSPECTION` must still define the observation and target identity declarations, owner-issued bindings, purpose-bound mappings, schema and bounded query protocol, immutable capture mechanism, redaction and retention behavior, and conformance evidence. Cross-version, public, or remote use additionally waits for a reviewed and approved `FND-COMPAT` inspection profile; workload limits wait for `FND-BUDGET` evidence.

Production operator authentication, grants, endpoints, command policy, and audit remain `OPS-ADMIN` work at CP14. Logs, metrics, traces, health, crash reporting, fault aggregation, privacy, cardinality, and operational retention remain `OPS-OBSERVABILITY` work, with ADR 0046 owning the close/fault report semantics. Those surfaces may project authorized inspection data but do not become inspection protocols by implication.

## What we deliberately will not do

- Treat reflection over language objects as the supported inspection API.
- Combine read access and arbitrary mutation in one inspector permission.
- Reveal a hidden entity, field, relation, or network decision merely by returning a more detailed denial.
- Run game callbacks or acquire unbounded world locks to format an observation.
- Confuse display names, runtime handles, network identities, durable identities, and catalog identities.
- Promise an arbitrary whole-world dump, distributed live query, or historical time-travel debugger in 1.0.
- Ship creator-only inspection or workspace powers in ordinary production game projections.

## Consequences

### Benefits

- Developers can explain live behavior without depending on Robusta internals.
- Creator UI, tests, diagnostics, and operator tools share one meaning and provenance vocabulary.
- Prediction, interest, containment, catalog adoption, and lifecycle problems become distinguishable.
- Read-only inspection can remain available during pause and bounded fault diagnosis without becoming a mutation escape hatch.
- Secrecy and resource limits are designed into inspection instead of added after an information leak or operational stall.

### Costs and limitations

- Every supported capability needs projection metadata, sensitivity classification, stable diagnostics, and bounded traversal behavior.
- Capturing a coherent view may require immutable observations, versioned reads, or bounded coordination at commit boundaries.
- Source-aware and authority-aware tools are more complex than raw object browsers.
- Redacted observations may be insufficient for an operator lacking the necessary role, by design.
- Historical replay, profiling, and mutation workflows remain separate products.

## How we will prove the decision works

- An external station-like game developer inspects a damaged door and sees its runtime identity, lifecycle, components, current durability, prototype and package origin, birth catalog generation, applied migrations, containment or attachment relations, and frame-qualified position without private engine access.
- A contrasting game's hidden card and a station-like antagonist role are absent or redacted for an ordinary client and visible only to an explicitly authorized authority-side observer. Under the versioned protocol threat model and controlled measurement environment, unauthorized responses use the declared common shapes, counts, continuation behavior, and measured timing or traffic tolerance; residual channels and limits remain documented rather than being claimed away.
- A creator compares separately captured predicted-client and authoritative observations and receives independently labeled step, freshness, confirmation, interest, visibility, attachment epoch, and schema information without a simultaneity claim or transmission of unrelated server-only state to the client. Detach and reconnect invalidate the old attachment targets, mappings, and cursors.
- Inspection during component-value mutation, entity ending, cross-map movement, and catalog adoption returns one immutable post-phase or post-step boundary after included values, structures, indexes, and commit records agree, never a mixed result. An ended target produces the declared ended or stale outcome.
- Two worlds sharing one immutable catalog are inspected concurrently; every observation retains the correct world and generation identity and neither query can access the other's mutable state without separate authority.
- A paused open world reports its stable published observation. After an owner-closing fence, a deliberately faulted world exposes a pre-fence `KnownSound` observation only when it was ledgered under a named external retention authority, plus the sealed host or supervisor fault report and bounded late incidents. Missing or integrity-unknown evidence returns `Unavailable` without owner resolution or another game callback.
- A large station-like world is queried with pagination, cancellation, size limits, and a slow consumer. Gameplay and supervision retain their budgets, continuation behavior is explicit when the observed generation changes, and oversized requests fail predictably.
- An attempted reflective field read, arbitrary setter, unauthorized remote request, public-UGC escalation, and secret-bearing diagnostic are rejected with stable source-aware or policy-aware results.
- Cross-version fixtures admit an observation only through exact descriptors and a valid complete report under the reviewed inspection profile; decoding or a `ReadOnlyInspection` outcome grants no additional authority.
- Headless CLI, automated test, creator UI, and authenticated operator views decode the same versioned observation schema through separately authorized projections, while package scans find no workspace watcher, creator draft access, or permissive development endpoint in ordinary production client and authority artifacts.

## Implementation notes

No inspection schema, ADR 0044 inspection identity declaration or codec surface, ADR 0047 inspection profile, immutable value-and-structure observation boundary, owner-issued attachment binding, provenance projection, redaction policy, query engine, fault-safe retention integration, remote protocol, UI, or conformance evidence exists. Implementation status remains `Not started`.

## Follow-up decisions

- Inspection envelope, target and cursor identity declarations, ADR 0044 surface profiles, purpose-bound mappings, and an exact ADR 0047 inspection compatibility profile.
- Commit-boundary capture, immutable observation, pagination, cancellation, and resource-budget mechanisms.
- Sensitivity declarations, subject-aware authorization, redaction, audit, retention, and timing-disclosure policy.
- Platform and game-defined inspector contribution contracts and diagnostics for unavailable values.
- Authority/client comparison, interest-decision explanation, prediction history, and network transport.
- Creator and operator UI, headless query language, production projection, and remote deployment rules.
- ADR 0046-safe immutable retention and the boundary between inspection, profiling, tracing, crash reporting, and replay.

## References

- [ADR 0002](0002-judge-quality-through-user-outcomes.md)
- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0015](0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0023](../technical/0023-generate-versioned-authoritative-replication-schemas.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0037](0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [ADR 0042](../technical/0042-use-typed-message-kinds-and-transactional-structural-commits.md)
- [ADR 0043](../technical/0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](../technical/0044-generate-bounded-identity-declarations.md)
- [ADR 0045](../technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md)
- [ADR 0046](../technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md)
- [ADR 0047](../technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md)
- [World-model question 24](../../workshops/world-model-question-set.md#24-how-should-a-developer-inspect-a-running-world)
- [Space Station 14 admin tooling and View Variables](https://docs.spacestation14.com/en/community/admin/admin-tooling.html#view-variables)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
