# ADR 0035: Persist declared world state through versioned checkpoints

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0002, 0003, 0008, 0011-0016, 0019, 0021, 0022, 0026, 0029, 0030-0032

## The question

What does saving a world preserve, and what may a developer or player rely on when that save is restored under the same or a later game release?

## The promise

A world save captures one coherent, declared durable state at a committed simulation boundary. It can be validated, migrated, backed up, and restored into a fresh world without pretending that arbitrary runtime memory, external services, or presentation state are durable.

## Why this matters

Saving an object graph or process image would accidentally persist entity handles, callbacks, native resources, connections, and implementation details. Leaving saves entirely opaque would prevent the compatibility, diagnostics, migration, and rollback outcomes promised by ADR 0008.

## How Robust Toolbox answers today

Robust Toolbox provides powerful map and entity serialization used by SS14 authoring. SS14's mapping workflow nevertheless distinguishes editing from gameplay testing and warns creators not to save test mutations as authored maps. That is useful evidence that a map artifact, running world, and durable checkpoint need separate meanings.

## How the Robusta prototype answers today

The predecessor has no general world-save contract, durable schema, migration pipeline, or crash-consistent repository that satisfies the accepted version and rollback promises.

## Options considered

### Option A: Versioned checkpoints of declared durable state

The platform records required foundation state plus game-declared durable state in a versioned envelope at a committed boundary, then restores it through a validated fresh-world construction path.

### Option B: Capture the complete runtime or language object graph

This appears comprehensive but couples saves to memory layout, threads, callbacks, native state, and ephemeral identities that cannot be restored safely across versions.

### Option C: Leave saves as opaque game-owned blobs

Games gain freedom, but the platform cannot provide uniform compatibility checks, migration, rollback, inspection, security limits, or creator tooling.

## Decision

Robusta will use Option A.

The product contract is:

1. A **world checkpoint** is a named save contract containing all platform-required foundation state and all game state needed to honor that save profile. A map source, replay, crash dump, account record, and arbitrary exported subset are different artifacts.
2. Capture observes one complete committed authoritative boundary. It never records a half-applied entity, relationship, timer, or structural change.
3. Every authoritative value that can affect future outcomes is either persisted, deterministically reconstructed through an identified versioned rule, or explicitly reset under a save-profile rule whose gameplay outcome is documented. A profile cannot silently omit consequential state merely because a game did not mark a field durable.
4. The completeness rule covers simulation step and time, admitted random-stream state, live entity and lifecycle state, relationship state, runtime map and compact spatial state, authoritative game rules, and every admitted durable schedule. It does not require persisting caches or values that an identified rule reconstructs exactly.
5. The envelope records distinct identity fields: an immutable checkpoint identity for this artifact; an optional world-lineage identity only when continuity across checkpoints is promised; an optional parent checkpoint identity for a lineage predecessor; zero or more source checkpoint identities with declared derivation or migration roles; and a repository slot or operator-facing name used only to select an artifact. None is a runtime world-scope identity, and changing a repository slot or display name does not change checkpoint identity.
6. The envelope also identifies the exact game release receipt, catalog and schema identities, save-profile version, source simulation step and time, reconstruction/reset rules, and integrity and resource-limit information needed before decoding payloads.
7. Restore validates and migrates the checkpoint, constructs a fresh unpublished world, resolves its declared state, and publishes the world only after the complete operation succeeds.
8. Runtime entity and network handles, sessions, sockets, tasks, callbacks, physics caches, presentation state, and native resources are not durable. They are reconstructed, reattached, reset through a declared outcome, or deliberately absent under their own contracts.
9. Ordinary world timers do not silently survive restart. Persisted delayed work must opt into a durable schedule contract defining its payload, owner, remaining or absolute time meaning, expiry behavior, and idempotency; other timers reset only through the save profile's declared gameplay outcome.
10. Host, account, economy, or other external durable services are outside one world's checkpoint unless they join through an explicitly declared checkpoint transaction. Robusta does not imply atomicity across arbitrary databases or processes.
11. Loading under a different release produces one explicit outcome: directly compatible, migrated forward, inspectable read-only where supported, or rejected before world publication with a compatibility report.
12. Migration creates a new checkpoint and preserves the original or a verified backup. Application rollback may select an older executable, but Robusta makes no general reverse-migration promise and retains the pre-migration data needed for safe rollback.
13. Save creation and repository selection are atomic. A crash or quota failure may leave an unselected temporary artifact, never a selected partial checkpoint.
14. The envelope and decoder enforce versioned size, count, depth, allocation, decompression, and reference limits before unbounded work. Corrupt, truncated, malicious, or oversized input fails without publishing a world or replacing a known-good checkpoint.

## What we deliberately will not do

- Serialize arbitrary process memory or runtime object graphs.
- Claim that reconnecting a session resumes the same runtime handles or callbacks.
- Modify the only copy of a checkpoint during migration.
- Treat application rollback and writable-data rollback as the same operation.
- Claim one world checkpoint atomically includes undeclared external services.

## Consequences

### Benefits

- Save compatibility and failure are explainable before a world becomes live.
- Platform and game state can evolve through explicit migrations.
- Backups preserve a real recovery path when an update fails.
- Transient implementation details do not become permanent SDK contracts.

### Costs and limitations

- Every durable type and field needs a supported schema and migration policy.
- Consistent online capture may require copy-on-write or bounded coordination.
- Arbitrary asynchronous execution cannot resume where it stopped.
- Cross-service consistency requires additional durable transaction design.

## How we will prove the decision works

- A station-like world restores maps, entities, compact grid state, relationships, declared timers, and world rules from one committed boundary.
- Fault injection during capture, repository publication, migration, and restore never selects a partial save or publishes a partial world.
- One forward migration creates a new checkpoint, preserves its backup, and supports executable rollback without losing the original data.
- Unsupported newer data and missing required schemas fail before any world is published.
- Client prediction, connections, native caches, and ordinary timers are absent or reconstructed rather than accidentally serialized.
- Saving and restoring one world does not mutate another world or host session.
- Omitting a consequential authoritative value without a deterministic reconstruction or declared reset outcome fails save-profile validation.
- Corrupt, truncated, decompression-bomb, and oversized fixtures fail within declared resource limits before unbounded allocation or world publication, while the selected known-good checkpoint remains intact.

## Implementation notes

No save envelope, durable schema, repository, checkpoint coordinator, migration runner, or restore pipeline exists.

## Follow-up decisions

- Save envelope, canonical encoding, integrity, compression, and resource limits.
- Durable component, relationship, map, grid, time, and random-state schemas.
- Repository transactions, backup retention, encryption, privacy, and operator policy.
- Migration graph, tooling, validation, and read-only inspection.
- Durable delayed work and external-service checkpoint participation.

## References

- [ADR 0008](0008-explicit-versions-migrations-and-rollback.md)
- [ADR 0012](0012-separate-game-host-and-world-state.md)
- [ADR 0016](0016-separate-simulation-host-and-presentation-time.md)
- [ADR 0022](../technical/0022-install-exact-receipts-into-immutable-content-addressed-layouts.md)
- [World-model question 20](../../workshops/world-model-question-set.md#20-what-does-saving-a-world-promise)
- [ADR coherence audit](../../status/adr-coherence-and-first-release-baseline-2026-07-19.md)
- [Space Station 14 mapping workflow](https://docs.spacestation14.com/en/space-station-14/mapping/guides/general-guide.html)
