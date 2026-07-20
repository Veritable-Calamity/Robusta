# Documentation Changelog

## 2026-07-19 — Space, persistence, and preview ADRs accepted

- Accepted product ADRs 0030-0038 via Option A, closing world-model questions 13-23 while leaving implementation `Not started` and questions 24-26 queued.
- Recorded that ADR 0037 amends ADR 0024's catalog-adoption rollback contract without promising arbitrary postcommit world rewind.
- Qualified ADR 0033 so optional batteries-included station conveniences are ordinary, separately versioned game or component packages using the same published SDK and declared trust mechanisms available to independent developers; they receive no privileged internals and create no new trust tier.
- Qualified ADR 0038 to support authority-hosted collaborative mapping for authenticated creators, including live or in-world editing, while canonical source-document transactions remain the authored truth and arbitrary gameplay-world state is not serialized back into map sources.

## 2026-07-19 — Space, persistence, and preview ADR review set drafted

- Added draft product ADRs 0030-0034 for runtime maps and typed coordinate domains, distinct spatial and lifecycle relations, explicit cross-world reconstruction, a genre-neutral platform floor, and a declared advanced-extension ladder.
- Added draft product ADRs 0035-0038 for versioned declared checkpoints, scoped durable identities and missing-reference policies, stable live state during catalog adoption, and source-oriented map editing with isolated local-authority previews.
- Set Option A as the current review position for each new ADR without accepting any decision or claiming implementation.
- Updated the workshop queue, decision indexes, roadmap gates, coherence audit, and first-release baseline so dependent technical ADRs remain blocked until this product review completes.

## 2026-07-19 — ADRs 0026-0029 accepted

- Accepted ADR 0026 via Option A, defining the supported in-process game-code conformance boundary and fail-closed fault outcomes without claiming arbitrary trusted code is sandboxed.
- Accepted ADR 0027 via Option A, selecting one-click offline play through a separately installed and launcher-supervised local authority.
- Accepted ADR 0028 via Option A, amending ADR 0017 so host-owned session and world scopes are siblings joined by explicit attachment scopes.
- Accepted ADR 0029 via Option A, supplying ADR 0020 with phase-scoped access, deterministic buffered effects, safe exclusive fallback, and serial-oracle semantics.
- Added evidence-ledger entries and conformance scenarios for detectable violations, localized and integrity-unknown faults, and the separate local-authority journey; implementation remains not started.

## 2026-07-19 — ADR coherence and first-release baseline audit

- Audited product ADRs 0000-0016 and technical ADRs 0017-0025 against the greenfield scaffold, the pinned Robusta predecessor, Robust Toolbox, Space Station 14, and established engine and supply-chain practices.
- Recorded the ADR 0017 scope-graph conflict, offline-authority topology collision, deterministic-parallelism enforcement gap, unresolved spatial and persistence foundations, and the bounded Robusta 1.0 qualification profile.
- Added draft product ADRs 0026-0027 for supported-code fault containment and separate local authority, plus draft technical ADRs 0028-0029 for corrected host scopes and phase-scoped deterministic effects. No draft is accepted and no implementation is claimed.
- Pinned predecessor, Robust Toolbox, and Space Station 14 source references used by the audit.

## 2026-07-19 — Remaining technical ADRs accepted

- Accepted ADR 0020 via Option C: deterministic parallel authoritative batches are required from the first release, with stable merge boundaries and a mandatory serial oracle.
- Accepted ADR 0025 from its recorded review position: typed semantic migration leads, textual replacement is limited to reviewable suggestions, and legacy binary emulation is prohibited.
- Closed the initial technical ADR review set; implementation remains not started.

## 2026-07-19 — Seven technical ADRs accepted

- Accepted technical ADRs 0017-0019 and 0021-0024 via Option A; implementation remains not started.
- Kept ADR 0020 open to compare serial-first and deterministic parallel-first authoritative scheduling.
- Kept ADR 0025 open with typed migration leading, limited interest in text-assisted suggestions, and legacy binary emulation explicitly ruled out.

## 2026-07-19 — Initial technical ADR set derived

- Added draft technical ADRs 0017-0025 for runtime ownership, SDK boundaries, entity lifecycle, deterministic scheduling, canonical content, immutable delivery, authoritative replication, creator supervision, and assisted migration.
- Linked the behavioral decision gates and development plan to the corresponding drafts without accepting them or claiming implementation.
- Left space/maps, persistence, replay formats, and dependent editor semantics gated on their queued product decisions.

## 2026-07-19 — Entity lifecycle and simulation time accepted

- Accepted ADR 0015 via Option A for atomic entity birth, capability mutation, death, relationship cleanup, and explicit stale-reference outcomes.
- Accepted ADR 0016 via Option A for fixed authoritative steps, bounded catch-up, whole-world pause, simulation-time timers, and non-authoritative presentation time.
- Closed both product decision gates and added stable behavioral scenarios and evidence-ledger traceability without claiming runtime implementation.

## 2026-07-19 — Capability-oriented contract names and executable schemas

- Renamed the product behavior catalog and first-release evidence baseline so their identities describe their contents; milestone codes remain only as roadmap metadata.
- Split architecture checks into product-behavior, evidence-ledger, capability-registry, measurement, migration, platform-pipeline, and status-schema contracts.
- Replaced milestone-coded delivery evidence identifiers, added collision-resistant packet identities, and made generated packets and the product behavior catalog execute their JSON Schema contracts.

## 2026-07-18 — Entity lifecycle and simulation time proposed

- Added proposed ADR 0015 for atomic entity birth, capability mutation, death, relationship cleanup, and stale-reference behavior.
- Added proposed ADR 0016 for fixed authoritative steps, bounded catch-up, whole-world pause, simulation-time timers, and non-authoritative presentation time.
- Recorded the comparison workshop and nine explicit product choices awaiting approval; no runtime implementation or accepted decision is claimed.

## 2026-07-18 — Product behavioral scenario catalog established (roadmap M1)

- Converted the accepted proof statements for ADRs 0003-0009 and 0011-0013 into technology-neutral Given/When/Then scenarios with stable conformance-test names.
- Added a versioned behavioral-scenario schema and explicit product and technical decision gates without claiming runtime implementation.
- Added architecture tests that keep the scenario catalog aligned with the evidence ledger and accepted product ADRs.

## 2026-07-18 — First-release scope and evidence baseline established (roadmap M0)

- Accepted ADR 0014 defining the exact 1.0 boundary, Windows and Ubuntu support matrix, artifact channels, and launcher/registry responsibilities.
- Added the evidence-packet schema, complete ADR traceability ledger, capability register, metric baselines, and first-release baseline assessment.
- Added independent reference-game charters and a versioned Robust Toolbox migration census and conformance corpus.
- Added clean-machine CI, versioned SDK packing, external NuGet consumption verification, and architecture tests for the release scope and evidence baseline.

## 2026-07-18 — Initial development plan

- Added an evidence-gated development plan derived from accepted product ADRs 0000-0013.
- Sequenced product-decision and technical-ADR gates, a published walking skeleton, native gameplay, authoritative multiplayer, delivery and trust, creator workflow, migration, and release qualification.
- Added milestone exit evidence, the capability definition of done, ADR traceability, major risks, and work safe to begin before the next design workshop.
- Kept every ADR implementation status at `Not started`; the plan records intended sequencing rather than implementation evidence.

## 2026-07-18 — Initial design baseline

- Added the Robusta Platform Constitution.
- Added the product quality bar and plain-language glossary.
- Established the ADR lifecycle and template.
- Recorded eleven accepted product ADRs covering:
  - platform identity and audience;
  - measurable quality and release readiness;
  - the game-authoring model and supported SDK boundary;
  - game packaging and runtime isolation;
  - content authoring and compilation;
  - multiplayer authority and synchronization;
  - trust tiers and public UGC;
  - compatibility, upgrades, migrations, and rollback;
  - the creator development workflow;
  - migration from Robust Toolbox.
- Added a record of the foundational workshop.
- Added the next workshop question set for worlds, objects, time, maps, and persistence.
- Added a reference map from prototype-era Robusta ADRs to the new product decisions.
- Added the first world-model ADR and its comparison workshop for review: a world as an isolated simulation containing multiple maps.

## 2026-07-18 — World ownership update

- Accepted ADR 0011: a world is an isolated mutable simulation that may contain multiple maps.
- Updated the world glossary and question backlog to reflect the accepted decision.
- Added World Model Workshop 02.
- Added proposed ADR 0012 separating platform infrastructure, immutable game definitions, host and player-session state, and mutable world state.

## 2026-07-18 — World ownership and object-model update

- Reconciled ADR 0011 and its workshop record with the previously accepted world decision.
- Accepted ADR 0012 as written: platform infrastructure, immutable game definitions, host and player-session state, and mutable world state have explicit owners.
- Added glossary terms for avatars, player sessions, content-catalog generations, and durable services.
- Updated the ADR register and world-model question backlog.
- Added World Model Workshop 03.
- Added proposed ADR 0013: entities represent independent world participants with identity and lifecycle, while values, definitions, bulk data, presentation state, and wider service state remain distinct.

## 2026-07-18 — ADR 0013 accepted, constructed grids clarified, and Codex handoff prepared

- Accepted ADR 0013 with implementation status `Not started`.
- Clarified the Robust Toolbox hybrid model: a grid is an entity, ordinary tile cells live in compact chunk data, and independently anchored objects remain entities.
- Recorded Space Station 14's `Space → Lattice → Plating → Floor` construction path. Each coordinate has one current tile value; base-turf rules and optional history preserve the relationship for placement and deconstruction.
- Recorded first-tile grid creation and Robust Toolbox's ability to split disconnected grid sections and reassign their contents.
- Clarified that Robusta's later map-and-grid ADR may choose one resolved tile plus history, explicit logical layers, or a hybrid; ADR 0013 only rejects an automatic entity per ordinary cell or layer.
- Updated the workshop record, ADR register, glossary, source notes, and world-model backlog.
- Added a Codex continuation brief and left no active proposed ADR or newly opened workshop question.
