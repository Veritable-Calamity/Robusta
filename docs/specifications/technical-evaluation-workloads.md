# Technical Evaluation Workloads and Conformance Decomposition

- **Status:** Proposed planning specification
- **Date:** 2026-07-19
- **Implementation claim:** None
- **Budget status:** Unmeasured; fixture sizes are comparison inputs, not passing release thresholds

## Purpose

These workloads let competing technical mechanisms be compared against the same station-like and contrasting-game demands. They do not select a library, storage layout, wire format, or implementation. Numeric release budgets remain unset until the fixtures run on the supported reference machines and independent reference-game owners approve their representativeness.

## Fixture tiers

| Fixture | Calibration input | Required observations | Release role |
|---|---|---|---|
| W0 - Published loopback walk | One client, one separately launched loopback authority, one exact Preview receipt and schema set, one world, one runtime map, 64 live entities, one bounded generated input/state interaction, one invalid input, and 600 fixed steps | Protected rendezvous and authentication outcome, receipt/schema negotiation, readiness signal, generated input admission and authoritative state result, invalid-input rejection, startup and shutdown time, process ownership, payload separation, step stability, diagnostics, and orphan count | M2 walking-skeleton path |
| W1 - Station-like static space | One world; two instances of the same map definition; a 256 x 256 fixed tilemap; one 128 x 128 static constructed grid at 50% occupied cells; 2,000 spatial entities; 250 nested containment or attachment relations; 1,000 transform changes and 500 representative spatial queries per step during the active segment | Frame conversion cost and failure, stale-handle behavior, relation commit cost, query latency, memory, allocations, cell storage cost, collision work, inspection fidelity, and client-interest candidates | M3 mechanism comparison; values are not release budgets |
| W2 - Contrasting arena | Eight concurrent short-round worlds; no grid or inventory dependency; 256 live participants or projectiles per world; repeated birth/death bursts; spectators; deterministic named random streams | World isolation, fairness, disposal, short-lived identity churn, headless capability omission, step stability, and per-world resource attribution | M2-M4 anti-overfitting comparison |
| W3 - Checkpoint graph | Two maps, one static grid, 10,000 durable records, cyclic local references, required and optional missing-target fixtures, one external deferred proxy, declared timer/random state, and a prior-version migration | Capture fence duration, encoded size, peak memory, restore time, fresh-handle reconstruction, migration outcome, backup selection, corruption limits, and diagnostic completeness | M5 persistence comparison |
| W4 - Catalog adoption | Two worlds, mixed birth generations, 5,000 eligible objects, one reversible migration, one incompatible layout change, and one client lacking the target generation | Preparation time, fence duration, reversal time, retained generations, client admission outcome, postcommit fault reporting, and unaffected-world behavior | M6 catalog-adoption comparison |
| W5 - Collaborative map document | One 10,000-node draft, two authenticated authors, one observer, 1,000 ordered edits, stale-base conflicts, undo/compensation, disconnect/reconnect, autosave, and injected gameplay-only mutations in the derived view | Command latency, deterministic convergence, conflict diagnostics, audit growth, recovery time, unauthorized disclosure, canonical fingerprint, and transient-state exclusion | M6 creator-authority comparison |
| W6 - Interest and secrecy | Two ordinary clients plus an authorized observer; two maps; visible, occluded, contained-secret, owner-only, and out-of-range entities; map change and reconnect | Candidate and admitted set size, CPU, memory, bandwidth, entry/exit order, tombstones, secrecy denials, and side-channel-sensitive expansion | M4 networking comparison |

Larger stress tiers may multiply entity, cell, relation, participant, or revision counts by four and sixteen. Stress results expose crossover and failure behavior; they do not silently become Supported budgets.

## Required measurement records

Every run must record:

- exact source revision, package receipt, catalog/schema fingerprints, operating system, runtime, native dependencies, CPU, memory, and worker-count configuration;
- fixture identity, seed, admitted input trace, warm-up, sample count, and whether the serial oracle or a parallel configuration ran;
- p50, p95, and p99 step or operation latency as applicable, peak resident memory, allocation volume, payload size, and failure reason;
- structured diagnostics and inspection output sufficient to identify the host, session, world, map, frame, entity, document, checkpoint, or catalog generation involved; and
- raw durable evidence location. A summarized chart without raw evidence cannot qualify a capability.

The first collected values remain baselines. A capability receives a passing budget only through explicit review; `null` never means zero or pass.

## Adversarial and fault fixtures

Mechanism reviews must include:

- stale, reused-storage, disconnected-frame, cross-world, and ended-map references;
- forbidden relation cycles, cardinality violations, missing ending dispositions, and unauthorized contained state;
- worker delays, reordered completion, duplicate commands, structural conflicts, and fault injection before and after commit boundaries;
- truncated, corrupt, oversized, deeply nested, duplicate-identity, and decompression-expansion checkpoint inputs;
- catalog preparation failure, known reversible commit failure, integrity-unknown postcommit failure, and an unready client;
- stale document revisions, conflicting edits, unauthorized creator roles, disconnect/reconnect, recovery after interrupted source publication, and attempts to persist damage, timers, AI, physics aftermath, sessions, or network state; and
- missing native libraries, wrong architecture, unsupported platform, platform-thread misuse, and failure during owned-resource cleanup for any candidate client or physics backend.

## Conformance decomposition

The existing product scenarios should later become small executable cases rather than single end-to-end tests.

| Product scenario | Future conformance cases |
|---|---|
| `one-click-separate-local-authority` | process tree and cleanup; protected rendezvous and loopback authentication; readiness; exact receipt/schema negotiation; generated input/state exchange; invalid-input rejection; side-payload audit; compatibility rejection; resource budget |
| `multi-map-frame-and-relation-integrity` | **M3:** duplicate-map separation, explicit conversion failures, stale map/frame rejection, atomic same-world relocation, typed relation mutation, and map-ending disposition; **M4:** contained-state interest and secrecy |
| `constructed-grid-scale` | **M3:** static cell lifecycle, compact-address stability, static attachment, deconstruction round trip, inspection and collision hooks, and dense-data scale; **M4:** declared network synchronization hook; **M5:** checkpoint/save and restore hook |
| `atomic-world-checkpoint-and-reference-migration` | committed capture; repository publication fault; fresh-handle cyclic restore; missing-target policies; rename migration; malicious-input limits; backup preservation |
| `catalog-adoption-existing-state` | future-birth adoption; prepare rejection; fenced reversible commit; unready-client admission; incompatible restart; integrity-unknown postcommit reporting |
| `isolated-map-edit-preview` | source round trip; invalid-draft recovery; revision conflict; compensating undo; unauthorized denial; collaborative reconnect; gameplay-state exclusion; preview isolation; production-payload exclusion |
| `two-game-platform-boundary` | station slice; contrasting slice; headless omission; optional-package absence cost; official/independent equal path; promotion review |

## Outstanding ownership blockers

- Name the station-like and contrasting reference-game repositories.
- Name at least one accountable maintainer for each who is independent of Robusta implementation approval.
- Approve the representative fixture inputs or replace them with versioned alternatives before treating measurements as release evidence.
- Establish numeric pass/fail budgets only after first measurements on Windows 11 and Ubuntu 24.04 clean-machine environments.

## References

- [Product behavior scenario catalog](product-behavior-scenarios.json)
- [Metrics baseline](../status/metrics-baseline.json)
- [Reference-game charters](../status/reference-games.md)
- [First-release technical scope matrix](../status/first-release-technical-scope-matrix.md)
- [Development plan](../status/development-plan.md)
