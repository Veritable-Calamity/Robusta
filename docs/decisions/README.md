# Architecture Decision Records

This directory records decisions for the release-grade Robusta platform.

## Two decision levels

- `product/` records user-visible promises and top-level product choices in plain language.
- `technical/` will record implementation mechanisms that fulfill the product decisions.

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

## Active proposals

None.
