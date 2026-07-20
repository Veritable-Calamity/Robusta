# ADR 0023: Generate versioned authoritative replication schemas

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Multiplayer workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0006, 0008, 0012, 0015, 0016
- **Related decisions:** 0018, 0019, 0020, 0021, 0022

## The question

How will synchronization declarations become compatible wire schemas for authority, input, snapshots, entity lifecycle, interest, prediction, correction, and reconnect?

## The promise preserved

The server remains final authority, ordinary game code declares synchronization intent instead of packets, clients stay responsive through bounded prediction and smoothing, and incompatible peers fail before gameplay.

## Why this matters

Network identity and ordering cannot reuse memory handles or callback timing. Handwritten routine codecs would multiply compatibility and security mistakes across every game.

## Options considered

### Option A: Generated schemas with input streams and authoritative snapshots

Compile declarations into canonical schemas and codecs. Clients send sequenced inputs; the server publishes interest-filtered authoritative lifecycle and state snapshots with acknowledgements and correction metadata.

### Option B: Reflection-driven object replication

This reduces code generation but makes schema evolution, side auditing, performance, and NativeAOT-style deployment harder to reason about.

### Option C: Custom packets for each feature

This offers control but makes ordinary multiplayer authoring low-level and inconsistent with the product promise.

## Decision

Robusta will use Option A:

1. SDK declarations classify fields and actions as server-only, shared authoritative, locally predicted, remotely interpolated, or client-only cosmetic. Generation fails invalid combinations.
2. A canonical network schema assigns stable type, field, event, action, and entity-archetype identities from package-qualified declarations. Its fingerprint enters the release receipt and handshake.
3. Network entity identities are server-issued session identities with generation or tombstone protection. They map to world-local `EntityRef` values but are never the same value.
4. Clients send authenticated, monotonically sequenced input commands naming the last acknowledged authoritative step. The server validates session, authority, rate, payload, target lifecycle, and admissible step before simulation admission.
5. The server emits interest-filtered full or delta snapshots containing step numbers, baselines, complete committed births and structural changes, state updates, removals, and ownership changes.
6. A client applies snapshots in authoritative order, retains bounded prediction history only for declared predictable state, replays admitted local inputs after correction, and discards history on removal or incompatible structural change.
7. Interest loss and entity death are distinct wire outcomes. Tombstones reject late data and remain long enough to cover the protocol's bounded reordering window.
8. Reconnect performs a fresh compatibility handshake and authoritative resynchronization. It does not assume old world-local or network identities remain valid.
9. The transport is replaceable behind bounded reliable, ordered, and datagram capabilities. Game code does not select lanes or access sockets.
10. Offline play starts the same authority and schema path locally rather than using divergent game rules.

## What we deliberately will not do

- Infer compatibility from CLR type names, source compilation, or receipt version alone.
- Replicate preparing or partially changed entities.
- Let prediction create shared authoritative outcomes.
- Expose transport fragmentation, lanes, or resend policy as ordinary component code.

## Consequences

### Compatibility and migration

Schema changes are classified as compatible, migration/restart-required, or incompatible. Released schemas are retained for diagnostics; automatic wire translation requires an explicit adapter and conformance evidence.

### Security

The server validates every client-originated action and bounds message size, frequency, history, decompression, entity allocation, and interest expansion. Generated codecs are fuzzed and never instantiate arbitrary runtime types from the wire.

### Operations

Diagnostics expose receipt and schema mismatch, input rejection, snapshot and baseline age, correction counts, interest size, bandwidth, loss, reorder, and reconnect outcomes without leaking secrets.

## How we will prove the decision works

- The two-client fault matrix covers latency, loss, duplication, corruption, and reordering.
- `AuthoritativeLifecycleAndLateWorkSafety` verifies atomic birth/change/removal and tombstone behavior.
- Predicted movement, remote interpolation, authoritative interaction, interest entry/exit, and reconnect scenarios pass in an external game.
- Schema permutation tests prove stable fingerprints; incompatible schemas fail before world admission.
- Codec fuzzing and resource-exhaustion tests enforce all published bounds.

## Implementation notes

No network schema generator, transport, replication store, prediction history, or reconnect protocol is implemented.

## Follow-up decisions

- Initial transport and cryptographic session protocol.
- Interest model after the space product gate.
- Prediction budgets and supported predictable operations.

## References

- [ADR 0006](../product/0006-server-authority-and-declarative-sync.md)
- [ADR 0015](../product/0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0016](../product/0016-separate-simulation-host-and-presentation-time.md)
