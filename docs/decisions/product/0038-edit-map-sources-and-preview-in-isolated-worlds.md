# ADR 0038: Edit map sources and preview them in isolated worlds

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0005, 0007, 0009, 0011-0015, 0021, 0024, 0027, 0029, 0030, 0031, 0033, 0035-0037

## The question

What should creating, editing, saving, inspecting, and testing a map feel like, and how does that workflow avoid a second privileged engine path?

## The promise

Creators edit readable map sources through published document operations, inspect their resolved meaning, and launch isolated local-authority previews. Source compilation uses the canonical schemas and semantic validators; preview construction then uses the ordinary runtime lifecycle, spatial, and authority contracts. Preview gameplay never silently becomes authored content.

## Why this matters

An editor that mutates private runtime state or uses a separate serializer will drift from release behavior. Saving an arbitrary test world as a source map captures accidental simulation state. A creator still needs recoverable drafts, visual placement, immediate diagnostics, and fast gameplay preview.

## How Robust Toolbox answers today

SS14 offers valuable in-game mapping with real client/server simulation, entity and tile spawners, inspectors, and map commands. Its documented workflow also requires special builds, free runtime map IDs, separate map/grid save commands, round restarts for testing, and care not to save test mutations. Robusta should preserve in-context editing while making artifact and preview boundaries explicit.

## How the Robusta prototype answers today

The predecessor has no accepted map-source model, editor transaction protocol, isolated preview workflow, or release audit proving development powers are absent.

## Options considered

### Option A: Canonical source-document editing with ordinary isolated previews

Edit an authored document through explicit creator-document capabilities and canonical schemas and validators. Compile the valid document, then preview it in a separate ordinary local-authority world.

### Option B: Edit a gameplay world and serialize its current state as the map

This reuses runtime tools but mixes authored intent with timers, damage, spawned effects, sessions, and other transient state.

### Option C: Build separate editor semantics, compiler, and runtime model

This goes beyond the editor-specific draft model required by Option A: it duplicates the meaning, validation, compilation, and preview behavior of maps, allowing editor output to drift from runtime lifecycle, physics, prototypes, and migration.

## Decision

Robusta will use Option A, including server-hosted collaborative mapping sessions. Option B's useful in-context, multi-user editing experience is retained; its arbitrary gameplay-world serialization is not.

The product contract is:

1. A **map source** is a readable authored artifact compiled through ADR 0021. It describes intended initial spatial, compact-data, entity, relationship, and game-defined map state; it is not a world checkpoint. Compilation produces the immutable package-qualified **map definition** from ADR 0030, which may later create runtime map instances.
2. The selected workspace lock and catalog generation determine every available prototype, resource, schema, and editor extension. Resolved values and source provenance are inspectable before placement or publication.
3. Editing may be presented through a live, in-context map view. Every change that can persist still originates as a published, schema-qualified editor-document command against source identity and provenance; the live view does not turn arbitrary runtime mutation into authored state.
4. A server host may deliberately start a collaborative mapping session as a declared creator-authority profile. The authority owns one canonical draft revision and ordered edit journal and admits authenticated participants with explicit author, reviewer, or observer capabilities. Accepted commands name their base revision and preconditions; conflicts are rejected or resolved by a documented deterministic policy.
5. The mapping world is a derived projection of the canonical draft, not the artifact of record. Saving writes the source document and accepted document transactions. It never serializes an arbitrary entity store, gameplay world, world checkpoint, native resource, callback, timer queue, session, or network state as a map source.
6. Placement, movement, compact-cell, entity, and relationship edits performed through declared mapping tools update both the canonical draft and its derived view transactionally. Damage, timers, AI, physics aftermath, spawned effects, ordinary gameplay commands, and other mutations that did not originate as accepted document commands remain transient and are excluded from save.
7. An editor may preserve an incomplete or invalid working draft with diagnostics and recovery history. Creator permission does not bypass validation or expose private storage mutation. Only a fully validated compiled definition may enter preview, tests, packaging, or publication, where ordinary runtime lifecycle and authority invariants apply.
8. Multi-user undo and redo are revision-aware document transactions or explicit compensating transactions; they may not silently erase another participant's accepted work. Draft recovery and audit history identify actor, command, base revision, result, and source locations without treating gameplay callbacks as editor history.
9. Saving writes the declared source artifact atomically and never mutates the installed immutable catalog. A declared package-qualified map-definition identity is stable under its identity policy; compiling the same semantic source with the same exact dependencies produces the same canonical definition fingerprint. Changing semantics may retain the logical definition identity while producing a new catalog generation and fingerprint.
10. Gameplay preview compiles a validated draft into a new immutable development generation and starts a separate isolated preview world. A collaborative mapping authority may supervise that preview, but the editable projection is not promoted in place. Preview movement, damage, timers, spawned objects, and player actions never flow back except through an explicit reviewed import operation.
11. Local preview uses ADR 0027's launcher-supervised loopback authority. A remotely reachable collaborative mapping session is not offline play: it is explicitly configured by the server host, uses the versioned creator protocol and authenticated creator roles, and exposes no launcher or package-management authority.
12. Several editor sessions or preview worlds may run without sharing mutable state. Runtime world identity, map identity, authority-process identity, editor-session identity, document identity, logs, diagnostics, and supervisor ownership remain distinct even when one authority host runs several worlds.
13. Ordinary production game-client and authority-server projections contain no editor authority, draft parser, workspace watcher, or collaborative-mapping endpoint. A host may intentionally install and launch separately declared creator-client and creator-authority projections from exact trusted artifacts; their identities, enabled interfaces, and trust are visible in configuration, receipts, logs, and diagnostics, and they are never activated merely by joining or administering an ordinary game server. These projections are trusted editor extensions under ADR 0007, not public-add-on capabilities, and their installation never grants executable editor power to public UGC.
14. Document history and undo, supervisor rollback under ADR 0024 as amended by ADR 0037, gameplay checkpoints, and map identities remain distinct. Rejecting or undoing an edit changes the draft projection; it does not claim arbitrary gameplay-world rewind or a general world-to-map serialization path.

## What we deliberately will not do

- Save an arbitrary gameplay world as authored map source by default.
- Give editor tools an undocumented private mutation or serialization path.
- Turn collaborative mapping permission into production gameplay authority or expose a mapping session without explicit authentication and role admission.
- Require runtime numeric map IDs to identify source maps.
- Allow an invalid draft to enter release packaging or gameplay preview.
- Let preview changes modify authoring state without an explicit reviewed import.
- Ship creator authority, workspace watchers, permissive draft inputs, or development endpoints in production game client or authority-server payloads.

## Consequences

### Benefits

- Visual authoring exercises real platform semantics without contaminating source with gameplay state.
- Draft recovery and strict publication validation can coexist.
- Preview is reproducible, isolated, and available through the supported creator workflow.
- Editor extensions remain testable public contracts rather than engine internals.

### Costs and limitations

- Robusta needs a draft model, transaction history, inspector, fast compilation, and preview supervision.
- Not every arbitrary system effect can be made reversible or editable.
- Game-specific map concepts require supported editor extensions.
- Collaborative hosting requires creator-specific client and authority projections, authenticated roles, ordered commands, conflict handling, audit retention, redaction, remote deployment controls, and resource limits.
- Cross-branch semantic source merging remains later work; server-hosted collaborative editing is part of this accepted contract.

## How we will prove the decision works

- From a clean machine, a creator starts `robusta dev`, creates a map, places compact cells and prototype entities, edits declared relations, and inspects resolved provenance.
- Invalid placement or relationship changes preserve a recoverable draft but cannot preview or package.
- Undo and redo restore complete committed document states; reopening preserves the declared definition identity, and exact recompilation preserves the canonical fingerprint.
- A server and two clients preview the draft through ADR 0027's separately launched local authority; gameplay mutations disappear when preview ends and the draft remains unchanged.
- Two simultaneous previews do not share entities, timers, maps, random state, or diagnostics.
- A host starts an authenticated creator authority and two creators concurrently place, move, and relate objects; accepted revisions converge, stale commands receive stable conflict outcomes, and unauthorized users can neither edit nor retrieve hidden draft material.
- Save and reopen reproduce the canonical source and revision history while injected damage, timers, AI changes, and spawned effects remain absent; disconnect and reconnect preserve document identity and admitted history.
- Headless validation and release packaging produce the same diagnostics as the editor.
- Production client and authority-server scans find no editor authority, workspace watcher, development endpoint, or alternate permissive draft parser; explicit creator projections contain only their declared endpoints and separately authorized operator interfaces remain in their declared artifacts.
- Preview supervision proves that client, authority-process, runtime-world, runtime-map, and editor-session identities cannot be substituted for one another.

## Implementation notes

No map source schema, draft model, edit protocol, collaborative creator-authority or creator-client projection, authentication and role contract, inspector, transaction history, preview coordinator, or release capability audit exists.

## Follow-up decisions

- Map source and compiled IR schemas after spatial decisions are accepted.
- Creator edit command, base-revision, precondition, transaction-history, conflict, and extension protocols.
- Draft autosave, crash recovery, source diff, semantic branch merge, audit retention, and redaction policy.
- Creator identity, authentication, roles, remote deployment, supervision, and resource limits.
- Runtime inspection and source-provenance APIs.
- Preview lifecycle, client attachment, resource limits, and supervisor integration.

## References

- [ADR 0009](0009-one-supported-creator-workflow.md)
- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0021](../technical/0021-compile-content-into-a-canonical-provenance-catalog.md)
- [ADR 0024](../technical/0024-supervise-the-creator-loop-as-an-observable-transaction.md)
- [ADR 0027](0027-run-offline-play-through-a-separate-local-authority.md)
- [ADR 0030](0030-define-runtime-maps-and-frame-qualified-coordinates.md)
- [ADR 0031](0031-separate-spatial-containment-attachment-and-lifecycle-relations.md)
- [ADR 0033](0033-provide-platform-mechanics-with-game-defined-semantics.md)
- [ADR 0035](0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [World-model question 23](../../workshops/world-model-question-set.md#23-what-should-map-editing-and-preview-feel-like)
- [Space Station 14 mapping workflow](https://docs.spacestation14.com/en/space-station-14/mapping/guides/general-guide.html)
