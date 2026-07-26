# External Reference-Game Charters

## Shared rules

Each reference game is maintained in its own repository and has at least one accountable maintainer who is not the approver of the corresponding Robusta implementation. It restores released or Preview packages from the declared artifact feed. Repository-relative project references, friend access, linked engine source, internal namespaces, and undocumented host hooks invalidate the evidence.

CI records the exact package versions, feed, source revision, operating system, commands, evidence packets, and release receipt. Each reference game restores and uses the published Test SDK for its applicable headless and loopback qualification scenarios; repository-only helpers or private runtime access do not substitute for that evidence. Temporary in-repository fixtures may validate tooling but do not satisfy external ownership or use.

## Station-like multiplayer slice

- **Owner:** Station reference-game maintainers (independent repository team)
- **Product sponsor:** Robusta migration workstream owner
- **Purpose:** exercise prototypes, maps and constructed grids, entity interaction, prediction, inventory, entity-bound UI, persistence, server administration, and migration-relevant behavior.
- **First evidence:** Published walking skeleton (roadmap M2)
- **Completion evidence:** Release qualification (roadmap M8)
- **Initial technical fixtures:** W0 published loopback walk, W1 station-like static space, W3 checkpoint graph, W4 catalog adoption, W5 collaborative map document, and W6 interest/secrecy from the [technical evaluation workloads](../specifications/technical-evaluation-workloads.md).
- **First-release spatial boundary:** static compact-grid construction, anchoring, containment, and same-world relocation. Dynamic grid split/merge is a later capability proof under ADR 0031.
- **Inspection and replay qualification:** authorized and ordinary-client projections prove discovery control, redaction, committed-boundary consistency, attachment invalidation, and bounded queries; one representative authoritative segment re-executes only inside its admitted domain with real sinks absent and committed state/effect-intent comparison.

## Contrasting game

- **Owner:** Contrasting reference-game maintainers (independent repository team)
- **Product sponsor:** Robusta SDK workstream owner
- **Definition:** a round-based 2D arena game with no station, construction-grid, door, inventory-container, or persistent-world assumptions.
- **Purpose:** exercise fast world replacement, many short-lived participants, deterministic match rules, client prediction, spectator sessions, and data-oriented arena state.
- **First evidence:** Published walking skeleton (roadmap M2)
- **Completion evidence:** Release qualification (roadmap M8)
- **Initial technical fixtures:** W0 published loopback walk and W2 contrasting arena from the [technical evaluation workloads](../specifications/technical-evaluation-workloads.md), plus the no-station-package and headless capability-omission profiles.
- **Inspection, testing, and replay qualification:** the same published inspection, Test SDK, and replay contracts work without station, grid-construction, inventory, or persistent-world packages; short-round worlds remain isolated and replay verification depends only on declared arena semantics and exact artifacts.

The fixture inputs are provisional until the reference-game owners approve or replace them with versioned alternatives. Named individuals and repository URLs are recorded in the ledger when those repositories are provisioned. Until then, no external-use facet may pass and no provisional measurement becomes a release budget.

