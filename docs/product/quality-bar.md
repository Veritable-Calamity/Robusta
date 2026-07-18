# Robusta Product Quality Bar

**Status:** Accepted baseline  
**Accepted:** 2026-07-18

This document defines what the words **done**, **supported**, **preview**, and **release-quality** mean for Robusta.

## A capability is not done when code merely exists

A platform capability is complete only when all applicable parts below exist:

1. **Supported Game SDK surface** — a game can use it without referencing engine internals.
2. **External use** — a game outside the engine repository demonstrates the capability.
3. **Creator diagnostics** — invalid use explains what failed, where, why, and how to correct it.
4. **Automated tests** — ordinary behavior, failure behavior, and boundary cases are exercised.
5. **Documentation** — a developer can learn and use it without reading engine source.
6. **Inspection or debugging support** — important runtime state can be understood during development.
7. **Packaging behavior** — client/server inclusion, identities, dependencies, and release inputs are known.
8. **Compatibility treatment** — upgrades, deprecation, saved data, and network impact are classified.
9. **Performance evidence** — when performance matters, representative workloads and budgets exist.
10. **Security and trust treatment** — when capability crosses a trust boundary, permissions and isolation are explicit.

A capability may be marked `Preview` while some of these are incomplete, but the missing pieces must be published.

## Platform release bar

A Robusta platform release is release-quality only when a developer on a clean supported machine can:

1. Install the SDK and creator tools.
2. Create a game without cloning Robusta.
3. Run an offline/local session through one supported command.
4. Run a real client and dedicated server.
5. Build distributable client and server packages.
6. Receive source-quality diagnostics for invalid code or content.
7. Upgrade through a documented compatibility path.
8. Roll back a failed game or platform update.
9. Operate a server with structured logs, health information, and graceful shutdown.
10. Diagnose common failures without reading Robusta implementation source.

## Required external evidence

Before 1.0, Robusta must have at least two separately maintained reference games:

- **Station-like multiplayer slice** — exercises migration-relevant systems, prediction, prototypes, maps, entity interaction, and entity-bound UI.
- **Contrasting game** — a game with meaningfully different rules and structure, used to expose assumptions accidentally copied from Space Station 14.

The reference games must consume published SDK artifacts like ordinary users. They may not rely on friend access, repository-relative project references, or undocumented host changes.

## Outcome measures

The project should track user-facing measures such as:

- time from installing tools to seeing a playable sample;
- time from a normal edit to visible result;
- percentage of ordinary game features requiring no engine changes;
- quality and location accuracy of diagnostics;
- package build reproducibility;
- update and rollback success;
- client/server compatibility failure clarity;
- server tick stability and resource use under stated workloads;
- migration coverage for representative Robust Toolbox features.

Internal architecture metrics may support these measures, but do not replace them.

## Labels

- **Experimental** — behavior may be incomplete or discarded; no compatibility promise.
- **Preview** — intended direction is known, but compatibility or product completeness is limited and documented.
- **Supported** — covered by published contracts, tests, diagnostics, documentation, and compatibility policy.
- **Deprecated** — supported temporarily with a documented replacement and removal window.
- **Removed** — no longer part of the supported platform; migration guidance remains available when practical.
