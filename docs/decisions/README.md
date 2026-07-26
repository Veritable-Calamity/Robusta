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
| 0039 | Inspect running worlds through authorized snapshots | Accepted | Not started |
| 0040 | Test isolated worlds through the supported runtime | Accepted | Not started |
| 0041 | Record versioned authoritative replays with declared determinism | Accepted | Not started |

ADR 0033 permits batteries-included station conveniences only as ordinary, separately versioned game or component packages built through the same published SDK and declared trust mechanisms available to independent developers. They receive no privileged platform internals and remain subject to every accepted ADR. ADR 0038 also permits authority-hosted collaborative mapping sessions for authenticated creators, while canonical source-document transactions and their history—not arbitrary live gameplay state—remain the authored truth. ADRs 0039-0041 accept bounded authorized inspection, ordinary-runtime isolated testing, and verified in-domain authoritative replay via Option A; they amend ADR 0014's first-release floor, and ADR 0041 also removes only the replay-specific deferrals in ADRs 0042 and 0043.

## Accepted technical decisions

Their product-decision coverage is listed in [`technical/README.md`](technical/README.md).

| ADR | Decision | Decision status | Implementation |
|---|---|---|---|
| 0017 | Enforce explicit runtime ownership scopes | Accepted | In progress |
| 0018 | Publish a layered Game SDK with capability boundaries | Accepted | Not started |
| 0019 | Use generational entity handles and transactional structural commits | Accepted | Not started |
| 0020 | Run fixed-step worlds through a deterministic phase scheduler | Accepted | Not started |
| 0021 | Compile content into a canonical provenance catalog | Accepted | Not started |
| 0022 | Install exact receipts into immutable content-addressed layouts | Accepted | Not started |
| 0023 | Generate versioned authoritative replication schemas | Accepted | Not started |
| 0024 | Supervise the creator loop as an observable transaction | Accepted | Not started |
| 0025 | Migrate through a source-located intermediate model and conformance corpus | Accepted | Not started |
| 0028 | Model sessions and worlds as sibling host scopes | Accepted | In progress |
| 0029 | Enforce phase-scoped access and buffered deterministic effects | Accepted | Not started |
| 0042 | Use typed message kinds and transactional structural commits | Accepted | Not started |
| 0043 | Use a typed identity and compatibility spine | Accepted | In progress |
| 0044 | Generate bounded identity declarations and per-kind profiles | Accepted | Not started |
| 0045 | Generate typed capability graphs and closed activation plans | Accepted | Not started |
| 0046 | Coordinate owner shutdown through acquisition ledgers and fault profiles | Accepted | Not started |
| 0047 | Evaluate dimensional compatibility through bounded exact policy profiles | Accepted | Not started |
| 0048 | Generate stable component and world-resource schemas | Accepted | Not started |
| 0049 | Keep ECS storage private behind world-owned envelopes | Accepted | Not started |
| 0050 | Generate phase-scoped queries with canonical iteration | Accepted | Not started |
| 0051 | Plan and publish structural changes through atomic commit frontiers | Accepted | Not started |

Accepted product ADR 0037 amends ADR 0024's catalog-adoption rollback contract; it does not authorize arbitrary postcommit world rewind.

ADR 0044 refines accepted ADR 0043 by selecting the declaration toolchain, allocation profiles, codec allow-list, and diagnostic-redaction rules. It is accepted via Option A; implementation has not started.

ADRs 0045-0047 were accepted independently via Option A. Their bounded first implementation scopes may proceed under their complete predecessor gates, but implementation remains `Not started`. ADR 0046's CP02 cleanup/fault profile and ADR 0047's CP01 core/Preview compatibility profile remain separate gates requiring review and approval before their profile-governed production behavior begins.

Accepted ADR 0041 amends ADRs 0042 and 0043 only to add the bounded replay product requirement. Replay's artifact, identities, mappings, compatibility profile, fault profile, and verifier remain separate `REPLAY-AUTHORITATIVE` work.

## Accepted CP03 technical decisions

The first CP03 simulation-kernel decision batch was accepted independently via Option A in strict dependency order: ADR 0048 defines semantic state, ADR 0049 keeps physical storage private, ADR 0050 defines generated query behavior, and ADR 0051 defines structural planning and atomic publication. All four retain implementation status `Not started`. Their acceptance satisfies the CP03 design gate but supplies no implementation evidence; production CP03 work still waits for the roadmap's CP02 predecessor/evidence boundary and every retained specification and profile gate.

| ADR | Decision | Program ID | Decision status | Implementation | Accepted option |
|---|---|---|---|---|---|
| 0048 | [Generate stable component and world-resource schemas](technical/0048-generate-stable-component-and-world-resource-schemas.md) | `SIM-STATE` | Accepted | Not started | Option A |
| 0049 | [Keep ECS storage private behind world-owned envelopes](technical/0049-keep-ecs-storage-private-behind-world-owned-envelopes.md) | `SIM-STORAGE` | Accepted | Not started | Option A |
| 0050 | [Generate phase-scoped queries with canonical iteration](technical/0050-generate-phase-scoped-queries-with-canonical-iteration.md) | `SIM-QUERY` | Accepted | Not started | Option A |
| 0051 | [Plan and publish structural changes through atomic commit frontiers](technical/0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) | `SIM-COMMIT` | Accepted | Not started | Option A |

## Queued decision program

The [`ADR development program`](../status/adr-development-program.md) consolidates the platform roadmap's 99 source questions into dependency-ordered product ADR, technical ADR, and specification-first work packages. Program entries are planning identifiers, reserve no ADR numbers, and have no decision authority until an ADR is drafted and accepted or a specification is reviewed under an already accepted parent ADR.
