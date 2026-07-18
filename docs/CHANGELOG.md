# Documentation Changelog

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
