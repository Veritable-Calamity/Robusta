# World Model Workshop 05: Space, foundations, persistence, and preview

- **Workshop status:** Accepted
- **Date:** 2026-07-19
- **Questions:** 13-23 from the world-model question set
- **Decision outcome:** ADRs 0030-0038 accepted via Option A on 2026-07-19; ADR 0037 amends ADR 0024

## Why these questions are together

Maps and coordinate meaning constrain containment, transfer, saves, and map authoring. The platform-versus-game boundary constrains which spatial mechanics Robusta must supply, while the extension boundary decides how unusual games can add capabilities without depending on internals. Save scope and durable references then determine what can survive restart, and prototype adoption determines what a creator preview may change safely.

This set resolves product behavior only. It does not select an ECS layout, transform representation, grid storage, physics or rendering library, save encoding, database, editor UI framework, or wire protocol.

## Accepted decisions

| ADR | Product choice | Decision outcome | Main rejected shortcuts |
|---|---|---|---|
| [0030](../decisions/product/0030-define-runtime-maps-and-frame-qualified-coordinates.md) | Runtime maps are world-local root frames; spatial values identify their frame and other coordinate domains use typed adapters. | Accepted via Option A | One universal plane; treating every spatial datum as an entity relation |
| [0031](../decisions/product/0031-separate-spatial-containment-attachment-and-lifecycle-relations.md) | Spatial parent, containment, attachment, lifecycle ownership, and references are distinct typed relations. | Accepted via Option A | One universal parent; entirely game-private relation models |
| [0032](../decisions/product/0032-reconstruct-explicitly-across-world-transfers.md) | Same-world relocation preserves world-local identity; cross-world transfer reconstructs declared state under new runtime identities. | Accepted via Option A | Cross-world `EntityRef`; incompatible game-specific transfer conventions |
| [0033](../decisions/product/0033-provide-platform-mechanics-with-game-defined-semantics.md) | Robusta supplies a complete genre-neutral mechanical floor while games own domain meaning. | Accepted via Option A | Minimal ECS kernel; station-game framework in engine core |
| [0034](../decisions/product/0034-use-a-declared-ladder-for-advanced-game-extensions.md) | Advanced work follows SDK, published extension, platform contribution, or explicit unsupported classification. | Accepted via Option A | Privileged internal escape hatch; no extension path |
| [0035](../decisions/product/0035-persist-declared-world-state-through-versioned-checkpoints.md) | Saves are versioned checkpoints of platform-required and game-declared durable state. | Accepted via Option A | Runtime-object-graph capture; opaque game blobs |
| [0036](../decisions/product/0036-use-explicit-durable-identities-and-reference-policies.md) | Saved references use typed scope-aware identities and declared missing-target behavior. | Accepted via Option A | Serialized runtime handles; best-effort string matching |
| [0037](../decisions/product/0037-keep-live-state-stable-unless-explicitly-migrated.md) | Prototype changes affect future births; existing state changes only through prepared migration with atomic per-world visibility or restart. | Accepted via Option A; amends ADR 0024 | Silent rebasing of live objects; restart for every safe change |
| [0038](../decisions/product/0038-edit-map-sources-and-preview-in-isolated-worlds.md) | Creators edit source documents through published transactions and canonical validators, then preview compiled definitions in separate ordinary local-authority worlds. | Accepted via Option A | Saving gameplay state as a map; separate editor semantics, compiler, and runtime model |

The decisions were accepted in dependency order: ADRs 0031 and 0032 use ADR 0030's identity and frame distinctions; ADRs 0035-0038 then use the spatial, relation, platform, and extension boundaries.

Each decision retains implementation status `Not started`. Technical mechanisms derived from these product promises require separate ADR review and acceptance.

## Acceptance qualifications

- **ADR 0033:** Any optional platform-provided, batteries-included station feature must be an ordinary, separately versioned game or component package built through the same published SDK and declared trust mechanisms available to independent developers. It receives no privileged platform internals and must comply with ADR 0033 and every other accepted ADR. This qualification does not create another trust tier.
- **ADR 0038:** A server host may run collaborative mapping sessions for authenticated creators, including live or in-world visual editing. Canonical source-document transactions and their recorded history remain the authored truth; arbitrary gameplay-world state is never serialized back as the map source. This creator-authority profile remains distinct from an ordinary production authority payload.

## Coherence constraints carried through acceptance

- A readable map source compiles into an immutable package-qualified map definition/template, which may instantiate one or more world-local runtime maps. Their logical identities, fingerprints, save records, and runtime identities are not interchangeable.
- Runtime-map publication and ending are atomic lifecycle changes; stale map or frame references never alias a replacement, and every map-owned or map-located participant has an explicit ending disposition.
- `EntityRef` remains world-local and generational. Durable continuity, catalog identity, network identity, and session identity stay separate.
- No transform, containment, attachment, or ordinary reference implies deletion ownership.
- Visual hiding, gameplay visibility, observer authorization, network interest, and secrecy are separate concerns.
- A cross-world operation fences one committed source revision and uses a durable activation record before target publication. It can promise recoverable states, but not an instantaneous failure-proof transaction across processes.
- Application rollback does not imply reverse migration of writable data.
- A checkpoint cannot omit authoritative state that affects future outcomes unless a versioned rule reconstructs it or a declared save-profile outcome resets it.
- Prototype defaults are birth inputs, not live bindings over mutable world state.
- Editor permission does not bypass runtime invariants, and development powers are absent from release artifacts.
- Invalid editor drafts use document transactions and shared semantic validators; they are not disguised live worlds. Ordinary lifecycle and authority contracts begin when a valid compiled definition is previewed.
- Space Station 14 is the station-like perspective and migration source, not the ontology of the new platform.
- The contrasting reference game must be able to omit constructed grids, station inventory rules, persistent worlds, and similar genre assumptions.
- Accepted ADR 0034 Option A is consistent with accepted technical ADR 0018's unimplemented advanced-interface clause.
- A nonconforming extension makes the affected execution profile and capabilities ineligible for the `Supported` label unless an enforced boundary proves they cannot be influenced; installation must disclose the lost guarantees.
- Accepted ADR 0037 amends ADR 0024 so catalog-adoption rollback covers preparation rejection and proven reversible commit failure while every target is fenced. Target clients must admit the new generation before migrated state or births can be published; the rule never implies arbitrary postcommit world rewind.
- Supported creator-tool artifacts may carry editor powers, but production game client and authority-server payloads may not; separately authenticated operator interfaces remain distinct.

## Effect on the first-release baseline

The first-release station-like game has a bounded, coherent target: multiple runtime map spaces, typed positions, one static compact grid, explicit containment and attachment, basic collision and spatial query, versioned checkpoint restore with one forward migration, and isolated map preview. Dynamic grid splitting and general durable cross-world object-graph transfer are later capability proofs unless ADR 0014 is explicitly superseded. The platform still owes technical ADRs for transforms, grids, physics, spatial interest, save format and transactions, catalog adoption, and editor protocols before those public contracts freeze.

These accepted decisions do not expand Robusta 1.0 to full Space Station 14 parity. No implementation or evidence is claimed by accepting them.

## Questions deliberately left for the next queue

- Question 24: the complete runtime inspection and source-provenance experience.
- Question 25: isolated-world test fixtures, fake time, and contamination-proof parallel tests.
- Question 26: replay, trace comparison, and any stronger numerical determinism promise.
- Transform graphs, compact grids, topology changes, physics ordering, and spatial interest remain technical decisions to derive under the accepted product constraints.
- Persistence encoding, repositories, migrations, durable work, and external-service coordination remain technical decisions to derive under accepted ADRs 0035-0037.

## Sources reviewed

- [ADR coherence and first-release baseline audit](../status/adr-coherence-and-first-release-baseline-2026-07-19.md)
- [Robust Toolbox coordinate systems](https://docs.spacestation14.com/en/robust-toolbox/coordinate-systems.html)
- [Robust Toolbox grids](https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html)
- [Space Station 14 mapping workflow](https://docs.spacestation14.com/en/space-station-14/mapping/guides/general-guide.html)
- The pinned Robusta predecessor, Robust Toolbox, and Space Station 14 revisions listed in the audit source notes
