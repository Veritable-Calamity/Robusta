# M0 Outcome and Evidence Contract

- **Status:** Implemented
- **Date:** 2026-07-18
- **Owner:** Robusta maintainers

Milestone 0 establishes what may be claimed and how later milestones prove it. It does not demonstrate a gameplay capability or resolve any world-model question reserved for M1.

## First-release contract

ADR 0014 accepts the exact 1.0 boundary, support matrix, distribution channels, and launcher/registry split. The support matrix is intentionally small enough to test on clean machines:

| Environment | Creator tools | Client | Dedicated server | Clean-machine image |
|---|---:|---:|---:|---|
| Windows 11 x64, 24H2+ | Supported | Supported | Supported | `windows-2025` |
| Ubuntu 24.04 LTS x64 | Supported | Supported | Supported | `ubuntu-24.04` |
| Ubuntu 24.04-based OCI container | No | No | Supported | built from the Ubuntu job |

The GitHub Actions workflow builds and tests on both clean-machine images, creates a versioned local NuGet-compatible feed, and proves that a project outside the repository restores SDK packages without project references.

## Auditable evidence

- [`evidence/evidence-packet.schema.json`](evidence/evidence-packet.schema.json) is the common packet format.
- [`evidence/ledger.json`](evidence/ledger.json) maps every accepted product ADR to scenarios and durable evidence locations.
- [`capabilities.json`](capabilities.json) publishes labels and missing evidence. All feature capabilities remain `Experimental`; M0 governance is not a gameplay claim.
- [`metrics-baseline.json`](metrics-baseline.json) defines measurements, owners, fixtures, units, and collection milestones. Values not honestly measurable from the scaffold are explicitly `null`.
- CI-generated evidence is uploaded from `artifacts/evidence`; accepted durable evidence is copied or linked beneath `docs/status/evidence/`.

Evidence packets are append-only observations. A failed or superseded run remains addressable. A capability label changes only after the ledger links all applicable quality-bar facets.

## External reference games

The reference-game charters are in [`reference-games.md`](reference-games.md). Each game must live in a repository with maintainers independent of Robusta implementation approval. Both consume only published artifacts. A fixture inside this repository may test packaging mechanics, but never counts as external-game evidence.

## Migration baseline

[`migration/census-v1.json`](migration/census-v1.json) is the first versioned feature inventory and [`migration/conformance-corpus-v1.json`](migration/conformance-corpus-v1.json) defines the representative cases required by ADR 0010. M0 records the observable legacy concepts and expected assessment categories; it does not claim conversion automation or freeze native contracts.

## Exit assessment

- ADR 0014 accepts the implementation-facing 1.0 and platform-support decisions.
- Every accepted product ADR, including ADR 0014, has a ledger entry.
- Capability labels and evidence paths are public, and scaffold-only outputs are marked as such.
- Both external games have separate charters, accountable ownership roles, and the published-artifact rule.

M0 is complete. Product ADRs 0000-0013 remain unimplemented unless their own executable evidence later demonstrates otherwise; ADR 0014 is `In progress` because later milestones must prove its delivery behavior.

