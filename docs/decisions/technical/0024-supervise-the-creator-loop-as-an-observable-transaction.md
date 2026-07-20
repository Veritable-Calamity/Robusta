# ADR 0024: Supervise the creator loop as an observable transaction

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Creator workflow workstream
- **Supersedes:** None
- **Amended by:** ADR 0037, decision clause 12, which narrows decision clause 6 and its rollback proof
- **Product decisions served:** 0001, 0002, 0003, 0005, 0009, 0012, 0014, 0037, 0038
- **Related decisions:** 0017, 0018, 0021, 0022, 0023, 0025, 0035, 0037, 0038

## The question

How will `robusta dev` discover, build, compile, launch, observe, reload, restart, reconnect, and clean up a game through one cross-platform workflow?

## The promise preserved

Every creator change has a visible outcome using release-equivalent validation, and normal or failed development sessions leave no orphaned owned processes.

## Why this matters

File watching alone cannot decide whether a change is safely reloadable. Without an explicit supervisor protocol, tools disagree, logs interleave ambiguously, and partial restarts strand clients or child processes.

## Options considered

### Option A: One supervisor with a generated impact graph and transaction journal

The CLI owns the session, runs each stage through structured adapters, classifies changed outputs through exact identities, and executes reload or restart as a journaled transaction.

### Option B: Independent watch commands for each tool

This is easy to prototype but leaves creators to coordinate dependencies, process lifetimes, and errors manually.

### Option C: Always restart everything

This is honest but unnecessarily slow and does not exercise transactional catalog adoption or targeted client reconnect.

## Decision

Robusta will use Option A:

1. `robusta dev` discovers one explicit workspace manifest and exact development lock; ambiguity or missing identity fails with a source-quality diagnostic.
2. The supervisor builds a directed impact graph for restore, code generation, compilation, content compilation, packaging checks, server, and clients. Stage inputs and outputs use digests rather than timestamps alone.
3. Every tool emits versioned JSON Lines events with session, process, stage, severity, diagnostic code, source span where applicable, and human-readable text. The CLI renders those events but preserves the machine-readable stream.
4. The supervisor owns all processes through operating-system job or process-group facilities, records their identities, and performs bounded graceful stop followed by bounded forced termination only for its own tree.
5. Change classification compares generated API, component layout, catalog, resource, localization, network schema, and runtime receipt identities against an explicit reload matrix.
6. A reload transaction prepares a new immutable catalog or resource generation, validates every affected world, commits adoption at declared boundaries, and rolls all affected worlds back on failure.
7. A restart transaction stops affected processes, rebuilds exact outputs, starts a fresh authority, and reconnects configured clients. It never claims world preservation unless a later accepted persistence contract supports it.
8. Every observed change ends as reloaded, rebuilt, restarted, reconnecting/reconnected, rejected, or ignored with a stated reason.
9. Release packaging invokes the same generators, compiler, schemas, and validation modes. Development-only endpoints and powers are excluded by explicit build inputs and audited in release artifacts.

## What we deliberately will not do

- Infer safe reload from filename extensions alone.
- Parse compiler prose when a structured adapter is available.
- Kill processes not created or adopted by the current supervisor session.
- Preserve arbitrary live worlds across code or schema changes by accident.

## Consequences

### Compatibility and migration

The event protocol, workspace manifest, reload matrix, and tool adapters are versioned. Older games receive explicit tool/runtime compatibility outcomes rather than partially working watches.

### Security

Workspace commands are data interpreted by supported stages, not arbitrary shell hooks by default. Secrets are redacted at event sources. Development endpoints bind locally unless explicitly configured.

### Operations

Session journals support crash recovery, orphan detection, stage timing, restart diagnosis, and cleanup audits. Cross-platform process supervision requires separate Windows and Linux implementations behind one contract.

## How we will prove the decision works

- `CleanMachineEditRestartReconnect` runs the full resource, content, layout, schema, restart, and reconnect sequence on Windows and Ubuntu.
- `ProcessTreeCleanup` injects failure and interruption at every stage and proves unrelated processes remain untouched.
- Identical invalid input produces the same diagnostic code in development and release builds.
- Release scans find no development-only endpoint, watcher, credential, or permissive parser.
- A failed catalog adoption leaves every affected world on its prior generation.

## Implementation notes

`tools/Cli` is a command scaffold. No workspace manifest, event protocol, supervisor, watcher, reload matrix, or reconnect coordinator exists.

[Accepted ADR 0037](../product/0037-keep-live-state-stable-unless-explicitly-migrated.md), decision clause 12, formally amends decision clause 6 and its rollback proof. In this ADR, "rolls all affected worlds back on failure" means either prepare rejection before publication or reversal of a known reversible commit failure while every target and client publication remains fenced. It never means arbitrary postcommit gameplay rewind, implicit live-object rebasing, or whole-world rollback after integrity becomes unknown. Catalog adoption and client-generation admission must implement ADR 0037's complete preparation, fencing, inverse, publication, and fault rules.

## Follow-up decisions

- Workspace manifest and structured event schemas.
- Editor integration and collaborative creator-authority protocols required by [accepted ADR 0038](../product/0038-edit-map-sources-and-preview-in-isolated-worlds.md).
- Preview supervision, document-command impact classification, and creator-projection packaging.

## References

- [ADR 0009](../product/0009-one-supported-creator-workflow.md)
- [ADR 0014](../product/0014-define-first-release-boundary-and-delivery.md)
- [ADR 0037](../product/0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [ADR 0038](../product/0038-edit-map-sources-and-preview-in-isolated-worlds.md)
- [Creator workflow scenarios](../../specifications/product-behavior-scenarios.json)
