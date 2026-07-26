# ADR 0043: Use a typed identity and compatibility spine

- **Decision status:** Accepted
- **Implementation status:** In progress
- **Date:** 2026-07-21
- **Decision level:** Technical
- **Owners:** Runtime, content, delivery, persistence, multiplayer, and creator-workflow workstreams
- **Supersedes:** None
- **Amended by:** ADR 0041, which adds bounded replay identities and replay-reexecution compatibility without changing this ADR's nominal-kind, mapping, or common compatibility-spine semantics
- **Product decisions served:** 0002-0006, 0008, 0011, 0012, 0015, 0027, 0030-0032, 0035-0041
- **Related decisions:** 0017, 0019, 0021-0024, 0028, 0039-0042, 0044-0051

## The question

How will Robusta identify and correlate runtime entities, maps, frames, network objects, checkpoints, durable concepts, catalog definitions, documents, packages, receipts, hosts, sessions, and worlds without allowing one identity to substitute for another or treating one version number as universal compatibility?

## The promise preserved

Developers and operators can follow an object or artifact across compilation, installation, launch, networking, editing, saving, and reconstruction while every boundary keeps its own identity and lifetime. A stale runtime handle cannot become a restored object, a display name cannot become a package or durable identity, and knowing any identity does not grant authority.

## Why this matters

The accepted ADRs deliberately assign different meanings to identity:

- `EntityRef` is world-local and generational;
- a network entity is attachment-local and renewed on reconnect;
- maps and spatial frames are world-local runtime instances;
- checkpoints have immutable artifact identities and optional lineage;
- durable identities express opt-in continuity under an issuer and domain;
- catalog definitions are package-qualified immutable declarations;
- map documents and revisions remain distinct from compiled definitions and preview worlds;
- packages and exact release receipts drive installation and compatibility; and
- hosts, sessions, worlds, and attachments have independent lifetimes.

Without a common technical spine, each subsystem can still encode these values as interchangeable GUIDs or strings and then join them accidentally. Conversely, one global object identity would contradict world isolation, reconstruction, session reattachment, immutable releases, and authored-document provenance.

Compatibility has the same shape. A peer may share a package name but not a network schema; a checkpoint may be forward-migratable but not directly loadable; a map document may compile under one lock but not another; and a catalog generation may be usable for future births but not for live-state migration. One release version or equality check cannot answer all of those questions.

## Options considered

### Option A: Nominal identity types, explicit mapping records, and a dimensional compatibility vector

Give each identity kind a distinct generated type, owner, scope, encoding, reuse rule, and allowed persistence surfaces. Cross-kind correlation occurs only through typed, purpose-bound mapping records. Exact receipts publish separate compatibility dimensions, and each operation evaluates the subset it needs through one versioned rules engine.

This makes invalid substitution difficult and produces explainable compatibility outcomes. It requires generated codecs, mapping ownership, more fields in schemas and diagnostics, explicit migrations, and discipline around dynamic tooling.

### Option B: One globally unique identity for every object and artifact

Assign a UUID-like identity once and carry it through runtime, network, saves, documents, and transfers.

This makes correlation superficially easy but falsely implies continuity and authority. It prevents fresh identities on restore or reconnect, makes copies and previews ambiguous, and turns a leaked runtime detail into a permanent data contract.

### Option C: Let each subsystem use strings or GUIDs by convention

Keep identity representations locally simple and document which values may be compared.

This minimizes initial infrastructure, but conventions cannot stop cross-kind equality, scope loss during serialization, name-based fallback, or inconsistent compatibility decisions. Failures would emerge at runtime and often after state publication.

## Decision

Robusta will use Option A: nominal identity types, explicit mapping records, and a dimensional compatibility vector.

The mechanism contract is:

1. Every foundational identity is a nominal generated value type. Its declaration records a stable identity-kind schema, scope kind, issuer kind, encoding version, maximum encoded size, generation or reuse rule, serialization permissions, redaction class, and diagnostic formatter. Public SDK code does not receive a general comparable `Identity`, raw GUID, or string alias.
2. Equality and ordering are defined only between the same identity type under the same declared scope. Comparing an entity to a network object, a runtime map to a map definition, a checkpoint to a world, or a package to a receipt is a compile-time error in typed code and a stable kind-mismatch result at dynamic boundaries.
3. A dynamic inspector or protocol envelope may carry a bounded `IdentityDescriptor` containing identity-kind schema, encoded scope, encoded value, and encoding version. It has no cross-kind equality or resolution behavior. Converting it to a nominal type revalidates kind, scope, encoding, provenance, and caller authority.
4. Identity is separate from location, display, selection, provenance, authentication, and authorization. A path, URL, channel, package alias, repository slot, display name, trace identifier, socket, credential, role, or permission is never silently promoted to an identity. Possessing or guessing an identity never grants access to its target.
5. Runtime identities include an incarnation component or a generational table so storage reuse cannot create aliasing. Exhausted generations retire a slot or incarnation rather than wrapping. Parsers reject missing scope, unknown kind, invalid generation, noncanonical encoding, and oversized values before lookup.
6. Stable artifact and definition identities use canonical, domain-separated inputs. Their digest algorithm and canonical encoding are versioned in the containing schema or receipt. A cryptographic collision or duplicate declaration is an explicit integrity failure, never a reason to choose whichever record loaded first.
7. Games may declare package-qualified durable identity domains and schema-defined document or gameplay identities through the published SDK. They cannot redefine platform identity kinds, weaken their scope, claim another issuer, or gain a universal registry namespace by choosing the same local text.
8. Cross-kind correlation uses an immutable typed mapping record with source identity, target identity, mapping purpose, owning scope or transaction, authority, source and target compatibility context, provenance, creation boundary, retention rule, and terminal disposition. A mapping supports lookup for that purpose; it does not establish equality, inheritance, ownership, or permission.
9. Mapping tables belong to the narrowest lifetime that can validate both endpoints. They close or tombstone their entries before either endpoint can be reused. A broader service may retain an immutable diagnostic record, but it cannot keep a runtime endpoint live or resolve a new endpoint through an expired mapping.
10. The initial identity kinds and their non-substitution rules are:

| Identity kind | Owner and scope | Stability and permitted persistence | Explicit mappings | Never substitutes for |
|---|---|---|---|---|
| `HostInstanceId` | One game-host process incarnation | Fresh on process start; diagnostic and supervision journals only | Exact receipt loaded; hosted world and session instances | Publisher, game, receipt, credential, world, or durable identity |
| `WorldInstanceId` | One live `WorldScope` under a host | Fresh for every construction or restore; never a checkpoint identity | Host, pinned catalog generation, optional checkpoint source, maps, committed operations | World lineage, checkpoint, runtime map, or transfer identity |
| `SessionId` | One authenticated `SessionScope` under an authority | Fresh according to session authentication policy; may survive world replacement but not be assumed across reauthentication | Host, connection, session-world attachments, account or durable subject only through an authorized service | Credential, account, avatar entity, attachment, or network identity |
| `SessionWorldAttachmentId` | One live join between one session and one world | Fresh on attach or reconnect; retained only for bounded protocol diagnostics | Session, world, avatar association, network-entity table | Session, world, avatar, or permission |
| `EntityRef` | One world incarnation and its slot-generation table | Runtime only; may appear in bounded world-local traces but never as save, document, network, or durable identity | Network entity, checkpoint-local record during restore, durable concept when declared, catalog birth provenance | Any mapped endpoint or entity in another world |
| `RuntimeMapId` | One live world and its map-generation table | Runtime only; fresh for each instantiation even from the same definition | World, map-definition identity and fingerprint, root frame, optional checkpoint record | Map source, document, definition, checkpoint, or another instance |
| `SpatialFrameId` | One live world and frame-generation table, normally beneath a runtime map | Runtime only; stale-safe and invalid after frame or map ending | Runtime map, owning structure or entity where applicable, parent frame | Raw coordinate, grid-cell address, map, entity, or durable identity |
| `NetworkEntityId` | One `SessionWorldAttachmentScope` and protocol epoch | Wire-visible only for that attachment; tombstoned through the reorder window and renewed on reconnect | One currently admitted `EntityRef` under the attachment mapping | `EntityRef`, session, durable identity, or network identity in another attachment |
| `CheckpointId` | One immutable checkpoint artifact | Durable in repository metadata, parent/source links, migration records, and receipts where selected | Optional lineage, source receipt and schemas, checkpoint-local records, restore result | Runtime world, repository slot, display name, or lineage identity |
| `WorldLineageId` | An explicitly declared checkpoint continuity domain | Durable only when a save profile promises lineage | Checkpoints in that lineage and authorized durable concepts | Live world, checkpoint, host, issuer, or authority |
| `CheckpointRecordId` | One checkpoint artifact | Durable only inside that checkpoint and its explicit migration representation | Included records and fresh restore-time entities | Durable identity, runtime slot, catalog definition, or record in another checkpoint |
| `ReplayArtifactId` | One immutable authoritative replay artifact | Canonical durable artifact identity under the replay profile; retained only by authorized replay storage and diagnostics | Source checkpoint or construction, exact receipts and descriptors, replay records, verification results | Runtime world, checkpoint, receipt, file path, display name, or authority |
| `ReplayRecordId` | One replay artifact | Durable only inside that artifact and its admitted migration representation | Original declared identities and separately allocated re-execution identities through purpose-bound comparison mappings | Runtime identity, checkpoint record, durable concept, or record in another replay |
| `DurableId<TDomain>` | One declared issuer and package-qualified domain | Stable only for the continuity contract of that domain; allowed in checkpoints, transfer records, or authorized service data | Checkpoint records, reconstructed entities, external proxies, explicit migrations | Credential, session, entity, catalog definition, or proof of ownership |
| `CatalogDefinitionId<TKind>` | One package-qualified definition namespace | Stable logical definition identity in catalogs, documents, saves, and schemas; exact resolved meaning also requires generation or fingerprint | Package, source provenance, catalog generation, runtime birth or map instantiation | CLR type, file path, short name, live object, or exact content fingerprint |
| `CatalogGenerationId` | One immutable resolved catalog projection | Exact artifact identity; retained while leased or referenced by a transaction, world, checkpoint, or diagnostic policy | Receipt, package lock, definitions and schema fingerprints, pinned worlds | Logical definition, mutable catalog, package, or release receipt |
| `DocumentId` and `DocumentRevisionId` | One creator workspace or declared remote document authority | Document identity follows its authored identity policy; every accepted revision is immutable and ordered within that authority | Source provenance, exact workspace lock, compiled definition and fingerprint, editor session | Runtime map, world, checkpoint, catalog generation, or gameplay state |
| `PackageId` and `PackageRevisionId` | Publisher-qualified package namespace and one exact package artifact | Logical package identity is stable; exact revision includes version and canonical digest and is receipt material | Dependencies, definitions, schemas, side projections, receipt membership | Definition, receipt, publisher trust, filesystem directory, or loaded assembly |
| `ReleaseReceiptId` | One canonical signed exact release receipt | Immutable installation, selection, launch, update, rollback, and diagnostic identity | Exact runtime, packages, projections, schemas, catalog generations, extensions, platform requirements | Game name, update channel, installed path, host, or trust decision by itself |

11. The table is a minimum platform set, not a license to compare every row through one interface. Additional identities such as editor session, structural operation, transfer activation, or external-service transaction require their own accepted contract and must follow the same type, scope, mapping, compatibility, and non-authority rules. ADR 0041 adds the two replay kinds to the minimum set, while `REPLAY-AUTHORITATIVE` still owns their exact declarations, codecs, mappings, and retention.
12. A world pins exactly one admitted catalog generation at a committed boundary. A runtime entity may retain package-qualified birth-definition provenance and a generation; those fields explain construction but do not make the entity equal to its definition. Runtime catalog references that intentionally follow an adopted generation are separately declared per ADR 0037.
13. One map definition may instantiate several runtime maps, and one document revision may compile into a definition fingerprint under an exact workspace lock. Compilation and instantiation each create provenance mappings. Neither mapping permits edits to mutate a catalog or a running map in place.
14. A session controls or observes an entity only through an explicit session-world attachment and avatar-association record. A network identity resolves through that attachment's table to one live entity. Detach, reconnect, world replacement, interest loss, and entity death have distinct terminal mappings and cannot reuse an old network resolution.
15. Checkpoint capture assigns checkpoint-local record identities and writes declared durable and catalog references. Restore allocates a fresh `WorldInstanceId`, runtime maps, frames, and entities, then builds temporary mappings from checkpoint records and admitted durable identities. It publishes none of those runtime mappings until the complete graph and every missing-reference policy validate.
16. Cross-world transfer, when later supported, maps one immutable source export revision and declared durable identities to newly constructed target-world identities. It never carries `EntityRef`, `RuntimeMapId`, `SpatialFrameId`, `NetworkEntityId`, or pending operation identities into the target as though they remained live.
17. Rename, move, split, merge, clone, or issuer change uses an explicit typed migration record. It identifies old and new kinds and scopes, cardinality, collision policy, retained tombstones, provenance, and compatibility effect. The runtime never guesses from case folding, path similarity, nearest version, display text, registration order, or an ambiguous short name.
18. Each exact release receipt publishes a compatibility vector with separate identities for at least runtime contract, SDK/API surface, generated component and message layouts, package lock, catalog format and semantic generation, network schema, checkpoint envelope and save profile, creator-document schema and compiler, advanced-extension interfaces, native dependencies, and operating-system/architecture requirements.
19. Compatibility is an operation-specific relation, not equality and not one Boolean. The common result vocabulary is `Exact`, `Compatible`, `MigrationRequired`, `AdapterRequired`, `RestartRequired`, `ReconnectRequired`, `ReadOnlyInspection`, or `Incompatible`, with stable reasons and every compared dimension. An operation defines which results it may proceed under; it cannot silently downgrade an incompatible required dimension.
20. One versioned compatibility rules engine evaluates canonical descriptors for install, launch, multiplayer handshake, catalog adoption, checkpoint inspection and restore, map compilation and preview, package loading, extension admission, and replay re-execution. Each operation selects only its relevant dimensions but receives the same answer for the same operation, inputs, and policy on every supported machine. Replay publication additionally waits for the separately reviewed ADR 0047 replay-reexecution profile required by ADR 0041.
21. Compatibility policy is itself identified and included in the exact tool or runtime receipt. Range expressions are resolved to exact identities before launch or publication. A mutable channel name, installed directory, process assembly list, or current clock cannot change a completed compatibility result.
22. Compatibility and mapping checks fail closed before their target becomes visible. Unknown required identity kind, unsupported encoding, missing issuer, expired mapping, incompatible schema, ambiguous migration, collision, or corrupt descriptor produces a stable diagnostic and no guessed target. Optional absence is allowed only where its schema declares that outcome.
23. Wire, checkpoint, catalog, document, receipt, and diagnostic codecs use generated bounded parsers. They do not reflectively instantiate arbitrary runtime types from identity-kind data. Untrusted inputs have length, nesting, count, allocation, and lookup-rate limits before registry or mapping access.
24. Diagnostic correlation carries separate structured fields for every applicable identity rather than concatenating them into one composite string. Traces may link records across scopes, but a trace or correlation identifier is not an authority, transaction, lifecycle, or compatibility identity.
25. Logging and inspection apply the identity declaration's redaction class. Session, account-correlated durable, private document, and external-service identifiers may require pseudonymization or omission. Redaction preserves stable local correlation where authorized without exposing credentials, private draft names, or personally identifying values.
26. Retention is explicit. Runtime mapping tables end with their owner; network tombstones cover the declared reorder window; catalog generations remain while leased; checkpoint and migration records follow repository policy; durable issuer tombstones follow their domain contract; and diagnostic correlation has bounded retention. Garbage collection cannot use "not currently resolved" as proof that a durable artifact is unreferenced.

## What we deliberately will not do

- Create a universal object ID shared by runtime, network, saves, documents, and transfers.
- Expose raw GUIDs or strings as interchangeable SDK identities.
- Treat a display name, file path, CLR type, repository slot, channel, or package directory as canonical identity.
- Let a mapping imply equality, ownership, trust, authentication, or authorization.
- Preserve runtime entity, map, frame, network, host, session, or world identity across reconstruction merely for convenient correlation.
- Infer compatibility from one semantic version, package version, receipt equality, or successful deserialization.
- Guess a missing reference or migration target.
- Build a global mutable identity directory that keeps every runtime and artifact reachable.

## Consequences

### Compatibility and migration

Existing raw integer, GUID, string, type-name, and path identifiers require conversion to nominal types with declared scopes. Analyzers and code fixes can handle unambiguous local substitutions; mixed-use fields and name-based lookup require manual classification. Persisted aliases become explicit migration records, not indefinite fallback lookup.

Identity encodings, mapping records, and compatibility descriptors are independently versioned. A decoder may understand an older encoding without declaring the referenced artifact compatible for the requested operation. Changing an identity's meaning requires a new kind or explicit migration rather than reinterpreting old bytes.

### Security and failure handling

Strong types reduce accidental confused-deputy paths but are not an access-control mechanism. Every resolution rechecks the caller, mapping purpose, endpoint lifetime, and operation authority. Session and durable identifiers remain non-secret identifiers; credentials and authorization stay separate.

Malformed, forged, stale, cross-scope, or colliding identities fail before target access. A corrupt live mapping table faults its owning attachment, world, or host according to the affected integrity boundary. It never falls back to a global search. Package and receipt signatures establish artifact integrity and publisher claims under their trust policy; identifier shape alone establishes neither.

### Operations

Structured identities make logs and support reports more useful but increase cardinality, storage, and privacy obligations. Operators need bounded mapping metrics, tombstone counts, unresolved-reference reports, generation lease reports, compatibility explanations, and redaction-aware search. Diagnostic tooling must display both logical and exact identities without collapsing them.

## Bounded first-release scope

The 1.0 scope includes generated types and codecs for every identity in the minimum table; stale-safe entity, map, frame, world, session, attachment, and network identities; package-qualified catalog and package identities; exact catalog-generation and release-receipt identities; checkpoint, checkpoint-local, lineage, and declared durable identities; document and revision identities; purpose-bound mapping tables; and compatibility evaluation for install, launch, handshake, catalog admission, checkpoint restore, and map compilation or preview.

The 1.0 scope does not include a universal cross-service durable registry, general durable cross-world graph transfer, semantic document branch merge, automatic reverse migration, global object search, or cross-release live-object rebasing. Accepted ADR 0041 separately adds bounded replay artifact and replay-local identities plus replay-reexecution compatibility within a validated domain. Their exact ADR 0044 declarations, purpose-bound comparison mappings, durable codecs, and ADR 0047 profile remain governed by `REPLAY-AUTHORITATIVE`; they do not create a universal registry, reuse ephemeral runtime identities, or weaken this ADR's operation-specific compatibility rules. Games may declare bounded durable domains, but operating a federated issuer remains a separate capability.

## How we will prove the decision works

- Architecture and compiler fixtures reject equality, assignment, serialization, lookup, and API substitution between every incompatible pair in the minimum identity table.
- Codec fuzzing covers unknown kinds, missing scope, noncanonical bytes, generation wrap, collisions, oversized inputs, forged issuers, corrupt mappings, and ambiguous migrations and produces bounded stable failures.
- `StaleRuntimeIdentityNeverAliasesReuse` repeatedly reuses entity, map, frame, world, attachment, and network storage without resolving an old value to a replacement.
- One world instantiates the same map definition twice, two worlds use equal local coordinates, and diagnostics preserve distinct document, definition, generation, runtime-map, frame, and world identities.
- Reconnect and world replacement create fresh attachment and network identities while an authenticated session survives only where its own policy allows; late packets and mappings cannot reach the replacement entity.
- A cyclic checkpoint restores through checkpoint-local records into fresh runtime identities, preserves declared durable continuity, rejects a missing required target, and does not confuse the source checkpoint with the new world.
- Clone, rename, package move, definition split, and durable collision fixtures proceed only through explicit typed migrations and never by name or path guessing.
- Two packages with the same local definition name, two publishers with the same package local name, and two documents with the same display name never cross-resolve.
- Compatibility matrix tests independently vary runtime, SDK, component layout, package, catalog, network, checkpoint, document, extension, native, and platform dimensions and produce the expected operation-specific outcome and reason.
- Clean Windows and Ubuntu runs generate byte-identical canonical identity descriptors and compatibility results from identical exact inputs.
- Install, handshake, restore, adoption, and preview reject incompatible required dimensions before process, session, world, catalog generation, or preview publication.
- Authorization tests prove that possession of session, durable, document, entity, package, or receipt identifiers grants no read, mutation, installation, or administration authority.
- Redaction tests preserve authorized correlation while excluding credentials, private draft material, and configured personally identifying durable values from logs and client projections.

## Implementation notes

The internal ownership kernel now uses distinct nominal types for host, world, session, session-world attachment, and catalog-generation identities. These types are runtime-only and intentionally lack public, durable, or wire codecs. The common identity declaration schema, generators, bounded codecs, mapping-record contract, compatibility descriptor, compatibility rules engine, migration registry, redaction policy, and full conformance matrix remain unimplemented.

## Dependencies and interaction with accepted product decisions

This decision derives its identity lifetimes from accepted ADRs 0017, 0019, 0021-0023, 0028, and 0030-0041. It does not select an entity-store layout, digest algorithm, wire encoding, checkpoint or replay format, document format, or database. Those mechanisms may vary while preserving the types, scopes, mappings, and compatibility outcomes.

Accepted product [ADR 0039](../product/0039-inspect-running-worlds-through-authorized-snapshots.md) displays only authorized redacted identity descriptors and mapping provenance without gaining resolution authority. [ADR 0040](../product/0040-test-isolated-worlds-through-the-supported-runtime.md) creates scoped identity fixtures through ordinary allocation and exposes only permitted projections. [ADR 0041](../product/0041-record-versioned-authoritative-replays-with-declared-determinism.md) introduces distinct replay artifact and replay-local record identities, fresh re-execution runtime identities, purpose-bound comparison mappings, and a replay-reexecution compatibility operation rather than reusing checkpoint, receipt, or original ephemeral identities.

Accepted ADR 0042 defines ephemeral operation and message-schema identities. Those identities use this ADR's nominal declaration and compatibility rules, while their ordering and commit meaning remain owned by ADR 0042. Neither decision permits a generic identity escape hatch.

## Follow-up decisions

- Identity declaration, canonical descriptor, mapping-record, and compatibility-vector schemas.
- Incarnation, generation, random-allocation, digest, collision, and tombstone algorithms by identity kind.
- Compatibility policy language and stable diagnostic taxonomy.
- Durable issuer repository, federation, privacy, retention, and recovery policy.
- Catalog rename, split, merge, and alias migration mechanics.
- Diagnostic redaction, high-cardinality storage, and operator search budgets.
- Replay artifact and record declarations, purpose-bound comparison mappings, durable codecs, and the reviewed replay-reexecution profile under accepted ADR 0041.

## References

- [ADR 0019](0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0021](0021-compile-content-into-a-canonical-provenance-catalog.md)
- [ADR 0022](0022-install-exact-receipts-into-immutable-content-addressed-layouts.md)
- [ADR 0023](0023-generate-versioned-authoritative-replication-schemas.md)
- [ADR 0028](0028-model-sessions-and-worlds-as-sibling-host-scopes.md)
- [ADR 0030](../product/0030-define-runtime-maps-and-frame-qualified-coordinates.md)
- [ADR 0032](../product/0032-reconstruct-explicitly-across-world-transfers.md)
- [ADR 0035](../product/0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [ADR 0036](../product/0036-use-explicit-durable-identities-and-reference-policies.md)
- [ADR 0037](../product/0037-keep-live-state-stable-unless-explicitly-migrated.md)
- [ADR 0038](../product/0038-edit-map-sources-and-preview-in-isolated-worlds.md)
- [ADR 0042](0042-use-typed-message-kinds-and-transactional-structural-commits.md)
