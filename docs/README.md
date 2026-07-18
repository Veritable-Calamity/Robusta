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

The first catalog is [`specifications/m1-behavioral-scenarios.json`](specifications/m1-behavioral-scenarios.json). It gives the accepted ADR proof statements stable Given/When/Then contracts and test names while leaving unresolved M1 choices behind explicit decision gates.

## 5. Guides

Guides explain how to use the released platform. They describe supported workflows rather than internal architecture.

## 6. Status and evidence

Roadmaps, compatibility matrices, implementation status, benchmark results, and conformance reports show what has actually been built.

The current evidence-gated roadmap is [`status/development-plan.md`](status/development-plan.md). Milestone 0's implemented governance contract, support matrix, evidence ledger, capability register, metrics, reference-game charters, and migration baseline begin at [`status/m0-outcome-and-evidence-contract.md`](status/m0-outcome-and-evidence-contract.md).

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
