# ADR 0028: Model sessions and worlds as sibling host scopes

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime and multiplayer workstreams
- **Supersedes:** None
- **Amends:** ADR 0017
- **Product decisions served:** 0002, 0004, 0006, 0011, 0012, 0015, 0016
- **Related decisions:** 0017, 0019, 0023, 0024, 0026

## The question

How should runtime scopes represent independently owned worlds, player sessions, immutable catalog generations, and the temporary replication relationship between one session and one world?

## The promise preserved

A host may run several worlds and retain sessions across world replacement without making either lifetime depend on the other. Worlds may safely pin different immutable catalog generations, and every session-to-world replication relationship has an explicit owner and cleanup boundary.

## Why this matters

ADR 0017 currently lists process, catalog generation, host, session, and world scopes in one hierarchical order. That shape implies a world belongs to one session and that a host belongs to one catalog generation.

The product model requires different relationships:

- one host owns multiple worlds and sessions;
- many sessions may participate in one world;
- a session may survive destruction or replacement of its current world;
- a host may temporarily run worlds using different immutable catalog generations; and
- per-session interest, baselines, acknowledgements, network identities, and avatar attachment belong to neither the session nor the world alone.

A simple parent-child scope chain cannot represent those lifetimes correctly.

## Options considered

### Option A: Sibling world and session scopes with an explicit attachment scope

A host owns independent `WorldScope` and `SessionScope` instances. An explicit `SessionWorldAttachmentScope` joins one live session to one live world and owns their per-session replication and avatar-association state. Worlds hold immutable catalog-generation references rather than becoming children of catalog scopes.

### Option B: Keep session-to-world nesting

This makes attachment convenient but prevents many sessions from sharing a world and makes world lifetime incorrectly depend on one session.

### Option C: Make sessions children of worlds

This represents participation but prevents a connection and authenticated session from surviving world replacement or transfer.

### Option D: Use a general dependency graph and ambient scope resolver

This can represent every relationship, but makes lifetime capture, disposal, diagnostics, and public capability access difficult to audit. It risks recreating a service locator.

## Decision

Robusta will use Option A. ADR 0017's single ordered scope hierarchy is amended as follows:

1. A game-host process loads one exact executable game installation and creates one `HostScope`.
2. The host owns independent collections of `WorldScope`, `SessionScope`, and `SessionWorldAttachmentScope` instances.
3. `WorldScope` and `SessionScope` are siblings. Neither is an ancestor, child, or service-resolution source for the other.
4. A `WorldScope` owns authoritative simulation state, entities, systems, time, random state, structural work, maps, physics, and world-level replication sources.
5. A `SessionScope` owns one authenticated connection/session identity, session configuration, administration state, and other state intended to survive world replacement.
6. A `SessionWorldAttachmentScope` joins exactly one live session and one live world through typed endpoint capabilities. It owns participation or observation role, current avatar association, per-session interest and visibility, network-entity mappings and tombstones, snapshot baselines and acknowledgements, input sequence and admission, and attachment-specific diagnostics and budgets.
7. A session may have zero or more observation attachments where later product contracts allow them, but may control an avatar in only one world at a time unless that product rule is explicitly changed.
8. Ending either endpoint closes the attachment first. Ending a world leaves its sessions alive; ending a session leaves its worlds alive.
9. Reconnect or reattachment creates a fresh attachment identity and fresh attachment-local network state. Old network identities and baselines cannot resolve through the new attachment.
10. Catalog generations are immutable installation-owned artifacts, not ancestors of hosts or worlds. A world acquires a typed lease or reference to one exact generation when created.
11. One host may supervise worlds pinned to different catalog generations. A generation remains available while referenced by a world, active transaction, diagnostic record, or another declared retention owner.
12. Transactional catalog adoption replaces a world's generation reference according to its separate reload contract. It does not mutate the referenced generation or re-parent the world.
13. Scope validation rejects captures from host into mutable world, session, or attachment state; from world into session or attachment state; and from session into world or attachment state.
14. Attachments may use only their declared endpoint capabilities. They do not receive an arbitrary resolver for either endpoint scope.
15. Host shutdown closes admission, ends attachments, then disposes sessions and worlds through bounded, fully reported cleanup before releasing catalog references and host infrastructure.

## What we deliberately will not do

- Make a world's lifetime depend on one player session.
- Make a session's lifetime depend on its current avatar or world.
- Treat an immutable catalog generation as the owner of mutable host or world state.
- Store per-session interest, baselines, or network identities as unexplained mutable state directly in the host or world.
- Allow an attachment to resolve arbitrary services from both endpoint scopes.
- Preserve attachment-local network identities across detach, reconnect, or world replacement.
- Use a general ambient scope graph as the ordinary game API.

## Consequences

### Compatibility and migration

ADR 0017's scope-order clause and any tests assuming session-owned worlds are replaced by the sibling-and-attachment model.

Runtime and networking code must separate authoritative world replication sources, durable connection/session state, and per-session-per-world replication state. Prototype or legacy code that stores avatar, interest, network mapping, and connection state in one player object requires migration to the three-owner model.

Public identities for host, world, session, attachment, catalog generation, entity, and network entity remain distinct and opaque. No persisted or wire identity may rely on an in-memory scope object.

### Security

Typed endpoint capabilities prevent an attachment from becoming an unrestricted bridge between world and session services. This is a lifetime and authority boundary, not a sandbox for trusted game code.

### Operations

Logs, metrics, traces, health reports, and cleanup records carry host, world, session, attachment, and catalog-generation identities where applicable.

Operational metrics include active and orphaned attachment counts, attachment age, interest size, baseline age, input backlog, detach reason, catalog lease counts, and teardown duration.

Leak detection reports an attachment surviving either endpoint, a catalog generation retained without a declared owner, or a longer-lived service capturing shorter-lived mutable state.

## How we will prove the decision works

- One host runs two worlds and two sessions in a many-to-many participation fixture without either scope type owning the other.
- Destroying one world closes only its attachments; both sessions and the other world remain healthy.
- Ending one session closes only its attachments; participating worlds and other sessions remain healthy.
- A session detaches from an avatar in one world and attaches to a replacement avatar in another without retaining old network identities, baselines, or interest state.
- Reconnect creates a fresh attachment and rejects all late work from the prior attachment.
- Two worlds in one host pin different immutable catalog generations without mutable catalog sharing.
- A catalog generation cannot be collected while leased and is released after its final declared owner ends.
- Architecture tests reject every forbidden lifetime capture and any public arbitrary-scope resolver.
- Fault injection during attachment creation and teardown leaves neither endpoint corrupted and reports every acquired resource.
- Host shutdown disposes attachments before endpoint scopes and leaves no owned process, scope, lease, or replication state behind.

## Implementation notes

No host, world, session, attachment, catalog-lease, or replication-scope implementation exists. This decision corrects the ownership model before those public and runtime contracts are frozen.

## Follow-up decisions

- Session membership, observer roles, avatar ownership, and cross-world transfer.
- Attachment and network identity schemas.
- Interest, visibility, baseline, reconnect, and input-admission behavior.
- Catalog-generation adoption and existing-entity treatment.
- Scope implementation library, typed endpoint capabilities, and generated activation metadata.
- Attachment fault escalation, cleanup budgets, and operational limits.

## References

- [ADR 0011](../product/0011-define-world-as-isolated-simulation.md)
- [ADR 0012](../product/0012-separate-game-host-and-world-state.md)
- [ADR 0017](0017-enforce-explicit-runtime-ownership-scopes.md)
- [ADR 0023](0023-generate-versioned-authoritative-replication-schemas.md)
