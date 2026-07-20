# ADR 0020: Run fixed-step worlds through a deterministic phase scheduler

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0006, 0011, 0012, 0015, 0016
- **Related decisions:** 0017, 0019, 0023

## The question

How will fixed world steps order input, timers, systems, events, structural commits, pause, catch-up, and presentation while remaining testable without a wall clock?

## The promise preserved

Gameplay advances in fixed numbered steps independent of render rate and machine jitter. Pause, timers, lifecycle commits, overload, and headless execution have stable observable behavior.

## Why this matters

Scheduling is where time, entity atomicity, networking, random state, and event delivery meet. An implicit update loop would turn incidental callback order and thread timing into game rules.

## Options considered

### Option A: Fixed phase graph with serial authoritative execution first

Represent simulation progress with a step number and fixed integer duration. Resolve system dependencies into a stable graph, run authoritative systems serially in stable topological order, and add parallel batches only after representative evidence establishes safe conflicts and merge rules.

This gives the simplest determinism oracle, diagnostics, and structural-change model. Its risk is that a serial-first implementation may shape APIs and data access around single-thread assumptions or postpone essential scalability work.

### Option B: Variable delta frame loop

This is straightforward but contradicts accepted ADR 0016 by making authoritative outcomes sensitive to rendering and load. It is retained as a rejected comparison, not a viable choice.

### Option C: Fixed phase graph with deterministic parallel execution first

Use the same integer clock, published phases, and dependency graph as Option A, but require systems to declare read/write capabilities and execute independent batches concurrently from the first release. Each batch commits outputs through deterministic merge points; a serial execution mode remains available as an oracle and diagnostic fallback.

This makes concurrency constraints part of the architecture before game code can acquire hidden single-thread assumptions and may use modern hardware better. Its cost is substantially greater implementation, testing, debugging, and scheduling complexity before representative workloads establish that the gain is material.

## Decision

Robusta will use Option C. Deterministic parallel batches are a first-release behavior, and serial execution remains a required correctness oracle and diagnostic fallback.

1. Each world stores a `ulong` step number and an immutable positive step duration represented in integer time units. Simulation deadlines are expressed as step boundaries, not floating-point wall time.
2. The initial authoritative phase order is: admit ordered external inputs; make due timers eligible; run declared systems and queued gameplay events; commit structural transactions; publish committed observations and replication state.
3. Systems declare stable identities plus before, after, and capability dependencies. Startup rejects cycles, missing required dependencies, and ambiguous duplicate identities with a graph diagnostic.
4. Systems also declare authoritative read and write capabilities. The scheduler builds stable parallel batches only when those declarations prove that systems are independent; an undeclared or ambiguous conflict fails validation rather than racing.
5. A system requiring exclusive world access or an integration that cannot safely partition its state runs as a one-system batch at its declared graph position.
6. Parallel systems do not directly publish shared structural changes, events, timer mutations, or replication output. They write to isolated command and observation buffers.
7. Batch outputs merge at a deterministic boundary using stable system identity and operation sequence, independent of worker count, completion order, task delay, or operating-system scheduling.
8. The same graph can execute serially using identical phase, ordering, buffering, and merge semantics. The serial path is continuously tested as an oracle and may be selected for diagnostics or constrained environments; it is not a separate gameplay mode.
9. Simulation timers use a world-owned priority queue ordered by deadline step and monotonic insertion sequence. Repeating timers schedule from their prior deadline, never from late wall-clock execution.
10. Timer ownership is explicit. Ending an owner cancels its entries before later eligible work can run.
11. Pause requests commit between steps. A paused world admits no ordinary gameplay input and advances no simulation phases; host-directed inspection, diagnostics, networking, and teardown remain outside the step scheduler.
12. Production hosts use a monotonic clock and accumulator with both maximum steps per service cycle and elapsed-work budgets. Remaining backlog stays queued and is reported; steps are never skipped or stretched.
13. Tests use a manual host driver that advances an exact step count without sleeping.
14. Authoritative random state is world-owned and accessed through versioned named streams. Each parallel system or declared partition receives a deterministic stream independent of worker assignment. The exact algorithm is part of the exact runtime receipt; replay and cross-version preservation remain deferred.
15. Presentation uses a separate client clock and immutable confirmed snapshots. It cannot invoke authoritative phases or fire simulation timers.

## What we deliberately will not do

- Let wall-clock timestamps enter gameplay without becoming ordered inputs.
- Hide overload by dropping steps, enlarging deltas, or running unlimited catch-up.
- Permit system registration or filesystem order to decide execution order.
- Permit worker completion order, thread identity, or shared unsynchronized mutation to affect authoritative results.
- Expose a parallel-only game API that cannot run through the serial oracle.
- Promise bitwise cross-platform numerical replay in this ADR.

## Consequences

### Compatibility and migration

Legacy frame-update gameplay must move to fixed-step systems or explicitly non-authoritative presentation. Timer and ordering differences require migration diagnostics and conformance fixtures.

### Security

Input admission and server authority remain explicit. Resource budgets prevent one overloaded world from starving host administration, though trusted game code can still exhaust its game process.

### Operations

Metrics include current step, step duration, backlog steps, simulated lag, phase and batch duration, worker utilization, serial-oracle comparisons, timer counts, catch-up work, and pause reasons. Dependency and conflict graphs are inspectable.

## How we will prove the decision works

- `FixedStepFrameRateAndHeadlessIndependence` compares traces at 30, 60, and 144 FPS and on a headless server.
- `BoundedCatchUpReportsOverload` injects load and proves no skipped or stretched steps.
- `WholeWorldPauseBoundaryAndInputRejection` verifies every paused and continuing responsibility.
- Timer tests cover equal deadlines, repeat drift, cancellation, stale targets, and restart boundaries.
- Scheduler graph tests randomize registration order and produce the same execution trace or the same stable cycle diagnostic.
- Parallel execution must produce the same trace under randomized worker counts, task delays, and valid batch interleavings as the serial oracle.
- Representative single- and multi-world workloads publish serial and parallel results, crossover points, worker utilization, and scheduling overhead; correctness does not depend on parallel speedup.

## Implementation notes

No scheduler, world clock, timer queue, random stream, or presentation clock is implemented.

The [2026-07-19 coherence audit](../../status/adr-coherence-and-first-release-baseline-2026-07-19.md) found that Option C is not enforceable until access lifetimes, aliases, effects, reductions, affinity, I/O, and faults have a contract. [ADR 0029](0029-enforce-phase-scoped-access-and-buffered-effects.md) is accepted and supplies that contract.

## Follow-up decisions

- Numerical determinism and replay after the persistence product gate.
- Prediction and correction buffer sizing.
- Conflict-capability vocabulary, partition declarations, and stable merge-key representation.
- Worker-pool sizing and operational tuning defaults.

## References

- [ADR 0016](../product/0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [Time behavioral scenarios](../../specifications/product-behavior-scenarios.json)
