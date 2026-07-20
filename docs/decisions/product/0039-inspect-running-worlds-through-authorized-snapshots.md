# ADR 0039: Inspect running worlds through authorized snapshots

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends if accepted:** ADR 0014 by making the bounded inspection floor an explicit Robusta 1.0 diagnostics requirement
- **Related decisions:** 0002, 0003, 0006, 0007, 0009, 0011-0016, 0019-0024, 0026-0029, 0030, 0031, 0033-0038

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

Publish schema-derived inspection projections for worlds, maps, entities, relations, time, content provenance, and networking. Observe complete committed boundaries, identify freshness and authority, apply role- and subject-aware redaction before results leave the owning process, and route any requested mutation through a separate declared game, editor, or operator command.

This provides a common source for creator UI, command-line tools, automated tests, crash diagnostics, and authorized operations. It requires every supported capability to define an inspection projection, sensitivity, limits, and unavailable outcomes.

### Option B: Let each subsystem expose private debug APIs or reflective object views

This is quick for maintainers and can expose every implementation field. It makes internal layout an accidental contract, produces inconsistent identity and authorization rules, invokes unsafe getters or callbacks, and excludes external games from supported tooling.

### Option C: Inspect by serializing a world checkpoint or copying arbitrary runtime memory

This reuses persistence or debugger machinery, but a checkpoint intentionally excludes transient network, prediction, cache, and presentation state, while raw memory contains native resources, aliases, credentials, and half-updated implementation detail. Neither is an honest live inspection model.

## Current review position

Option A is recommended for review. No decision is accepted by this proposal.

Acceptance includes the stated amendment to ADR 0014's diagnostics floor. If first-release inspection is not intended, ADR 0014 must instead be revised explicitly; it must not be broadened or narrowed through implementation alone.

If accepted, the product contract will be:

1. An **inspection observation** is a read-only, typed view of one named target at one identified observation boundary. Its envelope distinguishes authority process, world, map, entity, client or session subject where relevant, simulation step, catalog generation, schema version, and capture freshness. It is not a runtime handle, checkpoint, replay, editor document, or permission grant.
2. Authoritative world inspection observes one complete committed boundary. It never reports a half-applied lifecycle, relationship, timer, catalog-adoption, or structural transaction. A live query may report that its target changed or ended before capture rather than combining values from different boundaries.
3. The first-release inspection floor covers world health and time; runtime map and frame identity; entity identity and lifecycle; component and capability presence; declared authoritative field values; definition, package, schema, birth-generation, and migration provenance; spatial parent, containment, attachment, lifecycle ownership, and reference relations; frame-qualified position; admitted timers and delayed-work summaries; and applicable authority, prediction, confirmation, interest, visibility, and replication state.
4. A supported platform or Game SDK capability supplies a stable inspection projection sufficient to explain its user-visible outcomes. Generated schema metadata identifies names, types, provenance, sensitivity, and whether a value is authoritative, predicted, derived, cached, presentation-only, or unavailable. An unavailable or unsupported value has an explicit reason; tools do not guess by reflecting private fields.
5. Inspection distinguishes prototype birth inputs, current mutable world values, explicit current-catalog references, resolved inheritance, applied migrations, and derived values. Display names and source locations aid people but never replace canonical identities.
6. Client, authority, host, session, editor, and durable-service observations remain separately labeled. A client-side inspector sees only state legitimately admitted to that client. Comparing client prediction with authority state requires separate authorization to each target and never copies authority-only secrets into the ordinary client projection.
7. Read authority, discovery authority, and mutation authority are distinct. Learning that a world, entity, component, relation, field, or hidden subject exists can itself require authorization. Redaction happens in the process that owns the unredacted state, before serialization, logging, caching, or transmission, and uses stable omitted, redacted, denied, ended, and unavailable outcomes without revealing hidden values through diagnostic detail. Each production protocol declares its discovery and side-channel threat model, response-shape policy, measurement environment, and timing or traffic tolerance; this contract does not claim absolute non-disclosure through every shared-resource timing channel.
8. Local creator tools, automated tests, authenticated creator-authority sessions, and remote operator tools use the same versioned inspection model but receive different declared capabilities. Public UGC cannot acquire executable inspection power, and administrator or creator status never implies launcher, package-manager, credential, or unrestricted host authority.
9. Inspection is read-only and does not invoke arbitrary game getters, callbacks, script, filesystem access, network access, or simulation work. A requested change uses a separately authorized, validated, audited game command, map-document transaction, catalog migration, or operator operation with its own lifecycle and failure result. There is no general `set field`, `invoke method`, or private component mutation promise.
10. Queries declare scope, projection, filtering, ordering, maximum work, maximum result size, timeout, and cancellation behavior. Enumeration and pagination use stable continuation identities or explicitly report that the observed generation changed. A large or adversarial query cannot pause a world indefinitely, allocate without bound, or starve gameplay, networking, supervision, or diagnostics.
11. Paused worlds remain inspectable at their last committed boundary. A faulted or integrity-unknown world exposes only observations captured through a boundary that remains trustworthy, plus host-owned fault and provenance records; inspection does not execute more game code in an attempt to recover arbitrary state.
12. Inspection results and audit records follow explicit retention, privacy, and disclosure rules. Secret values, credentials, tokens, raw personal data, and undeclared native memory are never included merely because a caller has general debug access. Production endpoints are disabled unless an operator deliberately configures the appropriate authenticated projection.
13. Ordinary production client and authority packages contain only the minimum inspection producers needed for supported diagnostics and declared operator use. Creator-only UI, workspace access, private draft inspection, and permissive development endpoints remain in separately declared creator artifacts and are absent from ordinary production projections.
14. First release requires structured local and authenticated operator inspection, headless query support, source provenance, relation and network explanation, redaction, pagination, and capture at committed boundaries. Arbitrary historical time travel, fleet-wide distributed joins, private-memory browsing, a universal mutation console, and automatic root-cause inference are later capabilities or explicit non-goals.

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
- A creator compares a predicted client object with its authoritative counterpart and receives separately labeled step, confirmation, interest, visibility, and schema information without transmitting unrelated server-only state to the client.
- Inspection during component mutation, entity ending, cross-map movement, and catalog adoption returns one complete before or after boundary, never a mixed result. An ended target produces the declared ended or stale outcome.
- Two worlds sharing one immutable catalog are inspected concurrently; every observation retains the correct world and generation identity and neither query can access the other's mutable state without separate authority.
- A paused world reports its stable committed state. A deliberately faulted world exposes the last trustworthy boundary and host-owned fault record without invoking another game callback.
- A large station-like world is queried with pagination, cancellation, size limits, and a slow consumer. Gameplay and supervision retain their budgets, continuation behavior is explicit when the observed generation changes, and oversized requests fail predictably.
- An attempted reflective field read, arbitrary setter, unauthorized remote request, public-UGC escalation, and secret-bearing diagnostic are rejected with stable source-aware or policy-aware results.
- Headless CLI, automated test, creator UI, and authenticated operator views decode the same versioned observation schema, while package scans find no workspace watcher, creator draft access, or permissive development endpoint in ordinary production client and authority artifacts.

## Implementation notes

No inspection schema, observation boundary, provenance projection, redaction policy, query engine, remote protocol, UI, or conformance evidence exists. Implementation status remains `Not started`.

## Follow-up decisions

- Inspection envelope, schema generation, stable identity vocabulary, and compatibility rules.
- Commit-boundary capture, immutable observation, pagination, cancellation, and resource-budget mechanisms.
- Sensitivity declarations, subject-aware authorization, redaction, audit, retention, and timing-disclosure policy.
- Platform and game-defined inspector contribution contracts and diagnostics for unavailable values.
- Authority/client comparison, interest-decision explanation, prediction history, and network transport.
- Creator and operator UI, headless query language, production projection, and remote deployment rules.
- Fault-safe last-boundary capture and the boundary between inspection, profiling, tracing, and replay.

## References

- [ADR 0002](0002-judge-quality-through-user-outcomes.md)
- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0015](0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0023](../technical/0023-generate-versioned-authoritative-replication-schemas.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0037](0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [World-model question 24](../../workshops/world-model-question-set.md#24-how-should-a-developer-inspect-a-running-world)
- [Space Station 14 admin tooling and View Variables](https://docs.spacestation14.com/en/community/admin/admin-tooling.html#view-variables)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
