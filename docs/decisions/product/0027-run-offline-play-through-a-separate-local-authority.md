# ADR 0027: Run offline play through a separate local authority

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0004, 0006-0008, 0014, 0017, 0022, 0023

## The question

When a player chooses offline play, where does the authoritative simulation run, how is it started, and how do client/server payload separation and security promises remain true?

## The promise

Offline play remains one-click and responsive while exercising the same authority, synchronization, compatibility, and lifecycle rules used online. Supporting offline play must not require merging server-only material into the client runtime or loading game code into the launcher.

## Why this matters

ADR 0006 requires offline play to use the same game rules through a local authority, but does not select its process topology. That choice affects behavioral fidelity, package contents, startup, diagnostics, crash containment, save ownership, and network exposure.

Embedding authority inside the client is operationally simple, but couples presentation and authority failures, places server code in the client process, and encourages shortcuts around the real handshake and replication path. A separate local authority costs another process and startup transaction, but more faithfully exercises the dedicated-server product and preserves side-specific runtime boundaries.

## Options considered

### Option A: Launcher-supervised loopback authority

The launcher starts a separate authority process from the exact server-side payload, waits for readiness, and starts a client from the client-side payload. The client connects over an authenticated loopback session using the ordinary generated schema and authoritative protocol.

The launcher provides supervision and rendezvous only. It does not load game assemblies, simulate gameplay, or proxy authoritative state.

### Option B: Authority embedded in the client process

The client hosts an authoritative world alongside presentation and connects through an in-memory transport.

This may start faster, but requires server capabilities in the client process, weakens fault and payload separation, and creates an offline-only topology that may conceal multiplayer defects.

### Option C: Let each game choose its own offline topology

Games may embed authority, launch a server, or implement separate offline rules.

This maximizes flexibility but fragments packaging, creator tooling, diagnostics, and conformance. It also weakens the promise that offline and online play exercise the same game rules.

## Decision

Robusta will use Option A:

1. Selecting offline play starts a separate local authority and client through one launcher-supervised transaction. Players do not configure ports, start a server console, or connect manually.
2. The authority uses the ordinary server host, authoritative world model, input admission, generated network schema, snapshots, and compatibility checks. Local play does not bypass authority because both processes run on one machine.
3. The client connects directly to the authority through a loopback endpoint. The launcher communicates readiness and connection information but does not enter the gameplay data path.
4. The authority binds only to a local endpoint by default. Discovery, public listening, remote administration, and unrelated client admission are disabled unless the user explicitly starts an ordinary dedicated-server configuration.
5. The local session uses launch-scoped authentication material that is not exposed in logs, diagnostics, or ordinary command-line inspection. A later technical decision selects the protected rendezvous mechanism.
6. An offline-capable installation resolves client, shared, and server projections from one exact release receipt. The client and authority receive separate runtime views; server-only assemblies and resources are not merged into the client projection.
7. Content-addressed storage may deduplicate identical files without weakening logical side separation. Artifact inspection must still distinguish which payload each process may load.
8. A game declaring offline support distributes the server-side material required by its local authority. Publishers must not place secrets in that material: an offline installation is controlled by the player and does not provide confidentiality from the local machine owner.
9. The authority owns authoritative saved-world mutation. Client settings and presentation data remain separate. Durable save, backup, migration, and crash-recovery behavior follow their explicit contracts.
10. Closing play, failed startup, authority failure, client failure, update, or rollback produces a bounded and observable supervision outcome. The launcher cleans up only the process tree it created.
11. Local authority does not provide anti-cheat protection against the machine owner. Its purpose is behavioral parity, authority discipline, packaging separation, and reuse of the supported multiplayer path.
12. A later optimized local transport may replace operating-system loopback only if conformance evidence proves identical ordering, validation, compatibility, correction, and failure behavior.

## What we deliberately will not do

- Load server or game assemblies into the launcher.
- Merge server-only material into the client payload or process.
- Maintain separate offline gameplay rules.
- Skip schema negotiation, input validation, authority checks, or correction because the connection is local.
- Expose the offline authority to external interfaces by default.
- Require players to operate a dedicated server manually for ordinary offline play.
- Describe locally installed server material as secret from the player.
- Treat process separation alone as containment for hostile executable game code.

## Consequences

### Benefits

- Offline play exercises the supported authority and networking contracts.
- Client and server payload boundaries remain inspectable.
- Authority crashes and presentation crashes are isolated.
- Dedicated-server behavior receives routine use during local development and play.
- One launcher transaction can provide clear readiness, failure, and cleanup outcomes.

### Costs and limitations

- Offline play starts at least two game processes and requires readiness coordination.
- Startup time and baseline memory use are higher than an embedded authority.
- Offline-capable releases must make their server projection available to players.
- Loopback authentication, process cleanup, save ownership, and failure reporting require cross-platform implementation.
- Local authority cannot protect publisher secrets or prevent modification by the machine owner.

## How we will prove the decision works

- On supported Windows and Ubuntu systems, one action starts an exact verified authority and client and reaches playable offline state without manual server configuration.
- Process and module inspection shows separate launcher, authority, and client processes; the launcher loads no game assemblies.
- Artifact inspection shows the client view contains no server-only assemblies or resources, while the authority loads the exact server projection from the same receipt.
- Offline play completes the ordinary compatibility handshake and uses the same generated input, snapshot, correction, lifecycle-removal, and reconnect contracts as an online loopback test.
- Invalid or unauthorized client actions are rejected by the local authority exactly as they are by a dedicated server.
- Network inspection confirms that the default offline authority accepts only its authenticated local session and exposes no public listener or discovery endpoint.
- Launch credentials do not appear in command-line listings, logs, crash reports, or structured diagnostics.
- Injected authority-startup failure, authority crash, client crash, and launcher interruption produce clear diagnostics and leave no orphaned process tree.
- Authoritative save writes, interrupted writes, update, migration, and rollback follow the exact receipt and writable-data contracts.
- Startup time and memory use are measured against the published offline-play budget.

## Implementation notes

No offline authority implementation is claimed. The client and server entry points remain scaffolds. This decision does not select loopback transport, rendezvous APIs, launch-envelope representation, readiness protocol, authentication primitive, or save format.

## Follow-up decisions

- Offline install profile and side-projection receipt semantics.
- Protected authority rendezvous and local-session authentication.
- Readiness, liveness, shutdown, and process-tree supervision.
- Loopback transport and any conformance requirement for an optimized local transport.
- Authoritative save ownership, crash recovery, and migration.
- Offline startup-time and memory budgets.

## References

- [ADR 0004](0004-distribute-games-as-isolated-application-packages.md)
- [ADR 0006](0006-server-authority-and-declarative-sync.md)
- [ADR 0014](0014-define-first-release-boundary-and-delivery.md)
- [ADR 0022](../technical/0022-install-exact-receipts-into-immutable-content-addressed-layouts.md)
- [ADR 0023](../technical/0023-generate-versioned-authoritative-replication-schemas.md)
