# World Model Workshop 04: Entity lifecycle and simulation time

- **Workshop status:** Proposed for review
- **Date:** 2026-07-18
- **Questions:** 5-12 from the world-model question set
- **Decision outcome:** ADRs 0015 and 0016 are proposed; neither is accepted

## Why these questions are together

Entity birth, structural change, and death need a boundary at which observers see a complete state. Simulation steps, pause, and timers determine where such a boundary can exist. Discussing either half alone would quietly constrain the other.

The proposals remain product-level. They state what developers and players can rely on, while deferring entity storage, handles, scheduler phases, timer queues, clock APIs, thread models, prediction buffers, and serialization.

## Accepted constraints

The recommendations preserve the decisions already accepted:

- ADR 0003 requires ordinary component, system, event, prototype, and networking work through the supported Game SDK.
- ADR 0006 makes the server authoritative while allowing bounded client prediction and smoothing.
- ADR 0011 makes time, entities, systems, events, timers, and random state belong to one isolated world.
- ADR 0012 keeps host and player-session responsibilities separate from mutable world state.
- ADR 0013 makes identity and lifecycle fundamental to an entity while transform, name, prototype origin, networking, and saving remain optional.

## Current predecessor behavior and lesson

Robust Toolbox has explicit entity and component lifetime events, supports dynamic component changes, separates tick from frame updates, and processes queued deletion at a defined point in its update work. These are useful precedents. They also show why game-facing guarantees should not depend on learning engine-internal stage values, deletion recursion, or update-loop order.

The greenfield Robusta projects remain scaffolds. The prototype-era implementation is evidence only, and the accepted baseline contains no decision for atomic publication, stale-reference outcomes, fixed steps, pause, or timer persistence.

## Proposal A: atomic, observable entity lifecycle

[ADR 0015](../decisions/product/0015-give-entities-an-atomic-observable-lifecycle.md) recommends:

- prepare and validate an entity before ordinary world code can discover it;
- publish birth, capability changes, and semantic death as complete world changes;
- leave the old state intact when preparation fails;
- remove an ending entity from live use before cleanup finishes;
- cancel work according to declared ownership, while relationships explicitly choose end, detach, rehome, transfer, or rejection behavior;
- prevent old references from ever resolving to replacement entities;
- make required stale-target use fail clearly, optional lookup return absence, and only explicitly best-effort late work become an inspectable no-op;
- leave save retention, durable identity, and cross-world transfer to later decisions.

### Alternatives deliberately not recommended

- Publish incomplete entities and finish initialization while they are already discoverable.
- Apply component changes immediately wherever they are requested.
- Make all entities immutable after birth.
- Use one stale-reference behavior for required mutation, optional lookup, and late cleanup.
- Cascade every deletion merely from spatial parenting or an ordinary reference.

## Proposal B: fixed simulation time separated from host and presentation time

[ADR 0016](../decisions/product/0016-separate-simulation-host-and-presentation-time.md) recommends:

- advance authoritative gameplay in numbered fixed-duration world steps;
- use host time to schedule step attempts, not as gameplay time;
- perform bounded catch-up without skipping or stretching authoritative steps, then expose sustained overload as slowdown and health diagnostics;
- make whole-world pause take effect between steps and freeze world simulation while host, session, administration, inspection, and teardown work may continue;
- reject ordinary gameplay input received while paused rather than silently replaying it after resume;
- use simulation time for ordinary delayed work, cancel owned work when its owner ends, and never silently resume ordinary timers after process restart;
- keep durable wall-clock schedules in explicit host or durable services;
- allow client interpolation, cosmetics, and permitted prediction on presentation time without granting authority;
- defer a stronger replay and bitwise cross-platform determinism promise.

### Alternatives deliberately not recommended

- Variable wall-clock delta for authoritative gameplay.
- Silent step dropping or enlarged steps under load.
- Unlimited catch-up.
- Arbitrary nested platform pause clocks for maps, entities, or systems.
- Wall-clock timers as the default for world gameplay.
- Rendering locked to the authoritative step rate or allowed to change shared state.

## Decision points requiring explicit approval

1. Birth, capability mutation, and death publish only complete states at explicit world boundaries.
2. Relationship cleanup uses declared ownership and disposition instead of universal cascade deletion.
3. Required, optional, and explicitly best-effort stale-target operations have different but documented outcomes.
4. Authoritative gameplay uses fixed-duration steps, bounded catch-up, no skipped or stretched steps, and visible slowdown under sustained overload.
5. Whole-world pause is the only standard platform pause domain in 1.0.
6. Ordinary gameplay input received while paused is rejected instead of queued silently.
7. Ordinary timers use simulation time and do not silently resume after process restart; durable wall-clock schedules and any future persisted delayed work are separate explicit contracts.
8. Presentation time remains non-authoritative and always accepts server correction and removal.
9. Complete replay and bitwise cross-platform determinism remain deferred to the persistence and tooling gate.

Approval may accept both proposals, accept either proposal independently, or request changes by decision-point number. Until then, both ADRs remain `Proposed`, the catalog's entity/time gate remains open, and public entity, scheduler, timer, pause, and presentation contracts remain unfrozen.

## Technology-neutral proof

The proposals define observable fixtures for atomic birth and rollback, atomic capability changes, declared relationship cleanup, stale-reference non-aliasing, world disposal, exact headless step advancement, frame-rate independence, overload reporting, pause behavior, timer ownership and cancellation, and non-authoritative client smoothing.

Those fixtures supplement the accepted cross-capability scenarios in [`m1-behavioral-scenarios.json`](../specifications/m1-behavioral-scenarios.json). Stable executable names will be added after the decisions are accepted; no runtime evidence is claimed now.

## Technical questions deferred

- Entity handle and storage representation.
- Lifecycle states, preparation and rollback, structural queues, and event ordering.
- Relationship ownership metadata and cleanup scheduling.
- Scheduler phases, input admission, clock sources, and catch-up budgets.
- Timer ordering, repetition, ownership, and persistence representation.
- Random streams, replay recording, and numerical determinism.
- Prediction, interpolation, correction, and cosmetic lifetimes.

## Sources reviewed

- [Robust Toolbox ECS documentation](https://docs.spacestation14.com/en/robust-toolbox/ecs.html)
- [Current Robust Toolbox entity manager](https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/EntityManager.cs)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
- Accepted Robusta ADRs 0003, 0006, and 0011-0013 in this repository
