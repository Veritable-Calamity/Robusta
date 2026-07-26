# Documentation Structure

Robusta documentation is divided by purpose so that product promises are not confused with current implementation details.

## 1. Platform constitution

The constitution states the durable promises of the product. It answers questions such as who Robusta is for, what a game is, what users may rely on, and how safety and compatibility are treated.

A technical design that conflicts with the constitution must be changed or accompanied by an explicit proposal to amend the constitution.

## 2. Product ADRs

Product ADRs record decisions visible to game developers, public creators, players, or operators. They remain readable without detailed engine knowledge.

Examples:

- A game is an independently installable application package.
- The server is the final authority in multiplayer.
- Public add-ons do not receive unrestricted computer access.

## 3. Technical ADRs

Technical ADRs choose mechanisms: storage layouts, libraries, protocols, file encodings, process supervision, renderer design, and similar implementation matters.

Every technical ADR must identify which product decision or user promise it serves. Technical ADRs may change more frequently than product ADRs.

## 4. Specifications

Specifications define behavior precisely enough to test. Planned examples include:

- entity and component lifecycle;
- event ordering;
- prototype composition;
- network state and prediction;
- package manifests and locks;
- save and map migration.

The product catalog is [`specifications/product-behavior-scenarios.json`](specifications/product-behavior-scenarios.json). It gives accepted ADR proof statements stable Given/When/Then contracts and conformance-test identifiers while leaving unresolved choices behind explicit decision dependencies.

## 5. Guides

Guides explain how to use the released platform. They describe supported workflows rather than internal architecture.

## 6. Status and evidence

Roadmaps, compatibility matrices, implementation status, benchmark results, and conformance reports show what has actually been built.

The proposed whole-platform checkpoint spine is [`status/platform-development-roadmap.md`](status/platform-development-roadmap.md), and its [`ADR development program`](status/adr-development-program.md) consolidates the 99 source questions into dependency-ordered decision and specification gates; the detailed first-release plan remains [`status/development-plan.md`](status/development-plan.md). The established first-release scope and evidence baseline, support matrix, evidence ledger, capability register, metrics, reference-game charters, and migration baseline begin at [`status/first-release-scope-and-evidence-baseline.md`](status/first-release-scope-and-evidence-baseline.md). The roadmap records this evidence-governance baseline as the completed M0 checkpoint. The [`2026-07-19 ADR coherence and first-release baseline audit`](status/adr-coherence-and-first-release-baseline-2026-07-19.md) records cross-decision conflicts and implementation pain; the [technical scope matrix](status/first-release-technical-scope-matrix.md) and [evaluation workloads](specifications/technical-evaluation-workloads.md) turn its bounded 1.0 profile into taskable gates. The [space, persistence, and preview workshop](workshops/2026-07-19-world-model-05-space-persistence-and-preview.md) records ADRs 0030-0038 accepted via Option A. The [inspection, testing, and replay workshop](workshops/2026-07-19-world-model-06-inspection-testing-and-replay.md) records ADRs 0039-0041 accepted via Option A and their explicit ADR 0014 first-release amendments; ADR 0041 also removes only the replay-specific deferrals in ADRs 0042 and 0043. Technical ADRs 0042-0051 are accepted via Option A, and the initial internal ownership kernel begins ADRs 0017, 0028, and 0043 without claiming broader runtime capability. ADRs [0044](decisions/technical/0044-generate-bounded-identity-declarations.md), [0045](decisions/technical/0045-generate-typed-capability-graphs-and-closed-activation-plans.md), [0046](decisions/technical/0046-coordinate-owner-shutdown-through-acquisition-ledgers-and-fault-profiles.md), and [0047](decisions/technical/0047-evaluate-dimensional-compatibility-through-bounded-exact-policy-profiles.md) remain unimplemented; the next foundation reviews are ADR 0046's CP02 cleanup/fault profile and ADR 0047's CP01 core/Preview compatibility profile. The [CP03 simulation-kernel decision record](workshops/2026-07-24-simulation-kernel-01-state-storage-query-and-commit.md) records ADRs 0048-0051 accepted independently via Option A; all four remain unimplemented, and production CP03 work still waits for the CP02 predecessor/evidence boundary and retained gates. The [2D client/platform options note](reference/2d-client-platform-options.md) keeps the client bakeoff work reviewable without selecting a backend.

## Order of authority

When documents disagree, use this order:

1. Accepted platform constitution.
2. Accepted product ADRs.
3. Accepted technical ADRs.
4. Published behavioral specifications.
5. Guides and examples.
6. Planning notes and workshop drafts.

A discrepancy between the documentation and running code is a defect to be resolved; it does not silently change the accepted decision.

## 7. Handoffs

Handoff notes preserve the accepted design state, source context, paused next step, and user instructions when work moves between tools or contributors. They are informative summaries; accepted ADRs remain authoritative.
