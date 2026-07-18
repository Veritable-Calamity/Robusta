# ADR 0016: Separate simulation, host, and presentation time

- **Decision status:** Proposed
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0006, 0009, 0011, 0012, 0014, 0015

## The question

What does one simulation step mean, what pauses, how do delayed actions behave, and what may a client render independently?

## The promise

Gameplay rules should see stable world time regardless of rendering frequency or machine speed. Pause and timers should behave predictably, overloaded hosts should fail visibly instead of silently changing game rules, and clients should remain smooth without presentation becoming authoritative.

## Why this matters

Time affects physics, input, systems, lifecycle commits, timers, random state, networking, testing, replay, and saving. A variable or implicit time model makes the same game behave differently at different frame rates and makes overload indistinguishable from altered game rules.

ADRs 0011 and 0012 already place simulation time, timers, and random state inside an isolated world. ADR 0006 requires server authority while permitting client prediction and smoothing. This proposal defines how those promises meet, without selecting clocks, queues, numeric formats, scheduler phases, or interpolation algorithms.

## How Robust Toolbox answers today

Robust Toolbox distinguishes tick updates from frame updates. Its entity manager updates systems and queued events during tick work and exposes separate frame updates for presentation-facing work. Current Space Station 14 guidance also treats the server result as authoritative while clients may predict or smooth state.

Those are useful foundations. The successor still needs an explicit product promise for fixed steps, overload, pause scope, timer ownership, headless operation, and the boundary between world time and wall-clock services.

## How the Robusta prototype answers today

The greenfield runtime is scaffolded and does not implement simulation time. Prototype-era behavior is evidence only, and this baseline contains no accepted decision that settles fixed steps, catch-up, pause, timer persistence, or presentation time. No compatibility claim is made.

## Options considered

### Option A: Fixed world steps with bounded catch-up and separate presentation time

Advance authoritative gameplay through numbered fixed-duration steps. Use host time only to decide when to attempt steps. Catch up within a published budget; if the host remains behind, slow relative to wall time and report overload rather than skipping or stretching steps. Let clients interpolate and animate independently without gaining authority.

This makes tests and authoritative ordering stable while keeping rendering smooth.

### Option B: Variable wall-clock delta

Advance gameplay by elapsed wall time. This follows real time naturally, but machine load and frame frequency alter physics, timers, ordering, and reproducibility.

### Option C: Skip steps or enlarge the delta under load

Preserve wall-clock pace by dropping work or simulating a larger interval. This hides overload by changing interactions, collision, lifecycle ordering, and timer behavior.

### Option D: Unlimited catch-up

Run every overdue step until current. This preserves simulated elapsed time, but can create an overload spiral that starves networking, administration, and diagnostics.

### Option E: Universal nested pause clocks

Give maps, entities, and systems independent platform pause domains. This is powerful, but makes containment, physics, timers, transfers, and networking ambiguous. Games can model local suspension explicitly without making it a second platform clock.

## Decision

If accepted, Robusta will separate authoritative simulation time, host or durable time, and presentation time:

1. **World simulation uses numbered, fixed-duration steps.** A world declares its simulation rate when created; the base step duration remains fixed for that world's lifetime in the first release. Gameplay input, timers, systems, lifecycle changes, and authoritative events belong to defined steps and observable ordering.
2. **Host time schedules attempts; it is not gameplay time.** Tests and previews can advance an exact number of steps without waiting for a wall clock. External I/O or wall-clock facts affect gameplay only after they become explicit ordered simulation inputs.
3. **Catch-up is bounded and overload is visible.** A host may run overdue steps within a stated budget. It does not silently skip authoritative steps or enlarge their duration. If the budget is insufficient, the world falls behind wall time and reports backlog, duration, and operational health while networking and administration retain their own service budgets.
4. **Whole-world pause is the standard platform pause.** Pause takes effect between complete steps. Simulation time, gameplay systems, physics, random advancement, ordinary lifecycle work, and simulation-time timers stop. Host and session responsibilities such as connections, heartbeats, administration, inspection, diagnostics, pause replication, and teardown may continue.
5. **Paused gameplay input is explicit.** Ordinary gameplay input received while paused is rejected with a paused outcome unless a later accepted contract defines that operation outside simulation. It is not silently queued for resume. Resume continues at the same simulation instant and does not add elapsed wall time.
6. **Local suspension is game state, not another hidden clock.** Games may model stun, disabled systems, frozen objects, or local time effects through explicit rules. Robusta 1.0 does not promise arbitrary platform pause domains for individual maps, entities, or systems.
7. **Ordinary delayed work uses simulation time.** A timer never fires early and becomes eligible at the first simulation boundary at or after its deadline. Pause preserves remaining simulation duration. Equal-deadline and repeating behavior is deterministic and inspectable under the later technical ordering contract.
8. **Delayed work has ownership.** Work owned by an entity, capability, system, or world is cancelled when its owner ends. Merely retaining a reference does not imply ownership; when unrelated delayed work runs, it follows ADR 0015's stale-target behavior. Ordinary world timers do not silently count process downtime, survive world destruction, or move between worlds.
9. **Durable schedules are separate.** Real-world appointments, account expiry, matchmaking, leases, and similar schedules belong to explicit host or durable services with declared overdue and recovery policy. Ordinary world timers do not silently resume after save, load, or process restart. The later persistence contract will decide whether delayed work can opt into persistence and, if so, its representation, ordering, overdue, and recovery behavior.
10. **Presentation time is non-authoritative.** Clients may interpolate confirmed states, run declared cosmetic animation, and make bounded predictions allowed by ADR 0006. Presentation cannot advance world time, fire gameplay timers, commit lifecycle changes, create authoritative entities, or decide shared outcomes. It accepts correction and removal from the server.
11. **Pause and removal bound presentation.** UI and declared cosmetics may continue while paused, but presentation does not extrapolate authoritative simulation beyond the paused state. A cosmetic exit effect may retain detached presentation data after removal; the ended entity is not live or targetable.
12. **Rendering availability never changes authority.** Render frequency, dropped frames, and the absence of rendering on a dedicated server do not change authoritative results. Presentation time is not saved or networked as world truth.

Given the same compatible runtime and catalog, initial state, ordered external inputs, and random seed, declared authoritative ordering should not depend on render rate, wall-clock jitter, or thread races. This proposal does not yet promise bit-for-bit numerical identity across every platform or complete replay determinism; the persistence and replay gate will decide any stronger contract.

## What we deliberately will not do

- Let render rate or wall-clock jitter define authoritative gameplay.
- Skip authoritative steps or silently stretch their duration under overload.
- Change a live world's base step duration in the first release.
- Promise arbitrary per-map, per-entity, or per-system platform pause clocks.
- Silently replay queued gameplay input after resume.
- Let ordinary world timers count process downtime, resume after process restart, or transfer between worlds implicitly.
- Let rendering or cosmetic animation change shared state.
- Promise bitwise cross-platform replay before the replay decision.
- Select clock APIs, timer queues, phase counts, thread models, numeric formats, prediction buffers, or interpolation algorithms here.

## Consequences

### Benefits

- Simulation behavior is independent from rendering frequency.
- Tests and previews can advance exact steps without wall time.
- Overload changes operational health rather than silently changing game rules.
- Pause, delayed work, headless servers, and presentation have clear boundaries.
- Timer cancellation composes with entity and world lifecycle.
- Later network prediction and replay work have an explicit authority boundary.

### Costs and limitations

- Hosts need catch-up budgets, backlog metrics, and overload policy.
- Input admission and ordering must be specified technically.
- Games needing local time effects model them explicitly.
- Durable real-time schedules require a service separate from ordinary world timers.
- Stronger numerical determinism, replay, save, and transfer behavior remains a later decision.

## How we will prove the decision works

1. Two worlds advance independently, and pausing one does not affect the other.
2. An isolated test advances exactly 100 steps without wall-clock waiting and observes exact step numbers and timer order.
3. The same ordered input trace at 30, 60, and 144 rendered frames per second produces the same authoritative lifecycle and timer trace.
4. Under induced overload, no authoritative step is skipped or stretched; backlog and duration are reported.
5. A pause requested during a step takes effect only after that step completes.
6. While paused, simulation time, physics, world timers, and gameplay systems remain unchanged while connections, diagnostics, inspection, and pause replication continue.
7. Gameplay input received during pause is explicitly rejected and never appears after resume.
8. A delayed action retains the same remaining simulation delay across a long pause.
9. Ending a timer owner cancels its work, while unrelated delayed work targeting a stale entity follows ADR 0015.
10. An ordinary world timer does not silently execute after process restart; any future persisted delayed work is exercised under its own accepted persistence contract.
11. A headless server and rendered client agree on authoritative results.
12. Client interpolation and cosmetic exit effects remain smooth while server correction and removal remain final.

These scenarios will receive stable names in the behavioral specification after acceptance and executable evidence as the corresponding runtime and networking capabilities arrive.

## Implementation notes

No scheduler, timer, pause, prediction, or rendering-time implementation is claimed. Public time APIs and durable timing formats remain gated by this proposal and later technical ADRs.

## Follow-up decisions

- Simulation-rate representation and clock source.
- Scheduler phases, dependency ordering, input admission, and structural commit boundaries.
- Catch-up budget, overload thresholds, and operational health.
- Random streams and the stronger determinism or replay promise.
- Timer identity, tie-breaking, repetition, ownership, and cancellation APIs.
- Pause authority, reason tracking, and network behavior.
- Prediction buffers, interpolation, correction, and cosmetic lifetime.
- Save, restart, and cross-world transfer treatment for delayed work.

## References

- [ADR 0003](0003-preserve-straightforward-game-authoring.md)
- [ADR 0006](0006-server-authority-and-declarative-sync.md)
- [ADR 0009](0009-one-supported-creator-workflow.md)
- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0012](0012-separate-game-host-and-world-state.md)
- [ADR 0014](0014-define-first-release-boundary-and-delivery.md)
- [ADR 0015 proposal](0015-give-entities-an-atomic-observable-lifecycle.md)
- [M1 development gate](../../status/development-plan.md#m1---behavioral-and-technical-gates)
- [World-model questions 9-12](../../workshops/world-model-question-set.md#c-how-does-time-work)
- [Current Robust Toolbox entity manager tick and frame updates](https://github.com/space-wizards/RobustToolbox/blob/master/Robust.Shared/GameObjects/EntityManager.cs)
- [Space Station 14 prediction guide](https://docs.spacestation14.com/en/ss14-by-example/prediction-guide.html)
