# ADR 0004: Distribute Games as Versioned, Isolated Application Packages

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0001, ADR 0007, ADR 0008

## The question

What is a Robusta game, and how should a player or operator install and run one?

## The promise

A published game is a verifiable, reproducible application that can coexist with other games and versions without sharing unsafe process state or corrupting their files.

## Why this matters

A game is more than a content folder. It contains code, data, maps, assets, compatibility requirements, and often saved state. Treating these as loose files makes installation, joining a server, updating, rolling back, and diagnosing mismatches fragile.

Robust Toolbox and Space Station 14 support flexible content delivery, including game code associated with servers. The Robusta prototype's later decisions move toward immutable packages, exact locks, provenance, side separation, and a launcher that starts dedicated processes instead of loading arbitrary game code into itself.

## Options considered

### Load loose content and code into one persistent runtime

This minimizes launch overhead but mixes trust, versions, writable data, and failure state across games.

### Require each game to ship an entirely unrelated engine copy

This is conceptually isolated but wastes storage and complicates security and platform updates.

### Use exact application packages with side-by-side runtimes

Each game has an exact logical environment and dedicated process. Identical files may be deduplicated internally without weakening the isolation contract.

## Decision

A Robusta game is a versioned, verifiable, installable application package.

Each release includes or resolves:

- a unique game and publisher identity;
- an exact runtime and dependency receipt;
- separate shared, client, and server material;
- compiled content and schema identities;
- file hashes, provenance, and signature information where applicable;
- license and dependency inventory;
- declared compatibility and trust information.

Installed package files are immutable. Saves, settings, logs, and caches live in separate writable locations.

A game runs in a dedicated process. The launcher, updater, credential holder, and package manager do not load arbitrary game assemblies into their own persistent process.

Installation and update are atomic. Multiple game and runtime versions may coexist. Rollback selects a prior exact receipt rather than reconstructing overwritten files.

## What we deliberately will not do

- Let an unknown server silently inject executable code into the launcher process.
- Treat the current working directory as the identity of a game installation.
- Overwrite installed package files in place during an update.
- Put user saves or configuration inside immutable package directories.
- Assume all client and server files may be shipped to both sides.

## Consequences

### Benefits

- Games and versions are isolated and reproducible.
- Compatibility can be checked before launch or connection.
- Updates and rollback are reliable.
- Launcher credentials remain separate from game code.
- Client packages can exclude server-only code and secrets.

### Costs and limitations

- Package resolution, installation, supervision, and storage management become platform responsibilities.
- Switching incompatible games may require a process restart.
- Package identity and exact-lock design must be completed early.

## How we will prove the decision works

- Install two incompatible games and run them without shared mutable state.
- Install two versions of one game side by side.
- Interrupt an update and demonstrate that the prior installation remains usable.
- Roll back to a prior receipt.
- Verify that the launcher process never loads the game assemblies it manages.
- Verify that a client package contains no server-only assemblies or resources.
