# ADR 0026: Define the supported game-code conformance and fault-containment boundary

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0002, 0003, 0004, 0007, 0011, 0012, 0015-0020, 0024

## The question

Which isolation, determinism, recovery, and diagnostic guarantees apply to trusted executable game code running inside a Robusta game-host process, and what happens when that code violates the supported execution contract or fails during simulation?

## The promise

Game developers can tell which coding practices receive Robusta's world-isolation, deterministic-scheduling, lifecycle, and fault-reporting guarantees. A failure in conforming game code has a defined outcome, while arbitrary executable code is never misrepresented as safely sandboxed merely because it runs behind SDK interfaces.

## Why this matters

A full game package is intentionally installed executable software. It may contain ordinary C#, advanced extensions, or operator-installed executable code with the operating-system permissions of its game process.

Analyzers, capability declarations, dependency scopes, and deterministic schedulers can enforce supported patterns, but they cannot make arbitrary in-process code harmless. Code can create threads, mutate static state, call native libraries, block indefinitely, bypass declared capabilities, or corrupt memory. Claiming hard isolation between worlds under those conditions would be false.

Treating every game error as an unexplained process crash would also make the SDK, world boundary, and creator diagnostics much less useful than promised.

## Options considered

### Option A: Supported conformance boundary with process-level hard containment

Define a supported in-process execution contract. Robusta validates declared capabilities and catches faults at platform invocation boundaries. Conforming faults stop the affected world without publishing further authoritative work from the failed boundary. The external game-process supervisor remains the hard containment mechanism for hangs, corruption, or failures whose scope cannot be trusted.

Code that bypasses the contract remains trusted executable software, but loses the affected determinism, isolation, and recovery guarantees.

### Option B: Promise hard world isolation for arbitrary in-process game code

This would offer the strongest apparent multi-world promise. It is not technically honest: arbitrary managed, native, static, threaded, or unsafe code can affect every world and service in the process.

### Option C: Put every world or executable extension in a separate process

This provides a stronger hard boundary, but adds IPC, deployment, debugging, latency, and resource costs and conflicts with the accepted ability for one trusted host to run several lightweight worlds. It may remain a future deployment profile.

### Option D: Treat all in-process faults as unspecified

This is simple for the runtime but leaves creators unable to distinguish supported failures from corruption and weakens world isolation, diagnostics, and operational recovery.

## Decision

Robusta will use Option A. The product contract is:

1. A **conforming game system or extension** uses published SDK entry points, declares its capabilities and side effects, keeps mutable gameplay state in its assigned owner, and obeys scheduler, threading, lifecycle, and cancellation rules.
2. Validation rejects detectable violations, such as forbidden references, undeclared capabilities, invalid lifetime capture, side leakage, or unsupported registration, with source-quality diagnostics.
3. Expected gameplay failures use explicit results. Exceptions are not the ordinary control path for stale targets, rejected input, validation failure, or denied authority.
4. An unhandled exception during authoritative work faults the affected world at a safe boundary. Robusta stops further gameplay advancement and authoritative publication for that world and reports the world, step, phase, system, operation, and available provenance.
5. Robusta does not claim that arbitrary state written before an exception has been rolled back. Automatic retry, system quarantine, or continued simulation is prohibited unless a later contract proves that the affected work was isolated and replay-safe.
6. Other worlds may continue only when the fault occurred through a conforming world-scoped boundary and host or shared-state integrity remains known. A fault involving host, session, catalog, durable, native, or otherwise shared integrity escalates to host-process termination or another explicitly documented broader outcome.
7. Lifecycle preparation and buffered scheduler outputs retain the atomicity promised by ADRs 0015, 0019, and 0020. This does not silently extend whole-step transactional rollback to arbitrary game-state mutation or external I/O.
8. Blocking, runaway allocation, deadlock, native failure, process corruption, or undeclared thread activity may prevent safe in-process recovery. An external supervisor detects loss of health, captures diagnostics where possible, and terminates only the owned game-process tree.
9. Launcher, updater, package manager, and credential processes remain separate and do not load game assemblies. A failed game process cannot corrupt their in-process state or that of another game process.
10. A full game process is not an operating-system sandbox. Unless a later isolation profile says otherwise, trusted game code may use the files, network, processes, native libraries, and other powers granted to that operating-system identity.
11. Code that deliberately bypasses supported SDK, ownership, scheduling, or capability rules is **nonconforming**. Robusta may run it as trusted game material where explicitly allowed, but does not promise deterministic scheduling, multi-world isolation, safe reload, or localized recovery for its effects.
12. Public UGC remains outside this trusted-code category and continues to require a genuinely capability-limited data or interpreted boundary.

## What we deliberately will not do

- Describe dependency injection, analyzers, capability declarations, assembly load contexts, or exception handlers as a security sandbox.
- Promise that arbitrary in-process code cannot affect another world in the same process.
- Continue an authoritative world after an unhandled fault when its state integrity is unknown.
- Claim whole-step rollback when only structural or buffered outputs were transactional.
- Abort arbitrary managed threads and then claim the process remains sound.
- Hide a conformance violation behind a generic crash or silently downgrade its guarantees.
- Apply trusted full-game assumptions to public UGC.

## Consequences

### Benefits

- World-isolation and determinism claims have an honest applicability boundary.
- Ordinary creator mistakes receive actionable diagnostics and defined fault outcomes.
- The platform can preserve lightweight multi-world hosts for conforming trusted games.
- Severe failures remain contained from launcher credentials and unrelated game processes.
- Future stronger process or operating-system isolation can be added without rewriting the ordinary SDK contract.

### Costs and limitations

- Some violations cannot be detected statically or safely recovered in process.
- A single nonconforming or native extension can still terminate or corrupt its game-host process.
- World-local recovery requires careful invocation boundaries, health state, cleanup, and diagnostics.
- Developers must understand the difference between trusted, conforming, and capability-confined code.
- Operators needing hostile-code containment require a stronger deployment boundary.

## How we will prove the decision works

- Architecture and analyzer fixtures reject every statically detectable conformance violation with a stable diagnostic and source location.
- Fault injection at system, event, timer, lifecycle, and structural-commit boundaries produces the documented world-fault outcome and publishes no later authoritative result from the failed boundary.
- A conforming world-scoped exception faults and disposes one world while another world and host session remain healthy.
- A shared-state or integrity-unknown fault escalates predictably rather than allowing gameplay to continue.
- Blocking and runaway fixtures cause the external supervisor to report loss of health and terminate only the owned game-process tree.
- Process audits prove launcher, updater, package, and credential processes never load game assemblies.
- Documentation and installation consent state plainly that a full game is executable software with the game process's operating-system permissions.
- Public UGC denial tests continue to prove that public add-ons cannot acquire trusted executable powers.

## Implementation notes

No conformance validator, world-fault state, escalation policy, health watchdog, or fault-injection evidence is implemented.

## Follow-up decisions

- Exact supported-code rules for static state, threads, tasks, native code, reflection, unsafe code, blocking work, and external I/O.
- Runtime fault states, diagnostic schema, cleanup budgets, and host escalation policy.
- Deterministic scheduler capability and side-effect declarations.
- External supervision, health deadlines, crash capture, restart, and recovery behavior.
- Optional process-per-world or operating-system isolation profiles.
- Declarative public-UGC execution and resource-budget contract.

## References

- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0011](0011-define-world-as-isolated-simulation.md)
- [ADR 0017](../technical/0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0018](../technical/0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [.NET assembly loading and unloading](https://learn.microsoft.com/en-us/dotnet/standard/assembly/load-unload)
