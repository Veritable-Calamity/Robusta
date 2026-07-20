# ADR 0032: Reconstruct explicitly across world transfers

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0006, 0008, 0011-0016, 0019, 0023, 0028, 0030, 0031

## The question

What remains the same when an object moves between maps in one world, and what happens when gameplay transfers a player or durable object to another world?

## The promise

Same-world relocation preserves one world-local entity identity. Cross-world transfer never lets an ordinary entity reference resolve across worlds; it reconstructs declared transferable state with explicit success, failure, and durable-continuity outcomes.

## Why this matters

ADR 0011 forbids ordinary cross-world entity references, while ADR 0012 lets sessions survive world replacement. Games still need portals, shuttles, match changes, character travel, and editor operations. Treating all movement alike would either break identity unnecessarily or leak one world's state into another.

## How Robust Toolbox answers today

Robust Toolbox primarily models map movement inside one entity manager. Cross-server or independently owned simulation transfer is game-specific, so raw entity identity and references are not a suitable portability contract.

## How the Robusta prototype answers today

The predecessor has a singular-world assumption and no transactional cross-world transfer contract.

## Options considered

### Option A: Preserve identity inside a world; export and reconstruct across worlds

Map relocation is one atomic spatial change. Cross-world transfer exports a declared graph, validates it in the target, creates new target-local identities, and records any durable continuity explicitly.

### Option B: Preserve the same entity identity across worlds

This appears convenient but contradicts world-scoped identity, complicates concurrency and cleanup, and lets stale references cross isolation boundaries.

### Option C: Leave all transfer behavior to games

This avoids a platform transaction but produces incompatible identity, session, timer, save, networking, and failure rules.

## Decision

Robusta will use Option A.

The product contract is:

1. Moving an entity between maps or spatial frames inside one world is an atomic relation and transform transaction. The entity retains its world-local identity and ordinary world-owned state unless a declared relation policy changes it.
2. A world boundary is a runtime ownership and identity boundary, not necessarily the boundary of one server authority or durable coordinator. A server, supervisor, or transfer coordinator may span worlds, but `EntityRef`, pending commands, live timer or callback identities, events, physics contacts, and network identities do not migrate or resolve into the target world. A declared timer payload and time meaning may instead reconstruct a new target-world timer under a later durable-work contract.
3. Cross-world transfer starts from an explicit export policy naming the root, included owned or related state, excluded state, catalog requirements, durable identities, session intent, and failure behavior.
4. At a committed source-world boundary, the coordinator fences the transferable graph against further authoritative mutation and captures one immutable export revision. No mutation affecting exported state may commit until the transfer aborts and the source resumes or cutover commits and the source disposition completes.
5. The target validates that exact export revision and constructs a new unpublished graph with new world-local entity and network identities. It may not publish a graph reconstructed from a different or partially observed source revision.
6. Any player session detaches and reattaches through the explicit ADR 0028 attachment scope. Connection identity is not an entity identity.
7. A durable character, account, shipment, or similar concept may preserve a separately declared durable identity. That identity links continuity but never aliases the source and target `EntityRef` values.
8. The transfer coordinator exposes fenced, prepared, committed, rejected, aborted, and recovery-required outcomes. It does not claim an impossible instantaneous transaction across process failure.
9. A durable single-use activation record identifies the source fence, immutable export revision, target reconstruction, and coordinator epoch. Before activation, only the fenced source ownership is recognized and the target stays unpublished. After activation, the source can never resume that graph and only the matching target reconstruction may publish.
10. Commit permits at most one active side. Uncertain completion is fenced in a recovery-required state, possibly with neither side active, and recovery consults the durable journal and activation record rather than guessing or creating a second activation.
11. Source entities follow their declared transfer or ending dispositions. Old references become stale and cannot resolve to target entities.
12. Undeclared state, native resources, presentation objects, open UI, transient prediction, and arbitrary callbacks do not transfer.
13. Inspection and diagnostics correlate the source fence and export revision, durable continuity, target reconstruction, session reattachment, activation record, and final disposition without presenting the new entities as the same world-local objects.

## What we deliberately will not do

- Resolve an `EntityRef` in a different world.
- Copy arbitrary live memory, callbacks, tasks, physics state, or presentation objects.
- Preserve network identity across a new session-world attachment.
- Promise exactly-once distributed transfer without a durable coordinator and recovery record.
- Infer the transferable graph from transform ancestry alone.

## Consequences

### Benefits

- World isolation remains true while games can support travel and match changes.
- Same-world portals and map changes avoid unnecessary reconstruction.
- Sessions and durable character continuity stay distinct from entity identity.
- Failure recovery has explicit, auditable states.

### Costs and limitations

- Games must classify transferable state and relationships.
- Cross-world transfer is more expensive than same-world relocation.
- Durable identity, persistence, and coordinator contracts must be completed first.
- Some live effects and timers must be redesigned or deliberately dropped.

## How we will prove the decision works

- An entity moves between two maps in one world and retains its `EntityRef` with one committed before/after outcome.
- A player transfers between two worlds while the session survives, the old attachment closes, and the target receives fresh entity and network identities.
- Old commands, timers, references, and packets cannot affect reconstructed target entities.
- A mutation attempted after the source fence cannot enter the immutable export revision or make the target publish stale state.
- Failure before and after each durable transfer point produces at most one documented active side or a fenced recovery-required state with neither side active, never two silently active copies.
- A nested transferable graph preserves declared durable continuity and applies every excluded or ending disposition.
- A transfer rejected for catalog or schema incompatibility leaves the source authoritative and unchanged.

## Implementation notes

No same-world relocation transaction, transfer representation, coordinator, durable identity, or reconstruction pipeline exists.

For Robusta 1.0, the bounded baseline requires same-world multi-map relocation and session-driven world replacement, not general durable cross-world object-graph transfer. The complete cross-world proof suite gates a later Supported transfer capability unless ADR 0014 is explicitly superseded.

## Follow-up decisions

- Durable identities and save/reference semantics.
- Transfer journal, immutable export revision, activation record, coordinator fencing, retry, and operator recovery.
- Catalog/schema negotiation for target reconstruction.
- Session membership and cross-world avatar policy.
- Reconstruction of declared timer state, delayed work, and external durable services.

## References

- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0012](0012-separate-game-host-and-world-state.md)
- [ADR 0028](../technical/0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [World-model question 16](../../workshops/world-model-question-set.md#16-how-should-moving-between-maps-or-worlds-appear)
