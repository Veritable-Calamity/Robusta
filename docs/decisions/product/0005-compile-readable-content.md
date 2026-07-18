# ADR 0005: Compile Readable Content into a Deterministic Package-Aware Catalog

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0003, ADR 0004, ADR 0008, ADR 0010

## The question

How should developers describe objects, maps, assets, and other game data?

## The promise

Creators work with readable, source-control-friendly files, while released games run from content that has already been fully resolved, validated, and tied back to its source.

## Why this matters

Robust Toolbox's YAML prototypes are approachable and productive. Developers can compose objects from components, inherit from existing recipes, and refer to resources using readable paths. At large scale, however, inheritance, file ordering, overrides, and resource origins can become difficult to understand.

The Robusta prototype has basic prototypes and resources, while its later design calls for package-aware identity, deterministic ordering, provenance, explicit override behavior, and transactional validation.

The top-level decision should describe the desired creator and release behavior rather than prematurely commit every authoring format.

## Options considered

### Interpret loose authoring files directly at runtime

This keeps the pipeline simple but makes runtime behavior depend on file discovery, ordering, parser differences, and late validation.

### Require creators to author a compiled or binary format

This simplifies runtime loading but harms readability, source control, manual review, and migration.

### Use readable source plus a deterministic content compiler

Creators retain approachable files, while the release receives a resolved catalog with stable identities and complete diagnostics.

## Decision

Robusta content is authored in human-readable, source-control-friendly files and compiled into a deterministic, validated, package-aware content catalog.

The compiler must:

- retain source file, location, and package provenance;
- resolve references and inheritance in a documented order;
- reject duplicate or ambiguous identities;
- distinguish merge, replace, remove, and reset behavior explicitly;
- enforce declared package dependencies;
- separate client, server, and shared content;
- validate resources, localization, maps, and prototype references;
- produce normalized release output and schema fingerprints;
- allow developers to inspect the final resolved form.

Content identity includes its package context. Two packages may use the same local name without accidental collision. Altering another package's definition requires an explicit supported extension or patch rule.

YAML may remain a useful authoring format, especially for familiarity and migration, but this ADR does not make YAML itself the permanent product contract.

## What we deliberately will not do

- Treat physical installation paths as content identity.
- Let filesystem enumeration order determine game behavior.
- Allow packages to silently replace one another's definitions.
- Leave validation until an object is first used during gameplay.
- Produce diagnostics that discard the original authoring location.
- Force released games to repeatedly interpret raw source files as their authoritative database.

## Consequences

### Benefits

- Creator files remain readable and diffable.
- Released content is deterministic and faster to validate at startup.
- Package composition and provenance become understandable.
- IDE tools, runtime inspection, networking, saves, and migration can share one resolved metadata source.

### Costs and limitations

- The content compiler becomes a substantial product.
- Authoring-format compatibility and compiled-format compatibility must be managed separately.
- Some dynamic or ad-hoc content patterns may need explicit extension mechanisms.

## How we will prove the decision works

- Two clean machines produce identical normalized catalogs from the same source and lock.
- Invalid references report the original file and location.
- A developer can inspect a prototype's final components, inherited values, package source, and patches.
- Two packages can use identical local names without collision.
- An undeclared cross-package override fails before launch.
- Imported Robust Toolbox content is classified as exact, converted with warning, manual, or unsupported.
