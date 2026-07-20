# ADR 0022: Install exact receipts into immutable content-addressed layouts

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Delivery and trust workstreams
- **Supersedes:** None
- **Product decisions served:** 0000, 0001, 0002, 0004, 0007, 0008, 0014
- **Related decisions:** 0017, 0018, 0021, 0023, 0024

## The question

How will manifests, exact receipts, signatures, installation layout, updates, writable data, migration, and rollback produce reproducible isolated game applications?

## The promise preserved

Players and operators install a verified exact release atomically, keep incompatible versions side by side, and return to a previous receipt without reconstructing overwritten files or exposing launcher credentials to game code.

## Why this matters

Package identity and filesystem layout become security and recovery boundaries. An in-place updater or loosely resolved dependency set would invalidate exact releases and rollback.

## Options considered

### Option A: Signed canonical receipt over content-addressed immutable objects

Resolve and sign one canonical receipt, store verified payloads by digest, assemble an immutable release view, and atomically switch a small selection pointer.

### Option B: Mutable application directory

This is familiar but interruption can produce mixed versions and rollback depends on backups or redownload.

### Option C: Container images as the only distribution form

Containers suit dedicated servers but do not by themselves cover desktop clients, user data, launcher separation, or publisher-selected origins.

## Decision

Robusta will use Option A:

1. A canonical release receipt names publisher and game identities, release identity, exact runtime and SDK, packages and dependencies, side projections, catalog and network schema identities, format versions, hashes, provenance, licenses, trust classification, and signature metadata.
2. Publisher signatures cover the canonical receipt; every payload is verified against a receipt digest before it enters the immutable object store. Signature suites and trust roots are versioned policy inputs.
3. The installer stages verified objects and a release view under a new receipt identity. A final atomic pointer or registry transaction makes the release selectable.
4. Installed release views and objects are read-only. User settings, saves, logs, caches, crash data, and server state use identity-scoped writable roots outside the installation.
5. Client, dedicated-server, and common payload sets are explicit. The launcher installs or launches only the requested side and never exposes server-only material to a client bundle.
6. The launcher chooses an installed exact runtime from the receipt and starts the game in a new process with an explicit working-data envelope. It never loads managed game assemblies.
7. Updates create another complete receipt and release view. Rollback changes selection to a retained prior receipt; it never rewrites the prior release.
8. Writable-data migrations declare source and target format identities, preflight checks, backup or copy-on-write behavior, commit points, and reverse or read-only outcomes. They never silently reinterpret data.
9. Garbage collection removes only objects unreachable from installed receipts, retained rollback points, active processes, migrations, or operator pins, and reports reclaimed and retained material.

## What we deliberately will not do

- Resolve dependency ranges differently on player machines after publication.
- Write saves, settings, logs, or caches into immutable release views.
- Trust a mutable channel name as release identity.
- Delete rollback material before retention and active-use checks succeed.

## Consequences

### Compatibility and migration

SDK, manifest, catalog, network, and durable-data coordinates remain separate in the receipt. Migration tools operate on copies or versioned envelopes and produce compatibility reports.

### Security

Verification precedes installation and execution. Publisher trust, revocation, digest agility, archive extraction limits, path traversal prevention, and downgrade policy require explicit tests. Signatures do not sandbox trusted game code.

### Operations

Install, verify, select, launch, migrate, rollback, retain, and collect are journaled with receipt identities. Disk use increases because exact versions coexist and rollback points are retained.

## How we will prove the decision works

- `SideBySideGamesAndVersions`, `InterruptedUpdatePreservesPriorRelease`, and `ExactRollback` run with failure injection at every durable transition.
- Receipt reproduction resolves identical bytes and identities from an exact lock on clean machines.
- Process and artifact audits prove launcher non-loading and client/server separation.
- Corrupt payloads, forged receipts, traversal archives, revoked keys, and incompatible formats fail before selection.
- Data migration tests restore the backup or prior envelope after every injected failure.

## Implementation notes

The repository can build a local SDK feed, but no application receipt, signature verifier, immutable installer, launcher, or migration transaction exists.

## Follow-up decisions

- Canonical manifest and receipt schemas.
- Signature suites, trust roots, transparency, and revocation policy.
- Retention and garbage-collection defaults.

## References

- [ADR 0004](../product/0004-distribute-games-as-isolated-application-packages.md)
- [ADR 0008](../product/0008-explicit-versions-migrations-and-rollback.md)
- [ADR 0014](../product/0014-define-first-release-boundary-and-delivery.md)
