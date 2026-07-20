# Architecture Decision Records

This directory records decisions for the release-grade Robusta platform.

## Two decision levels

- `product/` records user-visible promises and top-level product choices in plain language.
- `technical/` records implementation mechanisms that fulfill the product decisions.

A technical ADR must name the product ADRs it serves. Technical choices may be replaced without changing the product promise when the replacement preserves documented behavior.

## Decision status

- **Draft** — incomplete working text.
- **Proposed** — ready for explicit review.
- **Accepted** — chosen direction; future work should follow it.
- **Rejected** — considered and deliberately not chosen.
- **Superseded** — replaced by a later ADR; retained as history.
- **Withdrawn** — no longer under consideration before acceptance.

## Implementation status

Decision acceptance is separate from implementation:

- **Not started**
- **In progress**
- **Demonstrated** — working evidence exists in an external game or platform test.
- **Released** — shipped as a supported capability.
- **Partially superseded** — some implementation remains while the design changes.

## Editing rules

After acceptance, an ADR may be edited for spelling, broken links, or clearly non-substantive clarification. A material change requires a new ADR that supersedes or amends the old one.

Implementation status and evidence links may be updated without superseding the design decision, provided the decision itself does not change.

## Numbering

This is a new greenfield ADR sequence. It does not continue the numbering of the prototype-era Robusta repository.

- Product decisions begin at `0000`.
- Technical decisions continue in the same overall sequence when they are introduced.
- File names use `NNNN-short-kebab-title.md`.

## Accepted product decisions

| ADR | Decision | Decision status | Implementation |
|---|---|---|---|
| 0000 | Adopt the Robusta Platform Constitution | Accepted | Not started |
| 0001 | Build a complete game platform for independent teams | Accepted | Not started |
| 0002 | Judge quality through user outcomes and external use | Accepted | Not started |
| 0003 | Preserve straightforward game authoring behind a supported SDK | Accepted | Not started |
| 0004 | Distribute games as versioned, isolated application packages | Accepted | Not started |
| 0005 | Compile readable content into a deterministic package-aware catalog | Accepted | Not started |
| 0006 | Use server authority with declarative synchronization intent | Accepted | Not started |
| 0007 | Separate trusted executable games from public UGC | Accepted | Not started |
| 0008 | Support exact release receipts, side-by-side runtimes, migration, and rollback | Accepted | Not started |
| 0009 | Provide one supported creator development workflow | Accepted | Not started |
| 0010 | Target assisted Robust Toolbox migration rather than binary compatibility | Accepted | Not started |
| 0011 | Define a world as an isolated simulation containing multiple maps | Accepted | Not started |
| 0012 | Separate immutable game definitions, host sessions, and mutable world state | Accepted | Not started |
| 0013 | Use entities for independent world participants, not all data | Accepted | Not started |
| 0014 | Define the first-release boundary and delivery responsibilities | Accepted | In progress |
| 0015 | Give entities an atomic, observable lifecycle | Accepted | Not started |
| 0016 | Separate simulation, host, and presentation time | Accepted | Not started |
| 0026 | Define the supported game-code conformance and fault-containment boundary | Accepted | Not started |
| 0027 | Run offline play through a separate local authority | Accepted | Not started |
| 0030 | Define runtime maps and frame-qualified coordinates | Accepted | Not started |
| 0031 | Separate spatial, containment, attachment, and lifecycle relations | Accepted | Not started |
| 0032 | Reconstruct explicitly across world transfers | Accepted | Not started |
| 0033 | Provide platform mechanics with game-defined semantics | Accepted | Not started |
| 0034 | Use a declared ladder for advanced game extensions | Accepted | Not started |
| 0035 | Persist declared world state through versioned checkpoints | Accepted | Not started |
| 0036 | Use explicit durable identities and reference policies | Accepted | Not started |
| 0037 | Keep live state stable unless explicitly migrated | Accepted | Not started |
| 0038 | Edit map sources and preview them in isolated worlds | Accepted | Not started |

ADR 0033 permits batteries-included station conveniences only as ordinary, separately versioned game or component packages built through the same published SDK and declared trust mechanisms available to independent developers. They receive no privileged platform internals and remain subject to every accepted ADR. ADR 0038 also permits authority-hosted collaborative mapping sessions for authenticated creators, while canonical source-document transactions and their history—not arbitrary live gameplay state—remain the authored truth.

## Active drafts and proposals

| ADR | Decision | Decision status | Implementation | Current review position |
|---|---|---|---|---|
| 0039 | Inspect running worlds through authorized snapshots | Proposed | Not started | Option A recommended |
| 0040 | Test isolated worlds through the supported runtime | Proposed | Not started | Option A recommended |
| 0041 | Record versioned authoritative replays with declared determinism | Proposed | Not started | Option A recommended |

These proposals answer world-model questions 24-26 but do not settle them until explicitly accepted. Each states the exact amendment its acceptance would make to ADR 0014's first-release diagnostics or qualification floor.

## Accepted technical decisions

Their product-decision coverage is listed in [`technical/README.md`](technical/README.md).

| ADR | Decision | Decision status | Implementation |
|---|---|---|---|
| 0017 | Enforce explicit runtime ownership scopes | Accepted | Not started |
| 0018 | Publish a layered Game SDK with capability boundaries | Accepted | Not started |
| 0019 | Use generational entity handles and transactional structural commits | Accepted | Not started |
| 0020 | Run fixed-step worlds through a deterministic phase scheduler | Accepted | Not started |
| 0021 | Compile content into a canonical provenance catalog | Accepted | Not started |
| 0022 | Install exact receipts into immutable content-addressed layouts | Accepted | Not started |
| 0023 | Generate versioned authoritative replication schemas | Accepted | Not started |
| 0024 | Supervise the creator loop as an observable transaction | Accepted | Not started |
| 0025 | Migrate through a source-located intermediate model and conformance corpus | Accepted | Not started |
| 0028 | Model sessions and worlds as sibling host scopes | Accepted | Not started |
| 0029 | Enforce phase-scoped access and buffered deterministic effects | Accepted | Not started |

Accepted product ADR 0037 amends ADR 0024's catalog-adoption rollback contract; it does not authorize arbitrary postcommit world rewind.

## Proposed technical decisions

| ADR | Decision | Decision status | Implementation | Current review position |
|---|---|---|---|---|
| 0042 | Use typed message kinds and transactional structural commits | Proposed | Not started | Option A recommended |
| 0043 | Use a typed identity and compatibility spine | Proposed | Not started | Option A recommended |

Neither proposal authorizes implementation before review and acceptance.
