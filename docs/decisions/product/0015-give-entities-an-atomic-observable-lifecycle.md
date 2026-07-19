# ADR 0015: Give entities an atomic, observable lifecycle

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0006, 0010, 0011, 0012, 0013

## The question

How does an entity become live, stop being live, fail through an old reference, and change its capabilities while running?

## The promise

A game developer should never observe a half-created entity, a half-applied capability change, or a replacement entity through an old reference. Birth, structural change, and death should have explicit world boundaries, complete outcomes, notifications, cleanup, and actionable failures.

## Why this matters

Lifecycle behavior reaches almost every later contract: prototypes create entities, systems query components, events observe changes, timers retain targets, networks remove objects, saves retain references, and world disposal must release resources. If these semantics remain accidental, ordinary game code becomes dependent on callback order, storage reuse, or timing quirks that cannot safely evolve.

ADR 0013 says that identity and lifecycle are the minimum meaning of an entity while transform, networking, saving, and prototype origin remain optional. This proposal defines the user-visible lifecycle promise without selecting an entity store, handle layout, callback API, or scheduler.

## How Robust Toolbox answers today

Robust Toolbox exposes several entity and component lifecycle stages. Components receive add, initialize, startup, shutdown, and remove notifications; entities can be created uninitialized, initialized, started, queued for deletion, and recursively terminated through transform children. Its current entity manager also separates queued deletion from immediate deletion and offers both existence checks and resolving operations.

That model demonstrates why rich lifecycle signals are useful, but it also leaves a successor an important design opportunity: make the complete-state boundary, stale-target outcomes, ownership cleanup, and structural-change atomicity part of the supported contract rather than behavior a game learns from engine internals.

## How the Robusta prototype answers today

The greenfield repository contains only SDK and runtime scaffolds. The prototype-era source is evidence rather than the new contract, and no accepted prototype decision in this baseline settles atomic publication, stale references, relationship cleanup, or capability-change visibility. This proposal therefore makes those promises explicit rather than claiming compatibility with prototype behavior.

## Options considered

### Option A: Atomic lifecycle boundaries with context-appropriate stale failures

Prepare and validate birth or capability changes before publishing them. Commit each validated change at one world boundary. Remove an ending entity from live use before cleanup completes. Let required operations fail clearly on stale targets, optional lookup return absence, and explicitly best-effort late work become an inspectable no-op.

This gives systems one complete before state and one complete after state, supports dynamic composition, and avoids forcing optional lookup and required mutation into the same failure shape.

### Option B: Publish first and initialize or mutate in place

Make entities visible as soon as identity is allocated and apply changes immediately. This is mechanically direct, but queries, events, networking, and reentrant game code can observe partial state. Recovery after validation or initialization failure becomes ambiguous.

### Option C: Make entities immutable after birth

Require replacement instead of changing capabilities. This is easy to reason about but conflicts with the accepted component-and-system authoring model and makes ordinary changes such as equipping, transforming, or enabling behavior unnaturally expensive.

### Option D: Use one stale-reference result everywhere

Always throw, always return absence, or always ignore. Uniformity is attractive, but throwing makes optional lookup noisy while silent absence or no-op makes required mutation deceptively succeed.

## Decision

If accepted, Robusta will give every entity a world-scoped, all-or-nothing lifecycle:

1. **Birth is prepared before publication.** An entity may have an identity within controlled construction work, but ordinary queries, systems, events, networking, and game code cannot discover or act on it. Its requested components, initial values, dependencies, and validation complete before one committed publication boundary.
2. **Failed birth publishes nothing.** Failure cleans acquired resources and leaves no live query result, timer, event, or replication record. The diagnostic identifies the failed definition, capability, dependency, or source when known.
3. **Capability changes are atomic.** A live entity may add, remove, or replace optional components when side, authority, dependencies, and game rules allow it. Observers see either the complete old capability set or the complete new set, and receive one committed change observation. Failure leaves the old set intact.
4. **Identity and lifecycle cannot be removed.** Other platform capabilities remain optional unless a later accepted decision requires one for a particular category. Runtime capability changes do not mutate the entity's immutable prototype or catalog generation.
5. **Death has semantic finality before cleanup completes.** Once ending commits, the entity disappears from live discovery and accepts no ordinary gameplay mutation, new timer, or new event. Cleanup may continue, but cannot revive a usable zombie. Cleanup failures remain observable and do not prevent best-effort cleanup of remaining resources.
6. **Ownership, not reference shape, controls cleanup.** Entity- or capability-owned work is cancelled when its owner ends. Parent, child, containment, attachment, and similar relationships declare whether an endpoint's death ends, detaches, rehomes, transfers, or blocks a dependent. A reference alone never implies cascading deletion. World disposal remains able to terminate every world-owned resource.
7. **Stale references never alias replacements.** Reusing internal capacity cannot make an old reference resolve to a new entity. Cross-world, malformed, not-yet-live, ending, and ended targets remain distinguishable where that improves correction.
8. **Failure behavior matches intent and remains explicit.** A command requiring a live target returns a structured stale-target failure or documented exception and never silently succeeds. Optional lookup returns absence. Late delivery, cancellation, or cleanup may safely do nothing only when the operation explicitly declares that best-effort behavior and exposes it to diagnostics or inspection.
9. **Authority remains explicit.** Shared authoritative capability changes and removal are decided by the server. Late client or network work for an ended entity cannot mutate a later entity. Client-only cosmetic state remains separate.

The technical design will define the exact commit boundaries, lifecycle states, validation transaction, observation order, cleanup scheduler, and failure types.

## What we deliberately will not do

- Publish half-initialized entities or partial capability sets.
- Leave successful-looking partial state after validation failure.
- Reuse identity in a way that revives an old reference.
- Treat a required mutation against a stale target as successful.
- Cascade deletion merely because one entity references or spatially parents another.
- Permit cleanup callbacks to resurrect an ended entity.
- Select storage, handle bits, callback names, event transport, scheduler phases, or a threading model in this product ADR.
- Decide save retention, durable identity, or cross-world transfer semantics here.

## Consequences

### Benefits

- Ordinary systems can reason from complete before and after states.
- Failed creation and mutation have testable rollback behavior.
- Dynamic component composition remains available without exposing partial changes.
- Stale handles are safe against internal storage reuse.
- Relationship cleanup becomes deliberate rather than an accidental transform rule.
- Networking, inspection, tests, and later persistence work share one semantic lifecycle.

### Costs and limitations

- Implementations need preparation, validation, rollback, and safe structural boundaries.
- Relationship types must declare disposition rather than inherit one universal cascade.
- APIs must distinguish required use, optional lookup, and explicitly best-effort work.
- Cleanup failures need containment and diagnostics.
- This proposal does not yet define the exact behavior of saved references or entity transfer between worlds.

## How we will prove the decision works

1. A prototype-created door is invisible while being assembled; its first ordinary observer sees every required capability and final initial value.
2. A missing dependency fails creation with provenance and leaves no live entity, timer, query result, event, or replication record.
3. A multi-capability change appears entirely at one boundary or leaves the entity unchanged.
4. Removing a capability cancels its owned delayed work before that work can run.
5. Removing a container applies its declared item disposition; an absent disposition rejects removal without losing either entity.
6. Ending an entity with an owned dependent ends it exactly once, while an unrelated reference merely becomes stale.
7. Required stale-target mutation fails clearly, optional lookup returns absence, and declared late delivery is safely ignored and inspectable.
8. Reusing internal storage never lets an old reference target a new entity.
9. Authoritative removal reaches clients, and delayed work for the removed identity cannot alter a later entity.
10. Disposing one world completes its cleanup without affecting another world, the host, or player sessions.

These scenarios will receive stable names in the behavioral specification after acceptance and executable conformance tests when the corresponding runtime capability exists.

## Implementation notes

No entity lifecycle implementation is claimed. Public entity handles, stores, lifecycle APIs, structural-change scheduling, replication removal, and cleanup mechanisms remain gated by this proposal and later technical ADRs.

## Follow-up decisions

- Technical entity identity, handle validation, lifecycle state, storage, and reuse.
- Structural-change transaction and event ordering.
- Relationship ownership and disposition declarations.
- Cleanup failure and diagnostics policy.
- Network removal and late-message treatment.
- Save references and durable identity.
- Entity and owned-work transfer between worlds.

## References

- [ADR 0003](0003-preserve-straightforward-game-authoring.md)
- [ADR 0006](0006-server-authority-and-declarative-sync.md)
- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0012](0012-separate-game-host-and-world-state.md)
- [ADR 0013](0013-use-entities-for-independent-world-participants.md)
- [Behavioral and technical decision gate (roadmap M1)](../../status/development-plan.md#m1---behavioral-and-technical-gates)
- [World-model questions 5-8](../../workshops/world-model-question-set.md#b-what-is-a-game-object)
- [Robust Toolbox ECS documentation](https://docs.spacestation14.com/en/robust-toolbox/ecs.html)
- [Current Robust Toolbox entity manager](https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/EntityManager.cs)
