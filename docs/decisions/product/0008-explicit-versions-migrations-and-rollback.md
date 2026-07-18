# ADR 0008: Support Exact Release Receipts, Side-by-Side Runtimes, Migration, and Rollback

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0004, ADR 0005, ADR 0006, ADR 0010

## The question

How should old games, upgrades, saved data, and client/server compatibility work?

## The promise

A released game continues to have a known working environment; upgrades are checked before use; data changes are explicit; incompatible versions can coexist; and a failed update can be reversed.

## Why this matters

A platform has several different kinds of compatibility: source code, compiled game API, package format, content data, network agreement, saves, maps, and replays. Treating them as one version number either hides important failures or forces every game to upgrade in lockstep.

Robust Toolbox uses a SemVer-like model and Space Station 14 pins an exact engine revision. The Robusta prototype's package direction proposes separate compatibility coordinates and exact resolved locks. The greenfield product should preserve that precision internally while presenting understandable results to users.

## Options considered

### One global platform version that every game must follow

This is simple to explain but couples unrelated games and makes long-term support and rollback difficult.

### Let each game choose loose version ranges at runtime

This is flexible but risks different installations resolving to different working sets and failing unpredictably.

### Record exact release receipts while allowing compatible authoring ranges

Developers may express supported ranges, but packaging resolves a precise tested set. Multiple runtime versions can coexist.

## Decision

Every published game release records an exact release receipt containing the runtime, Game SDK, dependencies, packages, hashes, compiled content identity, network schemas, and relevant data-format versions used by that release.

Robusta tracks different compatibility concerns separately internally, including:

- Game SDK and source compatibility;
- package and manifest format;
- authoring and compiled content schema;
- network connection schema;
- saved data, maps, and replay formats;
- game and add-on package compatibility.

Ordinary users receive clear outcomes rather than a wall of version numbers:

- compatible and ready;
- another runtime will be installed;
- a save migration is required;
- client and server cannot connect;
- an add-on is incompatible;
- rollback is available.

Compatible runtimes may be installed side by side. Package updates are atomic. Saved data is never silently reinterpreted under new rules; it is compatible, explicitly migrated, opened read-only where supported, or rejected with a clear explanation.

## What we deliberately will not do

- Force every installed game onto the newest runtime immediately.
- Resolve loose dependency ranges differently on each player's machine for a published release.
- Modify saves or maps destructively without an explicit migration and backup path.
- Guess that two network schemas are compatible because their code compiles.
- Make rollback depend on reconstructing files that an update overwrote.

## Consequences

### Benefits

- Released games remain reproducible.
- Unrelated games can upgrade on their own schedules.
- Compatibility failures are found before gameplay.
- Data integrity and rollback improve substantially.

### Costs and limitations

- The platform must manage several internal compatibility coordinates.
- Maintaining migrations and older supported runtimes requires policy and infrastructure.
- Package storage and lifecycle management become more complex.

## How we will prove the decision works

- Install and run two games requiring incompatible runtime versions.
- Reproduce a published package from its source and exact lock.
- Reject an incompatible client before it enters gameplay.
- Upgrade a saved world through an explicit migration with backup.
- Simulate a failed update and return to the prior release without data loss.
- Produce a human-readable compatibility report for a game upgrade.
