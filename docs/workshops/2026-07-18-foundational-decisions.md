# Foundational Product Design Workshop

- **Date:** 2026-07-18
- **Outcome:** All recommended decision statements accepted
- **Scope:** Product identity, audience, authoring, packages, content, multiplayer, trust, compatibility, creator workflow, release quality, and migration
- **Implementation claim:** None

## Purpose

Establish the top-level answers that will guide the greenfield, release-grade Robusta platform before technical mechanisms are selected.

## Accepted conclusions

1. Robusta is a complete game platform, not merely an engine library.
2. Independent game teams are the primary creators.
3. Player and operator safety are non-negotiable.
4. Superiority is demonstrated through user outcomes, not architectural labels.
5. Components, systems, events, prototypes, interfaces, and assets remain the normal game-authoring model.
6. Ordinary games do not require Robusta source changes, host edits, internal references, or manual registration.
7. Games use a small supported Game SDK and explicit extension points.
8. A game is a versioned, verifiable, installable application package.
9. Human-readable content is compiled into a deterministic, package-aware catalog.
10. The server is authoritative; clients may make controlled predictions.
11. Game developers declare synchronization intent while the platform supplies routine networking.
12. Executable game packages, operator extensions, public UGC, and future isolated scripts are distinct trust tiers.
13. Signatures and validation establish identity and policy compliance, not confinement.
14. Exact working sets are recorded; compatible runtimes can coexist; migrations and rollback are supported.
15. Robusta provides one supported development workflow with explicit outcomes for every edit.
16. A capability is complete only when an external game can use it with tests, diagnostics, documentation, tooling, packaging, and an upgrade story.
17. Migration from Robust Toolbox targets concepts, content, and ordinary source through dedicated tools rather than binary compatibility.

## ADRs created from this workshop

- ADR 0000 — Adopt the Robusta Platform Constitution.
- ADR 0001 — Build a complete game platform for independent teams.
- ADR 0002 — Judge quality through user outcomes and external use.
- ADR 0003 — Preserve straightforward game authoring behind a supported SDK.
- ADR 0004 — Distribute games as versioned, isolated application packages.
- ADR 0005 — Compile readable content into a deterministic package-aware catalog.
- ADR 0006 — Use server authority with declarative synchronization intent.
- ADR 0007 — Separate trusted executable games from public UGC.
- ADR 0008 — Support exact release receipts, side-by-side runtimes, migration, and rollback.
- ADR 0009 — Provide one supported creator development workflow.
- ADR 0010 — Target assisted Robust Toolbox migration rather than binary compatibility.

## Important distinction

These decisions are accepted product direction. They do not state that the current Robusta prototype or the future greenfield implementation already satisfies them.

## Next workshop

The next question group covers the **world model**:

- what a world is;
- what a game object is;
- how objects begin and end;
- how time, pause, maps, coordinates, and containment work;
- what belongs to the platform versus the game;
- how saving, loading, inspection, testing, and definition changes should appear to creators.

See `world-model-question-set.md`.
