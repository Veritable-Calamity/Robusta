# ADR 0019: Use generational entity handles and transactional structural commits

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0006, 0011, 0012, 0013, 0015, 0016
- **Related decisions:** 0017, 0018, 0020, 0023

## The question

How will the runtime represent world-local entities and commit birth, capability changes, and death so ordinary code sees only complete states and stale handles cannot alias replacements?

## The promise preserved

Entity lifecycle outcomes are atomic and observable. Required stale use fails, optional lookup returns absence, declared best-effort late work is inspectable, and reused storage never revives an old reference.

## Why this matters

An entity store, public handle, and structural update path will otherwise freeze accidental semantics into the SDK, scheduler, network layer, and saves.

## Options considered

### Option A: World token, slot, generation, and staged structural transaction

Use an opaque handle validated against one world's generation table. Prepare changes in isolated staging, validate them, then commit at a scheduler boundary before publishing observations.

### Option B: Monotonic integer identifiers with immediate mutation

This is simple but does not protect against wrong-world use or partial observation and eventually makes reuse or unbounded growth problematic.

### Option C: Object references as entity identity

Object references couple identity to allocation, are unsafe across serialization and networking, and make stale-state diagnostics weak.

## Decision

Robusta will use Option A:

1. The public `EntityRef` is an opaque value containing or resolving through a world-instance token, slot, and generation. It is never a network or durable identity.
2. Each world owns a generation table. Releasing a slot advances its generation before reuse; generation exhaustion retires the slot rather than wrapping into aliasing.
3. Lifecycle states are `Preparing`, `Live`, `Ending`, and `Ended`. Only `Live` entities participate in ordinary queries and gameplay delivery.
4. Birth and component add, remove, or replacement use a structural transaction: allocate staging, resolve dependencies, validate side and authority, acquire reversible resources, then commit all indexes and component stores together.
5. A failed preparation rolls back in reverse acquisition order and publishes no live handle or committed observation.
6. Structural requests made during a step enter a world-owned command buffer. ADR 0020 defines its commit phase and deterministic ordering.
7. Commit updates storage and query indexes before emitting one immutable lifecycle-change record. Observers cannot mutate the transaction being observed.
8. Ending first removes the entity from live indexes and advances its generation, then runs declared relationship dispositions and best-effort cleanup. Cleanup cannot restore `Live`.
9. APIs distinguish `RequireLive`, `TryResolve`, and explicitly named best-effort delivery. Failures carry target classification and world/operation diagnostics.
10. Component storage layout remains implementation-private; the handle table and transaction semantics do not require one permanent archetype or sparse-set design.

## What we deliberately will not do

- Expose slot or generation arithmetic as game logic.
- Apply structural mutations reentrantly inside arbitrary callbacks.
- Use an ordinary reference or spatial parent as implicit ownership.
- Serialize `EntityRef` as a save or network identity.

## Consequences

### Compatibility and migration

Legacy raw IDs and immediate component mutation require analyzers and explicit structural commands. Save, map, network, and transfer identities need separate mapping decisions.

### Security

Handle validation prevents accidental or malicious cross-world and stale targeting inside supported APIs, but does not sandbox trusted game code.

### Operations

Diagnostics expose lifecycle state, world identity, operation, and provenance without exposing reusable storage internals. Cleanup failures are aggregated and attributed.

## How we will prove the decision works

- `AtomicPublicationAndFailedBirthRollback` and `AtomicCapabilityChangeAndOwnedWorkCancellation` pass under injected failure at every preparation stage.
- `DeclaredEndingAndStaleReferenceSafety` fuzzes slot reuse, cross-world handles, malformed values, repeated ending, and cleanup failures.
- Query snapshots show exactly one complete before state and one complete after state.
- Network conformance proves late work for a removed entity cannot affect a replacement.
- Allocation and query benchmarks publish representative budgets without changing public identity semantics.

## Implementation notes

No entity handle, store, transaction, or structural command buffer exists in the greenfield runtime.

## Follow-up decisions

- Scheduler phases and lifecycle observation ordering.
- Relationship ownership and containment after the space product gate.
- Network and durable identity mapping.

## References

- [ADR 0013](../product/0013-use-entities-for-independent-world-participants.md)
- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [Lifecycle behavioral scenarios](../../specifications/product-behavior-scenarios.json)
