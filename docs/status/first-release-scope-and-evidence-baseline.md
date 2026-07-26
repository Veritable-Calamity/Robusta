# First-Release Scope and Evidence Baseline

- **Baseline status:** Established
- **Roadmap checkpoint:** M0 complete
- **Date:** 2026-07-18
- **Last reconciled:** 2026-07-26
- **Owner:** Robusta maintainers

This baseline establishes what may be claimed and how later delivery work must prove it. It does not demonstrate a gameplay capability. Accepted ADRs 0030-0041 answer world-model questions 13-26 through Option A; their product semantics are settled, while the dependent spatial, persistence, catalog-adoption, collaborative-authoring, inspection, Test SDK, and authoritative-replay mechanisms remain incomplete. ADR 0039 establishes bounded authorized committed observations, ADR 0040 establishes a published Test SDK over ordinary runtime activation and cleanup, and ADR 0041 establishes bounded authoritative re-execution only inside an exact validated compatibility domain and fixed declared partition scheme. ADR 0041 does not establish universal bitwise, cross-platform, cross-backend, cross-partition-scheme, or cross-release determinism. Technical ADRs 0042-0051 are accepted via Option A; ADRs 0048-0051 settle the CP03 semantic-state, private-storage, canonical-query, and atomic-commit design boundaries while retaining implementation status `Not started`. ADR 0046's CP02 cleanup/fault profile, ADR 0047's CP01 core/Preview compatibility profile, the CP02 predecessor/evidence boundary, CP03 subordinate specifications and workloads, and the feature-specific inspection, test-execution or world-construction, replay-reexecution, and replay-owner profiles remain separate gates. The internal ownership kernel provides initial evidence for ADRs 0017, 0028, and 0043 without demonstrating gameplay.

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

The first-release scope and evidence-governance baseline is established, satisfying the M0 roadmap checkpoint. Every accepted product ADR other than ADR 0014 remains `Not started` unless its own executable evidence later demonstrates otherwise; ADR 0014 is `In progress` because later delivery evidence must prove its behavior. Technical ADRs 0017, 0028, and 0043 are `In progress` through the bounded ownership and ephemeral-identity groundwork; other accepted technical ADRs remain `Not started`.

Accepted ADRs 0039-0041 enter the evidence ledger and binding roadmap traceability, but all three retain implementation status `Not started`. Their acceptance opens `OBS-INSPECTION`, `TEST-RUNTIME`, and `REPLAY-AUTHORITATIVE`; it does not define their schemas or public packages, approve their compatibility and fault profiles, grant inspection or operator authority, freeze a durable replay format, or provide implementation evidence. Accepted ADRs 0042-0051 likewise enter implementation evidence only as their bounded contracts are built and demonstrated. ADRs 0048-0051 satisfy the four-decision CP03 design gate but remain unimplemented; immediate work moves to the retained CP01/CP02 foundation profiles, bounded CP03 subordinate specifications, internal conformance fixtures and workload characterization, the remaining CP04 decisions, and the technical packages and profiles opened by ADRs 0039-0041. Numeric performance and resource budgets remain `null` until the versioned workloads are measured on supported environments and reviewed.

