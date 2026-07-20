# First-Release Scope and Evidence Baseline

- **Baseline status:** Established
- **Roadmap checkpoint:** M0 complete
- **Date:** 2026-07-18
- **Last reconciled:** 2026-07-19
- **Owner:** Robusta maintainers

This baseline establishes what may be claimed and how later delivery work must prove it. It does not demonstrate a gameplay capability. Accepted ADRs 0030-0038 answer world-model questions 13-23; their product semantics are settled, while the dependent spatial, persistence, catalog-adoption, and collaborative-authoring technical mechanisms and all implementation evidence remain open. Questions 24-26 now have review-ready proposed answers in ADRs 0039-0041, and foundational technical ADRs 0042-0043 are also proposed; none is accepted or implementation authority. Each product proposal explicitly identifies the amendment its acceptance would make to ADR 0014's first-release diagnostics or qualification floor.

The [2026-07-19 ADR coherence and first-release baseline audit](adr-coherence-and-first-release-baseline-2026-07-19.md) identifies conflicts and missing contracts and distinguishes a Preview walking skeleton from Robusta 1.0. The [technical scope matrix](first-release-technical-scope-matrix.md) records the reconciled release/deferred boundary, while the [evaluation workloads](../specifications/technical-evaluation-workloads.md) make comparison and later budgeting taskable. This M0 record remains the evidence-governance baseline; neither planning artifact is evidence that the 1.0 feature boundary is executable yet.

## First-release contract

ADR 0014 defines the exact 1.0 boundary, support matrix, distribution channels, and launcher/registry split. The support matrix is intentionally small enough to test on clean machines:

| Environment | Creator tools | Client | Dedicated server | Clean-machine image |
|---|---:|---:|---:|---|
| Windows 11 x64, 24H2+ | Supported | Supported | Supported | `windows-2025` |
| Ubuntu 24.04 LTS x64 | Supported | Supported | Supported | `ubuntu-24.04` |
| Ubuntu 24.04-based OCI container | No | No | Supported | built from the Ubuntu job |

The GitHub Actions workflow builds and tests on both clean-machine images, creates a versioned local NuGet-compatible feed, and proves that a project outside the repository restores SDK packages without project references.

## Auditable evidence

- [`evidence/evidence-packet.schema.json`](evidence/evidence-packet.schema.json) is the common packet format.
- [`evidence/ledger.json`](evidence/ledger.json) maps every accepted product ADR to scenarios and durable evidence locations.
- [`capabilities.json`](capabilities.json) publishes labels and missing evidence. All feature capabilities remain `Experimental`; establishing evidence governance is not a gameplay claim.
- [`metrics-baseline.json`](metrics-baseline.json) defines measurements, owners, fixtures, units, collection outcomes, and separate roadmap metadata. Values not honestly measurable from the scaffold are explicitly `null`.
- CI-generated evidence is uploaded from `artifacts/evidence`; accepted durable evidence is copied or linked beneath `docs/status/evidence/`.

Evidence packets are append-only observations. A failed or superseded run remains addressable. A capability label changes only after the ledger links all applicable quality-bar facets.

## External reference games

The reference-game charters are in [`reference-games.md`](reference-games.md). Each game must live in a repository with maintainers independent of Robusta implementation approval. Both consume only published artifacts. A fixture inside this repository may test packaging mechanics, but never counts as external-game evidence.

The repository locations and named independent maintainers are not yet assigned. That remains an external-use blocker, and the fixture inputs in the evaluation-workload specification remain provisional until those owners approve or replace them.

## Migration baseline

[`migration/census-v1.json`](migration/census-v1.json) is the first versioned feature inventory and [`migration/conformance-corpus-v1.json`](migration/conformance-corpus-v1.json) defines the representative cases required by ADR 0010. This migration baseline records the observable legacy concepts and expected assessment categories; it does not claim conversion automation or freeze native contracts.

## Baseline assessment

- ADR 0014 defines the implementation-facing 1.0 and platform-support decisions.
- Every accepted product ADR, including ADR 0014, has a ledger entry.
- Capability labels and evidence paths are public, and scaffold-only outputs are marked as such.
- Both external games have separate charters, accountable ownership roles, and the published-artifact rule.

The first-release scope and evidence-governance baseline is established, satisfying the M0 roadmap checkpoint. Every accepted product ADR other than ADR 0014 remains `Not started` unless its own executable evidence later demonstrates otherwise; ADR 0014 is `In progress` because later delivery evidence must prove its behavior. Accepted technical ADRs 0017-0025 and 0028-0029 are also unimplemented.

Proposed ADRs 0039-0043 do not enter the accepted evidence ledger or binding roadmap traceability until explicitly accepted. Numeric performance and resource budgets remain `null` until the versioned workloads are measured on supported environments and reviewed.

