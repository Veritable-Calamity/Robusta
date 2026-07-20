# Next Workshop: Worlds, Objects, Time, Maps, and Persistence

**Status:** Questions 1-23 are answered by accepted ADRs 0011-0013, 0015-0016, and 0030-0038. Questions 24-26 have review-ready proposed answers in ADRs 0039-0041; none is accepted.

This question set remains at the product and layman level. It should determine what game developers and players can rely on before the team chooses an ECS layout, physics library, map encoding, or save format.

## A. What is a running world?

### 1. What does Robusta mean by a “world”? — Accepted in ADR 0011

A world is one isolated mutable simulation. It may contain several maps, and one trusted host may run several worlds.

### 2. Can one application run more than one world at once? — Accepted in ADR 0011

Yes for trusted tests, editors, previews, or multiple simulations when useful. Different or untrusted executable games still require separate processes.

### 3. What belongs to a world, and what belongs to the game as a whole? — Accepted in ADR 0012

Immutable game definitions, host and player-session state, and mutable world state have explicit, separate owners.

## B. What is a game object?

### 4. What makes something an object rather than merely data? — Accepted in ADR 0013

Entities represent independent world participants with identity and lifecycle. Ordinary tile cells, construction layers, effects, UI state, and wider world or host records use purpose-built models unless gameplay gives them an independent identity. A grid may be an entity while its cells remain compact data; the later map-and-grid decision will choose the exact cell and layering model.

### 5. How does an object begin its life? — Accepted in ADR 0015

What must a developer be able to rely on while prototype data is applied, components are added, and systems are notified?

### 6. How does an object end its life? — Accepted in ADR 0015

What happens to child objects, contained items, pending events, network references, saved references, and cleanup work when something is removed?

### 7. How should stale references fail? — Accepted in ADR 0015

When code retains a reference to an object that no longer exists, should the result be a clear error, an empty answer, a safe no-op, or depend on context?

### 8. Can an object's capabilities change while it is running? — Accepted in ADR 0015

May components be added or removed freely? When should such changes take effect, and how should systems observing the object be notified?

## C. How does time work?

### 9. What does one step of game time mean? — Accepted in ADR 0016

Should simulation advance at a fixed rate? What happens when the computer cannot keep up? Which behavior must remain deterministic?

### 10. What does pause mean? — Accepted in ADR 0016

Can a whole world, one map, one object, or one system be paused? Which timers and actions continue?

### 11. How should delayed actions and timers behave? — Accepted in ADR 0016

What happens across pause, save/load, lag, server restart, and world migration?

### 12. How should rendering time differ from game time? — Accepted in ADR 0016

What may the client smooth or animate without changing the authoritative simulation?

## D. How do space and maps work?

### 13. What is a map?

**Accepted answer:** [ADR 0030](../decisions/product/0030-define-runtime-maps-and-frame-qualified-coordinates.md)

Is it a file, a running space, a scene, a grid, a collection of objects, or a reusable template? Can a world contain several maps?

### 14. How should positions be understood?

**Accepted answer:** [ADR 0030](../decisions/product/0030-define-runtime-maps-and-frame-qualified-coordinates.md)

What is the relationship between local position, parent-relative position, grid position, map position, and screen position?

### 15. Can objects contain other objects?

**Accepted answer:** [ADR 0031](../decisions/product/0031-separate-spatial-containment-attachment-and-lifecycle-relations.md)

How should inventories, lockers, vehicles, hands, rooms, and nested containers affect position, visibility, physics, networking, and deletion?

### 16. How should moving between maps or worlds appear?

**Accepted answer:** [ADR 0032](../decisions/product/0032-reconstruct-explicitly-across-world-transfers.md)

Should transfer preserve object identity? What happens to network ownership, timers, references, and saved state?

## E. What foundations belong to Robusta?

### 17. Which concepts must every game receive from the platform?

**Accepted answer:** [ADR 0033](../decisions/product/0033-provide-platform-mechanics-with-game-defined-semantics.md)

Candidates include identity, metadata, transform, parent/child relationships, maps, grids, containers, spatial search, physics bodies, ownership, and appearance.

### 18. Which concepts should remain game-defined?

**Accepted answer:** [ADR 0033](../decisions/product/0033-provide-platform-mechanics-with-game-defined-semantics.md)

Examples include health, inventory rules, factions, combat, jobs, quests, rounds, atmosphere, power, and game modes.

Any optional batteries-included station features supplied by the platform remain ordinary, separately versioned game or component packages. They use the same published SDK and declared trust mechanisms available to independent developers, receive no privileged internals, and must comply with every accepted ADR.

### 19. How should advanced games extend foundations without bypassing the SDK?

**Accepted answer:** [ADR 0034](../decisions/product/0034-use-a-declared-ladder-for-advanced-game-extensions.md)

When should the answer be a normal game system, a supported advanced extension point, a platform contribution, or an unsupported use case?

## F. Saving, loading, and change

### 20. What does saving a world promise?

**Accepted answer:** [ADR 0035](../decisions/product/0035-persist-declared-world-state-through-versioned-checkpoints.md)

Does a save capture everything, only durable game state, or a game-selected subset? What must remain stable across platform upgrades?

### 21. How are references represented in saved data?

**Accepted answer:** [ADR 0036](../decisions/product/0036-use-explicit-durable-identities-and-reference-policies.md)

What happens when an object, prototype, package, or component no longer exists in the new release?

### 22. What happens when prototypes change while objects already exist?

**Accepted answer:** [ADR 0037](../decisions/product/0037-keep-live-state-stable-unless-explicitly-migrated.md)

Do existing objects keep old values, receive safe updates, restart, or require an explicit migration?

### 23. What should map editing and preview feel like?

**Accepted answer:** [ADR 0038](../decisions/product/0038-edit-map-sources-and-preview-in-isolated-worlds.md)

Can creators place objects, inspect resolved prototypes, save maps, and run a local preview without a separate privileged engine path?

ADR 0038 also permits a server host to run collaborative mapping sessions for authenticated creators. Live or in-world editing remains a presentation over canonical source-document transactions and history; it does not make arbitrary gameplay-world state the authored map source.

## G. Testing and inspection

### 24. How should a developer inspect a running world?

**Proposed answer:** [ADR 0039](../decisions/product/0039-inspect-running-worlds-through-authorized-snapshots.md), Option A recommended and not accepted.

What information should be visible about an object's components, prototype origin, parent, container, position, network state, and lifecycle?

### 25. How easy should isolated world tests be?

**Proposed answer:** [ADR 0040](../decisions/product/0040-test-isolated-worlds-through-the-supported-runtime.md), Option A recommended and not accepted.

Can a test create a small world with fake time, known prototypes, and no global state? Can several tests or previews run without contaminating one another?

### 26. How should replay and determinism fit the world model?

**Proposed answer:** [ADR 0041](../decisions/product/0041-record-versioned-authoritative-replays-with-declared-determinism.md), Option A recommended and not accepted.

What should be recordable, reproducible, or comparable when diagnosing a bug or validating a migration?

## Recommended discussion order

Questions 1-23 are settled. Review proposed ADRs 0039, 0040, and 0041 in that order for runtime inspection, isolated tests, replay, and the bounded determinism promise. Acceptance still requires an explicit decision for each ADR.
